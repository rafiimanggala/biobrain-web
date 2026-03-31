import { AfterViewInit, Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { filter, switchMap, tap } from 'rxjs/operators';
import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { SchoolClassModel, SchoolClassService } from 'src/app/core/services/school-class/school-class.service';
import { StudentsService } from 'src/app/core/services/students/students.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { TeacherListItemsStore } from 'src/app/share/list-item-stores/teacher-list-items-store';
import { StringsService } from 'src/app/share/strings.service';
import { AddTeacherOperation } from '../../operations/add-teacher.operation';
import { EmailClassOperation } from '../../operations/email-class.operation';
import { InviteStudentOperation } from '../../operations/invite-student.operation';
import { OpenTeachersListOperation } from '../../operations/open-teachers-list.operation';
import { RenameClassOperation } from '../../operations/rename-class.operation';
import {SchoolClassChangesService} from "../../school-class-changes.service";

@Component({
  selector: 'app-class-admin-sidenav',
  templateUrl: './class-admin-sidenav.component.html',
  styleUrls: ['./class-admin-sidenav.component.scss'],
  providers:[TeacherListItemsStore]
})
export class ClassAdminSidenavComponent extends BaseComponent implements OnInit, AfterViewInit {

  schoolClass$: BehaviorSubject<SchoolClassModel | null> = new BehaviorSubject<SchoolClassModel | null>(null);
  schoolTeachers: GetTeacherListItemsQuery_Result[] = [];


  constructor(
    public readonly strings: StringsService,
    _userService: CurrentUserService,
    _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _schoolClassService: SchoolClassService,
    _studentService: StudentsService,
     _teacherListItemsStore: TeacherListItemsStore,
    private readonly _renameClassOperation: RenameClassOperation,
    private readonly _inviteStudentOperation: InviteStudentOperation,
    private readonly _emailClassOperation: EmailClassOperation,
    private readonly _addTeacherOperation: AddTeacherOperation,
    private readonly _openTeacherListOperation: OpenTeachersListOperation,
    private readonly _schoolClassChangesService: SchoolClassChangesService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);

    let schoolClass$ =  _activeSchoolClassService.schoolClassIdChanges$.pipe(
      filter(hasValue),
      switchMap((classId) => {
        return _schoolClassService.getById(classId);
      })
    );

    this.subscriptions.push(
      _teacherListItemsStore.items$.subscribe((_ => this.schoolTeachers = _))
    );

    this.subscriptions.push(
      schoolClass$.pipe(
        tap((schoolClass: SchoolClassModel) => {
          this.schoolClass$.next(schoolClass);
          _teacherListItemsStore.bind({schoolId: schoolClass.schoolId});
        })
      ).subscribe()
    );
  }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
  }

  async onRenameClass() {
    var schoolClass = this.schoolClass$.getValue();
    if (!schoolClass) return;

    var result = await this._renameClassOperation.perform(schoolClass.name, schoolClass.schoolClassId);
    if (result.isSuccess()) {
      this.schoolClass$.next(new SchoolClassModel(schoolClass.schoolClassId, schoolClass.schoolId, schoolClass.courseId, schoolClass.year, result.data, schoolClass.autoJoinClassCode, schoolClass.teacherIds, schoolClass.students));
    }
  }

  async onAddTeacher() {
    if(!this.schoolTeachers) return;

    var schoolClass = this.schoolClass$.getValue();
    if (!schoolClass) return;

    var teachersToSelect = this.schoolTeachers.filter(x => !schoolClass?.teacherIds.includes(x.teacherId));

    var result = await this._addTeacherOperation.perform(schoolClass.schoolClassId, teachersToSelect);
    if(result.isSuccess()){
      schoolClass.teacherIds.push(result.data);
      this.schoolClass$.next(schoolClass);
    }
   }

   async onTeachersList(){
    if(!this.schoolTeachers) return;

    var schoolClass = this.schoolClass$.getValue();
    if (!schoolClass) return;

    var teachersToSelect = this.schoolTeachers.filter(x => schoolClass?.teacherIds.includes(x.teacherId));
    await this._openTeacherListOperation.perform(teachersToSelect, schoolClass.schoolClassId);
    var updated = await this._schoolClassService.getById(schoolClass.schoolClassId).toPromise();
    this.schoolClass$.next(updated);
   }

  async onEmailClass() {
    var schoolClass = this.schoolClass$.getValue();
    if (!schoolClass) return;

    this._emailClassOperation.perform(schoolClass.schoolClassId);
  }

  public async onInviteStudentByEmail(): Promise<void> {
    const schoolClass = this.schoolClass$.getValue();
    if (!schoolClass) return;

    const result = await this._inviteStudentOperation.perform(schoolClass);
    if (result.isSuccess() && result.data) {
      const updated = await this._schoolClassService.getById(schoolClass.schoolClassId).toPromise();
      this._schoolClassChangesService.next(updated.schoolClassId);
    }
  }
}
