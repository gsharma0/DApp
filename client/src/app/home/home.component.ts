import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode=false;
  users:any={};
  constructor(private http:HttpClient) { }

  ngOnInit(): void {
    //this.getUsers();
  }
  registertoggle(){
    this.registerMode = !this.registerMode;
  }

  getUsers(){
    this.http.get( environment.apiUrl + 'users').subscribe(response =>{
      this.users =response;
    })
  } 

  cancelRegistration(event:boolean){
    this.registerMode=event;
  }

}
