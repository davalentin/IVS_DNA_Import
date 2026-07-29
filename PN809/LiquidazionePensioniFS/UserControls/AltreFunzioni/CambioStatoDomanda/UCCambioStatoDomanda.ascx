<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCCambioStatoDomanda.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.CambioStatoDomanda.UCCambioStatoDomanda" %>

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
        <td style="width: 720px"  class="full-width">
            <asp:Panel ID="Panel1" runat="server" Style="border-style: solid; border-color: #000080; min-height: 200px; border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px; background-position: right top; background-repeat: no-repeat;"
                CssClass="full-width form-container">
                <br />
                <!-- Pannello per la ricerca del numero domanda -->
                <div class="form-container background-light-blue">
                    <div class="single-line-container">
                        <label class="input-label">Numero Domanda:</label>

                        <div>
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda"
                                        Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                        onkeypress="return blockNonNumbers(this, event, false, false);" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                        ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAggiornaStatoDomanda" Enabled="true" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                        ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13" ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server"
                                        Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCAggiornaStatoDomanda" Enabled="true" />
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                        Enabled="true" ErrorMessage="Inserire un numero Domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCAggiornaStatoDomanda" />
                        </div>

                        <asp:Button ID="btnRicerca" runat="server" Text="Cerca" SkinID="btnAzione1" Width="80px" CausesValidation="false" OnClick="btnRicerca_Click"
                                        OnClientClick="if(Page_ClientValidate('UCAggiornaStatoDomanda')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary mt-0 mb-0" />
                    </div>
                </div>

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
                            <td style="width: 720px" colspan="2">
                                <label style="color: #336699; font-weight: bold; font-size: medium; width: 720px" class="section-label mt-8 mb-16">Riepilogo dati domanda</label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label class="etichettaBold">
                                    Numero Domanda:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:Label ID="lblNumeroDomanda" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label class="etichettaBold">
                                    Stato Domanda:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:Label ID="lblStatoPensioneAttuale" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label class="etichettaBold">
                                    Numero Certificato:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:Label ID="lblNumeroCertificatoAttuale" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label class="etichettaBold">
                                    Data Elab. Webdom:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:Label ID="lblDataElabWebdomAttuale" runat="server"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td>
                                <br />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table class="tabellaFormattazione mt-16">
                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label>Nuovo Stato:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:DropDownList ID="ddlStatoPensione" runat="server" Width="50%" CssClass="txtUppercase tb8"></asp:DropDownList>
                            </td>
                        </tr>

                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label>Nuovo Numero Certificato:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNuovoNCertificato"
                                    Width="150px" MaxLength="8" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                    onkeypress="return blockNonNumbers(this, event, false, false);" />
                            </td>
                        </tr>

                        <tr>
                            <td class="Row1" style="width: 10%; text-align: left;">
                                <label>Nuova Data Elab. Webdom:</label>
                            </td>
                            <td class="Row1" style="width: 30%; text-align: left;">
                                <asp:TextBox Style="text-align: left" runat="server" onblur="setpnlTxtPerfRequisitiVisibility(this);"
                                    onkeydown="checkTabPress(this)" ID="txtNuovaDataElaborazioneWebdom" Width="150px" Text="gg/mm/aaaa"
                                    CssClass="txtUppercase tb8 date-picker-base dateGGmmAAAA" TabIndex="5" MaxLength="10"
                                    DataFormatString="{0:dd/MM/yyyy}"> </asp:TextBox>
                            </td>
                        </tr>

                    </table>

                    <div class="justify-end">
                        <asp:Button ID="Button1" runat="server" Text="Aggiorna dati" SkinID="btnAzione1" CausesValidation="false" OnClick="btnCambiaStato_Click" Width="130px"
                                    OnClientClick="BlockUI();"  CssClass="ghost-update mr-0 mt-16"/>
                    </div>

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