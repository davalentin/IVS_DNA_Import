<%@ Page Language="C#" Title="" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="InvioCalcolo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.InvioCalcolo" %>

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

        function CreatePopUpConsultazioniANF() {
            // jQuery UI Dialog
            var result;
            $('#consultazioniANF').dialog({
                autoOpen: false,
                width: 720,
                height: 480,
                modal: true,
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'hidden'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    "Annulla": function () {
                        $(this).dialog("close");
                        document.getElementById('<%=HdnConsultazioniANFVerificate.ClientID %>').value = ''; //sbianco l'hdn filed
                        result = false;
                    },
                    "Continua Calcolo": function () {
                        $(this).dialog("close");
                        document.getElementById('<%=HdnConsultazioniANFVerificate.ClientID %>').value = 'SI';
                        document.getElementById('<%= btnInvioCalcolo.ClientID %>').click();
                        result = true;
                    }
                }
            });
            $("#consultazioniANF").parent().appendTo($("form:first"));
        }

        function ShowPopUpConsultazioniANF() {
            CreatePopUpConsultazioniANF();
            $('#consultazioniANF').dialog('open');
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
    <div id="consultazioniANF" style="display: none;" title="Consultazione Unificata ANF">
        <asp:Label ID="lblConsultazioneANF" runat="server"></asp:Label>
    </div>
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
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#detrazioni" runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCInvioCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlInvioCalcolo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divWait" runat="server"
            class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabInvioCalcolo">
                    <li><a href="">Calcolo Domanda</a></li>
                </asp:Panel>
            </ul>
            <div>
                <div class="tab_container iframe-bg-engine grid-group grid-end" style="background-position: right top; background-repeat: no-repeat;
                    background-image: url('../App_Themes/BlueINPS1/Images/engine.png');">
                    <div runat="server" id="divResult">
                        <table class="tabellaFormattazione grid grid-size-25-col-2" width="100%">
                            <tr style="height: 10px;" class="none">
                                <td colspan="4">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td align="center" colspan="2" class="Row1 shift-full-grid text-left" style="width: 30%">
                                    <label style="color: #336699; font-weight: bold; font-size: larger">
                                        Risultato del Calcolo Domanda</label>
                                </td>
                                <td style="width: 10%" class="none">
                                </td>
                            </tr>
                            <tr style="height: 20px;">
                                <td colspan="4" class="none">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td class="Row1" style="width: 25%">
                                    <label>
                                        Esito:</label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblEsito" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        Dettaglio Esito:</label>
                                </td>
                                <td style="width: 65%;">
                                    <asp:Label runat="server" ID="lblDettaglio" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        Stato Domanda:</label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblStato" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td class="Row1" style="width: 25%;">
                                    <asp:Label runat="server" ID="lblCertificatoTitolo" Text="Certificato: "></asp:Label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblCertificatoValore" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td colspan="2" style="width: 90%">
                                    <asp:Label runat="server" ID="lblMessCalcoloDefinitivo" Style="font-weight: bold;
                                        font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%" class="none">
                                </td>
                                <td colspan="2" style="width: 90%">
                                    <asp:Label runat="server" ID="msgResultIndennizzo" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%" class="none">
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div runat="server" id="divSeparator">
                    </div>
                    <div runat="server" id="divIntro">
                        <table class="tabellaFormattazione grid-col-1" width="98%">
                            <tr>
                                <td style="width: 5%"></td>
                                <td align="justify" class="Row1">
                                    <label>
                                        In questa sezione cliccando <b>'Calcola'</b> sarà possibile effettuare il passo
                                        finale, relativamente alla domanda in lavorazione ottenendo le informazioni relative
                                        al calcolo.</label>
                                </td>
                                <td style="width: 10%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td align="justify" class="Row1">
                                    <asp:Label ID="lblTipoCalcolo" runat="server" Style="font-weight: bold; font-size: small;"
                                        Text="Tipo calcolo:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlTipoCalcolo" CssClass="tb8 txtUppercase"
                                        Style="font-weight: bold; font-size: small;" TabIndex="1" Width="120px">
                                        <asp:ListItem Value="V" Text="VERIFY"></asp:ListItem>
                                        <asp:ListItem Value="D" Text="DEFINITIVO"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Label ID="lblUtilizzaCalcoloReing" Visible="false" runat="server" Style="font-weight: bold; font-size: small; padding-left: 15%"
                                        Text="Utilizza calcolo reingegnerizzato:"></asp:Label>
                                    <asp:CheckBox runat="server" ID="chkUtilizzaCalcoloReing" CssClass="tb8 offClass"
                                        Visible="false" TabIndex="2" />
                                </td>
                                <td style="width: 10%"></td>
                            </tr>
                        </table>
                    </div>
                    <div id="divAvvisoNuovoCalcolo" runat="server" Visible="false">
                        <table class="tabellaFormattazione" width="100%">
                            <tr>
                                <td style="width: 5%"></td>
                                <td colspan="2" style="width: 90%">
                                    <asp:Label runat="server" ID="lblAvvisoNuovoCalcolo" Style="font-weight: bold; font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                        </table>
                    </div>
                    <table width="100%" class="footer-actions-group footer-actions-group--invio-calcolo">
                        <tr runat="server" id="rowMargin">
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <td style="text-align: right;">
                                <asp:Button ID="btnInvioCalcolo" CausesValidation="false" runat="server" Text="Calcola"
                                    SkinID="btnAzione1" Width="150px" OnClick="btnInvioCalcolo_Click" OnClientClick="mainValidate()" CssClass="primary" />
                            </td>
                            <td style="text-align: left;" class="none">
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
            </div>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HdnConsultazioniANFVerificate" runat="server"></asp:HiddenField>
</asp:Content>
