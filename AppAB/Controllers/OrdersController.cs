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

namespace AppAB.Controllers
{
    public class OrdersController : Controller
    {
        private abEntities db = new abEntities();
        public const string CartSessionKey = "CartId";

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.aspnetusers);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(string id)
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

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.aspnetusers, "Id", "Email");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,total_price,user_id")] orders orders)
        {
            orders.id = "Blaha";
            if (ModelState.IsValid)
            {
                db.orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.aspnetusers, "Id", "Email", orders.user_id);
            return View(orders);
        }

        [HttpPost]
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
                    newOrder.total_price = product.price;
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
                    order_items item = db.order_items.Where(i => i.order_id == order.id && i.product_id == product.id).FirstOrDefault();
                    if (item != null)
                    {
                        //Increase quantity if the product is already on the order
                        item.Quantity += 1;
                        order.total_price += item.products.price;
                    }
                    else
                    {
                        //Add product to order
                        item = new order_items();
                        item.order_id = order.id;
                        item.product_id = product.id;
                        item.Quantity = 1;

                        order.order_items.Add(item);
                        order.total_price += product.price;
                    }                    
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.user_id = new SelectList(db.aspnetusers, "Id", "Email", orders.user_id);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,total_price,user_id")] orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.aspnetusers, "Id", "Email", orders.user_id);
            return View(orders);
        }

        // GET: Orders/Delete/5
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
