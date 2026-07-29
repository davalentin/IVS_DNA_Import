<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Supplementi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Supplementi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Supplementi/UCSupplementi.ascx" TagName="UCSupplementi"
    TagPrefix="UCSupp" %>
<%@ Register Src="~/UserControls/Supplementi/UCSupplementiAgoCI.ascx" TagName="UCSupplementiAgoCI"
    TagPrefix="UCSuppAgoCI" %>
<%@ Register Src="~/UserControls/Supplementi/UCSupplementiENPALS.ascx" TagName="UCSupplementiENPALS"
    TagPrefix="UCSuppENPALS" %>
<%@ Register Src="~/UserControls/CrossSuppLiqAgo/UCContribuzioneEnpals.ascx" TagName="UCContribEnpals"
    TagPrefix="UCCENPALS" %>
<%@ Register Src="~/UserControls/Supplementi/UCSupplementiCumulo.ascx" TagName="UCSupplementiCumulo"
    TagPrefix="UCSuppCumulo" %>
<%@ Register Src="~/UserControls/Supplementi/UCSupplementiCumuloStorico.ascx" TagName="UCSupplementiCumuloStorico"
    TagPrefix="UCSuppCumulStorico" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });

        function validatePage() {
            var flag = true;
            var tipoApp = ('<%=this.domanda.TipoAppartenenza%>');
            if (tipoApp != '' && (tipoApp.toUpperCase() == 'AGO' || tipoApp.toUpperCase() == 'CI')) {
                if (document.getElementById("<%=pnlTabSupplementi.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabSupplementiAGO');
            }
            else {
                if (document.getElementById("<%=pnlTabSupplementi.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabSupplementi');
            }
            return flag;
        }

        $(function () {
            $('#dialog-confirmPage').dialog({
                autoOpen: false,

                show: 'blind',
                hide: 'blind',
                height: 220,
                width: 450,
                modal: true,
                centerX: true,
                centerY: true,
                dialogClass: 'fixed-dialog',
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    'Annulla': function () {
                        $(this).dialog('close');
                        return false;
                    },
                    'Ok': function () {
                        $(this).dialog('close');
                        document.getElementById('<%= btnSalva.ClientID %>').click();
                        return true;
                    }
                }
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#supplementi" runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCTabSupplementi"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary3" ValidationGroup="UCTabSupplementiAGO"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="UCTabSupplementiContrib"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary2" ValidationGroup="UCTabSupplementiRetrib"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary4" ValidationGroup="UCTabSupplementiENPALSRetrib"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary5" ValidationGroup="UCTabSupplementiENPALSContrib"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary6" ValidationGroup="UCTabRecordSupplementiENPALS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlMenuSupplementi">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xl">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabSupplementi">
                    <li><a href="#supplementi">Supplementi
                        <asp:Image ID="imgSupplementi" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabContribuzioneEnpals">
                    <li><a href="#contribuzioneEnpals">Contribuzione
                        <asp:Image ID="imgContribuzioneEnpals" runat="server" ImageAlign="Top" /></a>
                    </li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabQuoteSupplementi">
                    <li><a href="#quoteSupplementi">
                        <asp:Label ID="lblTitleQuoteSupplementi" runat="server" Text="Quote Supplementi" />
                        <asp:Image ID="imgQuoteSupplementi" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
                  <asp:Panel runat="server" ID="pnlTabStorico">
                    <li><a href="#quotaSupplementiCumuloStorico">Storico Supplementi
                        <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 150px;">
                <div id="supplementi" class="tab_content">
                    <UCSupp:UCSupplementi runat="server" ID="ucSupplementi" OnSalvaSupplementi="event_ucSalvaSupplementi"
                        OnEliminaSupplementi="event_ucEliminaSupplementi" OnErrorSalvaSupplementi="event_ucErrorSalvaSupplementi"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowAvviso="event_ucShowAvviso" />
                    <UCSuppAgoCI:UCSupplementiAgoCI runat="server" ID="ucSupplementiAgoCI" OnSalvaSupplementi="event_ucSalvaSupplementi"
                        OnEliminaSupplementi="event_ucEliminaSupplementi" OnErrorSalvaSupplementi="event_ucErrorSalvaSupplementi"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowPopUp="event_ucShowPopUp" OnHidePopUp="event_ucHidePopUp" OnShowAvviso="event_ucShowAvviso"
                        OnHideAvviso="event_ucHideAvviso" />
                    <UCSuppENPALS:UCSupplementiENPALS runat="server" ID="ucSupplementiENPALS" OnSalvaSupplementi="event_ucSalvaSupplementi"
                        OnEliminaSupplementi="event_ucEliminaSupplementi" OnErrorSalvaSupplementi="event_ucErrorSalvaSupplementi"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnHideTastoSalva="event_ucHideTastoSalva" OnShowTastoSalva="event_ucShowTastoSalva"
                        OnInitData="event_ucInitData" OnHideAvviso="event_ucHideAvviso" />
                </div>
                <div id="contribuzioneEnpals" class="tab_content">
                    <UCCENPALS:UCContribEnpals runat="server" ID="ucContribEnpals" OnSalvaContribuzioneEnpals="event_ucSalvaContribuzioneEnpals" />
                </div>
                <div id="quoteSupplementi" class="tab_content">
                    <UCSuppCumulo:UCSupplementiCumulo runat="server" ID="ucSupplementiCumulo" OnSalvaSupplementi="event_ucSalvaSupplementi"
                        OnEliminaSupplementi="event_ucEliminaSupplementi" OnErrorSalvaSupplementi="event_ucErrorSalvaSupplementi"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnHideTastoSalva="event_ucHideTastoSalva" OnShowTastoSalva="event_ucShowTastoSalva"
                        OnInitializeData="event_ucInitData" OnHideAvviso="event_ucHideAvviso" OnShowAvviso="event_ucShowAvviso" />
                </div>
                <div id="quotaSupplementiCumuloStorico" class="tab_content">
                    <UCSuppCumulStorico:UCSupplementiCumuloStorico runat="server" ID="ucSupplementiCumuloStorico" />
                </div>
            </div>
            <table width="100%" style="text-align: center;" class="footer-actions-group">
                <tr>
                    <td style="width: 1%">
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Style="display: none" Text="Salva" Width="160px" OnClientClick="$('#dialog-confirm').dialog('open'); return false;" CssClass="tertiary" />
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" OnClientClick="mainValidate()"
                            CausesValidation="false" Width="160px" OnClick="SalvaSupplementi_Click" CssClass="tertiary" />
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="160px"
                            OnClientClick="aspnetForm.target ='_self'; BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            Attenzione il Montante è inferiore all’Ammontare.<br />
            Confermare ?</p>
    </div>
</asp:Content>
