import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { DialogComponent } from '../../../core/dialogs/dialog-component';

import { WhatsNewDialogData } from './whats-new-dialog-data';

@Component({
  selector: 'whats-new-dialog',
  templateUrl: 'whats-new-dialog.component.html',
  styleUrls: ['whats-new-dialog.component.scss'],
})
export class WhatsNewDialogComponent extends DialogComponent<WhatsNewDialogData> {
  get safeContent(): SafeHtml {
    return this._sanitizer.bypassSecurityTrustHtml(this.dialogData.content);
  }

  constructor(
    private readonly _sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public dialogData: WhatsNewDialogData,
  ) {
    super(dialogData);
  }

  onGotIt(): void {
    this.close();
  }
}
