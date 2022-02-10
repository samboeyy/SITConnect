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
using System.Net.Mail;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string SITConnectDBConnectionString =
           System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;
        public string action = null;
        static string randomNum;
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

                            Random random = new Random();
                            randomNum = random.Next(000000, 999999).ToString();
                            createOTP(email, randomNum);
                            SendVerificationCode(randomNum);

                            Response.Redirect("Verification.aspx", false);

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
        protected string createOTP(string email, string randomNumber)
        {
            string otp = null;
            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);
            string sql = "UPDATE ACCOUNT SET VERIFICATIONCODE = @VERIFICATIONCODE WHERE EMAIL = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@VerificationCode", randomNumber);
            cmd.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["VerificationCode"] != null)
                        {
                            otp = reader["VerificationCode"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { con.Close(); }
            return otp;
        }
        protected string SendVerificationCode(string vcode)
        {
            string address = "SITConnect <sitconnect0123@gmail.com>";
            string str = null;
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                    Credentials = new NetworkCredential("sitconnect0123@gmail.com", "Sitconnect123!"),
                    EnableSsl = true
            };
            var message = new MailMessage
            {
                Subject = "SITConnect 2FA Login Authentication",
                Body = "Dear user, your verification is " + vcode + "\nPlease input this in the verification field to login."
            };
            message.To.Add(tb_email.Text.ToString());
            message.From = new MailAddress(address);
            try
            {
                smtpClient.Send(message);
                return str;
            }
             catch
            {
                throw;
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