<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiPI_GAS_CL.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiAssicurativiPI_GAS_CL" %>
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
            //$(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtPrimoVersamento.ClientID%>")).mask("99/99/9999");
        }
        if ((document.getElementById("<%=tipoFondo.ClientID%>")).value != "PI") {
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
                //$(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).unmask();
                //$(document.getElementById("<%=txtUltimoVersamento.ClientID%>")).mask("99/99/9999");
            }
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
                        OnPageIndexChanging="gvRecordFondo_onPageIndexChanging"
                        OnDataBound="gvRecordFondo_DataBound" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Codice natura" HeaderStyle-CssClass="intestazioneTabella Row1 width-fixed-230"
                                ItemStyle-CssClass="TblRecordset3">
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
                                    <asp:Label runat="server" ID="lblCodiceNonCalcolo" Text='<%# Bind("strCodiceNoCalcolo") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlCodiceNonCalcolo" runat="server"
                                        TabIndex="4" Width="70px" Text=' <%# Bind("_CodiceNonCalcolo")%>'>
                                        <asp:ListItem Text=" " Value=" "></asp:ListItem>
                                        <asp:ListItem Text="1 - SI" Value="S"></asp:ListItem>
                                        <asp:ListItem Text="0 - NO" Value="N"></asp:ListItem>
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
                                        TabIndex="5" Text=' <%# Bind("strDecorrenzaValidita")%>'>
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
                                        Width="100px" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="6"
                                        MaxLength="7" Text=' <%# Bind("strDataSospensione")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtSospensioneRecordFondo"
                                        ControlToValidate="txtSospensioneRecordFondo" ErrorMessage="Sospensione record fondo in formato non valido"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRecordFondo" Enabled="true" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtSospensioneRecordFondo"
                                        Display="Dynamic" ErrorMessage="Sospensione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCRecordFondo"
                                        ID="customCheckDataSospensione" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
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
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tipo Pensione:</label>
            </td>
            <td class="fiel full-gridd" colspan="3">
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
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1" MaxLength="10"></asp:TextBox>
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
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA date-picker-base-maxActual" TabIndex="2"
                        MaxLength="10"></asp:TextBox>
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
        <asp:Panel runat="server" ID="pnlAttivitaSvolta">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Attività Svolta:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlAttivitaSvolta" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="3">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlAttivitaSvolta_RF" Display="Dynamic"
                        Text="*" CssClass="field-is-required" ErrorMessage="Attività svolta: Si prega di inserire l'Attività Svolta"
                        ControlToValidate="ddlAttivitaSvolta" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Header -->
<asp:Panel ID="pnlAttEconomProfInd" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25"">
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
                    CssClass="offClass  field-is-required onClassDomanda" Enabled="false" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtProfessioneIndividuale" ControlToValidate="txtProfessioneIndividuale"
                    ErrorMessage="Professione Individuale obbligatoria" ValidationGroup="UCTabDatiAssicurativi"
                    Display="Dynamic" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Pannello GAS -->
<asp:Panel ID="pnlGAS" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25">
        <asp:Panel ID="pnlCodDirittoQuoteFisse" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Cod. Diritto Quote Fisse:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodDirittoQuoteFisse" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="14">
                        <asp:ListItem Text="1 - Non spettano" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2 - Spettano" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3 - Non spettano" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4 - Spettano" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Con. Int. / P.T.:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlConvenzione" Width="90%" CssClass="txtUppercase tb8">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Anz. al 5/46:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtMesiAnte46" Width="50%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtMesiAnte46" ControlToValidate="txtMesiAnte46"
                    Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="Anz. al 5/46: è possibile inserire solo numeri (max. 3 cifre)"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Anz. dal 5/46:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtAnzianitaUtileDal46" Width="50%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtAnzianitaUtileDal46" ControlToValidate="txtAnzianitaUtileDal46"
                    Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="Anz. dal 5/46: è possibile inserire solo numeri (max. 3 cifre)"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti utili:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtMesiUtiliIndennitaAggiuntiva" Width="50%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtMesiUtiliIndennitaAggiuntiva" ControlToValidate="txtMesiUtiliIndennitaAggiuntiva"
                    Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="Riscatti utili: è possibile inserire solo numeri"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti non utili:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtMesiNonUtiliIndennitaAggiuntiva" Width="50%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtMesiNonUtiliIndennitaAggiuntiva" ControlToValidate="txtMesiNonUtiliIndennitaAggiuntiva"
                    Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="Riscatti non utili: è possibile inserire solo numeri"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <asp:Panel ID="pnlIndennitaAggiuntiva" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Serv. Utile Indenn. Agg.:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        TabIndex="4" ID="txtServizioUtileIndennitaAggiuntiva" Width="50%" MaxLength="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtServizioUtileIndennitaAggiuntiva" ControlToValidate="txtServizioUtileIndennitaAggiuntiva"
                        Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="Serv. Utile Indenn. Agg.: è possibile inserire solo numeri"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Retr. Indenn. Agg.:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        TabIndex="4" ID="txtRetribuzione" Width="50%" MaxLength="9"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtRetribuzione" ControlToValidate="txtRetribuzione"
                        Enabled="true" ValidationExpression="\d{1,4}(,\d{1,4})?$" ErrorMessage="Retr. Indenn. Agg.: è possibile inserire solo numeri (4 interi e 4 decimali)"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Ditta:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtDitta" Width="50%" MaxLength="4"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDitta" ControlToValidate="txtDitta" Enabled="true"
                    ValidationExpression="^[a-zA-Z0-9]+$" ErrorMessage="Ditta: formato non corretto"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    % Riduzione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtPercentualeRiduzione" Width="50%" MaxLength="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtPercentualeRiduzione" ControlToValidate="txtPercentualeRiduzione"
                    Enabled="true" ValidationExpression="^[0-9]*$" ErrorMessage="% Riduzione: è possibile inserire solo numeri"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cod. Dimis.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlCodiceDimissioni" runat="server" Width="30%" CssClass="txtUppercase tb8 xxs">
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Pensione Rid.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlCodicePensioneRidotta" runat="server" Width="30%" CssClass="txtUppercase tb8 xxs">
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Conguagli a 01/1975:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="4" ID="txtConguaglio" Width="50%" MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtConguaglio" ControlToValidate="txtConguaglio"
                    Enabled="true" ValidationExpression="\d{1,4}(,\d{1,4})?$" ErrorMessage="% Riduzione: è possibile inserire solo numeri"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello GAS -->
<!-- Pannello PI Ex-Dipendenti -->
<asp:Panel runat="server" ID="pnlPI" Visible="false">
    <asp:Panel runat="server" ID="pnlPICommon" Visible="false">
        <table class="tabellaFormattazione grid grid-size-25">
          
            <asp:Panel runat="server" ID="pnlServizioUtile">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Servizio Utile:</label>
                    </td>
                    <td class="Row1 full-grid fileds-date-input" colspan="3">
                        <asp:TextBox ID="txtServizioUtileAA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="11" MaxLength="2"></asp:TextBox>
                        <label>
                            AA</label>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" Display="Dynamic"
                            Text="*" CssClass="field-is-required" ErrorMessage="Servizio Utile AA: campo obbligatorio" ControlToValidate="txtServizioUtileAA"
                            ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator9" ControlToValidate="txtServizioUtileAA"
                            ErrorMessage="Servizio Utile AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                        <asp:TextBox ID="txtServizioUtileMM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="12" MaxLength="2"></asp:TextBox>
                        <label>
                            MM</label>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" Display="Dynamic"
                            Text="*" CssClass="field-is-required" ErrorMessage="Servizio Utile MM: campo obbligatorio" ControlToValidate="txtServizioUtileMM"
                            ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator10" ControlToValidate="txtServizioUtileMM"
                            ErrorMessage="Servizio Utile MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                        <asp:TextBox ID="txtServizioUtileGG" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                            TabIndex="13" MaxLength="2"></asp:TextBox>
                        <label>
                            GG</label>
                        <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" Display="Dynamic"
                            Text="*" CssClass="field-is-required" ErrorMessage="Servizio Utile GG: campo obbligatorio" ControlToValidate="txtServizioUtileGG"
                            ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtServizioUtileGG"
                            ErrorMessage="Servizio Utile GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                    </td>
                </tr>
            </asp:Panel>
          
        </table>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlPICatU" Visible="false">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Livello:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList ID="ddlLivello" runat="server" Width="30%" CssClass="txtUppercase tb8"
                        TabIndex="6">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                        <asp:ListItem Text="7" Value="7"></asp:ListItem>
                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                        <asp:ListItem Text="9" Value="9"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane maggiorazione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        ID="txtSettimaneMaggiorazione" Width="61%" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSettimaneMaggiorazione" ControlToValidate="txtSettimaneMaggiorazione"
                        ErrorMessage="Settimane maggiorazione: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane esclusive:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        ID="txtSettimaneEsclusive" Width="61%" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSettimaneEsclusive" ControlToValidate="txtSettimaneEsclusive"
                        ErrorMessage="Settimane esclusive: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane INPDAI:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        ID="txtSettimaneINPDAI" Width="61%" MaxLength="4"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVtxtSettimaneINPDAI" ControlToValidate="txtSettimaneINPDAI"
                        ErrorMessage="Settimane INPDAI: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlPICatV" Visible="false">
        <table class="tabellaFormattazione grid grid-size-25">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Servizio Non Utile:</label>
                </td>
                <td class="Row1 full-grid fileds-date-input" colspan="3">
                    <asp:TextBox ID="txtServizioNonUtileAA" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                        TabIndex="11" MaxLength="2"></asp:TextBox>
                    <label>
                        AA</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioNonUtileAA" ControlToValidate="txtServizioNonUtileAA"
                        ErrorMessage="Servizio Non Utile AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                    <asp:TextBox ID="txtServizioNonUtileMM" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                        TabIndex="12" MaxLength="2"></asp:TextBox>
                    <label>
                        MM</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioNonUtileMM" ControlToValidate="txtServizioNonUtileMM"
                        ErrorMessage="Servizio Non Utile MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                    <asp:TextBox ID="txtServizioNonUtileGG" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                        TabIndex="13" MaxLength="2"></asp:TextBox>
                    <label>
                        GG</label>
                    <asp:RegularExpressionValidator ID="REVtxtServizioNonUtileGG" ControlToValidate="txtServizioNonUtileGG"
                        ErrorMessage="Servizio Non Utile GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<!-- Fine Pannello PI Ex-Dipendenti -->
<!-- Pannello CL -->
<asp:Panel runat="server" ID="pnlCL" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input" style="width: 25%">
                <asp:TextBox ID="txtServizioUtileAA_CL" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <label>
                    AA</label>
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAA_CL" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Servizio Utile AA: campo obbligatorio" ControlToValidate="txtServizioUtileAA_CL"
                    ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                <asp:RegularExpressionValidator ID="REVtxtServizioUtileAA_CL" ControlToValidate="txtServizioUtileAA_CL"
                    ErrorMessage="Servizio Utile AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:TextBox ID="txtServizioUtileMM_CL" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="12" MaxLength="2"></asp:TextBox>
                <label>
                    MM</label>
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileMM_CL" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Servizio Utile MM: campo obbligatorio" ControlToValidate="txtServizioUtileMM_CL"
                    ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                <asp:RegularExpressionValidator ID="REVtxtServizioUtileMM_CL" ControlToValidate="txtServizioUtileMM_CL"
                    ErrorMessage="Servizio Utile MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Importo altra pensione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtImportoAltraPensione" runat="server" CssClass="tb8 txtUppercase"
                    Width="90px" TabIndex="11" MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtImportoAltraPensione" ControlToValidate="txtImportoAltraPensione"
                    ErrorMessage="Importo altra pensione: formato non valido" ValidationExpression="^\d{1,4}(,\d{1,4})?$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Anni differimento:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtAnniDifferimento" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtAnniDifferimento" ControlToValidate="txtAnniDifferimento"
                    ErrorMessage="Anni differimento: formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <asp:Panel runat="server" ID="pnlCodicePensioneSenzaRequisiti" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Pensione no requ.:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlCodicePensioneSenzaRequisiti" Width="50px"
                        CssClass="txtUppercase tb8 xxs" TabIndex="3" Enabled="false">
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Età perf. Req.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtEtaPerfezionamentoRequisiti" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtEtaPerfezionamentoRequisiti" ControlToValidate="txtEtaPerfezionamentoRequisiti"
                    ErrorMessage="Età perf. Req.: formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Perf. Requisiti:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDataPerfezionamentoRequisiti"
                    Width="50%" Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="1"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDataPerfezionamentoRequisiti" ControlToValidate="txtDataPerfezionamentoRequisiti"
                    Enabled="true" ErrorMessage="Data perf. Requisiti in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataPerfezionamentoRequisiti"
                    Display="Dynamic" ErrorMessage="Data Perf. Requisiti: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabDatiAssicurativiFS" ID="customCheckDataDataPerfezionamentoRequisiti"
                    ClientValidationFunction="checkCorrettezzaData" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDataPerfezionamentoRequisiti"
                    ControlToValidate="txtDataPerfezionamentoRequisiti" Display="Dynamic" Text="*" CssClass="field-is-required"
                    ErrorMessage="Data Perf. Requisiti obbligatoria" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contr. Provv.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlContrProvv" Width="50px" CssClass="txtUppercase tb8 xxs"
                    TabIndex="3">
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello CL -->
<!-- Pannello Codice Requisiti -->
<asp:Panel ID="pnlCodiceRequisiti" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Requisiti:</label>
            </td>
            <td class="field full-grid inline-fields colspan2-1">
                <asp:DropDownList runat="server" ID="ddlCodRequisiti1" Width="90%" TabIndex="55"
                    CssClass="txtUppercase tb8">
                </asp:DropDownList>
                <div>
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="28" ID="txtCodiceRequisiti2" Width="5%" MaxLength="1" Text="0"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateCodiceRequisiti2" ControlToValidate="txtCodiceRequisiti2"
                        ErrorMessage="Codice requisiti in formato non valido" ValidationExpression="^[a-zA-Z0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </div>
                
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Codice Requisiti -->
<!-- Pannello Common Footer -->
<asp:Panel ID="pnlPensioneAGO" runat="server"  Visible="false">
    <table class="tabellaFormattazione grid grid-size-25" style="width: 100%">
        <tr>
            <td colspan="4" style="width: 100%">
                <div id="pdivPensioneAGO"
                    runat="server"
                    style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 100%; margin-left: 0px">
                    <table cellpadding="3" cellspacing="1" border="0" width="100%">
                        <!-- Titolo pannello -->
                        <tr>
                            <td class="Row1" colspan="4">
                                <label style="font-weight: bold;">
                                    PENSIONE AGO
                                </label>
                            </td>
                        </tr>

                        <!-- Codice categoria -->
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>Codice categoria:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:TextBox ID="txtPensioneAGOCodiceCategoria" runat="server"
                                    CssClass="tb8 txtUppercase"
                                    Width="80px" />
                            </td>
                            <td></td>
                            <td></td>
                        </tr>

                        <!-- Sede -->
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>Sede:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:TextBox ID="txtPensioneAGOSede" runat="server"
                                    CssClass="tb8 txtUppercase"
                                    Width="80px" />
                            </td>
                            <td></td>
                            <td></td>
                        </tr>

                        <!-- Certificato -->
                        <tr>
                            <td class="Row1" style="width: 180px; padding-left: 5px;">
                                <label>Certificato:</label>
                            </td>
                            <td class="field" style="width: 150px; padding-left: 2px;">
                                <asp:TextBox ID="txtPensioneAGOCertificato" runat="server"
                                    CssClass="tb8 txtUppercase"
                                    Width="120px" />
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Panel>




<asp:Panel ID="pnlCommonFooter" runat="server">
    <table class="tabellaFormattazione grid grid-size-25">
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
                    Enabled="false" />
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
                    OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativiFS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
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
<asp:HiddenField runat="server" ID="tipoFondo" Value="" />
