import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from '../../components/home/home.component';
import { RoleManagementComponent } from '../../components/role-management/role-management.component';
import { RolePermissionComponent } from '../../components/role-permission/role-permission.component';
import { SettingComponent } from '../../components/setting/setting.component';
import { UsersComponent } from '../../components/users/users.component';

const routes: Routes = [
  {path:'Dashboard',component: HomeComponent},
  {path:'Users',component: UsersComponent},
  {path:'Setting',component: SettingComponent},
  {path:'Role', component: RoleManagementComponent },
  {path:'Role/:roleId', component: RolePermissionComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CoreRoutingModule { }
