<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Eliminazione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Eliminazione" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Eliminazione/UCEliminazione.ascx" TagName="UCEliminazione"
    TagPrefix="UCELM" %>
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
            if (document.getElementById("<%=pnlEliminazione.ClientID%>") != null)
                flag = Page_ClientValidate('UCELM');
            return flag;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#divEliminazione"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="VSUCElim" ValidationGroup="UCELM" Font-Size="Small"
        CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlEliminazione">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabEliminazione">
                    <li><a href="#divEliminazione">Eliminazione
                        <asp:Image ID="imgEliminazione" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 150px;">
                <div id="divEliminazione" class="tab_content">
                    <UCELM:UCEliminazione runat="server" ID="ucEliminazione" OnShowAvviso="event_ucShowAvvisoEliminazione"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaEliminazione" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalvaEliminazione" runat="server" Text="Salva" SkinID="btnAzione1"
                            CausesValidation="false" Width="190px" OnClick="SalvaEliminzione_Click" OnClientClick="mainValidate()" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="190px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
