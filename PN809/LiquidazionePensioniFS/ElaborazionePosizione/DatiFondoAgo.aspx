<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master" AutoEventWireup="true" CodeBehind="DatiFondoAgo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiFondoAgo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DatiFondoAgo/UCRegistrazioniFondo.ascx" TagName="UCRegistrazioni"
    TagPrefix="UCREG" %>
<%@ Register Src="~/UserControls/DatiFondoAgo/UCDatiFondo.ascx" TagName="UCDatiFondo"
    TagPrefix="UCDF" %>
<%@ Register Src="~/UserControls/DatiFondoAgo/UCDatiCalcolo.ascx" TagName="UCDatiCalcolo"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiFondoAgo/UCArticolo2.ascx" TagName="UCArticolo2"
    TagPrefix="UCART2" %>
<%@ Register Src="~/UserControls/DatiFondoAgo/UCPrivilegiate.ascx" TagName="UCPrivilegiate"
    TagPrefix="UCPRI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
    <script type="text/javascript" src="Javascript/validate2.js"></script>
    <script type="text/javascript" src="Javascript/Utility.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);

                valorizzaDecorrenzaRegistrazione();

                return false;
            });

            valorizzaDecorrenzaRegistrazione();
        });

        function validatePage() {
            var flag = true;

            if (document.getElementById("<%=pnlTabDatiFondo.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiFondo');
            }
            if (flag && document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcolo');
            }
            if (flag && document.getElementById("<%=pnlTabPrivilegiate.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabPrivilegiate');
            }
            if (flag && document.getElementById("<%=pnlTabArticolo2.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabArticolo2');
            }
            return flag;
        }

        function valorizzaDecorrenzaRegistrazione() {
            var decorrenza = getDecorrenzaRegistrazione();
            $("span[id*='lblDecorrenzaRegistrazione']").each(function () {
                $(this).text(decorrenza);
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_Registrazioni"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiFondo" ValidationGroup="UCTabDatiFondo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcolo" ValidationGroup="UCTabDatiCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrivilegiate" ValidationGroup="UCTabPrivilegiate"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabArticolo2" ValidationGroup="UCTabArticolo2"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlLiquidazionePensione">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="width: 720px; margin: 0 auto; margin-top: 5px; float: left;">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabRegistrazioniFondo">
                    <li><a href="#dati_Registrazioni">Registrazioni Fondo
                        <asp:Image ID="imgRegistrazioniFondo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiFondo">
                    <li><a href="#dati_Fondo">Dati Fondo
                        <asp:Image ID="imgDatiFondo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo">
                    <li><a href="#dati_Calcolo">Dati Calcolo
                        <asp:Image ID="imgDatiCalcolo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrivilegiate">
                    <li><a href="#dati_Privilegiate">Privilegiate
                        <asp:Image ID="imgPrivilegiate" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabArticolo2">
                    <li><a href="#dati_Articolo2">Art.2 Comma 12 L.335
                        <asp:Image ID="imgArticolo2" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
                <div id="dati_Registrazioni" class="tab_content">
                    <UCREG:UCRegistrazioni runat="server" ID="ucRegistrazioniFondo" OnShowAvviso="event_ucShowAvviso"
                        OnShowPulsanteSalva="event_ucShowPulsanteSalva" OnRecordSelezionato="event_ucRecordSelezionato" />
                </div>
                <div id="dati_Fondo" class="tab_content">
                    <UCDF:UCDatiFondo runat="server" ID="ucDatiFondo" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiFondo="event_ucUpdateSemaforoDatiFondo" />
                </div>
                <div id="dati_Calcolo" class="tab_content">
                    <UCDC:UCDatiCalcolo runat="server" ID="ucDatiCalcolo" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiCalcolo="event_ucUpdateSemaforoDatiCalcolo" />
                </div>
                <div id="dati_Privilegiate" class="tab_content">
                    <UCPRI:UCPrivilegiate runat="server" ID="ucPrivilegiate" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiPrivilegiate="event_ucUpdateSemaforoDatiPrivilegiate" />
                </div>
                <div id="dati_Articolo2" class="tab_content">
                    <UCART2:UCArticolo2 runat="server" ID="ucArticolo2" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiArticolo2="event_ucUpdateSemaforoDatiArticolo2" />
                </div>
            </div>
            <table width="100%">
                <tr>
                    <td style="text-align: center">
                        <asp:Button ID="btnSalvaFondo" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="150px" OnClick="SalvaFondo_Click" OnClientClick="mainValidate()" Visible="false" Enabled="true" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="150px" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
