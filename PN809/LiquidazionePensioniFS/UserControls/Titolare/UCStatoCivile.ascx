<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCStatoCivile.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Titolare.UCStatoCivile" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<script type="text/javascript">
    function validatePage() 
    {
        var flag = true;
        if (document.getElementById("ctl00_ContentPlaceHolder1_pnlTabAnagrafica") != null) {
            flag = Page_ClientValidate('UCTabAnagrafica');
        }
        if (flag) 
        {
            if (document.getElementById("ctl00_ContentPlaceHolder1_pnlTabStatoCivile") != null) {
                flag = Page_ClientValidate('UCTabStatoCivile');
            }
        }
        return flag;
    }
</script>

<asp:Panel runat="server" ID="pnlStatoCivile">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvStatoCivile" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvStatoCivile_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvStatoCivile_RowCommand" OnRowCancelingEdit="gvStatoCivile_RowCancelingEdit"
                        OnRowUpdating="gvStatoCivile_RowUpdating" OnRowDataBound="gvStatoCivile_RowDataBound"
                        OnRowDeleting="gvStatoCivile_RowDeleting" OnPageIndexChanging="gvStatoCivile_onPageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                       
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="txtLabelDecorrenzaStatoCivile" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>'
                                    CssClass="txtUppercase">      
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaStatoCivile"
                                        MaxLength="7" Text=' <%# Bind("Decorrenza", "{0:MM/yyyy}")%>' Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredTxtDecorrenzaStatoCivile" ControlToValidate="txtDecorrenzaStatoCivile"
                                        Enabled="true" ErrorMessage="Decorrenza Stato Civile obbligatoria" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabStatoCivile"/>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaStatoCivile"
                                        Display="Dynamic" ControlToValidate="txtDecorrenzaStatoCivile" Enabled="true"
                                        ErrorMessage="Decorrenza Stato Civile: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabStatoCivile"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaStatoCivile" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabStatoCivile"
                                        ID="customCheckDataDataStatoCivile" ClientValidationFunction="checkCorrettezzaData" />  
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stato Civile" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="txtStatoCivile" Text='<%#Bind("SCivile")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlStatoCivile" runat="server" Width="300px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredddlStatoCivile" ControlToValidate="ddlStatoCivile"
                                        Enabled="true" ErrorMessage="Stato Civile obbligatorio" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabStatoCivile"/>
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
                <td style="text-align: Center" class="tab-actions-group__first">
                    <asp:Button ID="btnSalva" runat="server" Enabled="true" SkinID="btnAzione1" Text="Salva Stato Civile" Width="140px" 
                        onclick="btnSalva_Click" OnClientClick="if(validatePage()){aspnetForm.target ='_self'; BlockUI();}" CausesValidation="false" CssClass="primary"
                         />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdn_txtDecorrenzaPensioneSC" />
</asp:Panel>
