<%@ Page Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="LiquidazionePensioneAgo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.LiquidazionePensioneAgo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCDatiGenerici.ascx" TagName="UCDatiGenerici"
    TagPrefix="UCDG" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCOpzione.ascx" TagName="UCOpzione"
    TagPrefix="UCO" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCPrecedentePensione.ascx"
    TagName="UCPrecedentePensione" TagPrefix="UCPP" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCIstruttoria.ascx" TagName="UCIstruttoria"
    TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCDatiAssicurativi.ascx"
    TagName="UCDatiAssicurativi" TagPrefix="UCDA" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCINAIL.ascx" TagName="UCINAIL"
    TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/CrossSuppLiqAgo/UCContribuzioneEnpals.ascx" TagName="UCContribEnpals"
    TagPrefix="UCCENPALS" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCLiquidazionePensioneStorico_AGO.ascx"
    TagName="UCLPStorico" TagPrefix="UCLPS" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCSentenzaArt4.ascx" TagName="UCSentenzaArt4"
    TagPrefix="UCSA4" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneAgo/UCSentenze.ascx" TagName="UCSentenze"
    TagPrefix="UCS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
    <style type="text/css">
        .fixed-dialog
        {
            position: fixed;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);

            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);

                if (activeTab == "#istruttoria") {
                    //var codNatura3 = GetCodNatura3();
                    VisualizzaRowAzienda();
                    VisualizzaRowAttivitaUsuranti();
                }

                return false;
            });

            var skipSet = Get_SKIP_SetChkBenefici();
            if (skipSet != "TRUE")
                SetChkBenefici();
            else {
                SetHdnBenefici();
            }
            try {
                var siglaCategoria = Get_siglaCategoria();
                if (!(siglaCategoria == 'VOTOT' || siglaCategoria == 'SOTOT' || siglaCategoria == 'IOTOT'))
                    SetAttEconomicaProfIndividualeCumulo();
            }
            catch (ex)
            { }
        });

        function Get_SKIP_SetChkBenefici() {
            var SKIP_SetChkBenefici = $("#<%= hdnSKIP_SetChkBenefici.ClientID %>").val();
            return SKIP_SetChkBenefici;
        }

        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlTabDatiGenerici.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiGenerici');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabPrecedentePensione.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabPrecedentePensione');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabDatiAssicurativi.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabDatiAssicurativi');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabIstruttoria.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabIstruttoria');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabOpzione.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabOpzione');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabINAIL.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabINAIL');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabSentenzaArt4.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabSentenzaArt4');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabSentenze.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabSentenze');
                }
            }

            return flag;
        }

        function AbilitaTab() {
            document.getElementById("<%=pnlTabPrecedentePensione.ClientID %>").style.display = 'block';
        }

        function DisabilitaTab() {
            document.getElementById("<%=pnlTabPrecedentePensione.ClientID %>").style.display = 'none';
        }

        function ConfirmPage() {
            var ddl = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_ddlRiduzioneRetributiva');
            var tipoCalcolo = typeof getTipoCalcolo === "function" ? getTipoCalcolo() : undefined;
            var hfDataTitolareAdd62 = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_HiddenDataTitolareAdd62').value;
            var noShow = false;
            if (ddl != null) {
                var selectedValue = ddl.options[ddl.selectedIndex].value;
                if (hfDataTitolareAdd62 != null && hfDataTitolareAdd62 != "") {
                    var codNat1 = typeof GetCodNatura1 === "function" ? GetCodNatura1() : undefined;
                    var dataScadenza = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_txtScadenza').value;
                    if (dataScadenza != null && dataScadenza != "") {
                        var dateApp = hfDataTitolareAdd62.split("/");
                        var d1 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                        dateApp = dataScadenza.split("/");
                        var d2 = new Date(dateApp[1], dateApp[0] - 1, 1);
                        if (d1 < d2) {
                            noShow = true;
                        }
                    }
                    if (codNat1 != '1')
                        noShow = true;

                    var ddlSoggettoDerogato = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_ddlSoggettoDerogato');          
                    if (!(ddlSoggettoDerogato!= null && $('#ctl00_ContentPlaceHolder1_ucIstruttoria_ddlSoggettoDerogato').is(':visible') == true && $('#ctl00_ContentPlaceHolder1_ucIstruttoria_ddlSoggettoDerogato').val() == ""))
                        noShow = true;

                    var siglaCategoria = document.getElementById("ctl00_ContentPlaceHolder1_ucIstruttoria_HiddenFieldSiglaCategoria").value;
                    var ddlRiduzioneAssegno = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_ddlRiduzioneAssegno');          
                    if (siglaCategoria == "VOCRED" && !(ddlRiduzioneAssegno != null &&  $('#ctl00_ContentPlaceHolder1_ucIstruttoria_ddlRiduzioneAssegno').is(':visible') == true && $('#ctl00_ContentPlaceHolder1_ucIstruttoria_ddlRiduzioneAssegno').val() == ""))            
                        noShow = true;
                }
                if (selectedValue.toUpperCase() == 'SI' || tipoCalcolo == 1 || noShow == true)
                    document.getElementById('<%= btnSalvaLiquidazionePensioneAgo.ClientID %>').click();
                else
                    $('#dialog-confirmPage').dialog('open');
            }
            return false;
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
                        document.getElementById('<%= btnSalvaLiquidazionePensioneAgo.ClientID %>').click();
                        return true;
                    }
                }
            });
        });

        function SetChkBenefici() {
            EnableBenefici(CheckBeneficiDisabled());
            setTimeout(function () {
                SetChkBenefici();
            }, 250);
        }

        function SetAttEconomicaProfIndividualeCumulo() {

            var HiddenFieldIsRicTfrTotCum_Liq = document.getElementById("<%= HiddenFieldIsRicTfrTotCum_Liq.ClientID %>").value;
            if (HiddenFieldIsRicTfrTotCum_Liq != "SI") {
                EnableAttEconomicaProfIndividualeCumulo(GetEnteCassa());
            }
        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_generici" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiGenericiVS" ValidationGroup="UCTabDatiGenerici"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabOpzioneVS" ValidationGroup="UCTabOpzione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrecedentePensioneVS" ValidationGroup="UCTabPrecedentePensione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabIstruttoriaVS" ValidationGroup="UCTabIstruttoria"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiAssicurativiVS" ValidationGroup="UCTabDatiAssicurativi"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabINAILGrid" ValidationGroup="UCTabGridINAIL"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabINAIL" ValidationGroup="UCTabINAIL"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabSentenzaArt4" ValidationGroup="UCTabSentenzaArt4"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabSentenze" ValidationGroup="UCTabSentenze"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlLiquidazionePensioneAgo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <div>
                <ul class="tabsLine2 tabs">
                    <asp:Panel runat="server" ID="pnlTabDatiGenerici">
                        <li><a href="#dati_generici">Generici
                            <asp:Image ID="imgDatiGenerici" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabDatiAssicurativi">
                        <li><a href="#dati_assicurativi">Assicurativi
                            <asp:Image ID="imgDatiAssicurativi" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabIstruttoria">
                        <li><a href="#istruttoria">Istruttoria
                            <asp:Image ID="imgIstruttoria" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabINAIL">
                        <li><a href="#inail">Inail/Accomp.
                            <asp:Image ID="imgINAIL" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabPrecedentePensione">
                        <li><a href="#precedente_pensione">Pens. Prov.
                            <asp:Image ID="imgPrecedentePensione" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabOpzione">
                        <li><a href="#opzione">Opzione
                            <asp:Image ID="imgOpzione" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabContribuzioneEnpals">
                        <li><a href="#contribuzioneEnpals">Contribuzione
                            <asp:Image ID="imgContribuzioneEnpals" runat="server" ImageAlign="Top" /></a>
                        </li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabSentenzaArt4">
                        <li><a href="#sentenzaArt4">Sentenza Art. 4
                            <asp:Image ID="imgSentenzaArt4" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabSentenze">
                        <li><a href="#sentenze">Sentenze<asp:Image ID="imgSentenze" ImageAlign="Top" runat="server" /></a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabStorico">
                        <li><a href="#storico">Storico GP
                            <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" /></a></li>
                    </asp:Panel>
                </ul>
            </div>
            <div class="tab_container" style="min-height: 90px; padding-top: 15px">
                <div id="dati_generici" class="tab_content">
                    <UCDG:UCDatiGenerici runat="server" ID="ucDatiGenerici" OnShowAvviso="event_ucShowAvvisoDatiGenerici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici" OnShowAvvisoTrattenutaFondoCredito="event_ucShowAvvisoTrattenutaFondoCredito" />
                </div>
                <div id="dati_assicurativi" class="tab_content">
                    <UCDA:UCDatiAssicurativi runat="server" ID="ucDatiAssicurativi" OnShowAvviso="event_ucShowAvvisoDatiAssicurativi"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi" />
                </div>
                <div id="opzione" class="tab_content">
                    <UCO:UCOpzione runat="server" ID="ucOpzione" OnShowAvviso="event_ucShowAvvisoDatiOpzione"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiOpzione" />
                </div>
                <div id="precedente_pensione" class="tab_content">
                    <UCPP:UCPrecedentePensione runat="server" ID="ucPrecedentePensione" OnShowAvviso="event_ucShowAvvisoDatiPrecedentePensione"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiPrecedentePensione" />
                </div>
                <div id="istruttoria" class="tab_content">
                    <UCI:UCIstruttoria runat="server" ID="ucIstruttoria" OnShowAvviso="event_ucShowAvvisoDatiIstruttoria"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiIstruttoria" OnHideAvviso="event_ucHideAvviso" />
                </div>
                <div id="inail" class="tab_content">
                    <UCI:UCINAIL runat="server" ID="ucInail" OnShowAvviso="event_ucShowAvvisoInail" OnShowAvvisoElimina="event_ucShowAvvisoEliminaInail" />
                </div>
                <div id="contribuzioneEnpals" class="tab_content">
                    <UCCENPALS:UCContribEnpals runat="server" ID="ucContribEnpals" OnSalvaContribuzioneEnpals="event_ucSalvaContribuzioneEnpals" />
                </div>
                <div id="sentenzaArt4" class="tab_content">
                    <UCSA4:UCSentenzaArt4 runat="server" ID="ucSentenzaArt4" OnShowAvviso="event_ucShowAvvisoSentenzaArt4"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaSentenzaArt4" />
                </div>
                <div id="sentenze" class="tab_content">
                    <UCS:UCSentenze runat="server" ID="ucSentenze" OnShowAvviso="event_ucShowAvvisoSentenze"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaSentenze"></UCS:UCSentenze>
                </div>
                <!--Storico-->
                <div id="storico" class="tab_content">
                    <UCLPS:UCLPStorico ID="ucStorico" runat="server" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Text="Salva Tutto" Visible="false" Width="170px" OnClientClick="if(mainValidateForConfirm()) {return ConfirmPage();}" CssClass="tertiary ml-0" />
                        <asp:Button ID="btnSalvaLiquidazionePensioneAgo" runat="server" Text="Salva Tutto"
                            SkinID="btnAzione1" CausesValidation="false" Style="display: none" Visible="false"
                            OnClick="SalvaLiquidazionePensioneAgo_Click" Width="170px" OnClientClick="mainValidate()" CssClass="tertiary ml-0" />
                        <asp:Button ID="btnSalvaLiquidazionePensioneAgoNoRiduzione" runat="server" CausesValidation="false"
                            SkinID="btnAzione1" Width="170px" OnClick="SalvaLiquidazionePensioneAgo_Click"
                            Text="Salva Tutto" Visible="true" OnClientClick="mainValidate()" CssClass="tertiary ml-0" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="170px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            <asp:Label ID="lblEtaTit" runat="server">Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?</asp:Label></p>
    </div>
    <asp:HiddenField runat="server" ID="HiddenFieldSedi" />
    <asp:HiddenField runat="server" ID="hdnSKIP_SetChkBenefici" Value="FALSE" />
    <asp:HiddenField runat="server" ID="HiddenFieldIsRicTfrTotCum_Liq" Value="NO" />
</asp:Content>
