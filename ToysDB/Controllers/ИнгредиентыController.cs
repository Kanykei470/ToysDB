using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using ToysDB.ViewModels;

namespace ToysDB.Controllers
{
    public class ИнгредиентыController : Controller
    {
        private readonly ToysContext _context;

        public ИнгредиентыController(ToysContext context)
        {
            _context = context;
        }

        // GET: Ингредиенты
        public async Task<IActionResult> Index(
            //int? finprod, string name
            )
        {
            //IQueryable<Ингредиенты> Ингредиенты = _context.Ингредиентыs.Include(p => p.ПродукцияNavigation).Include(u => u.СырьеNavigation);
            //if (finprod != null && finprod != 0)
            //{
            //    Ингредиенты = Ингредиенты.Where(p => p.ПродукцияNavigation.Id == finprod);
            //}

            //List<ГотоваяПродукция> ГотоваяПродукция = await _context.ГотоваяПродукцияs.ToListAsync();

            //ГотоваяПродукция.Insert(0, new ГотоваяПродукция { Наименование = "Все", Id = 0 });

            //ИнгViewModel ingridientsViewModel = new ИнгViewModel
            //{
            //    Ингредиенты = Ингредиенты,
            //    ГотовыйПродукт = new SelectList(ГотоваяПродукция, "Id", "Наименование"),
            //    Наименование = name,
            //    ВыбранныйПродукт = finprod
            //};
            //if (finprod.HasValue)
            //{
            //    var itemToSelect = ingridientsViewModel.ГотовыйПродукт.FirstOrDefault(x => x.Value == finprod.Value.ToString());
            //    itemToSelect.Selected = true;
            //}

            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.Get_Raw_Materials").ToListAsync();
            var ingridients = await _context.Ингредиентыs.FromSqlRaw("dbo.Get_Ingredients").ToListAsync();

            foreach (var ing in ingridients)
            {
                foreach (var raw in rawMaterials)
                {
                    if (ing.Сырье == raw.Id)
                    {
                       ing.СырьеNavigation.Наименование=raw.Наименование;
                    }
                }
            }
            return View(ingridients);
        }

        // GET: Ингредиенты/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);
            var ingr = await _context.Ингредиентыs.FromSqlRaw("dbo.GetID_Ingredients @id", ID).ToListAsync();
            SqlParameter rmID = new SqlParameter("@Id", ingr.FirstOrDefault().Сырье);
            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @id", rmID).ToListAsync();
            SqlParameter production = new SqlParameter("@Id", ingr.FirstOrDefault().Продукция);
            var pID = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", production).ToListAsync();

            if ((ingr.FirstOrDefault().Сырье == rawMaterials.FirstOrDefault().Id) &&
                (ingr.FirstOrDefault().Продукция == pID.FirstOrDefault().Id)) 
            {
                ingr.FirstOrDefault().СырьеNavigation.Наименование = rawMaterials.FirstOrDefault().Наименование;
                ingr.FirstOrDefault().ПродукцияNavigation.Наименование = pID.FirstOrDefault().Наименование;
            }
            if (ingr == null)
            {
                return NotFound();
            }

            return View(ingr.FirstOrDefault());
        }

        // GET: Ингредиенты/Create
        public IActionResult Create()
        {
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
            ViewData["Сырье"] = new SelectList(_context.Сырьёs, "Id", "Наименование");
            return View();
        }

        // POST: Ингредиенты/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Продукция,Сырье,Количество")] Ингредиенты ингредиенты)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ингредиенты);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", ингредиенты.Продукция);
            ViewData["Сырье"] = new SelectList(_context.Сырьёs, "Id", "Наименование", ингредиенты.Сырье);
            return View(ингредиенты);
        }

        // GET: Ингредиенты/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ингредиенты = await _context.Ингредиентыs.FindAsync(id);
            if (ингредиенты == null)
            {
                return NotFound();
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", ингредиенты.Продукция);
            ViewData["Сырье"] = new SelectList(_context.Сырьёs, "Id", "Наименование", ингредиенты.Сырье);
            return View(ингредиенты);
        }

        // POST: Ингредиенты/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Продукция,Сырье,Количество")] Ингредиенты ингредиенты)
        {
            if (id != ингредиенты.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ингредиенты);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ИнгредиентыExists(ингредиенты.Id))
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
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", ингредиенты.Продукция);
            ViewData["Сырье"] = new SelectList(_context.Сырьёs, "Id", "Наименование", ингредиенты.Сырье);
            return View(ингредиенты);
        }

        // GET: Ингредиенты/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ингредиенты = await _context.Ингредиентыs
                .Include(и => и.ПродукцияNavigation)
                .Include(и => и.СырьеNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ингредиенты == null)
            {
                return NotFound();
            }

            return View(ингредиенты);
        }

        // POST: Ингредиенты/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var ингредиенты = await _context.Ингредиентыs.FindAsync(id);
            _context.Ингредиентыs.Remove(ингредиенты);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ИнгредиентыExists(byte id)
        {
            return _context.Ингредиентыs.Any(e => e.Id == id);
        }
    }
}
