import { AbstractControl, AsyncValidator, NG_ASYNC_VALIDATORS, ValidationErrors } from '@angular/forms';
import { Directive, Input } from '@angular/core';
import { Observable, of } from 'rxjs';

import { map } from 'rxjs/operators';
import { Api } from '../../api/api.service';
import { GetSchoolClassesListQuery } from '../../api/school-classes/get-school-classes-list.query';

@Directive({
  selector: '[schoolClassNameValidator]',
  providers: [{ provide: NG_ASYNC_VALIDATORS, useExisting: ClassNameValidatorDirective, multi: true }],
})
export class ClassNameValidatorDirective implements AsyncValidator {
  @Input('schoolClassNameValidator') schoolId: string | undefined;
  @Input('schoolClassNameValidator_Year') year: number | undefined | null;
  @Input('schoolClassNameValidator_ExcludeClassId') excludeFromValidationClassId: string | undefined | null;

  constructor(private readonly _api: Api) {}

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    if (!this.schoolId) {
      return of(null);
    }

    if (!this.year) {
      return of(null);
    }

    return this._api.send(new GetSchoolClassesListQuery(this.schoolId)).pipe(
      map(x => {
        const namesToCheck = new Set<string>(
          x.filter(s => !this.excludeFromValidationClassId || this.excludeFromValidationClassId !== s.schoolClassId)
            .filter(s => s.year === this.year)
            .map(s => s.name)
        );

        if (namesToCheck.has(control.value as string)) {
          return {
            duplicatedName: {
              value: control.value as string,
            },
          };
        }

        return null;
      })
    );
  }
}
