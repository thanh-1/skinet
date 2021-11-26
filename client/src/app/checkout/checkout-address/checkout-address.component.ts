import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/account/account.service';
import { IAddress } from 'src/app/shared/models/address';

@Component({
  selector: 'app-checkout-address',
  templateUrl: './checkout-address.component.html',
  styleUrls: ['./checkout-address.component.scss']
})
export class CheckoutAddressComponent implements OnInit {

  @Input() addressForm: FormGroup;

  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  saveUserAddress()
  {
    this.accountService.updateAddress(this.addressForm.get('addressForm').value).subscribe((address: IAddress) => {
      this.toastr.success('Address Saved!');

      // Reset value of Address Form in Checkout Form
      this.addressForm.get('addressForm').reset(address);
    }, error => {
      console.log(error);
      this.toastr.error(error.message);
    });
  }

}
