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
using Microsoft.AspNet.Identity;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class NhatKiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/NhatKies
        public IEnumerable<NhatKy> GetNhatKies()
        {
            if (isAdmin())
                return db.NhatKies;
            else
            {
                string id = User.Identity.GetUserId();
                return db.NhatKies.Where(u => u.IDQuanLy == id).ToList();
            }

        }

        // GET: api/NhatKies/5
        [ResponseType(typeof(NhatKy))]
        public IHttpActionResult GetNhatKy(int id)
        {
            NhatKy nhatKy = db.NhatKies.Find(id);
            
            if (nhatKy == null)
            {
                return NotFound();
            }
            if (!isAccess(nhatKy.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);

            return Ok(nhatKy);
        }

        // PUT: api/NhatKies/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "QuanLy")]
        public IHttpActionResult PutNhatKy(int id, NhatKy nhatKy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != nhatKy.IDNhatKy)
            {
                return BadRequest();
            }

            NhatKy nhatKyBefore = db.NhatKies.Find(id);
            if (nhatKyBefore == null) return NotFound();

            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == nhatKyBefore.IDTram);
            if (!isAccess(tmp.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);

            nhatKyBefore.IDQuanLy = User.Identity.GetUserId();
            nhatKyBefore.NoiDung = nhatKy.NoiDung;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhatKyExists(id))
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

        // POST: api/NhatKies
        [ResponseType(typeof(NhatKy))]
        [Authorize(Roles = "QuanLy")]
        public IHttpActionResult PostNhatKy(NhatKy nhatKy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == nhatKy.IDTram);
            if (tmp == null) return NotFound();
            if (!isAccess(tmp.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);
            nhatKy.IDQuanLy = User.Identity.GetUserId();
            nhatKy.ThoiGian = getDateTime();

            db.NhatKies.Add(nhatKy);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = nhatKy.IDNhatKy }, nhatKy);
        }

        // DELETE: api/NhatKies/5
        [ResponseType(typeof(NhatKy))]
        public IHttpActionResult DeleteNhatKy(int id)
        {
            NhatKy nhatKy = db.NhatKies.Find(id);
            
            if (nhatKy == null)
            {
                return NotFound();
            }
            if (!isAccess(nhatKy.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == nhatKy.IDTram);
            if (!isAccess(tmp.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);

            db.NhatKies.Remove(nhatKy);
            db.SaveChanges();

            return Ok(nhatKy);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NhatKyExists(int id)
        {
            return db.NhatKies.Count(e => e.IDNhatKy == id) > 0;
        }
        private bool isAdmin()
        {
            var idUser = User.Identity.GetUserId();
            var role = db.UserBTSs.SingleOrDefault(u => u.IDUser == idUser).ChucVu;
            return role.Equals("Admin");
        }
        private bool isAccess(string idQuanLy)
        {
            if (isAdmin()) return true;
            return idQuanLy.Equals(User.Identity.GetUserId());
        }
        private DateTime getDateTime()
        {
            DateTime UTCNow = DateTime.UtcNow.AddHours(7);
            return UTCNow;
        }
    }
}