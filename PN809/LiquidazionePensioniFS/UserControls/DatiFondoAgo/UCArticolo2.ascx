<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCArticolo2.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiFondoAgo.UCArticolo2" %>
<asp:Panel runat="server" ID="pnlArticolo2">
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
    <table class="tabellaFormattazione" cellpadding="3" cellspacing="1" border="0" width="100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Scadenza benefici:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtScadenzaBenefici" Width="50%" CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA" MaxLength="10">
                </asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtScadenzaBenefici" ControlToValidate="txtScadenzaBenefici"
                        ErrorMessage="Scadenza benefici in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabArticolo2"
                        Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtScadenzaBenefici" Display="Dynamic"
                    ErrorMessage="Scadenza benefici: data illogica" Text="*" ValidationGroup="UCTabArticolo2"
                    ID="customCheckDatatxtScadenzaBenefici" ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    PAL con benefici:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtPALConBenefici" Width="70%" CssClass="tb8 txtUppercase"
                    MaxLength="11">
                </asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtPALConBenefici" Display="Dynamic"
                    ControlToValidate="txtPALConBenefici" Enabled="true" ErrorMessage="PAL con benefici: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" ValidationGroup="UCTabArticolo2" ValidationExpression="\d{0,6}(,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Scadenza illimitata:
                </label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:CheckBox runat="server" CssClass="tb8 offClass onClassExCombattente" ID="chkScadenzaIllimitata" />
            </td>
        </tr>
    </table>
    <div style="width: 720px; margin-top: 100px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnSalvaArticolo2" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva" Width="150px" OnClick="btnSalvaArticolo2_Click" OnClientClick="if(Page_ClientValidate('UCTabArticolo2')){aspnetForm.target ='_self'; BlockUI();}" />
                    <asp:Button ID="btnEliminaArticolo2" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina" Width="150px" OnClick="btnEliminaArticolo2_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Art.2 Comma 12 L.335?')) return false; else BlockUI();" />
                    <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                        OnClientClick="BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
