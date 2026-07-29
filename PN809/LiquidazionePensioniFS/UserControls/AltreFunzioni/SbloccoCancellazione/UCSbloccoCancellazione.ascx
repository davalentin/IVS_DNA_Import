<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCSbloccoCancellazione.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.SbloccoCancellazione.UCSbloccoCancellazione" %>

<script type="text/javascript">
    $(function () {
        var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
        // alert(availableTags);
        $("#<%=txtSede.ClientID%>").autocomplete({
            minLength: 0,
            source: availableTags,
            open: function () {
                $(this)
                    .autocomplete("widget")
                    .css({
                        "margin-top": "8px",
                        "width": $(this).outerWidth() + "px"
                    })
            }
        });
    }); 
</script>

<table class="tabellaFormattazione">
    <tr>
        <td style="width: 720px" class="full-width">
        <asp:Panel ID="panSbloccoCancellazione" runat="server" Style="border-style: solid; border-color: #000080;
                border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 0px; background-position: right top; background-repeat: no-repeat;
                        background-image: url('../App_Themes/BlueINPS1/Images/lucchetto.jpg');" CssClass="iframe-bg-lucchetto full-width form-container">
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr align="center">
                        <td class="Row1" align="right">
                            <label>
                                Numero Domanda: </label>
                        </td>
                        <td class="field" align="left">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtNumeroDomanda"
                                Width="150px" MaxLength="13" onblur="extractNumber(this,0,false);" onkeyup="extractNumber(this,0,false);"
                                onkeypress="return blockNonNumbers(this, event, false, false);"/>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator4" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Numero domanda non valido" ValidationExpression="^[0-9]{13}$" runat="server"
                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCSbloccoCancellazione" Enabled="true" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" ControlToValidate="txtNumeroDomanda"
                                ErrorMessage="Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13" ValidationExpression="^[1-9]{1}[0-9]{12}$" runat="server"
                                Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="UCSbloccoCancellazione" Enabled="true" />                                
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator4" ControlToValidate="txtNumeroDomanda"
                                Enabled="true" ErrorMessage="Inserire un numero domanda" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCSbloccoCancellazione" />
                        </td>
                    </tr>
                    <tr align="center">
                        <td class="Row1" align="right">
                            <label>
                                Sede: </label>
                        </td>
                        <td class="field" align="left">
                            <asp:TextBox runat="server" CssClass="tb8 txtUppercase" ID="txtSede"
                                Width="300px"/>
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtSede"
                                Enabled="true" ErrorMessage="Inserire la sede" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCSbloccoCancellazione" />
                        </td>
                    </tr>
                    <tr align="center">
                        <td class="Row1" align="right">
                            <label>
                                Sigla Categoria: </label>
                        </td>
                        <td class="field" align="left">
                            <asp:DropDownList runat="server" CssClass="tb8 txtUppercase" ID="ddlCategoriaPensione"
                                Width="150px" />
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="ddlCategoriaPensione"
                                Enabled="true" ErrorMessage="Inserire la sigla categoria" Text="*" CssClass="field-is-required" Display="Dynamic"
                                ValidationGroup="UCSbloccoCancellazione" />
                        </td>
                    </tr>
                    <tr align="center">
                        <td class="Row1" align="right">
                            <label>
                                Tipo Operazione: </label>
                        </td>
                        <td class="field" align="left">
                            <asp:DropDownList runat="server" CssClass="tb8 txtUppercase" ID="ddlTipoOperazione" Width="150px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <table class="tabellaFormattazione" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btnSbloccoCancellazione" runat="server" Text="Sblocco Cancellazione" SkinID="btnAzione1"
                                CausesValidation="false" OnClick="btnSbloccoCancellazione_Click" OnClientClick="if(Page_ClientValidate('UCSbloccoCancellazione')){aspnetForm.target ='_self'; BlockUI();}" CssClass="primary force-right"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>

<asp:HiddenField runat="server" ID="HiddenFieldSedi"/>