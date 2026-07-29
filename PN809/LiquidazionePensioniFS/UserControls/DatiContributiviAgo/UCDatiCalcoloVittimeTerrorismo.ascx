<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiCalcoloVittimeTerrorismo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCDatiCalcoloVittimeTerrorismo" %>
<script type="text/javascript">
    $(document).ready(function () {
        $("table[id$='gvDatiRetributiviVittime'] table[id$='gvDatiContributiviVittime']").ready(function () {
            var that = this;

            $(this).find("input[id$='txtDecorrenzaBeneficio']").each(function () {
                decorrenzaBeneficioOnChange(this, that);
            });

            $(this).find("input[id$='txtDecorrenzaBeneficio']").change(function () {
                decorrenzaBeneficioOnChange(this, that);
            });
        });
        $("table[id$='gvDatiRetributiviVittime']").ready(function () {
            $(this).find("select[id$='ddlCodiceGestione']").change(function () {
                switchCodiceTipoQuota(this);
            });
        });
        $("table[id$='gvDatiRetributiviVittime'] select[id$='ddlCodiceGestione']").each(function () {
            switchCodiceTipoQuota($(this));
        });
    });

    function decorrenzaBeneficioOnChange(textbox, grid) {
        var soggettoBeneficiario = $("#<%= hdnSoggettoBeneficiario.ClientID %>").val();
        var tipologiaPrestazione = $("#<%= hdnTipologiaPrestazione.ClientID %>").val();
        var tipologiaBeneficio = $("#<%= hdnTipologiaBeneficio.ClientID %>").val();

        if ((soggettoBeneficiario == 4 || soggettoBeneficiario == 5 || soggettoBeneficiario == 6 || soggettoBeneficiario == 7 || soggettoBeneficiario == 8) &&
                    tipologiaBeneficio == 2 && tipologiaPrestazione == 2) {
            var decorrenzaBeneficio = $(textbox).val();
            if (decorrenzaBeneficio.indexOf("/") !== -1) {
                var dateApp = decorrenzaBeneficio.split("/");
                var date = new Date(dateApp[1], dateApp[0] - 1, 1);

                var ddl = $(grid).find("select[id$='ddlBeneficio']");

                if (date > new Date(2006, 11, 1)) {
                    ddl.val("Y");
                    ddl.attr("disabled", true);
                }
                else {
                    ddl.removeAttr("disabled");
                }
            }
        }
    }

    function riabilitaCampi() {
        $("table[id$='gvDatiRetributiviVittime'] table[id$='gvDatiContributiviVittime']").ready(function () {
            var that = this;

            $(this).find("select[id$='ddlBeneficio']").each(function () {
                $(this).removeAttr("disabled");
            });
        });
    }

    function switchCodiceTipoQuota(itemCodiceGestione) {
        var ddlTipoQuotaGestioneA = $(itemCodiceGestione).closest("tr").find("select[id$='ddlTipoQuotaGestioneA']");
        var ddlTipoQuotaGestioneAltre = $(itemCodiceGestione).closest("tr").find("select[id$='ddlTipoQuotaGestioneAltre']");

        if ($(itemCodiceGestione).find('option:selected').text().split(' - ')[0] == 'A') {
            // riporto il valore dall'altra dropdownlist
            $(ddlTipoQuotaGestioneA).val($(ddlTipoQuotaGestioneAltre).val());
            // svuoto il valore dell'altra dropdownlist
            $(ddlTipoQuotaGestioneAltre).val('');
            $(ddlTipoQuotaGestioneAltre).hide();
            $(ddlTipoQuotaGestioneA).show();
        }
        else {
            // riporto il valore dall'altra dropdownlist
            $(ddlTipoQuotaGestioneAltre).val($(ddlTipoQuotaGestioneA).val());
            // svuoto il valore dell'altra dropdownlist
            $(ddlTipoQuotaGestioneA).val('');
            $(ddlTipoQuotaGestioneAltre).show();
            $(ddlTipoQuotaGestioneA).hide();
        }
    }
</script>
<asp:Panel runat="server" ID="pnlDatiCalcoloVittime">
    <br />
    <table class="tabellaFormattazione" width="100%">
        <tr>
            <td class="Row1" style="width: 62%;">
                <label style="font-weight: bold" class="section-label">
                    Dati calcolo terrorismo
                </label>
            </td>
        </tr>
        <tr>
            <td class="Row1 shift-full-grid">
                <label style="font-weight: bold; color: Red">
                    Verificare che sia stato compilato il quadro “Maggiorazioni/Benefici”.</label>
            </td>
        </tr>
    </table>
    <div id="pdivRetributivo" runat="server" class="mt-32">
        <table class="tabellaFormattazione" style="width: 100%">
            <tr>
                <td class="Row1">
                    <label class="section-label">
                        Dati Retributivi Vittime</label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvDatiRetributiviVittime" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" AllowPaging="false" OnRowCommand="gvDatiRetributiviVittime_RowCommand"
                        OnRowDataBound="gvDatiRetributiviVittime_RowDataBound" OnRowCancelingEdit="gvDatiRetributiviVittime_RowCancelingEdit"
                        OnRowEditing="gvDatiRetributiviVittime_RowEditing" OnRowUpdating="gvVittime_RowUpdating"
                        EnableViewState="true" OnDataBound="gvDatiRetributiviVittime_DataBound" OnLoad="gvDatiRetributiviVittime_Load">
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="18%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaBeneficio" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaBeneficio"
                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%#Bind("DecorrenzaBeneficio") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenzaBeneficio"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Beneficio"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        Text="*" CssClass="field-is-required" />
                                    <asp:RequiredFieldValidator ID="RFVtxtDecorrenzaBeneficio" runat="server" ErrorMessage="Decorrenza Beneficio: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaBeneficio" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaBeneficio" Display="Dynamic"
                                        ErrorMessage="Decorrenza Beneficio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="18%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="35px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="A" Value="A"></asp:ListItem>
                                        <asp:ListItem Text="B" Value="B"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuota" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" Visible="false">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblTipoQuota_item" Width="130px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlTipoQuotaGestioneA" Width="130px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:DropDownList runat="server" ID="ddlTipoQuotaGestioneAltre" Width="40px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="4" Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVittimeRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloVittimeRetr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Reddito / Retribuzione Media" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="18%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblRetribuzioneMedia"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRetribuzioneMedia" Width="100px"
                                        CssClass="txtUppercase tb8 " MaxLength="12" Text=' <%# Bind("RMS")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtRetribuzioneMedia" ControlToValidate="txtRetribuzioneMedia"
                                        Display="Dynamic" ErrorMessage="Retribuzione Media: inserire l'importo in formato valido (max 7 interi e 6 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,6})?" ValidationGroup="UCTabDatiCalcoloVittimeRetr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtRetribuzioneMedia" runat="server"
                                        ErrorMessage="Reddito/Retribuzione media: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtRetribuzioneMedia"
                                        ValidationGroup="UCTabDatiCalcoloVittimeRetr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBeneficio"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlBeneficio" Width="40px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="W" Value="W"></asp:ListItem>
                                        <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                        <asp:ListItem Text="Z" Value="Z"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlBeneficio" runat="server" ErrorMessage="Beneficio: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlBeneficio" ValidationGroup="UCTabDatiCalcoloVittimeRetr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteRetributivi" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="pdivContributivo" runat="server" class="mt-32">
        <table class="tabellaFormattazione" style="width: 100%">
            <tr>
                <td class="Row1">
                    <label class="section-label">
                        Dati Contributivi Vittime</label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvDatiContributiviVittime" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" AllowPaging="false" OnRowCommand="gvDatiContributiviVittime_RowCommand"
                        OnRowDataBound="gvDatiContributiviVittime_RowDataBound" OnRowCancelingEdit="gvDatiContributiviVittime_RowCancelingEdit"
                        OnRowEditing="gvDatiContributiviVittime_RowEditing" OnRowUpdating="gvVittime_RowUpdating"
                        EnableViewState="true" OnDataBound="gvDatiContributiviVittime_DataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaBeneficio" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaBeneficio"
                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%#Bind("DecorrenzaBeneficio") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenzaBeneficio"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Beneficio"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Text="*" CssClass="field-is-required" />
                                    <asp:RequiredFieldValidator ID="RFVtxtDecorrenzaBeneficio" runat="server" ErrorMessage="Decorrenza Beneficio: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaBeneficio" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaBeneficio" Display="Dynamic"
                                        ErrorMessage="Decorrenza Beneficio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="7%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuota"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlQuota" Width="35px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="C" Value="C"></asp:ListItem>
                                        <asp:ListItem Text="D" Value="D"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlQuota" runat="server" ErrorMessage="Quota: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlQuota" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="5" Width="50px" Text='<%#Bind("Settimane") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVittimeContr" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloVittimeContr" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ammontare" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAmmontare"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAmmontare" Width="100px"
                                        CssClass="txtUppercase tb8 " MaxLength="14" Text=' <%# Bind("Ammontare")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtAmmontare" ControlToValidate="txtAmmontare"
                                        Display="Dynamic" ErrorMessage="Ammontare: inserire l'importo in formato valido (max 9 interi e 4 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,9}(,\d{1,4})?" ValidationGroup="UCTabDatiCalcoloVittimeContr" />
                                    <asp:RequiredFieldValidator ID="RFVtxtAmmontare" runat="server" ErrorMessage="Ammontare: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtAmmontare" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Montante" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMontante"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtMontante" Width="100px"
                                        CssClass="txtUppercase tb8 " MaxLength="16" Text=' <%# Bind("Montante")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtMontante" ControlToValidate="txtMontante"
                                        Display="Dynamic" ErrorMessage="Montante: inserire l'importo in formato valido (max 8 interi e 7 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,7})?" ValidationGroup="UCTabDatiCalcoloVittimeContr" />
                                    <asp:RequiredFieldValidator ID="RFVtxtMontante" runat="server" ErrorMessage="Montante: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtMontante" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBeneficio"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlBeneficio" Width="40px" CssClass="txtUppercase tb8 xxs">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="W" Value="W"></asp:ListItem>
                                        <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                                        <asp:ListItem Text="Z" Value="Z"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlBeneficio" runat="server" ErrorMessage="Beneficio: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlBeneficio" ValidationGroup="UCTabDatiCalcoloVittimeContr"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteContributivi" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div id="pdivImportoPensione" runat="server">
        <table class="tabellaFormattazione" style="width: 100%">
            <tr>
                <td class="Row1">
                    <label class="section-label">
                        Importo Pensione Vittime</label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView runat="server" ID="gvDatiImportoPensioneVittime" SkinID="grdElenco1"
                        AutoGenerateColumns="false" CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" Width="100%"
                        BorderColor="Black" AutoGenerateEditButton="true" PageSize="10" AllowPaging="true"
                        OnRowCommand="gvDatiImportoPensioneVittime_RowCommand" OnRowDataBound="gvDatiImportoPensioneVittime_RowDataBound"
                        OnRowCancelingEdit="gvDatiImportoPensioneVittime_RowCancelingEdit" OnRowEditing="gvDatiImportoPensioneVittime_RowEditing"
                        OnRowUpdating="gvVittime_RowUpdating" EnableViewState="true" OnDataBound="gvDatiImportoPensioneVittime_DataBound" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderText="Decorrenza Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaBeneficio" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDecorrenzaBeneficio"
                                        CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7" Text='<%#Bind("DecorrenzaBeneficio") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="validateDecorrenza" ControlToValidate="txtDecorrenzaBeneficio"
                                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Beneficio"
                                        ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiCalcoloVittimeImp"
                                        Text="*" CssClass="field-is-required" />
                                    <asp:RequiredFieldValidator ID="RFVtxtDecorrenzaBeneficio" runat="server" ErrorMessage="Decorrenza Beneficio: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaBeneficio" ValidationGroup="UCTabDatiCalcoloVittimeImp"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaBeneficio" Display="Dynamic"
                                        ErrorMessage="Decorrenza Beneficio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiCalcoloVittimeImp"
                                        ID="customCheckDataDecorrenza" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceGestione" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlCodiceGestione" Width="100px" CssClass="txtUppercase tb8 xs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldddlCodiceGestione" runat="server" ErrorMessage="Codice Gestione: campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="ddlCodiceGestione" ValidationGroup="UCTabDatiCalcoloVittimeImp"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" ID="txtSettimaneRetributive" runat="server"
                                        MaxLength="5" Width="50px"></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="regularTxtSettimaneRetributive"
                                        ControlToValidate="txtSettimaneRetributive" Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiCalcoloVittimeImp" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtSettimaneRetributive" runat="server"
                                        ErrorMessage="Settimane: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtSettimaneRetributive"
                                        ValidationGroup="UCTabDatiCalcoloVittimeImp" Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Importo Pensione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoPensione"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtImportoPensione" Width="100px"
                                        CssClass="txtUppercase tb8 " MaxLength="16" Text=' <%# Bind("ImportoPensione")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoPensione" ControlToValidate="txtImportoPensione"
                                        Display="Dynamic" ErrorMessage="Importo Pensione: inserire l'importo in formato valido (max 8 interi e 7 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,7})?" ValidationGroup="UCTabDatiCalcoloVittimeImp" />
                                    <asp:RequiredFieldValidator ID="RFVtxtImportoPensione" runat="server" ErrorMessage="Importo Pensione: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtImportoPensione" ValidationGroup="UCTabDatiCalcoloVittimeImp"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Beneficio" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBeneficio"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblBeneficio" Text="Z"></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteImportoPensione" CommandName="Elimina" CommandArgument="<% # ((GridViewRow) Container).RowIndex %>"
                                        runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblIdCodeGestione" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiCalcoloVittime" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati Terrorismo" Width="190px" OnClientClick="BlockUI();"
                        OnClick="btnSalvaDatiCalcoloVittime_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiCalcoloVittime" runat="server" SkinID="btnAzione1"
                        CausesValidation="false" Enabled="true" Text="Elimina Dati Terrorismo" Width="190px"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo Terrorismo?')) return false; else BlockUI();"
                        OnClick="btnEliminaDatiCalcoloVittime_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdnSoggettoBeneficiario" />
<asp:HiddenField runat="server" ID="hdnTipologiaPrestazione" />
<asp:HiddenField runat="server" ID="hdnTipologiaBeneficio" />
