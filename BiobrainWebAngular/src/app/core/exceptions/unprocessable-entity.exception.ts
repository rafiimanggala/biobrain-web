export class UnprocessableEntityException implements Error {
  constructor(public message: string, public name: string) {
  }
}
