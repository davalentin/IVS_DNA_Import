<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Test</h2>
            <label class="etichettaBold">Sede:</label>
            <asp:TextBox ID="txtSede" runat="server" MaxLength="4" CssClass="txtUppercase tb8" Width="50px" />
            <br />
            <br />
            <label class="etichettaBold">CentroOperativo:</label>
            <asp:TextBox ID="txtCO" runat="server" MaxLength="2" CssClass="txtUppercase tb8" Width="50px" />
            <br />
            <br />
            <label class="etichettaBold">NumDomus:</label>
            <asp:TextBox ID="txtDomus" runat="server" MaxLength="13" CssClass="txtUppercase tb8" Width="150px" />
            <br />
            <br />
            <label class="etichettaBold">Gestione:</label>
            <asp:TextBox ID="txtGestione" runat="server" MaxLength="3" CssClass="txtUppercase tb8" Width="50px" />
            <br />
            <br />
            <label class="etichettaBold">IndConvInt:</label>
            <asp:CheckBox ID="chkIndConvInt" runat="server" />
            <br />
            <br />
            <label class="etichettaBold">Tipo Visualizzazione (SCRIWO):</label>
            <asp:DropDownList ID="ddlTipoVisualizzazione" runat="server">
                <asp:ListItem Text="scriwo" Value="scriwo"></asp:ListItem>
                <asp:ListItem Text="scriwoView" Value="scriwoView"></asp:ListItem>
            </asp:DropDownList>
            <br />
            <br />
            <asp:Button ID="btnLIQPENS_U" runat="server" OnClick="btnLIQPENS_U_Click" Text="Vai a LIQPENS da Unicarpe" SkinID="btnAzione1" />
            <asp:Button ID="btnLIQPENS_W" runat="server" OnClick="btnLIQPENS_W_Click" Text="Vai a LIQPENS da WebDom" SkinID="btnAzione1" />
            <asp:Button ID="btnLIQPENS_S" runat="server" OnClick="btnLIQPENS_S_Click" Text="Vai a LIQPENS da Sistema Unico" SkinID="btnAzione1" />
            <asp:Button ID="btnLIQPENS_P" runat="server" OnClick="btnLIQPENS_P_Click" Text="Vai a LIQPENS da Previsan" SkinID="btnAzione1" />
            <asp:Button ID="btnLIQPENS_SCRIWO" runat="server" OnClick="btnLIQPENS_SCRIWO_Click" Text="Vai a LIQPENS da SCRIWO" SkinID="btnAzione1" />
        </div>
    </form>
</body>
</html>
