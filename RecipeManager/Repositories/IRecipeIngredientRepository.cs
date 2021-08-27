using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public interface IRecipeIngredientRepository : IBaseRepository<Ingredient> {
        public Task<IEnumerable<Ingredient>> ReadRecipeIngredientsAsync(Guid recipeId);
        public Task DeleteRecipeIngredientsAsync(Guid recipeId);
        public Task<int> CurrentCountAsync(Guid recipeId);
    }
}
