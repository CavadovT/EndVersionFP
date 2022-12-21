using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.SeedWork
{
    public interface IUnitOfWork:IDisposable
    {
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntityAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
