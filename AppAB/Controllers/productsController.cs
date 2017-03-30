using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppAB;
using AppAB.Models;

namespace AppAB.Controllers
{
    public class ProductsController : Controller
    {
        private abEntities db = new abEntities();

        // GET: products(/parfumi)
        public ActionResult Index(string filterCategory)
        {
            //Query parfumes
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);

            //Filtriranje če je izbrana kategorija
            if (String.IsNullOrEmpty(filterCategory))
            {
                products = products.Where(p => p.product_subcategories.product_categories.name == "parfumi");
            }
            else
            {
                products = products.Where(p => p.product_subcategories.name == filterCategory);
            }

            //Seznam kategorij za izbiro/filter
            var showCategories = db.product_subcategories.Where(s => s.product_categories.name == "parfumi");
            ViewBag.listCategories = showCategories;

            //Prepare viewModel
            List<ProductListViewModel> productsList = getProductsList(products.ToList());

            ViewBag.Title = "Parfumi";
            ViewBag.action = "Index";
            return View(productsList);
        }


        // GET: products/nega
        public ActionResult Nega(string filterCategory)
        {
            //Query izdelke za nego
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            
            //Filtriranje če je izbrana kategorija
            if (String.IsNullOrEmpty(filterCategory))
            {
                products = products.Where(p => p.product_subcategories.product_categories.name == "nega_obraza" ||
                                            p.product_subcategories.product_categories.name == "nega_telesa" ||
                                            p.product_subcategories.product_categories.name == "nega_las");
            }
            else
            {
                products = products.Where(p => p.product_subcategories.name == filterCategory);
            }

            //Seznam kategorij za filter
            var showCategories = db.product_subcategories.Where(s => s.product_categories.name == "nega_obraza" ||
                                                                     s.product_categories.name == "nega_telesa" ||
                                                                     s.product_categories.name == "nega_las");
            ViewBag.listCategories = showCategories;

            //Prepare viewModel
            List<ProductListViewModel> productsList = getProductsList(products.ToList());

            ViewBag.action = "Nega";
            ViewBag.Title = "Izdelki za nego telesa";
            return View("Index", productsList);
        }

        // GET: products/nohti
        public ActionResult Nohti(string filterCategory)
        {
            //Query izdelke za nohte
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            

            //Filtriranje če je izbrana kategorija
            if (String.IsNullOrEmpty(filterCategory))
            {
                products = products.Where(p => p.product_subcategories.product_categories.name == "nohti");
            }
            else
            {
                products = products.Where(p => p.product_subcategories.name == filterCategory);
            }
            
            //Seznam kategorij za filter
            var showCategories = db.product_subcategories.Where(s => s.product_categories.name == "nohti");
            ViewBag.listCategories = showCategories;

            //Prepare viewModel
            List<ProductListViewModel> productsList = getProductsList(products.ToList());

            ViewBag.action = "Nohti";
            ViewBag.Title = "Izdelki za nego nohtov";
            return View("Index", productsList);
        }

        // GET: products/make_up
        public ActionResult MakeUp(string filterCategory)
        {
            //Query izdelke za nohte
            var products = db.products.Include(p => p.product_brands).Include(p => p.product_subcategories).Include(p => p.product_subcategories.product_categories);
            

            //Filtriranje če je izbrana kategorija
            if (String.IsNullOrEmpty(filterCategory))
            {
                products = products.Where(p => p.product_subcategories.product_categories.name == "make_up");
            }
            else
            {
                products = products.Where(p => p.product_subcategories.name == filterCategory);
            }            

            //Seznam kategorij za filter
            var showCategories = db.product_subcategories.Where(s => s.product_categories.name == "make_up");
            ViewBag.listCategories = showCategories;

            //Prepare viewModel
            List<ProductListViewModel> productsList = getProductsList(products.ToList());

            ViewBag.action = "Makeup";
            ViewBag.Title = "Makeup";
            return View("Index", productsList);
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
        [Authorize(Roles = "admin")]
        public ActionResult Create(string subcategory)
        {
            ViewBag.brand = new SelectList(db.product_brands, "id", "name");
            
            if (string.IsNullOrEmpty(subcategory))
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
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include = "name,description,image,price,brand,subcategory")] ProductCreateEditViewModel product)
        {
            if (ModelState.IsValid)
            {
                products newProduct = new products {
                    name=product.name,
                    description=product.description,
                    image= product.image,
                    price= product.price,
                    brand=product.brand,
                    subcategory=product.subcategory,
                    create_date = DateTime.Now
                };
                
                db.products.Add(newProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.brand = new SelectList(db.product_brands, "id", "name", product.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", product.subcategory);
            return View(product);
        }

        // GET: products/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products productDB = db.products.Find(id);
            ProductCreateEditViewModel product;
            if (productDB == null)
            {
                return HttpNotFound();
            }
            else
            {
                product = new ProductCreateEditViewModel
                {
                    name = productDB.name,
                    description = productDB.description,
                    image = productDB.image,
                    price = productDB.price,
                    brand = productDB.brand,
                    subcategory = productDB.subcategory
                };
            }
            ViewBag.brand = new SelectList(db.product_brands, "id", "name", product.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", product.subcategory);
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Edit([Bind(Include = "id,name,description,image,price,brand,subcategory")] ProductCreateEditViewModel product)
        {
            if (ModelState.IsValid)
            {
                products productToEdit = db.products.Find(product.id);
                productToEdit.name = product.name;
                productToEdit.description = product.description;
                productToEdit.image = product.image;
                productToEdit.price = product.price;
                productToEdit.brand = product.brand;
                productToEdit.subcategory = product.subcategory;

                //products.create_date = DateTime.Now;

                db.Entry(productToEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.brand = new SelectList(db.product_brands, "id", "name", product.brand);
            ViewBag.subcategory = new SelectList(db.product_subcategories, "id", "name", product.subcategory);
            return View(product);
        }

        // GET: products/Delete/5
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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

        //
        //Helper functions
        //
        //Get viewModels list for index page
        private List<ProductListViewModel> getProductsList(List<products> products)
        {
            List<ProductListViewModel> productsList = new List<ProductListViewModel>();
            foreach (products product in products)
            {
                ProductListViewModel vievModel = new ProductListViewModel
                {
                    id = product.id,
                    name = (product.name.Length > 19) ? (product.name.Substring(0, 19) + "...") : product.name,
                    description = (product.description.Length > 14) ? (product.description.Substring(0, 14) + "...") : product.description,
                    image = product.image,
                    price = product.price
                };
                productsList.Add(vievModel);
            }
            return productsList;
        }
    }
}
