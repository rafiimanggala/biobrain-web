import { Component } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { distinctUntilChanged, filter, map, startWith, switchMap, tap } from 'rxjs/operators';
import { Colors } from 'src/app/share/values/colors';

import { GlossaryTerm } from '../../../api/content/content-data-models';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { SidenavService } from '../../../core/services/side-nav.service';
import {
  ContentTypes,
  LearningMaterialContent,
} from '../../components/learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';
import { hasValue } from '../../../share/helpers/has-value';
import { isNullOrWhitespace } from '../../../share/helpers/is-null-or-white-space';
import { StringsService } from '../../../share/strings.service';
import { GlossarySearchMessageBus, SearchAlgorithms } from '../../services/glossary-search-message-bus.service';
import { LearningContentProviderService } from '../../services/learning-content-provider.service';
import { filterGlossary } from '../../utils/glossary-text-filter';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';

@Component({
  selector: 'app-glossary',
  templateUrl: './glossary.component.html',
  styleUrls: ['./glossary.component.scss'],
})
export class GlossaryComponent {
  glossary$: Observable<GlossaryTermViewModel[]>;

  subjectName$: Observable<string>;
  courseId: string = '';
  searchLetter$: Observable<string>;
  isDemoMode = false;

  private _expandedTerm: string | undefined = undefined;

  constructor(
    public readonly strings: StringsService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _contentProvider: LearningContentProviderService,
    private readonly _sidenavService: SidenavService,
    private readonly _glossarySearchMessageBus: GlossarySearchMessageBus,
    private readonly _routingService: RoutingService,
    private readonly _currentUserService: CurrentUserService,
  ) {
    const { search$ } = this._glossarySearchMessageBus;
    
    const user$ = this._currentUserService.userChanges$.pipe(
      tap(_ => this.isDemoMode = _?.isDemoSubscription() ?? true)
    );

    const glossary$ = this._activeCourseService.courseChanges$.pipe(
      map(x => x?.subjectCode),
      distinctUntilChanged(),
      switchMap(x => x ? this._contentProvider.getGlossaryForSubject(x) : Promise.resolve([])),
      map(x => x.map(this.convertGlossaryTermToViewModel.bind(this))),
      map(x => x.sort((a, b) => a.normalizedHeader.localeCompare(b.normalizedHeader))),
    );

    this.glossary$ = combineLatest([glossary$, search$, user$]).pipe(
      map(([glossary, searchData, user]) => filterGlossary(glossary, searchData, this.isDemoMode)),
    );

    this.subjectName$ = this._activeCourseService.courseChanges$.pipe(
      tap(x => {this.courseId = x?.courseId ?? '';}),
      map(x => x?.subject?.name ?? ''),
      distinctUntilChanged(),
    );

    this.searchLetter$ = search$.pipe(
      map(x => x.algorithm === SearchAlgorithms.StartFromLetter ? x.searchText : ''),
      filter(hasValue),
      startWith(''),
    );
  }

  get hexColor(): string {
    return Colors.red;
  }

  onCourseClick() {
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }

  convertGlossaryTermToViewModel(term: GlossaryTerm): GlossaryTermViewModel {
    const header = isNullOrWhitespace(term.header) ? term.term : term.header;

    return {
      header,
      normalizedHeader: header.replace(/["#$%&'()*+,./:<>?\\{}~]/g, '').toLowerCase(),
      learningContent: {
        text: term.definition,
        contentType: ContentTypes.glossaryDefinition,
      },
      isDisabled: false
    };
  }

  showExpansionPanel(term: GlossaryTermViewModel): void {
    this._expandedTerm = term.normalizedHeader;
  }

  isExpanded(term: GlossaryTermViewModel): boolean {
    return this._expandedTerm === term.normalizedHeader;
  }
}

interface GlossaryTermViewModel {
  readonly header: string;
  readonly normalizedHeader: string;
  readonly learningContent: LearningMaterialContent;
  isDisabled: boolean;
}
