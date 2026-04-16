import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AiEnabledGuard } from '../core/guards/ai-enabled.guard';
import { AuthGuard } from '../core/guards/auth.quard';
import { CourseSelectedGuard } from '../core/guards/course-selected.guard';
import { SchoolClassSelectedGuard } from '../core/guards/school-class-selected.guard';
import { BaseMasterPage } from '../share/layouts/base-master-page/base-master-page.component';
import { UserRoles } from '../share/values/user-roles.enum';
import { ClassAdminMasterPage } from './layouts/class-admin-master-page/class-admin-master-page.component';
import { ClassAdminComponent } from './views/class-admin/class-admin.component';

import { ClassResultsPageComponent } from './views/class-results-page/class-results-page.component';
import { TeacherCustomQuizComponent } from './views/custom-quiz/teacher-custom-quiz.component';
import { AiInsightsComponent } from './views/ai-insights/ai-insights.component';
import { AiPracticeSetComponent } from './views/ai-practice-set/ai-practice-set.component';
import { MyClassesComponent } from './views/my-classes/my-classes.component';
import { QuizAssignmentResultPageComponent } from './views/quiz-assignment-result-page/quiz-assignment-result-page.component';
import { QuizTemplatesComponent } from './views/quiz-templates/quiz-templates.component';
import { StudentQuizAssignmentResultsPageComponent } from './views/student-quiz-assignment-results-page/student-quiz-assignment-results-page.component';
import { TeacherAssignedWorkComponent } from './views/teacher-assigned-work/teacher-assigned-work.component';

const routes: Routes = [
  {
    path: 'teacher',
    component: BaseMasterPage,
    canActivate: [AuthGuard],
    data: {
      roles: [UserRoles.teacher],
    },
    children: [
      {
        path: 'my-classes',
        component: MyClassesComponent,
      },
      {
        path: 'class-results',
        canActivate: [SchoolClassSelectedGuard, CourseSelectedGuard],
        component: ClassResultsPageComponent,
      },
      {
        path: 'work-assigned',
        canActivate: [SchoolClassSelectedGuard, CourseSelectedGuard],
        component: TeacherAssignedWorkComponent,
      },
      {
        path: 'quiz-result/:quizAssignmentId',
        canActivate: [SchoolClassSelectedGuard, CourseSelectedGuard],
        component: QuizAssignmentResultPageComponent,
      },
      {
        path: 'student-results/:studentId',
        canActivate: [SchoolClassSelectedGuard, CourseSelectedGuard],
        component: StudentQuizAssignmentResultsPageComponent,
      },
      {
        path: 'class-admin',
        component: ClassAdminMasterPage,
        canActivate: [CourseSelectedGuard],
        children: [
          {
            path: '',
            component: ClassAdminComponent,
          },
        ],
      },
      {
        path: 'custom-quiz',
        canActivate: [CourseSelectedGuard],
        component: TeacherCustomQuizComponent,
      },
      {
        path: 'quiz-templates',
        canActivate: [CourseSelectedGuard],
        component: QuizTemplatesComponent,
      },
      {
        path: 'ai-practice-set',
        canActivate: [CourseSelectedGuard, AiEnabledGuard],
        component: AiPracticeSetComponent,
      },
      {
        path: 'ai-insights',
        canActivate: [CourseSelectedGuard, AiEnabledGuard],
        component: AiInsightsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TeacherRoutingModule {
}
