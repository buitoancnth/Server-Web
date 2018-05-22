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
using System.Web;

namespace ServerBTS2.Controllers
{
    [Authorize]
    public class NhaMangsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

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
        // POST: api/Account/UpdateImage

        [Route("api/ChangeImageNhaMang")]
        [ResponseType(typeof(NhaMang))]
        public IHttpActionResult ChangeImageNhaMang(int idNhaMang)
        {
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
                    NhaMang nhaMang = db.NhaMangs.Find(idNhaMang);
                    if(nhaMang == null) return StatusCode(HttpStatusCode.NotFound);
                    nhaMang.Image = ex + postedFile.FileName;
                    db.SaveChanges();
                    return Ok(nhaMang);
                }
                return StatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }


        }

        // POST: api/NhaMangs
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(NhaMang))]
        public IHttpActionResult PostNhaMang(String tenNhaMang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(tenNhaMang.Equals("")) StatusCode(HttpStatusCode.BadRequest);
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
                    NhaMang nhaMang = new NhaMang();
                    nhaMang.TenNhaMang = tenNhaMang;
                    nhaMang.Image = ex + postedFile.FileName;
                    db.NhaMangs.Add(nhaMang);
                    db.SaveChanges();
                    NhaMang nhaMangx = db.NhaMangs.SingleOrDefault(u => u.Image.Equals(ex + postedFile.FileName));
                    return CreatedAtRoute("DefaultApi", new { id = nhaMangx.IDNhaMang }, nhaMangx);
                }
                return StatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }
            
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