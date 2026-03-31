import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';

import { Api } from '../../../api/api.service';
import { GetLearningMaterialUserAssignmentQuery } from '../../../api/material-assignments/get-learning-material-user-assignment.query';
import { RoutingService } from '../../../auth/services/routing.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';
import { CompleteLearningMaterialAssignmentOperation } from '../../operations/complete-learning-material-assignment.operation';

@Component({
  selector: 'app-learning-material-page',
  templateUrl: './learning-material-page.component.html',
  styleUrls: ['./learning-material-page.component.scss'],
})
export class LearningMaterialPageComponent implements OnInit {

  constructor(
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _routingService: RoutingService,
    private readonly _appEvents: AppEventProvider,
    private readonly _strings: StringsService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _completeLearningMaterialAssignmentOperation: CompleteLearningMaterialAssignmentOperation,
    private readonly _api: Api,
  ) {
  }

  async ngOnInit(): Promise<void> {
    const learningMaterialUserAssignmentId = this._activatedRoute.snapshot.paramMap.get('learningMaterialUserAssignmentId');
    
    if (!hasValue(learningMaterialUserAssignmentId)) {
      this._appEvents.errorEmit(this._strings.errors.routeParameterWasNotFound('learningMaterialUserAssignmentId'));
      await this._routingService.navigateToHome();
      return;
    }

    const assignment = await firstValueFrom(this._api.send(new GetLearningMaterialUserAssignmentQuery(learningMaterialUserAssignmentId)));
    const levelId = assignment.contentTreeNodeId;
    const canPerformResult = await this._completeLearningMaterialAssignmentOperation.canPerform(levelId);
    if (canPerformResult.isFailed()) {
      await this._routingService.navigateToHome();
      return;
    }

    if(assignment.schoolClassId){
      this._activeSchoolClassService.setActiveSchoolClassId(assignment.schoolClassId);
    }

    if(assignment.schoolId){
      this._activeSchoolService.setSchoolId(assignment.schoolId);
    }

    if(assignment.schoolName){
      this._activeSchoolService.setSchoolName(assignment.schoolName);
    }

    const { courseId, topic } = canPerformResult.data;
    this._activeCourseService.setActiveCourseId(courseId);

    if (!assignment.completedAtUtc) {
      await this._completeLearningMaterialAssignmentOperation.perform(assignment.learningMaterialUserAssignmentId, levelId);
    }

    await this._routingService.navigateToMaterialPage(courseId, topic.nodeId, levelId);
  }
}
