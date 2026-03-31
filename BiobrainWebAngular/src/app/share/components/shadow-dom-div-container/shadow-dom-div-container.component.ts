import { Component, Input, ViewEncapsulation } from '@angular/core';
import { SafeHtml } from '@angular/platform-browser';
import { Observable, ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-shadow-dom-div-container',
  templateUrl: './shadow-dom-div-container.component.html',
  styleUrls: ['./shadow-dom-div-container.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom
})
export class ShadowDomDivContainerComponent {
  content$ = new ReplaySubject<SafeHtml>(1);
  formattedHtml$: Observable<SafeHtml>;

  constructor() {
    this.formattedHtml$ = this.content$.asObservable();
  }

  @Input() set innerContent(value: SafeHtml | null | undefined) {
    this.content$.next(value ?? '');
  }
}
