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
    public class СырьёController : Controller
    {
        private readonly ToysContext _context;

        public СырьёController(ToysContext context)
        {
            _context = context;
        }

        // GET: Сырьё
        public async Task<IActionResult> Index()
        {
            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.Get_Raw_Materials").ToListAsync();
            var units = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.Get_Units").ToListAsync();
            foreach (var rm in rawMaterials)
            {
                foreach (var u in units)
                {
                    if (rm.ЕдиницаИзмерения == u.Id)
                    {
                        rm.ЕдиницаИзмеренияNavigation.Наименование = u.Наименование;
                    }
                }

            }
            return View(rawMaterials);
        }

        // GET: Сырьё/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            //Единицы и
            SqlParameter ID = new SqlParameter("@Id", id);
            var rawM = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @id", ID).ToListAsync();
            SqlParameter unitID = new SqlParameter("@Id", rawM.FirstOrDefault().ЕдиницаИзмерения);
            var unit = await _context.ЕдиницыИзмеренияs.FromSqlRaw("dbo.GetID_Units @id", unitID).ToListAsync();

            if (rawM.FirstOrDefault().ЕдиницаИзмерения == unit.FirstOrDefault().Id)
                rawM.FirstOrDefault().ЕдиницаИзмеренияNavigation.Наименование = unit.FirstOrDefault().Наименование;

            if (rawM == null)
            {
                return NotFound();
            }

            return View(rawM.FirstOrDefault());
        }

        // GET: Сырьё/Create
        public IActionResult Create()
        {
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование");
            return View();
        }

        // POST: Сырьё/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] Сырьё rawMaterial)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlParameter Title = new SqlParameter("@Title", rawMaterial.Наименование);
                    SqlParameter Unit = new SqlParameter("@Unit", rawMaterial.ЕдиницаИзмерения);
                    SqlParameter Summa = new SqlParameter("@Summa", rawMaterial.Сумма);
                    SqlParameter Amount = new SqlParameter("@Amount", rawMaterial.Количество);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Raw_Materials @Title, @Unit, @Summa, @Amount", Title, Unit, Summa, Amount);

                    return RedirectToAction(nameof(Index));
                }
                ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", rawMaterial.ЕдиницаИзмерения);

                return View(rawMaterial);
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: Сырьё/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            SqlParameter Id = new SqlParameter("@Id", id);
            var dataRawMaterial = await _context.Сырьёs.FromSqlRaw("dbo.selectByIdRawMaterial @Id", Id).ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            if (dataRawMaterial.FirstOrDefault() == null)
            {
                return NotFound();
            }
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Наименование", dataRawMaterial.FirstOrDefault().ЕдиницаИзмерения);
            return View(dataRawMaterial);
        }

        // POST: Сырьё/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Наименование,ЕдиницаИзмерения,Сумма,Количество")] Сырьё сырьё)
        {
            if (id != сырьё.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(сырьё);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!СырьёExists(сырьё.Id))
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
            ViewData["ЕдиницаИзмерения"] = new SelectList(_context.ЕдиницыИзмеренияs, "Id", "Id", сырьё.ЕдиницаИзмерения);
            return View(сырьё);
        }

        // GET: Сырьё/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
           //???

            return View();
        }

        // POST: Сырьё/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            var сырьё = await _context.Сырьёs.FindAsync(id);
            _context.Сырьёs.Remove(сырьё);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool СырьёExists(byte id)
        {
            return _context.Сырьёs.Any(e => e.Id == id);
        }
    }
}
