export class SchoolClassCacheModel {
  public readonly fullName: string;

  constructor(
    public readonly schoolClassId: string,
    public readonly schoolId: string,
    public readonly name: string,
    public readonly year: number,
    public readonly autoJoinClassCode: string | null | undefined
  ) {
    this.fullName = `${year} ${name}`;
  }
}
