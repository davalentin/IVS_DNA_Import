<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="True"
    CodeBehind="SceltaRuolo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SceltaRuolo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">   
    <asp:Label ID="lblCF" runat="server"></asp:Label>
    <asp:Panel ID="pnlSceltaRuolo" runat="server">
        <UCA:UCAvviso runat="server" ID="UCAvviso" Visible="false" />
        <div align="center" class="boxtable scelta-roulo">
            <br />
            <div class="scelta-roulo__title-box" style="display: none">
                <h1 class="scelta-roulo__title">Liquidazione Pensioni (Nuova IVS)</h1>
                <p class="scelta-roulo__message">Portale operativo per la liquidazione delle domande di invalidità, vecchiaia e superstiti.</p>
            </div>
            <div class="jmbRicerca backTitle scelta-roulo__label-box" style="width: 400px;">
                <h2>
                    <label class="text-medium scelta-roulo__label">Selezionare ruolo</label>
                </h2>
            </div>
            <asp:Panel runat="server" ID="pnlScelta" DefaultButton="btnSceltaRuolo" CssClass="scelta-roulo__field-box">
                <asp:DropDownList runat="server" ID="ddlRuoli" CssClass="tb8" Width="260px">
                </asp:DropDownList>

                <asp:Button runat="server" ID="btnSceltaRuolo" Text="Scegli" SkinID="btnAzione1" OnClick="btnSceltaRuolo_Click"
                    OnClientClick="BlockUI()" CausesValidation="false" CssClass="primary" />
            </asp:Panel>
            <br />
            <br />
            <div class="scelta-roulo__image-box" style="display: none">
                <img class="scelta-roulo__image" src="App_Themes/iFrame/Images/IVS.svg" alt="IVS" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>

