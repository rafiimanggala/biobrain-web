import { AfterViewInit, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { ThemeService } from '../../../core/app/theme.service';
import { firstValueFrom } from '../../helpers/first-value-from';
import { PolygonHelper } from '../../helpers/polygone.helper';
import { StringsService } from '../../strings.service';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Point } from '../biobrain-canvas/models/point.model';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';

@Component({
  selector: 'app-alphabet',
  templateUrl: './alphabet.component.html',
  styleUrls: ['./alphabet.component.scss']
})
export class AlphabetComponent extends BaseComponent implements AfterViewInit {
  private _selectedLetter: string | null | undefined;

  @Output() letterChange = new EventEmitter<string>();
  @Input() set letter(value: string | null | undefined) {
    const needReDraw = this._selectedLetter !== value;
    this._selectedLetter = value;

    if (needReDraw) {
      setTimeout(() => {
        void this.Draw();
      }, 0);
    }
  }

  private get _baseWidth() {
    return 445 * this.dpr;
  }
  private get _baseHeight() {
    return 600 * this.dpr;
  }
  private readonly _maxCoefficient = 1.2;
  private _koef = 0.55;
  private readonly padding = 2;
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

  private _xCoords: number[] = [];
  private _yCoords: number[] = [];
  private _hexagones: Polygon[] = [];

  get dpr() {
    return this.canvas?.dpr ?? 1;
  }

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;

  constructor(
    private readonly _strings: StringsService,
    private readonly _themeService: ThemeService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);
  }

  async ngAfterViewInit(): Promise<void> {
    await this.Draw();
  }

  async Draw(): Promise<void> {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }

    const { primary, accent } = await firstValueFrom(this._themeService.colors$);
    const hCoeff = (this.canvas.height - this.padding) / this._baseHeight;
    const wCoeff = (this.canvas.width - this.padding) / this._baseWidth;
    const coeff = Math.min(wCoeff, hCoeff);
    this._koef = Math.min(Math.max(coeff, this._koef), this._maxCoefficient);
    const width = this._hexWidth * 6 + this._d * 5;
    const height = this._hexHeight * 7 + this._d * 8;
    this._drawRect = new Rect((this.canvas.width - width) / 2, (this.canvas.height - height) / 2, width, height);
    this.createCoordinates();

    const rects = this.getRects();
    this._hexagones = [];
    for (let i = 65; i < 91; i++) {
      const letter = String.fromCharCode(i);
      const fillColor = letter === this._selectedLetter ? accent : primary;
      const polygone = this.canvas.drawHexagone(rects[i - 65], fillColor, primary, 0, true);
      polygone.refData = letter;
      this._hexagones.push(polygone);
      this.canvas.drawText(polygone.refData, rects[i - 65], Colors.white, this._fontSize, true);
    }
  }

  onCanvasClick(event: MouseEvent) {
    const polygon = PolygonHelper.findPolygone(new Point(event.offsetX * this.dpr, event.offsetY * this.dpr), this._hexagones);
    if (!polygon) return;

    this._selectedLetter = polygon.refData;
    this.letterChange.emit(polygon.refData);
    void this.Draw();
  }

  private createCoordinates() {
    this._xCoords = [];
    this._yCoords = [];
    for (var i = 0; i < 11; i++) {
      this._xCoords.push(this._drawRect.x + (this._hexWidth + this._d) * i / 2);
    }
    for (var i = 0; i < 9; i++) {
      this._yCoords.push(this._drawRect.y + (this._hexHeight * 0.75 + this._d) * i);
    }
  }

  private getRects(): Rect[] {
    return [
      // A
      new Rect(this._xCoords[5], this._yCoords[0], this._hexWidth, this._hexHeight),
      // B
      new Rect(this._xCoords[4], this._yCoords[1], this._hexWidth, this._hexHeight),
      // C
      new Rect(this._xCoords[6], this._yCoords[1], this._hexWidth, this._hexHeight),
      // D
      new Rect(this._xCoords[8], this._yCoords[1], this._hexWidth, this._hexHeight),
      // E
      new Rect(this._xCoords[3], this._yCoords[2], this._hexWidth, this._hexHeight),
      // F
      new Rect(this._xCoords[5], this._yCoords[2], this._hexWidth, this._hexHeight),
      // G
      new Rect(this._xCoords[7], this._yCoords[2], this._hexWidth, this._hexHeight),
      // H
      new Rect(this._xCoords[6], this._yCoords[3], this._hexWidth, this._hexHeight),
      // I
      new Rect(this._xCoords[1], this._yCoords[4], this._hexWidth, this._hexHeight),
      // J
      new Rect(this._xCoords[3], this._yCoords[4], this._hexWidth, this._hexHeight),
      // K
      new Rect(this._xCoords[7], this._yCoords[4], this._hexWidth, this._hexHeight),
      // L
      new Rect(this._xCoords[9], this._yCoords[4], this._hexWidth, this._hexHeight),
      // M
      new Rect(this._xCoords[0], this._yCoords[5], this._hexWidth, this._hexHeight),
      // N
      new Rect(this._xCoords[2], this._yCoords[5], this._hexWidth, this._hexHeight),
      // O
      new Rect(this._xCoords[4], this._yCoords[5], this._hexWidth, this._hexHeight),
      // P
      new Rect(this._xCoords[6], this._yCoords[5], this._hexWidth, this._hexHeight),
      // Q
      new Rect(this._xCoords[8], this._yCoords[5], this._hexWidth, this._hexHeight),
      // R
      new Rect(this._xCoords[1], this._yCoords[6], this._hexWidth, this._hexHeight),
      // S
      new Rect(this._xCoords[3], this._yCoords[6], this._hexWidth, this._hexHeight),
      // T
      new Rect(this._xCoords[5], this._yCoords[6], this._hexWidth, this._hexHeight),
      // U
      new Rect(this._xCoords[7], this._yCoords[6], this._hexWidth, this._hexHeight),
      // V
      new Rect(this._xCoords[6], this._yCoords[7], this._hexWidth, this._hexHeight),
      // W
      new Rect(this._xCoords[8], this._yCoords[7], this._hexWidth, this._hexHeight),
      // X
      new Rect(this._xCoords[10], this._yCoords[7], this._hexWidth, this._hexHeight),
      // Y
      new Rect(this._xCoords[7], this._yCoords[8], this._hexWidth, this._hexHeight),
      // Z
      new Rect(this._xCoords[9], this._yCoords[8], this._hexWidth, this._hexHeight)
    ];
  }
}

