using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesMenu.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoryList { get; set; }

        public SubCategory SubCategory { get; set; }

        // we need this property to store names of Subcategories
        public List<string> SubCategoryList { get; set; }

        // prop for display error messages 
        public string StatusMessage { get; set; }

    }
}
