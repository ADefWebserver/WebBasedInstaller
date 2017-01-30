using WebBasedInstaller.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using System.Web.Http;
using System;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;

namespace WebBasedInstaller.Controllers
{
    public class Angular2AdminController : ApiController
    {
        // NewDatabaseVersion should always be "0.0.0"
        string NewDatabaseVersion = "0.0.0";

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

        // POST: /api/Angular2Admin/CreateAdminLogin
        #region public IHttpActionResult CreateAdminLogin(DTOAuthentication Authentication)
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult CreateAdminLogin(DTOAuthentication Authentication)
        {
            // Database connection
            InstallWizardEntities DB = new InstallWizardEntities();

            // This method can only be called if the Database version is NewDatabaseVersion
            // Meaning that application is not set-up
            var DatabaseVersion = OData4Controller.GetDatabaseVersion(NewDatabaseVersion);

            if(DatabaseVersion.VersionNumber != NewDatabaseVersion)
            {
                return ResponseMessage(
                        Request.CreateResponse(
                            System.Net.HttpStatusCode.Forbidden, "Failure"
                            ));
            }

            // Get values passed
            var paramEmail = Authentication.Email;
            var paramPassword = Authentication.Password;

            try
            {
                // Run Script to create all set-up tables                
                string strSQLScript = GetSQLScript("01.0.0.sql");
                DB.Database.ExecuteSqlCommand(strSQLScript);
            }
            catch (Exception ex)
            {
                return ResponseMessage(
                    Request.CreateResponse(
                    System.Net.HttpStatusCode.Forbidden, ex.Message
                    ));
            }

            // Try to get user
            var user = UserManager.FindById(paramEmail);
            if (user == null)
            {
                try
                {
                    // Create Admin
                    CreateAdmin(paramEmail, paramPassword);
                    
                    /****** Insert Version 1.0.0 data ******/
                    Models.Version objVersion = new Models.Version();
                    objVersion.VersionNumber = "1.0.0";
                    DB.Versions.Add(objVersion);
                    DB.SaveChanges();

                    return Ok("Created");
                }
                catch (Exception ex)
                {
                    return ResponseMessage(
                        Request.CreateResponse(
                        System.Net.HttpStatusCode.Forbidden, ex.Message
                        ));
                }
            }
            else
            {
                return ResponseMessage(
                    Request.CreateResponse(
                    System.Net.HttpStatusCode.Forbidden, "User already exists"
                    ));
            }           
        }
        #endregion

        // Helpers

        #region private ApplicationUser CreateAdmin(string strEmail, string strPassword)
        private ApplicationUser CreateAdmin(string strEmail, string strPassword)
        {
            const string AdministratorRoleName = "Administrator";

            // See if User exists
            ApplicationUser objUser = UserManager.FindByName(strEmail);

            if (objUser == null)
            {
                // Create Administrator Role
                var roleManager =
                    new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                if (!roleManager.RoleExists(AdministratorRoleName))
                {
                    roleManager.Create(new IdentityRole(AdministratorRoleName));
                }

                // Create Admin user
                var objNewUser = new ApplicationUser { UserName = strEmail, Email = strEmail };
                var UserCreateResult = UserManager.Create(objNewUser, strPassword);

                // Make user an Admin
                UserManager.AddToRole(objNewUser.Id, AdministratorRoleName);
            }

            // Return User
            objUser = UserManager.FindByName(strEmail);
            return objUser;
        }
        #endregion

        #region private String GetSQLScript(string SQLScript)
        private String GetSQLScript(string SQLScript)
        {
            string strSQLScript;
            string strFilePath = System.Web.Hosting.HostingEnvironment.MapPath(String.Format(@"~/!SQLScripts/{0}", SQLScript));
            using (StreamReader reader = new StreamReader(strFilePath))
            {
                strSQLScript = reader.ReadToEnd();
                reader.Close();
            }
            return strSQLScript;
        } 
        #endregion
    }
}
