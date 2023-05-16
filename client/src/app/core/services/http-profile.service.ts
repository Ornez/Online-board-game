import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {ProfileData} from "@core/models/profile-data";

@Injectable({
  providedIn: "root"
})
export class HttpProfileService {

  constructor(private http: HttpClient) {}

  public getProfile(): Observable<ProfileData> {
    //const url = 'http-backend/api/user/profile';
    const url = 'api/user/profile';
    return this.http.get<ProfileData>(url);
  }
}
