<%@ Page Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="LiquidazionePensioneCi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.LiquidazionePensioneCi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCDatiGenerici.ascx" TagName="UCDatiGenerici"
    TagPrefix="UCDG" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCOpzioneCi.ascx" TagName="UCOpzione"
    TagPrefix="UCO" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCPrecedentePensioneCi.ascx"
    TagName="UCPrecedentePensione" TagPrefix="UCPP" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCDatiAssicurativiCi.ascx"
    TagName="UCDatiAssicurativi" TagPrefix="UCDA" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCIstruttoriaCi.ascx" TagName="UCIstruttoria"
    TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/LiquidazionePensioneCi/UCInailCi.ascx" TagName="UCInail"
    TagPrefix="UCII" %>
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
                return false;
            });
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

            return flag;
        }

        function AbilitaTab() {
            ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'block';
        }

        function DisabilitaTab() {
            ctl00_ContentPlaceHolder1_pnlTabPrecedentePensione.style.display = 'none';
        }

        function ConfirmPage() {
            var ddl = document.getElementById('ctl00_ContentPlaceHolder1_ucIstruttoria_ddlRiduzioneRetributiva');
            if (ddl != null) {
                var selectedValue = ddl.options[ddl.selectedIndex].value;
                if (selectedValue.toUpperCase() == 'SI')
                    document.getElementById('<%= btnSalvaLiquidazionePensioneCi.ClientID %>').click();
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
                        document.getElementById('<%= btnSalvaLiquidazionePensioneCi.ClientID %>').click();
                        return true;
                    }
                }
            });
        });
        
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
    <asp:ValidationSummary runat="server" ID="tabDatiAssicurativiVS" ValidationGroup="UCTabDatiAssicurativi"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabIstruttoriaVS" ValidationGroup="UCTabIstruttoria"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabInailVS" ValidationGroup="UCTabGridINAIL"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlLiquidazionePensioneCi">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDatiGenerici">
                    <li><a href="#dati_generici">Generici
                        <asp:Image ID="imgDatiGenerici" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiAssicurativi">
                    <li><a href="#dati_assicurativi">Assicurativi
                        <asp:Image ID="imgDatiAssicurativi" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabOpzione">
                    <li><a href="#opzione">Opzione
                        <asp:Image ID="imgOpzione" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrecedentePensione">
                    <li><a href="#precedente_pensione">Pens. Prov.
                        <asp:Image ID="imgPrecedentePensione" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabIstruttoria">
                    <li><a href="#istruttoria">Istruttoria
                        <asp:Image ID="imgIstruttoria" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabInail">
                    <li><a href="#inail">Inail/Accomp.
                        <asp:Image ID="imgInail" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
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
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiIstruttoria" />
                </div>
                <div id="inail" class="tab_content">
                    <UCII:UCInail runat="server" ID="ucInail" OnShowAvviso="event_ucShowAvvisoDatiInail"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDatiInail" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <%--<asp:Button ID="btnSalvaLiquidazionePensioneCi" runat="server" Text="Salva Tutto"
                            SkinID="btnAzione1" CausesValidation="false" Width="150px" OnClick="SalvaLiquidazionePensioneCi_Click"
                            OnClientClick="mainValidate()" />--%>
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Text="Salva Tutto" Visible="false" Width="170px" OnClientClick="if(mainValidateForConfirm()) {return ConfirmPage();}" CssClass="tertiary ml-0" />
                        <asp:Button ID="btnSalvaLiquidazionePensioneCi" runat="server" Text="Salva Tutto"
                            SkinID="btnAzione1" CausesValidation="false" Style="display: none" Visible="false"
                            OnClick="SalvaLiquidazionePensioneCi_Click" Width="170px" OnClientClick="mainValidate()" CssClass="tertiary ml-0" />
                        <asp:Button ID="btnSalvaLiquidazionePensioneCiNoRiduzione" runat="server" CausesValidation="false"
                            SkinID="btnAzione1" Width="170px" OnClick="SalvaLiquidazionePensioneCi_Click"
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
</asp:Content>
