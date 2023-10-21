import { Injectable } from '@angular/core';
import {Router} from "@angular/router";
import {Tab} from "../models/enums/Tab";

@Injectable({
  providedIn: 'root'
})
export class NavigatorService {
  constructor(private router : Router) { }

  public openProfilePage($event : string){
    this.router.navigate([$event, 'profile'])
  }
  public openFollowingPage($event : string){
    this.router.navigate([$event, 'following'])
  }
  public openFollowersPage($event : string){
    this.router.navigate([$event, 'followers'])
  }

  public openWhoToFollowPage($event : string){
    this.router.navigate(['connect-people'])
  }
  public openCreatorsForYouPage($event : string) {
    this.router.navigate(['creators-for-you'])
  }

  navigateToMayBeInterestingPage(currentTab : Tab) {
    currentTab = 0;
    this.router.navigate(['explore']);
  }

  navigateToTrendingPage(currentTab : Tab) {
    currentTab = 1;
    this.router.navigate(['trending']);
  }

  searchByWord(word: string) {
    this.router.navigate(['search'], {queryParams : {q : word}})
  }
}
