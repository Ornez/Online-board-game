import {Component, Inject, OnInit} from "@angular/core";
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {Fight} from "@game/models/fight";
import {Scroll} from "@game/enums/scroll.enum";
import {FightState} from "@game/enums/fight-state.enum";
import {GameStoreService} from "@game/store/game-store.service";
import {Player} from "@game/models/player";
import {RoundStoreService} from "@game/store/round-store.service";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {takeUntil} from "rxjs";
import {FightResult} from "@game/enums/fight-result.enum";

@Component({
  selector: 'player-details-dialog',
  templateUrl: './fight-dialog.component.html',
  styleUrls: ['./fight-dialog.component.scss']
})
export class FightDialogComponent extends BaseComponent implements OnInit {

  fightPlayer: Player;
  fightState: FightState = FightState.BEGIN;
  usedScrolls: boolean[] = [false, false, false];
  fightResult: FightResult = undefined;

  diceRolledResult: number[] = [0, 0];
  playerStrength: number = 0;

  FightState = FightState;
  Scroll = Scroll;

  get selectedScrolls(): number {
    return this.usedScrolls.filter(element => element === true).length;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public fight: Fight,
              private gamedStore: GameStoreService,
              public roundStore: RoundStoreService) {
    super();
  }

  public ngOnInit() {
    this.initDataHandle();
    this.fightPlayer = this.gamedStore.players$.value.find((player: Player) => player.userData.username === this.fight.userData.username)
  }

  public onRollTheDice(): void {
    this.gamedStore.rollTheDice(this.selectedScrolls);
    this.fightState = FightState.ROLL_THE_DICE;
  }

  public onToggleUsedScroll(index: number): void {
    if (this.roundStore.isCurrentPlayersRound()) {
      this.usedScrolls[index] = !this.usedScrolls[index];
    }
    this.gamedStore.calculateFight(this.selectedScrolls);
  }

  public onFight(): void {
    this.fightState = FightState.FIGHT;
    this.gamedStore.fight(this.selectedScrolls)
  }

  public isScrollSelected(index: number): boolean {
    return this.usedScrolls[index];
  }

  private initDataHandle(): void {
    this.roundStore.diceRolledResult$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((result: number[]) => this.diceRolledResult = result);

    this.roundStore.playerStrength$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((strength: number) => this.playerStrength = strength);

    this.roundStore.numberOfUsedScrolls$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((numberOfScrolls: number) => {
        this.usedScrolls = [false, false, false];
        for (let index = 0; index < numberOfScrolls; index++) {
          this.usedScrolls[index] = true;
        }
      });

    this.roundStore.fightResult$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((fightResult: FightResult) => this.fightResult = fightResult)
  }
}
