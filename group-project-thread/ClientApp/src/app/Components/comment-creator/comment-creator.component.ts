import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {PagePostDTO} from "../../models/post/pagePostDTO";
import {PageUserDTO} from "../../models/user/pageUserDTO";
import PostFormatter from "../../helpers/postFormatter";
import {CommentService} from "../../Services/comment.service";

@Component({
  selector: 'app-comment-creator',
  templateUrl: './comment-creator.component.html',
  styleUrls: ['./comment-creator.component.scss', '../page-post/page-post.component.scss']
})
export class CommentCreatorComponent {

  @Input() post: PagePostDTO;
  @Input() replyArgs: {isCommentReply: boolean} = {isCommentReply: false};
  @Output() onReplyClickEvent = new EventEmitter();
    constructor(private commentService : CommentService) { }

  @ViewChild('inputComponent') commentInput: {inputValue: string} = {inputValue: ""};

  isButtonDisabled() {
    return PostFormatter.isInputLengthInvalid(this.commentInput.inputValue);
  }

  getCircleColor(user: PageUserDTO) {
    return PostFormatter.getCircleColor(user.username);
  }

  isAvatarNull(user: PageUserDTO) {
    return user.avatar === null;
  }

  getFirstInitial(user: PageUserDTO) {
    return user.username[0].toUpperCase();
  }


  onReplyClick() {
    if(!this.replyArgs.isCommentReply) {
      this.commentService.postComment({postId:this.post.id, textContent: this.commentInput.inputValue, images:[]}).subscribe(Response => {
        this.commentInput.inputValue = "";
        console.log(Response);

        if(Response.ok){
          this.onReplyClickEvent.emit();
        }
      });
    }
    else {
      this.commentService.postComment({commentId:this.post.id, textContent: this.commentInput.inputValue, images:[]}).subscribe(Response => {
        this.commentInput.inputValue = "";
        console.log(Response);
        if(Response.ok){
          this.onReplyClickEvent.emit();
        }
      });
    }
  }
}
