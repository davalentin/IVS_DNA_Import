<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCProrataCi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviCi.UCProrataCi" %>
<script type="text/javascript">

    function GetHdnIsInProrata() {
        var isInProrata = "false";
        if (document.getElementById("<%=hdnIsInProrata.ClientID%>") != null) {
            isInProrata = document.getElementById("<%=hdnIsInProrata.ClientID%>").value;
        }
        return isInProrata;
    }

    function checkEta(source, args) {
        var eta = args.Value;
        if (eta > 255)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function ApplicazioneArt48Checked() {
        var chkApplArt48 = document.getElementById("<%=chkApplicazioneArt48.ClientID %>");
        var txtDecorrenzaArt48 = document.getElementById("<%=txtDecorrenzaArt48.ClientID %>");
        var revDecorrenzaArt48 = document.getElementById("<%=REDecorrenzaArt48.ClientID %>");
        var cvDecorrenzaArt48 = document.getElementById("<%=customCheckDataDecorrenzaArt48.ClientID %>");
        var rfvDecorrenzaArt48 = document.getElementById("<%=RFDecorrenzaArt48.ClientID %>");
        var isBloccoArt48 = document.getElementById("<%=hdnIsBloccoArt48.ClientID %>");
        var isArt48Checked = document.getElementById("<%=hdnIsArt48Checked.ClientID %>");

        if (chkApplArt48 != null && txtDecorrenzaArt48 != null && revDecorrenzaArt48 != null &&
            cvDecorrenzaArt48 != null && rfvDecorrenzaArt48 != null && isArt48Checked != null) {
            if (chkApplArt48.checked) {
                isArt48Checked.value = 'true';
                if (isBloccoArt48.value == 'true') {
                    chkApplArt48.disabled = true;
                    if (txtDecorrenzaArt48.value == '') {
                        DisabilitaValidatore(revDecorrenzaArt48);
                        DisabilitaValidatore(cvDecorrenzaArt48);
                        DisabilitaValidatore(rfvDecorrenzaArt48);
                        txtDecorrenzaArt48.disabled = true;
                        $(txtDecorrenzaArt48).datepicker('disable');
                    }
                }
                else {
                    AbilitaValidatore(revDecorrenzaArt48);
                    AbilitaValidatore(cvDecorrenzaArt48);
                    AbilitaValidatore(rfvDecorrenzaArt48);
                    txtDecorrenzaArt48.removeAttribute('disabled');
                    $(txtDecorrenzaArt48).datepicker('enable');
                }
            }
            else {
                isArt48Checked.value = 'false';
                DisabilitaValidatore(revDecorrenzaArt48);
                DisabilitaValidatore(cvDecorrenzaArt48);
                DisabilitaValidatore(rfvDecorrenzaArt48);
                txtDecorrenzaArt48.disabled = true;
                txtDecorrenzaArt48.value = '';
                $(txtDecorrenzaArt48).datepicker('disable');
            }
        }
    }

    $(document).ready(function () {
        ApplicazioneArt48Checked();
    });

</script>
<asp:Panel ID="pnlTable" runat="server">
    <%-- Tabella visualizzazione elenco contribuzioni estere--%>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div id="divStati" class="bckGridViewElenco" style="width: 700px">
                    <asp:GridView ID="gvIstituzioniEstere" SkinID="grdElenco1" BorderWidth="1" BorderColor="Black"
                        DataKeyNames="Id" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" OnDataBinding="gvIstituzioniEstere_DataBinding"
                        OnRowCommand="gvIstituzioniEstere_RowCommand" OnRowDataBound="gvIstituzioniEstere_RowDataBound">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato trovato." SkinID="lblNoData"
                                    Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="img" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField HeaderText="Stato" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="38%"
                                HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                DataField="nomeStato"></asp:BoundField>
                            <asp:BoundField HeaderText="Istituzione" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="32%"
                                HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                                DataField="codiceIstituzione" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                            <asp:BoundField DataField="id" HeaderText="Id" Visible="False" />
                            <asp:TemplateField HeaderText="Operazione" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="30%"
                                ControlStyle-CssClass="pulsante1 editIconOnly tertiary">
                                <ItemTemplate>
                                    <asp:Button runat="server" ID="btnModifica" Text="Modifica" CommandName="modifica"
                                        CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" SkinID="btnAzione1" CssClass="editIconOnly tertiary" />
                                </ItemTemplate>
                                <HeaderStyle CssClass="intestazioneTabella Row1" />
                                <ItemStyle Width="26%" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <%--Fine tabelle visualizzazione elenco contribuzioni estere--%>
</asp:Panel>
<asp:Panel ID="editpan" runat="server" Visible="False">
    <table id="editab" width="100%" class="tabellaFormattazione grid grid-size-25">
        <tr>
            <td class="Row1 if-empty-none" colspan="4">
                <asp:Label ID="lblIdPrestazioneEE" runat="server" Visible="false" />
            </td>
        </tr>
        <%-- ENG - Avviso per Stato Croazia --%>
        <tr>
            <td class="Row1 if-empty-none" colspan="4">
                <asp:Label ID="lblAvvisoStatoCroazia" runat="server" Style="font-weight: bold" ForeColor="Black" Text="Dal 1° gennaio 2023 l'importo della prestazione estera deve essere espresso in EURO"
                    Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Stato/Istituzione:</label>
            </td>
            <td class="Row1 full-grid" colspan="3">
                <asp:Label ID="lblCodiceStatoEE" runat="server" />
                <asp:Label ID="lblCodiceIstituzione" runat="server" />
                <asp:Label ID="lblSigla" runat="server" />
                <asp:Label ID="lblCitta" runat="server" />
                <asp:Label ID="lblNomeStato" runat="server" Visible="false" />
                <asp:Label ID="lblMatricolaIstituzioneEE" runat="server" Visible="false" />
                <asp:Label ID="lblCodicePi" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Data Precedente Liquidazione:</label>
            </td>
            <td class="Row1" style="width: 25%;">
                <asp:TextBox MaxLength="7" Width="123px" ID="txtDataPrecedenteLiquidazione" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                    runat="server" TabIndex="1"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataPrecedenteLiquidazione"
                    ControlToValidate="txtDataPrecedenteLiquidazione" Display="Dynamic" Enabled="true"
                    ErrorMessage="Data Precedente Liquidazione: inserire la data nel formato valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrata"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataPrecedenteLiquidazione"
                    Display="Dynamic" ErrorMessage="Data Precedente Liquidazione: data illogica"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrata" ID="customCheckDataDataPrecedenteLiquidazione"
                    ClientValidationFunction="checkCorrettezzaData" />
            </td>
            <td class="Row1">
                <label>
                    Data Ricalcolo:</label>
            </td>
            <td class="Row1 " style="width: 25%">
                <asp:TextBox Width="123px" ID="txtDataRicalcolo" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                    runat="server" MaxLength="7" TabIndex="2"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDataRicalcolo" ControlToValidate="txtDataRicalcolo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Data Ricalcolo: inserire la data nel formato valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrata"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataRicalcolo" Display="Dynamic"
                    ErrorMessage="Data Ricalcolo: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrata"
                    ID="customCheckDataDataRicalcolo" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Settimane misura a decorrenza pensione:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtSettimaneMisuraDecorrenzaPensione" Width="123px" runat="server"
                    CssClass="tb8 txtUppercase" MaxLength="4" TabIndex="3"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RESettimaneMisuraDecorrenzaPensione" ControlToValidate="txtSettimaneMisuraDecorrenzaPensione"
                    ErrorMessage="Settimane misura a decorrenza pensione: settimane in formato non valido"
                    ValidationExpression="^[0-9]+" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabProrata"
                    Enabled="true" />
            </td>
            <td class="Row1">
                <label>
                    Settimana a ricalcolo:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtSettimaneARicalcolo" runat="server" Width="123px" CssClass="tb8 txtUppercase"
                    MaxLength="4" TabIndex="4"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RESettimaneARicalcolo" ControlToValidate="txtSettimaneARicalcolo"
                    ErrorMessage="Settimane a ricalcolo: settimane in formato non valido" ValidationExpression="^[0-9]+"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabProrata" Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Settimane Diritto:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtSettimaneDiritto" runat="server" Width="123px" CssClass="tb8 txtUppercase"
                    MaxLength="4" TabIndex="5"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RESettimaneDiritto" ControlToValidate="txtSettimaneDiritto"
                    ErrorMessage="Settimane diritto: settimane in formato non valido" ValidationExpression="^[0-9]+"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabProrata" Enabled="true" />
            </td>
            <td class="Row1 ">
            </td>
            <td class="Row1 ">
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Sospensione Integrazione Trattamento minimo:</label>
            </td>
            <td class="Row1">
                <asp:DropDownList runat="server" ID="ddlSospensioneIntegrazioneTrattamentoMinimo"
                    CssClass="tb8 txtUppercase xxs" Width="30%">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlSospensioneIntegrazioneTrattamentoMinimo"
                    ControlToValidate="ddlSospensioneIntegrazioneTrattamentoMinimo" Display="Dynamic"
                    ErrorMessage="Sospensione Integrazione Trattamento minimo obbligatorio" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabProrata" Enabled="true"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1">
                <label>
                    Età:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtEta" runat="server" Width="123px" CssClass="tb8 txtUppercase"
                    MaxLength="3" TabIndex="7"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REEta" ControlToValidate="txtEta" ErrorMessage="Età: anni in formato non valido"
                    ValidationExpression="^[0-9]+$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabProrata"
                    Enabled="true" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Applicazione art.48 n.1408/1971 o analoga disposizione:</label>
            </td>
            <td class="Row1 ">
                <asp:CheckBox ID="chkApplicazioneArt48" CssClass="tb8" runat="server" TabIndex="8"
                    onClick="ApplicazioneArt48Checked()"></asp:CheckBox>
            </td>
            <td class="Row1">
                <label>
                    Decorrenza:</label>
            </td>
            <td class="Row1 ">
                <asp:TextBox Width="123px" ID="txtDecorrenzaArt48" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                    runat="server" MaxLength="7" TabIndex="9"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REDecorrenzaArt48" ControlToValidate="txtDecorrenzaArt48"
                    Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Art. 48: inserire la data nel formato valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrata"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaArt48" Display="Dynamic"
                    ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrata"
                    ID="customCheckDataDecorrenzaArt48" ClientValidationFunction="checkCorrettezzaData" />
                <asp:RequiredFieldValidator ID="RFDecorrenzaArt48" ControlToValidate="txtDecorrenzaArt48"
                    ErrorMessage="Decorrenza Art. 48: la data è obbligatoria" Display="Dynamic" runat="server"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrata" Enabled="false" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <asp:Label ID="lblIntegrazioneExJugoslavia" runat="server"></asp:Label>
            </td>
            <td class="Row1 ">
                <asp:TextBox ID="txtIntegrazioneExJugoslavia" Width="123" runat="server" CssClass="tb8 txtUppercase"
                    MaxLength="16" TabIndex="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REIntegrazioneExJugoslavia" ControlToValidate="txtIntegrazioneExJugoslavia"
                    ErrorMessage="Integrazione a carico dell'ex Jugoslavia: importo in formato non valido"
                    ValidationExpression="^[0-9]+\,[0-9]+|[0-9]+$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                    ValidationGroup="UCTabProrata" Enabled="true" />
            </td>
            <td class="Row1">
                <asp:Label ID="lblDecorrenzaIntegrazione" runat="server" Text="Decorrenza Integrazione:"></asp:Label>
            </td>
            <td class="Row1 ">
                <asp:TextBox Width="123px" ID="txtDecorrenzaIntegrazione" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                    runat="server" MaxLength="7" TabIndex="11"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REDecorrenzaIntegrazione" ControlToValidate="txtDecorrenzaIntegrazione"
                    Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Integrazione: inserire la data nel formato valido"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrata"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaIntegrazione"
                    Display="Dynamic" ErrorMessage="Decorrenza Integrazione: data illogica" Text="*" CssClass="field-is-required"
                    ValidationGroup="UCTabProrata" ID="customCheckDataDecorrenzaIntegrazione" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
    </table>
    <asp:Label runat="server" ID="lblCodiceConvenzione" Visible="false"></asp:Label>
    <asp:Panel ID="pnlImportiEsteri" runat="server">
        <table class="tabellaContenuti">
            <tr>
                <td class="Row1">
                    <div id="divImportiEsteri" class="bckGridViewElenco" style="width: 700px">
                        <asp:GridView runat="server" ID="gvImportiEsteri" SkinID="grdElenco1" AutoGenerateColumns="false"
                            CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%" BorderColor="Black"
                            AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvImportiEsteri_RowCommand"
                            OnRowDataBound="gvImportiEsteri_RowDataBound" OnRowCancelingEdit="gvImportiEsteri_RowCancelingEdit"
                            OnRowEditing="gvImportiEsteri_RowEditing" OnRowUpdating="gvImportiEsteri_RowUpdating"
                            OnPageIndexChanging="gvImportiEsteri_onPageIndexChanging" EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
                            <Columns>
                                <asp:TemplateField HeaderText="Decorrenza" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDecorrenzaPrestazioneEE" Text='<%#Bind("strDecorrenzaPrestazione", "{0:MM/yyyy}")%>'
                                            Width="100px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtDecorrenzaPrestazioneEE" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                                            Text='<%#Bind("strDecorrenzaPrestazione", "{0:MM/yyyy}")%>' Width="100px" MaxLength="7">      
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="validateDecorrenzaPrestazioneEE" runat="server" ErrorMessage="Decorrenza Prestazione Estera: dato obbligatorio"
                                            Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaPrestazioneEE" ValidationGroup="UCTabProrataGrid"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator runat="server" ID="REDecorrenzaPrestazioneEE" ControlToValidate="txtDecorrenzaPrestazioneEE"
                                            Display="Dynamic" Enabled="true" ErrorMessage="Decorrenza Prestazione Estera: inserire la data nel formato valido"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrataGrid"
                                            Text="*" CssClass="field-is-required" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPrestazioneEE"
                                            Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrataGrid"
                                            ID="customCheckDataDecorrenzaPrestazioneEE" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Importo Prestazione" ItemStyle-HorizontalAlign="Center"
                                    HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblImportoPrestazioneEE" Text='<%#Bind("strImportoPrestazione")%>'
                                            Width="120px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtImportoPrestazioneEE" Text='<%#Bind("strImportoPrestazione")%>'
                                            CssClass="tb8 txtUppercase" Width="120px" MaxLength="26"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="validateImportoPrestazioneEE" runat="server" ErrorMessage="Importo Prestazione Estera: dato obbligatorio"
                                            Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoPrestazioneEE" ValidationGroup="UCTabProrataGrid"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="REImportoPrestazioneEE" ControlToValidate="txtImportoPrestazioneEE"
                                            ErrorMessage="Importo Prestazione Estera: importo in formato non valido" ValidationExpression="^[0-9]+\,[0-9]+|[0-9]+$"
                                            runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabProrataGrid"
                                            Enabled="true" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cessazione" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="intestazioneTabella Row1"
                                    ItemStyle-CssClass="TblRecordset3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCessazionePrestazioneEE" Text='<%#Bind("strCessazionePrestazione", "{0:MM/yyyy}")%>'
                                            Width="100px"></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtCessazionePrestazioneEE" Text='<%#Bind("strCessazionePrestazione", "{0:MM/yyyy}")%>'
                                            CssClass="tb8 txtUppercase date-picker dateMMaaaa" Width="100px" MaxLength="7"></asp:TextBox>
                                        <asp:RegularExpressionValidator runat="server" ID="RECessazionePrestazioneEE" ControlToValidate="txtCessazionePrestazioneEE"
                                            Display="Dynamic" Enabled="true" ErrorMessage="Cessazione Prestazione Estera: inserire la data nel formato valido"
                                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabProrataGrid"
                                            Text="*" CssClass="field-is-required" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtCessazionePrestazioneEE"
                                            Display="Dynamic" ErrorMessage="Cessazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabProrataGrid"
                                            ID="customCheckDataCessazionePrestazioneEE" ClientValidationFunction="checkCorrettezzaData" />
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                            runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:HiddenField runat="server" ID="modalitaEditImporti" Value="false" />
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right;" class="tab-actions-group__first">
                    <asp:Button ID="btnConfermaModifiche" SkinID="btnAzione1" runat="server" Text="Salva Stato"
                        OnClick="ConfermaModifiche_Click" ValidationGroup="UCTabProrata" Width="160px"
                        OnClientClick="if(Page_ClientValidate('UCTabProrata')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left;">
                    <asp:Button ID="btnAnnullaModifiche" SkinID="btnAzione1" runat="server" Text="Indietro"
                        OnClick="AnnullaModifiche_Click" Width="160px" OnClientClick="BlockUI()" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hdnNRecordProrata" Value="" />
    <asp:HiddenField runat="server" ID="hdnIsInProrata" Value="" />
    <asp:HiddenField runat="server" ID="hdnIsBloccoArt48" Value="" />
    <asp:HiddenField runat="server" ID="hdnIsArt48Checked" Value="" />
</asp:Panel>
<div runat="server" style="margin-top: 25px; margin-right: 40px;" id="divSalvaProrata"
    visible="false" class="containerWidth xs">
    <table width="100%">
        <tr>
            <td colspan="4" style="text-align: center;">
                <asp:Button ID="btnCancelProRata" SkinID="btnAzione1" runat="server" Text="Elimina e Riacquisisci Stati"
                    Style="padding-left: 0px; padding-right: 0px;" CausesValidation="false" OnClick="btnCancelProRata_Click"
                    Width="180px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i dati di Pro Rata Estera?')) return false; else BlockUI();" CssClass="ghost-delete" />
            </td>
        </tr>
    </table>
</div>
