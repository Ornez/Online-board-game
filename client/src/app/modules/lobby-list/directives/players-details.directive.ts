import { Directive, ElementRef, HostListener, Input, OnDestroy } from "@angular/core";
import { Overlay, OverlayPositionBuilder, OverlayRef } from "@angular/cdk/overlay";
import { ComponentPortal } from "@angular/cdk/portal";
import {LobbyPlayer} from "@lobby-list/models/lobby-player";
import {LobbyPlayersDetailsTooltipComponent} from "@lobby-list/components/lobby-players-details-tooltip/lobby-players-details-tooltip.component";

@Directive({
  selector: '[playersDetails]'
})
export class PlayersDetailsDirective implements OnDestroy{

  @Input() playersDetails: LobbyPlayer[] = [];
  private overlayRef: OverlayRef;

  constructor(private overlay: Overlay,
              private overlayPositionBuilder: OverlayPositionBuilder,
              private elementRef: ElementRef) {
  }

  @HostListener('mouseenter')
  onMouseEnter(): void {
    const positionStrategy = this.overlayPositionBuilder
      .flexibleConnectedTo(this.elementRef)
      .withPositions([{
        originX:  'start', originY:  'bottom',
        overlayX: 'start', overlayY: 'top',
        offsetX:  -20,
      }]);

    this.overlayRef = this.overlay.create({ positionStrategy});
    const tooltipRef = this.overlayRef.attach(new ComponentPortal(LobbyPlayersDetailsTooltipComponent))
    tooltipRef.instance.playersDetails = this.playersDetails;
  }

  @HostListener('mouseleave')
  onMouseLeave(): void {
    this.overlayRef ? this.overlayRef.detach() : undefined;
  }

  public ngOnDestroy(): void {
    this.onMouseLeave();
  }
}
