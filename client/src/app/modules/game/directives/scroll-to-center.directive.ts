import {AfterViewInit, Directive, ElementRef} from "@angular/core";

@Directive({
  selector: '[scrollToCenter]'
})
export class ScrollToCenterDirective implements AfterViewInit{

  constructor(private elementRef: ElementRef) {}

  public ngAfterViewInit(): void {
    const scrollY: number = 3150 / 2 - window.innerHeight / 2;
    const scrollX: number = 3150 / 2 - window.innerWidth / 2;
    this.elementRef.nativeElement.scroll(scrollX, scrollY);
  }
}
