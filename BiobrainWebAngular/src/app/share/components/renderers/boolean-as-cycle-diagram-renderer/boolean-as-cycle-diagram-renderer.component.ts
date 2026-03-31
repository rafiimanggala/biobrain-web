import { AgRendererComponent } from 'ag-grid-angular';
import { Component } from '@angular/core';
import { ICellRendererParams } from 'ag-grid-community';
import deepEqual from 'deep-equal';

@Component({
  selector: 'app-boolean-as-cycle-diagram',
  templateUrl: './boolean-as-cycle-diagram-renderer.component.html',
  styleUrls: ['./boolean-as-cycle-diagram-renderer.component.scss'],
})
export class BooleanAsCycleDiagramRendererComponent implements AgRendererComponent {
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
    const value = this.params.getValue() as boolean;

    if (value === null || value === undefined) {
      return 'white';
    }

    return value ? 'green' : 'red';
  }
}
