import { Component } from '@angular/core';
import { AgRendererComponent } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import deepEqual from 'deep-equal';

import { getScoreColor } from '../../../../teachers/helpers/get-score-color';
import { hasValue } from '../../../helpers/has-value';

@Component({
  selector: 'app-progress-cycle',
  templateUrl: './progress-cycle-renderer.component.html',
  styleUrls: ['./progress-cycle-renderer.component.scss'],
})
export class ProgressCycleRendererComponent implements AgRendererComponent {
  params: ICellRendererParams | undefined;

  refresh(params: ICellRendererParams): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: ICellRendererParams): void {
    this.params = params;
  }

  resolveColor(): string {
    if (!this.params?.data) {
      return 'white';
    }

    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
    const progress = this.params.getValue() as number;
    if (!hasValue(progress)) return 'white';

    return getScoreColor(progress);
  }
}


