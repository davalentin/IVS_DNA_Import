<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCGestioneAziendeEditorialiPerTipo0171.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeEditorialiPerTipo0171.UCGestioneAziendeEditorialiPerTipo0171" %>

<script type="text/javascript">
    // La funzione standard indexOf degli array non funziona su IE8, per questo è stata realizzata questa funzione
    function indexOf(array, obj) {
        for (var i = 0; i < array.length; i++) {
            if (array[i] === obj) { return i; }
        }
        return -1;
    }

    function HideAutoCompleteHack() {
        $(".ui-autocomplete").hide();
    }

    $(document).ready(function () {
        $("body").click(function () {
            HideAutoCompleteHack();
        });

        //Codice
        var availableTagsCodice = document.getElementById("<%=HiddenFieldCodice.ClientID%>").value.split(';');
        $("#<%=txtFiltroCodice.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsCodice,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function (event, ui) {
                var n = indexOf(availableTagsCodice, ui.item.value);
                $("#<%=txtFiltroCodice.ClientID%>").autocomplete("widget").attr('title', availableTagsCodice[n]);
            }
        });

        //DenominazioneAzienda
        var availableTagsDenominazioneAzienda = document.getElementById("<%=HiddenFieldDenominazioneAzienda.ClientID%>").value.split(';');
        $("#<%=txtFiltroDenominazioneAzienda.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsDenominazioneAzienda,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function (event, ui) {
                var n = indexOf(availableTagsDenominazioneAzienda, ui.item.value);
                $("#<%=txtFiltroDenominazioneAzienda.ClientID%>").autocomplete("widget").attr('title', availableTagsDenominazioneAzienda[n]);
            }
        });

        var grid = document.getElementById("<%=gvAnagraficaAccordi.ClientID%>");
        var page = ("<%=gvAnagraficaAccordi.PageIndex%>");
        var zeroLength = "0";
        var length;
        if (grid.rows.length > 0) {
            if (page > 0) {
                if (grid.rows.length - 1 < 10)
                    length = zeroLength.concat(grid.rows.length - 1);
                else
                    length = grid.rows.length - 1;
            }
            else {
                if (grid.rows.length < 10)
                    length = zeroLength.concat(grid.rows.length);
                else
                    length = grid.rows.length;
            }

            if (document.getElementById("ctl00_ContentPlaceHolder1_ucGestAzEditoriali_gvAnagraficaAccordi_ctl" + length + "_txtDenominazioneAziendaCode") != null) {
                $("#ctl00_ContentPlaceHolder1_ucGestAzEditoriali_gvAnagraficaAccordi_ctl" + length + "_txtDenominazioneAziendaCode").autocomplete({
                    minLength: 0,
                    source: availableTagsDenominazioneAzienda,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    },
                    focus: function (event, ui) {
                        var n = indexOf(availableTagsDenominazioneAzienda, ui.item.value);
                        $("#ctl00_ContentPlaceHolder1_ucGestAzEditoriali_gvAnagraficaAccordi_ctl" + length + "_txtDenominazioneAziendaCode").autocomplete("widget").attr('title', availableTagsDenominazioneAzienda[n]);
                    }
                });
            }
        }
    });
</script>

<table class="tabellaFormattazione">
    <!--filtro ricerca-->
    <tr>
        <td style="width: 720px" class="pb-24">
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" CssClass="form-container background-light-blue">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Codice:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroCodice"
                                Width="100px" MaxLength="4" />
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Abilitata:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:DropDownList runat="server" ID="ddlFiltroAbilitata" CssClass="tb8 txtUppercase xxs"
                                Width="50px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Denominazione Azienda:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroDenominazioneAzienda" CssClass="tb8 txtUppercase"
                                Width="200px" MaxLength="200" />
                        </td>
                        <td class="Row1">
                            <label>
                                Sottogruppo Oneri:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroSottoGruppoOneri"
                                Width="100px" MaxLength="4" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Ultima data accordi da:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroDataAccordiDa" CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA"
                                MaxLength="10" />
                        </td>
                        <td class="Row1">
                            <label>
                                Ultima data accordi a:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA" ID="txtFiltroDataAccordiA"
                                MaxLength="10" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <div class="flex-group flex-group-reverse flex-group-right">
                                <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="BlockUI();" CssClass="primary mr-0" />
                                <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <!-- fine filtro ricerca-->
    <!--- griglia anagrafiche accordi-->
    <tr>
        <td class="pb-24">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Anagrafiche Accordi</label>
            <center>
                <asp:GridView runat="server" ID="gvAnagraficaAccordi" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination no-border" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAnagraficaAccordi_RowEditing" Width="1050px" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAnagraficaAccordi_RowCommand" OnRowCancelingEdit="gvAnagraficaAccordi_RowCancelingEdit"
                    OnRowDataBound="gvAnagraficaAccordi_RowDataBound" OnPageIndexChanging="gvAnagraficaAccordi_onPageIndexChanging"
                    OnRowDeleting="gvAnagraficaAccordi_onRowDeleting" PagerSettings-Mode="NumericFirstLast" RowStyle-HorizontalAlign="Center"
                    PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Abilitata" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAbilitata" Text='<%# Bind("AbilitataTxt")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlAbilitata" CssClass="tb8 txtUppercase xxs"
                                    Width="50px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Codice" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodice" Text='<%#Bind("Codice")%>' CssClass="txtUppercase"> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" ID="txtCodice" Text='<%#Bind("Codice")%>'
                                    runat="server" Width="95px" MaxLength="4">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regularTxtCodice" ControlToValidate="txtCodice"
                                    Display="Dynamic" ErrorMessage="Inserire il Codice in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaAccordi" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Denominazione Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="23%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDenominazioneAziendaCode" Text='<%#Bind("DenominazioneAzienda")%>' CssClass="txtUppercase"> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" ID="txtDenominazioneAziendaCode" runat="server"
                                    MaxLength="200" Text=' <%# Bind("DenominazioneAzienda")%>' Width="120px">
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Data Accordi" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataAccordi" Text='<%# Bind("DataAccordi", "{0:dd/MM/yyyy}")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                    ID="txtDataAccordi" MaxLength="10" Text='<%# Bind("DataAccordi", "{0:dd/MM/yyyy}")%>'>
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="validateDataAccordi" ControlToValidate="txtDataAccordi"
                                    Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                    ValidationGroup="GrigliaAccordi" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtDataAccordi" Display="Dynamic"
                                    ErrorMessage="La Data Accordi inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaAccordi"
                                    ID="customCheckDataInizioEsodo" ClientValidationFunction="checkCorrettezzaData" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Domande Liquidabili" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDomandeLiquidabili" Text='<%# Bind("DomandeLiquidabili")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtDomandeLiquidabili" MaxLength="7"
                                    Text=' <%# Bind("DomandeLiquidabili")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtDomandeLiquidabili" ControlToValidate="txtDomandeLiquidabili"
                                    Display="Dynamic" ErrorMessage="Inserire Domande Liquidabili in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaAccordi" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Domande Liquidate" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="11%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDomandeLiquidate" Text='<%# Bind("DomandeLiquidate")%>' CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtDomandeLiquidate" MaxLength="7"
                                    Text=' <%# Bind("DomandeLiquidate")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtDomandeLiquidate" ControlToValidate="txtDomandeLiquidate"
                                    Display="Dynamic" ErrorMessage="Inserire Domande Liquidate in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaAccordi" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="5%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                    OnClientClick="BlockUI();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
    <!--fine griglia anagrafiche oneri-->
    <!--griglia aziende--->
    <tr>
        <td class="pb-24">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Anagrafiche Aziende</label>
            <center>
                <asp:GridView runat="server" ID="gvAnagraficaAziende" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination no-border" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAnagraficaAziende_RowEditing" Width="1050px" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAnagraficaAziende_RowCommand" OnRowCancelingEdit="gvAnagraficaAziende_RowCancelingEdit"
                    OnRowDataBound="gvAnagraficaAziende_RowDataBound" OnPageIndexChanging="gvAnagraficaAziende_onPageIndexChanging"
                    OnRowDeleting="gvAnagraficaAziende_onRowDeleting" PagerSettings-Mode="NumericFirstLast" RowStyle-HorizontalAlign="Center"
                    PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Denominazione Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="48%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDenominazioneAzienda" Text='<%#Bind("DenominazioneAzienda")%>' CssClass="txtUppercase"> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtDenominazioneAzienda" MaxLength="200" Text='<%#Bind("DenominazioneAzienda")%>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sottogruppo primo onere" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSottogruppoPrimoOnere" Text='<%#Bind("SottogruppoPrimoOnere")%>' CssClass="txtUppercase"> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtSottogruppoPrimoOnere" MaxLength="4" Width="100px" Text='<%#Bind("SottogruppoPrimoOnere")%>'>
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtSottogruppoPrimoOnere" ControlToValidate="txtSottogruppoPrimoOnere"
                                    Display="Dynamic" ErrorMessage="Inserire il sottogruppo in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaAziende" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sottogruppo secondo onere" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSottogruppoSecondoOnere" Text='<%#Bind("SottogruppoSecondoOnere")%>' CssClass="txtUppercase"> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtSottogruppoSecondoOnere" MaxLength="4" Width="100px" Text='<%#Bind("SottogruppoSecondoOnere")%>'>
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtSottogruppoSecondoOnere" ControlToValidate="txtSottogruppoSecondoOnere"
                                    Display="Dynamic" ErrorMessage="Inserire il sottogruppo in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]*$" ValidationGroup="GrigliaAziende" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="5%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnElimina" CommandName="Elimina" CommandArgument="Elimina" runat="server"
                                    OnClientClick="BlockUI();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
    <!--fine griglia aziende-->
</table>
<asp:HiddenField ID="HiddenFieldCodice" runat="server" />
<asp:HiddenField ID="HiddenFieldDenominazioneAzienda" runat="server" />
