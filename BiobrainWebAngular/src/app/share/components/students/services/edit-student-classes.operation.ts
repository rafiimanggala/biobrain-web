import { Injectable } from '@angular/core';
import { SelectClassesDialog } from 'src/app/admin/dialogs/select-classes-dialog/select-classes-dialog.component';
import { GetSchoolDataFromRouteService } from 'src/app/admin/services/get-school-data-from-route.service';
import { Api } from 'src/app/api/api.service';
import { GetStudentByIdQuery } from 'src/app/api/students/get-student-by-id.query';
import { GetStudentClassesQuery } from 'src/app/api/students/get-student-classes.query';
import { UpdateStudentClassesCommand } from 'src/app/api/students/update-student-classes.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';


@Injectable({
  providedIn: 'root'
})
export class EditStudentClassesOperation extends DisposableSubscriptionService {
  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
  ) {
    super();
  }

  async perform(studentId: string, schoolId: string): Promise<void> {
    const studentClasses = await firstValueFrom(this._api.send(new GetStudentClassesQuery(studentId, schoolId)));

    const schoolClassIds = studentClasses.map(_ => _.schoolClassId);
    const dialogData = {
      schoolId: schoolId,
      schoolClassIds,
    };
    const dialogResult = await this._dialog.show(SelectClassesDialog, dialogData);
    if (!dialogResult.hasData()) return;

    this._loaderService.show();
    try {
      await firstValueFrom(this._api.send(new UpdateStudentClassesCommand(studentId, schoolId, dialogResult.data.schoolClassIds)));
    } finally {
      this._loaderService.hide();
    }
  }
}
