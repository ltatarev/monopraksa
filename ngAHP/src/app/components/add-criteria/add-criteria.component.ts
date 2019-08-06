import { Component, OnInit } from "@angular/core";
import { FormGroup, FormArray, FormBuilder, Validators } from "@angular/forms";

@Component({
  selector: "app-add-criteria",
  templateUrl: "./add-criteria.component.html",
  styleUrls: ["./add-criteria.component.css"]
})

export class AddCriteriaComponent implements OnInit {
  addCriteriaForm: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.addCriteriaForm = this.fb.group({
      criteriaArray: this.fb.array([], Validators.required)
    });

    this.addCriteria();
    this.addCriteria();
  }

  get criteriaForms() {
    return this.addCriteriaForm.get("criteriaArray") as FormArray;
  }

  addCriteria() {
    const criteria = this.fb.group({
      criteriaName: ["", Validators.required]
    });

    this.criteriaForms.push(criteria);
  }

  minLen() {
    return this.criteriaForms.length > 2;
  }

  maxLen() {
    return this.criteriaForms.length >= 10;
  }

  deleteCriteria(i) {
    if (this.criteriaForms.length > 2) {
      this.criteriaForms.removeAt(i);
    }
  }

  onSubmit() {
    let formValue = this.addCriteriaForm.value;
    let criteria = [];
    for (let c of formValue.criteriaArray) {
      criteria.push(c);
    }

    console.log(criteria);
  }
}
