<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCAggiornamento.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.Aggiornamento.UCAggiornamento" %>
<asp:Panel ID="pnlElabora" runat="server" CssClass="aggiornamento-card">
    
    <div class="data-container">
        <h4><asp:Literal runat="server" ID="lblTitolo" /></h4>
        <label>Domande da aggiornare: </label> <asp:Label runat="server" ID="lblDomandeTotali" CssClass="font-bold"></asp:Label>
    </div>

    <div class="btn-container">
        <asp:Button runat="server" ID="btnPDF" OnClick="btnGeneraPDF_Click" SkinID="btnAzione1"
                    Text="Riepilogo Elaborazione" OnClientClick="BlockUI();" Width="180px" CssClass="tertiary" />
        <asp:Button runat="server" ID="btnElabora" OnClick="btnElabora_Click" SkinID="btnAzione1"
                    Text="Elabora" OnClientClick="BlockUI();" Width="180px" CssClass="primary mr-0" />
    </div>
</asp:Panel>
