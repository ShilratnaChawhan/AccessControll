import { Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @ViewChild('sidebar') sidebar!: ElementRef;

  toggleSidebar() {
    if (this.sidebar) {
      this.sidebar.nativeElement.classList.toggle('active');
    }
  }
}


