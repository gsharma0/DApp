import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { member } from '../_models/member';
import { PaginatedResult } from '../_models/Pagination';
import { UserParams } from '../_models/UserParams';
import { AccountService } from './account.service';
import { User } from 'src/app/_models/User';
import { getPaginatedResult, getPaginationHeaders } from './PaginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  baseUrl = environment.apiUrl;
  user :User;
  userParams : UserParams;
 /*  httpoptions = {
    headers: new HttpHeaders({
      Authorization: 'Bearer ' + JSON.parse(localStorage.getItem('user')).token
    })
  } */

  constructor(private http: HttpClient, private acctSer :AccountService) { 
    //Copied from member-list component to persidt filters
    this.acctSer.currUser$.pipe(take(1)).subscribe(user =>{
      this.user = user;
      this.userParams = new UserParams(user);
    })
  }

  /*  Commented as we used interceptor to pass http headers
  getMembers() {
    return this.http.get<member[]>(this.baseUrl + 'users', this.httpoptions)
  }

  getMember(username:string) {
    return this.http.get<member>(this.baseUrl + 'users/' + username, this.httpoptions)
  } */

  members : member[]=[];
  memberCache = new Map(); //act as dictionary
  

  // Commented to use Pagination 
  /* getMembers() {
    if(this.members.length > 0) return of(this.members);
    return this.http.get<member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members= members
        return members;
      }     
      ));
  }
 */

  //Methods to used to persist filters
  getUserParams(){
    return this.userParams;
  }

  setUserParams(params:UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    this.userParams = new UserParams(this.user);
    return this.userParams;
  }


  getMembers(userParams :UserParams) {

    var key = Object.values(userParams).join("-");
//implement to do caching
    var response = this.memberCache.get(key);
    if(response){
      return of(response);
    }
    
   let params = getPaginationHeaders(userParams.pageNumber,userParams.pageSize)

   params = params.append('minAge', userParams.minAge.toString());
   params = params.append('maxAge', userParams.maxAge.toString());
   params = params.append('gender', userParams.gender);
   params = params.append('orderBy', userParams.orderBy);
   
   return getPaginatedResult<member[]>(this.baseUrl + 'users', params,this.http)
   .pipe(map(response => { //implement to do caching
     this.memberCache.set(key,response);
     return response;
   }));
  }

 


  getMember(username:string) {
   // old method of caching, commented for pagination,sorting
    //const member = this.members.find(x=> x.userName===username);
   // if(member !== undefined) return of(member);

   //added new method of caching
    const member = [...this.memberCache.values()]
    .reduce((arr,elem) => arr.concat(elem.result), [])
    .find((member :member) => member.userName === username);

    if(member){
      return of(member);
    }
    return this.http.get<member>(this.baseUrl + 'users/' + username )
  }

  updateMember(member:member){
    return this.http.put(this.baseUrl + 'users/' , member).pipe(
      map(() =>  {
        const index = this.members.indexOf(member);
      this.members[index] = member;
      }))
  }

  setMainPhoto(photoId : number){
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId,{});
   }

   deletePhoto(photoId : number){
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId,{});
   }

   addLike(username:string){
    return this.http.post(this.baseUrl + "likes/" + username,{});
  }

  //Comented for new pagination methods
  // getLikes(predicate:string){
  //   return this.http.get<Partial<member[]>>(this.baseUrl + "likes?predicate=" + predicate);
  // }

   getLikes(predicate:string, pageNumber, pageSize){
    let params = getPaginationHeaders(pageNumber,pageSize)
    params = params.append('predicate', predicate);
    return getPaginatedResult<Partial<member[]>>(this.baseUrl + "likes", params,this.http);
  }

}
