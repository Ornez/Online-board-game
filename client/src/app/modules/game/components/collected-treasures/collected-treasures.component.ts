import {Component, Input} from "@angular/core";

@Component({
  selector: 'collected-treasures',
  templateUrl: './collected-treasures.component.html',
  styleUrls: ['./collected-treasures.component.scss']
})
export class CollectedTreasuresComponent {

  @Input() treasures: number;

  get treasuresArray(): any[] {
    return new Array(Math.round(this.treasures));
  }

}
