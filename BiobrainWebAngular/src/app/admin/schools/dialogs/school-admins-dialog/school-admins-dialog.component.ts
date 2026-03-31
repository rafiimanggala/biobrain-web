import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { interval } from 'rxjs';
import { debounce, distinctUntilChanged, map } from 'rxjs/operators';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { TeacherlListItemsMultipleSelectStore } from 'src/app/share/list-item-stores/teacher-list-items-multiple-select-store';
import { TeacherListItemsStore } from 'src/app/share/list-item-stores/teacher-list-items-store';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { SchoolAdminsDialogData } from './school-admins-dialog-data';


const debounceTimeMs = 200;

@Component({
  selector: 'app-school-admins-dialog',
  templateUrl: './school-admins-dialog.component.html',
  styleUrls: ['./school-admins-dialog.component.scss'],
  providers: [TeacherListItemsStore, TeacherlListItemsMultipleSelectStore],
})
export class SchoolAdminsDialog extends DialogComponent<SchoolAdminsDialogData, SchoolAdminsDialogData> implements OnInit {
  @ViewChild('adminsInput') adminsInput: ElementRef<HTMLInputElement> | undefined;
  @ViewChild('adminsAuto') adminsAuto: MatAutocomplete | undefined;
  adminsCtrl = new FormControl();

  private _selectedAdmins: string[] = [];

  constructor(
    public readonly strings: StringsService,
    public readonly teacherListItemsStore: TeacherListItemsStore,
    public readonly teacherMultipleSelectStore: TeacherlListItemsMultipleSelectStore,
    @Inject(MAT_DIALOG_DATA) public readonly data: SchoolAdminsDialogData,
    private readonly _appEvents: AppEventProvider,
  ) {
    super(data);
  }

  ngOnInit(): void {
    this.teacherListItemsStore.attachSearchText(
      this.adminsCtrl.valueChanges.pipe(
        map(x => x as string | null | undefined ?? ''),
        debounce(() => interval(debounceTimeMs)),
        distinctUntilChanged(),
      )
    );

    this.teacherListItemsStore.bind({ schoolId: this.data.settings.schoolId });
    this.teacherListItemsStore.attachUnavailableItems(this.teacherMultipleSelectStore.selectedItems$);

    this.teacherMultipleSelectStore.attachItems(this.teacherListItemsStore.items$);
    this.teacherMultipleSelectStore.setValues(this.data.teachers ?? []);

    this.pushSubscribtions(
      this.teacherMultipleSelectStore.values$.subscribe(x => {
        this._selectedAdmins = x;
      })
    );
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) {
      return;
    }

    this.data.teachers = this._selectedAdmins;
    this.close(DialogAction.save, this.data);
  }

  resetAdminsInput(event: MatChipInputEvent): void {
    if (this.adminsAuto?.showPanel) {
      return;
    }

    const { input } = event;
    if (input) {
      input.value = '';
    }

    this._appEvents.errorEmit(this.strings.mustSelectValue);
    this.adminsCtrl.setValue(null);
  }

  clearAdminsInput(): void {
    const input = this.adminsInput?.nativeElement;
    if (input) {
      input.value = '';
    }

    this.adminsCtrl.setValue(null);
  }

  onTeacherSelected($event: MatAutocompleteSelectedEvent): void {
    this.teacherMultipleSelectStore.setValue($event.option.value);
    this.clearAdminsInput();
  }
}
