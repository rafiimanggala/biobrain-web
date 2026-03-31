import { AfterViewInit, Component, ElementRef, EventEmitter, HostListener, OnDestroy, Output, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';

import { PolygonHelper } from '../../helpers/polygone.helper';
import { StringsService } from '../../strings.service';
import { HorizontalAlign } from '../../values/horizontal-align';
import { VerticalAlign } from '../../values/vartical-align';

import { Point } from './models/point.model';
import { Polygon } from './models/polygon.model';
import { Rect } from './models/rect.model';

export enum DrawType {
  fill,
  stroke
}

@Component({
  selector: 'app-biobrain-canvas',
  templateUrl: './biobrain-canvas.component.html',
  styleUrls: ['./biobrain-canvas.component.scss']
})
export class BiobrainCanvasComponent extends BaseComponent implements AfterViewInit, OnDestroy {
  private readonly _mouseMovehandler: EventListener;
  private readonly _trackedPolygons: Polygon[] = [];

  @Output() redraw: EventEmitter<void> = new EventEmitter();

  @ViewChild('canvas') canvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('container') container?: ElementRef<HTMLCanvasElement>;

  get width() {
    return (this.canvas?.nativeElement.width ?? 0);
  }
  get height() {
    return (this.canvas?.nativeElement.height ?? 0);
  }
  set height(h: number) {
    if (!this.canvas) return;
    this.canvas.nativeElement.height = h
  }

  @HostListener('window:resize', [])
  private _onResize(): void {
    if(!this.canvas) return;
    this.canvas.nativeElement.width  = this.canvas.nativeElement.offsetWidth * this.dpr;
    this.canvas.nativeElement.height = this.canvas.nativeElement.offsetHeight * this.dpr;
    this.redraw.emit();
  }

  dpr = 1;

  private _context: CanvasRenderingContext2D | null = null;

  constructor(appEvents: AppEventProvider, private readonly _strings: StringsService) {
    super(appEvents);

    const handler: (ev: MouseEvent) => void = (ev: MouseEvent) => {
      if (!this.canvas) {
        throw Error('No canvas');
      }

      if (!this.canvas.nativeElement) {
        throw Error('No canvas nativeElement');
      }

      ev.preventDefault();
      ev.stopPropagation();

      // const canvasOffset = this.canvas.nativeElement.getBoundingClientRect();
      // const offsetX = canvasOffset.left;
      // const offsetY = canvasOffset.top;
      //
      // const mouseX = ev.clientX - offsetX;
      // const mouseY = ev.clientY - offsetY;

      // Put your mousemove stuff here
      const polygon = PolygonHelper.findPolygone(new Point(ev.offsetX * this.dpr, ev.offsetY * this.dpr), this._trackedPolygons);
      if (polygon) {
        this.canvas.nativeElement.style.cursor = 'pointer';
      } else {
        this.canvas.nativeElement.style.cursor = 'default';
      }
    };
    this._mouseMovehandler = handler as EventListener;
  }

  ngAfterViewInit(): void {
    if (!this.canvas) return;
    // Get the device pixel ratio, falling back to 1.
    this.dpr = window.devicePixelRatio || 1;
    // Get the size of the canvas in CSS pixels.
    const rect = this.canvas.nativeElement.getBoundingClientRect();
    // Give the canvas pixel dimensions of their CSS
    // size * the device pixel ratio.
    this.canvas.nativeElement.width = rect.width * this.dpr;
    this.canvas.nativeElement.height = rect.height * this.dpr;

    this._context = this.canvas.nativeElement.getContext('2d');
    this.canvas.nativeElement.addEventListener('mousemove', this._mouseMovehandler);
  }

  ngOnDestroy(): void {
    this.canvas?.nativeElement?.removeEventListener('mousemove', this._mouseMovehandler);
  }

  clear() {
    if (this._context && this.canvas)
      this._context.clearRect(0, 0, this.canvas.nativeElement.width, this.canvas.nativeElement.height);
  }

  definePath(polygon: Polygon): void {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    if (!polygon.vertices || polygon.vertices.length < 2) {
      return;
    }

    this._context.beginPath();
    this._context.moveTo(polygon.vertices[0].x, polygon.vertices[0].y);

    for (let i = 1; i < polygon.vertices.length; i++) {
      this._context.lineTo(polygon.vertices[i].x, polygon.vertices[i].y);
    }

    this._context.closePath();
  }

  drawPolygone(
    polygone: Polygon,
    fillColor = '#000',
    lineColor = '#000',
    lineWidth = 1,
    trackPolygonHover: boolean = false,
    drawType: DrawType = DrawType.fill,
  ): Polygon {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return polygone;
    }

    this._context.lineWidth = lineWidth;
    this._context.strokeStyle = lineColor;
    this._context.fillStyle = fillColor;

    if (!polygone.vertices || polygone.vertices.length < 2) {
      return polygone;
    }

    if (trackPolygonHover) {
      this._trackedPolygons.push(polygone);
    }

    this.definePath(polygone);

    this._context.stroke();
    if (drawType == DrawType.fill)
      this._context.fill();
    return polygone;
  }

  drawHexagone(
    rect: Rect,
    fillColor = '#000',
    lineColor = '#000',
    lineWidth = 1,
    trackPolygonHover: boolean,
    drawType: DrawType = DrawType.fill,
  ): Polygon {
    const p1 = new Point(rect.x, rect.y + rect.height * 0.25);
    const p2 = new Point(rect.x, rect.y + rect.height * 0.75);
    const p3 = new Point(rect.x + rect.width * 0.5, rect.y + rect.height);
    const p4 = new Point(rect.x + rect.width, rect.y + rect.height * 0.75);
    const p5 = new Point(rect.x + rect.width, rect.y + rect.height * 0.25);
    const p6 = new Point(rect.x + rect.width * 0.5, rect.y);
    const polygon = new Polygon([p1, p2, p3, p4, p5, p6]);
    this.drawPolygone(polygon, fillColor, lineColor, lineWidth, trackPolygonHover, drawType);
    return polygon;
  }

  drawWideHexagone(
    rect: Rect,
    fillColor = '#000',
    lineColor = '#000',
    lineWidth = 1,
    trackPolygonHover: boolean,
    drawType: DrawType = DrawType.fill,
  ): Polygon {
    const p1 = new Point(rect.x + rect.width * 0.25, rect.y);
    const p2 = new Point(rect.x + rect.width * 0.75, rect.y);
    const p3 = new Point(rect.x + rect.width, rect.y + rect.height * 0.5);
    const p4 = new Point(rect.x + rect.width * 0.75, rect.y + rect.height);
    const p5 = new Point(rect.x + rect.width * 0.25, rect.y + rect.height);
    const p6 = new Point(rect.x, rect.y + rect.height * 0.5);
    const polygon = new Polygon([p1, p2, p3, p4, p5, p6]);
    this.drawPolygone(polygon, fillColor, lineColor, lineWidth, trackPolygonHover, drawType);
    return polygon;
  }

  // Text

  drawText(text: string, rect: Rect, fontColor: string, fontSize: number, isBold = false, verticalAlign: VerticalAlign = VerticalAlign.center, horizontalAlign: HorizontalAlign = HorizontalAlign.center) {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    this._context.font = `${isBold ? 'bold' : ''} ${fontSize}px "Nunito", sans-serif`;
    this._context.fillStyle = fontColor;
    const measure = this._context.measureText(text);
    const center = rect.getCenter();
    let x = center.x - measure.width / 2;
    let y = center.y + fontSize / 2;
    if (verticalAlign == VerticalAlign.start) {
      y = rect.top + fontSize;
    }
    if (verticalAlign == VerticalAlign.end) {
      y = rect.bottom;
    }
    if (horizontalAlign == HorizontalAlign.start) {
      x = rect.left;
    }
    if (horizontalAlign == HorizontalAlign.end) {
      x = rect.right - measure.width;
    }
    this._context.fillText(text, x, y);
  }

  drawTextOnBottomCenter(text: string, rect: Rect, fontColor: string, fontSize: number, isBold = false, bottomOffset = 1) {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    this._context.font = `${isBold ? 'bold' : ''} ${fontSize}px "Nunito", sans-serif`;
    this._context.fillStyle = fontColor;
    const measure = this._context.measureText(text);
    const center = rect.getCenter();
    const x = center.x - measure.width / 2;
    const y = rect.bottom - bottomOffset*1.5;
    this._context.fillText(text, x, y);
  }

  drawTextOnTopLeft(text: string, rect: Rect, fontColor: string, fontSize: number, isBold = false, offset = 1) {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    this._context.font = `${isBold ? 'bold' : ''} ${fontSize}px "Nunito", sans-serif`;
    this._context.fillStyle = fontColor;
    const measure = this._context.measureText(text);

    const x = rect.left + offset;
    const y = rect.top + offset + measure.fontBoundingBoxAscent;
    this._context.fillText(text, x, y);
  }

  drawTextAbove(text: string, rect: Rect, fontColor: string, fontSize: number, isBold = false, offset = 1) {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    this._context.font = `${isBold ? 'bold' : ''} ${fontSize}px "Nunito", sans-serif`;
    this._context.fillStyle = fontColor;
    const measure = this._context.measureText(text);
    const center = rect.getCenter();

    const x = center.x - measure.width / 2;
    const y = rect.top - offset;
    this._context.fillText(text, x, y);
  }

  // Arc

  drawArc(rect: Rect, strokeWidth: number, angle: number, color: string, diffArc: number = -Math.PI / 2.5) {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return;
    }

    const center = rect.getCenter();
    this._context.lineCap = 'round';
    this._context.strokeStyle = color;
    this._context.lineWidth = strokeWidth;
    this._context.beginPath();
    this._context.arc(center.x, center.y, rect.width / 2, 0 + diffArc, angle + diffArc);
    this._context.stroke();
  }

  // Convert

  rectToPolygon(rect: Rect, data: any | null = null): Polygon {
    return new Polygon([new Point(rect.x, rect.y), new Point(rect.x + rect.width, rect.y), new Point(rect.x + rect.width, rect.y + rect.height), new Point(rect.x, rect.y + rect.height)], data);
  }

  // Measurement
  measureText(text: string, fontSize: number, isBold = false): TextMetrics {
    if (!this._context) {
      this.error(this._strings.errors.noCanvasContext);
      return new TextMetrics();
    }

    this._context.font = `${isBold ? 'bold' : ''} ${fontSize}px "Nunito", sans-serif`;
    return this._context.measureText(text);
  }
}

