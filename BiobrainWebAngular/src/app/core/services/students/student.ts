export class Student {
  constructor(
    public readonly studentId: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly email: string,
    public readonly schoolId: string | null | undefined,
    public readonly schoolClassIds: string[]
  ) {
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}
