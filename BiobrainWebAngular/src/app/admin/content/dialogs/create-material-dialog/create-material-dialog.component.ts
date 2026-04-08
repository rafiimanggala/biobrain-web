import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { CreateMaterialCommand } from 'src/app/api/content/create-material.command';
import { UpdateMaterialCommand } from 'src/app/api/content/update-material.command';
import { SummernoteService } from 'src/app/admin/services/summernote/summernote.service';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { CreateMaterialDialogData, CreateMaterialDialogResult } from './create-material-dialog-data';

@Component({
  selector: 'app-create-material-dialog',
  templateUrl: './create-material-dialog.component.html',
  styleUrls: ['./create-material-dialog.component.scss'],
  providers: [SummernoteService]
})
export class CreateMaterialDialogComponent extends DialogComponent<CreateMaterialDialogData, CreateMaterialDialogResult> {
  public isSaving: boolean = false;
  public errorMessage: string | null = null;
  public savedCount: number = 0;
  public lastSavedHeader: string | null = null;

  constructor(
    public readonly strings: StringsService,
    public readonly summernoteService: SummernoteService,
    @Inject(MAT_DIALOG_DATA) public readonly data: CreateMaterialDialogData,
    private readonly _api: Api
  ) {
    super(data);
  }

  get isEditMode(): boolean {
    return !!this.data.materialId;
  }

  get dialogTitle(): string {
    return this.isEditMode ? 'Edit Material' : 'Create Material';
  }

  onClose(): void {
    if (this.savedCount > 0 && !this.isEditMode) {
      this.close(DialogAction.save, new CreateMaterialDialogResult('', '', '', ''));
    } else {
      this.close();
    }
  }

  async onSubmit(form: NgForm): Promise<void> {
    await this.doSubmit(form, false);
  }

  async onSaveAndAddAnother(form: NgForm): Promise<void> {
    await this.doSubmit(form, true);
  }

  private async doSubmit(form: NgForm, addAnother: boolean): Promise<void> {
    if (!form.valid || this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;

    try {
      if (this.isEditMode) {
        await this._api.send(new UpdateMaterialCommand(
          this.data.materialId!,
          this.data.header,
          this.data.text,
          this.data.videoLink || null
        )).toPromise();

        this.close(DialogAction.save, new CreateMaterialDialogResult(
          this.data.materialId!,
          this.data.header,
          this.data.text,
          this.data.videoLink
        ));
        return;
      }

      const result = await this._api.send(new CreateMaterialCommand(
        this.data.courseId,
        this.data.header,
        this.data.text,
        this.data.videoLink || null,
        this.data.nodeId
      )).toPromise();

      this.savedCount += 1;
      this.lastSavedHeader = this.data.header;

      if (addAnother) {
        this.data.header = '';
        this.data.text = '';
        this.data.videoLink = '';
        form.resetForm({
          header: '',
          videoLink: '',
          text: ''
        });
        this.isSaving = false;
        return;
      }

      this.close(DialogAction.save, new CreateMaterialDialogResult(
        result.materialId,
        this.lastSavedHeader ?? '',
        this.data.text,
        this.data.videoLink
      ));
    } catch (err: any) {
      this.errorMessage = err?.message ?? 'Failed to save material.';
      this.isSaving = false;
    }
  }
}
