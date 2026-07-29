<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Oneri.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Oneri" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Oneri/UCOneri.ascx" TagName="UCOneri" TagPrefix="UCO" %>
<%@ Register Src="~/UserControls/Oneri/UCPrepensionamento.ascx" TagName="UCPrepensionamento"
    TagPrefix="UCP" %>
<%@ Register Src="~/UserControls/Oneri/UCOneriStoricoGP.ascx" TagName="UCOneriStoricoGP"
    TagPrefix="UCOSGP" %>
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
            if (document.getElementById("<%=pnlTabOneri.ClientID%>") != null)
                flag = Page_ClientValidate('UCTabOneri');
            if (flag) {
                if (document.getElementById("<%=pnlTabPrepensionamento.ClientID %>") != null) {
                    flag = Page_ClientValidate('UCTabPrepensionamento');
                }
            }
            return flag;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#exCombattente" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabOneri" ValidationGroup="UCTabOneri"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrepensionamento" ValidationGroup="UCTabPrepensionamento"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlOneri">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth md">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabOneri">
                    <li><a href="#oneri">Oneri
                        <asp:Image ID="imgOneri" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrepensionamento">
                    <li><a href="#prepensionamento">Prepensionamento
                        <asp:Image ID="imgPrepensionamento" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabStorico">
                    <li><a href="#storico">Storico GP
                        <asp:Image ID="imgStorico" ImageAlign="Top" runat="server" Visible="false" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 150px;">
                <div id="oneri" class="tab_content">
                    <UCO:UCOneri runat="server" ID="ucOneri" OnShowAvviso="event_ucShowAvvisoOneri" OnShowAvvisoElimina="event_ucShowAvvisoEliminaOneri"
                        OnSalvaOnere="event_ucSalvaOnere" OnAnnullaOnere="event_ucAnnullaOnere" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" />
                </div>
                <div id="prepensionamento" class="tab_content">
                    <UCP:UCPrepensionamento runat="server" ID="ucPrepensionamento" OnShowAvviso="event_ucShowAvvisoPrepensionamento"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaPrepensionamento" />
                </div>
                <div id="storico" class="tab_content">
                    <UCOSGP:UCOneriStoricoGP runat="server" ID="ucOneriStoricoGP" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="160px" OnClick="SalvaOneri_Click" OnClientClick="mainValidate()" CssClass="tertiary"/>
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="160px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
