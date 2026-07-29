<%@ Page Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="PrepensionamentoArt37Legge416198LetteraB.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.PrepensionamentoArt37Legge416198LetteraB" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlWelcome" runat="server" CssClass="utility-list">
        <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />

        <div style="margin-left: 50px; width: 650px;" class="full-width no-margin">
            <div class="page-title" style="padding-left: 20px">
                <h2 class="page-title-secondlevel">E' possibile effettuare le seguenti operazioni:</h2>
            </div>
            <ul class="list-actions">
                 <li id="liGestioneAziendeEditorialiLetteraB" style="margin-bottom: 30px;" runat="server" class="mb-16">
                     <strong>
                        <a href="AltreFunzioni/GestioneAziendeEditorialiLetteraB.aspx" onclick="BlockUI();" class="link-button tertiary ghost ghost--small">Gestione Aziende Editoriali art. 37 legge 416/1981, lettera (b)</a>
                     </strong> 
                     <div>Visualizzazione, inserimento, modifica e cancellazione di Aziende Editoriali art. 37 legge 416/1981, lettera (b)</div>

                 </li>
                <li id="liCambioDataPrepensionamentoLetteraB" style="margin-bottom: 30px;" runat="server" class="mb-16">
                    <strong>
                        <a href="AltreFunzioni/CambioDataPrepensionamentoLetteraB.aspx" onclick="BlockUI();" class="link-button tertiary ghost ghost--small">Cambio data limite domande Aziende Editoriali art. 37 legge 416/1981, lettera (b)</a>
                    </strong> 
                    <div>Cambio data limite Aziende Editoriali art. 37 legge 416/1981, lettera (b)</div>
                </li>                
            </ul>
        </div>
    </asp:Panel>
</asp:Content>