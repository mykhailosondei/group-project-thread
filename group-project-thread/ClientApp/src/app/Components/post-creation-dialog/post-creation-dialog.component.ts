import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {CommentCreateDialogData} from "../../models/coment/CommentCreateDialogData";
import {PageUserDTO} from "../../models/user/pageUserDTO";
import PostFormatter from "../../helpers/postFormatter";
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import {UserDTO} from "../../models/user/userDTO";
import {UserService} from "../../Services/user.service";
import {ImageUploadService} from "../../Services/image-upload.service";

@Component({
  selector: 'app-post-creation-dialog',
  templateUrl: './post-creation-dialog.component.html',
  styleUrls: ['./post-creation-dialog.component.scss', '../post-editor-dialog/post-editor-dialog.component.scss', '../page-post/page-post.component.scss', '../comment-creation-dialog/comment-creation-dialog.component.scss']
})
export class PostCreationDialogComponent implements OnInit {
  faTimes = faTimes;
  constructor(public dialogRef: MatDialogRef<PostCreationDialogComponent>,
              private readonly userService : UserService,
              private readonly imageUploadService : ImageUploadService) { }

  currentUser: UserDTO = {} as UserDTO;

  @ViewChild('editInputBox') editInputBox : {inputValue: string} = {inputValue: ""};
  imageUrls: string[] = [];

  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe(response => {
      if(response.ok){
        this.currentUser = response.body!;
      }
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  getCircleColor(user :PageUserDTO | UserDTO) {
    return  PostFormatter.getCircleColor(user.username);
  }

  isAvatarNull(user : PageUserDTO | UserDTO): boolean {
    return user.avatar === null;
  }

  getFirstInitial(user : PageUserDTO | UserDTO): string {
    return user.username[0].toUpperCase();
  }

  isButtonDisabled() {
    if(this.editInputBox.inputValue === '' && this.imageUrls.length === 0) return true;
    return PostFormatter.isInputLengthTooBig(this.editInputBox.inputValue) || this.editInputBox.inputValue === "";
  }

  onPhotoLoaded($event: string) {
    this.imageUrls.push($event);
  }

  deleteImage($event:string) {
    this.imageUrls = this.imageUrls.filter(i => i !== $event);
    var deletionName = $event.split('/').pop()!;
    this.imageUploadService.deleteImage(deletionName).subscribe();
  }
}
