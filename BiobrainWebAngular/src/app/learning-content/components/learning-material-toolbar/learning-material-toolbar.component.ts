import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

import { LevelTypeModel } from '../../../api/models/level-type.model';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';
import { AssignLearningMaterialToClassOperation } from '../../operations/assign-learning-material-to-class.operation';
import { AssignLearningMaterialsAndQuizzesOperation } from '../../operations/assign-learning-materials-and-quizzes.operation';
import { AssignQuizToClassOperation } from '../../operations/assign-quiz-to-class.operation';
import { TakeQuizOperation } from '../../operations/take-quiz.operation';
import { TakeTopicQuizOperation } from '../../operations/take-topic-quiz.operation';
import { TakeSubsectionQuizOperation } from '../../operations/take-subsection-quiz.operation';
import { ViewAssignedWorkOperation } from '../../operations/view-assigned-work.operation';
import { ViewQuizOperation } from '../../operations/view-quiz.operation';

@Component({
  selector: 'app-learning-material-toolbar',
  templateUrl: './learning-material-toolbar.component.html',
  styleUrls: ['./learning-material-toolbar.component.scss'],
})
export class LearningMaterialToolbarComponent implements OnChanges {
  @Input() courseId: string | undefined;
  @Input() topicId: string | undefined;

  @Input() levels: LevelTypeModel[] = [];
  @Input() selectedLevelId: string | undefined | null;

  @Output() selectedLevelIdChange = new EventEmitter<string>();

  public quizButtonVisible = false;
  public viewQuizButtonVisible = false;
  public assignQuizButtonVisible = false;
  public assignMaterialButtonVisible = false;
  public assignAllButtonVisible = false;
  public assignedWorkButtonVisible = false;
  public topicQuizButtonVisible = false;
  public subsectionQuizButtonVisible = false;

  constructor(
    public strings: StringsService,
    private readonly _takeQuizOperation: TakeQuizOperation,
    private readonly _viewQuizOperation: ViewQuizOperation,
    private readonly _assignQuizToClassOperation: AssignQuizToClassOperation,
    private readonly _assignLearningMaterialToClassOperation: AssignLearningMaterialToClassOperation,
    private readonly _assignLearningMaterialsAndQuizzesOperation: AssignLearningMaterialsAndQuizzesOperation,
    private readonly _viewAssignedWorkOperation: ViewAssignedWorkOperation,
    private readonly _takeTopicQuizOperation: TakeTopicQuizOperation,
    private readonly _takeSubsectionQuizOperation: TakeSubsectionQuizOperation,
  ) {
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (changes.courseId || changes.selectedLevelId) {
      this.quizButtonVisible = await this._quizButtonVisible();
      this.assignQuizButtonVisible = await this._assignQuizButtonVisible();
      this.viewQuizButtonVisible = await this._viewQuizButtonVisible();
      this.assignMaterialButtonVisible = await this._assignMaterialButtonVisible();
      this.assignAllButtonVisible = await this._assignAllButtonVisible();
      this.assignedWorkButtonVisible = await this._assignedWorkButtonVisible();
      this.subsectionQuizButtonVisible = await this._subsectionQuizButtonVisible();
    }
    if (changes.courseId || changes.topicId) {
      this.topicQuizButtonVisible = await this._topicQuizButtonVisible();
    }
  }

  public onQuiz(courseId: string, nodeId: string): void {
    void this._takeQuizOperation.perform(courseId, nodeId);
  }

  public onViewQuiz(courseId: string, nodeId: string): void {
    void this._viewQuizOperation.perform(courseId, nodeId);
  }

  public onAssignQuiz(nodeId: string): void {
    void this._assignQuizToClassOperation.perform(nodeId);
  }

  public onAssignMaterial(nodeId: string): void {
    void this._assignLearningMaterialToClassOperation.perform(nodeId);
  }

  public onAssignAll(nodeId: string): void {
    void this._assignLearningMaterialsAndQuizzesOperation.perform(nodeId, this.levels.map(x => x.levelTypeId));
  }

  public onTopicQuiz(courseId: string, topicId: string): void {
    void this._takeTopicQuizOperation.perform(courseId, topicId);
  }

  public onSubsectionQuiz(nodeId: string): void {
    void this._takeSubsectionQuizOperation.perform(nodeId);
  }

  public onSelectedLevelIdChange(selectedLevelId: string): void {
    this.selectedLevelIdChange.next(selectedLevelId);
  }

  async onAssignedWorkClicked(unitId: string): Promise<void> {
    await this._viewAssignedWorkOperation.perform(unitId);
  }

  private async _quizButtonVisible(): Promise<boolean> {
    if (!hasValue(this.courseId) || !hasValue(this.selectedLevelId)) return false;
    return (await this._takeQuizOperation.canPerform(this.courseId, this.selectedLevelId)).isSuccess();
  }

  private async _viewQuizButtonVisible(): Promise<boolean> {
    if (!hasValue(this.courseId) || !hasValue(this.selectedLevelId)) return false;
    return (await this._viewQuizOperation.canPerform(this.courseId, this.selectedLevelId)).isSuccess();
  }

  private async _assignQuizButtonVisible(): Promise<boolean> {
    if (!hasValue(this.selectedLevelId)) return false;
    return (await this._assignQuizToClassOperation.canPerform(this.selectedLevelId)).isSuccess();
  }

  private async _assignMaterialButtonVisible(): Promise<boolean> {
    if (!hasValue(this.selectedLevelId)) return false;
    return (await this._assignLearningMaterialToClassOperation.canPerform(this.selectedLevelId)).isSuccess();
  }

  private async _assignAllButtonVisible(): Promise<boolean> {
    if (!hasValue(this.selectedLevelId)) return false;
    return (await this._assignLearningMaterialsAndQuizzesOperation.canPerform(this.selectedLevelId)).isSuccess();
  }

  private async _assignedWorkButtonVisible(): Promise<boolean> {
    if (!hasValue(this.selectedLevelId)) return false;
    const canPerformResult = await this._viewAssignedWorkOperation.canPerform(this.selectedLevelId);
    return canPerformResult.isSuccess();
  }

  private async _topicQuizButtonVisible(): Promise<boolean> {
    if (!hasValue(this.courseId) || !hasValue(this.topicId)) return false;
    return (await this._takeTopicQuizOperation.canPerform(this.courseId, this.topicId)).isSuccess();
  }

  private async _subsectionQuizButtonVisible(): Promise<boolean> {
    if (!hasValue(this.selectedLevelId)) return false;
    return (await this._takeSubsectionQuizOperation.canPerform()).isSuccess();
  }
}
