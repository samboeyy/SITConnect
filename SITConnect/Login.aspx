<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src =" https://www.google.com/recaptcha/api.js?render=6Lcuk2UeAAAAAJwN8fSZXkwbiGaJMkIm2R0EAXds"></script>
</head>
<body>
  <div class="topnav">
      <a class="active" id="logo" href="HomePage.aspx" style ="float:left;">SITConnect</a>
      <a href="Registration.aspx">Register</a>
      <a href="Login.aspx">Login</a>
      <a href="HomePage.aspx">Home</a>
</div>

    <asp:Label ID="headerlogin" runat="server" Text="Login"></asp:Label>
    <form id="form1" runat="server">
        <div>
        <fieldset>
            <p>Email : <asp:TextBox ID ="tb_email" runat="server" Height="35px" Width ="287px" /></p>
            <p>Password : <asp:TextBox ID ="tb_pwd" runat="server" Height="35px" Width ="290px" /></p>
            <p> <asp:Button ID="btnSubmit" runat="server" Text ="Login" OnClick ="LoginMe" Height ="45px" Width ="133px" />
            <br />
            <br />

            <input type ="hidden" id="g-recaptcha-response" name ="g-recaptcha-response" />
            <asp:Label ID="lblMessage" runat="server" Text="Error message here (lblMessage)" Visible="false"></asp:Label>
        </p>
            <asp:Label ID="lbl_gScore" runat="server" Text =""></asp:Label>
            </fieldset>
            </div>
    </form>
        <script>
            grecaptcha.ready(function ()) {
                grecaptcha.execute('6Lcuk2UeAAAAAJwN8fSZXkwbiGaJMkIm2R0EAXds', { action: 'Login' }).then(function (token) {
                    document.getElementById("g-recaptcha-response").value = token;
                });
            });
        </script>
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
