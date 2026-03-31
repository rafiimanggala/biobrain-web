import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ResetPasswordPageComponent } from './views/reset-password-page/reset-password-page.component';
import { LoginComponent } from './views/login/login.component';
import { SetPasswordPageComponent } from './views/set-password-page/set-password-page.component';
import { SignUpComponent } from './views/sign-up/sign-up.component';
import { UserRoles } from '../share/values/user-roles.enum';
import { AdminLoginComponent } from './views/admin-login/admin-login.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'sign-up',
    component: SignUpComponent
  },
  {
    path: 'freetrial',
    component: SignUpComponent
  },
  {
    path: 'reset-password',
    component: ResetPasswordPageComponent
  },
  {
    path: 'set-password',
    component: SetPasswordPageComponent
  },
  {
    data: {
      roles: [UserRoles.systemAdministrator]
    },
    path: '1B0CDE58-444B-4CF5-BA24-9C02FA8B0533/login',
    component: AdminLoginComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AuthRoutingModule {}
