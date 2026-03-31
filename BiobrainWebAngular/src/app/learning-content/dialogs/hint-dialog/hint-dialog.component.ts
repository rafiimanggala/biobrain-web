import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';

import {
  ContentTypes,
  LearningMaterialContent
} from '../../components/learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';
import { HintDialogData } from './hint-dialog-data';

@Component({
  selector: 'app-hint-dialog',
  templateUrl: './hint-dialog.component.html',
  styleUrls: ['./hint-dialog.component.scss'],
})
export class HintDialogComponent extends DialogComponent<HintDialogData> {
  learningContent: LearningMaterialContent;

  constructor(
    public readonly strings: StringsService,
    matIconRegistry: MatIconRegistry,
    sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public readonly data: HintDialogData
  ) {
    super(data);
    matIconRegistry.addSvgIcon("answer_button", sanitizer.bypassSecurityTrustResourceUrl('../../../../assets/images/answer_button.svg')); 

    this.learningContent = {
      contentType: ContentTypes.glossaryDefinition,
      text: this.data.text
    };
  }

  onClose(): void {
    this.close();
  }
}
