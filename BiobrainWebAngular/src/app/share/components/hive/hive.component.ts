import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { ThemeService } from '../../../core/app/theme.service';
import { firstValueFrom } from '../../helpers/first-value-from';
import { StringsService } from '../../strings.service';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent, DrawType } from '../biobrain-canvas/biobrain-canvas.component';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';

@Component({
  selector: 'app-hive',
  templateUrl: './hive.component.html',
  styleUrls: ['./hive.component.scss']
})
export class HiveComponent extends BaseComponent implements AfterViewInit {
  private _streak: number = 0;

  @Input() set streak(value: number | null | undefined) {
    if(!value) return;
    const needReDraw = this._streak !== value;
    this._streak = value;

    if (needReDraw) {
      setTimeout(() => {
        void this.Draw();
      }, 0);
    }
  }

  private get _baseWidth() {
    return 410 * this.dpr;
  }
  private get _baseHeight() {
    return 425 * this.dpr;
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

    const coeff = (this.canvas.width - this.padding) / this._baseWidth;
    this._koef = Math.min(Math.max(coeff, this._koef), this._maxCoefficient);

    this.canvas.height = this._baseHeight*this._koef + this.padding;
    const width = this._hexWidth * 5.5 + this._d * 5;
    const height = this._hexHeight * 5 + this._d * 5;
    this._drawRect = new Rect((this.canvas.width - width) / 2, (this.canvas.height - height) / 2, width, height);
    this.createCoordinates();

    const rects = this.getRects();
    this._hexagones = [];
    
    for (let i = 0; i < 21; i++) {
      const letter = String.fromCharCode(i);
      const fillColor = Colors.yellow;
      const polygone = this.canvas.drawHexagone(rects[i], fillColor, primary, 0, false, i < this._streak ? DrawType.fill : DrawType.stroke);
      polygone.refData = letter;
      if(i < this._streak){
        this.canvas.drawText(((i+1)*7).toString(), rects[i], primary, this._fontSize, true);
      }
      this._hexagones.push(polygone);
    }
  }

  private createCoordinates() {
    this._xCoords = [];
    this._yCoords = [];
    for (var i = 0; i < 10; i++) {
      this._xCoords.push(this._drawRect.x + (this._hexWidth + this._d) * i / 2);
    }
    for (var i = 0; i < 6; i++) {
      this._yCoords.push(this._drawRect.y + (this._hexHeight * 0.75 + this._d) * i);
    }
  }

  private getRects(): Rect[] {
    return [
      new Rect(this._xCoords[1], this._yCoords[0], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[3], this._yCoords[0], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[0], this._yCoords[1], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[2], this._yCoords[1], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[4], this._yCoords[1], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[6], this._yCoords[1], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[8], this._yCoords[1], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[1], this._yCoords[2], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[3], this._yCoords[2], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[5], this._yCoords[2], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[7], this._yCoords[2], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[9], this._yCoords[2], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[2], this._yCoords[3], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[4], this._yCoords[3], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[6], this._yCoords[3], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[8], this._yCoords[3], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[1], this._yCoords[4], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[3], this._yCoords[4], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[5], this._yCoords[4], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[2], this._yCoords[5], this._hexWidth, this._hexHeight),
      new Rect(this._xCoords[4], this._yCoords[5], this._hexWidth, this._hexHeight),
    ];
  }
}

