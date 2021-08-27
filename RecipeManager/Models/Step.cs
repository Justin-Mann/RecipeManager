using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Models {
    public class Step {
        public Guid Id { get; set; }
        [Required]
        public Guid RecipeId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int? SortOrder { get; set; }
        [Required]
        public string Detail { get; set; }
        public decimal? Duration { get; set; }
        public string DurationUnit { get; set; }
    }
}
