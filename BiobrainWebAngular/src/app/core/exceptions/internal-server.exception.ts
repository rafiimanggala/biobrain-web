export class InternalServerException implements Error {
  constructor(public message: string) {}
  name!: string;
  stack?: string;
}

