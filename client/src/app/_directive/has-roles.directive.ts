import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/User';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRoles]'
})
export class HasRolesDirective implements OnInit {

  user:User;
  @Input() appHasRoles : string []; 
  constructor(private viewContainerRef:ViewContainerRef,private viewtemplateRef:TemplateRef<any>, private acctSer:AccountService) 
  { 
    this.acctSer.currUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }
  ngOnInit(): void {
    if(!this.user?.roles || this.user ==null){
      this.viewContainerRef.clear();
    }
    if(this.user?.roles.some(r=> this.appHasRoles.includes(r))){
      this.viewContainerRef.createEmbeddedView(this.viewtemplateRef);
    }else{
      this.viewContainerRef.clear();
    }

  }

}
