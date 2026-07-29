<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDantePensioneDiretta.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa.UCDantePensioneDiretta" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
<script type="text/javascript">
    $(document).ready(function () {
        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_HiddenFieldSedi").value.split(';');
        $("#<%=txtSede.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });
    });
</script>
<asp:Panel runat="server" ID="pnlPensioneDiretta">
    <div runat="server" id="divCodiceFiscalePensioneDiretta">
        <br />
        <table class="tabellaFormattazione grid grid-col-6-dante-causa">
            <tr>
                <td style="width: 10%;" class="Row1">
                    <label>
                        Categoria:</label>
                </td>
                <td align="left" style="width: 28%;" class="field">
                    <asp:DropDownList runat="server" TabIndex="1" ID="ddlCategoriaPensione" CssClass="tb8 txtUppercase"
                        Width="100px" Enabled="false">
                    </asp:DropDownList>
                    <asp:CustomValidator EnableClientScript="true" runat="server" Display="Dynamic" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCPensioneDirettaDC" ID="ddlCategoriaPensione_CV" ClientValidationFunction="validateDropDownList"
                        ErrorMessage="Selezionare la categoria" />
                </td>
                <td style="width: 5%;" class="Row1">
                    <label>
                        Sede:</label>
                </td>
                <td align="left" style="width: 25%;" class="field">
                    <asp:TextBox runat="server" ID="txtSede" TabIndex="2" CssClass="tb8 txtUppercase"
                        Style="width: 85%;" MaxLength="4" Enabled="false"></asp:TextBox>
                    <asp:CustomValidator EnableClientScript="true" Display="Dynamic" runat="server" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCPensioneDirettaDC" ID="txtSede_CV" ClientValidationFunction="validateSedeCodeMandatory"
                        ErrorMessage="Selezionare la sede" />
                </td>
                <td style="width: 10%;" class="Row1">
                    <label>
                        Certificato:</label>
                </td>
                <td align="left" style="width: 22%;" class="field">
                    <asp:TextBox runat="server" ID="txtCertificato" TabIndex="3" CssClass="tb8 txtUppercase"
                        Width="100px" MaxLength="8" Enabled="false"></asp:TextBox>
                    <asp:CustomValidator EnableClientScript="true" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                        ValidationGroup="UCPensioneDirettaDC" ID="txtCertificato_CV" ClientValidationFunction="validateNumeroCertificato"
                        ErrorMessage="Numero certificato non valido" />
                </td>
            </tr>
        </table>
        <br />
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td style="width: 30%" class="Row1">
                    <label>
                        Decorrenza:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenza" Width="95px"
                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="4" MaxLength="7" Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ControlToValidate="txtDecorrenza"
                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCPensioneDirettaDC"
                        Text="*" CssClass="field-is-required" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtDecorrenza"
                        Enabled="true" ErrorMessage="Inserire la Decorrenza" Text="*" Display="Dynamic"
                        ValidationGroup="UCPensioneDirettaDC" CssClass="offClass  field-is-required onClassInvioPosizione" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCPensioneDirettaDC"
                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlEliminazione" Visible="false">
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Codice Eliminazione:</label>
                    </td>
                    <td class="field">
                        <asp:DropDownList Width="70%" runat="server" ID="ddlCodiceEliminazione" TabIndex="5"
                            CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Decorrenza Eliminazione:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaEliminazione"
                            Width="95px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="6"
                            Text="mm/aaaa" MaxLength="7"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator2" ControlToValidate="txtDecorrenzaEliminazione"
                            Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Eliminazione"
                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCPensioneDirettaDC"
                            Text="*" CssClass="field-is-required" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredValidatorDecorrenzaEliminazione"
                            ControlToValidate="txtDecorrenzaEliminazione" Enabled="true" ErrorMessage="Inserire la Decorrenza Eliminazione"
                            Text="*" Display="Dynamic" ValidationGroup="UCPensioneDirettaDC" CssClass="offClass field-is-required  onClassInvioPosizione" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaEliminazione"
                            Display="Dynamic" ErrorMessage="Decorrenza Eliminazione: data illogica" Text="*" CssClass="field-is-required"
                            ValidationGroup="UCPensioneDirettaDC" ID="customCheckDataDecorrenzaEliminazione"
                            ClientValidationFunction="checkCorrettezzaData" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Decorrenza Eliminazione Contabile:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaEliminazioneCont"
                            Width="95px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="7"
                            Text="mm/aaaa" MaxLength="7"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDecorrenzaEliminazioneCont"
                            Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Eliminazione Contabile"
                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCPensioneDirettaDC"
                            Text="*" CssClass="field-is-required" />
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtDecorrenzaEliminazioneCont"
                            Enabled="true" ErrorMessage="Inserire la Decorrenza Eliminazione Contabile" Text="*" 
                            Display="Dynamic" ValidationGroup="UCPensioneDirettaDC" CssClass="offClass field-is-required onClassInvioPosizione" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaEliminazioneCont"
                            Display="Dynamic" ErrorMessage="Decorrenza Eliminazione Contabile: data illogica"
                            Text="*" CssClass="field-is-required" ValidationGroup="UCPensioneDirettaDC" ID="customCheckDataDecorrenzaEliminazioneContabile"
                            ClientValidationFunction="checkCorrettezzaData" />
                    </td>
                </tr>
            </asp:Panel>
        </table>
        <div runat="server" id="divAgoCI" visible="true">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Codice Maggiorazione per 781 Contributi:</label>
                    </td>
                    <td class="field full-grid">
                        <asp:DropDownList runat="server" ID="ddlCodiceMaggiora" TabIndex="8" Width="70%"
                            CssClass="tb8 txtUppercase">
                            <asp:ListItem>1= si diritto (accertamento automatico)</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlNaturaPensione" Visible="false">
                    <tr>
                        <td style="width: 30%" class="Row1">
                            <label>
                                Natura Pensione:</label>
                        </td>
                        <td class="field cod-nat">
                            <asp:DropDownList runat="server" ID="ddlCodNatura1" Width="8%" TabIndex="9" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                            <span style="visibility: hidden">&nbsp;</span>
                            <asp:DropDownList runat="server" ID="ddlCodNatura2" Width="8%" TabIndex="10" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                            <span style="visibility: hidden">&nbsp;</span>
                            <asp:DropDownList runat="server" ID="ddlCodNatura3" Width="8%" TabIndex="11" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </div>
        <div runat="server" id="divAgo" visible="true">
            <table class="tabellaFormattazione grid grid-size-20">
                <asp:Panel runat="server" ID="pnlTipoPensione">
                    <tr>
                        <td style="width: 30%" class="Row1">
                            <label>
                                Tipo Pensione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" TabIndex="15" ID="ddlTipoPensione" Width="70%" CssClass="tb8 txtUppercase">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Codice Benefici di legge:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox runat="server" ID="txtCodiceBeneficiLegge" TabIndex="16" CssClass="txtUppercase tb8"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Importo pensione al 31.12.84:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox runat="server" ID="txtImportopensione84" TabIndex="17" CssClass="txtUppercase tb8"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegextxtImportopensione84" ControlToValidate="txtImportopensione84"
                            Display="Dynamic" ErrorMessage="Importo Pensione al 31.12.84: inserire l'importo in formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCPensioneDirettaDC" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Importo pensione al 1.1.85:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox runat="server" ID="txtImportopensione85" TabIndex="18" CssClass="txtUppercase tb8"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegextxtImportopensione85" ControlToValidate="txtImportopensione85"
                            Display="Dynamic" ErrorMessage="Importo pensione al 1.1.85: inserire l'importo in formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCPensioneDirettaDC" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Importo pensione al 1.1.90:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox runat="server" ID="txtImportopensione90" TabIndex="19" CssClass="txtUppercase tb8"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegextxtImportopensione90" ControlToValidate="txtImportopensione90"
                            Display="Dynamic" ErrorMessage="Importo pensione al 1.1.90: inserire l'importo in formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCPensioneDirettaDC" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Numero Contributi Diretta:</label>
                    </td>
                    <td class="field">
                        <asp:TextBox runat="server" ID="txtNumeroContributiDiretta" TabIndex="20" CssClass="txtUppercase tb8"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegextxtNumeroContributiDiretta"
                            ControlToValidate="txtNumeroContributiDiretta" Display="Dynamic" ErrorMessage="Numero Contributi Diretta: inserire l'importo in formato valido"
                            Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCPensioneDirettaDC" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Panel>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btSalvaPensioneDiretta" TabIndex="21" OnClick="btnSalvaPensioneDiretta_Click"
                        runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva Pensione Diretta"
                        Width="170px" CausesValidation="true" ValidationGroup="UCPensioneDirettaDC" OnClientClick="if(Page_ClientValidate('UCPensioneDirettaDC')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
            </tr>
        </table>
    </div>

