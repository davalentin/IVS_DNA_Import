<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcolo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondo.UCDatiCalcolo" %>
    
    <script type="text/javascript">
    // Memo 79
        function sommaServizioUtili() {
            // Prendi anni
            var anni1 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoAA.ClientID %>").value) || 0;
            var anni2 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoOIAA.ClientID %>").value) || 0;

            // Prendi mesi
            var mesi1 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoMM.ClientID %>").value) || 0;
            var mesi2 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoOIMM.ClientID %>").value) || 0;

            // Prendi giorni
            var giorni1 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoGG.ClientID %>").value) || 0;
            var giorni2 = parseInt(document.getElementById("<%=txtAnniServUtiliDirittoOIGG.ClientID %>").value) || 0;

            // Somma base
            var totAnni = anni1 + anni2;
            var totMesi = mesi1 + mesi2;
            var totGiorni = giorni1 + giorni2;

            // Normalizza giorni: ogni 30 giorni > aggiungi un mese
            if (totGiorni > 29) {
                var extraMesi = Math.floor(totGiorni / 30);
                totMesi += extraMesi;
                totGiorni = totGiorni % 30;
            }

            // Normalizza mesi: ogni 12 mesi > aggiungi un anno
            if (totMesi > 11) {
                var extraAnni = Math.floor(totMesi / 12);
                totAnni += extraAnni;
                totMesi = totMesi % 12;
            }

            // Imposta i valori negli input
            document.getElementById("<%=txtAnniServUtiliDirittoTotAA.ClientID %>").value = totAnni;
            document.getElementById("<%=txtAnniServUtiliDirittoTotMM.ClientID %>").value = totMesi;
            document.getElementById("<%=txtAnniServUtiliDirittoTotGG.ClientID %>").value = totGiorni;
        }
    </script>

<asp:Panel ID="pnlUCDatiCalcolo" runat="server">
    <table class="tabellaFormattazione grid grid-size-25">
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
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Pensione Annua Lorda:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtPensioneAnnuaLorda" runat="server" CssClass="tb8 txtUppercase"
                            Width="50%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" Display="Dynamic"
                            ControlToValidate="txtPensioneAnnuaLorda" Enabled="true" ErrorMessage="Pensione Annua Lorda: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtPensioneAnnuaLorda"
                            Display="Dynamic" Enabled="true" ErrorMessage="Pensione Annua Lorda: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Servizio Utile Diritto:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 29%">
                        <asp:TextBox ID="txtAnniServUtiliDirittoAA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniServUtiliDirittoAA"
                            ControlToValidate="txtAnniServUtiliDirittoAA" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di anni in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RFV_txtAnniServUtiliDirittoAA" ControlToValidate="txtAnniServUtiliDirittoAA"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoMM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniServUtiliDirittoMM"
                            ControlToValidate="txtAnniServUtiliDirittoMM" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di mesi in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RFV_txtAnniServUtiliDirittoMM" ControlToValidate="txtAnniServUtiliDirittoMM"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoGG" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REV_txtAnniServUtiliDirittoGG"
                            ControlToValidate="txtAnniServUtiliDirittoGG" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto: inserire il numero di giorni in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RFV_txtAnniServUtiliDirittoGG" ControlToValidate="txtAnniServUtiliDirittoGG"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            GG</label>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlNSettimane_OrganizzazioniInternazionali">
                <tr>
                    <td class="Row1" style="width: 22%">
                    </td>
                    <td class="Row1" style="width: 25%">
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Servizio Utile Diritto OI:</label>
                    </td>
                    <td class="Row1" style="width: 29%">
                        <asp:TextBox ID="txtAnniServUtiliDirittoOIAA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator34"
                            ControlToValidate="txtAnniServUtiliDirittoOIAA" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto OI: inserire il numero di anni in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtAnniServUtiliDirittoOIAA"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto OI: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoOIMM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator35"
                            ControlToValidate="txtAnniServUtiliDirittoOIMM" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto OI: inserire il numero di mesi in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtAnniServUtiliDirittoOIMM"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto OI: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoOIGG" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" onblur="sommaServizioUtili()"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator36"
                            ControlToValidate="txtAnniServUtiliDirittoOIGG" Display="Dynamic" ErrorMessage="Anni di Servizio Utili per il Diritto OI: inserire il numero di giorni in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" ControlToValidate="txtAnniServUtiliDirittoOIGG"
                            Display="Dynamic" Enabled="true" ErrorMessage="Anni Servizio Utili Diritto OI: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            GG</label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 22%">
                    </td>
                    <td class="Row1" style="width: 25%">
                    </td>
                    <td class="Row1" style="width: 24%">
                        <label>
                            Servizio Utile Diritto TOT:</label>
                    </td>
                    <td class="Row1" style="width: 29%">
                        <asp:TextBox ID="txtAnniServUtiliDirittoTotAA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" ReadOnly="true"></asp:TextBox>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoTotMM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" ReadOnly="true"></asp:TextBox>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtAnniServUtiliDirittoTotGG" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2" ReadOnly="true"></asp:TextBox>
                        <label>
                            GG</label>
                    </td>
                </tr>
                </asp:Panel>
                <tr runat="server" id="trDivisore" visible="false">
                    <td class="Row1" style="width: 22%">
                        <label>
                            Divisore:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:TextBox ID="txtDivisore" runat="server" CssClass="tb8 txtUppercase" Width="50%"
                            MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REV_txtDivisore" Display="Dynamic"
                            ControlToValidate="txtDivisore" Enabled="true" ErrorMessage="Divisore: Inserire valori interi"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,2}" />
                    </td>
                </tr>
                <tr runat="server" id="trCapitolo" visible="false">
                    <td class="Row1" style="width: 22%">
                        <label>
                            Capitolo:</label>
                    </td>
                    <td class="Row1" style="width: 25%" colspan="2">
                        <asp:DropDownList runat="server" ID="ddlCapitolo" Width="90%" CssClass="tb8 txtUppercase xl"
                    TabIndex="8"/>
                    </td>
                </tr>
                <tr runat="server" >
                    <td class="Row1" style="width: 22%" runat="server" id="tdLblCoefficienteTrasformazione" visible="false">
                        <label>
                            Coefficiente di Trasformazione:</label> 
                    </td>
                    <td class="Row1" style="width: 25%" runat="server" id="tdTxtCoefficienteTrasformazione" visible="false">
                        <asp:TextBox ID="txtCoefficienteTrasformazione" runat="server" CssClass="tb8 txtUppercase"
                            Width="50%" MaxLength="11"></asp:TextBox>
                    </td> 
                     <td class="Row1 none" style="width: 22%" runat="server" id="tdNoCoefficienteTrasformazione" colspan ="2"> 
                    </td>
                    <td class="Row1 bold" style="width: 24%">
                        <label>
                            Benefici L.336/70:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox runat="server" ID="txtRetribuzioneSenzaBenefici336" CssClass="txtUppercase tb8 offClass onClassLegge336"
                            TabIndex="18" MaxLength="11" Width="75%" />
                        <asp:RegularExpressionValidator runat="server" ID="txtRetribuzioneSenzaBenefici336_RV"
                            ControlToValidate="txtRetribuzioneSenzaBenefici336" Display="Dynamic" ErrorMessage="Retribuzione senza benefici L.336/70: Inserire massimo 6 cifre intere e 4 decimali"
                            Text="*" CssClass="field-is-required" ValidationExpression="\d{1,6}(\,\d{1,4})?" ValidationGroup="UCTabDatiCalcolo" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Common FS_PT -->
        <asp:Panel ID="pnlDatiRetributivi" runat="server" Visible="false">
            <!-- Pannello Dati Calcolo Retributivi FS_PT-->
            <table class="tabellaFormattazione grid grid-size-25">
                <tr style="min-height: 50px; vertical-align: bottom">
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblDatiRetributivi" runat="server" Text="Dati Retributivi:" Style="font-weight: bold;
                            font-size: 15px;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaA" runat="server" Text="QUOTA A" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblData92" runat="server" Text="Dati al 31/12/92" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAAQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileAAQtaA" ControlToValidate="txtServizioUtileAAQtaA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/92: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMMQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileMMQtaA" ControlToValidate="txtServizioUtileMMQtaA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/92: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGGQtaA"
                            ErrorMessage="Servizio Utile al 31/12/92: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileGGQtaA" ControlToValidate="txtServizioUtileGGQtaA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/92: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
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
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtRetribuzioneQtaA" ControlToValidate="txtRetribuzioneQtaA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Retribuzione ultimo mese: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="Row1 none" colspan="2">
                    </td>
                    <asp:Panel ID="pnlQuotaRetributivaAnnua" runat="server" Visible="false">
                        <td class="Row1 bold" style="width: 24%">
                            <label>
                                Quota pensione retributiva annua:</label>
                        </td>
                        <td class="Row1 regular" style="width: 20%">
                            <asp:TextBox ID="txtQuotaRetributivaAnnua" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator23"
                                Display="Dynamic" ControlToValidate="txtQuotaRetributivaAnnua" Enabled="true"
                                ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </asp:Panel>
                </tr>
                <tr>
                    <td class="Row1 none" colspan="2">
                    </td>
                    <td class="Row1 bold" style="width: 24%">
                        <label>
                            Importo Indennità Integrativa Speciale:</label>
                    </td>
                    <td class="Row1 regular" style="width: 20%">
                        <asp:TextBox ID="txtImpIndenIntegrSpecQtaA" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" TabIndex="17" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" Display="Dynamic"
                            ControlToValidate="txtImpIndenIntegrSpecQtaA" Enabled="true" ErrorMessage="Importo Indennità Integrativa Speciale: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvtxtImpIndenIntegrSpecQtaA" ControlToValidate="txtImpIndenIntegrSpecQtaA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Importo Indennità Integrativa Speciale: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>            
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaB" runat="server" Text="QUOTA B" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblData94" runat="server" Text="Dati al 31/12/94" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input" style="width: 34%">
                        <asp:TextBox ID="txtServizioUtileAAQtaB1" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="19" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator8" ControlToValidate="txtServizioUtileAAQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileAAQtaB1" ControlToValidate="txtServizioUtileAAQtaB1"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/94: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB1" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="20" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtServizioUtileMMQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileMMQtaB1" ControlToValidate="txtServizioUtileMMQtaB1"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/94: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB1" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="21" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtServizioUtileGGQtaB1"
                            ErrorMessage="Servizio Utile al 31/12/94: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileGGQtaB1" ControlToValidate="txtServizioUtileGGQtaB1"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/94: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
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
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtRMSQtaB1" ControlToValidate="txtRMSQtaB1"
                            Display="Dynamic" Enabled="false" ErrorMessage="Retribuzione Media: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="Row1 none" colspan="2">
                    </td>
                    <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB94" runat="server" Visible="false">
                        <td class="Row1 bold" style="width: 24%">
                            <label>
                                Quota pensione retributiva annua:</label>
                        </td>
                        <td class="Row1 regular" style="width: 20%">
                            <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB94" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator24"
                                Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB94" Enabled="true"
                                ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </asp:Panel>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="Label2" runat="server" Text="Dati al 31/12/95" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileAAQtaB2" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator16" ControlToValidate="txtServizioUtileAAQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfVTxtServizioUtileAAQtaB2" ControlToValidate="txtServizioUtileAAQtaB2"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/95: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB2" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator17" ControlToValidate="txtServizioUtileMMQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileMMQtaB2" ControlToValidate="txtServizioUtileMMQtaB2"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/95: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB2" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator18" ControlToValidate="txtServizioUtileGGQtaB2"
                            ErrorMessage="Servizio Utile al 31/12/95: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileGGQtaB2" ControlToValidate="txtServizioUtileGGQtaB2"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/95: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            GG</label>
                    </td>
                    <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB95" runat="server" Visible="false">
                        <td class="Row1" style="width: 24%">
                            <label>
                                Quota pensione retributiva annua:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB95" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator31"
                                Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB95" Enabled="true"
                                ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </asp:Panel>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblData97" runat="server" Text="Dati al 31/12/97" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileAAQtaB3" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator19" ControlToValidate="txtServizioUtileAAQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileAAQtaB3" ControlToValidate="txtServizioUtileAAQtaB3"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/97: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileMMQtaB3" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator20" ControlToValidate="txtServizioUtileMMQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileMMQtaB3" ControlToValidate="txtServizioUtileMMQtaB3"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/97: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileGGQtaB3" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator21" ControlToValidate="txtServizioUtileGGQtaB3"
                            ErrorMessage="Servizio Utile al 31/12/97: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileGGQtaB3" ControlToValidate="txtServizioUtileGGQtaB3"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati al 31/12/97: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            GG</label>
                    </td>
                    <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB97" runat="server" Visible="false">
                        <td class="Row1" style="width: 24%">
                            <label>
                                Quota pensione retributiva annua:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB97" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator32"
                                Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB97" Enabled="true"
                                ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </asp:Panel>
                </tr>
            </table>
            <asp:Panel runat="server" ID="pnlDatiPost97" Visible="false">
                <table class="tabellaFormattazione grid grid-size-25">
                    <tr>
                        <td class="Row1" style="text-align: left">
                            <asp:Label ID="Label1" runat="server" Text="Dati dal 01/01/98" Style="font-weight: bold"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-size-25">
                    <tr>
                        <td class="Row1" style="width: 22%">
                            <label>
                                Servizio Utile:</label>
                        </td>
                        <td class="Row1 fileds-date-input">
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
                        <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaB98" runat="server" Visible="false">
                            <td class="Row1" style="width: 24%">
                                <label>
                                    Quota pensione retributiva annua:</label>
                            </td>
                            <td class="Row1" style="width: 20%">
                                <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaB98" runat="server" CssClass="tb8 txtUppercase"
                                    Width="75%" MaxLength="11"></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtQuotaPensioneRetributivaAnnuaB98"
                                    Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaB98" Enabled="true"
                                    ErrorMessage="Quota Retributiva Annua dal 01/01/98: Inserire valori interi o decimali"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                            </td>
                        </asp:Panel>
                    </tr>
                </table>
            </asp:Panel>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblCessazione" runat="server" Text="Dati Cessazione" Style="font-weight: bold"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 22%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 fileds-date-input">
                        <asp:TextBox ID="txtServizioUtileCessazioneAA" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator25" ControlToValidate="txtServizioUtileCessazioneAA"
                            ErrorMessage="Servizio Utile Cessazione: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileCessazioneAA" ControlToValidate="txtServizioUtileCessazioneAA"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati Cessazione: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            AA</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneMM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator26" ControlToValidate="txtServizioUtileCessazioneMM"
                            ErrorMessage="Servizio Utile Cessazione: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileCessazioneMM" ControlToValidate="txtServizioUtileCessazioneMM"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati Cessazione: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            MM</label>
                        <asp:TextBox ID="txtServizioUtileCessazioneGG" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator27" ControlToValidate="txtServizioUtileCessazioneGG"
                            ErrorMessage="Servizio Utile Cessazione: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtServizioUtileCessazioneGG" ControlToValidate="txtServizioUtileCessazioneGG"
                            Display="Dynamic" Enabled="false" ErrorMessage="Servizio Utile dati Cessazione: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                        <label>
                            GG</label>
                    </td>
                    <asp:Panel ID="pnlQuotaPensioneRetributivaAnnuaCessazione" runat="server" Visible="false">
                        <td class="Row1" style="width: 24%">
                            <label>
                                Quota pensione retributiva annua:</label>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <asp:TextBox ID="txtQuotaPensioneRetributivaAnnuaCessazione" runat="server" CssClass="tb8 txtUppercase"
                                Width="75%" MaxLength="11"></asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator33"
                                Display="Dynamic" ControlToValidate="txtQuotaPensioneRetributivaAnnuaCessazione"
                                Enabled="true" ErrorMessage="QuotaRetributivaAnnua: Inserire valori interi o decimali"
                                Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        </td>
                    </asp:Panel>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Retributivi FS_PT-->
        <!-- Pannello Dati Calcolo Contributivi FS_PT -->
        <asp:Panel ID="pnlDatiContributiviFS_PT" runat="server" Visible="false">
            <table class="tabellaFormattazione grid grid-size-25" width="100%">
                <tr style="min-height: 50px; vertical-align: bottom">
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblContributiviFS_PT" runat="server" Text="Dati Contributivi da Legge 335:"
                            Style="font-weight: bold; font-size: 15px;"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25" width="100%">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Importo Contributivo Totale:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtImportoContributivoTotaleFS_PT" runat="server" CssClass="tb8 txtUppercase"
                            Width="53%" TabIndex="43" MaxLength="12"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                            ControlToValidate="txtImportoContributivoTotaleFS_PT" Enabled="true" ErrorMessage="Importo Contributivo Totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtImportoContributivoTotaleFS_PT"
                            ControlToValidate="txtImportoContributivoTotaleFS_PT" Display="Dynamic" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                            Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <label>
                            Settimane:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox runat="server" ID="txtSettimaneFS_PT" CssClass="tb8 txtUppercase" Width="75%"
                            MaxLength="11" TabIndex="48"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator13"
                            ControlToValidate="txtSettimaneFS_PT" Display="Dynamic" ErrorMessage="Numero Settimane L. 335 non valido: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtSettimaneFS_PT" ControlToValidate="txtSettimaneFS_PT"
                            Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 335: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Montante:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtMontanteFS_PT" runat="server" CssClass="tb8 txtUppercase" Width="53%"
                            MaxLength="12"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                            ControlToValidate="txtMontanteFS_PT" Enabled="false" ErrorMessage="Montante: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtMontanteFS_PT" ControlToValidate="txtMontanteFS_PT"
                            Display="Dynamic" Enabled="false" ErrorMessage="Montante L. 335: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 25%">
                        Quota pensione contributiva annua:
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtImportoQuotaCFS_PT" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" TabIndex="45" MaxLength="12"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" Display="Dynamic"
                            ControlToValidate="txtImportoQuotaCFS_PT" Enabled="true" ErrorMessage="Importo Quota C: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtImportoQuotaCFS_PT" ControlToValidate="txtImportoQuotaCFS_PT"
                            Display="Dynamic" Enabled="false" ErrorMessage="Importo Quota C: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Contributivi FS_PT -->
        <!-- Pannello Dati Calcolo Contributivi L.214 FS_PT -->
        <asp:Panel ID="pnlDatiCalcoloContributiviLegge214_VL_FS_PT" runat="server" Visible="false">
            <table class="tabellaFormattazione grid grid-size-25" width="100%">
                <tr>
                    <td class="Row1" style="text-align: left">
                        <asp:Label ID="lblDatiContributiviL214" runat="server" Text="Dati Contributivi da L. 214"
                            Style="font-weight: bold; font-size: 15px;" CssClass="section-label mt-32"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="tabellaFormattazione grid grid-size-25" width="100%">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Importo contributivo totale:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox runat="server" ID="txtImportoContribTotaleQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="53%" MaxLength="12" TabIndex="46"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator28"
                            Display="Dynamic" ControlToValidate="txtImportoContribTotaleQuotaDL214" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtImportoContribTotaleQuotaDL214"
                            ControlToValidate="txtImportoContribTotaleQuotaDL214" Display="Dynamic" Enabled="true"
                            ErrorMessage="Importo Contributivo Totale L. 214: campo obbligatorio" ValidationGroup="UCTabDatiCalcolo"
                            Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <label>
                            Settimane:</label>
                    </td>
                    <td class="Row1" style="width: 20%">
                        <asp:TextBox runat="server" ID="txtNSettimaneQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="75%" MaxLength="11" TabIndex="48"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator30"
                            ControlToValidate="txtNSettimaneQuotaDL214" Display="Dynamic" ErrorMessage="Numero Settimane L. 214 non valido: inserire il numero di settimane in un formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcolo" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtNSettimaneQuotaDL214" ControlToValidate="txtNSettimaneQuotaDL214"
                            Display="Dynamic" Enabled="true" ErrorMessage="Numero Settimane L. 214: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Montante:</label>
                    </td>
                    <td class="Row1" >
                        <asp:TextBox runat="server" ID="txtMontanteQuotaDL214" CssClass="tb8 txtUppercase"
                            Width="53%" MaxLength="12" TabIndex="47"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator29"
                            Display="Dynamic" ControlToValidate="txtMontanteQuotaDL214" Enabled="true" ErrorMessage="Montante L. 214: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d{0,7}(,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtMontanteQuotaDL214" ControlToValidate="txtMontanteQuotaDL214"
                            Display="Dynamic" Enabled="true" ErrorMessage="Montante L. 214: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Quota pensione contributiva annua:</label>
                    </td>
                    <td class="Row1">
                        <asp:TextBox ID="txtQuotaPensioneContributivaAnnuaDL214" runat="server" CssClass="tb8 txtUppercase"
                            Width="75%" MaxLength="11"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator22"
                            Display="Dynamic" ControlToValidate="txtQuotaPensioneContributivaAnnuaDL214"
                            Enabled="true" ErrorMessage="QuotaPensioneRetributivaAnnuaDL214: Inserire valori interi o decimali"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcolo" ValidationExpression="\d+(\,\d{1,4})?" />
                        <asp:RequiredFieldValidator runat="server" ID="RfvTxtQuotaPensioneContributivaAnnuaDL214"
                            ControlToValidate="txtQuotaPensioneContributivaAnnuaDL214" Display="Dynamic"
                            Enabled="false" ErrorMessage="Quota pensione contributiva annua: campo obbligatorio"
                            ValidationGroup="UCTabDatiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <!-- Fine Pannello Dati Calcolo Contributivi L.214 -->
    </div>
</asp:Panel>
<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" CausesValidation="false" ValidationGroup="UCTabDatiCalcolo"
                    SkinID="btnAzione1" Width="180px" OnClick="btnSalvaDatiCalcolo_Click" Text="Salva Dati Calcolo"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiCalcolo')){aspnetForm.target ='_self'; BlockUI();}"  CssClass="force-right primary"/>
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo" Width="180px" OnClick="btnEliminaDatiCalcolo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elenco Registrazioni" Width="180px" OnClick="TornaElencoRegistrazioni_Click"
                    OnClientClick="BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField ID="FlagUnicarpe" runat="server" />
<asp:HiddenField ID="HdnFondo" runat="server" />
