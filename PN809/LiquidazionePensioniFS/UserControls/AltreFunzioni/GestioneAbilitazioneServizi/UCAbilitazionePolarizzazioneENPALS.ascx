<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAbilitazionePolarizzazioneENPALS.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneServizi.UCAbilitazionePolarizzazioneENPALS" %>

<style>
    .content-table {
        border-collapse: separate;
        border-spacing: 0 8px;
    }
</style>

<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px; text-align: center" class="form-container background-light-blue">
            <asp:Panel ID="pnlAbilitazionePolarizzazioneENPALS" runat="server">
                <table class="tabellaFormattazione content-table" width="100%">
                    <tr>
                        <td colspan="2">
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 68%; text-align: left;">
                            <label>
                                Abilitazione nuova polarizzazione delle domande ENPALS:</label>
                        </td>
                        <td style="width: 32%; text-align: center;">
                            <asp:DropDownList runat="server" Width="220px" CssClass="tb8 txtUppercase" ID="ddlPolarizzazioneENPALS">
                                <asp:ListItem Text="" Value="" />
                                <asp:ListItem Text="Disabilitata" Value="NO" />
                                <asp:ListItem Text="Abilitata" Value="SI" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_ddlPolarizzazioneENPALS" runat="server" ErrorMessage="Abilitazione nuova polarizzazione delle domande: campo obbligatorio"
                                Text="*" CssClass="field-is-required" ControlToValidate="ddlPolarizzazioneENPALS" ValidationGroup="UCAbilitazionePolarizzazioneENPALS"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 68%; text-align: left;">
                            <label>
                                Abilitazione nuova polarizzazione delle domande ai Superstiti ENPALS:</label>
                        </td>
                        <td style="width: 32%; text-align: center;">
                            <asp:DropDownList runat="server" Width="220px" CssClass="tb8 txtUppercase" ID="ddlPolarizzazioneSuperstitiENPALS">
                                <asp:ListItem Text="" Value="" />
                                <asp:ListItem Text="Disabilitata" Value="NO" />
                                <asp:ListItem Text="Abilitata" Value="SI" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFV_ddlPolarizzazioneSuperstitiENPALS" runat="server"
                                ErrorMessage="Abilitazione nuova polarizzazione delle domande ai Superstiti: campo obbligatorio"
                                Text="*" CssClass="field-is-required" ControlToValidate="ddlPolarizzazioneSuperstitiENPALS" ValidationGroup="UCAbilitazionePolarizzazioneENPALS"></asp:RequiredFieldValidator>
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
                        <td align="end">
                            <asp:Button ID="btnApplica" runat="server" Text="Applica" SkinID="btnAzione1" CausesValidation="false" CssClass=" primary mr-0"
                                OnClick="btnApplica_Click" OnClientClick="if(Page_ClientValidate('UCAbilitazionePolarizzazioneENPALS')){aspnetForm.target ='_self'; BlockUI()}" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
