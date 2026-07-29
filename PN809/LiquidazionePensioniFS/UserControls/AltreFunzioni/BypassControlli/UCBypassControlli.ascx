<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCBypassControlli.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.BypassControlli.UCBypassControlli" %>
<script type="text/javascript">
    $(document).ready(function () {
        var MaxLength = 1000;
        $(document.getElementById("<%=txtNote.ClientID %>")).keypress(function (e) {
            if ($(this).val().length >= MaxLength) {
                e.preventDefault();
            }
        });

    });

    function CleanFields() {
        $(document.getElementById('<%=txtInsertNumeroDomanda.ClientID %>')).val("");
        $(document.getElementById('<%=txtInsertCodCategoria.ClientID %>')).val("");
        $(document.getElementById('<%=txtInsertCodiceSede.ClientID %>')).val("");
        $(document.getElementById('<%=txtInsertNCertificato.ClientID %>')).val("");


        $(document.getElementById('<%=ddlBypassInsert.ClientID %>')).get(0).selectedIndex = 0;
        $(document.getElementById('<%=lblDescrizioneBypass.ClientID %>')).text("Nessun Bypass selezionato");
        $(document.getElementById('<%=txtNote.ClientID %>')).val("");
    }

    function CreatePopUp() {
        $('#divdialog').dialog({
            autoOpen: false,
            show: 'blind',
            hide: 'blind',
            modal: true,
            resizable: false,
            draggable: true,
            dialogClass: 'fixed-dialog',
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Ok': function () {
                    $(this).dialog('close');
                    return true;
                }
            }
        });
    }

    function ShowNota() {
        CreatePopUp();
        var text = $(document.getElementById("<%=hdnTextDialog.ClientID %>")).val();
        $('#textDialog').text(text);
        $('#divdialog').dialog('open');
        SetScroll();
        return false;
    }

    function SetScroll() {

        window.scrollBy(document.getElementById("<%= scrollX.ClientID %>").value, document.getElementById("<%= scrollY.ClientID %>").value);
    }

    function findScrollPosition() {

        var scrolledX;
        var scrolledY;

        scrolledX = document.documentElement.scrollLeft;
        scrolledY = document.documentElement.scrollTop;


        document.getElementById("<%= scrollX.ClientID %>").value = scrolledX;
        document.getElementById("<%= scrollY.ClientID %>").value = scrolledY;
    }

    
</script>
<asp:ValidationSummary runat="server" ID="validSummaryFilter" ValidationGroup="UCBypassControlliFiltro"
    Font-Size="Small" CssClass="errorBox" />
<asp:ValidationSummary runat="server" ID="tabBypassControlliInsert" ValidationGroup="UCBypassControlliInsert"
    Font-Size="Small" CssClass="errorBox" />
<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px" class="full-width pb-24">
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="pnlFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 765px; margin-left: 0px" CssClass="form-container background-light-blue full-width">
                <table class="tabellaFormattazione is-contents section-table overwrite-grid col-2" width="100%">
                    <tr>
                     <td style="width: 3%">
                        </td>                      
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Numero Domanda:</label>
                        </td>
                        <td class="field" style="width: 24%">
                            <asp:TextBox runat="server" ID="txtFiltroNumeroDomanda" CssClass="tb8 txtUppercase"
                                Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);" />                  
                            <asp:RegularExpressionValidator ID="revFiltroLunghezzaNumeroDomanda" ControlToValidate="txtFiltroNumeroDomanda"
                                ErrorMessage="Filtro di ricerca: Numero Domanda deve essere lungo 13" ValidationExpression="^[0-9]{13}$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro" />
                        </td>
                         <td style="width: 3%">
                        </td>                      
                        <td class="Row1" style="text-align: left; width: 17%">
                            <label class="section-table__label">
                                Chiave Pensione:</label>
                        </td>
                        <td class="field overwrite-flex-row" style="text-align: left; width: 30%">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtFiltroCodCategoria" Width="27px"
                                CssClass="txtUppercase tb8 " MaxLength="3" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revFiltroCategoriaNonValida" ControlToValidate="txtFiltroCodCategoria"
                                ErrorMessage="Filtro di ricerca: Categoria pensione non  valida" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro" />             
                            <asp:RegularExpressionValidator ID="revFiltroLunghezzaCategoria" ControlToValidate="txtFiltroCodCategoria"
                                ErrorMessage="Filtro di ricerca: Categoria pensione deve essere lunga 3" ValidationExpression="^[0-9]{3}$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro" />
                            -
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtFiltroCodiceSede" Width="42px"
                                CssClass="txtUppercase tb8  " MaxLength="4" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revFiltroCodiceSedeNonValido" ControlToValidate="txtFiltroCodiceSede"
                                ErrorMessage="Filtro di ricerca: Codice sede pensione non  valida" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro" />                 
                            <asp:RegularExpressionValidator ID="revFiltroLunghezzaCodiceSede" ControlToValidate="txtFiltroCodiceSede"
                                ErrorMessage="Filtro di ricerca: Codice sede pensione deve essere lungo 4" ValidationExpression="^[0-9]{4}$"
                                runat="server" Text="*"  Display="Dynamic" ValidationGroup="UCBypassControlliFiltro"
                                CssClass="Validator_SuPannelloFiltroRicerca field-is-required" />
                            -
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtFiltroNCertificato" Width="81px"
                                CssClass="txtUppercase tb8 " MaxLength="8" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revFiltroNCertificatNonValido" ControlToValidate="txtFiltroNCertificato"
                                ErrorMessage="Filtro di ricerca: Certificato pensione non valido" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro"
                                CssClass="Validator_SuPannelloFiltroRicerca field-is-required" />                 
                            <asp:RegularExpressionValidator ID="revFiltroLunghezzaNCertificato" ControlToValidate="txtFiltroNCertificato"
                                ErrorMessage="Filtro di ricerca: Certificato pensione deve essere lungo 8" ValidationExpression="^[0-9]{8}$"
                                runat="server" Text="*" Display="Dynamic" ValidationGroup="UCBypassControlliFiltro"
                                CssClass="Validator_SuPannelloFiltroRicerca field-is-required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%">
                        </td>
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Matricola:</label>
                        </td>
                        <td class="field" style="width: 24%">
                            <asp:TextBox runat="server" ID="txtFiltroMatricola" CssClass="tb8 txtUppercase" Width="150px"
                                MaxLength="8" />
                            <asp:RegularExpressionValidator runat="server" ID="revTxtFiltroMatricola" ControlToValidate="txtFiltroMatricola"
                                Enabled="true" Display="Dynamic" Text="*" CssClass="field-is-required" ValidationGroup="UCBypassControlliFiltro"
                                ValidationExpression="^[a-zA-Z0-9]{8}$" ErrorMessage="Filtro di ricerca: La matricola deve essere lunga 8 caratteri" />
                        </td>
                        <td style="width: 3%">
                        </td>
                        <td class="Row1" style="text-align: left; width: 17%">
                            <label class="section-table__label">
                                Tipo Bypass:</label>
                        </td>
                        <td class="field" style="width: 30%">
                            <asp:DropDownList runat="server" ID="ddlFiltroBypass" CssClass="tb8 txtUppercase offClassFilter"
                                Width="196px" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%">
                        </td>
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Bloccate:</label>
                        </td>
                        <td class="field" style="width: 24%">
                            <asp:DropDownList runat="server" ID="ddlLock" CssClass="tb8 txtUppercase xxs offClassFilter"
                                Width="60%">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="SI" Text="SI"></asp:ListItem>
                                <asp:ListItem Value="NO" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td colspan="3" class="full-grid">
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <div class="flex-group flex-group-reverse flex-group-right">
                                <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="if(validatePageFiltro()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary mr-0" />
                                <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();"
                                    Enabled="false" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td style="width: 720px" class="full-width pb-24">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Elenco Bypass Effettuati</label>
            <asp:GridView runat="server" ID="gvBypassControlli" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                Width="100%" PageSize="10" AllowPaging="true" OnRowCommand="gvBypassControlli_RowCommand"
                OnRowDataBound="gvBypassControlli_RowDataBound" OnPageIndexChanging="gvBypassControlli_onPageIndexChanging"
                OnRowDeleting="gvBypassControlli_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                <EmptyDataTemplate>
                    <center>
                        <asp:Label ID="lblNoData" runat="server" Text="Nessun record trovato." SkinID="lblNoData"
                            Visible="true"></asp:Label>
                    </center>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="Tipo Bypass" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblBypass"> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Pensione" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="left">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblpensione" Text='<%#Eval("CodCategoria") == null? "" : String.Format("{0}-{1}-{2}", Eval("CodCategoria"), Eval("CodiceSede").ToString().PadLeft(4, Convert.ToChar("0")),
                                                                                Eval("NCertificato").ToString().PadLeft(8, Convert.ToChar("0"))) %>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Numero Domanda" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblNumeroDomanda" Text='<%#Bind("NDomus")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Matricola" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMatricola" Text='<%#Bind("Matricola")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Note" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ControlStyle-CssClass="link-button tertiary ghost ghost--small">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="lblNote" Text='<%# ValorizzaTesto(((GridViewRow) Container)) %>'
                                CommandArgument='<%#Eval("Note") %>' CommandName="ShowNota" OnClientClick="findScrollPosition();"> 
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument='<%#Eval("Id")%>'
                                Visible='<%# !(bool)Eval("Lock")%>' runat="server" OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td style="width: 720px" class="full-width pb-24">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Inserimento</label>
            <asp:Panel ID="pnlInserimento" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 765px; margin-left: 0px" CssClass="form-container full-width">
                <table class="tabellaFormattazione is-contents section-table overwrite-grid col-3" width="100%">
                    <tr>
                        <td class="radioButton" style="width: 3%">
                            <asp:RadioButton runat="server" ID="radioInsertDomanda" CssClass="radioButton" AutoPostBack="true"
                                CausesValidation="true" OnCheckedChanged="radioInsert_CheckedChanged" onclick="findScrollPosition();"
                                GroupName="rdb_SuPannelloInsert" />
                        </td>
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Numero Domanda:
                            </label>
                        </td>
                        <td class="field" style="width: 24%;">
                            <asp:TextBox CssClass="tb8 txtUppercase offClassInsert onClassDomandaInsert" ID="txtInsertNumeroDomanda"
                                runat="server" Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);"
                                onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false); "></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revInsertLunghezzaNumeroDomanda" ControlToValidate="txtInsertNumeroDomanda"
                                ErrorMessage="Inserimento: Il Numero Domanda può contenere solo numeri (13 cifre)"
                                ValidationExpression="^[0-9]{13}$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCBypassControlliInsert" />
                            <asp:RequiredFieldValidator runat="server" ID="rfvInsertNumeroDomandaRichiesto" ControlToValidate="txtInsertNumeroDomanda"
                                ErrorMessage="Inserimento: Inserire un  un Numero Domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCBypassControlliInsert" />
                        </td>
                        <td class="radioButton" style="width: 3%">
                            <asp:RadioButton runat="server" ID="radioInsertPensione" CssClass="radioButton" AutoPostBack="true"
                                CausesValidation="true" OnCheckedChanged="radioInsert_CheckedChanged" onclick="findScrollPosition();"
                                GroupName="rdb_SuPannelloInsert" />
                        </td>
                        <td class="Row1" style="text-align: left; width: 17%">
                            <label class="section-table__label">
                                Chiave Pensione:</label>
                        </td>
                        <td class="field overwrite-flex-row" style="width: 30%; text-align: left">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtInsertCodCategoria" Width="27px"
                                CssClass="txtUppercase tb8" MaxLength="3" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revInsertCategoriaNonValida" ControlToValidate="txtInsertCodCategoria"
                                ErrorMessage="Inserimento: Categoria pensione non  valida" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                            <asp:RequiredFieldValidator runat="server" ID="rfvInsertCategoriaRichiesta" ControlToValidate="txtInsertCodCategoria"
                                ErrorMessage="Inserimento: Inserire una categoria pensione" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCBypassControlliInsert" />
                            <asp:RegularExpressionValidator ID="revInsertLunghezzaCategoria" ControlToValidate="txtInsertCodCategoria"
                                ErrorMessage="Inserimento: Categoria pensione deve essere lunga 3" ValidationExpression="^[0-9]{3}$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                            -
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtInsertCodiceSede" Width="42px"
                                CssClass="txtUppercase tb8 " MaxLength="4" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revInsertCodiceSedeNonValido" ControlToValidate="txtInsertCodiceSede"
                                ErrorMessage="Inserimento: Codice sede pensione non  valida" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                            <asp:RequiredFieldValidator runat="server" ID="rfvInsertCodiceSedeRichiesto" ControlToValidate="txtInsertCodiceSede"
                                ErrorMessage="Inserimento: Inserire un codice sede pensione" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCBypassControlliInsert" />
                            <asp:RegularExpressionValidator ID="revInsertLunghezzaCodiceSede" ControlToValidate="txtInsertCodiceSede"
                                ErrorMessage="Inserimento: Codice sede pensione deve essere lungo 4" ValidationExpression="^[0-9]{4}$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                            -
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtInsertNCertificato" Width="81px"
                                CssClass="txtUppercase tb8 " MaxLength="8" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="revInsertNCertificatNonValido" ControlToValidate="txtInsertNCertificato"
                                ErrorMessage="Inserimento: Certificato pensione non valido" ValidationExpression="^[0-9]*$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                            <asp:RequiredFieldValidator runat="server" ID="refInsertNCertificatoRichiesto" ControlToValidate="txtInsertNCertificato"
                                ErrorMessage="Inserimento: Inserire un certificato di pensione" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCBypassControlliInsert" />
                            <asp:RegularExpressionValidator ID="revInsertLunghezzaNCertificato" ControlToValidate="txtInsertNCertificato"
                                ErrorMessage="Inserimento: Certificato pensione deve essere lungo 8" ValidationExpression="^[0-9]{8}$"
                                runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%" class="overwrite-block">
                        </td>
                        <td class="Row1" style="text-align: left; width: 17%">
                            <label class="section-table__label">
                                Matricola:</label>
                        </td>
                        <td class="field" style="text-align: left; width: 24%" colspan="4">
                            <asp:Label runat="server" ID="lblMatricolaInsert"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%" class="overwrite-block">
                        </td>
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Tipo Bypass:
                            </label>
                        </td>
                        <td class="field" style="width: 24%" colspan="4">
                            <asp:DropDownList runat="server" ID="ddlBypassInsert" CssClass="tb8 txtUppercase"
                                onChange="findScrollPosition();" Width="91.2%" OnSelectedIndexChanged="ddlBypassInsert_OnSelectedIndexChanged"
                                AutoPostBack="true" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatortxtBypass" ControlToValidate="ddlBypassInsert"
                                Enabled="true" ErrorMessage="Inserimento: Inserire un tipo di Bypass" Text="*" CssClass="field-is-required"
                                Display="Dynamic" ValidationGroup="UCBypassControlliInsert" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%" class="overwrite-block">
                        </td>
                        <td class="Row1" style="vertical-align: top; width: 17%">
                            <label class="section-table__label">
                                Descrizione Bypass:
                            </label>
                        </td>
                        <td class="field" colspan="4" style="width: 24%;">
                            <asp:Label runat="server" ID="lblDescrizioneBypass" Text="Nessun Bypass selezionato"
                                Style="width: 95% !important; display: block;"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%" class="overwrite-block">
                        </td>
                        <td class="Row1" style="width: 17%">
                            <label class="section-table__label">
                                Note:</label>
                        </td>
                        <td class="field" style="width: 24%" colspan="4">
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtNote" runat="server" Width="90.2%"
                                TextMode="MultiLine" Rows="5">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="revTxtNote" runat="server" ControlToValidate="txtNote"
                                ErrorMessage="Inserimento: E' possibile inserire massimo 1000 caratteri." SetFocusOnError="true"
                                ValidationExpression="[\s\S]{0,1000}" ValidationGroup="UCBypassControlliInsert"
                                Text="*" CssClass="field-is-required" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <div class="flex-group flex-group-reverse flex-group-right">
                                <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                                    OnClick="btnSalva_Click" OnClientClick="if(validatePageInsert()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
                                <asp:Button ID="btnAnnulla" runat="server" Text="Annulla" SkinID="btnAzione1" CausesValidation="false"
                                    OnClientClick="CleanFields(); return false;" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
<div id="divdialog" title="Nota" style="display: none; border-style: none; border-color: White;">
    <div id="textDialog">
    </div>
</div>
<asp:HiddenField runat="server" ID="hdnTextDialog" />
<asp:HiddenField runat="server" ID="scrollX" />
<asp:HiddenField runat="server" ID="scrollY" />
