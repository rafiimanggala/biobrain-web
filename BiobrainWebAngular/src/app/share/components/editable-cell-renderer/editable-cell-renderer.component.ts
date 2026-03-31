import { Component, OnDestroy } from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { ICellRendererParams } from "ag-grid-community";
import { hasValue } from "../../helpers/has-value";

@Component({
  selector: 'button-cell-renderer',
  template: `
      <div fxLayout="row" fxLayoutGap="10px" fxLayoutAlign="start">
        <span>{{text}}</span> 
        <button mat-icon-button aria-label="Edit" (click)="btnClickedHandler($event)">
          <mat-icon color="primary">edit</mat-icon>
        </button>
      </div>
    `,
})
export class EditableCellRenderer implements ICellRendererAngularComp, OnDestroy {

  text: string = 'Click Me'
  width: number = -1;

  refresh(params: ICellRendererParams): boolean {
    this.text = params.value;
    return true;
  }

  private params: any;

  agInit(params: ICellRendererParams): void {
    this.params = params;
    this.text = params.value;
    if (hasValue(this.params.width)) this.width = this.params.width;
  }

  btnClickedHandler(event: any) {
    this.params.clicked(this.params.data);
  }

  ngOnDestroy() {
    // no need to remove the button click handler 
    // https://stackoverflow.com/questions/49083993/does-angular-automatically-remove-template-event-listeners
  }
}