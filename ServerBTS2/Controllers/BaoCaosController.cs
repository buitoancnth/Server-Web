using ServerBTS2.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class BaoCaosController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //GET: api/BaoCaos
        public IEnumerable<BaoCao> GetAll()
        {
            if (isAdmin())
                return db.BaoCaos;
            else
            {
                string id = User.Identity.GetUserId();
                return db.BaoCaos.Where(u => u.IDQuanLy ==id ).ToList();
            }
                
        }
        //GET api/BaoCaos/5
        [ResponseType(typeof(BaoCao))]
        public IHttpActionResult GetByID(int id)
        {
            BaoCao baoCao = db.BaoCaos.Find(id);
            if (baoCao == null)
            {
                return NotFound();
            }
            if (!isAccess(baoCao.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);
            return Ok(baoCao);
        }
        //POST: api/BaoCaos
        [ResponseType(typeof(BaoCao))]
        [Authorize(Roles = "QuanLy")]
        public IHttpActionResult PostBaoCao(BaoCao baoCao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            baoCao.IDQuanLy = User.Identity.GetUserId();
            baoCao.ThoiGian = getDateTime();
            db.BaoCaos.Add(baoCao);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = baoCao.IDBaoCao }, baoCao);
        }
        //DELETE: api/BaoCaos/5
        [ResponseType(typeof(BaoCao))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteBaoCao(int id)
        {
            BaoCao baoCao = db.BaoCaos.Find(id);
            if (baoCao == null)
            {
                return NotFound();
            }
            db.BaoCaos.Remove(baoCao);
            db.SaveChanges();
            return Ok(baoCao);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool BaoCaoExist(int id)
        {
            return db.BaoCaos.Count(e => e.IDBaoCao == id) > 0;
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