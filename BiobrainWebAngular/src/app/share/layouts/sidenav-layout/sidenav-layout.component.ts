import { AfterViewInit, Component, HostListener, OnDestroy } from '@angular/core';
import { MatDrawerMode } from '@angular/material/sidenav/drawer';
import { Subscription } from 'rxjs';
import { StringsService } from 'src/app/share/strings.service';

import { SidenavService } from '../../../core/services/side-nav.service';

@Component({
  selector: 'app-sidenav-layout',
  templateUrl: './sidenav-layout.component.html',
  styleUrls: ['./sidenav-layout.component.scss'],
})
export class SidenavLayoutComponent implements AfterViewInit, OnDestroy {
  sideNavMode: MatDrawerMode = 'side';
  isOpened = true;
  hasBackdrop = false;

  private readonly _subscriptions: Subscription[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _sidenavService: SidenavService) {
    this._subscriptions.push(
      _sidenavService.showNavigation$.subscribe(isOpened => this.isOpened = isOpened),
      _sidenavService.isNavigationStatic$.subscribe(isNavigationStatic => {
        this.sideNavMode = isNavigationStatic ? 'side' : 'over';
        this.hasBackdrop = !isNavigationStatic;
      }),
    );
  }

  ngAfterViewInit(): void {
    this._detectScreenSize();
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(_ => _.unsubscribe());
  }

  @HostListener('window:resize', [])
  private _onResize(): void {
    this._detectScreenSize();
  }

  private _detectScreenSize(): void {
    this._sidenavService.updateWindowSize(window.innerWidth);
  }
}
