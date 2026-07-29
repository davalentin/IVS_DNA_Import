<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="PresaInCarico.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.PresaInCarico" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
    <script type="text/javascript" src="../Javascript/Utility.js"></script>
    <script type="text/javascript" src="../Javascript/jquery.blockUI.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(true);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <asp:Panel runat="server" ID="pnlPresaInCarico">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divWait" runat="server"
            class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabInvioCalcolo">
                    <li><a href="">Presa In Carico</a></li>
                </asp:Panel>
            </ul>
            <div runat="server" id="divIntro" class="tab_container" style="padding-top: 50px; padding-bottom: 50px">
                <table class="tabellaFormattazione" width="98%">
                    <tr>
                        <td style="width: 5%"></td>
                        <td align="justify" class="Row1">
                            <asp:Label ID="lblPresaInCarico" runat="server">
                                In questa sezione cliccando <b>'Presa In Carico'</b> sarà possibile prendere in
                                carico la domanda assegnata all'operatore avente matricola:
                            </asp:Label>
                            <asp:Label ID="lblMatricolaOperatore" runat="server"></asp:Label>
                            <asp:Label ID="lblAutomazione" runat="server" Visible="false" >
                                <p style="color:red;text-align:center"><b >ATTENZIONE TRATTASI DI DOMANDA AUTOMATIZZATA</b></p>
                                <br /> <br />
                                    Verificare che sia conclusa l’attività di automazione attraverso lo stato nella dashboard del Sistema Pensioni.
                                <br /><br/>
                                Cliccando <b>'Presa In Carico Domanda Automatizzata' </b>sarà possibile prendere in carico la domanda elaborata dal processo automatizzato.
                                Si ricorda che all'atto della presa in carico, la definizione della domanda dovrà essere completata esclusivamente dell'operatore.
                            </asp:Label>
                        </td>
                        <td style="width: 10%"></td>
                    </tr>
                </table>
            </div>
            <table width="100%" class="footer-actions-group position-right">
                <tr runat="server" id="rowMargin">
                    <td colspan="2"></td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPresaInCarico" CausesValidation="false" runat="server" Text="Presa In Carico"
                            SkinID="btnAzione1" Width="150px" OnClick="btnPresaInCarico_Click" OnClientClick="BlockUI()" CssClass="primary" />
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
                <tr style="height: 5px;">
                    <td colspan="2"></td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
