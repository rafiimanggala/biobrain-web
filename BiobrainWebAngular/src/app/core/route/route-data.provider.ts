import { Injectable } from '@angular/core';
import { ActivatedRoute, Data } from '@angular/router';

@Injectable()
export class RouteDataProvider {
  canReturnToRoute(route: ActivatedRoute): boolean {
    const routeDataStack = this.getChildActivatedRouteDataStack(route);

    while (routeDataStack.length) {
      const routeData = routeDataStack.pop();
      if(!routeData) continue;

      if (routeData.hasOwnProperty('navigatingSave')) {
        return routeData['navigatingSave'];
      }
    }

    return false;
  }

  private getChildActivatedRouteDataStack(route: ActivatedRoute): Data[] {
    const stack: Data[] = [];

    let activatedChildRoute = route.firstChild;
    while (activatedChildRoute) {
      if (activatedChildRoute.snapshot && activatedChildRoute.snapshot.data) {
        stack.push(activatedChildRoute.snapshot.data);
      }

      activatedChildRoute = activatedChildRoute.firstChild;
    }

    return stack;
  }
}
