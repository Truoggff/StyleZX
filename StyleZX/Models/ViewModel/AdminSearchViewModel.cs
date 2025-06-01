using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StyleZX.Models.ViewModel
{
    public class AdminSearchViewModel
    {
        public string Keyword { get; set; }

        public IPagedList<User> Users { get; set; }
        public IPagedList<Category> Categories { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IPagedList<Order> Orders { get; set; }
    }


}