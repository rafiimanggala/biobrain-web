import { formatDate, TitleCasePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Moment } from 'moment';
import { Subscription } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import { GetCourseReleasesListQuery, GetCourseReleasesListQuery_Result } from 'src/app/api/content/get-course-releases-list.query';
import { ImportCommand } from 'src/app/api/content/import.command';
import { PublishCourseCommand } from 'src/app/api/content/publish-course.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { MapCourseCommand } from 'src/app/api/content/map-intro.command';

@Component({
  selector: 'app-content-loader',
  templateUrl: './content-loader.component.html',
  styleUrls: ['./content-loader.component.scss'],
})
export class ContentLoaderComponent implements OnInit, OnDestroy {

  subscriptions: Subscription[] = [];
  courses: GetCourseReleasesListQuery_Result[] = [];
  fileName?: string;
  fileToUpload?: File;
  resultLog: string[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService
  ) { }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(this._titlecasePipe.transform(this.strings.contentLoader));
    }, 0);
    await this.getCoursesInternal();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  formatDate(dateUtc: Moment): string {
    return dateUtc.local().format("DD-MMM-YYYY");
  }

  private async getCoursesInternal() {
    try {
      this._loaderService.show();
      this.courses = await firstValueFrom(this._api.send(new GetCourseReleasesListQuery()));
    }
    catch (e: any) {
      this._appEvents.errorEmit(e.message);
    }
    finally {
      this._loaderService.hide();
    }
  }

  async publishCourse(course: GetCourseReleasesListQuery_Result) {
    if (!course) return;

    var result = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.publish, this.strings.publishConfirmation, this.strings.publish, this.strings.cancel));
    if (result.action != DialogAction.yes) return;

    try {
      var data = await firstValueFrom(this._api.send(new PublishCourseCommand(course.courseId)));
      console.log(`version: ${data.version}`);
    }
    catch (e: any) {
      // this._loaderService.show();
      this._appEvents.errorEmit(e.message);
    }
    finally {
      // this._loaderService.hide();
    }

    await this._dialog.show(InformationDialog, new InformationDialogData(this.strings.publish, this.strings.publishResult));
    await this.getCoursesInternal();
  }

  onFileSelected(event: any): void {
    if (event.target.files.length > 1) return;

    let file = event.target.files[0] as File;

    this.fileName = file.name;
    this.fileToUpload = file;
    console.log(this.fileToUpload.arrayBuffer);

    // if (typeof (FileReader) !== 'undefined') {
    //   const reader = new FileReader();

    //   reader.onload = (e: any) => {
    //     console.log(e);
    //     if(!this.fileToUpload) return;
    //     this.fileToUpload?.arrayBuffer = e.target.result;
    //   };

    //   reader.readAsArrayBuffer(this.fileToUpload);
    // }
  }

  async uploadFile() {
    if (!this.fileToUpload) return;
    try {
      var data = await firstValueFrom(this._api.sendFile(new ImportCommand(this.fileToUpload)));
      console.log(`CourseId: ${data}`);
    }
    catch (e: any) {
      // this._loaderService.show();
      this._appEvents.errorEmit(e.message);
    }
    finally {
      // this._loaderService.hide();
    }

    await this.getCoursesInternal();
  }

  async mapIntro(){
    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new MapCourseCommand()));
    }
    catch (e: any) {
      this._appEvents.errorEmit(e.message);
    }
    finally {
      this._loaderService.hide();
    }
  }
}

