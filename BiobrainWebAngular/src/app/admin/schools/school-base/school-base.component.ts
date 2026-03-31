import { Component, OnInit } from '@angular/core';

import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { GetSchoolDataFromRouteService } from '../../services/get-school-data-from-route.service';
import { NavigationItem } from 'src/app/core/models/navigation-item';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-school-base',
  templateUrl: './school-base.component.html',
  styleUrls: ['./school-base.component.scss'],
  providers: [GetSchoolDataFromRouteService],
})
export class SchoolBaseComponent extends DisposableSubscriberComponent implements OnInit {
  navItems: NavigationItem[];

  constructor(
    private readonly _strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _getSchoolDataFromRouteService: GetSchoolDataFromRouteService,
  ) {
    super();

    this.navItems = [
      new NavigationItem(this._strings.teachers, 'teachers'),
      new NavigationItem(this._strings.classes, 'classes'),
      new NavigationItem(this._strings.students, 'students'),
    ];
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this._getSchoolDataFromRouteService
        .getSchoolName()
        .pipe(map(x => `${this._titlecasePipe.transform(this._strings.school)} - ${x}`))
        .subscribe(x => {
          this._subTitleProvider.subTitleSubject.next(x);
        })
    );
  }
}
