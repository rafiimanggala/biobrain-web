import { TitleCasePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import {
  CourseImportItem,
  ImportCoursesCommand,
  ImportCoursesCommand_Result
} from 'src/app/api/courses/import-courses.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';

interface ParsedRow {
  row: number;
  data: CourseImportItem | null;
  valid: boolean;
  error: string;
}

@Component({
  selector: 'app-course-import',
  templateUrl: './course-import.component.html',
  styleUrls: ['./course-import.component.scss'],
})
export class CourseImportComponent implements OnInit, OnDestroy {

  subscriptions: Subscription[] = [];
  fileName: string = '';
  parsedRows: ParsedRow[] = [];
  importResult: ImportCoursesCommand_Result | null = null;
  errorMessage: string = '';
  parseError: string = '';

  readonly csvHeaders = [
    'SubjectCode', 'CurriculumCode', 'Year', 'SubHeader',
    'Postfix', 'IsForSell', 'IsBase', 'CourseGroup'
  ];

  readonly displayedColumns = [
    'row', 'subjectCode', 'curriculumCode', 'year', 'subHeader',
    'postfix', 'isForSell', 'isBase', 'courseGroup', 'status'
  ];

  constructor(
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService,
  ) {}

  ngOnInit(): void {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(
        this._titlecasePipe.transform('Course Import')
      );
    }, 0);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0] as File;
    if (!file) return;

    this.fileName = file.name;
    this.clearResults();

    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.parseCsv(e.target.result);
    };
    reader.readAsText(file);
  }

  private parseCsv(content: string): void {
    this.parsedRows = [];
    this.parseError = '';

    const lines = content
      .split(/\r?\n/)
      .map(l => l.trim())
      .filter(l => l.length > 0);

    if (lines.length < 2) {
      this.parseError = 'CSV must have a header row and at least one data row.';
      return;
    }

    const headerLine = lines[0];
    const headers = headerLine.split(',').map(h => h.trim().toLowerCase());

    const expectedLower = this.csvHeaders.map(h => h.toLowerCase());
    const missing = expectedLower.filter(h => !headers.includes(h));
    if (missing.length > 0) {
      this.parseError = `Missing columns: ${missing.join(', ')}`;
      return;
    }

    const colIndex: Record<string, number> = {};
    expectedLower.forEach(h => {
      colIndex[h] = headers.indexOf(h);
    });

    for (let i = 1; i < lines.length; i++) {
      const cols = this.splitCsvLine(lines[i]);
      const row: ParsedRow = {
        row: i + 1,
        data: null,
        valid: true,
        error: ''
      };

      try {
        const subjectCode = parseInt(cols[colIndex['subjectcode']], 10);
        const curriculumCode = parseInt(cols[colIndex['curriculumcode']], 10);
        const year = parseInt(cols[colIndex['year']], 10);
        const courseGroup = parseInt(cols[colIndex['coursegroup']], 10);

        if (isNaN(subjectCode) || isNaN(curriculumCode) || isNaN(year)) {
          row.valid = false;
          row.error = 'SubjectCode, CurriculumCode, and Year must be numbers.';
        }

        row.data = {
          subjectCode,
          curriculumCode,
          year,
          subHeader: (cols[colIndex['subheader']] || '').trim(),
          postfix: (cols[colIndex['postfix']] || '').trim(),
          isForSell: this.parseBool(cols[colIndex['isforsell']]),
          isBase: this.parseBool(cols[colIndex['isbase']]),
          courseGroup: isNaN(courseGroup) ? 0 : courseGroup
        };
      } catch {
        row.valid = false;
        row.error = 'Failed to parse row.';
      }

      this.parsedRows = [...this.parsedRows, row];
    }
  }

  private splitCsvLine(line: string): string[] {
    const result: string[] = [];
    let current = '';
    let inQuotes = false;

    for (let i = 0; i < line.length; i++) {
      const char = line[i];
      if (char === '"') {
        inQuotes = !inQuotes;
      } else if (char === ',' && !inQuotes) {
        result.push(current.trim());
        current = '';
      } else {
        current += char;
      }
    }
    result.push(current.trim());
    return result;
  }

  private parseBool(value: string): boolean {
    if (!value) return false;
    const lower = value.trim().toLowerCase();
    return lower === 'true' || lower === '1' || lower === 'yes';
  }

  clearFile(): void {
    this.fileName = '';
    this.parsedRows = [];
    this.clearResults();
    this.parseError = '';
  }

  private clearResults(): void {
    this.importResult = null;
    this.errorMessage = '';
  }

  get validRowCount(): number {
    return this.parsedRows.filter(r => r.valid).length;
  }

  get invalidRowCount(): number {
    return this.parsedRows.filter(r => !r.valid).length;
  }

  get canSubmit(): boolean {
    return this.validRowCount > 0 && !this.parseError;
  }

  async submitImport(): Promise<void> {
    if (!this.canSubmit) return;

    this.clearResults();

    const items = this.parsedRows
      .filter(r => r.valid && r.data !== null)
      .map(r => r.data) as CourseImportItem[];

    try {
      this._loaderService.show();
      this.importResult = await firstValueFrom(
        this._api.send(new ImportCoursesCommand(items))
      );
    } catch (e: any) {
      this.errorMessage = e.message || 'An error occurred during import.';
      this._appEvents.errorEmit(e.message);
    } finally {
      this._loaderService.hide();
    }
  }

  get sampleCsv(): string {
    return `${this.csvHeaders.join(',')}\n1,1,2024,Advanced,AP,true,false,1\n2,1,2025,Standard,,false,true,0`;
  }

  copySample(): void {
    const textarea = document.createElement('textarea');
    textarea.value = this.sampleCsv;
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
  }
}
