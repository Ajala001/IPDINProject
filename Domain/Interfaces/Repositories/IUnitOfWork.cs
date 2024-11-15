﻿using Microsoft.EntityFrameworkCore.Storage;

namespace App.Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
        Task<int> SaveAsync(CancellationToken stoppingToken);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
