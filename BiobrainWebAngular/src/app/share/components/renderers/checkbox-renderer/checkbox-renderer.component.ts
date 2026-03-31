import { Component, NgZone } from '@angular/core';

import { AgRendererComponent } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import { MatCheckboxChange } from '@angular/material/checkbox';
import deepEqual from 'deep-equal';

export interface CheckboxRendererProps extends ICellRendererParams {
  onChecked: (checked: boolean, params: CheckboxRendererProps) => void;
  checkedStateEvaluator: (params: CheckboxRendererProps) => boolean;
}

@Component({
  selector: 'app-checkbox-renderer',
  templateUrl: './checkbox-renderer.component.html',
  styleUrls: ['./checkbox-renderer.component.scss'],
})
export class CheckboxRendererComponent implements AgRendererComponent {
  params: CheckboxRendererProps | undefined;

  constructor(private readonly _ngZone: NgZone) {}

  refresh(params: CheckboxRendererProps): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: CheckboxRendererProps): void {
    this.params = params;
  }

  onChange(event: MatCheckboxChange): void {
    this._ngZone.run(() => this.params?.onChecked(event.checked, this.params));
  }

  getCheckedState(): boolean {
    return this.params?.checkedStateEvaluator(this.params) ?? false;
  }
}
