<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAnte67ES.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCAnte67ES" %>
<!-- Sezione Art57-->
<div id="divArt57" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label2" runat="server" Text="Art.67" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvElementArt57" SkinID="grdElenco1" AutoGenerateColumns="False"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1px" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvElementiArt57_RowEditing" Width="100%" AllowPaging="True" OnRowCommand="gvElementiArt57_RowCommand"
                        OnRowCancelingEdit="gvElementiArt57_RowCancelingEdit" OnRowUpdating="gvElementiArt57_RowUpdating"
                        OnRowDataBound="gvElementiArt57_RowDataBound" OnRowDeleting="gvElementiArt57_RowDeleting"
                        OnPageIndexChanging="gvElementiArt57_onPageIndexChanging" EnableModelValidation="True" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Contributi" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblContributi" Text='<%# Bind("Contributi") %>' CssClass="txtUppercase">       
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtArt57Contributi" MaxLength="9"
                                        Text='<%# Bind("Contributi")%>' Width="100px" ValidationGroup="UCAnte67ES_Ante57"></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="RFVContributi"
                                        ControlToValidate="txtArt57Contributi" Enabled="true" ErrorMessage="Contributi : E' un campo obbligatorio"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAnte67ES_Ante57" />
                                   <asp:RegularExpressionValidator runat="server" ID="REVContributi" ControlToValidate="txtArt57Contributi"
                                    Display="Dynamic" Enabled="true" ErrorMessage="Contributi: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCAnte67ES_Ante57" ValidationExpression="\d{1,4}(,\d{1,4})?" />
                                </EditItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella Row1"></HeaderStyle>
                                <ItemStyle CssClass="TblRecordset3"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblArt57Decorrenza" Text='<%# Bind("Decorrenza","{0:MM/yyyy}") %>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <%-- <asp:TextBox ID="txtLabelRetribuzione" runat="server" CssClass="tb8" MaxLength="11"
                                        Width="63%" Text='<%# Bind("Retribuzione") %>' ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" ></asp:TextBox>--%>
                                    <asp:TextBox ID="txtDecorrenza" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                                        MaxLength="11" Text='<%# Bind("Decorrenza","{0:MM/yyyy}") %>' ValidationGroup="UCAnte67ES_Ante57">
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RRVDecorrenza" ControlToValidate="txtDecorrenza"
                                        ErrorMessage="Decorrenza in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAnte67ES_Ante57" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenza"
                                        ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza : E' un campo obbligatorio"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAnte67ES_Ante57" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                        ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCAnte67ES_Ante57"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />  
                                </EditItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella Row1"></HeaderStyle>
                                <ItemStyle CssClass="TblRecordset3"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="Delete" CommandArgument="Delete" runat="server" />
                                    <!--  <asp:LinkButton ID="btnSalva" CommandName="Salva" CommandArgument="Salva" runat="server" /> -->
                                </ItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella"></HeaderStyle>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
</div>
<!-- Sezione ART24 -->
<div id="divArt24" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label1" runat="server" Text="Art.24" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Contributi Art.24:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtContributiArt24" runat="server" CssClass="tb8 txtUppercase" MaxLength="9"
                    Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVContribArt24" ControlToValidate="txtContributiArt24"
                    Display="Dynamic" Enabled="true" ErrorMessage="Contributi Art.24: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCDatiAnte67ES" ValidationExpression="\d{1,4}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Decorrenza Art.24:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtDecorrenzaArt24" runat="server" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                    Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator ControlToValidate="txtDecorrenzaArt24" ErrorMessage="Decorrenza Art.24 in formato non valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" runat="server"
                    Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDatiAnte67ES" Enabled="true" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaArt24" Display="Dynamic"
                    ErrorMessage="Decorrenza Art.24: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDatiAnte67ES"
                    ID="customCheckDataDecorrenzaArt24" ClientValidationFunction="checkCorrettezzaData" />  
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Pensione in Pagamento:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtPensioneInPagamento" runat="server" CssClass="tb8 txtUppercase" MaxLength="9"
                    Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVPensioneInPagamento" ControlToValidate="txtPensioneInPagamento"
                    Display="Dynamic" Enabled="true" ErrorMessage="Pensione in Pagamento: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCDatiAnte67ES" ValidationExpression="\d{1,4}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Pensione Fondo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtPensioneFondo" runat="server" CssClass="tb8 txtUppercase" MaxLength="9" Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVPensioneFondo" ControlToValidate="txtPensioneFondo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Pensione Fondo: Inserire valori interi o decimali (max 4 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCDatiAnte67ES" ValidationExpression="\d{1,4}(,\d{1,4})?" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Pensione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlCodicePensione" CssClass="tb8 txtUppercase" Width="50%" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="A" Value="A"></asp:ListItem>
                    <asp:ListItem Text="B" Value="B"></asp:ListItem>
                    <asp:ListItem Text="C" Value="C"></asp:ListItem>
                    <asp:ListItem Text="D" Value="D"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</div>
<!-- Sezione BUTTON -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;">
        <tr>
            <td style="text-align: right; vertical-align: bottom;">
                <asp:Button ID="btnSalvaAnte67" runat="server" CausesValidation="false" ValidationGroup="UCDatiAnte67ES"
                    SkinID="btnAzione1" Width="150px" Text="Salva Ante 67" OnClientClick="if(Page_ClientValidate('UCDatiAnte67ES')){aspnetForm.target ='_self'; BlockUI();}"
                    OnClick="btnSalvaAnte67_Click" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaAnte67" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Ante 67" Width="150px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Ante 67 Fondo?')) return false; else BlockUI();"
                    OnClick="btnEliminaAnte67_Click" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
