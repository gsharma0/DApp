import { HttpClient } from '@angular/common/http';
import { Container } from '@angular/compiler/src/i18n/i18n_ast';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Group } from '../_models/group';
import { Message } from '../_models/message';
import { User } from '../_models/User';
import { getPaginatedResult, getPaginationHeaders } from './PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  baseUrl = environment.apiUrl;
  hubUrl =environment.hubUrl;
  private hubConnection:HubConnection;
  private messageThreadsSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadsSource.asObservable();

  constructor(private http:HttpClient) { }

  createHubConnection(user:User, otherusername:string){
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'message?user=' + otherusername,{
      accessTokenFactory:() => user.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("RecieveMessageThread", messsages =>{
      this.messageThreadsSource.next(messsages);
    })

    this.hubConnection.on("NewMessage",message =>{
      this.messageThread$.pipe(take(1)).subscribe(messages =>{
        this.messageThreadsSource.next([...messages,message]);
      })
    })

    this.hubConnection.on("UpdatedGroup", (group: Group) =>{
      if(group.connections.some(x=>x.username === otherusername)){
        this.messageThread$.pipe(take(1)).subscribe(messages =>{
          messages.forEach(message=>{
            if(!message.dateRead){
              message.dateRead = new Date(Date.now());
            }
          })
          this.messageThreadsSource.next([...messages]);
        })
      }
      
    })  

  }

  stopHubConnection(){
    if(this.hubConnection){
      this.hubConnection.stop();
    }    
  }

  stopHubConnection1(username:string){
    if(this.hubConnection){
      this.hubConnection.stop();
    }    
  }

  getMessages(pageNumber,pageSize,container){
    let params = getPaginationHeaders(pageNumber,pageSize);
    params = params.append('Container', container)
   return  getPaginatedResult<Message[]>(this.baseUrl + 'message',params, this.http);
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl + "message/thread/" + username);
  }

  async sendMessage(userName:string,content:string){
    //commented to use signalR instead of message service
    // return this.http.post<Message>(this.baseUrl + 'message', {recipientUsername :userName,content});
    return this.hubConnection.invoke('SendMessage', {recipientUsername :userName,content})
    .catch(error=>console.log(error));
  }

  deleteMessage(id:number){
    return this.http.delete(this.baseUrl + 'message/'+ id);
  }
}
