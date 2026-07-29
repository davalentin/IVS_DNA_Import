<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSblocco.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.SbloccoDomanda.UCSblocco" %>
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

        if ((sedeOperatore == null && sedeDomanda == null) || sedeDomanda.value != sedeOperatore.value)
        {
            CreatePopUp();
            $('#changeSedeOperatore').dialog('open');
        }
    }

</script>

<div class="single-line-container">
    <label class="input-label">Numero Domanda: </label>

    <div>
        <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda"
                                Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"/>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSblocco" Enabled="true" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13" ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server"
                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCTabSblocco" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                Enabled="true" ErrorMessage="Inserire un numero Domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCTabSblocco" />
    </div>

    <asp:Button ID="btnSblocco" runat="server" Text="Sblocco" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnSblocco_Click" OnClientClick="if(Page_ClientValidate('UCTabSblocco')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary"/>


</div>

<asp:HiddenField runat="server" ID="HdnSedeOperatore" />
<asp:HiddenField runat="server" ID="HdnSedeDomanda" />
<div id="changeSedeOperatore" title="Cambia sede" style="display: none;">
    <p></p>
</div>
<asp:Button ID="btnConfermaPopUp" CausesValidation="true" Style="display: none" runat="server" 
    OnClick="btnConfermaPopUp_Click" OnClientClick="BlockUI();" Text="" />
