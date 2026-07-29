<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" CodeBehind="RiassegnazioneDomanda.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.RiassegnazioneDomanda" %>

<%@ Register Src="~/UserControls/AltreFunzioni/RiassegnazioneDomanda/UCRiassegnazioneDomanda.ascx" TagName="UCRiassegnazioneDomanda" TagPrefix="UCRD" %>
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
        <h2 class="page-title-secondlevel">Gestione riassegnazione domanda</h2>
        <h6 class="page-subtitle">Riassegnazione domanda ad un altro utente</h6>
    </div>

    <table width="720px" class="full-width">
        <tr>
            <td align="left" style="width:720px" class="full-width">
                <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
            </td>
        </tr>
        <tr>
        <td>
            <asp:ValidationSummary runat="server" ID="riassegnazioneRicercaDomanda" ValidationGroup="UCRiassegnazioneRicercaDomanda" Font-Size="Small" CssClass="errorBox" />
            <asp:ValidationSummary runat="server" ID="riassegnazioneAggiornaDomanda" ValidationGroup="UCRiassegnazioneAggiornaDomanda" Font-Size="Small" CssClass="errorBox" />
        </td>
        </tr>
        <tr>
            <td align="center" style="width:720px" class="full-width">
            <UCRD:UCRiassegnazioneDomanda runat="server" ID="ucRiassegnazioneDomanda" Visible="true" OnShowAvviso="event_ucShowAvviso" OnHideInfo="event_ucHideInfo" OnCambioSede="event_ucCambioSede"/>
            </td>
        </tr>
    </table>
</asp:Content>
