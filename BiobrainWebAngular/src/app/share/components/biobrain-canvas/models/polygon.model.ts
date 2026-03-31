import { Point } from "./point.model";

export class Polygon {
    constructor(
        public vertices: Point[],
        public refData: string = ''
    ) {
    }

    get left() { return Math.min(...this.vertices.map(_ => _.x)); }
    get right() { return Math.max(...this.vertices.map(_ => _.x)); }
    get top() { return Math.min(...this.vertices.map(_ => _.y)); }
    get bottom() { return Math.max(...this.vertices.map(_ => _.y)); }
}