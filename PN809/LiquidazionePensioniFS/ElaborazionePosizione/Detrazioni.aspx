<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Detrazioni.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Detrazioni" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Detrazioni/UCDetrazioni.ascx" TagName="UCDetrazioni"
    TagPrefix="UCD" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
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
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#detrazioni" runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCDetrazioni"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlDetrazioni">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDetrazioni">
                    <li><a href="#detrazioni">Detrazioni
                        <asp:Image ID="imgDetrazioni" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 250px;">
                <div id="detrazioni" class="tab_content">
                    <UCD:UCDetrazioni runat="server" ID="ucDetrazioni" OnAcquisizioneDetrazioni="event_ucAcquisizioneDetrazioni"
                        OnAggiornamentoDetrazioni="event_ucAggiornamentoDetrazioni" OnRicaricaSoggetti="event_ucRicaricaSoggetti" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group footer-actions-group--right footer-actions-group--detrazioni">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalvaDetrazioni" runat="server" Text="Salva" SkinID="btnAzione1"
                            CausesValidation="false" Width="150px" OnClick="SalvaDetrazioni" OnClientClick="mainValidate()" CssClass="primary" />
                    </td>
                    <td style="text-align: left;" class="none">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="aspnetForm.target ='_self'; BlockUI()"
                            PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
