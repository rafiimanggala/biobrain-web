import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { Api } from '../../../api/api.service';
import { SubmitFeedbackCommand } from '../../../api/feedback/submit-feedback.command';
import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';

import { StarRatingDialogData, StarRatingDialogResult } from './star-rating-dialog-data';

export const STAR_RATING_RESULT_PREFIX = 'biobrain.starRatingResult.';

// Google Place ID for BioBrain — replace with actual Place ID once available
const GOOGLE_PLACE_ID = 'ChIJn1Izqh1o1moRCCAqOE0pqIg';
const GOOGLE_REVIEW_URL = GOOGLE_PLACE_ID
  ? `https://search.google.com/local/writereview?placeid=${GOOGLE_PLACE_ID}`
  : '';

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
  copiedToClipboard = false;

  readonly stars = [1, 2, 3, 4, 5];

  get displayName(): string {
    return this.dialogData.userName || '';
  }

  get showGoogleReview(): boolean {
    return this.submitted && this.rating >= 4 && !!GOOGLE_REVIEW_URL;
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

    if (this.showGoogleReview) {
      // Don't auto-close — let user interact with Google review CTA
      return;
    }

    // Show success briefly, then auto-close
    setTimeout(() => this.close(DialogAction.save, result), 1500);
  }

  async copyAndOpenGoogle(): Promise<void> {
    const reviewText = this.feedback.trim();
    if (reviewText) {
      try {
        await navigator.clipboard.writeText(reviewText);
        this.copiedToClipboard = true;
      } catch {
        // Clipboard API may fail in some browsers — proceed anyway
      }
    }
    window.open(GOOGLE_REVIEW_URL, '_blank');
  }

  onDone(): void {
    const result = new StarRatingDialogResult(this.rating, this.feedback.trim());
    this.close(DialogAction.save, result);
  }

  onMaybeLater(): void {
    this.close(DialogAction.cancel);
  }
}
