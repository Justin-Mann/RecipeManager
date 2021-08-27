using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecipeManager.Models;
using RecipeManager.Services;
using System;
using System.Threading.Tasks;

namespace RecipeManager.Controllers {
    public class RecipeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IRecipeService _recipeService;

        public RecipeController(ILogger<HomeController> logger, IRecipeService recipeService) {
            _logger = logger;
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index() {
            return View(await _recipeService.GetAllPublicRecipesAsync());
        }

        public async Task<IActionResult> MyCollection() {
            var userId = new Guid("51000000-0000-0000-0000-000000000000");//TODO: userservice and get logged in user from service
            return View(await _recipeService.GetAllUserRecipesAsync(userId));
        }

        public IActionResult Create() { 
            return View();
        }

        public async Task<IActionResult> Delete(Guid id) {
            await _recipeService.RemoveAsync(id);
            return RedirectToAction("MyCollection");
        }

        public async Task<IActionResult> CreateNew(RecipeEntity newRecipe) {
            newRecipe.Id = await _recipeService.AddAsync(newRecipe);
            return RedirectToAction("Edit", new { id = newRecipe.Id });
        }

        public async Task<IActionResult> Edit(Guid id) {
            return View(await _recipeService.GetRecipeDetailAsync(id));
        }

        public async Task<IActionResult> UpdateRecipe(RecipeEntity thisRecipe) {
            await _recipeService.UpdateAsync(thisRecipe);
            return RedirectToAction("Edit", new { id = thisRecipe.Id });
        }

        public async Task<IActionResult> Detail(Guid id) {
            return View(await _recipeService.GetRecipeDetailAsync(id));
        }

        public async Task<IActionResult> AddRecipeStep(Step newStep) {
            newStep.Id = Guid.NewGuid();
            await _recipeService.AddRecipeStepAsync(newStep);
            return RedirectToAction("Edit", new { id = newStep.RecipeId });
        }

        public async Task<IActionResult> RemoveRecipeStep(Guid recipeStepId, Guid recipeId) {
            await _recipeService.RemoveRecipeStepAsync(recipeStepId);
            return RedirectToAction("Edit", new { id = recipeId });
        }

        public async Task<IActionResult> AddRecipeIngredient(Ingredient newIngredient) {
            newIngredient.Id = Guid.NewGuid();
            await _recipeService.AddRecipeIngredientAsync(newIngredient);
            return RedirectToAction("Edit", new { id = newIngredient.RecipeId });
        }

        public async Task<IActionResult> RemoveRecipeIngredient(Guid recipeIngredientId, Guid recipeId) {
            await _recipeService.RemoveRecipeIngredientAsync(recipeIngredientId);
            return RedirectToAction("Edit", new { id = recipeId });
        }

        public async Task<IActionResult> MakeRecipePublic(Guid recipeId) {
            await _recipeService.MakeRecipePublicAsync(recipeId);
            return RedirectToAction("Edit", new { id = recipeId });
        }

        public async Task<IActionResult> MakeRecipePrivate(Guid recipeId) {
            await _recipeService.MakeRecipePrivateAsync(recipeId);
            return RedirectToAction("Edit", new { id = recipeId });
        }
    }
}