import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';

import { Course } from '../../../core/services/courses/course';
import { SubjectsService } from '../../../core/services/subjects/subjects.service';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-student-course-icon',
  templateUrl: './student-course-icon.component.html',
  styleUrls: ['./student-course-icon.component.scss'],
})
export class StudentCourseIconComponent implements OnInit, OnChanges {
  @Input() course?: Course;
  @Output() courseSelected = new EventEmitter();

  public subjectCssClass = '';

  constructor(
    public readonly strings: StringsService,
    public readonly subjectService: SubjectsService,
  ) {
  }

  ngOnInit(): void {
    this._bindCssClassForSubject();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.course) this._bindCssClassForSubject();
  }

  public onClick(): void {
    this.courseSelected.emit();
  }

  private _bindCssClassForSubject(): void {
    this.subjectCssClass = this._getCssClassForSubject(this.course?.subjectCode);
  }

  private _getCssClassForSubject(subjectCode: number | undefined): string {
    switch (subjectCode) {
      case this.subjectService.biology:
        return 'biology';
      case this.subjectService.chemistry:
        return 'chemistry';
      case this.subjectService.physics:
        return 'physics';
      case this.subjectService.biology10:
        return 'biology10';
      case this.subjectService.chemistry10:
        return 'chemistry10';
      case this.subjectService.forensics:
        return 'forensics';
      case this.subjectService.psychology:
        return 'psychology';
      case this.subjectService.physics10:
        return 'physics10';
      case this.subjectService.marine10:
        return 'marine10';
      case this.subjectService.science:
        return 'science';
      case this.subjectService.live:
        return 'live';
      default:
        return '';
    }
  }
}
