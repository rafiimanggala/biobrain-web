import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subscription, timer } from 'rxjs';

import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';
import { AppHintDialogData } from './app-hint.dialog-data';


@Component({
  selector: 'app-hint-dialog',
  templateUrl: 'app-hint.dialog.html',
  styleUrls: ['app-hint.dialog.scss'],
})
export class AppHintDialog extends DialogComponent<AppHintDialogData> implements OnInit {

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: AppHintDialogData,
  ) {
    super(dialogData);
  }

  ngOnInit(): void {
    if (this.dialogData.duration) {
      var tooltipTimer = timer(this.dialogData.duration);
      this.pushSubscribtions(tooltipTimer.subscribe(() => this.onClose()));
    }
  }

  onClose(): void {
    this.close();
  }
}
