<%@ Page Title="" Language="C#" MasterPageFile="~/ProcedureOperatore.Master" AutoEventWireup="True"
    CodeBehind="SceltaSede.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.SceltaSede" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(function() {
            var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
            // alert(availableTags);
            $("#<%=txtSceltaSede.ClientID%>").autocomplete({
                minLength: 0,
                source: availableTags,
                open: function () {
                    $(this)
                        .autocomplete("widget")
                        .css({
                            "margin-top": "8px",
                            "width": $(this).outerWidth() + "px"
                        })
                }
            });
        });
    </script>

    <script type="text/javascript">
    	function CheckSede(val, args) {
    		if (args.Value == "") {
    			args.IsValid = false;
    			return ;
    		}
    		if (document.getElementById("<%=HiddenFieldSedi.ClientID%>") != null) {
    			var availableTags = document.getElementById("<%=HiddenFieldSedi.ClientID%>").value.split(';');
    			for (var i = 0; i < availableTags.length; i++) {
    				if (args.Value.toUpperCase() == availableTags[i]) {
    					return ;
    				}
    			}
    		}
    		args.IsValid = false;
    		return ;
    	}
    </script>

    <script type="text/javascript">
        function Validation() {
        	$("#<%= RequiredFieldValidator3.ClientID %>").show();
        	ValidatorEnable($("#<%= RequiredFieldValidator3.ClientID %>")[0]);

        	$("#<%= CustomValidatorSede.ClientID %>").show();
        	ValidatorEnable($("#<%= CustomValidatorSede.ClientID %>")[0]);
     }


     function validatePage() {
         var flag = true;
         if (document.getElementById("<%=pnlSceltaOpe.ClientID%>") != null) {
             flag = Page_ClientValidate('sedi');
         }
         if (flag) {
             if (document.getElementById("<%=pnlSceltaAdmin.ClientID%>") != null) {
                 flag = Page_ClientValidate('sedi');
             }
         }

         return flag;
     }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCF" runat="server"></asp:Label>
    <asp:Panel ID="pnlSceltaSede" runat="server">
        <UCA:UCAvviso runat="server" ID="UCAvviso" Visible="false" />
        <div align="center" class="boxtable">
            <div class="jmbRicerca backTitle" style="width: 400px;">
                <h2>Seleziona sede</h2>
                <div class="page-subtitle" style="display: none">Seleziona la sede per la quale vuoi operare</div>
            </div>
            <asp:Panel runat="server" ID="pnlSceltaOpe" DefaultButton="btnSceltaSede">
                <asp:DropDownList runat="server" ID="ddlSedi" CssClass="tb8" Width="300px">
                </asp:DropDownList>
                <asp:Button runat="server" ID="btnSceltaSede" Text="Scegli" SkinID="btnAzione1" OnClick="btnSceltaSede_Click"
                    OnClientClick="BlockUI()" CausesValidation="false" />
            </asp:Panel>
            <asp:Panel runat="server" ID="pnlSceltaAdmin" DefaultButton="btnSceltaAdmin">
                <table style="width:441px;">
                    <tr>
                        <td class="talign-center">
                            <asp:ValidationSummary runat="server" ID="validSummary" ValidationGroup="sedi" Font-Size="Small" DisplayMode="List" style="text-align:left; margin-left:4px;" Visible="true" />
                            <br />
                            <label>Sede</label>
                            <asp:TextBox Style="text-align: left" runat="server" ID="txtSceltaSede" Width="300px" CssClass="txtUppercase tb8" TabIndex="1"></asp:TextBox>
                            <asp:CustomValidator runat="server" ID="CustomValidatorSede" ControlToValidate="txtSceltaSede"
                                ValidationGroup="sedi" ErrorMessage="La sede selezionata non è valida" ClientValidationFunction="CheckSede"
                                Text="*" CssClass="field-is-required" Display="Dynamic" Enabled="false"></asp:CustomValidator>
                            <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator3" ControlToValidate="txtSceltaSede"
                                ErrorMessage="Inserire una sede" Text="*" CssClass="field-is-required" Display="Dynamic" ValidationGroup="sedi" style="text-align:left;"/>
                            <asp:Button runat="server" ID="btnSceltaAdmin" Text="Scegli" SkinID="btnAzione1"
                                OnClientClick="mainValidate()" OnClick="btnSceltaSede_Click" CausesValidation="false" CssClass="primary"/>
                            <br />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <br />
            <br />
        </div>
    </asp:Panel>
    <asp:HiddenField ID="HiddenFieldSedi" runat="server" />
</asp:Content>
