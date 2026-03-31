import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';
import { Action, ActionListDialogData } from './action-list.dialog-data';

@Component({
  selector: 'action-list-dialog',
  templateUrl: 'action-list.dialog.html',
  styleUrls: ['action-list.dialog.scss'],
})
export class ActionListDialog extends DialogComponent<ActionListDialogData, Action> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: ActionListDialogData,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onSubmit(action: Action): void {
    this.close(DialogAction.yes, action);
  }
}
