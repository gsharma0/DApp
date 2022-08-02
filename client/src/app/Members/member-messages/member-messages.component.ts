import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection:ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm :NgForm;
 @Input() username :string;
  /*messages : Message[]; */
  @Input() messages : Message[];
  messageContent:string;


  constructor(public messageService:MessageService) { }

  ngOnInit(): void {
    //this.loadMessageThread();
  }
//moved this code to member-detail component (Parent Component) to load only on tab select
  /* loadMessageThread(){
    this.messageService.getMessageThread(this.username).subscribe(response =>{
      this.messages = response;
    })
  } */
//commented t use SignalR(below method) instead of message Service
 /*  createMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message =>{
      this.messages.push(message);
      this.messageForm.reset();
    }) */

    createMessage(){
      this.messageService.sendMessage(this.username, this.messageContent).then(() =>{
        this.messageForm.reset();
      })


  }

}
