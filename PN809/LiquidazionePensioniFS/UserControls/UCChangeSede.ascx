<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCChangeSede.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCChangeSede" %>

<script type="text/javascript">
        $(function() {
            // jQuery UI Dialog    
            var result;
            $('#cambiaSede').dialog({
                autoOpen: false,
                width: 400,
                modal: true,
                resizable: false,
                draggable: true,
                open: function(event, ui){$('body').css('overflow','auto');$('.ui-widget-overlay').css('width','100%'); },
                close: function(event, ui){$('body').css('overflow','auto'); },
                buttons: {
                    "Annulla": function () {
                        $(this).dialog("close");
                        result = false;
                    },
                    "Conferma": function() {
                        <%=this.Page.ClientScript.GetPostBackEventReference(new PostBackOptions(this.btnChangeSede))%>;
                        $(this).dialog("close");
                    }
                }
            });
            $("#cambiaSede").parent().appendTo($("form:first"));
        });
</script>

<div id="cambiaSede" title="Cambia sede" style="display: none;">
    <p>
        Sei sicuro?</p>
</div>
<asp:Panel runat="server" ID="pnlChgSede">
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="vertical-align: middle" class="TblRecordset">
                Sede:
                <asp:Label ID="lblSede" Font-Bold="true" runat="server"/><span style="visibility: hidden">&nbsp;</span><asp:Label ID="lblCentroOperativo"
                    Font-Italic="true" Font-Size="Smaller" runat="server"></asp:Label>
                <span style="visibility: hidden">&nbsp;&nbsp;</span>
            </td>
            <td style="vertical-align: middle" class="TblRecordset">
                <asp:ImageButton runat="server" ID="btnChangeSede" AlternateText="Cambia Sede" Style="border: 0px;"
                    OnClientClick=" $('#cambiaSede').dialog('open');return false;" OnClick="btnChangeSede_Click" Height="20px"/>
            </td>
        </tr>
    </table>
</asp:Panel>
