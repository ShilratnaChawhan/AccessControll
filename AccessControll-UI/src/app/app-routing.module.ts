import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ForgotComponent } from './components/forgot/forgot.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { LayoutComponent } from './shared/components/layout/layout.component';

const routes: Routes = [
  {path:'',redirectTo: 'login',pathMatch: "full"},
  {path:'login',component: LoginComponent},
  {path:'register',component: RegisterComponent},
  {path:'register/:id',component: RegisterComponent},
  {path:'forgot',component: ForgotComponent},
  {path:'forgot/:id',component: ForgotComponent},
  { path: '', component: LayoutComponent,
    children: [
      { path: '', loadChildren: () => import('./shared/module/core.module').then(m => m.CoreModule) },
  ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
