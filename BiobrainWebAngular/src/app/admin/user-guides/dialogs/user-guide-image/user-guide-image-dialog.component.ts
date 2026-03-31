import { Component, Inject, Sanitizer } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { UserGuideImageDialogData } from './user-guide-image-dialog-data';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-user-guide-image-dialog',
  templateUrl: './user-guide-image-dialog.component.html',
  styleUrls: ['./user-guide-image-dialog.component.scss'],
})
export class UserGuideIamgeDialog extends DialogComponent<UserGuideImageDialogData, UserGuideImageDialogData> {
  fileLocalUrl: any;
  constructor(
    public readonly strings: StringsService,
    private readonly sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public readonly data: UserGuideImageDialogData,
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

    this.close(DialogAction.save, this.data);
  }

  // non-multiple, return File
  uploadFile(event: any): void {
    if (event.target.files.length > 1) return;

    let file = event.target.files[0] as File;
    this.data.fileToUpload = file;

    this.createPreviewInternal();
  }

  private createPreviewInternal() {
    var reader = new FileReader();

    reader.onloadend = () => {
      console.log("Preview");
      this.fileLocalUrl = this.sanitizer.bypassSecurityTrustUrl(reader?.result?.toString() ?? '');
    }

    if (this.data.fileToUpload) {
      reader.readAsDataURL(this.data.fileToUpload);
    } else {
      this.fileLocalUrl = null;
    }
  }

}
