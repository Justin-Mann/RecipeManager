using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeManager.Models {
    public class User {
        public Guid Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UName { get; set; }
    }
}
