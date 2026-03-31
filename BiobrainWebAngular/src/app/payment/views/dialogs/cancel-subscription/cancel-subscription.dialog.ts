import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';

@Component({
  selector: 'cancel-subscription-dialog',
  templateUrl: 'cancel-subscription.dialog.html',
  styleUrls: ['cancel-subscription.dialog.scss'],
})
export class CancelSubscriptionDialog extends DialogComponent<string> implements OnInit {
  content?: SafeHtml;

  constructor(
    public readonly strings: StringsService,
    public readonly sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public dialogData: string,
  ) {
    super(dialogData);
  }

  ngOnInit(): void {
    let text = this.strings.cancelSubscriptionMessage(this.dialogData);
    this.content = this.sanitizer.bypassSecurityTrustHtml(text);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }
}
