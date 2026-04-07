import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RequestHandlingService {
  glossaryTermSelected$ = new Subject<string>();
  answerValueSelected$ = new Subject<string>();
  swichBookmark$ = new Subject<{sender: HTMLImageElement, materialId: string}>();
  switchExcludedMaterial$ = new Subject<{materialId: string}>();

  handle(data: {ev: any, request: string}): void {
    if (data.request.startsWith('glossary.')) {
      this.glossaryTermSelected$.next(data.request.replace('glossary.', ''));
    }

    if (data.request.startsWith('answer.')) {
      this.answerValueSelected$.next(data.request.replace('answer.', ''));
    }

    if (data.request.startsWith('bookmark.')) {
      var img = data.ev.composedPath()[0] as HTMLImageElement;
      this.swichBookmark$.next({sender: img, materialId: data.request.replace( 'bookmark.', '')});
    }

    if (data.request.startsWith('excludeMaterial.')) {
      this.switchExcludedMaterial$.next({materialId: data.request.replace('excludeMaterial.', '')});
    }
  }
}
