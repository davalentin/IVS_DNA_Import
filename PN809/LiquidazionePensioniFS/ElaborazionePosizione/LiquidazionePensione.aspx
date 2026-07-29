<%@ Page Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="LiquidazionePensione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.LiquidazionePensione" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiGenericiEL_TT_ET.ascx"
    TagName="UCDatiGenericiEL_TT_ET" TagPrefix="UCDgEL_TT_ET" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiGenericiVL_FS_PT.ascx"
    TagName="UCDatiGenericiVL_FS_PT" TagPrefix="UCDgVL_FS_PT" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiGenericiPI_GAS_CL.ascx"
    TagName="UCDatiGenericiPI_GAS_CL" TagPrefix="UCDgPI_GAS_CL" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiGenericiDZ_ES_PM.ascx"
    TagName="UCDatiGenericiDZ_ES_PM" TagPrefix="UCDgDZ_ES_PM" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiGenericiINPDAP.ascx"
    TagName="UCDatiGenericiINPDAP" TagPrefix="UCDGI" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiAssicurativiEL_TT_ET.ascx"
    TagName="UCDatiAssicurativiEL_TT_ET" TagPrefix="UCDaEL_TT_ET" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiAssicurativiVL_FS_PT.ascx"
    TagName="UCDatiAssicurativiVL_FS_PT" TagPrefix="UCDaVL_FS_PT" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiAssicurativiPI_GAS_CL.ascx"
    TagName="UCDatiAssicurativiPI_GAS_CL" TagPrefix="UCDaPI_GAS_CL" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiAssicurativiDZ_ES_PM.ascx"
    TagName="UCDatiAssicurativiDZ_ES_PM" TagPrefix="UCDaDZ_ES_PM" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCDatiAssicurativiINPDAP.ascx"
    TagName="UCDatiAssicurativiINPDAP" TagPrefix="UCDAI" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCOpzione.ascx" TagName="UCOpzione"
    TagPrefix="UCO" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCPrecedentePensione.ascx"
    TagName="UCPrecedentePensione" TagPrefix="UCPP" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCIstruttoria.ascx" TagName="UCIstruttoria"
    TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCBititolaritaINAIL.ascx" TagName="UCBititolaritaINAIL"
    TagPrefix="UCBI" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCLegge460.ascx" TagName="UCLegge460"
    TagPrefix="UCL460" %>
<%@ Register Src="~/UserControls/LiquidazionePensione/UCLiquidazionePensioneStorico.ascx"
    TagName="UCLPStorico" TagPrefix="UCLPS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
    <script type="text/javascript" src="../Javascript/Utility.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                if (activeTab == "#dati_assicurativiFS") {
                    SetCodiceNaturaDatiAssicurativi();
                    setDecorrenzaCalcoloFSPT();
                }
                if (activeTab == "#dati_generici")
                    SetCodiceNaturaDatiGenerici();
                return false;
            });

            setDecorrenzaCalcoloFSPT();
        });

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
                    flag = Page_ClientValidate('UCTabDatiAssicurativiFS');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabIstruttoria.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabIstruttoria');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabBititolaritaINAIL.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabBititolarita');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabLegge460.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabLegge460');
                }
            }

            return flag;
        }

        function SetCodiceNaturaDatiAssicurativi() {

            var codNatura1 = getDDLCodNatura1Value(); //prendo il valore dalle ddl CodNatura
            $($("table[id*=gvRecordFondo] select[id*=ddlCodNatura1]")).val(codNatura1); //setto il valore recuperato sulla ddl
            var codNatura2 = getDDLCodNatura2Value();
            $($("table[id*=gvRecordFondo] select[id*=ddlCodNatura2]")).val(codNatura2);
            var codNatura3 = getDDLCodNatura3Value();
            $($("table[id*=gvRecordFondo] select[id*=ddlCodNatura3]")).val(codNatura3);
        }

        function SetCodiceNaturaDatiGenerici() {
            var codNatura1 = getDDLCodNaturaValueCentralizzata("ddlCodNatura1"); //restituisce la cella dove andare a settare il valore
            setDDLCodNatura1Value(codNatura1) //setto il valore recuperato per la ddl
            var codNatura2 = getDDLCodNaturaValueCentralizzata("ddlCodNatura2");
            setDDLCodNatura2Value(codNatura2)
            var codNatura3 = getDDLCodNaturaValueCentralizzata("ddlCodNatura3");
            setDDLCodNatura3Value(codNatura3)
        }

        function SetTrimestreRequisiti() {
            var RequisitiAnte247;
            if (document.getElementById("<%=hdnIsContributiva.ClientID %>") != null && document.getElementById("<%=hdnIsContributiva.ClientID %>").value == "True") {
                if (typeof getRequisitiAnte247 === 'function')
                    RequisitiAnte247 = getRequisitiAnte247();
                if (document.getElementById("<%=hdnRequisitiAnte247Trimestre.ClientID %>") != null && document.getElementById("<%=hdnRequisitiAnte247Anno.ClientID %>") != null) {
                    setDDLRequisitiAnte247Value(document.getElementById("<%=hdnRequisitiAnte247Trimestre.ClientID %>").value, RequisitiAnte247);
                    setTxtRequisitiAnte247Value(document.getElementById("<%=hdnRequisitiAnte247Anno.ClientID %>").value, RequisitiAnte247);
                }
            }                    
        }

        function AbilitaTab() {
            //ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'block';
            if (typeof ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione !== 'undefined'
                && ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione !== null) {

                ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'block';
            }
        }

        function DisabilitaTab() {
           //ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'none';
            if (typeof ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione !== 'undefined'
                && ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione !== null) {

                ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'none';
            }
        }

        function setDecorrenzaCalcoloFSPT() {
            try {
                valorizzaDecorrenzaCalcoloPerBonus(getDataInizioBonus());
            }
            catch (err) { }
        }

        function chkDimissioniAnte97OnChange(checkbox) {
            try {
                if (checkbox && checkbox.length != 0) {
                    if ($(checkbox).is(':checked'))
                        setTipoCalcoloRetributivoDisabled();
                    else
                        unlockTipoCalcolo();
                }
            }
            catch (err) {
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_generici" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiGenericiVS" ValidationGroup="UCTabDatiGenerici"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrecedentePensioneVS" ValidationGroup="UCTabPrecedentePensione"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabIstruttoriaVS" ValidationGroup="UCTabIstruttoria"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiAssicurativiVS" ValidationGroup="UCTabDatiAssicurativiFS"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabBititolarita" ValidationGroup="UCTabBititolarita"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabBititolaritaGrid" ValidationGroup="UCTabINAIL"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabLegge460" ValidationGroup="UCTabLegge460"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="RecordFondo" ValidationGroup="UCRecordFondo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="GrigliaRipartizioni" ValidationGroup="GrigliaRipartizioni"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlLiquidazionePensione">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth md">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDatiGenerici">
                    <li><a href="#dati_generici">Dati Generici
                        <asp:Image ID="imgDatiGenerici" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiAssicurativi">
                    <li><a href="#dati_assicurativiFS">Dati Assicurativi
                        <asp:Image ID="imgDatiAssicurativi" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabIstruttoria">
                    <li><a href="#istruttoria">Istruttoria
                        <asp:Image ID="imgIstruttoria" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <!-- Tab non utilizzata dai FS -->
                <asp:Panel runat="server" ID="pnlTabOpzione">
                    <li><a href="#opzione">Opzione<asp:Image ID="imgOpzione" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrecedentePensione">
                    <li><a href="#precedente_pensione">Precedente Pensione
                        <asp:Image ID="imgPrecedentePensione" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabBititolaritaINAIL">
                    <li><a href="#bititolarita_inail">Bitit. / Inail
                        <asp:Image ID="imgBititolaritaINAIL" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabLegge460">
                    <li><a href="#legge460">Legge 4/60
                        <asp:Image ID="imgLegge460" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabStorico">
                    <li><a href="#storico">Storico GP
                        <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
                <div id="dati_generici" class="tab_content">
                    <UCDgEL_TT_ET:UCDatiGenericiEL_TT_ET runat="server" ID="ucDatiGenericiEL_TT_ET" OnShowAvviso="event_ucShowAvvisoDatiGenerici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici" OnShowError="event_ucShowErrorDatiGenerici" Visible="false" OnShowAvvisoTrattenutaFondoCredito="event_ucShowAvvisoTrattenutaFondoCredito" />
                    <UCDgVL_FS_PT:UCDatiGenericiVL_FS_PT runat="server" ID="UCDatiGenericiVL_FS_PT" OnShowAvviso="event_ucShowAvvisoDatiGenerici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici" OnShowError="event_ucShowErrorDatiGenerici" Visible="false"  OnShowAvvisoTrattenutaFondoCredito="event_ucShowAvvisoTrattenutaFondoCredito" />
                    <UCDgPI_GAS_CL:UCDatiGenericiPI_GAS_CL runat="server" ID="ucDatiGenericiPI_GAS_CL"
                        OnShowAvviso="event_ucShowAvvisoDatiGenerici" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici"
                        OnShowError="event_ucShowErrorDatiGenerici"  Visible="false" OnShowAvvisoTrattenutaFondoCredito="event_ucShowAvvisoTrattenutaFondoCredito" />
                    <UCDgDZ_ES_PM:UCDatiGenericiDZ_ES_PM runat="server" ID="ucDatiGenericiDZ_ES_PM" OnShowAvviso="event_ucShowAvvisoDatiGenerici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici" OnShowError="event_ucShowErrorDatiGenerici" Visible="false" OnShowAvvisoTrattenutaFondoCredito="event_ucShowAvvisoTrattenutaFondoCredito" />
                    <UCDGI:UCDatiGenericiINPDAP runat="server" ID="ucDatiGenericiINPDAP" OnShowAvviso="event_ucShowAvvisoDatiGenerici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiGenerici" OnShowError="event_ucShowErrorDatiGenerici"
                        Visible="false" />
                </div>
                <div id="dati_assicurativiFS" class="tab_content">
                    <UCDaEL_TT_ET:UCDatiAssicurativiEL_TT_ET runat="server" ID="ucDatiAssicurativiEL_TT_ET"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowAvviso="event_ucShowAvvisoDatiAssicurativi" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi"
                        Visible="false" />
                    <UCDaVL_FS_PT:UCDatiAssicurativiVL_FS_PT runat="server" ID="ucDatiAssicurativiVL_FS_PT"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowAvviso="event_ucShowAvvisoDatiAssicurativi" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi"
                        Visible="false" />
                    <UCDaPI_GAS_CL:UCDatiAssicurativiPI_GAS_CL runat="server" ID="ucDatiAssicurativiPI_GAS_CL"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowAvviso="event_ucShowAvvisoDatiAssicurativi" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi"
                        OnManageCodiceNoCalcoloPIU="ManageCodiceNoCalcoloPIU" OnManageExCombattente="ManageExCombattente"
                        Visible="false" />
                    <UCDaDZ_ES_PM:UCDatiAssicurativiDZ_ES_PM runat="server" ID="ucDatiAssicurativiDZ_ES_PM"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnShowAvviso="event_ucShowAvvisoDatiAssicurativi" OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi"
                        Visible="false" />
                    <UCDAI:UCDatiAssicurativiINPDAP runat="server" ID="ucDatiAssicurativiINPDAP" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnShowAvviso="event_ucShowAvvisoDatiAssicurativi"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiAssicurativi" Visible="false" />
                </div>
                <div id="istruttoria" class="tab_content">
                    <UCI:UCIstruttoria runat="server" ID="ucIstruttoria" OnShowAvviso="event_ucShowAvviso"
                        OnHideAvviso="event_ucHideAvviso" />
                </div>
                <!--Tab non presente per i FS-->
                <div id="opzione" class="tab_content">
                    <UCO:UCOpzione runat="server" ID="ucOpzione" />
                </div>
                <div id="precedente_pensione" class="tab_content">
                    <UCPP:UCPrecedentePensione runat="server" ID="ucPrecedentePensione" OnShowAvviso="event_ucShowAvvisoPrecedentePensione"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaPrecedentePensione" />
                </div>
                <div id="bititolarita_inail" class="tab_content">
                    <UCBI:UCBititolaritaINAIL runat="server" ID="ucBititolaritaInail" OnShowAvviso="event_ucShowAvvisoBititolaritaInail"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaBititolaritaInail" />
                </div>
                <div id="legge460" class="tab_content">
                    <UCL460:UCLegge460 runat="server" ID="ucLegge460" OnShowAvviso="event_ucShowAvvisoLegge460"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaLegge460" Visible="false" />
                </div>
                <!--Storico-->
                <div id="storico" class="tab_content">
                    <UCLPS:UCLPStorico ID="ucStorico" runat="server" Visible="false" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalvaLiquidazionePensione" runat="server" Text="Salva Tutto" SkinID="btnAzione1"
                            CausesValidation="false" Width="180px" OnClick="SalvaLiquidazionePensione_Click"
                            OnClientClick="try{setAllDisabledControlsInHiddenField();}catch(err){} mainValidate()" CssClass="tertiary ml-0" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="180px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="hdnIsContributiva" />
    <asp:HiddenField runat="server" ID="hdnRequisitiAnte247Trimestre" />
    <asp:HiddenField runat="server" ID="hdnRequisitiAnte247Anno" />
</asp:Content>
