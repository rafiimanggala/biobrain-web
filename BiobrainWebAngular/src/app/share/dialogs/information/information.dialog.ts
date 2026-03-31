import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';

import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';

import { InformationDialogData } from './information.dialog-data';

@Component({
  selector: 'confirm-dialog',
  templateUrl: 'information.dialog.html',
  styleUrls: ['information.dialog.scss'],
})
export class InformationDialog extends DialogComponent<InformationDialogData> {
  get html() {return this._sanitizer.bypassSecurityTrustHtml(this.dialogData.text)}
  constructor(
    private readonly _sanitizer: DomSanitizer,
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: InformationDialogData,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close();
  }
}
