import {Injectable} from "@angular/core";
import {BaseState, StateService} from "@shared/common/state.service";
import {StatisticsData} from "@core/models/statistics-data";
import {finalize, Observable} from "rxjs";
import {HttpStatisticsService} from "@core/services/http-statistics.service";

interface StatisticsState {
  statistics: BaseState<StatisticsData>
}

const initialState: StatisticsState = {
  statistics: {}
}

@Injectable({
  providedIn: "root"
})
export class StatisticsStoreService extends StateService<StatisticsState>  {

  readonly statistics$: Observable<StatisticsData> = this.select(state => state.statistics.data);
  readonly statisticsLoading$: Observable<boolean> = this.select(state => state.statistics.loading);

  constructor(private http: HttpStatisticsService) {
    super(initialState);
  }

  public getStatistics(): void {
    this.setState({statistics: {loading: true}} as Partial<StatisticsState>)
    this.http.getStatistics()
      .pipe(finalize(() => this.setState({statistics: {loading: false}} as Partial<StatisticsState>)))
      .subscribe( (statistics: StatisticsData) => this.setState({statistics: {data: statistics}} as Partial<StatisticsState>));
  }

  public clearStatistics(): void {
    this.setState({statistics: {data: undefined}} as Partial<StatisticsState>);
  }
}
