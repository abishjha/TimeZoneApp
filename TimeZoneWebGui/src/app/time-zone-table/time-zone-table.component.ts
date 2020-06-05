import { Component, OnInit, Input } from '@angular/core';
import { TimeZone, User } from '@app/_models';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { TimeZoneService, DataService, AuthenticationService } from '@app/_services';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';
import { environment } from '@environments/environment';

@Component({
  selector: 'time-zone-table',
  templateUrl: './time-zone-table.component.html'
})
export class TimeZoneTableComponent implements OnInit {

  @Input() user: User;

  selectedTimeZone: TimeZone;

  timeZoneForm: FormGroup;
  loading = false;
  submitted = false;
  error = '';
  success = '';

  currentUser: User;
  // so we can use the environment variable from the template
  environment = environment;

  constructor(
    private formBuilder: FormBuilder,
    private timeZoneService: TimeZoneService,
    private modalService: NgbModal,
    private dataService: DataService,
    private authenticationService: AuthenticationService
  ) { }

  ngOnInit() {
    this.currentUser = this.authenticationService.currentUserValue;

    this.timeZoneForm = this.formBuilder.group({
      name: ['', Validators.required],
      city: ['', Validators.required],
      differenceToGMT: ['', Validators.required]
    });
  }

  // convenience getter for easy access to form fields
  get f() { return this.timeZoneForm.controls; }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.timeZoneForm.invalid) {
      return;
    }

    this.loading = true;
    this.timeZoneService.addOrUpdate(this.user.id, this.f.name.value, this.f.city.value, this.f.differenceToGMT.value, ((this.selectedTimeZone) ? this.selectedTimeZone.id : null))
      .pipe(first())
      .subscribe(
        data => {
          this.success = 'Success adding/updating ' + this.f.name.value;
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
    this.timeZoneService.deleteOne(this.selectedTimeZone.id)
      .pipe(first())
      .subscribe(
        data => {
          this.success = 'Deleted ' + this.selectedTimeZone.name;
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

    if (this.selectedTimeZone) {
      this.timeZoneForm.get('name').setValue(this.selectedTimeZone.name);
      this.timeZoneForm.get('city').setValue(this.selectedTimeZone.city);
      this.timeZoneForm.get('differenceToGMT').setValue(this.selectedTimeZone.differenceToGMT);
    }
  }

  resetFormValues() {
    this.success = '';
    this.error = '';
    this.submitted = false;
    this.selectedTimeZone = null;
    this.timeZoneForm.reset();
  }
}
