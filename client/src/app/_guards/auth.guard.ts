import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  canActivate(): Observable<boolean> {
    return this.accServ.currUser$.pipe(
      map(user => {
        if(user) {
          return true;}
          this.toast.error("you are not passed");
          return false;
             
      })
    )
  }
  constructor(private accServ:AccountService, private toast:ToastrService){

  }
}
