import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { Directive, Input } from '@angular/core';

@Directive({
  selector: '[appForbiddenValues]',
  providers: [{ provide: NG_VALIDATORS, useExisting: ForbiddenValidatorDirective, multi: true }],
})
export class ForbiddenValidatorDirective implements Validator {
  @Input('appForbiddenValues') forbiddenList: Set<string> | undefined;

  validate(control: AbstractControl): ValidationErrors | null {
    return this.forbiddenList ? forbiddenValuesValidator(this.forbiddenList)(control) : null;
  }
}


export function forbiddenValuesValidator(forbiddenList: Set<string> | undefined): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    if (forbiddenList === undefined) {
      return null;
    }

    const forbidden = forbiddenList.has(control.value);

    if (forbidden) {
      return {
        forbiddenValue: {
          value: control.value as unknown,
        },
      };
    }

    return null;
  };
}
