import { Component, HostBinding, NgZone } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import deepEqual from 'deep-equal';
import { removeTags } from 'src/app/share/helpers/regex-helper';

import { StringsService } from '../../../strings.service';

export interface QuizNameCellRendererComponentParams extends ICellRendererParams {
  readonly quizName: string;
}

@Component({
  selector: 'app-ag-grid-quiz-name-cell',
  templateUrl: './quiz-name-cell-renderer.component.html',
  styleUrls: ['./quiz-name-cell-renderer.component.scss'],
})
export class QuizNameCellRendererComponent implements ICellRendererAngularComp {
  params: QuizNameCellRendererComponentParams | undefined;
  @HostBinding('style.width') public width = '100%';

  public safeValue: SafeHtml = "";
  constructor(private readonly _ngZone: NgZone, private readonly snitizer: DomSanitizer, public readonly strings: StringsService) {
  }

  refresh(params: ICellRendererParams): boolean {
    this.safeValue = this.snitizer.bypassSecurityTrustHtml(params.value);
    return !deepEqual(this.params, params);
  }

  agInit(params: QuizNameCellRendererComponentParams): void {
    this.params = params;
    this.safeValue = this.snitizer.bypassSecurityTrustHtml(removeTags(params.value));
  }

  onClick(): void {
  }
}
