<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCGestioneAbilTrasf.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.GestioneAbilitazioneTrasformazioni.UCGestioneAbilTrasf" %>
<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px" class="full-width">
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Filtro di ricerca</label>
            <asp:Panel ID="panFiltro" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" CssClass="full-width background-light-blue form-container">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Sigla Categoria:</label>
                        </td>
                        <td class="field" style="width: 25%">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtFiltroSiglaCategoria"
                                Width="100px" MaxLength="8" />
                        </td>
                        <td class="Row1" style="width: 25%">
                            <label>
                                Sede:</label>
                        </td>
                        <td class="field" style="width: 25%">
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
                        </td>
                        <%--<td class="Row1">
                            <label>
                                Tipo Abilitazione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlFiltroAbilitazione" CssClass="tb8 txtUppercase"
                                Width="125px">
                                <asp:ListItem Text='' Value="" />
                                <asp:ListItem Text='MANUALE' Value="MANUALE" />
                                <asp:ListItem Text='AUTOMATICA' Value="AUTOMATICA" />
                                <asp:ListItem Text='ALL' Value="ALL" />
                            </asp:DropDownList>
                        </td>--%>
                    </tr>
                    <%--<tr>
                        <td class="Row1">
                            <label>
                                Trasformazione da provvisoria a definitiva:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlFiltroTrasformazione" CssClass="tb8 txtUppercase"
                                Width="100px">
                                <asp:ListItem Text='' Value="" />
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>--%>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="end">
                            <asp:Button ID="btnAnnullaFiltro" runat="server" Text="Annulla Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnAnnullaFiltro_Click" OnClientClick="BlockUI();" />
                            <asp:Button ID="btnApplicaFiltro" runat="server" Text="Applica Filtro" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnApplicaFiltro_Click" OnClientClick="BlockUI();" CssClass="primary mr-0" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td style="width: 720px" class="full-width">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Trasformazioni Abilitate</label>
            <asp:GridView runat="server" ID="gvTrasformazioni" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella full-width intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                OnRowEditing="gvTrasformazioni_RowEditing" Width="720px" PageSize="10" AllowPaging="true"
                OnRowCommand="gvTrasformazioni_RowCommand" OnRowCancelingEdit="gvTrasformazioni_RowCancelingEdit"
                OnRowDataBound="gvTrasformazioni_RowDataBound" OnPageIndexChanging="gvTrasformazioni_onPageIndexChanging"
                OnRowDeleting="gvTrasformazioni_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
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
                    <%--<asp:TemplateField HeaderText="Trasformazioni" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTrasformazione" Text='<%#Bind("Trasformazione")%>'
                                Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlTrasformazione" runat="server"
                                Width="100px">
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Abilitazione" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblAbilitazione" Text='<%#Bind("TipoAbilitazione")%>'
                                Width="100px"> 
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlAbilitazioneGrid" runat="server"
                                Width="125px">
                                <asp:ListItem Text='MANUALE' Value="MANUALE" />
                                <asp:ListItem Text='AUTOMATICA' Value="AUTOMATICA" />
                                <asp:ListItem Text='ALL' Value="ALL" />
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>--%>
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
        <td style="width: 720px" class="full-width">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label mt-32">
                Esecuzione operazioni su tutte le sedi</label>
            <asp:Panel ID="pnlOpAllSedi" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px" CssClass="full-width form-container">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td class="Row1" style="width: 20%">
                            <label>
                                Operazione:</label>
                        </td>
                        <td class="field" style="width: 38%">
                            <asp:DropDownList runat="server" ID="ddlOpOperazione" CssClass="tb8 txtUppercase"
                                Width="250px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Text="Salvataggio su tutte le sedi" Value="SAVE"></asp:ListItem>
                                <asp:ListItem Text="Eliminazione su tutte le sedi" Value="DELETE"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="Row1" style="width: 20%">
                            <label>
                                Sigla Categoria:</label>
                        </td>
                        <td class="field" style="width: 22%">
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
                        </td>
                        <%--<td class="Row1">
                            <label>
                                Tipo Abilitazione:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlAbilitazioneAllSedi" CssClass="tb8 txtUppercase"
                                Width="120px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Text='MANUALE' Value="MANUALE" />
                                <asp:ListItem Text='AUTOMATICA' Value="AUTOMATICA" />
                                <asp:ListItem Text='ALL' Value="ALL" />
                            </asp:DropDownList>
                        </td>--%>
                    </tr>
                    <%--<tr>
                        <td class="Row1">
                            <label>
                                Trasformazioni da provvisoria a definitiva:</label>
                        </td>
                        <td class="field">
                            <asp:DropDownList runat="server" ID="ddlOpTrasformazione" CssClass="tb8 txtUppercase"
                                Width="100px">
                                <asp:ListItem></asp:ListItem>
                                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>--%>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="end">
                            <asp:Button ID="btnEseguiOp" runat="server" Text="Esegui Operazione" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnEseguiOp_Click" OnClientClick="BlockUI();" cssClass="primary mr-0" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
