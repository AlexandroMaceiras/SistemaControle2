using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SistemaControle.Models;

namespace SistemaControle.Controllers.MVC
{
    [Authorize]
    public class NotasController : Controller
    {
        private ControleContext db = new ControleContext();

        // GET: Notas
        public ActionResult Index()
        {
            var notas = db.Notas.Include(n => n.GruposDetalhes);
            return View(notas.ToList());
        }

        // GET: Notas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notas notas = db.Notas.Find(id);
            if (notas == null)
            {
                return HttpNotFound();
            }
            return View(notas);
        }

        // GET: Notas/Create
        public ActionResult Create()
        {
            ViewBag.GruposDetalhesId = new SelectList(db.GruposDetalhes, "GruposDetalhesId", "GruposDetalhesId");
            return View();
        }

        // POST: Notas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NotaId,GruposDetalhesId,Percentual,Nota")] Notas notas)
        {
            if (ModelState.IsValid)
            {
                db.Notas.Add(notas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GruposDetalhesId = new SelectList(db.GruposDetalhes, "GruposDetalhesId", "GruposDetalhesId", notas.GruposDetalhesId);
            return View(notas);
        }

        // GET: Notas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notas notas = db.Notas.Find(id);
            if (notas == null)
            {
                return HttpNotFound();
            }
            ViewBag.GruposDetalhesId = new SelectList(db.GruposDetalhes, "GruposDetalhesId", "GruposDetalhesId", notas.GruposDetalhesId);
            return View(notas);
        }

        // POST: Notas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NotaId,GruposDetalhesId,Percentual,Nota")] Notas notas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GruposDetalhesId = new SelectList(db.GruposDetalhes, "GruposDetalhesId", "GruposDetalhesId", notas.GruposDetalhesId);
            return View(notas);
        }

        // GET: Notas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notas notas = db.Notas.Find(id);
            if (notas == null)
            {
                return HttpNotFound();
            }
            return View(notas);
        }

        // POST: Notas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notas notas = db.Notas.Find(id);
            db.Notas.Remove(notas);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
