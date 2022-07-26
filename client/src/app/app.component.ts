import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/User';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  ngOnInit() {
    //this.getUsers();
    this.SetCurrentUser();
  }
  title = 'client';
  users :any;

  //See Comment below
 // constructor(private http:HttpClient, private accSer: AccountService){
    constructor( private accSer: AccountService, private presence:PresenceService){
  
  }
  

  SetCurrentUser(){
    const user :User = JSON.parse(localStorage.getItem('user')!);    
    if(user){
      this.accSer.setCurrUser(user);
      this.presence.CreateHubConnection(user);
    }
  }

  //Moved to home component to test Parent to child communication
  /* getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(response =>{
      this.users =response;
    },error =>{
      console.log(error);
    })
  } */

}


