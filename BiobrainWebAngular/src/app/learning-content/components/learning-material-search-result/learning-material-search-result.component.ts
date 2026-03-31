import { Component, Input, OnInit } from '@angular/core';

import { RoutingService } from '../../../auth/services/routing.service';
import { ContentTreeService } from '../../../core/services/content/content-tree.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';

@Component({
  selector: 'app-learning-material-search-result',
  templateUrl: './learning-material-search-result.component.html',
  styleUrls: ['./learning-material-search-result.component.scss']
})
export class LearningMaterialSearchResultComponent implements OnInit {
  @Input() searchResult?: SearchResultViewModel;

  public title?: string;
  public text?: string;
  public readonly searchResultType = SearchResultType;

  constructor(
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
  ) { }

  async ngOnInit(): Promise<void> {
    if (!hasValue(this.searchResult)) return;
    if (!hasValue(this.searchResult.nodeId)) return;

    const node = await firstValueFrom(this._contentTreeService.getNode(this.searchResult.nodeId));
    if (!hasValue(node)) return;

    this.title = node.fullName;
    this.text = this.searchResult.text;
  }

  async onClick(searchResult: SearchResultViewModel): Promise<void> {
    if (!hasValue(searchResult.nodeId)) return;

    if (searchResult.type === SearchResultType.ContentTree) {
      await this._navigateToTopic(searchResult);
    } else {
      await this._navigateToLevel(searchResult);
    }
  }

  private async _navigateToLevel(searchResult: SearchResultViewModel): Promise<void> {
    const level = await firstValueFrom(this._contentTreeService.getNode(searchResult.nodeId));
    if (!hasValue(level)) return;

    const topic = level.parent;
    if (!hasValue(topic)) return;

    await this._routingService.navigateToMaterialPage(searchResult.courseId, topic.nodeId, level.nodeId);
  }

  private async _navigateToTopic(searchResult: SearchResultViewModel): Promise<void> {
    const topic = await firstValueFrom(this._contentTreeService.getNode(searchResult.nodeId));
    if (!hasValue(topic)) return;

    await this._routingService.navigateToMaterialPage(searchResult.courseId, topic.nodeId, undefined);
  }
}

export enum SearchResultType {
  ContentTree,
  MaterialHeader,
  MaterialText
}

export interface SearchResultViewModel {
  type: SearchResultType;
  text: string;
  courseId: string;
  nodeId: string;
}
