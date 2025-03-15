import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ApiService } from '../../../api.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit{
  @ViewChild('sidebar') sidebar!: ElementRef;

  
  constructor(private authService: ApiService) {}
  ngOnInit(): void {
    this.getMenuByRole();
  }
  
  menus: any[] = [];
  menuLoading: boolean = false;
  menuSuccess: boolean = false;
  menuError: string = '';

  getMenuByRole(): void {
    this.menuLoading = true;
    let token :any = localStorage.getItem('jwtToken')
    var payloadData = JSON.parse(this.authService.decodeJwt(this.authService.getPayload(token)));
    this.authService.getMenuByRole(payloadData.RoleId).subscribe({
      next: (data) => {
        this.menus = data;
        console.log(data);
        this.menuLoading = false;
      },
      error: (error) => {
        console.error('Error loading menus', error);
        this.menuLoading = false;
        this.menuError = 'Failed to load menus';
      }
    });
  }


  toggleSidebar() {
    if (this.sidebar) {
      this.sidebar.nativeElement.classList.toggle('active');
    }
  }
}


