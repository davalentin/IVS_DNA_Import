<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMaternitaAcnaCi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCMaternitaAcnaCi" %>

<!-- Pannello Maternità -->
<asp:Panel ID="pnlMaternita" runat="server" Visible="true">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align:left">
                <asp:Label ID="lblTitoloMaternita" runat="server" Text="Maternità" style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width:25%">
                <asp:Label ID="lblImportoIVSMaternita" runat="server" Text="Importo IVS:"></asp:Label>
            </td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtImportoIVSMaternita" runat="server" Style="text-align: left" CssClass="tb8 txtUppercase" Width="130" TabIndex="1"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtImportoIVSMaternita"
                    Display="Dynamic" ControlToValidate="txtImportoIVSMaternita" Enabled="true" ErrorMessage="Importo IVS: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaternitaAcnaCI" ValidationExpression="\d+(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:25%">
                <asp:Label ID="lblSettimane31dic92Maternita" runat="server" Text="Settimane al 31/12/92:"></asp:Label>
            </td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtSettimane31dic92Maternita" runat="server" Style="text-align: left" CssClass="tb8 txtUppercase" Width="130" TabIndex="2"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimane31dic92Maternita" 
                    ControlToValidate="txtSettimane31dic92Maternita" Display="Dynamic" ErrorMessage="Numero Settimane al 31/12/92 non valido: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabMaternitaAcnaCI" />
            </td>
            <td class="Row1" style="text-align:right; width:20%">
                <asp:Label ID="lblSettimaneDL50392Maternita" runat="server" Text="Settimane D.L. 503/92:"></asp:Label>
            </td>
            <td style="width:35px"></td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtSettimaneDL50392Maternita" runat="server" CssClass="tb8 txtUppercase" Width="130" TabIndex="3"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimaneDL50392Maternita" 
                    ControlToValidate="txtSettimaneDL50392Maternita" Display="Dynamic" ErrorMessage="Numero Settimane D.L. 503/92 non valido: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabMaternitaAcnaCI" />
            </td>
            <td style="width:20px"></td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Maternità -->

<!-- Pannello Cengio -->
<asp:Panel ID="pnlCengio" runat="server" Visible="true">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align:left">
                <asp:Label ID="lblTitoloCengio" runat="server" Text="Cengio" style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width:25%">
                <asp:Label ID="lblImportoIVSCengio" runat="server" Text="Importo IVS:"></asp:Label>
            </td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtImportoIVSCengio" runat="server" Style="text-align: left" CssClass="tb8 txtUppercase" Width="130" TabIndex="4"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtImportoIVSCengio"
                    Display="Dynamic" ControlToValidate="txtImportoIVSCengio" Enabled="true" ErrorMessage="Importo IVS: Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabMaternitaAcnaCI" ValidationExpression="\d+(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width:25%">
                <asp:Label ID="lblSettimane31dic92Cengio" runat="server" Text="Settimane al 31/12/92:"></asp:Label>
            </td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtSettimane31dic92Cengio" runat="server" Style="text-align: left" CssClass="tb8 txtUppercase" Width="130" TabIndex="5"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtSettimane31dic92Cengio" 
                    ControlToValidate="txtSettimane31dic92Cengio" Display="Dynamic" ErrorMessage="Numero Settimane al 31/12/92 non valido: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabMaternitaAcnaCI" />
            </td>
            <td class="Row1" style="text-align:right; width:20%">
                <asp:Label ID="lblSettimaneDL50392Cengio" runat="server" Text="Settimane D.L. 503/92:"></asp:Label>
            </td>
            <td style="width:35px"></td>
            <td class="field"  style="width:25%">
                <asp:TextBox ID="txtSettimaneDL50392Cengio" runat="server" CssClass="tb8 txtUppercase" Width="130" TabIndex="6"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtlblSettimaneDL50392Cengio" 
                    ControlToValidate="txtSettimaneDL50392Cengio" Display="Dynamic" ErrorMessage="Numero Settimane D.L. 503/92 non valido: inserire il numero di settimane in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabMaternitaAcnaCI" />
            </td>
            <td style="width:20px"></td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Cengio -->

<div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaMaternitaAcna" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Maternità / Acna"
                    Width="160px" OnClick="btnSalvaMaternitaAcna_Click" OnClientClick="if(Page_ClientValidate('UCTabMaternitaAcnaCI')){aspnetForm.target ='_self'; BlockUI();}" 
                    CausesValidation="false" CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaMaternitaAcna" runat="server" Enabled="true" SkinID="btnAzione1" Text="Elimina Maternità / Acna"
                    Width="160px" OnClick="btnEliminaMaternitaAcna_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati di Maternità / Acna?')) return false; else BlockUI();" 
                    CausesValidation="false" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>