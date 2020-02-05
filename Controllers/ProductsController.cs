using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LojaVirtual.Data;
using LojaVirtual.Models;
using LojaVirtual.Models.ViewModels;
using System.Net.Http.Headers;
using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using LojaVirtual.Utils;

namespace LojaVirtual.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hosting;


        public ProductsController(ApplicationDbContext context, IMapper mapper, IHostingEnvironment hosting)
        {
            _context = context;
            _mapper = mapper;
            _hosting = hosting;
        }

        // GET: Products
        public async Task<IActionResult> Index(string filtroAtual, string filtro, int? pagina)
        {

            if(filtro != null)
            {
                pagina = 1;
            }
            else
            {
                filtro = filtroAtual;
            }

            ViewData["filtro"] = filtro;

            var products = from product in _context.Products.Include(p => p.ImageProducts).Include(p => p.Brand) select product;

            switch(filtroAtual)
            {
                case "MenorPreco":
                    products = products.OrderBy(p => p.Price);
                    break;

                case "MaiorPreco":
                    products = products.OrderByDescending(p => p.Price);
                    break;

                case "Novidades":
                    products = products.OrderByDescending(p => p.Id);
                    break;

                default:
                    products.ToList();
                    break;
            }


            int pageSize = 12;
            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pagina ??1, pageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department).Include(p=>p.ImageProducts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["BrandsId"] = new SelectList(_context.Brands, "Id", "Name");

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModels viewModels)
        {
            var product = _mapper.Map<Product>(viewModels.Product);
            if (HttpContext.Request.Form.Files != null)
            {
                var fileName = string.Empty;
                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        var FileExtension = Path.GetExtension(fileName);

                        string newFileName = myUniqueFileName + FileExtension;

                        var imageProduct = new ImageProduct()
                        {
                            Image = newFileName
                        };

                        product.ImageProducts.Add(imageProduct);

                        string[] contentTypes = new string[] { ".jpg", ".png", ".jpeg" };
                        if (!contentTypes.Contains(FileExtension))
                        {
                            return RedirectToAction("Index", "Products");
                        }

                        // Combines two strings into a path.
                        fileName = Path.Combine(_hosting.WebRootPath, "photos") + $@"\{newFileName}";

                        //if you want to store path of folder in database
                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["DepartmentId"] = new SelectList(_context.Brands, "Id", "Name", product.DepartmentId);
                return View(product);
            }
            return View(viewModels);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Brands, "Id", "Name", product.DepartmentId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Amount,DepartmentId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Brands, "Id", "Name", product.DepartmentId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
