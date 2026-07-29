<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="CambioStatoDomanda.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.CambioStatoDomanda" %>

<%@ Register Src="~/UserControls/AltreFunzioni/CambioStatoDomanda/UCCambioStatoDomanda.ascx" TagName="UCCambioStatoDomanda" TagPrefix="UCCSD" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function validatePage() {
        var flag = true;
        flag = Page_ClientValidate('UCRiassegnazioneRicercaDomanda');

        if (flag) {
            flag = Page_ClientValidate('UCRiassegnazioneAggiornaDomanda');
        }

        return flag;
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="page-title">
        <h2 class="page-title-secondlevel">Gestione cambio dati domanda</h2>
    </div>

    <table class="full-width" width="720px">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
        <td>
            <asp:ValidationSummary runat="server" ID="VSAggiornaStatoDomanda" ValidationGroup="UCAggiornaStatoDomanda" Font-Size="Small" CssClass="errorBox" />
        </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
            <UCCSD:UCCambioStatoDomanda runat="server" ID="ucCambioStatoDomanda" Visible="true" OnShowAvviso="event_ucShowAvviso" OnShowAvvisoStatoCambiato="event_ucShowAvvisoStatoCambiato"
                OnReloadUChangeSede="event_ReloadUChangeSede" OnHideAvviso="event_HideAvviso"/>
            </td>
        </tr>
    </table>
</asp:Content>
