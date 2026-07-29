<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCContribuzioneEnpals.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.CrossSuppLiqAgo.UCContribuzioneEnpals" %>

<div id="divQuotaA" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="true">
    <table class="tabellaFormattazione" style="width: 50%" >
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="lblTitoloQuotaA" runat="server" Text="Quota A" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
      </table>
    <table class="tabellaFormattazione" style="width: 100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Enpals:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAEnpals" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
             
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Inps:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAInps" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                    Width="60%" Enabled="false"></asp:TextBox>

            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Figurativa:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAFigurativa" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false"></asp:TextBox>
               
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Volontaria:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAVolontaria" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false"></asp:TextBox>
            
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    D'Ufficio:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAUfficio" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
          
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Estera:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaAEstera" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
           
            </td>
        </tr>
    </table>
</div>

<div id="divQuotaB" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="true">
    <table class="tabellaFormattazione" style="width: 50%" >
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label1" runat="server" Text="Quota B" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
      </table>
    <table class="tabellaFormattazione" style="width: 100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Enpals:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBEnpals" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
            
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Inps:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBInps" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                    Width="60%" Enabled="false"></asp:TextBox>
           
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Figurativa:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBFigurativa" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false" ></asp:TextBox>
              
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Volontaria:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBVolontaria" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false"></asp:TextBox>
             
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    D'Ufficio:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBUfficio" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
           
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Estera:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaBEstera" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
           
            </td>
        </tr>
    </table>
</div>

<div id="divQuotaC" style="border-style: solid; border-color: #000080; border-collapse: collapse;
    border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" runat="server"
    visible="true">
    <table class="tabellaFormattazione" style="width: 50%" >
        <tr>
            <td class="Row1" style="text-align: left">
                <asp:Label ID="Label2" runat="server" Text="Quota C" Style="font-weight: bold"></asp:Label>
            </td>
        </tr>
      </table>
    <table class="tabellaFormattazione" style="width: 100%">
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Enpals:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCEnpals" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
              
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Inps:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCInps" runat="server" MaxLength="5" CssClass="tb8 txtUppercase"
                    Width="60%" Enabled="false"></asp:TextBox>
            
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    Figurativa:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCFigurativa" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false"></asp:TextBox>
            
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Volontaria:</label>
            </td>
            <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCVolontaria" runat="server" MaxLength="5" CssClass="tb8 txtUppercase" Width="60%" Enabled="false"></asp:TextBox>
            
            </td>
        </tr>
        <tr>
            <td class="Row1" style="width: 25%">
                <label>
                    D'Ufficio:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCUfficio" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
            
            </td>
            <td class="Row1" style="width: 25%">
                <label>
                    Estera:</label>
            </td>
             <td class="field" style="width: 25%">
                <asp:TextBox ID="txtQuotaCEstera" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase" Enabled="false"></asp:TextBox>
            
            </td>
        </tr>
    </table>
</div>
<!-- Pannello bottoni -->
<div style="margin-right: 40px;" class="containerWidth xs">
    <table width="100%" style="min-height: 100px;">
        <tr>
            <td style="text-align: center ; vertical-align: bottom;">
                <asp:Button ID="btnSalva" runat="server" CausesValidation="false" OnClientClick="BlockUI()"
                    SkinID="btnAzione1" Width="150px" OnClick="btnSalva_Click" Text="Salva Contribuzione" />
            </td>
         
        </tr>
    </table>
</div>
<asp:HiddenField ID="HdnTipologia" runat="server" />


