<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizzaReport.aspx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.VisualizzaReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/javascript" src="../Javascript/jquery-1.4.2.min.js"></script>

<script type="text/javascript" src="../Javascript/Utility.js"></script>

<script type="text/javascript" src="../Javascript/jquery.blockUI.js"></script>

<script language="javascript" type="text/javascript">

    // Questo blocco di codice serve a rimuovere la doppia scrollbar dal reportViewer
    window.onload = function() {
        var viewer = document.getElementById("<%=ReportViewer1.ClientID %>");
        var frame = document.getElementById("ReportFrame<%=ReportViewer1.ClientID %>");
        if (frame != null && viewer != null) {
            try {
                var reportDiv = eval("ReportFrame<%=ReportViewer1.ClientID %>").document.getElementById("report").contentDocument.getElementById("oReportDiv");
                reportDiv.removeAttribute("style");
            }
            catch (err) { }
        }
    }

    // Questo blocco di codice serve per ridimensionare il reportViewer quando viene ridimensionata la pagina
    window.onresize = function() {
        var viewer = document.getElementById("<%= ReportViewer1.ClientID %>");
        var htmlheight = document.documentElement.clientHeight;
        viewer.style.height = (htmlheight - 100) + "px";
    }

    // Questo blocco di codice esegue il polling sul ReportViewer finchè non si è completamento caricato 
    // in modo da mostrare il pulsante di esportazione solo con il report caricato
    function polling() {
        if (document.getElementById("ReportViewer1") != null) {
            if (document.getElementById("ReportViewer1").ClientController != null) {
                if (document.getElementById("ReportViewer1").ClientController.TotalPages != null) {
                    document.getElementById("btnExportReport").style.display = "block";
                    return;
                }
            }
        }
        setTimeout(function() {
            polling();
        }, 500);
    }

    $(document).ready(function() {
        polling();
    });
    
    
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div1" align="center" style="border-style: solid; border-color: #000080;
        border-collapse: collapse; border-width: 0px; width: 100%; height: 690px; margin-left: 4px;
        margin-top: 4px; overflow: visible;" runat="server">
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Size="6pt" Width="100%"
            Height="90%" SizeToReportContent="true" DocumentMapWidth="100%" DocumentMapCollapsed="true"
            PromptAreaCollapsed="true" ShowFindControls="false" ShowRefreshButton="false"
            ShowZoomControl="false" AsyncRendering="true" ShowParameterPrompts="false" ShowExportControls="false"
            ProcessingMode="Local" ShowPrintButton="false" ShowPageNavigationControls="true">
        </rsweb:ReportViewer>
        <br />
        <table>
            <tr style="min-height: 50px;">
                <td>
                    <asp:Button ID="btnExportReport" runat="server" Text="Esporta Report" SkinID="btnAzione1"
                        CausesValidation="false" OnClick="btnExportReport_Click" Style="display: none;" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
