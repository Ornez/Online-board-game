import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {StatisticsData} from "@core/models/statistics-data";

@Injectable({
  providedIn: "root"
})
export class HttpStatisticsService {

  constructor(private http: HttpClient) {}

  public getStatistics(): Observable<StatisticsData> {
    //const url = 'http-backend/api/user/statistics';
    const url = 'api/user/statistics';
    return this.http.get<StatisticsData>(url);
  }
}
