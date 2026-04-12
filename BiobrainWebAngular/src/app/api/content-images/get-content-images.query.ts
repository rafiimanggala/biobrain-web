import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetContentImagesQuery extends Query<GetContentImagesResult> {
  constructor(
    public readonly search: string = '',
    public readonly page: number = 1,
    public readonly pageSize: number = 50,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.contentImages()}/GetImages`;
  }
}

export interface GetContentImagesResult {
  readonly images: ContentImageItem[];
  readonly totalCount: number;
}

export interface ContentImageItem {
  readonly imageId: string;
  readonly code: string;
  readonly fileName: string;
  readonly description: string;
  readonly fileLink: string;
  readonly fileSize: number;
  readonly createdAt: string;
}
