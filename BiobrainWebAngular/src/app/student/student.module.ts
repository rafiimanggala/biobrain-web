import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { ApiModule } from '../api/api.module';
import { CoreModule } from '../core/core.module';
import { MaterialModule } from '../share/material.module';
import { SharedModule } from '../share/shared.module';

import { StudentCourseIconComponent } from './components/student-course-icon/student-course-icon.component';
import { JoinToClassDialogComponent } from './dialogs/join-to-class-dialog/join-to-class-dialog.component';
import { UseAccessCodeDialogComponent } from './dialogs/use-access-code-dialog/use-access-code-dialog.component';
import { StudentRoutingModule } from './student-routing.module';
import { CustomQuizComponent } from './views/custom-quiz/custom-quiz.component';
import { MyCoursesComponent } from './views/my-courses/my-courses.component';

const components = [

  MyCoursesComponent,
  StudentCourseIconComponent,
  CustomQuizComponent,
];

const dialogs: any[] = [JoinToClassDialogComponent, UseAccessCodeDialogComponent];

const operations: any[] = [];

const services: any[] = [];

@NgModule({
  declarations: [
    components,
    dialogs,
  ],
  providers: [
    operations,
    services,
  ],
  entryComponents: [dialogs],
  imports: [
    CoreModule,
    CommonModule,
    SharedModule,
    ApiModule,
    MaterialModule,
    StudentRoutingModule,
  ],
})
export class StudentModule {
}
