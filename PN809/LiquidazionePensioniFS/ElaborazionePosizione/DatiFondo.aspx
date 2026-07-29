<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DatiFondo.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiFondo" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DatiFondo/UCRegistrazioniFondo.ascx" TagName="UCRegistrazioni"
    TagPrefix="UCREG" %>
<%@ Register Src="~/UserControls/DatiFondo/UCDatiCalcoloDZ_New.ascx" TagName="UCDatiCalcoloDZ_new"
    TagPrefix="UCDCDZ_new" %>
<%@ Register Src="~/UserControls/DatiFondo/UCDatiFondo.ascx" TagName="UCDatiFondo"
    TagPrefix="UCDF" %>
<%@ Register Src="~/UserControls/DatiFondo/UCDatiCalcolo.ascx" TagName="UCDatiCalcolo"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/CrossDatiFondoContr/UCDatiCalcolo707.ascx" TagName="UCDatiCalcolo707"
    TagPrefix="UCDC707" %>
<%@ Register Src="~/UserControls/DatiFondo/UCLegge460.ascx" TagName="UCLegge460"
    TagPrefix="UCL460" %>
<%@ Register Src="~/UserControls/DatiFondo/UCArticolo2.ascx" TagName="UCArticolo2"
    TagPrefix="UCART2" %>
<%@ Register Src="~/UserControls/DatiFondo/UCPrivilegiate.ascx" TagName="UCPrivilegiate"
    TagPrefix="UCPRI" %>
<%@ Register Src="~/UserControls/DatiFondo/UCMiglioramentiContrattualiFS.ascx" TagName="UCMiglioramentiContrattualiFS"
    TagPrefix="UCMGCNT" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript" src="../Javascript/validate2.js"></script>
    <script type="text/javascript" src="../Javascript/Utility.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
 
                var activeTab = LoadClickTab(this);

                valorizzaDecorrenzaRegistrazione();

                return false;
            });

            if(!setLocation())
                valorizzaDecorrenzaRegistrazione();
        });

        function validatePage() {
            var flag = true;

            if (document.getElementById("<%=pnlTabDatiCalcoloDZ_new.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcoloDZ_new');
            }
            if (flag && document.getElementById("<%=pnlTabDatiFondo.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiFondo');
            }
            if (flag && document.getElementById("<%=pnlTabDatiCalcolo.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcolo');
            }
            if (flag && document.getElementById("<%=pnlTabDatiCalcolo707.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabDatiCalcolo707');
            }
            if (flag && document.getElementById("<%=pnlTabLegge460.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabLegge460');
            }
            if (flag && document.getElementById("<%=pnlTabPrivilegiate.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabPrivilegiate');
            }
            if (flag && document.getElementById("<%=pnlTabArticolo2.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabArticolo2');
            }
            if (flag && document.getElementById("<%=pnlTabMiglioramentiContrattualiFS.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabMiglioramentiContrattualiFS');
            }
            return flag;
        }

        function valorizzaDecorrenzaRegistrazione() {
            var decorrenza = getDecorrenzaRegistrazione();
            $("span[id*='lblDecorrenzaRegistrazione']").each(function () {
                $(this).text(decorrenza);
            });
        }

        function setLocation() {
            var retval = false;
            var elementDZ = document.getElementById("<%=btnHidFondoDZ.ClientID%>");
            if (elementDZ)
            {
                if (elementDZ.value != null) {
                    var element = document.getElementById('idDatiCalcoloDZ');
                    if (element) {
                        element.click();
                        retval = true;
                    }
                }
            }

            return retval;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#dati_Registrazioni"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloDZ_new" ValidationGroup="UCTabDatiCalcoloDZ_new"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiFondo" ValidationGroup="UCTabDatiFondo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcolo" ValidationGroup="UCTabDatiCalcolo"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="UCTabDatiCalcolo707"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabLegge460" ValidationGroup="UCTabLegge460"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabPrivilegiate" ValidationGroup="UCTabPrivilegiate"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabArticolo2" ValidationGroup="UCTabArticolo2"
        Font-Size="Small" CssClass="errorBox" />
     <asp:ValidationSummary runat="server" ID="tabMiglioramentiContrattualiFS" ValidationGroup="UCTabMiglioramentiContrattualiFS"
        Font-Size="Small" CssClass="errorBox" />

    <asp:Panel runat="server" ID="pnlDatiFondo">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabRegistrazioniFondo">
                    <li><a href="#dati_Registrazioni">
                        <asp:Label ID="lblTitleRegistrazioniFondo" runat="server" Text="Registrazioni Fondo" />
                        <asp:Image ID="imgRegistrazioniFondo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcoloDZ_new">
                    <li><a href="#dati_CalcoloDZ_new"  id="idDatiCalcoloDZ">Dati Calcolo
                        <asp:Image ID="imgDatiCalcoloDZ_new" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiFondo">
                    <li><a href="#dati_Fondo">
                        <asp:Label ID="lblTitleDatiFondo" runat="server" Text="Dati Fondo" />
                        <asp:Image ID="imgDatiFondo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo">
                    <li><a href="#dati_Calcolo">Dati Calcolo
                        <asp:Image ID="imgDatiCalcolo" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcolo707">
                    <li><a href="#dati_Calcolo_707">Dati Calcolo 707
                        <asp:Image ID="imgDatiCalcolo707" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabMiglioramentiContrattualiFS">
                    <li><a href="#dati_Miglioramenti_Contrattuali_FS">Adeguamenti Contrattuali
                        <asp:Image ID="imgMiglioramentiContrattuali" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabLegge460">
                    <li><a href="#dati_Legge460">Legge 4/60
                        <asp:Image ID="imgLegge460" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabPrivilegiate">
                    <li><a href="#dati_Privilegiate">Privilegiate
                        <asp:Image ID="imgPrivilegiate" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabArticolo2">
                    <li><a href="#dati_Articolo2">Art.2 Comma 12 L.335
                        <asp:Image ID="imgArticolo2" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 90px;">
                <div id="dati_Registrazioni" class="tab_content">
                    <UCREG:UCRegistrazioni runat="server" ID="ucRegistrazioniFondo" OnShowAvviso="event_ucShowAvviso"
                        OnShowPulsanteSalva="event_ucShowPulsanteSalva" OnRecordSelezionato="event_ucRecordSelezionato" />
                </div>
                <div id="dati_CalcoloDZ_new" class="tab_content">
                    <UCDCDZ_new:UCDatiCalcoloDZ_new runat="server" ID="ucDatiCalcoloDZ_new" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiCalcolo="event_ucUpdateSemaforoDatiCalcoloDZ_new" />
                </div>
                <div id="dati_Fondo" class="tab_content">
                    <UCDF:UCDatiFondo runat="server" ID="ucDatiFondo" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiFondo="event_ucUpdateSemaforoDatiFondo" />
                </div>
                <div id="dati_Calcolo" class="tab_content">
                    <UCDC:UCDatiCalcolo runat="server" ID="ucDatiCalcolo" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiCalcolo="event_ucUpdateSemaforoDatiCalcolo" />
                </div>
                <div id="dati_Calcolo_707" class="tab_content">
                    <UCDC707:UCDatiCalcolo707 runat="server" ID="ucDatiCalcolo707" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiCalcolo707="event_ucUpdateSemaforoDatiCalcolo707" />
                </div>
                 <div id="dati_Miglioramenti_Contrattuali_FS" class="tab_content">
                    <UCMGCNT:UCMiglioramentiContrattualiFS runat="server" ID="ucMiglioramentiContrattualiFS" OnShowAvviso="event_ucShowAvviso"
                         OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnInitializeData="event_ucTornaARegistrazioniFondo" OnUpdateSemaforoQuoteMiglioramentiContrattuali = "event_ucUpdateSemaforoQuoteMiglioramentiContrattuali"
                         />
                </div>
                <div id="dati_Legge460" class="tab_content">
                    <UCL460:UCLegge460 runat="server" ID="ucLegge460" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiLegge460="event_ucUpdateSemaforoDatiLegge460" />
                </div>
                <div id="dati_Privilegiate" class="tab_content">
                    <UCPRI:UCPrivilegiate runat="server" ID="ucPrivilegiate" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiPrivilegiate="event_ucUpdateSemaforoDatiPrivilegiate" />
                </div>
                <div id="dati_Articolo2" class="tab_content">
                    <UCART2:UCArticolo2 runat="server" ID="ucArticolo2" OnShowAvviso="event_ucShowAvviso"
                        OnHidePulsanteSalva="event_ucHidePulsanteSalva" OnTornaARegistrazioniFondo="event_ucTornaARegistrazioniFondo"
                        OnUpdateSemaforoDatiArticolo2="event_ucUpdateSemaforoDatiArticolo2" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: center">
                        <asp:Button ID="btnSalvaFondo" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="180px" OnClick="SalvaFondo_Click" OnClientClick="mainValidate()" Visible="false" class="tertiary" />
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" OnClientClick="BlockUI()" PostBackUrl="~/ElaborazionePosizione.aspx"
                            Width="180px" Visible="true" />
                        <asp:HiddenField ID="btnHidFondoDZ" runat="server" Value="" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
