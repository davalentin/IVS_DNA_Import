<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLiquidazioni.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneLiquidazioni.UCLiquidazioni" %>
<table class="tabellaFormattazione">
    <tr>
        <td style="width: 740px" class="full-width pb-24">
            <label class="section-label" style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 740px; margin-left: 0px"  CssClass="full-width form-container background-light-blue">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1">
                            <label>
                                Sigla Categoria:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroSiglaCategoria"
                                Width="100px" MaxLength="8" />
                        </td>
                        <td class="Row1">
                            <label>
                                Sede:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroSede" Width="100px"
                                MaxLength="4" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Tipologia:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtFiltroTipologia" CssClass="tb8 txtUppercase" Width="100px"
                                Enabled="false" />
                            <%--<asp:DropDownList runat="server" ID="ddlFiltroTipologia" CssClass="tb8 txtUppercase"
                                Width="100px">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                        <td class="Row1">
                            <label>
                                Ricostituzione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlFiltroRicostituzione" CssClass="tb8 txtUppercase"
                                Width="100px">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Manuali abilitate:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlFiltroManualiAbilitate" CssClass="tb8 txtUppercase xxs"
                                Width="100px">
                                <asp:ListItem Text='' Value="" />
                                <asp:ListItem Text='NO' Value="NO" />
                                <asp:ListItem Text='SI' Value="SI" />
                            </asp:DropDownList>
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
    <tr>
        <td style="width: 740px" class="full-width pb-24">
            <br />
            <label class="section-label" style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Liquidazioni Abilitate</label>
            <asp:GridView runat="server" ID="gvLiquidazioni" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella full-width no-border intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                OnRowEditing="gvLiquidazioni_RowEditing" Width="740px" PageSize="10" AllowPaging="true"
                OnRowCommand="gvLiquidazioni_RowCommand" OnRowCancelingEdit="gvLiquidazioni_RowCancelingEdit"
                OnRowDataBound="gvLiquidazioni_RowDataBound" OnPageIndexChanging="gvLiquidazioni_onPageIndexChanging"
                OnRowDeleting="gvLiquidazioni_onRowDeleting" PagerSettings-Mode="NumericFirstLast"  PagerStyle-CssClass="default-pagination-tables">
                <Columns>
                    <asp:TemplateField HeaderText="Sigla Categoria" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSiglaCategoria" Text='<%# Bind("SiglaCategoria")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtSiglaCategoria" MaxLength="8"
                                Text=' <%# Bind("SiglaCategoria")%>' Width="100px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sede" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSede" Text='<%#Bind("Sede")%>' Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSede" runat="server" Width="100px"
                                MaxLength="4">
                            </asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipologia" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipologia" Text='<%#Bind("Tipologia")%>' Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlTipologia" runat="server" Width="100px"
                                Enabled="false">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ricostituzione" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRicostituzione" Text='<%#Bind("Ricostituzione")%>'
                                Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlRicostituzione" runat="server"
                                Width="100px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manuali abilitate" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <!-- TO DO -->
                            <asp:Label runat="server" ID="lbdManualiAbilitate" Text='<%#Bind("ManualiAbilitate")%>'
                                Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlManualiAbilitateGrid" runat="server"
                                Width="100px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr runat="server" id="trLiquidazioniFSPT_INPDAP">
        <td style="width: 740px" class="full-width pb-24">
            <br />
            <label class="section-label" style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Liquidazioni Abilitate FS - PT - INPDAP</label>
            <asp:GridView runat="server" ID="gvLiquidazioniFSPT_INPDAP" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella full-width no-border intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                OnRowEditing="gvLiquidazioniFSPT_INPDAP_RowEditing" Width="740px" PageSize="10" AllowPaging="true"
                OnRowCommand="gvLiquidazioniFSPT_INPDAP_RowCommand" OnRowCancelingEdit="gvLiquidazioniFSPT_INPDAP_RowCancelingEdit"
                OnRowDataBound="gvLiquidazioniFSPT_INPDAP_RowDataBound" OnPageIndexChanging="gvLiquidazioniFSPT_INPDAP_onPageIndexChanging"
                OnRowDeleting="gvLiquidazioniFSPT_INPDAP_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                <Columns>
                    <asp:TemplateField HeaderText="Sigla Categoria" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSiglaCategoria" Text='<%# Bind("SiglaCategoria")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtSiglaCategoria" MaxLength="8"
                                Text=' <%# Bind("SiglaCategoria")%>' Width="80px"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sede" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSede" Text='<%#Bind("Sede")%>' Width="60px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSede" runat="server" Width="60px"
                                MaxLength="4">
                            </asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipologia" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipologia" Text='<%#Bind("Tipologia")%>' Width="80px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlTipologia" runat="server" Width="80px"
                                Enabled="false">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ricostituzione" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRicostituzione" Text='<%#Bind("Ricostituzione")%>'
                                Width="80px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlRicostituzione" runat="server"
                                Width="80px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manuali abilitate" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <!-- TO DO -->
                            <asp:Label runat="server" ID="lbdManualiAbilitate" Text='<%#Bind("ManualiAbilitate")%>'
                                Width="80px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlManualiAbilitateGrid" runat="server"
                                Width="80px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Ricostituzione da Automatica" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRicostituzioneDaAutomatica" Text='<%#Bind("RicostituzioneDaAutomatica")%>'
                                Width="80px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlRicostituzioneDaAutomatica"
                                runat="server" Width="80px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Automatiche abilitate" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <!-- TO DO -->
                            <asp:Label runat="server" ID="lbdAutomaticheAbilitate" Text='<%#Bind("AutomaticheAbilitate")%>'
                                Width="80px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlAutomaticheAbilitate" runat="server"
                                Width="80px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server"
                                OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td style="width: 740px" class="full-width pb-24">
            <br />
            <label class="section-label" style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Esecuzione operazioni su tutte le sedi</label>
            <asp:Panel ID="pnlOpAllSedi" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 740px; margin-left: 0px" CssClass="full-width form-container">
                <table class="tabellaFormattazione grid grid-size-20" width="100%">
                    <tr>
                        <td class="Row1">
                            <label>
                                Operazione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlOpOperazione" CssClass="tb8 txtUppercase"
                                Width="300px">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="Row1">
                            <label>
                                Sigla Categoria:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtOpSiglaCategoria"
                                Width="100px" MaxLength="8" />
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Tipologia:</label>
                        </td>
                        <td class="field">
                            <asp:TextBox runat="server" ID="txtOpTipologia" CssClass="tb8 txtUppercase" Width="100px"
                                Enabled="false" />
                            <%--<asp:DropDownList runat="server" ID="ddlOpTipologia" CssClass="tb8 txtUppercase"
                                Width="100px" Enabled="false">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>--%>
                        </td>
                        <td class="Row1">
                            <label>
                                Ricostituzione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlOpRicostituzione" CssClass="tb8 txtUppercase"
                                Width="100px">
                                <asp:ListItem></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1">
                            <label>
                                Manuali abilitate:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlManualiAbilitateAllSedi" CssClass="tb8 txtUppercase xxs"
                                Width="100px">
                                <asp:ListItem Text='' Value="" />
                                <asp:ListItem Text='NO' Value="NO" />
                                <asp:ListItem Text='SI' Value="SI" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione grid grid-col-1 grid-col-1--right" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnEseguiOp" runat="server" Text="Esegui Operazione" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnEseguiOp_Click" OnClientClick="BlockUI();"  CssClass="primary" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
