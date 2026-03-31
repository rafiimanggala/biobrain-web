import { HttpHeaders } from '@angular/common/http';
import { ApiPath } from '../api-path.service';

export abstract class RequestBase<TResult> {

  deserializeResult(data: unknown): TResult {
    return data as TResult;
  }

  abstract getUrl(apiPath: ApiPath): string;

  abstract getMethod(): 'get' | 'post';

  abstract getHeader(): HttpHeaders | null;


}
