import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  currentUser$: Observable<User | null>;
  // loggedIn:boolean=false;
  constructor(private accountService: AccountService,private route:Router
    ,private toastr:ToastrService) { }

  ngOnInit(): void {
    //this.getCurrentUser();
    this.currentUser$ = this.accountService.currUser$;
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      //console.log(response);
      //this.loggedIn =true;
      this.route.navigateByUrl('/members');
    }, error => {
      //this.loggedIn =false;
      //console.log(error);
      this.toastr.error(error);
    })
  }

  loggedOut() {
    this.accountService.logout();
    this.route.navigateByUrl('/');
    //this.loggedIn=false;
  }

  /* getCurrentUser(){
    this.accountService.currUser$.subscribe(user =>{
      this.loggedIn =!!user;
    }) 
  }*/

}
