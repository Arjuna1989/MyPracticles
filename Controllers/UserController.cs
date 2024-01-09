using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dto;
using API.Entity;
using API.Extensions;
using API.Filters;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UserController(IUserRepository repo, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _repo = repo;

        }


        [HttpGet]
        public async Task<ActionResult<PagedList<UserDto>>> GetUsers([FromQuery]UserParams userParams)
        {
           var currentUser = await _repo.GetUserDtoByNameAsync(User.GetUserName());
           userParams.CurrentUserName = currentUser.UserName;

           if(String.IsNullOrEmpty(userParams.Gender)){

            userParams.Gender = userParams.Gender=="male"?"female":"male";
           }
            var users = await _repo.GetUsers(userParams);
             Response.AddHttpExtension(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
            return Ok(users);

        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<UserDto>> GetUser(string userName)
        {
            return Ok(await _repo.GetUserByNameAsync(userName));

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            return Ok(await _repo.GetUserByIdAsync(id));

        }

        [HttpPut]
        public async Task<ActionResult<UserDto>> UpdateUser(UserUpdateDto updateDto)
        {

            if (!await _repo.IsUserExist(User.GetUserName())) return BadRequest("User can not findable");
            var user = await _repo.GetUserDtoByNameAsync(User.GetUserName());
            _mapper.Map(updateDto, user);

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("An error occured during the Updation");

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {

            if (await _repo.GetUserByIdAsync(id) == null) return BadRequest("User can not findable");
            var user = await _repo.GetUserByIdAsync(id);

            _repo.DeleteUser(user);
            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("An error occured during the Deletion");

        }


        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> UploadPhoto(IFormFile file)
        {
            var user = await _repo.GetUserByNameAsync(User.GetUserName());
            if (user == null) return BadRequest("User Not Found");

            var imgUploadResult = await _photoService.AddPhotoAsync(file);

            if (imgUploadResult == null) return BadRequest("Error occured during the image uplaod");

            var photo = new Photo
            {
                UserId = user.Id,
                PublicId = imgUploadResult.PublicId,
                Url = imgUploadResult.SecureUrl.AbsoluteUri,
                IsMain = !user.Photos.Any()
            };

            user.Photos.Add(photo);

            if (await _repo.SaveAll()) return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, _mapper.Map<PhotoDto>(photo));

            return BadRequest("Photo upload failed! Please retry");
        }


        [HttpPost("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _repo.GetUserByNameAsync(User.GetUserName());
            if (user == null) return BadRequest("Photo related user not found!");

            var toBeMainPhoto = user.Photos.SingleOrDefault(x => x.Id == photoId);
            if (toBeMainPhoto == null) return NotFound("The relevent Photo is not found in the collection");

            var currentMainPhoto = user.Photos.SingleOrDefault(x => x.IsMain == true);
            if (currentMainPhoto == null) currentMainPhoto.IsMain = false;
            toBeMainPhoto.IsMain = true;

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("Problem setting the main photo !");
        }


        [HttpPost("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _repo.GetUserByNameAsync(User.GetUserName());
            if (user == null) return BadRequest("Photo related user not found!");

            var photoToDelete = user.Photos.SingleOrDefault(x => x.Id == photoId);
            if (photoToDelete == null) return NotFound("The relevent Photo is not found in the collection");

            if (photoToDelete.IsMain == true) return BadRequest("Main photo can not be deleted !");

            var PhotoDeletetionResponse = await _photoService.DeletePhotoAsync(photoToDelete.PublicId);
            if (PhotoDeletetionResponse.Error != null) return BadRequest("Error occured while photo deleting from the cloud!");

            user.Photos.Remove(photoToDelete);
            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("Problem deleting the photo !");
        }

    }
}