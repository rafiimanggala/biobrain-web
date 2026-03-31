import { Component, ViewChild } from '@angular/core';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { StringsService } from '../../strings.service';
import { QuizOverviewPageService } from 'src/app/learning-content/views/quiz-overview/services/quiz-overview-page.service';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';
import { HexagoneQuestionsComponent } from '../hexagone-questions/hexagone-questions.component';
import { QuestionViewModel } from '../hexagone-questions/models/question.model';

@Component({
  selector: 'app-quiz-overview-sidenav',
  templateUrl: './quiz-overview-sidenav.component.html',
  styleUrls: ['./quiz-overview-sidenav.component.scss'],
})
export class QuizOverviewSidenavComponent extends BaseComponent {

  name = '';
  isStudent: boolean = false;
  @ViewChild(HexagoneQuestionsComponent) hexagones: HexagoneQuestionsComponent | undefined;

  constructor(
    public readonly strings: StringsService,
    public readonly quizOverviewPageService: QuizOverviewPageService,
    private readonly _routingService: RoutingService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents); 
    this.quizOverviewPageService.dataRefreshed.subscribe(this.redrawHexagones.bind(this));
  }

  async onQuestionSelected(questionId: string): Promise<void> {
    if (!this.quizOverviewPageService.data$) {
      return;
    }
    this.quizOverviewPageService.questionClicked.emit(questionId);
  }
  
  redrawHexagones(){
    if(!this.hexagones) return;
    this.hexagones.Draw();
  }

  async onBack(data: { node: ContentTreeNode }){
    // this.location.back();
    const node = data?.node;
    if (!node) {
      this.error(this.strings.errors.contentTreeNodeWasNotFound);
      return;
    }

    const level = node;
    const topic = level?.parent;
    if (!level || !topic) {
      await this._routingService.navigateToMaterialPage(node.row.courseId, undefined, undefined);
    } else {
      await this._routingService.navigateToMaterialPage(node.row.courseId, topic.nodeId, level.nodeId);
    }
  }

  isAnyQuestionExcluded(questions: QuestionViewModel[]|null): boolean{
    if(!questions) return false;
    return questions.some(_ => _.isExcluded);
  }
}
