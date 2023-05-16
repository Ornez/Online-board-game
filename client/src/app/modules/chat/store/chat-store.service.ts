import {Injectable} from "@angular/core";
import {BehaviorSubject, Subject, takeUntil} from "rxjs";
import {ChatMessage} from "../models/chat-message";
import {WsChatService} from "../services/ws-chat.service";
import {ChatMessageToSend} from "@chat/models/chat-message-to-send";
import {AuthService} from "@auth/services/auth.service";

@Injectable({
  providedIn: 'root'
})
export class ChatStoreService {

  readonly chatSubscription$: Subject<boolean> = new Subject<boolean>();

  readonly chat$: BehaviorSubject<ChatMessage[]> =  new BehaviorSubject<ChatMessage[]>([]);
  readonly numberOfUnreadMessages$: BehaviorSubject<number> =  new BehaviorSubject<number>(0);

  constructor(private ws: WsChatService,
              private auth: AuthService) {}

  public connectChatHub(): void {
    this.ws.initChatConnection().pipe(takeUntil(this.chatSubscription$)).subscribe( () => {
      this.ws
        .getMessages()
        .pipe(takeUntil(this.chatSubscription$))
        .subscribe((messages: ChatMessage[]) => this.chat$.next(messages));

      this.ws
        .listenToMessageSent()
        .pipe(takeUntil(this.chatSubscription$))
        .subscribe((newMessage: ChatMessage) => {
          this.chat$.value.push(newMessage);
          this.chat$.next(this.chat$.value);
          newMessage.username !== this.auth.getUsername() ? this.numberOfUnreadMessages$.next(this.numberOfUnreadMessages$.value + 1) : undefined;
        });
    })
  }

  public sendMessage(message: ChatMessageToSend): void {
    this.ws.sendMessage(message);
  }

  public clearUnreadMessages(): void {
    this.numberOfUnreadMessages$.next(0);
  }

  public disconnectChatHub(): void {
    this.ws.closeChatConnection();
    this.chatSubscription$.next(true);
    this.chatSubscription$.complete();
    this.chat$.next([]);
    this.numberOfUnreadMessages$.next(0);
  }
}
