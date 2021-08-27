using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models {
    public class UserAuth {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Salt { get; set; }
        public string Pepper { get; set; }
        public int Iteration { get; set; }
        public string PWord { get; set; }
        public bool IsEnabled { get; set; }
    }
}