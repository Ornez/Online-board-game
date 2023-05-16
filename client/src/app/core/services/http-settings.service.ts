import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {SettingsData} from "@core/models/settings-data";

@Injectable({
  providedIn: "root"
})
export class HttpSettingsService {

  constructor(private http: HttpClient) {}

  public getSettings(): Observable<SettingsData> {
    const url = 'http-backend/api/user/settings';
    return this.http.get<SettingsData>(url);
  }

  public saveSettings(settings: SettingsData): Observable<any> {
    const requestOptions: Object = {responseType: 'text'}
    const url = 'http-backend/api/user/settings';
    return this.http.put<any>(url, settings, requestOptions);
  }
}
