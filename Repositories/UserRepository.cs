using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dto;
using API.Entity;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;

        }
        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }

        public async Task<UserDto> GetUserDtoByIdAsync(int Id)
        {
            return await _context.Users.Where(x => x.Id == Id)
                      .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                      .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByNameAsync(string UserName)
        {
            return await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == UserName);
        }

        public async Task<User> GetUserByIdAsync(int Id)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<UserDto> GetUserDtoByNameAsync(string UserName)
        {
            return await _context.Users.Where(x => x.UserName == UserName)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
        }

        public async Task<PagedList<UserDto>> GetUsers(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Now.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query =userParams.OrderBy switch
            {
                "created"=> query.OrderByDescending(u =>u.Created),
                _ => query.OrderByDescending(u =>u.LastActive)
            };

            return await PagedList<UserDto>.CreatePageListAsync(
                query.AsNoTracking().ProjectTo<UserDto>(_mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize);
        }

        public async Task<bool> IsUserExist(string UserName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == UserName);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        Task<User> IUserRepository.GetUserByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
        }
    }
}