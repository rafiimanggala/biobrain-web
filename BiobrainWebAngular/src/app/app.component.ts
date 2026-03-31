import { Component } from '@angular/core';
import { AppUpdateService } from './core/services/app/app-update.service';
 


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  constructor(private readonly appUpdateService: AppUpdateService){
    // console.log(`Application version is: ${versionInfo()}`);
  }
}
