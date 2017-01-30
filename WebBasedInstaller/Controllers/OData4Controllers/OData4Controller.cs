using WebBasedInstaller.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace WebBasedInstaller.Controllers
{
    public class OData4Controller : ODataController
    {
        // NewDatabaseVersion should always be "0.0.0"
        string NewDatabaseVersion = "0.0.0";

        // TargetDatabaseVersion should be changed to 
        // the version that the current code requires
        string TargetDatabaseVersion = "1.1.0";

        #region public IHttpActionResult CurrentUser()
        // odata/CurrentUser() - must be a Post call
        [ODataRoute("CurrentUser()")]
        public IHttpActionResult CurrentUser()
        {
            // User to return
            User objUser = new User();

            // See if the user is logged in
            if (this.User.Identity.IsAuthenticated)
            {
                // They are logged in
                objUser.UserName = this.User.Identity.Name;
            }
            else
            {
                // They are not logged in
                objUser.UserName = "[Not Logged in]";
            }

            // Return the result
            return Ok(objUser);
        }
        #endregion

        #region public IHttpActionResult CurrentVersion()
        // odata/CurrentVersion() - must be a Post call
        [ODataRoute("CurrentVersion()")]
        public IHttpActionResult CurrentVersion()
        {
            // Version object to return
            DTOVersion objVersion = GetDatabaseVersion(NewDatabaseVersion);
            objVersion.isNewDatabase =
                (objVersion.VersionNumber == NewDatabaseVersion);
            objVersion.isUpToDate =
                (objVersion.VersionNumber == TargetDatabaseVersion);

            // Return the result
            return Ok(objVersion);
        }
        #endregion

        #region public IHttpActionResult UpdateDatabase()
        // odata/UpdateDatabase() - must be a Post call
        [ODataRoute("UpdateDatabase()")]
        public IHttpActionResult UpdateDatabase()
        {
            DTOStatus objDTOStatus = new DTOStatus();
            if (this.User.Identity.IsAuthenticated &&
                this.User.IsInRole("Administrator"))
            {
                InstallWizardEntities db = new InstallWizardEntities();
                objDTOStatus.StatusMessage = "";
                objDTOStatus.Success = true;

                // Get the update scripts
                List<string> ColScripts = UpdateScripts();
                foreach (var sqlScript in ColScripts)
                {
                    try
                    {
                        // Run the script
                        db.Database.ExecuteSqlCommand(GetSQLScript(sqlScript));
                    }
                    catch (Exception ex)
                    {
                        objDTOStatus.StatusMessage = ex.Message;
                        objDTOStatus.Success = false;
                        return Ok(objDTOStatus);
                    }
                }
            }
            else
            {
                objDTOStatus.StatusMessage = "Not an authenticated Adminsitrator";
                objDTOStatus.Success = false;
            }

            // Return the result            
            return Ok(objDTOStatus);
        }
        #endregion

        // Helpers

        #region public static DTOVersion GetDatabaseVersion(string NewDatabaseVersion)
        public static DTOVersion GetDatabaseVersion(string NewDatabaseVersion)
        {
            // Version object to return
            DTOVersion objVersion = new DTOVersion();

            // If Version returned is NewDatabaseVersion 
            // we will assume the Version table does not exist
            objVersion.VersionNumber = NewDatabaseVersion;

            var ObjConnection =
                System.Configuration.ConfigurationManager
                .ConnectionStrings["DefaultConnection"];

            var strConnectionString = ObjConnection.ConnectionString;
            var DB = new InstallWizardEntities();
            // Set the connection string for InstallWizardEntities to the 
            // DefaultConnection setting in the Web.config
            DB.Database.Connection.ConnectionString = strConnectionString;

            if (strConnectionString != "")
            {
                try
                {
                    // There is actually a connection string
                    // Test it by trying to read the Version table
                    var result = (from version in DB.Versions
                                  orderby version.VersionNumber descending
                                  select version).FirstOrDefault();

                    // We have to find at least one Version record
                    if (result != null)
                    {
                        // Set Version number
                        objVersion.VersionNumber = result.VersionNumber;
                    }
                }
                catch
                {
                    // Do nothing if we cannot connect
                    // the method will return NewDatabaseVersion
                }
            }

            return objVersion;
        }
        #endregion

        #region private List<string> UpdateScripts()
        private List<string> UpdateScripts()
        {
            List<string> ColScripts = new List<string>();

            ColScripts.Add("01.1.0.sql");

            return ColScripts;
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