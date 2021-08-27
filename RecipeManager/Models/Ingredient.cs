using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Models {
    public class Ingredient {
        public Guid Id { get; set; }
        [Required]
        public Guid RecipeId { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal? Units { get; set; }
        public string UnitOfMeasure { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
