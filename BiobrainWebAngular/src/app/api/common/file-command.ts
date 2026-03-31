import { HttpHeaders } from '@angular/common/http';
import { RequestBase } from './requestBase';

export abstract class FileCommand<TResult> extends RequestBase<TResult> {

  constructor(public file: File){
    super();
  }

  getMethod(): 'get' | 'post' {
    return 'post';
  }

  getHeader(): HttpHeaders | null{
    return null;
  }
}
