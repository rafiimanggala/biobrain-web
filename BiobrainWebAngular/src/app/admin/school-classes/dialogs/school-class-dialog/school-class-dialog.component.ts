import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable, combineLatest, interval } from 'rxjs';
import { debounce, distinctUntilChanged, map, withLatestFrom } from 'rxjs/operators';
import { StringsService } from 'src/app/share/strings.service';

import { AppEventProvider } from '../../../../core/app/app-event-provider.service';
import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';
import { CoursesService } from '../../../../core/services/courses/courses.service';
import { TeacherlListItemsMultipleSelectStore } from '../../../../share/list-item-stores/teacher-list-items-multiple-select-store';
import { TeacherListItemsStore } from '../../../../share/list-item-stores/teacher-list-items-store';

import { SchoolClassDialogData } from './school-class-dialog-data';
import { Course } from 'src/app/core/services/courses/course';

@Component({
  selector: 'app-school-class-dialog',
  templateUrl: './school-class-dialog.component.html',
  styleUrls: ['./school-class-dialog.component.scss'],
  providers: [TeacherListItemsStore, TeacherlListItemsMultipleSelectStore]
})
export class SchoolClassDialog extends DialogComponent<SchoolClassDialogData, SchoolClassDialogData> implements OnInit {
  @ViewChild('teachersInput') adminsInput: ElementRef<HTMLInputElement> | undefined;
  @ViewChild('teachersAuto') adminsAuto: MatAutocomplete | undefined;
  teachersCtrl: FormControl;
  yearCtrl: FormControl;
  nameCtrl: FormControl;
  courseCtrl: FormControl;
  dialogForm: FormGroup;
  availableCourses: Course[] = [];

  readonly years = [9, 10, 11, 12];

  constructor(
    public readonly strings: StringsService,
    private readonly _builder: FormBuilder,
    public readonly coursesService: CoursesService,
    public readonly teacherListItemsStore: TeacherListItemsStore,
    public readonly teacherMultipleSelectStore: TeacherlListItemsMultipleSelectStore,
    @Inject(MAT_DIALOG_DATA) public readonly data: SchoolClassDialogData,
    private readonly _appEvents: AppEventProvider
  ) {
    super(data);

    this.yearCtrl = new FormControl(this.data.year);
    this.nameCtrl = new FormControl(this.data.name);
    this.courseCtrl = new FormControl(this.data.courseId);
    this.teachersCtrl = new FormControl();
    combineLatest([coursesService.items$, coursesService.getCoursesForSchool(this.dialogData.settings.schoolId)])
    .pipe(
      map(([courses, availableCourses]) => courses.filter(_ => availableCourses.some(ac => ac.courseId == _.courseId)))
    ).subscribe(_ => this.availableCourses = _);

    this.dialogForm = this._builder.group({
      yearCtrl: this.yearCtrl,
      nameCtrl: this.nameCtrl,
      courseCtrl: this.courseCtrl
    });

    this.teacherListItemsStore.attachSearchText(
      this.teachersCtrl.valueChanges.pipe(
        map(x => x as string | null | undefined ?? ''),
        debounce(() => interval(200)),
        distinctUntilChanged(),
      )
    );

    this.teacherListItemsStore.bind({ schoolId: this.data.settings.schoolId });
    this.teacherListItemsStore.attachUnavailableItems(this.teacherMultipleSelectStore.selectedItems$);

    this.teacherMultipleSelectStore.attachItems(this.teacherListItemsStore.items$);
    this.teacherMultipleSelectStore.setValues(this.data.teacherIds ?? []);

    this.pushSubscribtions(
      this.teacherMultipleSelectStore.values$.subscribe(x => {
        this.data.teacherIds = x;
      })
    );
  }

  ngOnInit(): void {
    this.pushSubscribtions(
      this.yearCtrl.valueChanges.subscribe(
        () => this.nameCtrl.updateValueAndValidity()
      ),

      this.yearCtrl.valueChanges.subscribe(
        x => this.data.year = Number(x)
      ),

      this.nameCtrl.valueChanges.subscribe(
        x => this.data.name = String(x)
      ),

      this.courseCtrl.valueChanges.subscribe(
        x => this.data.courseId = String(x)
      )
    );
  }

  resetTeachersInput(event: MatChipInputEvent): void {
    if (this.adminsAuto?.showPanel) {
      return;
    }

    const { input } = event;
    if (input) {
      input.value = '';
    }

    this._appEvents.errorEmit(this.strings.mustSelectValue);
    this.teachersCtrl.setValue(null);
  }

  clearTeachersInput(): void {
    const input = this.adminsInput?.nativeElement;
    if (input) {
      input.value = '';
    }

    this.teachersCtrl.setValue(null);
  }

  onTeacherSelected($event: MatAutocompleteSelectedEvent): void {
    this.teacherMultipleSelectStore.setValue($event.option.value);
    this.clearTeachersInput();
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: FormGroup): void {
    if (form.invalid) {
      return;
    }

    this.close(DialogAction.save, this.data);
  }
}

