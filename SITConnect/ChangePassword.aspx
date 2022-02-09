<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="SITConnect.ChangePassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <p>New Password : <asp:TextBox ID="tb_new_pwd" runat="server" Width="303px" TextMode="Password" />
         <asp:Label ID="lblMessage" runat="server" Text="Error message here (lblMessage)" Visible="false"></asp:Label>
        <p><asp:Button ID="btn_changepwd" runat="server" Text="Submit" OnClick="changePassword" Width="604px" /></p>
        <div>
        </div>
    </form>
</body>
</html>
