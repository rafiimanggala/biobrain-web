import { Component } from '@angular/core';
import { of } from 'rxjs';
import { filter, take, switchMap } from 'rxjs/operators';

import { Api } from 'src/app/api/api.service';
import { CreateStudentCustomQuizCommand } from 'src/app/api/quiz-assignments/create-student-custom-quiz.command';
import {
  GetStudentCustomQuizzesQuery,
  GetStudentCustomQuizzesQuery_Item,
} from 'src/app/api/quiz-assignments/get-student-custom-quizzes.query';
import { RetakeStudentCustomQuizCommand } from 'src/app/api/quiz-assignments/retake-student-custom-quiz.command';
import { EnsureQuizResultForAssignmentCommand } from 'src/app/api/quiz-results/ensure-quiz-result-for-assignment.command';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { QuizzesService } from 'src/app/core/services/quizzes/quizzes.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { hasValue } from 'src/app/share/helpers/has-value';
import { StringsService } from 'src/app/share/strings.service';

interface TreeNodeItem {
  node: ContentTreeNode;
  level: number;
  checked: boolean;
  indeterminate: boolean;
  children: TreeNodeItem[];
  parent: TreeNodeItem | null;
}

@Component({
  selector: 'app-custom-quiz',
  templateUrl: './custom-quiz.component.html',
  styleUrls: ['./custom-quiz.component.scss'],
})
export class CustomQuizComponent extends BaseComponent {
  public selectedCourseId: string | null = null;
  public quizName = 'self-created';
  public questionCount = 20;
  public readonly questionCountOptions = [20, 30, 40, 60];
  public treeItems: TreeNodeItem[] = [];
  public isCreating = false;
  public lastCreatedQuizName: string | null = null;
  public myQuizzes: GetStudentCustomQuizzesQuery_Item[] = [];
  public retakingQuizId: string | null = null;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    private readonly _quizzesService: QuizzesService,
    private readonly _activeCourseService: ActiveCourseService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);

    this.pushSubscribtions(
      this._activeCourseService.courseChanges$.pipe(
        filter(hasValue),
        take(1)
      ).subscribe(course => {
        this.onCourseSelected(course.courseId);
      })
    );
  }

  public onCourseSelected(courseId: string): void {
    this.selectedCourseId = courseId;
    this.treeItems = [];
    this._loadContentTree(courseId);
    void this._loadMyQuizzes(courseId);
  }

  public onNodeChecked(item: TreeNodeItem, checked: boolean): void {
    this._setChecked(item, checked);
    this._updateParentState(item.parent);
  }

  public get selectedNodeIds(): string[] {
    return this._collectSelectedLeafIds(this.treeItems);
  }

  public get canCreate(): boolean {
    return this.quizName.trim().length > 0
      && hasValue(this.selectedCourseId)
      && this.selectedNodeIds.length > 0
      && this.questionCount > 0
      && !this.isCreating;
  }

  public async onCreate(): Promise<void> {
    if (this.selectedNodeIds.length === 0) {
      this.error('Please select at least one topic');
      return;
    }

    if (!this.canCreate) {
      return;
    }

    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      this.error('User not found');
      return;
    }

    this.isCreating = true;
    try {
      const trimmedName = this.quizName.trim();
      const createResult = await firstValueFrom(
        this._api.send(new CreateStudentCustomQuizCommand(
          trimmedName,
          this.selectedCourseId!,
          this.selectedNodeIds,
          this.questionCount,
          user.userId
        ))
      );

      const ensureResult = await firstValueFrom(
        this._api.send(new EnsureQuizResultForAssignmentCommand(
          user.userId,
          createResult.quizStudentAssignmentId
        ))
      );

      this.lastCreatedQuizName = trimmedName;
      this._activeCourseService.setActiveCourseId(this.selectedCourseId!);
      await this._quizzesService.reloadAndWait();
      void this._loadMyQuizzes(this.selectedCourseId!);
      this.isCreating = false;
      await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
    } catch (err) {
      this.handleError(err);
      this.isCreating = false;
    }
  }

  public async onRedo(): Promise<void> {
    await this.onCreate();
  }

  public async onRetakeQuiz(quiz: GetStudentCustomQuizzesQuery_Item): Promise<void> {
    if (this.retakingQuizId) return;
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      this.error('User not found');
      return;
    }

    this.retakingQuizId = quiz.quizId;
    try {
      const retakeResult = await firstValueFrom(
        this._api.send(new RetakeStudentCustomQuizCommand(quiz.quizId, user.userId))
      );

      const ensureResult = await firstValueFrom(
        this._api.send(new EnsureQuizResultForAssignmentCommand(
          user.userId,
          retakeResult.quizStudentAssignmentId
        ))
      );

      this._activeCourseService.setActiveCourseId(this.selectedCourseId!);
      await this._quizzesService.reloadAndWait();
      this.retakingQuizId = null;
      await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
    } catch (err) {
      this.handleError(err);
      this.retakingQuizId = null;
    }
  }

  private async _loadMyQuizzes(courseId: string): Promise<void> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      this.myQuizzes = [];
      return;
    }
    try {
      const result = await firstValueFrom(
        this._api.send(new GetStudentCustomQuizzesQuery(user.userId, courseId))
      );
      this.myQuizzes = result.quizzes ?? [];
    } catch {
      this.myQuizzes = [];
    }
  }

  private _loadContentTree(courseId: string): void {
    this.startLoading();
    const sub = this._contentTreeService.getMeta(courseId).pipe(
      switchMap(meta => {
        if (!hasValue(meta) || meta.length === 0) {
          return of([]);
        }
        const topMeta = meta.sort((a, b) => a.dbRow.depth - b.dbRow.depth)[0];
        return this._contentTreeService.getMetaValuesWithParent(
          topMeta.dbRow.contentTreeMetaId, null
        );
      })
    ).subscribe(
      nodes => {
        this.treeItems = hasValue(nodes)
          ? nodes.map(n => this._buildTreeItem(n, 0, null))
          : [];
        this.endLoading();
      },
      err => {
        this.handleError(err);
        this.endLoading();
      }
    );
    this.pushSubscribtions(sub);
  }

  private _buildTreeItem(
    node: ContentTreeNode, level: number, parent: TreeNodeItem | null
  ): TreeNodeItem {
    const item: TreeNodeItem = {
      node,
      level,
      checked: false,
      indeterminate: false,
      children: [],
      parent,
    };
    item.children = (node.children || [])
      .sort((a, b) => a.order - b.order)
      .map(child => this._buildTreeItem(child, level + 1, item));
    return item;
  }

  private _setChecked(item: TreeNodeItem, checked: boolean): void {
    item.checked = checked;
    item.indeterminate = false;
    for (const child of item.children) {
      this._setChecked(child, checked);
    }
  }

  private _updateParentState(parent: TreeNodeItem | null): void {
    if (!parent) {
      return;
    }
    const allChecked = parent.children.every(c => c.checked);
    const noneChecked = parent.children.every(c => !c.checked && !c.indeterminate);

    parent.checked = allChecked;
    parent.indeterminate = !allChecked && !noneChecked;

    this._updateParentState(parent.parent);
  }

  private _collectSelectedLeafIds(items: TreeNodeItem[]): string[] {
    return items.reduce((acc: string[], item) => {
      if (item.children.length === 0) {
        return item.checked ? [...acc, item.node.nodeId] : acc;
      }
      if (item.checked) {
        // Parent is fully checked — collect ALL leaf nodes underneath
        return [...acc, ...this._getAllLeafIds(item)];
      }
      // Parent partially checked — recurse into children
      return [...acc, ...this._collectSelectedLeafIds(item.children)];
    }, []);
  }

  public isLevelNode(item: TreeNodeItem): boolean {
    return /^Level\s+\d+$/.test(item.node.row.name);
  }

  private _getAllLeafIds(item: TreeNodeItem): string[] {
    if (item.children.length === 0) {
      return [item.node.nodeId];
    }
    return item.children.reduce(
      (acc: string[], child) => [...acc, ...this._getAllLeafIds(child)],
      []
    );
  }
}
