using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppAB;
using Microsoft.AspNet.Identity;
using AppAB.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Globalization;

namespace AppAB.Controllers
{
    public class OrdersController : Controller
    {
        private abEntities db = new abEntities();
        public const string CartSessionKey = "CartId";
        private CultureInfo slovensko = new CultureInfo("sl-SI");

        //
        // GET: Orders
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.aspnetusers).ToList();

            //Prepare a list of viewModels
            List<OrdersListViewModel> list = null;
            if (orders != null)
            {
                list = new List<OrdersListViewModel>();
                foreach (orders order in orders)
                {
                    OrdersListViewModel orderViewModel = new OrdersListViewModel
                    {
                        id=order.id,
                        userName = order.aspnetusers.UserName,
                        numberOfItems = order.order_items.Count(),
                        total_price=order.total_price
                    };
                    list.Add(orderViewModel);
                }
            }
            return View(list);
        }

        //
        //GET: Orders/Details/id
        //Get contents of the order/cart //Coming from list of orders(admin)
        [Authorize(Roles = "admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderViewModel viewModel = null;

            //Get order from db
            orders order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Set up our ViewModel
                viewModel = new OrderViewModel();
                viewModel.items = order.order_items.ToList();
                viewModel.orderTotal = order.total_price;
            }
            ViewBag.Title = order.aspnetusers.UserName;
            
            // Return the view
            return View(viewModel);
        }

        //GET: Orders/MyCart/id
        //Get contents of the the cart //Coming from nav-link Kosarica
        [Authorize(Roles = "admin,user")]
        public ActionResult MyCart(string id)
        {
            orders order=null;
            OrderViewModel viewModel = null;

            //If id is null find orderId(from Session) if it exists
            if (string.IsNullOrEmpty(id) && System.Web.HttpContext.Current.Session[CartSessionKey] != null)
            {
                id = System.Web.HttpContext.Current.Session[CartSessionKey].ToString();
            }

            //Get order from db
            order = db.orders.Find(id);

            //Only set viewModel if the order was made by current user
            if (order != null && order.order_items.Count() > 0 && System.Web.HttpContext.Current.User.Identity.GetUserId() == order.user_id)
            {
                viewModel = new OrderViewModel();
                viewModel.items = order.order_items.ToList();
                viewModel.orderTotal = order.total_price;
            }
            ViewBag.Title = "Moja košarica";

            // Return the view
            return View("Details", viewModel);
        }

        //
        //Method for adding products to order/cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,user")]
        public ActionResult AddToOrder(int? productId)
        {
            if (productId==null)
            {
                return RedirectToAction("Index", "Products");
            }
            else
            {
                //Find current user and product to add
                products product = db.products.Find(productId);
                aspnetusers user = db.aspnetusers.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

                string redirectID = "";
                if (System.Web.HttpContext.Current.Session[CartSessionKey] == null)
                {
                    redirectID = CreateNewOrder(product, user);                    
                }
                else
                {
                    //Find the order
                    orders order = db.orders.Find(System.Web.HttpContext.Current.Session[CartSessionKey].ToString());

                    //If order was made by current user, add product to it, if not, create a new order
                    if (order.user_id == user.Id)
                    {
                        AddProductToOrder(order, product);
                        redirectID = order.id;
                    }
                    else
                    {
                        redirectID = CreateNewOrder(product, user);
                    }            
                }
                return RedirectToAction("MyCart", new { id = redirectID });
            }            
        }

        //Create new order and return its ID
        private string CreateNewOrder(products product, aspnetusers user)
        {
            //Generate new GUID
            orders findOrder;
            string guidid = "";
            do
            {
                Guid GUIDid = Guid.NewGuid();
                guidid = GUIDid.ToString();
                findOrder = db.orders.Find(guidid);
            }
            while (findOrder != null);

            //Create new order with selected(added to cart) product
            orders newOrder = new orders();
            newOrder.id = guidid;
            newOrder.user_id = user.Id;
            newOrder.total_price = Convert.ToDecimal(product.price, slovensko);
            db.orders.Add(newOrder);

            //Add entry to order_items
            order_items item = new order_items();
            item.order_id = guidid;
            item.product_id = product.id;
            item.Quantity = 1;
            db.order_items.Add(item);

            db.SaveChanges();

            //Set cookie
            System.Web.HttpContext.Current.Session[CartSessionKey] = guidid;

            return guidid;
        }

        private void AddProductToOrder(orders order, products product)
        {
            order_items item = db.order_items.Where(i => i.order_id == order.id && i.product_id == product.id).FirstOrDefault();
            if (item != null)
            {
                //Increase quantity if the product is already on the order
                item.Quantity += 1;
                order.total_price += Convert.ToDecimal(product.price, slovensko);
            }
            else
            {
                //Add the product to order
                item = new order_items();
                item.order_id = order.id;
                item.product_id = product.id;
                item.Quantity = 1;

                order.order_items.Add(item);
                order.total_price += Convert.ToDecimal(product.price, slovensko);
            }
            db.SaveChanges();
        }

        //
        // AJAX: /Orders/RemoveFromOrder/5
        [HttpPost]
        public ActionResult RemoveFromOrder(int id)
        {
            //Find the user and order
            aspnetusers user = db.aspnetusers.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            orders order = db.orders.Find(System.Web.HttpContext.Current.Session[CartSessionKey].ToString());

            //If order was made by current user, remove the product from it
            if (order.user_id == user.Id)
            {
                order_items item = db.order_items.Where(i => i.id == id).FirstOrDefault();
                if (item != null)
                {
                    //Update total price
                    order.total_price -= Convert.ToDecimal(item.products.price, slovensko);

                    //Fill the viewModel, confirmation message
                    var results = new RemoveFromOrderViewModel
                    {
                        message = Server.HtmlEncode(item.products.name) +
                            " je bil odstranjen iz vaše košarice.",
                        totalPrice = order.total_price,
                        deleteId = id,
                        itemCount = 0
                    };

                    //Decrease quantity if there's more than 1 product or else remove it from the order
                    if (item.Quantity > 1)
                    {
                        item.Quantity -= 1;
                        results.itemCount = item.Quantity;            
                    }
                    else
                    {
                        order.order_items.Remove(item);
                        db.order_items.Remove(item);
                    }

                    //Save changes to db and return our viewModel
                    db.SaveChanges();                    
                    return Json(results);
                }
            }
            return Json(true);           
        }

        //
        //Delete not fully implemented yet(products FK order_items)
        // GET: Orders/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            orders orders = db.orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            orders orders = db.orders.Find(id);
            db.orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
