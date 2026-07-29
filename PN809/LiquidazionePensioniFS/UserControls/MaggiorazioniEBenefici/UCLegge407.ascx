<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLegge407.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCLegge407" %>
<!-- Pannello D.L.407 -->
<asp:Panel ID="pnlDL407" runat="server">
    <div id="pdivRetributivo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 15px; margin-bottom: 4px"
        runat="server">
        <table class="tabellaFormattazione grid tabellaFormattazione.grid-size-20-col-5">
            <tr>
                <td class="Row1" style="text-align: left">
                    <asp:Label ID="lblTitoloDL407" runat="server" Text="Decreto Legislativo 407" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="tabellaFormattazione grid grid-size-20-col-5--2" cellpadding="3" cellspacing="1" border="0" width="100%">
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Retribuzione Media Settimanale A:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtRMSA" CssClass="tb8 txtUppercase" MaxLength="15"
                        Width="90%" TabIndex="1"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSA" Display="Dynamic"
                        ControlToValidate="txtRMSA" Enabled="true" ErrorMessage="Retribuzione Media Settimanale A: Inserire massimo 6 cifre intere e 4 decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
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
                    <asp:TextBox runat="server" ID="txtSettimaneA" CssClass="tb8 txtUppercase" Width="100%"
                        MaxLength="4" TabIndex="2"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneA" ControlToValidate="txtSettimaneA"
                        Display="Dynamic" ErrorMessage="Numero settimane A non valido: inserire il numero di settimane in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabLegge407" />
                </td>
                <td class="Row1 none" style="width: 5%">
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Retribuzione Media Settimanale B:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtRMSB" CssClass="tb8 txtUppercase" Width="90%"
                        MaxLength="15" TabIndex="3"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSB" Display="Dynamic"
                        ControlToValidate="txtRMSB" Enabled="true" ErrorMessage="Retribuzione Media Settimanale B: Inserire massimo 6 cifre intere e 4 decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
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
                    <asp:TextBox runat="server" ID="txtSettimaneB" CssClass="tb8 txtUppercase" Width="100%"
                        MaxLength="4" TabIndex="4"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneB" ControlToValidate="txtSettimaneB"
                        Display="Dynamic" ErrorMessage="Numero settimane B non valido: inserire il numero di settimane in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabLegge407" />
                </td>
                <td class="Row1 none" style="width: 5%">
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 33%">
                </td>
                <td class="Row1" style="width: 30%">
                </td>
                <td class="Row1" style="width: 3%">
                </td>
                <td class="Row1" style="width: 14%">
                    <label class="etichettaBold">
                        Settimane C:</label>
                </td>
                <td class="Row1" style="width: 15%">
                    <asp:TextBox runat="server" ID="txtSettimaneC" CssClass="tb8 txtUppercase" Width="100%"
                        MaxLength="4" TabIndex="5"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneC" ControlToValidate="txtSettimaneC"
                        Display="Dynamic" ErrorMessage="Numero settimane C non valido: inserire il numero di settimane in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabLegge407" />
                </td>
                <td class="Row1 none" style="width: 5%">
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 33%">
                    <label>
                        Retribuzione Media Settimanale D:</label>
                </td>
                <td class="Row1" style="width: 30%">
                    <asp:TextBox runat="server" ID="txtRMSD" CssClass="tb8 txtUppercase" Width="90%"
                        MaxLength="15" TabIndex="6"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMSD" Display="Dynamic"
                        ControlToValidate="txtRMSD" Enabled="true" ErrorMessage="Retribuzione Media Settimanale D: Inserire massimo 6 cifre intere e 4 decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                </td>
                <td class="Row1" style="width: 3%">
                    <label>
                        €</label>
                </td>
                <td class="Row1" style="width: 14%">
                    <label class="etichettaBold">
                        Settimane D:</label>
                </td>
                <td class="Row1" style="width: 15%">
                    <asp:TextBox runat="server" ID="txtSettimaneD" CssClass="tb8 txtUppercase" Width="100%"
                        MaxLength="4" TabIndex="7"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneD" ControlToValidate="txtSettimaneD"
                        Display="Dynamic" ErrorMessage="Numero settimane D non valido: inserire il numero di settimane in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabLegge407" />
                </td>
                <td class="Row1 none" style="width: 5%">
                </td>
            </tr>
        </table>
        <!-- Fine Pannello D.L.407 -->
    </div>
</asp:Panel>
<asp:Panel ID="pnlELAnteArmonizzazione" runat="server" Visible="false" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-top: 15px; margin-bottom: 4px">
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="text-align: left">
                <label style="font-weight: bold" class="section-label mt-32">
                    Dati Ante 01/01/93 (Quota A)</label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-25" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaA_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="1" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_AA" ControlToValidate="txtELAnteArmQtaA_AA"
                    ErrorMessage="Servizio Utile Quota A: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />
                <%--   <asp:TextBox ID="txtELAnteArmQtaA_MM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="2" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_MM" ControlToValidate="txtELAnteArmQtaA_MM"
                            ErrorMessage="Servizio Utile Quota A: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />--%>
            </td>
            <td class="Row1" style="width: 25%" runat="server" id="Td3">
                <label>
                    ControCodice Retr.:</label>
            </td>
            <td class="Row1" style="width: 36%" runat="server" id="Td4">
                <asp:TextBox ID="txtELAnteArmQtaA_CC" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="5" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaA_CC" ControlToValidate="txtELAnteArmQtaA_CC"
                    ErrorMessage="ControCodice Retr. Quota A: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaA_RetrPens" runat="server" CssClass="tb8 txtUppercase"
                    Width="50%" TabIndex="4" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQuotaA_RetrPens"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaA_RetrPens" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile Quota A: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Pensionabile S.L 336:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaA_RetrPensSL336" runat="server" CssClass="tb8 txtUppercase"
                    Width="50%" TabIndex="4" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQtaA_RetrPensSL336"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaA_RetrPensSL336" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile S.L 336 Quota A: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
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
    <table class="tabellaFormattazione grid grid-size-25" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaB_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="6" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_AA" ControlToValidate="txtELAnteArmQtaB_AA"
                    ErrorMessage="Servizio Utile Quota B: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />
                <%-- <asp:TextBox ID="txtELAnteArmQtaB_MM" runat="server" CssClass="tb8 txtUppercase"
                            Width="30px" TabIndex="7" MaxLength="2"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_MM" ControlToValidate="txtELAnteArmQtaB_MM"
                            ErrorMessage="Servizio Utile Quota B: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />--%>
            </td>
            <td class="Row1" style="width: 25%" runat="server" id="Td6">
                <label>
                    ControCodice Retr.:</label>
            </td>
            <td class="Row1" style="width: 25%" runat="server" id="Td7">
                <asp:TextBox ID="txtELAnteArmQtaB_CC" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="10" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtELAnteArmQtaB_CC" ControlToValidate="txtELAnteArmQtaB_CC"
                    ErrorMessage="ControCodice Retr. Quota B: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Pensionabile:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaB_RetrPens" runat="server" CssClass="tb8 txtUppercase"
                    Width="50%" TabIndex="9" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQtaB_RetrPens"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaB_RetrPens" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile Quota B: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Pensionabile S.L 336:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaB_RetrPensSL336" runat="server" CssClass="tb8 txtUppercase"
                    Width="50%" TabIndex="9" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtELAnteArmQtaB_RetrPensSL336"
                    Display="Dynamic" ControlToValidate="txtELAnteArmQtaB_RetrPensSL336" Enabled="true"
                    ErrorMessage="Retribuzione Pensionabile S.L 336 Quota B: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabLegge407" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
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
    <table class="tabellaFormattazione grid grid-size-25" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox ID="txtELAnteArmQtaC_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV" ControlToValidate="txtELAnteArmQtaC_AA"
                    ErrorMessage="Servizio Utile Quota C: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabLegge407" />
            </td>
             <td class="Row1" style="width: 25%">
                
            </td>
             <td class="Row1" style="width: 25%">
             
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaLegge407" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva D.L.407" Width="180px" OnClick="SalvaLegge407_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabLegge407')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaLegge407" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina D.L.407" Width="180px" OnClick="EliminaLegge407_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare il D.L.407?')) return false; else BlockUI();" CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>
