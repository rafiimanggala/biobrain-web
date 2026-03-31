import { TitleCasePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import { GetCoursesListQuery, GetCoursesListQuery_Result } from 'src/app/api/content/get-courses-list.query';
import { ImportContentCommand, ImportContentCommand_Result } from 'src/app/api/content/import-content.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';

@Component({
  selector: 'app-content-import',
  templateUrl: './content-import.component.html',
  styleUrls: ['./content-import.component.scss'],
})
export class ContentImportComponent implements OnInit, OnDestroy {

  subscriptions: Subscription[] = [];
  courses: GetCoursesListQuery_Result[] = [];
  selectedCourseId: string = '';
  jsonContent: string = '';
  importResult: ImportContentCommand_Result | null = null;
  errorMessage: string = '';
  fileName: string = '';

  constructor(
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService
  ) {}

  async ngOnInit(): Promise<void> {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(
        this._titlecasePipe.transform('Content Import')
      );
    }, 0);
    await this.loadCourses();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  private async loadCourses(): Promise<void> {
    try {
      this._loaderService.show();
      this.courses = await firstValueFrom(this._api.send(new GetCoursesListQuery()));
    } catch (e: any) {
      this._appEvents.errorEmit(e.message);
    } finally {
      this._loaderService.hide();
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0] as File;
    if (!file) return;

    this.fileName = file.name;

    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.jsonContent = e.target.result;
    };
    reader.readAsText(file);
  }

  clearFile(): void {
    this.fileName = '';
    this.jsonContent = '';
    this.importResult = null;
    this.errorMessage = '';
  }

  get isFormValid(): boolean {
    return !!this.selectedCourseId && !!this.jsonContent.trim();
  }

  async importContent(): Promise<void> {
    if (!this.isFormValid) return;

    this.importResult = null;
    this.errorMessage = '';

    try {
      // Validate JSON before sending
      JSON.parse(this.jsonContent);
    } catch {
      this.errorMessage = 'Invalid JSON format. Please check your input.';
      return;
    }

    try {
      this._loaderService.show();
      this.importResult = await firstValueFrom(
        this._api.send(new ImportContentCommand(this.selectedCourseId, this.jsonContent))
      );
    } catch (e: any) {
      this.errorMessage = e.message || 'An error occurred during import.';
      this._appEvents.errorEmit(e.message);
    } finally {
      this._loaderService.hide();
    }
  }

  get sampleJson(): string {
    return JSON.stringify({
      topics: [{
        name: 'Topic Name',
        subtopics: [{
          name: 'Subtopic Name',
          materials: [{
            name: 'Material Title',
            content: '<p>HTML content here</p>'
          }],
          questions: [{
            header: 'Question header',
            text: 'Question text',
            type: 'MultipleChoice',
            hint: 'hint text',
            feedback: 'explanation',
            answers: [
              { text: 'Answer A', isCorrect: true },
              { text: 'Answer B', isCorrect: false }
            ]
          }]
        }]
      }]
    }, null, 2);
  }

  copySample(): void {
    const textarea = document.createElement('textarea');
    textarea.value = this.sampleJson;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
  }

  loadSample(): void {
    this.jsonContent = this.sampleJson;
    this.fileName = '';
  }
}
