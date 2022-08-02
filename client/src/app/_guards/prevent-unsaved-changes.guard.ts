import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../Members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmServce : ConfirmService){}

  canDeactivate( component: MemberEditComponent): Observable<boolean> |  boolean  {
    if(component.editForm.dirty){
      //return confirm("Are you sure?");
      return this.confirmServce.confirm();
    }
    return true;
  }
  
}
