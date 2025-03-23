import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-role-permission',
  templateUrl: './role-permission.component.html',
  styleUrl: './role-permission.component.css'
})
export class RolePermissionComponent {
  roles: any[] = [];
  selectedRoleId: number = 0;
  permissionModel: any | null = null;
  menus: any[] = [];
  loading: boolean = false;
  saveSuccess: boolean = false;
  saveError: string = '';
  deleteSuccess: boolean = false;
  deleteError: string = '';

  constructor(private permissionService: ApiService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loadAllRoles();
    this.loadMenus();
    this.route.paramMap.subscribe(params => {
      const roleId = params.get('roleId');
      if (roleId) {
        this.selectedRoleId = +roleId;
        this.loadPermissions();
      }
    });
  }

  /** ✅ Load All Roles */
  loadAllRoles(): void {
    this.loading = true;
    this.permissionService.getRoles().subscribe({
      next: (data) => {
        this.roles = data;
        this.loading = false;
        if (this.selectedRoleId === 0 && this.roles.length > 0) {
          this.selectedRoleId = this.roles[0].role_Id;
          this.loadPermissions();
        }
      },
      error: (error) => {
        console.error('Error loading roles', error);
        this.loading = false;
      }
    });
  }

  /** ✅ Load All Menus */
  loadMenus(): void {
    this.loading = true;
    this.permissionService.getMenus().subscribe({
      next: (data) => {
        this.menus = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading menus', error);
        this.loading = false;
      }
    });
  }

  /** ✅ Load Permissions for Selected Role */
  loadPermissions(): void {
    if (!this.selectedRoleId) return;

    this.loading = true;
    this.permissionService.getRolePermissions(this.selectedRoleId).subscribe({
      next: (data) => {
        this.mergeMenusWithPermissions(data);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading permissions', error);
        this.loading = false;
      }
    });
  }

  /** ✅ Merge Menus with Permissions (Ensures all menus are displayed) */
  private mergeMenusWithPermissions(rolePermissions: any): void {
    this.permissionModel = {
      roleId: this.selectedRoleId,
      roleName: rolePermissions.roleName,
      permissions: this.menus.map(menu => {
        const existingPermission = rolePermissions.permissions.find((p: any) => p.menuId === menu.menu_Id);
        return {
          menuId: menu.menu_Id,
          menuCode: menu.menu_Code,
          canAdd: existingPermission ? existingPermission.canAdd : false,
          canEdit: existingPermission ? existingPermission.canEdit : false,
          canView: existingPermission ? existingPermission.canView : false,
          canDelete: existingPermission ? existingPermission.canDelete : false,
          isActive: existingPermission ? existingPermission.isActive : true,
          rolePermissionId: existingPermission ? existingPermission.rolePermissionId : 0
        };
      })
    };
  }

  /** ✅ Handle Role Change */
  onRoleChange(): void {
    this.loadPermissions();
  }

  /** ✅ Save Permissions */
  savePermissions(): void {
    if (!this.permissionModel) return;

    this.loading = true;
    this.saveSuccess = false;
    this.saveError = '';

    this.permissionService.saveRolePermissions(this.permissionModel).subscribe({
      next: () => {
        this.saveSuccess = true;
        this.loading = false;
        setTimeout(() => (this.saveSuccess = false), 3000);
      },
      error: (error) => {
        this.saveError = error.message || 'Error saving permissions';
        this.loading = false;
        setTimeout(() => (this.saveError = ''), 5000);
      }
    });
  }

  /** ✅ Delete a Specific Permission */
  deletePermission(permission: any, index: number): void {
    if (!permission.rolePermissionId) {
      // If this is a new permission (not saved yet), just remove it from the array
      if (this.permissionModel) {
        this.permissionModel.permissions.splice(index, 1);
      }
      return;
    }

    if (!confirm(`Are you sure you want to delete permission for menu "${permission.menuCode}"?`)) {
      return;
    }

    this.loading = true;
    this.deleteSuccess = false;
    this.deleteError = '';

    this.permissionService.deleteRolePermission(permission.rolePermissionId).subscribe({
      next: () => {
        this.deleteSuccess = true;
        this.loading = false;
        if (this.permissionModel) {
          this.permissionModel.permissions.splice(index, 1);
        }
        setTimeout(() => (this.deleteSuccess = false), 3000);
      },
      error: (error) => {
        this.deleteError = error.message || 'Error deleting permission';
        this.loading = false;
        setTimeout(() => (this.deleteError = ''), 5000);
      }
    });
  }
}
