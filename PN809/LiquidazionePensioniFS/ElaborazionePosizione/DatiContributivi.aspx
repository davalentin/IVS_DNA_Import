<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DatiContributivi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiContributivi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloEL_TT_ET.ascx" TagName="UCDatiCalcoloEL_TT_ET"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloVL_FS_PT.ascx" TagName="UCDatiCalcoloVL_FS_PT"
    TagPrefix="UCDCVL" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiAgoGAS_ES.ascx" TagName="UCDatiAgoGAS_ES"
    TagPrefix="UCDAGAS_ES" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiAgo_PI.ascx" TagName="UCDatiAgo_PI"
    TagPrefix="UCDA_PI" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiFondo_PI.ascx"
    TagPrefix="UCDFPI"
    TagName="UCDatiFondo_PI" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiFondoGAS_ES.ascx" TagName="UCDatiFondoGAS_ES"
    TagPrefix="UCDFGAS" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCArt11e14GAS_ES.ascx" TagName="UCArt11e14GAS_ES"
    TagPrefix="UCA1114GAS_ES" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloDZ.ascx" TagName="UCDatiCalcoloDZ"
    TagPrefix="UCDCDZ" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCTipoPensioneNonSelezionato.ascx"
    TagName="UCTipoPensioneNonSelezionato" TagPrefix="UCTPNS" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCAnte67ES.ascx" TagName="UCAnte67"
    TagPrefix="UCANT67" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCSL336_ES.ascx" TagName="UCSl336"
    TagPrefix="UCSL_336" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloPM.ascx" TagName="UCDatiCalcoloPM"
    TagPrefix="UCDCPM" %>
<%--<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloPI.ascx" TagName="UCDatiCalcoloPI"
    TagPrefix="UCDCPI" %>--%>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiAgoAltraPensione_ET.ascx" TagName="UcDatiAgoAltraPensione"
    TagPrefix="UCDAAP" %>
<%@ Register Src="~/UserControls/DatiContributivi/UCDatiCalcoloStoricoGP.ascx" TagName="UcDatiCalcoloStorico"
    TagPrefix="UCDCS" %>
<%@ Register Src="~/UserControls/CrossDatiFondoContr/UCDatiCalcolo707.ascx" TagName="UcDatiCalcolo707"
    TagPrefix="UCDC707" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <style type="text/css">
        .fixed-dialog {
            position: fixed;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);

            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });


        function validatePage() {
            var flag = true;
            var tipoFondo = ('<%= (this.domanda != null ? this.domanda.Tipofondo.ToString() : "") %>');
            if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'EL' || tipoFondo.toUpperCase() == 'TT')) {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcolo');
            }
            else if (tipoFondo != '' && tipoFondo.toUpperCase() == 'ET') {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcolo');
                if (document.getElementById("<%=pnlTabDatiAgo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiAgoGAS');
            }
            else if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'VL' || tipoFondo.toUpperCase() == 'FS' || tipoFondo.toUpperCase() == 'PT')) {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcoloVL');
            }
            else if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'GAS' || tipoFondo.toUpperCase() == 'ES')) {
                if (document.getElementById("<%=pnlTabDatiFondo.ClientID %>") != null)
                    flag = Page_ClientValidate('UCTabDatiFondoGAS');
                if (flag) {
                    if (document.getElementById("<%=pnlTabDatiAgo.ClientID %>") != null)
                        flag = Page_ClientValidate('UCTabDatiAgoGAS');
                }
                if (flag) {
                    if (document.getElementById("<%=pnlTabArt11e14.ClientID %>") != null)
                        flag = Page_ClientValidate('UCTabArt11_14GAS');
                }
            }
            else if (tipoFondo != '' && tipoFondo.toUpperCase() == 'DZ') {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcoloDZ');
            }
            else if (tipoFondo != '' && tipoFondo.toUpperCase() == 'PM') {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcoloPM');
            }
            else if (tipoFondo != '' && tipoFondo.toUpperCase() == 'PI') {
                if (document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabDatiCalcoloPI');

                if (flag) {
                    if (document.getElementById("<%=pnlTabDatiAgo.ClientID %>") != null)
                        flag = Page_ClientValidate('UCTabDatiAgoPI');
                }

            }
            else if (tipoFondo.toUpperCase() == 'PI') {
                if (document.getElementById("<%=pnlTabDatiAgo.ClientID %>") != null)
                    if (document.getElementById('UCTabDatiAgoPI'));
            }
            return flag;
        }


        function ConfirmPage() {
            var ddl = null;
            var tipoFondo = ('<%= (this.domanda != null ? this.domanda.Tipofondo.ToString() : "") %>');
            if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'EL' || tipoFondo.toUpperCase() == 'TT' || tipoFondo.toUpperCase() == 'ET' || tipoFondo.toUpperCase() == 'GAS')) {
                ddl = document.getElementById('ctl00_ContentPlaceHolder1_ucDatiCalcoloEL_TT_ET_ddlRiduzioneRetributiva');
            }
            else if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'VL')) {
                ddl = document.getElementById('ctl00_ContentPlaceHolder1_ucDatiCalcoloVL_FS_PT_ddlRiduzioneRetributiva');
            }
            else if (tipoFondo != '' && (tipoFondo.toUpperCase() == 'DZ')) {
                ddl = document.getElementById('ctl00_ContentPlaceHolder1_ucDatiCalcoloDZ_ddlRiduzioneRetributiva');
            }

            if (ddl != null) {
                var selectedValue = ddl.options[ddl.selectedIndex].value;
                if (selectedValue.toUpperCase() == 'SI')
                    document.getElementById('<%= btnSalva.ClientID %>').click();
                else
                    $('#dialog-confirmPage').dialog('open');
            }
            return false;
        }

        function ConfirmContributiviPage() {
            if (CheckAmmontareMaggioreDiMontante()) {
                $('#dialog-ContributiviPage').dialog('open');
            }
            else {
                if (document.getElementById('<%= btnSalvaNoRiduzione.ClientID %>'))
                    document.getElementById('<%= btnSalvaNoRiduzione.ClientID %>').click();
                else if (document.getElementById('<%= btnSalva.ClientID %>'))
                    document.getElementById('<%= btnSalva.ClientID %>').click();
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
                resizable: false,
                draggable: true,
                centerX: true,
                centerY: true,
                dialogClass: 'fixed-dialog',
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

        $(function () {
            $('#dialog-ContributiviPage').dialog({
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
                        if (document.getElementById('<%= btnSalvaNoRiduzione.ClientID %>'))
                            document.getElementById('<%= btnSalvaNoRiduzione.ClientID %>').click();
                        else if (document.getElementById('<%= btnSalva.ClientID %>'))
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
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_calcolo" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcolo" ValidationGroup="UCTabDatiCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcolo707" ValidationGroup="UCTabDatiCalcolo707"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloVL" ValidationGroup="UCTabDatiCalcoloVL"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiFondoGAS" ValidationGroup="UCTabDatiFondoGAS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiAgoGAS" ValidationGroup="UCTabDatiAgoGAS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabArt11_14GAS" ValidationGroup="UCTabArt11_14GAS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloDZ" ValidationGroup="UCTabDatiCalcoloDZ"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiFondoGAS_GvElementiCalcolo" ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabUCDatiAnte67ES" ValidationGroup="UCDatiAnte67ES"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabUCAnte67_gvArt57" ValidationGroup="UCAnte67ES_Ante57"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabUCSL336" ValidationGroup="UCSL336_ES"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabUCDCPM" ValidationGroup="UCTabDatiCalcoloPM"
        Font-Size="Small" CssClass="errorBox" />
    <%--<asp:ValidationSummary runat="server" ID="tabUCDCPI" ValidationGroup="UCTabDatiCalcoloPI"
        Font-Size="Small" CssClass="errorBox" />--%>
    <asp:ValidationSummary runat="server" ID="tabDatiAgoPI" ValidationGroup="UCTabDatiAgoPI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlDatiCalcolo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo">
                    <li><a href="#dati_calcolo">Dati Calcolo
                        <asp:Image ID="imgDatiCalcolo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo707">
                    <li><a href="#dati_calcolo_707">Dati Calcolo 707
                        <asp:Image ID="imgDatiCalcolo707" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiFondo">
                    <li><a href="#dati_fondo">Dati Fondo
                        <asp:Image ID="imgDatiFondo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiAgo">
                    <li><a href="#dati_ago" runat="server"><span id="spanDatiAgo" runat="server">Dati Ago </span>
                        <asp:Image ID="imgDatiAgo" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabArt11e14">
                    <li><a href="#art11_14">Art 11 e 14
                        <asp:Image ID="imgArt11_14" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabAnte67">
                    <li><a href="#ante67">Ante 67
                        <asp:Image ID="imgAnte67" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabSL336">
                    <li><a href="#sl336D">S.L. 336
                        <asp:Image ID="imgSL336" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabStorico">
                    <li><a href="#storico">Storico GP
                        <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
                <div id="dati_calcolo" class="tab_content">
                    <UCDC:UCDatiCalcoloEL_TT_ET ID="ucDatiCalcoloEL_TT_ET" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo"
                        OnShowPopUp="event_ucShowPopUp" OnHidePopUp="event_ucHidePopUp" />
                    <UCDCVL:UCDatiCalcoloVL_FS_PT ID="ucDatiCalcoloVL_FS_PT" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo"
                        OnShowPopUp="event_ucShowPopUp" OnHidePopUp="event_ucHidePopUp" />
                    <UCDCDZ:UCDatiCalcoloDZ ID="ucDatiCalcoloDZ" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo" />
                    <UCDCPM:UCDatiCalcoloPM ID="ucDatiCalcoloPM" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo" />
                    <%--                    <UCDCPI:UCDatiCalcoloPI ID="ucDatiCalcoloPI" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo" />--%>
                </div>
                <div id="dati_fondo" class="tab_content">
                    <UCDFGAS:UCDatiFondoGAS_ES ID="ucDatiFondoGAS_ES" runat="server" OnShowAvviso="event_ucShowAvvisoDatiFondoGAS"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiFondoGAS" />

                    <UCDFPI:UCDatiFondo_PI
                        ID="ucDatiFondo_PI"
                        runat="server"
                        OnShowAvviso="event_ucShowAvvisoDatiFondoPI"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiFondoPI"
                        OnHideAvviso="event_ucHideAvviso" />

                </div>
                <div id="dati_ago" class="tab_content">
                    <UCDAGAS_ES:UCDatiAgoGAS_ES ID="ucDatiAgoGAS_ES" runat="server" OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiAgoGAS" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAgoGAS"
                        OnShowPopUp="event_ucShowPopUp" OnHidePopUp="event_ucHidePopUp" />

                    <UCDA_PI:UCDatiAgo_PI ID="ucDatiAgo_PI" runat="server"
                        OnCaricaDatiCalcolo="event_ucCaricaDatiCalcolo"
                        OnShowAvviso="event_ucShowAvvisoDatiAgoPI"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAgoPI"
                        OnShowPopUp="event_ucShowPopUp"
                        OnHidePopUp="event_ucHidePopUp"
                        OnHideAvviso="event_ucHideAvviso" />


                    <UCDAAP:UcDatiAgoAltraPensione ID="ucDatiAgoAltraPensione" runat="server" OnShowAvviso="event_ucShowAvvisoDatiAgoAltraPensione"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAgoAltraPensione" />
                </div>


                <div id="art11_14" class="tab_content">
                    <UCA1114GAS_ES:UCArt11e14GAS_ES ID="ucArt11e14GAS_ES" runat="server" OnShowAvviso="event_ucShowAvvisoDatiArt11e14GAS"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiArt11e14GAS" />
                </div>
                <!--ante67-->
                <div id="ante67" class="tab_content">
                    <UCANT67:UCAnte67 ID="ucAnte67Es" runat="server" OnShowAvviso="event_ucShowAvvisoAnte67"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaAnte67" />
                </div>
                <!--S.L 336-->
                <div id="sl336D" class="tab_content">
                    <UCSL_336:UCSl336 ID="ucSL336" runat="server" OnShowAvviso="event_ucShowAvvisoSL336"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaSL336" />
                </div>
                <!--Storico-->
                <div id="storico" class="tab_content">
                    <UCDCS:UcDatiCalcoloStorico ID="ucStorico" runat="server" />
                </div>
                <!--Dati Calcolo 707-->
                <div id="dati_calcolo_707" class="tab_content">
                    <UCDC707:UcDatiCalcolo707 ID="ucDatiCalcolo707" runat="server"
                        OnShowAvviso="event_ucShowAvvisoDatiCalcolo707" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiCalcolo707" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td id="tdSave" style="text-align: right;">
                        <asp:Button ID="btnPopUpContributivi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Style="display: none" Text="Salva" Width="150px" OnClientClick="if(mainValidateForConfirm()){return ConfirmContributiviPage();}" />
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Text="Salva" Visible="false" Width="150px" OnClientClick="if(mainValidateForConfirm()) {return ConfirmPage();}" CssClass="tertiary" />
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Style="display: none" Visible="false" OnClick="SalvaDati_Click" Width="150px"
                            OnClientClick="mainValidate()" CssClass="tertiary"/>
                        <asp:Button ID="btnSalvaNoRiduzione" runat="server" CausesValidation="false" SkinID="btnAzione1"
                            Width="150px" OnClick="SalvaDati_Click" Text="Salva" Visible="true" OnClientClick="mainValidate()" CssClass="tertiary" />
                    </td>
                    <td id="tdBack" style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px"
                            OnClientClick="BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnTest1" runat="server" />
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            <asp:Label ID="lblEtaTit" runat="server"></asp:Label>
        </p>
    </div>
    <div id="dialog-ContributiviPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            Attenzione il Montante è inferiore all’Ammontare.<br />
            Confermare ?
        </p>
    </div>
</asp:Content>
