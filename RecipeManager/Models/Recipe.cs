using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Models {
    public class Recipe {
        public Guid Id { get; set; }
        [Required]
        public Guid OwnerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
    }
}
