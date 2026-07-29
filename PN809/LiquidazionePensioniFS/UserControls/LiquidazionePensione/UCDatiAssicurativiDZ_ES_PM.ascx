<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiDZ_ES_PM.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiAssicurativiDZ_ES_PM" %>
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendariInizioFineAssicurazione();
    });

    function SetCalendariInizioFineAssicurazione() {
        if ($(document.getElementById("<%=pnlTxtPrimoVersamento.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                minDate: '-100y',
                maxDate: '0',
                yearRange: '-100:' + '+0:'
            });
        }
        if ($(document.getElementById("<%=pnlTxtUltimoVersamento.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                minDate: '-100y',
                maxDate: '0',
                yearRange: '-100:' + '+0:'
            });
        }
    }
</script>
<!-- Pannello Common Header -->
<asp:Panel runat="server" ID="pnlCommonHeader">
    <hr />
    <table class="tabellaContenuti">
        <tr>
            <td align="left">
                <asp:Label runat="server" ID="lblRecordFondo" Font-Bold="true">Dati Record fondo</asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size mb-32" style="width: 700px">
                    <asp:GridView runat="server" ID="gvRecordFondo" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvRecordFondo_RowCommand"
                        OnRowDataBound="gvRecordFondo_RowDataBound" OnRowCancelingEdit="gvRecordFondo_RowCancelingEdit"
                        OnRowEditing="gvRecordFondo_RowEditing" EnableViewState="true" OnRowUpdating="gvRecordFondo_RowUpdating"
                        OnPageIndexChanging="gvRecordFondo_onPageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice natura" HeaderStyle-CssClass="intestazioneTabella Row1 min-size-256 width-fixed-230"
                                ItemStyle-CssClass="TblRecordset3 min-size-256">
                                <ItemTemplate>
                                    <div class="full-width cod-nat">
                                        <asp:TextBox runat="server" ID="lblcodiceNatura1" Text='<%#Bind("_CodiceNatura1")%>'
                                            Enabled="false" Width="10px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
                                        <asp:TextBox runat="server" ID="lblCodiceNatura2" Text='<%#Bind("_CodiceNatura2")%>'
                                            Enabled="false" Width="10px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
                                        <asp:TextBox runat="server" ID="lblCodiceNatura3" Text='<%#Bind("_CodiceNatura3")%>'
                                            Enabled="false" Width="10px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
                                    </div>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <div class="full-width cod-nat">
                                        <asp:Label runat="server" ID="lblCodiceNatura" CssClass="txtUppercase none">      
                                        </asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlCodNatura1" Width="50px" CssClass="txtUppercase tb8 xxs width-33">
                                        </asp:DropDownList>
                                        <asp:DropDownList runat="server" ID="ddlCodNatura2" Width="50px" CssClass="txtUppercase tb8 xxs width-33">
                                        </asp:DropDownList>
                                        <asp:DropDownList runat="server" ID="ddlCodNatura3" Width="50px" CssClass="txtUppercase tb8 xxs width-33">
                                        </asp:DropDownList>
                                    </div>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice non calcolo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceNonCalcolo" Text='<%#Bind("_CodiceNonCalcolo")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodiceNonCalcolo" runat="server"
                                        Width="50px" Text=' <%# Bind("_CodiceNonCalcolo")%>'>
                                        <asp:ListItem Text=" " Value=" "></asp:ListItem>
                                        <asp:ListItem Text="SI" Value="S"></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="N"></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Text=' <%# Bind("strDecorrenzaValidita")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaRecordFondo"
                                        Width="100px" MaxLength="7" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                                        Text=' <%# Bind("strDecorrenzaValidita")%>'>
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDecorrenzaRecordFondo"
                                        ControlToValidate="txtDecorrenzaRecordFondo" ErrorMessage="Decorrenza record fondo in formato non valido"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRecordFondo" Enabled="true" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaRecordFondo"
                                        Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCRecordFondo"
                                        ID="customCheckDataDecorrenzaRecordFondo" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sospensione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSospensione" Text=' <%# Bind("strDataSospensione")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtSospensioneRecordFondo"
                                        Width="100px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7"
                                        Text=' <%# Bind("strDataSospensione")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtSospensioneRecordFondo"
                                        ControlToValidate="txtSospensioneRecordFondo" ErrorMessage="Sospensione record fondo in formato non valido"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRecordFondo" Enabled="true" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtSospensioneRecordFondo"
                                        Display="Dynamic" ErrorMessage="Sospensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCRecordFondo"
                                        ID="customCheckDataSospensioneRecordFondo" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <hr />
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tipo Pensione:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                <asp:HiddenField ID="hdnTipoPensione" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Pensione:</label>
            </td>
            <td class="field" colspan="3">
                <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiAssicurativi" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Primo Versamento:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtPrimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPrimoVersamento"
                        Enabled="true" ErrorMessage="Data primo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo versamento: Inserire la data del primo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtPrimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataPrimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Ultimo Versamento:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtUltimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtUltimoVersamento" ControlToValidate="txtUltimoVersamento"
                        Enabled="true" ErrorMessage="Data ultimo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" Display="Dynamic"
                        ErrorMessage="Ultimo versamento: Inserire la data dell'ultimo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtUltimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataUltimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Header -->
<asp:Panel ID="pnlAttEconomProfInd" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Attività Economica:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="120px"
                    CssClass="txtUppercase tb8 onClassDomanda" TabIndex="3" MaxLength="2" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator20" ControlToValidate="txtAttivitaEconomica"
                    ErrorMessage="Attivita Economica non valido" ValidationExpression="^[0-9]{3}$"
                    runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                    CssClass="offClass  field-is-required onClassDomanda" Enabled="false" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtAttivitaEconomica" ControlToValidate="txtAttivitaEconomica"
                    ErrorMessage="Attività Economica obbligatoria" ValidationGroup="UCTabDatiAssicurativi"
                    Display="Dynamic" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Professione Individuale:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale"
                    Width="120px" CssClass="txtUppercase tb8 onClassDomanda" TabIndex="4" MaxLength="3"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator27" ControlToValidate="txtProfessioneIndividuale"
                    ErrorMessage="Professione Individuale non valido" ValidationExpression="^[0-9]{3}$"
                    runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                    CssClass="offClass field-is-required  onClassDomanda" Enabled="false" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtProfessioneIndividuale" ControlToValidate="txtProfessioneIndividuale"
                    ErrorMessage="Professione Individuale obbligatoria" ValidationGroup="UCTabDatiAssicurativi"
                    Display="Dynamic" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Pannello DZ -->
<asp:Panel ID="pnlDZ" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Cessazione Servizio:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataCessazioneServizio"
                    Width="50%" Text="" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDataCessazioneServizio" ControlToValidate="txtDataCessazioneServizio"
                    Enabled="true" ErrorMessage="Data Cessazione Servizio in formato non valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataCessazioneServizio"
                    Display="Dynamic" ErrorMessage="Data Cessazione Servizio: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiAssicurativiFS" ID="customCheckDataDataCessazioneServizio"
                    ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti:</label>
            </td>
            <td class="Row1 fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtRiscattiAA_DZ" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                <label>
                    AA</label>
                <asp:RegularExpressionValidator ID="REVtxtRiscattiAA_DZ" ControlToValidate="txtRiscattiAA_DZ"
                    ErrorMessage="Riscatti: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:TextBox ID="txtRiscattiMM_DZ" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                <label>
                    MM</label>
                <asp:RegularExpressionValidator ID="REVtxtRiscattiMM_DZ" ControlToValidate="txtRiscattiMM_DZ"
                    ErrorMessage="Riscatti: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Maggiorazione Pensione privilegiata:</label>
            </td>
            <td class="Row1 fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtMaggiorazionePensionePrivilegiata_AA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                <label>
                    AA</label>
                <asp:RegularExpressionValidator ID="REVtxtMaggiorazionePensionePrivilegiata_AA" ControlToValidate="txtMaggiorazionePensionePrivilegiata_AA"
                    ErrorMessage="Maggiorazione Pensione privilegiata: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:TextBox ID="txtMaggiorazionePensionePrivilegiata_MM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" MaxLength="2"></asp:TextBox>
                <label>
                    MM</label>
                <asp:RegularExpressionValidator ID="REVtxtMaggiorazionePensionePrivilegiata_MM" ControlToValidate="txtMaggiorazionePensionePrivilegiata_MM"
                    ErrorMessage="Maggiorazione Pensione privilegiata: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice benefici:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:DropDownList ID="ddlCodiceBenefici" runat="server" CssClass="tb8 txtUppercase"
                    Width="40px" MaxLength="2">
                    <asp:ListItem Selected="True" Text="0" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                    <asp:ListItem Text="3" Value="3"></asp:ListItem>
                    <asp:ListItem Text="4" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cod. Diritto Quote Fisse:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodDirittoQuoteFisse_DZ" Width="90%" CssClass="txtUppercase tb8">
                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1 - Non spettano" Value="1"></asp:ListItem>
                    <asp:ListItem Text="2 - Spettano" Value="2"></asp:ListItem>
                    <asp:ListItem Text="3 - Non spettano" Value="3"></asp:ListItem>
                    <asp:ListItem Text="4 - Spettano" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Caro Pane:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCaroPane" Width="90%" CssClass="txtUppercase tb8">
                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1 - Spetta" Value="1"></asp:ListItem>
                    <asp:ListItem Text="2 - Non spetta" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    DZ - ES:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodiceDz" Width="50px" CssClass="txtUppercase tb8 xxs">
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Classe Ante 50:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtClasseAnte50" Width="30%" CssClass="txtUppercase tb8"
                    MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtClasseAnte50" ControlToValidate="txtClasseAnte50"
                    Enabled="true" ValidationExpression="^[0-9]+$" ErrorMessage="Classe Ante 50: formato non corretto"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Ditta:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtDitta" CssClass="txtUppercase tb8" Width="70%"
                    MaxLength="4"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDitta" ControlToValidate="txtDitta" Enabled="true"
                    ValidationExpression="^[0-9]+$" ErrorMessage="Ditta: formato non corretto" runat="server"
                    Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    % Liquidazione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:Label runat="server" ID="lblPercentualeLiquidazionePensione" Text="0"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Raggiunto requisiti al 31/12/1997:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlRaggiuntoRequisiti311297" Width="50px" CssClass="txtUppercase tb8 xxs">
                    <asp:ListItem Text=" " Value=""></asp:ListItem>
                    <asp:ListItem Selected="True" Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <span style="visibility: hidden">&nbsp;</span>
            </td>
            <td class="field" style="width: 25%">
                <span style="visibility: hidden">&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="shift-full-grid">
                <div id="pdivDatiPerEsodo" style="border-style: solid; border-color: #000080; border-collapse: collapse;
                    border-width: 1px; width: 100%; margin-left: 0px" runat="server">
                    <table cellpadding="3" cellspacing="1" border="0" width="100%" class="tabellaFormattazione grid grid-size-20">
                        <tr>
                            <td class="Row1 shift-full-grid" colspan="4">
                                <label style="font-style: italic" class="section-label mt-32">Dati per Esodo</label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Codice:</label>
                            </td>
                            <td class="field" style="width: 25%">
                                <asp:DropDownList runat="server" ID="ddlCodiceEsodo" Width="50px" CssClass="tb8 txtUppercase xxs">
                                    <asp:ListItem Selected="True" Text="NO" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="SI" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="width: 25%">
                            </td>
                            <td style="width: 25%">
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Maggiore Anzianità:</label>
                            </td>
                            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                                <asp:TextBox ID="txtMaggiorazioneAnzianitaEsodo_AA" runat="server" CssClass="tb8 txtUppercase"
                                    Width="30px" MaxLength="2"></asp:TextBox>
                                <label>
                                    AA</label>
                                <asp:RegularExpressionValidator ID="REVtxtMaggiorazioneAnzianitaEsodo_AA" ControlToValidate="txtMaggiorazioneAnzianitaEsodo_AA"
                                    ErrorMessage="Maggiorazione Anzianità: formato Anno non valido" ValidationExpression="^[0-9]+$"
                                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                                <asp:TextBox ID="txtMaggiorazioneAnzianitaEsodo_MM" runat="server" CssClass="tb8 txtUppercase"
                                    Width="30px" MaxLength="2"></asp:TextBox>
                                <label>
                                    MM</label>
                                <asp:RegularExpressionValidator ID="REVtxtMaggiorazioneAnzianitaEsodo_MM" ControlToValidate="txtMaggiorazioneAnzianitaEsodo_MM"
                                    ErrorMessage="Maggiorazione Anzianità: formato Mese non valido" ValidationExpression="^[0-9]+$"
                                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                            </td>
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Retribuzione senza Esodo:</label>
                            </td>
                            <td class="field" style="width: 25%">
                                <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzioneAlNettoBeneficiEsodo"
                                    Width="70%" MaxLength="11" CssClass="txtUppercase tb8" Text=""></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REVtxtRetribuzioneAlNettoBeneficiEsodo"
                                    ControlToValidate="txtRetribuzioneAlNettoBeneficiEsodo" Display="Dynamic" ErrorMessage="Retribuzione senza Esodo: formato non valido "
                                    Text="*" CssClass="field-is-required" ValidationExpression="^\d{1,6}(,\d{1,4})?$" ValidationGroup="UCTabDatiAssicurativiFS" />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello DZ -->
<!-- Pannello ES -->
<asp:Panel runat="server" ID="pnlES" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Attività Svolta:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlAttivitaSvolta" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="5">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAttivitaSvolta_RF" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Attività svolta: Si prega di inserire l'Attività Svolta"
                    ControlToValidate="ddlAttivitaSvolta" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Convenzioni Internazionali</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtConvenzioneInternazionale" Width="70%" CssClass="txtUppercase tb8"
                    MaxLength="1"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtConvenzioneInternazionale"
                    ControlToValidate="txtConvenzioneInternazionale" Enabled="true" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Convenzioni Internazionali: valori ammessi da A a Q" ValidationExpression="[a-qA-Q]$"
                    ValidationGroup="UCTabDatiAssicurativiFS"></asp:RegularExpressionValidator>
            </td>
            <td class="Row1" style="width: 25%">
            </td>
            <td class="field" style="width: 25%">
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cod. Diritto Quote Fisse:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodDirittoQuoteFisse_ES" Width="90%" CssClass="txtUppercase tb8">
                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1 - Quota non spettante" Value="1"></asp:ListItem>
                    <asp:ListItem Text="2 - Quota spettante" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti:</label>
            </td>
            <td class="field fileds-date-input fileds-date-input--col-2" style="width: 25%">
                <asp:TextBox ID="txtRiscattiAA_ES" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                <label>
                    AA</label>
                <asp:RegularExpressionValidator ID="REVtxtRiscattiAA_ES" ControlToValidate="txtRiscattiAA_ES"
                    ErrorMessage="Riscatti: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:TextBox ID="txtRiscattiMM_ES" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
                <label>
                    MM</label>
                <asp:RegularExpressionValidator ID="REVtxtRiscattiMM_ES" ControlToValidate="txtRiscattiMM_ES"
                    ErrorMessage="Riscatti: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
            </td>
            <td class="field" style="width: 25%">
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello ES -->
<!-- Pannello PM -->
<asp:Panel ID="pnlPM" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Attività Svolta:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlAttivitaSvolta_1PM" Width="45%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="ddlAttivitaSvolta_2PM" Width="44%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tipo Liquidazione:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlTipoLiquidazionePM" Width="90%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
            </td>
        </tr>
         <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cod. Diritto Quote Fisse:</label>
            </td>
              <td class="field" colspan="3">
                   <asp:TextBox ID="txtCodiceDirittoQuoteFisse" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                    MaxLength="2"></asp:TextBox>
            </td>
             </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlAnnoUtileUltimoDecennio" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Anno utile ult. decennio:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlAnnoUtileUltimoDecennio" CssClass="txtUppercase tb8 xxs"
                        Width="30%">
                        <asp:ListItem Text="NO" Value="NO" />
                        <asp:ListItem Text="SI" Value="SI" />
                    </asp:DropDownList>
                </td>
            </asp:Panel>
            <td class="Row1" style="width: 25%">
                <label>
                    Legge 413:</label>
            </td>
            <td class="field" style="width: 25%; text-align: left;">
                <asp:Label runat="server" ID="lblCL413"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello PM -->
<!-- Pannello Common Footer -->
<asp:Panel ID="pnlCommonFooter" runat="server">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Specifico:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodiceSpecifico" CssClass="txtUppercase tb8"
                    TabIndex="19" Width="90%">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlCodiceSpecifico_RF" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Codice Specifico: Si prega di inserire il codice specifico"
                    ControlToValidate="ddlCodiceSpecifico" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Requisiti 1:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodRequisiti1" Width="90%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
                <%--<asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="28" ID="txtCodiceRequisiti2" Width="5%" MaxLength="1" Text="0" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateCodiceRequisiti2" ControlToValidate="txtCodiceRequisiti2"
                    ErrorMessage="Codice requisiti in formato non valido" ValidationExpression="^[a-zA-Z0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />--%>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Requisiti 2:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodRequisiti2" Width="90%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Footer -->
<div style="width: 100%; margin-top: 25px; margin-right: 40px;">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiAssicurativi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Assicurativi" Width="180px" OnClick="SalvaDatiAssicurativi_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativiFS')){aspnetForm.target ='_self'; BlockUI();}" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiAssicurativi" SkinID="btnAzione1" runat="server" Width="180px"
                    Text="Elimina Dati Assicurativi" CausesValidation="False" OnClick="btnEliminaDatiAssicurativi_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Assicurativi?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
<asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
