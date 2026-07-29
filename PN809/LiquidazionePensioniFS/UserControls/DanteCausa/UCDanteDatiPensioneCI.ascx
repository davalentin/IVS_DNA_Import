<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDanteDatiPensioneCI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa.UCDanteDatiPensioneCI" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />

<script type="text/javascript">

    function setDataLabel() 
    {
        if (document.getElementById("<%=txtDatiPensioneDCal.ClientID%>").value != "" && document.getElementById("<%=txtDatiPensioneDCal.ClientID%>").value.toUpperCase() != "MM/AAAA")
            document.getElementById("<%=lblDatiDC.ClientID%>").innerText = document.getElementById("<%=txtDatiPensioneDCal.ClientID%>").value;
        else
            document.getElementById("<%=lblDatiDC.ClientID%>").innerText = "";
    }

    function renderTxtImporti() {
        if (document.getElementById("<%=ddlArt1L5991.ClientID%>").value != "True") {

            document.getElementById("<%=txtTotale90.ClientID%>").disabled = 1;
            document.getElementById("<%=txtTotale90.ClientID%>").value = "";
            document.getElementById("<%=txtTotale90.ClientID%>").className = "tboxdisable";

            document.getElementById("<%=txtTotale9294.ClientID%>").disabled = 1;
            document.getElementById("<%=txtTotale9294.ClientID%>").value = "";
            document.getElementById("<%=txtTotale9294.ClientID%>").className = "tboxdisable";

            document.getElementById("<%=txtMensile.ClientID%>").disabled = 1;
            document.getElementById("<%=txtMensile.ClientID%>").value = "";
            document.getElementById("<%=txtMensile.ClientID%>").className = "tboxdisable";
        }
        else
         {
            document.getElementById("<%=txtTotale90.ClientID%>").disabled = 0;
            document.getElementById("<%=txtTotale90.ClientID%>").value = document.getElementById("<%=htxtTotale90.ClientID%>").value;
            document.getElementById("<%=txtTotale90.ClientID%>").className = "tb8 txtUppercase";

            document.getElementById("<%=txtTotale9294.ClientID%>").disabled = 0;
            document.getElementById("<%=txtTotale9294.ClientID%>").value = document.getElementById("<%=htxtTotale9294.ClientID%>").value;
            document.getElementById("<%=txtTotale9294.ClientID%>").className = "tb8 txtUppercase";

            document.getElementById("<%=txtMensile.ClientID%>").disabled = 0;
            document.getElementById("<%=txtMensile.ClientID%>").value = document.getElementById("<%=htxtMensile.ClientID%>").value;
            document.getElementById("<%=txtMensile.ClientID%>").className = "tb8 txtUppercase";
        }
    }

    function setHiddenValueLabel() 
    {
        document.getElementById("<%=htxtMensile.ClientID%>").value    = document.getElementById("<%=txtMensile.ClientID%>").value;
        document.getElementById("<%=htxtTotale9294.ClientID%>").value = document.getElementById("<%=txtTotale9294.ClientID%>").value;
        document.getElementById("<%=htxtTotale90.ClientID%>").value   = document.getElementById("<%=txtTotale90.ClientID%>").value;
    }

    window.onload = function () {
        var ddlArticolo = document.getElementById('<%= ddlArticolo.ClientID %>');
        var txtImporto = document.getElementById('<%= txtImporto.ClientID %>');

        ddlArticolo.addEventListener('change', function () {
            if (ddlArticolo.value === "") {
                txtImporto.value = ""; 
            }
        });
    };
</script>

<asp:Panel runat="server" ID="pnlPensioneCI"><br/>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
           <td class="Row1" style="width:35%;font-size:small;font-weight:bold">
                <label>Dati Pensione del Dante Causa al </label>
           </td>
           <td class="field">
             <asp:TextBox Style="text-align: left" runat="server" onblur="setDataLabel();" ID="txtDatiPensioneDCal" Width="20%"
                CssClass="txtUppercase tb8 date-picker-maxActual dateMMaaaa" TabIndex="1" Text="mm/aaaa" MaxLength="7">
             </asp:TextBox>             
             <asp:RegularExpressionValidator runat="server" ID="validateDatiPensioneDCal" ControlToValidate="txtDatiPensioneDCal"
                ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Formato data non corretto"
                Display="Dynamic" ValidationGroup="UCPensioniCI" />
            <asp:CustomValidator runat="server" ControlToValidate="txtDatiPensioneDCal" Display="Dynamic"
                ErrorMessage="Dati Pensione del Dante Causa al: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCPensioniCI"
                ID="customCheckDataDatiPensioneDCal" ClientValidationFunction="checkCorrettezzaData" />  
           </td> 
        </tr>
    </table>
    <table class="tabellaFormattazione grid grid-size-20">
        <tr>
            <td style="height:5px" colspan="5" class="shift-full-grid"></td>
        </tr>
        <tr>             
           <td style="width:20%" class="Row1">
                <label>Tipo Perequazione</label></td>
          <td colspan="4" class="field" >
            <asp:DropDownList runat="server" TabIndex="2"  onfocus="setDataLabel()"  ID="ddlTipoPerequazione" CssClass="tb8 txtUppercase" Width="15%"/>
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Pensione Virtuale Integrata</label>
            </td>
           <td style="width:28%">
            <asp:TextBox runat="server" TabIndex="3" ID="txtPensioneVirtualeIntegrata" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtPensioneVirtualeIntegrata" ControlToValidate="txtPensioneVirtualeIntegrata"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Pensione Virtuale Integrata"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,4}" ValidationGroup="UCPensioniCI" />
           </td>  
           <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Pensione Virtuale Pura</label></td>
          <td class="field" style="width:26%">
            <asp:TextBox runat="server" TabIndex="4" ID="txtPensioneVirtualePura" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtPensioneVirtualePura" ControlToValidate="txtPensioneVirtualePura"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Pensione Virtuale Pura"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,4}" ValidationGroup="UCPensioniCI" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Pensione Adeguata</label></td>
          <td style="width:28%">
            <asp:TextBox runat="server" TabIndex="5" ID="txtPensioneAdeguata" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtPensioneAdeguata" ControlToValidate="txtPensioneAdeguata"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Pensione Adeguata"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
            </td>  
          <td style="width:4%"></td>
           <td style="width:22%" class="Row1">
                <span style="visibility: hidden">&nbsp;</span></td>
          <td style="width:26%"></td>              
        </tr>
        <tr>
            <td style="height:10px" colspan="5" class="shift-full-grid"></td>
        </tr>
        <tr>
        <td style="font-size:small;font-weight:bold" colspan="5" class="relevant-section shift-full-grid">
            <label class="section-label inline">Dati Pensione del Dante Causa&nbsp;</label>
            <label id="lblDatiDC" runat="server" class="section-label inline"><%=(txtDatiPensioneDCal.Text == "mm/aaaa" || txtDatiPensioneDCal.Text ==  "MM/AAAA" ? String.Empty : txtDatiPensioneDCal.Text)%></label>
            <label class="section-label inline">&nbsp;Decorrenza &quot;SO&quot;</label>
        </td>
        </tr>
        <tr>
        <td style="height:5px" colspan="5" class="shift-full-grid"></td>
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Totale Quote Fisse</label>
            </td>
          <td style="width:28%">
                <asp:TextBox runat="server" TabIndex="6" ID="txtTotaleQuoteFisse" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="validateTxtTotaleQuoteFisse" ControlToValidate="txtTotaleQuoteFisse"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Totale Quote Fisse"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,4}" ValidationGroup="UCPensioniCI" />
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Totale Supplementi</label>
            </td>
          <td style="width:26%">
            <asp:TextBox runat="server" TabIndex="7" ID="txtTotaleSupplementi" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtTotaleSupplementi" ControlToValidate="txtTotaleSupplementi"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Totale Supplementi"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
            </td>              
        </tr>
        <tr>
        <td style="height:10px" colspan="5" class="shift-full-grid"></td>
        </tr>
        <tr>
        <td style="font-size:small;font-weight:bold" colspan="5" class="relevant-section shift-full-grid">
            <label class="section-label mt-32">Articolo 3/4/5 Legge140/8 e DPCM 16/12/89</label>
        </td>
        </tr>
        <tr>
        <td style="height:5px" colspan="5" class="shift-full-grid"></td>
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Articolo</label>
            </td>
          <td style="width:28%" colspan="3" class="full-grid">
                <asp:DropDownList runat="server" TabIndex="8" ID="ddlArticolo" CssClass="tb8 txtUppercase" Width="60%"/>
          </td>  
          <td style="width:26%" class="none"></td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Totale</label>
            </td>
          <td style="width:28%">
               <asp:TextBox runat="server" TabIndex="9" ID="txtTotale" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
               <asp:RegularExpressionValidator runat="server" ID="validateTxtTotale" ControlToValidate="txtTotale"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Totale"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,4}" ValidationGroup="UCPensioniCI" />
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Importo</label>
            </td>
          <td style="width:26%">
            <asp:TextBox runat="server" TabIndex="10" ID="txtImporto" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtImporto" ControlToValidate="txtImporto"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Importo"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Art. 6/140</label>
            </td>
          <td style="width:28%">
            <label runat="server" id="lblArt6"/>
          </td>  
          <td style="width:4%"></td>
           <td style="width:22%" class="Row1">
                <label></label>
            </td>
          <td style="width:26%" class="none"></td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Decorrenza Art. 6</label>
                </td>
          <td style="width:28%">
                <label runat="server" id="lblDecorrenzaArt6"/>
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Importo Art. 6</label>
                </td>
          <td style="width:26%">
            <asp:TextBox runat="server" ID="txtImportoDecorr" TabIndex="11" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtImportoDecorr" ControlToValidate="txtImportoDecorr"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Importo Art. 6"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Art. 1 L.59/91</label>
           </td>
          <td style="width:28%">
                <asp:DropDownList runat="server" ID="ddlArt1L5991" onchange="renderTxtImporti();"  TabIndex="12" CssClass="tb8 txtUppercase" Width="25%"/>
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Totale '90</label>
            </td>
          <td style="width:26%">
            <asp:TextBox runat="server" ID="txtTotale90" TabIndex="13" onblur="setHiddenValueLabel();" Width="80%" CssClass="tb8 txtUppercase"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtTotale90" ControlToValidate="txtTotale90"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Totale '90"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
            <input type="hidden" name="htxtTotale90" id="htxtTotale90" value="" runat="server" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Totale '92-'94</label>
            </td>
          <td style="width:28%">
            <asp:TextBox runat="server" ID="txtTotale9294" TabIndex="14" onblur="setHiddenValueLabel();" Width="80%" CssClass="tb8 txtUppercase"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtTotale9294" ControlToValidate="txtTotale9294"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Totale '92-'94"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
            <input type="hidden" name="htxtTotale9294" id="htxtTotale9294" value="" runat="server" />
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Mensile</label>
            </td>
          <td style="width:26%">
             <asp:TextBox runat="server" ID="txtMensile" TabIndex="15" onblur="setHiddenValueLabel();" Width="80%" CssClass="tb8 txtUppercase"></asp:TextBox>
             <asp:RegularExpressionValidator runat="server" ID="validateTxtMensile" ControlToValidate="txtMensile"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Mensile"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
             <input type="hidden" name="htxtMensile" id="htxtMensile" value="" runat="server" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Aumento Sentenza 72/90</label>
            </td>
          <td style="width:28%">
            <asp:TextBox runat="server" ID="txtAumentoSentenza7290" TabIndex="16" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtAumentoSentenza7290" ControlToValidate="txtAumentoSentenza7290"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Aumento Sentenza 72/90"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Aumento Sentenza 72/90 su Art.2 DPCM</label>
            </td>
          <td style="width:26%">
            <asp:TextBox runat="server" ID="txtAumentoSentenza7290Art2" TabIndex="17" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtAumentoSentenza7290Art2" ControlToValidate="txtAumentoSentenza7290Art2"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Aumento Sentenza 72/90 su Art.2 DPCM"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>              
        </tr>
        <tr>  
           <td style="width:20%" class="Row1">
                <label>Aumento Totale Art.2 DPCM</label>
            </td>
          <td style="width:28%">
            <asp:TextBox runat="server" ID="txtAumentoTotaleArt2DPCM" TabIndex="18" CssClass="tb8 txtUppercase" Width="80%"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="validateTxtAumentoTotaleArt2DPCM" ControlToValidate="txtAumentoTotaleArt2DPCM"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Aumento Totale Art.2 DPCM"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>  
          <td style="width:4%" class="none"></td>
           <td style="width:22%" class="Row1">
                <label>Importo in pagamento alla data della morte (sentenza 495/93)</label>
            </td>
          <td style="width:26%">
          <asp:TextBox runat="server" ID="txtImportoPagamentoDataMorte49593" TabIndex="19" Width="80%" CssClass="tb8 txtUppercase"></asp:TextBox>
          <asp:RegularExpressionValidator runat="server" ID="validatetxtImportoPagamentoDataMorte49593" ControlToValidate="txtImportoPagamentoDataMorte49593"
                Display="Dynamic" ErrorMessage="Inserire un formato valido per Importo Pagamento Alla Data Morte"
                Text="*" CssClass="field-is-required" ValidationExpression="[0-9]*,?[0-9]{0,2}" ValidationGroup="UCPensioniCI" />
          </td>              
        </tr>     
    </table><br/><br/>  
    <table width="100%" class="tab-actions-group">
        <tr>
            <td style="text-align: center" class="tab-actions-group__first">
              <%--<asp:Button ID="btnAnnulla" runat="server" SkinID="btnAzione1" Enabled="true" Text="Pulisci"
                    Width="100px" CausesValidation="true" ValidationGroup="" />--%>
                <asp:Button ID="btSalvaPensioneCI" TabIndex="19" runat="server" SkinID="btnAzione1" Enabled="true" Text="Salva Pensione CI" Width="150px" CausesValidation="true" 
                    OnClientClick="if(Page_ClientValidate('UCPensioniCI')){aspnetForm.target ='_self'; BlockUI();}" onclick="btSalvaPensioneCI_Click" CssClass="primary"/>
            </td>
        </tr>
    </table>
</asp:Panel>
