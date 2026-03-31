import { Component, OnInit } from '@angular/core';
import { distinctUntilChanged, tap } from 'rxjs/operators';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { BookmarkModel } from 'src/app/core/services/bookmarks/bookmark.model';
import { BookmarksService } from 'src/app/core/services/bookmarks/bookmarks.service';
import { StringsService } from 'src/app/share/strings.service';

import { RoutingService } from '../../../auth/services/routing.service';

@Component({
  selector: 'app-bookmarks-page',
  templateUrl: './bookmarks-page.component.html',
  styleUrls: ['./bookmarks-page.component.scss']
})
export class BookmarksPageComponent extends BaseComponent implements OnInit {
  subjectName: string = '';
  courseId: string = '';
  bookmarks: BookmarkModel[] = [];

  constructor(
    private readonly _bookmarksService: BookmarksService,
    private readonly _routingService: RoutingService,
    private readonly _activeCourseService: ActiveCourseService,
    public readonly strings: StringsService,
    readonly appEvents: AppEventProvider
  ) {
    super(appEvents);
    this.pushSubscribtions(
      this._activeCourseService.courseChanges$.pipe(
        distinctUntilChanged(),
        tap(x => {
          this.subjectName = x?.subject?.name ?? '';
          this.courseId = x?.courseId ?? '';
        }),
      ).subscribe()
    );
  }

  async ngOnInit(): Promise<void> {
    this.bookmarks = await this._bookmarksService.getBookmarks();
  }

  async onBookmarkClick(bookmark: BookmarkModel){
    if(!bookmark || !this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, bookmark.nodeId, bookmark.levelId);
  }

  async onDeleteClick(bookmark: BookmarkModel){
    if(!bookmark) return;
    await this._bookmarksService.deleteBookmark(bookmark.bookmarkId);
    this.bookmarks = await this._bookmarksService.getBookmarks();
  }
}
