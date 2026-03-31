import { AfterViewInit, Component, ElementRef, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ThemeService } from 'src/app/core/app/theme.service';
import { firstValueFrom } from '../../helpers/first-value-from';

import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Polygon } from '../biobrain-canvas/models/polygon.model';
import { Rect } from '../biobrain-canvas/models/rect.model';

@Component({
  selector: 'app-bee',
  templateUrl: './bee.component.html',
  styleUrls: ['./bee.component.scss']
})
export class BeeComponent extends BaseComponent implements AfterViewInit, OnChanges {

  private get _baseImgWidth(){ return 1928; }
  private get _baseImgHeight(){ return 1059; }

  private get _baseHexX(){ return 1530; }
  private get _baseHexY(){ return 835; }

  private get _baseWidth() { return 208; }
  private get _baseHeight() { return 182; }
  private readonly _maxCoefficient = 0.9;
  private _koef = 0.0001;
  private _drawRect = new Rect(0, 0, 0, 0);

  private get _hexWidth() {
    return this. _baseWidth * this._koef * this._dpr;
  }
  private get _hexHeight() {
    return this._baseHeight * this._koef * this._dpr;
  }
  private get _hexX() {
    return this. _baseHexX * this._koef * this._dpr;
  }
  private get _hexY() {
    return this._baseHexY * this._koef * this._dpr;
  }
  private get _fontSize() {
    return 80 * this._koef * this._dpr;
  }
  private get _dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private readonly _hexagones: Polygon[] = [];

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;
  @ViewChild('beeImg') image?: ElementRef<HTMLImageElement>;

  @Input() days = 0;

  constructor(
    appEvents: AppEventProvider,
    private readonly _themeService: ThemeService,
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

  async Draw() {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }
    
    const { primary, accent } = await firstValueFrom(this._themeService.colors$);
    const element = this.getImageElement();

    const coeff = element.width / this._baseImgWidth;
    this._koef = Math.min(Math.max(coeff, this._koef), this._maxCoefficient);
    this._drawRect = new Rect(this._hexX, this._hexY, this._hexWidth, this._hexHeight);
    // console.log(this._drawRect, element, this._koef);

    this.canvas.drawWideHexagone(this._drawRect, accent, accent, 1, false);
    var textRect =  new Rect(this._drawRect.x, this._drawRect.y-10*this._koef, this._drawRect.width, this._drawRect.height);
    this.canvas.drawText(this.days.toString(), textRect, Colors.white, this._fontSize);
  }

  private getCanvasElement(): HTMLCanvasElement {
    if (!this.canvas || !this.canvas.canvas || !this.canvas.canvas.nativeElement) {
      this.error('no canvas');
      throw Error('no canvas');
    }
    return this.canvas.canvas.nativeElement;
  }

  private getImageElement(): HTMLImageElement {
    if (!this.image || !this.image.nativeElement) {
      this.error('no canvas');
      throw Error('no canvas');
    }
    return this.image.nativeElement;
  }

}

