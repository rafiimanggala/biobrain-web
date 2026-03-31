import { AfterViewInit, Component, HostBinding, NgZone, OnInit, ViewChild } from '@angular/core';
import { MatTooltip } from '@angular/material/tooltip';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { IHeaderAngularComp } from 'ag-grid-angular';
import { IHeaderParams } from 'ag-grid-community/dist/lib/headerRendering/header/headerComp';
import deepEqual from 'deep-equal';
import { Observable, Subscription, timer } from 'rxjs';

import { StringsService } from '../../../strings.service';

export interface ButtonWithTooltipHeaderRendererComponentParams extends IHeaderParams {
  readonly handleClick: (params: IHeaderParams) => void;
  readonly tooltip: string;
  readonly showTooltipOnInit: boolean;
}

@Component({
  selector: 'app-ag-grid-button-with-tooltip-header-renderer',
  templateUrl: './button-with-tooltip-header-renderer.component.html',
  styleUrls: ['./button-with-tooltip-header-renderer.component.scss'],
})
export class ButtonWithTooltipHeaderRendererComponent implements IHeaderAngularComp, AfterViewInit {
  params: ButtonWithTooltipHeaderRendererComponentParams | undefined;
  tooltipSubscription: Subscription|undefined;
  @HostBinding('style.width') public width = '100%';
  @ViewChild('tooltip') tooltipControl?: MatTooltip;

  constructor(private readonly _ngZone: NgZone, private readonly snitizer: DomSanitizer, public readonly strings: StringsService) {
  }

  ngAfterViewInit(): void {
    if(!this.params?.showTooltipOnInit) return;
    this.tooltipControl?.show();
    var tooltipTimer = timer(5000);
    this.tooltipSubscription = tooltipTimer.subscribe(() => this.hideTooltip(tooltipTimer));
  }

  refresh(params: IHeaderParams): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: ButtonWithTooltipHeaderRendererComponentParams): void {
    this.params = params;
  }

  onClick(): void {
    this._ngZone.run(() => this.params?.handleClick(this.params));
  }

  hideTooltip(tooltipTimer: Observable<number>) {
    this.tooltipSubscription?.unsubscribe();
    this.tooltipControl?.hide();
  }
}

