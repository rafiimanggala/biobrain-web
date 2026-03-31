import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { RoutingService } from '../../../auth/services/routing.service';
import { CoursesService } from '../../../core/services/courses/courses.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';

@Component({
  selector: 'app-page-bradscrumbs',
  templateUrl: './page-bradscrumbs.component.html',
  styleUrls: ['./page-bradscrumbs.component.scss']
})
export class PageBreadcrumbsComponent implements OnChanges {
  @Input() courseId: string | undefined;
  @Input() pageName: string | undefined;

  public subjectName: string | undefined;
  public subHeader: string | undefined | null;

  constructor(
    private readonly _coursesService: CoursesService,
    private readonly _routingService: RoutingService,
  ) {
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (changes.courseId) await this._setSubjectNameInternal();
  }

  public async onRouterLinkClick(): Promise<void> {
    await this._routingService.navigateToHome();
  }

  private async _setSubjectNameInternal(): Promise<void> {
    if (!hasValue(this.courseId)) return undefined;

    const course = await firstValueFrom(this._coursesService.findById(this.courseId));
    if (!hasValue(course)) return undefined;

    this.subjectName = course.subject.name;
    this.subHeader = course.subHeader;
  }

  onCourseClick() {
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }
}
