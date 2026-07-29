<%@ Page Language="C#"  MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true" 
CodeBehind="RisultatoVisualizzaStatoPratiche.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.RisultatoVisualizzaStatoPratiche" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<%@ Register Src="~/UserControls/VisualizzaStatoPratiche/UCVisualizzaStatoPratiche.ascx" TagName="UCVisualizzaStatoPratiche" TagPrefix="UCVSP" %>

<%@ Register Src="~/UserControls/RisultatoRicerca/UCSinonimi.ascx" TagName="UCSinonimi" TagPrefix="UCS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
  
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false"  />
    <%--<input type="hidden" name="hdnSelected" id="hdnSelected" value="#risultati" />--%>
    <div runat="server" id='divWait'>
    <asp:Panel runat="server" ID="pnlRisultatiRicerca">
    <UCVSP:UCVisualizzaStatoPratiche runat="server" ID="ucVisualizzaStatoPratiche" OnEliminaPraticaEvent="event_ucEliminaPratica" OnReloadUChangeSede="event_ReloadUChangeSede" />
    </asp:Panel>
    </div>
</asp:Content>    


