using WebBasedInstaller.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Web.Http;
using System;
using System.Text;

namespace WebBasedInstaller.Controllers
{
    public class Angular2AuthenticationController : ApiController
    {
        // ********************************************************
        // Angular2Login

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        #region public ApplicationSignInManager SignInManager
        public ApplicationSignInManager SignInManager
        {
            get
            {
                // You need to install nuget packag: Microsoft.AspNet.WebApi.Owin
                // for the line below to work
                return _signInManager ??
                    Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        #endregion

        #region public ApplicationUserManager UserManager
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
        #endregion

        // POST: /api/Angular2Authentication/AdminLogin
        #region public ActionResult AdminLogin(DTOAuthentication Authentication)
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult AdminLogin(DTOAuthentication Authentication)
        {
            // Sign user out before you try to sign them in
            SignInManager.AuthenticationManager.SignOut();           

            // Get values passed
            var paramEmail = Authentication.Email;
            var paramPassword = Authentication.Password;

            // Try to get user
            var user = UserManager.FindByName(paramEmail);
            if (user != null)
            {
                // See if user is an Administrator
                if (!UserManager.IsInRole(user.Id, "Administrator"))
                {
                    // User is not an Administrator
                    // Do not try to log in
                    return ResponseMessage(
                        Request.CreateResponse(
                            System.Net.HttpStatusCode.Forbidden, "Failure"
                            ));
                }
            }
            else
            {
                // User not found
                return ResponseMessage(
                    Request.CreateResponse(
                    System.Net.HttpStatusCode.Forbidden, "Failure"
                    ));
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, 
            // change to shouldLockout: true
            var result = SignInManager.PasswordSignIn(
                paramEmail, paramPassword, false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return Ok("Success");
                default:
                    return ResponseMessage(
                        Request.CreateResponse(
                            System.Net.HttpStatusCode.Forbidden, "Failure"
                            ));
            }
        }
        #endregion
    }
}
