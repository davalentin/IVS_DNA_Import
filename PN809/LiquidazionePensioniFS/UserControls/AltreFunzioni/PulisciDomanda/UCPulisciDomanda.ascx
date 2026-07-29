<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPulisciDomanda.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.PulisciDomanda.UCPulisciDomanda" %>

<script type="text/javascript">
    function CreatePopUp() {
        // jQuery UI Dialog  
        var sedeDomanda = document.getElementById('<%=HdnSedeDomanda.ClientID %>').value;
        $('#changeSedeOperatore').text("La sede della domanda è " + sedeDomanda + ". Cambiare sede per proseguire?");
        var result;
        $('#changeSedeOperatore').dialog(
        {
            autoOpen: false,
            width: 400,
            modal: true,
            resizable: false,
            draggable: false,

            buttons:
            {
                "Annulla": function () {
                    $(this).dialog("close");
                    result = false;
                },
                "Conferma": function () {
                    $(this).dialog("close");
                    document.getElementById('<%= btnConfermaPopUp.ClientID %>').click();
                }
            }
        });
        $("#changeSedeOperatore").parent().appendTo($("form:first"));
    }

    function ShowPopUp() {
        var sedeOperatore = document.getElementById('<%=HdnSedeOperatore.ClientID %>');
        var sedeDomanda = document.getElementById('<%=HdnSedeDomanda.ClientID %>');

        if ((sedeOperatore == null && sedeDomanda == null) || sedeDomanda.value != sedeOperatore.value) {
            CreatePopUp();
            $('#changeSedeOperatore').dialog('open');
        }
    }

</script>

<table style="width: 720px;" class="full-width">
    <tr>
        <td style="width: 720px" class="full-width">
            <asp:Panel ID="Panel1" runat="server" Style="border-style: solid; border-color: #000080;
                min-height: 200px; border-collapse: collapse; border-width: 1px; width: 720px;
                margin-left: 0px; background-position: right top; background-repeat: no-repeat;
                background-image: url('../App_Themes/BlueINPS1/Images/clean.png');" CssClass="full-width form-container">
                <br />
                <!-- Pannello per la ricerca del numero domanda -->
                <div class="form-container background-light-blue">
                    <div class="single-line-container">
                        <label class="input-label">Numero Domanda:</label>
                        <div>
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda" Width="150px"
                                        MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                        onkeypress="return blockNonNumbers(this, event, false, false);" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                        ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAggiornaStatoDomanda" Enabled="true" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                        ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13"
                                        ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCAggiornaStatoDomanda" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                        Enabled="true" ErrorMessage="Inserire un numero Domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCAggiornaStatoDomanda" />
                        </div>

                        <asp:Button ID="btnRicerca" runat="server" Text="Cerca" SkinID="btnAzione1" Width="80px" CssClass="primary mt-0 mb-0"
                                        CausesValidation="false" OnClick="btnRicerca_Click" OnClientClick="if(Page_ClientValidate('UCPulisciDomanda')){aspnetForm.target ='_self'; BlockUI();}" />
                    </div>
                </div>

                <!-- Fine Pannello per la ricerca del numero domanda -->
                <!-- Pannello per la visualizzazione dei dati domanda -->
                <asp:Panel ID="pnlInfoDomanda" runat="server" Visible="false">
                    <br />
                    <table class="tabellaFormattazione">
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <label style="color: #336699; font-weight: bold; font-size: medium; width: 720px" class="section-label mt-16 mb-32">
                                    Riepilogo dati WebDom</label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <label class="etichettaBold">
                                    Numero Domanda:</label>
                            </td>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <asp:Label ID="lblNumeroDomanda" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <label class="etichettaBold">
                                    Stato WebDom:</label>
                            </td>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <asp:Label ID="lblStatoWebDom" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <label class="etichettaBold">
                                    Data Inizio:</label>
                            </td>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <asp:Label ID="lblDataInizio" runat="server"></asp:Label>
                            </td>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <label class="etichettaBold">
                                    Data Fine:</label>
                            </td>
                            <td class="Row1" style="width: 25%; text-align: left;">
                                <asp:Label ID="lblDataFine" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 25%; text-align: center;" colspan="4">
                                <asp:Button ID="btnChiudiAttivita" runat="server" Text="Chiudi Attività" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnChiudiAttivita_Click" OnClientClick="BlockUI();" />
                            </td>
                        </tr>
                    </table>
                    <br />
                </asp:Panel>
                <!-- Fine Pannello per la visualizzazione dei dati domanda -->
            </asp:Panel>
        </td>
    </tr>
</table>

<asp:HiddenField runat="server" ID="HdnSedeOperatore" />
<asp:HiddenField runat="server" ID="HdnSedeDomanda" />
<div id="changeSedeOperatore" title="Cambia sede" style="display: none;">
    <p></p>
</div>
<asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server" 
    OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />