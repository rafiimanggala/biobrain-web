import { Point } from '@angular/cdk/drag-drop';

import { Polygon } from '../components/biobrain-canvas/models/polygon.model';

export class PolygonHelper {

  static findPolygone(point: Point, polygones: Polygon[]): Polygon | null {
    let result: Polygon | null = null;
    polygones.forEach(x => {
      if (this.isInside(point, x)) {
        result = x;
      }
    });
    return result;
  }

  static isInside(p: Point, polygone: Polygon): boolean {
    // ray-casting algorithm based on
    // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html/pnpoly.html

    let inside = false;
    for (let i = 0, j = polygone.vertices.length - 1; i < polygone.vertices.length; j = i++) {
      const xi = polygone.vertices[i].x, yi = polygone.vertices[i].y;
      const xj = polygone.vertices[j].x, yj = polygone.vertices[j].y;

      const intersect = yi > p.y != yj > p.y
                && p.x < (xj - xi) * (p.y - yi) / (yj - yi) + xi;
      if (intersect) inside = !inside;
    }

    return inside;
  }
}
