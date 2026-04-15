import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Api } from '../../../api/api.service';
import { SubmitFeedbackCommand } from '../../../api/feedback/submit-feedback.command';
import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';

import { StarRatingDialogData, StarRatingDialogResult } from './star-rating-dialog-data';

export const STAR_RATING_RESULT_PREFIX = 'biobrain.starRatingResult.';

@Component({
  selector: 'star-rating-dialog',
  templateUrl: 'star-rating-dialog.component.html',
  styleUrls: ['star-rating-dialog.component.scss'],
})
export class StarRatingDialogComponent extends DialogComponent<StarRatingDialogData, StarRatingDialogResult> {
  rating = 0;
  hoveredRating = 0;
  feedback = '';
  submitting = false;
  submitted = false;

  readonly stars = [1, 2, 3, 4, 5];

  get displayName(): string {
    return this.dialogData.userName || '';
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData: StarRatingDialogData,
    private readonly _api: Api,
  ) {
    super(dialogData);
  }

  onStarHover(star: number): void {
    this.hoveredRating = star;
  }

  onStarLeave(): void {
    this.hoveredRating = 0;
  }

  onStarClick(star: number): void {
    this.rating = star;
  }

  getStarIcon(star: number): string {
    const active = this.hoveredRating > 0 ? this.hoveredRating : this.rating;
    return star <= active ? 'star' : 'star_border';
  }

  async onSubmit(): Promise<void> {
    if (this.rating === 0) {
      return;
    }

    this.submitting = true;
    const trimmedFeedback = this.feedback.trim();

    try {
      await this._api.send(new SubmitFeedbackCommand(this.rating, trimmedFeedback)).toPromise();
    } catch {
      // Don't block the dialog close on error
    } finally {
      this.submitting = false;
    }

    this.submitted = true;
    const result = new StarRatingDialogResult(this.rating, trimmedFeedback);

    // Show success briefly, then auto-close
    setTimeout(() => this.close(DialogAction.save, result), 1500);
  }

  onMaybeLater(): void {
    this.close(DialogAction.cancel);
  }
}
