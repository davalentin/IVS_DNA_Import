<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCFamiliari.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Familiari.UCFamiliari" %>
<script type="text/javascript">
    function showerror() {
        $('#ExtraInfo').show();
        //$('#top').show();
    }

    function checkfinepostdec(source, args) {
        var gridViewID = "<%=gvCodiciMaggiorazione.ClientID %>";
        var inputs = [];
        inputs = document.getElementById(gridViewID).getElementsByTagName("input");
        var txtDecCarico = inputs[0].value;
        var txtFineCarico = inputs[1].value;
        var dateFineCarico = txtFineCarico.split("/");
        var annofine = dateFineCarico[1];
        var mesefine = dateFineCarico[0];
        var dateDecCarico = txtDecCarico.split("/");
        var annodec = dateDecCarico[1];
        var mesedec = dateDecCarico[0];

        if (annofine < annodec)
            args.IsValid = false;
        if (mesefine < mesedec && annofine == annodec)
            args.IsValid = false;
        return false;
    }

    function checkDataDecorrenzaOriginaria(source, args) {
        var decorrenzaOriginaria = document.getElementById("<%=hfDecorrenzaOriginaria.ClientID%>").value;
        var dateDecOrig = decorrenzaOriginaria.split("/");
        var annodecOrig = dateDecOrig[2].substr(0, 4);
        var mesedecOrig = dateDecOrig[1];

        var gridViewID = "<%=gvCodiciMaggiorazione.ClientID %>";
        var inputs = [];
        inputs = document.getElementById(gridViewID).getElementsByTagName("input");
        var txtDecCarico = inputs[0].value;
        var dateDecCarico = txtDecCarico.split("/");
        var annodec = dateDecCarico[1];
        var mesedec = dateDecCarico[0];

        //trasformo le date nel formato aaaammgg (es. 200811)
        data1str = annodecOrig + mesedecOrig;
        data2str = annodec + mesedec;
        //controllo se la seconda data è successiva alla prima
        if (data2str - data1str < 0)
            args.IsValid = false;
        else
            args.IsValid = true;

        return false;

    }

    function cleanfield() {
        document.getElementById("<%=txtCFAltriFamiliari.ClientID%>").value = '';
        document.getElementById("<%=LbSesso.ClientID %>").innerHTML = '';
        document.getElementById("<%=LbNome.ClientID%>").innerHTML = '';
        document.getElementById("<%=LbDataDiNascita.ClientID%>").innerHTML = '';
        document.getElementById("<%=Lbcognome.ClientID%>").innerHTML = '';
        document.getElementById("<%=LbComunedinascita.ClientID%>").innerHTML = '';
        document.getElementById("<%=LbProvinciadinascita.ClientID%>").innerHTML = '';
        return false;
    }

    function validatePage() {
        var flag = true;
        if (document.getElementById("ctl00_ContentPlaceHolder1_pnlFamiliari") != null) {
            flag = Page_ClientValidate('UCFamiliariCF');
        }
        if (flag) {
            if (document.getElementById("ctl00_ContentPlaceHolder1_pnlFamiliari") != null) {
                flag = Page_ClientValidate('UCFamiliari');
            }
        }
        return flag;
    }

       
</script>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Panel runat="server" ID="pnlTopFamiliari">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" width="100%"/>
    <div>
        <asp:Panel ID="pnlSearch" runat="server" Visible="true">
            <table id="main" class="tabellaFormattazione grid grid-size-20" style="width: auto">
                <tr>
                    <td class="Row1">
                        <label>
                            Codice Fiscale:
                        </label>
                    </td>
                    <td class="field partial-grid" colspan="2">
                        <asp:TextBox ID="txtCFAltriFamiliari" runat="server" CssClass="txtUppercase tb8"
                            Enabled="true" Style="text-align: left" Width="160px" MaxLength="16"></asp:TextBox>
                        <asp:CustomValidator ControlToValidate="txtCFAltriFamiliari" runat="server" Text="*" CssClass="field-is-required"
                            Display="Dynamic" ValidationGroup="UCFamiliariCF" ID="txtCFAltriFamiliari_CV"
                            ClientValidationFunction="validateCodiceFiscale" ErrorMessage="Codice fiscale non valido" />
                        <asp:CustomValidator ControlToValidate="txtCFAltriFamiliari" runat="server" Text="*" CssClass="field-is-required"
                            Display="Dynamic" ValidationGroup="UCFamiliari" ID="txtCFAltriFamiliari_CV2"
                            ClientValidationFunction="validateCodiceFiscale" ErrorMessage="Codice fiscale non valido" />
                        <asp:RequiredFieldValidator ID="RquiredCF" runat="server" ControlToValidate="txtCFAltriFamiliari"
                            ErrorMessage="Inserire un codice fiscale" ValidationGroup="UCFamiliariCF" Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:RequiredFieldValidator ID="RquiredCF2" runat="server" ControlToValidate="txtCFAltriFamiliari"
                            ErrorMessage="Inserire un codice fiscale" ValidationGroup="UCFamiliari" Display="Dynamic">*</asp:RequiredFieldValidator>
                        <asp:ImageButton ID="imgCercaAltriFamiliari" runat="server" AlternateText="Cerca"
                            ImageAlign="AbsMiddle" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/search24.png" CssClass="search-cta"
                            OnClick="CercaAltriFamiliari" ToolTip="Cerca" CausesValidation="false" OnClientClick="if(Page_ClientValidate('UCFamiliariCF')){aspnetForm.target ='_self'; BlockUI();}" />
                        <div class="search-cta-label" style="display:none">Cerca</div>
                    </td>
                    <td>
                        <asp:Button ID="btnUpdateArca" runat="server" Text="Aggiorna da ARCA" Visible="false"
                            OnClick="btnUpdateArca_Click" OnClientClick="BlockUI();" CausesValidation="false"
                            SkinID="btnAzione1"  CssClass="ghost-update"/>
                        <asp:Label ID="hidenCF" runat="server" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Cognome:
                        </label>
                    </td>
                    <td class="field">
                        <asp:Label ID="Lbcognome" runat="server"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label>
                            Nome:
                        </label>
                    </td>
                    <td class="field">
                        <asp:Label ID="LbNome" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Cognome Acquisito:
                        </label>
                    </td>
                    <td class="field">
                        <asp:Label ID="lbCognAcquisito" runat="server"></asp:Label>
                    </td>
                    <td class="Row1">
                    </td>
                    <td class="field">
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Sesso:
                        </label>
                    </td>
                    <td class="field">
                        <label>
                            <asp:Label ID="LbSesso" runat="server"></asp:Label>
                            <span style="visibility: hidden">&nbsp;</span></label>
                    </td>
                    <td class="Row1">
                        <label>
                            Data di Nascita:</label>
                    </td>
                    <td class="field">
                        <asp:Label ID="LbDataDiNascita" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Comune di Nascita:
                        </label>
                    </td>
                    <td class="field">
                        <asp:Label ID="LbComunedinascita" runat="server"></asp:Label>
                    </td>
                    <td class="Row1">
                        <label>
                            Provincia nascita:
                        </label>
                    </td>
                    <td class="field">
                        <asp:Label ID="LbProvinciadinascita" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr id="trParentela" runat="server">
                    <td class="Row1">
                        <label id="GP">
                            Grado Di Parentela:
                        </label>
                    </td>
                    <td class="field full-grid" colspan="3">
                        <asp:DropDownList ID="DropParentela" runat="server" AutoPostBack="true" Width="400px"
                            CssClass="tb8 txtUppercase xl" OnPreRender="DropParentela_PreRender" OnSelectedIndexChanged="ShowGridCodiciMaggiorazione_SelectedIndexChanged"
                            Enabled="false">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="dropParentela_RF" runat="server" ControlToValidate="DropParentela"
                            ErrorMessage="Inserire il grado di parentela" ValidationGroup="UCFamiliari" Display="Dynamic">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr runat="server" id="rowAgoCi">
                    <td class="Row1">
                        <label>
                            Scadenza Rev.San.:
                        </label>
                    </td>
                    <td class="field">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtRevSan" Width="100px"
                            Text="MM/AAAA" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="10"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="validateRevSan" ControlToValidate="txtRevSan"
                            Display="Dynamic" ErrorMessage="Inserire la data nel formato valido per Scadenza Rev.San."
                            Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCFamiliari" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtRevSan" Display="Dynamic"
                            ErrorMessage="Scadenza Rev.San.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari"
                            ID="customCheckDataRevSan" ClientValidationFunction="checkCorrettezzaData" />
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlDataMorte" Visible="false">
                    <tr>
                        <td class="Row1">
                            <label>
                                Data Morte:</label>
                        </td>
                        <td class="field">
                            <asp:Label runat="server" ID="lblDataMorteValue" Width="100px"></asp:Label>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </asp:Panel>
        <br />
        <asp:Panel runat="server" ID="pnlFamiliari" Visible="true">
            <div id="DivFamiliari" style="border: 1px solid black; margin-right: 3px; margin-left: 3px;">
                <asp:GridView ID="ViewFamiliari" SkinID="grdElenco1" DataKeyNames="Id" runat="server"
                    AllowSorting="True" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333"
                    GridLines="None" Width="100%" OnRowCommand="ViewFamiliari_RowCommand" OnRowDataBound="ViewFamiliari_RowDataBound"
                    OnDataBinding="ViewFamiliari_DataBinding">
                    <EmptyDataRowStyle ForeColor="Red" />
                    <EmptyDataTemplate>
                        <center>
                            <asp:Label ID="lblNoData" runat="server" Text="Nessun familiare trovato." SkinID="lblNoData"
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
                        <asp:BoundField HeaderText="Cognome" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            DataField="Cognome"></asp:BoundField>
                        <asp:BoundField HeaderText="Nome" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            DataField="Nome"></asp:BoundField>
                        <asp:BoundField HeaderText="Data Nascita" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            DataField="DataNascita" DataFormatString="{0:dd/MM/yyyy}"></asp:BoundField>
                        <asp:BoundField HeaderText="Parentela" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3">
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Provenienza" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            Visible="false"></asp:BoundField>
                        <asp:BoundField HeaderText="Dec." ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            DataFormatString="{0:MM/yyyy}"></asp:BoundField>
                        <asp:BoundField HeaderText="Cess." ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3"
                            DataFormatString="{0:MM/yyyy}"></asp:BoundField>
                        <asp:BoundField DataField="Id" HeaderText="Id" Visible="False" />
                        <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="21%"
                            HeaderStyle-CssClass="intestazioneTabella Row1 formatLink" ItemStyle-CssClass="TblRecordset3">
                            <ItemTemplate>
                                <asp:Button runat="server" SkinID="btnAzione1" ID="btnmodifica" Text="Modifica" CommandName="modfam"
                                    CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" OnClientClick="BlockUI()" CssClass="tertiary editIconOnly" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="26%" ControlStyle-CssClass="pulsante1 ghost-delete trashIconOnly">
                            <ItemTemplate>
                                <asp:Button CssClass="ghost-delete trashIconOnly" runat="server" ID="btnelimina" Text="Elimina" CommandName="delfam" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                    SkinID="btnAzione1" OnClick="btnElimina_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare il Familiare?')) return false; else BlockUI();" />
                            </ItemTemplate>
                            <HeaderStyle CssClass="intestazioneTabella Row1" />
                            <ItemStyle Width="26%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div class="familiari-external-placeholder" style="display: none">Nessun familiare trovato</div>
            </div>
        </asp:Panel>
        <table id="ExtraInfo" width="100%" visible="false" runat="server">
            <tr>
                <td>
                    <asp:GridView runat="server" ID="gvCodiciMaggiorazione" SkinID="grdElenco1" AutoGenerateColumns="False"
                        CssClass="intestazioneTabella intestazioneTabella--scrollable intestazioneTabella__with-pagination" BorderWidth="1px" BorderColor="Black" Width="100%"
                        AllowPaging="True" PageSize="20" OnRowCommand="gvCodiciMaggiorazione_RowCommand" OnRowDataBound="gvCodiciMaggiorazione_RowDataBound"
                        OnRowEditing="gvCodiciMaggiorazione_RowEditing" OnRowDeleting="gvCodiciMaggiorazione_RowDeleting"
                        OnDataBound="gvCodiciMaggiorazione_DataBound" PagerStyle-CssClass="default-pagination-tables">
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5%"
                                ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnSave" runat="server" ToolTip="Salva" CommandName="Save" Text="<img id=btnSaveImg width=16 height=16 border=0 src=../App_themes/BlueINPS1/Images/save24.png />"
                                        ValidationGroup="UCFamiliari" />
                                    <asp:LinkButton ID="btnEdit" runat="server" ToolTip="Modifica" CommandName="Edit"
                                        Text="<img id=btnEditImg width=20 height=20 border=0 src=../App_themes/BlueINPS1/Images/pencil.png />"
                                        ValidationGroup="UCFamiliari" />
                                    <asp:LinkButton ID="btnAnnulla" runat="server" ToolTip="Annulla" CommandName="Annulla"
                                        Text="<img id=btnAnnullaImg width=16 height=16 border=0 src=../App_themes/BlueINPS1/Images/cancel24.png />" />
                                    <asp:LinkButton ID="btnInsert" runat="server" ToolTip="Aggiungi" CommandName="Insert"
                                        Text="<img id=btnInsertImg width=16 height=16 border=0 src=../App_themes/BlueINPS1/Images/add24.png />"
                                        ValidationGroup="UCFamiliari" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Grado di Parentela--%>
                            <asp:TemplateField HeaderText="Grado di Parentela" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblParentela" Text='<%#Bind("DescParentela") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Grado di Parentela" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlParentela" runat="server" AutoPostBack="true" Width="250px"
                                        OnSelectedIndexChanged="ddlParentela_SelectedIndexChanged" CssClass="tb8 txtUppercase xl">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfDdlParentela" runat="server" ControlToValidate="ddlParentela"
                                        ErrorMessage="Inserire il grado di parentela" ValidationGroup="UCFamiliari" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Codice Maggiorazione--%>
                            <asp:TemplateField HeaderText="Codice Maggiorazione" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMaggiorazione" Text='<%#Bind("Maggiorazione")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Codice Maggiorazione" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlCodMaggiorazione" runat="server"
                                        Width="80px">
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Decorrenze Carico--%>
                            <asp:TemplateField HeaderText="Decorrenza Carico" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAcqusizione" Text='<%# Bind("Acquisizione", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decorrenza Carico" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenzaCarico"
                                        MaxLength="7" Text=' <%# Bind("Acquisizione", "{0:MM/yyyy}")%>' Width="100px" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtDecorrenzaCarico" Display="Dynamic"
                                        ControlToValidate="txtDecorrenzaCarico" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaCarico" Display="Dynamic"
                                        ErrorMessage="Data Decorrenza Carico: Precedente alla Decorrenza Originaria"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari" ID="txtdecorrenzaValidorigi" ClientValidationFunction="checkDataDecorrenzaOriginaria" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtDecorrenzaCarico" runat="server"
                                        ErrorMessage="Decorrenza Carico: Campo obbligatorio" Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenzaCarico"
                                        ValidationGroup="UCFamiliari"></asp:RequiredFieldValidator>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaCarico" Display="Dynamic"
                                        ErrorMessage="Decorrenza Carico: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari"
                                        ID="customCheckDataDecorrenzaCarico" ClientValidationFunction="checkCorrettezzaData" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Fine Carico--%>
                            <asp:TemplateField HeaderText="Fine Carico" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessazione" Text='<%# Bind("Cessazione", "{0:MM/yyyy}")%>'
                                        CssClass="txtUppercase" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fine Carico" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtFineCarico"
                                        MaxLength="7" Text=' <%# Bind("Cessazione", "{0:MM/yyyy}")%>' Width="100px" />
                                    <asp:RegularExpressionValidator runat="server" ID="validateTxtFineCarico" Display="Dynamic"
                                        ControlToValidate="txtFineCarico" Enabled="true" ErrorMessage="Fine: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtFineCarico" Display="Dynamic"
                                        ErrorMessage="Data Fine Carico: Data precedente alla data di Decorrenza Carico"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari" ID="CustomValidator1" ClientValidationFunction="checkfinepostdec" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtFineCarico" Display="Dynamic"
                                        ErrorMessage="Data Fine: Campo Obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari"
                                        ID="CustomValidator5" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtFineCarico" Display="Dynamic"
                                        ErrorMessage="Fine Carico: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCFamiliari"
                                        ID="customCheckDataFineCarico" ClientValidationFunction="checkCorrettezzaData" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Diritto A.F.--%>
                            <asp:TemplateField HeaderText="Diritto A.F." HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDirittoAf" Text='<%# Bind("DirittoAf")%>' CssClass="txtUppercase" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Diritto A.F." HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlDirittoAf" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlDirittoAf_SelectedIndexChanged" CssClass="tb8 txtUppercase xl" Width="95%">
                                        <asp:ListItem Value=" ">&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="SI">SI</asp:ListItem>
                                        <asp:ListItem Value="NO">NO</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Quota A.F.--%>
                            <asp:TemplateField HeaderText="Quota A.F." HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblQuotaAf" Text='<%# Bind("QuotaAf")%>' CssClass="txtUppercase" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quota A.F." HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlQuotaAf" runat="server" AutoPostBack="true" CssClass="tb8 txtUppercase xl" Width="95%">
                                        <asp:ListItem Value=" ">&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="SI">SI</asp:ListItem>
                                        <asp:ListItem Value="NO">NO</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Contitolarità Fondo--%>
                            <asp:TemplateField HeaderText="Contitolarita Fondo" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblContitolarietaFondo" Text='<%# Bind("ContitolaritaFondo")%>' CssClass="txtUppercase" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Contitolarieta Fondo" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlContitolarietaFondo" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlContitolarietaFondo_SelectedIndexChanged" CssClass="tb8 txtUppercase xl"  Width="95%">
                                        <asp:ListItem Value=" ">&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="SI">SI</asp:ListItem>
                                        <asp:ListItem Value="NO">NO</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Contitolarita Ago--%>
                            <asp:TemplateField HeaderText="Contitolarita Ago" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblContitolarietaAgo" Text='<%# Bind("ContitolaritaAgo")%>' CssClass="txtUppercase" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Contitolarieta Ago" HeaderStyle-CssClass="intestazioneTabella"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="20%"
                                ItemStyle-Width="20%" FooterStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlContitolarietaAgo" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlContitolarietaAgo_SelectedIndexChanged" CssClass="tb8 txtUppercase xl" Width="95%">
                                        <asp:ListItem Value=" ">&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="SI">SI</asp:ListItem>
                                        <asp:ListItem Value="NO">NO</asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Bottone Delete--%>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-HorizontalAlign="Center"
                                HeaderStyle-Width="5%" ItemStyle-Width="5%" FooterStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" runat="server" ToolTip="elimina" CommandName="Delete"
                                        Text="<img id=btnDeleteImg width=20 height=20 border=0 src=../App_themes/BlueINPS1/Images/delete24.png />" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CodMaggiorazione" Visible="false" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodMaggiorazione" Text='<%#Bind("CodMaggiorazione")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CodParentela" Visible="false" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodParentela" Text='<%#Bind("CodParentela")%>'> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr id="trInformativaErroriInLoop" runat="server">
                <td class="Row1">
                    <label style="font-weight: bold;">
                    ATTENZIONE! In caso di errori presenti su più soggetti e che non permettono di effettuare una modifica diretta, sarà necessario compilare i quadri mancanti (escludendo questo) e procedere fino all'invio al calcolo in VERIFY. In questo modo, i controlli presenti in fase di invio renderanno questo quadro ROSSO e sarà permessa la correzione dei dati su ogni singolo familiare.
                    </label>
                </td>
            </tr>
        </table>
        <br />
        <asp:Panel ID="pnlButton" runat="server">
            <table width="100%" class="tab-actions-group">
                <tr>
                    <td style="text-align: center" class="tab-actions-group__first--force if-empty-none">
                        <asp:Button ID="btnSalva" SkinID="btnAzione1" runat="server" Text="Salva Familiare"
                            Width="150px" CausesValidation="false" OnClick="btnSalva_Click" OnClientClick="if(validatePage()){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                    </td>
                    <td style="text-align: center" class="tab-actions-group__first if-empty-none">
                        <asp:Button ID="btnAddFamiliare" SkinID="btnAzione1" runat="server" Width="150px"
                            Text="Aggiungi Familiare" CausesValidation="False" OnClick="btnAggiungiFamiliare_Click" CssClass="primary" />
                    </td>
                    <td style="text-align: center" class="if-empty-none">
                        <asp:Button ID="btnEliminaFamiliari" SkinID="btnAzione1" runat="server" Width="150px"
                            Text="Elimina Familiari" CausesValidation="False" OnClick="btnEliminaFamiliari_Click"
                            OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare tutti i familiari?')) return false; else BlockUI();" CssClass="ghost-delete" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Panel>
<asp:HiddenField ID="hfRowIndex" runat="server" />
<asp:HiddenField ID="hfDecorrenzaOriginaria" runat="server" />
<script>
    $(function () {
        $("#btnSaveImg").attr("src", "../App_themes/<%= Page.Theme %>/Images/save24.png");
        $("#btnEditImg").attr("src", "../App_themes/<%= Page.Theme %>/Images/pencil.png");
        $("#btnAnnullaImg").attr("src", "../App_themes/<%= Page.Theme %>/Images/cancel24.png");
        $("#btnInsertImg").attr("src", "../App_themes/<%= Page.Theme %>/Images/add24.png");
        $("#btnDeleteImg").attr("src", "../App_themes/<%= Page.Theme %>/Images/delete24.png");
    });
</script>
