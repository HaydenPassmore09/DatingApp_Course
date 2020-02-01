import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({ // Unlike a component where it is injectable by default we need to add this decorator
  providedIn: 'root'// this tells our service and any components that use this service which module
  // is providing this serive in this case its root (which is the app module)
})
export class AuthService {
  private baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  changeMemberPhoto(newPhotoUrl: string) {
    this.photoUrl.next(newPhotoUrl);
  }

  /*
  * This method sends a post request to the server (the .NET core applications AuthControllers Login() action)
  * and stores the JWT token returned
  */
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token); // saves the token recieved in the clients local storage
          localStorage.setItem('user', JSON.stringify(user.user)); // saves the user object as a string in local storage
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

}
