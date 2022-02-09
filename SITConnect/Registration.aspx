<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Registration</title>

    <script type ="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password length must be at least 12 characters.";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 number.";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 uppercase letter.";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_uppercase");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 lowercase letter.";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lowercase");
            }
            else if (str.search(/[!@#$%^&*]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 special character.";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special_character");
            }
            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent";
            document.getElementById("lbl_pwdchecker").style.color = "Blue";
        }
    </script>
</head>
<body>
      <div class="topnav">
      <a class="active" id="logo" href="HomePage.aspx" style ="float:left;">SITConnect</a>
      <a href="Registration.aspx">Register</a>
      <a href="Login.aspx">Login</a>
      <a href="HomePage.aspx">Home</a>
</div>
    <form id="form1" runat="server">
        <div>
            <fieldset>
            <legend>Registration</legend>
            <p>First Name: <asp:TextBox ID="tb_fname" runat="server" Width ="303px" /></p>
            <p>Last Name: <asp:TextBox ID="tb_lname" runat="server" Width ="303px" /></p>
            <p>Credit Card No: <asp:TextBox ID="tb_ccn" runat="server" Width ="303px"/></p>
            <p>Credit Card Date: <asp:TextBox ID="tb_ccd" runat="server" Width ="303px" type ="date" /></p>
            <p>Credit Card CVV: <asp:TextBox ID="tb_cvv" runat="server" Width ="303px" /></p>
            <p>Email: <asp:TextBox ID="tb_email" runat="server" Width ="303px" /></p>
            <p>Password : <asp:TextBox ID="tb_password" runat="server" Width="303px" TextMode="Password" />
            <asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker"></asp:Label></p>
            <p>Date of Birth: <asp:TextBox ID="tb_dob" runat="server" Width ="303px" type ="date" /></p>
            <p>Photo: <asp:TextBox ID="tb_photo" runat="server" Width ="303px" /> </p> 
            <br />
            <asp:Button ID="btn_checkPassword" runat="server" Text="Check Password" OnClick="btn_checkPassword_click" Width="604px" />
            <asp:Button ID="btn_register" runat="server" Text="Submit" OnClick="btn_Submit_Click" Width="604px" />
            </fieldset>
        </div>

    </form>
</body>
</html>
<style>
    /* Add a black background color to the top navigation */
.topnav {
  background-color: #333;
  overflow: hidden;
}
#logo a {
    float:left;
}
/* Style the links inside the navigation bar */
.topnav a {
  float: right;
  color: #f2f2f2;
  text-align: center;
  padding: 14px 16px;
  text-decoration: none;
  font-size: 17px;
}

/* Change the color of links on hover */
.topnav a:hover {
  background-color: #ddd;
  color: black;
}

/* Add a color to the active/current link */
.topnav a.active {
  background-color: #04AA6D;
  color: white;
}</style>
