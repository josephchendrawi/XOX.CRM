using CRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XOX.CRM.Lib;

namespace CRM.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService ProductService;

        public ProductController()
        {
            this.ProductService = new ProductService();
        }

        [UserAuthorize("PRODUCT", "VIEW")]
        public ActionResult List()
        {
            var products = ProductService.GetAllProducts();

            List<ProductModel> model = new List<ProductModel>();

            var parents = products.Where(d => d.Is_Package == true);

            foreach (var v in parents)
            {
                List<ProductModel> Child = new List<ProductModel>();

                var children = products.Where(d => d.Parent_Package_ID == v.ROW_ID && d.Is_Package == false);

                foreach (var y in children)
                {
                    Child.Add(new ProductModel()
                    {
                        ProductId = y.ROW_ID,
                        Category = y.PRD_CATEGORY,
                        Description = y.PRD_DESC,
                        Level = y.PRD_LVL,
                        Price = ((decimal)y.PRD_PRICE).ToString("0.00"),
                        PriceType = y.PRD_PRICE_TYPE,
                        Quota = y.QUOTA,
                        Type = y.PRD_TYPE,
                        VasFlag = y.VAS_FLG,
                        Child = new List<ProductModel>()
                    });
                }

                model.Add(new ProductModel()
                {
                    ProductId = v.ROW_ID,
                    Category = v.PRD_CATEGORY,
                    Description = v.PRD_DESC,
                    Level = v.PRD_LVL,
                    Price = ((decimal)v.PRD_PRICE).ToString("0.00"),
                    PriceType = v.PRD_PRICE_TYPE,
                    Quota = v.QUOTA,
                    Type = v.PRD_TYPE,
                    VasFlag = v.VAS_FLG,
                    Child = Child
                });
            }

            return View(model);
        }
    }
}