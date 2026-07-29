<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Bititolarita.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Bititolarita" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Bititolarita/UCAltrePensioniAgo.ascx" TagName="UCAltrePensioniAGO"
    TagPrefix="UCAPAGO" %>
<%@ Register Src="~/UserControls/Bititolarita/UCAltrePensioniCi.ascx" TagName="UCAltrePensioniCI"
    TagPrefix="UCAPCI" %>
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
            if ((document.getElementById("<%=pnlTabAltrePensioniAGO.ClientID%>") != null && (document.getElementById("ctl00_ContentPlaceHolder1_ucAltrePensioniAGO_modalitaEditAltrePensioni").value == "true"))) {
                flag = Page_ClientValidate('UCTabAltrePensioni');
            }
            if (flag == true) {
                if ((document.getElementById("<%=pnlTabAltrePensioniCI.ClientID%>") != null && (document.getElementById("ctl00_ContentPlaceHolder1_ucAltrePensioniCI_modalitaEditAltrePensioni").value == "true"))) {
                    flag = Page_ClientValidate('UCTabAltrePensioni');
                }
            }
            return flag;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#altre_pensioni_AGO"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabAltrePensioni" ValidationGroup="UCTabAltrePensioni"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlBititolarita">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabAltrePensioniAGO" Visible="false">
                    <li><a href="#altre_pensioni_AGO">Altre Pensioni
                        <asp:Image ID="imgAltrePensioniAGO" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabAltrePensioniCI" Visible="false">
                    <li><a href="#altre_pensioni_CI">Altre Pensioni
                        <asp:Image ID="imgAltrePensioniCI" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 50px;">
                <div id="altre_pensioni_AGO" class="tab_content">
                    <UCAPAGO:UCAltrePensioniAGO runat="server" ID="ucAltrePensioniAGO" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnShowAvviso="event_ucShowAvvisoAltrePensioni"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaAltrePensioni" OnShowError="event_ucShowErrorAltrePensioni" />
                </div>
                <div id="altre_pensioni_CI" class="tab_content">
                    <UCAPCI:UCAltrePensioniCI runat="server" ID="ucAltrePensioniCI" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnShowAvviso="event_ucShowAvvisoAltrePensioni"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaAltrePensioni" OnShowError="event_ucShowErrorAltrePensioni" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" OnClientClick="mainValidate()"
                            CausesValidation="false" Width="150px" OnClick="SalvaBititolarita_Click" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px"
                            OnClientClick="aspnetForm.target ='_self'; BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
