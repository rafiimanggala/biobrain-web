import { Injectable, OnDestroy } from "@angular/core";
import { combineLatest, Subscription } from "rxjs";
import { distinct } from "rxjs/operators";
import { Api } from "src/app/api/api.service";
import { CreateBookmarkForUserCommand } from "src/app/api/bookmarks/add-bookmark.command";
import { DeleteBookmarkForUserCommand } from "src/app/api/bookmarks/delete-bookmark.command";
import { GetBookmarksQuery } from "src/app/api/bookmarks/get-bookmarks.query";
import { CurrentUserService } from "src/app/auth/services/current-user.service";
import { firstValueFrom } from "src/app/share/helpers/first-value-from";
import { LoaderService } from "src/app/share/services/loader.service";
import { ActiveCourseService } from "../active-course.service";
import { BookmarkModel } from "./bookmark.model";

@Injectable({
    providedIn: 'root',
})
export class BookmarksService implements OnDestroy {
    private _status: BookmarkServiceStatus = BookmarkServiceStatus.init;
    private _subscriptions: Subscription[] = [];
    private _bookmarks: BookmarkModel[] = [];

    constructor(
        private readonly _api: Api,
        private readonly _currentUserServise: CurrentUserService,
        private readonly _courseService: ActiveCourseService,
        private readonly _loader: LoaderService
    ) {
        this._subscriptions.push(
            combineLatest([
                this._currentUserServise.userChanges$,
                this._courseService.courseIdChanges$,
            ]).pipe(distinct(x => (x[0]?.userId ?? '') + (x[1] ?? ''))).subscribe(((data: any) => {
                var courseId = data[1]
                if (!courseId) return;
                this.getBookmarksFromServerInternal.bind(this)(courseId);
            }).bind(this)));
    }

    public async addBookmark(materialId: string) {
        try {
            this._loader.show();
            var user = await this._currentUserServise.user;
            var courseId = await this._courseService.courseId;
            if (!user || !courseId) return;

            var result = await firstValueFrom(this._api.send(new CreateBookmarkForUserCommand(user.userId, courseId, materialId)));
            this.getBookmarksFromServerInternal(result.courseId);
        }
        catch (e) {
            console.log(e);
        }
        finally {
            this._loader.hideIfVisible();
        }

    }

    public async getBookmarks() : Promise<BookmarkModel[]> {
        var user = await this._currentUserServise.user;
        var courseId = await this._courseService.courseId;

        if (!user || !courseId) return[];

        // wait if loading
        for(var i = 0; i < 10; i++){
            if(this._status == BookmarkServiceStatus.ready) break;
            await new Promise(f => setTimeout(f, 200));
        }

        if (!this._bookmarks || this._bookmarks.length < 1) {
            await this.getBookmarksFromServerInternal(courseId)
        }
        return this._bookmarks;
    }

    public async deleteBookmark(bookmarkId: string) {
        try {
            this._loader.show();
            var user = await this._currentUserServise.user;
            if (!user) return;
            await firstValueFrom(this._api.send(new DeleteBookmarkForUserCommand(user.userId, bookmarkId)));
            var toDelete = this._bookmarks.findIndex(x => x.bookmarkId == bookmarkId);
            this._bookmarks.splice(toDelete, 1);
        }
        catch (e) {
            console.log(e);
        }
        finally {
            this._loader.hideIfVisible();
        }
    }

    public async swichBookmark(materialId: string): Promise<boolean> {
        var bookmark = this._bookmarks.find(b => b.materialId == materialId)
        if (bookmark) {
            await this.deleteBookmark(bookmark.bookmarkId);
            return false;
        }
        else {
            await this.addBookmark(materialId);
            return true;
        }
    }

    ngOnDestroy(): void {
        this._subscriptions?.forEach(x => x.unsubscribe());
    }

    private async getBookmarksFromServerInternal(courseId: string) {
        try {
            if (this._status != BookmarkServiceStatus.init)
                this._status = BookmarkServiceStatus.load;
                
            var user = await this._currentUserServise.user;
            if (!user) return;

            var result = await firstValueFrom(this._api.send(new GetBookmarksQuery(user.userId, courseId)));
            this._bookmarks = result ?? [];
            this._bookmarks.forEach(x => x.path = x.header.split(" > "));
            // this._bookmarks.forEach(x => x.header = x.header.replace(">", "ᐳ"));
        }
        catch (e) {
            console.log(e);
        }
        finally {
            this._status = BookmarkServiceStatus.ready;
        }
    }
}

export enum BookmarkServiceStatus {
    init,
    ready,
    load,
}