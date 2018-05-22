using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using ServerBTS2.Models;
using ServerBTS2.Providers;
using ServerBTS2.Results;
using System.Linq;
using System.Web.Http.Description;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Text.RegularExpressions;

namespace ServerBTS2.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [Authorize(Roles = "Admin")]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = UserManager.AddToRole(user.Id, model.ChucVu);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            UserBTS userBTS = new UserBTS
            {
                IDUser = user.Id,
                Ten = model.Ten,
                DiaChi = model.DiaChi,
                GioiTinh = model.GioiTinh,
                Image = model.Image,
                NgaySinh = model.NgaySinh,
                Email = model.Email,
                Phone = model.Phone,
                ChucVu = model.ChucVu
            };

            db.UserBTSs.Add(userBTS);
            db.SaveChanges();
            return Ok();
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }
        //GET: api/Account/GetUser
        [Route("GetUser")]
        public UserBTS GetUser()
        {
            return db.UserBTSs.Find(User.Identity.GetUserId());
        }

        //GET: api/Account/GetQuyen
        [AllowAnonymous]
        [Route("GetQuyen")]
        public UserBTS GetQuyen(string Email)
        {
            return db.UserBTSs.SingleOrDefault(u => u.Email == Email);
        }


        //GET: api/Account/GetUser
        [Route("GetUser")]
        public IHttpActionResult GetUser(string id)
        {
            UserBTS user = db.UserBTSs.Find(id);
            if (user == null) return NotFound();
            if (!isAdmin() && user.IDUser != User.Identity.GetUserId())
                return StatusCode(HttpStatusCode.Unauthorized);
            return Ok(user);
        }
        //GET: api/Account/GetAllUser
        [Authorize(Roles = "Admin")]
        [Route("GetAllUser")]
        public IEnumerable<UserBTS> GetAllUser()
        {
            return db.UserBTSs.ToList();
        }


        //GET: api/Account/GetAllQuanLy

        [Route("GetAllQuanLy")]
        public IEnumerable<UserBTS> GetAllQuanLy()
        {
            return db.UserBTSs.Where(u => u.ChucVu == "QuanLy").ToList();
        }

        //PUT: api/Account/PutUser
        [ResponseType(typeof(void))]
        [Route("PutUser")]
        public IHttpActionResult PutUser(UserBTS userBTS)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //var id = User.Identity.GetUserId();
                //var tmpidRole = db.Users.FirstOrDefault(u => u.Id == id).Roles.FirstOrDefault().RoleId;
                //var tmpnameRole = db.Roles.FirstOrDefault(u => u.Id == tmpidRole).Name;
                //var user=db.Users.Find(id);
                //if (id!=userBTS.IDUser||user.Email!= userBTS.Email||tmpnameRole!=userBTS.ChucVu)
                //{
                //    return BadRequest();
                //}
                //UserBTS 
                var id = User.Identity.GetUserId();
                UserBTS user = db.UserBTSs.Find(userBTS.IDUser);
                if (userBTS.IDUser != id || userBTS.Email != user.Email || userBTS.ChucVu != user.ChucVu)
                {
                    return BadRequest();
                }
                user.Ten = userBTS.Ten;
                user.DiaChi = userBTS.DiaChi;
                user.NgaySinh = userBTS.NgaySinh;
                user.GioiTinh = userBTS.GioiTinh;
                user.Phone = userBTS.Phone;
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        // POST: api/Account/UpdateImage

        [Route("PostImage")]
        [ResponseType(typeof(UserBTS))]
        public IHttpActionResult PostImage()
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
                    var id = User.Identity.GetUserId();
                    UserBTS user = db.UserBTSs.Find(id);
                    user.Image = ex + postedFile.FileName;
                    db.SaveChanges();
                    return Ok(user);
                    //return CreatedAtRoute("PostImage", new { id = user.IDUser }, user);
                }
                return StatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }


        }
        //PUT: api/Account/ChangeRole
        [Authorize(Roles = "Admin")]
        [Route("ChangeRole")]
        [ResponseType(typeof(void))]
        public IHttpActionResult ChangeRole(ChangeRole changeRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = db.Users.SingleOrDefault(u => u.UserName.Equals(changeRole.UserName));
            if (user == null)
            {
                return NotFound();
            }

            var roleOld = user.Roles.SingleOrDefault();
            var roleOldName = db.Roles.SingleOrDefault(u => u.Id.Equals(roleOld.RoleId)).Name;

            if (!roleOldName.Equals(changeRole.Role))
            {
                var userBTS = db.UserBTSs.Find(user.Id);
                userBTS.ChucVu = changeRole.Role;
                db.SaveChanges();
                UserManager.RemoveFromRole(user.Id, roleOldName);
                UserManager.AddToRole(user.Id, changeRole.Role);
            }
            return StatusCode(HttpStatusCode.OK);
        }
        //DELETE: api/Account/UserBTS/5
        [Authorize(Roles = "Admin")]
        [Route("DeleteUserBTS")]
        [ResponseType(typeof(UserBTS))]
        public async Task<IHttpActionResult> DeleteUserBTS(string id)
        {
            UserBTS userBTS = db.UserBTSs.Find(id);
            if (userBTS == null)
            {
                return NotFound();
            }
            ApplicationUser user = await UserManager.FindByIdAsync(id);
            UserManager.Delete(user);
            db.UserBTSs.Remove(userBTS);
            db.SaveChanges();
            return Ok(userBTS);
        }
        private bool checkDateValid(string dateTime)
        {
            try
            {
                DateTime myDate = DateTime.ParseExact(dateTime, "dd-MM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture);
                DateTime UTCNow = DateTime.UtcNow.AddHours(7);
                if (UTCNow.Subtract(myDate).TotalSeconds < 0) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        private bool isAdmin()
        {
            var idUser = User.Identity.GetUserId();
            var role = db.UserBTSs.SingleOrDefault(u => u.IDUser == idUser).ChucVu;
            return role.Equals("Admin");
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
