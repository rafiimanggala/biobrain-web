export interface Logger {
  log: (method: string, message: string) => void;
  error: (method: string, message: string, error: Error) => void;
}
