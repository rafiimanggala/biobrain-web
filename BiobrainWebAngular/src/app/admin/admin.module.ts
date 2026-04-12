import { AdminRoutingModule } from './admin-routing.module';
import { BaseAdminLayoutComponent } from './base-admin-layout/base-admin-layout.component';
import { ClassNameValidatorDirective } from './directives/class-name-validator.directive';
import { NgModule } from '@angular/core';
import { SchoolAdminsDialog } from './schools/dialogs/school-admins-dialog/school-admins-dialog.component';
import { SchoolBaseComponent } from './schools/school-base/school-base.component';
import { SchoolClassDialog } from './school-classes/dialogs/school-class-dialog/school-class-dialog.component';
import { SchoolClassGridComponent } from './school-classes/school-class-grid/school-class-grid.component';
import { SchoolClassListComponent } from './school-classes/school-class-list/school-class-list.component';
import { SchoolDialog } from './schools/dialogs/school-dialog/school-dialog.component';
import { SchoolLicensesDialog } from './schools/dialogs/school-licenses-dialog/school-licenses-dialog.component';
import { SchoolListComponent } from './schools/school-list/school-list.component';
import { SchoolNameRouterLinkRendererComponent } from '../share/components/renderers/school-name-router-link-renderer/school-name-router-link-renderer.component';
import { SharedModule } from '../share/shared.module';
import { StudentGridComponent } from './students/student-grid/student-grid.component';
import { StudentListComponent } from './students/student-list/student-list.component';
import { TeacherDialog } from './teachers/dialogs/teacher-dialog/teacher-dialog.component';
import { TeacherGridComponent } from './teachers/teacher-grid/teacher-grid.component';
import { TeacherListComponent } from './teachers/teacher-list/teacher-list.component';
import { ContentMapperComponent } from './content/content-mapper/content-mapper.component';
import { MaterialModule } from '../share/material.module';
import { ContentMapperTreeComponent } from './content/content-mapper-tree/content-mapper-tree.component';
import { ContentTreeNodeDialogComponent } from './content/dialogs/content-tree-node-dialog/content-tree-node-dialog.component';
import { SelectClassesDialog } from './dialogs/select-classes-dialog/select-classes-dialog.component';
import { AttachContentDialog } from './content/dialogs/attach-content-dialog/attach-content-dialog.component';
import { AddTeacherByEmailDialogComponent } from './dialogs/add-teacher-by-email-dialog/add-teacher-by-email-dialog.component';
import { PurchaseReportComponent } from './reports/purchase-report/purchase-report.component';
import { AllStudentsPageComponent } from './students/all-students-page/all-students-page.component';
import { UsageReportComponent } from './reports/usage-report/usage-report.component';
import { ContentLoaderComponent } from './content/content-loader/content-loader.component';
import { ContentImportComponent } from './content/content-import/content-import.component';
import { AccessCodesListComponent } from './access-codes/access-codes-list/access-codes-list.component';
import { EditBatchDialog } from './dialogs/edit-batch-dialog/edit-batch-dialog.component';
import { AutoMapOptionsDialogComponent } from './content/dialogs/auto-map-options-dialog/auto-map-options-dialog.component';
import { SelectCourseDialogComponent } from './content/dialogs/select-course-dialog/select-course-dialog.component';
import { TemplatesListComponent } from './templates/templates-list/templates-list.component';
import { TemplateDialog } from './templates/dialog/template-dialog.component';
import { ContentReportComponent } from './reports/content-report/content-report.component';
import { UsageReportDialogComponent } from './dialogs/usage-report-dialog/usage-report-dialog.component';
import { UsageReportOperation } from './operations/usage-report.operation';
import { PermanentDeleteUserOperation } from './operations/permanent-delete-user.operation';
import { UserGuidesListComponent } from './user-guides/user-guides-list/user-guides-list.component';
import { UserGuideEditorComponent } from './user-guides/user-guide-editor/user-guide-editor.component';
import { UserGuideNodeDialog } from './user-guides/dialogs/user-guide-node/user-guide-node-dialog.component';
import { NgxSummernoteModule } from 'ngx-summernote';
import { UserGuideIamgeDialog } from './user-guides/dialogs/user-guide-image/user-guide-image-dialog.component';
import { VoucherListComponent } from './vouchers/vouchers-list/voucher-list.component';
import { VoucherDialog } from './dialogs/voucher-dialog/voucher-dialog.component';
import { CreateMaterialDialogComponent } from './content/dialogs/create-material-dialog/create-material-dialog.component';
import { CreateQuestionDialogComponent } from './content/dialogs/create-question-dialog/create-question-dialog.component';
import { QuizSettingsDialogComponent } from './content/dialogs/quiz-settings-dialog/quiz-settings-dialog.component';
import { QuizManagerDialogComponent } from './content/dialogs/quiz-manager-dialog/quiz-manager-dialog.component';
import { ImageLibraryComponent } from './content/image-library/image-library.component';

const dialogs = [
  SchoolDialog,
  SchoolLicensesDialog,
  TeacherDialog,
  SchoolClassDialog,
  SchoolAdminsDialog,
  SelectClassesDialog,
  ContentTreeNodeDialogComponent,
  AttachContentDialog,
  AddTeacherByEmailDialogComponent,
  EditBatchDialog,
  AutoMapOptionsDialogComponent,
  SelectCourseDialogComponent,
  TemplateDialog,
  UsageReportDialogComponent,
  UserGuideNodeDialog,
  UserGuideIamgeDialog,
  VoucherDialog,
  CreateMaterialDialogComponent,
  CreateQuestionDialogComponent,
  QuizSettingsDialogComponent,
  QuizManagerDialogComponent
];

const components = [
  SchoolListComponent,
  BaseAdminLayoutComponent,
  TeacherGridComponent,
  TeacherListComponent,
  SchoolClassGridComponent,
  SchoolClassListComponent,
  StudentGridComponent,
  StudentListComponent,
  SchoolBaseComponent,
  SchoolNameRouterLinkRendererComponent,
  ContentMapperComponent,
  ContentMapperTreeComponent,
  PurchaseReportComponent,
  AllStudentsPageComponent,
  UsageReportComponent,
  ContentLoaderComponent,
  ContentImportComponent,
  AccessCodesListComponent,
  TemplatesListComponent,
  ContentReportComponent,
  UserGuidesListComponent,
  UserGuideEditorComponent,
  VoucherListComponent,
  ImageLibraryComponent
];

const operations = [UsageReportOperation, PermanentDeleteUserOperation]

const directives = [ClassNameValidatorDirective];

@NgModule({
  declarations: [components, dialogs, directives],
  imports: [SharedModule, AdminRoutingModule, MaterialModule, NgxSummernoteModule],
  providers: [
    operations,
  ],
})
export class AdminModule { }
