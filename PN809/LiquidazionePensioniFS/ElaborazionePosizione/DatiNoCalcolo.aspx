<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DatiNoCalcolo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiNoCalcolo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DatiNoCalcolo/UCRecordDatiNoCalcolo.ascx" TagName="UCRecordNoCalcolo"
    TagPrefix="UCREC" %>
<%@ Register Src="~/UserControls/DatiNoCalcolo/UCDatiNoCalcolo.ascx" TagName="UCDatiNoCalcolo"
    TagPrefix="UCDNC" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
    <script type="text/javascript" src="Javascript/validate2.js"></script>
    <script type="text/javascript" src="Javascript/Utility.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });



        });




        function validatePage() {
            var flag = true;

            if (document.getElementById("<%=pnlTabDatiNoCalcolo.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiNoCalcolo');
            }
            return flag;
        }

        //        function valorizzaDecorrenzaRegistrazione() {
        //            var decorrenza = getDecorrenzaRegistrazione();
        //            $("span[id*='lblDecorrenzaRegistrazione']").each(function () {
        //                $(this).text(decorrenza);
        //            });
        //        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_Registrazioni"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiNoCalcolo" ValidationGroup="UCTabDatiNoCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlLiquidazionePensione">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabRegistrazioniNoCalcolo">
                    <li><a href="#dati_Registrazioni">Registrazioni No Calcolo
                        <asp:Image ID="imgRegistrazioniNoCalcolo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiNoCalcolo">
                    <li><a href="#dati_NoCalcolo">Dati No Calcolo
                        <asp:Image ID="imgDatiNoCalcolo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
                <div id="dati_Registrazioni" class="tab_content">
                    <UCREC:UCRecordNoCalcolo runat="server" ID="ucRecordNoCalcolo" OnShowAvviso="event_ucShowAvviso"
                        OnShowPulsanteSalva="event_ucShowPulsanteSalva" OnRecordSelezionato="event_ucRecordSelezionato" />
                </div>
                <div id="dati_NoCalcolo" class="tab_content">
                    <UCDNC:UCDatiNoCalcolo runat="server" ID="ucDatiNoCalcolo" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaElencoRegistrazioni="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiNoCalcolo="event_ucUpdateSemaforoDatiNoCalcolo" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: center">
                        <asp:Button ID="btnSalvaDatiNoCalcolo" runat="server" Text="Salva" SkinID="btnAzione1"
                            CausesValidation="false" Width="150px" OnClick="SalvaDatiNoCalcolo_Click" OnClientClick="mainValidate()"
                            Visible="false" />
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="150px" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
