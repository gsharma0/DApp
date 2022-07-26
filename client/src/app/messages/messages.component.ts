import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/Pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

messages : Message[]=[];
pagination:Pagination;
container = "Inbox";
pageSize=5;
pageNumber=1;
loading = false;

  constructor(private messageservice: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageservice.getMessages(this.pageNumber, this.pageSize,this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading =false;
    })
  }

  pageChanged(event:any){
    if(this.pageNumber !=event.page){
      this.pageNumber =event.page;
      this.loadMessages();
    }
  }

  deleteMessage(id: number){
    this.messageservice.deleteMessage(id).subscribe(()=>{
      this.messages.splice(this.messages.findIndex(m=>m.id === id), 1);
      if(this.messages.length == 1){
        this.loadMessages();
      }      
    })
  }


}

