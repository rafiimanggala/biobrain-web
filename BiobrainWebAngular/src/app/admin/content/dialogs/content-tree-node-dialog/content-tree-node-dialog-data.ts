/* eslint-disable max-classes-per-file */
export class ContentTreeNodeDialogData {
  constructor(
    public entityId: string,
    public courseId: string,
    public parentId: string | null,
    public header: string,
    public order: number,
    public levelHeader: string,
    public isAvailableInDemo: boolean
  ) {}
}

export class ContentTreeNodeDialogResult extends ContentTreeNodeDialogData {
  constructor(
    public readonly result: Result,
    entityId: string,
    courseId: string,
    parentId: string | null,
    header: string,
    order: number,
    levelHeader: string,
    isAvailableInDemo: boolean
  ) {
    super(entityId, courseId, parentId, header, order, levelHeader, isAvailableInDemo);
  }

  static deleteResult(data: ContentTreeNodeDialogData): ContentTreeNodeDialogResult {
    return new ContentTreeNodeDialogResult(
      Result.delete,
      data.entityId,
      data.courseId,
      data.parentId,
      data.header,
      data.order,
      data.levelHeader,
      data.isAvailableInDemo
    );
  }

  static updateResult(data: ContentTreeNodeDialogData): ContentTreeNodeDialogResult {
    return new ContentTreeNodeDialogResult(
      Result.update,
      data.entityId,
      data.courseId,
      data.parentId,
      data.header,
      data.order,
      data.levelHeader,
      data.isAvailableInDemo
    );
  }
}

export enum Result {
  delete,
  update,
}
