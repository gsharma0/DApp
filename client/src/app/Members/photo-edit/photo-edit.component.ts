import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/Photo';
import { User } from 'src/app/_models/User';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css']
})
export class PhotoEditComponent implements OnInit {
@Input() member :member;
  uploader:FileUploader;
  hasBaseDropZoneOver:boolean;
  baseUrl = environment.apiUrl;
  user:User;
  
  constructor(private acctSer :AccountService, private memberService :MemberService) {
    this.acctSer.currUser$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.initilaizeUploader();
  }

  fileOverBase(e:any){
    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo:Photo){
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.acctSer.setCurrUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain =false;
        if(p.id === photo.id ) p.isMain = true;
      });
    })
    }

    deletePhoto(photoId: number){
      this.memberService.deletePhoto(photoId).subscribe(() => {
        this.member.photos = this.member.photos.filter(x =>x.id !== photoId);
      })
    }

  

  initilaizeUploader (){
    this.uploader = new FileUploader ({
      url : this.baseUrl + 'users/add-photo',
      authToken : 'Bearer ' + this.user.token,
      isHTML5 :true,
      allowedFileType:['image'],
      removeAfterUpload:true,
      autoUpload:false,
      maxFileSize : 10 * 1024 * 1024
    })
    this.uploader.onBeforeUploadItem = (file)=>{
      file.withCredentials =false;
    }

    this.uploader.onSuccessItem =(item, response,status,headers) =>{
      const photo =JSON.parse(response);
      this.member.photos.push(photo);
    }
  }



}
