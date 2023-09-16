import { Injectable } from '@angular/core';
import { HttpInternalService } from './http-internal.service';
import { Observable } from 'rxjs';
import { User } from 'oidc-client';
import { UserDTO } from '../models/userDTO';
import { HttpResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  public routePrefix: string = 'api/user';

  constructor(private httpService : HttpInternalService) { }

  public getAllUsers() : Observable<HttpResponse<UserDTO[]>>{
    return this.httpService.getFullRequest<UserDTO[]>(`${this.routePrefix}`);
  }

  public getUserByID(id: number) : Observable<HttpResponse<UserDTO>>{
    return this.httpService.getFullRequest<UserDTO>(`${this.routePrefix}/${id}`);
  }

  public getCurrentUser() : Observable<HttpResponse<UserDTO>>{
    return this.httpService.getFullRequest<UserDTO>(`${this.routePrefix}/currentUser}`);
  }

  public followUser(id: number): Observable<HttpResponse<void>>{
    return this.httpService.postFullRequest<void>(`${this.routePrefix}/${id}/follow`, {});
  }

  public unfollowUser(id: number): Observable<HttpResponse<void>>{
    return this.httpService.postFullRequest<void>(`${this.routePrefix}/${id}/unfollow`, {});
  }

  public putUser(id: number, value : UserDTO): Observable<HttpResponse<UserDTO>>{
    return this.httpService.putFullRequest<UserDTO>(`${this.routePrefix}/${id}`, value);
  }

  public deleteUser(id: number,): Observable<HttpResponse<void>>{
    return this.httpService.deleteFullRequest<void>(`${this.routePrefix}/${id}`);
  }

  public copyUser({id, username, email, dateOfBirth, password, imageID, avatar, posts, followersIds, 
    followingIds, bio, location, bookmarkedPostsIds, repostsIds}: UserDTO){
    return {
      id,
      username,
      email,
      dateOfBirth,
      password,
      imageID,
      avatar,
      posts,
      followersIds,
      followingIds, 
      bio,
      location,
      bookmarkedPostsIds,
      repostsIds
    }
  }
}
 
