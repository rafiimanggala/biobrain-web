import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelectionListChange } from '@angular/material/list';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { GetSubscriptionsListQuery_Result_Course } from 'src/app/api/payments/get-subscription-list.query';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';

@Component({
  selector: 'cancel-subjects-dialog',
  templateUrl: 'cancel-subjects.dialog.html',
  styleUrls: ['cancel-subjects.dialog.scss'],
})
export class CancelSubjectsDialog extends DialogComponent<GetSubscriptionsListQuery_Result_Course[], string[]> implements OnInit {
  models: {model: GetSubscriptionsListQuery_Result_Course, isSelected: boolean}[] = [];
  get anySelected(): boolean {return this.models.some(x => x.isSelected)}

  constructor(
    public readonly strings: StringsService,
    public readonly sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public dialogData: GetSubscriptionsListQuery_Result_Course[],
  ) {
    super(dialogData);
  }

  ngOnInit(): void {
    this.models = this.dialogData.sort((a,b) => a.status - b.status).map(x => {return {model: x, isSelected: false};});
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onSubmit(): void {
    this.close(DialogAction.save, this.models.filter(x => x.isSelected).map(x => x.model.courseId));
    console.log(this.models.filter(x => x.isSelected).map(x => x.model.courseId));
  }

  onSelectionChange(event: MatSelectionListChange){
    event.options.forEach(x => x.value.isSelected = x.selected);
  }
}
