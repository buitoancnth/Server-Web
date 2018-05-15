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
    public class TheLoaiAnhsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/TheLoaiAnhs
        public IQueryable<TheLoaiAnh> GetTheLoaiAnhs()
        {
            return db.TheLoaiAnhs;
        }

        // GET: api/TheLoaiAnhs/5
        [ResponseType(typeof(TheLoaiAnh))]
        public IHttpActionResult GetTheLoaiAnh(int id)
        {
            TheLoaiAnh theLoaiAnh = db.TheLoaiAnhs.Find(id);
            if (theLoaiAnh == null)
            {
                return NotFound();
            }

            return Ok(theLoaiAnh);
        }

        // PUT: api/TheLoaiAnhs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTheLoaiAnh(int id, TheLoaiAnh theLoaiAnh)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != theLoaiAnh.IDTheLoaiAnh)
            {
                return BadRequest();
            }

            db.Entry(theLoaiAnh).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheLoaiAnhExists(id))
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

        // POST: api/TheLoaiAnhs
        [ResponseType(typeof(TheLoaiAnh))]
        public IHttpActionResult PostTheLoaiAnh(TheLoaiAnh theLoaiAnh)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TheLoaiAnhs.Add(theLoaiAnh);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = theLoaiAnh.IDTheLoaiAnh }, theLoaiAnh);
        }

        // DELETE: api/TheLoaiAnhs/5
        [ResponseType(typeof(TheLoaiAnh))]
        public IHttpActionResult DeleteTheLoaiAnh(int id)
        {
            TheLoaiAnh theLoaiAnh = db.TheLoaiAnhs.Find(id);
            if (theLoaiAnh == null)
            {
                return NotFound();
            }

            db.TheLoaiAnhs.Remove(theLoaiAnh);
            db.SaveChanges();

            return Ok(theLoaiAnh);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TheLoaiAnhExists(int id)
        {
            return db.TheLoaiAnhs.Count(e => e.IDTheLoaiAnh == id) > 0;
        }
    }
}