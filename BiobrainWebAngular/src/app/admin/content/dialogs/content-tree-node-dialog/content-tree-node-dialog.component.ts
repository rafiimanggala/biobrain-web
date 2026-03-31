import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { ContentTreeNodeDialogData, ContentTreeNodeDialogResult } from './content-tree-node-dialog-data';

@Component({
  selector: 'app-content-tree-node-dialog',
  templateUrl: './content-tree-node-dialog.component.html',
  styleUrls: ['./content-tree-node-dialog.component.scss']
})
export class ContentTreeNodeDialogComponent extends DialogComponent<ContentTreeNodeDialogData, ContentTreeNodeDialogResult> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: ContentTreeNodeDialogData,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) {
      return;
    }

    this.close(DialogAction.save, ContentTreeNodeDialogResult.updateResult(this.data));
  }

  onDelete(form: NgForm): void {
    this.close(DialogAction.delete, ContentTreeNodeDialogResult.deleteResult(this.data));
  }
}
