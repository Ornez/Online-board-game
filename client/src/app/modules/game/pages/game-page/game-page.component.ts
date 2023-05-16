import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from "@angular/core";
import {GameStoreService} from "@game/store/game-store.service";
import {Toolbar} from "@core/enums/toolbar.enum";
import {ActivatedRoute} from "@angular/router";
import {ChatStoreService} from "@chat/store/chat-store.service";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {filter, takeUntil} from "rxjs";
import {RouteField} from "@game/models/route-field";
import {BoardUtils} from "@game/utils/board-utils";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {FieldRotationDialogComponent} from "@game/components/field-rotation-dialog/field-rotation-dialog.component";
import {FieldMenuDialogComponent} from "@game/components/field-menu-dialog/field-menu-dialog.component";
import {FieldMenuAction} from "@game/enums/field-menu-action.enum";
import {ObjectUtils} from "@shared/utils/object-utils";
import {AuthService} from "@auth/services/auth.service";
import {UserData} from "@game/models/user-data";
import {FieldRotationService} from "@game/services/field-rotation.service";
import {PossibleRouteField} from "@game/models/possible-route-field";
import {Position} from "@game/models/position";
import {RoundStoreService} from "@game/store/round-store.service";
import {FieldInfoDialogComponent} from "@game/components/field-info-dialog/field-info-dialog.component";
import {Player} from "@game/models/player";
import {PlayerMovement} from "@game/models/player-movement";
import {GameOverDialogComponent} from "@game/components/game-over-dialog/game-over-dialog.component";
import {GameResults} from "@game/models/game-results";

@Component({
  selector: 'game-page',
  templateUrl: './game-page.component.html',
  styleUrls: ['./game-page.component.scss']
})
export class GamePageComponent extends BaseComponent implements OnInit, OnDestroy{
  routeFields: RouteField[] = [];
  possibleRouteFields: PossibleRouteField[] = [];
  fieldsToMovePlayer: Position[] = [];
  players: Player[] = [];

  fieldRotationDialogRef: MatDialogRef<FieldRotationDialogComponent>;
  currentRoundPlayer: UserData;

  Toolbar = Toolbar;
  BoardUtils = BoardUtils;

  @ViewChild('fieldsWrapper') fieldsWrapper: ElementRef<HTMLDivElement>;

  constructor(public gameStore: GameStoreService,
              private chatStore: ChatStoreService,
              public roundStore: RoundStoreService,
              public auth: AuthService,
              public fieldRotation: FieldRotationService,
              private route: ActivatedRoute,
              private dialog: MatDialog) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
    this.gameStore.connectGameHub();
  }

  public onOpenFieldMenu(event, field: RouteField): void {
    const mousePosition: Position = {x: event.clientX, y: event.clientY};
    const menuItems: FieldMenuAction[] = [FieldMenuAction.SCROLL_TO_POINT, FieldMenuAction.SHOW_INFO];

    if (this.canPlayerMoveToField(field.position.x, field.position.y)) {
      menuItems.push(this.hasFieldEnemy(field.position.x, field.position.y) ? FieldMenuAction.FIGHT : FieldMenuAction.MOVE);
    }
    if (this.canPlayerOpenChest(field)) {
      menuItems.push(FieldMenuAction.OPEN_CHEST);
    }
    if (this.canPlayerPickUpItem(field)) {
      menuItems.push(FieldMenuAction.PICK_UP_ITEM);
    }

    this.dialog
      .open(FieldMenuDialogComponent, {data: menuItems, autoFocus: false, hasBackdrop: true, backdropClass: 'mat-dialog--no-background'})
      .updatePosition({left: mousePosition.x + 10 + 'px', top: mousePosition.y + 10 + 'px'})
      .afterClosed()
      .pipe(filter(fieldMenuAction => !ObjectUtils.isNil(fieldMenuAction)))
      .subscribe((fieldMenuAction: FieldMenuAction) => this.fieldMenuActions(fieldMenuAction, mousePosition, field));
  }

  private fieldMenuActions(fieldMenuAction: FieldMenuAction, mousePosition: Position, field: RouteField): void {
    switch(fieldMenuAction) {
      case FieldMenuAction.SCROLL_TO_POINT: {
        const scrollY: number = this.fieldsWrapper.nativeElement.scrollTop - window.innerHeight / 2 + mousePosition.y;
        const scrollX: number = this.fieldsWrapper.nativeElement.scrollLeft - window.innerWidth / 2 + mousePosition.x;
        this.fieldsWrapper.nativeElement.scroll(scrollX, scrollY);
        break;
      }
      case FieldMenuAction.SHOW_INFO: {
        const playersOnField = this.players.filter( player => player.position.x === field.position.x && player.position.y === field.position.y)

        this.dialog
          .open(FieldInfoDialogComponent, {data: {enemy: field.enemy, players: playersOnField, itemName: field.itemName, chest: field.chest}, autoFocus: false, hasBackdrop: true, backdropClass: 'mat-dialog--no-background', maxHeight: 'calc(100vh - 360px)', width: '350px' })
          .updatePosition({right: '20px', top: '110px'})
        break;
      }
      case FieldMenuAction.MOVE:
      case FieldMenuAction.FIGHT:{
        this.gameStore.movePlayer(field.position);
        break;
      }
      case FieldMenuAction.OPEN_CHEST: {
        this.gameStore.openChest();
        break;
      }
      case FieldMenuAction.PICK_UP_ITEM: {
        this.gameStore.pickUpItem();
        break;
      }
    }
  }

  public onSetRouteField(field: PossibleRouteField): void {
    this.gameStore.setRouteField(field.position, this.fieldRotation.getCurrentRotation());
  }

  public onDrawRouteField(): void {
    this.gameStore.getRouteField();
  }

  public onRoundOver(): void {
    this.gameStore.roundOver();
  }

  public canPlayerMoveToField(x: number, y: number): boolean {
    const isPlayerRound: boolean = this.roundStore.isCurrentPlayersRound();
    const canPlayerMoveToField: boolean = this.fieldsToMovePlayer.some((position: Position) => position.x === x && position.y ===y)
    return isPlayerRound && canPlayerMoveToField;
  }

  private canPlayerOpenChest(field: RouteField): boolean {
    const currentPlayer: Player = this.players.find( (player: Player) => player.userData.username === this.currentRoundPlayer.username);
    const hasFieldChest: boolean = field.chest;
    const isPlayerRound: boolean = this.roundStore.isCurrentPlayersRound();
    const hasPlayerAKey: boolean = currentPlayer.equipment.key
    const hasPlayerStandOnChestField = currentPlayer.position.x === field.position.x && currentPlayer.position.y === field.position.y;
    return hasFieldChest && isPlayerRound && hasPlayerAKey && hasPlayerStandOnChestField;
  }

  private canPlayerPickUpItem(field: RouteField): boolean {
    const currentPlayer: Player = this.players.find( (player: Player) => player.userData.username === this.currentRoundPlayer.username);
    const hasFieldItem: boolean = !ObjectUtils.isNil(field.itemName);
    const isPlayerRound: boolean = this.roundStore.isCurrentPlayersRound();
    const hasPlayerStandOnItemField = currentPlayer.position.x === field.position.x && currentPlayer.position.y === field.position.y;
    return hasFieldItem && isPlayerRound && hasPlayerStandOnItemField;
  }

  public hasFieldEnemy(x: number, y: number): boolean {
    return !ObjectUtils.isNil(this.routeFields.find( (field: RouteField) => field.position.x === x && field.position.y === y).enemy);
  }

  private movePlayer(movement: PlayerMovement): void {
    const player = this.players.find((player: Player) => player.userData.username === movement.userData.username);
    movement.path.forEach((position: Position) => player.position = position)
  }

  private initDataHandle(): void {
    this.gameStore.routeFields$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((routeFields: RouteField[]) => this.routeFields = routeFields);

    this.gameStore.players$
      .pipe(
        takeUntil(this.destroyed$),
        filter(players => !ObjectUtils.isNil(players)))
      .subscribe((players: Player[]) => this.players = players);

    this.gameStore.playerMoved$
      .pipe(
        takeUntil(this.destroyed$),
        filter(movement => !ObjectUtils.isNil(movement)))
      .subscribe((movement: PlayerMovement) => this.movePlayer(movement));

    this.gameStore.gameOver$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((results: GameResults) => this.dialog.open(GameOverDialogComponent, {data: results, width: '800px', hasBackdrop: true, backdropClass: 'mat-dialog--no-clickable-background'}));

    this.fieldRotation.possibleFieldsWithCurrentFieldRotation$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((possibleRouteFields: PossibleRouteField[]) => this.possibleRouteFields = possibleRouteFields);

    this.roundStore.currentRoundPlayer$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((userData: UserData) => this.currentRoundPlayer = userData);

    this.roundStore.positionsToMove$
      .pipe(
        takeUntil(this.destroyed$),
        filter(positions => !ObjectUtils.isNil(positions)))
      .subscribe((positions: Position[]) => this.fieldsToMovePlayer = positions);
  }

  public ngOnDestroy(): void {
    this.chatStore.disconnectChatHub();
    this.gameStore.disconnectGameHub();
    this.roundStore.closeFightDialog();
    this.roundStore.closeFieldRotationDialog();
    this.fieldRotationDialogRef ? this.fieldRotationDialogRef.close() : undefined;
  }
}
