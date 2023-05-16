import {Injectable} from "@angular/core";
import {from, Observable, Subject} from "rxjs";
import {HttpTransportType, HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {ChatMessage} from "../models/chat-message";
import {WebSocketResponse} from "@lobby-list/models/web-socket-response";
import {ChatMessageToSend} from "@chat/models/chat-message-to-send";
import {AuthService} from "@auth/services/auth.service";

@Injectable({
  providedIn: "root"
})
export class WsChatService {

  chat$: Subject<ChatMessage | ChatMessage[]> = new Subject<ChatMessage | ChatMessage[]>();

  chatHubConnection: HubConnection;

  constructor(private auth: AuthService) {}

  public initChatConnection(): Observable<void> {
    this.chatHubConnection = new HubConnectionBuilder()
      .withUrl('ws-backend/chat', {
        accessTokenFactory: () => this.auth.getJwtToken(),
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return from(this.chatHubConnection.start());
  }

  public getMessages(): Observable<ChatMessage[]> {
    return from(this.chatHubConnection.invoke('GetMessages'));
  }

  public listenToMessageSent(): Observable<ChatMessage> {
    const newMessage$: Subject<ChatMessage> = new Subject<ChatMessage>();
    this.chatHubConnection.on('message_sent', (newMessage: ChatMessage) => newMessage$.next(newMessage))
    return newMessage$;
  }

  public sendMessage(message: ChatMessageToSend): Observable<WebSocketResponse> {
    return from(this.chatHubConnection.invoke('SendMessage', message));
  }

  public closeChatConnection(): void {
    this.chatHubConnection.stop();
  }
}
