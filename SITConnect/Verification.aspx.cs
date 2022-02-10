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
    public partial class Verification : System.Web.UI.Page
    {
        string SITConnectDBConnectionString =
          System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;
        public string action = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if(!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
            }
        }
        protected string verificationCodeOtp(string email)
        {
            string otp = null;
            SqlConnection con = new SqlConnection(SITConnectDBConnectionString);
            string sql = "SELECT VERIFICATIONCODE FROM ACCOUNT WHERE EMAIL = @EMAIL";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@EMAIL", email);
            
            try
            {
                con.Open();
                using (SqlDataReader reader= cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        otp = reader["VerificationCode"].ToString();
                    }
                 }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally 
            { 
                con.Close(); 
            }
            return otp;
        }
       protected void VerifyCode(object sender, EventArgs e)
        {
            if (HttpUtility.HtmlEncode(tb_verificationCode.Text.ToString()) == verificationCodeOtp(Session["LoggedIn"].ToString()))
            {
                createLog();
                Response.Redirect("HomePage.aspx", false);
            }
            else
            {
                lblMessage.Text = "Verification code is incorrect";
            }
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
                            cmd.Parameters.AddWithValue("@Email", Session["LoggedIn"].ToString());
                            cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Action", "Successfully logged in.");
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
    }
}