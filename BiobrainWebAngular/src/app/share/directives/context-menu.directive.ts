import { AfterViewInit, Directive, Input } from '@angular/core';
import { MatMenu } from '@angular/material/menu';
import { AgGridAngular } from 'ag-grid-angular';

import { CellRendererContextMenuComponent } from '../components/renderers/cell-renderer-context-menu/cell-renderer-context-menu.component';


const menuColumnWidth = 40;
const CONTEXT_MENU_ID = 'context-menu';

@Directive({
  selector: '[appContextMenu]',
})
export class ContextMenuDirective implements AfterViewInit {
  @Input() appContextMenu: MatMenu | null | undefined;

  constructor(private readonly _gridComponent: AgGridAngular) {}

  ngAfterViewInit(): void {
    if (!this.appContextMenu) {
      return;
    }

    setTimeout(() => {
      const { columnDefs } = this._gridComponent.gridOptions;
      if (!columnDefs) {
        return;
      }

      this._gridComponent.api.setColumnDefs([
        {
          colId: CONTEXT_MENU_ID,
          cellRendererFramework: CellRendererContextMenuComponent,
          width: menuColumnWidth,
          maxWidth: menuColumnWidth,
          minWidth: menuColumnWidth,
          headerClass: ['ag-grid-context-menu-header'],
          cellStyle: {
            padding: 0,
            border: 'none'
          },
          pinned: 'left',
          resizable: false,
          suppressSizeToFit: true,
          suppressAutoSize: true,
          cellRendererParams: {
            suppressHide: true,
            contextMenu: {
              menu: this.appContextMenu,
            },
          },
        },
        ...columnDefs,
      ]);
    }, 0);
  }
}
