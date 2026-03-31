import { Component, OnInit } from '@angular/core';

import { RoutingService } from '../../services/routing.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {

  constructor(
    private readonly _routingService: RoutingService,
  ) {
  }

  ngOnInit(): void {
    this._routingService.navigateToHome();
  }
}
