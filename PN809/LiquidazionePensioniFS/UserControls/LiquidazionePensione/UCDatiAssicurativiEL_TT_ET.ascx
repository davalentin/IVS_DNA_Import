<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiEL_TT_ET.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiAssicurativiEL_TT_ET" %>
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendariInizioFineAssicurazione();

        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiEL_TT_ET_HiddenFieldAziende").value.split(';');
        $("#<%=txtAziendaET.ClientID%>").autocomplete({
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

        chkDimissioniAnte97OnChange($("#<%= chkDimissioniAnte97.ClientID %>"));
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
                            <asp:TemplateField HeaderText="Codice natura" HeaderStyle-CssClass="intestazioneTabella Row1 width-fixed-230"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <div class="full-width cod-nat">
                                        <asp:TextBox runat="server" ID="lblcodiceNatura1" Text='<%#Bind("_CodiceNatura1")%>'
                                            Enabled="false" Width="13px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
                                        <asp:TextBox runat="server" ID="lblCodiceNatura2" Text='<%#Bind("_CodiceNatura2")%>'
                                            Enabled="false" Width="13px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
                                        <asp:TextBox runat="server" ID="lblCodiceNatura3" Text='<%#Bind("_CodiceNatura3")%>'
                                            Enabled="false" Width="13px" CssClass="tb8 txtUppercase width-33"></asp:TextBox>
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
                                        TabIndex="4" Width="50px" Text=' <%# Bind("_CodiceNonCalcolo")%>'>
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
                                    <%--<asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaRecordFondo" Width="100px" MaxLength="7"
                                         CssClass="txtUppercase tb8 date-picker-maxActual" TabIndex="5" Text=' <%# Bind("strDecorrenzaValidita")%>'>
                                    </asp:TextBox>--%>
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
                <asp:Panel runat="server" ID="pnlTxtPrimoVersamento" CssClass="full-width">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPrimoVersamento"
                        ErrorMessage="Data primo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo versamento: Inserire la data del primo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtPrimoVersamento"></asp:RequiredFieldValidator>
                    <%--<asp:CustomValidator runat="server" ControlToValidate="txtPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo Versamento: data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ID="customPrimoVersamento" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />--%>
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
                <asp:Panel runat="server" ID="pnlTxtUltimoVersamento" CssClass="full-width">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtUltimoVersamento" ControlToValidate="txtUltimoVersamento"
                        ErrorMessage="Data ultimo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <%--<asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ID="customUltimoVersamento" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />--%>
                    <asp:RequiredFieldValidator runat="server" ID="RFUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo versamento: Inserire la data dell'ultimo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtUltimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataUltimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:Panel runat="server" ID="pnlDecorrenzaTeorica" Visible="false">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Teorica:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaTeorica" Width="50%"
                        Text="" CssClass="txtUppercase tb8 date-picker dateMMaaaa" TabIndex="3" MaxLength="7"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="txtDecorrenzaTeorica_RF" Display="Dynamic"
                        ErrorMessage="Decorrenza Teorica: Inserire la decorrenza teorica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ControlToValidate="txtDecorrenzaTeorica" Enabled="false"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="validateTxtDecorrenzaTeorica" ControlToValidate="txtDecorrenzaTeorica"
                        ErrorMessage="Data decorrenza teorica in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaTeorica" Display="Dynamic"
                        ErrorMessage="Decorrenza Teorica: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataDecorrenzaTeorica" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="field" style="width: 25%"></td>
                <td class="field" style="width: 25%"></td>
            </tr>
        </table>
    </asp:Panel>
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
</asp:Panel>
<!-- Fine Pannello Common Header -->
<!-- Pannello Common EL TT -->
<asp:Panel runat="server" ID="pnlCommonEL_TT" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Convenzione:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodiceConvenzione" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="4">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Attività Svolta:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlAttivitaSvolta" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="5">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAttivitaSvolta_RF" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Attività svolta: Si prega di inserire l'Attività Svolta"
                    ControlToValidate="ddlAttivitaSvolta" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Common EL TT -->
<!-- Pannello Elettrici -->
<asp:Panel runat="server" ID="pnlEL" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti:</label>
            </td>
            <td class="Row1 flex-align-center gap-4" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="6" ID="txtRiscattiAA" Width="20%" Text="AA" MaxLength="2" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtRiscattiAA" ControlToValidate="txtRiscattiAA"
                    ErrorMessage="Anni riscatto in formato non valido" ValidationExpression="^[0-9]+|aa|AA$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="7" ID="txtRiscattiMM" Width="20%" Text="MM" MaxLength="2" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtRiscattiMM" ControlToValidate="txtRiscattiMM"
                    ErrorMessage="Mesi riscatto in formato non valido" ValidationExpression="^[0-9]+|mm|MM$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtRiscattiMM" Display="Dynamic"
                    ErrorMessage="Riscatti: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customTxtRiscattiMM" ClientValidationFunction="checkCorrettezzaMese" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Anzianità Pregressa:</label>
            </td>
            <td class="Row1  flex-align-center gap-4" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="8" ID="txtAnzianitaPregressaAA" Width="20%" Text="AA" MaxLength="2"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtAnzianitaPregressaAA" ControlToValidate="txtAnzianitaPregressaAA"
                    ErrorMessage="Anni anzianità pregressa in formato non valido" ValidationExpression="^[0-9]+|aa|AA$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"></asp:RegularExpressionValidator>
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="9" ID="txtAnzianitaPregressaMM" Width="20%" Text="MM" MaxLength="2"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtAnzianitaPregressaMM" ControlToValidate="txtAnzianitaPregressaMM"
                    ErrorMessage="Anni riscatto in formato non valido" ValidationExpression="^[0-9]+|mm|MM$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtAnzianitaPregressaMM" Display="Dynamic"
                    ErrorMessage="Anzianità Pregressa: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customAnzianitaPregeressa" ClientValidationFunction="checkCorrettezzaMese" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Militare:</label>
            </td>
            <td class="Row1  flex-align-center gap-4" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="10" ID="txtServizioMilitareAA" Width="20%" Text="AA" MaxLength="2"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtServizioMilitareAA" ControlToValidate="txtServizioMilitareAA"
                    ErrorMessage="Anni servizio militare in formato non valido" ValidationExpression="^[0-9]+|aa|AA$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="11" ID="txtServizioMilitareMM" Width="20%" Text="MM" MaxLength="2"
                    onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                    onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtServizioMilitareMM" ControlToValidate="txtServizioMilitareMM"
                    ErrorMessage="Mesi servizio militare in formato non valido" ValidationExpression="^[0-9]+|mm|MM$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtServizioMilitareMM" Display="Dynamic"
                    ErrorMessage="Servizio militare: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customTxtServizioMilitareMM" ClientValidationFunction="checkCorrettezzaMese" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Articolo 3 Legge 1079:</label>
            </td>
            <td class="Row1  flex-align-center gap-4" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="12" ID="txtArt3AA" Width="20%" Text="AA" MaxLength="2" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtArt3AA" ControlToValidate="txtArt3AA"
                    ErrorMessage="Anni Articolo 4 Legge 1079 in formato non valido" ValidationExpression="^[0-9]+|aa|AA$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="13" ID="txtArt3MM" Width="20%" Text="MM" MaxLength="2" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateTxtArt3MM" ControlToValidate="txtArt3MM"
                    ErrorMessage="Mesi articolo 4 Legge 1079 in formato non valido" ValidationExpression="^[0-9]+|mm|MM$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtArt3MM" Display="Dynamic"
                    ErrorMessage="Articolo 3: mese non valido" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customTxtArt3MM" ClientValidationFunction="checkCorrettezzaMese" />
            </td>
        </tr>
        <asp:Panel ID="pnlCodDirittoQuoteFisse_EL" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Cod. Diritto Quote Fisse:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodDirittoQuoteFisse_EL" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="14">
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlInvalidita_MaggiorazioneAnte97" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Grado Invalidità:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlGradoInvalidita" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="51">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Percentuale Maggiorazione:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                        TabIndex="15" ID="txtPercentualeMaggiorazione" Width="5%" MaxLength="2"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateTxtPercentualeMaggiorazione" ControlToValidate="txtPercentualeMaggiorazione"
                        ErrorMessage="Percentuale maggiorazione in formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <label>
                        %</label>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Prorata ENEL:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlProrataEnel" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="16">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Azienda:</label>
            </td>
            <td class="field">
                <asp:DropDownList runat="server" ID="ddlCodiceAziendaEL" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="17">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Elettrici -->
<!-- Pannello Telefonici -->
<asp:Panel runat="server" ID="pnlTT" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Dimissioni ante 1-7-97:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:CheckBox ID="chkDimissioniAnte97" runat="server" CssClass="txtUppercase tb8"
                    onclick="chkDimissioniAnte97OnChange(this)" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti contributi fissi:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtContributiFissiAnno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator16" ControlToValidate="txtContributiFissiAnno"
                    ErrorMessage="Riscatti contributi fissi AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblContributiFissiAnno" runat="server" Text="a"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtContributiFissiMese" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="20"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator18" ControlToValidate="txtContributiFissiMese"
                    ErrorMessage="Riscatti contributi fissi MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblContributiFissiMese" runat="server" Text="m"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtContributiFissiGiorno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="21"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator19" ControlToValidate="txtContributiFissiGiorno"
                    ErrorMessage="Riscatti contributi fissi GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblContributiFissiGiorno" runat="server" Text="g"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti riserva matematica:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtRiscattiRiservaAnno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="22"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator21" ControlToValidate="txtRiscattiRiservaAnno"
                    ErrorMessage="Riscatti riserva matematica AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblRiscattiRiservaAnno" runat="server" Text="a"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtRiscattiRiservaMese" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="23"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator22" ControlToValidate="txtRiscattiRiservaMese"
                    ErrorMessage="Riscatti riserva matematica MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblRiscattiRiservaMese" runat="server" Text="m"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtRiscattiRiservaGiorno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="24"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator23" ControlToValidate="txtRiscattiRiservaGiorno"
                    ErrorMessage="Riscatti riserva matematica GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblRiscattiRiservaGiorno" runat="server" Text="g"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Periodi figurativi:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtPeriodiFigurativiAnno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="25"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator24" ControlToValidate="txtPeriodiFigurativiAnno"
                    ErrorMessage="Periodi figurativi AA: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblPeriodiFigurativiAnno" runat="server" Text="a"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtPeriodiFigurativiMese" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="26"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator25" ControlToValidate="txtPeriodiFigurativiMese"
                    ErrorMessage="Periodi figurativi MM: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblPeriodiFigurativiMese" runat="server" Text="m"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtPeriodiFigurativiGiorno" runat="server" CssClass="txtUppercase tb8"
                    MaxLength="2" Width="5%" TabIndex="27"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator26" ControlToValidate="txtPeriodiFigurativiGiorno"
                    ErrorMessage="Periodi figurativi GG: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:Label ID="lblPeriodiFigurativiGiorno" runat="server" Text="g"></asp:Label>
                <span style="visibility: hidden">&nbsp;</span>
            </td>
            <%--<td class="field" colspan="2"></td>
            <td class="field"></td>--%>
        </tr>
        <asp:Panel ID="pnlSupplementoOBG" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Supplemento OBG:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox ID="txtSupplementoOBG" runat="server" CssClass="tb8 txtUppercase" Width="90%"
                        TabIndex="28" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSupplementoOBG" Display="Dynamic"
                        ControlToValidate="txtSupplementoOBG" Enabled="true" ErrorMessage="Supplemento OBG: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Ditta:</label>
            </td>
            <td colspan="3" class="field full-grid">
                <asp:DropDownList runat="server" ID="ddlDitta" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="29">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Panel ID="pnlPensioneGenitori" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Pensione genitori:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox ID="txtPensioneGenitori" runat="server" CssClass="tb8 txtUppercase"
                        Width="90%" TabIndex="30" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtPensioneGenitori" Display="Dynamic"
                        ControlToValidate="txtPensioneGenitori" Enabled="true" ErrorMessage="Pensione genitori: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Art.5 Legge 58:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:CheckBox ID="chkArt5Legge58" runat="server" CssClass="txtUppercase tb8" TabIndex="31" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Telefonici -->
<!-- Pannello ElettroTramvieri -->
<asp:Panel runat="server" ID="pnlET" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Part Time:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlPartTime" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="32">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Cessazione Iscrizione:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCessazioneIscrizione"
                    Width="95px" Text="" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                    TabIndex="33" MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtCessazioneIscrizione"
                    ErrorMessage="Cessazione Iscrizione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtCessazioneIscrizione" Display="Dynamic"
                    ErrorMessage="Cessazione Iscrizione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customCheckDataCessazioneIscrizione" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Interruzione / PTV:</label>
            </td>
            <td class="field full-grid fileds-date-input " colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInterrPTVAnno" Width="30px"
                    CssClass="txtUppercase tb8" TabIndex="34" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtInterrPTVAnno"
                    ErrorMessage="Interruzione / PTV: Anno in formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <label>
                    a</label><span style="visibility: hidden">&nbsp;&nbsp;</span>
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInterrPTVMese" Width="30px"
                    CssClass="txtUppercase tb8" TabIndex="35" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtInterrPTVMese"
                    ErrorMessage="Interruzione / PTV: Mese in formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <label>
                    m</label><span style="visibility: hidden">&nbsp;&nbsp;</span>
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInterrPTVGiorno" Width="30px"
                    CssClass="txtUppercase tb8" TabIndex="36" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator13" ControlToValidate="txtInterrPTVGiorno"
                    ErrorMessage="Interruzione / PTV: Giorno in formato non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <label>
                    g</label>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlET_ServizioMilitare" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Servizio Militare:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlServizioMilitare" Width="15%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="48">
                        <asp:ListItem Text="NO" Value="False"></asp:ListItem>
                        <asp:ListItem Text="SI" Value="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane leva:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNSettimaneLeva" Width="30px"
                        CssClass="txtUppercase tb8" TabIndex="37" MaxLength="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator14" ControlToValidate="txtNSettimaneLeva"
                        ErrorMessage="Numero Settimane leva: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Settimane Richiamato:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtNSettimanaRichiamato"
                        Width="30px" CssClass="txtUppercase tb8" TabIndex="38" MaxLength="3"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator17" ControlToValidate="txtNSettimanaRichiamato"
                        ErrorMessage="Numero Settimane richiamato: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Contributi Ago L. 402/45:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtContribAgoL402" runat="server" CssClass="tb8 txtUppercase" Width="70%"
                        TabIndex="39" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" Display="Dynamic"
                        ControlToValidate="txtContribAgoL402" Enabled="true" ErrorMessage="Contributi Ago L. 402/45: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Contributi Ago L. 140/83:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtContribAgoL140" runat="server" CssClass="tb8 txtUppercase" Width="70%"
                        TabIndex="40" MaxLength="15"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator4" Display="Dynamic"
                        ControlToValidate="txtContribAgoL140" Enabled="true" ErrorMessage="Contributi Ago L. 140/83: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Azienda:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox runat="server" ID="txtAziendaET" TabIndex="41" CssClass="tb8 txtUppercase"
                    Width="90%"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="txtAziendaET_RF" ControlToValidate="txtAziendaET"
                    Display="Dynamic" Enabled="true" ErrorMessage="Azienda: si prega di inserire il codice"
                    ValidationGroup="UCTabDatiAssicurativiFS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <span style="visibility: hidden">&nbsp;&nbsp;</span>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Stipendio:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtStipendio" runat="server" CssClass="tb8 txtUppercase" Width="90%"
                    TabIndex="43" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" Display="Dynamic"
                    ControlToValidate="txtStipendio" Enabled="true" ErrorMessage="Stipendio: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtStipendio" ControlToValidate="txtStipendio"
                    Display="Dynamic" Enabled="true" ErrorMessage="Stipendio obbligatorio" ValidationGroup="UCTabDatiAssicurativiFS"
                    Text="*" CssClass="field-is-required" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Tredicesima:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtTredicesima" runat="server" CssClass="tb8 txtUppercase" Width="70%"
                    TabIndex="44" MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator6" Display="Dynamic"
                    ControlToValidate="txtTredicesima" Enabled="true" ErrorMessage="Tredicesima: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTredicesima" ControlToValidate="txtTredicesima"
                    Display="Dynamic" Enabled="true" ErrorMessage="Tredicesima obbligatoria" ValidationGroup="UCTabDatiAssicurativiFS"
                    Text="*" CssClass="field-is-required" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Quattordicesima:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuattordicesima" runat="server" CssClass="tb8 txtUppercase" Width="70%"
                    TabIndex="45" MaxLength="9"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" Display="Dynamic"
                    ControlToValidate="txtQuattordicesima" Enabled="true" ErrorMessage="Quattordicesima: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Elementi Accessori:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtElementiAccessori" runat="server" CssClass="tb8 txtUppercase"
                    Width="90%" TabIndex="46" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator8" Display="Dynamic"
                    ControlToValidate="txtElementiAccessori" Enabled="true" ErrorMessage="Elementi Accessori: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtElementiAccessori" Display="Dynamic"
                    ErrorMessage="Elementi Accessori è un dato obbligatorio." Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ControlToValidate="txtElementiAccessori"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    40% delle competenze:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox ID="txtCompetenze" runat="server" CssClass="tb8 txtUppercase" Width="90%"
                    TabIndex="47" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator9" Display="Dynamic"
                    ControlToValidate="txtCompetenze" Enabled="true" ErrorMessage="40% delle competenze: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Esodo:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlEsodo" Width="60%" CssClass="tb8 txtUppercase"
                    TabIndex="49">
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Esodo:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtRetribuzioneEsodo" runat="server" CssClass="tb8 txtUppercase"
                    Width="70%" TabIndex="50" MaxLength="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator10"
                    Display="Dynamic" ControlToValidate="txtRetribuzioneEsodo" Enabled="true" ErrorMessage="Retribuzione Esodo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
            </td>
        </tr>
        <asp:Panel ID="pnlInvaliditaTxt" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Grado Invalidità:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:TextBox ID="txtGradoInvalidita" runat="server" CssClass="tb8 txtUppercase" Width="30px"
                        TabIndex="51" MaxLength="2"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator15" ControlToValidate="txtGradoInvalidita"
                        ErrorMessage="Grado Invalidità: formato non valido" ValidationExpression="^[0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlPersonaleViaggiante" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Personale Viaggiante:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlPersonaleViaggiante" Width="80%" CssClass="tb8 txtUppercase"
                        Enabled="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlPersonaleViaggiante" Display="Dynamic"
                        ControlToValidate="ddlPersonaleViaggiante" ErrorMessage="Personale Viaggiante obbligatorio"
                        ValidationGroup="UCTabDatiAssicurativiFS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<!-- Fine Pannello ElettroTramvieri -->
<!-- Pannello TT_ET -->
<asp:Panel ID="pnlCommonTT_ET" runat="server" Visible="false">
    <asp:Panel ID="pnlInailTT_ET" runat="server" Visible="false">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Rendita Inail:</label>
                </td>
                <td class="field" style="width: 25%">
                    <asp:TextBox ID="txtRenditaInail" runat="server" CssClass="tb8 txtUppercase" Width="70%"
                        TabIndex="52" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRenditaInail" Display="Dynamic"
                        ControlToValidate="txtRenditaInail" Enabled="true" ErrorMessage="Rendita Inail: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Retribuzione Effettiva Inail:</label>
                </td>
                <td class="field" style="width: 25%">
                    <!-- Il MaxLength viene modificato lato codice per le TT -->
                    <asp:TextBox ID="txtRetribEffettivaInail" runat="server" CssClass="tb8 txtUppercase"
                        Width="70%" TabIndex="53" MaxLength="11"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribEffettivaInailET"
                        Display="Dynamic" ControlToValidate="txtRetribEffettivaInail" Enabled="false"
                        ErrorMessage="Retribuzione Effettiva Inail: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,6}(\,\d{1,4})?" />
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribEffettivaInailTT"
                        Display="Dynamic" ControlToValidate="txtRetribEffettivaInail" Enabled="false"
                        ErrorMessage="Retribuzione Effettiva Inail: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d{1,4}(\,\d{1,4})?" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
<!-- Fine Pannello TT_ET -->
<!-- Pannello Common Footer -->
<asp:Panel ID="pnlCommonFooter" runat="server">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Specifico:</label>
            </td>
            <td class="field full-grid">
                <asp:DropDownList runat="server" ID="ddlCodiceSpecifico" CssClass="txtUppercase tb8"
                    TabIndex="54" Width="90%">
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
                    Codice Requisiti:</label>
            </td>
            <td class="field full-grid inline-fields colspan2-1">
                <asp:DropDownList runat="server" ID="ddlCodRequisiti1" Width="90%" TabIndex="55"
                    CssClass="txtUppercase tb8">
                </asp:DropDownList>
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="28" ID="txtCodiceRequisiti2" Width="5%" MaxLength="1" Text="0"></asp:TextBox>
                <asp:RegularExpressionValidator ID="validateCodiceRequisiti2" ControlToValidate="txtCodiceRequisiti2"
                    ErrorMessage="Codice requisiti in formato non valido" ValidationExpression="^[a-zA-Z0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
        <tr runat="server" id="trDirittoQuoteFisse" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Diritto Quote Fisse:</label>
            </td>
            <td class="field">
                <asp:TextBox runat="server" ID="txtDirittoQuoteFisse" MaxLength="1" CssClass="txtUppercase tb8" Width="30px"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REV_txtDirittoQuoteFisse" ControlToValidate="txtDirittoQuoteFisse"
                    ErrorMessage="Diritto Quote Fisse deve essere un valore numerico" ValidationExpression="^[0-9]$"
                    Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true"></asp:RegularExpressionValidator>
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
<asp:HiddenField runat="server" ID="HiddenFieldAziende" />
