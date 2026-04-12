import { Component, OnInit } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material/tree';
import { Observable } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetContentTreeListQuery, GetContentTreeListQuery_Result } from '../../../api/content/get-content-tree-list.query';
import { TreeMode } from '../../../api/enums/tree-mode.enum';
import {
  CreateTeacherCustomQuizCommand,
  CreateTeacherCustomQuizCommand_Result,
} from '../../../api/quizzes/create-teacher-custom-quiz.command';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { ActiveSchoolService } from '../../../core/services/active-school.service';
import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';
import { TeacherCoursesService } from '../../../core/services/courses/teacher-courses.service';
import { StudentsService } from '../../../core/services/students/students.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { SnackBarService } from '../../../share/services/snack-bar.service';
import { StringsService } from '../../../share/strings.service';

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
  selector: 'app-teacher-custom-quiz',
  templateUrl: './teacher-custom-quiz.component.html',
  styleUrls: ['./teacher-custom-quiz.component.scss'],
})
export class TeacherCustomQuizComponent extends BaseComponent implements OnInit {
  courseGroups$: Observable<TeacherCourseGroup[]>;

  selectedCourseId = '';
  selectedClassId = '';
  quizName = '';
  questionCount = 20;
  saveAsTemplate = false;
  isSubmitting = false;
  dueDate: Date;
  minDueDate = new Date();

  questionCountOptions: number[] = [20, 30, 40, 60];

  students: StudentModel[] = [];
  isLoadingStudents = false;

  private _userId = '';
  private _schoolId = '';

  // Tree
  treeControl: FlatTreeControl<ContentTreeFlatNode>;
  treeFlattener: MatTreeFlattener<GetContentTreeListQuery_Result, ContentTreeFlatNode>;
  dataSource: MatTreeFlatDataSource<GetContentTreeListQuery_Result, ContentTreeFlatNode>;
  checklistSelection = new SelectionModel<ContentTreeFlatNode>(true);

  private _flatNodeMap = new Map<string, ContentTreeFlatNode>();
  private _nestedNodeMap = new Map<string, GetContentTreeListQuery_Result>();

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeClassService: ActiveSchoolClassService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _studentsService: StudentsService,
    private readonly _snackBarService: SnackBarService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    const defaultDue = new Date();
    defaultDue.setDate(defaultDue.getDate() + 7);
    this.dueDate = defaultDue;

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
          }
        }),
      ).subscribe(),

      this._activeClassService.schoolClassIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(classId => {
          this.selectedClassId = classId;
          this._loadStudentsForClass(classId);
        }),
      ).subscribe(),

      // Auto-select course when teacher has only one course
      this.courseGroups$.pipe(
        filter(groups => groups.length > 0),
        tap((groups: TeacherCourseGroup[]) => {
          const courseIds: string[] = [];
          groups.forEach(g => g.courses.forEach(c => {
            if (!courseIds.includes(c.courseId)) courseIds.push(c.courseId);
          }));
          if (courseIds.length === 1 && !this.selectedCourseId) {
            this.onCourseChange(courseIds[0]);
          }
        }),
      ).subscribe(),
    );
  }

  hasChild = (_: number, node: ContentTreeFlatNode): boolean => node.expandable;

  onCourseChange(courseId: string): void {
    this.selectedCourseId = courseId;
    this.checklistSelection.clear();
    this._loadContentTree(courseId);
  }

  onClassChange(classId: string): void {
    this.selectedClassId = classId;
    this.students = [];
    if (classId) {
      this._loadStudentsForClass(classId);
    }
  }

  /** Toggle a leaf node */
  toggleLeafNode(node: ContentTreeFlatNode): void {
    this.checklistSelection.toggle(node);
    this._checkAllParentsSelection(node);
  }

  /** Toggle a parent node: select/deselect all descendants */
  toggleParentNode(node: ContentTreeFlatNode): void {
    this.checklistSelection.toggle(node);
    const descendants = this.treeControl.getDescendants(node);
    if (this.checklistSelection.isSelected(node)) {
      this.checklistSelection.select(...descendants);
    } else {
      this.checklistSelection.deselect(...descendants);
    }
    this._checkAllParentsSelection(node);
  }

  /** Whether all descendants of node are selected */
  descendantsAllSelected(node: ContentTreeFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    if (descendants.length === 0) {
      return this.checklistSelection.isSelected(node);
    }
    return descendants.every(child => this.checklistSelection.isSelected(child));
  }

  /** Whether part of the descendants are selected */
  descendantsPartiallySelected(node: ContentTreeFlatNode): boolean {
    const descendants = this.treeControl.getDescendants(node);
    const someSelected = descendants.some(child => this.checklistSelection.isSelected(child));
    return someSelected && !this.descendantsAllSelected(node);
  }

  get selectedNodeIds(): string[] {
    return this.checklistSelection.selected.map(n => n.entityId);
  }

  get selectedStudentIds(): string[] {
    return this.students.filter(s => s.checked).map(s => s.studentId);
  }

  get isFormValid(): boolean {
    return this.quizName.trim().length > 0
      && this.selectedCourseId.length > 0
      && this.selectedClassId.length > 0
      && this.selectedNodeIds.length > 0
      && this.questionCount >= 20
      && this.questionCount <= 60
      && this.selectedStudentIds.length > 0;
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

  async onSubmit(): Promise<void> {
    if (this.isSubmitting) {
      return;
    }

    // Validate with specific snackbar messages
    if (!this.quizName.trim()) {
      this._snackBarService.showMessage('Please enter a quiz name.');
      return;
    }
    if (!this.selectedCourseId) {
      this._snackBarService.showMessage('Please select a course.');
      return;
    }
    if (!this.selectedClassId) {
      this._snackBarService.showMessage('Please select a class to assign the quiz to.');
      return;
    }
    if (this.selectedNodeIds.length === 0) {
      this._snackBarService.showMessage('Please select at least one topic.');
      return;
    }
    if (this.selectedStudentIds.length === 0) {
      this._snackBarService.showMessage('Please select at least one student.');
      return;
    }

    this.isSubmitting = true;

    try {
      const dueDateUtc = this.dueDate ? this.dueDate.toISOString() : null;
      const dueDateLocal = this.dueDate ? this.dueDate.toLocaleDateString('sv-SE') + 'T23:59:59' : null;

      const command = new CreateTeacherCustomQuizCommand(
        this.quizName.trim(),
        this.selectedCourseId,
        this.selectedNodeIds,
        this.questionCount,
        this.selectedClassId,
        this.saveAsTemplate,
        this._userId,
        this.selectedStudentIds,
        dueDateUtc,
        dueDateLocal,
      );

      const result = await firstValueFrom(this._api.send(command));
      this._snackBarService.showMessage('Quiz created and assigned successfully!');

      // Reset form
      this.quizName = '';
      this.checklistSelection.clear();
      this.saveAsTemplate = false;
      const resetDue = new Date();
      resetDue.setDate(resetDue.getDate() + 7);
      this.dueDate = resetDue;
      this.students = this.students.map(s => ({ ...s, checked: true }));
    } catch (err) {
      this.handleError(err);
      this._snackBarService.showMessage('Failed to create quiz. Please try again.');
    } finally {
      this.isSubmitting = false;
    }
  }

  private async _loadStudentsForClass(classId: string): Promise<void> {
    try {
      this.isLoadingStudents = true;
      const schoolId = this._schoolId || (await this._activeSchoolService.schoolId) || '';
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
    } catch (err) {
      this.handleError(err);
      this.students = [];
    } finally {
      this.isLoadingStudents = false;
    }
  }

  private async _loadContentTree(courseId: string): Promise<void> {
    try {
      this.startLoading();
      this._flatNodeMap.clear();
      this._nestedNodeMap.clear();

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
    this._nestedNodeMap.set(node.entityId, node);

    return flatNode;
  }

  private _checkAllParentsSelection(node: ContentTreeFlatNode): void {
    let parent = this._getParentNode(node);
    while (parent) {
      this._checkRootNodeSelection(parent);
      parent = this._getParentNode(parent);
    }
  }

  private _checkRootNodeSelection(node: ContentTreeFlatNode): void {
    const nodeSelected = this.checklistSelection.isSelected(node);
    const descendants = this.treeControl.getDescendants(node);
    const allSelected = descendants.every(child => this.checklistSelection.isSelected(child));

    if (nodeSelected && !allSelected) {
      this.checklistSelection.deselect(node);
    } else if (!nodeSelected && allSelected && descendants.length > 0) {
      this.checklistSelection.select(node);
    }
  }

  private _getParentNode(node: ContentTreeFlatNode): ContentTreeFlatNode | null {
    const currentLevel = node.level;
    if (currentLevel < 1) {
      return null;
    }

    const startIndex = this.treeControl.dataNodes.indexOf(node) - 1;
    for (let i = startIndex; i >= 0; i--) {
      const currentNode = this.treeControl.dataNodes[i];
      if (currentNode.level < currentLevel) {
        return currentNode;
      }
    }

    return null;
  }
}
