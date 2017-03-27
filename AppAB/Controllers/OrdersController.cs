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

            //Find orderId(Session) if it exists
            if (id == "MyCart")
            {
                HttpContextBase context = this.HttpContext;
                if (context.Session[CartSessionKey] == null)
                {
                    return View();
                }
                else
                {
                    id = context.Session[CartSessionKey].ToString();
                }

            }
            orders orders = db.orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            else if (System.Web.HttpContext.Current.User.Identity.GetUserId() != orders.user_id && !System.Web.HttpContext.Current.User.IsInRole("admin"))
            {
                //If order wasn't made by currently signed user return empty view/cart
                //Admin is an exception, so he can check others orders..
                return View();
            }

            return View(orders);
        }

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
                    newOrder.total_price = Convert.ToDecimal(product.price, new CultureInfo("sl-SI"));
                    db.orders.Add(newOrder);

                    //dodaj vnos v vmesno tabelo
                    order_items item = new order_items();
                    item.order_id = guidid;
                    item.product_id = product.id;
                    item.Quantity = 1;
                    db.order_items.Add(item);

                    //Set cookie
                    context.Session[CartSessionKey] = guidid;
                }
                else
                {
                    //Find the order
                    orders order = db.orders.Find(context.Session[CartSessionKey].ToString());

                    //If order was made by current user, add product to it, if not, create new order
                    if (order.user_id == user.Id)
                    {
                        order_items item = db.order_items.Where(i => i.order_id == order.id && i.product_id == product.id).FirstOrDefault();
                        if (item != null)
                        {
                            //Increase quantity if the product is already on the order
                            item.Quantity += 1;
                            order.total_price += Convert.ToDecimal(product.price, new CultureInfo("sl-SI"));
                        }
                        else
                        {
                            //Add product to order
                            item = new order_items();
                            item.order_id = order.id;
                            item.product_id = product.id;
                            item.Quantity = 1;

                            order.order_items.Add(item);
                            order.total_price += Convert.ToDecimal(product.price, new CultureInfo("sl-SI"));
                        }
                    }
                    else
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
                        newOrder.total_price = Convert.ToDecimal(product.price, new CultureInfo("sl-SI"));
                        db.orders.Add(newOrder);

                        //dodaj vnos v vmesno tabelo
                        order_items item = new order_items();
                        item.order_id = guidid;
                        item.product_id = product.id;
                        item.Quantity = 1;
                        db.order_items.Add(item);

                        //Set cookie
                        context.Session[CartSessionKey] = guidid;
                    }            
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id = "MyCart" });
            }
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
