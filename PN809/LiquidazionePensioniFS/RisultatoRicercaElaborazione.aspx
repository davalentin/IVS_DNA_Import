<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="RisultatoRicercaElaborazione.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.RisultatoRicercaElaborazione" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<%@ Register Src="~/UserControls/RisultatoRicerca/UCRisultatoRicerca.ascx" TagName="UCRisultatoRicerca" TagPrefix="UCRR" %>

<%@ Register Src="~/UserControls/RisultatoRicerca/UCSinonimi.ascx" TagName="UCSinonimi" TagPrefix="UCS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


   
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false"  />
    <%--<input type="hidden" name="hdnSelected" id="hdnSelected" value="#risultati" />--%>
    <asp:Panel runat="server" ID="pnlRisultatiRicerca">
    <UCRR:UCRisultatoRicerca runat="server" ID="ucRisultatoRicerca" visible="false" OnReloadUChangeSede="event_ReloadUChangeSede" />
    <UCS:UCSinonimi runat="server" ID="ucSinonimi"  Visible="false" />
    </asp:Panel>
</asp:Content>    
    