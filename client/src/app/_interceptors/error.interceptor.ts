import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private route: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          switch (error.status) {
            case 400:
              console.log(error);
              console.log(error.error.errors);
                if(error.error.errors){
                 
                  const modalStateErrors=[];
                  for(const key in error.error.errors){
                    modalStateErrors.push(error.error.errors[key]);
                  }
                  throw modalStateErrors.flat();
                  //console.log(modalStateErrors.flat());
                  //this.toastr.error(modalStateErrors.flat());
                }else if (typeof(error.error) === 'object'){
                  this.toastr.error(error.statusText,error.status)
                }
                else{
                  this.toastr.error(error.error, error.status);
                }
                
              break;
            case 404:
              this.route.navigateByUrl("/not-found");
              break;
              case 401:
                //this.route.navigateByUrl("/not-found");
                this.toastr.error("Unauthorized request");
                break;
            case 500:
              const naviExtra:NavigationExtras={state:{error:error.error}}
              this.route.navigateByUrl("/server-error",naviExtra);
              break;
            default:
              this.toastr.error("Something went wrong");
              //console.log(error);
              break;
          }
        }
        return throwError(error.error);
      })
    )
  }
}
