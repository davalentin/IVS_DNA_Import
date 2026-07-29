<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="SbloccoCancellazione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SbloccoCancellazione" %>

<%@ Register Src="~/UserControls/AltreFunzioni/SbloccoCancellazione/UCSbloccoCancellazione.ascx" TagName="UCSbloccoCancellazione" TagPrefix="UCSC" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function validatePage() {
        var flag = true;
        flag = Page_ClientValidate('UCSbloccoCancellazione');
        return flag;
    }
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione sblocco cancellazione domanda</h2>
    </div>

<table class="full-width" width="720px">

        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
        <td>
            <asp:ValidationSummary runat="server" ID="sbloccoCancellazione" ValidationGroup="UCSbloccoCancellazione" Font-Size="Small" CssClass="errorBox" />
        </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
                <UCSC:UCSbloccoCancellazione runat="server" ID="ucSbloccoCancellazione" Visible="true" OnShowAvviso="event_ucShowAvviso"/>
            </td>
        </tr>
    </table>

</asp:Content>
