<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="AggiornaIndennitaSpeciale.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AggiornaIndennSpec" %>

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

        function validatePage() {
            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#AggiornaIndennSpec"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCAggiornaIndennitaSpeciale"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlAggiornaIndennSpec">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divWait"
            runat="server" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabAggiornaIndennSpec">
                    <li><a href="">Aggiornamento Indennità Speciale</a></li>
                </asp:Panel>
            </ul>
            <div>
                <div class="tab_container">
                    <div runat="server" id="divIntro" style="background-position: right top; background-repeat: no-repeat;
                        background-image: url('../App_Themes/BlueINPS1/Images/engine.png');">
                        <table class="tabellaFormattazione" width="98%">
                            <tr>
                                <td colspan="3" class="full-grid" style="height: 70px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%">
                                </td>
                                <td align="justify" class="Row1">
                                    <label>
                                        In questa sezione cliccando <b>'Aggiorna'</b> sarà possibile effettuare l'aggiornamento
                                        finale per Indennità Speciale.</label>
                                </td>
                                <td style="width: 10%">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" class="full-grid" style="height: 125px;">
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div runat="server" id="divResult" style="background-position: right top; background-repeat: no-repeat;
                        background-image: url('../App_Themes/BlueINPS1/Images/engine.png');">
                        <table class="tabellaFormattazione" width="100%">
                            <tr style="height: 30px;">
                                <td colspan="4">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%">
                                </td>
                                <td align="center" colspan="2" class="Row1" style="width: 30%">
                                    <label style="color: #336699; font-weight: bold; font-size: larger">
                                        Risultato dell' aggiornamento Indennità Speciale</label>
                                </td>
                                <td style="width: 10%">
                                </td>
                            </tr>
                            <tr style="height: 40px;">
                                <td colspan="4">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%">
                                </td>
                                <td class="Row1" style="width: 25%">
                                    <label>
                                        Esito:</label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblEsito" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%">
                                </td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        Dettaglio Esito:</label>
                                </td>
                                <td style="width: 65%;">
                                    <asp:Label runat="server" ID="lblDettaglio" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%">
                                </td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        Stato Domanda:</label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblStato" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%">
                                </td>
                            </tr>
                            <tr style="height: 50px;">
                                <td colspan="4">
                                </td>
                            </tr>
                        </table>
                    </div>
                    <table width="100%">
                        <tr>
                            <td style="text-align: center;">
                                <asp:Button ID="btnAggIndennSpec" CausesValidation="false" runat="server" Text="Aggiorna"
                                    SkinID="btnAzione1" Width="150px" OnClick="btnAggIndennSpec_Click" OnClientClick="BlockUI()"  CssClass="ghost-update"/>
                            </td>
                        </tr>
                        <tr style="height: 5px;">
                            <td>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
