import { Component, OnDestroy, OnInit } from '@angular/core';

import { LoaderService } from '../../services/loader.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-loader',
  templateUrl: './loader.component.html',
  styleUrls: ['./loader.component.scss'],
})
export class LoaderComponent implements OnInit, OnDestroy {
  isLoading = false;
  private _subscription: Subscription | undefined;

  constructor(private readonly _loaderService: LoaderService) {}

  ngOnInit(): void {
    this._subscription = this._loaderService.loaderState$.subscribe(state => {
      this.isLoading = state.isLoading;
    });
  }

  ngOnDestroy(): void {
    this._subscription?.unsubscribe();
  }
}
