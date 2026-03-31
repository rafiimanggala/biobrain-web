import { Logger } from './logger';

export class NoneLogger implements Logger {
  log(_method: string, _message: string): void {
  }

  error(_method: string, _message: string, _error: Error): void {
  }
}
