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

  questionCountOptions: number[] = [20, 30, 40, 60];

  private _userId = '';

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

      this._activeClassService.schoolClassIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(classId => this.selectedClassId = classId),
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

  get isFormValid(): boolean {
    return this.quizName.trim().length > 0
      && this.selectedCourseId.length > 0
      && this.selectedClassId.length > 0
      && this.selectedNodeIds.length > 0
      && this.questionCount >= 20
      && this.questionCount <= 60;
  }

  async onSubmit(): Promise<void> {
    if (!this.isFormValid || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;

    try {
      const command = new CreateTeacherCustomQuizCommand(
        this.quizName.trim(),
        this.selectedCourseId,
        this.selectedNodeIds,
        this.questionCount,
        this.selectedClassId,
        this.saveAsTemplate,
        this._userId,
      );

      const result = await firstValueFrom(this._api.send(command));
      this._snackBarService.showMessage('Quiz created and assigned successfully!');

      // Reset form
      this.quizName = '';
      this.checklistSelection.clear();
      this.saveAsTemplate = false;
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
