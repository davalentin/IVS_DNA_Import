<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true"
    CodeBehind="VisualizzazioneStatoPratiche.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.VisualizzazioneStatoPratiche" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<%@ Register Src="~/UserControls/VisualizzaStatoPratiche/UCStatoPratiche.ascx" TagName="UCStatoPratiche" TagPrefix="UCSP"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function validatePage() {
            var flag = true;
            if (document.getElementById("<%=pnlStatoPratiche.ClientID%>") != null) {
                flag = Page_ClientValidate('VisualizzaStatoPratiche');
            }
            
            return flag;
        }
        
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  

    <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false"  />
    <asp:Panel runat="server" ID="pnlStatoPratiche" Width="720px" DefaultButton="btnRicerca" CssClass="DocumentsStateViewPage">
        <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="VisualizzaStatoPratiche" Font-Size="Small" CssClass="errorBox" />
        
        <div class="page-title" style="display: none">
            <h2 class="page-title-secondlevel">Ricerca lista</h2>
            <h6 class="page-subtitle">Inserisci uno o più criteri di ricerca per visualizzare la lista di domande associate.</h6>
        </div>

        <table width="100%" class="tableSearch tableSearch__main">
            <tr style="width:720px">
                <td>
                    <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche"  OnAggiungiParametro="event_AggiungiParametro" />
                </td>
            </tr>            
            <tr style="width:720px">
                <td>
                    <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche1" Visible="false" OnAggiungiParametro="event_AggiungiParametro" OnRimuoviParametro="event_RimuoviParametro"  />
                </td>
            </tr>
            <tr style="width:720px">
                <td>
                    <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche2" Visible="false" OnAggiungiParametro="event_AggiungiParametro" OnRimuoviParametro="event_RimuoviParametro" />
                </td>
            </tr>
            <tr style="width:720px">
                <td>
                    <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche3" Visible="false" OnRimuoviParametro="event_RimuoviParametro"/>
                </td>
            </tr>            
        </table>

        <table align="center" width="100%" class="tableSearch tableSearch__actions">
            <tr>
                <td align="center" class="tableSearch__actions-group">
                    <asp:Button ID="btnRicerca" runat="server" Text="Ricerca" SkinID="btnAzione1" OnClick="btnRicerca_Click" CausesValidation="false" 
                        OnClientClick="mainValidate()" style="width: 200px" CssClass="primary tableSearch__actions-group--last"/>
                    <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" Text="Annulla" 
                        onclick="btnAnnulla_Click" style="width: 200px" OnClientClick="BlockUI()"/>
                </td>
            </tr>
        </table>
    </asp:Panel>    
        
        
        
        
    <%--<asp:Label runat="server" ID="IntestazioneRicerca"  Font-Size="Large" >Selezionare uno o più criteri da utilizzare per la ricerca:</asp:Label>
    <div style="height:20px;"></div>
        <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche"  OnAggiungiParametro="event_AggiungiParametro" />
        <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche1" Visible="false" OnAggiungiParametro="event_AggiungiParametro" OnRimuoviParametro="event_RimuoviParametro"  />
        <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche2" Visible="false" OnAggiungiParametro="event_AggiungiParametro" OnRimuoviParametro="event_RimuoviParametro" />
        <UCSP:UCStatoPratiche runat="server" ID="ucStatoPratiche3" Visible="false" OnRimuoviParametro="event_RimuoviParametro"/>
       <asp:HiddenField runat="server" ID="hdnNParametri" />
    </asp:Panel>
            <table width="80%">
                <tr>
                    <td style="text-align: right">
                        <asp:Button ID="btnRicerca" runat="server" Text="Ricerca" SkinID="btnAzione1" OnClick="btnRicerca_Click" ValidationGroup="VisualizzaStatoPratiche"
                         CausesValidation="true" OnClientClick="BlockUI()" />
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" Text="Annulla" 
                            onclick="btnAnnulla_Click" style="width: 63px" />
                    </td>
                </tr>
            </table>--%>
     <asp:HiddenField runat="server" ID="hdnNParametri" />  
     
    <asp:HiddenField runat="server" ID="hdnNCriteri" Value="0" />
    <asp:HiddenField runat="server" ID="hdnCriterio1" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio1" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio1b" Value="null" />
    <asp:HiddenField runat="server" ID="hdnCriterio2" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio2" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio2b" Value="null" />
    <asp:HiddenField runat="server" ID="hdnCriterio3" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio3" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio3b" Value="null" />    
    <asp:HiddenField runat="server" ID="hdnCriterio4" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio4" Value="null" />
    <asp:HiddenField runat="server" ID="hdnValueCriterio4b" Value="null" />
    
    <input type="hidden" id="hfClientID" value="ctl00_ContentPlaceHolder1_ucStatoPratiche_HiddenFieldSedi" />
           
</asp:Content>
