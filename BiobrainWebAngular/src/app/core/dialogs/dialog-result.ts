import { hasValue } from '../../share/helpers/has-value';

import { DialogAction } from './dialog-action';

export class DialogResult<TResult> {
  constructor(public readonly action: DialogAction, public readonly data: TResult) {
  }

  public static hasData<T>(dialogResult: DialogResult<T>): dialogResult is DialogResult<NonNullable<T>> {
    return dialogResult.hasData();
  }

  public hasData(): this is DialogResult<NonNullable<TResult>> {
    return hasValue(this.data);
  }
}
