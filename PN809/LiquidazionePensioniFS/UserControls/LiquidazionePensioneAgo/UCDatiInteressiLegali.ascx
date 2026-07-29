<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiInteressiLegali.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCDatiInteressiLegali" %>
<table class="tabellaFormattazione">
    <tr>
        <td>
            <label style="color: #336699; font-weight: normal; font-style: italic; font-size: larger">
                Interessi Legali</label>
            <asp:GridView runat="server" ID="gvInteressiLegali" SkinID="grdElenco1" AutoGenerateColumns="false"
                CssClass="intestazioneTabella" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                OnRowEditing="gvInteressiLegali_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                OnRowCommand="gvInteressiLegali_RowCommand" OnRowCancelingEdit="gvInteressiLegali_RowCancelingEdit"
                OnRowDataBound="gvInteressiLegali_RowDataBound" OnPageIndexChanging="gvInteressiLegali_onPageIndexChanging"
                OnRowDeleting="gvInteressiLegali_onRowDeleting" PagerSettings-Mode="NumericFirstLast">
                <Columns>
                    <asp:TemplateField HeaderText="Tipo Interesse Legale" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="28%">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblTipoIntILegale" CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList CssClass="tb8 txtUppercase" runat="server" ID="ddlTipoIntILegale"
                                Width="150px">
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Inizio" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDataInizio" Text='<%# Bind("DataInizio", "{0:dd/MM/yyyy}")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                ID="txtDataInizio" MaxLength="10" Text='<%# Bind("DataInizio", "{0:dd/MM/yyyy}")%>'>
                            </asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateDataInizio" ControlToValidate="txtDataInizio"
                                Display="Dynamic" ErrorMessage="Inserire la data in formato giorno/mese/anno"
                                Text="*" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaInteressiLegali" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataInizio" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" ValidationGroup="GrigliaInteressiLegali"
                                ID="customCheckDataInizio" ClientValidationFunction="checkCorrettezzaData" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Data Fine" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblDataFine" Text='<%# Bind("DataFine", "{0:dd/MM/yyyy}")%>'
                                CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" runat="server"
                                ID="txtDataFine" MaxLength="10" Text=' <%# Bind("DataFine","{0:dd/MM/yyyy}")%>'>
                            </asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="validateDataFine" ControlToValidate="txtDataFine"
                                Display="Dynamic" ErrorMessage="Inserire la data nel formato giorno/mese/anno"
                                Text="*" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                                ValidationGroup="GrigliaInteressiLegali" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtDataFine" Display="Dynamic"
                                ErrorMessage="La data inserita non è corretta" Text="*" ValidationGroup="GrigliaInteressiLegali"
                                ID="customCheckDataFine" ClientValidationFunction="checkCorrettezzaData" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                        ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblImporto" Text='<%# Bind("Importo")%>' CssClass="txtUppercase">      
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtImporto" MaxLength="5"
                                Text=' <%# Bind("Importo")%>' Width="90px">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator runat="server" ID="regulartxtImporto" ControlToValidate="txtImporto"
                                Display="Dynamic" ErrorMessage="Importo inserito in formato non corretto" Text="*"
                                ValidationGroup="GrigliaInteressiLegali" ValidationExpression="^\d{1,2}(,\d{1,2})?$" />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                        ItemStyle-Width="2%">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="Elimina" runat="server"
                                OnClientClick="BlockUI();" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
<!--div bottoni-->
<div style="width: 720px; margin-top: 25px; margin-right: 40px;">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalva" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Interessi Legali" Width="150px" OnClick="btnSalvaInteressiLegali_Click"
                    OnClientClick="aspnetForm.target ='_self'; BlockUI();" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnElimina" SkinID="btnAzione1" runat="server" Width="150px" Text="Elimina Interessi Legali"
                    CausesValidation="False" OnClick="btnEliminaInteressiLegali_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare gli Interessi Legali?')) return false; else BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<!--fine div bottoni-->
