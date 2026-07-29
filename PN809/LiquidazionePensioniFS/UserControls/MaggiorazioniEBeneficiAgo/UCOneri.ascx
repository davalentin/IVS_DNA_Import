<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCOneri.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.MaggiorazioniEBeneficiAgo.UCOneri" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<script type="text/javascript">
    function SetSettimaneBeneficiParticolari(nSettimane) {
        $(document.getElementById("ctl00_ContentPlaceHolder1_ucOneri_gvBenefici_ctl02_lblSettimane")).text(nSettimane);
    }
</script>

<asp:Panel runat="server" ID="pnlOneri"><br />
    <!-- GridView Oneri -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align:left" colspan="2">
                <asp:Label ID="lblTitoloGV0neri" runat="server" Text="Oneri" style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 700px; margin: 7px;">
                        <asp:GridView runat="server" ID="gvOneri" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="true"
                        Width="100%" PageSize="10" AllowPaging="true" 
                        OnPageIndexChanging="gvOneri_onPageIndexChanging" OnRowDataBound="gvOneri_RowDataBound"
                        OnRowEditing="gvOneri_RowEditing" OnRowCommand="gvOneri_RowCommand"
                        OnRowUpdating="gvOneri_RowUpdating" OnRowCancelingEdit="gvOneri_RowCancelingEdit">
                        <EmptyDataRowStyle ForeColor="Red" />
                            <EmptyDataTemplate>
                                <center>
                                    <asp:Label ID="lblNoData" runat="server" Text="Nessun dato 'Oneri' trovato." SkinID="lblNoData"
                                        Visible="true"></asp:Label>
                                </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Gruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblGruppo" Text='<%#Bind("IdCodeGruppo")%>' Width="100px"> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblGruppo_Edit" Text='<%#Bind("IdCodeGruppo")%>' Width="100px"> 
                                    </asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SottoGruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSottoGruppo" Text='<%#Bind("IdCodeSottoGruppo")%>' Width="140px"> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlSottoGruppo" runat="server" Width="140px">
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dec. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>'
                                    CssClass="txtUppercase" Width="50px"/>  
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza_Edit" Text='<%# Bind("Decorrenza", "{0:MM/yyyy}")%>'
                                    CssClass="txtUppercase" Width="50px"/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cess. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessazione" Text='<%# Bind("Scadenza", "{0:MM/yyyy}")%>'
                                    CssClass="txtUppercase" Width="60px"/>  
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase" runat="server" ID="txtCessazione"
                                        MaxLength="7" Text=' <%# Bind("Scadenza", "{0:MM/yyyy}")%>' Width="60px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtCessazione" runat="server" ErrorMessage="Cessazione: Campo obbligatorio"
                                        Text="*" ControlToValidate="txtCessazione" ValidationGroup="UCTabOneri" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="validatetxtCessazione" Display="Dynamic"
                                        ControlToValidate="txtCessazione" Enabled="true" ErrorMessage="Cessazione: Inserire una data valida"
                                        Text="*" ValidationGroup="UCTabOneri" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                                        ErrorMessage="Cess. Ben.: data illogica" Text="*" ValidationGroup="UCTabOneri"
                                        ID="customCheckDataCessazioneBeneficio" ClientValidationFunction="checkCorrettezzaData" />  
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sett." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane" Text='<%#Bind("Settimane")%>' Width="40px"> 
                                    </asp:Label>
                                </ItemTemplate>
                                <%-- Modifica inserita a seguito della mail del 17/07/2014 inviata da Nunzio con oggetto: RE: ReEng Pensioni - Oneri Salvaguardia
                                
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtSettimane" MaxLength="4"
                                        Text=' <%# Bind("Settimane")%>' Width="40px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtSettimane" runat="server" ControlToValidate="txtSettimane"
                                        Display="Dynamic" ErrorMessage="Settimane: inserire il numero di settimane  in un formato valido"
                                        Text="*" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabOneri" />
                                </EditItemTemplate>--%>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Onere" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOnere" Text='<%#Bind("Onere")%>' Width="100px"> </asp:Label>
                                </ItemTemplate>
                                <%-- Modifica inserita a seguito della mail del 17/07/2014 inviata da Nunzio con oggetto: RE: ReEng Pensioni - Oneri Salvaguardia
                                
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 txtUppercase" runat="server" ID="txtOnere" MaxLength="12"
                                        Text=' <%# Bind("Onere")%>' Width="100px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtOnere" runat="server" ControlToValidate="txtOnere"
                                        Display="Dynamic" ErrorMessage="Onere: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                                        Text="*" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabOneri" />
                                </EditItemTemplate>--%>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditOneri" Value="false" />
    <!-- Fine GridView Oneri -->
    <br /><br /><br />
    <!-- GridView Benefici Particolari -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align:left" colspan="2">
                <asp:Label ID="lblTitoloBeneficiParticolari" runat="server" Text="Benefici Particolari" style="font-weight: bold"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaContenuti">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 700px; margin: 7px;">
                        <asp:GridView runat="server" ID="gvBenefici" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                        Width="100%" PageSize="10" AllowPaging="true" 
                        OnPageIndexChanging="gvBenefici_onPageIndexChanging" OnRowDataBound="gvBenefici_RowDataBound">
                        <EmptyDataRowStyle ForeColor="Red" />
                            <EmptyDataTemplate>
                                <center>
                                    <asp:Label ID="lblNoData" runat="server" Text="Nessun dato 'Benefici Particolari' trovato." SkinID="lblNoData"
                                        Visible="true"></asp:Label>
                                </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Benefici" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceBenefici" Text='<%#Bind("CodiceBenefici")%>' Width="150px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Settimane" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSettimane" Text='<%#Bind("Settimane")%>' Width="150px"> 
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditBenefici" Value="false" />
    <!-- Fine GridView Benefici Particolari -->
    
    <div id="pulsantiSaveDelete" style="width: 720px; margin-top: 200px; margin-right: 40px;">
        <table width="100%">
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnSalvaDatiOneri" runat="server" SkinID="btnAzione1"
                        Enabled="true" Text="Salva Oneri" Width="160px" OnClick="btnSalvaDatiOneri_Click" OnClientClick="BlockUI();"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>