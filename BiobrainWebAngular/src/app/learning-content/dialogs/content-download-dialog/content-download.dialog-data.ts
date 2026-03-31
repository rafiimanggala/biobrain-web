import { HttpEvent, HttpEventType } from "@angular/common/http";
import { Observable, Subscription } from "rxjs";
import { catchError, filter, last, map, tap } from "rxjs/operators";
import { GetCourseContentDataQuery_Result } from "src/app/api/content/get-course-content-data.query";
import { hasValue } from "src/app/share/helpers/has-value";
import { Content, ContentVersionRow } from "../../services/learning-content-db";

export class ContentDownloadData {
  progressPercent: number = 0;
  isError: boolean = false;
  isComplete: boolean = false;
  subscription: Subscription;
  content?: Content;

  constructor(
    public courseId: string,
    public courseName: string,
    public observer: Observable<HttpEvent<GetCourseContentDataQuery_Result>>,
    public localVersion: ContentVersionRow | undefined
  ) {
    this.subscription = observer.pipe(
      map(event => this.getEventMessage(event)),
      filter(message => hasValue(message) && message.length > 0),
      last(),
      catchError((e) => { this.isError = true; return e; })
    ).subscribe(this.unsubscribe);
  }

  private getEventMessage(event: HttpEvent<any>) {
    switch (event.type) {
      case HttpEventType.DownloadProgress:
        // Compute and show the % done:
        this.progressPercent = Math.round(100 * event.loaded / (event.total ?? 1));
        return `${this.courseName} - loaded: ${event.loaded}, total: ${event} `;

      case HttpEventType.Response:
        this.content = event.body;
        console.log(`File "${this.courseName}" was completely downloaded!`);
        return `File "${this.courseName}" was completely downloaded!`;

      default:
        return ``;
    }
  }

  unsubscribe() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  public complete(){    
    this.isComplete = true;
  };
}
