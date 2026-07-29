<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCCambioDataSistema.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioDataSistema.UCCambioDataSistema" %>

<asp:Panel ID="panel" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px; min-height: 150px" CssClass="form-container">
                <br />
                <br />
    <table style="width: 100%;" class="tabellaFormattazione">
        <tr>
            <td colspan="2">
                <div class="single-line-container">
                    <label class="input-label">Data del Sistema:</label>

                    <div style="display: inline">
                        <asp:TextBox runat="server" ID="txtDataSistema" CssClass="txtUppercase tb8 date-picker-base" Text="gg/mm/aaaa" MaxLength="10" Width="110px"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDataSistema" ControlToValidate="txtDataSistema"
                                    ErrorMessage="Data del Sistema in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDataSistema" Enabled="true" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataSistema" Display="Dynamic"
                                ErrorMessage="Data sistema: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDataSistema"
                                ID="customCheckDataDataSistema" ClientValidationFunction="checkCorrettezzaData" />  
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="end">
                <asp:Button runat="server" ID="btnRipristina" OnClick="btnRipristina_Click" OnClientClick="BlockUI();" Text="Ripristina" SkinID="btnAzione1" Width="80px" CausesValidation="false" />
                <asp:Button runat="server" ID="btnApplica" OnClick="btnApplica_Click" OnClientClick="if(Page_ClientValidate('UCDataSistema')){aspnetForm.target ='_self'; BlockUI();}"
                    Text="Applica" SkinID="btnAzione1" Width="80px" CausesValidation="false" CssClass="primary mr-0" />
            </td>
        </tr>
    </table>
</asp:Panel>