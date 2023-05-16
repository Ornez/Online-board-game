import {Injectable} from "@angular/core";
import {BaseState, StateService} from "@shared/common/state.service";
import {finalize, Observable} from "rxjs";
import {HttpAuthService} from "@auth/services/http-auth.service";
import {LoginDataPayload} from "@auth/models/login-data-payload";
import {LoginData} from "@auth/models/login-data";
import {AuthService} from "@auth/services/auth.service";
import {LobbyListStoreService} from "@lobby-list/store/lobby-list-store.service";
import {SettingsStoreService} from "@core/store/settings-store.service";

interface AuthState {
  login: BaseState;
  register: BaseState;
}

const initialState: AuthState = {
  login: {},
  register: {},
}

@Injectable({
  providedIn: "root"
})
export class AuthStoreService extends StateService<AuthState> {

  readonly loginLoading$: Observable<boolean> = this.select(state => state.login.loading);
  readonly loginSuccess$: Observable<boolean> = this.select(state => state.login.success);

  readonly registerLoading$: Observable<boolean> = this.select(state => state.register.loading);
  readonly registerSuccess$: Observable<boolean> = this.select(state => state.register.success);

  constructor(private http: HttpAuthService,
              private auth: AuthService,
              private lobbyListStore: LobbyListStoreService,
              private settingsStore: SettingsStoreService) {
    super(initialState);
  }

  public login(loginDataPayload: LoginDataPayload): void {
    this.setState({login: {loading: true}} as Partial<AuthState>);
    this.http.login(loginDataPayload)
      .pipe(finalize(() => this.setState({login: {loading: false}} as Partial<AuthState>)))
      .subscribe((loginData: LoginData) => {
        this.auth.saveLoginDataInLocalStorage(loginData);
        this.settingsStore.getSettings();
        this.lobbyListStore.disconnectLobbiesHub();
        this.lobbyListStore.connectLobbiesHub();
        this.setState({login: {success: true}} as Partial<AuthState>);
        this.setState({login: {success: undefined}} as Partial<AuthState>);
      })
  }

  public logout(): void {
    this.auth.clearLoginDataFromLocalStorage();
    this.generatePlayerToken();
  }

  public generatePlayerToken(): void {
    this.http.generatePlayerToken(this.auth.getJwtToken())
      .subscribe((loginData: LoginData) => {
        this.auth.saveLoginDataInLocalStorage(loginData);
        this.lobbyListStore.disconnectLobbiesHub();
        this.lobbyListStore.connectLobbiesHub();
      })
  }

  public register(registerDataPayload): void {
    this.setState({register: {loading: true}} as Partial<AuthState>);
    this.http.register(registerDataPayload)
      .pipe(finalize(() => this.setState({register: {loading: false}} as Partial<AuthState>)))
      .subscribe((loginData: LoginData) => {
        this.auth.saveLoginDataInLocalStorage(loginData);
        this.setState({register: {success: true}} as Partial<AuthState>);
        this.setState({register: {success: undefined}} as Partial<AuthState>);
      })
  }
}
