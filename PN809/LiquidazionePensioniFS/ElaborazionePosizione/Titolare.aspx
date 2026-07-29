<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="Titolare.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Titolare" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/Titolare/UCAnagrafica.ascx" TagName="UCAnagrafica"
    TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/Titolare/UCAnagraficaRIC.ascx" TagName="UCAnagraficaRIC"
    TagPrefix="UCARIC" %>
<%@ Register Src="~/UserControls/Titolare/UCStatoCivile.ascx" TagName="UCStatoCivile"
    TagPrefix="UCSC" %>
<%@ Register Src="~/UserControls/Titolare/UCResidenzeEstere.ascx" TagName="UCResidenzeEstere"
    TagPrefix="UCRE" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/superfish.css"
        media="screen" />
    <link rel="stylesheet" type="text/css" href="../App_Themes/BlueINPS1/StyleTabs.css"
        media="screen" />
    <script type="text/javascript" src="../Javascript/hoverIntent.js"></script>
    <script type="text/javascript" src="../Javascript/superfish.1.4.1.js"></script>
    <script type="text/javascript" src="../Javascript/supposition.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            LoadSelectedTab(false);
            //On Click Event
            $("ul.tabs li").click(function () {
                var activeTab = LoadClickTab(this);
                var decPensione = GetDecPensione();
                if (activeTab == "#stato_civile")
                    SetDecStatoCivile(decPensione);

                if (activeTab == "#residenze_estere")
                    SetDecResidenzeEstere(decPensione);
                return false;
            });

            var hiddenField = document.getElementById('<%= hiddInfoMessage.ClientID %>');
            if (hiddenField) {
                var value = hiddenField.value;
                if (value != '') {
                    openModal(value);
                    document.getElementById('<%= hiddInfoMessage.ClientID %>').value = '';
                }
            }
        });

        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlTabAnagrafica.ClientID%>") != null) {
                flag = Page_ClientValidate('UCTabAnagrafica');
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabStatoCivile.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabStatoCivile');
                }
            }
            if (flag) {
                if (document.getElementById("<%=pnlTabResidenzeEstere.ClientID%>") != null) {
                    flag = Page_ClientValidate('UCTabResidenzeEstere');
                }
            }

            return flag;
        }

        function SetDecStatoCivile(decPensione) {
            $("table[id*=gvStatoCivile] input[type=text][id*=txtDecorrenzaStatoCivile]").val(decPensione);
        }

        function SetDecResidenzeEstere(decPensione) {
            $("table[id*=gvResidenzeEstere] input[type=text][id*=txtDecorrenzaStatoEstero]").val(decPensione);
        }

        function openModal(message) {
            debugger;
            var modalPageUrl = '<%= GetAbsoluteUri("~/ElaborazionePosizione/InformationPopup.aspx") %>' + '?message=' + encodeURIComponent(message);

            $("#dialog-informationPage").load(modalPageUrl, function () {
                $("#dialog-informationPage").dialog({
                    show: 'blind',
                    hide: 'blind',
                    height: 300,
                    width: 450,
                    modal: true,
                    position: { my: "center", at: "center", of: window },
                     buttons: {
                        "Chiudi": function () {
                            $(this).dialog("close");
                        }
                    }
                });
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#anagrafica" runat="server" />
    <asp:ValidationSummary runat="server" ID="tabAnagraficaVS" ValidationGroup="UCTabAnagrafica"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabStatoCivileVS" ValidationGroup="UCTabStatoCivile"
        Font-Size="Small" CssClass="errorBox" />
    <asp:ValidationSummary runat="server" ID="tabResidenzeEstereVS" ValidationGroup="UCTabResidenzeEstere"
        Font-Size="Small" CssClass="errorBox" />
    <asp:Panel runat="server" ID="pnlTitolare">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="tabTitolare" class="containerWidth xs">
            <ul class="tabsLine2 tabs">
                <asp:Panel runat="server" ID="pnlTabAnagrafica">
                    <li><a href="#anagrafica">Anagrafica
                        <asp:Image ID="imgAnagrafica" runat="server" ImageAlign="Top" />
                    </a></li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabStatoCivile">
                    <li><a href="#stato_civile">Stato Civile
                        <asp:Image ID="imgStatoCivile" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTabResidenzeEstere">
                    <li><a href="#residenze_estere">Residenze Estere
                        <asp:Image ID="imgResidenzeEstere" runat="server" ImageAlign="Top" /></a> </li>
                </asp:Panel>
            </ul>
            <div class="tab_container" style="min-height: 80px;">
                <div id="anagrafica" class="tab_content">
                    <UCA:UCAnagrafica runat="server" ID="ucAnagrafica" OnShowAvvisoAnagrafica="event_ucShowAvvisoAnagrafica" />
                    <UCARIC:UCAnagraficaRIC runat="server" ID="ucAnagraficaRIC" OnShowAvvisoAnagrafica="event_ucShowAvvisoAnagrafica" />
                </div>
                <div id="stato_civile" class="tab_content">
                    <UCSC:UCStatoCivile runat="server" ID="ucStatoCivile" OnSalvaStatoCivile="event_ucSalvaStatoCivile"
                        OnErrorSalvaStatoCivile="event_ucErrorSalvaStatoCivile" OnAnnullaStatoCivile="event_ucAnnullaSalvaStatoCivile"
                        OnGetDecorrenzaPensione="event_ucGetDecorrenzaPensione" OnShowAvvisoStatoCivile="event_ucShowAvvisoStatoCivile" />
                </div>
                <div id="residenze_estere" class="tab_content">
                    <UCRE:UCResidenzeEstere runat="server" ID="ucResidenzeEstere" OnSalvaResidenzeEstere="event_ucSalvaResidenzeEstere"
                        OnErrorSalvaResidenzeEstere="event_ucErrorSalvaResidenzeEstere" OnAnnullaResidenzeEstere="event_ucAnnullaResidenzeEstere"
                        OnGetDecorrenzaPensione="event_ucGetDecorrenzaPensione" OnGetResidenzaEstera="event_ucGetResidenzaEstera"
                        OnShowAvvisoResidenzeEstere="event_ucShowAvvisoResidenzeEstere" OnShowAvvisoDeleteResidenzeEstere="event_ucShowAvvisoDeleteResidenzeEstere" />
                </div>
            </div>
            <table width="100%" class="footer-actions-group">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnAggiornaARCA" runat="server" Text="Aggiorna da ARCA" SkinID="btnAzione1"
                            CausesValidation="false" OnClick="btnAggiornaARCA_Click" Width="170px" OnClientClick="BlockUI()" class="ghost-update" />
                    </td>
                    <td style="text-align: center; width: 1px;" class="footer-actions-group__first">
                        <asp:Button ID="btnSalva" runat="server" Text="Salva" SkinID="btnAzione1" CausesValidation="false"
                            OnClick="btnSalvaTitolare_Click" OnClientClick="mainValidate()" Width="170px" CssClass="tertiary" />
                    </td>
                    <td style="text-align: left;">
                        <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                            SkinID="btnAzione1" CausesValidation="false" Width="210px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                            OnClientClick="BlockUI()" Visible="false" />
                        <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                            CausesValidation="false" PostBackUrl="~/ElaborazionePosizione.aspx" Width="170px"
                            OnClientClick="BlockUI()" Visible ="true" />
                         <asp:HiddenField ID="hiddInfoMessage" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
    </asp:Panel>
    <div id="dialog-informationPage" title="Information" style="border-style: none; border-color: White;"></div>
</asp:Content>
