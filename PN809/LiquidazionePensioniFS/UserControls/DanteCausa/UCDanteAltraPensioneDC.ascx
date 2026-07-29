<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDanteAltraPensioneDC.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa.UCDanteAltraPensioneDC" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />

<asp:Panel runat="server" ID="pnlPensioneDC"><br />
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td style="width:12%" class="Row1">
                <label>Categoria:</label>
            </td>
            <td align="left" class="field">
                <asp:DropDownList runat="server" ID="ddlCategoriaPensione" TabIndex="1" CssClass="tb8 txtUppercase" Width="100px"/>
            </td>
            <td style="width:13%" class="Row1">
                <label>Ente:</label>
            </td>
            <td align="left" class="field">
                    <asp:TextBox runat="server" ID="txtEnte" TabIndex="2" CssClass="tb8 txtUppercase" Width="25px" MaxLength="1"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="txtEnte_REV" runat="server" ValidationExpression="^[0-9]$" ControlToValidate="txtEnte" Display="Dynamic"
                        Enabled="true" ErrorMessage="Ente: è possibile inserire un solo carattere numerico" ValidationGroup="UCAltrePensioniDC" Text="*" CssClass="field-is-required" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>Codice U/C:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" TabIndex="3" ID="ddlCodiceUC" Width="50px" CssClass="tb8 txtUppercase"/>
            </td>
            <td style="width:13%" class="Row1">
                <label>Decorrenza:</label>
            </td>
            <td>
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenza" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="4" Text="mm/aaaa" MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecorrenzaArretrati" ControlToValidate="txtDecorrenza"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Decorrenza"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCAltrePensioniDC" Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                    ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCAltrePensioniDC"
                    ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />  
            </td> 
        </tr>
        <tr>
            <td class="Row1">
                <label>Cessazione:</label>
            </td>
            <td colspan="4">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCessazione" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="5" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtCessazione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Cessazione"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCAltrePensioniDC"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                    ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCAltrePensioniDC"
                    ID="customCheckDataCessazione" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlNaturaPensione" Visible="false">
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Natura Pensione:</label>
                    </td>
                    <td class="field full-grid cod-nat" colspan="3">
                        <asp:DropDownList runat="server" ID="ddlCodNatura1" Width="12%" TabIndex="9" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:DropDownList runat="server" ID="ddlCodNatura2" Width="12%" TabIndex="10" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                        <span style="visibility: hidden">&nbsp;</span>
                        <asp:DropDownList runat="server" ID="ddlCodNatura3" Width="12%" TabIndex="11" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                    </td>
                </tr>
                </asp:Panel>
        <tr>
            <td style="width:17%" class="Row1">
                <label>Codice Importo:</label>
            </td>
            <td colspan="4" class="field">
                <asp:DropDownList runat="server" ID="ddlCodiceImporto" TabIndex="6" Width="67%" CssClass="tb8 txtUppercase md"/>
            </td>                 
        </tr>      
    </table><br /><br />
    <table width="100%" class="tab-actions-group">
        <tr>
            <td align="center" class="tab-actions-group__first">
                <%--<asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" Enabled="true" Text="Pulisci"
                    Width="100px" CausesValidation="true" ValidationGroup="" />--%>
                    
                    <asp:Button TabIndex="7" ID="btSalvaAltraPensioneDC" runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva Altra Pensione DC"
                        Width="180px" CausesValidation="true" OnClientClick="if(Page_ClientValidate('UCAltrePensioniDC')){aspnetForm.target ='_self'; BlockUI();}" 
                    onclick="btSalvaAltraPensioneDC_Click" CssClass="primary"/>
            </td>
        </tr>
    </table>   
</asp:Panel>
