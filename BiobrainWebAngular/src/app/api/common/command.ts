import { HttpHeaders } from '@angular/common/http';
import { RequestBase } from './requestBase';

export abstract class Command<TResult> extends RequestBase<TResult> {
  getMethod(): 'get' | 'post' {
    return 'post';
  }

  getHeader(): HttpHeaders | null{
    return null;
  }
}
