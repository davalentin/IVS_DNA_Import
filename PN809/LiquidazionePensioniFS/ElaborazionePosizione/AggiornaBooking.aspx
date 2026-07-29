<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="AggiornaBooking.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AggiornaBooking" %>

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

        function CreatePopUpEsitoPrenotazione() {
            // jQuery UI Dialog
            var result;
            $('#esitoPrenotazione').dialog({
                autoOpen: false,
                width: 720,
                height: 380,
                modal: true,
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'hidden'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    "Chiudi": function () {
                        $(this).dialog("close");
                        result = false;
                    }
                }
            });
            $("#esitoPrenotazione").parent().appendTo($("form:first"));
        }

        function ShowPopUpEsitoPrenotazione() {
            CreatePopUpEsitoPrenotazione();
            $('#esitoPrenotazione').dialog('open');
        }  
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="esitoPrenotazione" style="display: none;" title="Esito Prenotazione Bonus">
        <asp:GridView ID="gvEsitoPrenotazione" runat="server" BorderWidth="1" BorderColor="Black"
            AutoGenerateColumns="false" AllowSorting="true" Visible="true" Width="100% "
            SkinID="grdElenco1" AllowPaging="false" PageSize="15">
            <EmptyDataTemplate>
                <center>
                    <asp:Label ID="lblNoData" runat="server" Text="Nessuna Elaborazione Prenotata." SkinID="lblNoData"
                        Visible="true"></asp:Label>
                </center>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="Anno Prenotazione" DataField="AnnoRichiesto" Visible="true"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                    ItemStyle-CssClass="TblRecordset3" />
                <asp:BoundField HeaderText="Esito Prenotazione" DataField="DescrizioneEsito" Visible="true"
                    ItemStyle-HorizontalAlign="Center" ItemStyle-Width="50%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                    ItemStyle-CssClass="TblRecordset3" />
            </Columns>
        </asp:GridView>
    </div>
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#aggiornaBooking"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCAggiornaBooking"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlAggiornaBooking">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divWait"
            runat="server" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabAggBooking">
                    <li><a href="">Aggiornamento BOOKING</a></li>
                </asp:Panel>
            </ul>
            <div>
                <div class="tab_container">
                    <div runat="server" id="divIntro" style="background-position: right top; background-repeat: no-repeat;
                        background-image: url('../App_Themes/BlueINPS1/Images/engine.png');" class="iframe-bg-engine">
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
                                        finale BOOKING.</label>
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
                        background-image: url('../App_Themes/BlueINPS1/Images/engine.png');" class="iframe-bg-engine">
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
                                        Risultato dell' aggiornamento BOOKING</label>
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
                                <asp:Button ID="btnAggBooking" CausesValidation="false" runat="server" Text="Aggiorna"
                                    SkinID="btnAzione1" Width="150px" OnClick="btnAggBooking_Click" OnClientClick="BlockUI()"  CssClass="ghost-update" />
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
