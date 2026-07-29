<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiFondoGAS_ES.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiFondoGAS_ES" %>
<style type="text/css">
    .fixed-dialog
    {
        position: fixed;
    }
</style>
<div id="divServizioUtile" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblTitolo" runat="server" Text="Dati Fondo" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Servizio Utile:</label>
            </td>
            <td class="field fileds-date-input" style="width: 25%">
                <asp:TextBox ID="txtServizioUtileAA" runat="server" CssClass="tb8 txtUppercase" MaxLength="2"
                    Width="22%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileAA" ControlToValidate="txtServizioUtileAA"
                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile AA: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="^[0-9]*$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileAA" ControlToValidate="txtServizioUtileAA"
                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile AA: campo obbligatorio"
                    ValidationGroup="UCTabDatiFondoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <label style="width: 22%">
                    AA</label>
                <asp:TextBox ID="txtServizioUtileMM" runat="server" CssClass="tb8 txtUppercase" MaxLength="2"
                    Width="22%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtServizioUtileMM" ControlToValidate="txtServizioUtileMM"
                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile MM: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="^[0-9]*$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtServizioUtileMM" ControlToValidate="txtServizioUtileMM"
                    Display="Dynamic" Enabled="true" ErrorMessage="Servizio Utile MM: campo obbligatorio"
                    ValidationGroup="UCTabDatiFondoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <label style="width: 22%">
                    MM</label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Retribuzione Pens.:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtRetribuzionePensionabile" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtRetribuzionePensionabile"
                    ControlToValidate="txtRetribuzionePensionabile" Display="Dynamic" Enabled="true"
                    ErrorMessage="Retribuzione Pens.: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="\d{1,6}(,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtRetribuzionePensionabile" ControlToValidate="txtRetribuzionePensionabile"
                    Display="Dynamic" Enabled="true" ErrorMessage="Retribuzione Pens.: campo obbligatorio"
                    ValidationGroup="UCTabDatiFondoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Controcodice:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtControcodice" runat="server" CssClass="tb8 txtUppercase" MaxLength="3" Width="30%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtControcodice" ControlToValidate="txtControcodice"
                    Display="Dynamic" Enabled="true" ErrorMessage="Controcodice: Inserire numeri"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="^[0-9]*$" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtControcodice" ControlToValidate="txtControcodice"
                    Display="Dynamic" Enabled="true" ErrorMessage="Controcodice: campo obbligatorio"
                    ValidationGroup="UCTabDatiFondoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</div>
<asp:Panel ID="pnlFondoES" runat="server" Visible="false">
<!-- Pannello CODICI -->
<div id="div1" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label1" runat="server" Text="Codici" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Esattoria:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtboxCodiceEsattoria" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                    Width="50%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegExpValidCodiceEsattoria" ControlToValidate="txtboxCodiceEsattoria"
                    Display="Dynamic" Enabled="true" ErrorMessage="Codice Esattoria: Inserire valori interi compresi tra 0 e 9999"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="[0-9]*" />
               <%-- <asp:RequiredFieldValidator runat="server" ID="RegExpReqCodiceEsattoria" ControlToValidate="txtboxCodiceEsattoria"
                    Display="Dynamic" Enabled="true" ErrorMessage="Codice Esattoria: campo obbligatorio"
                    ValidationGroup="UCTabDatiFondoGAS" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>--%>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Classe Ante 50:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtboxClasseAnte50" runat="server" CssClass="tb8 txtUppercase" MaxLength="2"
                    Width="50%"></asp:TextBox>
 <%--               <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator5" ControlToValidate="txtboxClasseAnte50"
                    Display="Dynamic" Enabled="true" ErrorMessage="Classe Ante 50: Inserire valori interi"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ValidationExpression="^[0-9]*$" />--%>
                <asp:RangeValidator ID="RangeValClasseAnte50" runat="server" Display="Dynamic" ErrorMessage="Classe Ante 50: Inserire valori interi compresi tra 0 e 18 "
                    Type="Integer" Enabled="true" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiFondoGAS" ControlToValidate="txtboxClasseAnte50"
                    MinimumValue="0" MaximumValue="18" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Articolo 58:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlArticolo58" CssClass="tb8 txtUppercase" Width="50%" runat="server">
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Articolo 59:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlArticolo59" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Optanti:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlOptanti" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Saltuari:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlSaltuari" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Promiscui:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlPromiscui" CssClass="tb8 txtUppercase" Width="50%" runat="server">
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice ES/DZ:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlCodiceEsDz" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Anno Utile:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlAnnoUtile" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Retribuzione:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlCodiceRetribuzione" CssClass="tb8 txtUppercase" Width="50%" runat="server">
                    <asp:ListItem Text=""  Value="" ></asp:ListItem>
                    <asp:ListItem Text="0" Value="0"></asp:ListItem>
                    <asp:ListItem Text="1" Value="1"></asp:ListItem>
                    <asp:ListItem Text="2" Value="2"></asp:ListItem>
                    <asp:ListItem Text="3" Value="3"></asp:ListItem>
                    <asp:ListItem Text="4" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Maggiorazione Privilegiata:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList ID="ddlMaggiorazionePrivilegiata" CssClass="tb8 txtUppercase xxs" Width="50%" runat="server">
                    <asp:ListItem Text="NO" Value="false"></asp:ListItem>
                    <asp:ListItem Text="SI" Value="true"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>
</div>
<!-- Pannello ELEMENTI DI CALCOLO -->
<div id="divElementiDiCalcolo" style="border-style: solid; border-color: #000080;
    border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px;
    margin-top: 4px;" runat="server">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label2" runat="server" Text="Elementi di calcolo" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvElementiDiCalcolo" SkinID="grdElenco1" AutoGenerateColumns="False"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1px" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvElementiCalcolo_RowEditing" Width="100%" AllowPaging="True" OnRowCommand="gvElementiCalcolo_RowCommand"
                        OnRowCancelingEdit="gvElementiCalcolo_RowCancelingEdit" OnRowUpdating="gvElementiCalcolo_RowUpdating"
                        OnRowDataBound="gvElementiCalcolo_RowDataBound" OnRowDeleting="gvElementiCalcolo_RowDeleting"
                        OnPageIndexChanging="gvElementiCalcolo_onPageIndexChanging" EnableModelValidation="True" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Mesi Servizio Utile" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                   
                                     <asp:Label runat="server" ID="txtMeseServizioUtile" Text='<%# Bind("MesiServizioUtile") %>'
                                        CssClass="txtUppercase">       
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtLabelMeseServizioUtile"
                                        MaxLength="7" Text='<%# Bind("MesiServizioUtile")%>' Width="100px" ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" ></asp:TextBox>
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredTxtLabelMeseServizioUtile"
                                        ControlToValidate="txtLabelMeseServizioUtile" Enabled="true" ErrorMessage="Mesi Servizio Utile : E' un campo obbligatorio"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtLabelMeseServizioUtile"
                                        Display="Dynamic" ControlToValidate="txtLabelMeseServizioUtile" Enabled="true"
                                        ErrorMessage="Mesi Servizio Utile : Deve essere un intero" Text="*" CssClass="field-is-required" ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo"
                                        ValidationExpression="^[0-9]*$" />
                                </EditItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella Row1"></HeaderStyle>
                                <ItemStyle CssClass="TblRecordset3"></ItemStyle>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Retribuzione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" >
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="txtRetribuzione" Text='<%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "Retribuzione")).ToString("######.####") %>' > 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                   <%-- <asp:TextBox ID="txtLabelRetribuzione" runat="server" CssClass="tb8" MaxLength="11"
                                        Width="63%" Text='<%# Bind("Retribuzione") %>' ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" ></asp:TextBox>--%>
                                    <asp:TextBox ID="txtLabelRetribuzione" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                                        Width="63%" Text='<%# Convert.ToDecimal(DataBinder.Eval(Container.DataItem, "Retribuzione")).ToString("######.####") %>' ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" ></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRetribuzione" ControlToValidate="txtLabelRetribuzione"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Retribuzione Pens.: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
                                    <asp:RequiredFieldValidator runat="server" ID="requireTxtRetribuzione" ControlToValidate="txtLabelRetribuzione"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Retribuzione Pens.: campo obbligatorio"
                                        ValidationGroup="UCDatiFondoGAS_ES_GvElementiCalcolo" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
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
</asp:Panel>
<!-- Pannello bottoni -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;" class="tab-actions-group">
        <tr>
            <td style="text-align: right; vertical-align: bottom;" class="tab-actions-group__first">
                <asp:Button ID="btnSalvaDatiFondo" runat="server" CausesValidation="false" ValidationGroup="UCTabDatiFondoGAS"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalvaDatiFondo_Click" Text="Salva Dati Fondo"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiFondoGAS')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
            </td>
            <td style="text-align: left; vertical-align: bottom;">
                <asp:Button ID="btnEliminaDatiFondo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Dati Fondo" Width="150px" OnClick="btnEliminaDatiFondo_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Fondo?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
