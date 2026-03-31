import { Component, OnInit } from '@angular/core';
import { LogoutOperation } from 'src/app/auth/operations/logout.operation';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { NavigationService } from 'src/app/core/services/navigation.service';
import { LoaderService } from 'src/app/share/services/loader.service';

@Component({
  selector: 'app-materials-clear',
  templateUrl: './materials-clear.component.html',
  styleUrls: ['./materials-clear.component.scss'],
})
export class MaterialsClearComponent implements OnInit {

  constructor(private navigation: RoutingService, private loader: LoaderService, private _logoutOperation: LogoutOperation){}

  async ngOnInit() {
    var dbs: {name:string}[] = await (window.indexedDB as any).databases();
    if(dbs.some(_ => _.name == "LearningContentDb")){
      indexedDB.deleteDatabase("LearningContentDb");
    }
    localStorage.clear();
    this.navigation.navigateToLoginPage();
  }

 }

