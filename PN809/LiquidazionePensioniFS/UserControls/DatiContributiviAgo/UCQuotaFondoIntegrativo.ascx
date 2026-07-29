<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCQuotaFondoIntegrativo.ascx.cs" 
Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCQuotaFondoIntegrativo" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<script type="text/javascript">

    $(function () {
        $('#dialog-confirmQFI').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 220,
            width: 450,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaQuotaFondoIntegrativo.ClientID %>').click();

                    return true;
                }
            }
        });
    });

</script>

<asp:Panel runat="server" ID="pnlDatiCalcolo">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">                                
            </td>
        </tr>
    </table>
    <br />
    <div id="divQuotaFondoIntegrativo" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDatiContributivi">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvQuotaFondoIntegrativo" Visible="true" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination"
                        EnableViewState="true" OnRowCancelingEdit="gvQuotaFondoIntegrativo_RowCancelingEdit"
                        OnRowCommand="gvQuotaFondoIntegrativo_RowCommand" OnRowDataBound="gvQuotaFondoIntegrativo_RowDataBound"
                        OnRowEditing="gvQuotaFondoIntegrativo_RowEditing" OnRowUpdating="gvQuotaFondoIntegrativo_RowUpdating"
                        PageSize="10" SkinID="grdElenco1" Width="100%" OnDataBound="gvQuotaFondoIntegrativo_DataBound" OnPageIndexChanging="gvQuotaFondoIntegrativo_PageIndexChanging"
                        PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red"/>
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessuna quota inserita."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Codice Gestione"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestioneQuotaFondo_item" runat="server" CssClass="txtUppercase" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlCodiceGestioneQuotaFondo" runat="server" CssClass="txtUppercase tb8 classContribCodGestione xs"
                                        Width="150px">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestioneQuotaFondo" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" ControlToValidate="ddlCodiceGestioneQuotaFondo" ValidationGroup="UCTabQuotaFondoIntegrativoAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota_item" Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuota" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" ControlToValidate="ddlQuota" ValidationGroup="UCTabQuotaFondoIntegrativoAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimane" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("Settimane") %>' Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimane" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*"  ControlToValidate="txtSettimane"
                                        ValidationGroup="UCTabQuotaFondoIntegrativoAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimane" runat="server"
                                        ControlToValidate="txtSettimane" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabQuotaFondoIntegrativoAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Ammontare"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAmmontare" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAmmontare" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text='<%#Bind("Ammontare") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtAmmontare" runat="server"
                                        ErrorMessage="Ammontare: Campo obbligatorio" Text="*" ControlToValidate="txtAmmontare"
                                        ValidationGroup="UCTabQuotaFondoIntegrativoAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtAmmontare" runat="server"
                                        ControlToValidate="txtAmmontare" Display="Dynamic" ErrorMessage="Ammontare: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaFondoIntegrativoAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontante" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMontante" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text=' <%# Bind("Montante")%>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtMontante" runat="server"
                                        ErrorMessage="Montante: Campo obbligatorio" Text="*" ControlToValidate="txtMontante"
                                        ValidationGroup="UCTabQuotaFondoIntegrativoAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtMontante" runat="server"
                                        ControlToValidate="txtMontante" Display="Dynamic" ErrorMessage="Montante: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaFondoIntegrativoAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteQuotaFondoIntegrativo" ToolTip="cancella" runat="server" Text=""
                                        CommandArgument="<%#((GridViewRow)Container).RowIndex %>" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 25px;">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnPopUp" Style="display: none" runat="server" SkinID="btnAzione1"
                        CausesValidation="false" Text="Salva Quota Fondo Integrativo" Width="190px" OnClientClick="if(validateTab()){$('#dialog-confirmQFI').dialog('open');}return false;" CssClass="primary" />
                    <asp:Button ID="btnSalvaQuotaFondoIntegrativo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Fondo Integrativo" Width="190px" OnClientClick="if(Page_ClientValidate('UCTabQuotaFondoIntegrativo')){aspnetForm.target ='_self'; BlockUI();}"
                        OnClick="btnSalvaQuotaFondoIntegrativo_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaQuotaFondoIntegrativo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Fondo Integrativo" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la Quota Fondo Integrativo?')) return false; else BlockUI();"
                        OnClick="btnEliminaQuotaFondoIntegrativo_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
