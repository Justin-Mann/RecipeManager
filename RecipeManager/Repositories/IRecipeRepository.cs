using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeManager.Models;

namespace RecipeManager.Repositories {
    public interface IRecipeRepository : IBaseRepository<Recipe> {
        public Task<IEnumerable<Recipe>> ReadPublicAsync();
        public Task<IEnumerable<Recipe>> ReadPrivateAsync(Guid userId);
    }
}
