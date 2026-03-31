import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.quard';
import { SubscriptionComponent } from './views/pages/subscription/subscription.component';

const routes: Routes = [
  {
    path: 'subscription',
    canActivate: [AuthGuard],
    component: SubscriptionComponent,
  },
  {
    path: 'freetrial/subscription',
    canActivate: [AuthGuard],
    component: SubscriptionComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PaymentRoutingModule {}
