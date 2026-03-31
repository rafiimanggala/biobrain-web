export class BookmarkModel {
  constructor(
  public bookmarkId: string,
  public materialId: string,
  public nodeId: string,
  public levelId: string,
  public path: string[],
  public header: string,
  ){}
}
