﻿using ApplicationCommon.DTOs.Comment;
using FluentValidation;

namespace group_project_thread.Validators;

public class CommentDTOValidator : AbstractValidator<CommentDTO>
{
    public CommentDTOValidator()
    {
        RuleFor(c => c.TextContent ).NotEmpty().
            When(p => p.Images.Count == 0).WithMessage("You can not create comment with no content")
            .MaximumLength(200)
            .WithMessage("More than characters max amount");
        
        RuleFor(p => p.Images).NotEmpty()
            .When(p => p.TextContent == String.Empty).WithMessage("You can not create comment with no content");

        RuleFor(p => p.Images.Count).InclusiveBetween(0, 4)
            .WithMessage("You can not add more than 4 pictures to your comment");

        RuleFor(p => new { p.CommentId, p.PostId }).Must(x => x.CommentId.HasValue ^ x.PostId.HasValue)
            .WithMessage("You cannot comment post and comment at the same time");
    }
}