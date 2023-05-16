import {Component, EventEmitter, Input, Output} from "@angular/core";
import {Equipment} from "@game/models/equipment";
import {Scroll} from "@game/enums/scroll.enum";
import {ObjectUtils} from "@shared/utils/object-utils";

@Component({
  selector: 'player-equipment',
  templateUrl: './player-equipment.component.html',
  styleUrls: ['./player-equipment.component.scss']
})
export class PlayerEquipmentComponent {

  @Input() equipment: Equipment;
  @Output() useScroll: EventEmitter<void> = new EventEmitter<void>();

  public useHealingScroll(scroll: Scroll): void {
    if (!ObjectUtils.isNil(scroll) && scroll === Scroll.HEALING_SCROLL) {
      this.useScroll.emit();
    }
  }
}
