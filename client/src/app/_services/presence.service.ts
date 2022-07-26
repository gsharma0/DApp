import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  huburl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineusers$ = this.onlineUsersSource.asObservable();

  constructor(private toaster: ToastrService, private router: Router) { }

  CreateHubConnection(user: User) {

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.huburl + 'presence', { accessTokenFactory: () => user.token })
      .withAutomaticReconnect()
      .build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      // this.toaster.info(username + ' is online');
      this.onlineusers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames, username]);
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      //this.toaster.warning(username + ' is offline');
      this.onlineusers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x=> x !== username)]);
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived',({username, knownAs})=>{
      this.toaster.info(knownAs + ' has send a message!')
      .onTap
      .pipe(take(1))
      //.subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'))
      .subscribe(() => this.router.navigate(['/members/'+ username],{queryParams:{tab:3}}))
      
    })

  }

  StopHubConnection(){
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
