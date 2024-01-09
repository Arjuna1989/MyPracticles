using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Dto;
using API.Entity;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly ITokenService _service;
        public AccountController(IUserRepository repository, ITokenService service)
        {
            _service = service;
            _repository = repository;

        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto user)
        {

            var userToRegister = await _repository.GetUserByNameAsync(user.UserName);
            if (userToRegister != null) return BadRequest("User already exist !");

            using var hmac = new HMACSHA512();
            userToRegister = new User
            {
                UserName = user.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = hmac.Key
            };

            await _repository.CreateUser(userToRegister);
            await _repository.SaveAll();

            return new UserDto
            {
                UserName = user.UserName,
                Token = _service.CreateToken(userToRegister)
            };




        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto user)
        {

            var userToLogin = await _repository.GetUserByNameAsync(user.UserName);
            if (userToLogin == null) return BadRequest("User name invalid !");

            byte[] computedHash;
            using var hmac = new HMACSHA512(userToLogin.PasswordSalt);
            computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != userToLogin.PasswordHash[i]) return BadRequest("Password Incorrect");


            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = _service.CreateToken(userToLogin)
            };




        }

    }
}