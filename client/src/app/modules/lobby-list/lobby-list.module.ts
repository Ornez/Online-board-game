import {NgModule} from '@angular/core';
import {SharedModule} from "@shared/shared.module";
import {LobbyAuthDialogComponent} from "@lobby-list/components/lobby-auth-dialog/lobby-auth-dialog.component";
import {LobbyListDialogComponent} from "@lobby-list/components/lobby-list-dialog/lobby-list-dialog.component";
import {LobbyPlayersDetailsTooltipComponent} from "@lobby-list/components/lobby-players-details-tooltip/lobby-players-details-tooltip.component";
import {PlayersDetailsDirective} from "@lobby-list/directives/players-details.directive";
import { CreateLobbyDialogComponent } from "@lobby-list/components/create-lobby-dialog/create-lobby-dialog.component";
import {
  FindLobbyDialogComponent
} from "@lobby-list/components/find-lobby-dialog/find-lobby-dialog.component";

@NgModule({
  declarations: [
    LobbyAuthDialogComponent,
    LobbyListDialogComponent,
    LobbyAuthDialogComponent,
    LobbyPlayersDetailsTooltipComponent,
    FindLobbyDialogComponent,
    CreateLobbyDialogComponent,
    PlayersDetailsDirective
  ],
  imports: [
    SharedModule
  ],
  providers: []
})
export class LobbyListModule { }
