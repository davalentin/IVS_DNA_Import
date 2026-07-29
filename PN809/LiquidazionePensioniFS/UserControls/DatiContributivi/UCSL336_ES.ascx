<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSL336_ES.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCSL336_ES" %>
<%--<div id="div1" style="border-style: none; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="true">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label5" runat="server" Text="Dati S.L. 336" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
      </table>
</div>--%>
<div id="divQuotaAQuotaB" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="true">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label2" runat="server" Text="Dati S.L. 336" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
      </table>
    <table class="tabellaFormattazione">
     <%--   <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label2" runat="server" Text="Dati S.L. 336" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>--%>
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label3" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    RMS:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaA" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtQuotaA"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Sett. Anz. Tot:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtSettAnzTot" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                    Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator8" runat="server" ControlToValidate="txtSettAnzTot"
                    Display="Dynamic" Enabled="true" ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="^[0-9]*$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Sett. Art. 24:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtSettArt24" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVSettArt24" runat="server"
                    ControlToValidate="txtSettArt24" Display="Dynamic" Enabled="true" ErrorMessage="Sett. Art. 24: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="^[0-9]*?$" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Sett. Art. 57:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtSettArt57" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVSettArt57" runat="server" ControlToValidate="txtSettArt57"
                    Display="Dynamic" Enabled="true" ErrorMessage="Sett. Art. 57: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="^[0-9]*?$" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label4" runat="server" Text="Quota B" Style="font-weight: bold"></asp:Label>
            </td>
           
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    RMS:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaB" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtQuotaB"
                    Display="Dynamic" Enabled="true" ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,6}(,\d{1,4})?$" />
            </td>
        </tr>
    </table>
</div>
<div runat="server" id="divDatiContributivi" style="border-style: solid; border-color: #000080;
    border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
    margin-top: 4px;">
    
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label1" runat="server" Text="Dati Contributivi" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contributi Totali:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtContributiTotali" CssClass="tb8 txtUppercase" MaxLength="9"
                    Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" Display="Dynamic"
                    ControlToValidate="txtContributiTotali" Enabled="true" ErrorMessage="Contributi Totali: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Contributi Art. 14:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtContributiArt14" runat="server" CssClass="tb8 txtUppercase" MaxLength="9"
                    Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                    ControlToValidate="txtContributiArt14" Enabled="true" ErrorMessage="Contributi Art. 14: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contributi AGO:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtContributiAGO" runat="server" CssClass="tb8 txtUppercase" MaxLength="9"
                    Width="60%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                    ControlToValidate="txtContributiAGO" Enabled="true" ErrorMessage="Contributi AGO: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Supplemento Fondo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtSupplementoFondo" runat="server" CssClass="tb8 txtUppercase" Width="60%"
                    MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" Display="Dynamic"
                    ControlToValidate="txtSupplementoFondo" Enabled="true" ErrorMessage="Supplemento Fondo: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCSL336_ES" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
            </td>
        </tr>
    </table>
</div>
<!-- Pannello bottoni -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;">
        <tr>
            <td style="text-align: right; vertical-align: bottom;">
                <asp:Button ID="btnSalva" runat="server" CausesValidation="false" ValidationGroup="UCSL336_ES"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalva_Click" Text="Salva Dati S.L. 336"
                    OnClientClick="if(Page_ClientValidate('UCSL336_ES')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnElimina" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati S.L. 336" Width="150px" OnClick="btnElimina_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati S.L. 336?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
