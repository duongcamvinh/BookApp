using Api.DTO;
using Api.Entities;
using Api.Interfaces;
using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public  async Task DeleteUserAsync(int id)
        {
           var user= _context.Users.FirstOrDefault(x => x.Id == id);
            _context.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUsers> GetUserByIdAsnyc(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<MemberDto> GetUserByNameAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                .Select(user => new MemberDto
                {
                    //Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                }).SingleOrDefaultAsync();
                
        }

        public async Task<AppUsers> GetUserByUserNameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUsers>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateUser(AppUsers user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
