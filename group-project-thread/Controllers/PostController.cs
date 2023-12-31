﻿using ApplicationBLL.QueryRepositories;
using ApplicationBLL.Services;
using ApplicationCommon.DTOs.Post;
using ApplicationCommon.Interfaces;
using ApplicationDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace group_project_thread.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostQueryRepository _postQueryRepository;
        private readonly PostService _postService;
        private readonly LikeService _likeService;
        private readonly IUserIdGetter _userIdGetter;

        public PostController(PostService postService, LikeService likeService, IUserIdGetter userIdGetter, PostQueryRepository postQueryRepository)
        {
            _postService = postService;
            _likeService = likeService;
            _userIdGetter = userIdGetter;
            _postQueryRepository = postQueryRepository;
        }

        // GET: api/<PostController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<PostDTO>> GetAllPosts()
        {
            return await _postQueryRepository.GetAllPosts();
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<PostDTO> GetPostById(int id)
        {
            return await _postQueryRepository.GetPostById(id, p => p.Author, p => p.Images);
        }
        
        [HttpGet("{userId}/posts")]
        [AllowAnonymous]
        public async Task<IEnumerable<PostDTO>> GetPostsByUserId(int userId)
        {
            return await _postService.GetPostsByUserId(userId);
        }
        // POST api/<PostController>
        [HttpPost]
        public async Task<PostDTO> CreatePost([FromBody] PostCreateDTO post)
        {
            int authorId = _userIdGetter.CurrentId;
            post.AuthorId = authorId;
            return await _postService.CreatePost(post);
        }
        
        [HttpPost("{id}/likePost")]
        public async Task LikePost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _likeService.LikePost(id, authorId);
        }
        
        [HttpPost("{id}/bookmarkPost")]
        public async Task BookmarkPost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _postService.BookmarkPost(id, authorId);
        }
        
        [HttpPost("{id}/viewPost")]
        public async Task ViewPost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _postService.ViewPost(id, authorId);
        } 
        
        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public async Task PutPost(int id, [FromBody] PostUpdateDTO post)
        {
            await _postService.PutPost(id, post);
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task DeletePost(int id)
        {
            await _postService.DeletePost(id);
        }

        [HttpPost("{id}/unlikePost")]
        public async Task UnlikePost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _likeService.DislikePost(id, authorId);
        }

        [HttpPost("{id}/removeFromBookmarks")]
        public async Task RemoveFromBookmarksPost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _postService.RemoveFromBookmarksPost(id, authorId);
        }
        
        [HttpPost("{id}/repost")]
        public async Task Repost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _postService.Repost(id, authorId);
        }
        
        [HttpPost("{id}/undoRepost")]
        public async Task UndoRepost(int id)
        {
            int authorId = _userIdGetter.CurrentId;
            await _postService.UndoRepost(id, authorId);
        }
    }
}
