<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
              <div class="topnav">
      <a class="active" id="logo" href="HomePage.aspx" style ="float:left;">SITConnect</a>
      <a href="Registration.aspx">Register</a>
      <a href="Login.aspx">Login</a>
      <a href="HomePage.aspx">Home</a>
</div>
        <div>
            <fieldset>
                <legend>Home Page</legend>
                <br />
                <asp:Label ID ="lblMessage" runat="server" EnableViewState ="false" />
                <br />
                <br />
                <p>First Name:<asp:Label ID="lbl_fname" runat="server" ></asp:Label></p>
                <p>Email:<asp:Label ID="lbl_email" runat="server"></asp:Label></p>
                <asp:Button ID ="btnChangePwd" runat ="server" Text ="Change Password" OnClick ="btnChangePwd_Click" Visible ="true"/>
                <asp:Button ID ="btnLogout" runat ="server" Text ="Logout"  OnClick="LogoutMe" Visible ="false" />
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
