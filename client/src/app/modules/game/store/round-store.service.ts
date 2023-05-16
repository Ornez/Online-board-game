import {Injectable} from "@angular/core";
import {BehaviorSubject, Subject} from "rxjs";
import {UserData} from "@game/models/user-data";
import {AuthService} from "@auth/services/auth.service";
import {Position} from "@game/models/position";
import {Fight} from "@game/models/fight";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {FightDialogComponent} from "@game/components/fight-dialog/fight-dialog.component";
import {FightResult} from "@game/enums/fight-result.enum";
import {FieldRotationDialogComponent} from "@game/components/field-rotation-dialog/field-rotation-dialog.component";

@Injectable()
export class RoundStoreService {

  readonly currentRoundPlayer$: BehaviorSubject<UserData> = new BehaviorSubject<UserData>(undefined);
  readonly movementPoints$: BehaviorSubject<number> = new BehaviorSubject<number>(4);
  readonly positionsToMove$: BehaviorSubject<Position[]> = new BehaviorSubject<Position[]>([]);

  readonly isDrawFieldButtonVisible$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  readonly isRoundOverButtonVisible$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  readonly diceRolledResult$: BehaviorSubject<number[]> = new BehaviorSubject<number[]>([0, 0]);
  readonly playerStrength$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  readonly numberOfUsedScrolls$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  readonly fightResult$: Subject<FightResult> = new Subject<FightResult>();

  private fightDialogRef: MatDialogRef<FightDialogComponent>;
  private fieldRotationDialogRef: MatDialogRef<FieldRotationDialogComponent>;

  constructor(private auth: AuthService,
              private dialog: MatDialog) {}

  public start(currentUser: UserData): void {
    this.currentRoundPlayer$.next(currentUser);
    this.movementPoints$.next(4);
    this.positionsToMove$.next([]);
    this.showDrawFieldButton();
    this.showRoundOverButton();
    this.diceRolledResult([0, 0]);
    this.setPlayerStrength(0);
    this.setNumberOfUsedScrolls(0);
    this.closeFightDialog();
    this.closeFieldRotationDialog();
  }

  public setMovementPoints(movementPoints: number): void {
    this.movementPoints$.next(movementPoints);
    movementPoints === 0 ? this.hideDrawFieldButton() : undefined;
  }

  public setPositionsToMove(positions: Position[]): void {
    this.positionsToMove$.next(positions);
  }

  public removePositionsToMove(): void {
    this.positionsToMove$.next([]);
  }

  public showDrawFieldButton(): void {
    const canUserDrawField: boolean = this.movementPoints$.value > 0 && this.isCurrentPlayersRound();
    this.isDrawFieldButtonVisible$.next(canUserDrawField);
  }

  public hideDrawFieldButton(): void {
    this.isDrawFieldButtonVisible$.next(false);
  }

  public showRoundOverButton(): void {
    this.isRoundOverButtonVisible$.next(this.isCurrentPlayersRound());
  }

  public openFieldRotationDialog(): void {
    this.closeFieldRotationDialog();
    this.removePositionsToMove();
    this.fieldRotationDialogRef = this.dialog.open(FieldRotationDialogComponent, {autoFocus: false, hasBackdrop: false});
    this.fieldRotationDialogRef.updatePosition({ right: '20px', bottom: '20px' });

  }

  public closeFieldRotationDialog(): void {
    this.fieldRotationDialogRef ? this.fieldRotationDialogRef.close() : undefined
  }

  public openFightDialog(fight: Fight): void {
    this.fightDialogRef = this.dialog.open(FightDialogComponent, {data: fight, autoFocus: false, hasBackdrop: true, backdropClass: 'mat-dialog--no-clickable-background'});
  }

  public closeFightDialog(): void {
    this.fightDialogRef ? this.fightDialogRef.close() : undefined;
  }

  public diceRolledResult(result: number[]): void {
    this.diceRolledResult$.next(result);
  }

  public setPlayerStrength(strength: number): void {
    this.playerStrength$.next(strength);
  }

  public setNumberOfUsedScrolls(numberOfScrolls: number): void {
    this.numberOfUsedScrolls$.next(numberOfScrolls);
  }

  public setFightResult(fightResult: FightResult): void {
    this.fightResult$.next(fightResult);
  }

  public isCurrentPlayersRound(): boolean {
    return this.currentRoundPlayer$?.value?.username === this.auth.getUsername();
  }
}
