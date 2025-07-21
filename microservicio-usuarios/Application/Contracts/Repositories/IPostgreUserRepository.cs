using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IPostgreUserRepository
{
    void AddUser(User user);
    void UpdateUser(User user);

}
