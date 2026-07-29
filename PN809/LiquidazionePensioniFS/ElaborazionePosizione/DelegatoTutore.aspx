<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DelegatoTutore.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DelegatoTutore" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/DelegatoTutore/UCDelegato.ascx" TagName="UCDelegato"
    TagPrefix="UCD" %>
<%@ Register Src="~/UserControls/DelegatoTutore/UCTutore.ascx" TagName="UCTutore"
    TagPrefix="UCT" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="Javascript/supposition.js"></script>
    <script type="text/javascript" src="Javascript/validate2.js"></script>
    <script type="text/javascript">
        var codiceFiscaleTutore = "<%=this.CodiceFiscale %>";
        $(document).ready(function () {
            var validationSummary = document.getElementById("<%=validSummaryDelegatoTutore.ClientID %>");
            validationSummary.style.display = 'none';

            //When page loads...
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                return false;
            });
        });

        function checkCFDelegatoAndTutore() {
            if (!checkCFDelegato())
                return false;
            if (!checkCFTutore())
                return false;
            return true;
        }


        function checkCFDelegato() {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate("UCDelegatoTutore");
            }
            if (Page_IsValid) {
                if (lblCFDelegato && lblCFDelegato.innerText == codiceFiscaleTutore) {
                    var validationSummary = document.getElementById("<%=validSummaryDelegatoTutore.ClientID %>");
                    validationSummary.innerHTML = "<li>Il codice fiscale dell'incaricato alla delega non può coincidere con quello del titolare.</li>";
                    validationSummary.style.display = 'block';
                    return false;
                }
                return true;
            }
            else
                return false;
        }


        function checkCFTutore() {
            if (typeof (Page_ClientValidate) == 'function') {
                Page_ClientValidate("UCDelegatoTutore");
            }
            if (Page_IsValid) {
                if (lblCFTutore && lblCFTutore.innerText == codiceFiscaleTutore) {
                    var validationSummary = document.getElementById("<%=validSummaryDelegatoTutore.ClientID %>");
                    validationSummary.innerHTML = "<li>Il codice fiscale dell'incaricato alla tutela non può coincidere con quello del titolare.</li>";
                    validationSummary.style.display = 'block';
                    return false;
                }
                return true;
            }
            else
                return false;
        }          
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#delegato" runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummaryDelegatoTutore" ValidationGroup="UCDelegatoTutore"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlDelegatoTutore">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabDelegato">
                    <li><a href="#delegato">Deleghe<asp:Image ID="imgDelegato" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabTutore">
                    <li><a href="#tutore">Tutele<asp:Image ID="imgTutore" ImageAlign="Top" runat="server" /></a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 250px;">
                <div id="delegato" class="tab_content">
                    <UCD:UCDelegato runat="server" ID="ucDelegato" OnShowAvviso="event_ucShowAvvisoDelegato"
                        OnErrorDelegato="event_ErrorUcDelegato" OnNotErrorDelegato="event_NotErrorUcDelegato"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaDelegato" OnGestisciDisabilitazioneTabDelegato="event_GestisciDisabilitazioneTabDelegato" />
                </div>
                <div id="tutore" class="tab_content">
                    <UCT:UCTutore runat="server" ID="ucTutore" OnShowAvviso="event_ucShowAvvisoTutore"
                        OnErrorTutore="event_ErrorUcTutore" OnNotErrorTutore="event_NotErrorUcTutore"
                        OnShowAvvisoElimina="event_ucShowAvvisoEliminaTutore" OnGestisciDisabilitazioneTabDelegato="event_GestisciDisabilitazioneTabDelegato" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnSalva" OnClientClick="if(checkCFDelegatoAndTutore()) BlockUI(); else return false;"
                            CausesValidation="true" runat="server" Text="Salva" SkinID="btnAzione1" ValidationGroup="UCDelegatoTutore"
                            OnClick="SalvaDati_Click" Width="150px" CssClass="tertiary" />
                        <%--						<asp:CustomValidator ValidateEmptyText="True" ControlToValidate="txtCodiceFiscaleDelegato" EnableClientScript="true" runat="server" Text="*" CssClass="field-is-required" Display="Dynamic"
							ValidationGroup="UCDelegatoTutore" ID="btnSalva_CV" ClientValidationFunction="checkCFDelegatoAndTutore"
							ErrorMessage="Il codice fiscale del delegato non può essere lo stesso del tutore"/>
                        --%>
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px"
                            OnClientClick="aspnetForm.target ='_self'; BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
