<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCQuotaFondoINPGI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCQuotaFondoINPGI" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">

    function setPeriodoRetr(i) {
        var row = i.parentNode.parentNode;
        var rowIndex = row.rowIndex;
        if (i.selectedIndex > 0) {
            var periodi = document.getElementById("<%=hdnPeriodiRetrib.ClientID%>").value.split(";");
            document.getElementById('<%=gvRetributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = periodi[i.selectedIndex - 1];
        }
        else
            document.getElementById('<%=gvRetributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = "";
    }

    function setPeriodoContr(i) {
        var row = i.parentNode.parentNode;
        var rowIndex = row.rowIndex;
        if (i.selectedIndex > 0) {
            var periodi = document.getElementById("<%=hdnPeriodiContrib.ClientID%>").value.split(";");
            document.getElementById('<%=gvContributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = periodi[i.selectedIndex - 1];
        }
        else
            document.getElementById('<%=gvContributiviINPGI.ClientID %>').rows[rowIndex].cells[2].childNodes[1].innerHTML = "";
    }

</script>
<asp:Panel runat="server" ID="pnlDatiCalcolo">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblRicNonContrib" runat="server" Text="I dati di calcolo sono disponibili per la sola visualizzazione.  Possono essere modificati con una Ricostituzione contributiva."
                    Style="font-weight: bold" ForeColor="Black" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <!--panel retributivo-->
    <div id="divRetributiviINPGI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td class=" full-grid">
                    <asp:Label runat="server" ID="lblDatiRetributivi">Dati Retributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center" class=" full-grid">
                    <asp:GridView ID="gvRetributiviINPGI" Visible="true" runat="server" AllowPaging="true"
                        AutoGenerateColumns="false" AutoGenerateEditButton="true" BorderColor="Black"
                        BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="gvRetributiviINPGI_RowCancelingEdit"
                        OnRowCommand="gvRetributiviINPGI_RowCommand" OnRowDataBound="gvRetributiviINPGI_RowDataBound"
                        OnRowEditing="gvRetributiviINPGI_RowEditing" OnRowUpdating="gvRetributiviINPGI_RowUpdating"
                        PageSize="10" SkinID="grdElenco1" Width="100%" OnDataBound="gvRetributiviINPGI_DataBound"
                        OnPageIndexChanging="gvRetributiviINPGI_PageIndexChanging" OnLoad="gvRetributiviINPGI_Load" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessuna quota inserita." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quote"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestioneRetribQuotaFondo_item" runat="server" CssClass="txtUppercase"
                                        Width="80px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlCodiceGestioneRetribQuotaFondo" runat="server" CssClass="txtUppercase tb8 classContribCodGestione xs"
                                        Width="80px" onchange="setPeriodoRetr(this)">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestioneRetribQuotaFondo" runat="server"
                                        ErrorMessage="Codice Gestione: campo obbligatorio" Text="*" ControlToValidate="ddlCodiceGestioneRetribQuotaFondo"
                                        ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" CssClass="field-is-required offClass"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Periodo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblPeriodoRetr" runat="server" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblPeriodoRetrib" runat="server" Width="150px" Text=" "></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimane" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimane" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                                        Text='<%#Bind("Settimane") %>' Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimane" runat="server" ErrorMessage="Settimane: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtSettimane" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimane" runat="server" ControlToValidate="txtSettimane"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Retribuzione Media Settimanale"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRetribuzioneMediaSettimanale" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRetribuzioneMediaSettimanale" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text='<%#Bind("RetribuzioneMediaSettimanale") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtRetribuzioneMediaSettimanale" runat="server"
                                        ErrorMessage="Retribuzione Media Settimanale: Campo obbligatorio" Text="*" ControlToValidate="txtRetribuzioneMediaSettimanale"
                                        ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtRetribuzioneMediaSettimanale" runat="server"
                                        ControlToValidate="txtRetribuzioneMediaSettimanale" Display="Dynamic" ErrorMessage="Retribuzione Media Settimanale: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblImportoCalcolato" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtImportoCalcolato" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text='<%#Bind("ImportoCalcolato") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtImportoCalcolato" runat="server"
                                        ErrorMessage="Importo Calcolato: Campo obbligatorio" Text="*" ControlToValidate="txtImportoCalcolato"
                                        ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtImportoCalcolato" runat="server" ControlToValidate="txtImportoCalcolato"
                                        Display="Dynamic" ErrorMessage="Importo Calcolato: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane Comma 707"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimaneComma707" runat="server" Text='<%#Bind("SettimaneComma707") %>'
                                        Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimaneComma707" runat="server" CssClass="tb8 txtUppercase"
                                        MaxLength="4" Text='<%#Bind("SettimaneComma707") %>' Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneComma707" runat="server"
                                        ErrorMessage="Settimane Comma 707: Campo obbligatorio" Text="*" ControlToValidate="txtSettimane"
                                        ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimaneComma707" runat="server" ControlToValidate="txtSettimaneComma707"
                                        Display="Dynamic" ErrorMessage="Settimane Comma 707: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo Comma 707"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblImportoComma707" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtImportoComma707" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="12" Style="text-align: left" Text='<%#Bind("ImportoComma707") %>'
                                        Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtImportoComma707" runat="server" ErrorMessage="Importo Comma 707: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtImportoComma707" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtImportoComma707" runat="server" ControlToValidate="txtImportoComma707"
                                        Display="Dynamic" ErrorMessage="Importo Comma 707: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaRetrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteQuotaRetribFondoINPGI" ToolTip="cancella" runat="server"
                                        Text="" CommandArgument="<%#((GridViewRow)Container).RowIndex %>" CommandName="Elimina" />
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
    <!-- fine panel retributivo--->
    <!--panel contributivo-->
    <div id="divContributiviINPGI" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblDatiContributivi">Dati Contributivi:</asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvContributiviINPGI" Visible="true" runat="server" AllowPaging="true"
                        AutoGenerateColumns="false" AutoGenerateEditButton="true" BorderColor="Black"
                        BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="gvContributiviINPGI_RowCancelingEdit"
                        OnRowCommand="gvContributiviINPGI_RowCommand" OnRowDataBound="gvContributiviINPGI_RowDataBound"
                        OnRowEditing="gvContributiviINPGI_RowEditing" OnRowUpdating="gvContributiviINPGI_RowUpdating"
                        PageSize="10" SkinID="grdElenco1" Width="100%" OnDataBound="gvContributiviINPGI_DataBound"
                        OnPageIndexChanging="gvContributiviINPGI_PageIndexChanging" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessuna quota inserita." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodiceGestioneQuotaFondo_item" runat="server" CssClass="txtUppercase"
                                        Width="80px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlCodiceGestioneQuotaFondo" runat="server" CssClass="txtUppercase tb8 classContribCodGestione xs"
                                        Width="80px" onchange="setPeriodoContr(this)">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestioneQuotaFondo" runat="server"
                                        ErrorMessage="Codice Gestione: campo obbligatorio" Text="*" ControlToValidate="ddlCodiceGestioneQuotaFondo"
                                        ValidationGroup="UCTabQuotaContrFondoINPGIAgo" CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Periodo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblPeriodoContr" runat="server" Width="150px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblPeriodoContrib" runat="server" Width="150px" Text=" "></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Settimane"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSettimaneContr" runat="server" Text='<%#Bind("Settimane") %>' Width="40px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSettimaneContr" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                                        Text='<%#Bind("Settimane") %>' Width="40px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneContr" runat="server" ErrorMessage="Settimane: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtSettimaneContr" ValidationGroup="UCTabQuotaContrFondoINPGIAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimaneContr" runat="server" ControlToValidate="txtSettimaneContr"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabQuotaContrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblMontante" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtMontante" runat="server" CssClass="txtUppercase tb8 " MaxLength="12"
                                        Style="text-align: left" Text='<%#Bind("Montante") %>' Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtMontante" runat="server" ErrorMessage="Montante: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtMontante" ValidationGroup="UCTabQuotaContrFondoINPGIAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtMontante" runat="server" ControlToValidate="txtMontante"
                                        Display="Dynamic" ErrorMessage="Montante: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaContrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Quota Contributivo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblQuota" runat="server" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtQuota" runat="server" CssClass="txtUppercase tb8 " MaxLength="12"
                                        Style="text-align: left" Text='<%#Bind("Quota") %>' Width="100px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtQuota" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtQuota" ValidationGroup="UCTabQuotaContrFondoINPGIAgo"
                                        CssClass="offClass field-is-required"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtQuota" runat="server" ControlToValidate="txtQuota"
                                        Display="Dynamic" ErrorMessage="Quota: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabQuotaContrFondoINPGIAgo" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteQuotaContribFondoINPGI" ToolTip="cancella" runat="server"
                                        Text="" CommandArgument="<%#((GridViewRow)Container).RowIndex %>" CommandName="Elimina" />
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
    <!-- fine panel contributivo--->
    <div style="margin-top: 25px;">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalvaQuotaFondoINPGI" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Fondo INPGI" Width="190px" OnClientClick="if(Page_ClientValidate('UCTabQuotaFondoINPGI')){aspnetForm.target ='_self'; BlockUI();}"
                        OnClick="btnSalvaQuotaFondoINPGI_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaQuotaFondoINPGI" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Fondo INPGI" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la Quota Fondo INPGI?')) return false; else BlockUI();"
                        OnClick="btnEliminaQuotaFondoINPGI_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="modalitaEditContributivi" Value="false" />
<asp:HiddenField runat="server" ID="modalitaEditRetributivi" Value="false" />
<asp:HiddenField runat="server" ID="hdnPeriodiRetrib" Value="" />
<asp:HiddenField runat="server" ID="hdnPeriodiContrib" Value="" />
