import {NgModule} from '@angular/core';
import {SharedModule} from "@shared/shared.module";
import {GamePageComponent} from "@game/pages/game-page/game-page.component";
import {GameStoreService} from "@game/store/game-store.service";
import {WsGameService} from "@game/services/ws-game.service";
import {GameRoutingModule} from "@game/game-routing.module";
import {RouteFieldComponent} from "@game/components/route-field/route-field.component";
import {ScrollToCenterDirective} from "@game/directives/scroll-to-center.directive";
import {GamePlayersComponent} from "@game/components/game-players/game-players.component";
import {PlayerDetailsDialogComponent} from "@game/components/player-details-dialog/player-details-dialog.component";
import {PlayerHealthComponent} from "@game/components/player-health/player-health.component";
import {PlayerEquipmentComponent} from "@game/components/player-equipment/player-equipment.component";
import {EmptyRouteFieldComponent} from "@game/components/empty-route-field/empty-route-field.component";
import {FightDialogComponent} from "@game/components/fight-dialog/fight-dialog.component";
import {FieldRotationDialogComponent} from "@game/components/field-rotation-dialog/field-rotation-dialog.component";
import {MatMenuModule} from "@angular/material/menu";
import {FieldMenuDialogComponent} from "@game/components/field-menu-dialog/field-menu-dialog.component";
import {FieldRotationService} from "@game/services/field-rotation.service";
import {RoundStoreService} from "@game/store/round-store.service";
import {PlayersPositionsComponent} from "@game/components/players-positions/players-positions.component";
import {FieldInfoDialogComponent} from "@game/components/field-info-dialog/field-info-dialog.component";
import {CollectedTreasuresComponent} from "@game/components/collected-treasures/collected-treasures.component";
import {GameOverDialogComponent} from "@game/components/game-over-dialog/game-over-dialog.component";

@NgModule({
  declarations: [
    GamePageComponent,
    RouteFieldComponent,
    ScrollToCenterDirective,
    GamePlayersComponent,
    PlayerDetailsDialogComponent,
    PlayerHealthComponent,
    PlayerEquipmentComponent,
    EmptyRouteFieldComponent,
    FightDialogComponent,
    FieldRotationDialogComponent,
    FieldMenuDialogComponent,
    PlayersPositionsComponent,
    FieldInfoDialogComponent,
    CollectedTreasuresComponent,
    GameOverDialogComponent
  ],
  imports: [
    SharedModule,
    GameRoutingModule,
    MatMenuModule
  ],
  providers: [
    WsGameService,
    GameStoreService,
    FieldRotationService,
    RoundStoreService
  ],
})
export class GameModule { }
