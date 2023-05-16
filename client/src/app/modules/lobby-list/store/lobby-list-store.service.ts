import {Injectable} from "@angular/core";
import {BaseState, StateService} from "@shared/common/state.service";
import {Observable, Subject, take, takeUntil} from "rxjs";
import {Lobby} from "@lobby-list/models/lobby";
import {WsLobbyListService} from "@lobby-list/services/ws-lobby-list.service";
import {LobbyJoinPayload} from "@lobby-list/models/lobby-join-payload";
import {WebSocketResponse} from "@lobby-list/models/web-socket-response";
import {LobbyCreatePayload} from "@lobby-list/models/lobby-create-payload";
import {LobbyCreateWebSocketResponse} from "@lobby-list/models/lobby-create-web-socket-response";

interface LobbyListState {
  lobbies: BaseState<Lobby[]>;
  createLobby: BaseState<number>;
  joinTheLobby: BaseState<number>;
}

const initialState: LobbyListState = {
  lobbies: {},
  createLobby: {},
  joinTheLobby: {}
}

@Injectable({
  providedIn: 'root'
})
export class LobbyListStoreService extends StateService<LobbyListState> {

  lobbiesSubscription$: Subject<boolean> = new Subject<boolean>();

  readonly lobbies$: Observable<Lobby[]> = this.select(state => state.lobbies.data);

  readonly createLobby$: Observable<number> = this.select(state => state.createLobby.data);
  readonly createLobbyLoading$: Observable<boolean> = this.select(state => state.createLobby.loading);
  readonly createLobbySuccess$: Observable<boolean> = this.select(state => state.createLobby.success);

  readonly joinTheLobbySuccess$: Observable<boolean> = this.select(state => state.joinTheLobby.success);
  readonly joinTheLobbyLoading$: Observable<boolean> = this.select(state => state.joinTheLobby.loading);

  constructor(private ws: WsLobbyListService) {
    super(initialState);
  }

  public connectLobbiesHub(): void {
    this.ws.initLobbiesConnection().pipe(takeUntil(this.lobbiesSubscription$)).subscribe( () => {
      this.ws
        .getLobbies()
        .pipe(takeUntil(this.lobbiesSubscription$))
        .subscribe((lobbies: Lobby[]) => this.setState({lobbies: {data: lobbies}} as Partial<LobbyListState>));

      this.ws
        .listenToLobbyCreate()
        .pipe(takeUntil(this.lobbiesSubscription$))
        .subscribe((createdLobby: Lobby) => {
          this.state.lobbies.data.push(createdLobby)
          this.setState({lobbies: {data: this.state.lobbies.data}} as Partial<LobbyListState>);
        });

      this.ws
        .listenToLobbyUpdate()
        .pipe(takeUntil(this.lobbiesSubscription$))
        .subscribe((updatedLobby: Lobby) => {
          const index = this.state.lobbies.data.findIndex( lobby => lobby.id === updatedLobby.id);
          this.state.lobbies.data[index] = updatedLobby;
          this.setState({lobbies: {data: this.state.lobbies.data}} as Partial<LobbyListState>);
        });

      this.ws
        .listenToLobbyDeleted()
        .pipe(takeUntil(this.lobbiesSubscription$))
        .subscribe((deletedLobby: Lobby) => {
          const index = this.state.lobbies.data.findIndex( lobby => lobby.id === deletedLobby.id);
          this.state.lobbies.data.splice(index, 1);
          this.setState({lobbies: {data: this.state.lobbies.data}} as Partial<LobbyListState>);
        });
    })
  }

  public createLobby(lobby: LobbyCreatePayload): void {
    this.setState({createLobby: {loading: true}} as Partial<LobbyListState>)
    this.ws.createLobby(lobby)
      .pipe(take(1))
      .subscribe( (lobbyCreateWebSocketResponse: LobbyCreateWebSocketResponse) => {
        this.setState({createLobby: {loading: false}} as Partial<LobbyListState>)
        this.setState({createLobby: {success: lobbyCreateWebSocketResponse.success, data: lobbyCreateWebSocketResponse.lobbyId}} as Partial<LobbyListState>)
        this.setState({createLobby: {success: undefined, data: undefined}} as Partial<LobbyListState>)
      })
  }

  public joinTheLobby(lobbyJoinPayload: LobbyJoinPayload): void {
    this.setState({joinTheLobby: {loading: true}} as Partial<LobbyListState>)
    this.ws.joinLobby(lobbyJoinPayload)
      .pipe(takeUntil(this.lobbiesSubscription$))
      .subscribe( (webSocketResponse: WebSocketResponse) => {
        this.setState({joinTheLobby: {loading: false, data: 1}} as Partial<LobbyListState>)
        this.setState({joinTheLobby: {success: webSocketResponse.success}} as Partial<LobbyListState>)
        this.setState({joinTheLobby: {success: undefined}} as Partial<LobbyListState>)
      })
  }

  public disconnectLobbiesHub(): void {
    this.ws.closeLobbiesConnection();
    this.lobbiesSubscription$.next(true);
    this.lobbiesSubscription$.complete();
    this.setState({lobbies: {data: undefined}} as Partial<LobbyListState>);
  }
}
