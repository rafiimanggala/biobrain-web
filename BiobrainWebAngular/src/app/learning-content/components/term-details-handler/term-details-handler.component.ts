import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { Dialog } from '../../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { LearningContentProviderService } from '../../services/learning-content-provider.service';
import { GlossaryTermDialogData } from '../../dialogs/glossary-term-dialog/glossary-term-dialog-data';
import { GlossaryTermDialogComponent } from '../../dialogs/glossary-term-dialog/glossary-term-dialog.component';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-term-details-handler',
  templateUrl: './term-details-handler.component.html',
  styleUrls: ['./term-details-handler.component.scss'],
})
export class TermDetailsHandlerComponent implements OnDestroy {
  private readonly _subscriptions: Subscription[] = [];

  constructor(
    private readonly _strings: StringsService,
    private readonly _requestService: RequestHandlingService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
  ) {
    this._subscriptions.push(
      this._requestService.glossaryTermSelected$.subscribe({
        next: term => this._openGlossaryDialog(term),
      }),
    );
  }

  public ngOnDestroy(): void {
    this._subscriptions.forEach(x => x.unsubscribe());
  }

  private async _openGlossaryDialog(termRef: string): Promise<void> {
    if (!termRef) {
      return;
    }

    const course = await firstValueFrom(this._activeCourseService.courseChanges$);
    if (!course) {
      this._appEvents.errorEmit(this._strings.errors.termNotFound);
      return;
    }

    const term = await this._learningContentProvider.getTermByRef(termRef, course.subjectCode);
    if (!term) {
      this._appEvents.errorEmit(this._strings.errors.termNotFound);
      return;
    }

    const dialogData = new GlossaryTermDialogData(term.header ? term.header : term.term, term.definition);
    await this._dialog.show(GlossaryTermDialogComponent, dialogData, {panelClass: "glossary-dialog-panel"});
  }
}
