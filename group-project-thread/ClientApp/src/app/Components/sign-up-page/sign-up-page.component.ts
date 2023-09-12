import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-sign-up-page',
  templateUrl: './sign-up-page.component.html',
  styleUrls: ['./sign-up-page.component.scss', '../../../assets/LoginAndRegisCommon.scss']
})
export class SignUpPageComponent {
  passType: string = "password";
  repeatPassType: string = "password";
  isText: boolean = false;
  isTextRepeatPass: boolean = false;
  eyeIcon: string = "fa-eye-slash";
  repeatEyeIcon: string = "fa-eye-slash";
  firstpassword: string = "";
  repeatedPassword: string = "";
  isEqualMessage: string = "";
  isEnoughChrMessage: string = "";
  messageColor: string = "red";
  passwordsMatch: boolean = false;
  selectedDay: number | null = null;
  days: number[] = [];
  selectedYear: number | null = null;
  years: number[] = [];
  selectedMonth: number | null = null;
  months: { value: number; name: string }[] = [
    { value: 1, name: 'January' },
    { value: 2, name: 'February' },
    { value: 3, name: 'March' },
    { value: 4, name: 'April' },
    { value: 5, name: 'May' },
    { value: 6, name: 'June' },
    { value: 7, name: 'July' },
    { value: 8, name: 'August' },
    { value: 9, name: 'September' },
    { value: 10, name: 'October' },
    { value: 11, name: 'November' },
    { value: 12, name: 'December' }
  ];
  regisForm!: FormGroup;

  constructor(private fb: FormBuilder) {
    // Generate an array of days (1 to 31) and populate the 'days' property
    for (let i = 1; i <= 31; i++) {
      this.days.push(i);
    }
    for (let i = 2023; i>=1904;i--){
      this.years.push(i);
    }
  }
  ngOnInit() : void{
    this.regisForm=this.fb.group({
      username: ['', Validators.required],
      email: ['', Validators.required],
      password: ['', [Validators.required, this.passwordValidator()]],
      repeatPassword: ['', [Validators.required, this.repeatpasswordValidator()]],
      selectedMonth: ['', Validators.required],
      selectedDay: ['', Validators.required],
      selectedYear: ['', Validators.required]
    })
  }
  
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.passType = "text" : this.passType = "password";
  }

  hideShowRepeatPass(){
    this.isTextRepeatPass = !this.isTextRepeatPass;
    this.isTextRepeatPass ? this.repeatEyeIcon = "fa-eye" : this.repeatEyeIcon = "fa-eye-slash";
    this.isTextRepeatPass ? this.repeatPassType = "text" : this.repeatPassType = "password";
  }


  private validateAllFields(formGroup: FormGroup){
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl){
        control.markAsDirty({onlySelf:true});
      }else if(control instanceof FormGroup){
        this.validateAllFields(control);
      }
    })
  }

  passwordValidator(): Validators {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const password = control.value;

      
      if (password.length < 8) {
        return { 'passwordLength': true };
      }

      return null; 
    };
  }

  repeatpasswordValidator(): Validators {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const repeatpassword = control.value;
      
      if (repeatpassword !== this.firstpassword) {
        return { 'passwordMatch': true };
      }
      return null;
    };
  }

  updateDates(){
    let daysInMonth: number = this.daysBasedOnDropDowns();
    
    this.days = [];
    for (let i = 1; i <= daysInMonth; i++) {
    this.days.push(i);
    }
  }
  
  daysBasedOnDropDowns(): number{
    if (this.selectedYear != null && this.selectedMonth != null) {
      return new Date(this.selectedYear, this.selectedMonth, 0).getDate();
    } 
    else if(this.selectedMonth != null){ 
      return this.getDaysInMonth();
    }
    else {
      return 31;
    }
  }

  getDaysInMonth(): number{
    
    let thirtyDays: number[] = [4, 6, 9, 11];
    let monthToNum: number = Number(this.selectedMonth!);
    console.log(typeof(monthToNum));
    if(thirtyDays.includes(monthToNum)){
      return 30;
    }
    else if (monthToNum === 2){
      return 29;
    }
    else{
      return 31;
    }
    
  }
}
