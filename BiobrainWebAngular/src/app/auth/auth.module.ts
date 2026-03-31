import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';

import { MaterialModule } from '../share/material.module';
import { SharedModule } from '../share/shared.module';

import { AuthRoutingModule } from './auth-routing.module';
import { ChangeEmailDialogComponent } from './dialogs/change-email-dialog/change-email-dialog.component';
import { ChangeSelfPasswordDialogComponent } from './dialogs/change-self-password-dialog/change-self-password-dialog.component';
import { ChangePasswordDialogComponent } from './dialogs/change-password-dialog/change-password-dialog.component';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { JwtInterceptor } from './interceptors/jwt.itnterceptor';
import { LoginOperation } from './operations/login.operation';
import { LogoutOperation } from './operations/logout.operation';
import { RefreshAuthorizationOperation } from './operations/refresh-authorization.operation';
import { HomeComponent } from './views/home/home.component';
import { LoginComponent } from './views/login/login.component';
import { ResetPasswordPageComponent } from './views/reset-password-page/reset-password-page.component';
import { SetPasswordPageComponent } from './views/set-password-page/set-password-page.component';
import { SignUpDetailsComponent } from './views/sign-up-details/sign-up-details.component';
import { SignUpComponent } from './views/sign-up/sign-up.component';
import { ChangeSelfEmailDialogComponent } from './dialogs/change-self-email-dialog/change-self-email-dialog.component';
import { AdminLoginComponent } from './views/admin-login/admin-login.component';
import { AdminLoginOperation } from './operations/admin-login.operation';

const components = [
  LoginComponent,
  HomeComponent,
  SignUpComponent,
  SignUpDetailsComponent,
  ResetPasswordPageComponent,
  SetPasswordPageComponent,
  AdminLoginComponent
];

const dialogs = [
  ChangeSelfPasswordDialogComponent,
  ChangeEmailDialogComponent,
  ChangeSelfEmailDialogComponent,
  ChangePasswordDialogComponent
];

const operations = [
  LoginOperation,
  LogoutOperation,
  RefreshAuthorizationOperation,
  AdminLoginOperation
];

@NgModule({
  imports: [
    AuthRoutingModule,
    MaterialModule,
    SharedModule,
  ],
  declarations: [
    components,
    dialogs,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
    operations,
  ],
  entryComponents: [dialogs],
})
export class AuthModule {
  // static forRoot() {
  //     return {
  //         ngModule: AuthModule,
  //         providers: [
  //             { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  //             { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  //             AuthApi,
  //             UserService,
  //             PermissionService
  //         ],
  //         imports: [
  //             CoreModule
  //         ],
  //         exports: [
  //             UserService,
  //             PermissionService
  //         ]
  //     };
  // }
}
