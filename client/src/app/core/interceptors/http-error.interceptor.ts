import { Injectable } from "@angular/core";
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import {MessageService} from "@core/services/message.service";

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

  constructor(private message: MessageService) {}

  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          this.message.showErrorMessage(error.error);
          return throwError(error);
        })
      )
  }
}
