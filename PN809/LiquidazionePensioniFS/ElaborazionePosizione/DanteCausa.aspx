<%@ Page Language="C#" Title="" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DanteCausa.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DanteCausa" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DanteCausa/UCDanteAltraPensioneDC.ascx" TagName="UCAltraPensioneDC"
    TagPrefix="UCDAP" %>
<%@ Register Src="~/UserControls/DanteCausa/UCDanteAnagrafica.ascx" TagName="UCDanteAnagrafica"
    TagPrefix="UCDA" %>
<%@ Register Src="~/UserControls/DanteCausa/UCDanteDatiPensioneCI.ascx" TagName="UCDanteDatiPensioneCI"
    TagPrefix="UCDPCI" %>
<%@ Register Src="~/UserControls/DanteCausa/UCDantePensioneDiretta.ascx" TagName="UCDantePensioneDiretta"
    TagPrefix="UCDPD" %>
<%@ Register Src="~/UserControls/DanteCausa/UCDanteSentenza49593.ascx" TagName="UCDanteSentenza49593"
    TagPrefix="UCDSE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
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
            //scattano i validatori delle tab se visibili
            var flag = Page_ClientValidate('UCDanteAnagrafica');

            if (document.getElementById("<%=pnlTabPensioneD.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate('UCPensioneDirettaDC');
            }
            if (document.getElementById("<%=pnlTabAltraPensioneDC.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate('UCAltrePensioniDC');
            }
            if (document.getElementById("<%=pnlTabPensioneCI.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate('UCPensioniCI');
            }
            if (document.getElementById("<%=pnlTabSentenza49593.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate('UCDanteSentenza49593');
            }
            if (document.getElementById("<%=pnlTabSentenza49593.ClientID%>") != null) {
                if (flag)
                    flag = Page_ClientValidate('UCTabSentenza495');
            }


            return flag;
        }

        function ConfirmPage() {
            var dataMatrimonio = document.getElementById("ctl00_ContentPlaceHolder1_ucanagrafica_txtDataMatrimonio") != null ? document.getElementById("ctl00_ContentPlaceHolder1_ucanagrafica_txtDataMatrimonio").value : "";
            var dataNascitaDC = document.getElementById("ctl00_ContentPlaceHolder1_ucanagrafica_lblDataNascitaAnagrafica").innerText;
            var dataNascitaTitolare = document.getElementById("ctl00_ContentPlaceHolder1_ucanagrafica_hdnDataNascitaContitolareConiuge").value;
            var flag = false;
            if (dataMatrimonio !== undefined && dataMatrimonio != "") {
                var dateApp = dataMatrimonio.split("/");
                var date1 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                if (dataNascitaDC !== undefined && dataNascitaDC != "") {
                    dateApp = dataNascitaDC.split("/");
                    var date2 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                    date2.setFullYear(date2.getFullYear() + 16);
                    if (date1 < date2)
                        flag = true;
                }
                if (!flag) {
                    if (dataNascitaTitolare !== undefined && dataNascitaTitolare != "") {
                        dateApp = dataNascitaTitolare.split("/");
                        var date2 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                        date2.setFullYear(date2.getFullYear() + 16);
                        if (date1 < date2)
                            flag = true;
                    }
                }
            }

            if (!flag)
                document.getElementById('<%= btnSalva.ClientID %>').click();
            else
                $('#dialog-confirmPage').dialog('open');

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
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#anagrafica" runat="server" />
    <asp:Panel runat="server" ID="pnlDanteCausa">
        <asp:ValidationSummary runat="server" ID="tabAnagraficaDC" ValidationGroup="UCDanteAnagrafica"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabPensioneDirDC" ValidationGroup="UCPensioneDirettaDC"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabAltrePensioniDC" ValidationGroup="UCAltrePensioniDC"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabPensioniCI" ValidationGroup="UCPensioniCI"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabSentenze49593" ValidationGroup="UCDanteSentenze49593"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabSentenze49593Post" ValidationGroup="UCDanteSentenze49593Post"
            Font-Size="Small" CssClass="errorBox" />
        <asp:ValidationSummary runat="server" ID="tabSentenza495" ValidationGroup="UCTabSentenza495"
            Font-Size="Small" CssClass="errorBox" />
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabAnagrafica">
                    <li><a href="#anagrafica">Anagrafica
                        <asp:Image ID="imgAnagrafica" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPensioneD">
                    <li><a href="#pensionediretta">Diretta
                        <asp:Image ID="imgPensioneD" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabAltraPensioneDC">
                    <li><a href="#pensionedc">Altra Pensione
                        <asp:Image ID="imgPensioneDC" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPensioneCI">
                    <li><a href="#pensioneci">Pensione CI
                        <asp:Image ID="imgPensioneCI" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabSentenza49593">
                    <li><a href="#sentenza49593">Sentenza 495
                        <asp:Image ID="imgSentenza49593" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container">
                <div id="anagrafica" class="tab_content">
                    <UCDA:UCDanteAnagrafica runat="server" ID="ucanagrafica" onerrortutore="event_ErrorUcAnagrafica"
                        onnoterrortutore="event_NotErrorucanagrafica" OnShowAvviso="event_ucShowAvvisoDanteAnagrafica" />
                </div>
                <div id="pensionediretta" class="tab_content">
                    <UCDPD:UCDantePensioneDiretta runat="server" ID="ucpensionediretta" onerrortutore="event_Errorucpensionediretta"
                        onnoterrortutore="event_NotErrorucpensionediretta" OnShowAvviso="event_ucShowAvvisoDantePensioneDir" />
                </div>
                <div id="pensionedc" class="tab_content">
                    <UCDAP:UCAltraPensioneDC runat="server" ID="ucaltrapensionedc" onerrortutore="event_Errorucaltrapensionedc"
                        onnoterrortutore="event_NotErrorucaltrapensionedc" OnShowAvviso="event_ucShowAvvisoDanteAltraPensione" />
                </div>
                <div id="pensioneci" class="tab_content">
                    <UCDPCI:UCDanteDatiPensioneCI runat="server" ID="ucpensioneci" onerrortutore="event_Errorucdatipensioneci"
                        onnoterrortutore="event_NotErrorucdatipensioneci" OnShowAvviso="event_ucShowDanteDatiPensione" />
                </div>
                <div id="sentenza49593" class="tab_content">
                    <UCDSE:UCDanteSentenza49593 runat="server" ID="ucsentenza49593" onerrortutore="event_Errorucsentenza49593"
                        onnoterrortutore="event_NotErrorucsentenza49593" OnShowAvviso="event_ucShowDanteSentenza49593"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDanteSentenza49593" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Visible="false" Text="Salva" Width="150px" OnClientClick="if(mainValidateForConfirm()) {return ConfirmPage();}" />
                        <asp:Button ID="btnSalva" CausesValidation="false" runat="server" Text="Salva" SkinID="btnAzione1"
                            Width="150px" OnClick="btnSalva_Click" OnClientClick="mainValidate()" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px"
                            OnClientClick="BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            La data di matrimonio è inferiore al compimento dei 16 anni di età del Titolare
            e/o del dante causa. Confermi l'acquisizione?</p>
    </div>
    <asp:HiddenField runat="server" ID="HiddenFieldSedi" />
</asp:Content>
