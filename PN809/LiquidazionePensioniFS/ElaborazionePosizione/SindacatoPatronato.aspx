<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="SindacatoPatronato.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SindacatoPatronato" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/SindacatoPatronato/UCSindacato.ascx" TagName="UCSindacato"
    TagPrefix="UCS" %>
<%@ Register Src="~/UserControls/SindacatoPatronato/UCPatronato.ascx" TagName="UCPatronato"
    TagPrefix="UCP" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
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
            if (document.getElementById("<%=pnlTabSindacato.ClientID%>") != null) {
                flag = Page_ClientValidate('UCSindacatoPatronato');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabPatronato.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCSindacatoPatronato');
                }
            }

            return flag;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#sindacato" runat="server" />
    <asp:ValidationSummary runat="server" ID="validateSummary" ValidationGroup="UCSindacatoPatronato"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlSindacatoPatronato">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabSindacato">
                    <li><a href="#sindacato">Sindacato
                        <asp:Image ID="imgSindacato" ImageAlign="Top" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/rosso_tab.png" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPatronato">
                    <li><a href="#patronato">Patronato
                        <asp:Image ID="imgPatronato" ImageAlign="Top" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/rosso_tab.png" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 80px;">
                <div id="sindacato" class="tab_content">
                    <UCS:UCSindacato runat="server" ID="ucSindacato" />
                </div>
                <div id="patronato" class="tab_content">
                    <UCP:UCPatronato runat="server" ID="ucPatronato" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="150px" OnClick="SalvaDati_Click" OnClientClick="mainValidate()" CssClass="tertiary" />
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
</asp:Content>
