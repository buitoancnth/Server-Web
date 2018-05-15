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
using System.Text.RegularExpressions;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class TramsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Trams
        public IEnumerable<Tram> GetTrams()
        {
            if (isAdmin())
                return db.Trams;
            else
            {
                string id = User.Identity.GetUserId();
                return db.Trams.Where(u => u.IDQuanLy == id).ToList();
            }
        }

        // GET: api/Trams/id
        [ResponseType(typeof(Tram))]
        [Route("api/TramsByIdQuanLy")]
        public IEnumerable<Tram> GetTramsByIdQuanLy(String id)
        {

            return db.Trams.Where(u => u.IDQuanLy == id).ToList();

        }

        // GET: api/Trams/5
        [ResponseType(typeof(Tram))]
        public IHttpActionResult GetTram(int id)
        {
            Tram tram = db.Trams.Find(id);
            if (tram == null)
            {
                return NotFound();
            }
            if (!isAccess(tram.IDQuanLy)) return StatusCode(HttpStatusCode.Unauthorized);

            return Ok(tram);
        }

        // PUT: api/Trams/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTram(int id, Tram tram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != tram.IDTram)
            {
                return BadRequest();
            }
            Tram tramBefore = db.Trams.Find(id);
            if (tramBefore == null)
            {
                return NotFound();
            }
            if (isAdmin())
            {
                if (tram.CotTiepDat != tramBefore.CotTiepDat || tram.CotAnten != tramBefore.CotAnten ||
                    tram.SanTram != tramBefore.SanTram)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }


                var tmpUser = db.UserBTSs.Find(tram.IDQuanLy);
                if (tmpUser == null) return NotFound();

                Tram tmpTram1 = db.Trams.SingleOrDefault(u => u.KinhDo == tram.KinhDo && u.ViDo == tram.ViDo);
                if (tmpTram1 != null && tmpTram1.IDTram != tramBefore.IDTram) return BadRequest();

                tramBefore.BanKinhPhuSong = tram.BanKinhPhuSong;
                tramBefore.KinhDo = tram.KinhDo;
                tramBefore.ViDo = tram.ViDo;
                tramBefore.TenTram = tram.TenTram;
                tramBefore.Tinh = tram.Tinh;

                if (tramBefore.IDQuanLy != tram.IDQuanLy)
                {

                    LichSuQuanLy lsqlOld = db.LichSuQuanLies.SingleOrDefault(u => u.IDQuanLy == tramBefore.IDQuanLy && u.IDTram == id);
                    lsqlOld.ThoiGianKetThuc = getDate();
                    tramBefore.IDQuanLy = tram.IDQuanLy;
                    LichSuQuanLy lsqlNew = new LichSuQuanLy
                    {
                        IDQuanLy = tram.IDQuanLy,
                        IDTram = tram.IDTram,
                        ThoiGianLamViec = getDate()
                    };
                    db.LichSuQuanLies.Add(lsqlNew);
                }


                db.SaveChanges();


            }
            else
            {

                if (tram.TenTram != tramBefore.TenTram || tram.Tinh != tramBefore.Tinh ||
                    tram.KinhDo != tramBefore.KinhDo || tram.ViDo != tramBefore.ViDo ||
                    tram.IDQuanLy != tramBefore.IDQuanLy ||
                    tram.BanKinhPhuSong != tramBefore.BanKinhPhuSong)
                {
                    return StatusCode(HttpStatusCode.Unauthorized);
                }
                if (!isAccess(tramBefore.IDQuanLy)) return StatusCode(HttpStatusCode.Unauthorized);

                tramBefore.CotAnten = tram.CotAnten;
                tramBefore.CotTiepDat = tram.CotTiepDat;
                tramBefore.SanTram = tram.SanTram;
                db.SaveChanges();
            }


            return StatusCode(HttpStatusCode.NoContent);
        }
        // PUT: api/Trams/5
        //public IHttpActionResult PutTram(int id, string tenTram,string idQuanLy)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if(tenTram=="") return NotFound();
        //    Tram tram = db.Trams.Find(id);
        //    if (tram == null)
        //    {
        //        return NotFound();
        //    }
        //    if (idQuanLy != ""&&tram.IDQuanLy!=idQuanLy)
        //    {
        //        var tmpUser = db.UserBTSs.Find(idQuanLy);
        //        if (tmpUser == null) return NotFound();
        //        tram.IDQuanLy = idQuanLy;
        //    }

        //    if (tenTram != "")
        //        tram.TenTram= tenTram;

        //    try
        //    {
        //        db.SaveChanges();

        //        LichSuQuanLy lsqlOld = db.LichSuQuanLies.SingleOrDefault(u => u.IDQuanLy == idQuanLy && u.IDTram == id);
        //        lsqlOld.ThoiGianKetThuc = getDate();
        //        db.SaveChanges();

        //        LichSuQuanLy lsqlNew = new LichSuQuanLy
        //        {
        //            IDQuanLy = idQuanLy,
        //            IDTram = tram.IDTram,
        //            ThoiGianLamViec = getDate()
        //        };
        //        db.LichSuQuanLies.Add(lsqlNew);

        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TramExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Trams
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Tram))]
        public IHttpActionResult PostTram(Tram tram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Tram tmpTram1 = db.Trams.SingleOrDefault(u => u.KinhDo == tram.KinhDo && u.ViDo == tram.ViDo);
            if (tmpTram1 != null) return BadRequest();
            var tmpUser = db.UserBTSs.Find(tram.IDQuanLy);
            if (tmpUser == null) return NotFound();

            db.Trams.Add(tram);
            db.SaveChanges();
            Tram tmpTram = db.Trams.SingleOrDefault(u => u.KinhDo == tram.KinhDo && u.ViDo == tram.ViDo);
            LichSuQuanLy lsql = new LichSuQuanLy
            {
                IDQuanLy = tmpUser.IDUser,
                IDTram = tmpTram.IDTram,
                ThoiGianLamViec = getDate()
            };
            db.LichSuQuanLies.Add(lsql);
            db.SaveChanges();
            return CreatedAtRoute("DefaultApi", new { id = tram.IDTram }, tram);
        }

        // DELETE: api/Trams/5
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Tram))]
        public IHttpActionResult DeleteTram(int id)
        {
            Tram tram = db.Trams.Find(id);
            if (tram == null)
            {
                return NotFound();
            }

            db.Trams.Remove(tram);
            LichSuQuanLy lsqlOld = db.LichSuQuanLies.SingleOrDefault(u => u.IDQuanLy == tram.IDQuanLy && u.IDTram == id);
            lsqlOld.ThoiGianKetThuc = getDate();
            db.SaveChanges();

            return Ok(tram);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
        private bool checkThongSo(int? thongSo)
        {
            if (thongSo < 0 || thongSo > 15 || thongSo == null) return false;
            return true;
        }
        private DateTime getDate()
        {
            DateTime UTCNow = DateTime.UtcNow.AddHours(7);
            return UTCNow;
        }
    }
}