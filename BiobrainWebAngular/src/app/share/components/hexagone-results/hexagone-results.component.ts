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

  private get _baseWidth() {
    return 295 * this.dpr;
  }
  private get _baseHeight() {
    return 210 * this.dpr;
  }
  private readonly _maxCoefficient = 0.9;
  private _koef = 0.6;
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
    const width = this._hexWidth * 4 + this._d * 3;
    const height = this._hexHeight * 2.5 + this._d * 2;
    this._drawRect = new Rect((this.canvas.width - width) / 2, (this.canvas.height - height) / 2, width, height);

    this._hexagones = [];
    for (let i = 0; i < this.results.length && i < 10; i++) {
      let dx: number;
      let dy: number;

      if (i > 2 && i < 7) {
        dx = (i - 3) * (this._hexWidth + this._d);
        dy = 0.75 * this._hexHeight + this._d;
      } else if (i < 3) {
        dx = (0.5 + i) * (this._hexWidth + this._d);
        dy = 0;
      } else {
        dx = (0.5 + i - 7) * (this._hexWidth + this._d);
        dy = 1.5 * this._hexHeight + 2 * this._d;
      }

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

