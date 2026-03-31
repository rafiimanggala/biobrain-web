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
  GetContentTreeListQuery,
  GetContentTreeListQuery_Result,
} from '../../../api/content/get-content-tree-list.query';
import { TreeMode } from '../../../api/enums/tree-mode.enum';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';
import { TeacherCoursesService } from '../../../core/services/courses/teacher-courses.service';
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

@Component({
  selector: 'app-ai-practice-set',
  templateUrl: './ai-practice-set.component.html',
  styleUrls: ['./ai-practice-set.component.scss'],
})
export class AiPracticeSetComponent extends BaseComponent implements OnInit {
  courseGroups$: Observable<TeacherCourseGroup[]>;

  selectedCourseId = '';
  selectedNodeId = '';
  questionType = 'MultipleChoice';
  questionCount = 5;
  isSubmitting = false;
  generatedCount: number | null = null;

  questionTypes: { value: string; label: string }[] = [
    { value: 'MultipleChoice', label: 'Multiple Choice' },
    { value: 'TrueFalse', label: 'True / False' },
    { value: 'FreeText', label: 'Free Text' },
  ];

  questionCountOptions: number[] = [1, 2, 3, 5, 10, 15, 20];

  private _userId = '';

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
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _snackBarService: SnackBarService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

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
    );
  }

  hasChild = (_: number, node: ContentTreeFlatNode): boolean => node.expandable;

  onCourseChange(courseId: string): void {
    this.selectedCourseId = courseId;
    this.selectedNodeId = '';
    this.generatedCount = null;
    this._loadContentTree(courseId);
  }

  onNodeSelect(nodeId: string): void {
    this.selectedNodeId = nodeId;
    this.generatedCount = null;
  }

  get isFormValid(): boolean {
    return this.selectedCourseId.length > 0
      && this.selectedNodeId.length > 0
      && this.questionCount >= 1
      && this.questionCount <= 20;
  }

  async onGenerate(): Promise<void> {
    if (!this.isFormValid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    this.generatedCount = null;

    try {
      const command = new GeneratePracticeSetCommand(
        this.selectedCourseId,
        this.selectedNodeId,
        this.questionCount,
        this.questionType,
        this._userId,
      );

      const result: GeneratePracticeSetCommand_Result = await firstValueFrom(
        this._api.send(command)
      );

      this.generatedCount = result.questionIds.length;
      this._snackBarService.showMessage(
        `Successfully generated ${this.generatedCount} question(s)!`
      );
    } catch (err) {
      this.handleError(err);
    } finally {
      this.isSubmitting = false;
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
