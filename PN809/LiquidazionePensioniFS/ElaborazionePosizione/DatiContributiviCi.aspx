<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="DatiContributiviCi.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.DatiContributiviCi" %>

<%@ Register Src="~/UserControls/DatiContributiviCi/UCProrataCi.ascx" TagName="UCProrataCi"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviCi/UCDatiCalcoloCi.ascx" TagName="UCDatiCalcoloCi"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviCi/UCImportiEsteriCi.ascx" TagName="UCImportiEsteriCi"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviCi/UCMaternitaAcnaCi.ascx" TagName="UCMaternitaAcnaCi"
    TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviCi/UCDatiPostDecOriginariaCi.ascx"
    TagName="UCDatiPostDecOriginariaCi" TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/DatiContributiviCi/UCIntegrazioneVirtuale.ascx"
    TagName="UCIntegrazioneVirtuale" TagPrefix="UCDC" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
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
            if (document.getElementById("<%=pnlTabProrata.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabProrata');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabDatiCalcoloCI.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabDatiCalcoloCI');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabImportiEsteriCI.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabImportiEsteriCI');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabMaternitaAcnaCI.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabMaternitaAcnaCI');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabDatiPostDecOriginariaCI.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabDatiPostDecOriginariaCI');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabIntegrazioneVirtuale.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCIntegrazioneVirtuale');
                }
            }

            return flag;
        }

        function AbilitaTab() {
            ctl00_ContentPlaceHolder1_pnlTabMaternitaAcnaCI.style.display = 'block';
        }

        function DisabilitaTab() {
            ctl00_ContentPlaceHolder1_pnlTabMaternitaAcnaCI.style.display = 'none';
        }

        $(function () {
            $('#dialog-confirmPage').dialog({
                autoOpen: false,

                show: 'blind',
                hide: 'blind',
                height: 220,
                width: 450,
                modal: true,
                centerX: true,
                centerY: true,
                dialogClass: 'fixed-dialog',
                resizable: false,
                draggable: true,
                open: function (event, ui) { $('body').css('overflow', 'auto'); $('.ui-widget-overlay').css('width', '100%'); },
                close: function (event, ui) { $('body').css('overflow', 'auto'); },
                buttons: {
                    'Annulla': function () {
                        $(this).dialog('close');
                        return false;
                    },
                    'Ok': function () {
                        $(this).dialog('close');
                        document.getElementById('<%= btnSalva.ClientID %>').click();
                        return true;
                    }
                }
            });
        });
        
    </script>
</asp:Content>
<asp:Content ID="ManiContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso ID="ucAvviso" runat="server" Visible="true" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#TabDatiProrata"
        runat="server" />
    <asp:ValidationSummary runat="server" ID="tabProrataVS" ValidationGroup="UCTabProrata"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabProrataVSGrid" ValidationGroup="UCTabProrataGrid"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloVS" ValidationGroup="UCTabDatiCalcoloCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloContrVS" ValidationGroup="UCTabDatiCalcoloContrCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloRetrVS" ValidationGroup="UCTabDatiCalcoloRetrCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiCalcoloContrEsteriVS" ValidationGroup="UCTabDatiCalcoloContrEsteriCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabImportiEsteriVS" ValidationGroup="UCTabImportiEsteriCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabMaternitaAcnaVS" ValidationGroup="UCTabMaternitaAcnaCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabDatiPostDecOriginariaVSGrid" ValidationGroup="UCTabDatiPostDecOriginariaCI"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabIntegrazioneVirtualeVS" ValidationGroup="UCTabIntegrazioneVirtuale"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlDatiView">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div id="main" style="margin: 0 auto; margin-top: 5px; float: left;" runat="server"
            class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabProrata">
                    <li><a href="#TabDatiProrata">Istituzione Estera
                        <asp:Image ID="imgProrataEstera" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiCalcoloCI">
                    <li><a href="#TabDatiCalcoloCI">Dati Calcolo
                        <asp:Image ID="imgDatiCalcoloCI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabImportiEsteriCI">
                    <li><a href="#TabImportiEsteriCI">Imp. Esteri
                        <asp:Image ID="imgImportiEsteriCI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabDatiPostDecOriginariaCI">
                    <li><a href="#TabDatiPostDecOriginariaCI">Post Dec. Orig.
                        <asp:Image ID="imgDatiPostDecOriginariaCI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabMaternitaAcnaCI">
                    <li><a href="#TabMaternitaAcnaCI">Mat./Acna
                        <asp:Image ID="imgMaternitaAcnaCI" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabIntegrazioneVirtuale">
                    <li><a href="#TabIntegrazioneVirtuale">Integrazione Virtuale
                        <asp:Image ID="imgTabIntegrazioneVirtuale" ImageAlign="Top" runat="server" />
                    </a></li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 80px;">
                <div id="TabDatiProrata" class="tab_content" style="display: block">
                    <UCDC:UCProrataCi ID="ucProrataCi" runat="server" OnShowAvvisoDatiProRata="event_ucShowAvvisoDatiProRata"
                        OnNascondiAvviso="event_ucNascondiAvviso" OnShowAvvisoEliminaDatiProRata="event_ucShowAvvisoEliminaDatiProRata"
                        OnAggiornaAnniTabIntegrazioneVirtuale="event_ucAggiornaAnniTabIntegrazioneVirtuale" OnAggiornaSemaforoTabIntegrazioneVirtuale="event_ucAggiornaSemaforoTabIntegrazioneVirtuale" />
                </div>
                <div id="TabDatiCalcoloCI" class="tab_content" style="display: block">
                    <UCDC:UCDatiCalcoloCi ID="ucDatiCalcoloCi" runat="server" OnShowAvvisoDatiCalcolo="event_ucShowAvvisoDatiCalcoloCi"
                        OnShowErrorDatiCalcolo="event_ucShowErrorDatiCalcoloCi" OnShowAvvisoEliminaDatiCalcolo="event_ucShowAvvisoEliminaDatiCalcoloCi"
                        OnInitializeData="event_ucInitializeData" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnNascondiAvviso="event_ucNascondiAvviso"
                        OnAbilitaPopUpDatiContributivi="event_ucAbilitaPopUpDatiContributivi" OnDisabilitaPopUpDatiContributivi="event_ucDisabilitaPopUpDatiContributivi" />
                </div>
                <div id="TabImportiEsteriCI" class="tab_content" style="display: block">
                    <UCDC:UCImportiEsteriCi ID="ucImportiEsteriCi" runat="server" OnShowAvvisoImportiEsteri="event_ucShowAvvisoImportiEsteriCi"
                        OnShowAvvisoEliminaImportiEsteri="event_ucShowAvvisoEliminaImportiEsteriCi" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnNascondiAvviso="event_ucNascondiAvviso" />
                </div>
                <div id="TabDatiPostDecOriginariaCI" class="tab_content" style="display: block">
                    <UCDC:UCDatiPostDecOriginariaCi ID="ucDatiPostDecOriginariaCi" runat="server" OnShowAvvisoDatiPostDecOriginaria="event_ucShowAvvisoDatiPostDecOriginariaCi"
                        OnShowAvvisoEliminaDatiPostDecOriginaria="event_ucShowAvvisoEliminaDatiPostDecOriginariaCi"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" />
                </div>
                <div id="TabMaternitaAcnaCI" class="tab_content" style="display: block">
                    <UCDC:UCMaternitaAcnaCi ID="ucMaternitaAcnaCi" runat="server" OnShowAvvisoMaternitaAcna="event_ucShowAvvisoMaternitaAcnaCi"
                        OnShowAvvisoEliminaMaternitaAcna="event_ucShowAvvisoEliminaMaternitaAcnaCi" OnAbilitaTastoSalva="event_ucAbilitaTastoSalva"
                        OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva" OnNascondiAvviso="event_ucNascondiAvviso" />
                </div>
                <div id="TabIntegrazioneVirtuale" class="tab_content" style="display: block">
                    <UCDC:UCIntegrazioneVirtuale ID="ucIntegrazioneVirtuale" runat="server" OnShowIntegrazioneVirtuale="event_ucShowAvvisoIntegrazioneVirtuale"
                        OnShowAvvisoEliminaIntegrazioneVirtuale="event_ucShowAvvisoEliminaIntegrazioneVirtuale"
                        OnAbilitaTastoSalva="event_ucAbilitaTastoSalva" OnDisabilitaTastoSalva="event_ucDisabilitaTastoSalva"
                        OnNascondiAvviso="event_ucNascondiAvviso" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                            Style="display: none" Text="Salva" Width="160px" OnClientClick="if(mainValidateForConfirm()){$('#dialog-confirm').dialog('open');}return false;" CssClass="tertiary" />
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            Width="160px" OnClick="SalvaDatiContributivi_Click" OnClientClick="mainValidate()" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="160px"
                            OnClientClick="BlockUI()" Visible="true" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div id="dialog-confirmPage" title="Confirm" style="border-style: none; border-color: White;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            Attenzione il Montante è inferiore all’Ammontare.<br />
            Confermare ?</p>
    </div>
</asp:Content>
