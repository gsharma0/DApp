import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { member } from '../_models/member';
import { MemberService } from '../_services/member.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<member> {

  constructor(private memberService : MemberService){}

  resolve(route: ActivatedRouteSnapshot): Observable<member> {
    return this.memberService.getMember(route.paramMap.get('username'));
  }
}
