<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCResidenzeEstere.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare.UCResidenzeEstere" %>

<script type="text/javascript">
    function CleanFields2() {
        $($("table[id*=gvResidenzeEstere] input[type=text][id*=txtDecorrenzaStatoEstero]")).val('');
        $($("table[id*=gvResidenzeEstere] select[id*=ddlStatoEstero]")).val('');
        return false;
    }

    
</script>

<asp:Panel runat="server" ID="pnlResidenzeEstere">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvResidenzeEstere" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvResidenzeEstere_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvResidenzeEstere_RowCommand" OnRowCancelingEdit="gvResidenzeEstere_RowCancelingEdit"
                        OnRowUpdating="gvResidenzeEstere_RowUpdating" OnRowDataBound="gvResidenzeEstere_RowDataBound"
                        OnRowDeleting="gvResidenzeEstere_RowDeleting"
                        OnPageIndexChanging="gvResidenzeEstere_onPageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                        
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                     <asp:Label runat="server" ID="txtDecorrenzaStatoEstero" Text='<%#Bind("Decorrenza", "{0:MM/yyyy}")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaStatoEstero"
                                        MaxLength="7" TabIndex="1" Text='<%#Bind("Decorrenza", "{0:MM/yyyy}")%>' Width="100px"></asp:TextBox>
                                     <asp:RequiredFieldValidator runat="server" ID="RequiredTxtDecorrenzaStatoEstero" ControlToValidate="txtDecorrenzaStatoEstero"
                                        Enabled="true" ErrorMessage="Decorrenza obbligatoria" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabResidenzeEstere"/>
                                     <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaStatoEstero"
                                        Display="Dynamic" ControlToValidate="txtDecorrenzaStatoEstero" Enabled="true"
                                        ErrorMessage="Decorrenza Residenza Estera: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabResidenzeEstere"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                   <%--<asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaStatoEstero"
                                        Display="Dynamic" ErrorMessage="Decorrenza Residenza Estera: Data inserita posteriore a quella odierna"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabResidenzeEstere" ID="customDecorrenzaStatoEstero" ClientValidationFunction="checkDataPostOdiernaMMAAAA" />--%>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaStatoEstero" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabResidenzeEstere"
                                        ID="customCheckDataDecorrenzaStatoEstero" ClientValidationFunction="checkCorrettezzaData" />  
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stato Estero" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="txtStatoEstero" Text='<%#Bind("StatoEstero")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlStatoEstero" runat="server"
                                         TabIndex="2" Width="300px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredddlStatoEstero" ControlToValidate="ddlStatoEstero"
                                        Enabled="true" ErrorMessage="Stato Estero obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabResidenzeEstere"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
        <div id="tastoAnnulla" style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first--force">
                    <asp:Button ID="btnSalva" runat="server" Enabled="true" SkinID="btnAzione1" CausesValidation="false"
                        Text="Salva Residenze Estere" Width="190px" onclick="btnSalva_Click" OnClientClick="if(Page_ClientValidate('UCTabResidenzeEstere')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnElimina" runat="server" SkinID="btnAzione1" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare le Residenze Estere?')) return false; else BlockUI();"
                        Enabled="true" Text="Elimina Residenze Estere" Width="190px"  Visible = "true" onclick="btnElimina_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdn_txtDecorrenzaPensione" />
    <asp:HiddenField runat="server" ID="hdn_lblCodiceComuneResidenza" />
</asp:Panel>
