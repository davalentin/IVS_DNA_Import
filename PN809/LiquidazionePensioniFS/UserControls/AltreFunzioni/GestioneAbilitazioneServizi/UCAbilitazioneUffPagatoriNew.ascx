<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAbilitazioneUffPagatoriNew.ascx.cs" 
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneServizi.UCAbilitazioneUffPagatoriNew" %>
<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px; text-align:center">
            <asp:Panel ID="pnlAbilitazioneUffPagatoriNew" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 1px;
                background-position: right top; background-repeat: no-repeat;">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label>
                                Procedura da utilizzare:</label>
                            <asp:DropDownList runat="server" Width="220px" CssClass="tb8 txtUppercase" ID="ddlAbilitazioneUffPagatori">
                                <asp:ListItem Text="" Value="" />
                                <asp:ListItem Text="VECCHIO SERVIZIO" Value="NO" />
                                <asp:ListItem Text="NUOVO SERVIZIO" Value="SI" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldddlReport" runat="server" ErrorMessage="Procedura da utilizzare: campo obbligatorio"
                                Text="*" ControlToValidate="ddlAbilitazioneUffPagatori" ValidationGroup="UCAbilitazioneUffPagatori"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnApplica" runat="server" Text="Applica" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnApplica_Click" OnClientClick="if(Page_ClientValidate('UCAbilitazioneUffPagatori')){aspnetForm.target ='_self'; BlockUI()}" />
                        </td>
                    </tr>
                </table>

            </asp:Panel>
        </td>
    </tr>
</table>