<%@ Page Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="true" CodeBehind="SistemaUnico.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SistemaUnico" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <UCA:UCAvviso Visible="false" ID="ucAvviso" runat="server" />
    <asp:Panel ID="pnlWelcome" runat="server">
        <table class="containerWidth xs">
            <tr>
                <td align="center" class="containerWidth xs">
                    <label style="color: #336699; font-weight: bold; font-size: larger; width: 720px">
                        Gestione ricerca da Sistema Unico</label>
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel1" runat="server" Style="border-style: solid; border-color: #000080; min-height: 200px; border-collapse: collapse; border-width: 1px; margin-left: 0px; background-position: right top; background-repeat: no-repeat;"
                        class="containerWidth xs">
                        <br />
                        <!-- Pannello per la ricerca del numero domanda -->
                        <asp:Panel ID="pnlRicercaDomanda" runat="server">
                            <table class="tabellaFormattazione">
                                <tr align="center">
                                    <td class="Row1" align="center" style="width: 100%;" colspan="2">
                                        <asp:Label ID="lblMsg" runat="server" ForeColor="Black" Font-Bold="true" Text="Di seguito sono riportate le informazioni richieste attraverso l'applicazione Sistema Unico"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <hr style="border-style: solid; border-color: #000080; border-width: 1px; margin-left: 5px; margin-right: 5px;" />
                            <table class="tabellaFormattazione">
                                <tr align="center">
                                    <td class="Row1" align="right" style="width: 50%;">Sede:
                                    </td>
                                    <td class="field" align="left" style="width: 50%;">
                                        <asp:Label ID="lblSede" runat="server" />
                                    </td>
                                </tr>
                                <tr align="center">
                                    <td class="Row1" align="right" style="width: 50%;">Numero Domanda:
                                    </td>
                                    <td class="field" align="left" style="width: 50%;">
                                        <asp:Label ID="lblDomus" runat="server" />
                                    </td>
                                </tr>
                            </table>
                            <hr style="border-style: solid; border-color: #000080; border-width: 1px; margin-left: 5px; margin-right: 5px;" />
                            <table class="tabellaFormattazione">
                                <tr align="center">
                                    <td class="Row1" align="center" style="width: 100%;" colspan="2">
                                        <asp:Label ID="lblMsgCortesia" runat="server" ForeColor="Red" Font-Bold="true" Text="Confermando i dati, si procederà con la ricerca del numero domanda visualizzato sopra"></asp:Label>
                                        <center>
                                            <asp:Image ID="Loading" runat="server" CssClass="loading" ImageUrl="~/App_Themes/BlueINPS1/Images/ajax-loader.gif" Visible="false"/>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <!-- Fine Pannello per la ricerca del numero domanda -->
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <table class="tabellaFormattazione containerWidth xs">
        <tr align="center">
            <td class="Row1" colspan="2" style="width: 100%;">
                <asp:Button runat="server" ID="btnRicercaNDomus" Text="Ricerca" SkinID="btnAzione1" OnClick="btnRicercaNDomus_Click" CausesValidation="false" />
            </td>
        </tr>
    </table>
</asp:Content>

