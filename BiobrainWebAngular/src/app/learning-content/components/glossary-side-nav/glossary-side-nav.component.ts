import { Component, OnInit } from '@angular/core';

import { SidenavService } from '../../../core/services/side-nav.service';
import { assertUnreachableStatement } from '../../../share/helpers/assert-unreachable-statement';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { StringsService } from '../../../share/strings.service';
import { GlossarySearchMessageBus, SearchAlgorithms } from '../../services/glossary-search-message-bus.service';

@Component({
  selector: 'app-glossary-side-nav',
  templateUrl: './glossary-side-nav.component.html',
  styleUrls: ['./glossary-side-nav.component.scss']
})
export class GlossarySideNavComponent implements OnInit {
  searchString = '';
  searchLetter = '';

  constructor(
    public readonly strings: StringsService,
    private readonly _glossarySearchMessageBus: GlossarySearchMessageBus,
    private readonly _sidenavService: SidenavService
  ) {
  }

  onSearchChanged($event: string): void {
    this._glossarySearchMessageBus.next({
      algorithm: SearchAlgorithms.HeaderContains,
      searchText: $event
    });

    this.searchLetter = '';
  }

  onLetterSelected($event: string): void {
    this._glossarySearchMessageBus.next({
      algorithm: SearchAlgorithms.StartFromLetter,
      searchText: $event
    });

    this.searchString = '';
    this._sidenavService.close();
  }

  onSearchComplete(): void {
    this._sidenavService.close();
  }

  async ngOnInit(): Promise<void> {
    const searchData = await firstValueFrom(this._glossarySearchMessageBus.search$);

    switch (searchData.algorithm) {
      case SearchAlgorithms.Unknown:
        break;
      case SearchAlgorithms.HeaderContains:
        this.searchString = searchData.searchText ?? '';
        this.searchLetter = '';
        break;
      case SearchAlgorithms.StartFromLetter:
        this.searchString = '';
        this.searchLetter = searchData.searchText ?? '';
        break;
      default:
        assertUnreachableStatement(searchData.algorithm);
    }
  }
}
