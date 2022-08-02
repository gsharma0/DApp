import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static:true}) memberTabs:TabsetComponent;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages:Message[] = [];
  member:member;
  activeTab:TabDirective;
  user:User;

  constructor(private memberSer:MemberService, private route:ActivatedRoute, private messageSer : MessageService
    ,private toaster :ToastrService, public presence:PresenceService, private acctServ:AccountService, private router :Router,
    private activatedRoute:ActivatedRoute) 
    { 
      this.acctServ.currUser$.pipe(take(1)).subscribe(user => this.user = user);
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    }


  ngOnInit(): void 
  {
    //this.loadMember();
    this.route.data.subscribe(data =>{
      this.member =data.member;
    })
    this.route.queryParams.subscribe(params =>{
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })
    this.galleryOptions=[{
      width: '500px',
      height:'500px',
      imagePercent:100,
      thumbnailsColumns:4,
      imageAnimation:NgxGalleryAnimation.Slide,
      preview:false
    }]   
    
    this.galleryImages = this.getImages();
  }

  getImages():NgxGalleryImage[]{
    const imageUrls=[];
    for(const photo of this.member.photos){
      imageUrls.push({
        small:photo?.url,
        medium:photo?.url,
        big:photo?.url
      })
      
    }
    return imageUrls;
  }
//commented to use route resolver :- Used to get data before template is loaded
  /* loadMember(){
    this.memberSer.getMember(this.route.snapshot.paramMap.get('username')).subscribe(
      member => {
        this.member = member;
        this.galleryImages = this.getImages();
    })    
  } */

  loadMessageThread(){
    this.messageSer.getMessageThread(this.member.userName).subscribe(response =>{
      this.messages = response;
    })
  }
  
  selectTab(tabId:number){
    this.memberTabs.tabs[tabId].active =true;
    this.router.navigated =false;
    //history.pushState(null,null, '/' + this.member.userName + '/tab?=' + tabId);
  }

  onTabActivated(data:TabDirective){
    
    this.activeTab=data;
    
    if(this.activeTab.heading === "Messages" && this.messages.length === 0){
      //this.loadMessageThread();Commented to use signalR instead of messageService
      this.messageSer.createHubConnection(this.user,this.member.userName);
    }else{
      
      this.messageSer.stopHubConnection();
      //this.messageSer.stopHubConnection1(this.member.userName);
    }
  }

  ngOnDestroy(): void {
    this.messageSer.stopHubConnection();
  }

  addLike(){
    this.memberSer.addLike(this.member.userName).subscribe(()=>{
      this.toaster.success("You have liked " + this.member.knownAs);
    })
  }

}
