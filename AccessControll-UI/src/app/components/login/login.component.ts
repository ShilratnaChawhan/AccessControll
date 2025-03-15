import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit{
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private authService: ApiService,private router : Router) {}

  ngOnInit(): void {
    localStorage.clear();
    sessionStorage.clear();
    
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const credentials = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password
      };
      this.authService.login(credentials).subscribe(
        response => {
          console.log('Login successful');
          this.authService.showAlertWithDelay('success', 'Login successful!','/dashboard');
          this.authService.saveToken(response);
          this.decriptToken();
        },
        error => {
          console.error('Login failed', error);
          this.authService.showAlert('danger', 'Login failed. Please check your credentials.');
        }
      );
    }else{
      this.loginForm.markAllAsTouched(); 
    }
  }

  decriptToken(){
    let token :any = localStorage.getItem('jwtToken')
    console.log(this.authService.decodeJwt(this.authService.getPayload(token)))
  }
}
