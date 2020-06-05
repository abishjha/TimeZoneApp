import { Component, OnInit, Input } from '@angular/core';
import { User } from '@app/_models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../environments/environment'
import { AuthenticationService, UserService, DataService } from '@app/_services';
import { MustMatch } from '@app/_helpers';
import { first } from 'rxjs/operators';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';



@Component({
  selector: 'user-table',
  templateUrl: './user-table.component.html'
})
export class UserTableComponent implements OnInit {
  userForm: FormGroup;
  loading = false;
  submitted = false;
  success = '';
  error = '';

  selectedUser: User;

  currentUser: User;
  // so we can use the environment variable from the template
  environment = environment;

  @Input() users: User[];

  constructor(
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private modalService: NgbModal,
    private dataService: DataService
  ) { }

  ngOnInit() {
    this.currentUser = this.authenticationService.currentUserValue;

    this.userForm = this.formBuilder.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      username: ['', [Validators.required, Validators.minLength(6)]],
      role: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    }, {
      validator: MustMatch('password', 'confirmPassword')
    });
  }


  // convenience getter for easy access to form fields
  get f() { return this.userForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.userForm.invalid) {
      return;
    }

    this.loading = true;
    this.userService.addOrUpdate(this.f.username.value, this.f.password.value, this.f.firstname.value, this.f.lastname.value, this.f.role.value, ((this.selectedUser) ? this.selectedUser.id : null))
      .pipe(first())
      .subscribe(
        data => {
          this.success = 'Success adding/updating ' + this.f.username.value;
          this.loading = false;
        },
        error => {
          this.error = error;
          this.loading = false;
        });
  }

  onDelete() {
    this.submitted = true;

    this.loading = true;
    this.userService.deleteOne(this.selectedUser.id)
      .pipe(first())
      .subscribe(
        data => {
          this.success = 'Deleted ' + this.selectedUser.username;
          this.loading = false;
        },
        error => {
          this.error = error;
          this.loading = false;
        });;
  }

  open(content) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' }).result.then((result) => {
      this.resetFormValues();
      this.dataService.getData();
    }, (reason) => {
      this.resetFormValues();
      this.dataService.getData();
    });;

    if (this.selectedUser) {
      this.userForm.get('firstname').setValue(this.selectedUser.firstName);
      this.userForm.get('lastname').setValue(this.selectedUser.lastName);
      this.userForm.get('username').setValue(this.selectedUser.username);
      this.userForm.get('role').setValue(this.selectedUser.role);
    }
  }

  resetFormValues() {
    this.success = '';
    this.error = '';
    this.submitted = false;
    this.selectedUser = null;
    this.userForm.reset();
  }
}
