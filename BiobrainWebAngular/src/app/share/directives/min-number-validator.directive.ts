import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator, ValidatorFn } from '@angular/forms';
import { Directive, Input } from '@angular/core';

@Directive({
  selector: '[appMinNumber]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MinNumberValidatorDirective, multi: true }],
})
export class MinNumberValidatorDirective implements Validator {
  @Input('appMinNumber') limit: number | undefined;

  validate(control: AbstractControl): ValidationErrors | null {
    return this.limit === undefined ? null : minNumberValueValidator(this.limit)(control);
  }
}


export function minNumberValueValidator(limit: number): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const numValue = Number(control.value);

    if (numValue < limit || Number.isNaN(numValue)) {
      return {
        lessThanMinValue: {
          value: control.value as unknown,
        },
      };
    }

    return null;
  };
}
