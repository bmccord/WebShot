<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebShotTestSite._Default" %>

<%@ Register Assembly="WebShotImageWebControl" Namespace="Julian.WebControls" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="lblUrl" runat="server" Text="Url"></asp:Label>
        <asp:TextBox ID="txtUrl" runat="server" Width="384px">www.google.com</asp:TextBox>
        <br />
        <asp:Label ID="Label1" runat="server" Text="Browser Width"></asp:Label>
        <asp:TextBox ID="txtWidth" runat="server">1024</asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Browser Height"></asp:Label>
        <asp:TextBox ID="txtHeight" runat="server">768</asp:TextBox><br />
        <asp:Label ID="Label3" runat="server" Text="WebShot Width"></asp:Label>
        <asp:TextBox ID="txtWebShotWidth" runat="server">1024</asp:TextBox>
        <br />
        <asp:Label ID="Label4" runat="server" Text="WebShot Height"></asp:Label>
        <asp:TextBox ID="txtWebShotHeight" runat="server">768</asp:TextBox>&nbsp;<br />
        <br />
        <asp:CheckBox ID="chkAutoSize" runat="server" Text="AutoSize" />
        <asp:CheckBox ID="chkAutoSizeWebShot" runat="server" Text="AutoSize WebShot" />
        <asp:CheckBox ID="chkKeepWebShotProportional" runat="server" Text="Keep WebShot Proportional" /><br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Generate" />
        <asp:Button ID="Postback" runat="server" onclick="Postback_Click" 
            Text="Test Postback" />
        <br />
        <br />
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/Notavailable.jpg" /></div>
        <cc1:WebShotImageWebControl ID="WebShotImageWebControl1" runat="server" ImageType="Jpeg">
        </cc1:WebShotImageWebControl>
    </form>
</body>
</html>
