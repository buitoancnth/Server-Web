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
using System.Web;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class HinhAnhTramsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // GET: api/HinhAnhTrams
        public IEnumerable<HinhAnhTram> GetHinhAnhTrams()
        {
            if (isAdmin())
                return db.HinhAnhTrams;
            else
            {
                string id = User.Identity.GetUserId();
                var tram = db.Trams.Where(u => u.IDQuanLy == id);
                return db.HinhAnhTrams.Where(u => tram.Count(p => p.IDTram == u.IDTram) > 0).ToList();

            }

        }

        // GET: api/HinhAnhTrams/5
        [ResponseType(typeof(HinhAnhTram))]
        public IHttpActionResult GetHinhAnhTram(int id)
        {
            HinhAnhTram hinhAnhTram = db.HinhAnhTrams.Find(id);
            if (hinhAnhTram == null)
            {
                return NotFound();
            }
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == hinhAnhTram.IDTram);
            if (!isAccess(tmp.IDQuanLy)) return StatusCode(HttpStatusCode.Unauthorized);
            return Ok(hinhAnhTram);
        }
        // GET: api/HinhAnhTrams/5
        [ResponseType(typeof(HinhAnhTram))]
        [Route("api/HinhAnhTramsByIDTram")]
        public IEnumerable<HinhAnhTram> GetHinhAnhsTramByIDTram(int id)
        {
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == id);
            if (tmp == null) return null;
            if (!isAccess(tmp.IDQuanLy)) return null;
            return db.HinhAnhTrams.Where(u => u.IDTram == id).ToList();

        }
        // POST: api/HinhAnhTrams
        [AllowAnonymous]
        [ResponseType(typeof(HinhAnhTram))]
        public IHttpActionResult PostHinhAnhTram(int idTram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == idTram);
            if (tmp == null) return NotFound();
            //if (!isAccess(tmp.IDQuanLy))
            //    return StatusCode(HttpStatusCode.Unauthorized);

            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    var ex = RandomString(10);

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        //var tmpHinhAnhTram = db.HinhAnhTrams.SingleOrDefault(u => u.Ten == postedFile.FileName);
                        //if (tmpHinhAnhTram != null)
                        //{
                        //    return StatusCode(HttpStatusCode.BadRequest);
                        //}
                        int MaxContentLength = 1024 * 1024 * 10; //Size = 10 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            return StatusCode(HttpStatusCode.BadRequest);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {
                            return StatusCode(HttpStatusCode.BadRequest);
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/image/" + ex + postedFile.FileName);
                            postedFile.SaveAs(filePath);
                        }
                    }
                    db.HinhAnhTrams.Add(new HinhAnhTram() { IDTram = idTram, Ten = ex + postedFile.FileName });
                    db.SaveChanges();
                    HinhAnhTram hinhAnhTram = db.HinhAnhTrams.SingleOrDefault(u => u.Ten.Equals(ex + postedFile.FileName));
                    return CreatedAtRoute("DefaultApi", new { id = hinhAnhTram.IDHinhAnh }, hinhAnhTram);
                }
                return StatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }


        }

        // DELETE: api/HinhAnhTrams/5
        [ResponseType(typeof(HinhAnhTram))]
        public IHttpActionResult DeleteHinhAnhTram(int id)
        {
            HinhAnhTram hinhAnhTram = db.HinhAnhTrams.Find(id);
            if (hinhAnhTram == null)
            {
                return NotFound();
            }
            var tmp = db.Trams.SingleOrDefault(u => u.IDTram == hinhAnhTram.IDTram);
            if (!isAccess(tmp.IDQuanLy)) return StatusCode(HttpStatusCode.Unauthorized);

            db.HinhAnhTrams.Remove(hinhAnhTram);
            db.SaveChanges();

            return Ok(hinhAnhTram);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool HinhAnhTramExists(int id)
        {
            return db.HinhAnhTrams.Count(e => e.IDHinhAnh == id) > 0;
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