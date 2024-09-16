using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using BookStore.FrontEnd.Site.Models.ViewModels;
using BookStore.FrontEnd.Site.Models.EFModels;

namespace BookStore.FrontEnd.Site.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            var db = new AppDbContext();
            var data = db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.Category.DisplayOrder)
                .Select(p => new ProductIndexVm
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    Price = p.Price,
                })
                .ToList();

            return View(data);
        }
    }
}