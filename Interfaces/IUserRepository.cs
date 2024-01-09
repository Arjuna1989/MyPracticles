using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using API.Dto;
using API.Entity;
using API.Helper;
using Microsoft.AspNetCore.Identity;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task CreateUser(User user);

        Task<UserDto> GetUserDtoByNameAsync(string UserName);

        Task<User> GetUserByNameAsync(string UserName);

        Task<UserDto> GetUserDtoByIdAsync(int Id);

         Task<User> GetUserByIdAsync(int Id);

        Task<PagedList<UserDto>> GetUsers(UserParams userParams);

        Task<bool> IsUserExist(string  UserName);

        void DeleteUser(User user);

        void UpdateUser(User user);

        Task<bool> SaveAll();


    }
}