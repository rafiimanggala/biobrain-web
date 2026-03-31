import { Component } from '@angular/core';
import { ITooltipComp, ITooltipParams } from 'ag-grid-community';
import { QuestionTextService } from '../../services/question-text.service';

@Component({
  selector: 'app-question-tooltip-renderer',
  templateUrl: './question-tooltip-renderer.component.html',
  styleUrls: ['./question-tooltip-renderer.component.scss'],
})
export class QuestionTooltipRendererComponent implements ITooltipComp {

  eGui: any;
    init(params: ITooltipParams & { color: string }) {
      // const host = document.querySelector("#host");
      // if(!host) return;
      // const shadow = host.attachShadow({ mode: "open" });
        const eGui = (this.eGui = document.createElement('div'));
        const color = params.color || '#999';

        eGui.classList.add('custom-question-tooltip');
        //@ts-ignore
        // eGui.style['background-color'] = color;
        eGui.style['position'] = 'absolute';
        const shadow = eGui.attachShadow({ mode: "open" });
        shadow.innerHTML = `${params.value}`;
    }

    getGui() {
        return this.eGui;
    }
 }

