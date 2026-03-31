import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { Directive, Input } from '@angular/core';

@Directive({
  selector: '[appMaxNumber]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MaxNumberValidatorDirective, multi: true }],
})
export class MaxNumberValidatorDirective implements Validator {
  @Input('appMaxNumber') limit: number | undefined;

  validate(control: AbstractControl): ValidationErrors | null {
    return this.limit === undefined ? null : maxNumberValueValidator(this.limit)(control);
  }
}


export function maxNumberValueValidator(limit: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const numValue = Number(control.value);

    if (numValue >= limit || Number.isNaN(numValue)) {
      return {
        greaterThenOrEqualToMaxValue: {
          value: control.value as unknown,
        },
      };
    }

    return null;
  };
}
