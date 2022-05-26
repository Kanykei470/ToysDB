using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;

namespace ToysDB.Controllers
{
    public class ГотоваяПродукцияController : Controller
    {
        private readonly ToysContext _context;

        public ГотоваяПродукцияController(ToysContext context)
        {
            _context = context;
        }

        // GET: ГотоваяПродукция
        public async Task<IActionResult> Index()
        {
            var units = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.Get_Units").ToListAsync();
            var finishedProduction = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.Get_Finished_Production").ToListAsync();
            foreach (var fp in finishedProduction)
            {
                foreach (var u in units)
                {
                    if (fp.ЕдиницаИзмерения == u.Id)
                    {
                        fp.ЕдиницаИзмеренияNavigation.Наименование = u.Наименование;
                    }
                }
               
            }

            return View(finishedProduction);
        }

        // GET: ГотоваяПродукция/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);
            var readyProducts = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", ID).ToListAsync();
            SqlParameter unitID = new SqlParameter("@Id", readyProducts.FirstOrDefault().ЕдиницаИзмерения);
            var unit = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.GetID_Units @id", unitID).ToListAsync();

            if (readyProducts.FirstOrDefault().ЕдиницаИзмерения == unit.FirstOrDefault().Id)
                readyProducts.FirstOrDefault().ЕдиницаИзмеренияNavigation.Наименование = unit.FirstOrDefault().Наименование;

            if (readyProducts == null)
            {
                return NotFound();
            }

            return View(readyProducts.FirstOrDefault());
        }

        // GET: ГотоваяПродукция/Create
        public IActionResult Create()
        {
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View();
        }

        // POST: ГотоваяПродукция/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] ГотоваяПродукция finishedProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Title = new SqlParameter("@Title", finishedProduct.Наименование);
                    SqlParameter Unit = new SqlParameter("@Unit", finishedProduct.ЕдиницаИзмерения);
                    SqlParameter Summa = new SqlParameter("@Summa", finishedProduct.Сумма);
                    SqlParameter Amount = new SqlParameter("@Amount", finishedProduct.Количество);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Finished_Production @Title, @Unit, @Summa, @Amount", Title, Unit, Summa, Amount);

                    return RedirectToAction(nameof(Index));
                }

                ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
                return View(finishedProduct);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: ГотоваяПродукция/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SqlParameter Id = new SqlParameter("@Id", id);
            var finishedProduct = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @Id", Id).ToListAsync();

            //var finishedProduct = await _context.FinishedProducts.FindAsync(id);
            if (finishedProduct.FirstOrDefault() == null)
            {
                return NotFound();
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View(finishedProduct.FirstOrDefault());
        }

        // POST: ГотоваяПродукция/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] ГотоваяПродукция finishedProduct)
        {
            if (id != finishedProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    SqlParameter Id = new SqlParameter("@Id", finishedProduct.Id);
                    SqlParameter Title = new SqlParameter("@Title", finishedProduct.Наименование);
                    SqlParameter Unit = new SqlParameter("@Unit", finishedProduct.ЕдиницаИзмерения);
                    SqlParameter Summa = new SqlParameter("@Summa", finishedProduct.Сумма);
                    SqlParameter Amount = new SqlParameter("@Amount", finishedProduct.Количество);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Finished_Production @Id, @Title, @Unit, @Summa, @Amount", Id, Title, Unit, Summa, Amount);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ГотоваяПродукцияExists(finishedProduct.Id))
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
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View(finishedProduct);
        }

        // GET: ГотоваяПродукция/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlParameter Id = new SqlParameter("@Id", id);
            var finishedProduct = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @Id", Id).ToListAsync();
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");


            return View(finishedProduct.FirstOrDefault());
        }

        // POST: ГотоваяПродукция/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter ID = new SqlParameter("@id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Finished_Production @id", ID);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ГотоваяПродукцияExists(byte id)
        {
            return _context.ГотоваяПродукцияs.Any(e => e.Id == id);
        }
    }
}
