import { Component, OnInit } from '@angular/core';
import { map, startWith } from 'rxjs/operators';

import { CreateSchoolClassService } from '../services/create-school-class.service';
import { DeleteSchoolClassService } from '../services/delete-school-class.service';
import { GetSchoolDataFromRouteService } from '../../services/get-school-data-from-route.service';
import { SchoolClassListStore } from '../school-class-grid/school-class-list-store';
import { StringsService } from 'src/app/share/strings.service';
import { UpdateSchoolClassService } from '../services/update-school-class.service';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'app-school-class-list',
  templateUrl: './school-class-list.component.html',
  styleUrls: ['./school-class-list.component.scss'],
  providers: [
    SchoolClassListStore,
    CreateSchoolClassService,
    UpdateSchoolClassService,
    DeleteSchoolClassService
  ]
})
export class SchoolClassListComponent implements OnInit {
  constructor(
    public readonly strings: StringsService,
    private readonly _getSchoolDataFromRouteService: GetSchoolDataFromRouteService,
    private readonly _schoolClassListStore: SchoolClassListStore,
    private readonly _createSchoolClassService: CreateSchoolClassService,
    private readonly _updateSchoolClassService: UpdateSchoolClassService,
    private readonly _deleteSchoolClassService: DeleteSchoolClassService,
  ) { }

  ngOnInit(): void {
    this._schoolClassListStore.attachBinding(
      this._getSchoolDataFromRouteService.getSchoolId().pipe(
        map(schoolId => ({ schoolId }))
      )
    );

    this._schoolClassListStore.attachReload(
      combineLatest([
        this._createSchoolClassService.createdClass$.pipe(startWith({})),
        this._updateSchoolClassService.updateSchoolClass$.pipe(startWith({})),
        this._deleteSchoolClassService.deletedSchoolClass$.pipe(startWith({})),
      ])
    );
  }

  onCreateSchoolClass(): void {
    this._createSchoolClassService.perform(this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  onEditSchoolClass(schoolClassId: string): void {
    this._updateSchoolClassService.perform(schoolClassId);
  }

  onDeleteSchoolClass(schoolClassId: string): void {
    this._deleteSchoolClassService.perform(schoolClassId);
  }
}
