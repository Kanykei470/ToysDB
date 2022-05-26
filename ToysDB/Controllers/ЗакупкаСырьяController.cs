using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToysDB.Models;

namespace ToysDB.Controllers
{
    public class ЗакупкаСырьяController : Controller
    {
        private readonly ToysContext _context;
        private readonly INotyfService _notyf;

        public ЗакупкаСырьяController(ToysContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: ЗакупкаСырья
        public async Task<IActionResult> Index()
        {
            var employeers = await _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToListAsync();
            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.Get_Raw_Materials").ToListAsync();
            var prm = await _context.ЗакупкаСырьяs.FromSqlRaw("dbo.Get_Purchase_Of_Raw_Materials").ToListAsync();

            foreach (var prms in prm)
            {
                foreach (var emp in employeers)
                {
                    if (prms.Сотрудник == emp.Id)
                    {
                        prms.СотрудникNavigation.Фио = emp.Фио;
                    }
                }
                foreach (var raw in rawMaterials)
                {
                    if (prms.Сырьё == raw.Id)
                    {
                        prms.СырьёNavigation.Наименование = raw.Наименование;
                    }
                }
            }

            return View(prm);
        }

        // GET: ЗакупкаСырья/Details/5
        public async Task<IActionResult> Details(byte? id)
        {
            SqlParameter ID = new SqlParameter("@Id", id);
            var purchaseOfRaw_Materials = await _context.ЗакупкаСырьяs.FromSqlRaw("dbo.GetID_Purchase_Of_Raw_Materials @id", ID).ToListAsync();
            SqlParameter emp = new SqlParameter("@Id", purchaseOfRaw_Materials.FirstOrDefault().Сотрудник);
            var empID = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", emp).ToListAsync();
            SqlParameter rawMaterials = new SqlParameter("@Id", purchaseOfRaw_Materials.FirstOrDefault().Сырьё);
            var rmID = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @id", rawMaterials).ToListAsync();

            if ((purchaseOfRaw_Materials.FirstOrDefault().Сотрудник == empID.FirstOrDefault().Id) &&
                (purchaseOfRaw_Materials.FirstOrDefault().Сырьё == rmID.FirstOrDefault().Id)) 
            { 
                purchaseOfRaw_Materials.FirstOrDefault().СотрудникNavigation.Фио= empID.FirstOrDefault().Фио;
                purchaseOfRaw_Materials.FirstOrDefault().СырьёNavigation.Наименование = rmID.FirstOrDefault().Наименование;
            }
        //Сотрудник и сырье
            if (ID == null)
            {
                return NotFound();
            }
            if (purchaseOfRaw_Materials == null)
            {
                return NotFound();
            }
            return View(purchaseOfRaw_Materials.FirstOrDefault());
        }

        // GET: ЗакупкаСырья/Create
        public IActionResult Create()
        {
            var rawList = _context.Сырьёs.FromSqlRaw("dbo.Get_Raw_Materials").ToList();
            var workerList = _context.Сотрудникиs.FromSqlRaw("dbo.Get_Employeers").ToList();
            ViewData["Сырьё"] = new SelectList(rawList, "Id", "Наименование");
            ViewData["Сотрудник"] = new SelectList(workerList, "Id", "Фио");
            return View();
        }

        // POST: ЗакупкаСырья/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Сырьё,Количество,Сумма,Дата,Сотрудник")] ЗакупкаСырья rawMaterialPurchase)
        {
            if (ModelState.IsValid)
            {
                var parameterReturn = new SqlParameter
                {
                    ParameterName = "p",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output,
                };

                SqlParameter SumCheck = new SqlParameter("@sum", rawMaterialPurchase.Сумма);
                _context.Database.ExecuteSqlRaw("exec  @p =  dbo.[SP_Purchase]  @sum", SumCheck, parameterReturn);
                int returnValue = (int)parameterReturn.Value;

                if (returnValue == 0)
                {
                    SqlParameter RawMaterial = new SqlParameter("@rawMaterial", rawMaterialPurchase.Сырьё);
                    SqlParameter Quantity = new SqlParameter("@quantity", rawMaterialPurchase.Количество);
                    SqlParameter Sum = new SqlParameter("@sum", rawMaterialPurchase.Сумма);
                    SqlParameter Date = new SqlParameter("@date", rawMaterialPurchase.Дата);
                    SqlParameter Worker = new SqlParameter("@worker", rawMaterialPurchase.Сотрудник);
                    await _context.Database.ExecuteSqlRawAsync("exec dbo.Insert_Purchase_Of_Raw_Materials @rawMaterial, @quantity, @sum, @date, @worker", RawMaterial, Quantity, Sum, Date, Worker);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Сумма", "Недостаточно бюджета!");
                }
            }
            ViewData["RawMaterial"] = new SelectList(_context.Сырьёs, "Id", "Name", rawMaterialPurchase.Сырьё);
            ViewData["Worker"] = new SelectList(_context.Сотрудникиs, "Id", "Fio", rawMaterialPurchase.Сотрудник);
            return View(rawMaterialPurchase);
        }

        // GET: ЗакупкаСырья/Edit/5
        public async Task<IActionResult> Edit(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var закупкаСырья = await _context.ЗакупкаСырьяs.FindAsync(id);
            if (закупкаСырья == null)
            {
                return NotFound();
            }
            ViewData["Сотрудник"] = new SelectList(_context.Сотрудникиs, "Id", "Фио", закупкаСырья.Сотрудник);
            ViewData["Сырьё"] = new SelectList(_context.Сырьёs, "Id", "Наименование", закупкаСырья.Сырьё);
            return View(закупкаСырья);

        }

        // POST: ЗакупкаСырья/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Сырьё,Количество,Сумма,Дата,Сотрудник")] ЗакупкаСырья rawMaterialPurchase)
        {
            if (id != rawMaterialPurchase.Id)
            {
                return NotFound();
            }
            var parameterReturn = new SqlParameter
            {
                ParameterName = "p",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output,
            };
            SqlParameter Id = new SqlParameter("@Id", rawMaterialPurchase.Id);
            SqlParameter Sum = new SqlParameter("@sum", rawMaterialPurchase.Сумма);
            _context.Database.ExecuteSqlRaw("exec @p = dbo.[updateCheck_rawMaterialPurchase] @Id, @Sum", Id, Sum, parameterReturn);
            int returnValue = (int)parameterReturn.Value;
            if (returnValue == 0)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        SqlParameter Id1 = new SqlParameter("@Id", rawMaterialPurchase.Id);
                        SqlParameter RawMaterial = new SqlParameter("@RawMaterial", rawMaterialPurchase.Сырьё);
                        SqlParameter Quantity = new SqlParameter("@quantity", rawMaterialPurchase.Количество);
                        SqlParameter Sum1 = new SqlParameter("@sum", rawMaterialPurchase.Сумма);
                        SqlParameter Date = new SqlParameter("@date", rawMaterialPurchase.Дата);
                        SqlParameter Worker = new SqlParameter("@worker", rawMaterialPurchase.Сотрудник);
                        await _context.Database.ExecuteSqlRawAsync("exec dbo.Update_Purchase_Of_Raw_Materials @Id,@RawMaterial,@Quantity, @Sum, @Date,@Worker", Id1, RawMaterial, Quantity, Sum1, Date, Worker);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ЗакупкаСырьяExists(rawMaterialPurchase.Id))
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
                ModelState.AddModelError("Сумма", "Недостаточно бюджета!");
            }
            ViewData["RawMaterial"] = new SelectList(_context.Сырьёs, "Id", "Name", rawMaterialPurchase.Сырьё);
            ViewData["Worker"] = new SelectList(_context.Сотрудникиs, "Id", "Fio", rawMaterialPurchase.Сотрудник);
            return View(rawMaterialPurchase);
        }


        // GET: ЗакупкаСырья/Delete/5
        public async Task<IActionResult> Delete(byte? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SqlParameter ID = new SqlParameter("@Id", id);
            var rawMaterialPurchase = await _context.ЗакупкаСырьяs.FromSqlRaw("dbo.GetID_Purchase_Of_Raw_Materials @id", ID).ToListAsync();
            SqlParameter workerID = new SqlParameter("@Id", rawMaterialPurchase.FirstOrDefault().Сотрудник);
            var worker = await _context.Сотрудникиs.FromSqlRaw("dbo.GetID_Employeers @id", workerID).ToListAsync();
            SqlParameter rawID = new SqlParameter("@Id", rawMaterialPurchase.FirstOrDefault().Сырьё);
            var rawMaterials = await _context.Сырьёs.FromSqlRaw("dbo.GetID_Raw_Materials @id", rawID).ToListAsync();

            if (rawMaterialPurchase.FirstOrDefault().Сотрудник == worker.FirstOrDefault().Id)
                rawMaterialPurchase.FirstOrDefault().СотрудникNavigation.Фио = worker.FirstOrDefault().Фио;
            if (rawMaterialPurchase.FirstOrDefault().Сырьё == rawMaterials.FirstOrDefault().Id)
                rawMaterialPurchase.FirstOrDefault().СырьёNavigation.Наименование = rawMaterials.FirstOrDefault().Наименование;
            if (rawMaterialPurchase.FirstOrDefault() == null)
            {
                return NotFound();
            }

            return View(rawMaterialPurchase.FirstOrDefault());
        }

        // POST: ЗакупкаСырья/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(byte id)
        {
            SqlParameter ID = new SqlParameter("@id", id);
            await _context.Database.ExecuteSqlRawAsync("exec dbo.Delete_Purchase_Of_Raw_Materials @id", ID);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ЗакупкаСырьяExists(byte id)
        {
            return _context.ЗакупкаСырьяs.Any(e => e.Id == id);
        }
    }
}
