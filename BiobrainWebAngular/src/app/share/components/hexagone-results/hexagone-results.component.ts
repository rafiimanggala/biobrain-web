import { AfterViewInit, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { PolygonHelper } from '../../helpers/polygone.helper';
import { StringsService } from '../../strings.service';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Point } from '../biobrain-canvas/models/point.model';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';

import { QuestionResult } from './models/question-result.model';

@Component({
  selector: 'app-hexagone-results',
  templateUrl: './hexagone-results.component.html',
  styleUrls: ['./hexagone-results.component.scss']
})
export class HexagoneResultsComponent extends BaseComponent implements AfterViewInit {

  @Output() questionSelected: EventEmitter<string> = new EventEmitter();

  private get _cols() {
    const count = this.results.length;
    if (count <= 10) return 4;
    if (count <= 20) return 5;
    if (count <= 30) return 6;
    return 8;
  }

  private get _hexScale() {
    const count = this.results.length;
    if (count <= 10) return 1.0;
    if (count <= 20) return 0.8;
    if (count <= 30) return 0.65;
    return 0.5;
  }

  private get _baseWidth() {
    return (this._cols * 75 * this._hexScale + 30) * this.dpr;
  }
  private get _baseHeight() {
    const rows = Math.ceil(this.results.length / this._cols);
    return (rows * 70 * this._hexScale + 50) * this.dpr;
  }
  private readonly _maxCoefficient = 0.9;
  private _koef = 0.6;
  private _drawRect = new Rect(0, 0, 0, 0);

  private get _hexWidth() {
    return 70.0 * this._hexScale * this._koef * this.dpr;
  }
  private get _hexHeight() {
    return 80.0 * this._hexScale * this._koef * this.dpr;
  }
  private get _d() {
    return 5.0 * this._hexScale * this._koef * this.dpr;
  }
  private get _fontSize() {
    return 18 * this._hexScale * this._koef * this.dpr;
  }

  get dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private _hexagones: Polygon[] = [];

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;

  @Input() results: QuestionResult[] = [];

  constructor(
    appEvents: AppEventProvider
  ) {
    super(appEvents);
  }

  ngAfterViewInit(): void {
    this.Draw();

  }

  Draw() {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }

    const hCoeff = this.canvas.height / this._baseHeight;
    const wCoeff = this.canvas.width / this._baseWidth;
    const coeff = Math.min(wCoeff, hCoeff);
    this._koef = Math.min(Math.max(coeff, this._koef), this._maxCoefficient);
    const cols = this._cols;
    const rows = Math.ceil(this.results.length / cols);
    const width = this._hexWidth * cols + this._d * (cols - 1);
    const height = this._hexHeight * (0.75 * rows + 0.25) + this._d * (rows - 1);
    this._drawRect = new Rect((this.canvas.width - width) / 2, (this.canvas.height - height) / 2, width, height);

    this._hexagones = [];
    const cols = this._cols;
    for (let i = 0; i < this.results.length; i++) {
      const row = Math.floor(i / cols);
      const col = i % cols;
      const isOffsetRow = row % 2 === 1;
      const dx = (col + (isOffsetRow ? 0.5 : 0)) * (this._hexWidth + this._d);
      const dy = row * (0.75 * this._hexHeight + this._d);

      const result = this.results[i];
      const hexagonRect = new Rect(dx + this._drawRect.x, dy + this._drawRect.y, this._hexWidth, this._hexHeight);
      const hexagone = this.canvas.drawHexagone(hexagonRect, result.color, result.color, 1, true);
      hexagone.refData = result.questionId;
      this._hexagones.push(hexagone);
      this.canvas.drawText(result.name, hexagonRect, Colors.white, this._fontSize);
    }
  }

  onCanvasClick(event: MouseEvent) {
    const polygon = PolygonHelper.findPolygone(new Point(event.offsetX * this.dpr, event.offsetY * this.dpr), this._hexagones);
    if (!polygon) return;
    this.questionSelected.emit(polygon.refData);
  }

}

