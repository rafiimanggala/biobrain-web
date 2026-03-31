import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetClassResultQuery extends Query<GetClassResultQuery_Result> {
  constructor(
    public readonly courseId: string, public readonly schoolClassId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizzesAnalytic()}/GetClassResultsCsv`;
  }

  deserializeResult(data: GetClassResultQuery_Result_Object): GetClassResultQuery_Result {
    return new GetClassResultQuery_Result(
      data.fileUrl,
    );
  }
}

export class GetClassResultQuery_Result {
  constructor(
    public readonly fileUrl: string,
  ) {
  }
}

interface GetClassResultQuery_Result_Object {
  fileUrl: string;
}
