<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ProcedureOperatore.Master"
    CodeBehind="Courtesy.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.Courtesy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <br />
    <div class="tab_container tab_container--timeout" style="width: 720px; border-top: 1px solid #000080;">
        <div style="margin: 0 auto; margin-top: 5px; float: left;" id="divWait" runat="server">
            <div runat="server" id="divSessionExpired" visible="false">
                <table class="tabellaFormattazione" width="98%">
                    <tr>
                        <td colspan="5" style="height: 20px;" class="shift-full-grid">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%;">
                        </td>
                        <td>
                            <img height="80px" width="80px" alt="" src="App_Themes/iFrame/Images/clock.svg" />
                        </td>
                        <td style="width: 3%;">
                        </td>
                        <td class="Row1">
                            <label style="vertical-align: top; font-size: large">La sessione è scaduta. Torna all'homepage per continuare</label>
                        </td>
                        <td style="width: 10%">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="height: 125px;" class="shift-full-grid">
                        </td>
                    </tr>
                </table>
            </div>
            <div runat="server" id="divRuoloNonAbilitato" visible="false">
                <table class="tabellaFormattazione" width="98%">
                    <tr>
                        <td colspan="5" style="height: 20px;" class="shift-full-grid">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 15%;">
                        </td>
                        <td>
                            <img height="80px" width="80px" alt="" src="App_Themes/BlueINPS1/Images/ko.png" />
                        </td>
                        <td style="width: 3%;">
                        </td>
                        <td class="Row1">
                            <label style="vertical-align: top; font-size: large">
                                <b>Utente non abilitato alla procedura!!!</b><br />
                                Non è possibile accedere alla procedura perchè non si è in possesso di nessun ruolo censito.</label>
                        </td>
                        <td style="width: 10%">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="height: 125px;" class="shift-full-grid">
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
