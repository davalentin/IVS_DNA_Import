<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLavorazioneManualeAutomatiche.ascx.cs" 
Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.LavorazioneManualeAutomatiche.UCLavorazioneManualeAutomatiche" %>
<script type="text/javascript">

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
</script>
<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px" class="full-width">
            <br />
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger" class="section-label">
                Elenco Domande Con Richiesta Di Lavorazione Manuale</label>
            <asp:GridView runat="server" ID="gvLavorazioneManualeAutomatiche" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella no-border intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                Width="100%" PageSize="10" AllowPaging="true" OnRowCommand="gvLavorazioneManualeAutomatiche_RowCommand"
                OnRowDataBound="gvLavorazioneManualeAutomatiche_RowDataBound" OnPageIndexChanging="gvLavorazioneManualeAutomatiche_onPageIndexChanging"
                OnRowDeleting="gvLavorazioneManualeAutomatiche_onRowDeleting" PagerSettings-Mode="NumericFirstLast" PagerStyle-CssClass="default-pagination-tables">
                <EmptyDataTemplate>
                    <center>
                        <asp:Label ID="lblNoData" runat="server" Text="Nessun record trovato." SkinID="lblNoData"
                            Visible="true"></asp:Label>
                    </center>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="Numero Domanda" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblNDomus" Text='<%#Bind("NDomus")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sigla Categoria" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSiglaCategoria" Text='<%#Bind("SiglaCategoria")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Sede" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblCodiceSede" Text='<%#Bind("CodiceSede")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Gruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblGruppo" Text='<%#Bind("Gruppo")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Prodotto" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblProdotto" Text='<%#Bind("Prodotto")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tipo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipo" Text='<%#Bind("Tipo")%>'> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data acquisizione" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDecorrenzaOriginaria"> 
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Autorizzazione Manuale" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" ID="ddlAutorizzazioneManuale" CommandArgument='<%#Eval("Id")%>' AutoPostBack="True" CommandName="Update"
                            OnSelectedIndexChanged="DdlAutorizzazioneManuale_SelectedIndexChanged" onchange="if (!window.confirm('Sei sicuro di voler procedere in via definitiva?')) return false; else BlockUI();">
                                <asp:ListItem Value="0" Text="In attesa di autorizzazione"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Autorizzazione concessa"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Autorizzazione negata"></asp:ListItem>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
<div id="divdialog" title="Nota" style="display: none; border-style: none; border-color: White;">
    <div id="textDialog">
    </div>
</div>
<asp:HiddenField runat="server" ID="hdnTextDialog" />
