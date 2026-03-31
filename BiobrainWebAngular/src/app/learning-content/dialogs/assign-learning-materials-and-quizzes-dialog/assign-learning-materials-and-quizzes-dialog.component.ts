import { Component, Inject, OnInit } from '@angular/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import moment, { Moment } from 'moment';
import { Observable, of } from 'rxjs';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { LearningMaterialsService } from '../../../core/services/learning-materials/learning-materials.service';
import { LearningMaterial } from '../../../core/services/learning-materials/learning.material';
import { Quiz } from '../../../core/services/quizzes/quiz';
import { QuizzesService } from '../../../core/services/quizzes/quizzes.service';
import { SchoolClassCacheModel } from '../../../core/services/school-classes/school-class';
import { SchoolClassesCacheService } from '../../../core/services/school-classes/school-classes.service';
import { StudentsService } from '../../../core/services/students/students.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';
import { StringsService } from '../../../share/strings.service';

import { AssignLearningMaterialsAndQuizzesDialogData } from './assign-learning-materials-and-quizzes-dialog.data';
import { AssignLearningMaterialsAndQuizzesDialogResult } from './assign-learning-materials-and-quizzes-dialog.result';

export const MY_FORMATS = {
  parse: {
    dateInput: 'LL',
  },
  display: {
    dateInput: 'DD MMM YY',
    monthYearLabel: 'YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'YYYY',
  },
};

type StudentModel = { studentId: string; fullName: string; checked: boolean };

type LearningMaterialSelectionModel = { nodeId: string; fullName: string; included: boolean };

type ClassStudents = Array<{ schoolClass: SchoolClassCacheModel | undefined; students: StudentModel[] }>;

@Component({
  selector: 'app-assign-learning-materials-and-quizzes-dialog',
  templateUrl: './assign-learning-materials-and-quizzes-dialog.component.html',
  styleUrls: ['./assign-learning-materials-and-quizzes-dialog.component.scss'],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ]
})
export class AssignLearningMaterialsAndQuizzesDialog extends
  DialogComponent<AssignLearningMaterialsAndQuizzesDialogData, AssignLearningMaterialsAndQuizzesDialogResult> implements OnInit {
  public schoolClasses$: Observable<Map<string, SchoolClassCacheModel> | undefined>;
  public learningMaterials$: Observable<LearningMaterial[]>;
  public quizzes$: Observable<Quiz[]>;

  public dueDate: Moment = moment().add(7, 'days').startOf('day');
  public hintsEnabled = true;
  public soundEnabled = true;
  public learningMaterialSelections: LearningMaterialSelectionModel[] = [];
  public studentsMap: ClassStudents = [];

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: AssignLearningMaterialsAndQuizzesDialogData,
    private readonly _schoolClassesService: SchoolClassesCacheService,
    private readonly _studentsService: StudentsService,
    private readonly _quizzesService: QuizzesService,
    private readonly _learningMaterialsService: LearningMaterialsService
  ) {
    super(data);

    this.schoolClasses$ = hasValue(this.dialogData.schoolClassIds)
      ? this._schoolClassesService
        .getByIds(this.dialogData.schoolId, this.dialogData.schoolClassIds)
        .pipe(toDictionary(_ => _.schoolClassId))
      : of(undefined);

    this.quizzes$ = this._quizzesService.getByIds(this.dialogData.quizIds);
    this.learningMaterials$ = this._learningMaterialsService.getByIds(this.dialogData.learningMaterialIds);
  }

  async ngOnInit(): Promise<void> {
    if (this.dialogData.learningMaterialIds.length > 0) {
      const materials = await firstValueFrom(this.learningMaterials$);
      this.learningMaterialSelections = materials.map(m => ({
        nodeId: m.row.nodeId,
        fullName: m.fullName,
        included: true,
      }));
    }

    if (hasValue(this.dialogData.schoolClassIds)) {
      const classes = toNonNullable(await firstValueFrom(this.schoolClasses$));
      this.studentsMap = await Promise.all(
        this.dialogData.schoolClassIds.map(
          async classId => ({
            schoolClass: toNonNullable(classes.get(classId)),
            students: (await firstValueFrom(this._studentsService.getForClassFromCache(this.dialogData.schoolId, classId)))
              .map(student => ({
                studentId: student.studentId,
                fullName: student.fullName,
                checked: true
              }))
          })
        )
      );
    } else {
      const students$ = this._studentsService.getByIds(this.dialogData.schoolId, this.dialogData.studentIdList);
      const students = (await firstValueFrom(students$)).map(student => ({
        studentId: student.studentId,
        fullName: student.fullName,
        checked: true,
      }));
      this.studentsMap = [{ schoolClass: undefined, students }];
    }
  }

  onClose(): void {
    this.close();
  }

  onSubmit(): void {
    const selectedMaterialIds = this.learningMaterialSelections
      .filter(m => m.included)
      .map(m => m.nodeId);

    const result = new AssignLearningMaterialsAndQuizzesDialogResult(
      this._getSelectedStudents(),
      this.dueDate.endOf('day'),
      this.hintsEnabled,
      this.soundEnabled,
      selectedMaterialIds,
    );
    this.close(DialogAction.save, result);
  }

  unselectAll(schoolClassId: string | undefined): void {
    this._findStudents(schoolClassId).forEach(s => s.checked = false);
  }

  selectAll(schoolClassId: string | undefined): void {
    this._findStudents(schoolClassId).forEach(s => s.checked = true);
  }

  checkboxName(cls: SchoolClassCacheModel|undefined, student: StudentModel): string {
    const classId = cls?.schoolClassId ?? 'unassigned';
    return `chk-${classId}-${student.studentId}`;
  }

  private _findStudents(schoolClassId: string | undefined): StudentModel[] {
    if (schoolClassId === undefined) {
      return this.studentsMap[0].students;
    }
    const item = this.studentsMap.find(x => x.schoolClass?.schoolClassId === schoolClassId);
    return toNonNullable(item?.students);
  }


  private _getSelectedStudents(): Record<string, string[]> {
    const result = {} as Record<string, string[]>;

    const excludeUndefined = hasValue(this.dialogData.schoolClassIds);

    for (const item of this.studentsMap) {
      const { schoolClass } = item;
      if (!schoolClass && excludeUndefined) {
        continue;
      }

      const checkedIds = item.students
        .filter(student => student.checked)
        .map(student => student.studentId);

      if (checkedIds.length > 0) {
        const key = schoolClass?.schoolClassId ?? '';
        result[key] = checkedIds;
      }
    }

    return result;
  }
}

