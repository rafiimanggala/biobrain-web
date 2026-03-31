import { Component, Input, Output, EventEmitter, HostBinding } from '@angular/core';

@Component({
  selector: 'app-term-header',
  templateUrl: './term-header.component.html',
  styleUrls: ['./term-header.component.scss'],
})
export class TermHeaderComponent {
  @Input() header: string | undefined;
  @Input() isFirst = false;
  @Input() isDisabled = false;
  @Output() showDescription = new EventEmitter();

  @HostBinding('class.app-term-header') appTermHeader = true;

  onClick(): void {
    this.showDescription.emit();
  }
}
