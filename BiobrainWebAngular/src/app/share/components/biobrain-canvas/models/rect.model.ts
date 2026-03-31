import { Point } from "./point.model";
import { Polygon } from "./polygon.model";

export class Rect {
    constructor(
        public x: number,
        public y: number,
        public width: number,
        public height: number,
    ) { 
    }

    get top() {return this.y;}
    get bottom() {return this.y + this.height;}
    get left() {return this.x;}
    get right() {return this.x + this.width;}

    getCenter(){
        return new Point(this.x + this.width/2, this.y + this.height/2);
    }

    toPolygon(data: any = null): Polygon{
        return new Polygon([new Point(this.x, this.y), new Point(this.x + this.width, this.y), new Point(this.x + this.width, this.y + this.height), new Point(this.x, this.y + this.height)], data);
    }
}