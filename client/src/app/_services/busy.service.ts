import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  bsyRequestCount=0;
  constructor(private spinner:NgxSpinnerService) { }

  busy(){
    this.bsyRequestCount++;
    this.spinner.show(undefined,{
      type:'line-scale-party',
      bdColor:'rgba(255,255,255,0)',
      color:'#41d7a7'
    });
  }

  hide(){
    this.bsyRequestCount--;
    if(this.bsyRequestCount <= 0){
      this.bsyRequestCount=0;
      this.spinner.hide();
    }
    
  }

}
