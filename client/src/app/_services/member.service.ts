import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
 /*  httpoptions = {
    headers: new HttpHeaders({
      Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user')).token
    })
  } */

  constructor(private http: HttpClient) { }

  /*  Commented as we used interceptor to pass http headers
  getMembers() {
    return this.http.get<member[]>(this.baseUrl + 'users', this.httpoptions)
  }

  getMember(username:string) {
    return this.http.get<member>(this.baseUrl + 'users/' + username, this.httpoptions)
  } */

  members : member[]=[];

  getMembers() {
    if(this.members.length > 0) return of(this.members);
    return this.http.get<member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members= members
        return members;
      }     
      ));
  }

  getMember(username:string) {
    const member = this.members.find(x=> x.userName===username);
    if(member !== undefined) return of(member);
    return this.http.get<member>(this.baseUrl + 'users/' + username)
  }

  updateMember(member:member){
    return this.http.put(this.baseUrl + 'users/' , member).pipe(
      map(() =>  {
        const index = this.members.indexOf(member);
      this.members[index] = member;

      }))
  }

}
