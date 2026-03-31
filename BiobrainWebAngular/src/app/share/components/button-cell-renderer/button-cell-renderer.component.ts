import { Component, ElementRef, OnDestroy, ViewChild } from "@angular/core";
import { ICellRendererAngularComp } from "ag-grid-angular";
import { ICellRendererParams } from "ag-grid-community";
import { hasValue } from "../../helpers/has-value";

@Component({
  selector: 'button-cell-renderer',
  template: `
      <button id="cellButton" class="ag-grid-custom-solid-button-cell cellButton" (click)="btnClickedHandler($event)" style="width: {{width > 0 ? width.toString() + 'px' : 'initial'}}">{{text}}</button>
    `,
})
export class ButtonCellRenderer implements ICellRendererAngularComp, OnDestroy {
  text: string = 'Click Me'
  width: number = -1;
  isDisabled?: (data: any) => boolean;
  
  constructor(private elRef:ElementRef<HTMLElement>) {}

  refresh(params: ICellRendererParams): boolean {
    this.text = params.value;
    return true;
  }

  private params: any;

  agInit(params: ICellRendererParams): void {
    this.params = params;
    this.text = params.value;
    this.isDisabled = this.params.isDisabled;
    if (hasValue(this.params.width)) this.width = this.params.width;

    var button = this.elRef.nativeElement.getElementsByClassName("cellButton")[0];
    if(button && this.isButtonDisabled()){
      button.setAttribute('disabled', 'true');
    }
  }

  isButtonDisabled(): boolean{
    var value = this.isDisabled && this.params?.data ? this.isDisabled(this.params.data) : false;
    return value;
  }

  btnClickedHandler(event: any) {
    this.params.clicked(this.params.data);
  }

  ngOnDestroy() {
    // no need to remove the button click handler 
    // https://stackoverflow.com/questions/49083993/does-angular-automatically-remove-template-event-listeners
  }
}