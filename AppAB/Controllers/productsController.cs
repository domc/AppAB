using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppAB;

namespace AppAB.Controllers
{
    public class productsController : Controller
    {
        private abdbEntities db = new abdbEntities();

        // GET: products(/parfumi)
        public ActionResult Index()
        {
            //Query parfumes
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            products = products.Where(p => p.product_subcategories.product_categories.id == 1);

            ViewBag.Title = "Parfumi";

            return View(products.ToList());
        }

        // GET: products/nega
        public ActionResult Nega()
        {
            //Query izdelke za nego
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            products = products.Where(p => p.product_subcategories.product_categories.id == 3 ||
                                            p.product_subcategories.product_categories.id == 4 ||
                                            p.product_subcategories.product_categories.id == 5);

            ViewBag.Title = "Izdelki za nego telesa";

            return View("Index", products.ToList());
        }

        // GET: products/nohti
        public ActionResult Nohti()
        {
            //Query izdelke za nohte
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            products = products.Where(p => p.product_subcategories.product_categories.id == 2);

            ViewBag.Title = "Izdelki za nego nohtov";

            return View("Index", products.ToList());
        }

        // GET: products/make_up
        public ActionResult MakeUp()
        {
            //Query izdelke za nohte
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            products = products.Where(p => p.product_subcategories.product_categories.id == 6);

            ViewBag.Title = "Makeup";

            return View("Index", products.ToList());
        }

        // GET: products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: products/Create/subcategory(ie "parfumi")
        public ActionResult Create(string subcategory)
        {
            ViewBag.brand = new SelectList(db.product_brands, "id", "name");
            
            if (String.IsNullOrEmpty(subcategory))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Show (only) the right subcategories
            var subcategories = db.product_subcategories.Where(p => p.product_categories.name.Contains(subcategory));

            //var parfumeSubcategories = db.product_subcategories.Where(p => p.product_categories.id == 1);
            ViewBag.subcategory = new SelectList(subcategories, "id", "name");

            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,image,price,create_date,brand,subcategory")] products products)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.brand = new SelectList(db.product_brands, "id", "name", products.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", products.subcategory);
            return View(products);
        }

        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.brand = new SelectList(db.product_brands, "id", "name", products.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", products.subcategory);
            return View(products);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,image,price,create_date,brand,subcategory")] products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.brand = new SelectList(db.product_brands, "id", "name", products.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", products.subcategory);
            return View(products);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = db.products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            products products = db.products.Find(id);
            db.products.Remove(products);
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
