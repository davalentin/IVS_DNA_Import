<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPeriodi.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Periodi.UCPeriodi" %>
<script type="text/javascript">

    function validateTab() {
        var flag = true;

        flag = Page_ClientValidate('UCPeriodi');

        if (flag)
            flag = Page_ClientValidate('UCPeriodiGrid');

        return flag;
    }

    $(document).ready(function () {
        $("select[id$='ddlGradoParentela']").each(function () {
            ddlGradoParentelaOnChange($(this));
        });
    });

    function ddlGradoParentelaOnChange(that) {
        if ($.trim($(that).val()) == 'R' || $.trim($(that).val()) == 'RU') {
            $(that).closest("tr").find("input[id$='txtPercGiudice']").removeAttr("disabled");
        }
        else {
            $(that).closest("tr").find("input[id$='txtPercGiudice']").val("");
            $(that).closest("tr").find("input[id$='txtPercGiudice']").attr("disabled", "disabled");
        }
    }

</script>
<asp:Panel ID="pnlPeriodi" runat="server" Visible="true">
    <table id="main" class="tabellaFormattazione" style="width: auto">
        <tr>
            <td class="Row1">
                <label>
                    Codice Fiscale:
                </label>
            </td>
            <td class="Row1" colspan="2">
                <asp:Label ID="lblCodiceFiscale" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Cognome:
                </label>
            </td>
            <td class="Row1">
                <asp:Label ID="Lbcognome" runat="server"></asp:Label>
            </td>
            <td class="Row1">
                <label>
                    Nome:
                </label>
            </td>
            <td class="Row1">
                <asp:Label ID="LbNome" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Cognome Acquisito:
                </label>
            </td>
            <td class="Row1">
                <asp:Label ID="lbCognAcquisito" runat="server"></asp:Label>
            </td>
            <td class="Row1"></td>
            <td class="Row1"></td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Sesso:
                </label>
            </td>
            <td class="Row1">
                <label>
                    <asp:Label ID="LbSesso" runat="server"></asp:Label>
                    <span style="visibility: hidden">&nbsp;</span></label>
            </td>
            <td class="Row1">
                <label>
                    Data di Nascita:</label>
            </td>
            <td class="Row1">
                <asp:Label ID="LbDataDiNascita" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Comune di Nascita:
                </label>
            </td>
            <td class="Row1">
                <asp:Label ID="LbComunedinascita" runat="server"></asp:Label>
            </td>
            <td class="Row1">
                <label>
                    Provincia nascita:
                </label>
            </td>
            <td class="Row1">
                <asp:Label ID="LbProvinciadinascita" runat="server"></asp:Label>
            </td>
        </tr>
        <asp:Panel runat="server" ID="pnlScadenzaRevSan" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Scadenza Rev.San.:
                    </label>
                </td>
                <td class="Row1">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtRevSan" Width="100px"
                        Text="MM/AAAA" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateRevSan" ControlToValidate="txtRevSan"
                        Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Scadenza Rev.San."
                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCPeriodi" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtRevSan" Display="Dynamic"
                        ErrorMessage="Scadenza Rev.San.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCPeriodi"
                        ID="customCheckDataRevSan" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Data Morte:</label>
                </td>
                <td class="Row1">
                    <asp:Label runat="server" ID="lblDataMorteValue" Width="100px"></asp:Label>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <br />
    <table style="width: 98%; margin: auto;" class="mt-16 mb-16">
        <tr>
            <td style="text-align: center">
                <asp:GridView runat="server" ID="gvPeriodi" SkinID="grdElenco1" AutoGenerateColumns="false"
                    CssClass="intestazioneTabella" BorderWidth="1px" BorderColor="Black" Width="100%"
                    AutoGenerateEditButton="true" AllowPaging="false" OnRowCommand="gvPeriodi_RowCommand"
                    OnRowDataBound="gvPeriodi_RowDataBound" OnRowEditing="gvPeriodi_RowEditing" OnRowCancelingEdit="gvPeriodi_RowCancelingEdit">
                    <Columns>
                        <asp:TemplateField HeaderText="Grado Di Parentela" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="250px">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblGradoParentela" CssClass="txtUppercase"></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList runat="server" ID="ddlGradoParentela" CssClass="txtUppercase tb8"
                                    Width="85%" onchange="ddlGradoParentelaOnChange(this)">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RFV_ddlGradoParentela" runat="server" ErrorMessage="Grado Di Parentela: campo obbligatorio"
                                    Text="*" CssClass="field-is-required" ControlToValidate="ddlGradoParentela" ValidationGroup="UCPeriodiGrid"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="% giudice" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPercGiudice" CssClass="txtUppercase" Text='<%# Bind("PercGiudice")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtPercGiudice" CssClass="txtUppercase tb8" Width="50px"
                                    Text='<%# Bind("PercGiudice")%>'>
                                </asp:TextBox>
                                <asp:HiddenField runat="server" ID="hdnIsFromWebDom" Value='<%# Bind("IsFromWebDom")%>' />
                                <asp:RegularExpressionValidator ID="REV_txtPercGiudice" runat="server" ErrorMessage="% giudice: formato non corretto"
                                    Text="*" CssClass="field-is-required" ControlToValidate="txtPercGiudice" ValidationGroup="UCPeriodiGrid" Display="Dynamic"
                                    ValidationExpression="^100(,00)?$|^\d{1,2}(,\d{1,2})?$"></asp:RegularExpressionValidator>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Decorrenza Periodo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDecorrenzaPeriodo" CssClass="txtUppercase" Text='<%# Bind("DecorrenzaPeriodo", "{0:MM/yyyy}")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtDecorrenzaPeriodo" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                                    Text='<%# Bind("DecorrenzaPeriodo", "{0:MM/yyyy}")%>' MaxLength="7">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFV_txtDecorrenzaPeriodo" runat="server" ErrorMessage="Decorrenza Periodo: campo obbligatorio"
                                    Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaPeriodo" ValidationGroup="UCPeriodiGrid"
                                    Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtDecorrenzaPeriodo" ControlToValidate="txtDecorrenzaPeriodo"
                                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per la Decorrenza Periodo."
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCPeriodiGrid" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaPeriodo" Display="Dynamic"
                                    ErrorMessage="Decorrenza Periodo: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCPeriodiGrid"
                                    ID="customCheckDecPeriodo" ClientValidationFunction="checkCorrettezzaData" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cessazione Periodo" HeaderStyle-CssClass="intestazioneTabella Row1"
                            ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCessazionePeriodo" CssClass="txtUppercase" Text='<%# Bind("CessazionePeriodo", "{0:MM/yyyy}")%>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtCessazionePeriodo" CssClass="txtUppercase tb8 date-picker dateMMaaaa"
                                    Text='<%# Bind("CessazionePeriodo", "{0:MM/yyyy}")%>' MaxLength="7">
                                </asp:TextBox>
                                <asp:RegularExpressionValidator runat="server" ID="REV_txtCessazionePeriodo" ControlToValidate="txtCessazionePeriodo"
                                    Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per la Cessazione Periodo."
                                    Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCPeriodiGrid" />
                                <asp:CustomValidator runat="server" ControlToValidate="txtCessazionePeriodo" Display="Dynamic"
                                    ErrorMessage="Cessazione Periodo: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCPeriodiGrid"
                                    ID="customCheckCesPeriodo" ClientValidationFunction="checkCorrettezzaData" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDeletePeriodi" CommandName="Elimina" CommandArgument="<% # ((GridViewRow)Container).RowIndex %>"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
    <table style="width: 98%; margin: auto;" class="tabellaFormattazione grid-col-1">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblPercGiudicePerConiuge" 
                    Text="Si ricorda che il campo “% giudice” è utilizzabile solo per l'ex coniuge. Le procedure di calcolo provvederanno ad attribuire automaticamente al coniuge la corretta percentuale, in relazione alla percentuale attribuita all'ex coniuge." 
                    Visible="false" Font-Italic="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                 <asp:Label runat="server" ID="lblPercGiudicePerExConiuge" 
                    Text="Si ricorda che la percentuale da inserire nel campo “% giudice” è pari al 60% dell’attribuzione ottenuta dal tribunale (es. coniuge divorziata ha ottenuto dal Tribunale l'attribuzione della percentuale pari al 65%, nel campo “% giudice” occorre inserire 39, pari al 65% del 60%)." 
                    Visible="false" Font-Italic="true"></asp:Label>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px;">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiPeriodi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati Periodi" Width="180px" OnClientClick="if(validateTab()){aspnetForm.target = '_self'; BlockUI();}"
                        OnClick="btnSalvaDatiPeriodi_Click" CssClass="primary"/>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiPeriodi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Dati Periodi" Width="180px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Periodi?')) return false; else BlockUI();"
                        OnClick="btnEliminaDatiPeriodi_Click"  CssClass="ghost-delete"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
