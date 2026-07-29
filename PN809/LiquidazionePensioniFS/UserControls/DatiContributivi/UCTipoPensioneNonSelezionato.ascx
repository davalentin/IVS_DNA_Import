<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCTipoPensioneNonSelezionato.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCTipoPensioneNonSelezionato" %>
<asp:Panel runat="server" ID="pnlTipoPensioneNonSelezionato">
<div runat="server" id="divMessaggioTipoPensioneNonSelezionato">
<br />
<hr />
<table class="tabellaFormattazione" style=" padding-top:30px">
<tr>
<td>
<asp:Image runat="server" ID="imgInfoTipoPensioneNonSelezionata" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/info.png" ImageAlign="Middle" />
</td>
<td>
<asp:Label runat="server" ID="lblMessaggioAvvisoTipoPensioneNonSelezionato" Font-Bold="true" Font-Size=Larger>
Selezionare il Tipo Calcolo nel pannello Liquidazione Pensione - Dati Generici prima di procedere all'acquisizione dei dati calcolo
</asp:Label>

</td>
</tr>

</table>

<hr />

</div>
</asp:Panel>