import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrl: './role-management.component.css'
})
export class RoleManagementComponent {
  roles: any[] = [];
  newRole: any = { role_Name: '', description: '',is_Active: true };
  roleLoading: boolean = false;
  roleSuccess: boolean = false;
  roleError: string = '';

  constructor(private permissionService: ApiService,private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadRoles();
    this.loadMenus();
   
  }

  loadRoles(): void {
    this.roleLoading = true;
    this.permissionService.getRoles().subscribe({
      next: (data) => {
        this.roles = data;
        this.roleLoading = false;
      },
      error: (error) => {
        console.error('Error loading roles', error);
        this.roleLoading = false;
        this.roleError = 'Failed to load roles';
      }
    });
  }

  createRole(): void {
    if (!this.newRole.role_Name.trim()) {
      this.roleError = 'Role name is required';
      return;
    }
    if (!this.newRole.description.trim()) {
      this.roleError = 'Role description is required';
      return;
    }
    this.roleLoading = true;
    this.roleSuccess = false;
    this.roleError = '';

    this.permissionService.createRole(this.newRole).subscribe({
      next: (role) => {
        this.roles.push(role);
        this.newRole = { role_Name: '', description: '',is_Active: true };
        this.roleSuccess = true;
        this.roleLoading = false;
        setTimeout(() => this.roleSuccess = false, 3000);
      },
      error: (error) => {
        console.error('Error creating role', error);
        this.roleError = error.message || 'Failed to create role';
        this.roleLoading = false;
      }
    });
  }

  deleteRole(role: any, index: number): void {
    if (!confirm(`Are you sure you want to delete role "${role.role_Name}"? This will also delete all permissions assigned to this role.`)) {
      return;
    }

    this.roleLoading = true;
    this.roleSuccess = false;
    this.roleError = '';

    this.permissionService.deleteRole(role.role_Id).subscribe({
      next: () => {
        this.roles.splice(index, 1);
        this.roleSuccess = true;
        this.roleLoading = false;
        setTimeout(() => (this.roleSuccess = false), 3000);
      },
      error: (error) => {
        console.error('Error deleting role', error);
        this.roleError = error.message || 'Failed to delete role';
        this.roleLoading = false;
      },
    });
  }

//



    menus: any[] = [];
    newMenu: any = { menu_Code: '', is_Active: true  };
    menuLoading: boolean = false;
    menuSuccess: boolean = false;
    menuError: string = '';
  
    loadMenus(): void {
      this.menuLoading = true;
      this.permissionService.getMenus().subscribe({
        next: (data) => {
          this.menus = data;
          this.menuLoading = false;
        },
        error: (error) => {
          console.error('Error loading menus', error);
          this.menuLoading = false;
          this.menuError = 'Failed to load menus';
        }
      });
    }
  
    createMenu(): void {
      if (!this.newMenu.menu_Code.trim()) {
        this.menuError = 'Menu code is required';
        return;
      }
  
      this.menuLoading = true;
      this.menuSuccess = false;
      this.menuError = '';
  
      this.permissionService.createMenu(this.newMenu).subscribe({
        next: (menu) => {
          this.menus.push(menu);
          this.newMenu = { menu_Code: '', is_Active: true };
          this.menuSuccess = true;
          this.menuLoading = false;
          setTimeout(() => this.menuSuccess = false, 3000);
        },
        error: (error) => {
          console.error('Error creating menu', error);
          this.menuError = error.message || 'Failed to create menu';
          this.menuLoading = false;
        }
      });
    }
  
    deleteMenu(menu: any, index: number): void {
      if (!confirm(`Are you sure you want to delete menu "${menu.menu_Code}"? This will also delete all permissions assigned to this menu.`)) {
        return;
      }
  
      this.menuLoading = true;
      this.menuSuccess = false;
      this.menuError = '';
  
      this.permissionService.deleteMenu(menu.menu_Id).subscribe({
        next: () => {
          this.menus.splice(index, 1);
          this.menuSuccess = true;
          this.menuLoading = false;
          setTimeout(() => this.menuSuccess = false, 3000);
        },
        error: (error) => {
          console.error('Error deleting menu', error);
          this.menuError = error.message || 'Failed to delete menu';
          this.menuLoading = false;
        }
      });
    }



}
