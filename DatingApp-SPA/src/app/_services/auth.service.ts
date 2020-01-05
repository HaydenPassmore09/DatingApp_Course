import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';


@Injectable({//Unlike a component where it is injectable by default we need to add this decorator
  providedIn: 'root'//this tells our service and any components that use this service which module
  //is providing this serive in this case its root (which is the app module)
})
export class AuthService {
  private baseUrl = 'http://localhost:5000/api/auth/';

  constructor(private http: HttpClient) { }

  /*
  * This method sends a post request to the server (the .NET core applications AuthControllers Login() action) and stores the JWT token returned
  */
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);//saves the token recieved in the clients local storage
        }
      })
    );
  }

  register(model:any){
    return this.http.post(this.baseUrl + 'register', model);
  }

}
