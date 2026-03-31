import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { MatExpansionPanel } from '@angular/material/expansion';

import { LearningMaterialContent } from '../learning-material-shadow-dom-node/learning-material-shadow-dom-node.component';

@Component({
  selector: 'app-term-expansion-panel',
  templateUrl: './term-expansion-panel.component.html',
  styleUrls: ['./term-expansion-panel.component.scss'],
})
export class TermExpansionPanelComponent implements AfterViewInit {
  @Input() header: string | undefined;
  @Input() isFirst = false;
  @Input() learningContent: LearningMaterialContent | undefined;

  @ViewChild('expansionPanel') expansionPanel: MatExpansionPanel | undefined;

  ngAfterViewInit(): void {
    setTimeout(() => this.expansionPanel?.open(), 1);
  }
}
