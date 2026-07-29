<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloStoricoGP.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiCalcoloStoricoGP" %>
<div id="pdivRetributivo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <!-- Pannello Dati Calcolo Retributivi EL-TT-ET -->
    <asp:Panel ID="pnlDatiCalcoloRetributivi_EL_TT_ET" runat="server" Visible="false">
        <!-- Pannello Dati Calcolo Retributivi Fondo ET -->
        <asp:Panel ID="pnlDatiCalcoloRetributiviET" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Ante 01/01/93 (Quota A)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields" style="width: 23%">
                        <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 13%">
                        <label>
                            Retribuzione Pensionabile:</label>
                    </td>
                    <td class="Row1" style="width: 21%">
                        <asp:TextBox ID="txtRetribPensionabileQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 28%">
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Post 31/12/92 (Quota B)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields" style="width: 23%">
                        <asp:TextBox ID="txtServizioUtileAAQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileMMQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileGGQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 13%">
                        <label>
                            Retribuzione Pensionabile:</label>
                    </td>
                    <td class="Row1" style="width: 21%">
                        <asp:TextBox ID="txtRetribPensionabileQtaB" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 28%">
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <label style="font-weight: bold" class="section-label mt-32">
                            Dati Post 31/12/94 (Quota C)</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 15%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 inline-fields">
                        <asp:TextBox ID="txtServizioUtileAAQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileMMQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:TextBox ID="txtServizioUtileGGQtaC" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Retributivi Fondo ET-->
        <asp:Panel ID="pnlDecretoCross" runat="server">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblTitoloDatiRetributivi" runat="server" Text="" Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione" width="100%">
                <asp:Panel ID="pnlRigaA" runat="server" Visible="true">
                    <tr>
                        <td class="Row1" style="width: 33%">
                            <label>
                                Retribuzione Media Settimanale A:</label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox runat="server" ID="txtRMSA" CssClass="tb8 txtUppercase" MaxLength="11"
                                Width="90%" OnBlur="setRetribuzioneUltimoAnno();"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 3%">
                            <label>
                                €</label>
                        </td>
                        <td class="Row1" style="width: 13%">
                            <label class="etichettaBold">
                                Settimane A:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneA" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 5%">
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione Media Settimanale B:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtRMSB" CssClass="tb8 txtUppercase" Width="90%"
                            MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                        <label class="etichettaBold">
                            Settimane B:</label>
                    </td>
                    <td class="Row1" style="width: 15%">
                        <asp:TextBox runat="server" ID="txtSettimaneB" CssClass="tb8 txtUppercase" Width="80%"
                            MaxLength="4"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlDatiCalcoloRetributivi_EL_TT" runat="server" Visible="false">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 33%">
                        </td>
                        <td class="Row1" style="width: 30%">
                        </td>
                        <td class="Row1" style="width: 3%">
                        </td>
                        <td class="Row1" style="width: 14%">
                            <label>
                                Settimane C:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneC" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 5%">
                        </td>
                    </tr>
                    <tr runat="server" id="rigaD">
                        <td class="Row1" style="width: 33%">
                            <label>
                                Retribuzione Media Settimanale D:</label>
                        </td>
                        <td class="Row1" style="width: 30%">
                            <asp:TextBox runat="server" ID="txtRMSD" CssClass="tb8 txtUppercase" Width="90%"
                                MaxLength="11"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 3%">
                            <label>
                                €</label>
                        </td>
                        <td class="Row1" style="width: 14%">
                            <label>
                                Settimane D:</label>
                        </td>
                        <td class="Row1" style="width: 15%">
                            <asp:TextBox runat="server" ID="txtSettimaneD" CssClass="tb8 txtUppercase" Width="80%"
                                MaxLength="4"></asp:TextBox>
                        </td>
                        <td class="Row1" style="width: 5%">
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione AGO annua:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox runat="server" ID="txtRetribuzioneAgoAnnua" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Pannello Dati Calcolo Retributivi Fondo Telefonici -->
        <asp:Panel ID="pnlDatiCalcoloRetributiviTT" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione ultimo anno:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtRetribUltimoAnnoRetrib" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11" Enabled="false"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 33%">
                        <label>
                            Retribuzione biennio:</label>
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtRetribuzioneBiennio" runat="server" CssClass="tb8 txtUppercase"
                            Width="90%" MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 3%">
                        <label>
                            €</label>
                    </td>
                    <td class="Row1" style="width: 14%">
                    </td>
                    <td class="Row1" style="width: 15%">
                    </td>
                    <td class="Row1" style="width: 5%">
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Retributivi Fondo Telefonici-->
    </asp:Panel>
    <!-- Fine Pannello Dati Calcolo Retributivi EL-TT-ET -->
    <!-- Pannello Dati Calcolo Retributivi VL -->
    <asp:Panel ID="pnlDatiRetributiviVL" runat="server" Visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblDecretoLegislativo164DatiRetrib" runat="server" Text="Decreto Legislativo 164"
                        Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <asp:Label ID="lblRetribuzioneMediaSettADatiRetrib" runat="server" Text="Retribuzione Media Settimanale A:"></asp:Label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox ID="txtRetribuzioneMediaSettADatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="11"></asp:TextBox>
                </td>
                <td class="Row1" style="text-align: right; width: 20%">
                    <asp:Label ID="lblSettimaneA1DatiRetrib" runat="server" Text="Settimane A1:"></asp:Label>
                </td>
                <td style="width: 35px" class="none">
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox ID="txtSettimaneA1DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="4"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                </td>
                <td class="Row1" style="width: 25%">
                </td>
                <td class="Row1" style="text-align: right; width: 20%">
                    <asp:Label ID="lblSettimaneA2DatiRetrib" runat="server" Text="Settimane A2:"></asp:Label>
                </td>
                <td style="width: 20px">
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox ID="txtSettimaneA2DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="4"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <asp:Label ID="lblRetribuzioneMediaSettBDatiRetrib" runat="server" Text="Retribuzione Media Settimanale B:"></asp:Label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox ID="txtRetribuzioneMediaSettBDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="11"></asp:TextBox>
                </td>
                <td class="Row1" style="text-align: right; width: 20%">
                    <asp:Label ID="lblSettimaneBDatiRetrib" runat="server" Text="Settimane B:"></asp:Label>
                </td>
                <td style="width: 20px">
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox ID="txtSettimaneBDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="4"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                </td>
                <td class="Row1" style="width: 25%">
                </td>
                <td class="Row1" style="text-align: right; width: 20%">
                    <asp:Label ID="lblSettimaneC1DatiRetrib" runat="server" Text="Settimane C1:"></asp:Label>
                </td>
                <td style="width: 20px">
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox ID="txtSettimaneC1DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                        Width="130" MaxLength="4"></asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlDatiRetributiviCustomVL" runat="server" Visible="false">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 25%">
                    </td>
                    <td class="Row1" style="width: 25%">
                    </td>
                    <td class="Row1" style="text-align: right; width: 20%">
                        <asp:Label ID="lblSettimaneC2DatiRetrib" runat="server" Text="Settimane C2:"></asp:Label>
                    </td>
                    <td style="width: 20px">
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtSettimaneC2DatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                            Width="130" MaxLength="4"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <asp:Label ID="lblRetribuzioneMediaSettDDatiRetrib" runat="server" Text="Retribuzione Media Settimanale D:"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtRetribuzioneMediaSettDDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                            Width="130" MaxLength="11"></asp:TextBox>
                    </td>
                    <td class="Row1" style="text-align: right; width: 20%">
                        <asp:Label ID="lblSettimaneDDatiRetrib" runat="server" Text="Settimane D:"></asp:Label>
                    </td>
                    <td style="width: 20px">
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtSettimaneDDatiRetrib" runat="server" CssClass="tb8 txtUppercase"
                            Width="130" MaxLength="4"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <!-- Fine Pannello Dati Calcolo Retributivi VL -->
    <!-- Pannello Riduzione Retributiva-->
    <asp:Panel ID="pnlRiduzioneRetributiva" runat="server" Visible="false">
        <table class="tabellaFormattazione  grid grid-size-20-col-5" width="100%">
            <tr style="min-height: 50px; vertical-align: bottom">
                <td class="Row1" style="width: 33%">
                    <label>
                        Riduzione Retributiva:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs" Width="25%"
                        runat="server">
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase"
                        Width="61%" MaxLength="5"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        %</label>
                </td>
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello Riduzione Retributiva-->
</div>
<div id="pdivContributivo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <!-- Pannello Dati Calcolo Contributivi EL-TT-ET -->
    <asp:Panel ID="pnlDatiCalcoloContributiviLegge335_EL_TT_ET" runat="server" Visible="false">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblTitoloContributiviL335" Text="Dati Contributivi da L. 335" runat="server"
                        Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Importo contributivo totale:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtImportoContributivoTotale" CssClass="tb8 txtUppercase"
                        Width="90%" MaxLength="12"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        €</label>
                </td>
                <td class="Row1" style="width: 14%">
                </td>
                <td class="Row1" style="width: 15%">
                </td>
                <td class="Row1" style="width: 5%">
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Montante:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtMontante" CssClass="tb8 txtUppercase" Width="90%"
                        MaxLength="12"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        €</label>
                </td>
                <td class="Row1" style="width: 14%">
                    <label class="etichettaBold">
                        Settimane:</label>
                </td>
                <td class="Row1" style="width: 15%">
                    <asp:TextBox runat="server" ID="txtSettimane" CssClass="tb8 txtUppercase" Width="80%"
                        MaxLength="4"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 5%">
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello Dati Calcolo Contributivi EL-TT-ET -->
    <!-- Pannello Dati Calcolo Contributivi L.335 VL -->
    <asp:Panel ID="pnlDatiCalcoloContributiviLegge335_VL" runat="server" Visible="false">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="Label1" runat="server" Text="Dati Contributivi da Legge 335" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Importo contributivo totale:</label>
                </td>
                <td class="Row1">
                    <asp:TextBox ID="txtImportTotale335_VL" runat="server" CssClass="tb8 txtUppercase"
                        Width="74%" MaxLength="11"></asp:TextBox>
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlDatiContributiviVLFelpe" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <asp:Label ID="lblMontante_VL" runat="server" Text="Montante:"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%" colspan="2">
                        <asp:TextBox ID="txtMontante_VL" runat="server" CssClass="tb8 txtUppercase" Width="130"
                            MaxLength="13"></asp:TextBox>
                    </td>
                    <td class="Row1" style="width: 20%">
                    </td>
                    <td class="Row1" style="width: 20px">
                    </td>
                    <td class="Row1" style="width: 30%">
                    </td>
                    <td class="Row1" style="width: 10px">
                    </td>
                </tr>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlDatiContributiviVLNoFelpe" Visible="true">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <asp:Label ID="lblMontanteDa0196a0697_VL" runat="server" Text="Montante da 01/96 a 06/97:"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtMontanteDa0196a0697_VL" runat="server" CssClass="tb8 txtUppercase"
                            Width="130" MaxLength="13"></asp:TextBox>
                    </td>
                    <td class="Row1" style="text-align: right; width: 20%">
                        <asp:Label ID="lblAnzianita96_VL" runat="server" Text="Anzianità:"></asp:Label>
                    </td>
                    <td style="width: 20px">
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtA96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="2"></asp:TextBox>
                        <asp:Label ID="lblA96_VL" runat="server" Text="a"></asp:Label>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:TextBox ID="txtM96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="2"></asp:TextBox>
                        <asp:Label ID="lblM96_VL" runat="server" Text="m"></asp:Label>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:TextBox ID="txtG96_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="3"></asp:TextBox>
                        <asp:Label ID="lblG96_VL" runat="server" Text="g"></asp:Label>
                    </td>
                    <td style="width: 10px">
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <asp:Label ID="lblMontanteDal0797_VL" runat="server" Text="Montante dal 07/97:"></asp:Label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtMontanteDa0697_VL" runat="server" CssClass="tb8 txtUppercase"
                            Width="130" MaxLength="13"></asp:TextBox>
                    </td>
                    <td class="Row1" style="text-align: right; width: 20%">
                        <asp:Label ID="lblAnzianita97_VL" runat="server" Text="Anzianità:"></asp:Label>
                    </td>
                    <td style="width: 20px">
                    </td>
                    <td class="Row1" style="width: 30%">
                        <asp:TextBox ID="txtA97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="2"></asp:TextBox>
                        <asp:Label ID="lblA97_VL" runat="server" Text="a"></asp:Label>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:TextBox ID="txtM97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="2"></asp:TextBox>
                        <asp:Label ID="lblM97_VL" runat="server" Text="m"></asp:Label>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:TextBox ID="txtG97_VL" runat="server" CssClass="tb8 txtUppercase" Width="30"
                            MaxLength="3"></asp:TextBox>
                        <asp:Label ID="lblG97_VL" runat="server" Text="g"></asp:Label>
                    </td>
                    <td style="width: 10px">
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </asp:Panel>
    <!-- Fine Pannello Dati Calcolo Contributivi VL -->
</div>
<div id="pdivContributivoL214_Common" style="border-style: solid; border-color: #000080;
    border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
    margin-top: 4px;" runat="server" visible="false">
    <asp:Panel ID="pnlDatiCalcoloContributiviLegge214_Common" runat="server">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblDatiContributiviL214" runat="server" Text="Dati Contributivi da L. 214"
                        Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Importo contributivo totale:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtImportoContribTotaleQuotaDL214" CssClass="tb8 txtUppercase"
                        Width="90%" MaxLength="12"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        €</label>
                </td>
                <td class="Row1" style="width: 14%">
                </td>
                <td class="Row1" style="width: 15%">
                </td>
                <td class="Row1" style="width: 5%">
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Montante:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtMontanteQuotaDL214" CssClass="tb8 txtUppercase"
                        Width="90%" MaxLength="12"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        €</label>
                </td>
                <td class="Row1" style="width: 14%">
                    <label class="etichettaBold">
                        Settimane:</label>
                </td>
                <td class="Row1" style="width: 15%">
                    <asp:TextBox runat="server" ID="txtNSettimaneQuotaDL214" CssClass="tb8 txtUppercase"
                        Width="80%" MaxLength="4"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 5%">
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
<div id="pdivComma707" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="false">
    <!-- Pannello dati comma 707 EL-TT-ET -->
    <asp:Panel runat="server" ID="pnlComma707_EL_TT_ET" Visible="false">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="section-label mt-32" style="text-align: left; font-weight: bold">
                    Calcolo ex comma 707
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota A:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaAComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    Quota B:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota C:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaCComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    Quota D:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaDComma707" CssClass="tb8 txtUppercase" MaxLength="4"
                        Width="60%"></asp:TextBox>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 45%">
                    Retribuzione ponderata AGO per calcolo limite:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtRetribuzionePonderataComma707" CssClass="tb8 txtUppercase"
                        MaxLength="12" Width="90%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    <label>
                        €</label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello dati comma 707 -->
    <!-- Pannello dati comma 707 -->
    <asp:Panel runat="server" ID="pnlComma707_VL" Visible="false">
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="section-label mt-32" style="text-align: left; font-weight: bold">
                    Calcolo ex comma 707
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione" width="100%">
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota A1:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaA1Comma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    Quota A2:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaA2Comma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota B:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaBComma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    Quota C1:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaC1Comma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 20%">
                    Quota C2:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaC2Comma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
                <td class="Row1" style="width: 20%">
                    Quota D:
                </td>
                <td class="field" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtQuotaDComma707VL" CssClass="tb8 txtUppercase"
                        MaxLength="4" Width="60%"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <!-- Fine Pannello dati comma 707 -->
</div>
<div style="min-height: 100px;">
</div>
