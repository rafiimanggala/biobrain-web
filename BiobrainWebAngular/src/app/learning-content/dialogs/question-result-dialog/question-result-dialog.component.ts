import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';

import { QuestionResultData } from '../../components/question-result-shadow-dom-node/question-result-shadow-dom-node.component';

import { QuestionResultDialogData } from './question-result-dialog-data';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-question-result-dialog',
  templateUrl: './question-result-dialog.component.html',
  styleUrls: ['./question-result-dialog.component.scss'],
})
export class QuestionResultDialogComponent extends DialogComponent<QuestionResultDialogData> {
  questionResult: QuestionResultData;
  header: string;

  constructor(
    public readonly strings: StringsService,
    private matIconRegistry: MatIconRegistry,
    private readonly sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public readonly data: QuestionResultDialogData,
  ) {
    super(data);
    this.matIconRegistry.addSvgIcon("answer_button", this.sanitizer.bypassSecurityTrustResourceUrl('../../../../assets/images/answer_button.svg')); 

    this.questionResult = {
      question: this.dialogData.question,
      isCorrect: this.dialogData.isCorrect,
      isSecondTry: this.dialogData.isSecondTry
    };

    this.header = this.data.isCorrect ? this.strings.correct : this.strings.incorrect;
  }

  onClose(): void {
    this.close();
  }
}
