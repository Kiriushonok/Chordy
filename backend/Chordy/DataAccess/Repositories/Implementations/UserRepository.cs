﻿using Chordy.DataAccess.Entities;
using Chordy.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chordy.DataAccess.Repositories.Implementations
{
    public class UserRepository(ChordyDbContext context) : IUserRepository
    {
        public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            await context.users.AddAsync(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            context.users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.users.ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            return await context.users.FirstOrDefaultAsync(user => user.Login == login, cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            context.users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
