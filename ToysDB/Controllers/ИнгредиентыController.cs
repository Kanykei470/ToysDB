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
        public async Task<IActionResult> Create([Bind("Id,Продукция,Сырье,Количество")] Ингредиенты ingredient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Product = new SqlParameter("@Product", ingredient.Продукция);
                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", ingredient.Сырье);
                    SqlParameter Amount = new SqlParameter("@Amount", ingredient.Количество);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Ingredients @Product, @RawMaterial, @Amount", Product, RawMaterial, Amount);

                    return RedirectToAction(nameof(Index));
                }
                var finishedProducts = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
                var dataRawMaterial = new SelectList(_context.Сырьёs, "Id", "Наименование");

                ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title", ingredient.Продукция);
                ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", ingredient.Сырье);
                return View(ingredient);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Ингредиенты/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var ingredient = await _context.Ингредиентыs.FromSqlRaw("dbo.GetID_Ingredients @Id", Id).ToListAsync();

            if (ingredient.FirstOrDefault() == null)
            {
                return NotFound();
            }
            var finishedProducts = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
            var dataRawMaterial = new SelectList(_context.Сырьёs, "Id", "Наименование");

            ViewData["Product"] = new SelectList(finishedProducts, "Id", "Title", ingredient.FirstOrDefault().Продукция);
            ViewData["RawMaterial"] = new SelectList(dataRawMaterial, "Id", "Title", ingredient.FirstOrDefault().Сырье);
            return View(ingredient.FirstOrDefault());
        }

        // POST: Ингредиенты/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Продукция,Сырье,Количество")] Ингредиенты ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", ingredient.Id);
                    SqlParameter Product = new SqlParameter("@Product", ingredient.Продукция);
                    SqlParameter RawMaterial = new SqlParameter("@RawMaterial", ingredient.Сырье);
                    SqlParameter Amount = new SqlParameter("@Amount", ingredient.Количество);

                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Ingredients @Id, @Product, @RawMaterial, @Amount", Id, Product, RawMaterial, Amount);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ИнгредиентыExists(ingredient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (SqlException ex)
                {
                    return NotFound(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Продукция",ingredient.Продукция);
            ViewData["Сырье"] =  new SelectList(_context.Сырьёs, "Id", "Сырье",ingredient.Сырье);

            return View(ingredient);
        }

        // GET: Ингредиенты/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var ingredient = await _context.Ингредиентыs.FromSqlRaw("dbo.GetID_Ingredients @Id", Id).ToListAsync();

            SqlParameter IdProduc = new SqlParameter("@IdProduc", ingredient.FirstOrDefault().Продукция);
            var product = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Ingredients @IdProduc", IdProduc).ToListAsync();

            SqlParameter IdRaw = new SqlParameter("@IdRaw", ingredient.FirstOrDefault().Сырье);
            var raw = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @IdRaw", IdRaw).ToListAsync();

            if (ingredient.FirstOrDefault().Продукция == product.FirstOrDefault().Id)
                ingredient.FirstOrDefault().ПродукцияNavigation.Наименование = product.FirstOrDefault().Наименование;
            if (ingredient.FirstOrDefault().Сырье == raw.FirstOrDefault().Id)
                ingredient.FirstOrDefault().СырьеNavigation.Наименование = raw.FirstOrDefault().Наименование;

            if (ingredient.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(ingredient.FirstOrDefault());
        }

        // POST: Ингредиенты/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter Id = new SqlParameter("@Id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Ingredients @Id", Id);
            return RedirectToAction(nameof(Index));
        }

        private bool ИнгредиентыExists(byte id)
        {
            return _context.Ингредиентыs.Any(e => e.Id == id);
        }
    }
}
