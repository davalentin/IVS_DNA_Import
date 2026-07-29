<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLiquidazionePensioneStorico_AGO.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCLiquidazionePensioneStorico_AGO" %>
<style type="text/css" media="screen">
    #divStorico h3.trigger
    {
        width: 90%;
        margin: 20px 0px 0px 10px;
    }
    #divStorico .fakeLink
    {
        color: Black;
        cursor: pointer;
    }
    
    #divStorico .fakeLink:hover
    {
        color: #ccc;
    }
    
    #divStorico div.collapsibleContainer
    {
        margin-left: 50px;
        border-style: solid;
        border-color: #000080;
        border-collapse: collapse;
        border-width: 1px;
        width: 90%;
        margin-top: 4px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {

        if ($("#<%= pnlDatiGenerici.ClientID %> .fakeLink"))
            $("#<%= pnlDatiGenerici.ClientID %> .fakeLink").click(function () {
                $(this).toggleClass("active").next().slideToggle("fast");
            });

        if ($("#<%= pnlDatiAssicurativi.ClientID %> .fakeLink"))
            $("#<%= pnlDatiAssicurativi.ClientID %> .fakeLink").click(function () {
                $(this).toggleClass("active").next().slideToggle("fast");
            });

        if ($("#<%= pnlDatiIstruttoria.ClientID %> .fakeLink"))
            $("#<%= pnlDatiIstruttoria.ClientID %> .fakeLink").click(function () {
                $(this).toggleClass("active").next().slideToggle("fast");
            });
    });
</script>
<div id="divStorico" style="margin-top: 4px;">
    <asp:Panel runat="server" ID="pnlDatiGenerici" Visible="false">
        <h3 class='trigger fakeLink'>
            Dati Generici
        </h3>
        <asp:Panel CssClass="collapsibleContainer PnlContenitoreDatiInterno" runat="server"
            ID="divDatiGenerici" Enabled="false">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Pensione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiGenerici" Text=""></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%">
                    </td>
                    <td class="field" style="width: 25%">
                    </td>
                </tr>
                <asp:Panel ID="pnlScadRevSanitaria" runat="server" Visible="false">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Data Revisione Sanitaria:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtScadRevSanitaria" Width="50%"
                                CssClass="txtUppercase tb8" TabIndex="7" Text="MM/AAAA" MaxLength="7"></asp:TextBox>
                        </td>
                        <td class="Row1" colspan="2">
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <asp:Panel runat="server" ID="pnlTipoCalcolo" Visible="true">
                        <td class="Row1" style="width: 25%">
                            <label>
                                Tipo Calcolo:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlTipoCalcolo" Width="90%" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTipoCalcoloCum" Visible="false">
                        <td class="Row1" style="width: 25%">
                            <label>
                                Contributivo:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlContributivoCum" Width="20%" CssClass="tb8 txtUppercase xxs">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="SI" Value="8"></asp:ListItem>
                                <asp:ListItem Text="NO" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </asp:Panel>
                </tr>
                <asp:Panel ID="pnlModalitaLiquidazione" runat="server" Visible="false">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Modalità Liquidazione:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlModalitaLiquidazione" Width="90%" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlCodiceMobilita" runat="server" Visible="false">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Codice Mobilità:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlCodMobilita" Width="90%" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlTipoCumulo" runat="server" Visible="false">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Tipo Cumulo:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlTipoCumulo" Width="90%" CssClass="txtUppercase tb8">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="False" Text="ESTERNO"></asp:ListItem>
                                <asp:ListItem Value="True" Text="INTERNO"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <asp:Panel runat="server" ID="pnlCumuloEsterno" Visible="false">
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Cumulo Esterno:</label>
                            </td>
                            <td class="field">
                                <asp:DropDownList runat="server" ID="ddlCumuloEsterno" Width="90%" CssClass="txtUppercase tb8">
                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                    <asp:ListItem Value="E" Text="COMPLETO"></asp:ListItem>
                                    <asp:ListItem Value="M" Text="INCOMPLETO"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </asp:Panel>
                    </tr>
                </asp:Panel>
            </table>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiAssicurativi" Visible="false">
        <h3 class='trigger fakeLink'>
            Dati Assicurativi
        </h3>
        <asp:Panel CssClass="collapsibleContainer PnlContenitoreDatiInterno" runat="server"
            ID="divDatiAssicurativi" Enabled="false">
            <table class="tabellaFormattazione grid grid-size-25">
                <asp:Panel runat="server" ID="pnlInizioFineAssicurazione">
                    <tr>
                        <td class="Row1" style="width: 23%">
                            <label>
                                Inizio Assicurazione:</label>
                        </td>
                        <td class="field" style="width: 27%">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioAssicurazione"
                                Width="100px" Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 23%">
                            <label>
                                Fine Assicurazione:</label>
                        </td>
                        <td class="field" style="width: 27%">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtFineAssicurazione" Width="100px"
                                Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlInizioFineUltimoLavoro" Visible="false">
                    <tr>
                        <td class="Row1">
                            <asp:Label runat="server" ID="lblInizioUltLAv" Text="Inizio Ultimo Lavoro:"></asp:Label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioUltLav" Width="100px"
                                Text="gg/mm/aaaa" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10"></asp:TextBox>
                        </td>
                        <td class="Row1">
                            <asp:Label runat="server" ID="lblFineUltLav" Text="Fine Ultimo Lavoro:"></asp:Label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtFineUltLav" Width="100px"
                                Text="gg/mm/aaaa" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                                MaxLength="10"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlAttEconomProfInd">
                    <tr>
                        <td class="Row1">
                            <label>
                                Attività Economica:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="120px"
                                CssClass="txtUppercase tb8 onClassDomanda autotab" MaxLength="2"></asp:TextBox>
                        </td>
                        <td class="Row1">
                            <label>
                                Professione Individuale:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale"
                                Width="120px" CssClass="txtUppercase tb8 onClassDomanda autotab" MaxLength="3"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlNSettimane_NContributiVolontariDiritto">
                    <tr>
                        <td class="Row1">
                            <label>
                                Numero Settimane:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtNumeroSettimaneOBG" Width="120px"
                                CssClass="txtUppercase tb8" MaxLength="4"></asp:TextBox>
                        </td>
                        <td class="Row1">
                            <label>
                                Numero Contributi Volontari Diritto:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtNumContrVolontari" Width="120px"
                                MaxLength="4" CssClass="txtUppercase tb8"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblNumContrVolontariAnz" Text="Numero Contributi Volontari per Anzianità:"></asp:Label>
                    </td>
                    <td class="field">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtNumContrVolontariAnz"
                            MaxLength="4" Width="120px" CssClass="txtUppercase tb8"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiIstruttoria" Visible="false">
        <h3 class='trigger fakeLink'>
            Dati Istruttoria
        </h3>
        <asp:Panel CssClass="collapsibleContainer PnlContenitoreDatiInterno" runat="server"
            ID="divDatiIstruttoria" Enabled="false">
            <table class="tabellaFormattazione">
                <asp:Panel ID="pnlCodiceRequisitoRidotto" runat="server">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Codice Requisiti ridotti:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlCodReqRidotti" Width="43%" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlSoggettoDerogato" runat="server" Visible="false">
                    <tr>
                        <td class="Row1">
                            <label>
                                Soggetto Derogato:</label>
                        </td>
                        <td class="field full-grid" colspan="3">
                            <asp:DropDownList runat="server" ID="ddlSoggettoDerogato" Width="90%" CssClass="tb8 txtUppercase"
                                Enabled="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td colspan="4">
                        <!-- Pannello Riduzione Retributiva-->
                        <asp:Panel ID="pnlRiduzioneRetributiva" runat="server">
                            <table width="100%" class="tabellaFormattazione grid">
                                <tr style="vertical-align: bottom">
                                    <td class="Row1" style="width: 25%">
                                        <label>
                                            Riduzione Retributiva:</label>
                                    </td>
                                    <td class="Row1" style="width: 65%">
                                        <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs" Width="15%"
                                            runat="server">
                                            <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                            <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase"
                                            Width="15%" TabIndex="14" MaxLength="5"></asp:TextBox>
                                        <label>
                                            %</label>
                                    </td>
                                    <td style="width: 15%">
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <!-- Fine Pannello Riduzione Retributiva-->
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
</div>
<div style="min-height: 100px;">
</div>
