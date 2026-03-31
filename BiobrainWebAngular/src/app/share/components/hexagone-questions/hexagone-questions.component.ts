import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, SimpleChanges, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { PolygonHelper } from '../../helpers/polygone.helper';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Point } from '../biobrain-canvas/models/point.model';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';
import { ThemeService } from 'src/app/core/app/theme.service';
import { QuestionViewModel } from './models/question.model';

@Component({
  selector: 'app-hexagone-questions',
  templateUrl: './hexagone-questions.component.html',
  styleUrls: ['./hexagone-questions.component.scss']
})
export class HexagoneQuestionsComponent extends BaseComponent implements AfterViewInit {

  @Output() questionSelected: EventEmitter<string> = new EventEmitter();

  private readonly padding = 2;
  private get _baseWidth() {
    return 410 * this.dpr;
  }
  private get _baseHeight() {
    return 450 * this.dpr;
  }
  private readonly _maxCoefficient = 0.9;
  private _koef = 0.55
  private _drawRect = new Rect(0, 0, 0, 0);

  private get _hexWidth() {
    return 70.0 * this._koef * this.dpr;
  }
  private get _hexHeight() {
    return 80.0 * this._koef * this.dpr;
  }
  private get _d() {
    return 5.0 * this._koef * this.dpr;
  }
  private get _fontSize() {
    return 18 * this._koef * this.dpr;
  }

  get dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private _hexagones: Polygon[] = [];
  private _xCoords: number[] = [];
  private _yCoords: number[] = [];

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;
  @ViewChild('canvas') canvasElement?: ElementRef;

  @Input() questions: QuestionViewModel[] | null | undefined = [];

  constructor(
    appEvents: AppEventProvider,
    private readonly _themeService: ThemeService 
  ) {
    super(appEvents);
  }

  redraw(changes: SimpleChanges): void {
      this.Draw();
  }

  ngAfterViewInit(): void {
    this.Draw();

  }

  Draw() {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }
    if(!this.questions) return;

    // const hCoeff = (this.canvas.height - this.padding) / this._baseHeight;
    const wCoeff = (this.canvas.width) / this._baseWidth;
    // const coeff = Math.min(wCoeff, hCoeff);
    this._koef = Math.min(Math.max(wCoeff, this._koef), this._maxCoefficient);
    const width = this._hexWidth * 5.5 + this._d * 5;
    let rows = this.getRowsNumberByElementCount(this.questions.length);
    const height = this._hexHeight * rows*0.88 + this._d * (rows-1);
    if(this.canvas.canvas)
      this.canvas.canvas.nativeElement.height = height;
    
    this._drawRect = new Rect((this.canvas.width - width) / 2, (this.canvas.height - height) / 2, width, this.canvas.height);
    this.createCoordinates();
    const rects = this.getRects();

    this._hexagones = [];
    for (let i = 0; i < this.questions.length && i < rects.length; i++) {
      const result = this.questions[i];
      const hexagonRect = rects[i];
      let color = result.isExcluded ? "#dbdbdb" : this._themeService.colors?.primary;
      const hexagone = this.canvas.drawHexagone(hexagonRect,  color, color, 1, true);
      hexagone.refData = result.questionId;
      this._hexagones.push(hexagone);
      this.canvas.drawText(result.questionHeader, hexagonRect, Colors.white, this._fontSize);
    }
  }

  private createCoordinates() {
    this._xCoords = [];
    this._yCoords = [];
    for (var i = 0; i < 10; i++) {
      this._xCoords.push(this._drawRect.x + (this._hexWidth + this._d) * i / 2);
    }
    for (var i = 0; i < 14; i++) {
      this._yCoords.push(this._drawRect.y + (this._hexHeight * 0.75 + this._d) * i);
    }
  }

  private getRects(): Rect[] {
    return [
      // 1
      new Rect(this._xCoords[1], this._yCoords[0], this._hexWidth, this._hexHeight),
      // 2
      new Rect(this._xCoords[0], this._yCoords[1], this._hexWidth, this._hexHeight),
      // 3
      new Rect(this._xCoords[2], this._yCoords[1], this._hexWidth, this._hexHeight),
      // 4
      new Rect(this._xCoords[6], this._yCoords[1], this._hexWidth, this._hexHeight),
      // 5
      new Rect(this._xCoords[8], this._yCoords[1], this._hexWidth, this._hexHeight),
      // 6
      new Rect(this._xCoords[1], this._yCoords[2], this._hexWidth, this._hexHeight),
      // 7
      new Rect(this._xCoords[3], this._yCoords[2], this._hexWidth, this._hexHeight),
      // 8
      new Rect(this._xCoords[5], this._yCoords[2], this._hexWidth, this._hexHeight),
      // 9
      new Rect(this._xCoords[7], this._yCoords[2], this._hexWidth, this._hexHeight),
      // 10
      new Rect(this._xCoords[2], this._yCoords[3], this._hexWidth, this._hexHeight),
      // 11
      new Rect(this._xCoords[4], this._yCoords[3], this._hexWidth, this._hexHeight),
      // 12
      new Rect(this._xCoords[6], this._yCoords[3], this._hexWidth, this._hexHeight),
      // 13
      new Rect(this._xCoords[8], this._yCoords[3], this._hexWidth, this._hexHeight),
      // 14
      new Rect(this._xCoords[1], this._yCoords[4], this._hexWidth, this._hexHeight),
      // 15
      new Rect(this._xCoords[3], this._yCoords[4], this._hexWidth, this._hexHeight),
      // 16
      new Rect(this._xCoords[5], this._yCoords[4], this._hexWidth, this._hexHeight),
      // 17
      new Rect(this._xCoords[2], this._yCoords[5], this._hexWidth, this._hexHeight),
      // 18
      new Rect(this._xCoords[4], this._yCoords[5], this._hexWidth, this._hexHeight),
      // 19
      new Rect(this._xCoords[6], this._yCoords[5], this._hexWidth, this._hexHeight),
      // 20
      new Rect(this._xCoords[1], this._yCoords[6], this._hexWidth, this._hexHeight),
      // 21
      new Rect(this._xCoords[3], this._yCoords[6], this._hexWidth, this._hexHeight),
      // 22
      new Rect(this._xCoords[5], this._yCoords[6], this._hexWidth, this._hexHeight),
      // 23
      new Rect(this._xCoords[7], this._yCoords[6], this._hexWidth, this._hexHeight),
      // 24
      new Rect(this._xCoords[9], this._yCoords[6], this._hexWidth, this._hexHeight),
      // 25
      new Rect(this._xCoords[0], this._yCoords[7], this._hexWidth, this._hexHeight),
      // 26
      new Rect(this._xCoords[2], this._yCoords[7], this._hexWidth, this._hexHeight),
      //27
      new Rect(this._xCoords[4], this._yCoords[7], this._hexWidth, this._hexHeight),
      //28
      new Rect(this._xCoords[8], this._yCoords[7], this._hexWidth, this._hexHeight),
      //29
      new Rect(this._xCoords[1], this._yCoords[8], this._hexWidth, this._hexHeight),
      //30
      new Rect(this._xCoords[3], this._yCoords[8], this._hexWidth, this._hexHeight),
      //31
      new Rect(this._xCoords[5], this._yCoords[8], this._hexWidth, this._hexHeight),
      //32
      new Rect(this._xCoords[7], this._yCoords[8], this._hexWidth, this._hexHeight),
      //33
      new Rect(this._xCoords[2], this._yCoords[9], this._hexWidth, this._hexHeight),
      //34
      new Rect(this._xCoords[4], this._yCoords[9], this._hexWidth, this._hexHeight),
      //35
      new Rect(this._xCoords[6], this._yCoords[9], this._hexWidth, this._hexHeight),
      //36
      new Rect(this._xCoords[8], this._yCoords[9], this._hexWidth, this._hexHeight),
      
      //37
      new Rect(this._xCoords[1], this._yCoords[10], this._hexWidth, this._hexHeight),
      //38
      new Rect(this._xCoords[5], this._yCoords[10], this._hexWidth, this._hexHeight),
      //39
      new Rect(this._xCoords[7], this._yCoords[10], this._hexWidth, this._hexHeight),
      //40
      new Rect(this._xCoords[9], this._yCoords[10], this._hexWidth, this._hexHeight),
      
      //41
      new Rect(this._xCoords[0], this._yCoords[11], this._hexWidth, this._hexHeight),
      //42
      new Rect(this._xCoords[2], this._yCoords[11], this._hexWidth, this._hexHeight),
      //43
      new Rect(this._xCoords[4], this._yCoords[11], this._hexWidth, this._hexHeight),
      //44
      new Rect(this._xCoords[6], this._yCoords[11], this._hexWidth, this._hexHeight),
      //45
      new Rect(this._xCoords[8], this._yCoords[11], this._hexWidth, this._hexHeight),
      
      //46
      new Rect(this._xCoords[1], this._yCoords[12], this._hexWidth, this._hexHeight),
      //47
      new Rect(this._xCoords[3], this._yCoords[12], this._hexWidth, this._hexHeight),
      //48
      new Rect(this._xCoords[7], this._yCoords[12], this._hexWidth, this._hexHeight),
      
      //49
      new Rect(this._xCoords[2], this._yCoords[13], this._hexWidth, this._hexHeight),
      //50
      new Rect(this._xCoords[4], this._yCoords[13], this._hexWidth, this._hexHeight),
    ];
  }
  private getRowsNumberByElementCount(count: number): number{
    if(count < 2) return 1;
    if(count < 6) return 2;
    if(count < 10) return 3;
    if(count < 14) return 4;
    if(count < 17) return 5;
    if(count < 20) return 6;
    if(count < 25) return 7;
    if(count < 29) return 8;
    if(count < 33) return 9;
    if(count < 37) return 10;
    if(count < 41) return 11;
    if(count < 46) return 12;
    if(count < 49) return 13;
    return 14;
  }

  onCanvasClick(event: MouseEvent) {
    const polygon = PolygonHelper.findPolygone(new Point(event.offsetX * this.dpr, event.offsetY * this.dpr), this._hexagones);
    if (!polygon) return;
    this.questionSelected.emit(polygon.refData);
  }

}

