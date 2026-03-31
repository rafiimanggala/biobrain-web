import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { LearningContentProviderService } from '../../services/learning-content-provider.service';
import { hasValue } from '../../../share/helpers/has-value';
import {
  ContentTypes,
  LearningMaterialContent,
} from '../learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';
import { BookmarksService } from 'src/app/core/services/bookmarks/bookmarks.service';
import { EmbedVideoService } from 'ngx-embed-video';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { AssertNotAvailableInDemoOperation } from '../../operations/assert-not-available-in-demo.operation';

@Component({
  selector: 'app-learning-material-content',
  templateUrl: './learning-material-content.component.html',
  styleUrls: ['./learning-material-content.component.scss'],
})
export class LearningMaterialContentComponent implements OnChanges {
  @Input() courseId: string | null | undefined;
  @Input() nodeId: string | null | undefined;

  public learningContent$: Observable<LearningMaterialContent>;

  private readonly _text$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  constructor(
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _bookmarksService: BookmarksService,
    private readonly embedService: EmbedVideoService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _assertNotAvailableInDemoOperation: AssertNotAvailableInDemoOperation,
  ) {
    this.learningContent$ = this._text$.pipe(
      map(x => ({
        text: x ?? '',
        contentType: ContentTypes.pages,
      })),
    );
  }

  public async ngOnChanges(_: SimpleChanges): Promise<void> {
    const text = await this._getPageText(this.courseId, this.nodeId);
    this._text$.next(text);
  }

  private async _getPageText(courseId: string | null | undefined, nodeId: string | null | undefined): Promise<string> {
    if (!hasValue(courseId) || !hasValue(nodeId)) return '';

    const page = await this._learningContentProvider.getPageByNodeId(courseId, nodeId);
    if (!page) return '';

    const node = await firstValueFrom(this._contentTreeService.getNode(nodeId));
    if(!node) {
      return '';
    }

    const user = await this._currentUserService.user;

    if(!user || (!node.isAvailableInDemo && user.isDemoSubscription())){
      await this._assertNotAvailableInDemoOperation.perform();
      return '';
    }

    const bookmarks = await this._bookmarksService.getBookmarks() ?? [];

    let materialText = '';
    page.materials.sort((x1, x2) => x1.order - x2.order).forEach(
      m => {
        var bookmarkIcon = bookmarks.some(b => b.materialId == m.materialId) ? "assets/icons/bookmark-solid.svg" : 'assets/icons/bookmark-regular.svg';
        var bookmarkButton = user.isStudent() ? `<button><img class=\"heading-icon\" onclick="sendRequest(event, 'bookmark.${m.materialId}');" src="${bookmarkIcon}"></button>` : '';
        var conteainsVideo = hasValue(m.videoLink) && m.videoLink.length > 0;
        var videoIcon = conteainsVideo ? "<img class=\"right-margin heading-icon\" src=\"assets/icons/play-solid.svg\">" : '';
        var embedVideo = conteainsVideo ? "<div class=\"embed-video\">" + this.embedService.embed(m.videoLink, {
          attr: { width: "100%", height: "100%" },
          query: { byline: 0, portrait: 0, title: 0 },
        })['changingThisBreaksApplicationSecurity']
          + "</div>"
          : "";
        materialText = materialText + `<details><summary><span>${m.header}</span><div class=\"header-actions\">${videoIcon}${bookmarkButton}</div></summary>${m.text}${embedVideo}</details>`;
      }
    );

    return materialText;
  }
}
