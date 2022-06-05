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
                SqlParameter Product = new SqlParameter("@product", продажаПродукции.Продукция);
                SqlParameter Amount = new SqlParameter("@amount", продажаПродукции.Количество);
                SqlParameter Sum = new SqlParameter("@sum", продажаПродукции.Сумма);
                SqlParameter Date = new SqlParameter("@date", продажаПродукции.Дата);
                SqlParameter Emp = new SqlParameter("@emp", продажаПродукции.Сотрудник);
                var outParam = new SqlParameter
                {
                    ParameterName = "@r",
                    DbType = System.Data.DbType.Int32,
                    Size = 100,
                    Direction = System.Data.ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Sale_Of_Products @product, @amount, @sum, @date, @emp, @r OUT", Product, Amount, Sum, Date, Emp, outParam);

                if (outParam.SqlValue.ToString() == "0")
                {
                    ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
                }
                else if (outParam.SqlValue.ToString() == "2")
                {
                    ModelState.AddModelError("Сумма", "Сумма не может быть ниже себестоимости!");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
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

            SqlParameter Id = new SqlParameter("@id", id);
            var продажаПродукции = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.GetID_Sale_Of_Products @id", Id).ToListAsync();
            if (продажаПродукции == null)
            {
                return NotFound();
            }
            ViewData["Продукция"] = new SelectList(_context.ГотоваяПродукцияs, "Id", "Наименование", продажаПродукции.FirstOrDefault().Продукция);
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", продажаПродукции.FirstOrDefault().Сотрудник);
            return View(продажаПродукции.FirstOrDefault());
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

            if (ModelState.IsValid)
            {
                SqlParameter Id = new SqlParameter("@id", id);
                var curSale = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.GetID_Sale_Of_Products @id", Id).ToListAsync();

                SqlParameter Product = new SqlParameter("@product", продажаПродукции.Продукция);
                SqlParameter Amount = new SqlParameter("@amount", продажаПродукции.Количество);
                SqlParameter Sum = new SqlParameter("@sum", продажаПродукции.Сумма);
                SqlParameter Date = new SqlParameter("@date", продажаПродукции.Дата);
                SqlParameter Emp = new SqlParameter("@emp", продажаПродукции.Сотрудник);
                SqlParameter CurAmount = new SqlParameter("@curamount", curSale.FirstOrDefault().Количество);
                var outParam = new SqlParameter
                {
                    ParameterName = "@r",
                    DbType = System.Data.DbType.Int32,
                    Size = 100,
                    Direction = System.Data.ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Sale_Of_Products @id, @product, @amount, @sum, @date, @emp, @curamount, @r OUT", Id, Product, Amount, Sum, Date, Emp, CurAmount, outParam);

                if (outParam.SqlValue.ToString() == "0")
                {
                    ModelState.AddModelError("Количество", "Недостаточно готовой продукции!");
                }
                else if (outParam.SqlValue.ToString() == "2")
                {
                    ModelState.AddModelError("Сумма", "Сумма не может быть ниже себестоимости!");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
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

            SqlParameter Id = new SqlParameter("@id", id);

            var продажаПродукции = await _context.ПродажаПродукцииs.FromSqlRaw("dbo.GetID_Sale_Of_Products @id", Id).ToListAsync();

            SqlParameter prodId = new SqlParameter("@id", продажаПродукции.FirstOrDefault().Продукция);

            var prodList = await _context.ГотоваяПродукцияs.FromSqlRaw("dbo.GetID_Finished_Production @id", prodId).ToListAsync();

            SqlParameter empId = new SqlParameter("@id", продажаПродукции.FirstOrDefault().Сотрудник);

            var empList = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", empId).ToListAsync();

            if (продажаПродукции.FirstOrDefault().Продукция == prodList.FirstOrDefault().Id)
                продажаПродукции.FirstOrDefault().ПродукцияNavigation.Наименование = prodList.FirstOrDefault().Наименование;
            if (продажаПродукции.FirstOrDefault().Сотрудник == empList.FirstOrDefault().Id)
                продажаПродукции.FirstOrDefault().СотрудникNavigation.Фио = empList.FirstOrDefault().Фио;

            if (продажаПродукции == null)
            {
                return NotFound();
            }

            return View(продажаПродукции.FirstOrDefault());
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
