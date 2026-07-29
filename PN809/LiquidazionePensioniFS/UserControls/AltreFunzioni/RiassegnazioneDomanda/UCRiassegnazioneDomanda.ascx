<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRiassegnazioneDomanda.ascx.cs" 
Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.RiassegnazioneDomanda.UCRiassegnazioneDomanda" %>
<script type="text/jscript">
    function CreatePopUpSede() {
        // jQuery UI Dialog
        var sedeDomanda = document.getElementById('<%=HdnSede.ClientID %>').value;
        $('#changeSedeUtente').text("La sede della domanda è " + sedeDomanda + ". Cambiare sede per proseguire?");
        var result;
        $('#changeSedeUtente').dialog(
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
        $("#changeSedeUtente").parent().appendTo($("form:first"));
    }

    function ShowPopUpSede() {
        CreatePopUpSede();
        $('#changeSedeUtente').dialog('open');
    }

</script>

    <div class="form-container background-light-blue">
        <div class="single-line-container">
            <label class="input-label">Numero Domanda:</label>

            <div>
                <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda"
                                                Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                                onkeypress="return blockNonNumbers(this, event, false, false);"/>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                                ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRiassegnazioneRicercaDomanda" Enabled="true" />
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                                ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13" ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server"
                                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCRiassegnazioneRicercaDomanda" Enabled="true" />                                
                                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                                Enabled="true" ErrorMessage="Inserire un numero Domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                                ValidationGroup="UCRiassegnazioneRicercaDomanda" />
            </div>

            <asp:Button ID="btnRicerca" runat="server" Text="Cerca" SkinID="btnAzione1" Width="80px" CausesValidation="false" OnClick="btnRicerca_Click" 
                                            OnClientClick="if(Page_ClientValidate('UCRiassegnazioneRicercaDomanda')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>
        </div>
    </div>
    
    <table style="width: 720px;" class="full-width">
        <tr>
            <td style="width: 720px" class="full-width">                    
                    <asp:Panel ID="Panel1" runat="server" Style="border-style: solid; border-color: #000080; min-height:200px; 
                        border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px; background-position: right top; background-repeat: no-repeat;
                        background-image: url('../App_Themes/BlueINPS1/Images/users.png');" CssClass="iframe-bg-users form-container">
                    <br />
                    
                    <!-- Pannello per la visualizzazione dei dati domanda -->
                    <asp:Panel ID="pnlInfoDomanda" runat="server" Visible="false" CssClass="full-width">
                    <br />
                        <table class="tabellaFormattazione">
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="width:720px" colspan="4" class="full-width">
                                    <label style="color: #336699; font-weight: bold; font-size:medium; width:720px" class="riassegnazione-section-title full-width section-label mt-8 mb-16 ">Riepilogo dati domanda</label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 20%;">
                                    <label class="etichettaBold">
                                        Numero Domanda:</label>
                                </td>
                                <td class="Row1" style="width: 30%; text-align: left">
                                    <asp:Label ID="lblNumeroDomanda" runat="server"></asp:Label>
                                </td>
                                <td class="Row1" style="width: 20%;">
                                    <label class="etichettaBold">
                                        Stato Pensione:</label>
                                </td>
                                <td class="Row1" style="width: 30%; text-align: left">
                                    <asp:Label ID="lblStatoPensione" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="Row1" style="width: 20%;">
                                    <label class="etichettaBold">
                                        Matricola attuale:</label>
                                </td>
                                <td class="Row1 full-grid" style="width: 80%; text-align: left" colspan="3">
                                    <asp:Label ID="lblMatricola" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                         </table>   

                         <br />

                        <div class="single-line-container mt-16">
                            <label class="input-label">Nuova Matricola:</label>

                            <div>
                                <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNuovaMatricola"
                                        Width="150px" MaxLength="8"/>                   
                                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtNuovaMatricola"
                                        Enabled="true" ErrorMessage="Inserire una matricola" Text="*" CssClass="field-is-required" Display="Dynamic"
                                        ValidationGroup="UCRiassegnazioneAggiornaDomanda" />
                            </div>
                        </div>

                        <div class="justify-end">
                            <asp:Button ID="btnRiassegna" runat="server" Text="Riassegna" SkinID="btnAzione1" Width="80px" CausesValidation="false" OnClick="btnRiassegna_Click" 
                                        OnClientClick="if(Page_ClientValidate('UCRiassegnazioneAggiornaDomanda')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary mr-0"/>
                        </div>

                        <br />
                    </asp:Panel>
                    <!-- Fine Pannello per la visualizzazione dei dati domanda -->
                </asp:Panel>
            </td>
        </tr>
    </table>
     <!-- Cambio Sede -->
          <div id="changeSedeUtente" title="Cambia sede" style="display: none;">
            <p></p>
        </div>
        <asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server" 
            OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />
    <asp:HiddenField ID="HdnSede" runat="server"></asp:HiddenField>
  