<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCIstruttoriaCi.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensioneCi.UCIstruttoriaCi" %>

<style type="text/css">
        .fixed-dialog{
          position: fixed;
        }
    </style>

<script type="text/javascript">
    function Confirm() {
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        var selectedValue = ddl.options[ddl.selectedIndex].value;
        if (selectedValue.toUpperCase() == 'SI')
            document.getElementById('<%= btnSalvaIstruttoria.ClientID %>').click();
        else
            $('#dialog-confirm').dialog('open');

        return false;

    }

    $(function() {
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
            open: function(event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
            close: function(event, ui) { $('body').css('overflow', 'auto'); },
            buttons: {
                'Annulla': function() {
                    $(this).dialog('close');
                    return false;
                },
                'Ok': function() {
                    $(this).dialog('close');
                    document.getElementById('<%= btnSalvaIstruttoria.ClientID %>').click();
                    return true;
                }
            }
        });
    });

    function checkPercentualeRiduzione(source, args) {
        var result = false;
        var ddl = document.getElementById('<%= ddlRiduzioneRetributiva.ClientID %>');
        if (ddl != null) {
            var selectedValue = ddl.options[ddl.selectedIndex].value;
            if (selectedValue.toUpperCase() == 'SI') {
                var txt = document.getElementById('<%= txtRiduzioneRetributiva.ClientID %>');
                if (txt.value == '')
                    result = false;
                else
                    result = true;
            }
            else
                result = true;
        }
        args.IsValid = result;
        return false;
    }
</script>

<asp:Panel runat="server" ID="pnlIstruttoria">
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Requisiti ridotti:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlCodReqRidotti" Width="94%" CssClass="tb8 txtUppercase"
                    TabIndex="1">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Panel ID="pnlSoggettoDerogato" runat="server" Visible="false">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Soggetto Derogato:</label>
            </td>
            <td class="field full-grid" colspan="3">
                <asp:DropDownList runat="server" ID="ddlSoggettoDerogato" Width="94%" CssClass="tb8 txtUppercase"
                    TabIndex="2" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        </asp:Panel>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Contratto per Equiparati:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCodContrEqu" Width="140px"
                    CssClass="txtUppercase tb8" MaxLength="4" TabIndex="3" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Codice Livello per Equiparati</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox Style="text-align: left" runat="server" ID="txtCodLivEqu" Width="140px"
                    CssClass="txtUppercase tb8" MaxLength="4" TabIndex="4" onblur="extractNumber(this,0,false);"
                    onkeyup="extractNumber(this,0,false);" onkeypress="return blockNonNumbers(this, event, false, false);"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <!-- Pannello Riduzione Retributiva-->
                <asp:Panel ID="pnlRiduzioneRetributiva" runat="server">
                    <table width="100%" class="tabellaFormattazione grid">
                        <tr style="vertical-align: bottom">
                            <td class="Row1" style="width: 25%">
                                <label>
                                    Riduzione Retributiva:</label>
                            </td>
                            <td class="Row1" style="width: 65%">
                                <asp:DropDownList ID="ddlRiduzioneRetributiva" CssClass="tb8 txtUppercase xxs" Width="15%" runat="server">
                                    <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                    <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:TextBox ID="txtRiduzioneRetributiva" runat="server" CssClass="tb8 txtUppercase"
                                    Width="14%" TabIndex="14" MaxLength="5"></asp:TextBox>
                                <label>
                                    %</label>
                                <asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator13"
                                    Display="Dynamic" ControlToValidate="txtRiduzioneRetributiva" Enabled="true"
                                    ErrorMessage="Riduzione Retributiva: Inserire valori interi o decimali" Text="*" CssClass="field-is-required"
                                    ValidationGroup="UCTabIstruttoria" ValidationExpression="\d{1,2}(\,\d{1,2})?" />
                                <asp:CustomValidator runat="server" ControlToValidate="ddlRiduzioneRetributiva" Display="Dynamic"
                                    ErrorMessage="Riduzione Retributiva: La percentuale è obbligatoria avendo selezionato 'SI'"
                                    Text="*" CssClass="field-is-required" ValidationGroup="UCTabIstruttoria" ID="customRiduzione" ClientValidationFunction="checkPercentualeRiduzione" />
                            </td>
                            <td style="width: 15%">
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- Fine Pannello Riduzione Retributiva-->
            </td>
        </tr>
    </table>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnPopUp" runat="server" SkinID="btnAzione1" CausesValidation="false" Visible="false"
                        Text="Salva Istruttoria" Width="170px" OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){return Confirm();}" CssClass="primary" />
                    <asp:Button ID="btnSalvaIstruttoria" runat="server" CausesValidation="false" Style="display: none" ValidationGroup="UCTabIstruttoria" 
                        SkinID="btnAzione1" Width="170px" OnClick="SalvaIstruttoria_Click" Text="Salva Istruttoria" Visible="false"
                        OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                    <asp:Button ID="btnSalvaIstruttoriaNoRiduzione" runat="server" CausesValidation="false" ValidationGroup="UCTabIstruttoria" 
                        SkinID="btnAzione1" Width="170px" OnClick="SalvaIstruttoria_Click" Text="Salva Istruttoria" Visible="true"
                        OnClientClick="if(Page_ClientValidate('UCTabIstruttoria')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaIstruttoria" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Istruttoria" Width="170px" OnClick="EliminaIstruttoria_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Istruttoria?')) return false; else BlockUI();" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>

<div id="dialog-confirm" title="Confirm" style="border-style: none; border-color: White;">
    <p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>Età titolare inferiore a 62 anni. Confermi la mancanza della percentuale di Riduzione?</p>
</div>