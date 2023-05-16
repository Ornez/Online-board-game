import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { LoginData } from "@auth/models/login-data";
import { LoginDataPayload } from "@auth/models/login-data-payload";
import { RegisterDataPayload } from "@auth/models/register-data-payload";

@Injectable({
  providedIn: 'root'
})
export class HttpAuthService {

  constructor(private http: HttpClient) {}

  public login(loginData: LoginDataPayload): Observable<LoginData> {
    //const url = '/http-backend/api/account/login';
    const url = '/api/account/login';
    return this.http.post<LoginData>(url, loginData);
  }

  public register(registerData: RegisterDataPayload): Observable<LoginData> {
    //const url = '/http-backend/api/account/register';
    const url = '/api/account/register';
    return this.http.post<LoginData>(url, registerData);
  }

  public generatePlayerToken(oldPlayerToken: string): Observable<LoginData> {
    //const url = '/http-backend/api/token/getToken';
    const url = '/api/token/getToken';
    return this.http.post<LoginData>(url, oldPlayerToken);
  }
}
