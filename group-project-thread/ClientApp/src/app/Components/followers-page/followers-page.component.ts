import {Component, NgZone} from '@angular/core';
import {UserService} from "../../Services/user.service";
import {ActivatedRoute, Router} from "@angular/router";
import {UserDTO} from "../../models/user/userDTO";
import {PageUserDTO} from "../../models/user/pageUserDTO";
import {finalize, Subject, takeUntil} from "rxjs";
import {NavigatorService} from "../../Services/navigator.service";
import {Tab} from "../../models/enums/Tab";
import {Location} from "@angular/common";
import {NavigationHistoryService} from "../../Services/navigation-history.service";
import {Endpoint} from "../side-navbar/side-navbar.component";
@Component({
  selector: 'app-followers-page',
  templateUrl: './followers-page.component.html',
  styleUrls: ['./followers-page.component.scss', '../../../assets/ContentFrame.scss']
})
export class FollowersPageComponent {
  protected username : string;
  protected user! : UserDTO;
  protected followers : PageUserDTO[];
  private unsubscribe$ = new Subject<void>();
  protected currentUser! : UserDTO;
  public loading : boolean;
  public noFollowersFound : boolean;
  protected  Endpoint : Endpoint;

  constructor(private userService: UserService, private route: ActivatedRoute, public navigatorService : NavigatorService,
  private historyOfPages : NavigationHistoryService, private location : Location) {
    this.followers = [];
  }

  handleUser(user: UserDTO) {
    this.user = user;
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
      this.fetchFollowersData(this.username);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  fetchFollowersData(username: string) {
    this.userService.getUserByUsername(username).subscribe(response => {
      if (response.body != null) {
        this.user = response.body;
        this.followers = [];
        if (this.user.followersIds.length == 0){
          this.noFollowersFound = true;
        }
        for (let i = 0; i < this.user.followersIds.length; i++) {
          const userId = this.user.followersIds[i];
          this.userService.getUserById(userId).subscribe(response => {
            if (response.body != null) {
              const userResponse: UserDTO = response.body;
              const follower: PageUserDTO = {
                avatar: userResponse.avatar,
                bio: userResponse.bio,
                followers: userResponse.followersIds.length,
                following: userResponse.followingIds.length,
                id: userResponse.id,
                username: userResponse.username,
              };
              this.followers.push(follower);
            }
          });
        }
        this.loading = false;
      }
    });
  }

  amIFollowing(id : number): boolean{
    return this.currentUser.followingIds.includes(id);
  }

  navigateToFollowing(username : string){
    if (this.historyOfPages.getPageInHistoryCounter() == 0){
      this.historyOfPages.setNavigateToUserPage();
    }
    this.historyOfPages.IncrementPageInHistoryCounter();
    this.navigatorService.openFollowingPage(username);
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
