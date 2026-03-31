import { Injectable } from '@angular/core';
import { Api } from 'src/app/api/api.service';
import { DisableAutoMapCommand } from 'src/app/api/content/disable-automap.command';
import { GetAutoMapOptionsQuery } from 'src/app/api/content/get-auto-map-options.query';
import { GetCourseQuizzesListQuery, GetCourseQuizzesListQuery_Result } from 'src/app/api/content/get-course-quizzes-list.query';
import { GetCoursesListQuery_Result } from 'src/app/api/content/get-courses-list.query';
import { UpdateAutoMapOptionsCommand } from 'src/app/api/content/update-auto-map-options.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { UnprocessableEntityException } from 'src/app/core/exceptions/unprocessable-entity.exception';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { FailedOrSuccessResult, Result } from 'src/app/share/helpers/result';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { AutoMapOptionsDialogData } from '../dialogs/auto-map-options-dialog/auto-map-options-dialog-data';
import { AutoMapOptionsDialogComponent } from '../dialogs/auto-map-options-dialog/auto-map-options-dialog.component';
import { SelectCourseDialogData } from '../dialogs/select-course-dialog/select-course-dialog-data';
import { SelectCourseDialogComponent } from '../dialogs/select-course-dialog/select-course-dialog.component';



@Injectable({
  providedIn: 'root',
})
export class ManageAutoMapOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService
  ) {
  }

  public async perform(quizId:string, parentId: string, baseCourses: GetCoursesListQuery_Result[], subjectCode: number, isAutoMapped:boolean): Promise<FailedOrSuccessResult> {

    var baseCourseId = "";
    var baseQuizId: string|undefined = undefined;  
    if(isAutoMapped){
      // Get automap options from server
      try {
        this._loaderService.show();
        let options = await firstValueFrom(this._api.send(new GetAutoMapOptionsQuery(quizId)));
        baseCourseId = options.baseCourseId;
        baseQuizId = options.baseQuizId;
      } 
      catch (e) {
        if(e instanceof RequestValidationException){
          this._appEvents.errorEmit(validationExceptionToString(e));
          return Result.failed();
        }
        console.log(e);
        this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
      }
      finally{      
        this._loaderService.hide();
      }
    }
    else{
      let dialogResult = await this._dialog.show(SelectCourseDialogComponent, new SelectCourseDialogData(baseCourses, baseCourses.find(_ => _.subjectCode == subjectCode)?.courseId));
      if(dialogResult.action != DialogAction.next || !dialogResult.hasData()){
        return Result.failed();
      }
      baseCourseId = dialogResult.data.courseId;
    }
    
    if(!baseCourseId) return Result.failed();

    // Get quizzes list for course
    var quizzes: GetCourseQuizzesListQuery_Result[] = [];
    try {
      this._loaderService.show();
      quizzes = await firstValueFrom(this._api.send(new GetCourseQuizzesListQuery(baseCourseId)));
    } 
    catch (e) {
      if(e instanceof RequestValidationException){
        this._appEvents.errorEmit(validationExceptionToString(e));
        return Result.failed();
      }
      console.log(e);
      this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
    }
    finally{      
      this._loaderService.hide();
    }

    // Open dialog
    let dialogResult = await this._dialog.show(AutoMapOptionsDialogComponent, new AutoMapOptionsDialogData(quizId, quizzes, baseQuizId, !isAutoMapped)); 
    if(dialogResult.action == DialogAction.delete){
      // Stop automap
      try {
        this._loaderService.show();
        await firstValueFrom(this._api.send(new DisableAutoMapCommand(quizId)));
      } 
      catch (e) {
        if(e instanceof RequestValidationException){
          this._appEvents.errorEmit(validationExceptionToString(e));
          return Result.failed();
        }
        console.log(e);
        this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
      }
      finally{      
        this._loaderService.hide();
      }
      return Result.success();
    } 
    if(dialogResult.action != DialogAction.save || !dialogResult.hasData() || !dialogResult.data.selectedBaseQuizId){
      return Result.failed();
    }
    baseQuizId = dialogResult.data.selectedBaseQuizId;

    try {      
      this._loaderService.show();
      await firstValueFrom(this._api.send(new UpdateAutoMapOptionsCommand(quizId, baseQuizId, parentId)));
      return Result.success();
    } 
    catch (e) {
      if(e instanceof RequestValidationException){
        this._appEvents.errorEmit(validationExceptionToString(e));
        return Result.failed();
      }
      console.log(e);
      this._appEvents.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
    }
    finally{
      this._loaderService.hide();
    }

    return Result.failed();
  }
}
