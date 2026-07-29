using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon.Entity
{
    public class ContenitoreObject
    {
        #region Constructors
        public ContenitoreObject(long numeroDomanda, byte? progStorico)
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, progStorico, out datiPensione);
            this.DatiPensione = datiPensione;
        }

        public ContenitoreObject(GestionePensione.DatiPensione datiPensione)
        {
            this.DatiPensione = datiPensione;
        }
        #endregion Constructors

        #region private variables
        private GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici _DatiMaggiorazioniBenefici;
        private GestioneIstruttoria.DatiIstruttoria _DatiIstruttoria;
        private GestioneAnagrafica.DatiAnagrafici _DatiAnagraficiTitolare;
        private GestioneEnpals.DatiEnpals _DatiEnpals;
        private GestioneCalcolo.DatiCalcoloRetributivoENPAL _DatiCalcoloRetributivoENPALS;
        private GestioneCalcolo.DatiCalcoloRetributivoENPAL _DatiCalcoloRetributivoENPALSStorico;
        private GestioneCalcolo.DatiCalcoloContributivoENPAL _DatiCalcoloContributivoENPALS;
        private GestioneCalcolo.DatiCalcoloContributivoENPAL _DatiCalcoloContributivoENPALSStorico;
        private GestioneDatiGenericiAgoCi.PensioniDatiGenerici _DatiPensioniDatiGenerici;
        private GestioneDetrazioniImposta.DatiDetrazioni _DatiDetrazioni;
        private GestioneDetrazioniImposta.DatiDetrazioni _DatiDetrazioniStorico;
        private GestioneDanteCausa.DatiDanteCausa _DatiDanteCausa;
        private List<GestioneCalcolo.DatiCalcoloRetributivo> _ListaDatiRetributivi;
        private List<GestioneCalcolo.DatiCalcoloRetributivo> _ListaDatiRetributiviStorico;
        private GestioneCalcolo.DatiCalcoloRetributivo _DatiRetributivi;
        private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaDatiContributivi;
        private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaDatiContributiviStorico;
        private GestioneCalcolo.DatiCalcoloContributivo _DatiContributivi;
        private List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> _ListaDatiRetributiviINPGI;
        private List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> _ListaDatiRetributiviINPGIStorico;
        private List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> _ListaDatiContributiviINPGI;
        private List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> _ListaDatiContributiviINPGIStorico;
        private GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo _DatiBeneficioVittimeTerrorismo;
        private GestioneDatiControlloFelpe.ControlloFelpe _DatiControlloFelpe;
        private GestionePagamento.DatiPagamento _DatiPagamento;
        private GestioneNuoveLiquidate.NuoveLiquidate _DatiNuoveLiquidate;
        private GestionePensioneInailInabilita.DatiInabilita _DatiInabilita;
        private List<GestionePensioneInailInabilita.DatiPensioniINAIL> _ListaDatiPensioniINAIL;
        private GestioneDatiStoricoGP.DatiStoricoGP _DatiStoricoGP;
        private List<GestioneSentenzaArt4.DatiSentenzaArt4> _ListaDatiSentenzaArt4;
        private List<GestioneSentenze.DatiSentenze> _ListaDatiSentenze;
        private GestionePrepensionamento.DatiPrepensionamento _DatiPrepensionamento;
        private GestionePensione.DatiEliminazione _DatiEliminazione;
        private List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> _ListaDatiCalcoloVittimeTerrorismo;
        private GestioneLavorazione.DatiLavorazione _DatiLavorazione;
        private List<GestioneOneri.DatiOneri> _ListaDatiOneri;
        private List<GestioneFamiliari.Familiare> _ListaFamiliari;
        private List<GestioneAnagrafica.DatiAnagrafici> _ListaAnagraficaFamiliari;
        private List<GestioneFamiliari.CodMaggFamiliari> _ListaCodMaggFamiliari;
        private List<DatiSupplementi> _ListaDatiSupplementi;
        private List<DatiSupplementi> _ListaDatiSupplementiNoStorico;
        private SupplementiBase _DatiSupplementiBase;
        private GestioneIntegrazioneArt11.IntegrazioneArt11 _DatiIntegrazioneArt11;
        private List<GestioneCalcolo.QuotePensione> _ListaQuotePensione;
        private List<GestioneCalcolo.QuotePensione> _ListaQuotePensioneStorico;
        private List<GestioneCalcolo.TrattenuteQuotePensione> _ListaTrattenuteQuotePensione;
        private List<GestioneCalcolo.TrattenuteQuotePensione> _ListaTrattenuteQuotePensioneStorico;
        private List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> _ListaQuoteMiglioramentiContrattuali;
        private List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> _ListaQuoteMiglioramentiContrattualiStorico;
        private List<GestioneAltrePensioni.AltraPensione> _ListaAltraPensione;
        private List<GestioneRipartizioneFondi.DatiRipartizioneFondi> _ListaDatiRipartizioneFondi;
        private List<GestioneBeneficiParticolari.DatiBeneficiParticolari> _ListaDatiBeneficiParticolari;
        private List<DatiSuppRecordENPALS> _ListaDatiSuppRecordENPALS;
        private List<DatiSupplementiENPALS> _ListaDatiSupplementiENPALS;
        private GestionePensione.DatiSindacato _DatiSindacato;
        private GestioneAnagrafica.DatiAnagrafici _DatiAnagraficiDanteCausa;
        private List<GestioneDanteCausa.DatiRedditoSentenza495_93> _ListaDatiRedditoSentenza495_93;
        private List<GestioneRedditi.RedditoDRedd> _ListaRedditoDRedd;
        private GestioneFondo.DatiFondo _DatiFondo;
        private GestioneFondo.DatiFondoVL _DatiFondoVL;
        private AreaTitolare _DatiAreaTitolare;
        private GestioneAnagrafica.DatiAnagrafici _DatiAnagraficiDelegato;
        private GestioneAnagrafica.DatiAnagrafici _DatiAnagraficiTutore;
        private List<GestioneAventiDiritto.AventiDiritto> _ListaAventiDiritto;
        private List<GestioneAnagrafica.DatiAnagrafici> _ListaAnagraficaAventiDiritto;
        private List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> _ListaPeriodoAventiDiritto;
        private GestionePensione.DatiPatronato _DatiPatronato;
        private DatiContribuzioneEnpals _DatiContribuzioneEnpalsSAI;
        private DatiContribuzioneEnpals _DatiContribuzioneEnpalsSAS;
        private List<DatiSupplementiCumulo> _ListaDatiSupplementiCumulo;
        private List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> _ListaRichiesteRicercaDomandeANF;
        private List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> _ListaDatiQuotaFondoIntegrativo;
        private List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> _ListaDatiQuotaFondoIntegrativoStorico;
        private GestioneFondo.DatiFondoEL _DatiFondoEL;
        private GestioneFondo.DatiFondoET _DatiFondoET;
        private GestioneFondo.DatiFondoTT _DatiFondoTT;
        private GestioneFondo.DatiFondoFST _DatiFondoFS;
        private GestioneFondo.DatiFondoPT _DatiFondoPT;
        private GestioneFondo.DatiFondoPI _DatiFondoPI;
        private GestioneFondo.DatiFondoGAS _DatiFondoGAS;
        private GestioneFondo.DatiFondoCL _DatiFondoCL;
        private GestioneFondo.DatiFondoDZ _DatiFondoDZ;
        private GestioneFondo.DatiFondoES _DatiFondoES;
        private GestioneFondo.DatiFondoPM _DatiFondoPM;
        private List<GestionePensioneINPDAP.DatiPensioneINPDAP> _ListaDatiPensioneINPDAP;
        private List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> _ListaDatiRipartizioneINPDAP;
        private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaDatiCalcoloContributivoRecordFondo;
        private List<GestioneDatiServizioUtileINPDAP.ServizioUtile> _ListaDatiServizioUtileINPDAP;
        private List<GestioneCalcolo.ServizioUtileINPDAP707> _ListaDatiServizioUtile707INPDAP;
        private List<GestioneRecordFondo.DatiRecordFondo> _ListaDatiRecordFondo;
        private List<GestioneDatiServizioUtile.ServizioUtile> _ListaDatiServizioUtile;
        private List<GestioneCalcolo.ServizioUtile707> _ListaDatiServizioUtile707;
        private List<GestioneFondo.DatiFondoPT> _ListaDatiFondoPT;
        private List<GestioneFondo.DatiFondoFST> _ListaDatiFondoFST;
        private List<GestioneFondo.DatiFondoPI> _ListaDatiFondoPI;
        private long _IdFondoPensione;
        private GestioneDL407.DatiDL407 _Dl407;
        private List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> _ListaRecordDatiFondoINPDAP;
        private object _ObjectFondoXX;
        //ENG - Memo 32_a/2018
        private List<DatiSupplementiCumulo> _ListaDatiSupplementiCumuloStorico;
        private List<DatiSupplementi> _ListaDatiSupplementiStorico;

        #region GestioneQuadri
        private GestioneQuadri.DatiQuadroLiquidazionePensione _DatiQuadroLiquidazionePensione;
        private GestioneQuadri.DatiQuadroEliminazione _DatiQuadroEliminazione;
        private GestioneQuadri.DatiQuadroDatiContributivi _DatiQuadroDatiContributivi;
        private GestioneQuadri.DatiQuadroMaggiorazioniBenefici _DatiQuadroMaggiorazioniBenefici;
        private GestioneQuadri.DatiQuadroOneri _DatiQuadroOneri;
        private GestioneQuadri.DatiQuadroBititolarita _DatiQuadroBititolarita;
        private GestioneQuadri.DatiQuadroSupplementi _DatiQuadroSupplementi;
        private GestioneQuadri.DatiQuadroDetrazioni _DatiQuadroDetrazioni;
        private GestioneQuadri.DatiQuadroRedditi _DatiQuadroRedditi;
        private GestioneQuadri.DatiQuadroFamiliari _DatiQuadroFamiliari;
        private GestioneQuadri.DatiQuadroTitolare _DatiQuadroTitolare;
        private GestioneQuadri.DatiQuadroRichiestaBonus _DatiQuadroRichiestaBonus;
        private GestioneQuadri.DatiQuadroDatiNoCalcolo _DatiQuadroNoCalcolo;
        private List<GestioneQuadri.DatiQuadroDatiRecordFondo> _ListaDatiQuadroDatiRecordFondo;
        #endregion GestioneQuadri

        #region Dati Flat
        private bool? _IsRiaperturaDomanda;
        private Utility.TipoCalcolo? _TipoCalcolo;
        private Utility.TipoAppartenenza? _TipoAppartenenza;
        private string _Tipologia;
        #endregion Dati Flat
        #endregion private variables

        #region public properties
        #region oggetti
        public GestionePensione.DatiPensione DatiPensione { get; set; }
        public GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici DatiMaggiorazioniBenefici
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiMaggiorazioniBenefici_GetEffettuata)
                {
                    this.DatiMaggiorazioniBenefici_GetEffettuata = true;
                    GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
                    GestioneMaggiorazioniBenefici.GetMaggiorazioniBeneficiByIdPensione(this.DatiPensione.Id, out datiMaggiorazioniBenefici);
                    this._DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
                }

                return _DatiMaggiorazioniBenefici;
            }
            set
            {
                _DatiMaggiorazioniBenefici = value;
                DatiMaggiorazioniBenefici_GetEffettuata = true;
            }
        }
        public GestioneIstruttoria.DatiIstruttoria DatiIstruttoria
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiIstruttoria_GetEffettuata)
                {
                    this.DatiIstruttoria_GetEffettuata = true;
                    GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                    GestioneIstruttoria.GetIstruttoriaByIdPensione(this.DatiPensione.Id, out datiIstruttoria);
                    this._DatiIstruttoria = datiIstruttoria;
                }

                return _DatiIstruttoria;
            }
            set
            {
                _DatiIstruttoria = value;
                DatiIstruttoria_GetEffettuata = true;
            }
        }
        public GestioneAnagrafica.DatiAnagrafici DatiAnagraficiTitolare
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiAnagraficiTitolare_GetEffettuata)
                {
                    this.DatiAnagraficiTitolare_GetEffettuata = true;
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(this.DatiPensione.Id, out datiAnagraficiTitolare);
                    this._DatiAnagraficiTitolare = datiAnagraficiTitolare;
                }

                return _DatiAnagraficiTitolare;
            }
            set
            {
                _DatiAnagraficiTitolare = value;
                DatiAnagraficiTitolare_GetEffettuata = true;
            }
        }
        public GestioneEnpals.DatiEnpals DatiEnpals
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiEnpals_GetEffettuata)
                {
                    this.DatiEnpals_GetEffettuata = true;
                    GestioneEnpals.DatiEnpals datiEnpals = null;
                    GestioneEnpals.GetDatiEnpalsByIdPensione(this.DatiPensione.Id, out datiEnpals);
                    this._DatiEnpals = datiEnpals;
                }

                return _DatiEnpals;
            }
            set
            {
                _DatiEnpals = value;
                DatiEnpals_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloRetributivoENPAL DatiCalcoloRetributivoENPALS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiCalcoloRetributivoENPALS_GetEffettuata)
                {
                    this.DatiCalcoloRetributivoENPALS_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoENPALS = null;
                    GestioneCalcolo.GetCalcoloRetributivoEnpalsByIdPensione(this.DatiPensione.Id, out datiCalcoloRetributivoENPALS);
                    this._DatiCalcoloRetributivoENPALS = datiCalcoloRetributivoENPALS;
                }

                return _DatiCalcoloRetributivoENPALS;
            }
            set
            {
                _DatiCalcoloRetributivoENPALS = value;
                DatiCalcoloRetributivoENPALS_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloRetributivoENPAL DatiCalcoloRetributivoENPALSStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiCalcoloRetributivoENPALSStorico_GetEffettuata)
                {
                    this.DatiCalcoloRetributivoENPALSStorico_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloRetributivoENPAL datiCalcoloRetributivoENPALSStorico = null;
                    GestioneCalcolo.GetCalcoloRetributivoEnpalsStoricoByIdPensione(this.DatiPensione.Id, out datiCalcoloRetributivoENPALSStorico);
                    this._DatiCalcoloRetributivoENPALSStorico = datiCalcoloRetributivoENPALSStorico;
                }

                return _DatiCalcoloRetributivoENPALSStorico;
            }
            set
            {
                _DatiCalcoloRetributivoENPALSStorico = value;
                DatiCalcoloRetributivoENPALSStorico_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloContributivoENPAL DatiCalcoloContributivoENPALS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiCalcoloContributivoENPALS_GetEffettuata)
                {
                    this.DatiCalcoloContributivoENPALS_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoENPALS = null;
                    GestioneCalcolo.GetCalcoloContributivoEnpalsByIdPensione(this.DatiPensione.Id, out datiCalcoloContributivoENPALS);
                    this._DatiCalcoloContributivoENPALS = datiCalcoloContributivoENPALS;
                }

                return _DatiCalcoloContributivoENPALS;
            }
            set
            {
                _DatiCalcoloContributivoENPALS = value;
                DatiCalcoloContributivoENPALS_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloContributivoENPAL DatiCalcoloContributivoENPALSStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiCalcoloContributivoENPALSStorico_GetEffettuata)
                {
                    this.DatiCalcoloContributivoENPALSStorico_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloContributivoENPAL datiCalcoloContributivoENPALSStorico = null;
                    GestioneCalcolo.GetCalcoloContributivoEnpalsStoricoByIdPensione(this.DatiPensione.Id, out datiCalcoloContributivoENPALSStorico);
                    this._DatiCalcoloContributivoENPALSStorico = datiCalcoloContributivoENPALSStorico;
                }

                return _DatiCalcoloContributivoENPALSStorico;
            }
            set
            {
                _DatiCalcoloContributivoENPALSStorico = value;
                DatiCalcoloContributivoENPALSStorico_GetEffettuata = true;
            }
        }
        public GestioneDatiGenericiAgoCi.PensioniDatiGenerici DatiPensioniDatiGenerici
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiPensioniDatiGenerici_GetEffettuata)
                {
                    this.DatiPensioniDatiGenerici_GetEffettuata = true;
                    GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
                    GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(this.DatiPensione.Id, out datiPensioniDatiGenerici);
                    this._DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
                }

                return _DatiPensioniDatiGenerici;
            }
            set
            {
                _DatiPensioniDatiGenerici = value;
                DatiPensioniDatiGenerici_GetEffettuata = true;
            }
        }
        public GestioneDetrazioniImposta.DatiDetrazioni DatiDetrazioni
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiDetrazioni_GetEffettuata)
                {
                    this.DatiDetrazioni_GetEffettuata = true;
                    GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
                    GestioneDetrazioniImposta.GetDetrazioniByIdPensione(this.DatiPensione.Id, out datiDetrazioni);
                    this._DatiDetrazioni = datiDetrazioni;
                }

                return _DatiDetrazioni;
            }
            set
            {
                _DatiDetrazioni = value;
                DatiDetrazioni_GetEffettuata = true;
            }
        }
        public GestioneDetrazioniImposta.DatiDetrazioni DatiDetrazioniStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiDetrazioniStorico_GetEffettuata)
                {
                    this.DatiDetrazioniStorico_GetEffettuata = true;
                    GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioniStorico = null;
                    GestioneDetrazioniImposta.GetDetrazioniStoricoByIdPensione(this.DatiPensione.Id, out datiDetrazioniStorico);
                    this._DatiDetrazioniStorico = datiDetrazioniStorico;
                }

                return _DatiDetrazioniStorico;
            }
            set
            {
                _DatiDetrazioniStorico = value;
                DatiDetrazioniStorico_GetEffettuata = true;
            }
        }
        public GestioneDanteCausa.DatiDanteCausa DatiDanteCausa
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiDanteCausa_GetEffettuata)
                {
                    this.DatiDanteCausa_GetEffettuata = true;
                    GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    GestioneDanteCausa.GetDanteCausabyIdPensione(this.DatiPensione.Id, out datiDanteCausa);
                    this._DatiDanteCausa = datiDanteCausa;
                }

                return _DatiDanteCausa;
            }
            set
            {
                _DatiDanteCausa = value;
                DatiDanteCausa_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.DatiCalcoloRetributivo> ListaDatiRetributivi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRetributivi_GetEffettuata)
                {
                    this.ListaDatiRetributivi_GetEffettuata = true;
                    List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributivi = null;
                    GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(this.DatiPensione.Id, out listaDatiRetributivi);
                    this._ListaDatiRetributivi = listaDatiRetributivi;
                }

                return _ListaDatiRetributivi;
            }
            set
            {
                _ListaDatiRetributivi = value;
                ListaDatiRetributivi_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.DatiCalcoloRetributivo> ListaDatiRetributiviStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRetributiviStorico_GetEffettuata)
                {
                    this.ListaDatiRetributiviStorico_GetEffettuata = true;
                    List<GestioneCalcolo.DatiCalcoloRetributivo> listaDatiRetributiviStorico = null;
                    GestioneCalcolo.GetCalcoloRetributivoStoricoCI_AGOByIdPensione(this.DatiPensione.Id, out listaDatiRetributiviStorico);
                    this._ListaDatiRetributiviStorico = listaDatiRetributiviStorico;
                }

                return _ListaDatiRetributiviStorico;
            }
            set
            {
                _ListaDatiRetributiviStorico = value;
                ListaDatiRetributiviStorico_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloRetributivo DatiRetributivi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiRetributivi_GetEffettuata)
                {
                    this.DatiRetributivi_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloRetributivo datiRetributivi = null;
                    GestioneCalcolo.GetCalcoloRetributivoByIdPensione(this.DatiPensione.Id, out datiRetributivi);
                    this._DatiRetributivi = datiRetributivi;
                }

                return _DatiRetributivi;
            }
            set
            {
                _DatiRetributivi = value;
                DatiRetributivi_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiContributivi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiContributivi_GetEffettuata)
                {
                    this.ListaDatiContributivi_GetEffettuata = true;
                    List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributivi = null;
                    GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(this.DatiPensione.Id, out listaDatiContributivi);
                    this._ListaDatiContributivi = listaDatiContributivi;
                }

                return _ListaDatiContributivi;
            }
            set
            {
                _ListaDatiContributivi = value;
                ListaDatiContributivi_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiContributiviStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiContributiviStorico_GetEffettuata)
                {
                    this.ListaDatiContributiviStorico_GetEffettuata = true;
                    List<GestioneCalcolo.DatiCalcoloContributivo> listaDatiContributiviStorico = null;
                    GestioneCalcolo.GetCalcoloContributivoStoricoCI_AGOByIdPensione(this.DatiPensione.Id, out listaDatiContributiviStorico);
                    this._ListaDatiContributiviStorico = listaDatiContributiviStorico;
                }

                return _ListaDatiContributiviStorico;
            }
            set
            {
                _ListaDatiContributiviStorico = value;
                ListaDatiContributiviStorico_GetEffettuata = true;
            }
        }
        public GestioneCalcolo.DatiCalcoloContributivo DatiContributivi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiContributivi_GetEffettuata)
                {
                    this.DatiContributivi_GetEffettuata = true;
                    GestioneCalcolo.DatiCalcoloContributivo datiContributivi = null;
                    GestioneCalcolo.GetCalcoloContributivoByIdPensione(this.DatiPensione.Id, out datiContributivi);
                    this._DatiContributivi = datiContributivi;
                }

                return _DatiContributivi;
            }
            set
            {
                _DatiContributivi = value;
                DatiContributivi_GetEffettuata = true;
            }
        }
        public List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ListaDatiRetributiviINPGI
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRetributiviINPGI_GetEffettuata)
                {
                    this.ListaDatiRetributiviINPGI_GetEffettuata = true;
                    List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributivi = null;
                    GestioneQuotaFondoINPGI.GetCalcoloRetributivoINPGIByIdPensione(this.DatiPensione.Id, out listaDatiRetributivi);
                    this._ListaDatiRetributiviINPGI = listaDatiRetributivi;
                }

                return _ListaDatiRetributiviINPGI;
            }
            set
            {
                _ListaDatiRetributiviINPGI = value;
                ListaDatiRetributiviINPGI_GetEffettuata = true;
            }
        }
        public List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ListaDatiRetributiviINPGIStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRetributiviINPGIStorico_GetEffettuata)
                {
                    this.ListaDatiRetributiviINPGIStorico_GetEffettuata = true;
                    List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributivi = null;
                    GestioneQuotaFondoINPGI.GetCalcoloRetributivoINPGIStoricoByIdPensione(this.DatiPensione.Id, out listaDatiRetributivi);
                    this._ListaDatiRetributiviINPGIStorico = listaDatiRetributivi;
                }

                return _ListaDatiRetributiviINPGIStorico;
            }
            set
            {
                _ListaDatiRetributiviINPGIStorico = value;
                ListaDatiRetributiviINPGIStorico_GetEffettuata = true;
            }
        }
        public List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ListaDatiContributiviINPGI
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiContributiviINPGI_GetEffettuata)
                {
                    this.ListaDatiContributiviINPGI_GetEffettuata = true;
                    List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributivi = null;
                    GestioneQuotaFondoINPGI.GetCalcoloContributivoINPGIByIdPensione(this.DatiPensione.Id, out listaDatiContributivi);
                    this._ListaDatiContributiviINPGI = listaDatiContributivi;
                }

                return _ListaDatiContributiviINPGI;
            }
            set
            {
                _ListaDatiContributiviINPGI = value;
                ListaDatiContributiviINPGI_GetEffettuata = true;
            }
        }
        public List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ListaDatiContributiviINPGIStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiContributiviINPGIStorico_GetEffettuata)
                {
                    this.ListaDatiContributiviINPGIStorico_GetEffettuata = true;
                    List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributivi = null;
                    GestioneQuotaFondoINPGI.GetCalcoloContributivoINPGIStoricoByIdPensione(this.DatiPensione.Id, out listaDatiContributivi);
                    this._ListaDatiContributiviINPGIStorico = listaDatiContributivi;
                }

                return _ListaDatiContributiviINPGIStorico;
            }
            set
            {
                _ListaDatiContributiviINPGIStorico = value;
                ListaDatiContributiviINPGIStorico_GetEffettuata = true;
            }
        }
        public GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo DatiBeneficioVittimeTerrorismo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiBeneficioVittimeTerrorismo_GetEffettuata)
                {
                    this.DatiBeneficioVittimeTerrorismo_GetEffettuata = true;
                    GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
                    GestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(this.DatiPensione.Id, out datiBeneficioVittimeTerrorismo);
                    this._DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismo;
                }

                return _DatiBeneficioVittimeTerrorismo;
            }
            set
            {
                _DatiBeneficioVittimeTerrorismo = value;
                DatiBeneficioVittimeTerrorismo_GetEffettuata = true;
            }
        }
        public GestioneDatiControlloFelpe.ControlloFelpe DatiControlloFelpe
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiControlloFelpe_GetEffettuata)
                {
                    this.DatiControlloFelpe_GetEffettuata = true;
                    GestioneDatiControlloFelpe.ControlloFelpe datiControlloFelpe = null;
                    GestioneDatiControlloFelpe.GetDatiControlloFelpeByIdPensione(this.DatiPensione.Id, out datiControlloFelpe);
                    this._DatiControlloFelpe = datiControlloFelpe;
                }

                return _DatiControlloFelpe;
            }
            set
            {
                _DatiControlloFelpe = value;
                DatiControlloFelpe_GetEffettuata = true;
            }
        }
        public GestionePagamento.DatiPagamento DatiPagamento
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiPagamento_GetEffettuata)
                {
                    this.DatiPagamento_GetEffettuata = true;
                    GestionePagamento.DatiPagamento datiPagamento = null;
                    GestionePagamento.GetPagamentoByIdPensione(this.DatiPensione.Id, out datiPagamento);
                    this._DatiPagamento = datiPagamento;
                }

                return _DatiPagamento;
            }
            set
            {
                _DatiPagamento = value;
                DatiPagamento_GetEffettuata = true;
            }
        }
        public GestioneNuoveLiquidate.NuoveLiquidate DatiNuoveLiquidate
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiNuoveLiquidate_GetEffettuata)
                {
                    this.DatiNuoveLiquidate_GetEffettuata = true;
                    GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidate = null;
                    GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(this.DatiPensione.Id, out datiNuoveLiquidate);
                    this._DatiNuoveLiquidate = datiNuoveLiquidate;
                }

                return _DatiNuoveLiquidate;
            }
            set
            {
                _DatiNuoveLiquidate = value;
                DatiNuoveLiquidate_GetEffettuata = true;
            }
        }
        public GestionePensioneInailInabilita.DatiInabilita DatiInabilita
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiInabilita_GetEffettuata)
                {
                    this.DatiInabilita_GetEffettuata = true;
                    GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
                    GestionePensioneInailInabilita.GetInabilitaByIdPensione(this.DatiPensione.Id, out datiInabilita);
                    this._DatiInabilita = datiInabilita;
                }

                return _DatiInabilita;
            }
            set
            {
                _DatiInabilita = value;
                DatiInabilita_GetEffettuata = true;
            }
        }
        public List<GestionePensioneInailInabilita.DatiPensioniINAIL> ListaDatiPensioniINAIL
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiPensioniINAIL_GetEffettuata)
                {
                    this.ListaDatiPensioniINAIL_GetEffettuata = true;
                    List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaDatiPensioniINAIL = null;
                    GestionePensioneInailInabilita.GetPensioniINAILByIdPensione(this.DatiPensione.Id, out listaDatiPensioniINAIL);
                    this._ListaDatiPensioniINAIL = listaDatiPensioniINAIL;
                }

                return _ListaDatiPensioniINAIL;
            }
            set
            {
                _ListaDatiPensioniINAIL = value;
                ListaDatiPensioniINAIL_GetEffettuata = true;
            }
        }
        public GestioneDatiStoricoGP.DatiStoricoGP DatiStoricoGP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiStoricoGP_GetEffettuata)
                {
                    this.DatiStoricoGP_GetEffettuata = true;
                    GestioneDatiStoricoGP.DatiStoricoGP datiStoricoGP = null;
                    GestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(this.DatiPensione.Id, out datiStoricoGP);
                    this._DatiStoricoGP = datiStoricoGP;
                }

                return _DatiStoricoGP;
            }
            set
            {
                _DatiStoricoGP = value;
                DatiStoricoGP_GetEffettuata = true;
            }
        }
        public List<GestioneSentenzaArt4.DatiSentenzaArt4> ListaDatiSentenzaArt4
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSentenzaArt4_GetEffettuata)
                {
                    this.ListaDatiSentenzaArt4_GetEffettuata = true;
                    List<GestioneSentenzaArt4.DatiSentenzaArt4> listaDatiSentenzaArt4 = null;
                    GestioneSentenzaArt4.GetDatiSentenzaArt4(this.DatiPensione.Id, out listaDatiSentenzaArt4);
                    this._ListaDatiSentenzaArt4 = listaDatiSentenzaArt4;
                }

                return _ListaDatiSentenzaArt4;
            }
            set
            {
                _ListaDatiSentenzaArt4 = value;
                ListaDatiSentenzaArt4_GetEffettuata = true;
            }
        }
        public List<GestioneSentenze.DatiSentenze> ListaDatiSentenze
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSentenze_GetEffettuata)
                {
                    this.ListaDatiSentenze_GetEffettuata = true;
                    List<GestioneSentenze.DatiSentenze> listaDatiSentenze = null;
                    GestioneSentenze.GetDatiSentenze(this.DatiPensione.Id, out listaDatiSentenze);
                    this._ListaDatiSentenze = listaDatiSentenze;
                }

                return _ListaDatiSentenze;
            }
            set
            {
                _ListaDatiSentenze = value;
                ListaDatiSentenze_GetEffettuata = true;
            }
        }
        public GestionePrepensionamento.DatiPrepensionamento DatiPrepensionamento
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiPrepensionamento_GetEffettuata)
                {
                    this.DatiPrepensionamento_GetEffettuata = true;
                    GestionePrepensionamento.DatiPrepensionamento datiPrepensionamento = null;
                    GestionePrepensionamento.GetDatiPrepensionamentoByIdPensione(this.DatiPensione.Id, out datiPrepensionamento);
                    this._DatiPrepensionamento = datiPrepensionamento;
                }

                return _DatiPrepensionamento;
            }
            set
            {
                _DatiPrepensionamento = value;
                DatiPrepensionamento_GetEffettuata = true;
            }
        }
        public GestionePensione.DatiEliminazione DatiEliminazione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiEliminazione_GetEffettuata)
                {
                    this.DatiEliminazione_GetEffettuata = true;
                    GestionePensione.DatiEliminazione datiEliminazione = null;
                    GestionePensione.GetEliminazioneByIdPensione(this.DatiPensione.Id, out datiEliminazione);
                    this._DatiEliminazione = datiEliminazione;
                }

                return _DatiEliminazione;
            }
            set
            {
                _DatiEliminazione = value;
                DatiEliminazione_GetEffettuata = true;
            }
        }
        public List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> ListaDatiCalcoloVittimeTerrorismo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiCalcoloVittimeTerrorismo_GetEffettuata)
                {
                    this.ListaDatiCalcoloVittimeTerrorismo_GetEffettuata = true;
                    List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
                    GestioneCalcoloVittimeTerrorismo.GetCalcoloVittimeTerrorismoByIdPensione(this.DatiPensione.Id, out listaDatiCalcoloVittimeTerrorismo);
                    this._ListaDatiCalcoloVittimeTerrorismo = listaDatiCalcoloVittimeTerrorismo;
                }

                return _ListaDatiCalcoloVittimeTerrorismo;
            }
            set
            {
                _ListaDatiCalcoloVittimeTerrorismo = value;
                ListaDatiCalcoloVittimeTerrorismo_GetEffettuata = true;
            }
        }
        public GestioneLavorazione.DatiLavorazione DatiLavorazione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiLavorazione_GetEffettuata)
                {
                    this.DatiLavorazione_GetEffettuata = true;
                    GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                    GestioneLavorazione.GetLavorazioneByIdPensione(this.DatiPensione.Id, out datiLavorazione);
                    this._DatiLavorazione = datiLavorazione;
                }

                return _DatiLavorazione;
            }
            set
            {
                _DatiLavorazione = value;
                DatiLavorazione_GetEffettuata = true;
            }
        }
        public List<GestioneOneri.DatiOneri> ListaDatiOneri
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiOneri_GetEffettuata)
                {
                    this.ListaDatiOneri_GetEffettuata = true;
                    List<GestioneOneri.DatiOneri> listaDatiOneri = null;
                    GestioneOneri.GetOneriByIdPensione(this.DatiPensione.Id, out listaDatiOneri);
                    this._ListaDatiOneri = listaDatiOneri;
                }

                return _ListaDatiOneri;
            }
            set
            {
                _ListaDatiOneri = value;
                ListaDatiOneri_GetEffettuata = true;
            }
        }
        public List<GestioneFamiliari.Familiare> ListaFamiliari
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaFamiliari_GetEffettuata)
                {
                    this.ListaFamiliari_GetEffettuata = true;
                    this.ListaAnagraficaFamiliari_GetEffettuata = true;
                    List<GestioneFamiliari.Familiare> listaFamiliari = null;
                    List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari;
                    GestioneFamiliari.GetFamiliariByIdPensione(this.DatiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);
                    this._ListaFamiliari = listaFamiliari;
                    this._ListaAnagraficaFamiliari = listaAnagraficaFamiliari;
                }

                return _ListaFamiliari;
            }
            set
            {
                _ListaFamiliari = value;
                ListaFamiliari_GetEffettuata = true;
            }
        }
        public List<GestioneAnagrafica.DatiAnagrafici> ListaAnagraficaFamiliari
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaAnagraficaFamiliari_GetEffettuata)
                {
                    this.ListaFamiliari_GetEffettuata = true;
                    this.ListaAnagraficaFamiliari_GetEffettuata = true;
                    List<GestioneFamiliari.Familiare> listaFamiliari = null;
                    List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaFamiliari;
                    GestioneFamiliari.GetFamiliariByIdPensione(this.DatiPensione.Id, out listaFamiliari, out listaAnagraficaFamiliari);
                    this._ListaFamiliari = listaFamiliari;
                    this._ListaAnagraficaFamiliari = listaAnagraficaFamiliari;
                }

                return _ListaAnagraficaFamiliari;
            }
            set
            {
                _ListaAnagraficaFamiliari = value;
                ListaAnagraficaFamiliari_GetEffettuata = true;
            }
        }
        public List<GestioneFamiliari.CodMaggFamiliari> ListaCodMaggFamiliari
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaCodMaggFamiliari_GetEffettuata)
                {
                    this.ListaCodMaggFamiliari_GetEffettuata = true;
                    List<GestioneFamiliari.CodMaggFamiliari> listaCodMaggFamiliari = null;
                    GestioneFamiliari.GetCodMaggiorazioneFamiliariByIdPensione(this.DatiPensione.Id, out listaCodMaggFamiliari);
                    this._ListaCodMaggFamiliari = listaCodMaggFamiliari;
                }

                return _ListaCodMaggFamiliari;
            }
            set
            {
                _ListaCodMaggFamiliari = value;
                ListaCodMaggFamiliari_GetEffettuata = true;
            }
        }
        public List<DatiSupplementi> ListaDatiSupplementi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementi_GetEffettuata)
                {
                    this.ListaDatiSupplementi_GetEffettuata = true;
                    List<DatiSupplementi> listaDatiSupplementi = null;
                    GestioneSupplementi.GetSupplementiByIdPensione(this.DatiPensione.Id, out listaDatiSupplementi);
                    this._ListaDatiSupplementi = listaDatiSupplementi;
                }

                return _ListaDatiSupplementi;
            }
            set
            {
                _ListaDatiSupplementi = value;
                ListaDatiSupplementi_GetEffettuata = true;
            }
        }

        public List<DatiSupplementi> ListaDatiSupplementiNoStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementiNoStorico_GetEffettuata)
                {
                    this.ListaDatiSupplementi_GetEffettuata = true;
                    List<DatiSupplementi> listaDatiSupplementiNoStorico = null;
                    GestioneSupplementi.GetSupplementiNoStoricoByIdPensione(this.DatiPensione.Id, out listaDatiSupplementiNoStorico);
                    this._ListaDatiSupplementiNoStorico = listaDatiSupplementiNoStorico;
                }

                return _ListaDatiSupplementiNoStorico;
            }
            set
            {
                _ListaDatiSupplementiNoStorico = value;
                ListaDatiSupplementiNoStorico_GetEffettuata = true;
            }
        }

        public SupplementiBase DatiSupplementiBase
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiSupplementiBase_GetEffettuata)
                {
                    this.DatiSupplementiBase_GetEffettuata = true;
                    SupplementiBase datiSupplementiBase = null;
                    GestioneSupplementi.GetDatiSupplementiBaseByIdPensione(this.DatiPensione.Id, out datiSupplementiBase);
                    this._DatiSupplementiBase = datiSupplementiBase;
                }

                return _DatiSupplementiBase;
            }
            set
            {
                _DatiSupplementiBase = value;
                DatiSupplementiBase_GetEffettuata = true;
            }
        }
        public GestioneIntegrazioneArt11.IntegrazioneArt11 DatiIntegrazioneArt11
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiIntegrazioneArt11_GetEffettuata)
                {
                    this.DatiIntegrazioneArt11_GetEffettuata = true;
                    GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11 = null;
                    GestioneIntegrazioneArt11.GetIntegrazioneArt11ByIdPensione(this.DatiPensione.Id, out datiIntegrazioneArt11);
                    this._DatiIntegrazioneArt11 = datiIntegrazioneArt11;
                }

                return _DatiIntegrazioneArt11;
            }
            set
            {
                _DatiIntegrazioneArt11 = value;
                DatiIntegrazioneArt11_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.QuotePensione> ListaQuotePensione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaQuotePensione_GetEffettuata)
                {
                    this.ListaQuotePensione_GetEffettuata = true;
                    List<GestioneCalcolo.QuotePensione> listaQuotePensione = null;
                    GestioneCalcolo.GetQuotePensioneByIdPensione(this.DatiPensione.Id, out listaQuotePensione);
                    this._ListaQuotePensione = listaQuotePensione;
                }

                return _ListaQuotePensione;
            }
            set
            {
                _ListaQuotePensione = value;
                ListaQuotePensione_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.QuotePensione> ListaQuotePensioneStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaQuotePensioneStorico_GetEffettuata)
                {
                    this.ListaQuotePensioneStorico_GetEffettuata = true;
                    List<GestioneCalcolo.QuotePensione> listaQuotePensioneStorico = null;
                    GestioneCalcolo.GetQuotePensioneStoricoByIdPensione(this.DatiPensione.Id, out listaQuotePensioneStorico);
                    this._ListaQuotePensioneStorico = listaQuotePensioneStorico;
                }

                return _ListaQuotePensioneStorico;
            }
            set
            {
                _ListaQuotePensioneStorico = value;
                ListaQuotePensioneStorico_GetEffettuata = true;
            }
        }

        public List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaQuoteMiglioramentiContrattuali
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaQuoteMiglioramentiContrattuali_GetEffettuata)
                {
                    this.ListaQuoteMiglioramentiContrattuali_GetEffettuata = true;
                    List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali = null;
                    GestioneMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiNoStoricoByIdPensione(this.DatiPensione.Id, out listaQuoteMiglioramentiContrattuali);
                    this._ListaQuoteMiglioramentiContrattuali = listaQuoteMiglioramentiContrattuali;
                }

                return _ListaQuoteMiglioramentiContrattuali;
            }
            set
            {
                _ListaQuoteMiglioramentiContrattuali = value;
                ListaQuoteMiglioramentiContrattuali_GetEffettuata = true;
            }
        }

        public List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaQuoteMiglioramentiContrattualiStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaQuoteMiglioramentiContrattualiStorico_GetEffettuata)
                {
                    this.ListaQuoteMiglioramentiContrattualiStorico_GetEffettuata = true;
                    List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattualiStorico = null;
                    GestioneMiglioramentiContrattuali.GetDatiQuoteMiglioramentiContrattualiStoricoByIdPensione(this.DatiPensione.Id, out listaQuoteMiglioramentiContrattualiStorico);
                    this._ListaQuoteMiglioramentiContrattualiStorico = listaQuoteMiglioramentiContrattualiStorico;
                }

                return _ListaQuoteMiglioramentiContrattualiStorico;
            }
            set
            {
                _ListaQuoteMiglioramentiContrattualiStorico = value;
                ListaQuoteMiglioramentiContrattualiStorico_GetEffettuata = true;
            }
        }
        public List<GestioneCalcolo.TrattenuteQuotePensione> ListaTrattenuteQuotePensione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaTrattenuteQuotePensione_GetEffettuata)
                {
                    this.ListaTrattenuteQuotePensione_GetEffettuata = true;
                    List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenuteQuotePensione = null;
                    GestioneCalcolo.GetTrattenuteQuotePensioneByIdPensione(DatiPensione.Id, out listaTrattenuteQuotePensione);
                    this._ListaTrattenuteQuotePensione = listaTrattenuteQuotePensione;
                }
                return this._ListaTrattenuteQuotePensione;
            }
            set
            {
                this.ListaTrattenuteQuotePensione_GetEffettuata = true;
                this._ListaTrattenuteQuotePensione = value;
            }
        }
        public List<GestioneCalcolo.TrattenuteQuotePensione> ListaTrattenuteQuotePensioneStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaTrattenuteQuotePensioneStorico_GetEffettuata)
                {
                    this.ListaTrattenuteQuotePensioneStorico_GetEffettuata = true;
                    List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenuteQuotePensioneStorico = null;
                    GestioneCalcolo.GetTrattenuteQuotePensioneStoricoByIdPensione(DatiPensione.Id, out listaTrattenuteQuotePensioneStorico);
                    this._ListaTrattenuteQuotePensioneStorico = listaTrattenuteQuotePensioneStorico;
                }
                return this._ListaTrattenuteQuotePensioneStorico;
            }
            set
            {
                this.ListaTrattenuteQuotePensioneStorico_GetEffettuata = true;
                this._ListaTrattenuteQuotePensioneStorico = value;
            }
        }
        public List<GestioneAltrePensioni.AltraPensione> ListaAltraPensione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.ListaAltraPensione_GetEffettuata)
                {
                    this.ListaAltraPensione_GetEffettuata = true;
                    List<GestioneAltrePensioni.AltraPensione> listaAltraPensione = null;
                    GestioneAltrePensioni.GetAltraPensioneByIdPensione(DatiPensione.Id, out listaAltraPensione);
                    this._ListaAltraPensione = listaAltraPensione;
                }
                return _ListaAltraPensione;
            }
            set
            {
                _ListaAltraPensione = value;
                ListaAltraPensione_GetEffettuata = true;
            }
        }
        public List<GestioneRipartizioneFondi.DatiRipartizioneFondi> ListaDatiRipartizioneFondi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRipartizioneFondi_GetEffettuata)
                {
                    this.ListaDatiRipartizioneFondi_GetEffettuata = true;
                    List<GestioneRipartizioneFondi.DatiRipartizioneFondi> listaDatiRipartizioneFondi = null;
                    GestioneRipartizioneFondi.GetRipartizioneFondiByIdPensione(this.DatiPensione.Id, out listaDatiRipartizioneFondi);
                    this._ListaDatiRipartizioneFondi = listaDatiRipartizioneFondi;
                }

                return _ListaDatiRipartizioneFondi;
            }
            set
            {
                _ListaDatiRipartizioneFondi = value;
                ListaDatiRipartizioneFondi_GetEffettuata = true;
            }
        }
        public List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiBeneficiParticolari_GetEffettuata)
                {
                    this.ListaDatiBeneficiParticolari_GetEffettuata = true;
                    List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
                    GestioneBeneficiParticolari.GetBeneficiParticolariByIdPensione(this.DatiPensione.Id, this.DatiPensione, out listaDatiBeneficiParticolari);
                    this._ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
                }

                return _ListaDatiBeneficiParticolari;
            }
            set
            {
                _ListaDatiBeneficiParticolari = value;
                ListaDatiBeneficiParticolari_GetEffettuata = true;
            }
        }
        public List<DatiSuppRecordENPALS> ListaDatiSuppRecordENPALS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSuppRecordENPALS_GetEffettuata)
                {
                    this.ListaDatiSuppRecordENPALS_GetEffettuata = true;
                    List<DatiSuppRecordENPALS> listaDatiSuppRecordENPALS = null;
                    GestioneSupplementi.GetDatiSuppRecordEnpalsByIdPensione(this.DatiPensione.Id, out listaDatiSuppRecordENPALS);
                    this._ListaDatiSuppRecordENPALS = listaDatiSuppRecordENPALS;
                }

                return _ListaDatiSuppRecordENPALS;
            }
            set
            {
                _ListaDatiSuppRecordENPALS = value;
                ListaDatiSuppRecordENPALS_GetEffettuata = true;
            }
        }
        public List<DatiSupplementiENPALS> ListaDatiSupplementiENPALS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementiENPALS_GetEffettuata)
                {
                    this.ListaDatiSupplementiENPALS_GetEffettuata = true;
                    List<DatiSupplementiENPALS> listaDatiSupplementiENPALS = null;
                    GestioneSupplementi.GetDatiSupplementiEnpalsByIdPensione(this.DatiPensione.Id, out listaDatiSupplementiENPALS);
                    this._ListaDatiSupplementiENPALS = listaDatiSupplementiENPALS;
                }

                return _ListaDatiSupplementiENPALS;
            }
            set
            {
                _ListaDatiSupplementiENPALS = value;
                ListaDatiSupplementiENPALS_GetEffettuata = true;
            }
        }
        public GestionePensione.DatiSindacato DatiSindacato
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiSindacato_GetEffettuata)
                {
                    this.DatiSindacato_GetEffettuata = true;
                    GestionePensione.DatiSindacato datiSindacato = null;
                    GestionePensione.GetSindacatoByIdPensione(this.DatiPensione.Id, out datiSindacato);
                    this._DatiSindacato = datiSindacato;
                }

                return _DatiSindacato;
            }
            set
            {
                _DatiSindacato = value;
                DatiSindacato_GetEffettuata = true;
            }
        }
        public GestioneAnagrafica.DatiAnagrafici DatiAnagraficiDanteCausa
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiAnagraficiDanteCausa_GetEffettuata)
                {
                    this.DatiAnagraficiDanteCausa_GetEffettuata = true;
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiDanteCausa = null;
                    GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(this.DatiPensione.Id, out datiAnagraficiDanteCausa);
                    this._DatiAnagraficiDanteCausa = datiAnagraficiDanteCausa;
                }

                return _DatiAnagraficiDanteCausa;
            }
            set
            {
                _DatiAnagraficiDanteCausa = value;
                DatiAnagraficiDanteCausa_GetEffettuata = true;
            }
        }
        public List<GestioneDanteCausa.DatiRedditoSentenza495_93> ListaDatiRedditoSentenza495_93
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRedditoSentenza495_93_GetEffettuata)
                {
                    this.ListaDatiRedditoSentenza495_93_GetEffettuata = true;
                    List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditoSentenza495_93 = null;
                    GestioneDanteCausa.GetRedditiSentenza495_93ByIdPensione(this.DatiPensione.Id, out listaDatiRedditoSentenza495_93);
                    this._ListaDatiRedditoSentenza495_93 = listaDatiRedditoSentenza495_93;
                }

                return _ListaDatiRedditoSentenza495_93;
            }
            set
            {
                _ListaDatiRedditoSentenza495_93 = value;
                ListaDatiRedditoSentenza495_93_GetEffettuata = true;
            }
        }
        public List<GestioneRedditi.RedditoDRedd> ListaRedditoDRedd
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.ListaRedditoDRedd_GetEffettuata)
                {
                    this.ListaRedditoDRedd_GetEffettuata = true;
                    List<BLCommon.GestioneRedditi.RedditoDRedd> listaRedditoDRedd = null;
                    BLCommon.GestioneRedditi.GetRedditiDReddByIdPensione(this.DatiPensione.Id, out listaRedditoDRedd);
                    this._ListaRedditoDRedd = listaRedditoDRedd;
                }
                return _ListaRedditoDRedd;
            }
            set
            {
                this.ListaRedditoDRedd_GetEffettuata = true;
                _ListaRedditoDRedd = value;
            }
        }
        public GestioneFondo.DatiFondo DatiFondo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.DatiFondo_GetEffettuata)
                {
                    this.DatiFondo_GetEffettuata = true;
                    GestioneFondo.DatiFondo datiFondo = null;
                    GestioneFondo.GetFondoDatiGenericiByIdPensione(this.DatiPensione.Id, out datiFondo);
                    this._DatiFondo = datiFondo;
                }
                return this._DatiFondo;
            }
            set
            {
                this.DatiFondo_GetEffettuata = true;
                this._DatiFondo = value;
            }
        }
        public GestioneFondo.DatiFondoVL DatiFondoVL
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.DatiFondoVL_GetEffettuata)
                {
                    this.DatiFondoVL_GetEffettuata = true;
                    GestioneFondo.DatiFondoVL datiFondoVL = null;
                    GestioneFondo.GetFondoVLByIdPensione(this.DatiPensione.Id, out datiFondoVL);
                    this._DatiFondoVL = datiFondoVL;
                }
                return this._DatiFondoVL;
            }
            set
            {
                this.DatiFondoVL_GetEffettuata = true;
                this._DatiFondoVL = value;
            }
        }
        public AreaTitolare DatiAreaTitolare
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiAreaTitolare_GetEffettuata)
                {
                    this.DatiAreaTitolare_GetEffettuata = true;
                    AreaTitolare datiAreaTitolare = null;
                    GestioneAnagrafica.GetAreaTitolareByDatiPensione(this.DatiPensione, out datiAreaTitolare);
                    this._DatiAreaTitolare = datiAreaTitolare;
                }

                return _DatiAreaTitolare;
            }
            set
            {
                _DatiAreaTitolare = value;
                DatiAreaTitolare_GetEffettuata = true;
            }
        }
        public GestioneAnagrafica.DatiAnagrafici DatiAnagraficiDelegato
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiAnagraficiDelegato_GetEffettuata)
                {
                    this.DatiAnagraficiDelegato_GetEffettuata = true;
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiDelegato = null;
                    GestioneDelegatoTutore.GetDelegatoByIdPensione(this.DatiPensione.Id, out datiAnagraficiDelegato);
                    this._DatiAnagraficiDelegato = datiAnagraficiDelegato;
                }

                return _DatiAnagraficiDelegato;
            }
            set
            {
                _DatiAnagraficiDelegato = value;
                DatiAnagraficiDelegato_GetEffettuata = true;
            }
        }
        public GestioneAnagrafica.DatiAnagrafici DatiAnagraficiTutore
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiAnagraficiTutore_GetEffettuata)
                {
                    this.DatiAnagraficiTutore_GetEffettuata = true;
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTutore = null;
                    GestioneDelegatoTutore.GetTutoreByIdPensione(this.DatiPensione.Id, out datiAnagraficiTutore);
                    this._DatiAnagraficiTutore = datiAnagraficiTutore;
                }

                return _DatiAnagraficiTutore;
            }
            set
            {
                _DatiAnagraficiTutore = value;
                DatiAnagraficiTutore_GetEffettuata = true;
            }
        }
        public List<GestioneAventiDiritto.AventiDiritto> ListaAventiDiritto
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaAventiDiritto_GetEffettuata)
                {
                    this.ListaAventiDiritto_GetEffettuata = true;
                    this.ListaAnagraficaAventiDiritto_GetEffettuata = true;
                    List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                    List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaAventiDiritto;
                    GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(this.DatiPensione.Id, out listaAventiDiritto, out listaAnagraficaAventiDiritto);
                    this._ListaAventiDiritto = listaAventiDiritto;
                    this._ListaAnagraficaAventiDiritto = listaAnagraficaAventiDiritto;
                }

                return _ListaAventiDiritto;
            }
            set
            {
                _ListaAventiDiritto = value;
                ListaAventiDiritto_GetEffettuata = true;
            }
        }
        public List<GestioneAnagrafica.DatiAnagrafici> ListaAnagraficaAventiDiritto
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaAnagraficaAventiDiritto_GetEffettuata)
                {
                    this.ListaAventiDiritto_GetEffettuata = true;
                    this.ListaAnagraficaAventiDiritto_GetEffettuata = true;
                    List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                    List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficaAventiDiritto;
                    GestioneAventiDiritto.GetAventiDirittoConAnagraficheByIdPensione(this.DatiPensione.Id, out listaAventiDiritto, out listaAnagraficaAventiDiritto);
                    this._ListaAventiDiritto = listaAventiDiritto;
                    this._ListaAnagraficaAventiDiritto = listaAnagraficaAventiDiritto;
                }

                return _ListaAnagraficaAventiDiritto;
            }
            set
            {
                _ListaAnagraficaAventiDiritto = value;
                ListaAnagraficaAventiDiritto_GetEffettuata = true;
            }
        }
        public List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> ListaPeriodoAventiDiritto
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.ListaPeriodoAventiDiritto_GetEffettuata)
                {
                    this.ListaPeriodoAventiDiritto_GetEffettuata = true;
                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodoAventiDiritto = null;
                    GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(this.DatiPensione.Id, null, out listaPeriodoAventiDiritto);
                    this._ListaPeriodoAventiDiritto = listaPeriodoAventiDiritto;
                }
                return _ListaPeriodoAventiDiritto;
            }
            set
            {
                this.ListaPeriodoAventiDiritto_GetEffettuata = true;
                _ListaPeriodoAventiDiritto = value;
            }
        }
        public GestionePensione.DatiPatronato DatiPatronato
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiPatronato_GetEffettuata)
                {
                    this.DatiPatronato_GetEffettuata = true;
                    GestionePensione.DatiPatronato datiPatronato = null;
                    GestionePensione.GetPatronatoByIdPensione(this.DatiPensione.Id, out datiPatronato);
                    this._DatiPatronato = datiPatronato;
                }

                return _DatiPatronato;
            }
            set
            {
                _DatiPatronato = value;
                DatiPatronato_GetEffettuata = true;
            }
        }
        public DatiContribuzioneEnpals DatiContribuzioneEnpalsSAI
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiContribuzioneEnpalsSAI_GetEffettuata)
                {
                    this.DatiContribuzioneEnpalsSAI_GetEffettuata = true;
                    DatiContribuzioneEnpals datiContribuzioneEnpalsSAI = null;
                    GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(this.DatiPensione.Id, TipologiaContribuzioneEnpals.SAI, out datiContribuzioneEnpalsSAI);
                    this._DatiContribuzioneEnpalsSAI = datiContribuzioneEnpalsSAI;
                }

                return this._DatiContribuzioneEnpalsSAI;
            }
            set
            {
                this._DatiContribuzioneEnpalsSAI = value;
                this.DatiContribuzioneEnpalsSAI_GetEffettuata = true;
            }
        }
        public DatiContribuzioneEnpals DatiContribuzioneEnpalsSAS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiContribuzioneEnpalsSAS_GetEffettuata)
                {
                    this.DatiContribuzioneEnpalsSAS_GetEffettuata = true;
                    DatiContribuzioneEnpals datiContribuzioneEnpalsSAS = null;
                    GestioneContribuzioneEnpals.GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(this.DatiPensione.Id, TipologiaContribuzioneEnpals.SAS, out datiContribuzioneEnpalsSAS);
                    this._DatiContribuzioneEnpalsSAS = datiContribuzioneEnpalsSAS;
                }

                return this._DatiContribuzioneEnpalsSAS;
            }
            set
            {
                this._DatiContribuzioneEnpalsSAS = value;
                this.DatiContribuzioneEnpalsSAS_GetEffettuata = true;
            }
        }
        public List<DatiSupplementiCumulo> ListaDatiSupplementiCumulo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementiCumulo_GetEffettuata)
                {
                    this.ListaDatiSupplementiCumulo_GetEffettuata = true;
                    List<DatiSupplementiCumulo> listaDatiSupplementiCumulo = null;
                    GestioneSupplementi.GetSupplementiCumuloByIdPensione(this.DatiPensione.Id, out listaDatiSupplementiCumulo);
                    this._ListaDatiSupplementiCumulo = listaDatiSupplementiCumulo;
                }

                return this._ListaDatiSupplementiCumulo;
            }
            set
            {
                this._ListaDatiSupplementiCumulo = value;
                this.ListaDatiSupplementiCumulo_GetEffettuata = true;
            }
        }
        public List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> ListaRichiesteRicercaDomandeANF
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;
                if (!this.ListaRichiesteRicercaDomandeANF_GetEffettuata)
                {
                    this.ListaRichiesteRicercaDomandeANF_GetEffettuata = true;
                    List<GestioneFamiliari.DatiRichiestaRicercaDomandeANF> listaRichiesteRicercaDomandeANF = null;
                    GestioneFamiliari.GetRichiesteRicercaDomandeANFByIdPensione(this.DatiPensione.Id, out listaRichiesteRicercaDomandeANF);
                    this._ListaRichiesteRicercaDomandeANF = listaRichiesteRicercaDomandeANF;
                }
                return this._ListaRichiesteRicercaDomandeANF;
            }
        }
        public List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaDatiQuotaFondoIntegrativo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiQuotaFondoIntegrativo_GetEffettuata)
                {
                    this.ListaDatiQuotaFondoIntegrativo_GetEffettuata = true;
                    List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaDatiQuotaFondoIntegrativo = null;
                    GestioneQuotaFondoIntegrativo.GetQuotaFondoIntegrativoByIdPensione(this.DatiPensione.Id, out listaDatiQuotaFondoIntegrativo);
                    this._ListaDatiQuotaFondoIntegrativo = listaDatiQuotaFondoIntegrativo;
                }

                return _ListaDatiQuotaFondoIntegrativo;
            }
            set
            {
                _ListaDatiQuotaFondoIntegrativo = value;
                ListaDatiQuotaFondoIntegrativo_GetEffettuata = true;
            }
        }

        public List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaDatiQuotaFondoIntegrativoStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiQuotaFondoIntegrativoStorico_GetEffettuata)
                {
                    this.ListaDatiQuotaFondoIntegrativoStorico_GetEffettuata = true;
                    List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaDatiQuotaFondoIntegrativoStorico = null;
                    GestioneQuotaFondoIntegrativo.GetQuotaFondoIntegrativoStoricoByIdPensione(this.DatiPensione.Id, out listaDatiQuotaFondoIntegrativoStorico);
                    this._ListaDatiQuotaFondoIntegrativoStorico = listaDatiQuotaFondoIntegrativoStorico;
                }

                return _ListaDatiQuotaFondoIntegrativoStorico;
            }
            set
            {
                _ListaDatiQuotaFondoIntegrativoStorico = value;
                ListaDatiQuotaFondoIntegrativoStorico_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoEL DatiFondoEL
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoEL_GetEffettuata)
                {
                    this.DatiFondoEL_GetEffettuata = true;
                    GestioneFondo.DatiFondoEL fondoEL = null;
                    GestioneFondo.GetFondoELByIdPensione(this.DatiPensione.Id, out fondoEL);
                    this._DatiFondoEL = fondoEL;
                }

                return this._DatiFondoEL;
            }
            set
            {
                this._DatiFondoEL = value;
                this.DatiFondoEL_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoET DatiFondoET
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoET_GetEffettuata)
                {
                    this.DatiFondoET_GetEffettuata = true;
                    GestioneFondo.DatiFondoET fondoET = null;
                    GestioneFondo.GetFondoETByIdPensione(this.DatiPensione.Id, out fondoET);
                    this._DatiFondoET = fondoET;
                }

                return this._DatiFondoET;
            }
            set
            {
                this._DatiFondoET = value;
                this.DatiFondoET_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoTT DatiFondoTT
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoTT_GetEffettuata)
                {
                    this.DatiFondoTT_GetEffettuata = true;
                    GestioneFondo.DatiFondoTT fondoTT = null;
                    GestioneFondo.GetFondoTTByIdPensione(this.DatiPensione.Id, out fondoTT);
                    this._DatiFondoTT = fondoTT;
                }
                return this._DatiFondoTT;
            }
            set
            {
                this._DatiFondoTT = value;
                this.DatiFondoTT_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoFST DatiFondoFS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoFS_GetEffettuata)
                {
                    this.DatiFondoFS_GetEffettuata = true;
                    GestioneFondo.DatiFondoFST fondoFS = null;
                    GestioneFondo.GetFondoFSTByIdPensione(this.DatiPensione.Id, out fondoFS);
                    this._DatiFondoFS = fondoFS;
                }

                return this._DatiFondoFS;
            }
            set
            {
                this._DatiFondoFS = value;
                this.DatiFondoFS_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoPT DatiFondoPT
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoPT_GetEffettuata)
                {
                    this.DatiFondoPT_GetEffettuata = true;
                    GestioneFondo.DatiFondoPT fondoPT = null;
                    GestioneFondo.GetFondoPTByIdPensione(this.DatiPensione.Id, out fondoPT);
                    this._DatiFondoPT = fondoPT;
                }

                return this._DatiFondoPT;
            }
            set
            {
                this._DatiFondoPT = value;
                this.DatiFondoPT_GetEffettuata = true;
            }
        }


        public GestioneFondo.DatiFondoPI DatiFondoPI
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoPI_GetEffettuata)
                {
                    this.DatiFondoPI_GetEffettuata = true;
                    GestioneFondo.DatiFondoPI fondoPI = null;
                    GestioneFondo.GetFondoPIByIdPensione(this.DatiPensione.Id, out fondoPI);
                    this._DatiFondoPI = fondoPI;
                }
                return this._DatiFondoPI;
            }
            set
            {
                this._DatiFondoPI = value;
                this.DatiFondoPI_GetEffettuata = true;
            }
        }

        public List<GestioneFondo.DatiFondoPI> ListaDatiFondoPI
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiFondoPI_GetEffettuata)
                {
                    this.DatiFondoPI_GetEffettuata = true;
                    List<GestioneFondo.DatiFondoPI> fondoPI = null;
                    GestioneFondo.GetFondoPIRecordFondoByIdPensione(this.DatiPensione.Id, out fondoPI);
                    this._ListaDatiFondoPI = fondoPI;
                }
                return this._ListaDatiFondoPI;
            }
            set
            {
                this._ListaDatiFondoPI = value;
                this.ListaDatiFondoPI_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoGAS DatiFondoGAS
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoGAS_GetEffettuata)
                {
                    this.DatiFondoGAS_GetEffettuata = true;
                    GestioneFondo.DatiFondoGAS fondoGAS = null;
                    GestioneFondo.GetFondoGASByIdPensione(this.DatiPensione.Id, out fondoGAS);
                    this._DatiFondoGAS = fondoGAS;
                }
                return this._DatiFondoGAS;
            }
            set
            {
                this._DatiFondoGAS = value;
                this.DatiFondoGAS_GetEffettuata = true;
            }
        }


        public GestioneFondo.DatiFondoCL DatiFondoCL
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoCL_GetEffettuata)
                {
                    this.DatiFondoCL_GetEffettuata = true;
                    GestioneFondo.DatiFondoCL fondoCL = null;
                    GestioneFondo.GetFondoCLByIdPensione(this.DatiPensione.Id, out fondoCL);
                    this._DatiFondoCL = fondoCL;
                }
                return this._DatiFondoCL;
            }
            set
            {
                this._DatiFondoCL = value;
                this.DatiFondoCL_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoDZ DatiFondoDZ
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoDZ_GetEffettuata)
                {
                    this.DatiFondoDZ_GetEffettuata = true;
                    GestioneFondo.DatiFondoDZ fondoDZ = null;
                    GestioneFondo.GetFondoDZByIdPensione(this.DatiPensione.Id, out fondoDZ);
                    this._DatiFondoDZ = fondoDZ;
                }
                return this._DatiFondoDZ;
            }
            set
            {
                this._DatiFondoDZ = value;
                this.DatiFondoDZ_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoES DatiFondoES
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoES_GetEffettuata)
                {
                    this.DatiFondoES_GetEffettuata = true;
                    GestioneFondo.DatiFondoES fondoES = null;
                    GestioneFondo.GetFondoESByIdPensione(this.DatiPensione.Id, out fondoES);
                    this._DatiFondoES = fondoES;
                }
                return this._DatiFondoES;
            }
            set
            {
                this._DatiFondoES = value;
                this.DatiFondoES_GetEffettuata = true;
            }
        }

        public GestioneFondo.DatiFondoPM DatiFondoPM
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiFondoPM_GetEffettuata)
                {
                    this.DatiFondoPM_GetEffettuata = true;
                    GestioneFondo.DatiFondoPM fondoPM = null;
                    GestioneFondo.GetFondoPMByIdPensione(this.DatiPensione.Id, out fondoPM);
                    this._DatiFondoPM = fondoPM;
                }
                return this._DatiFondoPM;
            }
            set
            {
                this._DatiFondoPM = value;
                this.DatiFondoPM_GetEffettuata = true;
            }
        }

        public List<GestionePensioneINPDAP.DatiPensioneINPDAP> ListaDatiPensioneINPDAP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiPensioneINPDAP_GetEffettuata)
                {
                    this.ListaDatiPensioneINPDAP_GetEffettuata = true;
                    List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiPensioneINPDAP = null;
                    GestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(this.DatiPensione.Id, out listaDatiPensioneINPDAP);
                    this._ListaDatiPensioneINPDAP = listaDatiPensioneINPDAP;
                }
                return this._ListaDatiPensioneINPDAP;
            }
            set
            {
                this._ListaDatiPensioneINPDAP = value;
                this.ListaDatiPensioneINPDAP_GetEffettuata = true;
            }
        }

        public List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> ListaDatiRipartizioneINPDAP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRipartizioneINPDAP_GetEffettuata)
                {
                    this.ListaDatiRipartizioneINPDAP_GetEffettuata = true;
                    List<GestioneRipartizioneINPDAP.DatiRipartizioneINPDAP> listaRipartizioneINPDAP = null;
                    GestioneRipartizioneINPDAP.GetRipartizioneINPDAPByIdPensione(this.DatiPensione.Id, out listaRipartizioneINPDAP);
                    this._ListaDatiRipartizioneINPDAP = listaRipartizioneINPDAP;
                }

                return this._ListaDatiRipartizioneINPDAP;
            }
            set
            {
                this._ListaDatiRipartizioneINPDAP = value;
                this.ListaDatiRipartizioneINPDAP_GetEffettuata = true;
            }

        }

        public List<GestioneCalcolo.DatiCalcoloContributivo> ListaDatiCalcoloContributivoRecordFondo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiCalcoloContributivoRecordFondo_GetEffettuata)
                {
                    this.ListaDatiCalcoloContributivoRecordFondo_GetEffettuata = true;
                    List<GestioneCalcolo.DatiCalcoloContributivo> ldaticalcolocontributivo = null;
                    GestioneCalcolo.GetCalcoloContributivoRecordFondoByIdPensione(this.DatiPensione.Id, out ldaticalcolocontributivo);
                    this._ListaDatiCalcoloContributivoRecordFondo = ldaticalcolocontributivo;
                }
                return this._ListaDatiCalcoloContributivoRecordFondo;
            }
            set
            {
                this._ListaDatiCalcoloContributivoRecordFondo = value;
                this.ListaDatiCalcoloContributivoRecordFondo_GetEffettuata = true;
            }

        }

        public List<GestioneDatiServizioUtileINPDAP.ServizioUtile> ListaDatiServizioUtileINPDAP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiServizioUtileINPDAP_GetEffettuata)
                {
                    this.ListaDatiServizioUtileINPDAP_GetEffettuata = true;
                    List<GestioneDatiServizioUtileINPDAP.ServizioUtile> listaDatiServizioUtileINPDAP = null;
                    GestioneDatiServizioUtileINPDAP.GetDatiServizioUtileByIdPensione(this.DatiPensione.Id, out listaDatiServizioUtileINPDAP);
                    this._ListaDatiServizioUtileINPDAP = listaDatiServizioUtileINPDAP;
                }

                return this._ListaDatiServizioUtileINPDAP;
            }
            set
            {

                this._ListaDatiServizioUtileINPDAP = value;
                this.ListaDatiServizioUtileINPDAP_GetEffettuata = true;
            }
        }

        public List<GestioneCalcolo.ServizioUtileINPDAP707> ListaDatiServizioUtile707INPDAP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiServizioUtile707INPDAP_GetEffettutata)
                {
                    this.ListaDatiServizioUtile707INPDAP_GetEffettutata = true;
                    List<GestioneCalcolo.ServizioUtileINPDAP707> listaDatiServizioUtile707 = null;
                    GestioneCalcolo.GetDatiServizioUtileINPDAP707ByIdPensione(this.DatiPensione.Id, out listaDatiServizioUtile707);
                    this._ListaDatiServizioUtile707INPDAP = listaDatiServizioUtile707;
                }

                return this._ListaDatiServizioUtile707INPDAP;
            }

            set
            {
                this._ListaDatiServizioUtile707INPDAP = value;
                this.ListaDatiServizioUtile707INPDAP_GetEffettutata = true;
            }
        }

        public List<GestioneRecordFondo.DatiRecordFondo> ListaDatiRecordFondo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiRecordFondo_GetEffettuata)
                {
                    this.ListaDatiRecordFondo_GetEffettuata = true;
                    List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo = null;
                    GestioneRecordFondo.GetRecordFondoByIdPensione(this.DatiPensione.Id, out listaDatiRecordFondo);
                    this._ListaDatiRecordFondo = listaDatiRecordFondo;
                }

                return this._ListaDatiRecordFondo;
            }
            set
            {
                this._ListaDatiRecordFondo = value;
                this.ListaDatiRecordFondo_GetEffettuata = true;
            }
        }


        public List<GestioneDatiServizioUtile.ServizioUtile> ListaDatiServizioUtile
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiServizioUtile_GetEffettuata)
                {
                    this.ListaDatiServizioUtile_GetEffettuata = true;
                    List<GestioneDatiServizioUtile.ServizioUtile> listaDatiServizioUtile = null;
                    GestioneDatiServizioUtile.GetDatiServizioUtileByIdPensione(this.DatiPensione.Id, out listaDatiServizioUtile);
                    this._ListaDatiServizioUtile = listaDatiServizioUtile;
                }

                return this._ListaDatiServizioUtile;
            }

            set
            {
                this._ListaDatiServizioUtile = value;
                this.ListaDatiServizioUtile_GetEffettuata = true;
            }
        }

        public List<GestioneCalcolo.ServizioUtile707> ListaDatiServizioUtile707
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiServizioUtile707_GetEffettuata)
                {
                    this.ListaDatiServizioUtile707_GetEffettuata = true;
                    List<GestioneCalcolo.ServizioUtile707> listaDatiServizioUtile707 = null;
                    GestioneCalcolo.GetDatiServizioUtile707ByIdPensione(this.DatiPensione.Id, out listaDatiServizioUtile707);
                    this._ListaDatiServizioUtile707 = listaDatiServizioUtile707;
                }

                return this._ListaDatiServizioUtile707;

            }
            set
            {
                this._ListaDatiServizioUtile707 = value;
                this.ListaDatiServizioUtile707_GetEffettuata = true;
            }
        }

        public List<GestioneFondo.DatiFondoPT> ListaDatiFondoPT
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiFondoPT_GetEffettuata)
                {
                    this.ListaDatiFondoPT_GetEffettuata = true;
                    List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
                    GestioneFondo.GetFondoPTRecordFondoByIdPensione(this.DatiPensione.Id, out listaDatiFondoPT);
                    this._ListaDatiFondoPT = listaDatiFondoPT;
                }
                return this._ListaDatiFondoPT;
            }
            set
            {
                this._ListaDatiFondoPT = value;
                this.ListaDatiFondoPT_GetEffettuata = true;
            }
        }

        public List<GestioneFondo.DatiFondoFST> ListaDatiFondoFST
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiFondoFST_GetEffettuata)
                {
                    this.ListaDatiFondoFST_GetEffettuata = true;
                    List<GestioneFondo.DatiFondoFST> listaDatiFondoFST = null;
                    GestioneFondo.GetFondoFSRecordFondoByIdPensione(this.DatiPensione.Id, out listaDatiFondoFST);
                    this._ListaDatiFondoFST = listaDatiFondoFST;
                }
                return this._ListaDatiFondoFST;
            }
            set
            {
                this._ListaDatiFondoFST = value;
                this.ListaDatiFondoFST_GetEffettuata = true;
            }
        }

        public long IdFondoPensione
        {
            get
            {
                if (this.DatiPensione == null)
                    return 0;

                if (!this.IdFondoPensione_GetEffettuata)
                {
                    this.IdFondoPensione_GetEffettuata = true;
                    long idFondo = 0;
                    GestioneFondo.GetIdFondoByIdPensione(this.DatiPensione.Id, out idFondo);
                    this._IdFondoPensione = idFondo;
                }
                return this._IdFondoPensione;
            }
            set
            {
                this._IdFondoPensione = value;
                this.IdFondoPensione_GetEffettuata = true;
            }
        }

        public GestioneDL407.DatiDL407 Dl407
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.Dl407_GetEffettuata)
                {
                    this.Dl407_GetEffettuata = true;
                    GestioneDL407.DatiDL407 dl407 = null;
                    GestioneDL407.GetDL407ByIdPensione(this.DatiPensione.Id, out dl407);
                    this._Dl407 = dl407;
                }
                return this._Dl407;
            }

            set
            {
                this._Dl407 = value;
                this.Dl407_GetEffettuata = true;
            }
        }

        public List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> ListaRecordDatiFondoINPDAP
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaRecordDatiFondoINPDAP_GetEffettuata)
                {
                    this.ListaRecordDatiFondoINPDAP_GetEffettuata = true;
                    List<GestioneRecordDatiFondoINPDAP.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP = null;
                    GestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(this.DatiPensione.Id, out listaRecordDatiFondoINPDAP);
                    this._ListaRecordDatiFondoINPDAP = listaRecordDatiFondoINPDAP;
                }

                return this._ListaRecordDatiFondoINPDAP;
            }
            set
            {
                this._ListaRecordDatiFondoINPDAP = value;
                this.ListaRecordDatiFondoINPDAP_GetEffettuata = true;
            }
        }

        public object ObjectFondoXX
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ObjectFondoXX_GetEffettuata)
                {
                    this.ObjectFondoXX_GetEffettuata = true;
                    object objectFondoXX = null;
                    GestioneFondo.GetFondoXXByDatiPensione(this.DatiPensione, out objectFondoXX);
                    this._ObjectFondoXX = objectFondoXX;
                }
                return this._ObjectFondoXX;
            }
            set
            {
                this._ObjectFondoXX = value;
                this.ObjectFondoXX_GetEffettuata = true;
            }
        }

        //ENG - Memo 32_a/2018
        public List<DatiSupplementiCumulo> ListaDatiSupplementiCumuloStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementiCumuloStorico_GetEffettuata)
                {
                    this.ListaDatiSupplementiCumuloStorico_GetEffettuata = true;
                    List<DatiSupplementiCumulo> listaDatiSupplementiCumuloStorico = null;
                    GestioneSupplementi.GetSupplementiCumuloStoricoByIdPensione(this.DatiPensione.Id, out listaDatiSupplementiCumuloStorico);
                    this._ListaDatiSupplementiCumuloStorico = listaDatiSupplementiCumuloStorico;
                }

                return this._ListaDatiSupplementiCumuloStorico;
            }
            set
            {
                this._ListaDatiSupplementiCumuloStorico = value;
                this.ListaDatiSupplementiCumuloStorico_GetEffettuata = true;
            }
        }

        public List<DatiSupplementi> ListaDatiSupplementiStorico
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiSupplementiStorico_GetEffettuata)
                {
                    this.ListaDatiSupplementiStorico_GetEffettuata = true;
                    List<DatiSupplementi> listaDatiSupplementiStorico = null;
                    GestioneSupplementi.GetSupplementiStoricoByIdPensione(this.DatiPensione.Id, out listaDatiSupplementiStorico);
                    this._ListaDatiSupplementiStorico = listaDatiSupplementiStorico;
                }

                return this._ListaDatiSupplementiStorico;
            }
            set
            {
                this._ListaDatiSupplementiStorico = value;
                this.ListaDatiSupplementiStorico_GetEffettuata = true;
            }
        }

        #region GestioneQuadri
        public GestioneQuadri.DatiQuadroLiquidazionePensione DatiQuadroLiquidazionePensione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroLiquidazionePensione_GetEffettuata)
                {
                    this.DatiQuadroLiquidazionePensione_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiquidazionePensione = null;
                    GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(this.DatiPensione, out datiQuadroLiquidazionePensione);
                    this._DatiQuadroLiquidazionePensione = datiQuadroLiquidazionePensione;
                }

                return _DatiQuadroLiquidazionePensione;
            }
            set
            {
                _DatiQuadroLiquidazionePensione = value;
                DatiQuadroLiquidazionePensione_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroEliminazione DatiQuadroEliminazione
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroEliminazione_GetEffettuata)
                {
                    this.DatiQuadroEliminazione_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroEliminazione datiQuadroEliminazione = null;
                    GestioneQuadri.GetQuadroEliminazioneByDatiPensione(this.DatiPensione, out datiQuadroEliminazione);
                    this._DatiQuadroEliminazione = datiQuadroEliminazione;
                }

                return _DatiQuadroEliminazione;
            }
            set
            {
                _DatiQuadroEliminazione = value;
                DatiQuadroEliminazione_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroDatiContributivi DatiQuadroDatiContributivi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroDatiContributivi_GetEffettuata)
                {
                    this.DatiQuadroDatiContributivi_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroDatiContributivi datiQuadroDatiContributivi = null;
                    GestioneQuadri.GetQuadroDatiContributiviByDatiPensione(this.DatiPensione, out datiQuadroDatiContributivi);
                    this._DatiQuadroDatiContributivi = datiQuadroDatiContributivi;
                }

                return _DatiQuadroDatiContributivi;
            }
            set
            {
                _DatiQuadroDatiContributivi = value;
                DatiQuadroDatiContributivi_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroMaggiorazioniBenefici DatiQuadroMaggiorazioniBenefici
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroMaggiorazioniBenefici_GetEffettuata)
                {
                    this.DatiQuadroMaggiorazioniBenefici_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroMaggiorazioniBenefici datiQuadroMaggiorazioniBenefici = null;
                    GestioneQuadri.GetQuadroMaggiorazioniBeneficiByDatiPensione(this.DatiPensione, out datiQuadroMaggiorazioniBenefici);
                    this._DatiQuadroMaggiorazioniBenefici = datiQuadroMaggiorazioniBenefici;
                }

                return _DatiQuadroMaggiorazioniBenefici;
            }
            set
            {
                _DatiQuadroMaggiorazioniBenefici = value;
                DatiQuadroMaggiorazioniBenefici_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroOneri DatiQuadroOneri
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroOneri_GetEffettuata)
                {
                    this.DatiQuadroOneri_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroOneri datiQuadroOneri = null;
                    GestioneQuadri.GetQuadroOneriByDatiPensione(this.DatiPensione, out datiQuadroOneri);
                    this._DatiQuadroOneri = datiQuadroOneri;
                }

                return _DatiQuadroOneri;
            }
            set
            {
                _DatiQuadroOneri = value;
                DatiQuadroOneri_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroBititolarita DatiQuadroBititolarita
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroBititolarita_GetEffettuata)
                {
                    this.DatiQuadroBititolarita_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroBititolarita datiQuadroBititolarita = null;
                    GestioneQuadri.GetQuadroBititolaritaByDatiPensione(this.DatiPensione, out datiQuadroBititolarita);
                    this._DatiQuadroBititolarita = datiQuadroBititolarita;
                }

                return _DatiQuadroBititolarita;
            }
            set
            {
                _DatiQuadroBititolarita = value;
                DatiQuadroBititolarita_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroSupplementi DatiQuadroSupplementi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroSupplementi_GetEffettuata)
                {
                    this.DatiQuadroSupplementi_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
                    GestioneQuadri.GetQuadroSupplementiByDatiPensione(this.DatiPensione, out datiQuadroSupplementi);
                    this._DatiQuadroSupplementi = datiQuadroSupplementi;
                }

                return _DatiQuadroSupplementi;
            }
            set
            {
                _DatiQuadroSupplementi = value;
                DatiQuadroSupplementi_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroDetrazioni DatiQuadroDetrazioni
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroDetrazioni_GetEffettuata)
                {
                    this.DatiQuadroDetrazioni_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroDetrazioni datiQuadroDetrazioni = null;
                    GestioneQuadri.GetQuadroDetrazioniByDatiPensione(this.DatiPensione, out datiQuadroDetrazioni);
                    this._DatiQuadroDetrazioni = datiQuadroDetrazioni;
                }

                return _DatiQuadroDetrazioni;
            }
            set
            {
                _DatiQuadroDetrazioni = value;
                DatiQuadroDetrazioni_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroRedditi DatiQuadroRedditi
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroRedditi_GetEffettuata)
                {
                    this.DatiQuadroRedditi_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroRedditi datiQuadroRedditi = null;
                    GestioneQuadri.GetQuadroRedditiByIdPensione(this.DatiPensione, out datiQuadroRedditi);
                    this._DatiQuadroRedditi = datiQuadroRedditi;
                }

                return _DatiQuadroRedditi;
            }
            set
            {
                _DatiQuadroRedditi = value;
                DatiQuadroRedditi_GetEffettuata = true;
            }
        }
        public GestioneQuadri.DatiQuadroFamiliari DatiQuadroFamiliari
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroFamiliari_GetEffettuata)
                {
                    this.DatiQuadroFamiliari_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroFamiliari datiQuadroFamiliari = null;
                    GestioneQuadri.GetQuadroFamiliariByDatiPensione(this.DatiPensione, out datiQuadroFamiliari);
                    this._DatiQuadroFamiliari = datiQuadroFamiliari;
                }

                return _DatiQuadroFamiliari;
            }
            set
            {
                _DatiQuadroFamiliari = value;
                DatiQuadroFamiliari_GetEffettuata = true;
            }
        }

        public GestioneQuadri.DatiQuadroTitolare DatiQuadroTitolare
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroTitolare_GetEffettuata)
                {
                    this.DatiQuadroTitolare_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
                    GestioneQuadri.GetQuadroTitolareByDatiPensione(this.DatiPensione, out datiQuadroTitolare);
                    this._DatiQuadroTitolare = datiQuadroTitolare;
                }

                return _DatiQuadroTitolare;
            }
            set
            {
                _DatiQuadroTitolare = value;
                DatiQuadroTitolare_GetEffettuata = true;
            }
        }

        public GestioneQuadri.DatiQuadroRichiestaBonus DatiQuadroRichiestaBonus
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroRichiestaBonus_GetEffettuata)
                {
                    this.DatiQuadroRichiestaBonus_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroRichiestaBonus datiQuadroRichiestaBonus = null;
                    GestioneQuadri.GetQuadroRichiestaBonusByDatiPensione(this.DatiPensione, out datiQuadroRichiestaBonus);
                    this._DatiQuadroRichiestaBonus = datiQuadroRichiestaBonus;
                }

                return _DatiQuadroRichiestaBonus;
            }
            set
            {
                _DatiQuadroRichiestaBonus = value;
                DatiQuadroRichiestaBonus_GetEffettuata = true;
            }
        }

        public GestioneQuadri.DatiQuadroDatiNoCalcolo DatiQuadroNoCalcolo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.DatiQuadroNoCalcolo_GetEffettuata)
                {
                    this.DatiQuadroNoCalcolo_GetEffettuata = true;
                    GestioneQuadri.DatiQuadroDatiNoCalcolo datiQuadroNoCalcolo = null;
                    GestioneQuadri.GetQuadroDatiNoCalcoloByDatiPensione(this.DatiPensione, out datiQuadroNoCalcolo);
                    this._DatiQuadroNoCalcolo = datiQuadroNoCalcolo;
                }

                return this._DatiQuadroNoCalcolo;
            }
            set
            {
                this._DatiQuadroNoCalcolo = value;
                this.DatiQuadroNoCalcolo_GetEffettuata = true;
            }
        }

        public List<GestioneQuadri.DatiQuadroDatiRecordFondo> ListaDatiQuadroDatiRecordFondo
        {
            get
            {
                if (this.DatiPensione == null)
                    return null;

                if (!this.ListaDatiQuadroDatiRecordFondo_GetEffettuata)
                {
                    this.ListaDatiQuadroDatiRecordFondo_GetEffettuata = true;
                    List<GestioneQuadri.DatiQuadroDatiRecordFondo> listaDatiQuadroDatiRecordFondo = null;
                    GestioneQuadri.GetQuadroDatiRecordFondoByDatiPensione(this.DatiPensione, out listaDatiQuadroDatiRecordFondo);
                    this._ListaDatiQuadroDatiRecordFondo = listaDatiQuadroDatiRecordFondo;
                }

                return this._ListaDatiQuadroDatiRecordFondo;
            }
            set
            {
                this._ListaDatiQuadroDatiRecordFondo = value;
                this.ListaDatiQuadroDatiRecordFondo_GetEffettuata = true;
            }
        }


        #endregion GestioneQuadri

        #region Dati Flat
        public bool IsRiaperturaDomanda
        {
            get
            {
                if (!this._IsRiaperturaDomanda.HasValue)
                    this._IsRiaperturaDomanda = Utility.IsRiaperturaDomanda(DatiLavorazione != null ? DatiLavorazione.CodFase : null);
                return _IsRiaperturaDomanda.GetValueOrDefault();
            }
            set
            {
                this._IsRiaperturaDomanda = value;
            }
        }

        public Utility.TipoCalcolo TipoCalcolo
        {
            get
            {
                if (!this._TipoCalcolo.HasValue)
                    this._TipoCalcolo = Utility.GetTipoCalcolo(DatiPensione);
                return this._TipoCalcolo.GetValueOrDefault();
            }
            set
            {
                this._TipoCalcolo = value;
            }
        }

        public Utility.TipoAppartenenza? TipoAppartenenza
        {
            get
            {
                if (!this._TipoAppartenenza.HasValue)
                {
                    if (DatiPensione != null)
                        this._TipoAppartenenza = Utility.GetTipoAppartenenza(DatiPensione.IndConvInt, DatiPensione.Gestione);
                }
                return this._TipoAppartenenza;
            }
            set
            {
                this._TipoAppartenenza = value;
            }
        }

        public string Tipologia
        {
            get
            {
                if (string.IsNullOrEmpty(this._Tipologia))
                {
                    if (TipoAppartenenza.HasValue)
                    {
                        switch (TipoAppartenenza.Value)
                        {
                            case Utility.TipoAppartenenza.FS:
                                this._Tipologia = "FS";
                                break;
                            case Utility.TipoAppartenenza.AGO:
                                this._Tipologia = "AGO";
                                break;
                            case Utility.TipoAppartenenza.CI:
                                this._Tipologia = "CI";
                                break;
                        }
                    }
                }
                return this._Tipologia;
            }
            set
            {
                this._Tipologia = value;
            }
        }
        #endregion Dati Flat
        #endregion oggetti

        // Le variabili servono a capire se è stata già effettuata la get del relativo oggetto
        #region variabili booleane
        public bool DatiMaggiorazioniBenefici_GetEffettuata { get; set; }
        public bool DatiIstruttoria_GetEffettuata { get; set; }
        public bool DatiAnagraficiTitolare_GetEffettuata { get; set; }
        public bool DatiEnpals_GetEffettuata { get; set; }
        public bool DatiCalcoloRetributivoENPALS_GetEffettuata { get; set; }
        public bool DatiCalcoloRetributivoENPALSStorico_GetEffettuata { get; set; }
        public bool DatiCalcoloContributivoENPALS_GetEffettuata { get; set; }
        public bool DatiCalcoloContributivoENPALSStorico_GetEffettuata { get; set; }
        public bool DatiPensioniDatiGenerici_GetEffettuata { get; set; }
        public bool DatiDetrazioni_GetEffettuata { get; set; }
        public bool DatiDetrazioniStorico_GetEffettuata { get; set; }
        public bool DatiDanteCausa_GetEffettuata { get; set; }
        public bool ListaDatiRetributivi_GetEffettuata { get; set; }
        public bool ListaDatiRetributiviStorico_GetEffettuata { get; set; }
        public bool DatiRetributivi_GetEffettuata { get; set; }
        public bool ListaDatiContributivi_GetEffettuata { get; set; }
        public bool ListaDatiContributiviStorico_GetEffettuata { get; set; }
        public bool DatiContributivi_GetEffettuata { get; set; }
        public bool ListaDatiRetributiviINPGI_GetEffettuata { get; set; }
        public bool ListaDatiRetributiviINPGIStorico_GetEffettuata { get; set; }
        public bool ListaDatiContributiviINPGI_GetEffettuata { get; set; }
        public bool ListaDatiContributiviINPGIStorico_GetEffettuata { get; set; }
        public bool DatiBeneficioVittimeTerrorismo_GetEffettuata { get; set; }
        public bool DatiControlloFelpe_GetEffettuata { get; set; }
        public bool DatiPagamento_GetEffettuata { get; set; }
        public bool DatiNuoveLiquidate_GetEffettuata { get; set; }
        public bool DatiInabilita_GetEffettuata { get; set; }
        public bool ListaDatiPensioniINAIL_GetEffettuata { get; set; }
        public bool DatiStoricoGP_GetEffettuata { get; set; }
        public bool ListaDatiSentenzaArt4_GetEffettuata { get; set; }
        public bool ListaDatiSentenze_GetEffettuata { get; set; }
        public bool DatiPrepensionamento_GetEffettuata { get; set; }
        public bool DatiEliminazione_GetEffettuata { get; set; }
        public bool ListaDatiCalcoloVittimeTerrorismo_GetEffettuata { get; set; }
        public bool DatiLavorazione_GetEffettuata { get; set; }
        public bool ListaDatiOneri_GetEffettuata { get; set; }
        public bool ListaFamiliari_GetEffettuata { get; set; }
        public bool ListaAnagraficaFamiliari_GetEffettuata { get; set; }
        public bool ListaCodMaggFamiliari_GetEffettuata { get; set; }
        public bool ListaDatiSupplementi_GetEffettuata { get; set; }
        public bool ListaDatiSupplementiNoStorico_GetEffettuata { get; set; }
        public bool DatiSupplementiBase_GetEffettuata { get; set; }
        public bool DatiIntegrazioneArt11_GetEffettuata { get; set; }
        public bool ListaQuotePensione_GetEffettuata { get; set; }
        public bool ListaQuotePensioneStorico_GetEffettuata { get; set; }
        public bool ListaAltraPensione_GetEffettuata { get; set; }
        public bool ListaDatiRipartizioneFondi_GetEffettuata { get; set; }
        public bool ListaDatiBeneficiParticolari_GetEffettuata { get; set; }
        public bool ListaDatiSuppRecordENPALS_GetEffettuata { get; set; }
        public bool ListaDatiSupplementiENPALS_GetEffettuata { get; set; }
        public bool DatiSindacato_GetEffettuata { get; set; }
        public bool DatiAnagraficiDanteCausa_GetEffettuata { get; set; }
        public bool ListaDatiRedditoSentenza495_93_GetEffettuata { get; set; }
        public bool ListaRedditoDRedd_GetEffettuata { get; set; }
        public bool DatiFondo_GetEffettuata { get; set; }
        public bool DatiFondoVL_GetEffettuata { get; set; }
        public bool ListaTrattenuteQuotePensione_GetEffettuata { get; set; }
        public bool ListaTrattenuteQuotePensioneStorico_GetEffettuata { get; set; }
        public bool DatiAreaTitolare_GetEffettuata { get; set; }
        public bool DatiAnagraficiDelegato_GetEffettuata { get; set; }
        public bool DatiAnagraficiTutore_GetEffettuata { get; set; }
        public bool ListaAventiDiritto_GetEffettuata { get; set; }
        public bool ListaAnagraficaAventiDiritto_GetEffettuata { get; set; }
        public bool ListaPeriodoAventiDiritto_GetEffettuata { get; set; }
        public bool DatiPatronato_GetEffettuata { get; set; }
        public bool DatiContribuzioneEnpalsSAI_GetEffettuata { get; set; }
        public bool DatiContribuzioneEnpalsSAS_GetEffettuata { get; set; }
        public bool ListaDatiSupplementiCumulo_GetEffettuata { get; set; }
        public bool ListaRichiesteRicercaDomandeANF_GetEffettuata { get; set; }
        public bool ListaDatiQuotaFondoIntegrativo_GetEffettuata { get; set; }
        public bool ListaDatiQuotaFondoIntegrativoStorico_GetEffettuata { get; set; }
        public bool DatiFondoEL_GetEffettuata { get; set; }
        public bool DatiFondoET_GetEffettuata { get; set; }
        public bool DatiFondoTT_GetEffettuata { get; set; }
        public bool DatiFondoFS_GetEffettuata { get; set; }
        public bool DatiFondoPT_GetEffettuata { get; set; }
        public bool DatiFondoPI_GetEffettuata { get; set; }
        public bool DatiFondoGAS_GetEffettuata { get; set; }
        public bool DatiFondoCL_GetEffettuata { get; set; }
        public bool DatiFondoDZ_GetEffettuata { get; set; }
        public bool DatiFondoES_GetEffettuata { get; set; }
        public bool DatiFondoPM_GetEffettuata { get; set; }
        public bool ListaDatiPensioneINPDAP_GetEffettuata { get; set; }
        public bool ListaDatiRipartizioneINPDAP_GetEffettuata { get; set; }
        public bool ListaDatiCalcoloContributivoRecordFondo_GetEffettuata { get; set; }
        public bool ListaDatiServizioUtileINPDAP_GetEffettuata { get; set; }
        public bool ListaDatiServizioUtile707INPDAP_GetEffettutata { get; set; }
        public bool ListaDatiRecordFondo_GetEffettuata { get; set; }
        public bool ListaDatiServizioUtile_GetEffettuata { get; set; }
        public bool ListaDatiServizioUtile707_GetEffettuata { get; set; }
        public bool ListaDatiFondoPT_GetEffettuata { get; set; }
        public bool ListaDatiFondoFST_GetEffettuata { get; set; }
        public bool ListaDatiFondoPI_GetEffettuata { get; set; }
        public bool IdFondoPensione_GetEffettuata { get; set; }
        public bool Dl407_GetEffettuata { get; set; }
        public bool ListaRecordDatiFondoINPDAP_GetEffettuata { get; set; }
        public bool ObjectFondoXX_GetEffettuata { get; set; }
        //ENG - Memo 32_a/2018
        public bool ListaDatiSupplementiCumuloStorico_GetEffettuata { get; set; }
        public bool ListaDatiSupplementiStorico_GetEffettuata { get; set; }
        public bool ListaQuoteMiglioramentiContrattuali_GetEffettuata { get; set; }
        public bool ListaQuoteMiglioramentiContrattualiStorico_GetEffettuata { get; set; }

        #region GestioneQuadri
        public bool DatiQuadroLiquidazionePensione_GetEffettuata { get; set; }
        public bool DatiQuadroEliminazione_GetEffettuata { get; set; }
        public bool DatiQuadroDatiContributivi_GetEffettuata { get; set; }
        public bool DatiQuadroMaggiorazioniBenefici_GetEffettuata { get; set; }
        public bool DatiQuadroOneri_GetEffettuata { get; set; }
        public bool DatiQuadroBititolarita_GetEffettuata { get; set; }
        public bool DatiQuadroSupplementi_GetEffettuata { get; set; }
        public bool DatiQuadroDetrazioni_GetEffettuata { get; set; }
        public bool DatiQuadroRedditi_GetEffettuata { get; set; }
        public bool DatiQuadroFamiliari_GetEffettuata { get; set; }
        public bool DatiQuadroTitolare_GetEffettuata { get; set; }
        public bool DatiQuadroRichiestaBonus_GetEffettuata { get; set; }
        public bool DatiQuadroNoCalcolo_GetEffettuata { get; set; }
        public bool ListaDatiQuadroDatiRecordFondo_GetEffettuata { get; set; }

        #endregion GestioneQuadri
        #endregion variabili booleane
        #endregion public properties
    }
}
