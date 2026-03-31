import { DatePipe, TitleCasePipe } from '@angular/common';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { ServiceWorkerModule } from '@angular/service-worker';

import { environment } from '../environments/environment';

import { AdminModule } from './admin/admin.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { RoutingService } from './auth/services/routing.service';
import { ThemeService } from './core/app/theme.service';
import { CoreModule } from './core/core.module';
import { AppUpdateService } from './core/services/app/app-update.service';
import { Logger } from './core/services/logger';
import { GoogleAnalyticsModule } from './google-analytics/ga.module';
import { LearningContentModule } from './learning-content/learning-content.module';
import { LearningContentLoaderService } from './learning-content/services/learning-content-loader.service';
import { PaymentModule } from './payment/payment.module';
import { MaterialModule } from './share/material.module';
import { SendLogsOperation } from './share/operations/send-logs.operation';
import { SharedModule } from './share/shared.module';
import { StringsService } from './share/strings.service';
import { StudentModule } from './student/student.module';
import { TeachersModule } from './teachers/teachers.module';


const components = [AppComponent];

const dialogs: any[] = [];

const operations: any[] = [SendLogsOperation];

const services = [
  StringsService,
  RoutingService,
  AppUpdateService,
  Logger
];

const pipes = [TitleCasePipe, DatePipe];

@NgModule({
  declarations: [
    dialogs,
    components
  ],
  imports: [
    AppRoutingModule,
    SharedModule,
    MaterialModule,
    AuthModule,
    CoreModule,
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production, registrationStrategy: 'registerImmediately' }),
    StudentModule,
    AdminModule,
    TeachersModule,
    PaymentModule,
    LearningContentModule,
    GoogleAnalyticsModule.forRoot(),
  ],
  providers: [
    operations,
    services,
    pipes,
    {
      provide: APP_INITIALIZER,
      useFactory: (_: LearningContentLoaderService) => () => undefined,
      deps: [LearningContentLoaderService],
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: (_: ThemeService) => () => undefined,
      deps: [ThemeService],
      multi: true
    }
  ],
  entryComponents: [dialogs],
  bootstrap: [AppComponent]
})
export class AppModule { }
