/* eslint-disable max-classes-per-file */
export class CreateMaterialDialogData {
  constructor(
    public courseId: string,
    public nodeId: string | null,
    public materialId: string | null,
    public header: string,
    public text: string,
    public videoLink: string
  ) {}
}

export class CreateMaterialDialogResult {
  constructor(
    public readonly materialId: string,
    public readonly header: string,
    public readonly text: string,
    public readonly videoLink: string
  ) {}
}
