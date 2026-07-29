<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCTipologieNonAbilitate.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.TipologieNonAbilitate.UCTipologieNonAbilitate" %>

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

    $(document).ready(function() {
        $("body").click(function() {
            HideAutoCompleteHack();
        });

        // Fondo
        var availableTagsFondo = document.getElementById("<%=HiddenFieldFondo.ClientID%>").value.split(';');
        $("#<%=txtFiltroFondo.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsFondo,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        // Gruppo
        var availableTagsGruppo = document.getElementById("<%=HiddenFieldGruppo.ClientID%>").value.split(';');
        var availableTagsDescGruppo = document.getElementById("<%=HiddenFieldDescGruppo.ClientID%>").value.split(';');
        $("#<%=txtFiltroGruppo.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsGruppo,,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function(event, ui) {
                var n = indexOf(availableTagsGruppo, ui.item.value);
                $("#<%=txtFiltroGruppo.ClientID%>").autocomplete("widget").attr('title', availableTagsDescGruppo[n]);
            }
        });


        // Prodotto
        var availableTagsProdotto = document.getElementById("<%=HiddenFieldProdotto.ClientID%>").value.split(';');
        var availableTagsDescProdotto = document.getElementById("<%=HiddenFieldDescProdotto.ClientID%>").value.split(';');
        $("#<%=txtFiltroProdotto.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsProdotto,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function(event, ui) {
                var n = indexOf(availableTagsProdotto, ui.item.value);
                $("#<%=txtFiltroProdotto.ClientID%>").autocomplete("widget").attr('title', availableTagsDescProdotto[n]);
            }
        });


        // Tipo
        var availableTagsTipo = document.getElementById("<%=HiddenFieldTipo.ClientID%>").value.split(';');
        var availableTagsDescTipo = document.getElementById("<%=HiddenFieldDescTipo.ClientID%>").value.split(';');
        $("#<%=txtFiltroTipo.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsTipo,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function(event, ui) {
                var n = indexOf(availableTagsTipo, ui.item.value);
                $("#<%=txtFiltroTipo.ClientID%>").autocomplete("widget").attr('title', availableTagsDescTipo[n]);
            }
        });


        // Filtro
        var availableTagsFiltro = document.getElementById("<%=HiddenFieldFiltro.ClientID%>").value.split(';');
        var availableTagsDescFiltro = document.getElementById("<%=HiddenFieldDescFiltro.ClientID%>").value.split(';');
        $("#<%=txtFiltroFiltro.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsFiltro,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function(event, ui) {
                var n = indexOf(availableTagsFiltro, ui.item.value);
                $("#<%=txtFiltroFiltro.ClientID%>").autocomplete("widget").attr('title', availableTagsDescFiltro[n]);
            }
        });

        // Filtro
        var availableTagsSiglaCategoria = document.getElementById("<%=HiddenFieldSiglaCategoria.ClientID%>").value.split(';');
        $("#<%=txtFiltroSiglaCategoria.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsSiglaCategoria,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });

        var grid = document.getElementById("<%=gvTipologieNonAbilitate.ClientID%>");
        var page = ("<%=gvTipologieNonAbilitate.PageIndex%>");
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
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtFondo") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtFondo").autocomplete({
                    minLength: 0,
                    source: availableTagsFondo,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    }
                });
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtGruppo") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtGruppo").autocomplete({
                    minLength: 0,
                    source: availableTagsGruppo,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    },
                    focus: function(event, ui) {
                        var n = indexOf(availableTagsGruppo, ui.item.value);
                        $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtGruppo").autocomplete("widget").attr('title', availableTagsDescGruppo[n]);
                    }
                });
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtProdotto") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtProdotto").autocomplete({
                    minLength: 0,
                    source: availableTagsProdotto,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    },
                    focus: function(event, ui) {
                        var n = indexOf(availableTagsProdotto, ui.item.value);
                        $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtProdotto").autocomplete("widget").attr('title', availableTagsDescProdotto[n]);
                    }
                });
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtTipo") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtTipo").autocomplete({
                    minLength: 0,
                    source: availableTagsTipo,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    },
                    focus: function(event, ui) {
                        var n = indexOf(availableTagsTipo, ui.item.value);
                        $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtTipo").autocomplete("widget").attr('title', availableTagsDescTipo[n]);
                    }
                });
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtFiltro") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtFiltro").autocomplete({
                    minLength: 0,
                    source: availableTagsFiltro,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    },
                    focus: function(event, ui) {
                        var n = indexOf(availableTagsFiltro, ui.item.value);
                        $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtFiltro").autocomplete("widget").attr('title', availableTagsDescFiltro[n]);
                    }
                });
            }
            if (document.getElementById("ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtSiglaCategoria") != null) {
                $("#ctl00_ContentPlaceHolder1_ucTipologieNonAbilitate_gvTipologieNonAbilitate_ctl" + length + "_txtSiglaCategoria").autocomplete({
                    minLength: 0,
                    source: availableTagsSiglaCategoria,
                    open: function () {
                        $(this)
                            .autocomplete("widget")
                            .css({
                                "margin-top": "8px",
                                "width": $(this).outerWidth() + "px"
                            })
                    }
                });
            }
        }
    });
</script>

<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px" class="full-width pb-24">
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" class="full-width form-container background-light-blue">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1">
                            <label>
                                Tipo Appartenenza:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroTipoAppartenenza"
                                Width="150px" Enabled="false"/>
                        </td>
                        <asp:Panel ID="pnlFondo" runat="server" Visible="false" >
                            <td class="Row1">
                                <label>
                                    Fondo:</label>
                            </td>
                            <td class="field">
                                <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroFondo" Width="150px" MaxLength="3"/>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltroFondo" ControlToValidate="txtFiltroFondo"
                                            ErrorMessage="Il Fondo non può contenere numeri e caratteri speciali" ValidationExpression="^[A-Za-z]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                            </td>
                        </asp:Panel>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Gruppo:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroGruppo" CssClass="tb8 txtUppercase" Width="150px" Enabled="false" MaxLength="4" 
                                onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltroGruppo" ControlToValidate="txtFiltroGruppo"
                                            ErrorMessage="Il Gruppo non può contenere lettere e caratteri speciali" ValidationExpression="^[0-9]{4}$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                        </td>
                        <td class="Row1">
                            <label>
                                Prodotto:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroProdotto" CssClass="tb8 txtUppercase" Width="150px" MaxLength="4"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltroProdotto" ControlToValidate="txtFiltroProdotto"
                                            ErrorMessage="Il Prodotto non può contenere lettere e caratteri speciali" ValidationExpression="^[0-9]{4}|ALL|all$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Tipo:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroTipo" CssClass="tb8 txtUppercase" Width="150px" MaxLength="4"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltroTipo" ControlToValidate="txtFiltroTipo"
                                            ErrorMessage="Il Tipo non può contenere lettere e caratteri speciali" ValidationExpression="^[0-9]{4}|ALL|all$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                        </td>
                        <td class="Row1">
                            <label>
                                Filtro:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroFiltro" CssClass="tb8 txtUppercase" Width="150px" MaxLength="3"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltroFiltro" ControlToValidate="txtFiltroFiltro"
                                            ErrorMessage="Il Filtro non può contenere caratteri speciali" ValidationExpression="^[0-9A-Za-z]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Sigla Categoria:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroSiglaCategoria" CssClass="tb8 txtUppercase" Width="150px" MaxLength="8"/>
                            <asp:RegularExpressionValidator ID="REV_txtFiltroSiglaCategoria" ControlToValidate="txtFiltroSiglaCategoria"
                                            ErrorMessage="La sigla categoria non può contenere caratteri speciali" ValidationExpression="^[A-Za-z0-9]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateFiltro" Enabled="true" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <div class="flex-group flex-group-reverse flex-group-right">
                                <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="if(validatePageFiltro()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary mr-0"/>
                                <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();"/>
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
                Tipologie Non Abilitate</label>
            <asp:GridView runat="server" ID="gvTipologieNonAbilitate" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella full-width intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                Width="720px" PageSize="10" AllowPaging="true" OnRowCommand="gvTipologieNonAbilitate_RowCommand" OnRowEditing="gvTipologieNonAbilitate_RowEditing"
                OnRowCancelingEdit="gvTipologieNonAbilitate_RowCancelingEdit" OnRowDataBound="gvTipologieNonAbilitate_RowDataBound" 
                OnPageIndexChanging="gvTipologieNonAbilitate_onPageIndexChanging" OnRowDeleting="gvTipologieNonAbilitate_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                <Columns>
                    <asp:TemplateField HeaderText="Tipologia" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipoAppartenenza" Text='<%# Bind("TipoAppartenenza")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtTipoAppartenenza"
                                Text=' <%# Bind("TipoAppartenenza")%>' Width="75px" Enabled="false" ></asp:TextBox> 
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Fondo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblFondo" Text='<%#Bind("Fondo")%>' Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase autotab" ID="txtFondo" runat="server" Width="75px" >
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFondo" ControlToValidate="txtFondo"
                                            ErrorMessage="Il Fondo non può contenere numeri e caratteri speciali" ValidationExpression="^[A-Za-z]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtFondo" ControlToValidate="txtFondo"
                                            Enabled="false" ErrorMessage="Inserire un Fondo" Text="*" CssClass="field-is-required" Display="Dynamic"
                                            ValidationGroup="UCTipologieNonAbilitateGrid" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Gruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblGruppo" Text='<%#Bind("Gruppo")%>' Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtGruppo" runat="server" Width="75px" MaxLength="4" 
                                onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtGruppo" ControlToValidate="txtGruppo"
                                            ErrorMessage="Il Gruppo può contenere solo numeri (max 4 cifre)" ValidationExpression="^[0-9]{4}$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtGruppo" ControlToValidate="txtGruppo"
                                            Enabled="true" ErrorMessage="Inserire un Gruppo" Text="*" CssClass="field-is-required" Display="Dynamic"
                                            ValidationGroup="UCTipologieNonAbilitateGrid" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Prodotto" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblProdotto" Text='<%#Bind("Prodotto")%>'
                                Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtProdotto" runat="server" Width="75px" MaxLength="4">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtProdotto" ControlToValidate="txtProdotto"
                                            ErrorMessage="Il Prodotto può contenere solo numeri (max 4 cifre) oppure ALL" ValidationExpression="^[0-9]{4}|ALL|all$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtProdotto" ControlToValidate="txtProdotto"
                                            Enabled="true" ErrorMessage="Inserire un Prodotto" Text="*" CssClass="field-is-required" Display="Dynamic"
                                            ValidationGroup="UCTipologieNonAbilitateGrid" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipo" Text='<%#Bind("Tipo")%>'
                                Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtTipo" runat="server" Width="75px" MaxLength="4">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtTipo" ControlToValidate="txtTipo"
                                            ErrorMessage="Il Tipo può contenere solo numeri (max 4 cifre) oppure ALL" ValidationExpression="^[0-9]{4}|ALL|all$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTxtTipo" ControlToValidate="txtTipo"
                                            Enabled="true" ErrorMessage="Inserire un Tipo" Text="*" CssClass="field-is-required" Display="Dynamic"
                                            ValidationGroup="UCTipologieNonAbilitateGrid" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Filtro" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblFiltro" Text='<%#Bind("Filtro")%>'
                                Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtFiltro" runat="server" Width="75px" MaxLength="3" >
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtFiltro" ControlToValidate="txtFiltro"
                                            ErrorMessage="Il Filtro non può contenere caratteri speciali" ValidationExpression="^[0-9A-Za-z]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sigla Categoria" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSiglaCategoria" Text='<%#Bind("SiglaCategoria")%>'
                                Width="75px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSiglaCategoria" runat="server" Width="75px" MaxLength="8" >
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidatorTxtSiglaCategoria" ControlToValidate="txtSiglaCategoria"
                                            ErrorMessage="La Sigla Categoria non può contenere caratteri speciali" ValidationExpression="^[0-9A-Za-z]*$" runat="server"
                                            Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTipologieNonAbilitateGrid" Enabled="true" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatortxtSiglaCategoria" ControlToValidate="txtSiglaCategoria"
                                            Enabled="true" ErrorMessage="Inserire la Sigla Categoria" Text="*" CssClass="field-is-required" Display="Dynamic"
                                            ValidationGroup="UCTipologieNonAbilitateGrid" />
                        </EditItemTemplate>
                    </asp:TemplateField>  
                    <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" OnClientClick="BlockUI();"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr> 
</table>

<asp:HiddenField ID="HiddenFieldFondo" runat="server" />
<asp:HiddenField ID="HiddenFieldGruppo" runat="server" />
<asp:HiddenField ID="HiddenFieldProdotto" runat="server" />
<asp:HiddenField ID="HiddenFieldTipo" runat="server" />
<asp:HiddenField ID="HiddenFieldFiltro" runat="server" />
<asp:HiddenField ID="HiddenFieldSiglaCategoria" runat="server" />
<asp:HiddenField ID="HiddenFieldDescGruppo" runat="server" />
<asp:HiddenField ID="HiddenFieldDescProdotto" runat="server" />
<asp:HiddenField ID="HiddenFieldDescTipo" runat="server" />
<asp:HiddenField ID="HiddenFieldDescFiltro" runat="server" />