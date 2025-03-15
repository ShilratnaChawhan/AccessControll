import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { BehaviorSubject, Observable } from 'rxjs';
import * as CryptoJS from 'crypto-js';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private alertMessageSubject = new BehaviorSubject<{ type: string; text: string } | null>(null);
  alertMessage$ = this.alertMessageSubject.asObservable();
  private apiUrl = "https://localhost:7299/api/AccessControll";
  private secret: string = "abcdefghijklmnop"; 
  private key = CryptoJS.enc.Utf8.parse(this.secret);
  constructor(private http: HttpClient, private router: Router) { }

  // JWT Helper
  getPayload(token: string): any{
    const decodedToken: any = jwtDecode(token);
    return decodedToken.payload
  }

  decodeJwt(payload: string): any{
    const decryptedPayload = CryptoJS.AES.decrypt(payload, this.key, {mode: CryptoJS.mode.ECB, padding: CryptoJS.pad.Pkcs7});
    let decryptedText = decryptedPayload.toString(CryptoJS.enc.Utf8);
    // console.log(decryptedText);
    return decryptedText;
  }
  
  encriptData(plainText : string):any{
    let encryptedBytes = CryptoJS.AES.encrypt(plainText, this.key, {mode: CryptoJS.mode.ECB, padding: CryptoJS.pad.Pkcs7});
    let encryptedString = encryptedBytes.toString();
    // console.log(encryptedString);
    return encryptedString;
  }

  decriptData(cipherText : string):any{
    let decryptedBytes = CryptoJS.AES.decrypt(cipherText, this.key, {mode: CryptoJS.mode.ECB, padding: CryptoJS.pad.Pkcs7});
    let decryptedText = decryptedBytes.toString(CryptoJS.enc.Utf8);
    // console.log(decryptedText);
    return decryptedText;
  }
  
  saveToken(token: string) {
    localStorage.setItem('jwtToken', token);
  }

  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  //global alert
  showAlert(type: 'success' | 'danger' | 'warning' | 'info', text: string) {
    this.alertMessageSubject.next({ type, text });

    // Auto-hide the alert after 3 seconds
    setTimeout(() => {
      this.clearAlert();
    }, 3000);
  }

  showAlertWithDelay(type: 'success' | 'danger' | 'warning' | 'info', text: string, route?: string) {
    this.showAlert(type, text);

    if (route) {
      setTimeout(() => {
        this.router.navigate([route]);
      }, 3000);
    }
  }

  clearAlert() {
    this.alertMessageSubject.next(null);
  }

  // Register a new user
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }
  // Confirm user registration with OTP
  confirmRegistration(confirmation: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/confirm-registration`, confirmation);
  }
  // Login with credentials
  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials, { responseType: 'text' });
  }
  // Logout the user
  logout(authToken: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/logout?authToken=${authToken}`,null);
  }
  
  // Request a password reset OTP
  forgotPassword(email: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/forgot-password`, email);
  }
  // Change the user's password
  changePassword(passwordChange: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/change-password`, passwordChange);
  }
  // Refresh the authentication token
  refreshToken(refreshToken: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/refresh-token`, { refreshToken } ,  { responseType: 'text' });
  }
  // Send an OTP to the user
  sendOtp(otpRequest: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/send-otp`, otpRequest);
  }
  // Verify the OTP provided by the user
  verifyOtp(otpVerification: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-otp`, otpVerification);
  }
  // Get all active users
  getAllUsers(): Observable<any> {
    return this.http.get(`${this.apiUrl}/get-all-users`);
  }
  // Role operations
  getRoles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/roles`);
  }

  createRole(role: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/role`, role);
  }

  deleteRole(roleId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/role/${roleId}`);
  }

  // Menu operations
  getMenus(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/menus`);
  }

  createMenu(menu: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/menu`, menu);
  }

  deleteMenu(menuId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/menu/${menuId}`);
  }

  // Permission operations
  getRolePermissions(roleId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/permissions/${roleId}`);
  }

  saveRolePermissions(model: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/permissions`, model);
  }

  deleteRolePermission(rolePermissionId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/permissions/${rolePermissionId}`);
  }

  //get menu bu role
  getMenuByRole(role: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/get-menu-by-role/${role}`);
  }

  //user
  updateUserRole(userId: number, roleId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/update-user-role`, { userId, roleId });
  }

  deleteUser(userId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete-user/${userId}`);
  }
}