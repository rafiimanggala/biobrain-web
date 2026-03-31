import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.quard';
import { CourseSelectedGuard } from '../core/guards/course-selected.guard';
import { SubscriptionGuard } from '../core/guards/subscription.quard';
import { QuizResultHistoryMasterPage } from '../learning-content/layouts/quiz-result-history-master-page/quiz-result-history-master-page.component';
import { BaseMasterPage } from '../share/layouts/base-master-page/base-master-page.component';
import { UserRoles } from '../share/values/user-roles.enum';
import { QuizResultHistoryComponent } from '../share/views/quiz-result-history/quiz-result-history.component';

import { CustomQuizComponent } from './views/custom-quiz/custom-quiz.component';
import { MyCoursesComponent } from './views/my-courses/my-courses.component';

const routes: Routes = [
  {
    path: 'student',
    component: BaseMasterPage,
    canActivate: [AuthGuard],
    data: {
      roles: [UserRoles.student],
    },
    children: [
      {
        path: 'my-courses',
        component: MyCoursesComponent,
      },
      {
        path: 'custom-quiz',
        component: CustomQuizComponent,
        canActivate: [SubscriptionGuard],
      },
      {
        path: 'quiz-result-history',
        component: QuizResultHistoryMasterPage,
        canActivate: [CourseSelectedGuard, SubscriptionGuard],
        children: [
          {
            path: '',
            component: QuizResultHistoryComponent,
          },
        ],
      },
    ],
  },
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StudentRoutingModule {
}
