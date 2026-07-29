<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCDanteSentenza49593.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DanteCausa.UCDanteSentenza49593" %>
<%@ Register Src="~/UserControls/UCAvviso.ascx" TagName="UCAvviso" TagPrefix="UCA" %>
<UCA:UCAvviso runat="server" ID="ucAvviso" Visible="false" />
<asp:Panel runat="server" ID="pnlSentenza49593">
    <asp:HiddenField runat="server" ID="modalitaEditSentenza" Value="false" />
    <div id="divSentenze" runat="server" style="margin-left: 10px; margin-right: 10px;"
        visible="false">
        <table class="tabellaFormattazione">
            <tr>
                <td style="width: 35%; font-size: small; font-weight: bold">
                    <br />
                    <label>Sentenze</label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:GridView ID="GridViewSentenze" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" RowStyle-HorizontalAlign="Center"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="GridViewSentenze_RowCancelingEdit"
                        OnRowCommand="GridViewSentenze_RowCommand" OnRowDataBound="GridViewSentenze_RowDataBound"
                        OnRowEditing="GridViewSentenze_RowEditing" OnRowUpdating="GridViewSentenze_RowUpdating"
                        OnPageIndexChanging="GridViewSentenze_PageIndexChanging" PageSize="10" SkinID="grdElenco1"
                        Width="100%" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:label ID="lblNoData" runat="server" Text="Nessuna Sentenza Trovata"
                                    SkinID="lblNoData" Visible="true"></asp:label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <%--Colonna Sentenza--%>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="192px"
                                HeaderText="Sentenza" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSentenza" Width="100px" runat="server" Text='<%#Bind("Sentenza") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlSentenza" runat="server" CssClass="txtUppercase tb8" Width="100px" selectedValue='<%# Bind("Sentenza") %>'>
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="SI" Value="SI"></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <%--Colonna Codice--%>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="192px"
                                HeaderText="Codice" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblCodice" Width="100px" runat="server" Text='<%#Bind("Codice") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtCodice" runat="server" CssClass="txtUppercase tb8" MaxLength="1"
                                        Text='<%#Bind("Codice") %>' Width="150px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtCodiceSentenza" runat="server" ControlToValidate="txtCodice"
                                        Display="Dynamic" ErrorMessage="Codice Sentenza in un formato non valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]$" ValidationGroup="UCDanteSentenze49593Sentenze" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <%--Colonna Data Dal--%>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="191px"
                                HeaderText="Data Dal" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblSentenzeDataDal" Width="100px" runat="server" Text='<%#Bind("DataDal") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSentenzeDataDal" runat="server" CssClass="tb8 date-picker txtUppercase dateMMaaaa"
                                        MaxLength="7" Text=' <%# Bind("DataDal", "{0:MM/yyyy}")%>' Width="150px"></asp:TextBox>                                        
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <%--Bottone Cancella--%>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteSentenze" ToolTip="cancella" runat="server"
                                        Text="" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--Hidden Guid--%>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="SentenzaHdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEditPre2009" Value="false" />
    <div id="divRedditiAnte2009" runat="server" style="margin-left: 10px; margin-right: 10px;"
        visible="False">
        <table class="tabellaFormattazione">
            <tr>
                <td style="width: 35%; font-size: small; font-weight: bold">
                    <br />
                    <label class="section-label mb-8">
                        Redditi del Dante Causa in applicazione sentenza 495/93 ante 2009</label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:GridView ID="gvSentenzaAnte2009" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" RowStyle-HorizontalAlign="Center"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="gvSentenzaAnte2009_RowCancelingEdit"
                        OnRowCommand="gvSentenzaAnte2009_RowCommand" OnRowDataBound="gvSentenzaAnte2009_RowDataBound"
                        OnRowEditing="gvSentenzaAnte2009_RowEditing" OnRowUpdating="gvSentenzaAnte2009_RowUpdating"
                        OnPageIndexChanging="gvSentenzaAnte2009_PageIndexChanging" PageSize="10" SkinID="grdElenco1"
                        Width="100%" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun reddito in applicazione alla sentenza ante 2009 trovato."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Anno"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAnno" Width="100px" runat="server" Text='<%#Bind("AnnoReddito") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAnno" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                                        Text='<%#Bind("AnnoReddito") %>' Width="150px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldTxtAnno" runat="server" ErrorMessage="Anno sentenza ante 2009: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtAnno" ValidationGroup="UCDanteSentenze49593"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtAnno" runat="server" ControlToValidate="txtAnno"
                                        Display="Dynamic" ErrorMessage="Anno sentenza ante 2009: inserire l'anno in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="UCDanteSentenze49593" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Importo"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblImporto" Width="100px" runat="server" Text='<%#Bind("RedditoTitolare") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtImporto" runat="server" CssClass="txtUppercase tb8 " MaxLength="10"
                                        Text='<%#Bind("RedditoTitolare") %>' Width="150px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldTxtImporto" runat="server" ErrorMessage="Importo sentenza ante 2009: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtImporto" ValidationGroup="UCDanteSentenze49593"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtImporto" runat="server" ControlToValidate="txtImporto"
                                        Display="Dynamic" ErrorMessage="Importo sentenza ante 2009: inserire l'importo in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderText="Reddito Coniuge"
                                ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoConiuge" Width="100px" runat="server" Text='<%#Bind("RedditoConiuge") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoConiuge" runat="server" CssClass="txtUppercase tb8 " MaxLength="15"
                                        Text=' <%# Bind("RedditoConiuge")%>' Width="150px"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoConiuge" runat="server" ControlToValidate="txtRedditoConiuge"
                                        Display="Dynamic" ErrorMessage="Reddito Coniuge sentenza ante 2009: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteSentenzaAnte2009" ToolTip="cancella" runat="server"
                                        Text="" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField runat="server" ID="modalitaEditPost2008" Value="false" />
    <div id="divRedditiPost2008" runat="server" style="margin-left: 10px; margin-right: 10px;"
        visible="False">
        <table class="tabellaFormattazione">
            <tr>
                <td style="width: 35%; font-size: small; font-weight: bold">
                    <br />
                    <label id="lblRedditiPost2008" runat="server" class="section-label mb-8 mt-32">
                        Redditi del Dante Causa in applicazione sentenza 495/93 post 2008</label><br />
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:GridView ID="gvSentenzaPost2008" runat="server" AllowPaging="true" AutoGenerateColumns="false"
                        AutoGenerateEditButton="true" BorderColor="Black" BorderWidth="1" RowStyle-HorizontalAlign="Center"
                        CssClass="intestazioneTabella intestazioneTabella__with-pagination" EnableViewState="true" OnRowCancelingEdit="gvSentenzaPost2008_RowCancelingEdit"
                        OnRowCommand="gvSentenzaPost2008_RowCommand" OnRowDataBound="gvSentenzaPost2008_RowDataBound"
                        OnRowEditing="gvSentenzaPost2008_RowEditing" OnRowUpdating="gvSentenzaPost2008_RowUpdating"
                        OnPageIndexChanging="gvSentenzaPost2008_PageIndexChanging" PageSize="10" SkinID="grdElenco1"
                        Width="100%" PagerStyle-CssClass="default-pagination-tables">
                        <EmptyDataRowStyle ForeColor="Red" />
                        <EmptyDataTemplate>
                            <center>
                                <asp:Label ID="lblNoData" runat="server" Text="Nessun reddito in applicazione alla sentenza post 2008 trovato."
                                    SkinID="lblNoData" Visible="true"></asp:Label>
                            </center>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="115px"
                                HeaderText="Anno" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblAnno" Width="90%" runat="server" Text='<%#Bind("AnnoReddito") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAnno" runat="server" CssClass="tb8 txtUppercase" MaxLength="4"
                                        Text='<%#Bind("AnnoReddito") %>' Width="70%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldTxtAnno" runat="server" ErrorMessage="Anno sentenza post 2008: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtAnno" ValidationGroup="UCDanteSentenze49593Post"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtAnno" runat="server" ControlToValidate="txtAnno"
                                        Display="Dynamic" ErrorMessage="Anno sentenza ppost 2008: inserire l'anno in un formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="^[0-9]{4}$" ValidationGroup="UCDanteSentenze49593Post" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="115px"
                                HeaderText="Reddito pensione DC" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoDC" Width="90%" runat="server" Text='<%#Bind("RedditoDaPensioneDC") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoDC" runat="server" CssClass="txtUppercase tb8 " MaxLength="10"
                                        Text='<%#Bind("RedditoDaPensioneDC") %>' Width="70%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldTxtRedditoDC" runat="server" ErrorMessage="Reddito pensione DC: Campo obbligatorio"
                                        Text="*" CssClass="field-is-required" ControlToValidate="txtRedditoDC" ValidationGroup="UCDanteSentenze49593Post"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoDC" runat="server" ControlToValidate="txtRedditoDC"
                                        Display="Dynamic" ErrorMessage="Reddito pensione DC: inserire l'importo in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593Post" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="115px"
                                HeaderText="Reddito diverso da pensione DC" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoPensioneNoDC" Width="90%" runat="server" Text='<%#Bind("RedditoTitolare") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoPensioneNoDC" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Text=' <%# Bind("RedditoTitolare")%>' Width="80%"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoPensioneNoDC" runat="server"
                                        ControlToValidate="txtRedditoPensioneNoDC" Display="Dynamic" ErrorMessage="Reddito diverso da pensione DC: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593Post" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="115px"
                                HeaderText="Reddito pensione coniuge" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoPensioneConiuge" Width="90%" runat="server" Text='<%#Bind("RedditoDaPensioneConiuge") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoPensioneConiuge" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Text=' <%# Bind("RedditoDaPensioneConiuge")%>' Width="80%"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoPensioneConiuge" runat="server"
                                        ControlToValidate="txtRedditoPensioneConiuge" Display="Dynamic" ErrorMessage="Reddito pensione coniuge: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593Post" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" HeaderStyle-Width="115px"
                                HeaderText="Reddito diverso da pensione coniuge" ItemStyle-CssClass="TblRecordset3">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedditoNoPensioneConiuge" Width="90%" runat="server" Text='<%#Bind("RedditoConiuge") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRedditoNoPensioneConiuge" runat="server" CssClass="txtUppercase tb8 "
                                        MaxLength="15" Text=' <%# Bind("RedditoConiuge")%>' Width="80%"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regularTxtRedditoNoPensioneConiuge" runat="server"
                                        ControlToValidate="txtRedditoNoPensioneConiuge" Display="Dynamic" ErrorMessage="Reddito pensione diverso da coniuge: inserire il reddito in formato valido"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,}\,?\d{0,}" ValidationGroup="UCDanteSentenze49593Post" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella" HeaderText="&nbsp;&nbsp;&nbsp;"
                                HeaderStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnDeleteSentenzaPost2008" ToolTip="cancella" runat="server"
                                        Text="" CommandName="Elimina" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                    <asp:HiddenField runat="server" ID="hdnGUID" Visible="false" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    <!-- ENG - Gestione Pensione Estera e redditi Sentenza 495-->
    <div id="divImportoMensilePensioneEstera" runat="server" style="margin-top: 20px;
        margin-left: 10px; margin-right: 10px;" visible="false">
        <div style="width: 70%; display: inline;" class="flex-align-center">
            <asp:Label ID="lblImportoMensilePensioneEstera" runat="server" Text="Importo mensile della Pensione Estera:" CssClass="flex-align-center font-semibold"></asp:Label>
        </div>
        <div style="display: inline; width: 30%;">
            <asp:TextBox ID="txtImportoMensilePensioneEstera" runat="server"></asp:TextBox>
            <asp:RegularExpressionValidator ID="revImportoMensilePensioneEstera" runat="server"
                ControlToValidate="txtImportoMensilePensioneEstera" Display="Dynamic" ErrorMessage="Importo Mensile Pensione Estera: inserire l'importo in formato valido (max 7 interi e 4 decimali)"
                Text="*" CssClass="field-is-required" ValidationExpression="\d{1,7}\,?\d{0,4}" ValidationGroup="UCTabSentenza495" />
        </div>
    </div>
    <div style="margin-top: 25px; margin-right: 40px;" class="containerWidth xs">
        <table width="100%" class="tab-actions-group">
            <tr>
                <td style="text-align: right" class="tab-actions-group__first">
                    <asp:Button ID="btnSalvaRedditi" TabIndex="6" runat="server" SkinID="btnAzione1"
                        Enabled="true" Text="Salva Redditi" Width="150px" CausesValidation="true" OnClientClick="if(Page_ClientValidate('UCTabSentenza495')){aspnetForm.target ='_self'; BlockUI();}"
                        OnClick="btnSalvaRedditi_Click" ValidationGroup="UCTabSentenza495" CssClass="primary"/>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaRedditi" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Elimina Redditi" Width="150px" OnClick="btnEliminaRedditi_Click"
                        OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Redditi?')) return false; else BlockUI();" CssClass="ghost-delete"/>
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
