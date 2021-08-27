using RecipeManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Services {
    public interface IRecipeService {
        public Task<Guid> AddAsync(RecipeEntity recipeEntity);
        public Task<IEnumerable<RecipeEntity>> GetAllPublicRecipesAsync();
        public Task<IEnumerable<RecipeEntity>> GetAllUserRecipesAsync(Guid userId);
        public Task<RecipeEntity> GetRecipeDetailAsync(Guid recipeId);
        public Task UpdateAsync(RecipeEntity recipeEntity);
        public Task AddRecipeStepAsync(Step recipeStep);
        public Task RemoveRecipeStepAsync(Guid recipeStepId);
        public Task AddRecipeIngredientAsync(Ingredient recipeIngredient);
        public Task RemoveRecipeIngredientAsync(Guid recipeIngredientId);
        public Task RemoveAsync(Guid id);
        public Task MakeRecipePublicAsync(Guid recipeId);
        public Task MakeRecipePrivateAsync(Guid recipeId);
    }
}