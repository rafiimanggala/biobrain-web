import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { Dialog } from '../../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { LearningContentProviderService } from '../../services/learning-content-provider.service';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { StringsService } from '../../../share/strings.service';
import { BookmarksService } from 'src/app/core/services/bookmarks/bookmarks.service';

@Component({
  selector: 'app-bookmarks-handler',
  templateUrl: './bookmarks-handler.component.html',
  styleUrls: ['./bookmarks-handler.component.scss'],
})
export class BookmarksHandlerComponent implements OnDestroy {
  private readonly _subscriptions: Subscription[] = [];

  constructor(
    private readonly _requestService: RequestHandlingService,
    private readonly _bookmarksService: BookmarksService,
  ) {
    this._subscriptions.push(
      this._requestService.swichBookmark$.subscribe({
        next: event => this._handle(event),
      }),
    );
  }

  public ngOnDestroy(): void {
    this._subscriptions.forEach(x => x.unsubscribe());
  }

  private async _handle(event: { sender: HTMLImageElement, materialId: string }): Promise<void> {
    if(!event.materialId) return;

    var isAdded = await this._bookmarksService.swichBookmark(event.materialId);

    if (event.sender) {
      event.sender.src = isAdded ? "assets/icons/bookmark-solid.svg" : "assets/icons/bookmark-regular.svg";
    }
  }
}
