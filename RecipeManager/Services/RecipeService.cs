using RecipeManager.Models;
using RecipeManager.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Services {
    public class RecipeService : IRecipeService {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRecipeIngredientRepository _recipeIngredientRepo;
        private readonly IRecipeStepRepository _recipeStepRepo;

        public RecipeService( IRecipeRepository recipeRepo, 
                              IRecipeIngredientRepository recipeIngredientRepo, 
                              IRecipeStepRepository recipeStepRepo ) {
            _recipeRepo = recipeRepo;
            _recipeIngredientRepo = recipeIngredientRepo;
            _recipeStepRepo = recipeStepRepo;
        }

        public async Task<Guid> AddAsync(RecipeEntity recipeEntity) {// everything is added using same user account atm...
            var newRecordId = await _recipeRepo.CreateAsync(new Recipe { OwnerId = Guid.Parse("51000000-0000-0000-0000-000000000000"), Name = recipeEntity.Name, IsPublic = recipeEntity.IsPublic, Description = recipeEntity.Description });
            return newRecordId;
        }

        public async Task<IEnumerable<RecipeEntity>> GetAllPublicRecipesAsync() {
            var publicRecipeCollection = new List<RecipeEntity>();
            var publicRecipes = await _recipeRepo.ReadPublicAsync();
            foreach(var recipe in publicRecipes) {
                publicRecipeCollection.Add(new RecipeEntity {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    Ingredients = await _recipeIngredientRepo.ReadRecipeIngredientsAsync(recipe.Id),
                    Steps = await _recipeStepRepo.ReadRecipeStepsAsync(recipe.Id)
                });
            }
            return publicRecipeCollection;
        }

        public async Task<IEnumerable<RecipeEntity>> GetAllUserRecipesAsync(Guid userId) {
            //TODO:: if using real login this would need to be smarter and have a redirect if no user logged in... just in case.
            var userRecipeCollection = new List<RecipeEntity>();
            var userRecipes = await _recipeRepo.ReadPrivateAsync(userId);
            foreach (var recipe in userRecipes) {
                userRecipeCollection.Add(new RecipeEntity {
                    Id = recipe.Id,
                    Name = recipe.Name,
                    Description = recipe.Description,
                    IsPublic = recipe.IsPublic,
                    Ingredients = await _recipeIngredientRepo.ReadRecipeIngredientsAsync(recipe.Id),
                    Steps = await _recipeStepRepo.ReadRecipeStepsAsync(recipe.Id)
                });
            }
            return userRecipeCollection;
        }

        public async Task<RecipeEntity> GetRecipeDetailAsync(Guid recipeId) {
            var thisRecipe = await _recipeRepo.ReadByIdAsync(recipeId);
            return new RecipeEntity {
                Id = thisRecipe.Id,
                Name = thisRecipe.Name,
                Description = thisRecipe.Description,
                IsPublic = thisRecipe.IsPublic,
                Ingredients = await _recipeIngredientRepo.ReadRecipeIngredientsAsync(recipeId),
                Steps = await _recipeStepRepo.ReadRecipeStepsAsync(recipeId)
            };
        }

        public async Task UpdateAsync( RecipeEntity recipeEntity) {
            var tasks = new List<Task>();
            tasks.Add(_recipeRepo.SoftUpdateAsync(new Recipe { Id = recipeEntity.Id, Name = recipeEntity.Name, Description = recipeEntity.Description }));
            await Task.WhenAll(tasks);
        }

        public async Task AddRecipeStepAsync(Step recipeStep) {
            await _recipeStepRepo.CreateAsync(recipeStep);
        }

        public async Task RemoveRecipeStepAsync(Guid recipeStepId) {
            await _recipeStepRepo.DeleteAsync(recipeStepId);
        }

        public async Task AddRecipeIngredientAsync(Ingredient recipeIngredient) {
            await _recipeIngredientRepo.CreateAsync(recipeIngredient);
        }

        public async Task RemoveRecipeIngredientAsync(Guid recipeIngredientId) {
            await _recipeIngredientRepo.DeleteAsync(recipeIngredientId);
        }

        public async Task RemoveAsync(Guid id) {
            var tasks = new List<Task>();
            tasks.Add(_recipeIngredientRepo.DeleteRecipeIngredientsAsync(id));
            tasks.Add(_recipeStepRepo.DeleteRecipeStepsAsync(id));
            await Task.WhenAll(tasks);
            await _recipeRepo.DeleteAsync(id);
        }

        public async Task MakeRecipePublicAsync(Guid recipeId) {
            var recipe = await _recipeRepo.ReadByIdAsync(recipeId);
            recipe.IsPublic = true;
            await _recipeRepo.UpdateAsync(recipe);
        }

        public async Task MakeRecipePrivateAsync(Guid recipeId) {
            var recipe = await _recipeRepo.ReadByIdAsync(recipeId);
            recipe.IsPublic = false;
            await _recipeRepo.UpdateAsync(recipe);
        }
    }
}
