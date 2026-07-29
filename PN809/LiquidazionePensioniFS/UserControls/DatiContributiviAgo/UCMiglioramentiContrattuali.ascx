<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCMiglioramentiContrattuali.ascx.cs"
    Inherits="INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.DatiContributiviAgo.UCMiglioramentiContrattuali" %>
<asp:Panel runat="server" ID="pnlMiglioramentiContrattuali">
    <div id="divMiglioramentiContrattuali" runat="server" style="margin-left: 10px; margin-right: 10px;">
        <br />
        <table class="tabellaFormattazione" style="padding-left: 10px; padding-bottom: 10px;
            width: 99%">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblMiglioramentiContrattuali"> Miglioramenti Contrattuali:</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:GridView runat="server" ID="gvMiglioramentiContrattuali" SkinID="grdElenco1" AutoGenerateColumns="false"
                        CssClass="intestazioneTabella" BorderWidth="1" Width="100%" BorderColor="Black"
                        OnRowCommand="gvMiglioramentiContrattuali_RowCommand" OnRowDataBound="gvMiglioramentiContrattuali_RowDataBound"
                        OnRowCancelingEdit="gvMiglioramentiContrattuali_RowCancelingEdit" OnRowEditing="gvMiglioramentiContrattuali_RowEditing"
                        OnDataBound="gvMiglioramentiContrattuali_DataBound" OnDataBinding="gvMiglioramentiContrattuali_DataBinding"
                        OnRowUpdating="gvMiglioramentiContrattuali_RowUpdating" OnRowDeleting="gvMiglioramentiContrattuali_RowDeleting"
                        EnableViewState="true">
                        <Columns>
                            <asp:CommandField ItemStyle-Width="6%" ShowEditButton="true" />
                            <asp:TemplateField HeaderText="Codice Gestione" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="16%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblEnteGestioneFondo_item" Width="100px" CssClass="txtUppercase"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlEnteGestioneFondo" Width="100px" CssClass="txtUppercase tb8 xxs">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RFVddlEnteGestioneFondo" runat="server" ErrorMessage="Codice Gestione : campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="ddlEnteGestioneFondo" ValidationGroup="UCGvMiglioramentiContrattuali"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:LinkButton ID="btnAggiungiQuote" CommandName="Aggiungi" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Decorrenza" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="17%" Visible="true">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDecorrenzaQuota_item" CssClass="txtUppercase"></asp:Label>
                                    <asp:Label runat="server" ID="lblValueDecorrenzaQuota" CssClass="txtUppercase" Visible="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtDecorrenzaQuota" Text=' <%# Bind("DataDecorrenza", "{0:dd/MM/yyyy}")%>'
                                        CssClass="tb8 txtUppercase date-picker-base dateGGmmAAAA" MaxLength="10"></asp:TextBox>
                                    <asp:CustomValidator runat="server" ControlToValidate="txtDecorrenzaQuota" Display="Dynamic"
                                        ErrorMessage="La Decorrenza Quota inserita non è corretta" Text="*" CssClass="field-is-required" ValidationGroup="UCGvMiglioramentiContrattuali"
                                        ID="customCheckDecorrenzaQuota" ClientValidationFunction="checkCorrettezzaData" />
                                </EditItemTemplate>
                            </asp:TemplateField>                           
                            <asp:TemplateField HeaderText="Importo" HeaderStyle-CssClass="intestazioneTabella Row1"
                                ItemStyle-CssClass="TblRecordset3" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblImportoQuota" Width="100px"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox Style="text-align: left" runat="server" ID="txtImportoQuota" Width="140px"
                                        CssClass="txtUppercase tb8 " MaxLength="16" Text=' <%# Bind("Quota")%>' />
                                    <asp:RegularExpressionValidator runat="server" ID="REVtxtImportoQuota" ControlToValidate="txtImportoQuota"
                                        Display="Dynamic" ErrorMessage="Importo Quota: inserire l'importo in formato valido (max 8 interi e 7 decimali)"
                                        Text="*" CssClass="field-is-required" ValidationExpression="\d{1,8}(,\d{1,7})?" ValidationGroup="UCGvMiglioramentiContrattuali" />
                                    <asp:RequiredFieldValidator ID="RFVtxtImportoQuota" runat="server" ErrorMessage="Importo Quota: Campo obbligatorio"
                                        Display="Dynamic" Text="*" CssClass="field-is-required" ControlToValidate="txtImportoQuota" ValidationGroup="UCGvMiglioramentiContrattuali"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="2%" ShowDeleteButton="true" />
                            <asp:TemplateField HeaderStyle-CssClass="intestazioneTabella Row1" ItemStyle-CssClass="TblRecordset3"
                                ItemStyle-Width="4%" Visible="false">
                                <ItemTemplate>
                                    <asp:Image ID="imgVisualizzaTrattenute" alt="Visualizza dati trattenute" title="Visualizza dati trattenute"
                                        Style="cursor: pointer" src="../App_Themes/<%= Page.Theme %>/Images/plus.png" runat="server" />
                                    <asp:HiddenField ID="hdnVisualizzaTrattenute" runat="server" />
                                    </td></tr><tr style="display: none">
                                        <td>
                                            <table width="100%">
                                                <td style="width: 22%">
                                                    <label style="font-weight: bold">
                                                        Trattenute:</label>
                                                </td>
                                                <td style="margin: 15px auto;">
                                                   
                                                </td>
                                            </table>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <br />
       
    </div>
    <div style="margin-top: 25px;">
        <table width="100%">
            <tr>
                <td style="text-align: right">
                    <asp:Button ID="btnSalvaMiglioramentiContrattuali" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="true" Text="Salva MiglioramentiContrattuali" Width="200px" OnClientClick="BlockUI()"
                        OnClick="btnSalvaMiglioramentiContrattuali_Click" CssClass="primary" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="btnEliminaMiglioramentiContrattuali" Style="text-align: center; padding-left: 0px;
                        padding-right: 0px" runat="server" SkinID="btnAzione1" CausesValidation="false"
                        Enabled="false" Text="Elimina MiglioramentiContrattuali" Width="200px" OnClientClick="if(!window.confirm('Sei sicuro di voler eliminare i Dati Calcolo?')) return false; else BlockUI();"
                        OnClick="btnEliminaMiglioramentiContrattuali_Click" CssClass="ghost-delete" />
                </td>
            </tr>
        </table>
    </div>
</asp:Panel>
