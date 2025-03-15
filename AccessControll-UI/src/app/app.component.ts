import { Component } from '@angular/core';
import { ApiService } from './api.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'AccessControll-UI';
  alertMessage: { type: string; text: string } | null = null;

  constructor(private alertService: ApiService) {
    this.alertService.alertMessage$.subscribe((message) => {
      this.alertMessage = message;
    });
  }
}
