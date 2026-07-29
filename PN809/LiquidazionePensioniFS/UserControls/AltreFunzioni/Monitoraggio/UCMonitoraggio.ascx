<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMonitoraggio.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.AltreFunzioni.Monitoraggio.UCMonitoraggio" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<script language="javascript" type="text/javascript">
    // Questo blocco di codice serve per visualizzare il pannello con i parametri aggiuntivi in maniera statica
    function ControlPanel() {
        var report = document.getElementById("<%=ddlReport.ClientID %>");
        var panel = document.getElementById("<%=pnlParamentriOption.ClientID %>");
        if (report != null) {
            switch (report.value) {
                case "StatoPratiche":
                case "StatoPraticheByRegion":
                case "StatoPraticheNazionali":
                    panel.style.display = "block";
                    break;
                case "":
                    panel.style.display = "none";
                    break;
            }
        }
    }
    window.onload = function() { ControlPanel(); }

    function SbiancaCheckBox() {
        document.getElementById("<%=chkSedDest.ClientID %>").checked = false;
        document.getElementById("<%=chkDecOrig.ClientID %>").checked = false;
        document.getElementById("<%=chkPerfReq.ClientID %>").checked = false;
        document.getElementById("<%=chkUnicarpe.ClientID %>").checked = false;
        document.getElementById("<%=chkMatricola.ClientID %>").checked = false;
    }
</script>

<div class="form-container background-light-blue">
    <div class="single-line-container">
        <label class="input-label">Seleziona Report:</label>

        <div>
            <asp:DropDownList runat="server" Width="220px" CssClass="tb8 txtUppercase" ID="ddlReport"
                              onchange="SbiancaCheckBox(); ControlPanel()" AutoPostBack="false">
                              <asp:ListItem Text="" Value="" />
                              <asp:ListItem Text="Stato Pratiche Sedi" Value="StatoPratiche" />
                              <asp:ListItem Text="Stato Pratiche Regionali" Value="StatoPraticheByRegion" />
                              <asp:ListItem Text="Stato Pratiche Nazionali" Value="StatoPraticheNazionali" />
           </asp:DropDownList>
           <!--<asp:RequiredFieldValidator ID="RequiredFieldddlReport" runat="server" ErrorMessage="Report: campo obbligatorio"
                                       Text="*" CssClass="field-is-required" ControlToValidate="ddlReport" ValidationGroup="UCMonitoraggio"></asp:RequiredFieldValidator>-->
        </div>
    </div>


    <table class="tabellaFormattazione">
        <tr>
            <td style="width: 720px; text-align:center" class="full-width">
                <asp:Panel ID="panMonitoraggio" runat="server" Style="border-style: solid; border-color: #000080;
                    border-collapse: collapse; border-width: 1px; width: 720px; margin-left: 1px;
                    background-position: right top; background-repeat: no-repeat; background-image: url('../App_Themes/BlueINPS1/Images/reportTorta.jpg');" CssClass="iframe-bg-reportTorta form-container no-bold full-width">

                    <asp:Panel ID="pnlParamentriOption" runat="server" Style="display: none;">
                        <div id="divParamentriOption" style="width: 710px; margin-left: 4px; margin-top: 4px;" runat="server">
                            <table class="tabellaFormattazione grid grid-size-25" width="100%">
                                <tr>
                                    <td class="Row1" style="width: 35%;">
                                        <label>
                                            Sede Destinazione:</label>
                                    </td>
                                    <td class="chkField" style="width: 15%; text-align: left">
                                        <asp:CheckBox ID="chkSedDest" runat="server" />
                                    </td>
                                    <td class="Row1" style="width: 35%;">
                                        <label>
                                            Decorrenza Originaria:</label>
                                    </td>
                                    <td class="chkField" style="width: 15%;">
                                        <asp:CheckBox ID="chkDecOrig" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="Row1" style="width: 35%;">
                                        <label>
                                            Data Perfezionamento Requisiti:</label>
                                    </td>
                                    <td class="chkField" style="width: 15%; text-align: left">
                                        <asp:CheckBox ID="chkPerfReq" runat="server" />
                                    </td>
                                    <td class="Row1" style="width: 35%;">
                                        <label>
                                            Unicarpe:</label>
                                    </td>
                                    <td class="chkField" style="width: 15%;">
                                        <asp:CheckBox ID="chkUnicarpe" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="Row1" style="width: 35%;">
                                        <label>
                                            Matricola:</label>
                                    </td>
                                    <td class="chkField" style="width: 15%; text-align: left">
                                        <asp:CheckBox ID="chkMatricola" runat="server" />
                                    </td>
                                    <td class="Row1" style="width: 35%;">
                                    </td>
                                    <td class="chkField" style="width: 15%;">
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </asp:Panel>
                    <br />
                    <table class="tabellaFormattazione" width="100%">
                        <tr>
                            <td align="center">
                                <asp:Button ID="btnReport" runat="server" Text="Genera Report" SkinID="btnAzione1"
                                    CausesValidation="false" OnClick="btnReport_Click" OnClientClick="if(Page_ClientValidate('UCMonitoraggio')){aspnetForm.target ='_self'; BlockUI()}" CssClass="primary mr-0 force-right"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</div>