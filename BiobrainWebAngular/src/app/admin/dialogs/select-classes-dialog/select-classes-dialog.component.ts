import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SchoolClassListStore } from 'src/app/admin/school-classes/school-class-grid/school-class-list-store';
import { SchoolCoursesListStore } from 'src/app/admin/stores/school-courses-list.store';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';

import { SelectClassesDialogData } from './select-classes-dialog-data';


@Component({
  selector: 'app-select-classes-dialog',
  templateUrl: './select-classes-dialog.component.html',
  styleUrls: ['./select-classes-dialog.component.scss'],
  providers: [
    SchoolClassListStore,
    SchoolCoursesListStore,
  ]
})
export class SelectClassesDialog extends DialogComponent<SelectClassesDialogData, SelectClassesDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: SelectClassesDialogData,
    public readonly schoolClassListStore: SchoolClassListStore,
  ) {
    super(data);
    this.schoolClassListStore.bind({ schoolId: this.data.schoolId });
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

  isSelected(schoolClassId: string) : boolean {
    return this.data.schoolClassIds.some(_ => _ === schoolClassId);
  }

  toggleSelection(schoolClassId: string, $event: MatCheckboxChange): void {
    const schoolClassIndex = this.data.schoolClassIds.findIndex(_ => _ === schoolClassId);
    const isFound = schoolClassIndex >= 0;

    if ($event.checked && !isFound) {
      this.data.schoolClassIds.push(schoolClassId);
      return;
    }

    if (!$event.checked && isFound) {
      this.data.schoolClassIds.splice(schoolClassIndex, 1);
    }
  }
}

