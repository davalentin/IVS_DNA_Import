<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAziendeVESO33.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeVESO33.UCAziendeVESO33" %>
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

        //DenominazioneAzienda
        var availableTagsDescrizione = document.getElementById("<%=HiddenFieldDescrizione.ClientID%>").value.split(';');
        $("#<%=txtFiltroDescrizione.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsDescrizione,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            },
            focus: function (event, ui) {
                var n = indexOf(availableTagsDescrizione, ui.item.value);
                $("#<%=txtFiltroDescrizione.ClientID%>").autocomplete("widget").attr('title', availableTagsDescrizione[n]);
            }
        });
    });
</script>
<table class="tabellaFormattazione">
    <!--filtro ricerca-->
    <tr>
        <td>
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; margin-left: 0px" CssClass="form-container background-light-blue">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 20%">
                            <label>
                                Codice Azienda:</label>
                        </td>
                        <td class="field" style="width: 20%">
                            <asp:TextBox runat="server" CssClass="txtUppercase tb8" ID="txtFiltroCodiceAzienda" Width="100px" MaxLength="4" />
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Ultima Decorrenza Ammessa Da:</label>
                        </td>
                        <td class="field" style="width: 30%">
                            <asp:TextBox runat="server" ID="txtUltimaDecorrenzaAmmessaDa" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" Width="100px" MaxLength="10" />
                            <asp:RegularExpressionValidator runat="server" ID="validateDataUltimaDecorrenzaAmmessaDa" ControlToValidate="txtUltimaDecorrenzaAmmessaDa"
                                Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeVESO33" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtUltimaDecorrenzaAmmessaDa" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaAziendeVESO33"
                                ID="customCheckDataUltimaDecorrenzaAmmessaDa" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Descrizione:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroDescrizione" CssClass="txtUppercase tb8"
                                Width="200px" Enabled="false" MaxLength="200" />
                        </td>
                        <td class="Row1">
                            <label>
                                Ultima Decorrenza Ammessa A:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtUltimaDecorrenzaAmmessaA" CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" Width="100px" MaxLength="10" />
                            <asp:RegularExpressionValidator runat="server" ID="validateDataUltimaDecorrenzaAmmessaA" ControlToValidate="txtUltimaDecorrenzaAmmessaA"
                                Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeVESO33" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtUltimaDecorrenzaAmmessaA" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaAziendeVESO33"
                                ID="customCheckDataUltimaDecorrenzaAmmessaA" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="end">
                            <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();" />
                            <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="BlockUI();"  CssClass="primary mr-0"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <!-- fine filtro ricerca-->
    <tr>
        <td>
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Aziende VESO33</label>
            <center>
                <asp:GridView runat="server" ID="gvAziendeVESO33" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAziendeVESO33_RowEditing" Width="1000px" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAziendeVESO33_RowCommand" OnRowCancelingEdit="gvAziendeVESO33_RowCancelingEdit"
                    OnRowDataBound="gvAziendeVESO33_RowDataBound" OnPageIndexChanging="gvAziendeVESO33_onPageIndexChanging"
                    OnRowDeleting="gvAziendeVESO33_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                    <Columns>
                        <asp:TemplateField HeaderText="Codice Azienda" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodiceAzienda" Text='<%# Bind("CodiceAziendaTraduzioneSuGP")%>'
                                    CssClass="txtUppercase">      
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtCodiceAzienda" MaxLength="4"
                                    Text=' <%# Bind("CodiceAziendaTraduzioneSuGP")%>' Width="50px">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="regulartxtCodiceAzienda" ControlToValidate="txtCodiceAzienda"
                                    Display="Dynamic" ErrorMessage="Inserire il Codice Azienda in un formato valido (numerico)"
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaAziendeVESO33" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrizione" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="58%">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescrizione" Text='<%#Bind("Descrizione")%>'> 
                                </asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtDescrizione" Width= "95%" MaxLength="150" Text='<%#Bind("Descrizione")%>'>
                                </asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ultima Decorrenza Ammessa" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="17%">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDataUltimaDecorrenzaIVS" Text='<%# Bind("UltimaDecorrenzaAmmessa", "{0:dd/MM/yyyy}")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                ID="txtDataUltimaDecorrenzaIVS" MaxLength="10" Text='<%# Bind("UltimaDecorrenzaAmmessa", "{0:dd/MM/yyyy}")%>'>
                            </asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateDataUltimaDecorrenza" ControlToValidate="txtDataUltimaDecorrenzaIVS"
                                Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeVESO33" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataUltimaDecorrenzaIVS" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="GrigliaAziendeVESO33"
                                ID="customCheckDataUltimaDecorrenzaVESO33" ClientValidationFunction="checkCorrettezzaData" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                            ItemStyle-Width="4%">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="Elimina" runat="server"
                                    OnClientClick="BlockUI();" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </center>
        </td>
    </tr>
</table>
<asp:HiddenField ID="HiddenFieldDescrizione" runat="server" />