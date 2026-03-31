import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'cell-renderer-context-menu',
  templateUrl: './cell-renderer-context-menu.component.html',
  styleUrls: ['./cell-renderer-context-menu.component.scss'],
})
export class CellRendererContextMenuComponent implements ICellRendererAngularComp {
  params!: ICellRendererParams;
  agInit(params: ICellRendererParams): void {
    this.params = params;
  }

  refresh(): boolean {
    return false;
  }
}
