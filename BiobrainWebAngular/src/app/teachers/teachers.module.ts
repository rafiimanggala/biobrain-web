import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { ReactiveFormsModule } from '@angular/forms';

import { ApiModule } from '../api/api.module';
import { CoreModule } from '../core/core.module';
import { MaterialModule } from '../share/material.module';
import { SharedModule } from '../share/shared.module';
import { ClassAdminSidenavComponent } from './components/class-admin-sidenav/class-admin-sidenav.component';

import { AddTeacherDialog } from './dialogs/add-teacher/add-teacher.dialog';
import { EmailClassDialog } from './dialogs/email-class/email-class.dialog';
import { InviteStudentDialog } from './dialogs/invite-student/invite-student.dialog';
import { RenameClassDialog } from './dialogs/rename-class/rename-class.dialog';
import { SelectClassDialog } from './dialogs/select-class-dialog/select-class-dialog.component';
import { TeacherListDialog } from './dialogs/teacher-list/teacher-list.dialog';
import { ClassAdminMasterPage } from './layouts/class-admin-master-page/class-admin-master-page.component';
import { TeacherRoutingModule } from './teacher-routing.module';
import { ClassAdminComponent } from './views/class-admin/class-admin.component';
import { ClassResultsPageComponent } from './views/class-results-page/class-results-page.component';
import { ClassResultsComponent } from './views/class-results/class-results.component';
import { TeacherCustomQuizComponent } from './views/custom-quiz/teacher-custom-quiz.component';
import { MyClassesComponent } from './views/my-classes/my-classes.component';
import { QuizAssignmentResultPageComponent } from './views/quiz-assignment-result-page/quiz-assignment-result-page.component';
import { QuizAssignmentResultComponent } from './views/quiz-assignment-result/quiz-assignment-result.component';
import { QuizTemplatesComponent } from './views/quiz-templates/quiz-templates.component';
import { StudentQuizAssignmentResultsPageComponent } from './views/student-quiz-assignment-results-page/student-quiz-assignment-results-page.component';
import { StudentQuizAssignmentResultsComponent } from './views/student-quiz-assignment-results/student-quiz-assignment-results.component';
import { AiInsightsComponent } from './views/ai-insights/ai-insights.component';
import { AiPracticeSetComponent } from './views/ai-practice-set/ai-practice-set.component';
import { TeacherAssignedWorkComponent } from './views/teacher-assigned-work/teacher-assigned-work.component';


const dialogs: any[] = [RenameClassDialog, InviteStudentDialog, EmailClassDialog, AddTeacherDialog, TeacherListDialog, SelectClassDialog];

@NgModule({
  declarations: [
    MyClassesComponent,
    ClassResultsComponent,
    ClassResultsPageComponent,
    QuizAssignmentResultPageComponent,
    QuizAssignmentResultComponent,
    StudentQuizAssignmentResultsComponent,
    StudentQuizAssignmentResultsPageComponent,
    ClassAdminComponent,
    ClassAdminSidenavComponent,
    TeacherAssignedWorkComponent,
    ClassAdminMasterPage,
    TeacherCustomQuizComponent,
    QuizTemplatesComponent,
    AiPracticeSetComponent,
    AiInsightsComponent,
    dialogs
  ],
  imports: [
    CommonModule,
    TeacherRoutingModule,
    CoreModule,
    SharedModule,
    ApiModule,
    MaterialModule,
    MatTableModule,
    ReactiveFormsModule,
  ],
  entryComponents: [
    SelectClassDialog,
  ],
})
export class TeachersModule {
}
