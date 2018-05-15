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
    public class NhaMangsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/NhaMangs
        public IQueryable<NhaMang> GetNhaMangs()
        {
            return db.NhaMangs;
        }

        // GET: api/NhaMangs/5
        [ResponseType(typeof(NhaMang))]
        public IHttpActionResult GetNhaMang(int id)
        {
            NhaMang nhaMang = db.NhaMangs.Find(id);
            if (nhaMang == null)
            {
                return NotFound();
            }

            return Ok(nhaMang);
        }

        // PUT: api/NhaMangs/5
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutNhaMang(int id, NhaMang nhaMang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nhaMang.IDNhaMang)
            {
                return BadRequest();
            }
            db.Entry(nhaMang).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaMangExists(id))
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

        // POST: api/NhaMangs
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(NhaMang))]
        public IHttpActionResult PostNhaMang(NhaMang nhaMang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.NhaMangs.Add(nhaMang);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nhaMang.IDNhaMang }, nhaMang);
        }

        // DELETE: api/NhaMangs/5
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(NhaMang))]
        public IHttpActionResult DeleteNhaMang(int id)
        {
            NhaMang nhaMang = db.NhaMangs.Find(id);
            if (nhaMang == null)
            {
                return NotFound();
            }

            db.NhaMangs.Remove(nhaMang);
            db.SaveChanges();

            return Ok(nhaMang);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NhaMangExists(int id)
        {
            return db.NhaMangs.Count(e => e.IDNhaMang == id) > 0;
        }
    }
}