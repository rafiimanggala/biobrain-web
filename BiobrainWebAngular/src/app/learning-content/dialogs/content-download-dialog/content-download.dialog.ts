import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { ContentDownloadData } from './content-download.dialog-data';

@Component({
  selector: 'content-download-dialog',
  templateUrl: 'content-download.dialog.html',
  styleUrls: ['content-download.dialog.scss'],
})
export class ContentDownloadDialog extends DialogComponent<ContentDownloadData[]> {
  
  get isFinished(): boolean {return this.dialogData.every(_ => _.isComplete || _.isError)};

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: ContentDownloadData[],
    dialogRef: MatDialogRef<ContentDownloadDialog>
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close();
    location.reload();
  }
}
