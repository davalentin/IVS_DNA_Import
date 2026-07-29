<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAssicurativiINPDAP.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneAgo.UCDatiAssicurativiINPDAP" %>
<script type="text/javascript">
    $(document).ready(function () {

        var availableTagsCausaCess = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiINPDAP_HiddenFieldCausaCessazione").value.split(';');
        $("#<%=txtCausaCessazione.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTagsCausaCess
        });

        var availableTags = document.getElementById("ctl00_ContentPlaceHolder1_ucDatiAssicurativiINPDAP_hiddenAttivitaSvolte").value.split(';');
        $("#<%=txtAttivitaSvoltaINPDAP.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags
        });

        ddlDirittoIndennIntegrSpecOnChange();
        $(document.getElementById("<%= txtAttivitaSvoltaINPDAP.ClientID %>")).change(function () {
            ddlDirittoIndennIntegrSpecOnChange();
        });

    });


    function validateMese(source, args) {
        var mesi = args.Value;
        if (mesi < 0 || mesi > 11)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function validateGiorno(source, args) {
        var giorni = args.Value;
        if (giorni < 0 || giorni > 30)
            args.IsValid = false;
        else
            args.IsValid = true;
        return false;
    }

    function ddlDirittoIndennIntegrSpecOnChange() {
        var ddl = document.getElementById("<%= ddlDirittoIndennIntegrSpec.ClientID %>");
        if (ddl) {

            var iisRapportata = $("#<%= ddlIISAbbattimentoAnni.ClientID %>");
            var riduzioneL537 = $("#<%= ddlRiduzioneL537.ClientID %>");

            if (ddl.value == "NO") {


                if (iisRapportata) {
                    iisRapportata.val("NO");
                    iisRapportata.attr('disabled', true);
                }

                if (riduzioneL537) {
                    riduzioneL537.val("NO");
                    riduzioneL537.attr('disabled', true);
                }
            }
            else {


                if (iisRapportata) {
                    iisRapportata.attr('disabled', false);
                }

                if (riduzioneL537) {
                    riduzioneL537.attr('disabled', false);
                }
            }
        }

    }
    function CheckBeneficiDisabled() {


        if (GetCodNatura3() == 'G') {
            return true;
        }

        return false;
    }
</script>
<!-- Pannello Common Header -->
<asp:Panel runat="server" ID="pnlCommonHeader">
    <div id="divBorder" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaFormattazione">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Tipo Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblTipoPensione"></asp:Label>
                    <asp:HiddenField ID="hdnTipoPensione" runat="server" />
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Pensione:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiAssicurativi" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlDecorrenzaCalcoloNuovaGestioneDatiFondoFSPT" Visible="false">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Calcolo:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:Label runat="server" ID="lblDecorrenzaCalcolo"></asp:Label>
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </div>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Primo Versamento:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtPrimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="1" MaxLength="10"
                        Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtPrimoVersamento"
                        ErrorMessage="Data primo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="requiredPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo versamento: Inserire la data del primo versamento" Text="*"
                        ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtPrimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtPrimoVersamento" Display="Dynamic"
                        ErrorMessage="Primo Versamento: data illogica" Text="*" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataPrimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Ultimo Versamento:</label>
            </td>
            <td class="Row1" style="width: 25%">
                <asp:Panel runat="server" ID="pnlTxtUltimoVersamento">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                        Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" TabIndex="2" MaxLength="10"
                        Enabled="false"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="validatetxtUltimoVersamento" ControlToValidate="txtUltimoVersamento"
                        ErrorMessage="Data ultimo versamento in formato non valido" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        runat="server" Text="*" Display="Dynamic" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                    <asp:RequiredFieldValidator runat="server" ID="RFUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo versamento: Inserire la data dell'ultimo versamento" Text="*"
                        ValidationGroup="UCTabDatiAssicurativi" ControlToValidate="txtUltimoVersamento"></asp:RequiredFieldValidator>
                    <asp:CustomValidator runat="server" ControlToValidate="txtUltimoVersamento" Display="Dynamic"
                        ErrorMessage="Ultimo Versamento: data illogica" Text="*" ValidationGroup="UCTabDatiAssicurativi"
                        ID="customCheckDataUltimoVersamento" ClientValidationFunction="checkCorrettezzaData" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <asp:Panel runat="server" ID="pnlCodiceSpecifico">
                <td class="Row1" style="width: 25%">
                    <label>
                        Codice Specifico:</label>
                </td>
                <td class="field" colspan="3">
                    <asp:DropDownList runat="server" ID="ddlCodiceSpecifico" CssClass="txtUppercase tb8"
                        TabIndex="3" Width="90%" Enabled="false">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="ddlCodiceSpecifico_RF" Display="Dynamic"
                        Text="*" ErrorMessage="Codice Specifico: Si prega di inserire il codice specifico"
                        ControlToValidate="ddlCodiceSpecifico" ValidationGroup="UCTabDatiAssicurativi"
                        Enabled="true" />
                </td>
            </asp:Panel>
        </tr>
    </table>
</asp:Panel>
<!-- Fine Pannello Common Header -->
<table class="tabellaFormattazione">
    <tr>
        <td class="Row1" style="width: 25%">
            <asp:Label ID="lblAttivitaFS" runat="server" Text="Qualifica Professionale:"></asp:Label>
        </td>
        <asp:Panel runat="server" ID="pnlDDLAttivitaSvoltaINPDAP" Visible="false">
            <td class="Row1" colspan="3">
                <asp:DropDownList runat="server" ID="ddlAttivitaSvoltaINPDAP" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4" Enabled="false">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="REQFddlAttivitaSvoltaINPDAP" Display="Dynamic"
                    Text="*" ErrorMessage="" ControlToValidate="ddlAttivitaSvoltaINPDAP" ValidationGroup="UCTabDatiAssicurativi"
                    Enabled="true" />
            </td>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlTXTAttivitaSvoltaINPDAP" Visible="false">
            <td class="Row1" colspan="3">
                <asp:TextBox runat="server" ID="txtAttivitaSvoltaINPDAP" Width="90%" CssClass="txtUppercase tb8"
                    TabIndex="4">
                </asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="REQFtxtAttivitaSvoltaINPDAP" Display="Dynamic"
                    Text="*" ErrorMessage="" ControlToValidate="txtAttivitaSvoltaINPDAP" ValidationGroup="UCTabDatiAssicurativi"
                    Enabled="true" />
            </td>
        </asp:Panel>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Causa di cessazione:</label>
        </td>
        <td class="Row1" colspan="2">
            <asp:TextBox ID="txtCausaCessazione" runat="server" Width="90%" Text="" CssClass="txtUppercase tb8"
                Enabled="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="Row1" style="width: 25%">
            <label>
                Titolare Altra Pensione:</label>
        </td>
        <td class="chkField" colspan="2">
            <asp:DropDownList runat="server" ID="ddlTitAltraPensione" Width="10%" CssClass="tb8 txtUppercase"
                TabIndex="28">
                <asp:ListItem Text="" Value=""></asp:ListItem>
                <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
<!-- Pannello Custom INPDAP -->
<asp:Panel runat="server" ID="pnlCustomINPDAP" Visible="false">
    <table class="tabellaFormattazione">
        <asp:Panel ID="pnlDecAnteAgosto95" runat="server" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Diritto Indennità Integrativa Speciale:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlDirittoIndennIntegrSpec" Width="30.5%" CssClass="tb8 txtUppercase"
                        TabIndex="24">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <asp:Panel runat="server" ID="pnlIntegrazioneMinimo" Visible="false">
                    <td class="Row1" style="width: 25%">
                        <label>
                            Integrazione al Minimo:</label>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <asp:DropDownList runat="server" ID="ddlIntegrazioneMinimo" Width="30.5%" CssClass="tb8 txtUppercase"
                            TabIndex="25">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                            <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </asp:Panel>
            </tr>
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Riduzione L.537:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlRiduzioneL537" Width="30.5%" CssClass="tb8 txtUppercase"
                        TabIndex="26">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="Row1" style="width: 25%">
                    <label>
                        I.I.S. RAP. ad Anni:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:DropDownList runat="server" ID="ddlIISAbbattimentoAnni" Width="30.5%" CssClass="tb8 txtUppercase"
                        TabIndex="27">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="Panel2" Visible="false">
            <tr>
                <td class="Row1" style="width: 25%">
                    <label>
                        Decorrenza Calcolo:</label>
                </td>
                <td class="Row1" style="width: 25%">
                    <asp:Label runat="server" ID="Label3"></asp:Label>
                </td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<!-- Fine Pannello Custom INPDAP -->
<!-- Pannello ripartizioni inpadap -->
<asp:Panel runat="server" ID="pnlRipartizioni">
    <div id="div1" style="border-style: solid; border-color: #000080; border-collapse: collapse;
        border-width: 1px; width: 710px; margin-left: 4px; margin-bottom: 8px; margin-top: 4px;">
        <table class="tabellaContenuti" style="width: 100%">
            <tr>
                <td align="left">
                    <asp:Label runat="server" ID="lblRipartizioniInpdap" Font-Bold="true">&nbsp; Ripartizioni INPDAP</asp:Label>
                </td>
            </tr>
            <tr>
                <td class="Row1">
                    <asp:GridView runat="server" ID="gvRipartizioni" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        AutoGenerateEditButton="true" PageSize="10" AllowPaging="true" OnRowCommand="gvRipartizioni_RowCommand"
                        OnRowDataBound="gvRipartizioni_RowDataBound" OnRowCancelingEdit="gvRipartizioni_RowCancelingEdit"
                        OnRowEditing="gvRipartizioni_RowEditing" EnableViewState="true" OnPageIndexChanging="gvRipartizioni_onPageIndexChanging">
                        <Columns>
                            <asp:TemplateField HeaderText="Ente" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="46%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnte"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="%" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="46%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblPercentuale" Text='<%# Bind("Importo")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtPercentuale" Width="13%" CssClass="tb8 txtUppercase" MaxLength="5"
                                        Text='<%# Bind("Importo")%>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="REV_txtPercentuale" runat="server" ErrorMessage="% (Percentuale) inserita in formato non corretto"
                                        Text="*" ControlToValidate="txtPercentuale" ValidationGroup="GrigliaRipartizioni"
                                        Display="Dynamic" ValidationExpression="^100(,00)?$|^\d{1,2}(,\d{1,2})?$">
                                    </asp:RegularExpressionValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<!-- Fine Pannello ripartizioni inpdap -->
<!--div bottoni-->
<div style="width: 720px; margin-top: 25px; margin-right: 40px;">
    <table width="100%">
        <tr>
            <td style="text-align: right">
                <asp:Button ID="btnSalvaDatiAssicurativi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Dati Assicurativi" Width="150px" OnClick="SalvaDatiAssicurativi_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabDatiAssicurativi')){aspnetForm.target ='_self'; BlockUI();}" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaDatiAssicurativi" SkinID="btnAzione1" runat="server" Width="150px"
                    Text="Elimina Dati Assicurativi" CausesValidation="False" OnClick="btnEliminaDatiAssicurativi_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Assicurativi?')) return false; else BlockUI();" />
            </td>
        </tr>
    </table>
</div>
<!--fine div bottoni-->
<asp:HiddenField runat="server" ID="modalitaEdit" Value="false" />
<asp:HiddenField runat="server" ID="HiddenFieldCausaCessazione" />
<asp:HiddenField runat="server" ID="hiddenAttivitaSvolte" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaCalcolo" />
<asp:HiddenField runat="server" ID="hdnDecorrenzaCalcoloOriginale" />
