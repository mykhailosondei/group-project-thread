import {Component, Input, ViewChild} from '@angular/core';
import {PagePostDTO} from "../../models/post/pagePostDTO";
import {UserWithPostDTO} from "../../models/user/UserWithinPostDTO";
import PostFormatter from "../../helpers/postFormatter";
import {CommentService} from "../../Services/comment.service";

@Component({
  selector: 'app-comment-creator',
  templateUrl: './comment-creator.component.html',
  styleUrls: ['./comment-creator.component.scss', '../page-post/page-post.component.scss']
})
export class CommentCreatorComponent {

  @Input() post: PagePostDTO;
    constructor(private commentService : CommentService) { }

  @ViewChild('inputComponent') commentInput: {inputValue: string} = {inputValue: ""};

  isButtonDisabled() {
    return PostFormatter.isInputLengthInvalid(this.commentInput.inputValue);
  }

  getCircleColor(user: UserWithPostDTO) {
    return PostFormatter.getCircleColor(user.username);
  }

  isAvatarNull(user: UserWithPostDTO) {
    return user.avatar === null;
  }

  getFirstInitial(user: UserWithPostDTO) {
    return user.username[0].toUpperCase();
  }


  onReplyClick() {
    this.commentService.postComment({postId:this.post.id, textContent: this.commentInput.inputValue, images:[]}).subscribe(Response => {
      this.commentInput.inputValue = "";
      console.log(Response);
    });
  }
}