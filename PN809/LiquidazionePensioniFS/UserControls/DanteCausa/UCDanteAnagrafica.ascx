<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDanteAnagrafica.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa.UCDanteAnagrafica" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
<script type="text/javascript">
    $(document).ready(function () {
        SetCalendarioDataMatrimonio();
        OnChangeddlresidenzaestero();
        $(document.getElementById("<%= ddlresidenzaestero.ClientID %>")).change(function () {
            OnChangeddlresidenzaestero();
        });
    });
    function SetCalendarioDataMatrimonio() {
        if ($(document.getElementById("<%=txtDataMorte.ClientID%>")).is(':disabled') == false) {
            $(document.getElementById("<%=txtDataMorte.ClientID%>")).datepicker({
                changeMonth: true,
                changeYear: true,
                changeDay: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                showOn: 'button',
                buttonImageOnly: true,
                buttonImage: '../App_Themes/<%= Page.Theme %>/Images/calendar1.png',
                yearRange: 'c-70:' + 'c+70:'
            });
            //$(document.getElementById("<%=txtDataMorte.ClientID%>")).unmask();
            //$(document.getElementById("<%=txtDataMorte.ClientID%>")).mask("99/99/9999");
        }
    }

    function OnChangeddlresidenzaestero() {
        var statoEstero = document.getElementById("<%= ddlresidenzaestero.ClientID %>") != null ? document.getElementById("<%= ddlresidenzaestero.ClientID %>").value : "";
        if (statoEstero != "" && statoEstero != "Z000") {
            $(document.getElementById("<%= txtResidenzaEE_Dal.ClientID %>")).datepicker("enable");
        }
        else {
            $(document.getElementById("<%= txtResidenzaEE_Dal.ClientID %>")).datepicker("disable");
        }
    }

    function Confirm() {
        var dataMatrimonio = document.getElementById("<%= txtDataMatrimonio.ClientID %>") != null ? document.getElementById("<%= txtDataMatrimonio.ClientID %>").value : "";
        var dataNascitaDC = document.getElementById("<%= lblDataNascitaAnagrafica.ClientID %>").innerText;
        var dataNascitaContitolareConiuge = document.getElementById("<%= hdnDataNascitaContitolareConiuge.ClientID %>").value;
        var flag = false;
        if (dataMatrimonio !== undefined && dataMatrimonio != "") {
            var dateApp = dataMatrimonio.split("/");
            var date1 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
            if (dataNascitaDC !== undefined && dataNascitaDC != "") {
                dateApp = dataNascitaDC.split("/");
                var date2 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                date2.setFullYear(date2.getFullYear() + 16);
                if (date1 < date2)
                    flag = true;
            }
            if (!flag) {
                if (dataNascitaContitolareConiuge !== undefined && dataNascitaContitolareConiuge != "") {
                    dateApp = dataNascitaContitolareConiuge.split("/");
                    var date2 = new Date(dateApp[2], dateApp[1] - 1, dateApp[0]);
                    date2.setFullYear(date2.getFullYear() + 16);
                    if (date1 < date2)
                        flag = true;
                }
            }
        }

        if (!flag)
            document.getElementById('<%= btnSalvaAnagrafica.ClientID %>').click();
        else
            $('#dialog-confirm').dialog('open');

        return false;

    }

    $(function () {
        $('#dialog-confirm').dialog({
            autoOpen: false,

            show: 'blind',
            hide: 'blind',
            height: 220,
            width: 450,
            modal: true,
            centerX: true,
            centerY: true,
            dialogClass: 'fixed-dialog',
            resizable: false,
            draggable: true,
            open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function (event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function () {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function () {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaAnagrafica.ClientID %>').click();
                    return true;
                }
            }
        });
    });

    function PrevalorizzaCittadinanza() {
        $('#<%= ddlCittadinanza.ClientID %>').val("Z000");
    }
</script>
<asp:Panel runat="server" ID="pnlAnagrafica">
    <div runat="server" visible="true" id="divDatiAnagrafica">
        <br />
        <table class="tabellaFormattazione grid grid-size-25">
            <asp:Panel runat="server" ID="pnlCodiceFiscale">
                <tr>
                    <td style="width: 20%" class="Row1">
                        <label>
                            Codice Fiscale:</label>
                    </td>
                    <td style="width: 80%" colspan="3" class="field full-grid">
                        <asp:Label runat="server" ID="lblCFAnagrafica" Enabled="true" CssClass="txtUppercase etichettaBold"></asp:Label>
                        <asp:TextBox ID="txtCodiceFiscaleAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                            Width="40%" Visible="false" MaxLength="16"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfvCodiceFiscale" ControlToValidate="txtCodiceFiscaleAnagrafica"
                            Enabled="false" ErrorMessage="Inserire il Codice Fiscale" Text="*" Display="Dynamic"
                            ValidationGroup="UCDanteAnagrafica" CssClass="offClass  field-is-required onClassInvioPosizione" />
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td style="width: 20%" class="Row1">
                    <label>
                        Cognome:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblCognomeAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtCognomeAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                        Width="60%" Visible="false"></asp:TextBox>
                </td>
                <td style="width: 20%" class="Row1">
                    <label>
                        Nome:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblNomeAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtNomeAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                        Width="60%" Visible="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 20%" class="Row1">
                    <label>
                        Sesso:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblSessoAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtSessoAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                        Width="60%" Visible="false" MaxLength="1"></asp:TextBox>
                </td>
                <td style="width: 20%" class="Row1">
                    <label>
                        Data di Nascita:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblDataNascitaAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtDataNascitaAnagrafica" runat="server" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                        Text="gg/mm/aaaa" MaxLength="10" Width="60%" Visible="false"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 20%" class="Row1">
                    <label>
                        Comune di Nascita:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblComuneNascitaAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtComuneNascitaAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                        Width="60%" Visible="false"></asp:TextBox>
                </td>
                <td style="width: 20%" class="Row1">
                    <label>
                        Provincia di Nascita:</label>
                </td>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblProvinciaNascitaAnagrafica" CssClass="txtUppercase etichettaBold"></asp:Label>
                    <asp:TextBox ID="txtProvinciaNascitaAnagrafica" runat="server" CssClass="tb8 txtUppercase etichettaBold"
                        Width="60%" Visible="false" MaxLength="3"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 25%" class="Row1">
                    <asp:Label runat="server" ID="lblDataMatrimonioUnioneCivile">
                    Data matrimonio / Unione civile:
                    </asp:Label>
                </td>
                <td style="width: 30%" class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataMatrimonio" CssClass="txtUppercase tb8 date-picker-base-maxActual dateGGmmAAAA"
                        TabIndex="1" Text="gg/mm/aaaa" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="txtDataMatrimonio"
                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire un formato valido per Data matrimonio"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$"
                        ValidationGroup="UCDanteAnagrafica" Text="*" CssClass="field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataMatrimonio" Display="Dynamic"
                        ErrorMessage="Data matrimonio: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCDanteAnagrafica" ID="customDataMatrimonioPost" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataMatrimonio" Display="Dynamic"
                        ErrorMessage="Data Matrimonio: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDanteAnagrafica"
                        ID="customCheckDataDataMatrimonio" ClientValidationFunction="checkCorrettezzaData" />
                </td>
                <td style="width: 20%" class="Row1">
                    <label>
                        Data morte:</label>
                </td>
                <td style="width: 30%" class="field">
                    <asp:TextBox Style="text-align: left" runat="server" ID="txtDataMorte" CssClass="txtUppercase tb8 dateGGmmAAAA"
                        TabIndex="1" Text="gg/mm/aaaa" MaxLength="10"></asp:TextBox>
                    <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator3" ControlToValidate="txtDataMorte"
                        Display="Dynamic" Enabled="true" ErrorMessage="Inserire un formato valido per Data morte"
                        ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" ValidationGroup="UCDanteAnagrafica"
                        Text="*" CssClass="field-is-required" />
                    <asp:RequiredFieldValidator runat="server" ID="RequiredValidatorDataMorte" ControlToValidate="txtDataMorte"
                        Enabled="true" ErrorMessage="Inserire la Data morte" Text="*" Display="Dynamic"
                        ValidationGroup="UCDanteAnagrafica" CssClass="offClass  onClassInvioPosizione field-is-required" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataMorte" Display="Dynamic"
                        ErrorMessage="Data Morte: Data inserita posteriore a quella odierna" Text="*" CssClass="field-is-required"
                        ValidationGroup="UCDanteAnagrafica" ID="customDataMortePost" ClientValidationFunction="checkDataPostOdiernaGGMMAAAA" />
                    <asp:CustomValidator runat="server" ControlToValidate="txtDataMorte" Display="Dynamic"
                        ErrorMessage="Data morte: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDanteAnagrafica"
                        ID="customCheckDataDataMorte" ClientValidationFunction="checkCorrettezzaData" />
                </td>
            </tr>
            <tr runat="server" id="trCodiceFascicolo" visible="false">
                <td style="width: 20%" class="Row1">
                    <label>
                        Codice fascicolo:</label>
                </td>
                <td style="width: 30%" class="field">
                    <asp:Label runat="server" ID="lblCodiceFascicolo" CssClass="txtUppercase etichettaBold"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <div runat="server" id="divCI_AGO" visible="false">
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Stato Estero di residenza:</label>
                    </td>
                    <td class="field full-grid">
                        <asp:DropDownList runat="server" ID="ddlresidenzaestero" TabIndex="2" Width="65%"
                            CssClass="txtUppercase tb8" onchange="OnChangeddlresidenzaestero">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Dal:</label>
                    </td>
                    <td class="field full-grid">
                        <asp:TextBox Style="text-align: left" runat="server" ID="txtResidenzaEE_Dal" Width="95px"
                            CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="3" Text="mm/aaaa"
                            MaxLength="7"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="RegularExpressiontxtresidenzadalSE"
                            ControlToValidate="txtResidenzaEE_Dal" Display="Dynamic" Enabled="true" ErrorMessage="Inserire la data nel formato valido per  Residenza Dal Stato Estero "
                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" ValidationGroup="UCDanteAnagrafica"
                            Text="*" CssClass="field-is-required" />
                        <asp:CustomValidator runat="server" ControlToValidate="txtResidenzaEE_Dal" Display="Dynamic"
                            ErrorMessage="Dal: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCDanteAnagrafica"
                            ID="customCheckDataDal" ClientValidationFunction="checkCorrettezzaData" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label>
                            Cittadinanza:</label>
                    </td>
                    <td class="field full-grid flex-space">
                        <asp:DropDownList runat="server" ID="ddlCittadinanza" TabIndex="4" Width="65%" CssClass="txtUppercase tb8 width-72-percent">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlCittadinanza" Enabled="true"
                            ErrorMessage="Inserire la Cittadinanza" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCDanteAnagrafica"
                            InitialValue="" ID="RFVCittadinanza" />
                        <asp:Button runat="server" ID="btnCittadinanza" SkinID="btnAzione1" OnClientClick="PrevalorizzaCittadinanza(); return false;"
                            Text="<< Italia" CssClass="tertiary" />
                    </td>
                </tr>
                <%--<tr>
                    <td style="width:30%" class="Row1">
                        <label>Codice Eliminazione:</label>
                    </td>
                    <td class="field">
                        <asp:DropDownList Width="90%" runat="server" ID="ddlCodiceEliminazione" CssClass="txtUppercase tb8">
                        </asp:DropDownList>
                    </td>                    
                </tr> --%>
                <tr id="trParentela" runat="server" visible="false">
                    <td style="width: 30%" class="Row1">
                        <label>
                            Relazione di parentela dell'avente diritto con il DC:</label>
                    </td>
                    <td class="field full-grid">
                        <asp:DropDownList runat="server" ID="ddlRelazioneParentela" Width="65%" CssClass="txtUppercase tb8" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 30%" class="Row1">
                        <label id="lblCodiceProvenienza" runat="server">
                            Codice Provenienza:</label>
                    </td>
                    <td class="field full-grid">
                        <asp:DropDownList runat="server" ID="ddlCodiceProvenienza" Width="65%" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Visible="false" Text="Salva Anagrafica" Width="150px" OnClientClick="if(Page_ClientValidate('UCDanteAnagrafica')){return Confirm();}" CssClass="primary"/>
                    <asp:Button ID="btnSalvaAnagrafica" TabIndex="6" runat="server" SkinID="btnAzione1"
                        Enabled="true" Text="Salva Anagrafica" Width="130px" CausesValidation="true"
                        ValidationGroup="UCDanteAnagrafica" OnClick="btnSalvaAnagrafica_Click" OnClientClick="if(Page_ClientValidate('UCDanteAnagrafica')){aspnetForm.target ='_self'; BlockUI();}"  CssClass="primary" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
<asp:HiddenField runat="server" ID="hdnIsResidenzaEE_DalEnabled" />
<asp:HiddenField runat="server" ID="hdnDataNascitaContitolareConiuge" />
<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p>
        <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
        La data di matrimonio è inferiore al compimento dei 16 anni di età del Contitolare
        coniuge e/o del dante causa. Confermi l'acquisizione?
    </p>
</div>
