<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCVittimeFS.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBenefici.UCVittimeFS" %>
<script type="text/javascript">
    $(document).ready(function () {
        changeSoggettoBeneficiario();
        changeTipologiaPrestazione();

        $("#divSoggettoBeneficiario").mouseover(function () {
            var soggetto = document.getElementById("<%= ddlSoggettoBeneficiario.ClientID %>");
            soggetto.title = soggetto.options[soggetto.selectedIndex].title;
        });

        $("#divTipologiaPrestazione").mouseover(function () {
            var tipologia = document.getElementById("<%= ddlTipologiaPrestazione.ClientID %>");
            tipologia.title = tipologia.options[tipologia.selectedIndex].title;
        });

        $("#divTipologiaBeneficioTerrorismo").mouseover(function () {
            var tipologia = document.getElementById("<%= ddlTipologiaBeneficioTerrorismo.ClientID %>");
            tipologia.title = tipologia.options[tipologia.selectedIndex].title;
        });
    });

    function changeSoggettoBeneficiario() {
        var soggettoBeneficiario = $("#<%= ddlSoggettoBeneficiario.ClientID %>");
        var tipologiaPrestazione = $("#<%= ddlTipologiaPrestazione.ClientID %>");
        $("#<%= ddlTipologiaPrestazione.ClientID %>").removeAttr("disabled");
        $("#<%= ddlTipologiaPrestazione.ClientID %> option").removeAttr("disabled");
        if (soggettoBeneficiario) {
            switch (soggettoBeneficiario.val()) {
                case "1":
                    $("#<%= ddlTipologiaPrestazione.ClientID %>").val("1");
                    $("#<%= ddlTipologiaPrestazione.ClientID %>").attr("disabled", "disabled");
                    break;
                case "2":
                    $("#<%= ddlTipologiaPrestazione.ClientID %> option[value=1]").attr("disabled", "disabled");
                    if ($("#<%= ddlTipologiaPrestazione.ClientID %> option:selected").val() == 1) {
                        $("#<%= ddlTipologiaPrestazione.ClientID %>").val('');
                    }
                    break;
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                    $("#<%= ddlTipologiaPrestazione.ClientID %>").val("2");
                    $("#<%= ddlTipologiaPrestazione.ClientID %>").attr("disabled", "disabled");
                    break;
            }
            $("#<%= ddlTipologiaPrestazione.ClientID %>").change();
        }
    }

    function changeTipologiaPrestazione() {
        var tipologiaPrestazione = $("#<%= ddlTipologiaPrestazione.ClientID %>");
        var soggettoBeneficiario = $("#<%= ddlSoggettoBeneficiario.ClientID %>");
        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").removeAttr("disabled");
        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option").removeAttr("disabled");
        if (tipologiaPrestazione) {
            switch (tipologiaPrestazione.val()) {
                case "1":
                    if (soggettoBeneficiario && soggettoBeneficiario.val() == 1) {
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").val("7");
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").attr("disabled", "disabled");
                    }
                    break;
                case "2":
                    if (soggettoBeneficiario && (soggettoBeneficiario.val() != 4 && soggettoBeneficiario.val() != 7)) {
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option[value!=1][value!=2]").attr("disabled", "disabled");
                        if ($("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option:selected").val() != 1 &&
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option:selected").val() != 2) {
                            $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").val('');
                        }
                    }

                    if (soggettoBeneficiario && (soggettoBeneficiario.val() == 5 || soggettoBeneficiario.val() == 6 || soggettoBeneficiario.val() == 8)) {
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").val("2");
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").attr("disabled", "disabled");
                    }

                    if (soggettoBeneficiario && (soggettoBeneficiario.val() == 4 || soggettoBeneficiario.val() == 7)) {
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option[value!=2][value!=3]").attr("disabled", "disabled");
                        if ($("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option:selected").val() != 2 &&
                            $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %> option:selected").val() != 3) {
                            $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").val('');
                        }
                    }
                    break;
                case "3":
                    if (soggettoBeneficiario && soggettoBeneficiario.val() == 2) {
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").val("5");
                        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").attr("disabled", "disabled");
                    }
                    break;
                case "4":
                    $("#<%= ddlTipologiaPrestazione.ClientID %>").attr("disabled", "disabled");
                    break;
            }
        }
    }

    function RiabilitaCampi() {
        $("#<%= ddlTipologiaPrestazione.ClientID %>").removeAttr("disabled");
        $("#<%= ddlTipologiaBeneficioTerrorismo.ClientID %>").removeAttr("disabled");
    }
</script>
<asp:Panel runat="server" ID="pnlVittime">
    <table class="tabellaFormattazione grid grid-size-25">
        <tr runat="server" ID="trSoggettoBeneficiario">
            <td class="Row1" style="width: 25%">
                <label>
                    Soggetto Beneficiario:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <div id="divSoggettoBeneficiario">
                    <asp:DropDownList runat="server" ID="ddlSoggettoBeneficiario" Width="97%" CssClass="tb8 txtUppercase"
                        onchange="changeSoggettoBeneficiario()">
                    </asp:DropDownList>
                </div>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlSoggettoBeneficiario" Display="Dynamic"
                    ErrorMessage="Soggetto Beneficiario: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabVittime"
                    ControlToValidate="ddlSoggettoBeneficiario"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr runat="server" ID="trCodiceEvento">
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Evento:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:DropDownList runat="server" ID="ddlCodiceEvento" Width="70%" CssClass="tb8 txtUppercase">
                    <asp:ListItem Value="" Text=""></asp:ListItem>
                    <asp:ListItem Value="I" Text="Italia"></asp:ListItem>
                    <asp:ListItem Value="E" Text="Estero"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlCodiceEvento" Display="Dynamic"
                    ErrorMessage="Codice Evento: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabVittime"
                    ControlToValidate="ddlCodiceEvento"></asp:RequiredFieldValidator>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Data Evento Terroristico:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox runat="server" ID="txtDataEventoTerroristico" CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA"
                    MaxLength="10" Text="GG/MM/AAAA"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ID="RFVtxtDataEventoTerroristico" ControlToValidate="txtDataEventoTerroristico"
                    Display="Dynamic" Enabled="true" ErrorMessage="Data Evento Terroristico: Campo obbligatorio"
                    ValidationGroup="UCTabVittime" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator runat="server" ID="REVDataEventoTerroristico" ControlToValidate="txtDataEventoTerroristico"
                    ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}$" Enabled="true" Text="*" CssClass="field-is-required"
                    ErrorMessage="Data Evento Terroristico: Formato non valido" Display="Dynamic"
                    ValidationGroup="UCTabVittime" />
                <asp:CustomValidator runat="server" ControlToValidate="txtDataEventoTerroristico"
                    Display="Dynamic" ErrorMessage="Decorrenza: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabVittime"
                    ID="customCheckDataEventoTerroristico" ClientValidationFunction="checkCorrettezzaData" />
            </td>
        </tr>
        <tr runat="server" ID="trTipologiaDellaPrestazione">
            <td class="Row1" style="width: 25%">
                <label>
                    Tipologia della prestazione:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <div id="divTipologiaPrestazione">
                    <asp:DropDownList runat="server" ID="ddlTipologiaPrestazione" Width="97%" CssClass="tb8 txtUppercase"
                        onchange="changeTipologiaPrestazione()">
                    </asp:DropDownList>
                </div>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlTipologiaPrestazione" Display="Dynamic"
                    ErrorMessage="Tipologia della prestazione: campo obbligatorio" Text="*" CssClass="field-is-required" ValidationGroup="UCTabVittime"
                    ControlToValidate="ddlTipologiaPrestazione"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr runat="server" ID="trTipologiaDelBeneficio">
            <td class="Row1" style="width: 25%">
                <label>
                    Tipologia del beneficio:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <div id="divTipologiaBeneficioTerrorismo">
                    <asp:DropDownList runat="server" ID="ddlTipologiaBeneficioTerrorismo" Width="97%"
                        CssClass="tb8 txtUppercase">
                    </asp:DropDownList>
                </div>
                <asp:RequiredFieldValidator runat="server" ID="RFVddlTipologiaBeneficioTerrorismo"
                    Display="Dynamic" ErrorMessage="Tipologia del beneficio: campo obbligatorio"
                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabVittime" ControlToValidate="ddlTipologiaBeneficioTerrorismo"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="margin-top: 100px; margin-right: 40px;" class="containerWidth xs">
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: right" class="tab-actions-group__first">
                <asp:Button ID="btnSalva" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Salva Vittime" Width="160px" OnClick="SalvaVittime_Click"
                    OnClientClick="if(Page_ClientValidate('UCTabVittime')){RiabilitaCampi(); aspnetForm.target ='_self'; BlockUI();}"  CssClass="primary" />
            </td>
            <td style="text-align: left">
                <asp:Button ID="btnEliminaVittime" runat="server" SkinID="btnAzione1" CausesValidation="false"
                    Enabled="true" Text="Elimina Vittime" Width="160px" OnClick="EliminaVittime_Click"
                    OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare Vittime?')) return false; else BlockUI();"  CssClass="ghost-delete"/>
            </td>
        </tr>
    </table>
</div>
