import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatSidenav } from '@angular/material/sidenav';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { LogoutOperation } from 'src/app/auth/operations/logout.operation';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { NavigationItem } from 'src/app/core/models/navigation-item';
import { NavigationService } from 'src/app/core/services/navigation.service';
import { StringsService } from 'src/app/share/strings.service';
import { ChangeSelfPasswordOperation } from '../../auth/operations/change-self-password.operation';

import { RoutingService } from '../../auth/services/routing.service';
import { SubTitleProviderService } from '../services/sub-title-provider.service';


@Component({
  selector: 'app-base-admin-layout',
  templateUrl: './base-admin-layout.component.html',
  styleUrls: ['./base-admin-layout.component.scss'],
})
export class BaseAdminLayoutComponent implements OnInit {
  @ViewChild('sidenav') sidenav?: MatSidenav;
  @ViewChild('menuButton') menuButton?: MatButton;
  @Input() isShowBackButton = true;

  navigationItems: NavigationItem[] = [];
  isNavigationAvailable: Observable<boolean>;
  isCouldNavigateToMyClasses: Observable<boolean>;

  constructor(
    public readonly strings: StringsService,
    public readonly currentUserService: CurrentUserService,
    public readonly subTitleProvider: SubTitleProviderService,
    private readonly _logoutOperation: LogoutOperation,
    private readonly _navigationService: NavigationService,
    private readonly _routingService: RoutingService,
    private readonly _changeSelfPasswordOperation: ChangeSelfPasswordOperation
  ) {
    this.isNavigationAvailable = this.currentUserService.userChanges$.pipe(map(user => user?.isSysAdmin() ?? false));
    this.isCouldNavigateToMyClasses = this.currentUserService.userChanges$.pipe(map(user => user?.isSchoolAdmin() ?? false));
  }

  ngOnInit(): void {
    this._navigationService.init();

    // TODO: move to navigation service
    this.navigationItems = [
      new NavigationItem(this.strings.contentMapper, 'content_mapper'),
      new NavigationItem('Content Import', 'content_import'),
      new NavigationItem('Course Import', 'courses_import'),
      new NavigationItem(this.strings.schools, 'schools'),
      new NavigationItem(this.strings.students, 'students'),
      new NavigationItem(this.strings.accessCodes, 'access_codes'),
      new NavigationItem(this.strings.vouchers, 'vouchers'),
      new NavigationItem(this.strings.purchaseReport, 'purchase_report'),
      new NavigationItem(this.strings.usageReport, 'usage_report'),
      new NavigationItem(this.strings.contentReport, 'content_report'),
      new NavigationItem(this.strings.userGuides, 'user_guides'),
      new NavigationItem('Image Library', 'image_library'),
    ];
  }

  onBackClick(): Promise<boolean> {
    return this._navigationService.back();
  }

  onHomeClick(): Promise<boolean> {
    return this._routingService.navigateToHome();
  }

  onMyClasses(): Promise<boolean>{
    return this._routingService.navigateToHome();
  }

  async onLogoutClick(): Promise<void> {
    await this._logoutOperation.perform();
  }

  async onChangePassword(): Promise<void> {
    await this._changeSelfPasswordOperation.perform();
  }
}
