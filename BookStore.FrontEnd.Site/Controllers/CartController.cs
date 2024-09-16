using BookStore.FrontEnd.Site.Models.EFModels;
using BookStore.FrontEnd.Site.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BookStore.FrontEnd.Site.Controllers
{

    public class CartController : Controller
    {
        [Authorize]
        
        public ActionResult AddItem(int productId)
        {
            //取得目前登入者
            var account = User.Identity.Name;

            //加入購屋車
            int qty = 1;
            Add2Cart(account, productId, qty);

            return new EmptyResult();
        }

        [Authorize]
        public ActionResult Info()
        {
            var account = User.Identity.Name;
            var cartInfo = GetCartInfo(account);

            return View(cartInfo);
        }


        [Authorize]

        public ActionResult UpdateItem(int productId, int newQty)
        {
            var account = User.Identity.Name;
            newQty = newQty <= 0 ? 0 : newQty;

            UpdateItemQty(account, productId, newQty);

            return new EmptyResult();
        }

        private void UpdateItemQty(string account, int productId, int newQty)
        {
            // 取得目前購物車主檔，若沒有，就立刻新增一筆並傳回
            var cart = GetCartInfo(account);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null) return;

            var db = new AppDbContext();
            // 如果 newQty == 0，就從購物車明細中刪除，否則就更新數量
            if (newQty == 0)
            {
                var entity = db.CartItems.Find(cartItem.Id);
                if (entity != null) db.CartItems.Remove(entity);

                db.SaveChanges();
                return;
            }

            var cartItemInDb = db.CartItems.FirstOrDefault(ci => ci.Id == cartItem.Id);
            cartItemInDb.Qty = newQty;
            db.SaveChanges();
        }

        /// <summary>
        /// 加入購屋車,若明細不存在舊新增一筆，若存在就更新數量
        /// </summary>
        /// <param name="account"></param>
        /// <param name="productId"></param>
        /// <param name="qty"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Add2Cart(string account, int productId, int qty)
        {
           int cartId = GetCartInfo(account).Id;
            //加入購物車
            AddCartItem(cartId, productId, qty);
        }


        /// <summary>
        /// 加入購屋車,若明細不存在就新增一筆,若存在就更新數量
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="productId"></param>
        /// <param name="qty"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AddCartItem(int cartId, int productId, int qty)
        {
            var db = new AppDbContext();
            var cartItem = db.CartItems.FirstOrDefault(c => c.CartId == cartId && c.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Qty = qty
                };
                db.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Qty += qty;
            }

            db.SaveChanges();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private CartVm GetCartInfo(string account)
        {
            var db = new AppDbContext();
            var cart = db.Carts.FirstOrDefault(c => c.MemberAccount == account);

            if (cart == null)
            {
                cart = new Cart { MemberAccount = account };
                db.Carts.Add(cart);
                db.SaveChanges();
            }

            var cartInfo = db.Carts.AsNoTracking().Include(c => c.CartItems)
                            .Where(c => c.MemberAccount == account)
                            .Select(c => new CartVm
                            {
                                Id = c.Id,
                                MemberAccount = c.MemberAccount,
                                CartItems = c.CartItems.Select(ci => new CartItemVm
                                {
                                    Id = ci.Id,
                                    CartId = ci.CartId,
                                    ProductId = ci.ProductId,
                                    Product = new ProductIndexVm
                                    {
                                        Id = ci.Product.Id,
                                        CategoryName = ci.Product.Category.Name,
                                        Name = ci.Product.Name,
                                        Price = ci.Product.Price,
                                    },
                                    Qty = ci.Qty
                                })
                            })
                            .First();
            return cartInfo;

        }
    }
}