import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { member } from 'src/app/_models/member';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild("editForm") editForm:NgForm;
  @HostListener('window:beforeunload',['$event']) unloadNotify($event:any){
    if(this.editForm.dirty){
     $event.returnValue=true;
    }
  }

  member:member;
  user: User;

  constructor(private acctServ: AccountService, private memberServ: MemberService, private toast :ToastrService) {
    this.acctServ.currUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.getMember();
  }

  getMember() {
    this.memberServ.getMember(this.user.username).subscribe(member => this.member= member);
  }
  updateMember(){
    console.log(this.member);
    this.memberServ.updateMember(this.member).subscribe(()=>{
      this.toast.success("Profile updated successfully")
      this.editForm.reset(this.member);
    })  
  }

}
