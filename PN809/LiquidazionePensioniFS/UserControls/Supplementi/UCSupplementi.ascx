<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSupplementi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Supplementi.UCSupplementi" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<script type="text/javascript">
    function CleanFields1() {
        $($("table[id*=gvSupplementi] input[type=text][id*=txtDecorrenzaSupplementi]")).val('');
        $($("table[id*=gvSupplementi] select[id*=ddlTipoSupplementi]")).val('');
        $($("table[id*=gvSupplementi] input[type=text][id*=txtSettimaneSupplementi]")).val('');
        $($("table[id*=gvSupplementi] select[id*=ddlRSupplementi]")).val('');
        $($("table[id*=gvSupplementi] input[type=text][id*=txtRMSSupplementi]")).val('');
        $($("table[id*=gvSupplementi] select[id*=ddlQuotaSupplementi]")).val('');
        $($("table[id*=gvSupplementi] input[type=text][id*=txtMontanteSupplementi]")).val('');
        return false;
    }

    function checkDataDecorrenzaOriginaria(source, args) {
        var decorrenzaOriginaria = document.getElementById("<%=hfDecorrenzaOriginaria.ClientID%>").value;
        if (decorrenzaOriginaria != null) {
            var dateDecOrig = decorrenzaOriginaria.split("/");
            var annodecOrig = dateDecOrig[2].substr(0, 4);
            var mesedecOrig = dateDecOrig[1];
        }

        var gridViewID = "<%=gvSupplementi.ClientID %>";
        var inputs = [];
        inputs = document.getElementById(gridViewID).getElementsByTagName("input");
        if (inputs != null) {
            var txtDec = inputs[0].value;
            var dateDec = txtDec.split("/");
            var annodec = dateDec[1];
            var mesedec = dateDec[0];
        }

        if (decorrenzaOriginaria != null && inputs != null) {
            //trasformo le date nel formato aaaammgg (es. 200811)
            data1str = annodecOrig + mesedecOrig;
            data2str = annodec + mesedec;
            //controllo se la seconda data è successiva alla prima
            if (data2str - data1str >= 0)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        return false;

    }

</script>
<asp:Panel runat="server" ID="pnlSupplementi">
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco full-size" style="width: 700px">
                    <asp:GridView runat="server" ID="gvSupplementi" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="True"
                        OnRowEditing="gvSupplementi_RowEditing" Width="100%" PageSize="10" AllowPaging="true"
                        OnPageIndexChanging="gvSupplementi_onPageIndexChanging" OnRowCommand="gvSupplementi_RowCommand"
                        OnRowCancelingEdit="gvSupplementi_RowCancelingEdit" OnRowUpdating="gvSupplementi_RowUpdating"
                        OnRowDataBound="gvSupplementi_RowDataBound" OnRowDeleting="gvSupplementi_RowDeleting" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataTemplate>
                            <center>
                                        <asp:Label ID="lblNoData" runat="server" Text="Nessun supplemento presente."
                                            SkinID="lblNoData"></asp:Label>
                                    </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="C/R" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRSupplementi" Text='<%#Bind("TipoSupplemento")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlRSupplementi" runat="server"
                                        Width="35px">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="R" Value="R"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlRSupplementi" runat="server" ErrorMessage="C/R: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlRSupplementi" ValidationGroup="UCTabSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTipoSupplementi" Text='<%#Bind("CodGestioneSupplemento")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlTipoSupplementi" CommandArgument='<%#Eval("Id")%>' AutoPostBack="True" runat="server"
                                        Width="40px" OnSelectedIndexChanged="ddlTipoSupplementi_SelectedIndexChanged">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CustomValidator runat="server" ID="requiredddlTipoSupplementi" ControlToValidate="ddlTipoSupplementi"
                                        Display="Dynamic" ErrorMessage="Scegliere il tipo = 1" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi"
                                        ClientValidationFunction="checkddlTipoSupplementi" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlTipoSupplementi" runat="server" ErrorMessage="Tipo Supplementi: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlTipoSupplementi" ValidationGroup="UCTabSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaSupplementi" Text='<%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaSupplementi"
                                        MaxLength="7" Text=' <%# Bind("DecorrenzaSupplemento", "{0:MM/yyyy}")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaSupplementi"
                                        Display="Dynamic" ControlToValidate="txtDecorrenzaSupplementi" Enabled="true"
                                        ErrorMessage="Decorrenza Supplementi: Inserire una data valida" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <%--<asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                        Display="Dynamic" ErrorMessage="Decorrenza Supplementi: Data inserita posteriore a quella odierna"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi" ID="customDecorrenzaSupplementi" ClientValidationFunction="checkDataPostOdiernaMMAAAA" />--%>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                        Display="Dynamic" ErrorMessage="Decorrenza Supplementi: Data inserita anteriore alla Data Decorrenza Pensione"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi" ID="customcheckDataDecorrenzaOriginaria"
                                        ClientValidationFunction="checkDataDecorrenzaOriginaria" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldlblDecorrenzaSupplementi" runat="server"
                                        ErrorMessage="Decorrenza Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaSupplementi"
                                        ValidationGroup="UCTabSupplementi" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaSupplementi"
                                        Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi"
                                        ID="customCheckDataDecorrenzaSupplemento" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimaneSupplementi" Text='<%#Bind("NSettimaneSupplemento")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneSupplementi" runat="server"
                                        Text='<%#Bind("NSettimaneSupplemento")%>' Width="60px" MaxLength="4">
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validatetxtSettimaneSupplementi"
                                        Display="Dynamic" ControlToValidate="txtSettimaneSupplementi" Enabled="true"
                                        ErrorMessage="N Settimane Supplementi: Inserire valori numerici" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi"
                                        ValidationExpression="^[0-9]*$" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneSupplementi" runat="server"
                                        ErrorMessage="N Settimane Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneSupplementi"
                                        ValidationGroup="UCTabSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="RMS" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRMSSupplementi"> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtRMSSupplementi" runat="server" Width="100px"
                                        MaxLength="9">
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validatetxtRMSSupplementi" Display="Dynamic"
                                        ControlToValidate="txtRMSSupplementi" Enabled="true" ErrorMessage="RMS Supplementi: inserire l'importo in formato valido (max 6 interi e 2 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi" ValidationExpression="\d{0,6}(,\d{1,2})?" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuotaSupplementi" Text='<%#Bind("QuotaSupplemento")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase xxs" ID="ddlQuotaSupplementi" runat="server"
                                        Width="50px">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CustomValidator runat="server" ID="requiredddlQuotaSupplementi" ControlToValidate="ddlQuotaSupplementi"
                                        Display="Dynamic" ErrorMessage="Scegliere quota di tipo = B" Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi"
                                        ClientValidationFunction="checkddlQuotaSupplementi" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuotaSupplementi" runat="server"
                                        ErrorMessage="Quota Supplementi: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="ddlQuotaSupplementi"
                                        ValidationGroup="UCTabSupplementi"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Montante" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="txtMontanteSupplementi" Text='<%#Bind("MontanteSupplemento")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtMontanteSupplementi" runat="server"
                                        Text='<%#Bind("MontanteSupplemento")%>' Width="110px" MaxLength="15" />
                                    <asp:RegularExpressionValidator runat="server" ID="validatetxtMontanteSupplementi"
                                        Display="Dynamic" ControlToValidate="txtMontanteSupplementi" Enabled="true" ErrorMessage="Montante Supplementi: Inserire valori interi o decimali"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabSupplementi" ValidationExpression="\d+(\,\d{1,4})?" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%">
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
    <div id="pulsantiSaveDelete" style="margin-top: 200px; margin-right: 40px;" class="containerWidth xl">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaTabSupplementi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Supplementi" Width="160px" OnClick="btnSalvaTabSupplementi_Click"
                        OnClientClick="BlockUI();" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaTabSupplementi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Supplementi" Width="160px" OnClick="btnEliminaTabSupplementi_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Supplementi?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="hfDecorrenzaOriginaria" />
    <asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
</asp:Panel>
