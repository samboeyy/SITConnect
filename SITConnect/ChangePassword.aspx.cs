using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string SITConnectDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;
        public string action = null;
        static string finalHash;
        static string salt;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
 
        protected void changePassword(object Sender, EventArgs e)
        {
                try
                {
                    using (SqlConnection con = new SqlConnection(SITConnectDBConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE [Account] SET [PasswordHash] = @PasswordHash, [PasswordSalt] = @PasswordSalt WHERE Email =@EMAIL"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                string pwd = HttpUtility.HtmlEncode(tb_new_pwd.Text.ToString().Trim());

                                //Generate random salt
                                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                                byte[] saltyByte = new byte[8];

                                //Fills array of bytes with a cryptographically strong sequence of random values
                                rng.GetBytes(saltyByte);
                                salt = Convert.ToBase64String(saltyByte);

                                SHA512Managed hashing = new SHA512Managed();

                                string pwdWithSalt = pwd + salt;
                                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                                finalHash = Convert.ToBase64String(hashWithSalt);

                                RijndaelManaged cipher = new RijndaelManaged();
                                cipher.GenerateKey();

                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@EMAIL", Session["LoggedIn"].ToString());

                                cmd.Connection = con;
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                                lblMessage.Text = "Password changed successfully!";

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                 Response.Redirect("Login.aspx");
                } 
            }
        }
