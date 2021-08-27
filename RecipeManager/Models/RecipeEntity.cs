using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RecipeManager.Models {
    public class RecipeEntity {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public IEnumerable<Step> Steps { get; set; }
        public IEnumerable<Ingredient> Ingredients { get; set; }

        public RecipeEntity() {
            Steps = Enumerable.Empty<Step>();
            Ingredients = Enumerable.Empty<Ingredient>();
        }
    }
}