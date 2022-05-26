using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.Data.SqlClient;

namespace ToysDB.Controllers
{
    public class ПродажаПродукцииController : Controller
    {
        private readonly ToysContext _context;
        private readonly INotyfService _notyf;

        public ПродажаПродукцииController(ToysContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: ПродажаПродукции
        public async Task<IActionResult> Index()
        {
            var employeers = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();
            var finishedproduction = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.Get_Finished_Production").ToListAsync();
            var saleOfProducts = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.Get_Sale_Of_Products").ToListAsync();

            foreach (var sp in saleOfProducts)
            {
                foreach (var emp in employeers)
                {
                    if (sp.Сотрудник == emp.Id)
                    {
                        sp.СотрудникNavigation.Фио = emp.Фио;
                    }
                }
                foreach (var fp in finishedproduction)
                {
                    if (sp.Продукция == fp.Id)
                    {
                        sp.ПродукцияNavigation.Наименование = fp.Наименование;
                    }
                }
            }

            return View(saleOfProducts);
        }

        // GET: ПродажаПродукции/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            //продукция и Сотрудник
            SqlParameter ID = new SqlParameter("@Id", id);
            var saleOfProducts = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.GetID_Sale_Of_Products @id", ID).ToListAsync();
            SqlParameter emp = new SqlParameter("@Id", saleOfProducts.FirstOrDefault().Сотрудник);
            var empID = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", emp).ToListAsync();
            SqlParameter production = new SqlParameter("@Id", saleOfProducts.FirstOrDefault().Продукция);
            var pID = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", production).ToListAsync();

            if ((saleOfProducts.FirstOrDefault().Сотрудник == empID.FirstOrDefault().Id) &&
                (saleOfProducts.FirstOrDefault().Продукция == pID.FirstOrDefault().Id))
            {
                saleOfProducts.FirstOrDefault().СотрудникNavigation.Фио = empID.FirstOrDefault().Фио;
                saleOfProducts.FirstOrDefault().ПродукцияNavigation.Наименование = pID.FirstOrDefault().Наименование;
            }
            if (ID == null)
            {
                return NotFound();
            }
            if (saleOfProducts == null)
            {
                return NotFound();
            }
            return View(saleOfProducts.FirstOrDefault());

        }

        // GET: ПродажаПродукции/Create
        public IActionResult Create()
        {
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование");
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио");
            return View();
        }

        // POST: ПродажаПродукции/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Продукция,Количество,Сумма,Дата,Сотрудник")] ПродажаПродукции продажаПродукции)
        {
            if (ModelState.IsValid)
            {
                var parameterReturn = new SqlParameter
                {
                    ParameterName = "p",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output,
                };

                SqlParameter Id = new SqlParameter("@id", продажаПродукции.Id);
                SqlParameter SumCheck = new SqlParameter("@quantity", продажаПродукции.Количество);
                _context.Database.ExecuteSqlRaw("exec @p=dbo.[SP_SaleProds] @id, @quantity", Id, SumCheck, parameterReturn);
                int returnValue = (int)parameterReturn.Value;

                if (returnValue == 0)
                {
                    SqlParameter RawMaterial = new SqlParameter("@rawMaterial", продажаПродукции.Продукция);
                    SqlParameter Quantity = new SqlParameter("@quantity", продажаПродукции.Количество);
                    SqlParameter Sum = new SqlParameter("@sum", продажаПродукции.Сумма);
                    SqlParameter Date = new SqlParameter("@date", продажаПродукции.Дата);
                    SqlParameter Worker = new SqlParameter("@worker", продажаПродукции.Сотрудник);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Sale_Of_Products @rawMaterial, @quantity, @sum, @date, @worker", RawMaterial, Quantity, Sum, Date, Worker);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
                }
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
            return View(продажаПродукции);

        }

        // GET: ПродажаПродукции/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var продажаПродукции = await _context.ПродажаПродукцииs.FindAsync(id);
            if (продажаПродукции == null)
            {
                return NotFound();
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
            return View(продажаПродукции);
        }

        // POST: ПродажаПродукции/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(byte id, [Bind("Id,Продукция,Количество,Сумма,Дата,Сотрудник")] ПродажаПродукции продажаПродукции)
        {


            if (id != продажаПродукции.Id)
            {
                return NotFound();
            }
            var parameterReturn = new SqlParameter
            {
                ParameterName = "p",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };
            SqlParameter Id = new SqlParameter("@Id", продажаПродукции.Id);
            SqlParameter Sum = new SqlParameter("@quantity", продажаПродукции.Количество);
            _context.Database.ExecuteSqlRaw("exec @p = dbo.[SP_SaleProds] @Id, @quantity", Id, Sum, parameterReturn);
            int returnValue = (int)parameterReturn.Value;
            if (returnValue == 0)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        SqlParameter Id1 = new SqlParameter("@Id", продажаПродукции.Id);
                        SqlParameter RawMaterial = new SqlParameter("@RawMaterial", продажаПродукции.Продукция);
                        SqlParameter Quantity = new SqlParameter("@quantity", продажаПродукции.Количество);
                        SqlParameter Sum1 = new SqlParameter("@sum", продажаПродукции.Сумма);
                        SqlParameter Date = new SqlParameter("@date", продажаПродукции.Дата);
                        SqlParameter Worker = new SqlParameter("@worker", продажаПродукции.Сотрудник);
                        await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Sale_Of_Products @Id,@RawMaterial,@Quantity, @Sum, @Date,@Worker", Id1, RawMaterial, Quantity, Sum1, Date, Worker);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ПродажаПродукцииExists(продажаПродукции.Id))
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
            }
            else if (returnValue == 1)
            {
                 ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
            }
            
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.Сотрудник);
            return View(продажаПродукции);

        }

        // GET: ПродажаПродукции/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlParameter ID = new SqlParameter("@Id", id);
            var production = await _context.ЗакупкаСырьяs.FromSqlRaw("dbo.GetID_Purchase_Of_Raw_Materials @id", ID).ToListAsync();
            SqlParameter workerID = new SqlParameter("@Id", production.FirstOrDefault().Сотрудник);
            var worker = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", workerID).ToListAsync();
            SqlParameter rawID = new SqlParameter("@Id", production.FirstOrDefault().Сырьё);
            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @id", rawID).ToListAsync();

            if (production.FirstOrDefault().Сотрудник == worker.FirstOrDefault().Id)
                production.FirstOrDefault().СотрудникNavigation.Фио = worker.FirstOrDefault().Фио;
            if (production.FirstOrDefault().Сырьё == rawMaterials.FirstOrDefault().Id)
                production.FirstOrDefault().СырьёNavigation.Наименование = rawMaterials.FirstOrDefault().Наименование;
            if (production.FirstOrDefault() == null)
            {
                return NotFound();
            }
            return View(production);
        }

        // POST: ПродажаПродукции/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter ID = new SqlParameter("@id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Sale_Of_Products @id", ID);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ПродажаПродукцииExists(byte id)
        {
            return _context.ПродажаПродукцииs.Any(e => e.Id == id);
        }

    }
}
