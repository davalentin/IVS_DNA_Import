<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiCi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCDatiAssicurativiCi" %>
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendariInizioFineAssicurazione();
        //ENG - Nuovo Codice CI28
        GestioneVisibilitaCodiceCI28();
    });
    function SetCalendariInizioFineAssicurazione() {
        if ($(document.getElementById("<%=pnlInizioFineAssicurazione.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                yearRange: '-70:' + '+0:',
                minDate: '-70y',
                maxDate: '+0'
            });
            //$(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtInizioAssicurazione.ClientID%>")).mask("99/99/9999");

            $(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                yearRange: '-70:' + '+0:',
                minDate: '-70y',
                maxDate: '+0',
            });
            //$(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtFineAssicurazione.ClientID%>")).mask("99/99/9999");
        }
    }   

    //ENG - Nuovo Codice CI28
    function GestioneVisibilitaCodiceCI28() {    

    var txtCodiceConvenzione=document.getElementById("<%=txtCodiceConvenzione.ClientID%>");

    if(txtCodiceConvenzione!=null && txtCodiceConvenzione!=undefined) {

    var codiceConvenzione=document.getElementById("<%=txtCodiceConvenzione.ClientID%>").value;

    if(codiceConvenzione == 13 || codiceConvenzione == 14 || codiceConvenzione == 26) {      
        $("#<%=ddlCodiceCI28.ClientID%>").show();
        $("#<%=lblCodiceCI28.ClientID%>").show();
      }
      else {
         $("#<%=ddlCodiceCI28.ClientID%>").val("");
         $("#<%=ddlCodiceCI28.ClientID%>").hide();
         $("#<%=lblCodiceCI28.ClientID%>").hide();
      }   
    }
  }
    

</script>
<asp:Panel runat="server" ID="pnlDatiAssicurativi">
    <table class="tabellaFormattazione grid grid-size-25">
        <asp:Panel runat="server" ID="pnlDelibera12688" Visible="false">
            <tr>
                <td class="Row1" style="text-align: left" colspan="4">
                    <asp:Label ID="lblDelibera12688" runat="server" Text="Settimane Italiane Diritto maggiori di 1040. E'possibile selezionare 'Delibera 126/88'"
                        Style="font-weight: bold" ForeColor="Red"></asp:Label>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlInizioFineAssicurazione">
            <tr>
                <td class="Row1" style="width: 23%">
                    <label>
                        Inizio Assicurazione:</label>
                </td>
                <td class="field" style="width: 27%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtInizioAssicurazione"
                        Width="100px" Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1"
                        MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtInizioAssicurazione"
                        ErrorMessage="Data Inizio Assicurazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: Inserire la data di Inizio Assicurazione"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtInizioAssicurazione"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: data inserita posteriore a quella odierna"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ID="customInizioAssicurazione"
                        ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtInizioAssicurazione" Display="Dynamic"
                        ErrorMessage="Inizio Assicurazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataInizioAssicurazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td class="Row1" style="width: 23%">
                    <label>
                        Fine Assicurazione:</label>
                </td>
                <td class="field" style="width: 27%">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtFineAssicurazione" Width="100px"
                        Text="gg/mm/aaaa" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtFineAssicurazione" ControlToValidate="txtFineAssicurazione"
                        ErrorMessage="Data Fine Assicurazione in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$"
                        runat="server" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: Data inserita posteriore a quella odierna"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ID="customFineAssicurazione"
                        ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                    <asp:RequiredFieldValidator runat="server" ID="RFFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: Inserire la data di Fine Assicurazione" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtFineAssicurazione"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtFineAssicurazione" Display="Dynamic"
                        ErrorMessage="Fine Assicurazione: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataFineAssicurazione" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
        </asp:Panel>
        <div runat="server" id="divAttEconomProfInd">
            <tr>
                <td class="Row1">
                    <label>
                        Attività Economica:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtAttivitaEconomica" Width="120px"
                        CssClass="txtUppercase tb8 onClassDomanda" TabIndex="3" MaxLength="2" onblur="extractNumber(this,0,false);"
                        onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtAttivitaEconomica"
                        ErrorMessage="Attivita Economica non valido" ValidationExpression="^[0-9]{3}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        CssClass="offClass  field-is-required onClassDomanda" Enabled="false" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtAttivitaEconomica" ControlToValidate="txtAttivitaEconomica"
                        ErrorMessage="Attività Economica obbligatoria" ValidationGroup="UCTabDatiAssicurativi"
                        Display="Dynamic" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
                <td class="Row1">
                    <label>
                        Professione Individuale:</label>
                </td>
                <td class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtProfessioneIndividuale"
                        Width="120px" CssClass="txtUppercase tb8 onClassDomanda" TabIndex="4" MaxLength="3"
                        onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                        onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtProfessioneIndividuale"
                        ErrorMessage="Professione Individuale non valido" ValidationExpression="^[0-9]{3}$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        CssClass="offClass  field-is-required onClassDomanda" Enabled="false" />
                    <asp:RequiredFieldValidator runat="server" ID="RFVtxtProfessioneIndividuale" ControlToValidate="txtProfessioneIndividuale"
                        ErrorMessage="Professione Individuale obbligatoria" ValidationGroup="UCTabDatiAssicurativi"
                        Display="Dynamic" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </div>
        <!-- Pannello Gestione Normale -->
        <asp:Panel ID="pnlGestioneNormale" runat="server" Visible="true">
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblRMS8888" runat="server" Text="R.M.S. 8888:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtRMS8888" runat="server" TabIndex="5" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMS8888" Display="Dynamic"
                        ControlToValidate="txtRMS8888" Enabled="true" ErrorMessage="R.M.S. 8888: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblRMS9090" runat="server" Text="R.M.S. 9090:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtRMS9090" runat="server" TabIndex="6" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtRMS9090" Display="Dynamic"
                        ControlToValidate="txtRMS9090" Enabled="true" ErrorMessage="R.M.S. 9090: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblVVMisura1292" runat="server" Text="VV Misura al 12/92:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtVVMisura1292" runat="server" TabIndex="7" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtVVMisura1292" ControlToValidate="txtVVMisura1292"
                        Display="Dynamic" ErrorMessage="VV Misura al 12/92 non valido: inserire il numero di VV Misura al 12/92 in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblVVMisuraDl50392" runat="server" Text="VV Misura D.L. 503/92:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtVVMisuraDl50392" runat="server" TabIndex="8" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtVVMisuraDl50392" ControlToValidate="txtVVMisuraDl50392"
                        Display="Dynamic" ErrorMessage="VV Misura al 503/92 non valido: inserire il numero di VV Misura al 503/92 in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblSettCalcoloContrib" runat="server" Text="Settimane per calcolo contributivo:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettCalcoloContrib" runat="server" TabIndex="9" MaxLength="4"
                        CssClass="tb8 txtUppercase" Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettCalcoloContrib"
                        ControlToValidate="txtSettCalcoloContrib" Display="Dynamic" ErrorMessage="Settimane per calcolo contributivo: inserire il numero di Settimane per calcolo contributivo in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblIVSArt11488" runat="server" Text="I.V.S. Art.11/488:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtIVSArt11488" runat="server" TabIndex="10" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtIVSArt11488" Display="Dynamic"
                        ControlToValidate="txtIVSArt11488" Enabled="true" ErrorMessage="I.V.S. Art.11/488: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblSettOBGDiritto" runat="server" Text="Settimane OBG Diritto:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettOBGDiritto" runat="server" TabIndex="11" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettOBGDiritto" ControlToValidate="txtSettOBGDiritto"
                        Display="Dynamic" ErrorMessage="Settimane OBG Diritto non valido: inserire il numero di Settimane OBG Diritto in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblSettVVDiritto" runat="server" Text="Settimane VV Diritto:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettVVDiritto" runat="server" TabIndex="12" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettVVDiritto" ControlToValidate="txtSettVVDiritto"
                        Display="Dynamic" ErrorMessage="Settimane VV Diritto non valido: inserire il numero di Settimane VV Diritto in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
            </tr>
        </asp:Panel>
        <!-- Fine Pannello Gestione Normale -->
        <!-- Pannello Gestione Speciale -->
        <asp:Panel ID="pnlGestioneSpeciale" runat="server" Visible="true">
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblSettItalianeDiritto" runat="server" Text="Settimane italiane diritto:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettItalianeDiritto" runat="server" TabIndex="13" MaxLength="4"
                        CssClass="tb8 txtUppercase" Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettItalianeDiritto"
                        ControlToValidate="txtSettItalianeDiritto" Display="Dynamic" ErrorMessage="Numero Settimane italiane diritto non valido: inserire il numero di Settimane italiane diritto in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblSettItalianeMisura" runat="server" Text="Settimane italiane misura:"></asp:Label>
                </td>
                <td class="Row1">
                    <asp:TextBox ID="txtSettItalianeMisura" runat="server" TabIndex="14" CssClass="tb8 txtUppercase"
                        Width="120px" Enabled="false"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <!-- Fine Pannello Gestione Speciale -->
        <!-- Pannello Comune (due) -->
        <asp:Panel ID="pnlComuneDue" runat="server" Visible="true">
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblSettFittizie" runat="server" Text="Settimane Fittizie:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettFittizie" runat="server" TabIndex="15" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettFittizie" ControlToValidate="txtSettFittizie"
                        Display="Dynamic" ErrorMessage="Settimane Fittizie: inserire il numero di Settimane Fittizie in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblImportoIVS" runat="server" Text="Importo I.V.S.:" Visible="false"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtImportoIVS" runat="server" TabIndex="16" CssClass="tb8 txtUppercase"
                        Width="120px" Visible="false"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtImportoIVS" Display="Dynamic"
                        ControlToValidate="txtImportoIVS" Enabled="true" ErrorMessage="Importo I.V.S.: Inserire valori interi o decimali"
                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi" ValidationExpression="\d+(\,\d{1,4})?" />
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:Label ID="lblSettEffettive" runat="server" Text="Settimane Effettive:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettEffettive" runat="server" TabIndex="17" MaxLength="4" CssClass="tb8 txtUppercase"
                        Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="validateTxtSettEffettive" ControlToValidate="txtSettEffettive"
                        Display="Dynamic" ErrorMessage="Settimane Effettive: inserire il numero di Settimane Effettive in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
                <td class="Row1">
                    <asp:Label ID="lblSettGodimentoAssegno" runat="server" Text="Settimane godimento assegno:"></asp:Label>
                </td>
                <td class="field">
                    <asp:TextBox ID="txtSettGodimentoAssegno" runat="server" TabIndex="18" MaxLength="4"
                        CssClass="tb8 txtUppercase" Width="120px"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="ValidateTxtSettGodimentoAssegno"
                        ControlToValidate="txtSettGodimentoAssegno" Display="Dynamic" ErrorMessage="Settimane godimento assegno: inserire il numero di Settimane godimento assegno in un formato valido"
                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlSettimaneOBGMisura" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Settimane OBG misura al 12/92:
                    </label>
                </td>
                <td>
                    <asp:TextBox ID="txtSettimaneOBGMisura12_92" runat="server" CssClass="tb8 txtUppercase"
                        Width="120px" Enabled="false" TabIndex="48"></asp:TextBox>
                </td>
                <td class="Row1">
                    <label>
                        Settimane OBG misura DL 503/92:
                    </label>
                </td>
                <td>
                    <asp:TextBox ID="txtSettimaneOBGMisuraDL503_92" runat="server" CssClass="tb8 txtUppercase"
                        Width="120px" Enabled="false" TabIndex="48"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <!-- Fine Pannello Comune (due) -->
        <asp:Panel ID="pnlAnzVecch" runat="server" Visible="false">
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Vecchiaia al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqVecch1294" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="19">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1">
                    <label>
                        Requisiti Anzianità al 12/94:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqAnz1294" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="20">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <label>
                        Requisiti Vecchiaia al 9/96:</label>
                </td>
                <td class="chkField">
                    <asp:DropDownList runat="server" ID="ddlReqVecch996" Width="50px" CssClass="tb8 txtUppercase xxs"
                        TabIndex="21">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="none">
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="Row1">
                <label>
                    Codice Convenzione:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtCodiceConvenzione" runat="server" CssClass="tb8 txtUppercase"
                    Width="30" TabIndex="22" onkeyup="GestioneVisibilitaCodiceCI28();"></asp:TextBox>
            </td>
            <td class="Row1">
                <label>
                    Anni Differimento:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtAnniDifferimento" runat="server" CssClass="tb8 txtUppercase"
                    Width="30" TabIndex="23" MaxLength="2"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtAnniDifferimento" ControlToValidate="txtAnniDifferimento"
                    Display="Dynamic" ErrorMessage="Anno non valido: inserire il numero di anni in un formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabDatiAssicurativi" />
            </td>
        </tr>
        <tr>
            <td class="Row1 pre__full-grid">
                <label>
                    Codice Virtuale:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodiceVirtuale" Width="100%" CssClass="tb8 txtUppercase xl"
                    TabIndex="24">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Decorrenza Codice Virtuale:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtDecCodVirtuale" Width="95px"
                    CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="25" Text="mm/aaaa"
                    MaxLength="7"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateDecCodVirtuale" ControlToValidate="txtDecCodVirtuale"
                    Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per Decorrenza Codice Virtuale"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCTabDatiAssicurativi"
                    Text="*" CssClass="field-is-required" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDecCodVirtuale" Display="Dynamic"
                    ErrorMessage="Decorrenza Codice Virtuale: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabDatiAssicurativi"
                    ID="customCheckDataDecorrenzaCodiceVirtuale" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Delibera 126/88:</label>
            </td>
            <td class="field">
                <asp:CheckBox ID="chkDelibera12688" runat="server" CssClass="tb8 txtUppercase" TabIndex="26" />
            </td>
            <td class="Row1">
                <label>
                    Importo Cristallizzazione:</label>
            </td>
            <td class="field">
                <asp:TextBox ID="txtImportoCristallizzazione" runat="server" CssClass="txtUppercase tb8"
                    TabIndex="27" MaxLength="8"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="RegextxtImportopensione84" ControlToValidate="txtImportoCristallizzazione"
                    Display="Dynamic" ErrorMessage="Importo Cristallizzazione: inserire l'importo in formato valido"
                    Text="*" CssClass="field-is-required" ValidationExpression="\d+(\,\d{1,2})?" ValidationGroup="UCTabDatiAssicurativi" />
            </td>
        </tr>
        <tr>
            <td colspan="4" class="shift-full-grid">
                <div id="div1" style="border-style: solid; border-color: #000080; border-collapse: collapse;
                    border-width: 1px; width: 100%; margin-left: 0px">
                    <table class="tabellaFormattazione grid grid-size-25">
                        <tr>
                            <td colspan="4" class="Row1 shift-full-grid">
                                <asp:Label runat="server" ID="lblBloccoArretratiEstero" Style="font-style: italic" CssClass="section-label mt-32">Blocco Arretrati Estero</asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1">
                                <label>
                                    Codice:</label>
                            </td>
                            <td class="field" style="padding-left: 27px;">
                                <asp:DropDownList ID="ddlCodice" runat="server" CssClass="tb8 txtUppercase xxs" Width="50"
                                    TabIndex="28">
                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                    <asp:ListItem Value="SI" Text="SI"></asp:ListItem>
                                    <asp:ListItem Value="NO" Text="NO"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="Row1" style="padding-left: 10px;">
                            </td>
                            <td class="field" style="padding-left: 27px;">
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1">
                                <label>
                                    Codice Ufficio Pagatore Istituzione estera:</label>
                            </td>
                            <td class="field" style="padding-left: 27px;">
                                <asp:TextBox ID="txtCodUffPagatoreIstEstera" runat="server" CssClass="tb8 txtUppercase"
                                    Width="60" MaxLength="3" TabIndex="29" />
                            </td>
                            <td class="Row1" style="padding-left: 10px;">
                            </td>
                            <td class="field" style="padding-left: 27px;">
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td class="Row1">
                <label>
                    Requisiti particolari per diritto:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlRequisitiParticolariDiritto" Width="100%"
                    CssClass="tb8 txtUppercase xl" TabIndex="30">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlCodiceMotivazioniCI28" Visible="false">
                <td class="Row1">
                    <label>
                        Codice Motivazioni CI 28:</label>
                </td>
                <td class="field">
                    <asp:DropDownList ID="ddlCodiceMotivazioniCI28" runat="server" CssClass="tb8 txtUppercase"
                        Width="120" TabIndex="31">
                    </asp:DropDownList>
                </td>
            </asp:Panel>
            <td class="Row1">
                <label>
                    Codice CI 21:</label>
            </td>
            <td class="field">
                <asp:DropDownList ID="ddlCodiceCI21" runat="server" CssClass="tb8 txtUppercase xxs"
                    Width="120" TabIndex="32">
                </asp:DropDownList>
            </td>
        </tr>
        <!-- ENG - Gestione Codice CI28 -->
        <tr>
            <td class="Row1">
                <label id="lblCodiceCI28" runat="server">
                    Codice CI 28:</label>
            </td>
            <td class="field">
                <asp:DropDownList ID="ddlCodiceCI28" runat="server" CssClass="tb8 txtUppercase" Width="120"
                    TabIndex="31">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiAssicurativi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Dati Assicurativi" CausesValidation="false" Width="170px" OnClick="SalvaDatiAssicurativi_Click"
                        OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativi')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaDatiAssicurativi" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Elimina Dati Assicurativi" Width="170px" Style="padding-left: 0px; padding-right: 0px;" CssClass="ghost-delete"
                        CausesValidation="false" OnClick="EliminaDatiAssicurativi_Click" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Assicurativi?')) return false; else BlockUI();" />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
</asp:Panel>
