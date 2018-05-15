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
using System.Globalization;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class MatDiensController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private double TIENPHAT = 50000;

        // GET: api/MatDiens
        public IEnumerable<MatDien> GetMatDiens()
        {
            if (isAdmin())
                return db.MatDiens;
            else
            {
                string id = User.Identity.GetUserId();
                var tram = db.Trams.Where(u => u.IDQuanLy == id);
                //var nhatram = db.NhaTrams.Where(u => tram.Count(p => p.IDTram == u.IDTram) > 0);
                return db.MatDiens.Where(u => tram.Count(p => p.IDTram == u.IDTram) > 0).ToList();

            }
        }


        // GET: api/MatDiens/5
        [ResponseType(typeof(MatDien))]
        [Route("api/MatDienByIDTram")]
        public IHttpActionResult GetMatDienByIDTram(int id)
        {
            var tmpTram = db.Trams.SingleOrDefault(u => u.IDTram == id);
            if (tmpTram == null)
            {
                return NotFound();
            }
            if (!isAccess(tmpTram.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);
            var listMatDien = db.MatDiens.Where(u => u.IDTram == id);
            return Ok(listMatDien);
        }

        // GET: api/MatDiens/3
        [ResponseType(typeof(MatDien))]
        public IHttpActionResult GetMatDien(int id)
        {
            MatDien matDien = db.MatDiens.Find(id);
            if (matDien == null)
            {
                return NotFound();
            }
            //var tmpNhaTram = db.NhaTrams.SingleOrDefault(u => u.IDNhaTram == matDien.IDNhaTram);
            var tmpTram = db.Trams.SingleOrDefault(u => u.IDTram == matDien.IDTram);
            if (!isAccess(tmpTram.IDQuanLy))
                return StatusCode(HttpStatusCode.Unauthorized);
            return Ok(matDien);
        }

        // POST: api/MatDiens
        [Authorize(Roles = "QuanLy")]
        [ResponseType(typeof(MatDien))]
        public IHttpActionResult PostMatDien(MatDien matDien)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var tmpNhaTram = db.NhaTrams.SingleOrDefault(u => u.IDNhaTram == matDien.IDNhaTram);
            //if (tmpNhaTram == null) return NotFound();
            var tmpTram = db.Trams.SingleOrDefault(u => u.IDTram == matDien.IDTram);
            if (tmpTram == null) return NotFound();
            if (!isAccess(tmpTram.IDQuanLy)) return StatusCode(HttpStatusCode.NotFound);

            DateTime UTCNow = DateTime.UtcNow.AddHours(7);
            DateTime thoiGianMatDien = matDien.NgayMatDien.AddHours(matDien.GioMatDien.Hours)
                .AddMinutes(matDien.GioMatDien.Minutes);
            if (thoiGianMatDien.Subtract(matDien.ThoiGianMayNo).TotalMinutes > 0) return BadRequest();
            if (matDien.ThoiGianMayNo.Subtract(matDien.ThoiGianNgung).TotalMinutes > 0) return BadRequest();
            if (matDien.ThoiGianNgung.Subtract(UTCNow).TotalMinutes > 0) return BadRequest();

            matDien.TongThoiGianChay = matDien.ThoiGianNgung.Subtract(matDien.ThoiGianMayNo).ToString();

            TimeSpan thoiGianTre = matDien.ThoiGianMayNo.Subtract(thoiGianMatDien);


            TimeSpan thoiGianChamUngCuu;
            if (matDien.QuangDuongDiChuyen <= 20)
                thoiGianChamUngCuu = DateTime.ParseExact("01:00", "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            else
                thoiGianChamUngCuu = DateTime.ParseExact("02:00", "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;

            if (thoiGianTre.Subtract(thoiGianChamUngCuu).TotalMinutes < 0)
                matDien.TienPhat = 0;
            else
                matDien.TienPhat = TIENPHAT;
            db.MatDiens.Add(matDien);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = matDien.IDMatDien }, matDien);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatDienExists(int id)
        {
            return db.MatDiens.Count(e => e.IDMatDien == id) > 0;
        }
        private bool TramExists(int id)
        {
            return db.Trams.Count(e => e.IDTram == id) > 0;
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