import { Component } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Api } from 'src/app/api/api.service';
import { CreateStudentCustomQuizCommand } from 'src/app/api/quiz-assignments/create-student-custom-quiz.command';
import { EnsureQuizResultForAssignmentCommand } from 'src/app/api/quiz-results/ensure-quiz-result-for-assignment.command';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { StudentCourse } from 'src/app/core/services/courses/student-course';
import { StudentCourseGroup } from 'src/app/core/services/courses/student-course-group';
import { StudentCoursesService } from 'src/app/core/services/courses/student-courses.service';
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
  public readonly courses$: Observable<StudentCourse[]>;

  public selectedCourseId: string | null = null;
  public quizName = '';
  public questionCount = 10;
  public readonly questionCountOptions = [10, 20, 30, 40, 50, 60];
  public treeItems: TreeNodeItem[] = [];
  public isCreating = false;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _studentCoursesService: StudentCoursesService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);

    this.courses$ = this._currentUserService.userChanges$.pipe(
      switchMap(user => hasValue(user)
        ? this._studentCoursesService.getStudentCourses(user.userId)
        : of([])),
      map(groups => groups.reduce(
        (acc: StudentCourse[], g: StudentCourseGroup) => [...acc, ...g.courses],
        []
      ))
    );

    this.pushSubscribtions(
      this.courses$.subscribe(courses => {
        if (courses.length === 1 && !this.selectedCourseId) {
          this.onCourseSelected(courses[0].courseId);
        }
      })
    );
  }

  public onCourseSelected(courseId: string): void {
    this.selectedCourseId = courseId;
    this.treeItems = [];
    this._loadContentTree(courseId);
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
      const createResult = await firstValueFrom(
        this._api.send(new CreateStudentCustomQuizCommand(
          this.quizName.trim(),
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

      await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
    } catch (err) {
      this.handleError(err);
      this.isCreating = false;
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
        return [...acc, item.node.nodeId];
      }
      return [...acc, ...this._collectSelectedLeafIds(item.children)];
    }, []);
  }
}
