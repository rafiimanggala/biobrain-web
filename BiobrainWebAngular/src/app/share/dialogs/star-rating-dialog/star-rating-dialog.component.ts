import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';

import { StarRatingDialogData, StarRatingDialogResult } from './star-rating-dialog-data';

@Component({
  selector: 'star-rating-dialog',
  templateUrl: 'star-rating-dialog.component.html',
  styleUrls: ['star-rating-dialog.component.scss'],
})
export class StarRatingDialogComponent extends DialogComponent<StarRatingDialogData, StarRatingDialogResult> {
  rating = 0;
  hoveredRating = 0;
  feedback = '';

  readonly stars = [1, 2, 3, 4, 5];

  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData: StarRatingDialogData,
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

  onSubmit(): void {
    if (this.rating === 0) {
      return;
    }
    const result = new StarRatingDialogResult(this.rating, this.feedback.trim());
    this.close(DialogAction.save, result);
  }

  onMaybeLater(): void {
    this.close(DialogAction.cancel);
  }
}
