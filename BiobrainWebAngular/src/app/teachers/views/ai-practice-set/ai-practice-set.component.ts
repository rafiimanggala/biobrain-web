import { Component, OnInit } from '@angular/core';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { Observable } from 'rxjs';
import { distinctUntilChanged, filter, switchMap, tap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import {
  GeneratePracticeSetCommand,
  GeneratePracticeSetCommand_Result,
} from '../../../api/ai/generate-practice-set.command';
import {
  GetQuestionsByIdsCommand,
  GetQuestionsByIdsCommand_Result,
} from '../../../api/ai/get-questions-by-ids.command';
import {
  GetContentTreeListQuery,
  GetContentTreeListQuery_Result,
} from '../../../api/content/get-content-tree-list.query';
import { Question } from '../../../api/content/content-data-models';
import { TreeMode } from '../../../api/enums/tree-mode.enum';
import {
  CreateTeacherCustomQuizCommand,
} from '../../../api/quizzes/create-teacher-custom-quiz.command';
import {
  GetTeacherSchoolClassesByCourseIdQuery,
  GetSchoolClassByCourseIdQuery_Result,
} from '../../../api/school-classes/get-teacher-school-classes-by-course-id.query';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolService } from '../../../core/services/active-school.service';
import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';
import { TeacherCoursesService } from '../../../core/services/courses/teacher-courses.service';
import { StudentsService } from '../../../core/services/students/students.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { SnackBarService } from '../../../share/services/snack-bar.service';
import { StringsService } from '../../../share/strings.service';
import { ThemeService } from '../../../core/app/theme.service';

interface ContentTreeFlatNode {
  expandable: boolean;
  header: string;
  level: number;
  entityId: string;
}

interface StudentModel {
  studentId: string;
  fullName: string;
  checked: boolean;
}

@Component({
  selector: 'app-ai-practice-set',
  templateUrl: './ai-practice-set.component.html',
  styleUrls: ['./ai-practice-set.component.scss'],
})
export class AiPracticeSetComponent extends BaseComponent implements OnInit {
  courseGroups$: Observable<TeacherCourseGroup[]>;

  selectedCourseId = '';
  selectedNodeId = '';
  quizName = '';
  questionType = 'MultipleChoice';
  questionCount = 5;
  difficultyLevel = 'Medium';
  isSubmitting = false;
  generatedCount: number | null = null;
  generatedQuestionIds: string[] = [];
  generatedQuestions: Question[] = [];
  isLoadingQuestions = false;
  showReview = false;
  themeColor = '';

  // Assign functionality
  schoolClasses: GetSchoolClassByCourseIdQuery_Result[] = [];
  selectedClassId = '';
  students: StudentModel[] = [];
  isLoadingStudents = false;
  isAssigning = false;
  dueDate: Date;
  minDueDate = new Date();
  showAssignPanel = false;

  questionTypes: { value: string; label: string }[] = [
    { value: 'MultipleChoice', label: 'Multiple Choice' },
    { value: 'TrueFalse', label: 'True / False' },
    { value: 'FreeText', label: 'Free Text' },
  ];

  difficultyLevels: { value: string; label: string }[] = [
    { value: 'Easy', label: 'Easy' },
    { value: 'Medium', label: 'Medium' },
    { value: 'Hard', label: 'Hard' },
  ];

  questionCountOptions: number[] = [1, 2, 3, 5, 10, 15, 20];

  private _userId = '';
  private _schoolId = '';

  // Tree
  treeControl: FlatTreeControl<ContentTreeFlatNode>;
  treeFlattener: MatTreeFlattener<GetContentTreeListQuery_Result, ContentTreeFlatNode>;
  dataSource: MatTreeFlatDataSource<GetContentTreeListQuery_Result, ContentTreeFlatNode>;

  private _flatNodeMap = new Map<string, ContentTreeFlatNode>();

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _studentsService: StudentsService,
    private readonly _snackBarService: SnackBarService,
    private readonly _themeService: ThemeService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    const defaultDue = new Date();
    defaultDue.setDate(defaultDue.getDate() + 7);
    this.dueDate = defaultDue;

    this._themeService.colors$.subscribe(colors => {
      this.themeColor = colors.primary;
    });

    this.treeControl = new FlatTreeControl<ContentTreeFlatNode>(
      node => node.level,
      node => node.expandable,
    );

    this.treeFlattener = new MatTreeFlattener(
      this._transformer.bind(this),
      node => node.level,
      node => node.expandable,
      node => node.children,
    );

    this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    this.courseGroups$ = this._currentUserService.userChanges$.pipe(
      filter(hasValue),
      switchMap(user => this._teacherCoursesService.getTeacherCourses(user.userId)),
    );
  }

  ngOnInit(): void {
    this.pushSubscribtions(
      this._currentUserService.userChanges$.pipe(
        filter(hasValue),
        tap(user => this._userId = user.userId),
      ).subscribe(),

      this._activeSchoolService.schoolIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(schoolId => this._schoolId = schoolId),
      ).subscribe(),

      this._activeCourseService.courseIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(courseId => {
          if (this.selectedCourseId !== courseId) {
            this.selectedCourseId = courseId;
            this._loadContentTree(courseId);
            this._loadSchoolClasses(courseId);
          }
        }),
      ).subscribe(),
    );
  }

  hasChild = (_: number, node: ContentTreeFlatNode): boolean => node.expandable;

  onCourseChange(courseId: string): void {
    this.selectedCourseId = courseId;
    this.selectedNodeId = '';
    this.generatedCount = null;
    this.generatedQuestions = [];
    this.selectedClassId = '';
    this.students = [];
    this._loadContentTree(courseId);
    this._loadSchoolClasses(courseId);
  }

  onNodeSelect(nodeId: string): void {
    this.selectedNodeId = nodeId;
    this.generatedCount = null;
    this.generatedQuestions = [];
  }

  onClassChange(classId: string): void {
    this.selectedClassId = classId;
    this.students = [];
    if (classId) {
      this._loadStudentsForClass(classId);
    }
  }

  get isFormValid(): boolean {
    return this.selectedCourseId.length > 0
      && this.selectedNodeId.length > 0
      && this.questionCount >= 1
      && this.questionCount <= 20;
  }

  get isAssignFormValid(): boolean {
    return this.generatedQuestionIds.length > 0
      && this.selectedClassId.length > 0
      && this.selectedStudentIds.length > 0
      && this.quizName.trim().length > 0;
  }

  get selectedStudentIds(): string[] {
    return this.students.filter(s => s.checked).map(s => s.studentId);
  }

  getAnswerLabel(index: number): string {
    return String.fromCharCode(65 + index); // A, B, C, D...
  }

  selectAllStudents(): void {
    this.students = this.students.map(s => ({ ...s, checked: true }));
  }

  unselectAllStudents(): void {
    this.students = this.students.map(s => ({ ...s, checked: false }));
  }

  toggleStudent(student: StudentModel): void {
    student.checked = !student.checked;
  }

  async onGenerate(): Promise<void> {
    if (!this.isFormValid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    this.generatedCount = null;
    this.generatedQuestions = [];
    this.showAssignPanel = false;

    try {
      const command = new GeneratePracticeSetCommand(
        this.selectedCourseId,
        this.selectedNodeId,
        this.questionCount,
        this.questionType,
        this._userId,
        this.difficultyLevel,
      );

      const result: GeneratePracticeSetCommand_Result = await firstValueFrom(
        this._api.send(command)
      );

      this.generatedQuestionIds = result.questionIds;
      this.generatedCount = result.questionIds.length;
      this.showReview = false;
      this._snackBarService.showMessage(
        `Successfully generated ${this.generatedCount} question(s)!`
      );

      // Fetch full question details
      await this._fetchQuestionDetails(result.questionIds);
    } catch (err) {
      this.handleError(err);
    } finally {
      this.isSubmitting = false;
    }
  }

  toggleReview(): void {
    this.showReview = !this.showReview;
  }

  toggleAssignPanel(): void {
    this.showAssignPanel = !this.showAssignPanel;
  }

  async onAssign(): Promise<void> {
    if (!this.isAssignFormValid || this.isAssigning) {
      return;
    }

    if (!this.quizName.trim()) {
      this._snackBarService.showMessage('Please enter a quiz name.');
      return;
    }
    if (!this.selectedClassId) {
      this._snackBarService.showMessage('Please select a class.');
      return;
    }
    if (this.selectedStudentIds.length === 0) {
      this._snackBarService.showMessage('Please select at least one student.');
      return;
    }

    this.isAssigning = true;

    try {
      const dueDateUtc = this.dueDate ? this.dueDate.toISOString() : null;
      const dueDateLocal = this.dueDate
        ? this.dueDate.toLocaleDateString('sv-SE') + 'T23:59:59'
        : null;

      const command = new CreateTeacherCustomQuizCommand(
        this.quizName.trim(),
        this.selectedCourseId,
        [this.selectedNodeId],
        this.generatedQuestionIds.length,
        this.selectedClassId,
        false,
        this._userId,
        this.selectedStudentIds,
        dueDateUtc,
        dueDateLocal,
      );

      await firstValueFrom(this._api.send(command));
      this._snackBarService.showMessage('Quiz assigned to class successfully!');
      this.showAssignPanel = false;
    } catch (err) {
      this.handleError(err);
      this._snackBarService.showMessage('Failed to assign quiz. Please try again.');
    } finally {
      this.isAssigning = false;
    }
  }

  private async _fetchQuestionDetails(questionIds: string[]): Promise<void> {
    if (questionIds.length === 0) {
      return;
    }

    this.isLoadingQuestions = true;

    try {
      const command = new GetQuestionsByIdsCommand(questionIds);
      const result: GetQuestionsByIdsCommand_Result = await firstValueFrom(
        this._api.send(command)
      );
      this.generatedQuestions = result.questions;
    } catch {
      // Fallback: if the endpoint doesn't exist yet, keep showing IDs only
      this.generatedQuestions = [];
    } finally {
      this.isLoadingQuestions = false;
    }
  }

  private async _loadSchoolClasses(courseId: string): Promise<void> {
    try {
      const schoolId = this._schoolId
        || (await this._activeSchoolService.schoolId)
        || '';
      if (!schoolId) {
        this.schoolClasses = [];
        return;
      }
      const query = new GetTeacherSchoolClassesByCourseIdQuery(courseId, schoolId);
      this.schoolClasses = await firstValueFrom(this._api.send(query));
    } catch {
      this.schoolClasses = [];
    }
  }

  private async _loadStudentsForClass(classId: string): Promise<void> {
    try {
      this.isLoadingStudents = true;
      const schoolId = this._schoolId
        || (await this._activeSchoolService.schoolId)
        || '';
      if (!schoolId) {
        this.students = [];
        return;
      }
      const students = await firstValueFrom(
        this._studentsService.getForClassFromCache(schoolId, classId),
      );
      this.students = students.map(s => ({
        studentId: s.studentId,
        fullName: s.fullName,
        checked: true,
      }));
    } catch {
      this.students = [];
    } finally {
      this.isLoadingStudents = false;
    }
  }

  private async _loadContentTree(courseId: string): Promise<void> {
    try {
      this.startLoading();
      this._flatNodeMap.clear();

      const query = new GetContentTreeListQuery(courseId, TreeMode.Topics);
      const tree = await firstValueFrom(this._api.send(query));
      this.dataSource.data = tree;
    } catch (err) {
      this.handleError(err);
    } finally {
      this.endLoading();
    }
  }

  private _transformer(node: GetContentTreeListQuery_Result, level: number): ContentTreeFlatNode {
    const existingNode = this._flatNodeMap.get(node.entityId);
    const flatNode: ContentTreeFlatNode = existingNode && existingNode.header === node.header
      ? existingNode
      : { expandable: false, header: '', level: 0, entityId: '' };

    flatNode.header = node.header;
    flatNode.level = level;
    flatNode.expandable = node.children.length > 0;
    flatNode.entityId = node.entityId;

    this._flatNodeMap.set(node.entityId, flatNode);
    return flatNode;
  }
}
