import { Directive, ElementRef, EventEmitter, OnDestroy, Output } from '@angular/core';

@Directive({
  selector: '[appAppMutationObserver]',
})
export class AppMutationObserverDirective implements OnDestroy {
  @Output() innerHtmlRendered = new EventEmitter();
  private readonly _observer: MutationObserver;

  constructor(private readonly _elementRef: ElementRef) {
    this._observer = new MutationObserver(mutations => {
      mutations.forEach((mutation, _) => {
        if (mutation.type === 'childList') {
          this.innerHtmlRendered.emit();
        }
      });
    });
    this._observer.observe(
      this._elementRef.nativeElement,
      {
        attributes: true,
        childList: true,
        characterData: true,
      },
    );
  }

  ngOnDestroy(): void {
    if (this._observer) {
      this._observer.disconnect();
    }
  }
}
