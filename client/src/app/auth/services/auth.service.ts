import {Injectable} from "@angular/core";
import {LoginData} from "@auth/models/login-data";
import {LocalStorageEnum} from "@shared/enums/local-storage.enum";
import {ObjectUtils} from "@shared/utils/object-utils";
import jwt_decode from "jwt-decode";
import {TokenData} from "@auth/models/token-data";
import {UserRole} from "@auth/enums/user-role.enum";


@Injectable({
  providedIn: "root"
})
export class AuthService  {

  public isLoggedIn(): boolean {
    return !ObjectUtils.isNil(localStorage.getItem(LocalStorageEnum.JWT_TOKEN)) && this.getUserRole() === UserRole.USER;
  }

  public getJwtToken(): string {
    return localStorage.getItem(LocalStorageEnum.JWT_TOKEN);
  }

  public getUsername(): string {
    return localStorage.getItem(LocalStorageEnum.USER_USERNAME);
  }

  public getUserRole(): string {
    return localStorage.getItem(LocalStorageEnum.USER_ROLE);
  }

  public saveLoginDataInLocalStorage(loginData: LoginData): void {
    const tokenData: TokenData = jwt_decode(loginData.token);
    localStorage.setItem(LocalStorageEnum.JWT_TOKEN, loginData.token);
    localStorage.setItem(LocalStorageEnum.USER_USERNAME, tokenData.username);
    localStorage.setItem(LocalStorageEnum.USER_ROLE, tokenData.userRole);
  }

  public clearLoginDataFromLocalStorage(): void {
    localStorage.removeItem(LocalStorageEnum.JWT_TOKEN);
    localStorage.removeItem(LocalStorageEnum.USER_USERNAME);
    localStorage.removeItem(LocalStorageEnum.USER_ROLE);
  }
}
