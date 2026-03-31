import { TitleCasePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Moment } from 'moment';
import { Subscription } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import { GetCourseReleasesListQuery, GetCourseReleasesListQuery_Result } from 'src/app/api/content/get-course-releases-list.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { GetContentStructureReportToCsv } from 'src/app/api/reports/get-content-structure-report-to-csv.query';
import moment from 'moment';

@Component({
  selector: 'app-content-report',
  templateUrl: './content-report.component.html',
  styleUrls: ['./content-report.component.scss'],
})
export class ContentReportComponent implements OnInit, OnDestroy {

  subscriptions: Subscription[] = [];
  courses: GetCourseReleasesListQuery_Result[] = [];
  fileName?: string;
  fileToUpload?: File;
  resultLog: string[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService
  ) { }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(this._titlecasePipe.transform(this.strings.contentReport));
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

  async getStructure(course: GetCourseReleasesListQuery_Result) {
    if (!course) return;

    try {
      var data = await firstValueFrom(this._api.send(new GetContentStructureReportToCsv(course.courseId)));
      console.log(`version: ${data.fileUrl}`);
      if (!data.fileUrl) return;
      const anchor = document.createElement('a');
      anchor.download = `Content-Structure_${course?.name ?? ""}_${moment().format('YYYY_MM_DD')}.csv`;
      anchor.href = data.fileUrl;
      anchor.click();
    }
    catch (e: any) {
      // this._loaderService.show();
      this._appEvents.errorEmit(e.message);
    }
    finally {
      // this._loaderService.hide();
    }
  }

  onFileSelected(event: any): void {
    if (event.target.files.length > 1) return;

    let file = event.target.files[0] as File;

    this.fileName = file.name;
    this.fileToUpload = file;
    console.log(this.fileToUpload.arrayBuffer);
  }
}

