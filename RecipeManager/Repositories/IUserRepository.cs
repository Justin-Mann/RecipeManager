using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public interface IUserRepository : IBaseRepository<User> {
        public bool Login(string userName, string password);
        public void Logout();
    }
}
