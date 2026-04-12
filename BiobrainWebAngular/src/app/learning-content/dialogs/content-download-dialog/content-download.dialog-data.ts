import { HttpEvent, HttpEventType } from "@angular/common/http";
import { Observable, Subscription } from "rxjs";
import { GetCourseContentDataQuery_Result } from "src/app/api/content/get-course-content-data.query";
import { Content, ContentVersionRow } from "../../services/learning-content-db";

export class ContentDownloadData {
  progressPercent: number = 0;
  isError: boolean = false;
  isComplete: boolean = false;
  subscription!: Subscription;
  content?: Content;
  readonly ready: Promise<void>;

  constructor(
    public courseId: string,
    public courseName: string,
    public observer: Observable<HttpEvent<GetCourseContentDataQuery_Result>>,
    public localVersion: ContentVersionRow | undefined
  ) {
    this.ready = new Promise<void>((resolve) => {
      this.subscription = observer.subscribe({
        next: (event) => this.handleEvent(event),
        error: (e) => {
          console.error(`Download failed for course "${this.courseName}"`, e);
          this.isError = true;
          resolve();
        },
        complete: () => resolve(),
      });
    });
  }

  private handleEvent(event: HttpEvent<any>): void {
    switch (event.type) {
      case HttpEventType.DownloadProgress:
        this.progressPercent = Math.round(100 * event.loaded / (event.total ?? 1));
        return;

      case HttpEventType.Response:
        this.content = event.body;
        console.log(`File "${this.courseName}" was completely downloaded!`);
        return;
    }
  }

  unsubscribe() {
    if (this.subscription) this.subscription.unsubscribe();
  }

  public complete(){
    this.isComplete = true;
  };
}
