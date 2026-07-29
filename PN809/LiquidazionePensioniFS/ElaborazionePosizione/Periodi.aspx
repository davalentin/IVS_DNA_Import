<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Periodi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Periodi" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Periodi/UCPeriodi.ascx" TagName="UCPeriodi" TagPrefix="UCP" %>
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
                return false;
            });
        });

        function validatePage() {
            var flag = true;

            if (document.getElementById("<%=pnlTabPeriodi.ClientID%>") != null) {
                flag = Page_ClientValidate('UCPeriodi');

                if (flag)
                    flag = Page_ClientValidate('UCPeriodiGrid');
            }
            return flag;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#periodi" runat="server" />
    <asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCPeriodi"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummaryGrid" ValidationGroup="UCPeriodiGrid"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlPeriodi">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabPeriodi">
                    <li><a href="#periodi">Periodi
                        <asp:Image ID="imgPeriodi" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container">
                <div id="periodi" class="tab_content">
                    <UCP:UCPeriodi runat="server" ID="ucPeriodi" OnShowAvviso="event_ucShowAvviso" OnHideAvviso="event_ucHideAvviso"
                        OnAggiornaSemaforo="event_ucAggiornaSemaforo" OnAbilitaPulsanti="event_ucAbilitaPulsanti"
                        OnDisabilitaPulsanti="event_ucDisabilitaPulsanti" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalvaPeriodi" runat="server" Text="Salva Tutto" SkinID="btnAzione1"
                            CausesValidation="false" Width="180px" OnClick="SalvaPeriodi_Click" OnClientClick="mainValidate()" CssClass="tertiary" />
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
</asp:Content>
