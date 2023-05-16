import { NgModule } from '@angular/core';
import { SharedModule } from "@shared/shared.module";
import { LobbyRoutingModule } from "@lobby/lobby-routing.module";
import { ChatModule } from "@chat/chat.module";
import { LobbyPageComponent } from "@lobby/pages/lobby-page/lobby-page.component";
import { LobbyPlayersComponent } from "@lobby/components/lobby-players/lobby-players.component";

@NgModule({
  declarations: [
    LobbyPageComponent,
    LobbyPlayersComponent
  ],
  imports: [
    SharedModule,
    LobbyRoutingModule,
    ChatModule
  ]
})
export class LobbyModule {
  constructor() {}
}
