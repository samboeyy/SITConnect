using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string SITConnectDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["SITConnectDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        public string action = null;


        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected byte [] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
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
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
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
        protected void btn_Submit_Click (object sender, EventArgs e)
        {
            string pwd = tb_password.Text.ToString().Trim();

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
            Key = cipher.Key;
            IV = cipher.IV;
            action = "Registered an account.";
            createLog();
        createAccount();
        }
        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SITConnectDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Account VALUES (@FName, @LName, @CCNumber, @CCDate, @CCCVV, @Email, @DoB, @Photo, @PasswordHash, @PasswordSalt, @IV, @Key)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@FName", HttpUtility.HtmlEncode(tb_fname.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LName", HttpUtility.HtmlEncode(tb_lname.Text.Trim()));
                            cmd.Parameters.AddWithValue("@CCNumber", HttpUtility.HtmlEncode(Convert.ToBase64String(encryptData(tb_ccn.Text.Trim()))));
                            cmd.Parameters.AddWithValue("@CCDate", HttpUtility.HtmlEncode(Convert.ToBase64String(encryptData(tb_ccd.Text.Trim()))));
                            cmd.Parameters.AddWithValue("@CCCVV", HttpUtility.HtmlEncode(Convert.ToBase64String(encryptData(tb_cvv.Text.Trim()))));
                            cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                            cmd.Parameters.AddWithValue("@DoB", HttpUtility.HtmlEncode(tb_dob.Text.Trim()));
                            cmd.Parameters.AddWithValue("@Photo", HttpUtility.HtmlEncode(tb_photo.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                            cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                            cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected void btn_checkPassword_click (object sender, EventArgs e)
        {
            int scores = checkPassword(tb_password.Text);
            string status = "";
            switch(scores)
            {
                case 1:
                    status = "Very Weak";
                    break;
                case 2:
                    status = "Weak";
                    break;
                case 3:
                    status = "Medium";
                    break;
                case 4:
                    status = "Strong";
                    break;
                case 5:
                    status = "Very Strong";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status: " + status;
            if(scores < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            }
            lbl_pwdchecker.ForeColor = Color.Green;
        }
        private int checkPassword(string password)
        {
            int score = 0;
            //Score 1 (lngth < 8)
            if (password.Length < 12)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            //Score 2 weak (contains lowercase letters)
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            //Score 3 medium (contains uppercase letters)
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            //Score 4 strong (contains numerals)
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            //Score 5 excellent (contains special characters)
            if (Regex.IsMatch(password, "[!@#$%^&*]"))
            {
                score++;
            }
            return score;
        }
    }
}