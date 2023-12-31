import {Component, OnInit} from '@angular/core';
import {UserService} from "../../Services/user.service";
import {ActivatedRoute} from "@angular/router";
import {UserDTO} from "../../models/user/userDTO";
import {PageUserDTO} from "../../models/user/pageUserDTO";
import {Subject} from "rxjs";
import {NavigatorService} from "../../Services/navigator.service";
import {Tab} from "../../models/enums/Tab";
import {Location} from "@angular/common";
import {NavigationHistoryService} from "../../Services/navigation-history.service";
import {Endpoint} from "../side-navbar/side-navbar.component";

@Component({
  selector: 'app-following-page',
  templateUrl: './following-page.component.html',
  styleUrls: ['./following-page.component.scss', '../../../assets/ContentFrame.scss']
})
export class FollowingPageComponent implements OnInit{
  protected username : string;
  protected user! : UserDTO;
  protected following : PageUserDTO[];
  private unsubscribe$ = new Subject<void>();
  protected currentUser! : UserDTO;
  public loading : boolean;
  noFollowingFound: boolean;
  protected Endpoint : Endpoint;
  constructor(private userService: UserService, private route: ActivatedRoute, public navigatorService : NavigatorService,
              private location : Location, private historyOfPages : NavigationHistoryService) {
    this.following = [];
  }

  ngOnInit(): void {
    this.loading = true;
    this.route.paramMap.subscribe(params => {
      this.username = params.get('username') || "DefaultUsername";
      this.userService.getCurrentUserInstance().subscribe(
        (user) => {
          this.currentUser = user;
          if (this.currentUser.username == this.username){
            this.Endpoint = Endpoint.Profile;
          }
          else {
            this.Endpoint = Endpoint.None;
          }
        });
      this.fetchFollowingData(this.username);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
  amIFollowing(id : number): boolean{
    return this.currentUser.followingIds.includes(id);
  }

  fetchFollowingData(username: string) {
    this.userService.getUserByUsername(this.username)
      .subscribe(response => {
        if (response.body != null) {
          this.user = response.body;
          this.following = [];
          if (this.user.followingIds.length == 0){
            this.noFollowingFound = true;
          }
          for (let i = 0; i < this.user.followingIds.length; i++) {
            const userId = this.user.followingIds[i];
            this.userService.getUserById(userId)
              .subscribe(response => {
                if (response.body != null) {
                  const userResponse: UserDTO = response.body;
                  const followingUser: PageUserDTO = {
                    avatar: userResponse.avatar,
                    bio: userResponse.bio,
                    followers: userResponse.followersIds.length,
                    following: userResponse.followingIds.length,
                    id: userResponse.id,
                    username: userResponse.username,
                  };
                  this.following.push(followingUser);
                }
              });
          }
          this.loading = false;
        }
      });
  }
  navigateToFollowers(username : string){
    if (this.historyOfPages.getPageInHistoryCounter() == 0){
      this.historyOfPages.setNavigateToUserPage();
    }
    this.historyOfPages.IncrementPageInHistoryCounter();
    this.navigatorService.openFollowersPage(username);
  }

  goBack(){
    const pagesCount = this.historyOfPages.getPageInHistoryCounter();
    if (pagesCount == 0 || this.historyOfPages.getNavigateToUserPage()){
      this.historyOfPages.resetCounter();
      this.historyOfPages.resetNavigateToUserPage();
      this.navigatorService.openProfilePage(this.username);
      return;
    }
    this.historyOfPages.resetCounter();
    this.location.historyGo(-pagesCount);
  }
}
