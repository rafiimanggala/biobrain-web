import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

import { MaterialPage, Quiz, ContentTree, GlossaryTerm } from './content-data-models';

export class GetContentDataQuery extends Query<GetContentDataQuery_Result> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetContentData`;
  }
}

export interface GetContentDataQuery_Result {
  readonly pages: MaterialPage[];
  readonly quizzes: Quiz[];
  readonly contentForest: ContentTree[];
  readonly glossaryTerms: GlossaryTerm[];
}
