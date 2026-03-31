import { Component } from '@angular/core';
import { first } from 'rxjs/operators';
import { CurrentUser } from 'src/app/auth/services/current-user';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';

import { SidenavService } from '../../../core/services/side-nav.service';
import { hasValue } from '../../helpers/has-value';
import { StringsService } from '../../strings.service';
import { QuizResultPageService } from '../../../learning-content/views/quiz-result/services/quiz-result-page.service';
import { ViewAssignedWorkOperation } from 'src/app/learning-content/operations/view-assigned-work.operation';

@Component({
  selector: 'app-quiz-result-sidenav',
  templateUrl: './quiz-result-sidenav.component.html',
  styleUrls: ['./quiz-result-sidenav.component.scss'],
})
export class QuizResultSidenavComponent extends BaseComponent {

  name = '';
  isStudent: boolean = false;

  constructor(
    public readonly strings: StringsService,
    public readonly quizResultPageService: QuizResultPageService,
    private readonly _routingService: RoutingService,
    private readonly _sidenavService: SidenavService,
    private readonly _viewAssignedWorkOperation: ViewAssignedWorkOperation,
    userService: CurrentUserService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
    this.subscriptions.push(userService.userChanges$.subscribe(this._setUser.bind(this)));

  }

  async onQuestionSelected(questionId: string): Promise<void> {
    if (!this.quizResultPageService.data$) {
      return;
    }
    const data = await this.quizResultPageService.data$.pipe(first()).toPromise();
    this._sidenavService.close();
    await this._routingService.navigateToQuizResultPage(data.quizResult.quizResultId, questionId, { replaceUrl: hasValue(data.question) });
  }

  async onNextLevel(data: { node: ContentTreeNode }): Promise<void> {
    const node = data?.node;
    if (!node) {
      this.error(this.strings.errors.contentTreeNodeWasNotFound);
      return;
    }

    const level = this.getNextSibling(node);
    const topic = level?.parent;
    if (!level || !topic) {
      await this._routingService.navigateToMaterialPage(node.row.courseId, undefined, undefined);
    } else {
      await this._routingService.navigateToMaterialPage(node.row.courseId, topic.nodeId, level.nodeId);
    }
  }

  async onNextTopic(data: { node: ContentTreeNode }): Promise<void> {
    const node = data?.node?.parent;
    if (!node) {
      this.error(this.strings.errors.contentTreeNodeWasNotFound);
      return;
    }

    const topic = this.getNextSibling(node);
    if (!topic) {
      await this._routingService.navigateToMaterialPage(node.row.courseId, undefined, undefined);
    } else {
      await this._routingService.navigateToMaterialPage(node.row.courseId, topic.nodeId, undefined);
    }
  }

  getNextSibling(node: ContentTreeNode): ContentTreeNode | null {
    const { parent } = node;
    if (!parent) {
      return null;
    }

    const sibling = parent.children.find(x => x.order === node.order + 1);
    if (sibling) {
      return sibling;
    }

    const parentSibling = this.getNextSibling(parent);
    if (!parentSibling) {
      return null;
    }

    parentSibling.children.sort((a, b) => a.order - b.order);
    return parentSibling.children.length === 0 ? null : parentSibling.children[0];
  }
  
  async onAssignedWork(data: { node: ContentTreeNode }){
    await this._viewAssignedWorkOperation.perform(data.node.nodeId);
  }

  private _setUser(user: CurrentUser | undefined): void {
    this.name = user?.firstName ?? '';
    this.isStudent = user?.isStudent() ?? false;
  }
}
