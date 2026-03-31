import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { RoutingService } from '../../../auth/services/routing.service';

import { ContentTreeService } from '../../../core/services/content/content-tree.service';
import { CoursesService } from '../../../core/services/courses/courses.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';

@Component({
  selector: 'app-learning-material-breadcrumbs',
  templateUrl: './learning-material-breadcrumbs.component.html',
  styleUrls: ['./learning-material-breadcrumbs.component.scss']
})
export class LearningMaterialBreadcrumbsComponent implements OnChanges {
  @Input() courseId: string | undefined;
  @Input() nodeId: string | undefined;
  @Input() className: string | null | undefined;

  public subjectName: string | undefined;
  public subHeader: string | undefined | null;
  public path: string[] | undefined;

  constructor(
    private readonly _coursesService: CoursesService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
  ) {
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (changes.courseId || changes.className) this._setSubjectNameInternal();
    if (changes.nodeId) this.path = await this._getPath();
  }

  public async onRouterLinkClick(): Promise<void> {
    await this._routingService.navigateToHome();
  }

  private async _setSubjectNameInternal(): Promise<void> {
    if (hasValue(this.className)) {
      this.subjectName = this.className;
      this.subHeader = null;
      return;
    }

    if (!hasValue(this.courseId)) return undefined;

    const course = await firstValueFrom(this._coursesService.findById(this.courseId));
    if (!hasValue(course)) return undefined;

    this.subjectName = course.subject.name;
    this.subHeader = course.subHeader;
  }

  private async _getPath(): Promise<string[]> {
    if (!hasValue(this.nodeId)) return [];

    const node = await firstValueFrom(this._contentTreeService.findNode(this.nodeId));
    if (!hasValue(node)) return [];

    return node.namePath;
  }
}
