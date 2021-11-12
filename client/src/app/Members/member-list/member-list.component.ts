import { Component, OnInit } from '@angular/core';
import { member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members : member[];
  constructor(private memberService : MemberService) { }

  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers(){
    this.memberService.getMembers().subscribe(members =>{
      this.members = members;
    })
  }

}
