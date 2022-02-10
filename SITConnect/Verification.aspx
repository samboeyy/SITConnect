<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Verification.aspx.cs" Inherits="SITConnect.Verification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verificaiton</title>
</head>
<body>
    <form id="form1" runat="server">

        <div>
            <fieldset>
            <p>Verification Code : <asp:TextBox ID ="tb_verificationCode" runat="server" Height="35px" Width ="287px" /></p>
            <p> <asp:Button ID="btnSubmit" runat="server" Text ="Submit Code" OnClick ="VerifyCode" Height ="45px" Width ="133px" />
                </fieldset>
            <br />
            <asp:Label ID="lblMessage" runat="server" Text="Error message here (lblMessage)" Visible="false"></asp:Label>

        </div>
    </form>
</body>
</html>
