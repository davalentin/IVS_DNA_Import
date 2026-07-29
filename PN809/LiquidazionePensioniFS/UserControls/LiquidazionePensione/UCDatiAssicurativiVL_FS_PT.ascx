<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiVL_FS_PT.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiAssicurativiVL_FS_PT" %>
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendariInizioFineAssicurazione();

        <%--var availableTagsCausaCess = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiVL_FS_PT_HiddenFieldCausaCessazione").value.split(';');
        $("#<%=txtCausaCessazione.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsCausaCess
        });--%>

        SetCalendariInizioFineAssicurazione();

        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiVL_FS_PT_hiddenAttivitaSvolte").value.split(';');
        $("#<%=txtAttivitaSvoltaFS.ClientID%>").autocomplete({
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

        ddlDirittoIndennIntegrSpecOnChange();
        $(document.getElementById("<%= ddlDirittoIndennIntegrSpec.ClientID %>")).change(function () {
            ddlDirittoIndennIntegrSpecOnChange();
        });

        ddlCodCapitalizzazioneOnChange($("#<%= ddlCodCapitalizzazione.ClientID %>"));
        $("#<%= ddlCodCapitalizzazione.ClientID %>").change(function () {
            ddlCodCapitalizzazioneOnChange(this);
        });
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

    function validateMese(source, args) {
        var mesi = args.Value;
        if (mesi < 0 || mesi > 11)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function validateGiorno(source, args) {
        var giorni = args.Value;
        if (giorni < 0 || giorni > 30)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function ddlDirittoIndennIntegrSpecOnChange() {
        var ddl = document.getElementById("<%= ddlDirittoIndennIntegrSpec.ClientID %>");
        if (ddl) {
            var iisConglobata = $("#<%= ddlIndennIntegrSpecConglobata.ClientID %>");
            var iisRapportata = $("#<%= ddlIISAbbattimentoAnni.ClientID %>");
            var riduzioneL537 = $("#<%= ddlRiduzioneL537.ClientID %>");
            var hdnReversibilita024 = $("#<%= hdnReversibilità024.ClientID %>").val();           

            if (ddl.value == "NO") {
                if (iisConglobata) {
                    iisConglobata.val("NO");
                    iisConglobata.attr('disabled', true);
                }

                if (iisRapportata) {
                    iisRapportata.val("NO");
                    iisRapportata.attr('disabled', true);
                }

                if (riduzioneL537) {
                    riduzioneL537.val("NO");
                    riduzioneL537.attr('disabled', true);
                }
            }
            else {

                if (iisConglobata)
                {

                if(hdnReversibilita024 == "SI")
                    iisConglobata.attr('disabled', true);
                 else
                    iisConglobata.attr('disabled', false);
                }

                if (iisRapportata) {
                    iisRapportata.attr('disabled', false);
                }

                if (riduzioneL537) {
                    riduzioneL537.attr('disabled', false);
                }
            }
        }
    }

    function valorizzaDecorrenzaCalcoloPerBonus(inizioBonus) {
        if (inizioBonus && inizioBonus.toUpperCase() != 'MM/AAAA') {
            if ($("#<%= lblDecorrenzaCalcolo.ClientID %>"))
                $("#<%= lblDecorrenzaCalcolo.ClientID %>").text("01/" + inizioBonus);
            if ($("#<%= txtDecorrenzaCalcolo.ClientID %>"))
                $("#<%= txtDecorrenzaCalcolo.ClientID %>").val("01/" + inizioBonus);
            if ($("#<%= hdnDecorrenzaCalcolo.ClientID %>"))
                $("#<%= hdnDecorrenzaCalcolo.ClientID %>").val("01/" + inizioBonus);
        }
    }

    function ddlCodCapitalizzazioneOnChange(that) {
        if ($(that).val() == 3) {
            $("#<%= txtImportoPercentualeCapitalizzazione.ClientID %>").val(5000);
            $("#<%= txtImportoPercentualeCapitalizzazione.ClientID %>").attr('readonly', true);
        }
        else
            $("#<%= txtImportoPercentualeCapitalizzazione.ClientID %>").removeAttr('readonly');
    }
    
</script>
<!-- Pannello Common Header -->
<asp:Panel runat="server" ID="pnlCommonHeader">
    <asp:Panel runat="server" ID="pnlRecordFondo">
        <hr />
        <table class="tabellaContenuti">
            <tr>
                <td align="left">
                    <asp:Label runat="server" ID="lblRecordFondo" Font-Bold="true">Dati Record fondo</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:Panel ID="pnlGridViewVL" runat="server" Visible="false">
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
                    </asp:Panel>
                    <asp:Panel ID="pnlGridViewFS_PT" runat="server" Visible="false">
                        <div class="bckGridViewElenco full-size" style="width: 700px">
                            <asp:GridView runat="server" ID="gvRecordFondoFS_PT" SkinID="grdElenco1" AutoGenerateColumns="false"
                                CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                                AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvRecordFondoFS_PT_RowCommand"
                                OnRowDataBound="gvRecordFondoFS_PT_RowDataBound" OnRowCancelingEdit="gvRecordFondoFS_PT_RowCancelingEdit"
                                OnRowEditing="gvRecordFondoFS_PT_RowEditing" EnableViewState="true" OnRowUpdating="gvRecordFondoFS_PT_RowUpdating"
                                OnPageIndexChanging="gvRecordFondoFS_PT_onPageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                                <Columns>
                                    <asp:TemplateField HeaderText="Codice natura" HeaderStyle-CssClass="intestazioneTabella Row1"
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
                                                <asp:DropDownList runat="server" ID="ddlCodNatura1" Width="50px" CssClass="txtUppercase tb8">
                                                </asp:DropDownList>
                                                <asp:DropDownList runat="server" ID="ddlCodNatura2" Width="50px" CssClass="txtUppercase tb8">
                                                </asp:DropDownList>
                                                <asp:DropDownList runat="server" ID="ddlCodNatura3" Width="50px" CssClass="txtUppercase tb8">
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
                                            <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaRecordFondo"
                                                Width="100px" MaxLength="10" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA"
                                                TabIndex="5" Text=' <%# Bind("strDecorrenzaValidita")%>'>
                                            </asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDecorrenzaRecordFondo"
                                                ControlToValidate="txtDecorrenzaRecordFondo" ErrorMessage="Decorrenza record fondo in formato non valido"
                                                ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRecordFondo" Enabled="true" />
                                            <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaRecordFondo"
                                                Display="Dynamic" ErrorMessage="Decorrenza Record Fondo: data illogica" Text="*" CssClass="field-is-required"
                                                ValidationGroup="UCRecordFondo" ID="customCheckDataDecorrenzaRecordFondo" ClientValidationFunction="checkCorrettezzaData" />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sospensione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                        ItemStyle-CssClass="TblRecordset3">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblSospensione" Text=' <%# Bind("strDataSospensione")%>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox Style="text-align: left" runat="server" ID="txtSospensioneRecordFondo"
                                                Width="100px" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="6"
                                                MaxLength="10" Text=' <%# Bind("strDataSospensione")%>'></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtSospensioneRecordFondo"
                                                ControlToValidate="txtSospensioneRecordFondo" ErrorMessage="Sospensione record fondo in formato non valido"
                                                ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRecordFondo" Enabled="true" />
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
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <hr />
    </asp:Panel>
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione grid grid-size-20">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                    <asp:HiddenField ID="hdnTipoPensione" runat="server" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiAssicurativi" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlDecorrenzaCalcoloNuovaGestioneDatiFondoFSPT" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Calcolo:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:Label runat="server" ID="lblDecorrenzaCalcolo"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Primo Versamento:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtPrimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPrimoVersamento"
                        ErrorMessage="Data primo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
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
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtUltimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtUltimoVersamento" ControlToValidate="txtUltimoVersamento"
                        ErrorMessage="Data ultimo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RFUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo versamento: Inserire la data dell'ultimo versamento" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="txtUltimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataUltimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlCodiceSpecifico">
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Specifico:</label>
                </td>
                <td class="field full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceSpecifico" CssClass="txtUppercase tb8"
                        TabIndex="3" Width="90%">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlCodiceSpecifico_RF" Display="Dynamic"
                        Text="*" CssClass="field-is-required" ErrorMessage="Codice Specifico: Si prega di inserire il codice specifico"
                        ControlToValidate="ddlCodiceSpecifico" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </td>
            </asp:Panel>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Header -->
<!-- Pannello Custom VL -->
<asp:Panel runat="server" ID="pnlCustomVL" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Convenzione:</label>
            </td>
            <td class="Row1">
                <asp:DropDownList runat="server" ID="ddlCodiceConvenzione" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="5">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Art. 22:</label>
            </td>
            <td class="Row1">
                <asp:DropDownList runat="server" ID="ddlCodArt22" CssClass="txtUppercase tb8" TabIndex="6"
                    Width="90%">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Codice Art. 22: Si prega di inserire il Codice Art. 22"
                    ControlToValidate="ddlCodArt22" ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
            </td>
        </tr>
        <tr id="rowDataInvalidita" runat="server" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Data Invalidità:</label>
            </td>
            <td class="Row1">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtInvalidita" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA" TabIndex="7"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REtxtInvalidita" ControlToValidate="txtInvalidita"
                    ErrorMessage="Data Invalidità in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="Data Invalidità: Si prega di inserire la Data Invalidità"
                    ControlToValidate="txtInvalidita" ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtInvalidita" Display="Dynamic"
                    ErrorMessage="Data Invalidità: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ID="customCheckDataDataInvalidita" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Versamenti Volontari:</label>
            </td>
            <td class="Row1 fileds-date-input">
                <asp:TextBox ID="txtVersamentiVolontariAA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="8" MaxLength="2"></asp:TextBox>
                <label>
                    a</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator15" ControlToValidate="txtVersamentiVolontariAA"
                    ErrorMessage="Versamenti Volontari: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtVersamentiVolontariMM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="9" MaxLength="2"></asp:TextBox>
                <label>
                    m</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtVersamentiVolontariMM"
                    ErrorMessage="Versamenti Volontari: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtVersamentiVolontariMM"
                    Display="Dynamic" ErrorMessage="Versamenti Volontari: numero Mesi non valido"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ID="customCheckVersamentiVolontariMM"
                    ClientValidationFunction="validateMese" />
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtVersamentiVolontariGG" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="10" MaxLength="2"></asp:TextBox>
                <label>
                    g</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtVersamentiVolontariGG"
                    ErrorMessage="Versamenti Volontari: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtVersamentiVolontariGG"
                    Display="Dynamic" ErrorMessage="Versamenti Volontari: numero Giorni non valido"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ID="CustomValidator1" ClientValidationFunction="validateGiorno" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Riscatti / Ricongiunzioni:</label>
            </td>
            <td class="Row1 fileds-date-input">
                <asp:TextBox ID="txtRiscattiRicongiunzioniAA" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="11" MaxLength="2"></asp:TextBox>
                <label>
                    a</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtRiscattiRicongiunzioniAA"
                    ErrorMessage="Riscatti / Ricongiunzioni: formato Anno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtRiscattiRicongiunzioniMM" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="12" MaxLength="2"></asp:TextBox>
                <label>
                    m</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator5" ControlToValidate="txtRiscattiRicongiunzioniMM"
                    ErrorMessage="Riscatti / Ricongiunzioni: formato Mese non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtRiscattiRicongiunzioniMM"
                    Display="Dynamic" ErrorMessage="Riscatti / Ricongiunzioni: numero Mesi non valido"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ID="CustomValidator3" ClientValidationFunction="validateMese" />
                <span style="visibility: hidden">&nbsp;</span>
                <asp:TextBox ID="txtRiscattiRicongiunzioniGG" runat="server" CssClass="tb8 txtUppercase"
                    Width="30px" TabIndex="13" MaxLength="2"></asp:TextBox>
                <label>
                    g</label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtRiscattiRicongiunzioniGG"
                    ErrorMessage="Riscatti / Ricongiunzioni: formato Giorno non valido" ValidationExpression="^[0-9]+$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" />
                <asp:CustomValidator runat="server" ControlToValidate="txtRiscattiRicongiunzioniGG"
                    Display="Dynamic" ErrorMessage="Riscatti / Ricongiunzioni: numero Giorni non valido"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ID="CustomValidator2" ClientValidationFunction="validateGiorno" />
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlCapitalizzazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice capitalizzazione:</label>
                </td>
                <td class="Row1">
                    <asp:DropDownList runat="server" ID="ddlCodCapitalizzazione" CssClass="txtUppercase tb8"
                        TabIndex="14" Width="90%">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Aliquota Irpef:</label>
                </td>
                <td class="Row1">
                    <asp:TextBox ID="txtAliquotaIRPEF" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                        TabIndex="15" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator8" Display="Dynamic"
                        ControlToValidate="txtAliquotaIRPEF" Enabled="true" ErrorMessage="Aliquota Irpef: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,2})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Imp - % Capitalizzazione:</label>
                </td>
                <td class="Row1">
                    <asp:TextBox ID="txtImportoPercentualeCapitalizzazione" runat="server" CssClass="tb8 txtUppercase"
                        Width="40%" TabIndex="16" MaxLength="19"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator7" Display="Dynamic"
                        ControlToValidate="txtImportoPercentualeCapitalizzazione" Enabled="true" ErrorMessage="Imp - % Capitalizzazione: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,2})?" />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione settimanale AGO (quota A):</label>
            </td>
            <td class="Row1">
                <asp:TextBox ID="txtRetrAgoQuotaA" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                    TabIndex="17" MaxLength="19"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator9" Display="Dynamic"
                    ControlToValidate="txtRetrAgoQuotaA" Enabled="true" ErrorMessage="Retribuzione settimanale AGO (quota A): Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,2})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione settimanale AGO (quota B):</label>
            </td>
            <td class="Row1">
                <asp:TextBox ID="txtRetrAgoQuotaB" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                    TabIndex="18" MaxLength="19"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator10"
                    Display="Dynamic" ControlToValidate="txtRetrAgoQuotaB" Enabled="true" ErrorMessage="Retribuzione settimanale AGO (quota B): Inserire valori interi o decimali"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS" ValidationExpression="\d+(\,\d{1,2})?" />
            </td>
        </tr>
        <tr runat="server" id="trDirittoQuoteFisse" visible="false">
            <td class="Row1" style="width: 25%">
                <label>
                    Diritto Quote Fisse:</label>
            </td>
            <td class="field">
                <asp:TextBox runat="server" ID="txtDirittoQuoteFisse" MaxLength="1" CssClass="txtUppercase tb8"
                    Width="30px"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REV_txtDirittoQuoteFisse" ControlToValidate="txtDirittoQuoteFisse"
                    ErrorMessage="Diritto Quote Fisse deve essere un valore numerico" ValidationExpression="^[0-9]$"
                    Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS" Enabled="true"></asp:RegularExpressionValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom VL -->
<!-- Pannello Custom FS -->
<asp:Panel runat="server" ID="pnlCustomFS" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <asp:Label ID="lblAttivitaFS" runat="server" Text="Qualifica Professionale:"></asp:Label>
            </td>
            <asp:Panel runat="server" ID="pnlDDLAttivitaSvoltaFS" Visible="false">
                <td class="Row1 full-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlAttivitaSvoltaFS" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="4">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlAttivitaSvoltaFS_RF" Display="Dynamic"
                        Text="*" CssClass="field-is-required" ErrorMessage="" ControlToValidate="ddlAttivitaSvoltaFS" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlTXTAttivitaSvoltaFS" Visible="false">
                <td class="Row1 full-grid" colspan="3">
                    <asp:TextBox runat="server" ID="txtAttivitaSvoltaFS" Width="90%" CssClass="txtUppercase tb8"
                        TabIndex="4">
                    </asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="txtAttivitaSvoltaFS_RF" Display="Dynamic"
                        Text="*" CssClass="field-is-required" ErrorMessage="" ControlToValidate="txtAttivitaSvoltaFS" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Causa di cessazione:</label>
            </td>
            <td class="Row1" colspan="2">
                <%--<asp:TextBox ID="txtCausaCessazione" runat="server" Width="90%" Text="" CssClass="txtUppercase tb8"></asp:TextBox>--%>
                <asp:DropDownList runat="server" ID="ddlCausaCessazioneFS" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="19">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Titolare Altra Pensione:</label>
            </td>
            <td class="chkField" colspan="2">
                <asp:DropDownList runat="server" ID="ddlTitAltraPensione" Width="10%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="28">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom FS -->
<!-- Pannello Custom PT -->
<asp:Panel runat="server" ID="pnlCustomPT" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Causa di cessazione:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <%--<asp:TextBox ID="txtCausaCessazionePT" runat="server" Width="90%" Text="" CssClass="txtUppercase tb8"></asp:TextBox>--%>
                <asp:DropDownList runat="server" ID="ddlCausaCessazionePT" Width="90%" CssClass="tb8 txtUppercase"
                    TabIndex="19">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Onere MEF:</label>
            </td>
            <td class="chkField" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlOnereMEF" Width="30%" CssClass="tb8 txtUppercase xxs"
                    TabIndex="28">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" Display="Dynamic"
                    ErrorMessage="Onere MEF: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                    ControlToValidate="ddlOnereMEF"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Ripartizione Inpdap
                </label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtRipartizioneInpdap" Width="68%"
                    Text="" CssClass="txtUppercase tb8" TabIndex="29" MaxLength="8"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator12" ControlToValidate="txtRipartizioneInpdap"
                    ErrorMessage="Ripartizione inpdap in formato non valido" ValidationExpression="^[0-9]?[0-9]?[0-9]?,[0-9]?[0-9]?[0-9]?[0-9]?$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom PT -->
<!-- Pannello Custom VL-PT -->
<asp:Panel runat="server" ID="pnlCustomVL_PT" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <asp:Label ID="lblAttivita" runat="server"></asp:Label>
            </td>
            <td class="Row1">
                <asp:DropDownList runat="server" ID="ddlAttivitaSvolta" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="ddlAttivitaSvolta_RF" Display="Dynamic"
                    Text="*" CssClass="field-is-required" ErrorMessage="" ControlToValidate="ddlAttivitaSvolta" ValidationGroup="UCTabDatiAssicurativiFS"
                    Enabled="true" />
            </td>
        </tr>
    </table>
</asp:Panel>
<!-- Pannello Custom FS-PT -->
<!-- Pannello Custom FS-PT -->
<asp:Panel runat="server" ID="pnlCustomFS_PT" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <asp:Panel runat="server" ID="pnlVecchiaGestioneDatiFondoFSPT" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%" visible="false" runat="server" id="tdPag1">
                    <label>
                        Pagamento Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1" style="width: 25%" visible="false" runat="server" id="tdPag2">
                    <asp:DropDownList runat="server" ID="ddlPagIndennIntegrSpec" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="21">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator6" Display="Dynamic"
                        ErrorMessage="Pagamento Indennità Integrativa Speciale: campo obbligatorio" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="ddlPagIndennIntegrSpec"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tredicesima Mensilità:</label>
                </td>
                <td class="Row1 partial-grid" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlTredicesimaMens" Width="10%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="22">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RFVddlTredicesimaMens" Display="Dynamic"
                        ErrorMessage="Tredicesima Mensilità: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ControlToValidate="ddlTredicesimaMens"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Calcolo:</label>
                </td>
                <td class="Row1 full-grid" colspan="3">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaCalcolo" Width="16.5%"
                        Text="" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="23"
                        MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator11" ControlToValidate="txtDecorrenzaCalcolo"
                        ErrorMessage="Decorrenza Calcolo in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator5" Display="Dynamic"
                        ErrorMessage="Decorrenza Calcolo: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ControlToValidate="txtDecorrenzaCalcolo"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaCalcolo" Display="Dynamic"
                        ErrorMessage="Decorrenza Calcolo: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativiFS"
                        ID="customCheckDataDecorrenzaCalcolo" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlDecAnteAgosto95" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Diritto Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlDirittoIndennIntegrSpec" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="24">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" Display="Dynamic"
                        ErrorMessage="Diritto Indennità Integrativa Speciale: campo obbligatorio" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativiFS" ControlToValidate="ddlDirittoIndennIntegrSpec"></asp:RequiredFieldValidator>
                </td>
                <asp:Panel runat="server" ID="pnlIntegrazioneMinimo" Visible="false">
                    <td class="Row1" style="width: 25%">
                        <label>
                            Integrazione al Minimo:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:DropDownList runat="server" ID="ddlIntegrazioneMinimo" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                            TabIndex="25">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </asp:Panel>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Riduzione L.537:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlRiduzioneL537" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="26">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        I.I.S. RAP. ad Anni:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlIISAbbattimentoAnni" Width="30.5%" CssClass="tb8 txtUppercase xxs"
                        TabIndex="27">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <asp:Panel runat="server" ID="pnlIISConglobata">
                <td class="Row1" style="width: 25%">
                    <label>
                        Indennità Integrativa Speciale Conglobata:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlIndennIntegrSpecConglobata" Width="30.5%"
                        CssClass="tb8 txtUppercase xxs" TabIndex="20">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlVVUtiliDiritto" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        VV utili diritto:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliDiritto" Width="45%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlVVUtiliMisura" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        VV utili misura:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtVVUtiliMisura" Width="45%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlAttivitaEconomica" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Attività Economica:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="68%"
                        Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlProfessioneIndividuale" Visible="false">
                <td class="Row1" style="width: 25%">
                    <label>
                        Professione individuale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale"
                        Width="68%" Text="" CssClass="txtUppercase tb8" TabIndex="29" Enabled="false" />
                </td>
            </asp:Panel>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom FS-PT -->
<!-- Pannello Custom Footer VL -->
<asp:Panel ID="pnlCustomFooterVL" runat="server" Visible="false">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Requisiti:</label>
            </td>
            <td class="field full-grid inline-fields colspan2-1">
                <asp:DropDownList runat="server" ID="ddlCodRequisiti1" Width="90%" TabIndex="20"
                    CssClass="txtUppercase tb8 width-50">
                </asp:DropDownList>
                <asp:TextBox Style="text-align: left" runat="server" CssClass="txtUppercase tb8 width-50"
                    TabIndex="30" ID="txtCodiceRequisiti2" Width="5%" MaxLength="1" Text="0"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validateCodiceRequisiti2" ControlToValidate="txtCodiceRequisiti2"
                        ErrorMessage="Codice requisiti in formato non valido" ValidationExpression="^[a-zA-Z0-9]+$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativiFS"
                        Enabled="true" />
                </div>
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
<asp:HiddenField runat="server" ID="HiddenFieldCausaCessazione" />
<asp:HiddenField runat="server" ID="hiddenAttivitaSvolte" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaCalcolo" />
<asp:HiddenField runat="server" ID="hdnReversibilità024" />
