import { Component, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { combineLatest, Observable, Subscription } from 'rxjs';
import { debounceTime, filter, map, switchMap, withLatestFrom } from 'rxjs/operators';

import { Material, MaterialPage } from '../../../api/content/content-data-models';
import { RoutingService } from '../../../auth/services/routing.service';
import { ContentTreeService } from '../../../core/services/content/content-tree.service';
import { hasValue } from '../../../share/helpers/has-value';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';
import { LoaderService } from '../../../share/services/loader.service';
import { StringsService } from '../../../share/strings.service';
import {
  SearchResultType,
  SearchResultViewModel,
} from '../../components/learning-material-search-result/learning-material-search-result.component';
import { ContentTreeRow } from '../../services/learning-content-db';
import { LearningContentProviderService } from '../../services/learning-content-provider.service';

@Component({
  selector: 'app-materials-search-page',
  templateUrl: './materials-search-page.component.html',
  styleUrls: ['./materials-search-page.component.scss'],
})
export class MaterialsSearchPageComponent implements OnDestroy {
  public readonly searchResults$: Observable<SearchResultViewModel[]>;
  public readonly searchControl: FormControl;

  private readonly _subscriptions: Subscription[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _routingService: RoutingService,
    private readonly _learningContentProviderService: LearningContentProviderService,
    private readonly _loaderService: LoaderService,
    private readonly _contentTreeService: ContentTreeService,
  ) {
    const routeCourseId$ = this._activatedRoute.paramMap.pipe(map(_ => toNonNullable(_.get('courseId'))));

    const getTerm = (paramMap: ParamMap): string => paramMap.get('term') ?? '';
    const routeTerm$ = this._activatedRoute.queryParamMap.pipe(map(getTerm));

    this.searchControl = new FormControl(getTerm(this._activatedRoute.snapshot.queryParamMap));

    this._subscriptions.push(
      this.searchControl.valueChanges.pipe(
        debounceTime(500),
        filter((value: string) => value.length > 1),
        withLatestFrom(routeCourseId$),
      ).subscribe({
        next: ([term, courseId]) => this._routingService.navigateToMaterialsSearch(courseId, term, true),
      }),
    );

    this.searchResults$ = combineLatest([routeCourseId$, routeTerm$]).pipe(
      switchMap(([courseId, term]) => this._search(courseId, term)),
    );
  }

  private static _findTermInMaterial(page: MaterialPage, material: Material, term: string): SearchResultViewModel | undefined {
    const termInHeader = findTermInHtml(material.header, term);
    if (hasValue(termInHeader)) {
      return {
        type: SearchResultType.MaterialHeader,
        text: termInHeader,
        courseId: page.courseId,
        nodeId: page.contentTreeNodeId,
      };
    }

    const termInText = findTermInHtml(material.text, term);
    if (hasValue(termInText)) {
      return {
        type: SearchResultType.MaterialText,
        text: termInText,
        courseId: page.courseId,
        nodeId: page.contentTreeNodeId,
      };
    }

    return undefined;
  }

  private static _findTermInContentTreeRow(row: ContentTreeRow, term: string): SearchResultViewModel | undefined {
    if (row.contentTreeMeta.couldAddContent || row.contentTreeMeta.couldAddEntry) return undefined;

    const termInHeader = findTextInText(row.name, term);
    if (hasValue(termInHeader)) {
      return {
        type: SearchResultType.ContentTree,
        text: termInHeader,
        courseId: row.courseId,
        nodeId: row.nodeId,
      };
    }
    return undefined;
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(_ => _.unsubscribe());
  }

  private async _search(courseId: string, term: string): Promise<SearchResultViewModel[]> {
    this._loaderService.show();
    const result = await Promise.all([
      this._searchInContentTree(courseId, term),
      this._searchInMaterialPages(courseId, term),
    ]);
    this._loaderService.hide();

    const results = result.reduce((r, i) => r.concat(i), []);
    return results.slice(0, 50);
  }

  private async _searchInMaterialPages(courseId: string, term: string): Promise<SearchResultViewModel[]> {
    const pages = await this._learningContentProviderService.getPagesForCourse(courseId);
    return pages.reduce(
      (result: SearchResultViewModel[], page) => result.concat(this._searchTermInMaterialPage(page, term)),
      [],
    );
  }

  private _searchTermInMaterialPage(page: MaterialPage, term: string): SearchResultViewModel[] {
    return page.materials.map(material => MaterialsSearchPageComponent._findTermInMaterial(page, material, term)).filter(hasValue);
  }

  private async _searchInContentTree(courseId: string, term: string): Promise<SearchResultViewModel[]> {
    const tree = await this._learningContentProviderService.getContentTreeForCourse(courseId);
    return this._recursiveSearchInContentTree(tree, term);
  }

  private _recursiveSearchInContentTree(rows: ContentTreeRow[], term: string): SearchResultViewModel[] {
    return rows.reduce(
      (result, item) => result.concat(this._recursiveSearchInContentTree(item.nodes ?? [], term)),
      rows.map(row => MaterialsSearchPageComponent._findTermInContentTreeRow(row, term)).filter(hasValue),
    );
  }
}

function findTermInHtml(text: string, term: string, size = 200): string | undefined {
  const textWithoutTags = removeTags(text);
  return findTextInText(textWithoutTags, term, size);
}

function removeTags(text: string): string {
  return text.replace(/<[^>]*>/g, ' ').replace(/&nbsp;/g, ' ').replace(/\s{2,}/g, ' ').trim();
}

function findTextInText(text: string, term: string, size = 200): string | undefined {
  const termIndex = text.toLowerCase().indexOf(term.toLowerCase());
  if (termIndex < 0) return undefined;

  const start = Math.max(0, termIndex - size / 2);
  const end = Math.min(text.length, termIndex + size / 2);
  return `${start > 0 ? '...' : ''}${text.slice(start, end)}${end < text.length ? '...' : ''}`;
}

