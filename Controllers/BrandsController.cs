using AutoMapper;
using LojaVirtual.Data;
using LojaVirtual.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LojaVirtual.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController( ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
               
                catch( Exception e)
                {
                    ModelState.AddModelError("", "Erro ocorrido : \n" + e.Message);
                }
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if(brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if(id != brand.Id)
            {
                return NotFound();
            }

            try
            {
                _context.Update(brand);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Brands.AnyAsync(p=>p.Id == brand.Id))
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
        public async Task<IActionResult> Delete(int? id)
        {
            if( id == null)
            {
                return NotFound();
            }
            var brand = await _context.Brands.FirstOrDefaultAsync(p=>p.Id == id);

            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brands = await _context.Brands.FindAsync(id);
            try
            {
                _context.Brands.Remove(brands);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                ModelState.AddModelError("","Erro ocorrido : \n" + e.Message);
            }
            return View(brands);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
