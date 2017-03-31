﻿using NLog;
using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaxNetXML.Models;

namespace TaxNetXML.Controllers {
    public class HomeController : Controller {
        private FileContext db = new FileContext();
        private static bool started = false;
        public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        //
        // Summary:
        //     Получает из БД список данных и отправляет его в представление пользователю.
        public async Task<ActionResult> Index() {
            if (!started) {
                _logger.Info("Application start");
                started = true;
            }

            return View(await db.Files.ToListAsync());
        }

        // GET: BackupDB/Create
        public ActionResult Create() {
            return View();
        }

        // POST: BackupDB/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,FileVersion,DateTime")] File file) {
            if (ModelState.IsValid) {
                db.Files.Add(file);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(file);
        }

        // GET: BackupDB/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                _logger.Debug("Method Edit, id = null, HttpStatusCode.BadRequest");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = await db.Files.FindAsync(id);
            if (file == null) {
                _logger.Debug("Method Edit, file = null, HttpNotFound");
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: BackupDB/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,FileVersion,DateTime")] File file) {
            if (ModelState.IsValid) {
                db.Entry(file).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(file);
        }

        // GET: BackupDB/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                _logger.Debug("Method Details, id = null, HttpStatusCode.BadRequest");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = await db.Files.FindAsync(id);
            if (file == null) {
                _logger.Debug("Method Details, file = null, HttpNotFound");
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: BackupDB/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                _logger.Debug("Method Delete, id = null, HttpStatusCode.BadRequest");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = await db.Files.FindAsync(id);
            if (file == null) {
                _logger.Debug("Method Delete, file = null, HttpNotFound");
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: BackupDB/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            File file = await db.Files.FindAsync(id);
            db.Files.Remove(file);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}