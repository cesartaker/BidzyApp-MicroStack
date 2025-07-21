using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Repositories
{
    public interface IMongoRoleRepository
    {
        public Task<string?> IdRoleByName(string roleName);
    }
}
