using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using WebBasedInstaller.Models;

namespace WebBasedInstaller.Controllers
{
    public class ODataConnectionSettingController : ODataController
    {
        #region public IHttpActionResult Post(DTOConnectionSetting objConnectionSetting)
        // POST: odata/ODataConnectionSetting
        public IHttpActionResult Post(DTOConnectionSetting objConnectionSetting)
        {
            // The return message
            string strMessage = "Success";

            strMessage = ConnectAndUpdateDatabase(objConnectionSetting);

            // Return the result
            return Ok(strMessage);
        }
        #endregion

        // Utility

        #region public string ConnectAndUpdateDatabase(DTOConnectionSetting objConnectionSetting)
        private string ConnectAndUpdateDatabase(DTOConnectionSetting objConnectionSetting)
        {
            // The return message
            string strMessage = "Success";

            // First try to connect to the the database using the last 
            // connection saved in the web.config
            // if we can connect we must NOT allow this method 
            // to overwrite the last connection saved in the web.config
            var ObjConnection =
                System.Configuration
                .ConfigurationManager
                .ConnectionStrings["DefaultConnection"];

            var strConnectionString = ObjConnection.ConnectionString;
            var DB = new InstallWizardEntities();

            // Set the connection string for InstallWizardEntities to the 
            // DefaultConnection setting in the Web.config
            DB.Database.Connection.ConnectionString = strConnectionString;

            // Try to connect to the database
            if (!DB.Database.Exists())
            {
                try
                {
                    // Try to connect with the new connection string
                    string strNewConnectionString =
                        CreateDatabaseConectionString(objConnectionSetting);

                    DB.Database.Connection.ConnectionString = strNewConnectionString;

                    // Try to connect to the database with new connection string
                    if (DB.Database.Exists())
                    {
                        // If connection was successful then write the connection string 
                        // to the web.config to be used on the next wizard step

                        if (DB.Database.Exists())
                        {
                            // the connection is good -- update the web.config
                            Configuration objConfig =
                                System.Web.Configuration
                                .WebConfigurationManager
                                .OpenWebConfiguration("~");

                            ConnectionStringsSection objConnectionStringsSection =
                                (ConnectionStringsSection)objConfig.ConnectionStrings;

                            if (objConnectionStringsSection != null)
                            {
                                objConnectionStringsSection
                                    .ConnectionStrings["DefaultConnection"]
                                    .ConnectionString = strNewConnectionString;

                                objConnectionStringsSection
                                    .ConnectionStrings["InstallWizardEntities"]
                                    .ConnectionString = CreateEntityConnectionString(objConnectionSetting);

                                objConfig.Save();
                            }
                        }
                    }
                    else
                    {
                        // We cannot connect
                        strMessage = "Connection Failed";
                    }
                }
                catch (Exception ex)
                {
                    // If there is an error
                    // return the error
                    strMessage = ex.Message;
                }
            }
            else
            {
                // We were able to connect to the database using 
                // the last connection saved in the web.config
                // therefore we will not update it with new values
                // We still return the defult success message
            }

            return strMessage;
        } 
        #endregion

        #region private string CreateDatabaseConectionString(DTOConnectionSetting objConnectionSetting)
        private string CreateDatabaseConectionString(DTOConnectionSetting objConnectionSetting)
        {
            string strConnectionSring = "";
            string strUserInfo = (!objConnectionSetting.IntegratedSecurity) ?
                String.Format("Persist Security Info=True;User ID={0};Password={1}",
                objConnectionSetting.Username, 
                objConnectionSetting.Password) :
                "Integrated Security=True";

            strConnectionSring = String.Format("Data Source={0};Initial Catalog={1};{2}",
                objConnectionSetting.ServerName,
                objConnectionSetting.DatabaseName,
                strUserInfo);

            return strConnectionSring;
        }
        #endregion

        #region private string CreateEntityConnectionString(DTOConnectionSetting objConnectionSetting)
        private string CreateEntityConnectionString(DTOConnectionSetting objConnectionSetting)
        {
            StringBuilder SB = new StringBuilder();
            string strConnectionSring = "";

            SB.Append("metadata=res://*/Models.InstallWizardDB.csdl|");
            SB.Append("res://*/Models.InstallWizardDB.ssdl|");
            SB.Append("res://*/Models.InstallWizardDB.msl;provider=System.Data.SqlClient;");
            SB.Append("provider connection string=\"");

            string strUserInfo = (!objConnectionSetting.IntegratedSecurity) ?
                String.Format("user id={0};password={1};{2}",
                objConnectionSetting.Username,
                objConnectionSetting.Password,
                "integrated security=False;MultipleActiveResultSets=True;App=EntityFramework\"") :
                "integrated security=True;MultipleActiveResultSets=True;App=EntityFramework\"";

            strConnectionSring = String.Format("{0}data source={1};initial catalog={2};{3}",
                SB.ToString(),
                objConnectionSetting.ServerName,
                objConnectionSetting.DatabaseName,
                strUserInfo);

            return strConnectionSring;
        }
        #endregion
    }
}