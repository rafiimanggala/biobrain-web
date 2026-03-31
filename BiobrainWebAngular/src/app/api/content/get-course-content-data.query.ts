import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

import { ContentTree, GlossaryTerm, MaterialPage, Quiz } from './content-data-models';

export class GetCourseContentDataQuery extends Query<GetCourseContentDataQuery_Result> {
  constructor(
    public courseId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourseContentData`;
  }
}


export interface GetCourseContentDataQuery_Result {
  readonly pages: MaterialPage[];
  readonly quizzes: Quiz[];
  readonly contentForest: ContentTree[];
  readonly glossaryTerms: GlossaryTerm[];
}
