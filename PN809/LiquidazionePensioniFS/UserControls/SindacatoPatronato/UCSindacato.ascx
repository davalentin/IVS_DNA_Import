<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSindacato.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.SindacatoPatronato.UCSindacato" %>

<script type="text/javascript">
    function CleanFields() {
        document.getElementById("<%=txtDecorrenzaSindacato.ClientID %>").value = 'MM/AAAA';
        document.getElementById("<%=txtFineSindacato.ClientID %>").value = 'MM/AAAA';
        return false;
    }



</script>

<asp:Panel runat="server" ID="pnlSindacato">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1">
                <label>
                    Sindacato:</label>
            </td>
            <td class="Row1">
                <label>
                    UGL Pens</label>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza Sindacato:</label>
            </td>
            <td class="field" style=" width:25%">
                <asp:TextBox runat="server" ID="txtDecorrenzaSindacato" CssClass="tb8 txtUppercase date-picker-maxActual dateMMaaaa"
                    Width="95px" Text="MM/AAAA" TabIndex="1" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecSindacato" ControlToValidate="txtDecorrenzaSindacato"
                    Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Decorrenza Sindacato"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCSindacatoPatronato" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSindacato" Display="Dynamic"
                    ErrorMessage="Decorrenza Sindacato: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required" ValidationGroup="UCSindacatoPatronato"
                    ID="customDecSindacato" ClientValidationFunction="checkDataPostOdiernaMMAAAA" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSindacato" Display="Dynamic"
                    ErrorMessage="Decorrenza Sindacato: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCSindacatoPatronato"
                    ID="customCheckDataDecorrenzaSindacato" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
            <td class="Row1">
                <label>
                    Cessazione Sindacato:</label>
            </td>
            <td class="field" style=" width:25%">
                <asp:TextBox runat="server" ID="txtFineSindacato" CssClass="tb8 txtUppercase date-picker-maxActual dateMMaaaa"
                    Width="95px" Text="MM/AAAA" TabIndex="2" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateFineSindacato" ControlToValidate="txtFineSindacato"
                    Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Cessazione Sindacato"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCSindacatoPatronato" />
                <asp:CustomValidator runat="server" ControlToValidate="txtFineSindacato" Display="Dynamic"
                    ErrorMessage="Cessazione Sindacato: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required" ValidationGroup="UCSindacatoPatronato"
                    ID="customCessazioneSindacato" ClientValidationFunction="checkDataPostOdiernaMMAAAA" />
                <asp:CustomValidator runat="server" ControlToValidate="txtFineSindacato" Display="Dynamic"
                    ErrorMessage="Cessazione Sindacato: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCSindacatoPatronato"
                    ID="customCheckDataCessazioneSindacato" ClientValidationFunction="checkCorrettezzaData" />                     
            </td>
        </tr>
    </table>
    <div style="width: 720px; margin-top: 25px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields();"
                        Enabled="true" Text="Pulisci" Width="100px" />
                </td>

            </tr>
        </table>
    </div>
    
</asp:Panel>
