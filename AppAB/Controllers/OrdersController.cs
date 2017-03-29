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

        // GET: Orders
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.aspnetusers);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "admin,user")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            orders orders = null;
            HttpContextBase context = this.HttpContext;
            ViewBag.Title = "Moja košarica";


            //Coming from nav-link Kosarica
            //Find orderId(from Session) if it exists
            if (id == "MyCart")
            {
                if (context.Session[CartSessionKey] != null)
                {
                    id = context.Session[CartSessionKey].ToString();

                    //Get order from db
                    orders = db.orders.Find(id);

                    //Only show if the order was made by current user
                    if (orders != null && System.Web.HttpContext.Current.User.Identity.GetUserId() != orders.user_id)
                    {
                        orders = null;
                    }
                }

            }
            //Coming from list of orders(admin)
            else
            {
                if (System.Web.HttpContext.Current.User.IsInRole("admin"))
                {
                    //Get order from db
                    orders = db.orders.Find(id);
                    if (orders == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.Title = orders.aspnetusers.UserName;
                }

            }


            // Set up our ViewModel
            var viewModel = new OrderViewModel();
            if (orders != null)
            {
                viewModel.items = orders.order_items.ToList();
                viewModel.orderTotal = orders.total_price;
            }
            else
            {
                viewModel = null;
            }
            // Return the view
            return View(viewModel);
        }

        //Method for adding products to cart
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public ActionResult AddToOrder(int? productId)
        {
            if (productId==null)
            {
                return RedirectToAction("Index", "Products");
            }
            else
            {
                HttpContextBase context = this.HttpContext;

                //Find current user and product to add
                products product = db.products.Find(productId);
                aspnetusers user = db.aspnetusers.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());

                if (context.Session[CartSessionKey] == null)
                {
                    CreateNewOrder(context, product, user);
                }
                else
                {
                    //Find the order
                    orders order = db.orders.Find(context.Session[CartSessionKey].ToString());

                    //If order was made by current user, add product to it, if not, create a new order
                    if (order.user_id == user.Id)
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
                            //Add product to order
                            item = new order_items();
                            item.order_id = order.id;
                            item.product_id = product.id;
                            item.Quantity = 1;

                            order.order_items.Add(item);
                            order.total_price += Convert.ToDecimal(product.price, slovensko);
                        }
                    }
                    else
                    {
                        CreateNewOrder(context, product, user);
                    }            
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = "MyCart" });
            }
        }

        private void CreateNewOrder(HttpContextBase context, products product, aspnetusers user)
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

            //Set cookie
            context.Session[CartSessionKey] = guidid;
        }

        //
        // AJAX: /Orders/RemoveFromOrder/5
        [HttpPost]
        public ActionResult RemoveFromOrder(int id)
        {
            //Find the user and order
            aspnetusers user = db.aspnetusers.Find(System.Web.HttpContext.Current.User.Identity.GetUserId());
            orders order = db.orders.Find(this.HttpContext.Session[CartSessionKey].ToString());

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
