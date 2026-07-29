<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCVisualizzaStatoPratiche.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.VisualizzaStatoPratiche.UCVisualizzaStatoPratiche" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">
    function CreatePopUp() {
        // jQuery UI Dialog  
        var sedeDomanda = document.getElementById('<%=HdnSedeDomanda.ClientID %>').value;
        $('#changeSedeOperatore').text("La sede della domanda è " + sedeDomanda + ". Cambiare sede per proseguire?");
        var result;
        $('#changeSedeOperatore').dialog(
        {
            autoOpen: false,
            width: 400,
            modal: true,
            resizable: false,
            draggable: false,

            buttons:
            {
                "Annulla": function () {
                    $(this).dialog("close");
                    result = false;
                },
                "Conferma": function () {
                    $(this).dialog("close");
                    document.getElementById('<%= btnConfermaPopUp.ClientID %>').click();
                }
            }
        });
        $("#changeSedeOperatore").parent().appendTo($("form:first"));
    }

    function ShowPopUp(nDomus, progStorico, sedeDomanda, tipoAppartenenza, stato) {
        var sedeOperatore = document.getElementById('<%=HdnSedeOperatore.ClientID %>');

        document.getElementById('<%=HdnNdomusSelezionato.ClientID %>').value = nDomus;
        document.getElementById('<%=HdnProgStorico.ClientID %>').value = progStorico;
        document.getElementById('<%=HdnSedeDomanda.ClientID %>').value = sedeDomanda;
        document.getElementById('<%=HdnTipoAppartenenza.ClientID %>').value = tipoAppartenenza;
        document.getElementById('<%=HdnStatoSelezionato.ClientID %>').value = stato;

        if (sedeOperatore != null && sedeDomanda == sedeOperatore.value)
            document.getElementById('<%= btnConfermaPopUp.ClientID %>').click();
        else {
            CreatePopUp();
            $('#changeSedeOperatore').dialog('open');
        }
    }

</script>
<asp:Panel ID="pnlRisultatoVisualizzaStatoPratiche" runat="server">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />

    <div class="page-title" style="display: none">
        <h2 class="page-title-secondlevel">Ricerca ricerca</h2>
        <h6 class="page-subtitle">Visualizza le pratiche che corrispondono ai criteri di ricerca<</h6>
    </div>

    <table class="tabellaFormattazione is-contents">
        <tr>
            <td class="titolo overwrite-flex-col full-width text-caption" style="padding-bottom: 10px;">
                <div class="force-block num-criteri" style="display: contents">
                    <label class="text-caption--main-bold">
                        Criteri selezionati:</label>
                    <asp:Label runat="server" ID="lblNCriteriSelezionati" CssClass="text-caption--main"></asp:Label>
                </div>
                <br />
                <hr />
                <div runat="server" id="divParametriNumeroDomanda" visible="false" class="divCriteriStatoPratiche text-caption--tab">
                    <label class="text-caption--bold">
                        Domanda</label>
                    <asp:Label runat="server" ID="lblParametriNumeroDomanda" CssClass="Row1 text-caption--regular" Font-Bold="false"></asp:Label>
                </div>
                <div runat="server" id="divParametriCategoriaPensione" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Categoria
                    <asp:Label runat="server" ID="lblParametriCategoriaPensione" Font-Bold="false" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriStatoPratica" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Stato Pratica
                    <asp:Label runat="server" ID="lblParametriStatoPratica" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriSede" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Sede
                    <asp:Label runat="server" ID="lblParametriSede" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriFondo" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Fondo
                    <asp:Label runat="server" ID="lblParametriFondo" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriCassa" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Cassa
                    <asp:Label runat="server" ID="lblParametriCassa" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriAnagrafica" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Cognome
                    <asp:Label runat="server" ID="lblParametriCognome" CssClass="text-caption--regular"></asp:Label>
                    &nbsp; Nome
                    <asp:Label runat="server" ID="lblParametriNome" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriCodiceFiscale" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Codice Fiscale
                    <asp:Label runat="server" ID="lblParametriCodiceFiscale" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriDataPresentazione" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Data Presentazione dal
                    <asp:Label runat="server" ID="lblParametriDataPresentazioneDal" CssClass="text-caption--regular"></asp:Label>
                    al
                    <asp:Label runat="server" ID="lblParametriDataPresentazioneAl" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriDataElaborazione" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Data Elaborazione dal
                    <asp:Label runat="server" ID="lblParametriDataElaborazioneDal" CssClass="text-caption--regular"></asp:Label>
                    al
                    <asp:Label runat="server" ID="lblParametriDataElaborazioneAl" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriMatricola" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Matricola
                    <asp:Label runat="server" ID="lblParametriMatricola" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriTipoDomandaInLavorazione" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    <asp:Label runat="server" ID="lblParametriTipoDomandaInLavorazione" CssClass="text-caption--regular"></asp:Label>
                    in lavorazione
                </div>
                <div runat="server" id="divParametriTipoDomandaLavorata" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    <asp:Label runat="server" ID="lblParametriTipoDomandaLavorata" CssClass="text-caption--regular"></asp:Label>
                    lavorate
                </div>
                <div runat="server" id="divParametriGruppo" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Gruppo
                    <asp:Label runat="server" ID="lblParametriGruppo" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriProdotto" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Prodotto
                    <asp:Label runat="server" ID="lblParametriProdotto" CssClass="text-caption--regular"></asp:Label>
                </div>
                <div runat="server" id="divParametriTipo" visible="false" class="divCriteriStatoPratiche text-caption--bold text-caption--tab">
                    Tipo
                    <asp:Label runat="server" ID="lblParametriTipo" CssClass="text-caption--regular"></asp:Label>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <table class="tabellaContenuti is-contents full-width" style="width: 100%; border-style: solid; border-width: thin;
                    border-color: Black; padding-top: 10px; padding-bottom: 20px; padding-left: 20px;
                    padding-right: 20px;">
                    <tr>
                        <td class="Row1 overwrite-block text-caption" align="left">
                            <label style="text-align: left; font-weight: bold;" class="text-caption--bold">
                                Pratiche Trovate:</label>
                            <asp:Label ID="lblNPraticheTrovate" runat="server" Font-Bold="true" CssClass="text-caption--regular"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="gvPratiche" runat="server" BorderWidth="1" BorderColor="Black"
                                AutoGenerateColumns="false" AllowSorting="true" Visible="true" Width="100% "
                                SkinID="grdElenco1" AllowPaging="true" OnPageIndexChanging="GvPratiche_onPageIndexChanging"
                                PageSize="10" OnRowCommand="GvPratiche_onRowCommand" OnSorting="GvPratiche_onSorting"
                                OnRowCreated="GvPratiche_RowCreated" PagerStyle-Font-Size="20px" PagerSettings-Mode="NumericFirstLast"
                                CssClass="data-table data-table--sorting data-table--scrollable" PagerStyle-CssClass="default-pagination-tables">
                                <EmptyDataTemplate>
                                    <center>
                                        <asp:Label ID="lblNoData" runat="server" Text="Nessuna posizione trovata per i criteri inseriti."
                                            SkinID="lblNoData" Visible="true"></asp:Label>
                                    </center>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Numero Domanda" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="NumeroDomanda">
                                        <ItemTemplate>
                                            <asp:LinkButton class="data-table_link" runat="server" Text='<%#Eval("NumeroDomanda")%>' ID="Domanda" OnClientClick='<%#String.Format("ShowPopUp({0},{1},{2},{3},{4}); return false;", Eval("NumeroDomanda"), "\"" + Eval("ProgStorico") + "\"", "\"" + Eval("Sede") + "" + Eval("CentroOperativo")  + "\"", "\"" + Eval("TipoAppartenenza") + "\"","\"" + Eval("Stato") + "\"")%>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prodotto" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="26%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3 txtUppercase" SortExpression="DescProdotto">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="DescProdotto" Text='<%#Eval("DescProdotto")%>' ToolTip='<%#Eval("DescTipo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Cognome" DataField="Cognome" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="15%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Cognome" />
                                    <asp:BoundField HeaderText="Nome" DataField="Nome" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="13%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Nome" />
                                    <asp:BoundField HeaderText="Categoria" DataField="Categoria" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="12%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Categoria" />
                                    <asp:TemplateField HeaderText="Sede" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="10%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="SedeCO">
                                        <ItemTemplate>
                                            <asp:Label ID="gvLblSede" runat="server" Text='<%# GetSedeForView(((GridViewRow) Container).DataItem)%>'
                                                Visible="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Certificato" DataField="Certificato" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Certificato" />
                                    <asp:BoundField HeaderText="Tipo" DataField="Tipo" Visible="true" ItemStyle-CssClass="TblRecordset3"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-Width="13%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        SortExpression="Tipo" />
                                    <asp:BoundField HeaderText="Stato" DataField="Stato" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="13%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink data-table__heading data-table__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Stato" />
                                    <asp:TemplateField HeaderText="TE08" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                                        HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                        Visible="false" ShowHeader="false">
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" ID="btnStampaPratica" ImageUrl='<%# GetButtonImage("pdficon.png") %>'
                                                Height="24px" Visible="<%# IsVisibleStampa(((GridViewRow) Container)) %>" OnClientClick='<%#String.Format("OpenNewPage(&#039;ElaborazionePosizione/Stampa.aspx?NumDomanda={0}{1}&#039;{2}", Eval("NumeroDomanda"), Eval("ProgStorico") != null ? "&ProgStorico=" + Eval("ProgStorico") : "", "); return false;")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false" ShowHeader="false" ItemStyle-CssClass="action" HeaderStyle-CssClass="action">
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" ID="btnEliminaPratica" CommandName="EliminaPratica"
                                                ImageUrl='<%# GetButtonImage("delete24.png") %>' CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                OnClientClick="if (!window.confirm('Sei sicuro di voler cancellare questa riga?')) return false; else BlockUI();"
                                                Visible="<%# IsVisibleDelete(((GridViewRow) Container), false, true) %>" CssClass="trashIconOnly icon-button" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--                                    <asp:ButtonField ButtonType="Image" CommandName="EliminaPratica"     />                                     --%>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="text-align: center;">
                <asp:Button ID="Back" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                    OnClick="btnBack_Click" OnClientClick="BlockUI()" />
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="HdnSedeOperatore" />
    <asp:HiddenField runat="server" ID="HdnNdomusSelezionato" />
    <asp:HiddenField runat="server" ID="HdnProgStorico" />
    <asp:HiddenField runat="server" ID="HdnSedeDomanda" />
    <asp:HiddenField runat="server" ID="HdnTipoAppartenenza" />
    <asp:HiddenField runat="server" ID="HdnStatoSelezionato" />
    <div id="changeSedeOperatore" title="Cambia sede" style="display: none;">
        <p>
        </p>
    </div>
    <asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server"
        OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />
</asp:Panel>
