using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Application.Contracts;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task <HttpStatusCode> CommitAsync();
    Task RollbackAsync();

   
}
