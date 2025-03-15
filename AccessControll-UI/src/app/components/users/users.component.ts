import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent implements OnInit{
  users: any[];
  isLoading = true;
  displayedColumns: string[] = [];

  constructor(private authService: ApiService) {}
  
  ngOnInit(): void {
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
}
