<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="MaggiorazioniEBenefici.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.MaggiorazioniEBenefici" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCExCombattente.ascx" TagName="UCExCombattente"
    TagPrefix="UCEC" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCBenefici.ascx" TagName="UCBenefici"
    TagPrefix="UCBen" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCLegge407.ascx" TagName="UCLegge407"
    TagPrefix="UCDL407" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCPrivilegiate.ascx" TagName="UCPrivilegiate"
    TagPrefix="UCDPR" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCArticolo2.ascx" TagName="UCArticolo2"
    TagPrefix="UCART2" %>
<%@ Register Src="~/UserControls/MaggiorazioniEBenefici/UCVittimeFS.ascx" TagName="UCVittimeFS"
    TagPrefix="UCVit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
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
            if (document.getElementById("<%=pnlTabExCombattente.ClientID%>") != null)
                flag = Page_ClientValidate('UCTabExCombattente');
            if (flag) {
                if (document.getElementById("<%=pnlTabBenefici.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabBenefici');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabLegge407.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabLegge407');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabPrivilegiate.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabPrivilegiate');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabArt2.ClientID%>") != null)
                    flag = Page_ClientValidate('UCTabArticolo2');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabVittime.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabVittime');
                }
            }

            return flag;
        }

        function GestisciSalvataggioConPopupPage() {


            var hdnVerificaAperturaPopupBenefici = getHiddenFieldVerificaAperturaPopup(); //dallo UCBenefici mi leggo l'hidden field per verificare quale tipo di beneficio è selezionato
            if (hdnVerificaAperturaPopupBenefici == "1") { //se è stato selezionato il beneficio Non Vedente, allora devo verificare se devo aprire effettivamente il popup

                var isPopupVisible = verificaAperturaPopupSettimanaContributiva(); //sullo UCBenefici mi verifico se devo aprire il Popup

                if (isPopupVisible == true) {
                    $('#dialog-confirmPage').dialog('open');
                    return false;
                }
                else {

                    RiabilitaCampi();
                    mainValidate();
                    return true;
                }


            } else {

                RiabilitaCampi();
                mainValidate();
                return true;

            }

        }

        $(function () {
            $('#dialog-confirmPage').dialog({
                autoOpen: false,

                show: 'blind',
                hide: 'blind',
                height: 230,
                width: 455,
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
                        setValueHiddenFieldVerificaAperturaPopup("0");
                        document.getElementById('<%= btnSalvaMaggiorazioniEBenefici.ClientID %>').click();
                        return true;
                    }
                }
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#exCombattente" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabExCombattente" ValidationGroup="UCTabExCombattente"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabBenefici" ValidationGroup="UCTabBenefici"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabLegge407" ValidationGroup="UCTabLegge407"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrivilegiate" ValidationGroup="UCTabPrivilegiate"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabArt2" ValidationGroup="UCTabArticolo2"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlMaggiorazioniEBenefici">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabExCombattente">
                    <li><a href="#exCombattente">Ex Combattente
                        <asp:Image ID="imgExCombattente" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabBenefici">
                    <li><a href="#benefici">Benefici
                        <asp:Image ID="imgBenefici" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabLegge407">
                    <li><a href="#dl407">D.L. 407
                        <asp:Image ID="imgLegge407" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrivilegiate">
                    <li><a href="#privilegiate">Privilegiate
                        <asp:Image ID="imgPrivilegiate" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabArt2">
                    <li><a href="#articolo2">Articolo 2
                        <asp:Image ID="imgArt2" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabVittime">
                    <li><a href="#vittime">Vittime
                        <asp:Image ID="imgVittime" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 150px;">
                <div id="exCombattente" class="tab_content">
                    <UCEC:UCExCombattente runat="server" ID="ucExCombattente" OnShowAvviso="event_ucShowAvvisoExCombattente"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaExCombattente" />
                </div>
                <div id="benefici" class="tab_content">
                    <UCBen:UCBenefici runat="server" ID="ucBenefici" OnShowAvviso="event_ucShowAvvisoBenefici"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaBenefici" />
                </div>
                <div id="dl407" class="tab_content">
                    <UCDL407:UCLegge407 runat="server" ID="ucLegge407" OnShowAvviso="event_ucShowAvvisoDL407"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDL407" />
                </div>
                <div id="privilegiate" class="tab_content">
                    <UCDPR:UCPrivilegiate runat="server" ID="ucPrivilegiate" OnShowAvviso="event_ucShowAvvisoPrivilegiate"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaPrivilegiate" />
                </div>
                <div id="articolo2" class="tab_content">
                    <UCART2:UCArticolo2 runat="server" ID="ucArticolo2" OnShowAvviso="event_ucShowAvvisoArticolo2"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaArticolo2" />
                </div>
                <div id="vittime" class="tab_content">
                    <UCVit:UCVittimeFS runat="server" ID="ucVittimeFS" OnShowAvviso="event_ucShowAvvisoVittime"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaVittime" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalvaMaggiorazioniEBenefici" runat="server" Text="Salva" SkinID="btnAzione1"
                            CausesValidation="false" Width="180px" OnClick="SalvaMaggiorazioniEBenefici_Click"
                            OnClientClick="return GestisciSalvataggioConPopupPage();" CssClass="tertiary" />
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
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            Attenzione: verificare la corretta attribuzione delle settimane di beneficio sulla
            quota di pensione calcolata con il sistema contributivo. Si rinvia al messaggio
            2114/2018.
        </p>
    </div>
</asp:Content>
