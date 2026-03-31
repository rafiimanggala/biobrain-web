import { HttpHeaders } from '@angular/common/http';
import { RequestBase } from './requestBase';

export abstract class Query<TResult> extends RequestBase<TResult> {
  getMethod(): 'get' | 'post' {
    return 'get';
  }

  getHeader(): HttpHeaders | null{
    return null;
  }
}
