import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

import { SubjectsService } from '../../../core/services/subjects/subjects.service';
import { StringsService } from '../../strings.service';
import { ThemeService } from 'src/app/core/app/theme.service';

@Component({
  selector: 'app-teacher-class-icon',
  templateUrl: './teacher-class-icon.component.html',
  styleUrls: ['./teacher-class-icon.component.scss'],
})
export class TeacherClassIconComponent implements OnChanges {
  @Input() schoolClassId?: string;
  @Input() schoolClassYear = 0;
  @Input() schoolClassName = '';
  @Input() courseId?: string;
  @Input() subjectCode?: number;
  @Input() curriculumCode?: number;
  @Input() streak?: number;
  @Output() schoolClassSelected = new EventEmitter();

  public subjectCssClass = '';

  constructor(
    public readonly strings: StringsService,
    public readonly subjectService: SubjectsService,
    private readonly _themeService: ThemeService
  ) {
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (changes.subjectCode) this.subjectCssClass = this._getCssClassForSubject(this.subjectCode);
  }

  public onClick(): void {
    this.schoolClassSelected.emit();
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
      case this.subjectService.sciencePen:
        return 'sciencePen';
      case this.subjectService.biology10Us:
        return 'biology10Us';
      case this.subjectService.earth10Jpc:
        return 'earth10Jpc';
      case this.subjectService.applied10Jpc:
        return 'applied10Jpc';
      default:
        return '';
    }
  }

  getStreakColor(subjectCode: number | undefined) {
    if (!subjectCode) return "000000";
    var colors = this._themeService.GetSubjectColors(subjectCode);
    // console.log(colors);
    return colors.accent;
  }
}
