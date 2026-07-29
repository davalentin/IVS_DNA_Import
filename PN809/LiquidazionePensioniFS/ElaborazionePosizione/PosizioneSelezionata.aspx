<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="PosizioneSelezionata.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.PosizioneSelezionata" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <br />
    <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
    <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
        <asp:Panel ID="pnlPresaInCarico" runat="server" Visible="false">
            <table class="borderNoTab">
                <tr>
                    <td class="Row1">
                        <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlPosizioneSelezionata" runat="server">
            <table class="borderNoTab">
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Numero Domanda:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label ID="lblNumeroDomanda" runat="server" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Codice Fiscale:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label ID="lblCodiceFiscale" runat="server" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Categoria:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblCategoria" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold" id="lblSedeText" runat="server">
                            Sede:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblSede" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Certificato:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblCertificato" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 26%;">
                        <label class="etichettaBold">
                            Tipo Domanda:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblTipoDomanda" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Cognome:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblCognome" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Nome:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblNome" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Sesso:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblSesso" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Data di Nascita:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblDataNascita" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Comune di Nascita:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblComuneNascita" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <asp:Label ID="etichettaProvinciaStatoNascita" runat="server" class="etichettaBold">
                                a:</asp:Label>
                    </td>
                    <td class="Row1">
                        <asp:Label ID="lblProvinciaStatoNascita" runat="server" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Indirizzo:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblIndirizzo" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold">
                            N. Civico:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblNCivico" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            CAP:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblCAP" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="etichettaComuneStatoResidenza" class="etichettaBold">
                                CR:</asp:Label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblComuneStatoResidenza" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Provincia di Residenza:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblProvinciaResidenza" CssClass="lblLightBlue"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label class="etichettaBold">
                            Residente all'estero:</label>
                    </td>
                    <td class="Row1">
                        <asp:Label runat="server" ID="lblResidenteEstero" CssClass="lblLightBlue"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <asp:Panel runat="server" ID="pnlFrazioneEstero" Visible="false">
                        <td class="Row1" style="width: 22%;">
                            <label class="etichettaBold">
                                Frazione:</label>
                        </td>
                        <td class="field" style="width: 28%;">
                            <asp:Label runat="server" ID="lblFrazione" CssClass="lblLightBlue" />
                        </td>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
                        <td class="Row1" style="width: 22%;">
                            <label class="etichettaBold">
                                Data Morte:</label>
                        </td>
                        <td class="field" style="width: 28%;">
                            <asp:Label runat="server" ID="lblDataMorte" CssClass="lblLightBlue" />
                        </td>
                    </asp:Panel>
                </tr>
            </table>
            <asp:Panel ID="pnlEsitoCalcolo" runat="server">
                <table class="borderNoTab mt-16">
                    <tr>
                        <td class="Row1" style="width: 29%;">
                            <label class="etichettaBold">
                                Esito Calcolo:</label>
                        </td>
                        <td class="Row1">
                            <asp:Label runat="server" ID="lblEsitoCalcolo" CssClass="lblLightBlue"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 29%;">
                            <label class="etichettaBold">
                                Dettaglio Esito Calcolo:</label>
                        </td>
                        <td class="Row1">
                            <asp:Label runat="server" ID="lblDettaglioEsitoCalcolo" CssClass="lblLightBlue"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" style="width: 29%;">
                            <asp:Label runat="server" ID="lblCertificatoEsitoTitolo" Text="Certificato: " class="etichettaBold"></asp:Label>
                        </td>
                        <td class="Row1">
                            <asp:Label runat="server" ID="lblCertificatoEsitoValore" CssClass="lblLightBlue"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="width: 100%;" class="Row1">
                            <asp:Label runat="server" ID="lblMessCalcoloDefinitivo"></asp:Label>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="pnlInformativaSupplementiRic" runat="server" Visible="false">
                <table class="borderNoTab">
                    <tr>
                        <td colspan="2" style="width: 100%; padding: 20px;" class="Row1">
                            <asp:Label ID="lblInformaticaSupplementiRic" runat="server" Text="Per questa pensione mancano le registrazioni di alcuni supplementi. E' necessario effettuare delle verifiche con i dati delle precedenti ricostituzioni ed eventualmente inserire le registrazioni mancanti."></asp:Label>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
        <div runat="server" id="divPulsantiRicerca" visible="false" style="width: 722px; margin: 0 auto; margin-top: 5px; float: left;" class="none">
            <table width="100%">
                <tr>
                    <td style="text-align: right">
                        <asp:Button ID="btnRicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" Width="210px" PostBackUrl="~/ElaborazionePosizione.aspx"
                            OnClientClick="BlockUI()" />
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="btnRisultati" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="210px" PostBackUrl="~/RisultatoRicercaElaborazione.aspx"
                            OnClientClick="BlockUI()" />
                    </td>
                </tr>
            </table>
        </div>
        <div runat="server" visible="false" id="divPulsantiStatoPratica" style="width: 722px;
            margin: 0 auto; margin-top: 5px; float: left;">
            <table width="100%">
                <tr>
                    <td style="text-align: right">
                        <asp:Button ID="btnVisualizza" runat="server" Text="Torna alla visualizzazione" SkinID="btnAzione1"
                            CausesValidation="false" Width="210px" PostBackUrl="~/VisualizzazioneStatoPratiche.aspx"
                            OnClientClick="BlockUI()" />
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="210px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
