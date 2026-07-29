<%@ Page Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master" AutoEventWireup="true" CodeBehind="AggiornaCalcoloNoInd.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.AggiornaCalcoloNoInd" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css" media="screen" />

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


        function openDialog(clientId) {
            var $dlg = $('#' + clientId);

            if (!$dlg.hasClass('ui-dialog-content')) {
                $dlg.dialog({
                    autoOpen: false,
                    modal: true,
                    width: 1000,
                    title: 'Legenda Causali Debito',
                    resizable: false
                });
            }

            $dlg.dialog('open');
        }
    </script>

    <style type="text/css">
        /* Contenitore scrollabile dentro la modal */
        .legend-container {
            max-height: 350px;
            overflow-y: auto;
            margin-top: 10px;
            border: 1px solid #ccc;
            padding: 5px;
        }

        /* Forza il rispetto delle percentuali colonne */
        .grid-legenda {
            table-layout: fixed;
            width: 100%;
            border-collapse: collapse;
        }

        .grid-legenda th, .grid-legenda td {
            padding: 6px 8px;
            vertical-align: top;
        }


        /* Centra completamente Sintetica e Analitica */
        .grid-legenda td:nth-child(1),
        .grid-legenda th:nth-child(1),
        .grid-legenda td:nth-child(2),
        .grid-legenda th:nth-child(2) {
            text-align: center;         /* centro orizzontale */
            vertical-align: middle;     /* centro verticale */
        }

        /* Colonna descrizione con wrap normale */
        .grid-legenda td:nth-child(3),
        .grid-legenda th:nth-child(3) {
            white-space: normal;
            word-wrap: break-word;
        }


        .grid-legenda th {
            background: #f5f5f5;
            font-weight: bold;
        }

        .ui-widget-overlay { z-index: 10000 !important; }
        .ui-dialog         { z-index: 10010 !important; }

        
        /* Stili per la i di Info */
        .info-icon-outline {
            display: inline-block;
            width: 18px;
            height: 18px;
            line-height: 18px;
            text-align: center;
            border-radius: 50%;
            border: 2px solid white; 
            color: white;         
            font-size: 12px;
            font-weight: bold;
            cursor: pointer;
            margin-left: 6px;
            transition: background-color 0.2s ease, color 0.2s ease;
        }

        .info-icon-outline:hover {
            background-color: white;
            color: #333;            
        }

        .info-icon:focus,
        .info-icon-dark:focus,
        .info-icon-outline:focus {
            outline: 2px solid #80bdff;
            outline-offset: 2px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#aggiornaCalcoloNoInd" runat="server" />
    <asp:Panel runat="server" ID="pnlAggiornaCalcoloNoInd">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divValutazioneEventualeScelta" runat="server" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabValutazioneEventualeScelta">
                    <li><a href="#valutazione_eventuale_scelta">Valutazione eventuale scelta causali di debito</a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabElencoCasualiDebito">
                    <li><a href="#elenco_casuali_debito">Elenco causali di debito</a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px; padding-top: 15px">
                <div id="valutazione_eventuale_scelta" runat="server" class="tab_content">
                    <div runat="server" id="divResult">
                        <table class="tabellaFormattazione" width="100%">
                            <tr style="height: 10px;">
                                <td colspan="4"></td>
                            </tr>
                            <tr style="height: 20px;">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%">
                                    <label>
                                        <b>Esito Calcolo</b>:
                                    </label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblEsito" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        <b>Dettaglio Esito Calcolo</b>:
                                    </label>
                                </td>
                                <td style="width: 65%;">
                                    <asp:Label runat="server" ID="lblDettaglio" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        <b>Certificato</b>:
                                    </label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="lblCertificatoValore" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td colspan="2" style="width: 90%">
                                    <asp:Label runat="server" ID="lblMessCalcoloDefinitivo" Style="font-size: smaller;">
                                        La ricostituzione presenta un indebito
                                    </asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                        </table>
                    </div>
                    <div runat="server" id="divSeparator" />
                    <div runat="server" id="divMessaggioCoda">
                        <table class="tabellaFormattazione" width="98%">
                            <tr>
                                <td style="width: 5%"></td>
                                <td align="justify" class="Row1">
                                    <label id="LblMessaggioCodaPannelloValutazioneEventualeScelta" runat="server">
                                    </label>
                                </td>
                                <td style="width: 10%"></td>
                            </tr>
                        </table>
                    </div>
                    <div id="divRisultatoValutazioneEventualeScelta" runat="server" visible="false">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="width: 10%; vertical-align: top; text-align: center">
                                    <asp:Image ID="imgIcon" runat="server" />
                                </td>
                                <td style="width: 90%; vertical-align: middle;">
                                    <asp:Label ID="lblMsg" runat="server" Font-Size="Medium"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div runat="server" id="divPulsanti">
                        <table class="tabellaFormattazione" width="100%">
                            <tr runat="server" id="rowMargin">
                                <td colspan="3"></td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <asp:Button ID="btnProseguiValidazione" CausesValidation="false" runat="server" Text="Prosegui validazione causali"
                                        SkinID="btnAzione1" Width="180px" OnClick="btnProseguiValidazione_Click" OnClientClick="BlockUI()"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:Button ID="btnTornaRicerca" runat="server" Text="Torna alla ricerca"
                                        SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/ElaborazionePosizione.aspx"
                                        OnClientClick="BlockUI()" />
                                </td>
                                <td style="text-align: left;">
                                    <asp:Button ID="btnAccogliDomanda" runat="server" Text="Accogli la domanda" SkinID="btnAzione1"
                                        CausesValidation="false" Width="180px" OnClick="btnAccogliDomanda_Click" OnClientClick="BlockUI()"/>
                                </td>
                            </tr>
                            <tr style="height: 5px;">
                                <td colspan="3"></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div id="elenco_casuali_debito" runat="server" class="tab_content">
                    
                    <!-- Modal per la visualizzazione delle descrizioni delle causali  -->
                    <div id="divLegendaCausali" runat="server" style="display:none;">
                        <div class="legend-container">
                            <asp:GridView ID="gvLegendaCausali"
                                          runat="server"
                                          AutoGenerateColumns="false"
                                          CssClass="grid-legenda"
                                          Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="Sintetica" HeaderText="Causale Sintetica">
                                        <HeaderStyle Width="25%" />
                                        <ItemStyle Width="25%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Analitica" HeaderText="Causale Analitica">
                                        <HeaderStyle Width="25%" />
                                        <ItemStyle Width="25%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Descrizione" HeaderText="Descrizione">
                                        <HeaderStyle Width="50%" />
                                        <ItemStyle Width="50%" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div runat="server" id="divResultElencoCasualiDebito" runat="server">
                        <table class="tabellaFormattazione" width="100%">
                            <tr style="height: 10px;">
                                <td colspan="4"></td>
                            </tr>
                            <tr style="height: 20px;">
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%">
                                    <label>
                                        <b>Esito Calcolo</b>:
                                    </label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="elencoCasualiDebito_EsitoCalcolo" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        <b>Dettaglio Esito Calcolo</b>:
                                    </label>
                                </td>
                                <td style="width: 65%;">
                                    <asp:Label runat="server" ID="elencoCasualiDebito_DettaglioEsito" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td class="Row1" style="width: 25%;">
                                    <label>
                                        <b>Certificato</b>:
                                    </label>
                                </td>
                                <td style="width: 65%">
                                    <asp:Label runat="server" ID="elencoCasualiDebito_Certificato" Style="font-size: smaller;"></asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                            <tr>
                                <td style="width: 5%"></td>
                                <td colspan="2" style="width: 90%">
                                    <asp:Label ID="lblResultCalcoloElencoCausaliDebito" runat="server" Style="font-size: smaller;">
                                        La ricostituzione presenta un indebito, le cui causali e relativo periodo sono riportati di seguito
                                    </asp:Label>
                                </td>
                                <td style="width: 5%"></td>
                            </tr>
                        </table>
                    </div>
                    <div />
                    <div id="elencoCasualiMessaggioCoda" runat="server">
                        <table class="tabellaFormattazione" width="98%">
                            <tr>
                                <td style="width: 5%"></td>
                                <td align="justify" class="Row1">
                                    <label runat="server" id="lblElencoCasualiMessaggioCoda">
                                        <b>Selezionando il pulsante "Valida causali di debito" la domanda sarà accolta 
                                            e verrà prodotto il modello TE08/Ind.</b>
                                    </label>
                                </td>
                            </tr>

                            <tr>
                                <td style="width: 5%"></td>
                                <td align="justify" class="Row1">
                                    <label runat="server" id="Label1">
                                        <b>Selezionando il pulsante "Accogli la domanda" la domanda sarà accolta e verrà 
                                            prodotto il modello TE08.</b>
                                    </label>
                                </td>
                            </tr>
                        </table>

                    </div>
                    <div />
                    <div id="elencoCasualiPeiodoIndebito" runat="server">
                        <table class="tabellaFormattazione" width="100%">
                            <tr>
                                <td width="5%" />
                                <td width="30%">
                                    <b>Periodo Indebito: </b>
                                </td>
                                <td class="periodoIndebito" style="font-size: smaller">
                                    <b>Dal: </b>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDal" Width="140px" CssClass="txtUppercase tb8"
                                        MaxLength="10" TabIndex="2" Enabled="false">
                                    </asp:TextBox>
                                    <b>Al: </b>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAl" Width="140px" CssClass="txtUppercase tb8"
                                        MaxLength="10" TabIndex="2" Enabled="false">
                                    </asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div />
                    <div id="tabellaCasualiDebito" runat="server">
                        <table class="tabellaFormattazione" width="100%">
                            <tr>
                                <td width="5%" />
                                <td width="95%">
                                    <b>Causali Debito</b>
                                </td>
                            </tr>
                            <tr>
                                <td />
                                <td>
                                    <asp:GridView runat="server" ID="casualiDebito" SkinID="grdElenco1" AutoGenerateColumns="false"
                                        AutoGenerateEditButton="true"
                                        OnRowDataBound="gvCasualiIndebito_RowDataBound"
                                        OnRowEditing="gvCasualiIndebito_RowEditing"
                                        OnRowCancelingEdit="gvCasualiIndebito_RowCancelingEdit"
                                        OnRowUpdating="gvCasualiIndebito_RowUpdating"
                                        DataKeyNames="Id"
                                        CssClass="intestazioneTabella" BorderWidth="1" BorderColor="Black"
                                        Width="100%" PageSize="10" AllowPaging="true" RowStyle-HorizontalAlign="Center" EnableViewState="true">

                                        <EmptyDataRowStyle ForeColor="Red" />
                                        <EmptyDataTemplate>
                                            <center>
                                                <asp:Label ID="lblNoData" runat="server"
                                                    Text="Nessun dato Causali Debito trovato."
                                                    SkinID="lblNoData" Visible="true"></asp:Label>
                                            </center>
                                        </EmptyDataTemplate>

                                        <Columns>
                                            <asp:TemplateField HeaderText="Causale Sintetica" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%" HeaderStyle-Width="20%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblCausaleSintetica" Text='<%#Bind("CausaleSintetica")%>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1"
                                                                ItemStyle-CssClass="TblRecordset3" 
                                                                ItemStyle-Width="15%" 
                                                                HeaderStyle-Width="20%">
                                                <HeaderTemplate>
                                                    Causale Analitica
                                                    <span class="info-icon-outline" onclick="openDialog('<%= divLegendaCausali.ClientID %>');">&#9432;</span>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblCausaleAnalitica" Text='<%# Bind("CausaleAnalitica") %>' />
                                                </ItemTemplate>

                                                <EditItemTemplate>
                                                    <asp:DropDownList runat="server" ID="ddlCausaleAnalitica" CssClass="tb8 txtUppercase"
                                                                      Style="width: 95%;" />
                                                </EditItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Conto Recupero" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="40%" HeaderStyle-Width="30%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblContoRecupero" Text='<%#Bind("ContoRecupero")%>' CssClass="txtUppercase" />
                                                </ItemTemplate>
                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="Importo (&euro;)" HeaderStyle-CssClass="intestazioneTabella Row1"
                                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%" HeaderStyle-Width="20%">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" ID="lblImporto"
                                                               Text='<%# Eval("Importo", "{0:N2}") %>'
                                                               CssClass="txtUppercase" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="divRisultatoElencoCasuali" runat="server" visible="false">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td style="width: 10%; vertical-align: top; text-align: center">
                                    <asp:Image ID="imgElencoCasuali" runat="server" />
                                </td>
                                <td style="width: 90%; vertical-align: middle;">
                                    <asp:Label ID="lblRisultatoElencoCasuali" runat="server" Font-Size="Medium"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div runat="server" id="elencoCasualiElencoPulsanti">
                        <table class="tabellaFormattazione" width="100%">
                            <tr>
                                <td colspan="3" style="color: red">
                                    <label id="lblPulsanteValidaCausaliDisabilitato" runat="server">
                                        In questa fase la "Validazione delle causali" non è attiva.
                                        Il debito deve essere gestito con <b>le consuete modalità</b> in procedura RI.<br /> 
                                        Per proseguire seleziona "Accogli Domanda".
                                    </label>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right;">
                                    <asp:Button ID="btnValidaCasuali" CausesValidation="false" runat="server" Text="Valida Causali di Debito"
                                        SkinID="btnAzione1" Width="180px" OnClick="btnValidaCasuali_Click" Enabled="false" OnClientClick="BlockUI()"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:Button ID="btnTornaRicercaElencoCasuali" runat="server" Text="Torna alla ricerca"
                                        SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/ElaborazionePosizione.aspx"
                                        OnClientClick="BlockUI()" />
                                </td>
                                <td style="text-align: left;">
                                    <asp:Button ID="btnAccogliDomandaElencoCasuali" runat="server" Text="Accogli la domanda" SkinID="btnAzione1"
                                        CausesValidation="false" Width="180px" OnClick="btnAccogliDomandaElencoCasuali_Click" OnClientClick="BlockUI()"/>
                                </td>
                            </tr>
                            <tr style="height: 5px;">
                                <td colspan="3"></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
