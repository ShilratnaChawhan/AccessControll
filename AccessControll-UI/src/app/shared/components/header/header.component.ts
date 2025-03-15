import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../../api.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @Output() toggleSidebarEvent = new EventEmitter<void>();

  constructor(private authService: ApiService){}
  toggleSidebar() {
    this.toggleSidebarEvent.emit(); 
  }

  onSubmit() {
    let jwtToken = localStorage.getItem('jwtToken')
    if(jwtToken != null){
      this.authService.logout(jwtToken).subscribe(
        response => {
          console.log('Logout successful', response);
          this.authService.showAlertWithDelay('success', 'Logout successful!','login');
          localStorage.removeItem('jwtToken');
        },
        error => {
          console.error('Logout failed', error);
          this.authService.showAlert('danger', 'Logout failed');
        }
      );
    }else{
      this.authService.showAlert('danger', 'Token Not Found!');
    }
  }
}
