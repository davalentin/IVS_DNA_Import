<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRisultatoRicerca.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.RisultatoRicerca.UCRisultatoRicerca" %>
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

    function ShowPopUp(nDomus, progStorico, sedeDomanda, isConsultazioneDomandaTRF, tipoDomanda) {
        
        var sedeOperatore = document.getElementById('<%=HdnSedeOperatore.ClientID %>');

        document.getElementById('<%=HdnNdomusSelezionato.ClientID %>').value = nDomus;
        document.getElementById('<%=HdnSedeDomanda.ClientID %>').value = sedeDomanda;
        document.getElementById('<%=HdnProgStorico.ClientID %>').value = progStorico;
        document.getElementById('<%=HdnIsConsultazioneDomandaTRF.ClientID %>').value = isConsultazioneDomandaTRF;
        document.getElementById('<%=HdnTipoDomanda.ClientID %>').value = tipoDomanda;

        if (sedeOperatore != null && sedeDomanda == sedeOperatore.value)
            document.getElementById('<%= btnConfermaPopUp.ClientID %>').click();
        else {
            CreatePopUp();
            $('#changeSedeOperatore').dialog('open');
        }
    }

</script>
<asp:Panel ID="pnlRisultatoRicercaElaborazione" runat="server">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <table class="tabellaFormattazione">
        <tr>
            <td class="titolo" style="padding-bottom: 10px;">
                <label>
                    Posizioni trovate per:</label>
                <asp:Label runat="server" ID="lblParametriRicerca"></asp:Label>
                <asp:Label runat="server" ID="lblParametriRicerca2"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table class="tabellaContenuti" style="width: 100%; border-style: solid; border-width: thin;
                    border-color: Black; padding-top: 10px; padding-bottom: 20px; padding-left: 20px;
                    padding-right: 20px;">
                    <tr>
                        <td class="Row1" align="left">
                            <label style="text-align: left; font-weight: bold;">
                                Domande Trovate:</label>
                            <asp:Label ID="lblNDomandeTrovate" runat="server" Font-Bold="true"></asp:Label>
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
                            <asp:GridView ID="gvDomande" runat="server" BorderWidth="1" BorderColor="Black" AutoGenerateColumns="false"
                                AllowSorting="true" Visible="true" Width="100% " SkinID="grdElenco1" AllowPaging="true"
                                OnPageIndexChanging="gvDomande_onPageIndexChanging" PageSize="10" OnRowCommand="gvDomande_onRowCommand"
                                OnSorting="gvDomande_onSorting" OnRowCreated="gvDomande_RowCreated" PagerSettings-Mode="NumericFirstLast">
                                <EmptyDataTemplate>
                                    <center>
                                        <asp:Label ID="lblNoData" runat="server" Text="Nessuna posizione trovata per i criteri inseriti."
                                            SkinID="lblNoData" Visible="true"></asp:Label>
                                    </center>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Numero Domanda" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="NumeroDomanda">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" Text='<%#Eval("NumeroDomanda")%>' ID="Domanda" OnClientClick='<%#String.Format("ShowPopUp({0},{1},{2},{3},{4}); return false;", Eval("NumeroDomanda"), "\"" + Eval("ProgStorico") + "\"", "\"" + Eval("Sede") + "" + Eval("CentroOperativo")  + "\"", "\"" + Eval("IsConsultazioneDomandaTRF") + "\"", "\"" + Eval("Tipo") + "\"")%>'></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Prodotto" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="26%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3 txtUppercase" SortExpression="DescProdotto">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="DescProdotto" Text='<%#Eval("DescProdotto")%>' ToolTip='<%#Eval("DescTipo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Categoria" DataField="Categoria" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="21%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Categoria" />
                                    <asp:TemplateField HeaderText="Sede" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="SedeCO">
                                        <ItemTemplate>
                                            <asp:Label ID="gvLblSede" runat="server" Text='<%# GetSedeForView(((GridViewRow) Container).DataItem)%>'
                                                Visible="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Certificato" DataField="Certificato" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="16%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Certificato" />
                                    <asp:BoundField HeaderText="Tipo" DataField="Tipo" Visible="true" ItemStyle-CssClass="TblRecordset3"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-Width="13%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        SortExpression="Tipo" />
                                    <asp:BoundField HeaderText="Stato" DataField="Stato" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="13%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Stato" />
                                    <asp:TemplateField HeaderText="TE08" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                                        HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                        Visible="false" ShowHeader="false">
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" ID="btnStampaPratica" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/pdficon.png"
                                                Height="24px" Visible="<%# IsVisibleStampa(((GridViewRow) Container)) %>" OnClientClick='<%#String.Format("OpenNewPage(&#039;ElaborazionePosizione/Stampa.aspx?NumDomanda={0}{1}&#039;{2}", Eval("NumeroDomanda"), Eval("ProgStorico") != null ? "&ProgStorico=" + Eval("ProgStorico") : "", "); return false;")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table class="tabellaContenuti" style="width: 100%; border-style: solid; border-width: thin;
                    border-color: Black; padding-top: 10px; padding-bottom: 20px; padding-left: 20px;
                    padding-right: 20px;">
                    <tr>
                        <td class="Row1" align="left">
                            <label style="text-align: left; font-weight: bold;">
                                Pensioni Trovate:
                            </label>
                            <asp:Label runat="server" ID="lblNPensioniTrovate" Font-Bold="true"></asp:Label>
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
                            <asp:GridView runat="server" ID="gvPensioni" Width="100%" AutoGenerateColumns="false"
                                Visible="true" SkinID="grdElenco1" BorderWidth="1" BorderColor="Black" OnRowDataBound="PensioniGridView_onRowDataBound"
                                AllowPaging="true" OnPageIndexChanging="gvPensioni_onPageIndexChanging" PageSize="10"
                                AllowSorting="true" OnSorting="gvPensioni_onSorting" OnRowCreated="gvPensioni_RowCreated"
                                PagerSettings-Mode="NumericFirstLast" CssClass="intestazioneTabella intestazioneTabella--sorting ">
                                <EmptyDataTemplate>
                                    <center>
                                        <asp:Label ID="lblNoData" runat="server" Text="Nessuna pensione trovata per i criteri inseriti."
                                            SkinID="lblNoData" Visible="true"></asp:Label>
                                    </center>
                                </EmptyDataTemplate>
                                <Columns>
                                    <asp:BoundField HeaderText="Certificato" DataField="Certificato" Visible="true" ItemStyle-CssClass="TblRecordset3"
                                        ItemStyle-HorizontalAlign="Center" ItemStyle-Width="14%" ControlStyle-CssClass="tb8"
                                        SortExpression="Certificato" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort" />
                                    <asp:BoundField HeaderText="Categoria" DataField="Categoria" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        ControlStyle-CssClass="tb8" ItemStyle-Width="15%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Categoria" />
                                    <asp:BoundField HeaderText="Sede" DataField="Sede" Visible="true" ControlStyle-CssClass="tb8"
                                        ItemStyle-Width="14%" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Sede" />
                                    <asp:BoundField HeaderText="Decorrenza" DataField="DataCalcolo" NullDisplayText="Data non disponibile"
                                        Visible="true" ItemStyle-HorizontalAlign="Center" ControlStyle-CssClass="tb8"
                                        ItemStyle-Width="14%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="DataCalcolo" />
                                    <asp:BoundField HeaderText="Tipo Componente" DataField="TipoComponente" Visible="true"
                                        ItemStyle-HorizontalAlign="Center" ControlStyle-CssClass="tb8" ItemStyle-Width="14%"
                                        HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort" ItemStyle-CssClass="TblRecordset3"
                                        SortExpression="TipoComponente" />
                                    <asp:TemplateField HeaderText="Codice Eliminazione" Visible="false" ItemStyle-HorizontalAlign="Center"
                                        ItemStyle-Width="14%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" SortExpression="Eliminazione">
                                        <ItemTemplate>
                                            <asp:Label ID="gvLblEliminazione" Visible="<%# IsVisibleCodEliminazione(((GridViewRow) Container)) %>"
                                                runat="server" Text='<%# Eval("Eliminazione")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Tipo" DataField="Tipo" Visible="false" ItemStyle-HorizontalAlign="Center"
                                        ControlStyle-CssClass="tb8" ItemStyle-Width="14%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink intestazioneTabella__heading intestazioneTabella__heading--sort"
                                        ItemStyle-CssClass="TblRecordset3" />
                                    <asp:TemplateField HeaderText="Operazione" ItemStyle-Width="14%" HeaderStyle-CssClass="intestazioneTabella Row1"
                                        ControlStyle-CssClass="pulsante1 tertiary">
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnRicostituzione" Text="Ricostituzione" CommandName="Ricostitizione"
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" CssClass="tertiary"/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:ButtonField HeaderText="Operazione" DataTextField="Tipo" ButtonType="Button"
                                        Text="Ricostituzione" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="26%"
                                        HeaderStyle-CssClass="intestazioneTabella Row1" ControlStyle-CssClass="pulsante1"
                                        CommandName="cmdRicostituzione"   />--%>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="width: 100%; margin-top: 25px; margin-right: 40px;">
        <table width="90%" style="text-align: center;">
            <tr>
                <td style="text-align: right; width: 50%;">
                    <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                        CausesValidation="false" Width="190px" PostBackUrl="~/ElaborazionePosizione.aspx"
                        OnClientClick="aspnetForm.target ='_self'; BlockUI()" />
                </td>
                <td style="text-align: left; width: 50%;">
                    <asp:Button ID="btnElencoSinonimi" runat="server" Text="Torna all'elenco delle persone"
                        SkinID="btnAzione1" CausesValidation="false" OnClick="onClickSinonimi" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="HdnSedeOperatore" />
    <asp:HiddenField runat="server" ID="HdnNdomusSelezionato" />
    <asp:HiddenField runat="server" ID="HdnProgStorico" />
    <asp:HiddenField runat="server" ID="HdnSedeDomanda" />
    <asp:HiddenField runat="server" ID="HdnIsConsultazioneDomandaTRF" />
    <asp:HiddenField runat="server" ID="HdnTipoDomanda" />
    <div id="changeSedeOperatore" title="Cambia sede" style="display: none;">
        <p>
        </p>
    </div>
    <asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server"
        OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />
</asp:Panel>
