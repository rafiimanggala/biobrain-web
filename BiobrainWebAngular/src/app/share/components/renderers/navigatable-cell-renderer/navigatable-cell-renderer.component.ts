import { Component, NgZone } from '@angular/core';

import { AgRendererComponent, ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import deepEqual from 'deep-equal';

export interface NavigatableCellRendererProps extends ICellRendererParams {
  handleNavigate: (props: NavigatableCellRendererProps) => void;
}

@Component({
  selector: 'app-navigatable-cell-renderer',
  templateUrl: './navigatable-cell-renderer.component.html',
  styleUrls: ['./navigatable-cell-renderer.component.scss']
})
export class NavigatableCellRendererComponent implements ICellRendererAngularComp {
  params: NavigatableCellRendererProps | undefined;

  constructor(private readonly _ngZone: NgZone) { }

  refresh(params: ICellRendererParams): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: NavigatableCellRendererProps): void {
    this.params = params;
  }

  onClick(): void {
    this._ngZone.run(() => this.params?.handleNavigate(this.params));
  }
}
