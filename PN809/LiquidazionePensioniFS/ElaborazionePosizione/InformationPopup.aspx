<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InformationPopup.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.InformationPopup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            <asp:Label ID="lblmessage" runat="server"></asp:Label>
            </p>
    </div>
    </form>
</body>
</html>
