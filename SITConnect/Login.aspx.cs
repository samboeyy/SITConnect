using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string SITConnectDBConnectionString =
           System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;
        public string action = null;
        public class MyObject
        {
            public string success { get; set; }
            public List <String> ErrorMessage { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(SITConnectDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE EMAIL = EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            try
            {
                connection.Open();
                
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if(reader ["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {connection.Close();}
            return h;
        }
        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(SITConnectDBConnectionString);
            string sql = " select PASSWORDSALT FROM ACCOUNT WHERE EMAIL= @EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader["PASSWORDSALT"] != null)
                        {
                            if(reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;

        }
        
        protected void LoginMe(object sender, EventArgs e)
        {
           /* if (ValidateCaptcha())
            {*/
                string pwd = HttpUtility.HtmlEncode(tb_pwd.Text.ToString().Trim());
                string email = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());

                if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
                        {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Email or password cannot be empty!";
                    return;
                }

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);
            
     
                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);

                        if (userHash.Equals(dbHash))
                        {
                            Session["LoggedIn"] = tb_email.Text;
                            Response.Redirect("HomePage.aspx", false);
                        string guid = Guid.NewGuid().ToString();
                        Session["AuthToken"] = guid;

                        Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                        Response.Redirect("HomePage.aspx", false);
                        action = "Successfully logged in.";
                        createLog();
                        }
                        else
                        {
                            lblMessage.Text = "Email or password is not valid. Please try again.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                    }

                }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { }
         
        }
        protected void createLog()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITConnectDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO ActionLog VALUES (@Email, @DateTime, @Action)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email" ,HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                            cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Action", action);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private bool IsLock(string email)
        {
            var flag = false;
            using (SqlConnection connection = new SqlConnection(SITConnectDBConnectionString))
            {
                string sql = "select IsLock FROM Account WHERE Email = @Email";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["IsLock"] != null)
                            {
                                if (reader["IsLock"] != DBNull.Value)
                                {
                                    flag = Convert.ToBoolean(reader["IsLock"].ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
                return flag;
            }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captcahResponse = Request.Form["g-captcha-response"];

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6Lcuk2UeAAAAAJwN8fSZXkwbiGaJMkIm2R0EAXds &response=" + captcahResponse);
        
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using(StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }
    }
}