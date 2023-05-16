import {Injectable} from "@angular/core";
import {BaseState, StateService} from "@shared/common/state.service";
import {ProfileData} from "@core/models/profile-data";
import {finalize, Observable} from "rxjs";
import {HttpProfileService} from "@core/services/http-profile.service";

interface ProfileState {
  profile: BaseState<ProfileData>
}

const initialState: ProfileState = {
  profile: {}
}

@Injectable({
  providedIn: "root"
})
export class ProfileStoreService extends StateService<ProfileState> {

  readonly profile$: Observable<ProfileData> = this.select(state => state.profile.data);
  readonly profileLoading$: Observable<boolean> = this.select(state => state.profile.loading);

  constructor(private http: HttpProfileService) {
    super(initialState);
  }

  public getProfile(): void {
    this.setState({profile: {loading: true}} as Partial<ProfileState>)
    this.http.getProfile()
      .pipe(finalize(() => this.setState({profile: {loading: false}} as Partial<ProfileState>)))
      .subscribe( (profile: ProfileData) => this.setState({profile: {data: profile}} as Partial<ProfileState>))
  }

  public clearProfile(): void {
    this.setState({profile: {data: undefined}} as Partial<ProfileState>);
  }
}
