import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit{
  users: any[];
  roles: any[] = [];
  isLoading = true;
  displayedColumns: string[] = [];
  isUpdateEnabled: boolean = false;

  enableUpdateButton() {
    this.isUpdateEnabled = true;
  }

  
  constructor(private authService: ApiService) {}
  
  ngOnInit(): void {
   this.loadUsers();
   this.loadRoles();
  }
  loadUsers() {
    this.authService.getAllUsers().subscribe(
      response => {
        this.users = response;
        this.displayedColumns = Object.keys(this.users[0]); 
        console.log('All users fetched successfully', response);
        this.isLoading = false;
      },
      error => {
        console.error('Failed to fetch users', error);
      }
    );
  }

  loadRoles() {
    this.authService.getRoles().subscribe({
      next: (data) => {
        this.roles = data;
      },
      error: (error) => {
        console.error('Error loading roles', error);
      }
    });
  }

  updateUserRole(userId: number, roleId: number) {
    this.authService.updateUserRole(userId, roleId).subscribe({
      next: (response) => {
        this.authService.showAlert('success', 'User role updated successfully!');
        this.loadUsers();
      },
      error: (error) =>   this.authService.showAlert('danger', error.error.message)
    });
    let closeButton = document.querySelector("#editRoleModal .btn-close") as HTMLElement;
    if (closeButton) {
      closeButton.click();
    }
  }

  deleteUser(userId: number) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.authService.deleteUser(userId).subscribe({
        next: (response) => {
          this.authService.showAlert('success', response.message);
          this.loadUsers();
        },
        error: (error) => this.authService.showAlert('danger', error.error.message)
      });
    }
  }

  getRoleName(roleId: number): string {
    const role = this.roles.find(r => r.role_Id === roleId);
    return role ? role.role_Name : 'Unknown Role'; 
  }
}
