import { Logger } from './logger';

export class ConsoleLogger implements Logger {
  log(method: string, message: string): void {
    console.log(this._formatMessage(method, message));
  }

  error(method: string, message: string, error: Error): void {
    console.log(`${this._formatMessage(method, message)}\r\nFailed due to error:`);
    console.error(error);
  }

  private _formatMessage(method: string, message: string): string {
    return `Method: ${method}\r\nMessage: ${message}`;
  }
}
