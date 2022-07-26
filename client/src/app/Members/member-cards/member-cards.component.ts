import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { member } from 'src/app/_models/member';
import { MemberService } from 'src/app/_services/member.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-cards',
  templateUrl: './member-cards.component.html',
  styleUrls: ['./member-cards.component.css']
})
export class MemberCardsComponent implements OnInit {

@Input() member : member;

  constructor(private memberService : MemberService, private toaster : ToastrService, 
    public presence:PresenceService) { }

  ngOnInit(): void {
  }

  addLike(member : member){
    this.memberService.addLike(member.userName).subscribe(()=>{
      this.toaster.success("You have liked " + member.knownAs);
    })
  }

}
