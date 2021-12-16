import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/Pagination';
import { User } from 'src/app/_models/User';
import { UserParams } from 'src/app/_models/UserParams';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  //members : member[];--commented for store data in service and use async operator in html for subscripe
 // members$ : Observable<member[]>; //commented to use pagination feature
 
 members:member[];
 userParams :UserParams;
 user :User;
 pagination:Pagination;
 genderList = [{value : 'male', display : 'Males'},{value :'female', display : 'Females'}];
 filterMode=false;
 
 //coommented to move code into memberService for persist filters
/*   constructor(private memberService : MemberService, private acctService :AccountService) {
     this.acctService.currUser$.pipe(take(1)).subscribe(user =>{
       this.user = user;
       this.userParams = new UserParams(user);
     })
   }
 */
   constructor(private memberService : MemberService) {
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    this.loadMembers();
    //this.members$ = this.memberService.getMembers();
  }
 /*  loadMembers(){
    this.memberService.getMembers().subscribe(members =>{
      this.members = members;
    }) 
  }*/
  //pageNumber :number =1;
  //itemsPerPage :number =5;


   loadMembers(){
this.memberService.setUserParams(this.userParams);

    //this.memberService.getMembers(this.pageNumber,this.itemsPerPage).subscribe(response =>{ //commented for pagination
      this.memberService.getMembers(this.userParams).subscribe(response =>{
      this.members = response.result;
      this.pagination = response.pagination;
    }) 
  }

  resetFilters(){
//this.userParams = new UserParams(this.user);
this.userParams = this.memberService.resetUserParams();
this.loadMembers();

  }

  pageChanged(event:any){
    //this.pageNumber= event.page;
    this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers();
  }

  filtertoggle(){
    this.filterMode = !this.filterMode;
  }


}
