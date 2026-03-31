import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCurriculaWithCountryRelationQuery extends Query<GetCurriculaWithCountryRelationQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.curricula()}/GetCurriculaWithCountryRelation`;
  }
}

export interface GetCurriculaWithCountryRelationQuery_Result {
  curriculumCode: number;
  orderPriority: number;
  name: string;
  availableCountries: GetCurriculaWithCountryRelationQuery_Country_Result[];
}

export interface GetCurriculaWithCountryRelationQuery_Country_Result {
  name: string;
  curriculumName: string;
  states: string[];
  isExclude: boolean
}
