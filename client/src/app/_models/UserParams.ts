import { User } from "./User";

export class UserParams{
pageNumber:number=1;
pageSize:number=6;
minAge:number=18;
maxAge:number=99;
gender:string;
orderBy = "lastActive";

constructor(user:User){
   this.gender = user.gender === 'female' ? 'male' : 'female';
}

}