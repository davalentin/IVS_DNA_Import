<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDatiAGO_PI.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributivi.UCDatiAGO_PI" %>

<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>

<asp:Panel runat="server" ID="pnlTop">
    <asp:Panel runat="server" ID="pnlElenco" Visible="true">
        <div class="containerWidth xs" style="margin-top: 5px;"></div>
        <div style="border: 1px solid black; margin-right: 3px; margin-left: 3px;" class="reset-style">
            <asp:GridView ID="gvElenco" runat="server"
                SkinID="grdElenco1"
                AutoGenerateColumns="False"
                Width="100%"
                GridLines="None"
                ShowHeader="true"
                ShowHeaderWhenEmpty="true"
                OnRowCommand="gvElenco_RowCommand"
                OnRowDataBound="gvElenco_RowDataBound"
                CssClass="intestazioneTabella full-width intestazioneTabella__with-pagination"
                PagerStyle-CssClass="default-pagination-tables">

                <Columns>

                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="2%" HeaderStyle-CssClass="intestazioneTabella Row1 formatLink"
                        ItemStyle-CssClass="TblRecordset3">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="img" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        HeaderText="Decorrenza AGO"
                        DataField="DecorrenzaAgo"
                        DataFormatString="{0:MM/yyyy}"
                        HeaderStyle-CssClass="intestazioneTabella"
                        ItemStyle-CssClass="TblRecordset3"
                        ItemStyle-HorizontalAlign="Center" />

                    <asp:TemplateField HeaderText=""
                        HeaderStyle-CssClass="intestazioneTabella"
                        ItemStyle-HorizontalAlign="Center"
                        ItemStyle-CssClass="TblRecordset3">

                        <ItemTemplate>

                            <asp:HiddenField ID="hdnIdDatiAgoFondoPI"
                                runat="server"
                                Value='<%# Eval("Id") %>' />

                            <asp:Button ID="btnModifica"
                                runat="server"
                                Text="Modifica"
                                SkinID="btnAzione1"
                                CommandName="Modifica"
                                CommandArgument="<%# Container.DataItemIndex %>"
                                CssClass="editIconOnly tertiary"
                                OnClientClick="BlockUI();" />


                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

                <EmptyDataTemplate>
                    <table width="100%" style="min-height: 40px;">
                        <tr>
                            <td style="text-align: center; padding: 10px;">
                                <asp:Label ID="lblNoData"
                                    runat="server"
                                    Text="Nessun dato trovato."
                                    SkinID="lblNoData" />
                            </td>
                        </tr>
                    </table>
                </EmptyDataTemplate>
            </asp:GridView>

            <div style="text-align: center; margin: 20px 0;">
                <asp:Button
                    ID="btnAggiungiDatiAgo"
                    runat="server"
                    Text="Aggiungi Dati Ago"
                    SkinID="btnAzione1"
                    OnClick="btnAggiungiDatiAgo_Click"
                    OnClientClick="BlockUI();" 
                     CssClass="tertiary force-left" />
            </div>

        </div>

    </asp:Panel>

    <br />

    <asp:Panel runat="server" ID="pnlDettaglio" Visible="false">
        <div style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" class="reset-style full-width">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblTitDatiAgo" runat="server" Text="Dati Ago" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                    </td>
                </tr>
            </table>

            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Decorrenza AGO:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtDecorrenzaAgo" Width="60%" CssClass="txtUppercase tb8" MaxLength="7"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtDecorrenzaAgo" runat="server"
                            ControlToValidate="txtDecorrenzaAgo" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Decorrenza AGO in formato non valido"
                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                            ValidationGroup="UCTabDatiAgoPI" />
                        <asp:CustomValidator runat="server" ID="customCheckDataDecorrenzaAgo"
                            ControlToValidate="txtDecorrenzaAgo" Display="Dynamic" Text="*" CssClass="field-is-required"
                            ErrorMessage="Decorrenza AGO: data illogica"
                            ValidationGroup="UCTabDatiAgoPI" ClientValidationFunction="checkCorrettezzaData" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>Tipo Liquidazione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:DropDownList runat="server" ID="ddlTipoLiquidazione" CssClass="tb8 txtUppercase xxs" Width="30%"></asp:DropDownList>
                        <%--<asp:RequiredFieldValidator runat="server" ID="RFVddlTipoLiquidazione" ControlToValidate="ddlTipoLiquidazione"
                            Display="Dynamic" ErrorMessage="Tipo Liquidazione obbligatorio" ValidationGroup="UCTabDatiAgoPI" Text="*" CssClass="field-is-required" />--%>
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Codice specifico:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtCodiceSpecifico" runat="server" MaxLength="1" Width="20%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtCodiceSpecifico" runat="server"
                            ControlToValidate="txtCodiceSpecifico" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Codice specifico: Inserire un carattere"
                            ValidationExpression="^[a-zA-Z]?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>Sospensione:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtSospensione" Width="60%" CssClass="txtUppercase tb8 date-picker dateMMaaaa" MaxLength="7"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSospensione" runat="server"
                            ControlToValidate="txtSospensione" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Sospensione AGO in formato non valido"
                            ValidationExpression="^[0-9]{1,2}\/[0-9]{4}|MM/AAAA|mm/aaaa$"
                            ValidationGroup="UCTabDatiAgoPI" />
                        <asp:CustomValidator runat="server" ID="customCheckDataSospensione"
                            ControlToValidate="txtSospensione" Display="Dynamic" Text="*" CssClass="field-is-required"
                            ErrorMessage="Sospensione: data illogica"
                            ValidationGroup="UCTabDatiAgoPI" ClientValidationFunction="checkCorrettezzaData" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Codice natura:</label>
                    </td>
                    <td class="field cod-nat" style="width: 25%">
                        <asp:TextBox ID="txtCodiceNatura1" runat="server" MaxLength="1" Width="15%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:TextBox ID="txtCodiceNatura2" runat="server" MaxLength="1" Width="15%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:TextBox ID="txtCodiceNatura3" runat="server" MaxLength="1" Width="15%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtCodiceNatura1" runat="server"
                            ControlToValidate="txtCodiceNatura1" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Codice natura: carattere non valido"
                            ValidationExpression="^[a-zA-Z0-9]?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                        <asp:RegularExpressionValidator ID="REVtxtCodiceNatura2" runat="server"
                            ControlToValidate="txtCodiceNatura2" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Codice natura: carattere non valido"
                            ValidationExpression="^[a-zA-Z0-9]?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                        <asp:RegularExpressionValidator ID="REVtxtCodiceNatura3" runat="server"
                            ControlToValidate="txtCodiceNatura3" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Codice natura: carattere non valido"
                            ValidationExpression="^[a-zA-Z0-9]?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>Settimane VV:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneVV" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSettimaneVV" runat="server"
                            ControlToValidate="txtSettimaneVV" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane VV: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Causa Carico:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtCausaCarico" runat="server" MaxLength="50" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtCausaCarico" runat="server" ControlToValidate="txtCausaCarico" Display="Dynamic"
                            Enabled="true" Text="*" CssClass="field-is-required" ErrorMessage="Causa Carico: Inserire un carattere" ValidationExpression="^[a-zA-Z]?$" ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                    <td class="Row1" style="width: 25%"></td>
                    <td class="field" style="width: 25%"></td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Diritto Quote fisse:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtDirittoQuoteFisse" runat="server" MaxLength="50" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtDirittoQuoteFisse" runat="server"
                            ControlToValidate="txtDirittoQuoteFisse" Display="Dynamic" Enabled="true" Text="*"
                            ErrorMessage="Diritto Quote Fisse: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                    <td class="Row1" style="width: 25%">
                        <label>Contribuzione Esclusiva:</label>
                    </td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtCtres" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtCtres" runat="server"
                            ControlToValidate="txtCtres" Display="Dynamic" Enabled="true" Text="*"
                            ErrorMessage="Contribuzione Esclusiva: Inserire valori interi o decimali"
                            ValidationExpression="\d{1,5}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />

                    </td>
                </tr>
                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Settimane Ex Combattente:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettExComb" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                            ControlToValidate="txtSettExComb" Display="Dynamic" Enabled="true" Text="*"
                            ErrorMessage="Settimane Ex Combattenti: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                     <td class="Row1" style="width: 25%">
                        <label>RMS Retributivo:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtRMSRetr" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server"
                            ControlToValidate="txtRMSRetr" Display="Dynamic" Enabled="true" Text="*"
                            ErrorMessage="RMS Retributivo: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            ValidationExpression="\d{1,6}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>
            </table>
        </div>

        <div style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" class="reset-style full-width">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblTitDatiRetributivi" runat="server" Text="Dati Retributivi" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaA" runat="server" Text="Quota A" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>RMS:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtRMSQuotaA" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSQuotaA" runat="server"
                            ControlToValidate="txtRMSQuotaA" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="RMS Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            ValidationExpression="\d{1,6}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>RMS Omogenea:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtRMSOmogeneaQuotaA" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSOmogeneaQuotaA" runat="server"
                            ControlToValidate="txtRMSOmogeneaQuotaA" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="RMS Omogenea Quota A: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            ValidationExpression="\d{1,6}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Settimane totali:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneTotQuotaA" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSettimaneTotQuotaA" runat="server"
                            ControlToValidate="txtSettimaneTotQuotaA" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane totali Quota A: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>Settimane esclusive:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneEscQuotaA" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSettimaneEscQuotaA" runat="server"
                            ControlToValidate="txtSettimaneEscQuotaA" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane esclusive Quota A: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblQuotaB" runat="server" Text="Quota B" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>RMS:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtRMSQuotaB" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSQuotaB" runat="server"
                            ControlToValidate="txtRMSQuotaB" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="RMS Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            ValidationExpression="\d{1,6}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>RMS Omogenea:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtRMSOmogeneaQuotaB" runat="server" MaxLength="11" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtRMSOmogeneaQuotaB" runat="server"
                            ControlToValidate="txtRMSOmogeneaQuotaB" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="RMS Omogenea Quota B: Inserire valori interi o decimali (max 6 interi e 4 decimali)"
                            ValidationExpression="\d{1,6}(,\d{1,4})?$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Settimane totali:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneTotQuotaB" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSettimaneTotQuotaB" runat="server"
                            ControlToValidate="txtSettimaneTotQuotaB" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane totali Quota B: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <td class="Row1" style="width: 25%">
                        <label>Settimane esclusive:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneEscQuotaB" runat="server" MaxLength="4" Width="60%" CssClass="tb8 txtUppercase"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVtxtSettimaneEscQuotaB" runat="server"
                            ControlToValidate="txtSettimaneEscQuotaB" Display="Dynamic" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane esclusive Quota B: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                </tr>
            </table>
        </div>

        <div style="border-style: solid; border-color: #000080; border-collapse: collapse; border-width: 1px; width: 710px; margin-left: 4px; margin-top: 4px;" class="reset-style full-width">
            <table class="tabellaFormattazione grid grid-size-20">
                <tr>
                    <td class="Row1 shift-full-grid" style="text-align: left">
                        <asp:Label ID="lblTitDatiContributivi" runat="server" Text="Dati Contributivi" Style="font-weight: bold" CssClass="section-label"></asp:Label>
                    </td>
                </tr>

                <tr>
                    <td class="Row1" style="width: 25%">
                        <label>Montante totale:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtMontanteTotale" CssClass="tb8 txtUppercase" MaxLength="12" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteTotale" Display="Dynamic"
                            ControlToValidate="txtMontanteTotale" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Montante totale: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            ValidationExpression="\d{1,7}(\,\d{1,4})?"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>

                    <%--<td class="Row1" style="width: 25%">
                        <label>Montante esclusivo:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox runat="server" ID="txtMontanteEsclusivo" CssClass="tb8 txtUppercase" MaxLength="12" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtMontanteEsclusivo" Display="Dynamic"
                            ControlToValidate="txtMontanteEsclusivo" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Montante esclusivo: Inserire valori interi o decimali (max 7 interi e 4 decimali)"
                            ValidationExpression="\d{1,7}(\,\d{1,4})?"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>--%>
                </tr>

                <%--<tr>
                    <td class="Row1" style="width: 25%">
                        <label>Settimane esclusive:</label></td>
                    <td class="field" style="width: 25%">
                        <asp:TextBox ID="txtSettimaneEsclusive" runat="server" CssClass="tb8 txtUppercase" MaxLength="4" Width="60%"></asp:TextBox>
                        <asp:RegularExpressionValidator runat="server" ID="REVtxtSettimaneEsclusive" Display="Dynamic"
                            ControlToValidate="txtSettimaneEsclusive" Enabled="true" Text="*" CssClass="field-is-required"
                            ErrorMessage="Settimane esclusive: Inserire valori interi"
                            ValidationExpression="^[0-9]*$"
                            ValidationGroup="UCTabDatiAgoPI" />
                    </td>
                    <td class="Row1" style="width: 25%"></td>
                    <td class="field" style="width: 25%"></td>
                </tr>--%>
            </table>
        </div>

        <br />
        <div class="containerWidth xs reset-style full-width" style="margin-right: 40px; margin-top: 10px;">
            <table width="100%" class="tab-actions-group">
                <tr>
                    <td style="text-align: center;" class="tab-actions-group__first">
                        <asp:Button
                            ID="btnSalvaDettaglio" runat="server" SkinID="btnAzione1"
                            Text="Salva" ValidationGroup="UCTabDatiAgoPI"
                            OnClick="btnSalvaDettaglio_Click"
                            OnClientClick="if(Page_ClientValidate('UCTabDatiAgoPI')){aspnetForm.target ='_self'; BlockUI();} else {return false;}" 
                            CssClass="primary force-right" />
                        &nbsp;

                         <asp:Button
                            ID="btnEliminaDettaglioDatiAgo"
                            runat="server"
                            SkinID="btnAzione1"
                            Text="Elimina"
                            CssClass="ghost-delete force-center"
                            CausesValidation="false"
                            OnClick="btnEliminaDettaglioDatiAgo_Click"
                            OnClientClick="BlockUI();"/>

                        &nbsp;

                <asp:Button runat="server" ID="btnTornaElenco" SkinID="btnAzione1" Text="Torna alla lista"
                    CausesValidation="False" OnClick="btnTornaElenco_Click"  OnClientClick="BlockUI();" 
                            CssClass="force-left" />
                    </td>
                </tr>
            </table>
        </div>


    </asp:Panel>

</asp:Panel>

<asp:HiddenField ID="hfRowIndex" runat="server" />
<asp:HiddenField ID="hfIdDatiAgo" runat="server" />
<asp:HiddenField ID="hdnTest3" runat="server" />
