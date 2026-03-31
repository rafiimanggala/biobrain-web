/* eslint-disable max-classes-per-file */
import { Injectable } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { HomePageService } from 'src/app/auth/services/home-page.service';

import { RoutingService } from '../../auth/services/routing.service';
import { AppEventProvider } from '../app/app-event-provider.service';
import { RouteDataProvider } from '../route/route-data.provider';

import { DisposableSubscriptionService } from './disposable-subscription.service';

class RouteClass {
  constructor(public parentPath: string, public fullPath: string, public navigatingSave: boolean) {
  }
}

@Injectable({
  providedIn: 'root',
})
export class NavigationService extends DisposableSubscriptionService {
  history: RouteClass[] = [];
  currentRoute: RouteClass | undefined;

  constructor(
    private readonly _router: Router,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _permissionService: HomePageService,
    private readonly _routeDataProvider: RouteDataProvider,
    private readonly _routingService: RoutingService,
    protected appEvents: AppEventProvider,
  ) {
    super();
  }

  public init(): void {
    try {
      this.currentRoute = new RouteClass(
        this._activatedRoute.firstChild?.snapshot.url.join('/') ?? '',
        this._router.url,
        this._routeDataProvider.canReturnToRoute(this._activatedRoute),
      );

      this.subscriptions.push(
        this._router.events.pipe(filter(x => x instanceof NavigationEnd)).subscribe(() => {
          const parentPath = this._activatedRoute.firstChild?.snapshot.url.join('/') ?? '';

          if (this.currentRoute?.fullPath === this._router.url) {
            return;
          }

          if (this.currentRoute?.navigatingSave) {
            this.history.push(this.currentRoute);
          }

          this.currentRoute = new RouteClass(
            parentPath,
            this._router.url,
            this._routeDataProvider.canReturnToRoute(this._activatedRoute),
          );
        }),
      );
    } catch (e) {
      this.appEvents.errorEmit(e);
    }
  }

  back(): Promise<boolean> {
    const lastRoute = this.history.pop();
    if (lastRoute) {
      this.currentRoute = lastRoute;
      return this._router.navigateByUrl(lastRoute.fullPath);
    }

    return this._routingService.navigateToHome();
  }
}
