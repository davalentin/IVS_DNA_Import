<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloENPALS.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiCalcoloENPALS" %>

<script type="text/javascript">

    function validateTabEnpals() {
        var flag = true;
        if (flag) {
            flag = Page_ClientValidate('UCTabDatiCalcoloENPALS');
        }
        return flag;
    }
</script>

<div id="pdivRetributivo" runat="server" style="margin-left: 10px; margin-right: 10px;" visible="false">
    <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td class="Row1 full-grid">
                <asp:Label runat="server" ID="lblDatiRetributivi" Style="font-weight: bold"> Dati Retributivi:</asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" class=" full-grid">
                <asp:GridView runat="server" ID="gvDatiRetributivi" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                    AutoGenerateEditButton="false" PageSize="10" AllowPaging="true"
                    OnRowDataBound="gvDatiRetributivi_RowDataBound" OnDataBound="gvDatiRetributivi_DataBound" OnRowCommand="gvDatiRetributivi_RowCommand"
                    OnRowEditing="gvDatiRetributivi_RowEditing" EnableViewState="true" RowStyle-HorizontalAlign="Center" PagerStyle-CssClass="default-pagination-tables">
                    <EmptyDataRowStyle ForeColor="Red" />
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun dato retributivo inserito."
                                SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="60px"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQuota" Width="40px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8 xxs">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                    <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RFV_ddlQuota" runat="server" Display="Dynamic"
                                    ErrorMessage="Quota: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Numero Contributi" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblNSettimane"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtNSettimane" CssClass="txtUppercase tb8" MaxLength="4" Width="50px" Text='<%#Bind("Periodi") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtNSettimane" ControlToValidate="txtNSettimane" Display="Dynamic"
                                    ErrorMessage="Numero Contributi: formato non valido" ValidationExpression="[0-9]*" ValidationGroup="UCTabDatiCalcoloENPALSRetr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Numero Contributi Giornalieri per Calcolo Retribuzione Pensionabile" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblNTotaleContributiCalcolo"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtNTotaleContributiCalcolo" CssClass="txtUppercase tb8" MaxLength="4" Width="50px" Text='<%#Bind("NTotaleContributiCalcolo") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtNTotaleContributiCalcolo" ControlToValidate="txtNTotaleContributiCalcolo" Display="Dynamic"
                                    ErrorMessage="Numero Contributi Giornalieri per Calcolo Retribuzione Pensionabile: formato non valido" ValidationExpression="[0-9]*"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblRMS" Width="70px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtRMS" CssClass="txtUppercase tb8" MaxLength="13" Width="70px" Text='<%#Bind("RM") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtRMS" ControlToValidate="txtRMS" Display="Dynamic"
                                    ErrorMessage="Retribuzione Media: formato non valido" ValidationExpression="^\d{0,9}(,\d{0,3})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblImporto" Width="70px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtImporto" CssClass="txtUppercase tb8" MaxLength="13" Width="70px" Text='<%#Bind("Importo") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtImporto" ControlToValidate="txtImporto" Display="Dynamic"
                                    ErrorMessage="Importo: formato non valido" ValidationExpression="^\d{0,9}(,\d{0,3})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Giorni 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblNSettimane707"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtNSettimane707" CssClass="txtUppercase tb8" MaxLength="4" Width="50px" Text='<%#Bind("Giorni707") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtNSettimane707" ControlToValidate="txtNSettimane707" Display="Dynamic"
                                    ErrorMessage="Giorni 707: formato non valido" ValidationExpression="[0-9]*"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Importo 707" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblImporto707" Width="70px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtImporto707" CssClass="txtUppercase tb8" MaxLength="13" Width="70px" Text='<%#Bind("Importo707") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtImporto707" ControlToValidate="txtImporto707" Display="Dynamic"
                                    ErrorMessage="Importo 707: formato non valido" ValidationExpression="^\d{0,9}(,\d{0,3})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSRetr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDeleteRetributivi" CommandName="Elimina" CommandArgument="<%# ((GridViewRow)Container).RowIndex %>"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td class="Row1" style="width: 30%; text-align: left">
                <label>Importo pro rata temporis:</label>
            </td>
            <td class="field" style="width: 70%; text-align: left">
                <asp:TextBox runat="server" ID="txtImportoProRataTemporis" MaxLength="13" CssClass="txtUppercase tb8" Width="130px" Enabled="false"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>
<div id="pdivContributivo" runat="server" style="margin-left: 10px; margin-right: 10px;" visible="false">
    <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td class="Row1">
                <asp:Label runat="server" ID="lblCalcoloContributivo" Style="font-weight: bold">Dati Contributivi:</asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:GridView ID="gvDatiContributivi" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                    AutoGenerateEditButton="false" BorderColor="Black" BorderWidth="1" CssClass="intestazioneTabella intestazioneTabella__with-pagination"
                    EnableViewState="true" OnRowDataBound="gvDatiContributivi_RowDataBound" OnDataBound="gvDatiContributivi_DataBound"
                    OnRowCommand="gvDatiContributivi_RowCommand" OnRowEditing="gvDatiContributivi_RowEditing"
                    PageSize="10" SkinID="grdElenco1" Width="100%" RowStyle-HorizontalAlign="Center" PagerStyle-CssClass="default-pagination-tables">
                    <EmptyDataRowStyle ForeColor="Red" />
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun dato contributivo inserito."
                                SkinID="lblNoData" Visible="true"></asp:Label>
                        </center>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="60px"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQuota" Width="40px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlQuota" Width="40px" CssClass="txtUppercase tb8 xxs">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                    <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RFV_ddlQuota" runat="server" Display="Dynamic"
                                    ErrorMessage="Quota: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota"
                                    ValidationGroup="UCTabDatiCalcoloENPALSContr"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Numero Contributi"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label ID="lblNumeroContributiTotale" runat="server" Width="40px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtNumeroContributiTotale" CssClass="txtUppercase tb8" MaxLength="4" Width="40px" Text='<%#Bind("NumeroContributiTotale") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtNumeroContributiTotale" ControlToValidate="txtNumeroContributiTotale" Display="Dynamic"
                                    ErrorMessage="Numero Contributi: formato non valido" ValidationExpression="[0-9]*"
                                    ValidationGroup="UCTabDatiCalcoloENPALSContr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo Quota Contributivo"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label ID="lblImportoContributivoTotale" runat="server" Width="40px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtImportoContributivoTotale" CssClass="txtUppercase tb8" MaxLength="13" Width="100px" Text='<%#Bind("ImportoContributivoTotale") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtImportoContributivoTotale" ControlToValidate="txtImportoContributivoTotale" Display="Dynamic"
                                    ErrorMessage="Importo Quota Contributivo: formato non valido" ValidationExpression="^\d{0,9}(,\d{0,3})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSContr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Montante Complessivo"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label ID="lblMontante" runat="server" Width="100px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtMontante" CssClass="txtUppercase tb8" MaxLength="16" Width="100px" Text='<%#Bind("Montante") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtMontante" ControlToValidate="txtMontante" Display="Dynamic"
                                    ErrorMessage="Montante Complessivo: formato non valido" ValidationExpression="^\d{0,12}(,\d{0,3})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSContr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Coefficiente di Trasformazione"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label ID="lblCoefficienteTrasformazione" runat="server" Width="100px"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtCoefficienteTrasformazione" CssClass="txtUppercase tb8" MaxLength="11" Width="100px" Text='<%#Bind("CoefficienteTrasformazione") %>'></asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtCoefficienteTrasformazione" ControlToValidate="txtCoefficienteTrasformazione" Display="Dynamic"
                                    ErrorMessage="Coefficiente di Trasformazione: formato non valido" ValidationExpression="^\d{0,4}(,\d{0,6})?$"
                                    ValidationGroup="UCTabDatiCalcoloENPALSContr" Text="*" CssClass="field-is-required" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;" HeaderStyle-Width="20px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDeleteContributivi" CommandName="Elimina" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</div>
<div id="pdivMisto" runat="server" style="margin-left: 10px; margin-right: 10px;" visible="false">
    <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td class="Row1" style="width: 50%; text-align: left">
                <label>Importo quota retributiva nel sistema misto:</label>
            </td>
            <td class="field" style="width: 50%; text-align: left">
                <asp:TextBox runat="server" ID="txtImportoQuotaRetributivaInMisto" MaxLength="13" CssClass="txtUppercase tb8" Width="130px" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoQuotaRetributivaInMisto" ControlToValidate="txtImportoQuotaRetributivaInMisto"
                    Display="Dynamic" ErrorMessage="Importo quota retributiva nel sistema misto: inserire l'importo in formato valido (max 9 interi e 3 decimali)"
                    Text="*" CssClass="field-is-required" ValidationExpression="\d{1,9}(,\d{1,3})?" ValidationGroup="UCTabDatiCalcoloENPALS" Enabled="true" />
            </td>
        </tr>
    </table>
</div>

<div id="pdivComune" runat="server" style="margin-left: 10px; margin-right: 10px;">
    <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px; width: 99%">
        <tr>
            <td class="Row1" style="width: 25%; text-align: left">
                <label>
                    Importo pensione:</label>
            </td>
            <td class="field" style="width: 25%; text-align: left">
                <asp:TextBox runat="server" ID="txtImportoPensione" MaxLength="14" CssClass="txtUppercase tb8" Width="130px" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoPensione" ControlToValidate="txtImportoPensione"
                    Display="Dynamic" ErrorMessage="Importo pensione: inserire l'importo in formato valido (max 9 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationExpression="\d{1,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloENPALS" Enabled="true" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left">
                <label>
                    Decorrenza Importo Pensione:
                </label>
            </td>
            <td class="field" style="width: 25%; text-align: left">
                <asp:Label runat="server" ID="lblDecorrenzaImportoPensione"></asp:Label>
            </td>
        </tr>
        <tr runat="server" id="trImportoPensione707" visible="false">
            <td class="Row1" style="width: 25%; text-align: left">
                <label>
                    Importo pensione 707:</label>
            </td>
            <td class="field" style="width: 25%; text-align: left">
                <asp:TextBox runat="server" ID="txtImportoPensione707" MaxLength="14" CssClass="txtUppercase tb8" Width="130px" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoPensione707" ControlToValidate="txtImportoPensione707"
                    Display="Dynamic" ErrorMessage="Importo pensione 707: inserire l'importo in formato valido (max 9 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationExpression="\d{1,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloENPALS" Enabled="true" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left"></td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%; text-align: left">
                <label>
                    Importo IIS:</label>
            </td>
            <td class="field" style="width: 25%; text-align: left">
                <asp:TextBox runat="server" ID="txtImportoIIS" MaxLength="14" CssClass="txtUppercase tb8" Width="130px" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtImportoIIS"
                    Display="Dynamic" ErrorMessage="Importo IIS: inserire l'importo in formato valido (max 9 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationExpression="\d{1,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloENPALS" Enabled="true" />
            </td>
            <td class="Row1" style="width: 25%; text-align: left">
                <label>
                    Decorrenza Importo IIS:
                </label>
            </td>
            <td class="field" style="width: 25%; text-align: left">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaImportoIIS" Text="gg/mm/aaaa"
                    CssClass="txtUppercase tb8 dateGGmmAAAA date-picker-base" MaxLength="10" Enabled="false"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REV_txtDecorrenzaImportoIIS" ControlToValidate="txtDecorrenzaImportoIIS"
                    ErrorMessage="Decorrenza Importo IIS in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$|^GG/MM/AAAA$|^gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiCalcoloENPALS"
                    Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaImportoIIS" Display="Dynamic"
                    ErrorMessage="Decorrenza Importo IIS: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloENPALS"
                    ID="customCheckDataDecorrenzaImportoIIS" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
    </table>
</div>

<div style="margin-top: 25px;">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Calcolo" Width="190px" OnClientClick="if(validateTabEnpals()){aspnetForm.target ='_self'; BlockUI();}"
                    OnClick="btnSalvaDatiCalcolo_Click" CssClass="primary" />
                <%--                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" OnClientClick="javascript:return CleanFields4();"
                        Enabled="true" Text="Pulisci" Width="100px" />
                --%>
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Calcolo" Width="190px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();"
                    OnClick="btnEliminaDatiCalcolo_Click" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
