import moment from 'moment';
import { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCourseReleasesListQuery extends Query<GetCourseReleasesListQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourseReleases`;
  }

  deserializeResult(data: GetCourseReleasesListQuery_Result_Object[]): GetCourseReleasesListQuery_Result[] {
    return data.map(model => GetCourseReleasesListQuery_Result.deserialize(model));
  }
}

export class GetCourseReleasesListQuery_Result {
  constructor(
    public readonly courseId: string,
    public readonly name: string,
    public readonly lastUploadDateTimeUtc: Moment,
    public readonly lastReleaseDateTimeUtc: Moment,
  ) {
  }

  static deserialize(
    obj: GetCourseReleasesListQuery_Result_Object
  ):GetCourseReleasesListQuery_Result {
    return new GetCourseReleasesListQuery_Result(
      obj.courseId,
      obj.name,
      moment.utc(obj.lastUploadDateTimeUtc),
      moment.utc(obj.lastReleaseDateTimeUtc),
    );
  }
}

export interface GetCourseReleasesListQuery_Result_Object {
  courseId: string;
  name: string;
  lastUploadDateTimeUtc: string;
  lastReleaseDateTimeUtc: string;
}
