/* eslint-disable max-classes-per-file */
export enum ResultCode {
  Running,
  Success,
  Failed
}

export abstract class Result<TData, TReason> {
  protected constructor(protected readonly resultCode: ResultCode) {
  }

  public static running(): RunningResult {
    return new RunningResult();
  }

  public static success(): SuccessResult<undefined>;
  public static success<T>(data: T): SuccessResult<T>;
  public static success<T>(data?: T): SuccessResult<T> | SuccessResult<undefined> {
    return data !== undefined ? new SuccessResult(data) : new SuccessResult(undefined);
  }

  public static failed(): FailedResult<undefined>;
  public static failed<T>(reason: T): FailedResult<T>;
  public static failed<T>(reason?: T): FailedResult<T> | FailedResult<undefined> {
    return reason !== undefined ? new FailedResult(reason) : new FailedResult(undefined);
  }

  public isRunning(): this is RunningResult {
    return this.resultCode === ResultCode.Success;
  }

  public isSuccess(): this is SuccessResult<TData> {
    return this.resultCode === ResultCode.Success;
  }

  public isFailed(): this is FailedResult<TReason> {
    return this.resultCode === ResultCode.Failed;
  }
}

export class RunningResult extends Result<undefined, undefined> {
  constructor() {
    super(ResultCode.Running);
  }
}

export class SuccessResult<TData> extends Result<TData, undefined> {
  constructor(public readonly data: TData) {
    super(ResultCode.Success);
  }
}

export class FailedResult<TReason> extends Result<undefined, TReason> {
  constructor(public readonly reason: TReason) {
    super(ResultCode.Failed);
  }
}

export type SuccessOrFailedResult<Data = undefined, Reason = undefined> = SuccessResult<Data> | FailedResult<Reason>;

export type FailedOrSuccessResult<Reason = undefined, Data = undefined> = SuccessResult<Data> | FailedResult<Reason>;

export type SuccessOrRunningResult<Data = undefined> = RunningResult | SuccessResult<Data>;
