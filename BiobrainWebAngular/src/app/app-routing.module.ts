import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './auth/views/home/home.component';
import { AuthGuard } from './core/guards/auth.quard';

const routes: Routes = [ 
  { path: '', component: HomeComponent, canActivate: [AuthGuard], },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
