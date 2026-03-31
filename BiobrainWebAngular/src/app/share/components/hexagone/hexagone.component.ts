import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { PolygonHelper } from '../../helpers/polygone.helper';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Point } from '../biobrain-canvas/models/point.model';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';

@Component({
  selector: 'app-hexagone',
  templateUrl: './hexagone.component.html',
  styleUrls: ['./hexagone.component.scss']
})
export class Hexagone extends BaseComponent implements AfterViewInit, OnChanges {

  @Output() questionSelected: EventEmitter<string> = new EventEmitter();

  private get _baseWidth() {
    return 70 * this._dpr;
  }
  private get _baseHeight() {
    return 80 * this._dpr;
  }
  private readonly _maxCoefficient = 0.9;
  private _koef = 0.6;
  private _drawRect = new Rect(0, 0, 0, 0);

  private get _hexWidth() {
    return 70.0 * this._koef * this._dpr;
  }
  private get _hexHeight() {
    return 80.0 * this._koef * this._dpr;
  }
  private get _d() {
    return 5.0 * this._koef * this._dpr;
  }
  private get _fontSize() {
    return 18 * this._koef * this._dpr;
  }
  private get _dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private readonly _hexagones: Polygon[] = [];

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;

  @Input() color : string | null = '';
  @Input() textColor : string | null = '';
  @Input() borderColor : string | null = null;
  @Input() text = '';
  @Input() borderWidth = 1;
  @Input() isWide = false;

  constructor(
    appEvents: AppEventProvider
  ) {
    super(appEvents);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['text'] && changes['text'].previousValue) {
      this.Draw();
    }
  }

  ngAfterViewInit(): void {
    this.Draw();

  }

  Draw() {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }
    const element = this.getCanvasElement();

    const hCoeff = element.height / this._baseHeight;
    const wCoeff = element.width / this._baseWidth;
    const coeff = Math.min(wCoeff, hCoeff);
    this._koef = Math.min(Math.max(coeff, this._koef), this._maxCoefficient);

    if(this.isWide){
      this._drawRect = new Rect((element.width - this._hexHeight) / 2 + this.borderWidth, (element.height - this._hexWidth ) / 2 + this.borderWidth, this._hexHeight- this.borderWidth*2, this._hexWidth - this.borderWidth*2);
      this.canvas.drawWideHexagone(this._drawRect, this.color ?? "", this.borderColor ?? this.color ?? "", this.borderWidth, false);//this.borderColor ?? this.color ?? ""
    }
    else{
      this._drawRect = new Rect((element.width - this._hexWidth) / 2 + this.borderWidth, (element.height - this._hexHeight ) / 2 + this.borderWidth, this._hexWidth- this.borderWidth*2, this._hexHeight - this.borderWidth*2);
      this.canvas.drawHexagone(this._drawRect, this.color ?? "", this.borderColor ?? this.color ?? "", this.borderWidth, false);//this.borderColor ?? this.color ?? ""
    }
    this.canvas.drawText(this.text, this._drawRect, this.textColor ?? Colors.white, this._fontSize, true);
  }

  onCanvasClick(event: MouseEvent) {
    const polygon = this.getPolygonByPoint(new Point(event.offsetX, event.offsetY));
    if (!polygon) return;
    this.questionSelected.emit(polygon.refData);
  }

  private getPolygonByPoint(point: Point): Polygon | null {
    let result: Polygon | null = null;
    this._hexagones.forEach(x => {
      if (PolygonHelper.isInside(point, x)) {
        result = x;

      }
    });
    return result;
  }

  private getCanvasElement(): HTMLCanvasElement {
    if (!this.canvas || !this.canvas.canvas || !this.canvas.canvas.nativeElement) {
      this.error('no canvas');
      throw Error('no canvas');
    }
    return this.canvas.canvas.nativeElement;
  }

}

