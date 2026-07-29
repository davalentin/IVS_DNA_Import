<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DatiContributiviAgo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiContributiviAgo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCDatiCalcoloAgo.ascx" TagName="UCDatiCalcoloAgo"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCDatiCalcoloENPALS.ascx" TagName="UCDatiCalcoloENPALS"
    TagPrefix="UCDENPALS" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCDatiCalcoloINPDAI.ascx" TagName="UCDatiCalcoloINPDAI"
    TagPrefix="UCDINPDAI" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCQuotePensione.ascx" TagName="UCQuotePensione"
    TagPrefix="UCQTPNSN" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCMiglioramentiContrattuali.ascx" TagName="UCMiglioramentiContrattuali"
    TagPrefix="UCMGCNT" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UcDatiCalcoloVittimeTerrorismo.ascx"
    TagName="UCDatiCalcoloTerrorismo" TagPrefix="UCDTERR" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCDatiCalcoloStoricoGP_AGO.ascx"
    TagName="UcDatiCalcoloStorico" TagPrefix="UCDCS" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCQuotaFondoIntegrativo.ascx"
    TagName="UcQuotaFondoIntegrativo" TagPrefix="UCQFI" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCQuotaFondoINPGI.ascx" TagName="UcQuotaFondoINPGI"
    TagPrefix="UCQFINPGI" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCDatiEsteri.ascx" TagName="UcDatiEsteri"
    TagPrefix="UCDE" %>
<%@ Register Src="~/UserControls/DatiContributiviAgo/UCQuotaFondoINPGIStoricoGP.ascx" TagName="UCQuotaFondoINPGIStoricoGP" 
    TagPrefix="UCQFIS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            GestioneVisibilitaPannelliGridView();
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });

        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null) {
                if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pdivRetributivo") != null) {
                    if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_modalitaEditRetributivi").value == "true")
                        flag = Page_ClientValidate('UCTabDatiCalcoloAgoRetr');
                }
                if (flag) {
                    if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pdivContributivo") != null) {
                        if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_modalitaEditContributivi").value == "true")
                            flag = Page_ClientValidate('UCTabDatiCalcoloAgoContr');
                    }
                }
                if (flag) {
                    if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_divPnlImportoLordoDecorrenza") != null ||
                        document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloAgo_pnlDatiCalcoloRendita") != null) {
                        flag = Page_ClientValidate('UCTabDatiCalcolo');
                    }
                }
            }

            if (flag) {
                if (document.getElementById("<%=pnlTabDatiCalcoloINPDAI.ClientID%>") != null) {
                    if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloINPDAI_pdivRetributivo") != null) {
                        if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloINPDAI_modalitaEditRetributivi").value == "true")
                            flag = Page_ClientValidate('UCTabDatiCalcoloAgoRetr');
                    }
                    if (flag) {
                        if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloINPDAI_pdivContributivo") != null) {
                            if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloINPDAI_modalitaEditContributivi").value == "true")
                                flag = Page_ClientValidate('UCTabDatiCalcoloAgoContr');
                        }
                    }
                    if (flag) {
                        if (document.getElementById("ctl00_ContentPlaceHolder1_ucDatiCalcoloINPDAI_pnlContributoSolidarieta") != null) {
                            flag = Page_ClientValidate('UCTabDatiCalcoloINPDAI');
                        }
                    }
                }
            }

            if (flag) {
                if (document.getElementById("<%=pnlTabDatiCalcoloENPALS.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabDatiCalcoloENPALS');
                }
            }

            if (flag) {
                if (document.getElementById("<%=pnlTabQuotaFondoIntegrativo.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabQuotaFondoIntegrativo');
                }
            }

            if (flag) {
                if (document.getElementById("<%=pnlTabQuotaFondoINPGI.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabQuotaFondoINPGI');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabDatiEsteri.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabDatiEsteri');
                }
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
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_contributivi_ago"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloAgoRetr" ValidationGroup="UCTabDatiCalcoloAgoRetr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloAgoContr" ValidationGroup="UCTabDatiCalcoloAgoContr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcolo" ValidationGroup="UCTabDatiCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloINPDAI" ValidationGroup="UCTabDatiCalcoloINPDAI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="UCTabDatiCalcoloENPALS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="gvQuotePensione" ValidationGroup="UCGvQuotePensione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="gvTrattenute" ValidationGroup="UCGvTrattenute"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloVittimeRetr" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloVittimeContr" ValidationGroup="UCTabDatiCalcoloVittimeContr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloVittimeImportoPensione" ValidationGroup="UCTabDatiCalcoloVittimeImportoPensione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloENPALSRetr" ValidationGroup="UCTabDatiCalcoloENPALSRetr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloENPALSContr" ValidationGroup="UCTabDatiCalcoloENPALSContr"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabQuotaFondoIntegrativo" ValidationGroup="UCTabQuotaFondoIntegrativo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabQuotaFondoINPGIRetr" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabQuotaFondoINPGIContr" ValidationGroup="UCTabQuotaContrFondoINPGIAgo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiEsteri" ValidationGroup="UCTabDatiEsteri"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiEsteriGrid" ValidationGroup="UCTabDatiEsteriGrid"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiEsteriGrid2" ValidationGroup="UCTabDatiEsteriGrid2"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel ID="pnlValidationSummary" runat="server">
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiCalcolo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth md">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo">
                    <li><a href="#dati_contributivi_ago">Dati Calcolo
                        <asp:Image ID="imgDatiCalcolo" ImageAlign="Top" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/rosso_tab.png" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcoloENPALS">
                    <li><a href="#dati_contributivi_ENPALS">Dati Calcolo
                        <asp:Image ID="imgDatiCalcoloENPALS" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcoloINPDAI">
                    <li><a href="#dati_contributivi_INPDAI">Dati Calcolo
                        <asp:Image ID="imgDatiCalcoloINPDAI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabQuotePensione">
                    <li><a href="#dati_quotePensione" id="href_quotePensione" runat="server">Quote Pensione
                        <asp:Image ID="imgQuotePensione" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                 <asp:Panel runat="server" ID="pnlTabMiglioramentiContrattuali">
                    <li><a href="#dati_miglioramentiContrattuali" id="A1" runat="server">Miglioramenti Contrattuali
                        <asp:Image ID="imgMiglioramentiContrattuali" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcoloTerrorismo">
                    <li><a href="#dati_contributivi_Terrorismo">Dati Calcolo Terrorismo
                        <asp:Image ID="imgDatiCalcoloTerrorismo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabStorico">
                    <li><a href="#storico">Storico GP
                        <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabQuotaFondoIntegrativo">
                    <li><a href="#quota_fondo_integrativo">Quota Fondo Integrativo
                        <asp:Image ID="imgQuotaFondoIntegrativo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabQuotaFondoINPGI">
                    <li><a href="#quota_fondo_inpgi">Quota Fondo INPGI
                        <asp:Image ID="imgQuotaFondoINPGI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiEsteri">
                    <li><a href="#dati_esteri">Dati Esteri
                        <asp:Image ID="imgDatiEsteri" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                     <asp:Panel runat="server" ID="pnlQuotaFondoInpgiStorico">
                    <li><a href="#quota_fondo_inpgi_Storico">Quota Fondo INPGI Storico GP
                        <asp:Image ID="imgQuotaFondoINPGIStorico" ImageAlign="Top" runat="server" Visible="false" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 50px;">
                <div id="dati_contributivi_ago" class="tab_content">
                    <UCDC:UCDatiCalcoloAgo runat="server" ID="ucDatiCalcoloAgo" OnGestisciTastoSalva="event_ucGestisciTastoSalva"
                        OnInitializeData="event_ucInitializeData" OnShowAvviso="event_ucShowAvviso" OnAbilitaPopUpDatiContributivi="event_ucAbilitaPopUpDatiContributivi"
                        OnDisabilitaPopUpDatiContributivi="event_ucDisabilitaPopUpDatiContributivi" OnUpdateDatiCalcoloTerrorismoRetributivi="event_ucUpdateDatiCalcoloTerrorismoRetributivi"
                        OnUpdateDatiCalcoloTerrorismoContributivi="event_ucUpdateDatiCalcoloTerrorismoContributivi"
                        OnHideAvviso="event_ucHideAvviso" />
                </div>
                <div id="dati_contributivi_ENPALS" class="tab_content">
                    <UCDENPALS:UCDatiCalcoloENPALS runat="server" ID="ucDatiCalcoloENPALS" OnShowAvviso="event_ucShowAvviso" />
                </div>
                <div id="dati_contributivi_INPDAI" class="tab_content">
                    <UCDINPDAI:UCDatiCalcoloINPDAI runat="server" ID="ucDatiCalcoloINPDAI" OnShowAvviso="event_ucShowAvviso"
                        OnGestisciTastoSalva="event_ucGestisciTastoSalva" OnInitializeData="event_ucInitializeData"
                        OnUpdateDatiCalcoloTerrorismoRetributivi="event_ucUpdateDatiCalcoloTerrorismoRetributivi"
                        OnUpdateDatiCalcoloTerrorismoContributivi="event_ucUpdateDatiCalcoloTerrorismoContributivi"
                        OnHideAvviso="event_ucHideAvviso" />
                </div>
                <div id="dati_quotePensione" class="tab_content">
                    <UCQTPNSN:UCQuotePensione runat="server" ID="ucQuotePensione" OnShowAvviso="event_ucShowAvviso"
                        OnGestisciTastoSalva="event_ucGestisciTastoSalva" OnInitializeData="event_ucInitializeData"
                        OnHideAvviso="event_ucHideAvviso" OnAddValidationGroupname="event_AddValidationGroupname" />
                </div>
                <div id="dati_miglioramentiContrattuali" class="tab_content">
                    <UCMGCNT:UCMiglioramentiContrattuali runat="server" ID="ucMiglioramentiContrattuali" OnShowAvviso="event_ucShowAvviso"
                        OnGestisciTastoSalva="event_ucGestisciTastoSalva" OnInitializeData="event_ucInitializeData"
                        OnHideAvviso="event_ucHideAvviso" OnAddValidationGroupname="event_AddValidationGroupname" />
                </div>
                <div id="dati_contributivi_Terrorismo" class="tab_content">
                    <UCDTERR:UCDatiCalcoloTerrorismo runat="server" ID="ucDatiCalcoloVittimeTerrorismo"
                        OnShowAvviso="event_ucShowAvviso" OnHideAvviso="event_ucHideAvviso" OnGestisciTastoSalva="event_ucGestisciTastoSalvaVittimeTerrorismo"
                        OnInitializeData="event_ucInitializeData" />
                </div>
                <!--Storico-->
                <div id="storico" class="tab_content">
                    <UCDCS:UcDatiCalcoloStorico ID="ucStorico" runat="server" />
                </div>
                <!--Storico-->
                <div id="quota_fondo_integrativo" class="tab_content">
                    <UCQFI:UcQuotaFondoIntegrativo runat="server" ID="ucQuotaFondoIntegrativo" OnShowAvviso="event_ucShowAvviso"
                        OnHideAvviso="event_ucHideAvviso" OnInitializeData="event_ucInitializeData" OnGestisciTastoSalva="event_ucGestisciTastoSalva" />
                </div>
                <div id="quota_fondo_inpgi" class="tab_content">
                    <UCQFINPGI:UcQuotaFondoINPGI runat="server" ID="ucQuotaFondoINPGI" OnShowAvviso="event_ucShowAvviso"
                        OnHideAvviso="event_ucHideAvviso" OnInitializeData="event_ucInitializeData" OnGestisciTastoSalva="event_ucGestisciTastoSalva" />
                </div>
                <div id="dati_esteri" class="tab_content" style="display: block">
                    <UCDE:UcDatiEsteri ID="ucDatiEsteri" runat="server" OnShowAvvisoDatiProRata="event_ucShowAvvisoDatiProRata"
                        OnNascondiAvviso="event_ucNascondiAvviso" OnShowAvvisoEliminaDatiProRata="event_ucShowAvvisoEliminaDatiProRata" />
                </div>
                 <div id="quota_fondo_inpgi_Storico" class="tab_content">
                    <UCQFIS:UCQuotaFondoINPGIStoricoGP ID="UCQuotaFondoINPGIStoricoGP" runat="server" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Style="display: none" Text="Salva" Width="190px" OnClientClick="if(mainValidateForConfirm()){$('#dialog-confirm').dialog('open');} return false;" CssClass="tertiary" />
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" OnClientClick="mainValidate()"
                            CausesValidation="false" Width="190px" OnClick="btnSalva_Click" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            PostBackUrl="~/ElaborazionePosizione.aspx" Width="190px" CausesValidation="false"
                            OnClientClick="BlockUI()" Visible="true" />
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
