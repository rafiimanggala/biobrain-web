import { KeyValue } from '@angular/common';
import { AfterViewInit, Component, HostListener, OnDestroy, ViewChild } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { BiobrainCanvasComponent, DrawType } from 'src/app/share/components/biobrain-canvas/biobrain-canvas.component';
import { Point } from 'src/app/share/components/biobrain-canvas/models/point.model';
import { Polygon } from 'src/app/share/components/biobrain-canvas/models/polygon.model';
import { Rect } from 'src/app/share/components/biobrain-canvas/models/rect.model';
import { PolygonHelper } from 'src/app/share/helpers/polygone.helper';
import { StringsService } from 'src/app/share/strings.service';
import { Colors } from 'src/app/share/values/colors';
import { HorizontalAlign } from 'src/app/share/values/horizontal-align';
import { VerticalAlign } from 'src/app/share/values/vartical-align';
import { ChemicalElementModel } from './chemical-element.model';
import { ChemicalElementsData } from './chemical-elements-data';

@Component({
  selector: 'app-chemical-elements',
  templateUrl: './chemical-elements.component.html',
  styleUrls: ['./chemical-elements.component.scss']
})
export class ChemicalElementsComponent extends BaseComponent implements AfterViewInit {

  private readonly sBlockName: string = "s Block";
  private readonly dBlockName: string = "d Block";
  private readonly pBlockName: string = "p Block";
  private readonly fBlockName: string = "f Block";

  private get _dpr() {
    return this.canvas?.dpr ?? 1;
  }

  private _selectedSymbol = '';
  private _scaleFactor = 1;

  // Constants
  private readonly _horizontalNumberOfCells = 18;
  private readonly _verticalNumberOfCells = 9;

  // Dip values
  private readonly _dipRectHeight = 28;
  private readonly _dipD = 4;
  private readonly _dipOffset = 2;
  private readonly _dipHeaderHeight = 60;
  private readonly _dipFontSize = 14;

  // values for draw
  private get _defaultRectHeight() { return this._dpr * this._dipRectHeight };
  private _rectHeight = this._defaultRectHeight;
  private get _d() { return this._dpr * this._dipD};
  private get _offset() { return this._dipOffset * this._scaleFactor };
  private get _headerHeight() { return this._dpr * this._dipHeaderHeight };
  private get _popupHeight() { return this._rectHeight * 2.75 };
  private get _popupLegendHeight() { return this._rectHeight * 2.25 };
  private get _strokeWidth() { return 2 * this._dpr * this._scaleFactor };

  private get _fontSize() { return this._dipFontSize * this._scaleFactor * this._dpr };
  private get _smallFontSize() { return this._fontSize * 0.6 };
  private get _mediumFontSize() { return this._fontSize * 0.8 };
  private get _popupLettersFont() { return this._fontSize * 1.5 };
  private get _popupNumberFont() { return this._fontSize };
  private get _popupNameFont() { return this._fontSize * 0.8 };

  // Rects
  private _drawRect = new Rect(0, 0, 0, 0);
  //Elments rects (to find clicked)
  private rectangles: Polygon[] = [];
  private popupPolygon = new Polygon([]);

  //Grid coordinates
  private _xCoords: number[] = [];
  private _yCoords: number[] = [];

  @ViewChild('canvas') canvas?: BiobrainCanvasComponent;

  constructor(
    private readonly strings: StringsService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);
    
    // screen.orientation.addEventListener("change", () => {
    //   this.error(`Orientation type is ${screen.orientation.type} & angle is ${screen.orientation.angle}.`);
    // });
  }

  ngAfterViewInit(): void {
    try{
    screen.orientation.lock('landscape');
    }
    catch(e: any){
      console.log(e);
    }
    this.Draw();
  }

  onRedraw(){
    if(!this.canvas) return;
    this.Draw();
  }

  Draw() {
    if (!this.canvas) {
      this.error('no canvas');
      return;
    }

    this.calcSizes(new Rect(0, 0, this.canvas.width, this.canvas.height));
    this.calcGridCoordinatesInternal();
    this.canvas.clear();
    this.drawElements();
    this.drawLegend();
    // DrawIcon(e.Surface.Canvas);
    // DrawHeader(e.Surface.Canvas);
    // if(!string.IsNullOrEmpty(selectedSymbol))
    //     DrawPopup(e.Surface.Canvas);
  }

  onCanvasClick(event: MouseEvent) {
    var point = new Point(event.offsetX * this._dpr, event.offsetY * this._dpr);
    if (this._selectedSymbol && this._selectedSymbol.length > 0 && PolygonHelper.isInside(point, this.popupPolygon)) {
      this.clearSelection();
      return;
    }
    const polygon = this.getPolygonByPoint(point);
    if (!polygon) {
      this.clearSelection();
      return;
    }

    this._selectedSymbol = polygon.refData;
    this.Draw();
    this.popupPolygon = this.drawPopup(polygon);
  }

  clearSelection(){
    this._selectedSymbol = '';
    this.Draw();
  }

  private getPolygonByPoint(point: Point): Polygon | null {
    let result: Polygon | null = null;
    this.rectangles.forEach(x => {
      if (PolygonHelper.isInside(point, x)) {
        result = x;
      }
    });
    return result;
  }

  private calcSizes(canvasRect: Rect) {
    var vertikalKoef = 2;
    var horizontalKoef = 2;
    // (numberofcells + koef) - Add x of cell height to horizontal and vertical size for annotation
    var newByHeight = (canvasRect.height - this._d * (this._verticalNumberOfCells + 5) - this._headerHeight) / (this._verticalNumberOfCells + vertikalKoef);
    var newByWidth = (canvasRect.width - this._d * (this._horizontalNumberOfCells - 1)) / (this._horizontalNumberOfCells + horizontalKoef);
    this._rectHeight = Math.round(newByWidth < newByHeight ? newByWidth : newByHeight);
    this._scaleFactor = this._rectHeight / this._defaultRectHeight;
    let width = this._rectHeight * (this._horizontalNumberOfCells + horizontalKoef) + this._d * (this._horizontalNumberOfCells - 1);
    let height = this._rectHeight * (this._verticalNumberOfCells + vertikalKoef) + this._d * (this._verticalNumberOfCells + 5);
    this._drawRect = new Rect((canvasRect.width - width) / 2, (canvasRect.height - height) / 2, width, height);
  }

  private calcGridCoordinatesInternal() {
    this._xCoords = [];
    this._yCoords = [];
    for (var i = 0; i < 18; i++)
      this._xCoords.push(this._drawRect.x + (this._rectHeight + this._d) * i + this._rectHeight * 1.5);
    for (var i = 0; i < 9; i++)
      this._yCoords.push((this._drawRect.y + (this._rectHeight + this._d) * i) + (i > 6 ? this._d * 2 + this._smallFontSize : 0) + this._rectHeight * 1);
  }

  private drawElements() {
    this.rectangles = [];

    ChemicalElementsData.chemicalElements.forEach(rect => {
      var polygon = this.drawElement(rect);
      if (!polygon) return;

      this.rectangles.push(polygon);
    });
  }

  private drawLegend() {
    this.drawGroupsPeriods();
    this.drawBlocks();
    this.drawPopupLegend();
  }

  private drawElement(rect: KeyValue<string, ChemicalElementModel>): Polygon | null {
    if (!this.canvas) return null;

    var element = rect.value;
    var rectangle = new Rect(this._xCoords[element.x], this._yCoords[element.y], this._rectHeight, this._rectHeight);
    var polygon = this.canvas.rectToPolygon(rectangle, rect.key);
    let bgColor = rect.key == this._selectedSymbol ? Colors.red : element.backgroundColor;

    // Draw elements
    if (rect.value.fontColor == Colors.white)
      this.canvas.drawPolygone(polygon, bgColor, bgColor, -1);
    else
      this.canvas.drawPolygone(polygon, bgColor, element.fontColor, 2, false, DrawType.stroke);

    this.canvas.drawTextOnBottomCenter(element.shortName, rectangle, element.fontColor, this._fontSize, true, this._offset);
    this.canvas.drawTextOnTopLeft(element.atomicNumber.toString(), rectangle, element.fontColor, this._smallFontSize, false, this._offset);

    // Draw groups
    if (element.isGroupFirst)
      this.canvas.drawTextAbove((element.x + 1).toString(), rectangle, Colors.darkGray, this._smallFontSize, false, this._offset);

    // Draw periods
    var periodsMeasure = this.canvas.measureText(this.strings.periods, this._smallFontSize, false);
    var periodColumnRect = new Rect(rectangle.x - periodsMeasure?.width - this._offset, rectangle.y + this._offset, periodsMeasure?.width, rectangle.height - this._offset * 2);
    if (element.isPeriodFirsr)
      this.canvas.drawText((element.y + 1).toString(), periodColumnRect, Colors.darkGray, this._smallFontSize, false, (element.y + 1) == 1 ? VerticalAlign.end : VerticalAlign.center);

    // Draw blocks
    if (element.blockName) {
      if (element.blockName == this.fBlockName) {
        this.canvas.drawTextAbove(element.blockName, rectangle, Colors.darkGray, this._smallFontSize, false, this._offset);
      }
      else {
        this.canvas.drawTextAbove(element.blockName, rectangle, Colors.darkGray, this._smallFontSize, false, this._offset * 2 + this._smallFontSize);
      }
    }
    return polygon;
  }

  private drawGroupsPeriods() {
    if (!this.canvas) return;

    var groupsWidth = this.canvas.measureText(this.strings.groups, this._smallFontSize, false);
    var periodsWidth = this.canvas.measureText(this.strings.periods, this._smallFontSize, false);

    this.canvas.drawText(this.strings.groups, new Rect(this._xCoords[0] - this._offset - groupsWidth.width, this._yCoords[0], groupsWidth.width, this._smallFontSize), Colors.darkGray, this._smallFontSize);
    this.canvas.drawText(this.strings.periods, new Rect(this._xCoords[0] - this._offset - periodsWidth.width, this._yCoords[0] - this._smallFontSize - this._offset, periodsWidth.width, this._smallFontSize), Colors.darkGray, this._smallFontSize);
  }

  private drawBlocks() {
    if (!this.canvas) return;

    // new Rectangle(xCoords[5], yCoords[1], RectHeight, RectHeight), CustomColors.DarkGray, MediumFontSize, D, RectHeight / 2
    var dWidth = this.canvas.measureText(this.dBlockName, this._mediumFontSize, false);
    var rectHeight = this._rectHeight / 2;

    var r = new Rect(this._xCoords[5], this._yCoords[1], this._rectHeight, this._rectHeight);
    this.canvas.drawPolygone(new Rect(r.left, r.top, rectHeight, rectHeight).toPolygon(), Colors.primary);
    this.canvas.drawPolygone(new Rect(r.left, r.top + rectHeight + this._offset, rectHeight, rectHeight).toPolygon(), Colors.accentChemistry);
    this.canvas.drawPolygone(new Rect(r.left + dWidth.width + 2 * this._offset + rectHeight, r.top, rectHeight, rectHeight).toPolygon(), Colors.primaryChemistry);
    this.canvas.drawPolygone(new Rect(r.left + dWidth.width + 2 * this._offset + rectHeight, r.top + rectHeight + this._offset, rectHeight, rectHeight).toPolygon(), Colors.accent);

    this.canvas.drawText(this.sBlockName, new Rect(r.left + rectHeight + this._offset, r.top, dWidth.width, rectHeight), Colors.darkGray, this._mediumFontSize);
    this.canvas.drawText(this.dBlockName, new Rect(r.left + rectHeight + this._offset, r.top + rectHeight + this._offset, dWidth.width, rectHeight), Colors.darkGray, this._mediumFontSize);
    this.canvas.drawText(this.pBlockName, new Rect(r.left + 2 * rectHeight + 3 * this._offset + dWidth.width, r.top, dWidth.width, rectHeight), Colors.darkGray, this._mediumFontSize);
    this.canvas.drawText(this.fBlockName, new Rect(r.left + 2 * rectHeight + 3 * this._offset + dWidth.width, r.top + rectHeight + this._offset, dWidth.width, rectHeight), Colors.darkGray, this._mediumFontSize);
  }

  private drawPopupLegend() {
    if (!this.canvas) return;

    let baseRect = new Rect(this._xCoords[9] + this._rectHeight / 2, this._yCoords[1] - this._rectHeight, this._popupLegendHeight, this._popupLegendHeight);
    this.canvas.drawPolygone(baseRect.toPolygon(), Colors.white, Colors.primaryChemistry, this._strokeWidth, false, DrawType.fill);

    //Atomic Number
    this.canvas.drawText(this.strings.atomic, new Rect(baseRect.left + this._offset, baseRect.top + this._offset, this._popupLegendHeight, this._popupLegendHeight), Colors.primaryChemistry, this._smallFontSize, false, VerticalAlign.start, HorizontalAlign.start);
    this.canvas.drawText(this.strings.number, new Rect(baseRect.left + this._offset, baseRect.top + this._offset + this._smallFontSize, this._popupLegendHeight, this._popupLegendHeight), Colors.primaryChemistry, this._smallFontSize, false, VerticalAlign.start, HorizontalAlign.start);
    //Mass
    this.canvas.drawText(this.strings.atomicMass, new Rect(baseRect.left, baseRect.bottom - this._offset - this._smallFontSize, this._popupLegendHeight, this._smallFontSize), Colors.primaryChemistry, this._smallFontSize);
    //Name
    this.canvas.drawText(this.strings.name, new Rect(baseRect.left, baseRect.bottom - this._offset - 2 * this._smallFontSize, this._popupLegendHeight, this._smallFontSize), Colors.primaryChemistry, this._smallFontSize);
    //Symbol
    this.canvas.drawText(this.strings.symbol, baseRect, Colors.primaryChemistry, this._mediumFontSize);
  }

  drawPopup(selectedPolygon: Polygon): Polygon {
    var element = ChemicalElementsData.chemicalElements.find(_ => _.key == this._selectedSymbol)?.value;
    if(!element) return new Polygon([]);

    var popupRect = new Rect(0, 0, this._popupHeight, this._popupHeight);

    //If can draw left - draw left
    if (PolygonHelper.isInside(new Point((selectedPolygon.left - this._popupHeight) - this._d, selectedPolygon.top), this._drawRect.toPolygon()))
      popupRect.x = selectedPolygon.left - this._popupHeight - this._d;
    else
      popupRect.x = selectedPolygon.right + this._d;

    //If can draw top - draw top
    if (PolygonHelper.isInside(new Point(popupRect.x, selectedPolygon.top - this._popupHeight - this._d), this._drawRect.toPolygon()))
      popupRect.y = selectedPolygon.top - this._popupHeight - this._d;
    else
      popupRect.y = selectedPolygon.bottom + this._d;

    var polygon = popupRect.toPolygon();
    this.canvas?.drawPolygone(polygon, Colors.white, Colors.primaryChemistry, this._strokeWidth);

    this.drawElementCard(element, popupRect);

    return polygon;
  }

  private drawElementCard(element: ChemicalElementModel, cardRect: Rect) {
    if(!this.canvas) return;

    this.canvas.drawText(element.atomicNumber.toString(), new Rect(cardRect.left + this._offset, cardRect.top + this._offset, cardRect.width, cardRect.height), Colors.primaryChemistry, this._popupNumberFont, false, VerticalAlign.start, HorizontalAlign.start);
    this.canvas.drawText(`${element.massNumber}`, new Rect(cardRect.left, cardRect.top, cardRect.width, cardRect.height - this._offset), Colors.primaryChemistry, this._smallFontSize, false, VerticalAlign.end, HorizontalAlign.center);
    this.canvas.drawText(element.name, new Rect(cardRect.left, cardRect.top, cardRect.width, cardRect.height  - this._offset*2 - this._smallFontSize), Colors.primaryChemistry, this._popupNameFont, false, VerticalAlign.end, HorizontalAlign.center);
    this.canvas.drawText(element.shortName, new Rect(cardRect.left, cardRect.top, cardRect.width, cardRect.height), Colors.primaryChemistry, this._popupLettersFont, false, VerticalAlign.center, HorizontalAlign.center);
  }
}

