<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcolo707.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossDatiFondoContr.UCDatiCalcolo707" %>

<asp:Panel ID="pnlDatiCalcolo" runat="server">
    <table class="tabellaFormattazione grid grid-size-20">
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
        <!-- Inizio Pannello Common FS_PT -->
        <asp:Panel ID="pnlDatiCommonFS_PT" runat="server">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Pensione Annua Lorda 707:</label>
                    </td>
                    <td class="Row1" style="width: 34%">
                        <asp:TextBox ID="txtPensioneAnnuaLorda707" runat="server" CssClass="tb8 txtUppercase"
                            Width="50%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                            ControlToValidate="txtPensioneAnnuaLorda707" Enabled="true" ErrorMessage="Pensione Annua Lorda: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtPensioneAnnuaLorda707"
                            Display="Dynamic" Enabled="true" ErrorMessage="Pensione Annua Lorda: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo707" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Common FS_PT -->
        <asp:Panel ID="pnlDatiRetributivi707" runat="server" Visible="true">
            <!-- Pannello Dati Calcolo Retributivi FS_PT-->
            <table class="tabellaFormattazione grid grid-size-20">
                <tr style="min-height: 50px; vertical-align: bottom">
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblDatiRetributivi707" runat="server" Text="Dati Retributivi 707:" Style="font-weight: bold;
                            font-size: 15px;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1  shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaA" runat="server" Text="QUOTA A" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1  shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblData92" runat="server" Text="Dati al 31/12/92" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAAQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMMQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGGQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Quota pensione retributiva annua 707:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtPensioneRetribAnnua707QtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" Display="Dynamic"
                            ControlToValidate="txtPensioneRetribAnnua707QtaA" Enabled="true" ErrorMessage="Quota pensione retributiva annua 707 Quota A: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr> 
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1  shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaB" runat="server" Text="QUOTA B" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1  shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblData94" runat="server" Text="Dati al 31/12/94" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="19" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtServizioUtileAAQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="20" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtServizioUtileMMQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB1" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="21" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtServizioUtileGGQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Quota pensione retributiva annua 707:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtPensioneRetribAnnua707QtaB1" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator15"
                            Display="Dynamic" ControlToValidate="txtPensioneRetribAnnua707QtaB1" Enabled="true" ErrorMessage="Quota pensione retributiva annua 707 Quota B: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="Label2" runat="server" Text="Dati al 31/12/95" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileAAQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator16" ControlToValidate="txtServizioUtileAAQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator17" ControlToValidate="txtServizioUtileMMQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB2" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator18" ControlToValidate="txtServizioUtileGGQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Quota pensione retributiva annua 707:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtPensioneRetribAnnua707QtaB2" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3"
                            Display="Dynamic" ControlToValidate="txtPensioneRetribAnnua707QtaB2" Enabled="true" ErrorMessage="Quota pensione retributiva annua 707 Quota B: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblData97" runat="server" Text="Dati al 31/12/97" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileAAQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator19" ControlToValidate="txtServizioUtileAAQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator20" ControlToValidate="txtServizioUtileMMQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB3" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator21" ControlToValidate="txtServizioUtileGGQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Quota pensione retributiva annua 707:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtPensioneRetribAnnua707QtaB3" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5"
                            Display="Dynamic" ControlToValidate="txtPensioneRetribAnnua707QtaB3" Enabled="true" ErrorMessage="Quota pensione retributiva annua 707 Quota B: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
            <!-- Solo per INPDAP -->
            <asp:Panel runat="server" ID="pnlDatiPost97" Visible="false">
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1 shift-full-grid" style="text-align: left">
                            <asp:Label ID="Label1" runat="server" Text="Dati dal 01/01/98" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-20">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 ">
                            <asp:TextBox ID="txtServizioUtileAAQtaB5" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REV_txtServizioUtileAAQtaB4" ControlToValidate="txtServizioUtileAAQtaB5"
                                ErrorMessage="Servizio Utile dal 01/01/98: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                            <label>
                                AA</label>
                            <asp:TextBox ID="txtServizioUtileMMQtaB5" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REV_txtServizioUtileMMQtaB3" ControlToValidate="txtServizioUtileMMQtaB5"
                                ErrorMessage="Servizio Utile dal 01/01/98: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                            <label>
                                MM</label>
                            <asp:TextBox ID="txtServizioUtileGGQtaB5" runat="server" CssClass="tb8 txtUppercase"
                                Width="30px" MaxLength="2"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REV_txtServizioUtileGGQtaB5" ControlToValidate="txtServizioUtileGGQtaB5"
                                ErrorMessage="Servizio Utile dal 01/01/98: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                            <label>
                                GG</label>
                        </td>
                        <asp:Panel ID="pnlQuotaPensioneRetributivaAnnua707B98" runat="server" Visible="false">
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtPensioneRetribAnnua707QtaB5" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaPensioneRetributivaAnnua707B98"
                                    Display="Dynamic" ControlToValidate="txtPensioneRetribAnnua707QtaB5" Enabled="true"
                                    ErrorMessage="Quota Retributiva Annua dal 01/01/98: Inserire valori interi o decimali"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </asp:Panel>
                    </tr>
                </table>
            </asp:Panel>
            <!-- fine pannello quota INPDAP-->
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblCessazione" runat="server" Text="Dati Cessazione" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileCessazioneAA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator25" ControlToValidate="txtServizioUtileCessazioneAA"
                            ErrorMessage="Servizio Utile Cessazione: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneMM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator26" ControlToValidate="txtServizioUtileCessazioneMM"
                            ErrorMessage="Servizio Utile Cessazione: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneGG" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator27" ControlToValidate="txtServizioUtileCessazioneGG"
                            ErrorMessage="Servizio Utile Cessazione: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo707" />
                        <label>
                            GG</label>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Quota pensione retributiva annua 707:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox ID="txtPensioneRetribAnnua707QtaB4" runat="server" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7"
                            Display="Dynamic" ControlToValidate="txtPensioneRetribAnnua707QtaB4" Enabled="true" ErrorMessage="Quota pensione retributiva annua 707 Quota B: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo707" ValidationExpression="\d+(\,\d{1,4})?" />
                    </td>
                </tr>
            </table>
            <!-- Fine Pannello Dati Calcolo Retributivi FS_PT-->
        </asp:Panel>        
    </div>
</asp:Panel>
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiCalcolo707" runat="server" CausesValidation="false" ValidationGroup="UCTabDatiCalcolo707"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiCalcolo707_Click" Text="Salva Dati Calcolo 707"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo707')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary"/>
                <asp:Button ID="btnEliminaDatiCalcolo707" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo 707" Width="155px" OnClick="btnEliminaDatiCalcolo707_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo 707?')) return false; else BlockUI();" CssClass="ghost-delete" />
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField ID="FlagUnicarpe" runat="server" />
<asp:HiddenField ID="HdnFondo" runat="server" />
