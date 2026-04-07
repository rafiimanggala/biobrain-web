import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { CoreModule } from '../core/core.module';
import { MaterialModule } from '../share/material.module';
import { SharedModule } from '../share/shared.module';

import { GlossarySideNavComponent } from './components/glossary-side-nav/glossary-side-nav.component';
import { LearningMaterialBreadcrumbsComponent } from './components/learning-material-breadcrumbs/learning-material-breadcrumbs.component';
import { LearningMaterialContentComponent } from './components/learning-material-content/learning-material-content.component';
import { LearningMaterialShadowDomNodeComponent } from './components/learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';
import { LearningMaterialToolbarComponent } from './components/learning-material-toolbar/learning-material-toolbar.component';
import { QuestionContentShadowDomNodeComponent } from './components/question-content-shadow-dom-node/question-content-shadow-dom-node.component';
import { QuestionContentComponent } from './components/question-content/question-content.component';
import { QuestionResultShadowDomNodeComponent } from './components/question-result-shadow-dom-node/question-result-shadow-dom-node.component';
import { TermDetailsHandlerComponent } from './components/term-details-handler/term-details-handler.component';
import { TermExpansionPanelComponent } from './components/term-expansion-panel/term-expansion-panel.component';
import { TermHeaderComponent } from './components/term-header/term-header.component';
import { TreeSidenavComponent } from './components/tree-sidenav/tree-sidenav.component';
import { AssignLearningMaterialsAndQuizzesDialog } from './dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.component';
import { GlossaryTermDialogComponent } from './dialogs/glossary-term-dialog/glossary-term-dialog.component';
import { HintDialogComponent } from './dialogs/hint-dialog/hint-dialog.component';
import { QuestionResultDialogComponent } from './dialogs/question-result-dialog/question-result-dialog.component';
import { SubsectionQuizDialogComponent } from './dialogs/subsection-quiz-dialog/subsection-quiz-dialog.component';
import { TopicQuizDialogComponent } from './dialogs/topic-quiz-dialog/topic-quiz-dialog.component';
import { GlossaryMasterPage } from './layouts/glossary-master-page/glossary-master-page.component';
import { QuizResultHistoryMasterPage } from './layouts/quiz-result-history-master-page/quiz-result-history-master-page.component';
import { QuizResultMasterPage } from './layouts/quiz-result-master-page/quiz-result-master-page.component';
import { LearningContentRoutingModule } from './learning-content-routing.module';
import { AssignedWorkComponent } from './views/assigned-work/assigned-work.component';
import { GlossaryComponent } from './views/glossary/glossary.component';
import { MaterialPageComponent } from './views/material/material-page.component';
import { QuizResultComponent } from './views/quiz-result/quiz-result.component';
import { QuizResultPageService } from './views/quiz-result/services/quiz-result-page.service';
import { QuizComponent } from './views/quiz/quiz.component';
import { LearningMaterialPageComponent } from './views/learning-material-page/learning-material-page.component';
import { MaterialsSearchPageComponent } from './views/materials-search-page/materials-search-page.component';
import { LearningMaterialSearchResultComponent } from './components/learning-material-search-result/learning-material-search-result.component';
import { BookmarksHandlerComponent } from './components/bookmarks-handler/bookmarks-handler.component';
import { ExcludedMaterialsHandlerComponent } from './components/excluded-materials-handler/excluded-materials-handler.component';
import { AskBiobrainComponent } from './components/ask-biobrain/ask-biobrain.component';
import { BookmarksPageComponent } from './components/bookmarks/bookmarks-page.component';
import { MatIconRegistry } from '@angular/material/icon';
import { ContentDownloadDialog } from './dialogs/content-download-dialog/content-download.dialog';
import { EmbedVideo } from 'ngx-embed-video';
import { ChemicalElementsComponent } from './components/chemical-elements/chemical-elements.component';
import { QuizOverviewPageService } from './views/quiz-overview/services/quiz-overview-page.service';
import { QuizOverviewComponent } from './views/quiz-overview/quiz-overview.component';
import { QuizOverviewMasterPage } from './layouts/quiz-overview-master-page/quiz-overview-master-page.component';
import { MaterialsClearComponent } from './views/materials-clear/materials-clear.component';

const dialogs: any[] = [
  QuestionResultDialogComponent,
  GlossaryTermDialogComponent,
  HintDialogComponent,
  AssignLearningMaterialsAndQuizzesDialog,
  ContentDownloadDialog,
  TopicQuizDialogComponent,
  SubsectionQuizDialogComponent,
];

@NgModule({
  declarations: [
    dialogs,
    GlossaryComponent,
    GlossaryMasterPage,
    GlossarySideNavComponent,
    LearningMaterialBreadcrumbsComponent,
    LearningMaterialContentComponent,
    LearningMaterialShadowDomNodeComponent,
    LearningMaterialToolbarComponent,
    MaterialPageComponent,
    QuestionContentComponent,
    QuestionContentShadowDomNodeComponent,
    QuestionResultShadowDomNodeComponent,
    QuizComponent,
    QuizResultComponent,
    QuizResultHistoryMasterPage,
    QuizResultMasterPage,
    TermDetailsHandlerComponent,
    BookmarksHandlerComponent,
    ExcludedMaterialsHandlerComponent,
    TermExpansionPanelComponent,
    TermHeaderComponent,
    TreeSidenavComponent,
    AssignedWorkComponent,
    LearningMaterialPageComponent,
    MaterialsSearchPageComponent,
    LearningMaterialSearchResultComponent,
    BookmarksPageComponent,
    ChemicalElementsComponent,
    QuizOverviewComponent,
    QuizOverviewMasterPage,
    MaterialsClearComponent,
    AskBiobrainComponent,
  ],
  exports: [dialogs],
  imports: [
    CommonModule,
    CoreModule,
    CommonModule,
    SharedModule,
    MaterialModule,
    LearningContentRoutingModule,
    EmbedVideo.forRoot()
  ],
  providers: [QuizResultPageService, QuizOverviewPageService, MatIconRegistry],
})
export class LearningContentModule {
}
