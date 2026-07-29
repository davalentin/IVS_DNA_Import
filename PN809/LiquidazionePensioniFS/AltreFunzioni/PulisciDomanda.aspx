<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="PulisciDomanda.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.PulisciDomanda" %>

<%@ Register Src="~/UserControls/AltreFunzioni/PulisciDomanda/UCPulisciDomanda.ascx" TagName="UCPulisciDomanda" TagPrefix="UCPD" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function validatePage() {
        var flag = true;
        flag = Page_ClientValidate('UCPulisciDomanda');

        if (flag) {
            flag = Page_ClientValidate('UCPulisciDomanda');
        }

        return flag;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione pulisci domanda</h2>
    </div>

    <table width="720px" class="full-width">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
        <td>
            <asp:ValidationSummary runat="server" ID="VSUCPulisciDomanda" ValidationGroup="UCPULDOM" Font-Size="Small" CssClass="errorBox" />
        </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
            <UCPD:UCPulisciDomanda runat="server" ID="ucPulisciDomanda" Visible="true" OnShowAvviso="event_ucShowAvviso" OnHideAvviso="event_ucHideAvviso"
                OnReloadUChangeSede="event_ReloadUChangeSede" />
            </td>
        </tr>
    </table>
</asp:Content>
