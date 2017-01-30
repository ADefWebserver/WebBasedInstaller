using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using WebBasedInstaller.Models;
namespace WebBasedInstaller.Controllers
{
    public class ODataProductsController : ODataController
    {
        // This connects to the database
        private InstallWizardEntities db = new InstallWizardEntities();
        // Note: All methods are decorated with [Authorize]
        // This means they can only be called if the user is logged in
        #region public IQueryable<DTOProduct> GetODataProducts()
        // GET: odata/ODataProducts
        [Authorize]
        [EnableQuery(PageSize = 100)]
        public IQueryable<DTOProduct> GetODataProducts()
        {
            // This returns all products in the database
            var result = (from product in db.Products
                          select new DTOProduct
                          {
                              Id = product.Id,
                              ProductName = product.ProductName,
                              ProductPrice = product.ProductPrice.ToString()
                          });
            return result.AsQueryable();
        }
        #endregion
        #region public IHttpActionResult Post(DTOProduct dtoProduct)
        // POST: odata/ODataProducts
        [Authorize]
        public IHttpActionResult Post(DTOProduct dtoProduct)
        {
            // Create a new Product
            var NewProduct = new Product();
            NewProduct.ProductName = dtoProduct.ProductName;
            NewProduct.ProductPrice = Convert.ToDecimal(dtoProduct.ProductPrice);
            // Save the Product
            db.Products.Add(NewProduct);
            db.SaveChanges();
            // Populate the ID that was created and pass it back
            dtoProduct.Id = NewProduct.Id;
            // Return the Product
            return Created(dtoProduct);
        }
        #endregion
        #region public IHttpActionResult Put([FromODataUri] int key, DTOProduct dtoProduct)
        // PUT: odata/ODataProducts(1)
        [Authorize]
        public IHttpActionResult Put([FromODataUri] int key, DTOProduct dtoProduct)
        {
            // Get the existing Product using the key that was passed
            Product ExistingProduct = db.Products.Find(key);
            // Did we find a Product?
            if (ExistingProduct == null)
            {
                // If not return NotFound
                return StatusCode(HttpStatusCode.NotFound);
            }
            // Update the Product 
            ExistingProduct.ProductName = dtoProduct.ProductName;
            ExistingProduct.ProductPrice = Convert.ToDecimal(dtoProduct.ProductPrice);
            // Save changes
            db.Entry(ExistingProduct).State = EntityState.Modified;
            db.SaveChanges();
            // Return the Updated Product
            // Return that the Product was Updated
            return Updated(ExistingProduct);
        }
        #endregion
        #region public IHttpActionResult Delete([FromODataUri] int key)
        // DELETE: odata/ODataProducts(1)
        [Authorize]
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            // Get the existing Product using the key that was passed
            Product ExistingProduct = db.Products.Find(key);
            // Did we find a Product?
            if (ExistingProduct == null)
            {
                // If not return NotFound
                return StatusCode(HttpStatusCode.NotFound);
            }
            // Delete the Product
            // (and any Product Detail(s))
            db.Products.Remove(ExistingProduct);
            // Save changes
            db.SaveChanges();
            // Return a success code
            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion
        // Utility
        #region protected override void Dispose(bool disposing)
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of the database object
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}