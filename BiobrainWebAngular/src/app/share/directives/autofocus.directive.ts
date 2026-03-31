import { Directive, ElementRef, OnInit } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
})
export class AutoFocusDirective implements OnInit {
  constructor(private readonly _el: ElementRef<HTMLElement>) {}

  ngOnInit(): void {
    this._el.nativeElement?.focus();
  }
}
