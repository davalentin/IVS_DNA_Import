<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCOneri.ascx.cs" Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri.UCOneri" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Panel runat="server" ID="pnlOneri">
    <br />
    <!-- GridView Oneri -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblTitoloGV0neri" runat="server" Text="Oneri" Style="font-weight: bold" CssClass="section-label"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblOneriSperDonna" runat="server" Text="Indicare come cessazione la decorrenza pensione con regole Monti/Fornero" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblEditoria" runat="server" Text="La cessazione beneficio deve corrispondere alla decorrenza pensione con le norme vigenti." Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblCessPrecoci" runat="server" Text="Cess. Ben: primo diritto a pensione fra pensione di vecchiaia e anticipata" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr><tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblCessBeneficioPrecoci" runat="server" Text="Cess. incumulabilità: decorrenza della pensione anticipata" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblCessQuota100" runat="server" Text="Cessazione beneficio: decorrenza più prossima tra quella della pensione di vecchiaia e quella dell’anticipata con i requisiti ordinari" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr><tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblCessBeneficioQuota100" runat="server" Text="Cessazione incumulabilità: decorrenza pensione di Vecchiaia" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblCessBeneficioQuota102" runat="server" Text="Cessazione incumulabilità: maggiore tra decorrenza beneficio e decorrenza pensione di vecchiaia" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblOpzDonna2023" runat="server" Text="Indicare come cessazione la prima decorrenza utile per l’accesso al pensionamento anticipato o di vecchiaia sulla base dei requisiti antecedenti all’introduzione del D.L. 4 del 2019" Style="font-weight: bold" ForeColor="Red" Visible="false"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 97%; margin: 7px;">
                    <asp:GridView runat="server" ID="gvOneri" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="true"
                        Width="100%" PageSize="10" AllowPaging="true" OnPageIndexChanging="gvOneri_onPageIndexChanging"
                        OnRowDataBound="gvOneri_RowDataBound" OnRowEditing="gvOneri_RowEditing" OnRowCommand="gvOneri_RowCommand"
                        OnRowUpdating="gvOneri_RowUpdating" OnRowCancelingEdit="gvOneri_RowCancelingEdit" OnLoad="gvOneri_Load" RowStyle-HorizontalAlign="Center"
                         EnableViewState="true" PagerStyle-CssClass="default-pagination-tables">
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
                                <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlGruppo" runat="server"
                                        Width="100px" Visible="false">
                                    </asp:DropDownList>
                                    <asp:Label runat="server" ID="lblGruppo_Edit" Text='<%#Bind("IdCodeGruppo")%>' Width="100px"> 
                                    </asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SottoGruppo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSottoGruppo" Text='<%#Bind("IdCodeSottoGruppo")%>'
                                        Width="100px"> 
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList CssClass="tb8 txtUppercase" ID="ddlSottoGruppo" runat="server"
                                        Width="100px">
                                    </asp:DropDownList>
                                    <asp:Label runat="server" ID="lblSottoGruppo" Text='<%#Bind("IdCodeSottoGruppo")%>'
                                        Width="100px" Visible="false" /> 
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Dec. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza" CssClass="txtUppercase" Width="70px" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenza_Edit" CssClass="txtUppercase" Width="75px" />
                                    <asp:Panel runat="server" ID="pnlTxtDecorrenza" Visible="false">
                                        <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtDecorrenza"
                                            MaxLength="7"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldtxtDecorrenza" runat="server" ErrorMessage="Decorrenza: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtDecorrenza" ValidationGroup="UCTabOneri" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtDecorrenza" Display="Dynamic"
                                            ControlToValidate="txtDecorrenza" Enabled="true" ErrorMessage="Decorrenza: Inserire una data valida"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenza" Display="Dynamic"
                                            ErrorMessage="Dec. Ben.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri"
                                            ID="customCheckDataDecorrenzaBenefici" ClientValidationFunction="checkCorrettezzaData" />
                                    </asp:Panel>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cess. Ben." HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessazione" CssClass="txtUppercase" Width="75px" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Panel runat="server" ID="pnlCessazione">
                                        <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtCessazione"
                                            MaxLength="7"></asp:TextBox>
                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldtxtCessazione" runat="server" ErrorMessage="Cessazione: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtCessazione" ValidationGroup="UCTabOneri" Display="Dynamic"></asp:RequiredFieldValidator>--%>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtCessazione" Display="Dynamic"
                                            ControlToValidate="txtCessazione" Enabled="true" ErrorMessage="Cessazione: Inserire una data valida"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                                            ErrorMessage="Cess. Ben.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri"
                                            ID="customCheckDataCessazioneBenefici" ClientValidationFunction="checkCorrettezzaData" />
                                    </asp:Panel>
                                    <asp:Panel runat="server" ID="pnlCessazioneFS_PT" Visible="false">
                                        <asp:TextBox CssClass="tb8 date-picker-base txtUppercase dateGGmmAAAA" runat="server"
                                            ID="txtCessazioneFS_PT" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldtxtCessazioneFS_PT" runat="server" ErrorMessage="Cessazione: Campo obbligatorio"
                                            Text="*" CssClass="field-is-required" ControlToValidate="txtCessazioneFS_PT" ValidationGroup="UCTabOneri"
                                            Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator runat="server" ID="validatetxtCessazioneFS_PT" Display="Dynamic"
                                            ControlToValidate="txtCessazioneFS_PT" Enabled="true" ErrorMessage="Cessazione: Inserire una data valida"
                                            Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri" ValidationExpression="^[0-9]{1,2}\/[0-9]{1,2}\/[0-9]{4}|GG/MM/AAAA|gg/mm/aaaa$" />
                                        <asp:CustomValidator runat="server" ControlToValidate="txtCessazioneFS_PT" Display="Dynamic"
                                            ErrorMessage="Cess. Ben.: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri"
                                            ID="customCheckDataCessazioneBeneficiFS_PT" ClientValidationFunction="checkCorrettezzaData" />
                                    </asp:Panel>
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
                                        Text="*" CssClass="field-is-required" ValidationExpression="^([0-9]+)$" ValidationGroup="UCTabOneri" />
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
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}(,\d{1,4})?" ValidationGroup="UCTabOneri" />
                                </EditItemTemplate>--%>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Cess. incumulabilità" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" HeaderStyle-Width="110px">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCessIncumul" CssClass="txtUppercase" Width="75px" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox CssClass="tb8 date-picker txtUppercase dateMMaaaa" runat="server" ID="txtCessIncumul"
                                        MaxLength="7"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldtxtCessIncumul" runat="server" ErrorMessage="Cess. incumulabilità: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtCessIncumul" ValidationGroup="UCTabOneri" Display="Dynamic"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ID="validatetxtCessIncumul" Display="Dynamic"
                                        ControlToValidate="txtCessIncumul" Enabled="true" ErrorMessage="Cess. incumulabilità: Inserire una data valida"
                                        Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri" ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$" />
                                    <asp:CustomValidator runat="server" ControlToValidate="txtCessazione" Display="Dynamic"
                                        ErrorMessage="Cess. incumulabilità: data illogica" Text="*" CssClass="field-is-required" ValidationGroup="UCTabOneri"
                                        ID="customCheckDataCessIncumul" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="1%" ItemStyle-Width="1%"
                                FooterStyle-Width="1%">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDelete" CommandName="elimina" CommandArgument="elimina" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:HiddenField runat="server" ID="modalitaEditOneri" Value="false" />
    <!-- Fine GridView Oneri -->
    <br />
    <br />
    <br />
    <asp:Panel runat="server" ID="pnlBeneficiParticolari">
    <!-- GridView Benefici Particolari -->
    <table class="tabellaFormattazione">
        <tr>
            <td class="Row1" style="text-align: left" colspan="2">
                <asp:Label ID="lblTitoloBeneficiParticolari" runat="server" Text="Benefici Particolari"
                    Style="font-weight: bold" CssClass="section-label mt-32"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="tabellaFormattazione grid-col-1">
        <tr>
            <td class="Row1">
                <div class="bckGridViewElenco" style="width: 97%; margin: 7px;">
                    <asp:GridView runat="server" ID="gvBenefici" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" BorderWidth="1" BorderColor="Black" AutoGenerateEditButton="false"
                        Width="100%" PageSize="10" AllowPaging="true" OnPageIndexChanging="gvBenefici_onPageIndexChanging"
                        OnRowDataBound="gvBenefici_RowDataBound" RowStyle-HorizontalAlign="Center" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun dato 'Benefici Particolari' trovato."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="Codice Benefici" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCodiceBenefici" Text='<%#Bind("CodiceBenefici")%>'
                                        Width="150px"> 
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
    </asp:Panel>
    <div id="pulsantiSaveDelete" style="margin-top: 200px; margin-right: 40px;" class="containerWidth md">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: center" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaDatiOneri" runat="server" SkinID="btnAzione1" Enabled="true"
                        Text="Salva Oneri" Width="160px" OnClick="btnSalvaDatiOneri_Click" OnClientClick="BlockUI();" CssClass="primary"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>