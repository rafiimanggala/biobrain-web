import { AfterViewInit, Component, HostListener, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { ThemeService } from '../../../core/app/theme.service';
import { firstValueFrom } from '../../helpers/first-value-from';
import { Colors } from '../../values/colors';
import { BiobrainCanvasComponent } from '../biobrain-canvas/biobrain-canvas.component';
import { Rect } from '../biobrain-canvas/models/rect.model';

@Component({
  selector: 'app-success-rate-diagram',
  templateUrl: './success-rate-diagram.component.html',
  styleUrls: ['./success-rate-diagram.component.scss']
})
export class SuccessRateDiagram extends BaseComponent implements AfterViewInit {

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;

  @Input() successRate = 0;
  @Input() diff = 0;
  @Input() primaryColor: string = Colors.red;
  @Input() accentColor: string = Colors.green;
  @Input() fontSize = 36;
  @Input() strokeWidth = 8;


  private get _strokeWidth() {
    return this.strokeWidth * this._dpr;
  }
  private get _fontSize() {
    return this.fontSize * this._dpr;
  }
  private get _dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private _drawRect = new Rect(0, 0, 0, 0);

  constructor(
    appEvents: AppEventProvider,
    private readonly _themeService: ThemeService,
  ) {
    super(appEvents);
  }

  async ngAfterViewInit(): Promise<void> {
    await this.Draw();
  }

  async Draw(): Promise<void> {
    if (!this.canvas) {
      // this.error('no canvas');
      return;
    }
    const element = this.getCanvasElement();

    const size = Math.min(element.width, element.height);
    this._drawRect = new Rect((element.width - size) / 2 + this._strokeWidth, (element.height - size) / 2 + this._strokeWidth, size - this._strokeWidth * 2, size - this._strokeWidth * 2);

    this.canvas.drawArc(this._drawRect, this._strokeWidth, 3 * Math.PI, this.primaryColor);

    if (this.successRate > 0) {
      this.canvas.drawArc(this._drawRect, this._strokeWidth + 1, 2 * Math.PI * this.successRate, this.accentColor);
    }

    const { primary } = await firstValueFrom(this._themeService.colors$);
    var textRect = this._drawRect;
    textRect.x = textRect.x + 5;
    this.canvas.drawText(`${Math.round(this.successRate * 100)}%`, textRect, primary, this._fontSize, true);
  }

  private getCanvasElement(): HTMLCanvasElement {
    if (!this.canvas || !this.canvas.canvas || !this.canvas.canvas.nativeElement) {
      this.error('no canvas');
      throw Error('no canvas');
    }
    return this.canvas.canvas.nativeElement;
  }

}

