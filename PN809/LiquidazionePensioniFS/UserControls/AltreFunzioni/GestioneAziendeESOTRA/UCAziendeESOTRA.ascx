<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAziendeESOTRA.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAziendeESOTRA.UCAziendeESOTRA" %>
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
                $("#<%=txtFiltroDescrizione.ClientID%>").autocomplete("widget").width(200)
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
        <td style="width: 720px">
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px">
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
                                Text="*" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeESOTRA" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtUltimaDecorrenzaAmmessaDa" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" ValidationGroup="GrigliaAziendeESOTRA"
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
                                Text="*" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeESOTRA" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtUltimaDecorrenzaAmmessaA" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" ValidationGroup="GrigliaAziendeESOTRA"
                                ID="customCheckDataUltimaDecorrenzaAmmessaA" ClientValidationFunction="checkCorrettezzaData" />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="BlockUI();" />
                            <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();" />
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
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Aziende ESOTRA</label>
            <center>
                <asp:GridView runat="server" ID="gvAziendeESOTRA" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                    OnRowEditing="gvAziendeESOTRA_RowEditing" Width="1000px" PageSize="10" AllowPaging="true"
                    OnRowCommand="gvAziendeESOTRA_RowCommand" OnRowCancelingEdit="gvAziendeESOTRA_RowCancelingEdit"
                    OnRowDataBound="gvAziendeESOTRA_RowDataBound" OnPageIndexChanging="gvAziendeESOTRA_onPageIndexChanging"
                    PagerSettings-Mode="NumericFirstLast">
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
                                    Text="*" ValidationExpression="^[0-9]{4}$" ValidationGroup="GrigliaAziendeESOTRA" />
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
                                Text="*" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaAziendeESOTRA" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataUltimaDecorrenzaIVS" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" ValidationGroup="GrigliaAziendeESOTRA"
                                ID="customCheckDataUltimaDecorrenzaESOTRA" ClientValidationFunction="checkCorrettezzaData" />
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
