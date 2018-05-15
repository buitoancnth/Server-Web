using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ServerBTS2.Models;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class HinhAnhsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/HinhAnhs
        public IQueryable<HinhAnh> GetHinhAnhs()
        {
            return db.HinhAnhs;
        }

        // GET: api/HinhAnhs/5
        [ResponseType(typeof(HinhAnh))]
        public IHttpActionResult GetHinhAnh(int id)
        {
            HinhAnh hinhAnh = db.HinhAnhs.Find(id);
            if (hinhAnh == null)
            {
                return NotFound();
            }

            return Ok(hinhAnh);
        }

        // PUT: api/HinhAnhs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutHinhAnh(int id, HinhAnh hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hinhAnh.IDHinhAnh)
            {
                return BadRequest();
            }

            db.Entry(hinhAnh).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HinhAnhExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/HinhAnhs
        [ResponseType(typeof(HinhAnh))]
        public IHttpActionResult PostHinhAnh(HinhAnh hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.HinhAnhs.Add(hinhAnh);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = hinhAnh.IDHinhAnh }, hinhAnh);
        }

        // DELETE: api/HinhAnhs/5
        [ResponseType(typeof(HinhAnh))]
        public IHttpActionResult DeleteHinhAnh(int id)
        {
            HinhAnh hinhAnh = db.HinhAnhs.Find(id);
            if (hinhAnh == null)
            {
                return NotFound();
            }

            db.HinhAnhs.Remove(hinhAnh);
            db.SaveChanges();

            return Ok(hinhAnh);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HinhAnhExists(int id)
        {
            return db.HinhAnhs.Count(e => e.IDHinhAnh == id) > 0;
        }
    }
}