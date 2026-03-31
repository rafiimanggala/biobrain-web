import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DomSanitizer } from '@angular/platform-browser';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';

import {
  ContentTypes,
  LearningMaterialContent
} from '../../components/learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';

import { GlossaryTermDialogData } from './glossary-term-dialog-data';

@Component({
  selector: 'app-glossary-term-dialog',
  templateUrl: './glossary-term-dialog.component.html',
  styleUrls: ['./glossary-term-dialog.component.scss'],
})
export class GlossaryTermDialogComponent extends DialogComponent<GlossaryTermDialogData> {
  learningContent: LearningMaterialContent;

  constructor(
    public readonly strings: StringsService,
    private readonly _sanitizer: DomSanitizer,
    @Inject(MAT_DIALOG_DATA) public readonly data: GlossaryTermDialogData
  ) {
    super(data);

    this.learningContent = {
      contentType: ContentTypes.glossaryDefinition,
      text: this.data.text
    };
  }

  onClose(): void {
    this.close();
  }
}
