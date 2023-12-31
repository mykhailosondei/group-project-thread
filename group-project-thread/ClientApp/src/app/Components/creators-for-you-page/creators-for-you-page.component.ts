import { Component } from '@angular/core';
import {BehaviorSubject, finalize} from "rxjs";
import {PageUserDTO} from "../../models/user/pageUserDTO";
import {UserDTO} from "../../models/user/userDTO";
import {NavigatorService} from "../../Services/navigator.service";
import {RecommendationService} from "../../Services/recommendation.service";
import {UserService} from "../../Services/user.service";
import {Tab} from "../../models/enums/Tab";
import {NavigationHistoryService} from "../../Services/navigation-history.service";
import {Location} from "@angular/common";
import {Endpoint} from "../side-navbar/side-navbar.component";

@Component({
  selector: 'app-creators-for-you-page',
  templateUrl: './creators-for-you-page.component.html',
  styleUrls: ['./creators-for-you-page.component.scss', '../../../assets/ContentFrame.scss']
})
export class CreatorsForYouPageComponent {
  protected readonly Tab = Tab;
  public users$ : BehaviorSubject<PageUserDTO[]> = new BehaviorSubject<PageUserDTO[]>([]);
  public currentUser! : UserDTO;
  loading: boolean;
  constructor(public navigator : NavigatorService,
              private recommendationService : RecommendationService,
              private userService : UserService, private historyOfPages : NavigationHistoryService,
              private location : Location) {
  }

  ngOnInit(){
    this.loading = true;
    this.userService.getCurrentUserInstance().subscribe(
      (user) => this.currentUser = user);
    this.getCreatorsForYou();
  }

  getCreatorsForYou(){
    this.recommendationService.getCreatorsForYou()
      .pipe(finalize(()=>
        this.loading = false))
      .subscribe((response) =>
      {
        if (response.body != null){
          this.users$.next(response.body)
        }
      })
  }

  navigateToWhoToFollow(){
    if (this.historyOfPages.getPageInHistoryCounter() == 0){
      this.historyOfPages.setNavigateToMainPage();
    }
    this.historyOfPages.IncrementPageInHistoryCounter();
    this.navigator.openWhoToFollowPage('');
  }

  goBack(){
    this.navigator.backToMainPage();
  }

  protected readonly Endpoint = Endpoint;
}
