using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public interface IRecipeStepRepository : IBaseRepository<Step> {
        public Task<IEnumerable<Step>> ReadRecipeStepsAsync(Guid recipeId);
        public Task DeleteRecipeStepsAsync(Guid recipeId);
        public Task<int> CurrentCountAsync(Guid recipeId);
    }
}
