import { Component, OnDestroy, OnInit } from '@angular/core';
import { StatisticsStoreService } from "@core/store/statistics-store.service";
import { BaseComponent } from "@shared/common/base-component.abstract";
import { takeUntil } from "rxjs";
import { StatisticsData } from "@core/models/statistics-data";

@Component({
  selector: 'statistics-dialog',
  templateUrl: './statistics-dialog.component.html',
  styleUrls: ['./statistics-dialog.component.scss'],
})
export class StatisticsDialogComponent extends BaseComponent implements OnInit, OnDestroy{
  statistics: StatisticsData;

  constructor(public statisticsStore: StatisticsStoreService) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
    this.statisticsStore.getStatistics();
  }

  public ngOnDestroy(): void {
    super.ngOnDestroy();
    this.statisticsStore.clearStatistics();
  }

  private initDataHandle(): void {
    this.statisticsStore.statistics$
      .pipe(takeUntil(this.destroyed$))
      .subscribe((statistics: StatisticsData) => this.statistics = statistics)
  }
}
