import { ErrorHandler, NgModule } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Dialog } from './dialogs/dialog.service';
import { BaseErrorHandler } from './error-handling/base-error-handler.service';
import { RouteDataProvider } from './route/route-data.provider';
import { BookmarksService } from './services/bookmarks/bookmarks.service';
import { AnalyticsService } from './services/google-analitics-state.service';
import { SidenavService } from './services/side-nav.service';
import { StudentProfileService } from './services/students/student-profile.service';

const services = [SidenavService,  AnalyticsService, StudentProfileService, BookmarksService];

@NgModule({
  providers: [
    {
      provide: ErrorHandler,
      useClass: BaseErrorHandler,
    },
    Title,
    Dialog,
    {
      provide: RouteDataProvider,
      useClass: RouteDataProvider,
      multi: false,
    },
    services,
  ],
})
export class CoreModule {
}
