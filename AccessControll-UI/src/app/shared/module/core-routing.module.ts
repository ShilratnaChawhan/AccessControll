import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from '../../components/home/home.component';
import { RoleManagementComponent } from '../../components/role-management/role-management.component';
import { RolePermissionComponent } from '../../components/role-permission/role-permission.component';
import { SettingComponent } from '../../components/setting/setting.component';
import { UsersComponent } from '../../components/users/users.component';

const routes: Routes = [
  {path:'dashboard',component: HomeComponent},
  {path:'user',component: UsersComponent},
  {path:'setting',component: SettingComponent},
  {path:'role-permissions', component: RoleManagementComponent },
  {path:'role-permissions/:roleId', component: RolePermissionComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CoreRoutingModule { }
