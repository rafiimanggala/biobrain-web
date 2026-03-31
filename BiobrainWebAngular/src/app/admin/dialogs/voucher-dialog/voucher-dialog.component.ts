import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';
import { VoucherDialogData } from './voucher-dialog-data';
import { VoucherSettings } from './voucher-settings';

@Component({
  selector: 'app-voucher-dialog',
  templateUrl: './voucher-dialog.component.html',
  styleUrls: ['./voucher-dialog.component.scss'],
})
export class VoucherDialog extends DialogComponent<VoucherDialogData, VoucherSettings> {
  voucherSettings: VoucherSettings;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: VoucherDialogData,
  ) {
    super(data);
    this.voucherSettings = new VoucherSettings();
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    this.close(DialogAction.save, this.voucherSettings);
  }
}
