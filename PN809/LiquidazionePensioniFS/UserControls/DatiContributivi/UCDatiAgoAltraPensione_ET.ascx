<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAgoAltraPensione_ET.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAgoAltraPensione_ET" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
    .pnlMain
    {
        border-style: solid;
        border-color: #000080;
        border-collapse: collapse;
        border-width: 1px;
        width: 710px;
        margin-left: 4px;
        margin-top: 4px;
    }
</style>
<script type="text/javascript">
   
</script>
<div class="pnlMain">
    <asp:Panel ID="pnlAltraPensione" runat="server">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left" colspan="4">
                    <asp:Label ID="lblAtraPensione" runat="server" Text="Altra Pensione" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Categoria:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <%--  <asp:TextBox ID="txtCategoria" runat="server" MaxLength="3" CssClass="tb8 txtUppercase" Width="30px"></asp:TextBox>--%>
                    <asp:DropDownList runat="server" ID="ddlCategoria" Width="70px" CssClass="txtUppercase tb8">
                        <asp:ListItem Selected="True" Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="IO" Value="IO"></asp:ListItem>
                        <asp:ListItem Text="IOA" Value="IOA"></asp:ListItem>
                        <asp:ListItem Text="IOC" Value="IOC"></asp:ListItem>
                        <asp:ListItem Text="IR" Value="IR"></asp:ListItem>
                        <asp:ListItem Text="IOS" Value="IOS"></asp:ListItem>
                        <asp:ListItem Text="IOM" Value="IOM"></asp:ListItem>
                        <asp:ListItem Text="IOP" Value="IOP"></asp:ListItem>
                        <asp:ListItem Text="IP" Value="IP"></asp:ListItem>
                        <asp:ListItem Text="VO" Value="VO"></asp:ListItem>
                        <asp:ListItem Text="VOA" Value="VOA"></asp:ListItem>
                        <asp:ListItem Text="VOC" Value="VOC"></asp:ListItem>
                        <asp:ListItem Text="VR" Value="VR"></asp:ListItem>
                        <asp:ListItem Text="VOS" Value="VOS"></asp:ListItem>
                        <asp:ListItem Text="VOM" Value="VOM"></asp:ListItem>
                        <asp:ListItem Text="VOP" Value="VOP"></asp:ListItem>
                        <asp:ListItem Text="VP" Value="VP"></asp:ListItem>
                        <asp:ListItem Text="SO" Value="SO"></asp:ListItem>
                        <asp:ListItem Text="SOA" Value="SOA"></asp:ListItem>
                        <asp:ListItem Text="SOC" Value="SOC"></asp:ListItem>
                        <asp:ListItem Text="SR" Value="SR"></asp:ListItem>
                        <asp:ListItem Text="SOS" Value="SOS"></asp:ListItem>
                        <asp:ListItem Text="SOP" Value="SOP"></asp:ListItem>
                        <asp:ListItem Text="SP" Value="SP"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlCategoria" ControlToValidate="ddlCategoria" Display="Dynamic" 
                    ErrorMessage="Categoria è un dato obbligatorio" ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Certificato:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtCertificato" runat="server" MaxLength="8" CssClass="tb8 txtUppercase" Width="80px"></asp:TextBox>
                     <asp:RequiredFieldValidator runat="server" ID="RFVtxtCertificato" ControlToValidate="txtCertificato" Display="Dynamic" 
                    ErrorMessage="Certificato è un dato obbligatorio" ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Base:
                    </label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox ID="txtBase" runat="server" MaxLength="11" CssClass="tb8 txtUppercase"
                        Width="110px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtBase" Display="Dynamic"
                        ControlToValidate="txtBase" Enabled="true" ErrorMessage="Base: Inserire valori interi o decimali (max 5 interi e 5 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(\,\d{1,5})?" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Liquidazione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox ID="txtTipoLiquidazione" runat="server" MaxLength="1" CssClass="tb8 txtUppercase"
                        Width="10px"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtTipoLiquidazione" ControlToValidate="txtTipoLiquidazione" Display="Dynamic" 
                    ErrorMessage="Tipo Liquidazione è un dato obbligatorio" ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenza" Width="70px"
                        Text="" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="1"
                        MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtDecorrenzaDatiAgo" ControlToValidate="txtDecorrenza"
                        ErrorMessage="Decorrenza in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza AGO: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ID="customCheckDataDecorrenzaAGO" ClientValidationFunction="checkCorrettezzaData" />
                     <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenza" ControlToValidate="txtDecorrenza" Display="Dynamic" 
                    ErrorMessage="Decorrenza è un dato obbligatorio" ValidationGroup="UCTabDatiAgoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        RMS/Imp:
                    </label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox ID="txtRmsImp" runat="server" MaxLength="10" CssClass="tb8 txtUppercase"
                        Width="100px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtRmsImp" Display="Dynamic"
                        ControlToValidate="txtRmsImp" Enabled="true" ErrorMessage="RMS/Imp: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Set. Anz. Tot:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtSetAnzTot" runat="server" MaxLength="3" CssClass="tb8 txtUppercase"
                        Width="30px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSetAnzTot" runat="server" ControlToValidate="txtSetAnzTot"
                        Display="Dynamic" Enabled="true" ErrorMessage="Set. Anz. Tot: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        % Rev.:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtRev" runat="server" MaxLength="3" CssClass="tb8 txtUppercase"
                        Width="30px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRev" runat="server" ControlToValidate="txtRev"
                        Display="Dynamic" Enabled="true" ErrorMessage="Set. Anz. Tot: Inserire valori interi"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="^[0-9]*$" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlSupplementi" runat="server">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="text-align: left" colspan="4">
                    <asp:Label ID="Label1" runat="server" Text="Primo Supplemento" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaPrimoSupp"
                        Text="" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="1"
                        MaxLength="7" Width="70px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtDecorrenzaPrimoSupp" ControlToValidate="txtDecorrenzaPrimoSupp"
                        ErrorMessage="Decorrenza Primo Supplemento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPrimoSupp" Display="Dynamic"
                        ErrorMessage="Decorrenza AGO: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ID="CustomValidator2" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Importo Contributivo:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtImportoContribPrimoSupp" runat="server" MaxLength="10" CssClass="tb8 txtUppercase"
                        Width="110px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoContribPrimoSupp"
                        Display="Dynamic" ControlToValidate="txtImportoContribPrimoSupp" Enabled="true"
                        ErrorMessage="Importo Primo Supplemento: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="text-align: left" colspan="4">
                    <asp:Label ID="Label2" runat="server" Text="Secondo Supplemento" Style="font-weight: bold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaSecondoSupp"
                        Width="70px" CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="1"
                        MaxLength="7"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtDecorrenzaSecondoSupp"
                        ErrorMessage="Decorrenza Primo Supplemento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAgoGAS" Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSecondoSupp"
                        Display="Dynamic" ErrorMessage="Decorrenza AGO: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS"
                        ID="CustomValidator3" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Importo Contributivo:
                    </label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtImportoContribSecondoSupp" runat="server" MaxLength="10" CssClass="tb8 txtUppercase"
                        Width="110px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoContribSecondoSupp"
                        Display="Dynamic" ControlToValidate="txtImportoContribSecondoSupp" Enabled="true"
                        ErrorMessage="Importo Secondo Supplemento: Inserire valori interi o decimali (max 5 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAgoGAS" ValidationExpression="\d{1,5}(\,\d{1,4})?" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</div>
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;">
        <tr>
            <td style="text-align: right; vertical-align: bottom;">
                <asp:Button ID="btnSalvaDatiAgoNoRiduzione" runat="server" CausesValidation="false"
                    ValidationGroup="UCTabDatiAgoGAS" SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiAgo_Click"
                    Text="Salva Dati Ago" Visible="true" OnClientClick="if(Page_ClientValidate('UCTabDatiAgoGAS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiAgo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Ago" Width="150px" OnClick="btnEliminaDatiAgo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Ago?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
