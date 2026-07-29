<%@ Page Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Default" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<%@ Register Src="~/UserControls/UCAvvisiHomePage.ascx" TagName="UCAvvisiHomePage" TagPrefix="UCAHP" %>
<%@ Register Src="~/UserControls/UCMessaggiHermesHomePage.ascx" TagName="UCMessaggiHermesHomePage" TagPrefix="UCMHP" %>
<%@ Register Src="~/UserControls/UCAggiornamentiHomePage.ascx" TagName="UCAggiornamentiHomePage" TagPrefix="UCAggHP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this, true, "<%# Page.Theme %>");
                return false;
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#avvisi" runat="server" />
    <asp:Panel ID="pnlWelcome" runat="server" Width="97%">
        <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
        <div style="margin-left: 50px; width: 99%;" class="full-width no-margin">
            <table align="center" id="homePageTitle" class="is-contents none">
                <tr>
                    <td align="center" style="width: 100%">
                        <div class="jmbRicerca backTitle welcome" style="width: 400px;">
                            <label class="homepageLabel text-medium">
                                Home Page</label>
                        </div>
                    </td>
                </tr>
            </table>

            <div class="page-title" style="display: none">
                <h1 class="page-title-firstlevel">Liquidazione Pensioni (Nuova IVS)</h1>
            </div>                     
            <br />
         <div align="left" class="" style=" margin-bottom: 30px;">
            <asp:Panel runat="server" ID="pnlSceltaTema" DefaultButton="btnScegliTema" CssClass="scelta-roulo__field-box" Visible="false">           
              <asp:HiddenField runat="server" ID="hTema" />
               <asp:Label ID="lblTema" runat="server"></asp:Label>               
                <asp:Button runat="server" ID="btnScegliTema" Text="Scegli" SkinID="btnAzione1" OnClick="btnScegliTema_Click"
                    OnClientClick="BlockUI()" CausesValidation="false" CssClass="primary" />
            </asp:Panel> 
         </div>
            <asp:Panel runat="server" ID="pnlHomePage" Width="90%">
                <ul class="tabsLine2 tabs">
                    <asp:Panel runat="server" ID="pnlTabAvvisi">
                        <li><a href="#avvisi">Avvisi</a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabMessaggiHermes">
                        <li><a href="#messaggiHermes">Messaggi Hermes</a></li>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlTabAggiornamenti" Visible="false">
                        <li><a href="#aggiornamenti">Aggiornamenti</a></li>
                    </asp:Panel>
                </ul>
                <div class="tab_container" style="min-height: 150px;">
                    <div id="avvisi" class="tab_content">
                        <UCAHP:UCAvvisiHomePage ID="ucAvvisiHomePage" runat="server" Visible="true" />
                    </div>
                    <div id="messaggiHermes" class="tab_content">
                        <UCMHP:UCMessaggiHermesHomePage ID="ucMessaggiHermesHomePage" runat="server" Visible="true" />
                    </div>
                    <div id="aggiornamenti" class="tab_content">
                        <UCAggHP:UCAggiornamentiHomePage ID="ucAggiornamentiHomePage" runat="server" Visible="true" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>
 
</asp:Content>
