<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiNoCalcolo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiNoCalcolo.UCDatiNoCalcolo" %>
<style type="text/css">
    .Align
    {
        text-align: center;
    }
    input[type="text"][disabled]
    {
        background: #dddddd;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=txtDecorrenzaRegistrazione.ClientID%>").unmask();
        $("#<%=txtDecorrenzaRegistrazione.ClientID%>").mask('99/99/9999');
        $("#<%=txtDecorrenzaRegistrazione.ClientID%>").blur(function () { ManageDatiNoCalcolo(); });
        ManageDatiNoCalcolo();

    });
    function IsTredicesima() {
        var str = $("#<%=txtDecorrenzaRegistrazione.ClientID%>").val();
        var mese = Number(str.substring(3, 5));
        var isTredicesima = (mese == 13);
        return isTredicesima;
    }
    function ManageDatiNoCalcolo() {

        if (IsTredicesima()) {
            ManagePrevalTredicesima();
            //disalbe field
            $("#<%=txtAggFamigliaFondo.ClientID%>").val('');
            $("#<%=txtAggFamigliaFondo.ClientID%>").attr('disabled', true);
            $("#<%=txtImportoMensile.ClientID%>").val('')
            $("#<%=txtImportoMensile.ClientID%>").attr('disabled', true);
            //enable field
            $("#<%=txtTredicesima.ClientID%>").attr('disabled', false);
            //Required field
            ValidatorEnable($('#<%=RFVtxtImportoMensile.ClientID%>')[0], false);
            ValidatorEnable($('#<%=RFVtxtAdeguataFondo.ClientID%>')[0], false);
            ValidatorEnable($('#<%=RFVtxtTredicesima.ClientID%>')[0], true);

        }
        else {
            $("#<%=txtAggFamigliaFondo.ClientID%>").attr('disabled', false);
            $("#<%=txtImportoMensile.ClientID%>").attr('disabled', false);
            $("#<%=txtTredicesima.ClientID%>").attr('disabled', true);
            $("#<%=txtTredicesima.ClientID%>").val('');
            //Required field
            ValidatorEnable($('#<%=RFVtxtImportoMensile.ClientID%>')[0], true);
            ValidatorEnable($('#<%=RFVtxtAdeguataFondo.ClientID%>')[0], true);
            ValidatorEnable($('#<%=RFVtxtTredicesima.ClientID%>')[0], false);
        }

        function ManagePrevalTredicesima() {
            var strPreval = $("#<%=HdnPrevalTredicesima.ClientID%>").val();
            var tredicesima = $("#<%=txtTredicesima.ClientID%>");
            var adeguataFondo = $("#<%= txtAdeguataFondo.ClientID %>");
            if (tredicesima.val() == '' && strPreval && IsTredicesima()) {
                $("#<%=txtTredicesima.ClientID%>").val(strPreval);
            }
            if (adeguataFondo.val() == '' && strPreval && IsTredicesima()) {
                $("#<%=txtAdeguataFondo.ClientID%>").val(strPreval);
            }
        }
    }
</script>
<asp:Panel runat="server" ID="pnlDatiFondo">
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 30%">
                <label style="font-weight: bold">
                    Decorrenza Registrazione:</label>
            </td>
            <td class="field" style="text-align: left; width: 25%">
                <asp:TextBox runat="server" ID="txtDecorrenzaRegistrazione" Width="50%" CssClass="tb8 txtUppercase dateGGmmAAAA"
                    MaxLength="10"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVtxtDecorrenzaRegistrazione" ControlToValidate="txtDecorrenzaRegistrazione"
                    ErrorMessage="Decorrenza Registrazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                    runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiNoCalcolo"
                    Enabled="true" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDecorrenzaRegistrazione" Display="Dynamic"
                    ErrorMessage="Decorrenza Registrazione: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo"
                    ControlToValidate="txtDecorrenzaRegistrazione"></asp:RequiredFieldValidator>
            </td>
            <td style="width: 45%">
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1">
                <label style="font-weight: bold">
                    Componenti familiari</label>
            </td>
        </tr>
        <tr>
            <td class="shift-full-grid">
                <asp:DataList ID="dataListComponentiFamiliari" runat="server" RepeatDirection="Horizontal"
                    HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="Align TblRecordset3 breakword"
                    FooterStyle-CssClass="Align" OnItemDataBound="dataListComponentiFamiliari_DataBound"
                    SkinID="dataList1" BorderWidth="1" Width="100%" BorderColor="Black" RepeatLayout="Table"
                    OnItemCommand="dataListComponentiFamiliari_ItemCommand" GridLines="Both">
                    <HeaderTemplate>
                        <div class="intestazioneTabella" style="width: 100%; padding: 0px">
                            <asp:Label runat="server" ID="lblTitle" Text="Cliccare sul codice fiscale per selezionarlo come componente familiare"></asp:Label>
                        </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkComponenteFamiliare" Text='<%#Bind("CodiceFiscale")%>'
                            CommandName="SelectCFComponenteFamiliare" OnClientClick="BlockUI()" Style="font-weight: bold"></asp:LinkButton>
                        <asp:Image runat="server" ID="imgComponenteFamiliare" />
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label Visible='<%#bool.Parse((dataListComponentiFamiliari.Items.Count==0).ToString())%>'
                            Style="font-weight: bold" runat="server" ID="lblNoRecord" Text="Nessun familiare presente!"
                            ForeColor="Red"></asp:Label>
                    </FooterTemplate>
                </asp:DataList>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Eccedenza AGO su Fondo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtEccedenzaAgo" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtEccedenzaAgo" ControlToValidate="txtEccedenzaAgo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Eccedenza AGO su Fondo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Adeguata Fondo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtAdeguataFondo" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtAdeguataFondo" ControlToValidate="txtAdeguataFondo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Adeguata Fondo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtAdeguataFondo" Display="Dynamic"
                    ErrorMessage="AdeguataFondo: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo"
                    ControlToValidate="txtAdeguataFondo"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <!-- 2 riga -->
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Fac.art.14 reg.33:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtFacArt14" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RFVtxtFacArt14" ControlToValidate="txtFacArt14"
                    Display="Dynamic" Enabled="true" ErrorMessage="Fac.art.14 reg.33: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Q.AGO esclusiva:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAgoEsclusiva" runat="server" CssClass="tb8 txtUppercase"
                    MaxLength="11" Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RFVQuotaAgoEsclusiva" ControlToValidate="txtQuotaAgoEsclusiva"
                    Display="Dynamic" Enabled="true" ErrorMessage="Adeguata Fondo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
        </tr>
        <!-- 3 riga -->
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Assegni familiari:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtAssegniFamiliari" runat="server" CssClass="tb8 txtUppercase"
                    MaxLength="11" Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtAssegniFamiliari" ControlToValidate="txtAssegniFamiliari"
                    Display="Dynamic" Enabled="true" ErrorMessage="Fac.art.14 reg.33: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Ind.int.speciale:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtIndIntSpeciale" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtIndIntSpeciale" ControlToValidate="txtIndIntSpeciale"
                    Display="Dynamic" Enabled="true" ErrorMessage="Adeguata Fondo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
        </tr>
        <!-- 4 riga -->
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Onere a carico amm:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtOnereCaricoAmm" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtOnereCaricoAmm" ControlToValidate="txtOnereCaricoAmm"
                    Display="Dynamic" Enabled="true" ErrorMessage="Onere a carico amm: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Agg.famiglia fondo:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtAggFamigliaFondo" runat="server" CssClass="tb8 txtUppercase"
                    MaxLength="11" Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtAggFamigliaFondo" ControlToValidate="txtAggFamigliaFondo"
                    Display="Dynamic" Enabled="true" ErrorMessage="Agg.famiglia fondo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
        </tr>
        <!-- 5 riga -->
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Importo mensile:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtImportoMensile" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoMensile" ControlToValidate="txtImportoMensile"
                    Display="Dynamic" Enabled="true" ErrorMessage="Importo mensile: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtImportoMensile" Display="Dynamic"
                    ErrorMessage="Importo Mensile: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo"
                    ControlToValidate="txtImportoMensile"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Art.21/26 reg.71:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtArt21" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtArt21" ControlToValidate="txtArt21"
                    Display="Dynamic" Enabled="true" ErrorMessage="Art.21/26 reg.71: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
            </td>
        </tr>
        <!-- 6 riga -->
        <tr>
            <td class="Row1" style="width: 25%">
            </td>
            <td class="Row1" style="width: 25%">
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Tredicesima:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtTredicesima" runat="server" CssClass="tb8 txtUppercase" MaxLength="11"
                    Width="63%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVtxtTredicesima" ControlToValidate="txtTredicesima"
                    Display="Dynamic" Enabled="true" ErrorMessage="Tredicesima: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo" ValidationExpression="\d{1,6}(,\d{1,4})?" />
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtTredicesima" Display="Dynamic"
                    ErrorMessage="Tredicesima: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiNoCalcolo"
                    ControlToValidate="txtTredicesima"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiNoCalcolo" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva Dati No Calcolo" Width="150px" OnClick="SalvaDatiNoCalcolo_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiNoCalcolo')){aspnetForm.target ='_self'; BlockUI();}" CssClass="force-right primary" />
                    <asp:Button ID="btnEliminaDatiNoCalcolo" SkinID="btnAzione1" runat="server" Width="150px"
                        Style="padding-left: 2px; padding-right: 2px" Text="Elimina Dati No Calcolo" CssClass="ghost-delete"
                        CausesValidation="False" OnClick="btnEliminaDatiNoCalcolo_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati No Calcolo?')) return false; else BlockUI();" />
                    <asp:Button ID="btnTornaElencoRegistrazioni" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elenco Registrazioni" Width="150px" OnClick="TornaElencoRegistrazioni_Click"
                        OnClientClick="BlockUI();"  CssClass="tertiary"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<input type="hidden" id="HdnPrevalTredicesima" name="hdnPrevalTredicesima" value=""
    runat="server" />
