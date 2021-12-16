import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() userFromHomeCom: any;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate:Date;
  validationErrors:string[] = [];


  model: any = {};
  constructor(private accountService: AccountService, private fb : FormBuilder,private router :Router) { }

  ngOnInit(): void {
    this.intializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  intializeForm(){
    /* commented for another good way to do code angular reactive forms */
   /*  this.registerForm = new FormGroup({
      username : new FormControl('', Validators.required),
      password : new FormControl('',[Validators.required,Validators.minLength(4), Validators.maxLength (8)]),
      confirmPassword : new FormControl('', [Validators.required, this.matchValues("password")]) 
    });*/
    this.registerForm = this.fb.group({
      gender : ['male'],
      username : ['', Validators.required],
      knownAs : ['', Validators.required],
      dateofBirth : ['', Validators.required],
      city : ['', Validators.required],
      country : ['', Validators.required],
      password : ['',[Validators.required,Validators.minLength(4), Validators.maxLength (8)]],
      confirmPassword : ['', [Validators.required, this.matchValues("password")]] 
    })

    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }


  matchValues(matchTo:string) : ValidatorFn{
    return (control :AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching :true }
    }
  }

  register() {

    this.accountService.register(this.registerForm.value).subscribe(response => {

      this.router.navigateByUrl("/members");
      //console.log(response);
      //this.cancel();
    }, errors =>{
      this.validationErrors = errors;
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  toControl(absCtrl: AbstractControl): FormControl {
    const ctrl = absCtrl as FormControl;
    // if(!ctrl) throw;
    return ctrl;
}


}
