using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;

namespace INPS.Pensioni.LiquidazionePensione.View.Web.UserControls.Oneri
{
    public partial class UCOneri : CustomBaseUserControl, IOneri, ITitolarePensione
    {

        #region IViewUI
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        #endregion IViewUI

        #region IAreaOneri
        public Presenter.SvrLiquidazione.AreaOneri areaOneri { get; set; }
        #endregion IAreaOneri

        #region ITitolare
        public AreaTitolare TitolarePensione { get; set; }
        public Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda domanda { get; set; }
        #endregion ITitolare

        List<DatiOneriBenefParticolari.DatiOneri> elencoOneriViewState = new List<DatiOneriBenefParticolari.DatiOneri>();
        List<DatiOneriBenefParticolari.DatiBeneficiParticolari> elencoBeneficiViewState = new List<DatiOneriBenefParticolari.DatiBeneficiParticolari>();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        internal void ValorizzaEtichetteOneri(IOneri iOneri)
        {
            ViewState["oneri"] = iOneri.areaOneri;

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            //ENG - Memo 121_2023
            string valoreControlloMemo121_2023 = string.Empty;
            Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
            Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("Abilitazione_Memo_121_2023", out valoreControlloMemo121_2023);
            if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                ViewState["Abilitazione_Memo_121_2023"] = valoreControlloMemo121_2023;

            //ENG - Eliminazione Scarto Oneri
            string valoreControlloEliminazioneScartoOneri0031_0105_0112 = string.Empty;
            if (ViewState["EliminazioneScartoOneri0031_0105_0112"] != null)
                valoreControlloEliminazioneScartoOneri0031_0105_0112 = (string)ViewState["EliminazioneScartoOneri0031_0105_0112"];
            else
            {
                Presenter.PresenterControlliDinamici presenterControlliDinamici = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esitoCaricamento = presenterControlliDinamici.GetControlloDinamicoByNomeControllo("EliminazioneScartoOneri0031_0105_0112", out valoreControlloEliminazioneScartoOneri0031_0105_0112);
                if (esitoCaricamento != null && esitoCaricamento.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK
                    && !String.IsNullOrEmpty(valoreControlloEliminazioneScartoOneri0031_0105_0112) && !String.IsNullOrEmpty(valoreControlloEliminazioneScartoOneri0031_0105_0112.Trim()))
                    ViewState["EliminazioneScartoOneri0031_0105_0112"] = valoreControlloEliminazioneScartoOneri0031_0105_0112.Trim();
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024 = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null)
                controlloDinamicoMemo123_2024 = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024", out controlloDinamicoMemo123_2024);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024"] = controlloDinamicoMemo123_2024;
            }

            //ENG - Memo 123/2024 
            string controlloDinamicoMemo123_2024OpzioneContrib = string.Empty;
            if (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null)
                controlloDinamicoMemo123_2024OpzioneContrib = (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"];
            else
            {
                Presenter.PresenterControlliDinamici pres = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esit = pres.GetControlloDinamicoByNomeControllo("AbilitazioneRIC_TRFMemo123_2024OpzioneContrib", out controlloDinamicoMemo123_2024OpzioneContrib);
                if (esit != null && esit.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] = controlloDinamicoMemo123_2024OpzioneContrib;
            }

            //Precoci - ScadenzaBeneficio
            if (datiPensione.IsDomandaAPEPrecociOrRicostituzione)
            {
                lblCessPrecoci.Visible = true;
                lblCessBeneficioPrecoci.Visible = true;
            }
            if (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
               (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
               (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((!String.IsNullOrEmpty(controlloDinamicoMemo123_2024) && controlloDinamicoMemo123_2024.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
               (!String.IsNullOrEmpty(controlloDinamicoMemo123_2024OpzioneContrib) && controlloDinamicoMemo123_2024OpzioneContrib.Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) || 
                datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
            {
                lblCessQuota100.Visible = true;
                if (datiPensione.IsDomandaQuota100OrRicostituzione && (ViewState["Abilitazione_Memo_121_2023"] == null || ViewState["Abilitazione_Memo_121_2023"].ToString().Trim().ToUpperInvariant() == "NO"))
                    lblCessBeneficioQuota100.Visible = true;

                if (datiPensione.IsDomandaQuota102OrRicostituzione)
                {
                    lblOneriSperDonna.Visible = true;
                    if ((ViewState["Abilitazione_Memo_121_2023"] == null || ViewState["Abilitazione_Memo_121_2023"].ToString().Trim().ToUpperInvariant() == "NO"))
                    {
                        lblCessBeneficioQuota102.Visible = true;
                    }
                }
            }
            if (iOneri.areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292)
                lblOpzDonna2023.Visible = true;

            if (iOneri.areaOneri != null)
            {
                if (iOneri.areaOneri.IsOneriSperDonnaObbligatori || iOneri.areaOneri.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione))
                    lblOneriSperDonna.Visible = true;
                if (iOneri.areaOneri.DatiOneriBenefParticolari != null)
                {
                    if (iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri != null)
                    {
                        ViewState["ElencoGruppo"] = iOneri.areaOneri.ListaGruppoOneri.ToList();
                        List<DatiOneriBenefParticolari.DatiOneri> elencoOneri = iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri.ToList();
                        if (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                        {
                            if (elencoOneri.Count(x => x.IdCodeGruppo.HasValue && x.IdCodeSottoGruppo.HasValue) >= 1)
                                elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());
                        }
                        else
                            //ENG - Integrazione Modifiche Accenture
                            if (elencoOneri.Any(x => x.IdCodeGruppo == GetIdGruppoFromValue("4400 ")) && elencoOneri.Count() <= 1 && !Utility.IsDomandaRipristino(datiPensione))
                            elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());
                        ViewState["ElencoOneri"] = elencoOneri;
                        List<CodiciOneriSottoGruppoOneri> elencoSottoGruppo = iOneri.areaOneri.ListaSottoGruppoOneri.ToList();
                        if (elencoSottoGruppo != null && elencoSottoGruppo.Count > 0 && elencoSottoGruppo.Any(x => x.Code == "0901") && (iOneri.areaOneri.IsPrepensionamentoEditoriaArt1c500L160_2019 ||
                            iOneri.areaOneri.IsPrepensionamentoEditoria))
                            elencoSottoGruppo.First(x => x.Code == "0901").Descrizione = "Prep art. 37, comma 1 lett.a), L.416/1981";
                        ViewState["ElencoSottoGruppo"] = elencoSottoGruppo;
                        gvOneri.DataSource = ViewState["ElencoOneri"];
                        if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica &&
                            iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri.Length > 0)
                        {
                            if ((!iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo.HasValue ||
                                iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo.Value == 0 ||
                                !iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].Scadenza.HasValue))
                                gvOneri.EditIndex = 0;

                            if (iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeGruppo.HasValue &&
                                iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo.HasValue &&
                                iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeGruppo == GetIdGruppoFromValue("2000 ") &&
                                iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri[0].IdCodeSottoGruppo == GetIdSottoGruppoFromValue("2010 "))
                                gvOneri.EditIndex = -1;

                            //FG - Controllo che ci sia almeno un onere di tipo prepensionamentoEditoria e nel caso visualizzo la label a video
                            foreach (DatiOneriBenefParticolari.DatiOneri on in iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri)
                            {
                                if (on.IdCodeGruppo.HasValue && on.IdCodeGruppo == GetIdGruppoFromValue("0900 "))
                                {
                                    lblEditoria.Visible = true;
                                    if (iOneri.areaOneri.IsPrepensionamentoEditoriaLetteraB)
                                        lblEditoria.Text = "La cessazione beneficio deve corrispondere alla decorrenza della pensione di vecchiaia secondo le norme vigenti.";
                                    break;
                                }
                            }
                        }
                        else if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                            iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri.Length > 0)
                        {
                            int index = iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri.ToList().FindIndex(x => x.IdCodeGruppo == GetIdGruppoFromValue("4700 ") && (x.IdCodeSottoGruppo == GetIdSottoGruppoFromValue("4701 ") || x.IdCodeSottoGruppo == GetIdSottoGruppoFromValue("4702 ")) && !x.Scadenza.HasValue);
                            gvOneri.EditIndex = index;
                        }
                    }
                    else
                    {
                        gvOneri.DataSource = null;
                    }

                    if (iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari != null)
                    {
                        ViewState["ElencoBenefici"] = iOneri.areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari.ToList();
                        gvBenefici.DataSource = ViewState["ElencoBenefici"];

                        if (this.domanda == null)
                            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                        if (Utility.IsDomandaVecchiaiaENAV(datiPensione, this.domanda.Categoria))
                            gvBenefici.Columns[1].Visible = false;
                    }

                    else
                    {
                        if (this.domanda == null)
                            this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                        if (Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica && CodeUtility.IsRicostituzione(datiPensione) && !this.domanda.IsDomandaENPALS &&
                            iOneri.areaOneri.DatiOneriBenefParticolariStorico != null && iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiBeneficiParticolari != null)
                        {
                            ViewState["ElencoBenefici"] = iOneri.areaOneri.DatiOneriBenefParticolariStorico.ListaDatiBeneficiParticolari.ToList();
                            gvBenefici.DataSource = ViewState["ElencoBenefici"];
                        }
                        else
                            gvBenefici.DataSource = null;
                    }
                }
            }

            gvOneri.DataBind();
            gvOneri.Visible = true;

            gvBenefici.DataBind();
            gvBenefici.Visible = true;
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];
            if (iOneri != null && iOneri.areaOneri != null && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && iOneri.areaOneri.IsBeneficioVittimeTerrorismo)
            {
                gvBenefici.Enabled = false;
            }

            if (iOneri != null && iOneri.areaOneri != null && ((iOneri.areaOneri.IsPrepensionamentoEditoriaLetteraB && iOneri.areaOneri.IsOneriPresentiDaAzienda) || iOneri.areaOneri.IsRicVOPGIMigrataFiltroEBA))
            {
                List<CodiciOneriSottoGruppoOneri> elencoSottoGruppo = iOneri.areaOneri.ListaSottoGruppoOneri.ToList();
                if (elencoSottoGruppo != null && elencoSottoGruppo.Count > 0 && elencoSottoGruppo.Any(x => x.Code == "0908"))
                    gvOneri.Rows[1].Visible = false;
            }
        }

        internal DatiOneriBenefParticolari GetValoriOneri()
        {
            this.areaOneri = (AreaOneri)ViewState["oneri"];
            List<DatiOneriBenefParticolari.DatiOneri> listDatiOneri = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];
            List<DatiOneriBenefParticolari.DatiBeneficiParticolari> listDatiBenefici = (List<DatiOneriBenefParticolari.DatiBeneficiParticolari>)ViewState["ElencoBenefici"];

            removeItemBlankOneri(ref listDatiOneri);
            removeItemBlankBenefici(ref listDatiBenefici);

            if (this.areaOneri == null)
                this.areaOneri = new AreaOneri();

            if (this.areaOneri.DatiOneriBenefParticolari == null)
                this.areaOneri.DatiOneriBenefParticolari = new DatiOneriBenefParticolari();

            if (listDatiOneri != null && listDatiOneri.Count() > 0)
            {
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri = new DatiOneriBenefParticolari.DatiOneri[listDatiOneri.Count()];
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri = listDatiOneri.ToArray();
            }
            else
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiOneri = null;

            if (listDatiBenefici != null && listDatiBenefici.Count() > 0)
            {
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = new DatiOneriBenefParticolari.DatiBeneficiParticolari[listDatiBenefici.Count()];
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = listDatiBenefici.ToArray();
            }
            else
                this.areaOneri.DatiOneriBenefParticolari.ListaDatiBeneficiParticolari = null;


            return this.areaOneri.DatiOneriBenefParticolari;
        }

        protected void btnSalvaDatiOneri_Click(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (Presenter.SvrLiquidazione.AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            this.areaOneri = new Presenter.SvrLiquidazione.AreaOneri();

            this.areaOneri.DatiOneriBenefParticolari = new DatiOneriBenefParticolari();
            this.areaOneri.DatiOneriBenefParticolari = GetValoriOneri();

            PresenterOneri presenterMaggiorazioneBenefici = new PresenterOneri();
            presenterMaggiorazioneBenefici.SalvaOneriBeneficiParticolari(this);

            RaiseShowAvviso(this, null);
        }


        protected void RaiseShowAvviso(object sender, EventArgs e)
        {
            ShowAvviso(sender, e);
        }

        protected void RaiseAbilitaTastoSalva(object sender, EventArgs e)
        {
            if (AbilitaTastoSalva != null)
                AbilitaTastoSalva(sender, e);
        }

        protected void RaiseDisabilitaTastoSalva(object sender, EventArgs e)
        {
            if (DisabilitaTastoSalva != null)
                DisabilitaTastoSalva(sender, e);
        }


        protected void RaiseSalvaOnere(object sender, EventArgs e)
        {
            if (SalvaOnere != null)
                SalvaOnere(sender, e);
        }

        protected void RaiseAnnullaOnere(object sender, EventArgs e)
        {
            if (AnnullaOnere != null)
                AnnullaOnere(sender, e);
        }

        #region GridView Oneri

        protected void gvOneri_Load(object sender, EventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);

            List<DatiOneriBenefParticolari.DatiOneri> listaOneri = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];

            if (this.areaOneri == null)
                this.areaOneri = (AreaOneri)ViewState["oneri"];

            string valoreControllo = string.Empty;
            if (ViewState["AbilitazioneMemo123_2021"] != null)
                valoreControllo = (string)ViewState["AbilitazioneMemo123_2021"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("AbilitazioneMemo123_2021", out valoreControllo);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["AbilitazioneMemo123_2021"] = valoreControllo;
            }

            //ENG - Memo 121_2023
            string valoreControlloMemo121_2023 = string.Empty;
            if (ViewState["Abilitazione_Memo_121_2023"] != null)
                valoreControlloMemo121_2023 = (string)ViewState["Abilitazione_Memo_121_2023"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("Abilitazione_Memo_121_2023", out valoreControlloMemo121_2023);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["Abilitazione_Memo_121_2023"] = valoreControlloMemo121_2023;
            }

            //ENG - Appunto 13_2023 OpzioneDonna2023_2024
            string valoreControlloOpzioneDonna2023_2024 = string.Empty;
            if (ViewState["Abilitazione_OpzioneDonna2023_2024"] != null)
                valoreControlloOpzioneDonna2023_2024 = (string)ViewState["Abilitazione_OpzioneDonna2023_2024"];
            else
            {
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                Presenter.SvrLiquidazione.AreaEsito esito = presenter.GetControlloDinamicoByNomeControllo("OpzioneDonna2023_2024", out valoreControlloOpzioneDonna2023_2024);
                if (esito != null && esito.RisultatoOperazione == Presenter.SvrLiquidazione.AreaEsito.TipoEsito.OK)
                    ViewState["Abilitazione_OpzioneDonna2023_2024"] = valoreControlloOpzioneDonna2023_2024;
            }

            // Se la domanda è APE Precoci oppure
            // Se la domanda è APE Sociale oppure
            // Se la domanda è Vittime Terrorismo o se non sono presenti oneri diversi da gruppo 4700 sottogruppo 4701 oppure da gruppo 0900
            // allora non mostro le colonne Settimane e Onere
            if (datiPensione.IsDomandaAPEPrecociOrRicostituzione || Utility.IsDomandaAPESociale(this.domanda.Categoria) ||
                CodeUtility.IsDomandaVittimeTerrorismo(datiPensione) || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || this.areaOneri.IsDomandaSperimentaleDonna_DL_4_2019OrRicostituzione ||
                this.areaOneri.IsDomandaAnzianitaPerLeggeBilancio2019OrRicostituzione || Utility.IsDomandaAnzianitaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaVecchiaiaMaggiorazioneAmiantoLegge208_2015(datiPensione) || Utility.IsDomandaRiliquidazioneAnzianitaAnticipataConFinestraDecorrenza(datiPensione) ||
                Utility.IsDomandaAnticipataConOpzionePL(datiPensione) || (!string.IsNullOrEmpty(valoreControllo) && valoreControllo == "SI" && Utility.IsDomandaAUTAnticipataInComputo(datiPensione, this.domanda.Categoria, false)) ||
                (listaOneri != null && listaOneri.Count(x => x.IdCodeGruppo != GetIdGruppoFromValue("4700 ") && x.IdCodeSottoGruppo != GetIdSottoGruppoFromValue("4701 ") && x.IdCodeSottoGruppo != GetIdSottoGruppoFromValue("4702 ") && x.IdCodeGruppo != GetIdGruppoFromValue("0900 ")) == 0) || this.areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione ||
                (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
            {
                gvOneri.Columns[(int)gvOneri_Colonne.Settimane].Visible = false;
                gvOneri.Columns[(int)gvOneri_Colonne.Onere].Visible = false;
            }
            //Precoci e Quota 100 - ScadenzaBeneficio
            //ENG - Memo 121_2023
            if (datiPensione.IsDomandaAPEPrecociOrRicostituzione ||
                ((String.IsNullOrEmpty(valoreControlloMemo121_2023) || valoreControlloMemo121_2023.Trim().ToUpperInvariant() == "NO") && (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)) &&
                (!(!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) &&
                (!(CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) &&
                !datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                gvOneri.Columns[(int)gvOneri_Colonne.CessBenIncumul].Visible = true;
            else
                gvOneri.Columns[(int)gvOneri_Colonne.CessBenIncumul].Visible = false;
        }

        protected void gvOneri_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (this.domanda == null)
                        this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

                    List<DatiOneriBenefParticolari.DatiOneri> listaOneri = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];
                    if (this.areaOneri == null)
                        this.areaOneri = (AreaOneri)ViewState["oneri"];
                    List<DatiOneriBenefParticolari.DatiOneri> elencoOneriStorico = null;
                    if (areaOneri.DatiOneriBenefParticolariStorico != null && areaOneri.DatiOneriBenefParticolariStorico.ListaDatiOneri != null)
                        elencoOneriStorico = areaOneri.DatiOneriBenefParticolariStorico.ListaDatiOneri.ToList();

                    // record non editabile, per unicarpe e non è sper donna senza scadenza
                    AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
                    if ((Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Automatica &&
                        !(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo == GetIdGruppoFromValue("4700 ") && !((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza.HasValue)) ||
                        (((Utility.IsDomandaAPESociale(this.domanda.Categoria) && CodeUtility.IsRicostituzione(datiPensione)) ||
                        (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) &&
                        ((datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.FS &&
                        !((datiPensione.IdTipoPLPerRIC == 16 || datiPensione.IdTipoPLPerRIC == 18 || datiPensione.IdTipoPLPerRIC == 20) && datiPensione.CodeProdotto == "0107") &&
                        !((datiPensione.IdTipoPLPerRIC == 15 || datiPensione.IdTipoPLPerRIC == 17 || datiPensione.IdTipoPLPerRIC == 19) && datiPensione.CodeProdotto == "0107" && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica) &&
                        !((datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione) && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica) &&
                        !(((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                        (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) && Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)) ||
                        datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaInabilitaAmiantoOrRicostituzione || (datiPensione.TipoAppartenenzaDomanda != AreaTitolare.DatiPensione.TipoAppDomanda.AGO &&
                        (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || ((datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione) && !(Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)) ||
                        (((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                        (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) && !(Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)))))) ||
                        datiPensione.IsDomandaCumuloAutomatica) && ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza.HasValue) ||
                        (CodeUtility.IsRicostituzione(datiPensione) && this.areaOneri.IsBeneficioVittimeTerrorismo && elencoOneriStorico != null && elencoOneriStorico.Exists(x => x.IdCodeSottoGruppo == ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo)
                        && !(domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))) ||
                        (CodeUtility.IsRicostituzione(datiPensione) && this.areaOneri.IsPrepensionamentoEditoriaArt1c500L160_2019) ||
                        (CodeUtility.IsRicostituzione(datiPensione) && this.areaOneri.IsPrepensionamentoEditoria && ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza.HasValue) ||
                        (CodeUtility.IsRicostituzione(datiPensione) && this.areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione)
                        || (ViewState["Abilitazione_Memo_121_2023"] != null && ViewState["Abilitazione_Memo_121_2023"].ToString().Trim().ToUpperInvariant() == "SI" && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione))
                        || (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                        (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                        (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione || datiPensione.IsDomandaVOAUTAnticipataTipoContributivoFiltroGSEOrRicostituzione)))
                    {
                        gvOneri.EditIndex = -1;
                        LinkButton button = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        button.Enabled = false;
                        button.Text = "&nbsp;&nbsp;&nbsp;";
                        ((Label)e.Row.FindControl("lblGruppo")).Text = GetValueGruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                        ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                        if ((this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                            this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)) || this.domanda.IsDomandaINPDAP)
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        ((Label)e.Row.FindControl("lblOnere")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Onere.ToString();
                        //Precoci - ScadenzaBeneficio
                        if (datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                            || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                            (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                            (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                            datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                            ((Label)e.Row.FindControl("lblCessIncumul")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).ScadenzaBeneficio);
                    }
                    // record editabile
                    else if (e.Row.Cells[0].Controls.Count == 3)
                    {
                        LinkButton cancel = ((LinkButton)(e.Row.Cells[0].Controls[2]));
                        cancel.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/cancel24.png />";
                        cancel.ToolTip = "Annulla";

                        LinkButton save = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        save.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/save24.png />";
                        save.ToolTip = "Salva";
                        save.CausesValidation = true;
                        save.ValidationGroup = "UCTabOneri";
                        save.CommandName = "Salva";

                        DropDownList ddlSG = new DropDownList();
                        ddlSG = (DropDownList)e.Row.FindControl("ddlSottoGruppo");
                        LoadDdlSottoGruppo(ddlSG, ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeGruppo, datiPensione, (DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem);

                        if (((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo == GetIdGruppoFromValue("4400 ") && e.Row.DataItemIndex == 1)
                        {
                            ddlSG.Enabled = false;
                        }

                        ddlSG.SelectedValue = ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeSottoGruppo.HasValue ? ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeSottoGruppo.Value.ToString() : string.Empty;

                        if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                        {
                            DatiOneriBenefParticolari.DatiOneri onereCorrente = (DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem);
                            if (onereCorrente != null)
                            {
                                //Se l'onere arriva dal prelievo non devo modificare il gruppo
                                if (!onereCorrente.IsFromPrelievo)
                                {
                                    ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo = GetIdGruppoFromValue("4400 ");
                                }
                            }
                        }

                        ((Label)e.Row.FindControl("lblGruppo_Edit")).Text = GetValueGruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());

                        // Solo per la tipologia Vittime Terrorismo (4400) il sotto gruppo è editabile
                        // Anche per le APE Precoci (5000) il sotto gruppo è editabile
                        if (((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo != GetIdGruppoFromValue("4400 ") &&
                            !Utility.IsDomandaAPESociale(this.domanda.Categoria)
                            && !(datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
                        {
                            Label lblSottoGruppo = (Label)e.Row.FindControl("lblSottoGruppo");
                            lblSottoGruppo.Visible = true;
                            lblSottoGruppo.Text = GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                            ddlSG.Visible = false;
                        }

                        //ENG - Integrazione Modifiche Accenture
                        if (Utility.IsDomandaRipristino(datiPensione))
                        {
                            ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                            ((Label)e.Row.FindControl("lblSottoGruppo")).Visible = true;
                            ((DropDownList)e.Row.FindControl("ddlSottoGruppo")).Visible = false;
                            ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Enabled = false;
                            ((Panel)e.Row.FindControl("pnlTxtDecorrenza")).Enabled = false;
                        }

                        if ((this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                            this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)) || this.domanda.IsDomandaINPDAP)
                        {
                            ((Panel)e.Row.FindControl("pnlCessazioneFS_PT")).Visible = true;
                            ((Panel)e.Row.FindControl("pnlCessazione")).Visible = false;
                            ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            ((TextBox)e.Row.FindControl("txtCessazioneFS_PT")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        else
                        {
                            if (CodeUtility.IsDomandaVittimeTerrorismo(datiPensione) || this.areaOneri.IsBeneficioVittimeTerrorismo)
                            {
                                ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Visible = false;
                                ((Panel)e.Row.FindControl("pnlTxtDecorrenza")).Visible = true;

                                ((TextBox)e.Row.FindControl("txtDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            }
                            else
                            {

                                if (GetValueGruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString()).Contains("0900") &&
                                    GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString()).Contains("0906") &&
                                    ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza != datiPensione.DecorrenzaOriginaria)
                                    ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Text = String.Format("{0:MM/yyyy}", datiPensione.DecorrenzaOriginaria);
                                else
                                    ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            }
                            ((TextBox)e.Row.FindControl("txtCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        // Modifica inserita a seguito della mail del 17/07/2014 inviata da Nunzio con oggetto: RE: ReEng Pensioni - Oneri Salvaguardia 
                        //((TextBox)e.Row.FindControl("txtSettimane")).Text = ((DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        //((TextBox)e.Row.FindControl("txtOnere")).Text = ((DatiOneri)(e.Row.DataItem)).Onere.ToString();

                        //Precoci - ScadenzaBeneficio
                        if (datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione ||
                            (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                            (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                            (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                            datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                            ((TextBox)e.Row.FindControl("txtCessIncumul")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).ScadenzaBeneficio);

                        if (btnSalvaDatiOneri.Enabled == true)
                            btnSalvaDatiOneri.Enabled = false;
                        RaiseDisabilitaTastoSalva(this, null);

                        if (this.domanda.IsDomandaENPALS && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Visible = false;
                            ((Panel)e.Row.FindControl("pnlTxtDecorrenza")).Visible = true;
                            ((TextBox)e.Row.FindControl("txtDecorrenza")).Text = ((Label)e.Row.FindControl("lblDecorrenza_Edit")).Text;
                            ((Label)e.Row.FindControl("lblGruppo_Edit")).Visible = false;

                            DropDownList ddlG = new DropDownList();
                            ddlG = (DropDownList)e.Row.FindControl("ddlGruppo");
                            LoadDdlGruppo(ddlG, ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeGruppo);
                            ddlG.SelectedValue = ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeGruppo.HasValue ? ((DatiOneriBenefParticolari.DatiOneri)e.Row.DataItem).IdCodeGruppo.Value.ToString() : string.Empty;
                            ((DropDownList)e.Row.FindControl("ddlGruppo")).Visible = true;
                            ((Label)e.Row.FindControl("lblSottoGruppo")).Visible = false;
                            ((DropDownList)e.Row.FindControl("ddlSottoGruppo")).Visible = true;
                        }

                        if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || ((datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione) && !(Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica))
                            || (((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione)
                            || (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)
                            || datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione) && !(Utility.IsRicostituzione_MotiviContributivi(datiPensione) && Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica))))
                            ((TextBox)e.Row.FindControl("txtCessazione")).Enabled = false;

                        if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                        {
                            DatiOneriBenefParticolari.DatiOneri onereCorrente = (DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem);
                            //Se l'onere arriva dal prelievo, l'unico campo che deve risultare modificabile è la data cessazione
                            if (onereCorrente != null && onereCorrente.IsFromPrelievo)
                            {
                                if (e.Row.FindControl("ddlSottoGruppo") != null)
                                    ((DropDownList)e.Row.FindControl("ddlSottoGruppo")).Enabled = false;
                                if (e.Row.FindControl("txtDecorrenza") != null)
                                    ((TextBox)e.Row.FindControl("txtDecorrenza")).Enabled = false;
                                if (e.Row.FindControl("txtCessazione") != null)
                                    ((TextBox)e.Row.FindControl("txtCessazione")).Enabled = true;
                            }
                            else
                            {
                                if (e.Row.FindControl("ddlSottoGruppo") != null)
                                    ((DropDownList)e.Row.FindControl("ddlSottoGruppo")).Enabled = true;
                                if (e.Row.FindControl("txtDecorrenza") != null)
                                    ((TextBox)e.Row.FindControl("txtDecorrenza")).Enabled = true;
                                if (e.Row.FindControl("txtCessazione") != null)
                                    ((TextBox)e.Row.FindControl("txtCessazione")).Enabled = true;
                            }

                        }
                    }
                    //ENG - Integrazione Modifiche Accenture
                    else if ((e.Row.DataItemIndex == listaOneri.Count() - 1 && listaOneri.Count() > 1 && listaOneri.ElementAt(1).IdCodeGruppo == null && !Utility.IsDomandaRipristino(datiPensione))
                        || (e.Row.DataItemIndex == listaOneri.Count() - 1 && domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
                    {
                        LinkButton add = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                        add.Text = (string)"<img width=16 height=16 border=0 src=../App_themes/" + Page.Theme + "/Images/add24.png />";
                        add.ToolTip = "Aggiungi";
                        if (!(domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
                        {
                            add.CommandArgument = "1";
                        }

                    }
                    else // caso griglia consultazione
                    {
                        if (((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo != GetIdGruppoFromValue("2000 ") ||
                            ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo != GetIdSottoGruppoFromValue("2010 "))
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            int index = e.Row.DataItemIndex;
                            edit.Text = (string)"<img width=24 height=24 border=0 src=../App_themes/" + Page.Theme + "/Images/pencil.png />";
                            edit.ToolTip = "Modifica";
                        }
                        else
                        {
                            LinkButton edit = ((LinkButton)(e.Row.Cells[0].Controls[0]));
                            edit.Visible = false;
                        }

                        ((Label)e.Row.FindControl("lblGruppo")).Text = GetValueGruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeGruppo.ToString());
                        ((Label)e.Row.FindControl("lblSottoGruppo")).Text = GetValueSottogruppoFromId(((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo.ToString());
                        if ((this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                            this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)) || this.domanda.IsDomandaINPDAP)
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:dd/MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        else
                        {
                            ((Label)e.Row.FindControl("lblDecorrenza")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Decorrenza);
                            ((Label)e.Row.FindControl("lblCessazione")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Scadenza);
                        }
                        ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Settimane.ToString();
                        ((Label)e.Row.FindControl("lblOnere")).Text = ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).Onere.ToString();

                        //Precoci - ScadenzaBeneficio
                        if (datiPensione.IsDomandaAPEPrecociOrRicostituzione || datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                            || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione || (!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)) ||
                            (CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                            (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"].ToString().Trim().ToUpperInvariant() == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) ||
                            datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                            ((Label)e.Row.FindControl("lblCessIncumul")).Text = String.Format("{0:MM/yyyy}", ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).ScadenzaBeneficio);
                        if (this.areaOneri.IsBeneficioVittimeTerrorismo && ((elencoOneriStorico == null || (elencoOneriStorico != null && !elencoOneriStorico.Exists(x => x.IdCodeSottoGruppo == ((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IdCodeSottoGruppo)))
                            || (domanda.TipoAppartenenza == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione) && !((DatiOneriBenefParticolari.DatiOneri)(e.Row.DataItem)).IsFromPrelievo)))
                        {
                            LinkButton delete = ((LinkButton)(e.Row.FindControl("btnDelete")));
                            delete.Text = "<img width=20 height=20 border=0 src=../App_themes/" + Page.Theme + "/Images/delete24.png />";

                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowDataBound " + ex);
            }
        }

        private void LoadDdlSottoGruppo(DropDownList ddlSG, long? idCodeGruppo, AreaTitolare.DatiPensione datiPensione, DatiOneriBenefParticolari.DatiOneri onereCorrente)
        {
            try
            {
                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];
                if (listaSottoGruppoOneri != null)
                {
                    if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)
                        && !onereCorrente.IsFromPrelievo)
                    {
                        listaSottoGruppoOneri = listaSottoGruppoOneri.FindAll(x => x.IdOnere == 12);
                    }
                    else
                        if (!(this.domanda.IsDomandaENPALS && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                        listaSottoGruppoOneri = listaSottoGruppoOneri.FindAll(x => x.IdOnere == (idCodeGruppo.HasValue ? idCodeGruppo.Value : 0));

                    if (listaSottoGruppoOneri != null && listaSottoGruppoOneri.Count > 0)
                    {
                        foreach (CodiciOneriSottoGruppoOneri sG in listaSottoGruppoOneri)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", sG.Descrizione);
                            li.Text = sG.Code;
                            li.Value = sG.Id.ToString();
                            ddlSG.Items.Add(li);
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo LoadDdl " + ex);
            }

        }

        private void LoadDdlGruppo(DropDownList ddlG, long? idCodeGruppo)
        {
            try
            {
                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];
                if (listaGruppoOneri != null)
                {
                    if (listaGruppoOneri != null && listaGruppoOneri.Count > 0)
                    {
                        foreach (CodiciOneriGruppoOneri g in listaGruppoOneri)
                        {
                            ListItem li = new ListItem();
                            li.Attributes.Add("title", g.Descrizione);
                            li.Text = g.Code;
                            li.Value = g.Id.ToString();
                            ddlG.Items.Add(li);
                        }
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo LoadDdl " + ex);
            }

        }

        protected void gvOneri_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvOneri.PageIndex = e.NewPageIndex;
                GvOneri_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_onPageIndexChanging" + ex);
            }
        }

        protected void gvOneri_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                gvOneri.EditIndex = e.NewEditIndex;
                GvOneri_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowEditing " + ex);
            }
        }

        protected void gvOneri_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                List<DatiOneriBenefParticolari.DatiOneri> elencoStatoCivile = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];
                GridViewRow row = gvOneri.Rows[e.RowIndex];
                if (((TextBox)(row.Cells[1].Controls[1])).Text != "")
                {
                    int i = ((gvOneri.PageIndex * 10) + e.RowIndex);

                    if (elencoStatoCivile.Count != i + 1)
                        elencoStatoCivile.RemoveAt(elencoStatoCivile.Count - 1);
                    gvOneri.EditIndex = -1;
                    ViewState["elencoStatoCivile"] = elencoStatoCivile;
                    GvOneri_Load();
                    RaiseAnnullaOnere(this, null);
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCStatoCivile, Errore nel metodo gvStatoCivile_RowUpdating " + ex);
            }
        }

        protected void gvOneri_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GvOneri_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowDeleting " + ex);
            }
        }

        protected void gvOneri_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (this.domanda == null)
                this.domanda = (AreaRispostaRiepilogo.DatiRiepilogoDomanda)Session["Domanda"];

            AreaTitolare.DatiPensione datiPensione = GetDatiPensione(this);
            List<DatiOneriBenefParticolari.DatiOneri> elencoOneri = (List<DatiOneriBenefParticolari.DatiOneri>)ViewState["ElencoOneri"];
            List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];
            this.areaOneri = (AreaOneri)ViewState["oneri"];

            if (e.CommandName == "Edit")
            {
                if (btnSalvaDatiOneri.Enabled == true)
                    btnSalvaDatiOneri.Enabled = false;
                RaiseDisabilitaTastoSalva(this, null);

                //ENG - Integrazione Modifiche Accenture
                if (elencoOneri.Where(x => x.IdCodeGruppo == GetIdGruppoFromValue("4400 ")).Any() && !Utility.IsDomandaRipristino(datiPensione) &&
                    !(datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione)))
                {
                    if (elencoOneri.Count == 1)
                        elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());

                    elencoOneri.ElementAt(1).IdCodeGruppo = GetIdGruppoFromValue("4400 ");
                    switch (elencoOneri.ElementAt(0).IdCodeSottoGruppo)
                    {
                        case 53:
                            elencoOneri.ElementAt(1).IdCodeSottoGruppo = GetIdSottoGruppoFromValue("4404 ");
                            break;
                        case 54:
                            elencoOneri.ElementAt(1).IdCodeSottoGruppo = GetIdSottoGruppoFromValue("4405 ");
                            break;
                        case 55:
                            elencoOneri.ElementAt(1).IdCodeSottoGruppo = GetIdSottoGruppoFromValue("4406 ");
                            break;
                        default:
                            break;
                    }

                    ViewState["ElencoOneri"] = elencoOneri;
                }

                if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    if (elencoOneri.Exists(x => x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue)) //se sto inserendo una nuova riga e annullo l'inserimento
                        elencoOneri.RemoveAll(x => x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue);

                    if (elencoOneri == null || !elencoOneri.Exists(x => !x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue))
                        elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());


                    ViewState["ElencoOneri"] = elencoOneri;
                }
            }
            else if (e.CommandName == "Salva")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                long? idCodeGruppo = null;

                if (this.domanda.IsDomandaENPALS && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                    idCodeGruppo = long.Parse(((DropDownList)(r.FindControl("ddlGruppo"))).SelectedItem.Value);
                else
                    idCodeGruppo = GetIdGruppoFromValue(((Label)(r.FindControl("lblGruppo_Edit"))).Text);

                long? idSottoGruppo = long.Parse(((DropDownList)(r.FindControl("ddlSottoGruppo"))).SelectedItem.Value);
                CodiciOneriSottoGruppoOneri sottoGruppo = listaSottoGruppoOneri.Find(x => x.Id == (idSottoGruppo.HasValue ? idSottoGruppo.Value : 0));

                if (sottoGruppo.IdOnere != idCodeGruppo)
                {
                    HasError = true;
                    ErrorMessage = "Selezionare un codice sotto gruppo idoneo.";
                    RaiseShowAvviso(this, null);
                    return;
                }

                //Eng - Opzione donna pannello oneri la data cessazione beneficio deve avere massimo il valore 09/2032
                if (this.areaOneri.IsOpzioneDonna_Legge197_2022_Art1_Comma292OrRicostituzione)
                {
                    //ENG - Risolta Anomalia Opzione Donna 2023 FS/PT/INPDAP
                    DateTime dataCessazione = DateTime.MinValue;
                    if (this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS || this.domanda.Tipofondo == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT || this.domanda.IsDomandaINPDAP)
                        dataCessazione = (DateTime)Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", ((TextBox)(r.FindControl("txtCessazioneFS_PT"))).Text));
                    else
                        dataCessazione = (DateTime)Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", ((TextBox)(r.FindControl("txtCessazione"))).Text));

                    DateTime dataLimiteSup = (DateTime)Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", "01/09/2032"));

                    //ENG - Appunto 13_2023 OpzioneDonna2023_2024
                    if (ViewState["Abilitazione_OpzioneDonna2023_2024"] != null && ViewState["Abilitazione_OpzioneDonna2023_2024"].ToString().Trim().ToUpperInvariant() == "SI")
                    {
                        if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2022, 12, 31)) 
                            && Utility.DataStrettamenteSuccessivaA(new DateTime(2024, 01, 01), datiPensione.DataPerfezionamentoRequisiti.Value))
                            dataLimiteSup = (DateTime)Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", "01/08/2031"));
                        if (datiPensione.DataPerfezionamentoRequisiti.HasValue && Utility.DataStrettamenteSuccessivaA(datiPensione.DataPerfezionamentoRequisiti.Value, new DateTime(2023, 12, 31)) 
                            && Utility.DataStrettamenteSuccessivaA(new DateTime(2025, 01, 01), datiPensione.DataPerfezionamentoRequisiti.Value))
                            dataLimiteSup = (DateTime)Utility.GetDateFromString(string.Format("{0:dd/MM/yyyy}", "01/12/2033"));

                    }

                    if (Utility.DataStrettamenteSuccessivaA(dataCessazione, dataLimiteSup))
                    {
                        HasError = true;
                        ErrorMessage = String.Format("Valore massimo consentito su cessazione beneficio {0:MM/yyyy}.", dataLimiteSup);
                        RequiredFieldValidator requiredFieldtxtCessIncumul = (RequiredFieldValidator)(r.FindControl("RequiredFieldtxtCessIncumul"));
                        requiredFieldtxtCessIncumul.Enabled = false;
                        RaiseShowAvviso(this, null);
                        return;
                    }
                }

                if (btnSalvaDatiOneri.Enabled == false)
                    btnSalvaDatiOneri.Enabled = true;
                RaiseAbilitaTastoSalva(this, null);

                //FG - recupero la data modificata nella "prima" riga e la inserisco nella "seconda" aggiungendo un mese
                if (this.areaOneri.IsPrepensionamentoEditoriaArt1c154L205_2017 || this.areaOneri.IsPrepensionamentoEditoriaArt1c500L160_2019)
                {
                    // Se sono nella condizione di doppio onere
                    if (elencoOneri.Count(x => x.IdCodeGruppo == GetIdGruppoFromValue("0900 ")) == 2)
                    {
                        DatiOneriBenefParticolari.DatiOneri primoOnere = elencoOneri.FirstOrDefault(x => x.IdCodeGruppo == GetIdGruppoFromValue("0900 "));
                        // Se l'onere che sto salvando corrisponde al primo del prepensionamento
                        if (primoOnere.IdCodeGruppo == idCodeGruppo && primoOnere.IdCodeSottoGruppo == idSottoGruppo)
                        {
                            DatiOneriBenefParticolari.DatiOneri secondoOnere = elencoOneri.LastOrDefault(x => x.IdCodeGruppo == GetIdGruppoFromValue("0900 "));

                            string dataOneriCessBen = null;
                            if (((TextBox)r.FindControl("txtCessazione")) != null && !string.IsNullOrEmpty(((TextBox)r.FindControl("txtCessazione")).Text))
                            {
                                dataOneriCessBen = ((TextBox)r.FindControl("txtCessazione")).Text;
                                secondoOnere.Decorrenza = Utility.ConvertString2Data_withMinValue(dataOneriCessBen);
                            }
                        }
                    }
                }
                else if (elencoOneri[r.DataItemIndex].IdCodeGruppo == GetIdGruppoFromValue("0900 ") && elencoOneri[r.DataItemIndex].IdCodeSottoGruppo == GetIdSottoGruppoFromValue("0904 "))
                {
                    string dataOneriCessBen = null;
                    if (((TextBox)r.FindControl("txtCessazione")) != null && ((TextBox)r.FindControl("txtCessazione")).Text != null && ((TextBox)r.FindControl("txtCessazione")).Text != string.Empty)
                    {
                        dataOneriCessBen = ((TextBox)r.FindControl("txtCessazione")).Text;


                        foreach (DatiOneriBenefParticolari.DatiOneri onere in elencoOneri)
                        {
                            if (onere.IdCodeGruppo == GetIdGruppoFromValue("0900 ") && (onere.IdCodeSottoGruppo == GetIdSottoGruppoFromValue("0901 ") && onere.IdCodeSottoGruppo == GetIdSottoGruppoFromValue("0903 ")))
                            {
                                if (!string.IsNullOrEmpty(dataOneriCessBen))
                                    onere.Decorrenza = Utility.ConvertString2Data_withMinValue(dataOneriCessBen).AddMonths(1);
                            }
                        }
                    }
                }

                if (this.domanda.IsDomandaENPALS && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura))
                {
                    elencoOneri[r.DataItemIndex].IdCodeGruppo = idCodeGruppo;
                    elencoOneri[r.DataItemIndex].IdCodeSottoGruppo = idSottoGruppo;
                }
                else
                {
                    elencoOneri[r.DataItemIndex].IdCodeGruppo = GetIdGruppoFromValue(((Label)(r.FindControl("lblGruppo_Edit"))).Text);

                    if (elencoOneri[r.DataItemIndex].IdCodeGruppo == GetIdGruppoFromValue("4700 ") || elencoOneri[r.DataItemIndex].IdCodeGruppo == GetIdGruppoFromValue("0900 "))
                        elencoOneri[r.DataItemIndex].IdCodeSottoGruppo = GetIdSottoGruppoFromValue(((Label)r.FindControl("lblSottoGruppo")).Text);
                    else
                        elencoOneri[r.DataItemIndex].IdCodeSottoGruppo = long.Parse(((DropDownList)(r.Cells[2].Controls[1])).SelectedItem.Value);
                }

                if ((this.domanda.Tipofondo.HasValue && (this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.FS ||
                            this.domanda.Tipofondo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoFondo.PT)) || this.domanda.IsDomandaINPDAP)
                {
                    elencoOneri[r.DataItemIndex].Decorrenza = Utility.GetDateFromString(((Label)(r.Cells[3].Controls[1])).Text);
                    elencoOneri[r.DataItemIndex].Scadenza = Utility.GetDateFromString(((TextBox)(r.Cells[4].FindControl("txtCessazioneFS_PT"))).Text);
                }
                else
                {
                    if (CodeUtility.IsDomandaVittimeTerrorismo(datiPensione) || this.areaOneri.IsBeneficioVittimeTerrorismo || (this.domanda.IsDomandaENPALS && CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura)))
                        elencoOneri[r.DataItemIndex].Decorrenza = Utility.GetDateFromString(((TextBox)(r.FindControl("txtDecorrenza"))).Text);
                    else
                        elencoOneri[r.DataItemIndex].Decorrenza = Utility.GetDateFromString(((Label)(r.FindControl("lblDecorrenza_Edit"))).Text);
                    elencoOneri[r.DataItemIndex].Scadenza = Utility.GetDateFromString(((TextBox)(r.Cells[4].FindControl("txtCessazione"))).Text);
                }
                short resShort = 0;
                short.TryParse(((Label)(r.Cells[5].Controls[1])).Text, out resShort);
                elencoOneri[r.DataItemIndex].Settimane = resShort != 0 ? resShort : (short?)null;
                decimal resDec = 0;
                decimal.TryParse(((Label)(r.Cells[6].Controls[1])).Text, out resDec);
                elencoOneri[r.DataItemIndex].Onere = resDec != 0 ? resDec : (decimal?)null;

                //Precoci - ScadenzaBeneficio
                //ENG - Memo 121_2023
                if (datiPensione.IsDomandaAPEPrecociOrRicostituzione ||
                    ((ViewState["Abilitazione_Memo_121_2023"] == null || ViewState["Abilitazione_Memo_121_2023"].ToString().Trim().ToUpperInvariant() == "NO")
                    && (datiPensione.IsDomandaQuota100OrRicostituzione || datiPensione.IsDomandaQuota102OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileOrRicostituzione
                    || datiPensione.IsDomandaAnticipataFlessibileOpzioneContributivoOrRicostituzione)) &&
                    (!(!CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && (datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione || datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione))) &&
                    (!(CodeUtility.IsRicostituzioneOrRiapertura(datiPensione, this.domanda.IsDomandaRiapertura) && ((ViewState["AbilitazioneRIC_TRFMemo123_2024"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OrRicostituzione) ||
                    (ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] != null && (string)ViewState["AbilitazioneRIC_TRFMemo123_2024OpzioneContrib"] == "SI" && datiPensione.IsDomandaAnticipataFlessibileLeggeBilancio2024OpzioneContributivoOrRicostituzione)))) &&
                    !datiPensione.IsDomandaVOAUTAnticipataFlessibileLeggeBilancio2024FiltroGSEOrRicostituzione)
                    elencoOneri[r.DataItemIndex].ScadenzaBeneficio = Utility.GetDateFromString(((TextBox)(r.FindControl("txtCessIncumul"))).Text);

                gvOneri.EditIndex = -1;
                RaiseSalvaOnere(this, null);


                if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    if (elencoOneri == null || !elencoOneri.Exists(x => !x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue))
                        elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());
                }

                ViewState["ElencoOneri"] = elencoOneri;
                GvOneri_Load();

            }
            else if (e.CommandName == "elimina")
            {
                Control c = (Control)e.CommandSource;
                GridViewRow r = (GridViewRow)c.NamingContainer;
                elencoOneri.RemoveAt(r.DataItemIndex);
                if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    gvOneri.EditIndex = -1;
                    if (elencoOneri == null || !elencoOneri.Exists(x => !x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue))
                        elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());
                }
                else
                {
                    elencoOneri.Add(new DatiOneriBenefParticolari.DatiOneri());
                }

                ViewState["ElencoOneri"] = elencoOneri;
                GvOneri_Load();
            }
            else if (e.CommandName == "Cancel")
            {
                if (datiPensione.TipoAppartenenzaDomanda == AreaTitolare.DatiPensione.TipoAppDomanda.AGO && ViewState["EliminazioneScartoOneri0031_0105_0112"] != null && (string)ViewState["EliminazioneScartoOneri0031_0105_0112"] == "SI" && Utility.IsDomandaBeneficioTerrorismoLegge206_2004(datiPensione))
                {
                    if (elencoOneri.Exists(x => x.IdCodeGruppo.HasValue && !x.IdCodeSottoGruppo.HasValue)) //se sto inserendo una nuova riga e annullo l'inserimento
                        elencoOneri.ElementAt(elencoOneri.Count() - 1).IdCodeGruppo = null;

                    ViewState["ElencoOneri"] = elencoOneri;
                }
            }

        }

        protected void gvOneri_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                //Reset the edit index.
                gvOneri.EditIndex = -1;
                //Bind data to the GridView control.
                GvOneri_Load();
                if (btnSalvaDatiOneri.Enabled == false)
                    btnSalvaDatiOneri.Enabled = true;
                RaiseAnnullaOnere(this, null);
                RaiseAbilitaTastoSalva(this, null);

            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvOneri_RowCancelingEdit " + ex);
            }

        }

        #endregion GridView Oneri

        #region GridView Benefici Particolari

        protected void gvBenefici_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    ((Label)e.Row.FindControl("lblCodiceBenefici")).Text = ((DatiOneriBenefParticolari.DatiBeneficiParticolari)(e.Row.DataItem)).CodiceBenefici;
                    ((Label)e.Row.FindControl("lblSettimane")).Text = ((DatiOneriBenefParticolari.DatiBeneficiParticolari)(e.Row.DataItem)).Settimane.ToString();
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvBeneficiParticolari_RowDataBound " + ex);
            }
        }

        protected void gvBenefici_onPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvBenefici.PageIndex = e.NewPageIndex;
                GvBenefici_Load();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo gvBenefici_onPageIndexChanging" + ex);
            }
        }

        #endregion GridView Benefici Particolari

        #region Private Methods Oneri

        private void GvOneri_Load()
        {
            try
            {
                elencoOneriViewState = ViewState["ElencoOneri"] as List<DatiOneriBenefParticolari.DatiOneri>;
                gvOneri.DataSource = elencoOneriViewState;
                gvOneri.DataBind();

                if (this.areaOneri != null && (this.areaOneri.IsPrepensionamentoEditoriaLetteraB || this.areaOneri.IsRicVOPGIMigrataFiltroEBA))
                {
                    List<CodiciOneriSottoGruppoOneri> elencoSottoGruppo = this.areaOneri.ListaSottoGruppoOneri.ToList();
                    if (elencoSottoGruppo != null && elencoSottoGruppo.Count > 0 && elencoSottoGruppo.Any(x => x.Code == "0908"))
                        gvOneri.Rows[1].Visible = false;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GvOneri_Load " + ex);
            }
        }

        private void removeItemBlankOneri(ref List<DatiOneriBenefParticolari.DatiOneri> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiOneriBenefParticolari.DatiOneri code)
                {
                    return (string.IsNullOrEmpty(code.IdCodeGruppo.ToString()) && string.IsNullOrEmpty(code.IdCodeSottoGruppo.ToString()) &&
                        string.IsNullOrEmpty(code.Decorrenza.ToString()) && string.IsNullOrEmpty(code.Scadenza.ToString()) &&
                        string.IsNullOrEmpty(code.ScadenzaBeneficio.ToString()) &&
                        string.IsNullOrEmpty(code.Settimane.ToString()) && string.IsNullOrEmpty(code.Onere.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        private string GetValueGruppoFromId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int index = Convert.ToInt32(id);

                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate (CodiciOneriGruppoOneri code)
                { return (code.Id == index); });
                return app.Code + " - " + app.Descrizione;
            }
            else
                return string.Empty;
        }

        private string GetValueSottogruppoFromId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                int index = Convert.ToInt32(id);

                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];

                CodiciOneriSottoGruppoOneri app = listaSottoGruppoOneri.Find(delegate (CodiciOneriSottoGruppoOneri code)
                { return (code.Id == index); });
                return app.Code + " - " + app.Descrizione;
            }
            else
                return string.Empty;
        }

        private long? GetIdGruppoFromValue(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                List<CodiciOneriGruppoOneri> listaGruppoOneri = (List<CodiciOneriGruppoOneri>)ViewState["ElencoGruppo"];

                CodiciOneriGruppoOneri app = listaGruppoOneri.Find(delegate (CodiciOneriGruppoOneri code)
                { return (code.Code == value.Substring(0, value.IndexOf(' '))); });
                return app.Id;
            }
            else
                return (long?)null;
        }

        private long? GetIdSottoGruppoFromValue(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                List<CodiciOneriSottoGruppoOneri> listaSottoGruppoOneri = (List<CodiciOneriSottoGruppoOneri>)ViewState["ElencoSottoGruppo"];

                CodiciOneriSottoGruppoOneri app = listaSottoGruppoOneri.Find(delegate (CodiciOneriSottoGruppoOneri code)
                { return (code.Code == value.Substring(0, value.IndexOf(' '))); });
                if (app != null)
                    return app.Id;
                else
                    return (long?)null;
            }
            else
                return (long?)null;
        }

        #endregion Private Methods Oneri

        #region Private Methods Benefici Particolari

        private void GvBenefici_Load()
        {
            try
            {
                elencoBeneficiViewState = ViewState["ElencoBenefici"] as List<DatiOneriBenefParticolari.DatiBeneficiParticolari>;
                gvBenefici.DataSource = elencoBeneficiViewState;
                gvBenefici.DataBind();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("UCOneri, Errore nel metodo GvBenefici_Load " + ex);
            }
        }

        private void removeItemBlankBenefici(ref List<DatiOneriBenefParticolari.DatiBeneficiParticolari> lista)
        {
            if (lista != null && lista.Count() > 0)
            {
                int index = lista.FindIndex(delegate (DatiOneriBenefParticolari.DatiBeneficiParticolari code)
                {
                    return (string.IsNullOrEmpty(code.CodiceBenefici) && string.IsNullOrEmpty(code.Settimane.ToString()));
                }
                    );

                if (index >= 0)
                {
                    lista.RemoveAt(index);
                }
            }
        }

        #endregion Private Methods Benefici Particolari

        public event EventHandler ShowAvviso;
        public event EventHandler AbilitaTastoSalva;
        public event EventHandler DisabilitaTastoSalva;
        public event EventHandler SalvaOnere;
        public event EventHandler AnnullaOnere;

        #region enum
        enum gvOneri_Colonne
        {
            Settimane = 4,
            Onere = 5,
            CessBenIncumul = 6
        }
        #endregion enum
    }
}