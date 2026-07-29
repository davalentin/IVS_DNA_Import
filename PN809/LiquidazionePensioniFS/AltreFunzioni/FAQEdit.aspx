<%@ Page Title="" Language="C#" MasterPageFile="~/AltreFunzioni/AltreFunzioni.Master" AutoEventWireup="true" 
    CodeBehind="FAQEdit.aspx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.FAQEdit" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style type="text/css" media="screen">
        .header
        {
            text-align: center;
        }
        
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".tooltips").hover(
		        function () { $(this).next().css({ display: "block" }) },
		        function () { $(this).next().css({ display: "none" }); }
	            );
            $(".tooltips").mousemove(function (e) {
                var mousex = e.pageX + 10;
                var mousey = e.pageY + 1;
                $(this).next().context.alt = '';
                $(this).next().css({ top: mousey, left: mousex }).fadeIn(0);
            });

            var src = $("#<%=HiddenFieldVisibleFAQ.ClientID%>").val();
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("src", "../App_Themes/<%= Page.Theme %>/Images/turn_on.png");
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("title", "Avviso visibile. Clicca per modificarne la visibilità.");
            }
            else {
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_off.png");
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("title", "Avviso non visibile. Clicca per modificarne la visibilità.");
            }
        });

        function imgbtnVisibleFAQ_ClientClick() {
            var src = $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("src");
            if (src == "../App_Themes/<%= Page.Theme %>/Images/turn_on.png") {
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_off.png");
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("title", "FAQ non visibile. Clicca per modificarne la visibilità.");
                $("#<%=HiddenFieldVisibleFAQ.ClientID%>").val("../App_Themes/<%= Page.Theme %>/Images/turn_off.png");
            }
            else {
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("src", "../App_Themes/<%=Page.Theme%>/Images/turn_on.png");
                $("#<%=imgbtnVisibleFAQ.ClientID%>").attr("title", "FAQ visibile. Clicca per modificarne la visibilità.");
                $("#<%=HiddenFieldVisibleFAQ.ClientID%>").val("../App_Themes/<%= Page.Theme %>/Images/turn_on.png");
            }
            return false;
        }
    </script>

    <asp:Panel runat="server" ID="PanelAvviso">
        <UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false"/>
    </asp:Panel>
    <asp:Panel ID="pnlTitle" Width="720px" runat="server" CssClass="full-width">
        <table width="720px" class="full-width">
            <tr>
                <td align="center" style="width: 720px" class="full-width">
                    <asp:Label ID="lblIntestazione" runat="server" Text="" Style="color: #336699; font-weight: bold;
                        font-size: larger; width: 720px" CssClass="full-width section-label"></asp:Label>
                    <br />
                </td>
            </tr>
        </table>
    </asp:Panel>   
    <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="ValidationGroupFAQ"
                            Font-Size="Small" Visible="true" />
    
    <div class="container mt-32">
        <div>
            <asp:Label ID="lblVisible" runat="server" Text="Visibilit&agrave:" Font-Bold="true" CssClass="d-block"></asp:Label>
                    <asp:ImageButton ID="imgbtnVisibleFAQ" runat="server" Height="25px" Width="25px"
                        ImageUrl='<%# setImage("turn_on.png") %>' class="tooltips" TabIndex="2"
                        OnClientClick="return imgbtnVisibleFAQ_ClientClick();" ToolTip="FAQ visibile. Clicca per modificarne la visibilità." CssClass="section-alert__img section-alert__img--toggle mb-16"/>
        </div>

        <div>
            <asp:Label ID="lblTipologia" runat="server" Text="Tipologia:" Font-Bold="true" CssClass="d-block"></asp:Label>
                    <asp:DropDownList runat="server" ID="ddlTipologia" Width="90%" CssClass="tb8 txtUppercase"></asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidatorTipologia"
                        ControlToValidate="ddlTipologia" ErrorMessage="Inserire una tipologia" Text="*" CssClass="field-is-required"
                        Display="Dynamic" ValidationGroup="ValidationGroupFAQ" />
        </div>
    </div>

    <asp:Panel ID="pnlDomanda" runat="server">
        <div class="container">
            <table width="100%" class="tblFAQ">
                <tr>
                    <td width="100%" align="left">
                        <asp:Label ID="lblDomanda" runat="server" Text="Domanda:" Font-Bold="true" CssClass="d-block mt-32"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="left">
                        <asp:TextBox CssClass="tb8" ID="txtDomanda" runat="server" Width="95%" TextMode="MultiLine" Rows="5">
                            </asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtDomanda" runat="server" ControlToValidate="txtDomanda" 
                                ErrorMessage="E' possibile inserire massimo 2000 caratteri." SetFocusOnError="true" ValidationExpression="[\s\S]{0,2000}" 
                                ValidationGroup="ValidationGroupFAQ" Text="*" CssClass="field-is-required" Display="Dynamic" />
                        <asp:RequiredFieldValidator ID="RFVtxtDomanda" runat="server" ControlToValidate="txtDomanda" Display="Dynamic"
                            ErrorMessage="E' obbligatorio inserire la domanda" ValidationGroup="ValidationGroupFAQ" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlRisposta" runat="server">
        <div class="container">
            <table width="100%" class="tblFAQ">
                <tr>
                    <td width="100%" align="left">
                        <asp:Label ID="lblRisposta" runat="server" Text="Risposta:" Font-Bold="true" CssClass="d-block mt-32"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="left">
                        <asp:TextBox CssClass="tb8" ID="txtRisposta" runat="server" Width="95%" TextMode="MultiLine" Rows="5">
                            </asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidatortxtRisposta" runat="server" ControlToValidate="txtRisposta" 
                                ErrorMessage="E' possibile inserire massimo 2000 caratteri." SetFocusOnError="true" ValidationExpression="[\s\S]{0,2000}" 
                                ValidationGroup="ValidationGroupFAQ" Text="*" CssClass="field-is-required" Display="Dynamic" />
                        <asp:RequiredFieldValidator ID="RFVtxtRisposta" runat="server" ControlToValidate="txtRisposta" Display="Dynamic"
                            ErrorMessage="E' obbligatorio inserire la risposta" ValidationGroup="ValidationGroupFAQ" Text="*" CssClass="field-is-required"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:HiddenField ID="HiddenFieldVisibleFAQ" runat="server" Value='<%# setImage("turn_on.png") %>' />

    <div class="container mt-32">
        <div class="justify-end">
            <asp:Button ID="btnIndietro" runat="server" Text="Indietro" SkinID="btnAzione1" TabIndex="7"
                                Width="121px" OnClientClick="BlockUI();" OnClick="btnIndietro_Click" />
            <asp:Button ID="btnAggiorna" runat="server" Text="" SkinID="btnAzione1" TabIndex="8"
                                Width="121px" ValidationGroup="ValidationGroupFAQ" 
                                CausesValidation="false"
                                OnClientClick="if(Page_ClientValidate('ValidationGroupFAQ')){aspnetForm.target ='_self'; BlockUI();}" OnClick="btnAggiorna_Click" CssClass="primary mr-0"/>
        </div>
    </div>
    
</asp:Content>