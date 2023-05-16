import {Component, Input} from "@angular/core";

@Component({
  selector: 'player-health',
  templateUrl: './player-health.component.html',
  styleUrls: ['./player-health.component.scss']
})
export class PlayerHealthComponent {

  @Input() healthPoints: number = 0;

  public loopRange(range: number): Array<any> {
    return new Array<any>(range);
  }
}
