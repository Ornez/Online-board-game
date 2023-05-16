import { Component, Input } from '@angular/core';
import {LobbyPlayer} from "@lobby-list/models/lobby-player";

@Component({
  selector: 'players-details-tooltip',
  templateUrl: './lobby-players-details-tooltip.component.html',
  styleUrls: ['./lobby-players-details-tooltip.component.scss'],
})
export class LobbyPlayersDetailsTooltipComponent {

  @Input() playersDetails: LobbyPlayer[] = []

  constructor() {}

}
