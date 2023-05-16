import {Injectable} from "@angular/core";
import {from, Observable, Subject} from "rxjs";
import {HttpTransportType, HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {Lobby} from "@lobby-list/models/lobby";
import {LobbyJoinPayload} from "@lobby-list/models/lobby-join-payload";
import {WebSocketResponse} from "@lobby-list/models/web-socket-response";
import {LobbyCreatePayload} from "@lobby-list/models/lobby-create-payload";
import {LobbyCreateWebSocketResponse} from "@lobby-list/models/lobby-create-web-socket-response";
import {AuthService} from "@auth/services/auth.service";

@Injectable({
  providedIn: "root"
})
export class WsLobbyListService {

  private hubConnection: HubConnection;

  constructor(private auth: AuthService) {}

  public initLobbiesConnection(): Observable<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('/lobbies', {
        accessTokenFactory: () => this.auth.getJwtToken(),
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return from(this.hubConnection.start());
  }

  public closeLobbiesConnection(): void {
    this.hubConnection ? this.hubConnection.stop() : undefined;
  }

  public getLobbies(): Observable<Lobby[]> {
    return from(this.hubConnection.invoke('GetLobbies'));
  }

  public listenToLobbyCreate(): Observable<Lobby> {
    const createdLobby$: Subject<Lobby> = new Subject<Lobby>();
    this.hubConnection.on('create_lobby', (createdLobby: Lobby) => createdLobby$.next(createdLobby))
    return createdLobby$;
  }

  public listenToLobbyUpdate(): Observable<Lobby> {
    const updateLobby$: Subject<Lobby> = new Subject<Lobby>();
    this.hubConnection.on('update_lobby', (updatedLobby: Lobby) => updateLobby$.next(updatedLobby))
    return updateLobby$;
  }

  public listenToLobbyDeleted(): Observable<Lobby> {
    const deletedLobby$: Subject<Lobby> = new Subject<Lobby>();
    this.hubConnection.on('delete_lobby', (deletedLobby: Lobby) => deletedLobby$.next(deletedLobby))
    return deletedLobby$;
  }

  public joinLobby(lobbyJoinPayload: LobbyJoinPayload): Observable<WebSocketResponse> {
    return from(this.hubConnection.invoke('JoinLobby', lobbyJoinPayload));
  }

  public createLobby(lobby: LobbyCreatePayload): Observable<LobbyCreateWebSocketResponse> {
    return from(this.hubConnection.invoke('CreateLobby', lobby));
  }
}
