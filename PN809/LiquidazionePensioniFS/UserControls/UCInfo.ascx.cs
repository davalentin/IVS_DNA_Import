using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;

using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Drawing;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;
using System.Linq;
using System.Collections.Generic;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls
{
    public partial class UCInfo : CustomBaseUserControl
    {
        #region InfoLiquidazione
        public InfoLiquidazione InfoLiquidazione { get; set; }
        #endregion InfoLiquidazione

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Debug.WriteLine("Infoliquidazione Stato Pannello: " + StatoPannello.Value);
            }
        }

        internal void BindData()
        {
            if (InfoLiquidazione != null)
            {
                CodeUtility valuesDecodifica = new CodeUtility();
                AreaDecodifica valoriDecodificati = valuesDecodifica.GetValuesDecodifica();
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                //ENG - Implementazione Meta Processo
                string controlloDinamicoSbloccaMetaProcesso = string.Empty;
                if (ViewState["SbloccaMetaProcesso"] != null)
                    controlloDinamicoSbloccaMetaProcesso = (string)ViewState["SbloccaMetaProcesso"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out controlloDinamicoSbloccaMetaProcesso);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["SbloccaMetaProcesso"] = controlloDinamicoSbloccaMetaProcesso;
                }

                if (datiPensione != null)
                    lblDomanda.Text = InfoLiquidazione.Domanda + " - " + datiPensione.Prodotto;
                else
                    lblDomanda.Text = InfoLiquidazione.Domanda;

                lblGestioneOld.Text = valueOrPlaceholder(datiPensione.Gestione);
                lblFondoOld.Text = valueOrPlaceholder(datiPensione.Fondo);
                lblEnteOld.Text = valueOrPlaceholder(datiPensione.Ente);
                lblFiltroOld.Text = valueOrPlaceholder(datiPensione.Filtro);
                lblCategoriaOld.Text = valueOrPlaceholder(InfoLiquidazione.Categoria);

                lblGestione.Text = valueOrPlaceholder(datiPensione.Gestione);
                lblFondo.Text = valueOrPlaceholder(datiPensione.Fondo);
                lblEnte.Text = valueOrPlaceholder(datiPensione.Ente);
                lblFiltro.Text = valueOrPlaceholder(datiPensione.Filtro);
                lblCategoria.Text = valueOrPlaceholder(InfoLiquidazione.Categoria);

                if (valoriDecodificati.ElencoCategoriePensione != null)
                {
                    List<AreaDecodifica.DatiCategoriaPensione> listaCategoriePensione = valoriDecodificati.ElencoCategoriePensione.ToList();
                    var codiceCategoria = listaCategoriePensione.Where(x => x.Sigla.Trim() == domanda.Categoria.Trim()).Select(x => x.Codice).FirstOrDefault();
                    codiceCategoria = !string.IsNullOrEmpty(codiceCategoria) ? codiceCategoria.Substring(1) : string.Empty;
                    hCodiceCategoria.Value = codiceCategoria;
                }
                hTipoAutomazione.Value = datiPensione.TipoAutomazione == null ? string.Empty : datiPensione.TipoAutomazione.ToString();
                hGruppo.Value = domanda.CodGruppo;
                hProdotto.Value = domanda.CodProdotto;
                hTipo.Value = domanda.CodTipo;
                hGestione.Value = datiPensione.CodeGestione;
                hFondo.Value = datiPensione.CodeFondo;
                hEnte.Value = datiPensione.Ente;
                hCodiceFase.Value = domanda.CodFase;
                hDecorrenzaPensione.Value = datiPensione.DecorrenzaOriginaria == null ? string.Empty : datiPensione.DecorrenzaOriginaria.ToString();


                UtilityTipoAppartenenza tipoApp = (UtilityTipoAppartenenza)Utility.GetTipoAppartenenzaRuolo((Ruoli)Session["Ruolo"]);
                DateTime? dataSistema = null;
                GetDataSistema(tipoApp, out dataSistema);

                //FG - Per le domande di APE Sociale e di APE Precoci si richiede di mostrare nel campo “Sede” della testata il valore della sede di destinazione e non più quello della domanda.
                string sede = "";
                string sedeAttuale = "";
                if (tipoApp == UtilityTipoAppartenenza.AGO && datiPensione.CodiceSedeDestinazione.HasValue)
                    sede = datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') + " - " +
                        (datiPensione.CentroOperativoDestinazione.HasValue ? datiPensione.CentroOperativoDestinazione.Value.ToString().PadLeft(2, '0') : "00");
                else
                    sede = InfoLiquidazione.Sede + " - " + datiPensione.CentroOperativo.ToString().PadLeft(2, '0');

                if (Utility.isPensioneOvunqueAttiva(tipoApp) && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, domanda.IsDomandaRiapertura))
                {
                    idTdLabelSedeAttuale.Visible = true;
                    idTdValoreSedeAttuale.Visible = true;
                    hrSedeAttuale.Visible = true;

                    if (datiPensione.CodiceSedeDestinazione.HasValue && datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') != datiPensione.CodiceSede.PadLeft(4, '0'))
                    {
                        sede = datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0');

                        if (ViewState["SbloccaMetaProcesso"] != null && ViewState["SbloccaMetaProcesso"].ToString() == "SI" && !String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6) && !String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6.Trim()))
                        {
                            sedeAttuale = datiPensione.CodiceSedeGP1ALZ6.PadLeft(4, '0') + "-" + datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault().ToString().PadLeft(2, '0');
                        }
                        else
                            sedeAttuale = datiPensione.CodiceSede.PadLeft(4, '0');
                    }
                    else
                    {
                        sede = datiPensione.CodiceSede.PadLeft(4, '0');

                        if (ViewState["SbloccaMetaProcesso"] != null && ViewState["SbloccaMetaProcesso"].ToString() == "SI" && !String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6) && !String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6.Trim()))
                        {
                            sedeAttuale = datiPensione.CodiceSedeGP1ALZ6.PadLeft(4, '0') + "-" + datiPensione.CentroOperativoGP1ALZ6.GetValueOrDefault().ToString().PadLeft(2, '0');
                        }
                        else
                            sedeAttuale = (!String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6) && !String.IsNullOrEmpty(datiPensione.CodiceSedeGP1ALZ6.Trim())) ? datiPensione.CodiceSedeGP1ALZ6.PadLeft(4, '0') : datiPensione.CodiceSede.PadLeft(4, '0');
                    }

                    lblSedeAttuale.Text = sedeAttuale;
                    lblSedeAttualeOld.Text = sedeAttuale;
                }
                else
                {
                    lblSedeAttuale.Text = "-";
                    idTdLabelSedeAttuale.Visible = false;
                    idTdValoreSedeAttuale.Visible = false;
                    hrSedeAttuale.Visible = false;
                }

                lblSedeOld.Text = valueOrPlaceholder(sede);
                lblCertificatoOld.Text = InfoLiquidazione.Certificato != null ? InfoLiquidazione.Certificato.PadLeft(8, '0') : "-";
                lblTipoOld.Text = valueOrPlaceholder(InfoLiquidazione.Tipo);

                lblSede.Text = valueOrPlaceholder(sede);
                lblCertificato.Text = InfoLiquidazione.Certificato != null ? InfoLiquidazione.Certificato.PadLeft(8, '0') : "-";
                lblTipo.Text = valueOrPlaceholder(InfoLiquidazione.Tipo);

                //ENG - memo 06_2024   
                string controlloDinamicoAbilitazioneMemo06_2024 = string.Empty;
                if (ViewState["AbilitazioneMemo06_2024"] != null)
                    controlloDinamicoAbilitazioneMemo06_2024 = (string)ViewState["AbilitazioneMemo06_2024"];
                else
                {
                    Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                    Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo06_2024", out controlloDinamicoAbilitazioneMemo06_2024);
                    if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                        ViewState["AbilitazioneMemo06_2024"] = controlloDinamicoAbilitazioneMemo06_2024;
                }
                if (!String.IsNullOrEmpty(controlloDinamicoAbilitazioneMemo06_2024) && controlloDinamicoAbilitazioneMemo06_2024.Trim().ToUpperInvariant() == "SI" &&
                    datiPensione.CodProPe != null && datiPensione.CodProPe.HasValue && datiPensione.CodProPe.Value == 8)
                {
                    lblUnicarpeOld.Text = "SB";
                    lblUnicarpe.Text = "SB";
                }
                else
                {
                    lblUnicarpeOld.Text = datiPensione.FlagUnicarpe.HasValue ? Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ? "SI" : "NO" : "-";
                    lblUnicarpe.Text = datiPensione.FlagUnicarpe.HasValue ? Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica ? "SI" : "NO" : "-";
                }

                lblCodiceFiscaleOld.Text = InfoLiquidazione.CodiceFiscale;
                lblNomeOld.Text = InfoLiquidazione.Nome;
                lblCognomeOld.Text = InfoLiquidazione.Cognome;
                lblStatoDomandaOld.Text = InfoLiquidazione.StatoDomanda;
                lblTipoDomandaOld.Text = datiPensione.Tipologia;
                lblTelematicaOld.Text = Session["URLDPI"] != null ? "SI" : "NO";

                lblCodiceFiscale.Text = valueOrPlaceholder(InfoLiquidazione.CodiceFiscale);
                lblNome.Text = valueOrPlaceholder(InfoLiquidazione.Nome);
                lblCognome.Text = valueOrPlaceholder(InfoLiquidazione.Cognome);
                lblStatoDomanda.Text = valueOrPlaceholder(InfoLiquidazione.StatoDomanda);
                lblTipoDomanda.Text = valueOrPlaceholder(datiPensione.Tipologia);
                lblTelematica.Text = Session["URLDPI"] != null ? "SI" : "NO";

                if (domanda.IsDomandaINPDAP)
                {
                    lblUnicarpe.Text = "-";
                    tdLblUnicarpe.Visible = false;
                    tdLblFonte.Visible = true;
                    tdValueUnicarpe.Visible = false;
                    tdValueFonte.Visible = true;

                    Utility.TipoFelpe? tipoFelpe = (Utility.TipoFelpe?)datiPensione.TipoFelpe;
                    switch (tipoFelpe)
                    {
                        case Utility.TipoFelpe.AMG:
                            lblFonteOld.Text = "Unicarpe";
                            lblFonte.Text = "Unicarpe";
                            break;
                        case Utility.TipoFelpe.SIN:
                            lblFonteOld.Text = "SIN";
                            lblFonte.Text = "SIN";
                            break;
                        case Utility.TipoFelpe.SPI:
                            lblFonteOld.Text = "ASI";
                            lblFonte.Text = "ASI";
                            break;
                        default:
                            lblFonteOld.Text = "-";
                            lblFonte.Text = "-";
                            break;
                    }
                } else
                {
                    lblFonte.Text = "-";
                }

                #region Ridimensionamento
                // Ridimensionamento della stringa di intestazione
                try
                {
                    int result;
                    Resizer(16, out result);
                    linkDomanda.Style.Add(HtmlTextWriterStyle.FontSize, result.ToString() + "px");
                    lblDomanda.Font.Size = new FontUnit(result.ToString() + "px");
                }
                catch (Exception)
                {
                    // Eccezione ignorata
                }
                #endregion Ridimensionamento

                #region SCRIPE
                CodeUtility.SetScripeSession(this, InfoLiquidazione.CodiceFiscale, InfoLiquidazione.Domanda);
                #endregion
            }
        }

        private void Resizer(int size, out int result)
        {
            Font f = new Font(lblDomanda.Font.Name, size, GraphicsUnit.Pixel);
            Bitmap bp = new Bitmap(1, 1);
            Graphics gp = Graphics.FromImage(bp);
            int width = (int)gp.MeasureString("Domanda: " + lblDomanda.Text, f).Width;

            if (width > 598)
                Resizer(--size, out result);
            else
                result = size;
        }

        private string valueOrPlaceholder(string value)
        {
            string trimmedValue = value.Trim();
            string finalValue = string.IsNullOrEmpty(trimmedValue) ? "-" : trimmedValue;
            return finalValue;
        }
    }
}