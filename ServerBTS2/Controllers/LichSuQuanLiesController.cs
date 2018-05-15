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
    [Authorize(Roles = "Admin")]
    public class LichSuQuanLiesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/LichSuQuanLies
        public IEnumerable<LichSuQuanLy> GetLichSuQuanLies()
        {
            return db.LichSuQuanLies;
        }

        // GET: api/LichSuQuanLies/5
        [ResponseType(typeof(LichSuQuanLy))]
        public IHttpActionResult GetLichSuQuanLy(int id)
        {
            LichSuQuanLy lichSuQuanLy = db.LichSuQuanLies.Find(id);
            if (lichSuQuanLy == null)
            {
                return NotFound();
            }

            return Ok(lichSuQuanLy);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LichSuQuanLyExists(int id)
        {
            return db.LichSuQuanLies.Count(e => e.IDLichSu == id) > 0;
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
    }
}