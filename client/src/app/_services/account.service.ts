import { HttpClient, JsonpClientBackend } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/User';
import { PresenceService } from './presence.service';


@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currUserSource = new ReplaySubject<User | null>(1);
  currUser$ = this.currUserSource.asObservable();

  constructor(private http: HttpClient, private presence :PresenceService) { }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrUser(user);
          this.presence.CreateHubConnection(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currUserSource.next(user);
          this.presence.CreateHubConnection(user);
        }
        return user;
      }
      

      )
    )
  }

  setCurrUser(user: User) {
    user.roles = [];
    const roles =this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currUserSource.next(null);
    this.presence.StopHubConnection();
  }

  getDecodedToken(token:string){
    return JSON.parse(atob(token.split('.')[1]));
  }

}
