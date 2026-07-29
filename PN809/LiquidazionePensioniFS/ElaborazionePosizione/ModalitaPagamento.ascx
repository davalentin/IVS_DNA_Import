<%@ Page Title="" Language="C#" MasterPageFile="~/ElaborazionePosizione/Liquidazione.Master"
    AutoEventWireup="true" CodeBehind="ModalitaPagamento.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.ModalitaPagamento" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<%@ Register Src="~/UserControls/UCInfo.ascx" TagName="UCInfo" TagPrefix="UCI" %>
<%@ Register Src="~/UserControls/ModalitaPagamento/UCPagamento.ascx" TagName="Pagamento"
    TagPrefix="UCP" %>
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
                return false;
            });
        });

        function WireAutoTab(CurrentElementID, NextElementID, FieldLength) {
            //Get a reference to the two elements in the tab sequence.
            var CurrentElement = $('#' + CurrentElementID);
            var NextElement = $('#' + NextElementID);

            CurrentElement.keyup(function (e) {
                //Retrieve which key was pressed.
                var KeyID = (window.event) ? event.keyCode : e.keyCode;

                //If the user has filled the textbox to the given length and 
                //the user just pressed a number or letter, then move the 
                //cursor to the next element in the tab sequence.    
                if (CurrentElement.val().length >= FieldLength
            && ((KeyID >= 48 && KeyID <= 90) ||
            (KeyID >= 96 && KeyID <= 105)))
                    NextElement.focus();
            });
        }


        function CustomAutoTab(CurrentElementID, FieldLength) {
            //Get a reference to the two elements in the tab sequence.
            var CurrentElement = $('#' + CurrentElementID);

            CurrentElement.keyup(function (e) {
                var KeyID = (window.event) ? event.keyCode : e.keyCode;
                if (CurrentElement.val().length >= FieldLength
            && ((KeyID >= 48 && KeyID <= 90) ||
            (KeyID >= 96 && KeyID <= 105)))
                    CurrentElement.blur();
                CurrentElement.focus();
            });
        }


        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlModalitaPagamento.ClientID%>") != null) {
                flag = Page_ClientValidate('UCPagamento');
            }

            return flag;
        }

        function savePagamento() {            
            if ($("*[id$='rdbEstero']").is(':checked'))
                $('#dialog-changeSedePensione').dialog('open');
            else
                document.getElementById('<%= btnSalvaPagamento.ClientID %>').click();
        }

        $(function () {            
            var sedeDomanda = "";
            if (document.getElementById('<%=HdnCodiceSedePoloEnpals.ClientID %>') != null)
                sedeDomanda = document.getElementById('<%=HdnCodiceSedePoloEnpals.ClientID %>').value;
            $('#dialog-changeSedePensione').text("La pensione verrà liquidata sulla sede " + sedeDomanda + " in quanto si è indicato un ufficio pagatore estero. Confermare?");

            $('#dialog-changeSedePensione').dialog({
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
                        document.getElementById('<%= btnSalvaPagamento.ClientID %>').click();
                        return true;
                    }
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
    <input type="hidden" name="hdnSelected" id="hdnSelected" value="#pagamento" runat="server" />
    <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="UCPagamento"
        Font-Size="Small" CssClass="errorBox" Visible="true" />
    <asp:Panel runat="server" ID="pnlModalitaPagamento">
        <UCI:UCInfo runat="server" ID="ucInfoLiquidazione" />
        <div runat="server" id='divWait' style="height: 500px;">
            <div style="margin: 0 auto; margin-top: 5px; float: left;" class="containerWidth xs">
                <ul class="tabsLine2 tabs">
                    <li><a href="#pagamento">Pagamento
                        <asp:Image ID="imgPagamento" ImageAlign="Top" runat="server" ImageUrl="~/App_Themes/BlueINPS1/Images/rosso_tab.png" /></a></li>
                </ul>
                <div class="tab_container" style="min-height: 200px;">
                    <div id="pagamento" class="tab_content">
                        <UCP:Pagamento runat="server" ID="ucPagamento" OnSalvaPagamentoEvent="event_ucSalvaPagamento"
                            OnEliminaPagamentoEvent="event_ucEliminaPagamento" OnVisualizzaEliminaPagamento="event_ucVisualizzaEliminaPagamento"
                            OnVisualizzaTastoSalva="event_ucVisualizzaTastoSalva" OnNessunaPosizioneTrovata="event_ucNessunaPosizioneTrovata"
                            OnNascondiPannelloAvviso="event_ucNascondiPannelloAvviso" OnParametriNonValidi="event_ucParametriNonValidi"
                            OnServiceErrorAvviso="event_ucServiceErrorAvviso" OnBloccaEliminaPagamento="event_ucBloccaEliminaPagamento"
                            OnManageBtnPopup="event_ucManageBtnPopup" />
                    </div>
                </div>
                <table width="100%">
                    <tr>
                        <td style="text-align: right;">
                            <asp:Button ID="btnPopUpPage" runat="server" SkinID="btnAzione1" CausesValidation="false"
                                Style="display: none" Text="Salva" Width="150px" OnClientClick="savePagamento(); return false;" />
                            <asp:Button ID="btnSalvaPagamento" runat="server" Text="Salva" SkinID="btnAzione1"
                                Width="150px" CausesValidation="false" OnClick="SalvaPagamento_Click" OnClientClick="mainValidate()" />
                        </td>
                        <td style="text-align: center;">
                            <asp:Button ID="btnEliminaPagamento" runat="server" Text="Elimina" SkinID="btnAzione1"
                                CausesValidation="false" Width="150px" Enabled="false" OnClick="EliminaPagamento_Click"
                                OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare la Modalità di Pagamento?')) return false; else {aspnetForm.target ='_self'; BlockUI();}" />
                        </td>
                        <td style="text-align: left;">
                            <asp:Button ID="btnTornaPosizioni" runat="server" Text="Torna alle posizioni trovate "
                                SkinID="btnAzione1" CausesValidation="false" Width="180px" PostBackUrl="~/RisultatoVisualizzaStatoPratiche.aspx"
                                OnClientClick="BlockUI()" Visible="false" />
                            <asp:Button ID="btnTornaARicerca" runat="server" Text="Torna alla ricerca" SkinID="btnAzione1"
                                CausesValidation="false" OnClientClick="aspnetForm.target ='_self'; BlockUI()"
                                PostBackUrl="~/ElaborazionePosizione.aspx" Width="150px" Visible="true" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </asp:Panel>
    <div id="dialog-changeSedePensione" title="Cambia sede pensione" style="display: none;">
        <p>
        </p>
    </div>
    <asp:HiddenField ID="HdnCodiceSedePoloEnpals" runat="server"></asp:HiddenField>
</asp:Content>
