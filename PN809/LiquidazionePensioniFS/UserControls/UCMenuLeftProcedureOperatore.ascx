<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMenuLeftProcedureOperatore.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCMenuLeftProcedureOperatore" %>

<div id="menu">
    <div id="blue">
        <!--[if lte IE 7.0]> 
<script type="text/javascript">
            blue.style.display = "inline";
</script>
<![endif]-->
        <!--[if IE 8]>
<script type="text/javascript"> blue.style.display = "inline-table";</script>
<![endif]-->
        <ul id="listMenu" class="<%# GetlLstMenuClass() %>">
            <li id="liHome" runat="server" style="padding-bottom: 5px; border: 2px"><a href="Default.aspx" onclick="BlockUI();">
                <img style="border: 0px" src="App_Themes/<%= Page.Theme %>/Images/home.png" alt="Home" class="none"/>
                <asp:Label ID="lblHomePage" SkinID="lblVoceMenu" runat="server" Text="Menu Iniziale" />
            </a></li>
            <li id="liElaborazionePosizione" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/ElaborazionePosizione.aspx" onclick="BlockUI();" id="aElaborazionePosizione" runat="server">
                    <asp:Label ID="lblElaborazionePosizione" SkinID="lblVoceMenu" runat="server" Text='<%# Page.Theme == "iFrame" ? "Ricerca domanda" : "Elaborazione Posizione" %>' />
                </a></li>
            <li id="liVisualizzazioneStatoPratiche" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/VisualizzazioneStatoPratiche.aspx?R=0" onclick="BlockUI();" id="aVisualizzazioneStatoPratiche" runat="server">
                    <asp:Label ID="lblVisualizzazioneStatoPratiche" SkinID="lblVoceMenu" runat="server" Text='<%# Page.Theme == "iFrame" ? "Ricerca Lista" : "Visualizzazione Stato Pratiche" %>' />
                </a></li>
            <li id="liTrasmissioneECalcolo" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="~/TrasmissioneECalcolo.aspx" onclick="BlockUI();" id="aTrasmissioneECalcolo" runat="server">
                    <asp:Label ID="TrasmissioneECalcolo" SkinID="lblVoceMenu" runat="server" Text="Trasmissione e Calcolo" />
                </a></li>
            <li id="liUtilitySistema" runat="server" style="padding-bottom: 5px; border: 2px">
            <a href="~/UtilitySistema.aspx" onclick="BlockUI();" id="aUtilitySistema" runat="server">
                <asp:Label ID="UtilitySistema" SkinID="lblVoceMenu" runat="server" Text="Utility di sistema" />
            </a></li>
            <li id="liMonitoraggioProduttivita" runat="server" style="padding-bottom: 5px; border: 2px">
                <a href="" id="aMonitoraggioProduttivita" runat="server">
                    <asp:Label ID="MonitoraggioProduttivita" SkinID="lblVoceMenu" runat="server" Text="Monitoraggio Produttività" />
                </a></li>
            <%--
            <li id="liExit" runat="server" style="padding-bottom: 5px; border: 2px">
            <a href="#" onclick="if(!window.confirm('Sei sicuro di voler chiudere il browser?')) return false; else closeBrowser();">
            <asp:Image ImageAlign="Middle" ID="imgExit" runat="server" ImageUrl="~/App_Themes/<%= Page.Theme %>/Images/exit.png"
                        AlternateText="" />
                <asp:Label ID="Exit" SkinID="lblVoceMenu" runat="server" Text="Uscita" />
            </a></li>
            --%>
            <li id="liDataSistema" runat="server" visible="false" style="padding-top: 10px; padding-bottom: 5px; border: 32px">
                <asp:Label runat="server" ID="lblDataSistema" style="color: Navy; font-weight: bold;" />
            </li>
        </ul>
    </div>
</div>
