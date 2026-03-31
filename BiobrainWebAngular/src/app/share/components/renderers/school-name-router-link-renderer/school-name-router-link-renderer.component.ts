import { Component, NgZone } from '@angular/core';

import { AgRendererComponent } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import { Router } from '@angular/router';

@Component({
  template: '<a style="color: black; text-decoration: none;" *ngIf="inRouterLink" [routerLink]="[inRouterLink]" (click)="navigate()">{{params?.value}}</a>',
})
export class SchoolNameRouterLinkRendererComponent implements AgRendererComponent {
  params: ICellRendererParams | undefined;

  get inRouterLink(): string {
    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
    const schoolId = this.params?.data?.schoolId as string ?? '';
    return `admin/schools/${schoolId}/teachers`;
  }

  constructor(
    private readonly _ngZone: NgZone,
    private readonly _router: Router
  ) {}

  agInit(params: ICellRendererParams): void {
    this.params = params;
  }

  refresh(): boolean {
    return false;
  }

  navigate(): Promise<boolean> {
    return this._ngZone.run(() => this._router.navigate([this.inRouterLink]));
  }
}
