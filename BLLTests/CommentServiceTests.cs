using ApplicationBLL.Exceptions;
using ApplicationBLL.QueryRepositories;
using ApplicationBLL.Services;
using ApplicationCommon.DTOs.Comment;
using ApplicationCommon.DTOs.Image;
using ApplicationCommon.DTOs.Post;
using ApplicationCommon.DTOs.User;
using ApplicationDAL.Context;
using ApplicationDAL.Entities;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit.Abstractions;

namespace BLLTests;

public class CommentServiceTests
{
    private readonly CommentService _commentService;
    private readonly Mock<ApplicationContext> _applicationContextMock = new();
    private readonly Mock<PostQueryRepository> _postQueryRepositoryMock = new();
    private readonly Mock<UserQueryRepository> _userQueryRepositoryMock = new();
    private readonly Mock<CommentQueryRepository> _commentQueryRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new(MockBehavior.Strict);
    private readonly Mock<IValidator<CommentDTO>> _commentValidatorMock = new();
    private readonly Mock<IValidator<CommentUpdateDTO>> _commentUpdateValidatorMock = new();
    private readonly ITestOutputHelper _outputHelper;
    private readonly Mock<ILogger<CommentService>> _logger = new();
    
    public CommentServiceTests(ITestOutputHelper outputHelper)
    {
        _commentService = new CommentService(
        _applicationContextMock.Object,
        _mapperMock.Object,
        _commentValidatorMock.Object,
        _logger.Object,
        _postQueryRepositoryMock.Object,
        _userQueryRepositoryMock.Object,
        _commentQueryRepositoryMock.Object,
        _commentUpdateValidatorMock.Object
        );
        _outputHelper = outputHelper;

        _mapperMock.Setup(m => m.Map<ImageDTO>(It.IsAny<Image>())).Returns((Image image) =>
        {
            ImageDTO imageDTO = new ImageDTO()
            {
                Id = image.Id,
                Url = image.Url
            };
            return imageDTO;
        });
        
        _mapperMock.Setup(m => m.Map<Image>(It.IsAny<ImageDTO>())).Returns((ImageDTO imageEntity) =>
        {
            Image image = new Image()
            {
                Id = imageEntity.Id,
                Url = imageEntity.Url
            };
            return image;
        });
        
        _mapperMock.Setup(m => m.Map<CommentDTO>(It.IsAny<Comment>())).Returns((Comment entity) =>
            new CommentDTO()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PostId = entity.PostId,
                CommentId = entity.CommentId,
                TextContent = entity.TextContent,
                Images = entity.Images.Select(I => _mapperMock.Object.Map<ImageDTO>(I)).ToList(),
                CommentsIds = entity.CommentsIds,
                LikesIds = entity.LikesIds,
                Bookmarks = entity.Bookmarks,
                ViewedBy = entity.ViewedBy
            }
        );
        _mapperMock.Setup(m => m.Map<Comment>(It.IsAny<CommentDTO>())).Returns((CommentDTO entity) =>
            new Comment()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PostId = entity.PostId,
                CommentId = entity.CommentId,
                TextContent = entity.TextContent,
                Images = entity.Images.Select(i => _mapperMock.Object.Map<Image>(i)).ToList(),
                CommentsIds = entity.CommentsIds,
                LikesIds = entity.LikesIds,
                Bookmarks = entity.Bookmarks,
                ViewedBy = entity.ViewedBy
                
            }
        );
        
        _mapperMock.Setup(m => m.Map<CommentDTO>(It.IsAny<CommentCreateDTO>())).Returns((CommentCreateDTO entity) =>
            new CommentDTO()
            {
                UserId = entity.UserId,
                PostId = entity.PostId,
                CommentId = entity.CommentId,
                TextContent = entity.TextContent,
                Images = entity.Images
            }
        );
        
        _mapperMock.Setup(m => m.Map<PostDTO>(It.IsAny<Post>())).Returns((Post entity) =>
        {
            var postDto = new PostDTO
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                Author = null,
                TextContent = entity.TextContent,
                Images = entity.Images.Select(I => _mapperMock.Object.Map<ImageDTO>(I)).ToList(),
                LikesIds = entity.LikesIds,
                CommentsIds = entity.CommentsIds,
                RepostersIds = entity.RepostersIds,
                Bookmarks = entity.Bookmarks,
                ViewedBy = entity.ViewedBy
            };
            return postDto;
        });
        
        _mapperMock.Setup(m => m.Map<Post>(It.IsAny<PostDTO>())).Returns((PostDTO entity) =>
            new Post
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                Author = null,
                TextContent = entity.TextContent,
                Images = entity.Images.Select(I => _mapperMock.Object.Map<Image>(I)).ToList(),
                LikesIds = entity.LikesIds,
                CommentsIds = entity.CommentsIds,
                RepostersIds = entity.RepostersIds,
                Bookmarks = entity.Bookmarks,
                ViewedBy = entity.ViewedBy
            });

        _mapperMock.Setup(m => m.Map<IEnumerable<PostDTO>>(It.IsAny<IEnumerable<Post>>()))
            .Returns((IEnumerable<Post> posts) => posts.Select(entity => new PostDTO
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UserId = entity.UserId,
                Author = null,
                TextContent = entity.TextContent,
                Images = entity.Images.Select(I => _mapperMock.Object.Map<ImageDTO>(I)).ToList(),
                LikesIds = entity.LikesIds,
                CommentsIds = entity.CommentsIds,
                RepostersIds = entity.RepostersIds,
                Bookmarks = entity.Bookmarks,
                ViewedBy = entity.ViewedBy
            }));
    }
    
    [Fact]
    public async Task GetCommentById_ShouldThrowException_OnNonExistingId()
    {
        //Arrange
        int id = 2;
        var comments = new List<Comment>
        {
            new Comment()
            {
                Id = 1,
                TextContent = "konevych"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        //Act
        //Assert
        var ex = await Assert.ThrowsAsync<CommentNotFoundException>(async () => await _commentQueryRepositoryMock.Object.GetCommentByIdPlain(id));
        _outputHelper.WriteLine("" + ex);
    }
    
    [Fact]
    public async Task GetCommentById_ShouldReturnValidComment_OnExistingId()
    {
        //Arrange
        int id = 1;
        var comments = new List<Comment>
        {
            new Comment()
            {
                Id = 1,
                TextContent = "konevych"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        //Act
        var comment = await _commentQueryRepositoryMock.Object.GetCommentByIdPlain(id);
        //Assert
        Assert.Equal(comment.TextContent, comments[0].TextContent);
    }

    [Fact]
    public async Task GetCommentsOfPostId_ShouldReturnAllComments_OnFullyValidInput()
    {
        
        //Arrange
        var posts = new List<Post>()
        {
            new Post()
            {
                Id = 1,
                TextContent = "none",
                CommentsIds = new List<int>() { 1, 2 }
            },
            new Post()
            {
                Id = 2,
                TextContent = "none2",
                CommentsIds = new List<int>() { 3 }
            }
        };
        var comments = new List<Comment>()
        {
            new Comment()
            {
                Id = 1,
                TextContent = "comm1"
            },
            new Comment()
            {
                Id = 2,
                TextContent = "comm2"
            },
            new Comment()
            {
                Id = 3,
                TextContent = "comm3"
            }
        };
        
        _postQueryRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).ReturnsAsync((int id) => _mapperMock.Object.Map<PostDTO>(posts.FirstOrDefault(p => p.Id == id)));

        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        
        //Act
        var result = (await _commentService.GetCommentsOfPostId(1)).ToList();
        //Assert
        Assert.Equal(result[0].TextContent, comments[0].TextContent);
        Assert.Equal(result[1].TextContent, comments[1].TextContent);
    }
    
    [Fact]
    public async Task GetCommentsOfPostId_ShouldReturnComments_OnParticularyValidIds()
    {
        
        //Arrange
        var posts = new List<Post>()
        {
            new Post()
            {
                Id = 1,
                TextContent = "none",
                CommentsIds = new List<int>() { 1, 4 }
            },
            new Post()
            {
                Id = 2,
                TextContent = "none2",
                CommentsIds = new List<int>() { 3 }
            }
        };
        var comments = new List<Comment>()
        {
            new Comment()
            {
                Id = 1,
                TextContent = "comm1"
            },
            new Comment()
            {
                Id = 2,
                TextContent = "comm2"
            },
            new Comment()
            {
                Id = 3,
                TextContent = "comm3"
            }
        };
        
        _postQueryRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).ReturnsAsync((int id) => _mapperMock.Object.Map<PostDTO>(posts.FirstOrDefault(p => p.Id == id)));

        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        
        //Act
        var result = (await _commentService.GetCommentsOfPostId(1)).ToList();
        //Assert
        Assert.Single(result);
        Assert.Equal(comments[0].TextContent, result[0].TextContent);
    }
    
    [Fact]
    public async Task GetCommentsOfPostId_ShouldReturnEmptyLis_OnInvalidIds()
    {
        
        //Arrange
        var posts = new List<Post>()
        {
            new Post()
            {
                Id = 1,
                TextContent = "none",
                CommentsIds = new List<int>() { 4, 5 }
            },
            new Post()
            {
                Id = 2,
                TextContent = "none2",
                CommentsIds = new List<int>() { 3 }
            }
        };
        var comments = new List<Comment>()
        {
            new Comment()
            {
                Id = 1,
                TextContent = "comm1"
            },
            new Comment()
            {
                Id = 2,
                TextContent = "comm2"
            },
            new Comment()
            {
                Id = 3,
                TextContent = "comm3"
            }
        };
        
        _postQueryRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).ReturnsAsync((int id) => _mapperMock.Object.Map<PostDTO>(posts.FirstOrDefault(p => p.Id == id)));

        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        
        //Act
        var result = (await _commentService.GetCommentsOfPostId(1)).ToList();
        //Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task PostComment_ShouldThrowDataExceptionOnInvalidComment()
    {
        //Arrange
        
        var comment = new CommentCreateDTO()
        {
            CommentId = 1,
            PostId = 1
        };

        _userQueryRepositoryMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);
        //Act
        
        //Assert

        var ex = Assert.ThrowsAsync<InvalidDataException>(async ()=> await _commentService.PostComment(comment));
        _outputHelper.WriteLine("" + ex);
    }
    
    [Fact]
    public async Task PostComment_ShouldThrowDataExceptionOnInvalidComment2()
    {
        //Arrange
        
        var comment = new CommentCreateDTO()
        {
            CommentId = null,
            PostId = null
        };

        _userQueryRepositoryMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);
        //Act
        
        //Assert

        var ex = Assert.ThrowsAsync<InvalidDataException>(async ()=> await _commentService.PostComment(comment));
        _outputHelper.WriteLine("" + ex);
    }
    
    [Fact]
    public async Task PostComment_ShouldAddCommentToDbSet_OnValidInput()
    {
        //Arrange
        
        var comment = new CommentCreateDTO()
        {
            PostId = 1,
            TextContent = "NEW"
        };

        var posts = new List<Post>()
        {
            new()
            {
                Id = 1,
                CommentsIds = new List<int>() { 1, 2 }
            }
        };
        
        var comments = new List<Comment>()
        {
            new Comment()
            {
                Id = 1,
                TextContent = "comm1"
            },
            new Comment()
            {
                Id = 2,
                TextContent = "comm2"
            },
            new Comment()
            {
                Id = 3,
                TextContent = "comm3"
            }
        };

        _userQueryRepositoryMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);
        
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        _commentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentDTO>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult()
            {
                Errors = new List<ValidationFailure>()
            });
        _applicationContextMock.Setup(c => c.Comments.Add(It.IsAny<Comment>())).Callback((Comment entity) =>
        {
            comments.Add(entity);
        });
        
        _postQueryRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).ReturnsAsync((int id) => _mapperMock.Object.Map<PostDTO>(posts.FirstOrDefault(p => p.Id == id)));
        //Act
        await _commentService.PostComment(comment);
        //Assert
        Assert.Equal(true, comments.Any(c => c.TextContent == "NEW"));
    }
    
    [Fact]
    public async Task PostComment_ShouldAddCommentToDbSet_OnValidInput2()
    {
        //Arrange
        
        var comment = new CommentCreateDTO()
        {
            CommentId = 1,
            TextContent = "NEW"
        };
        
        var comments = new List<Comment>()
        {
            new Comment()
            {
                Id = 1,
                TextContent = "comm1",
                CommentsIds = new List<int>() { 1, 2 }
            },
            new Comment()
            {
                Id = 2,
                TextContent = "comm2"
            },
            new Comment()
            {
                Id = 3,
                TextContent = "comm3"
            }
        };

        _userQueryRepositoryMock.Setup(u => u.GetUserById(It.IsAny<int>())).ReturnsAsync((UserDTO)null);
        
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        _commentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentDTO>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult()
            {
                Errors = new List<ValidationFailure>()
            });
        _applicationContextMock.Setup(c => c.Comments.Add(It.IsAny<Comment>())).Callback((Comment entity) =>
        {
            comments.Add(entity);
        });
        
        //Act
        await _commentService.PostComment(comment);
        //Assert
        Assert.Equal(true, comments.Any(c => c.TextContent == "NEW"));
    }
    
    [Fact]
    public async Task PostComment_ShouldThrowException_OnInvalidCommentInput()
    {
        //Arrange
        var comment = new CommentCreateDTO()
        {
            CommentId = 1,
            TextContent = "NEW"
        };
        var comments = new List<Comment>()
        {
            new()
            {
                Id = 1,
                TextContent = "bop"
            },
            new()
            {
                Id = 2,
                TextContent = "bop"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        _commentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentDTO>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult()
            {
                Errors = new List<ValidationFailure>() {new ValidationFailure()}
            });
        //Act
        //Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _commentService.PostComment(comment));
        _outputHelper.WriteLine("" + ex);
    }

    [Fact]
    public async Task PutComment_ShouldThrowException_OnInvalidCommentId()
    {
        //Arrange
        var comment = new CommentUpdateDTO()
        {
            TextContent = "NEW"
        };
        var comments = new List<Comment>()
        {
            new()
            {
                Id = 1,
                TextContent = "bop"
            },
            new()
            {
                Id = 2,
                TextContent = "bop"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        //Act
        //Assert
        var ex = Assert.ThrowsAsync<CommentNotFoundException>(async () => await _commentService.PutComment(3, comment));
        _outputHelper.WriteLine("" + ex);
    }
    
    [Fact]
    public async Task PutComment_ShouldThrowException_OnInvalidCommentInput()
    {
        //Arrange
        var comment = new CommentUpdateDTO()
        {
            TextContent = "",
            Images = new List<ImageDTO>()
        };
        var comments = new List<Comment>()
        {
            new()
            {
                Id = 1,
                TextContent = "bop"
            },
            new()
            {
                Id = 2,
                TextContent = "bop"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        _commentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentDTO>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult()
            {
                Errors = new List<ValidationFailure>() {new ValidationFailure()}
            });
        //Act
        //Assert
        var ex = Assert.ThrowsAsync<ValidationException>(async () => await _commentService.PutComment(3, comment));
        _outputHelper.WriteLine("" + ex);
    }
    
    [Fact]
    public async Task DeleteComment_ShouldDelete_OnValidInput()
    {
        //Arrange
        int idToDelete = 1;
        var comments = new List<Comment>()
        {
            new()
            {
                Id = 1,
                TextContent = "bop"
            },
            new()
            {
                Id = 2,
                TextContent = "bop"
            }
        };
        _applicationContextMock.Setup(c => c.Comments).ReturnsDbSet(comments);
        _applicationContextMock.Setup(c => c.Comments.Remove(It.IsAny<Comment>())).Callback((Comment entity) =>
        {
            comments.RemoveAll(c => c.Id == entity.Id);
        });
        
        _commentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<CommentDTO>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult()
            {
                Errors = new List<ValidationFailure>() {}
            });
        
        //Act
        await _commentService.DeleteComment(idToDelete);
        //Assert
        Assert.Single(comments);
        Assert.DoesNotContain(comments, c => c.Id == idToDelete);
    }
}