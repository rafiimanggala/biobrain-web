import { Component } from '@angular/core';
import { AnalyticsService } from 'src/app/core/services/google-analitics-state.service';

@Component({
  selector: 'app-base-master-page-component',
  templateUrl: './base-master-page.component.html',
  styleUrls: ['./base-master-page.component.scss'],
})
export class BaseMasterPage {
  constructor(
    private analyticsService: AnalyticsService,){
      analyticsService.initAnalytics();
    }
}
