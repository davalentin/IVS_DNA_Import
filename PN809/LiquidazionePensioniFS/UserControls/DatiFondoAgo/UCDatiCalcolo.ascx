<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcolo.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo.UCDatiCalcolo" %>
<asp:Panel ID="pnlDatiCalcolo" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 30%">
                <label style="font-weight: bold">
                    Decorrenza Registrazione:</label>
            </td>
            <td class="field" style="text-align: left; width: 25%">
                <asp:Label runat="server" ID="lblDecorrenzaRegistrazione" Width="50%"></asp:Label>
            </td>
            <td style="width: 45%">
            </td>
        </tr>
    </table>
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Pensione Annua Lorda:</label>
                    </td>
                    <td class="Row1" style="width: 34%">
                        <asp:TextBox ID="txtPensioneAnnuaLorda" runat="server" CssClass="tb8 txtUppercase"
                            Width="50%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                            ControlToValidate="txtPensioneAnnuaLorda" Enabled="true" ErrorMessage="Pensione Annua Lorda: Inserire valori interi o decimali"
                            Text="*" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtPensioneAnnuaLorda"
                            Display="Dynamic" Enabled="true" ErrorMessage="Pensione Annua Lorda: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Anni Servizio Utili Diritto:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtAnniServUtiliDiritto" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" TabIndex="2" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtAnniServUtiliDiritto"
                            Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di anni in un formato valido"
                            Text="*" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtAnniServUtiliDiritto"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        <asp:Panel ID="pnlDatiRetributivi" runat="server" Visible="false">
            <table class="tabellaFormattazione">
                <tr style="min-height: 50px; vertical-align: bottom">
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblDatiRetributivi" runat="server" Text="Dati Retributivi:" Style="font-weight: bold;
                            font-size: 15px;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblQuotaA" runat="server" Text="QUOTA A" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblData92" runat="server" Text="Dati al 31/12/92" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAAQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMMQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGGQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Retribuzione ultimo mese:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtRetribuzioneQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" Display="Dynamic"
                            ControlToValidate="txtRetribuzioneQtaA" Enabled="true" ErrorMessage="Retribuzione: Inserire valori interi o decimali"
                            Text="*" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" colspan="2">
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Importo Indennità Integrativa Speciale:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtImpIndenIntegrSpecQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" Display="Dynamic"
                            ControlToValidate="txtImpIndenIntegrSpecQtaA" Enabled="true" ErrorMessage="Importo Indennità Integrativa Speciale: Inserire valori interi o decimali"
                            Text="*" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
                    <tr>
                        <td class="Row1" colspan="2">
                        </td>
                        <td class="Row1" style="width: 24%">
                            <label>
                                Retribuzione senza benefici L.336/70:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox runat="server" ID="txtRetribuzioneSenzaBenefici336" CssClass="txtUppercase tb8 offClass onClassLegge336"
                                TabIndex="18" MaxLength="11" Width="75%" />
                            <asp:RegularExpressionValidator runat="server" ID="txtRetribuzioneSenzaBenefici336_RV"
                                ControlToValidate="txtRetribuzioneSenzaBenefici336" Display="Dynamic" ErrorMessage="Retribuzione senza benefici L.336/70: Inserire massimo 6 cifre intere e 4 decimali"
                                Text="*" ValidationExpression="\d{1,6}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcolo" />
                        </td>
                    </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblQuotaB" runat="server" Text="QUOTA B" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblData94" runat="server" Text="Dati al 31/12/94" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="19" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtServizioUtileAAQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="20" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtServizioUtileMMQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="21" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtServizioUtileGGQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Retribuzione Media:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtRMSQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator15"
                            Display="Dynamic" ControlToValidate="txtRMSQtaB1" Enabled="true" ErrorMessage="Retribuzione Media Quota B: Inserire valori interi o decimali"
                            Text="*" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="Label2" runat="server" Text="Dati al 31/12/95" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtServizioUtileAAQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator16" ControlToValidate="txtServizioUtileAAQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator17" ControlToValidate="txtServizioUtileMMQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator18" ControlToValidate="txtServizioUtileGGQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            GG</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblData97" runat="server" Text="Dati al 31/12/97" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtServizioUtileAAQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator19" ControlToValidate="txtServizioUtileAAQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator20" ControlToValidate="txtServizioUtileMMQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator21" ControlToValidate="txtServizioUtileGGQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            GG</label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblCessazione" runat="server" Text="Dati Cessazione" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtServizioUtileCessazioneAA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator25" ControlToValidate="txtServizioUtileCessazioneAA"
                            ErrorMessage="Servizio Utile Cessazione: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneMM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator26" ControlToValidate="txtServizioUtileCessazioneMM"
                            ErrorMessage="Servizio Utile Cessazione: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneGG" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator27" ControlToValidate="txtServizioUtileCessazioneGG"
                            ErrorMessage="Servizio Utile Cessazione: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <label>
                            GG</label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlDatiContributivi" runat="server" Visible="false">
            <table class="tabellaFormattazione" width="100%">
                <tr style="min-height: 50px; vertical-align: bottom">
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblContributivi" runat="server" Text="Dati Contributivi da Legge 335:"
                            Style="font-weight: bold; font-size: 15px;"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Montante:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtMontante" runat="server" CssClass="tb8 txtUppercase" Width="25%"
                            MaxLength="12"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                            ControlToValidate="txtMontante" Enabled="true" ErrorMessage="Montante: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Panel>
<div style="width: 720px; margin-right: 40px;">
    <table width="100%">
        <tr>
            <td style="text-align: center">
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" ValidationGroup="UCTabDatiCalcolo"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo_Click" Text="Salva Dati Calcolo"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){aspnetForm.target ='_self'; BlockUI();}" Enabled="true" />
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo" Width="150px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" />
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField ID="FlagUnicarpe" runat="server" />
<asp:HiddenField ID="HdnFondo" runat="server" />