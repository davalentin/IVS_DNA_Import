<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCChangeRuolo.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCChangeRuolo" %>

<script type="text/javascript">
        $(function() {
            // jQuery UI Dialog    
            var result;
            $('#cambiaRuolo').dialog({
                autoOpen: false,
                width: 400,
                modal: true,
                resizable: false,
                draggable: true,
                open: function(event, ui){$('body').css('overflow','hidden');$('.ui-widget-overlay').css('width','100%'); },
                close: function(event, ui){$('body').css('overflow','auto'); },
                buttons: {
                    "Annulla": function () {
                        $(this).dialog("close");
                        result = false;
                    },
                    "Conferma": function() {
                        <%=this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this.btnChangeRuolo))%>;
                        $(this).dialog("close");
                    }
                }
            });
            $("#cambiaRuolo").parent().appendTo($("form:first"));
        });
</script>

<div id="cambiaRuolo" title="Cambia ruolo" style="display: none;">
    <p>
        Sei sicuro?</p>
</div>
<asp:Panel runat="server" ID="pnlChgRuolo">
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="vertical-align: middle" class="TblRecordset">
                Ruolo:
                <asp:Label ID="lblRuolo" Font-Bold="true" runat="server" />
                <span style="visibility: hidden">&nbsp;&nbsp;</span>
            </td>
            <td style="vertical-align: middle" class="TblRecordset">
                <asp:ImageButton runat="server" ID="btnChangeRuolo" AlternateText="Cambia Ruolo" Style="border: 0px"
                    OnClientClick=" $('#cambiaRuolo').dialog('open');return false;" OnClick="btnChangeRuolo_Click" Height="20px"/>
            </td>
        </tr>
    </table>
</asp:Panel>
