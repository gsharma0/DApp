import { Component, OnInit } from '@angular/core';
import { member } from '../_models/member';
import { PaginatedResult, Pagination } from '../_models/Pagination';
import { MemberService } from '../_services/member.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
members : Partial<member[]>;
predicate='liked';
pageNumber=1;
pageSize =5;
pagination:Pagination;


  constructor(private memberService : MemberService) { }

  ngOnInit(): void {
    this.loadLikes();

  }

  loadLikes(){
    this.memberService.getLikes(this.predicate, this.pageNumber,this.pageSize).subscribe(respose => {
      this.members = respose.result;
      this.pagination = respose.pagination;
    })
  }

  pageChanged(event:any){
    this.pageNumber = event.page;
    this.loadLikes();
  }

}
