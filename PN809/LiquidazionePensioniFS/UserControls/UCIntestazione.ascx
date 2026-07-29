<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCIntestazione.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.UCIntestazione" %>
<%@ Register Src="~/UserControls/UCChangeSede.ascx" TagName="UCChangeSede" TagPrefix="UCS" %>
<%@ Register Src="~/UserControls/UCChangeRuolo.ascx" TagName="UCChangeRuolo" TagPrefix="UCR" %>
<div class="container">
    <div class="row">
        <div class="col-12 subheader">
            <div class="header-system-container" style="float: left; text-align: right; width: 41%; padding-right: 5px;">
                <div class="header-system">
                    <img src="../App_Themes/<%= Page.Theme %>/Images/folder.png" alt="sistema" />
                    <span>Sistema Integrato Prestazioni Pensionistiche</span>
                </div>
            </div>

            <div class="header-role" style="float: right; text-align: left; width: 53%; padding-left: 25px;">
                <asp:Panel runat="server" ID="pnlRuolo" Visible="true" Height="25px">
                    <UCR:UCChangeRuolo runat="server" ID="ucChangeRuolo" />
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlIntestazione" Visible="true" Height="25px">
                    <UCS:UCChangeSede runat="server" ID="ucChangeSede" />
                </asp:Panel>
            </div>
            
        </div>
    </div>
</div>
