import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
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

  getMembers() {
    return this.http.get<member[]>(this.baseUrl + 'users')
  }

  getMember(username:string) {
    return this.http.get<member>(this.baseUrl + 'users/' + username)
  }

}
