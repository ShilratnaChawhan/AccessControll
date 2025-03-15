import { Component, OnInit, ViewChild } from '@angular/core';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css'
})
export class LayoutComponent implements OnInit{
  @ViewChild(SidebarComponent) sidebarComponent!: SidebarComponent;

  ngOnInit(): void {
    this.loadScripts();
  }
  toggleSidebar() {
    if (this.sidebarComponent) {
      this.sidebarComponent.toggleSidebar();
    } 
  }

  loadScripts() {
    const scriptUrls = [
      'assets/vendor/bootstrap/js/bootstrap.bundle.min.js',
      // 'assets/vendor/jquery/jquery.min.js',
      // 'assets/js/script.js',
    ];

    for (const url of scriptUrls) {
      const script = document.createElement('script');
      script.src = url;
      script.async = true;
      document.body.appendChild(script);
    }
  }
}