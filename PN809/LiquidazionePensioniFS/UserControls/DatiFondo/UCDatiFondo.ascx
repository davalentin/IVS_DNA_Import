<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiFondo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCDatiFondo" %>
<script type="text/javascript">
    function getDecorrenzaRegistrazione() {
        // Se non è visibile la textbox allora sarà visibile la label
        var decorrenza = document.getElementById("<%= txtDecorrenzaRegistrazione.ClientID %>");
        if (decorrenza)
            return decorrenza.value;

        decorrenza = document.getElementById("<%= lblDecorrenzaRegistrazione.ClientID %>");
        if (decorrenza)
            return decorrenza.outerText;

        return "";
    }
</script>
<asp:Panel runat="server" ID="pnlUCDatiFondo">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 30%">
                <label style="font-weight: bold">
                    Decorrenza Registrazione:</label>
            </td>
            <td class="field" style="text-align: left; width: 25%">
                <asp:TextBox runat="server" ID="txtDecorrenzaRegistrazione" Width="50%" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                    MaxLength="10"></asp:TextBox>
                <asp:Label runat="server" ID="lblDecorrenzaRegistrazione" Visible="false"></asp:Label>
                <asp:RegularExpressionValidator ID="REVtxtDecorrenzaRegistrazione" ControlToValidate="txtDecorrenzaRegistrazione"
                    ErrorMessage="Decorrenza Registrazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiFondo" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaRegistrazione"
                    Display="Dynamic" ErrorMessage="Decorrenza Registrazione: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiFondo" ID="customCheckDatatxtDecorrenzaRegistrazione"
                    ClientValidationFunction="checkCorrettezzaData" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenzaRegistrazione" Display="Dynamic"
                    ErrorMessage="Decorrenza Registrazione: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo"
                    ControlToValidate="txtDecorrenzaRegistrazione"></asp:RequiredFieldValidator>
            </td>
            <td style="width: 45%"></td>
        </tr>
    </table>
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr id="trTipoPensione" runat="server">
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                </td>
                <td class="Row1 none" colspan="2"></td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Calcolo:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaCalcolo"></asp:Label>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensione"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <table class="tabellaFormattazione  grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tredicesima Mensilità:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlTredicesimaMens" Width="30.5%" CssClass="tb8 txtUppercase xxs">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlTredicesimaMens" Display="Dynamic"
                    ErrorMessage="Tredicesima Mensilità: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo"
                    ControlToValidate="ddlTredicesimaMens"></asp:RequiredFieldValidator>
            </td>

            <td class="Row1" style="width: 25%" id="tdLblTitAltraPensione" runat="server">
                <label>
                    Titolare Altra Pensione:</label>
            </td>
            <td class="field" style="width: 25%" id="tdDdlTitAltraPensione" runat="server">
                <asp:DropDownList runat="server" ID="ddlTitAltraPensione" Width="30.5%" CssClass="tb8 txtUppercase xxs">
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1 none" colspan="2" id="tdNOTitAltraPensione" runat="server" visible="false"></td>
        </tr>
        <asp:Panel runat="server" ID="pnlIncrementoContrattuale" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Incremento contrattuale:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:Label runat="server" ID="lblIncrementoContrattuale"></asp:Label>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlPagamentoIndennitaIntegrativaSpeciale" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Pagamento Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlPagIndennIntegrSpec" Width="10%" CssClass="tb8 txtUppercase xxs">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" Display="Dynamic"
                        ErrorMessage="Pagamento Indennità Integrativa Speciale: campo obbligatorio" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiFondo" ControlToValidate="ddlPagIndennIntegrSpec"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlDirittoIndennitaIntegrativaSpeciale" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Diritto Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlDirittoIndennIntegrSpec" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="24">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" Display="Dynamic"
                        ErrorMessage="Diritto Indennità Integrativa Speciale: campo obbligatorio" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiFondo" ControlToValidate="ddlDirittoIndennIntegrSpec"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlIntegrazioneMinimo" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Integrazione Minimo:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlIntegrazioneMinimo" Width="10%" CssClass="tb8 txtUppercase xxs">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr runat="server" id="trIndennIntegrSpecConglobata" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Indennità Integrativa Speciale Conglobata:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlIndennIntegrSpecConglobata" Width="30.5%"
                    CssClass="tb8 txtUppercase xxs" TabIndex="20">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>

        <asp:Panel runat="server" ID="Art4" Visible="false">
            <tr>
                <td class="Row1 shift-full-grid" style="width: 30%" colspan="2">
                    <label style="font-weight: bold" class="section-label">
                        Trattenute Art. 4 D.lgs. 165/97</label>
                </td>
            </tr>
            <tr>
                <asp:Panel runat="server" ID="pnlNumeroRate" Visible="false">
                    <td class="Row1" style="width: 24%">
                        <label>
                            Numero Rate:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox runat="server" ID="txtNumeroRate" CssClass="txtUppercase tb8 offClass onClassLegge336"
                            TabIndex="18" MaxLength="11" Width="75%" />
                        <asp:RegularExpressionValidator ID="RegExValNumeroRate" runat="server" ControlToValidate="txtNumeroRate"
                            ErrorMessage="Il Numero Rate non può essere un numero negativo" ValidationExpression="^\d+$" Text="*" CssClass="field-is-required"
                            Display="Dynamic" ValidationGroup="UCTabDatiFondo" Enabled="true"></asp:RegularExpressionValidator>

                    </td>

                </asp:Panel>

                <td class="Row1" style="width: 24%">
                    <label id="labelImportoSingolaRata" runat="server">
                        Importo Singola Rata:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoSingolaRata" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="11" Width="75%" />
                </td>
            </tr>
        </asp:Panel>


        <asp:Panel runat="server" ID="pnlIndennitaSpeciale" Visible="false">
            <tr>
                <td class="Row1 shift-full-grid" style="width: 30%" colspan="2">
                    <label style="font-weight: bold">
                        Indennita Speciale:</label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 24%">
                    <label>
                        Codice Indennizzo:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtCodiceIndennizzo" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="4" Width="75%" />
                </td>
                <td class="Row1" style="width: 24%">
                    <label id="label2" runat="server">
                        Importo Indenizzo:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoIndenizzo" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" />
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoIndenizzo" Display="Dynamic"
                        ControlToValidate="txtImportoIndenizzo" Enabled="true" ErrorMessage="Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 24%">
                    <label>
                        Data Inizio Indennizzo:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox Style="text-align: left" runat="server"
                        onkeydown="checkTabPress(this)" ID="txtInizioIndennizzo" Width="55%" Text="gg/mm/aaaa"
                        CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="5" MaxLength="10"
                        DataFormatString="{0:dd/MM/yyyy}"> </asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaInizioIndenizzo" ControlToValidate="txtInizioIndennizzo"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required"
                        ErrorMessage="Formato data non corretto" Display="Dynamic" ValidationGroup="UCTabDatiFondo" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioIndennizzo"
                        Display="Dynamic" ErrorMessage="Inizio Indennizzo: data illogica" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiFondo" ID="customCheckDataInizioIndenizzo"
                        ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 24%">
                    <label>
                        Data Fine Indennizzo:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox Style="text-align: left" runat="server"
                        onkeydown="checkTabPress(this)" ID="txtFineIndennizzo" Width="55%" Text="gg/mm/aaaa"
                        CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="5" MaxLength="10"
                        DataFormatString="{0:dd/MM/yyyy}"> </asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaFineIndenizzo" ControlToValidate="txtFineIndennizzo"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required"
                        ErrorMessage="Formato data non corretto" Display="Dynamic" ValidationGroup="UCTabDatiFondo" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineIndennizzo"
                        Display="Dynamic" ErrorMessage="Fine Indennizzo: data illogica" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiFondo" ID="customCheckDataFineIndenizzo"
                        ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>

                <tr>
               <td class="Row1" style="width: 24%">
                    <label id="lblImportoRataIniziale" runat="server">
                        Importo Rata Iniziale:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoRataIniziale" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" />
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                        ControlToValidate="txtImportoRataIniziale" Enabled="true" ErrorMessage="Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
                <td class="Row1" style="width: 24%">
                    <label id="lblImportoRataFinale" runat="server">
                        Importo Rata Finale:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoRataFinale" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" />
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                        ControlToValidate="txtImportoRataFinale" Enabled="true" ErrorMessage="Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
             <tr>
             <td class="Row1" style="width: 24%">
                    <label id="lblImportoRata" runat="server">
                        Importo Rata:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoRata" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" />
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                        ControlToValidate="txtImportoRata" Enabled="true" ErrorMessage="Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondo" ValidationExpression="\d+(\,\d{1,4})?" />
                     <td class="Row1" style="width: 24%">
                        <label>
                            Numero Rate:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox runat="server" ID="txtNumRate" CssClass="txtUppercase tb8 offClass onClassLegge336"
                            TabIndex="18" MaxLength="11" Width="75%" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="txtNumRate"
                            ErrorMessage="Il Numero Rate non può essere un numero negativo" ValidationExpression="^\d+$" Text="*" CssClass="field-is-required"
                            Display="Dynamic" ValidationGroup="UCTabDatiFondo" Enabled="true"></asp:RegularExpressionValidator>

                    </td>
                </tr>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlIndennitàSpecialeLorda" Visible="false">
            <tr>
                <td class="Row1" style="width: 24%">
                    <label id="lblImportoIndennitaSpecialeLorda" runat="server">
                        Importo Indennità Speciale Lorda:</label>
                </td>
                <td class="Row1" style="width: 20%">
                    <asp:TextBox runat="server" ID="txtImportoIndennitaSpecialeLorda" CssClass="txtUppercase tb8 offClass onClassLegge336"
                        TabIndex="18" MaxLength="20" Width="75%" Enabled = "false"/>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiFondo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Fondo" Width="180px" OnClick="SalvaDatiFondo_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiFondo')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary" />
                <asp:Button ID="btnEliminaDatiFondo" SkinID="btnAzione1" runat="server" Width="180px"
                    Text="Elimina Dati Fondo" CausesValidation="False" OnClick="btnEliminaDatiFondo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="180px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" />
            </td>
        </tr>
    </table>
</div>
