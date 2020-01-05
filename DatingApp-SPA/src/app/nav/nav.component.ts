import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {

  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      console.log("log in success");
    }, error => {
      console.log("error logging in");
    });
  }

  loggedIn(){
    const token = localStorage.getItem('token');
    return !!token;//typescript way of returning true if the token string is not empty
  }

  logout(){
    localStorage.removeItem('token');
    console.log('loggedout');
  }
}