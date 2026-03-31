import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { BrowserModule } from '@angular/platform-browser';
import { AgGridModule } from 'ag-grid-angular';
import { IConfig, NgxMaskModule } from 'ngx-mask';

import { AlphabetComponent } from './components/alphabet/alphabet.component';
import { BeeComponent } from './components/bee/bee.component';
import { BiobrainCanvasComponent } from './components/biobrain-canvas/biobrain-canvas.component';
import { ButtonCellRenderer } from './components/button-cell-renderer/button-cell-renderer.component';
import { ContentCardComponent } from './components/content-card/content-card.component';
import { EditableCellRenderer } from './components/editable-cell-renderer/editable-cell-renderer.component';
import { FormLabelComponent } from './components/form-label/form-label.component';
import { HexagoneResultsComponent } from './components/hexagone-results/hexagone-results.component';
import { Hexagone } from './components/hexagone/hexagone.component';
import { HiveComponent } from './components/hive/hive.component';
import { LoaderComponent } from './components/loader/loader.component';
import { PageBreadcrumbsComponent } from './components/page-bradscrumbs/page-bradscrumbs.component';
import { QuizResultHistorySidenavComponent } from './components/quiz-result-history-sidenav/quiz-result-history-sidenav.component';
import { QuizResultSidenavComponent } from './components/quiz-result-sidenav/quiz-result-sidenav.component';
import { BooleanAsCycleDiagramRendererComponent } from './components/renderers/boolean-as-cycle-diagram-renderer/boolean-as-cycle-diagram-renderer.component';
import { ButtonWithTooltipHeaderRendererComponent } from './components/renderers/button-with-tooltip-header-renderer/button-with-tooltip-header-renderer.component';
import { CellRendererContextMenuComponent } from './components/renderers/cell-renderer-context-menu/cell-renderer-context-menu.component';
import { CheckboxRendererComponent } from './components/renderers/checkbox-renderer/checkbox-renderer.component';
import { ClickableHeaderRendererComponent } from './components/renderers/clickable-header-renderer/clickable-header-renderer.component';
import { NavigatableCellRendererComponent } from './components/renderers/navigatable-cell-renderer/navigatable-cell-renderer.component';
import { ProgressCycleRendererComponent } from './components/renderers/progress-cycle-renderer/progress-cycle-renderer.component';
import { QuizNameCellRendererComponent } from './components/renderers/quiz-name-cell-renderer/quiz-name-cell-renderer.component';
import { QuizNameHeaderRendererComponent } from './components/renderers/quiz-name-header-renderer/quiz-name-header-renderer.component';
import { SchoolNameRouterLinkRendererComponent } from './components/renderers/school-name-router-link-renderer/school-name-router-link-renderer.component';
import { ShadowDomDivContainerComponent } from './components/shadow-dom-div-container/shadow-dom-div-container.component';
import { SidenavSearchComponent } from './components/sidenav-search/sidenav-search.component';
import { SignupHeaderComponent } from './components/signup-header/signup-header.component';
import { CreateStudentService } from './components/students/services/create-student.service';
import { DeleteStudentService } from './components/students/services/delete-student.service';
import { EditStudentClassesOperation } from './components/students/services/edit-student-classes.operation';
import { UpdateStudentService } from './components/students/services/update-student.service';
import { StudentDialog } from './components/students/student-dialog/student-dialog.component';
import { SuccessRateDiagram } from './components/success-rate-diagram/success-rate-diagram.component';
import { TeacherClassIconComponent } from './components/teacher-class-icon/teacher-class-icon.component';
import { ToolbarComponent } from './components/toolbar/toolbar.component';
import { ActionListDialog } from './dialogs/action-list/action-list.dialog';
import { AppHintDialog } from './dialogs/app-hint/app-hint.dialog';
import { ConfirmationDialog } from './dialogs/confirmation/confirmation.dialog';
import { DeleteConfirmationDialog } from './dialogs/delete-confirmation/delete-confirmation.dialog';
import { ErrorMessageDialogComponent } from './dialogs/error-message-dialog/error-message-dialog.component';
import { InformationDialog } from './dialogs/information/information.dialog';
import { PickDateDialogComponent } from './dialogs/pick-date-dialog/pick-date-dialog.component';
import { SelectItemDialog } from './dialogs/select-school-dialog/select-item-dialog.component';
import { StarRatingDialogComponent } from './dialogs/star-rating-dialog/star-rating-dialog.component';
import { UserProfileDialogComponent } from './dialogs/user-profile-dialog/user-profile-dialog.component';
import { WhatsNewDialogComponent } from './dialogs/whats-new-dialog/whats-new-dialog.component';
import { AutoFocusDirective } from './directives/autofocus.directive';
import { ContextMenuDirective } from './directives/context-menu.directive';
import { ForbiddenValidatorDirective } from './directives/forbidden-validator.directive';
import { MaxNumberValidatorDirective } from './directives/max-number-validator.directive';
import { MinNumberValidatorDirective } from './directives/min-number-validator.directive';
import { AppMutationObserverDirective } from './directives/mutation-observer.derective';
import { BaseMasterPage } from './layouts/base-master-page/base-master-page.component';
import { SidenavLayoutComponent } from './layouts/sidenav-layout/sidenav-layout.component';
import { MaterialModule } from './material.module';
import { QuizResultHistoryComponent } from './views/quiz-result-history/quiz-result-history.component';
import { QuizResultHistoryPageService } from './views/quiz-result-history/services/quiz-result-history-page.service';
import { QuizOverviewSidenavComponent } from './components/quiz-overview-sidenav/quiz-overview-sidenav.component';
import { HexagoneQuestionsComponent } from './components/hexagone-questions/hexagone-questions.component';
import { QuestionTooltipRendererComponent } from './components/question-tooltip-renderer/question-tooltip-renderer.component';
import { QuestionTextService } from './services/question-text.service';
import { UserGuidesComponent } from './components/user-guides/user-guides.component';
import { UserGuidesTreeComponent } from '../admin/user-guides/user-guides-tree/user-guides-tree.component';
import { UserGuideContentComponent } from './components/user-guide-content/user-guide-content.component';

export const options: Partial<IConfig> | (() => Partial<IConfig>) = new Object();

const modules: any[] = [
  BrowserModule,
  HttpClientModule,
  MaterialModule,
  AgGridModule.withComponents([
    SchoolNameRouterLinkRendererComponent,
    ClickableHeaderRendererComponent,
    ProgressCycleRendererComponent,
    NavigatableCellRendererComponent,
    BooleanAsCycleDiagramRendererComponent,
    CheckboxRendererComponent,
    QuizNameCellRendererComponent,
    EditableCellRenderer,
  ]),
  MatIconModule,
  MatMenuModule,
  FormsModule,
  ReactiveFormsModule,
  NgxMaskModule.forRoot(options)
];

const components: any[] = [
  AlphabetComponent,
  BiobrainCanvasComponent,
  BooleanAsCycleDiagramRendererComponent,
  CellRendererContextMenuComponent,
  CheckboxRendererComponent,
  ClickableHeaderRendererComponent,
  FormLabelComponent,
  Hexagone,
  HexagoneResultsComponent,
  LoaderComponent,
  ProgressCycleRendererComponent,
  QuizResultHistoryComponent,
  QuizResultHistorySidenavComponent,
  QuizOverviewSidenavComponent,
  QuizResultSidenavComponent,
  ShadowDomDivContainerComponent,
  SidenavSearchComponent,
  SuccessRateDiagram,
  ToolbarComponent,
  BaseMasterPage,
  SidenavLayoutComponent,
  ButtonCellRenderer,
  HiveComponent,
  BeeComponent,
  QuizNameHeaderRendererComponent,
  QuizNameCellRendererComponent,
  ButtonWithTooltipHeaderRendererComponent,  
  SignupHeaderComponent,
  ContentCardComponent,
  EditableCellRenderer,
  TeacherClassIconComponent,
  PageBreadcrumbsComponent,
  HexagoneQuestionsComponent,
  NavigatableCellRendererComponent,
  QuestionTooltipRendererComponent,
  UserGuidesComponent,
  UserGuidesTreeComponent,
  UserGuideContentComponent
];

const services: any[] = [
  QuizResultHistoryPageService,
  UpdateStudentService,
  EditStudentClassesOperation,
  DeleteStudentService,
  CreateStudentService,
  QuestionTextService
];

const pipes: any[] = [];

const directives = [
  ContextMenuDirective,
  ForbiddenValidatorDirective,
  MinNumberValidatorDirective,
  MaxNumberValidatorDirective,
  AutoFocusDirective,
  AppMutationObserverDirective,
];

const dialogs = [
  ConfirmationDialog,
  DeleteConfirmationDialog,
  InformationDialog,
  ErrorMessageDialogComponent,
  StudentDialog,
  UserProfileDialogComponent,
  AppHintDialog,
  SelectItemDialog,
  PickDateDialogComponent,
  ActionListDialog,
  StarRatingDialogComponent,
  WhatsNewDialogComponent
];

@NgModule({
  imports: [modules],
  exports: [modules, components, directives],
  declarations: [components, pipes, directives, dialogs],
  entryComponents: [dialogs],
  providers: [services],
})
export class SharedModule {
}
