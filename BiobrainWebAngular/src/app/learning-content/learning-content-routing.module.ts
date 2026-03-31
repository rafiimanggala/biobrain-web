import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.quard';
import { CourseSelectedGuard } from '../core/guards/course-selected.guard';
import { BaseMasterPage } from '../share/layouts/base-master-page/base-master-page.component';

import { GlossaryMasterPage } from './layouts/glossary-master-page/glossary-master-page.component';
import { QuizResultHistoryMasterPage } from './layouts/quiz-result-history-master-page/quiz-result-history-master-page.component';
import { QuizResultMasterPage } from './layouts/quiz-result-master-page/quiz-result-master-page.component';
import { AssignedWorkComponent } from './views/assigned-work/assigned-work.component';
import { GlossaryComponent } from './views/glossary/glossary.component';
import { MaterialsSearchPageComponent } from './views/materials-search-page/materials-search-page.component';
import { LearningMaterialPageComponent } from './views/learning-material-page/learning-material-page.component';
import { MaterialPageComponent } from './views/material/material-page.component';
import { QuizResultComponent } from './views/quiz-result/quiz-result.component';
import { QuizComponent } from './views/quiz/quiz.component';
import { SubscriptionGuard } from '../core/guards/subscription.quard';
import { BookmarksPageComponent } from './components/bookmarks/bookmarks-page.component';
import { ChemicalElementsComponent } from './components/chemical-elements/chemical-elements.component';
import { QuizOverviewComponent } from './views/quiz-overview/quiz-overview.component';
import { UserRoles } from '../share/values/user-roles.enum';
import { QuizOverviewMasterPage } from './layouts/quiz-overview-master-page/quiz-overview-master-page.component';
import { MaterialsClearComponent } from './views/materials-clear/materials-clear.component';
import { UserGuidesComponent } from '../share/components/user-guides/user-guides.component';

const routes: Routes = [
  {
    path: '',
    component: BaseMasterPage,
    canActivate: [AuthGuard, SubscriptionGuard],
    children: [   
      {
        path: 'bookmarks',
        component: BookmarksPageComponent,
        canActivate: [CourseSelectedGuard],
      }, 
      {
        path: 'periodic-table',
        component: ChemicalElementsComponent,
        canActivate: [CourseSelectedGuard],
      },   
      {
        path: 'glossary',
        component: GlossaryMasterPage,
        canActivate: [CourseSelectedGuard],
        children: [
          {
            path: '',
            component: GlossaryComponent,
          },
        ],
      },
      {
        path: 'materials/course/:courseId',
        component: MaterialPageComponent,
      },
      {
        path: 'clear-cache',
        component: MaterialsClearComponent,
      },
      {
        path: 'materials-search/course/:courseId',
        component: MaterialsSearchPageComponent,
      },
      {
        path: 'perform-assigned-learning-material/:learningMaterialUserAssignmentId',
        component: LearningMaterialPageComponent,
      },
      {
        path: 'quiz',
        canActivate: [CourseSelectedGuard],
        children: [
          {
            path: ':quizResultId',
            component: QuizComponent,
          },
          {
            path: ':quizResultId/question/:questionId',
            component: QuizComponent,
          },
        ],
      },
      {
        path: 'quiz-result',
        component: QuizResultMasterPage,
        canActivate: [CourseSelectedGuard],
        children: [
          {
            path: ':quizResultId',
            component: QuizResultComponent,
          },
          {
            path: ':quizResultId/question/:questionId',
            component: QuizResultComponent,
          },
        ],
      },
      {
        path: 'quiz-overview',
        component: QuizOverviewMasterPage,
        canActivate: [CourseSelectedGuard],
        data: {
          roles: [UserRoles.teacher],
        },
        children: [
          {
            path: ':quizId',
            component: QuizOverviewComponent,
          },
        ],
      },
      {
        path: 'assigned-work',
        component: QuizResultHistoryMasterPage,
        canActivate: [CourseSelectedGuard],
        children: [
          {
            path: ':rootContentNodeId',
            component: AssignedWorkComponent
          }
        ]
      },
      {
        path: 'user_guides',
        component: UserGuidesComponent,
        data: {
          roles: [UserRoles.teacher, UserRoles.student],
          navigatingSave: true,
        },
        canActivate: [AuthGuard]
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class LearningContentRoutingModule {
}
