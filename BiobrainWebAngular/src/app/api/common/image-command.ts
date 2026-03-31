import { HttpHeaders } from '@angular/common/http';
import { FileCommand } from './file-command';

export abstract class ImageCommand<TResult> extends FileCommand<TResult> {

  constructor(public file: File){
    super(file);
  }

  getMethod(): 'get' | 'post' {
    return 'post';
  }

  getHeader(): HttpHeaders | null{
    return new HttpHeaders().set('Content-Type', `${this.file.type}`);;
  }
}
