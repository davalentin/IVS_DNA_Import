<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPatronato.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.SindacatoPatronato.UCPatronato" %>

<script type="text/javascript">
    function CleanFields1() {
        document.getElementById("<%=txtCodiceUfficioZona.ClientID %>").value = '';
      
        return false;
    }
</script>

<asp:Panel runat="server" ID="pnlPatronato">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1">
                <label>
                    Patronato:</label>
            </td>
            <td class="Row1">
                <label>
                    Informa Famiglia</label>
            </td>
            <td class="Row1">
            </td>
            <td class="field">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                Codice ufficio di zona:
            </td>
            <td class="field">
                <asp:TextBox runat="server" ID="txtCodiceUfficioZona" CssClass="tb8 txtUppercase"
                    MaxLength="2" Width="95px" TabIndex="1"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateCodiceUfficioZona" ControlToValidate="txtCodiceUfficioZona"
                    Enabled="true" ErrorMessage="Inserire il codice nel formato valido per  Codice ufficio di zona"
                    Text="*" CssClass="field-is-required" ValidationExpression="^[a-zA-Z]+$" ValidationGroup="UCSindacatoPatronato" />
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <%--        <tr>
        <td class="Row1">
            <label>Tipo ufficio</label>
        </td>
        <td class="field">
        <asp:TextBox runat="server" ID="txtTipoUfficio" CssClass="tb8 txtUppercase"  Width="95px" ></asp:TextBox>
        </td>
        <td class="Row1">
        <label>Codice ufficio</label>
        </td>
        <td>
        <asp:TextBox runat="server" ID="txtCodiceUfficio" CssClass="tb8 txtUppercase"  Width="95px" >
        
        </asp:TextBox>
        </td>
        </tr>
        <tr>
        <td class="Row1">
        <label>Numero Pratica</label>
        </td>
        <td class="field">
        <asp:TextBox runat="server" ID="txtNumeroPratica" CssClass="tb8 txtUppercase"  Width="95px" ></asp:TextBox>
        </td>
        <td></td>
        <td></td>
        </tr>--%>
        
    </table>
    <div style="width: 720px; margin-top: 25px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields1();"
                        Enabled="true" Text="Pulisci" Width="100px" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
