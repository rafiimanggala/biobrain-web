import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { StringsService } from '../../strings.service';

@Component({
  selector: 'app-sidenav-search',
  templateUrl: './sidenav-search.component.html',
  styleUrls: ['./sidenav-search.component.scss']
})
export class SidenavSearchComponent {
  @ViewChild('inputRef') inputRef: ElementRef<HTMLInputElement> | undefined;

  @Output() searchTextChange = new EventEmitter<string>();
  @Input() searchText = '';

  @Output() searchComplete = new EventEmitter();

  constructor(
    public readonly strings: StringsService
  ) {
  }

  onChange(): void {
    this.searchTextChange.emit(this.searchText);
  }

  onComplete(): void {
    this.inputRef?.nativeElement?.blur();
    this.searchComplete.emit();
  }

  onCancel(): void {
    this.searchText = '';
    this.onChange();
    this.onComplete();
  }
}
