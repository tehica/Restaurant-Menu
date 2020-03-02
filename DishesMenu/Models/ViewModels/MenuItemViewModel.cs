﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DishesMenu.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }

        public IEnumerable<Category> Category { get; set; }

        public IEnumerable<SubCategory> SubCategory { get; set; }

    }
}
