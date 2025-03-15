import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  verifyOtpForm: FormGroup;
  userId: number = 0;
  isRegisterForm = true;
  isVerifyOtpForm = false;
  registeredEmail = '';
  isLoading = false;

  constructor(private fb: FormBuilder, private authService: ApiService,private router : Router,private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      // firstName: ['', Validators.required],
      // lastName: ['', Validators.required],
      phoneNumber: ['', Validators.required]
    });

    this.verifyOtpForm = this.fb.group({
      otp: ['', Validators.required]
    });

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.userId = params['id'];
        this.isRegisterForm = false;
        this.isVerifyOtpForm = true;
      }
    });

    this.route.queryParams.subscribe(queryParams => {
      if (queryParams['email']) {
        this.registeredEmail = queryParams['email'];
      }
    });
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      const user = {
        UserName: this.registerForm.value.username,
        UserEmail: this.registerForm.value.email,
        Password: this.registerForm.value.password,
        // FirstName: this.registerForm.value.firstName,
        // LastName: this.registerForm.value.lastName,
        PhoneNumber: this.registerForm.value.phoneNumber
      };
      this.authService.register(user).subscribe(
        response => {
          console.log('Registration successful', response);
          this.isRegisterForm = false;
          this.isVerifyOtpForm = true;
          this.authService.showAlert('success', 'Registration successful!');
          setTimeout(() => {
            this.router.navigate(['/register', response], { queryParams: { email: this.registerForm.value.email } });
            this.isLoading = false;
          }, 3000);
        },
        error => {
          console.error('Registration failed', error);
          this.authService.showAlert('danger', 'Registration failed');
          this.isLoading = false;
        }
      );
    }else{
      this.registerForm.markAllAsTouched(); 
    }
  }

  
  resendDisabled = false;
  countdown = 30;
  countdownInterval: any;

  onOtpSubmit() {
    if (this.verifyOtpForm.valid) {
      this.isLoading = true;
      const otpVerification = {
        UserId: this.userId,
        Otp: this.verifyOtpForm.value.otp
      };
      this.authService.confirmRegistration(otpVerification).subscribe(
        response => {
          console.log('OTP verified successfully', response);
          this.authService.showAlertWithDelay('success', 'OTP verified  successful!', '/login');
          this.isLoading = false;
        },
        error => {
          console.error('Failed to verify OTP', error);
          this.authService.showAlert('danger', 'Failed to send OTP');
          this.isLoading = false;
        }
      );
    }else{
      this.verifyOtpForm.markAllAsTouched(); 
    }
  }
  
  resendOtp() {
    this.resendDisabled = true;
    this.countdown = 30;

    this.countdownInterval = setInterval(() => {
      if (this.countdown > 0) {
        this.countdown--;
      } else {
        this.resendDisabled = false;
        clearInterval(this.countdownInterval);
      }
    }, 1000);
  }
}