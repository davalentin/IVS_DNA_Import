<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCLiquidazionePensioneStorico.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.LiquidazionePensione.UCDatiStorico" %>
<style type="text/css" media="screen">
    #divStorico h3.trigger
    {
        width: 90%;
        margin: 20px 0px 0px 10px;
    }
    #divStorico .fakeLink
    {
        color: Black;
        cursor: pointer;
    }
    
    #divStorico .fakeLink:hover
    {
        color: #ccc;
    }
    
    #divStorico div.collapsibleContainer
    {
        margin-left: 50px;
        border-style: solid;
        border-color: #000080;
        border-collapse: collapse;
        border-width: 1px;
        width: 90%;
        margin-top: 4px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {

        if ($("#<%= pnlDatiGenerici.ClientID %> .fakeLink"))
            $("#<%= pnlDatiGenerici.ClientID %> .fakeLink").click(function () {
                $(this).toggleClass("active").next().slideToggle("fast");
            });

        if ($("#<%= pnlDatiAssicurativi.ClientID %> .fakeLink"))
            $("#<%= pnlDatiAssicurativi.ClientID %> .fakeLink").click(function () {
                $(this).toggleClass("active").next().slideToggle("fast");
            });
    });
</script>
<div id="divStorico" style="margin-top: 4px;">
    <asp:Panel runat="server" ID="pnlDatiGenerici" Visible="false">
        <h3 class='trigger fakeLink'>
            Dati Generici
        </h3>
        <asp:Panel CssClass="collapsibleContainer PnlContenitoreDatiInterno" runat="server" id="divDatiGenerici" Enabled="false">
            <table class="tabellaFormattazione grid grid-size-25">
                <tr runat="server" id="trDeroga" visible="false">
                    <td class="Row1">
                        <label>
                            Deroga:
                        </label>
                    </td>
                    <td class="field full-grid" colspan="3">
                        <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlDeroga" runat="server" Enabled="false"
                            Width="513px" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Pensione:</label>
                    </td>
                    <td class="field">
                        <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiGenerici" Text=""></asp:Label>
                    </td>
                    <td class="Row1">
                    </td>
                    <td class="field">
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Tipo Calcolo:</label>
                    </td>
                    <td class="field full-grid" colspan="3">
                        <asp:DropDownList runat="server" ID="ddlTipoCalcolo" Width="90%" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="Row1">
                        <label>
                            Codice Comunicazioni / Provvisoria:</label>
                    </td>
                    <td class="field full-grid" colspan="3">
                        <asp:DropDownList runat="server" ID="ddlCodComunicazioni3" Width="90%" CssClass="tb8 txtUppercase">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDatiAssicurativi" Visible="false">
        <h3 class='trigger fakeLink'>
            Dati Assicurativi
        </h3>
        <asp:Panel CssClass="collapsibleContainer PnlContenitoreDatiInterno" runat="server" id="divDatiAssicurativi" Enabled="false">
            <table class="tabellaFormattazione grid grid-size-25">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Decorrenza Pensione:</label>
                    </td>
                    <td class="field" colspan="3">
                        <asp:Label runat="server" ID="lblDecorrenzaPensioneDatiAssicurativi" />
                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Primo Versamento:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:Panel runat="server" ID="pnlTxtPrimoVersamento">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtPrimoVersamento" Width="50%"
                                Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                        </asp:Panel>
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>
                            Ultimo Versamento:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:Panel runat="server" ID="pnlTxtUltimoVersamento">
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtUltimoVersamento" Width="50%"
                                Text="" CssClass="txtUppercase tb8 dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                        </asp:Panel>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pnlDatiAssicurativiVL" Visible="false">
                    <tr>
                        <td class="Row1" colspan="2">
                            <label>
                                Retribuzione settimanale AGO (quota A):</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtRetrAgoQuotaA" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                                MaxLength="19"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="Row1" colspan="2">
                            <label>
                                Retribuzione settimanale AGO (quota B):</label>
                        </td>
                        <td class="Row1">
                            <asp:TextBox ID="txtRetrAgoQuotaB" runat="server" CssClass="tb8 txtUppercase" Width="40%"
                                MaxLength="19"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </asp:Panel>
    </asp:Panel>
</div>
<div style="min-height: 100px;">
</div>
