<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UnicarpeTest.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UnicarpeTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h2>Unicarpe Test</h2>
    <label class="etichettaBold">Sede:</label> 
    <asp:TextBox ID="txtSede" runat="server" MaxLength="4" CssClass="txtUppercase tb8" Width="50px" /> <br /> <br />
    <label class="etichettaBold">NumDomus:</label>  
    <asp:TextBox ID="txtDomus" runat="server" MaxLength="13" CssClass="txtUppercase tb8" Width="150px" /> <br /> <br />
    <label class="etichettaBold">CentroOperativo:</label>  
    <asp:TextBox ID="txtCO" runat="server" MaxLength="2" CssClass="txtUppercase tb8" Width="50px" /> <br /> <br />
    <asp:Button ID="btnLIQPENS" runat="server" OnClick="btnLIQPENS_Click" Text="Vai a LIQPENS" SkinID="btnAzione1"/>
    </div>
    </form>
</body>
</html>
