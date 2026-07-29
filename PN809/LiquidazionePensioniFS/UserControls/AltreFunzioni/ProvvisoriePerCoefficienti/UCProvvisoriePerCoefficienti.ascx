<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCProvvisoriePerCoefficienti.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.ProvvisoriePerCoefficienti.UCProvvisoriePerCoefficienti" %>

<asp:Panel ID="panel" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" CssClass="form-container background-light-blue">

    <div class="single-line-container">
        <label class="input-label">Decorrenza Provvisoria Obbligatoria:</label>

        <div>
            <asp:TextBox runat="server" ID="txtDecorrenzaProvvisoriaObbligatoria" CssClass="txtUppercase tb8 date-picker dateMMaaaa" Text="mm/aaaa" MaxLength="7" Width="110px"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDecorrenzaProvvisoriaObbligatoria" ControlToValidate="txtDecorrenzaProvvisoriaObbligatoria"
                        ErrorMessage="Data Decorrenza Provvisoria Obbligatoria in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCProvvisoriePerCoefficienti" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaProvvisoriaObbligatoria" Display="Dynamic"
                    ErrorMessage="Data Decorrenza Provvisoria Obbligatoria: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCProvvisoriePerCoefficienti"
                    ID="customCheckDataDataSistema" ClientValidationFunction="checkCorrettezzaData" />  
        </div>

        <asp:Button runat="server" ID="btnApplica" OnClick="btnApplica_Click" OnClientClick="if(Page_ClientValidate('UCProvvisoriePerCoefficienti')){aspnetForm.target ='_self'; BlockUI();}"
                    Text="Applica" SkinID="btnAzione1" Width="80px" CausesValidation="false" CssClass="primary" />
    </div>
</asp:Panel>