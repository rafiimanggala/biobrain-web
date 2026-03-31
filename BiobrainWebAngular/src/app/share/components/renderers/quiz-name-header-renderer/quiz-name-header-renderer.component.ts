import { Component, HostBinding, NgZone } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { IHeaderAngularComp } from 'ag-grid-angular';
import { IHeaderParams } from 'ag-grid-community/dist/lib/headerRendering/header/headerComp';
import deepEqual from 'deep-equal';

import { StringsService } from '../../../strings.service';

export interface QuizNameHeaderRendererComponentParams extends IHeaderParams {
  readonly handleClick: (params: IHeaderParams) => void;
  readonly quizName: string;
  readonly index: number;
}

@Component({
  selector: 'app-ag-grid-quiz-name-header',
  templateUrl: './quiz-name-header-renderer.component.html',
  styleUrls: ['./quiz-name-header-renderer.component.scss'],
})
export class QuizNameHeaderRendererComponent implements IHeaderAngularComp {
  params: QuizNameHeaderRendererComponentParams | undefined;
  @HostBinding('style.width') public width = '100%';

  public safeValue: SafeHtml = "";

  // public unitIndex?: string;
  // public aosIndex?: string;
  // public keyKnowledgeName?: string;
  // public topicName?: string;
  // public levelIndex?: string;

  constructor(private readonly _ngZone: NgZone, private readonly snitizer: DomSanitizer, public readonly strings: StringsService) {
  }

  refresh(params: IHeaderParams): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: QuizNameHeaderRendererComponentParams): void {
    this.params = params;
    this.safeValue = this.snitizer.bypassSecurityTrustHtml(params.quizName)
    console.log(params);
    //this._setPathParts(params);
  }

  onClick(): void {
    this._ngZone.run(() => this.params?.handleClick(this.params));
  }

  // private _setPathParts(params: QuizNameHeaderRendererComponentParams): void {
  //   // if (!hasValue(params.path) || params.path.length < 5) return;

  //   this.unitIndex = params.path[0];
  //   this.aosIndex = params.path[1];
  //   this.keyKnowledgeName = params.path[2];
  //   this.topicName = params.path[3];
  //   this.levelIndex = params.path[4];
  // }
}
