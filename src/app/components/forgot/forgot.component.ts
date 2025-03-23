import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../api.service';

@Component({
  selector: 'app-forgot',
  templateUrl: './forgot.component.html',
  styleUrl: './forgot.component.css'
})
export class ForgotComponent {
  forgotPasswordForm: FormGroup;
  changePasswordForm: FormGroup;
  userId: number = 0;
  isForgotPasswordForm = true;
  isChangePasswordForm = false;
  registeredEmail = '';
  isLoading = false;

  constructor(private fb: FormBuilder, private authService: ApiService,private router : Router,private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.changePasswordForm = this.fb.group({
      otp: ['', Validators.required],
      newPassword: ['', Validators.required]
    });

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.userId = params['id'];
        this.isForgotPasswordForm = false;
        this.isChangePasswordForm = true;
      }
    });
    this.route.queryParams.subscribe(queryParams => {
      if (queryParams['email']) {
        this.registeredEmail = queryParams['email'];
      }
    });
  }

  onSubmit() {
    if (this.forgotPasswordForm.valid) {
      this.isLoading = true;
      const email = { UserEmail: this.forgotPasswordForm.value.email };
      this.authService.forgotPassword(email).subscribe(
        response => {
          console.log('OTP sent successfully', response);
          this.authService.showAlert('success', 'OTP sent successfully!');
          this.isForgotPasswordForm = false;
          this.isChangePasswordForm = true;
          setTimeout(() => {
            this.router.navigate(['/forgot', response], { queryParams: { email: this.forgotPasswordForm.value.email } });
            this.isLoading = false;
          }, 3000);
        },
        error => {
          console.error('Failed to send OTP', error);
          this.authService.showAlert('danger', 'Failed to send OTP');
          this.isLoading = false;
        }
      );
    }else{
      this.forgotPasswordForm.markAllAsTouched(); 
    }
  }



  onOtpSubmit() {
    if (this.changePasswordForm.valid) {
      this.isLoading = true;
      const passwordChange = {
        UserId: this.userId,
        Otp: this.changePasswordForm.value.otp,
        NewPassword: this.changePasswordForm.value.newPassword
      };
      this.authService.changePassword(passwordChange).subscribe(
        response => {
          console.log('Password changed successfully', response);
            this.isLoading = false;
            this.authService.showAlertWithDelay('success', 'Password Changed Successfully!', '/login');
        },
        error => {
          console.error('Failed to change password', error);
          this.authService.showAlert('danger', 'Failed to change password');
          this.isLoading = false;
        }
      );
    }else{
      this.changePasswordForm.markAllAsTouched(); 
    }
  }

  resendDisabled = false;
  countdown = 30;
  countdownInterval: any;

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
