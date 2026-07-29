using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon.Entity
{
    public class ContenitoreDecodifica
    {
        #region Constructors
        public ContenitoreDecodifica(ContenitoreObject contenitore)
        {
            this._Contenitore = contenitore;
        }
        #endregion Constructors

        #region private variables
        private ContenitoreObject _Contenitore;

        private List<GestioneDecodifica.DecodificaEnteCassaProfessionale> _ElencoDecodificaEnteCassaProfessionale;
        private List<GestioneDecodifica.GruppoOneri> _ElencoDecCodeGruppoOnere;
        private List<GestioneDecodifica.SottoGruppoOneri> _ElencoDecCodeSottoGruppoOnere;
        private List<GestioneBancheFideiussione.DecBancaFideiussione> _ElencoDecBancaFideiussione;
        private List<GestioneDecodifica.DecodificaTipoQuota> _ElencoDecodificaTipoQuota;
        private List<CtrlDecorrenzaRetrExINPDAI> _ElencoCtrlDecorrenzaRetrExINPDAI;
        private List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> _ElencoCodeGestioneCalcoloRetributivo;
        private List<GestioneDecodifica.CodeGestioneCalcoloContributivo> _ElencoCodeGestioneCalcoloContributivo;
        private List<GestioneDecodifica.DecodeEnte> _ElencoDecodificaEnte;
        private List<GestioneDecodifica.CatEnteAltraPensione> _ElencoCatEnteAltraPensione;
        private List<GestioneDecodifica.CodiceParticolare> _ElencoCodiceParticolare;
        private List<GestioneDecodifica.SoggettoBeneficiario> _ElencoSoggettoBeneficiario;
        private List<GestioneDecodificaAzienda.DecAzienda> _ElencoDecAzienda;
        private List<GestioneDecodificaAzienda.DecAzienda> _ElencoDecAziendaAll;
        private List<GestioneDecodifica.DecEnteGestioneFondo> _ElencoDecEnteGestioneFondo;
        private List<GestioneDecodifica.DecCodiceTrattenute> _ElencoDecCodiceTrattenute;
        private List<GestioneDecodifica.TipoCalcolo> _ElencoTipoCalcolo;
        private List<GestioneDecodifica.CodiceSpecifico> _ElencoCodiceSpecifico;
        private List<GestioneAziendeVOCRED_DAP.DecAziendeVOCRED_DAP> _ElencoDecAziendeVOCRED_DAP;
        private List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> _ElencoDecAziendeScadenzaAssegnoGGmmAAAA;
        private List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> _ElencoDecodAnagraficaAccordiPerTipo0179;
        private List<GestioneAnagraficaAccordiPerTipo0171.DecodAnagraficaAccordiPerTipo0171> _ElencoDecodAnagraficaAccordiPerTipo0171;
        private List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> _ElencoDecodAnagraficaAccordi;
        private List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> _ElencoDecodAnagraficaAccordiLetteraB;
        private List<GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179> _ElencoDecodAnagraficaAziendePerTipo0179;
        private List<GestioneAnagraficaAziendePerTipo0171.DecodAnagraficaAziendePerTipo0171> _ElencoDecodAnagraficaAziendePerTipo0171;
        private List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> _ElencoDecodAnagraficaAziendeLetteraB;
        private List<GestioneDecodifica.PensioneExInpdai> _ElencoPensioneExInpdai;
        private List<GestioneDecodifica.DecModalitaLiquidazione> _ElencoDecModalitaLiquidazione;
        private List<GestioneDecodifica.CodiceEliminazione> _ElencoCodiceEliminazione;
        private List<GestioneDecodifica.StatoEstero> _ElencoStatoEstero;
        private List<GestioneControlliDinamici.ControlloDinamico> _ElencoControlloDinamico;
        private List<GestioneDecodifica.SiglaFamiliare> _ElencoSiglaFamiliare;
        private List<GestioneDecodifica.CodMaggiorazioneFamiliari> _ElencoCodMaggiorazioneFamiliari;
        private List<GestioneDecodifica.CtrlEnteCassaCodiceGestione> _ElencoCtrlEnteCassaCodiceGestione;
        private List<GestioneDecodifica.CtrlCatAdeguata> _ElencoCtrlCatAdeguata;
        private List<GestioneDecodifica.CtrlTipoUfficio> _ElencoCtrlTipoUfficio;
        private List<GestioneBancheFideiussioneESPA.DecBancaFideiussione> _ElencoDecBancaFideiussioneESPA;
        private List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> _ElencoCodeGestioneQuotaFondoIntegrativo;
        private List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> _ElencoCodeGestioneQuotaFondoINPGI;
        private List<GestioneDecodifica.Cieco> _ElencoCodiceCieco;
        private List<GestioneDecodifica.SettimaneBeneficio> _ElencoTipoSettimaneBeneficioAGO_CI;
        private List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> _ElencoCodiceMaggiorazioneExCombattenti;
        private List<GestioneDecodifica.TipologiaPrestazione> _ElencoTipologiaPrestazione;
        private List<GestioneDecodifica.TipologiaBeneficioTerrorismo> _ElencoTipologiaBeneficioTerrorismo;
        private List<GestioneDecodifica.CDCMMR> _ElencoCDCMMR;
        private List<GestioneDecodifica.DecodificaLegge44997> _ElencoLegge44997;
        private List<GestioneDecodifica.DomandaRicorso> _ElencoDomandeRicorso;
        private List<GestioneDecodifica.Mobilita> _ElencoCodiceMobilita;
        private List<GestioneDecodifica.CodiciNatura> _ElencoCodiceNaturaAGO_CI;
        private List<GestioneDecodifica.DecRiconoscimentiInvalidita> _ElencoRiconoscimentoInvalidita;
        private List<GestioneDecodifica.DerogaENPALS> _ElencoDerogaENPALS;
        private List<GestioneAnagraficaAziende.DecodAnagraficaAziende> _ElencoAnagraficaAziende;
        private List<GestioneDecodifica.DecTipoCalcoloVincenteDAI> _ElencoTipoCalcoloVincenteDAI;
        private List<GestioneDecodifica.DecComparto> _ElencoDecComparto;
        private List<GestioneDecodifica.DecSettore> _ElencoDecSettore;
        private List<GestioneDecodifica.DecRuolo> _ElencoDecRuolo;
        private List<GestioneDecodifica.AttivitaSvolta> _ElencoAttivitaSvolte;
        private List<GestioneDecodifica.CodiceRequisito1> _ElencoCodiceRequisito1;
        private List<GestioneDecodifica.CodiceRequisito2> _ElencoCodiceRequisito2;
        private List<GestioneDecodifica.CodiceConvenzioneInternazionale> _ElencoCodiceConvenzioneInternazionale;
        private List<GestioneDecodifica.DecodificaCodiceArt22> _ElencoCodiceDecodificaArt22;
        private List<GestioneDecodifica.DecodificaCodiceCapitalizzazione> _ElencoCodiceDecodificaCapitalizzazione;
        private List<GestioneDecodifica.DecodificaCodeEsodo> _ElencoCodiceEsodo;
        private List<GestioneDecodifica.DecodificaPartTime> _ElencoCodiceDecodificaPartTime;
        private List<GestioneDecodifica.DecodificaCausaCessazione> _ElencoCodiceCausaCessazione;
        private List<GestioneDecodifica.DecodificaTipoLiquidazionePM> _ElencoTipoLiquidazionePM;
        private List<GestioneDecodifica.CodiceTipoLiquidazionePM> _ElencoCodiceTipoLiquidazionePM;
        private List<GestioneDecodifica.DecodificaLegge413> _ElencoCodiceLegge413;
        private List<GestioneDecodifica.DecodificaAttivitaSvolta2> _ElencoAttivitaSvolta2;
        private List<GestioneDecodifica.DecodificaTipoLiquidazione> _ElencoTipoLiquidazione;
        private List<GestioneDecodifica.CodiciNatura> _ElencoCodiciNaturaFS;
        private List<GestioneDecodifica.DecPersonaleViaggiante> _ElencoPersonaleViaggiante;
        private List<GestioneDecodifica.DecodificaEnteRipartizioneINPDAP> _ElencoDecodificaEnteRipartizioneINPDAP;
        private List<GestioneDecodifica.DecMicroqualificaINPDAP> _ElencoDecMicroqualificaNPDAP;
        private List<GestioneDecodifica.SettimaneBeneficio> _ElencoTipoBenefici;
        private List<GestioneDecodifica.DecPensioniPrivilegiate> _ElencoPensioniPrivilegiate;
        private List<GestioneDecodifica.ComunicazioneCampo3> _ElencoDecodificaComunicazioneCampo3;
        private List<GestioneDecodifica.DecodificaBanchePerSede> _ElencoDecodificaBanchePerSede;
        private List<GestioneDecodifica.CtrlScadenzaIndennizzoINDCOM> _ElencoCtrlScadenzaIndennizzoINDCOM;
        #endregion private variables

        #region public properties
        #region oggetti
        public List<GestioneDecodifica.DecodificaEnteCassaProfessionale> ElencoDecodificaEnteCassaProfessionale
        {
            get
            {
                if (!this.ElencoDecodificaEnteCassaProfessionale_GetEffettuata)
                {
                    this.ElencoDecodificaEnteCassaProfessionale_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionale = null;
                    GestioneDecodifica.GetDecodificaEnteCassaProfessionale(out elencoDecodificaEnteCassaProfessionale);
                    this._ElencoDecodificaEnteCassaProfessionale = elencoDecodificaEnteCassaProfessionale;
                }

                return _ElencoDecodificaEnteCassaProfessionale;
            }
            set
            {
                _ElencoDecodificaEnteCassaProfessionale = value;
            }
        }
        public List<GestioneDecodifica.GruppoOneri> ElencoDecCodeGruppoOnere
        {
            get
            {
                if (!this.ElencoDecCodeGruppoOnere_GetEffettuata)
                {
                    this.ElencoDecCodeGruppoOnere_GetEffettuata = true;
                    List<GestioneDecodifica.GruppoOneri> elencoDecCodeGruppoOnere = null;
                    GestioneDecodifica.GetGruppoOneri(out elencoDecCodeGruppoOnere);
                    this._ElencoDecCodeGruppoOnere = elencoDecCodeGruppoOnere;
                }

                return _ElencoDecCodeGruppoOnere;
            }
            set
            {
                _ElencoDecCodeGruppoOnere = value;
            }
        }
        public List<GestioneDecodifica.SottoGruppoOneri> ElencoDecCodeSottoGruppoOnere
        {
            get
            {
                if (!this.ElencoDecCodeSottoGruppoOnere_GetEffettuata)
                {
                    this.ElencoDecCodeSottoGruppoOnere_GetEffettuata = true;
                    List<GestioneDecodifica.SottoGruppoOneri> elencoDecCodeSottoGruppoOnere = null;
                    GestioneDecodifica.GetSottoGruppoOneri(out elencoDecCodeSottoGruppoOnere);
                    this._ElencoDecCodeSottoGruppoOnere = elencoDecCodeSottoGruppoOnere;
                }

                return _ElencoDecCodeSottoGruppoOnere;
            }
            set
            {
                _ElencoDecCodeSottoGruppoOnere = value;
            }
        }
        public List<GestioneBancheFideiussione.DecBancaFideiussione> ElencoDecBancaFideiussione
        {
            get
            {
                if (!this.ElencoDecBancaFideiussione_GetEffettuata)
                {
                    this.ElencoDecBancaFideiussione_GetEffettuata = true;
                    List<GestioneBancheFideiussione.DecBancaFideiussione> elencoDecBancaFideiussione = null;
                    GestioneBancheFideiussione.GetDecodificaBancaFideiussione(out elencoDecBancaFideiussione);
                    this._ElencoDecBancaFideiussione = elencoDecBancaFideiussione;
                }

                return _ElencoDecBancaFideiussione;
            }
            set
            {
                _ElencoDecBancaFideiussione = value;
            }
        }

        public List<GestioneDecodifica.DecodificaBanchePerSede> ElencoDecodificaBanchePerSede
        {
            get
            {
                if (!this.ElencoDecodificaBanchePerSede_GetEffettuata)
                {
                    this.ElencoDecodificaBanchePerSede_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaBanchePerSede> elencoDecodificaBanchePerSede = null;
                    GestioneDecodifica.GetDecodificaBanchePerSede(out elencoDecodificaBanchePerSede);
                    this._ElencoDecodificaBanchePerSede = elencoDecodificaBanchePerSede;
                }

                return _ElencoDecodificaBanchePerSede;
            }
            set
            {
                _ElencoDecodificaBanchePerSede = value;
            }
        }
        public List<GestioneDecodifica.DecodificaTipoQuota> ElencoDecodificaTipoQuota
        {
            get
            {
                if (!this.ElencoDecodificaTipoQuota_GetEffettuata)
                {
                    this.ElencoDecodificaTipoQuota_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaTipoQuota> elencoDecodificaTipoQuota = null;
                    GestioneDecodifica.GetDecodificaCodiceTipoQuota(out elencoDecodificaTipoQuota);
                    this._ElencoDecodificaTipoQuota = elencoDecodificaTipoQuota;
                }

                return _ElencoDecodificaTipoQuota;
            }
            set
            {
                _ElencoDecodificaTipoQuota = value;
            }
        }
        public List<CtrlDecorrenzaRetrExINPDAI> ElencoCtrlDecorrenzaRetrExINPDAI
        {
            get
            {
                if (!this.ElencoCtrlDecorrenzaRetrExINPDAI_GetEffettuata)
                {
                    this.ElencoCtrlDecorrenzaRetrExINPDAI_GetEffettuata = true;
                    List<CtrlDecorrenzaRetrExINPDAI> elencoCtrlDecorrenzaRetrExINPDAI = null;
                    GestioneCtrlDecorrenzaRetrExINPDAI.GetCtrlDecorrenzaRetrExINPDAI(out elencoCtrlDecorrenzaRetrExINPDAI);
                    this._ElencoCtrlDecorrenzaRetrExINPDAI = elencoCtrlDecorrenzaRetrExINPDAI;
                }

                return _ElencoCtrlDecorrenzaRetrExINPDAI;
            }
            set
            {
                _ElencoCtrlDecorrenzaRetrExINPDAI = value;
            }
        }
        public List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> ElencoCodeGestioneCalcoloRetributivo
        {
            get
            {
                if (!this.ElencoCodeGestioneCalcoloRetributivo_GetEffettuata)
                {
                    this.ElencoCodeGestioneCalcoloRetributivo_GetEffettuata = true;
                    List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetributivo);
                    this._ElencoCodeGestioneCalcoloRetributivo = elencoCodeGestioneCalcoloRetributivo;
                }

                return _ElencoCodeGestioneCalcoloRetributivo;
            }
            set
            {
                _ElencoCodeGestioneCalcoloRetributivo = value;
            }
        }
        public List<GestioneDecodifica.CtrlEnteCassaCodiceGestione> ElencoCtrlEnteCassaCodiceGestione
        {
            get
            {
                if (!this.ElencoCtrlEnteCassaCodiceGestione_GetEffettuata)
                {
                    this.ElencoCtrlEnteCassaCodiceGestione_GetEffettuata = true;
                    List<GestioneDecodifica.CtrlEnteCassaCodiceGestione> elencoCtrlEnteCassaCodiceGestione = null;
                    GestioneDecodifica.GetCtrlEnteCassaCodiceGestione(out elencoCtrlEnteCassaCodiceGestione);
                    this._ElencoCtrlEnteCassaCodiceGestione = elencoCtrlEnteCassaCodiceGestione;
                }

                return _ElencoCtrlEnteCassaCodiceGestione;
            }
            set
            {
                _ElencoCtrlEnteCassaCodiceGestione = value;
            }
        }
        public List<GestioneDecodifica.CtrlCatAdeguata> ElencoCtrlCatAdeguata
        {
            get
            {
                if (!this.ElencoCtrlCatAdeguata_GetEffettuata)
                {
                    this.ElencoCtrlCatAdeguata_GetEffettuata = true;
                    List<GestioneDecodifica.CtrlCatAdeguata> elencoCtrlCatAdeguata = null;
                    GestioneDecodifica.GetCtrlCatAdeguata(out elencoCtrlCatAdeguata);
                    this._ElencoCtrlCatAdeguata = elencoCtrlCatAdeguata;
                }

                return _ElencoCtrlCatAdeguata;
            }
            set
            {
                _ElencoCtrlCatAdeguata = value;
            }
        }
        public List<GestioneDecodifica.DecodeEnte> ElencoDecodificaEnte
        {
            get
            {
                if (!this.ElencoDecodificaEnte_GetEffettuata)
                {
                    this.ElencoDecodificaEnte_GetEffettuata = true;
                    List<GestioneDecodifica.DecodeEnte> elencoDecodificaEnte = null;
                    GestioneDecodifica.GetElencoEnte(out elencoDecodificaEnte);
                    this._ElencoDecodificaEnte = elencoDecodificaEnte;
                }
                return this._ElencoDecodificaEnte;
            }
            set
            {
                _ElencoDecodificaEnte = value;
            }
        }
        public List<GestioneDecodifica.CatEnteAltraPensione> ElencoCatEnteAltraPensione
        {
            get
            {
                if (!this.ElencoCatEnteAltraPensione_GetEffettuata)
                {
                    this.ElencoCatEnteAltraPensione_GetEffettuata = true;
                    List<GestioneDecodifica.CatEnteAltraPensione> elencoCatEnteAltraPensione = null;
                    GestioneDecodifica.GetCatEnteAltrePensioni(out elencoCatEnteAltraPensione);
                    this._ElencoCatEnteAltraPensione = elencoCatEnteAltraPensione;
                }
                return this._ElencoCatEnteAltraPensione;
            }
            set
            {
                _ElencoCatEnteAltraPensione = value;
            }
        }
        public List<GestioneDecodifica.CodiceParticolare> ElencoCodiceParticolare
        {
            get
            {
                if (!this.ElencoCodiceParticolare_GetEffettuata)
                {
                    this.ElencoCodiceParticolare_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceParticolare> elencoCodiceParticolare = null;
                    GestioneDecodifica.GetCodiciParticolari(out elencoCodiceParticolare);
                    this._ElencoCodiceParticolare = elencoCodiceParticolare;
                }

                return _ElencoCodiceParticolare;
            }
            set
            {
                _ElencoCodiceParticolare = value;
            }
        }
        public List<GestioneDecodifica.SoggettoBeneficiario> ElencoSoggettoBeneficiario
        {
            get
            {
                if (!this.ElencoSoggettoBeneficiario_GetEffettuata)
                {
                    this.ElencoSoggettoBeneficiario_GetEffettuata = true;
                    List<GestioneDecodifica.SoggettoBeneficiario> elencoSoggettoBeneficiario = null;
                    GestioneDecodifica.GetDecodificaSoggettoBeneficiario(out elencoSoggettoBeneficiario);
                    this._ElencoSoggettoBeneficiario = elencoSoggettoBeneficiario;
                }

                return _ElencoSoggettoBeneficiario;
            }
            set
            {
                _ElencoSoggettoBeneficiario = value;
            }
        }
        public List<GestioneDecodificaAzienda.DecAzienda> ElencoDecAzienda
        {
            get
            {
                if (!this.ElencoDecAzienda_GetEffettuata)
                {
                    this.ElencoDecAzienda_GetEffettuata = true;
                    List<GestioneDecodificaAzienda.DecAzienda> elencoDecAzienda = null;
                    if (Utility.IsRicostituzioneOrRiapertura(_Contenitore.DatiPensione, Utility.IsRiaperturaDomanda(_Contenitore.DatiPensione.Id)) && Utility.IsDomandaVOESO(_Contenitore.DatiPensione.SiglaCategoria))
                        GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(_Contenitore.DatiPensione.SiglaCategoria, null, out elencoDecAzienda);
                    else
                        GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(_Contenitore.DatiPensione.SiglaCategoria, _Contenitore.DatiPensione.Tipo, out elencoDecAzienda);
                    this._ElencoDecAzienda = elencoDecAzienda;
                }

                return _ElencoDecAzienda;
            }
            set
            {
                _ElencoDecAzienda = value;
                ElencoDecAzienda_GetEffettuata = true;
            }
        }
        public List<GestioneDecodificaAzienda.DecAzienda> ElencoDecAziendaAll
        {
            get
            {
                if (!this.ElencoDecAziendaAll_GetEffettuata)
                {
                    this.ElencoDecAziendaAll_GetEffettuata = true;
                    List<GestioneDecodificaAzienda.DecAzienda> elencoDecAziendaAll = null;
                    GestioneDecodificaAzienda.GetElencoAziendaAll(out elencoDecAziendaAll);
                    this._ElencoDecAziendaAll = elencoDecAziendaAll;
                }

                return _ElencoDecAziendaAll;
            }
            set
            {
                _ElencoDecAziendaAll = value;
                ElencoDecAziendaAll_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.DecEnteGestioneFondo> ElencoDecEnteGestioneFondo
        {
            get
            {
                if (!this.ElencoDecEnteGestioneFondo_GetEffettuata)
                {
                    this.ElencoDecEnteGestioneFondo_GetEffettuata = true;
                    List<GestioneDecodifica.DecEnteGestioneFondo> elencoDecEnteGestioneFondo = null;
                    GestioneDecodifica.GetDecEnteGestioneFondo(out elencoDecEnteGestioneFondo);
                    this._ElencoDecEnteGestioneFondo = elencoDecEnteGestioneFondo;
                }
                return this._ElencoDecEnteGestioneFondo;
            }
            set
            {
                this.ElencoDecEnteGestioneFondo_GetEffettuata = true;
                _ElencoDecEnteGestioneFondo = value;
            }
        }
        public List<GestioneDecodifica.DecCodiceTrattenute> ElencoDecCodiceTrattenute
        {
            get
            {
                if (!this.ElencoDecCodiceTrattenute_GetEffettuata)
                {
                    this.ElencoDecCodiceTrattenute_GetEffettuata = true;
                    List<GestioneDecodifica.DecCodiceTrattenute> elencoDecCodiceTrattenute = null;
                    GestioneDecodifica.GetDecCodiceTrattenute(out elencoDecCodiceTrattenute);
                    this._ElencoDecCodiceTrattenute = elencoDecCodiceTrattenute;
                }
                return this._ElencoDecCodiceTrattenute;
            }
            set
            {
                this.ElencoDecCodiceTrattenute_GetEffettuata = true;
                _ElencoDecCodiceTrattenute = value;
            }
        }
        public List<GestioneDecodifica.TipoCalcolo> ElencoTipoCalcolo
        {
            get
            {
                if (!this.ElencoTipoCalcolo_GetEffettuata)
                {
                    this.ElencoTipoCalcolo_GetEffettuata = true;
                    List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
                    GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
                    this._ElencoTipoCalcolo = elencoTipoCalcolo;
                }
                return this._ElencoTipoCalcolo;
            }
            set
            {
                this.ElencoTipoCalcolo_GetEffettuata = true;
                this._ElencoTipoCalcolo = value;
            }
        }
        public List<GestioneDecodifica.CodiceSpecifico> ElencoCodiceSpecifico
        {
            get
            {
                if (!this.ElencoCodiceSpecifico_GetEffettuata)
                {
                    this.ElencoCodiceSpecifico_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
                    GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
                    this._ElencoCodiceSpecifico = elencoCodiceSpecifico;
                }
                return this._ElencoCodiceSpecifico;
            }
            set
            {
                this.ElencoCodiceSpecifico_GetEffettuata = true;
                this._ElencoCodiceSpecifico = value;
            }
        }
        public List<GestioneAziendeVOCRED_DAP.DecAziendeVOCRED_DAP> ElencoDecAziendeVOCRED_DAP
        {
            get
            {
                if (!this.ElencoDecAziendeVOCRED_DAP_GetEffettuata)
                {
                    this.ElencoDecAziendeVOCRED_DAP_GetEffettuata = true;
                    List<GestioneAziendeVOCRED_DAP.DecAziendeVOCRED_DAP> elencoDecAziendeVOCRED_DAP = null;
                    GestioneAziendeVOCRED_DAP.GetDecodificaAziendeVOCRED_DAP(out elencoDecAziendeVOCRED_DAP);
                    this._ElencoDecAziendeVOCRED_DAP = elencoDecAziendeVOCRED_DAP;
                }
                return this._ElencoDecAziendeVOCRED_DAP;
            }
            set
            {
                _ElencoDecAziendeVOCRED_DAP = value;
                this.ElencoDecAziendeVOCRED_DAP_GetEffettuata = true;
            }
        }
        public List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> ElencoDecAziendeScadenzaAssegnoGGmmAAAA
        {
            get
            {
                if (!this.ElencoDecAziendeScadenzaAssegnoGGmmAAAA_GetEffettuata)
                {
                    this.ElencoDecAziendeScadenzaAssegnoGGmmAAAA_GetEffettuata = true;
                    List<GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA> elencoDecAziendeScadenzaAssegnoGGmmAAAA = null;
                    GestioneAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoDecAziendeScadenzaAssegnoGGmmAAAA);
                    this._ElencoDecAziendeScadenzaAssegnoGGmmAAAA = elencoDecAziendeScadenzaAssegnoGGmmAAAA;
                }
                return this._ElencoDecAziendeScadenzaAssegnoGGmmAAAA;
            }
            set
            {
                _ElencoDecAziendeScadenzaAssegnoGGmmAAAA = value;
                this.ElencoDecAziendeScadenzaAssegnoGGmmAAAA_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> ElencoDecodAnagraficaAccordiPerTipo0179
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAccordiPerTipo0179_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAccordiPerTipo0179_GetEffettuata = true;
                    List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> elencoDecodAnagraficaAccordiPerTipo0179 = null;
                    GestioneAnagraficaAccordiPerTipo0179.GetDecAnagraficaAccordi(out elencoDecodAnagraficaAccordiPerTipo0179);
                    this._ElencoDecodAnagraficaAccordiPerTipo0179 = elencoDecodAnagraficaAccordiPerTipo0179;
                }
                return this._ElencoDecodAnagraficaAccordiPerTipo0179;
            }
            set
            {
                this._ElencoDecodAnagraficaAccordiPerTipo0179 = value;
                this.ElencoDecodAnagraficaAccordiPerTipo0179_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAccordiPerTipo0171.DecodAnagraficaAccordiPerTipo0171> ElencoDecodAnagraficaAccordiPerTipo0171
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAccordiPerTipo0171_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAccordiPerTipo0171_GetEffettuata = true;
                    List<GestioneAnagraficaAccordiPerTipo0171.DecodAnagraficaAccordiPerTipo0171> elencoDecodAnagraficaAccordiPerTipo0171 = null;
                    GestioneAnagraficaAccordiPerTipo0171.GetDecAnagraficaAccordi(out elencoDecodAnagraficaAccordiPerTipo0171);
                    this._ElencoDecodAnagraficaAccordiPerTipo0171 = elencoDecodAnagraficaAccordiPerTipo0171;
                }
                return this._ElencoDecodAnagraficaAccordiPerTipo0171;
            }
            set
            {
                this._ElencoDecodAnagraficaAccordiPerTipo0171 = value;
                this.ElencoDecodAnagraficaAccordiPerTipo0171_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> ElencoDecodAnagraficaAccordi
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAccordi_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAccordi_GetEffettuata = true;
                    List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> elencoDecodAnagraficaAccordi = null;
                    GestioneAnagraficaAccordi.GetDecAnagraficaAccordi(out elencoDecodAnagraficaAccordi);
                    this._ElencoDecodAnagraficaAccordi = elencoDecodAnagraficaAccordi;
                }
                return this._ElencoDecodAnagraficaAccordi;
            }
            set
            {
                this._ElencoDecodAnagraficaAccordi = value;
                this.ElencoDecodAnagraficaAccordi_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> ElencoDecodAnagraficaAccordiLetteraB
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAccordiLetteraB_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAccordiLetteraB_GetEffettuata = true;
                    List<GestioneAnagraficaAccordiLetteraB.DecodAnagraficaAccordiLetteraB> elencoDecodAnagraficaAccordiLetteraB = null;
                    GestioneAnagraficaAccordiLetteraB.GetDecAnagraficaAccordi(out elencoDecodAnagraficaAccordiLetteraB);
                    this._ElencoDecodAnagraficaAccordiLetteraB = elencoDecodAnagraficaAccordiLetteraB;
                }
                return this._ElencoDecodAnagraficaAccordiLetteraB;
            }
            set
            {
                this._ElencoDecodAnagraficaAccordiLetteraB = value;
                this.ElencoDecodAnagraficaAccordiLetteraB_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179> ElencoDecodAnagraficaAziendePerTipo0179
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAziendePerTipo0179_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAziendePerTipo0179_GetEffettuata = true;
                    List<GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179> elencoDecodAnagraficaAziendePerTipo0179 = null;
                    GestioneAnagraficaAziendePerTipo0179.GetDecAnagraficaAziende(out elencoDecodAnagraficaAziendePerTipo0179);
                    this._ElencoDecodAnagraficaAziendePerTipo0179 = elencoDecodAnagraficaAziendePerTipo0179;
                }
                return this._ElencoDecodAnagraficaAziendePerTipo0179;
            }
            set
            {
                this._ElencoDecodAnagraficaAziendePerTipo0179 = value;
                this.ElencoDecodAnagraficaAziendePerTipo0179_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAziendePerTipo0171.DecodAnagraficaAziendePerTipo0171> ElencoDecodAnagraficaAziendePerTipo0171
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAziendePerTipo0171_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAziendePerTipo0171_GetEffettuata = true;
                    List<GestioneAnagraficaAziendePerTipo0171.DecodAnagraficaAziendePerTipo0171> elencoDecodAnagraficaAziendePerTipo0171 = null;
                    GestioneAnagraficaAziendePerTipo0171.GetDecAnagraficaAziende(out elencoDecodAnagraficaAziendePerTipo0171);
                    this._ElencoDecodAnagraficaAziendePerTipo0171 = elencoDecodAnagraficaAziendePerTipo0171;
                }
                return this._ElencoDecodAnagraficaAziendePerTipo0171;
            }
            set
            {
                this._ElencoDecodAnagraficaAziendePerTipo0171 = value;
                this.ElencoDecodAnagraficaAziendePerTipo0171_GetEffettuata = true;
            }
        }
        public List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> ElencoDecodAnagraficaAziendeLetteraB
        {
            get
            {
                if (!this.ElencoDecodAnagraficaAziendeLetteraB_GetEffettuata)
                {
                    this.ElencoDecodAnagraficaAziendeLetteraB_GetEffettuata = true;
                    List<GestioneAnagraficaAziendeLetteraB.DecodAnagraficaAziendeLetteraB> elencoDecodAnagraficaAziendeLetteraB = null;
                    GestioneAnagraficaAziendeLetteraB.GetDecAnagraficaAziende(out elencoDecodAnagraficaAziendeLetteraB);
                    this._ElencoDecodAnagraficaAziendeLetteraB = elencoDecodAnagraficaAziendeLetteraB;
                }
                return this._ElencoDecodAnagraficaAziendeLetteraB;
            }
            set
            {
                this._ElencoDecodAnagraficaAziendeLetteraB = value;
                this.ElencoDecodAnagraficaAziendeLetteraB_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.PensioneExInpdai> ElencoPensioneExInpdai
        {
            get
            {
                if (!this.ElencoPensioneExInpdai_GetEffettuata)
                {
                    this.ElencoPensioneExInpdai_GetEffettuata = true;
                    List<GestioneDecodifica.PensioneExInpdai> elencoPensioneExInpdai = null;
                    GestioneDecodifica.GetPensioniExInpdai(out elencoPensioneExInpdai);
                    this._ElencoPensioneExInpdai = elencoPensioneExInpdai;
                }
                return this._ElencoPensioneExInpdai;
            }
            set
            {
                this._ElencoPensioneExInpdai = value;
                this.ElencoPensioneExInpdai_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.DecModalitaLiquidazione> ElencoDecModalitaLiquidazione
        {
            get
            {
                if (!this.ElencoDecModalitaLiquidazione_GetEffettuata)
                {
                    this.ElencoDecModalitaLiquidazione_GetEffettuata = true;
                    List<GestioneDecodifica.DecModalitaLiquidazione> elencoDecModalitaLiquidazione = null;
                    GestioneDecodifica.GetElencoDecModalitaLiquidazione(out elencoDecModalitaLiquidazione);
                    this._ElencoDecModalitaLiquidazione = elencoDecModalitaLiquidazione;
                }
                return this._ElencoDecModalitaLiquidazione;
            }
            set
            {
                this._ElencoDecModalitaLiquidazione = value;
                this.ElencoDecModalitaLiquidazione_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.CodiceEliminazione> ElencoCodiceEliminazione
        {
            get
            {
                if (!this.ElencoCodiceEliminazione_GetEffettuata)
                {
                    this.ElencoCodiceEliminazione_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazione = null;
                    GestioneDecodifica.GetCodiceEliminazioneByTipologia(out elencoCodiceEliminazione, _Contenitore.TipoAppartenenza);
                    this._ElencoCodiceEliminazione = elencoCodiceEliminazione;
                }
                return this._ElencoCodiceEliminazione;
            }
            set
            {
                this._ElencoCodiceEliminazione = value;
                this.ElencoCodiceEliminazione_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.StatoEstero> ElencoStatoEstero
        {
            get
            {
                if (!this.ElencoStatoEstero_GetEffettuata)
                {
                    this.ElencoStatoEstero_GetEffettuata = true;
                    List<GestioneDecodifica.StatoEstero> elencoStatoEstero = null;
                    GestioneDecodifica.GetStatiEsteri(out elencoStatoEstero);
                    this._ElencoStatoEstero = elencoStatoEstero;
                }
                return this._ElencoStatoEstero;
            }
            set
            {
                this._ElencoStatoEstero = value;
                this.ElencoStatoEstero_GetEffettuata = true;
            }
        }
        public List<GestioneControlliDinamici.ControlloDinamico> ElencoControlloDinamico
        {
            get
            {
                if (!this.ElencoControlloDinamico_GetEffettuata)
                {
                    this.ElencoControlloDinamico_GetEffettuata = true;
                    List<GestioneControlliDinamici.ControlloDinamico> elencoControlloDinamico = null;
                    GestioneControlliDinamici.GetControlliDinamici(out elencoControlloDinamico);
                    this._ElencoControlloDinamico = elencoControlloDinamico;
                }
                return this._ElencoControlloDinamico;
            }
            set
            {
                this._ElencoControlloDinamico = value;
                this.ElencoControlloDinamico_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.SiglaFamiliare> ElencoSiglaFamiliare
        {
            get
            {
                if (!this.ElencoSiglaFamiliare_GetEffettuata)
                {
                    this.ElencoSiglaFamiliare_GetEffettuata = true;
                    List<GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliare = null;
                    GestioneDecodifica.GetSiglaFamiliareByTipologia(_Contenitore.Tipologia, out elencoSiglaFamiliare);
                    this._ElencoSiglaFamiliare = elencoSiglaFamiliare;
                }
                return this._ElencoSiglaFamiliare;
            }
            set
            {
                this._ElencoSiglaFamiliare = value;
                this.ElencoSiglaFamiliare_GetEffettuata = true;
            }
        }
        public List<GestioneDecodifica.CodMaggiorazioneFamiliari> ElencoCodMaggiorazioneFamiliari
        {
            get
            {
                if (!this.ElencoCodMaggiorazioneFamiliari_GetEffettuata)
                {
                    this.ElencoCodMaggiorazioneFamiliari_GetEffettuata = true;
                    List<GestioneDecodifica.CodMaggiorazioneFamiliari> elencoCodMaggiorazioneFamiliari = null;
                    GestioneDecodifica.GetCodMaggiorazioneFamiliari(_Contenitore.Tipologia, out elencoCodMaggiorazioneFamiliari);
                    this._ElencoCodMaggiorazioneFamiliari = elencoCodMaggiorazioneFamiliari;
                }
                return this._ElencoCodMaggiorazioneFamiliari;
            }
            set
            {
                this._ElencoCodMaggiorazioneFamiliari = value;
                this.ElencoCodMaggiorazioneFamiliari_GetEffettuata = true;
            }
        }

        public List<GestioneDecodifica.CodeGestioneCalcoloContributivo> ElencoCodeGestioneCalcoloContributivo
        {
            get
            {
                if (!this.ElencoCodeGestioneCalcoloContributivo_GetEffettuata)
                {
                    this.ElencoCodeGestioneCalcoloContributivo_GetEffettuata = true;
                    List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContributivo = null;
                    GestioneDecodifica.GetCodeGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContributivo);
                    this._ElencoCodeGestioneCalcoloContributivo = elencoCodeGestioneCalcoloContributivo;
                }

                return _ElencoCodeGestioneCalcoloContributivo;
            }
            set
            {
                _ElencoCodeGestioneCalcoloContributivo = value;
            }
        }

        public List<GestioneDecodifica.CtrlTipoUfficio> ElencoCtrlTipoUfficio
        {
            get
            {
                if (!this.ElencoCtrlTipoUfficio_GetEffettuata)
                {
                    this.ElencoCtrlTipoUfficio_GetEffettuata = true;
                    List<GestioneDecodifica.CtrlTipoUfficio> elencoCtrlTipoUfficio = null;
                    GestioneDecodifica.GetCtrlTipoUfficio(out elencoCtrlTipoUfficio);
                    this._ElencoCtrlTipoUfficio = elencoCtrlTipoUfficio;
                }

                return _ElencoCtrlTipoUfficio;
            }
            set
            {
                _ElencoCtrlTipoUfficio = value;
            }
        }

        public List<GestioneBancheFideiussioneESPA.DecBancaFideiussione> ElencoDecBancaFideiussioneESPA
        {
            get
            {
                if (!this.ElencoDecBancaFideiussioneESPA_GetEffettuata)
                {
                    this.ElencoDecBancaFideiussioneESPA_GetEffettuata = true;
                    List<GestioneBancheFideiussioneESPA.DecBancaFideiussione> elencoDecBancaFideiussioneESPA = null;
                    GestioneBancheFideiussioneESPA.GetDecodificaBancaFideiussione(out elencoDecBancaFideiussioneESPA);
                    this._ElencoDecBancaFideiussioneESPA = elencoDecBancaFideiussioneESPA;
                }

                return _ElencoDecBancaFideiussioneESPA;
            }
            set
            {
                _ElencoDecBancaFideiussioneESPA = value;
            }
        }

        public List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> ElencoCodeGestioneQuotaFondoIntegrativo
        {
            get
            {
                if (!this.ElencoCodeGestioneQuotaFondoIntegrativo_GetEffettuata)
                {
                    this.ElencoCodeGestioneQuotaFondoIntegrativo_GetEffettuata = true;
                    List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> elencoCodeGestioneQuotaFondoIntegrativo = null;
                    GestioneDecodifica.GetCodeGestioneQuotaFondoIntegrativo(out elencoCodeGestioneQuotaFondoIntegrativo);
                    this._ElencoCodeGestioneQuotaFondoIntegrativo = elencoCodeGestioneQuotaFondoIntegrativo;
                }

                return _ElencoCodeGestioneQuotaFondoIntegrativo;
            }
            set
            {
                _ElencoCodeGestioneQuotaFondoIntegrativo = value;
            }
        }

        public List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> ElencoCodeGestioneQuotaFondoINPGI
        {
            get
            {
                if (!this.ElencoCodeGestioneQuotaFondoINPGI_GetEffettuata)
                {
                    this.ElencoCodeGestioneQuotaFondoINPGI_GetEffettuata = true;
                    List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI = null;
                    GestioneDecodifica.GetCodeGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGI);
                    this._ElencoCodeGestioneQuotaFondoINPGI = elencoCodeGestioneQuotaFondoINPGI;
                }

                return _ElencoCodeGestioneQuotaFondoINPGI;
            }
            set
            {
                _ElencoCodeGestioneQuotaFondoINPGI = value;
            }
        }

        public List<GestioneDecodifica.Cieco> ElencoCodiceCieco
        {
            get
            {
                if (!this.ElencoCodiceCieco_GetEffettuata)
                {
                    this.ElencoCodiceCieco_GetEffettuata = true;
                    List<GestioneDecodifica.Cieco> elencoCodiceCieco = null;
                    GestioneDecodifica.GetCodiceCieco(out elencoCodiceCieco);
                    this._ElencoCodiceCieco = elencoCodiceCieco;
                }
                return _ElencoCodiceCieco;
            }
            set
            {
                _ElencoCodiceCieco = value;
            }
        }

        public List<GestioneDecodifica.SettimaneBeneficio> ElencoTipoSettimaneBeneficioAGO_CI
        {
            get
            {
                if (!this.ElencoTipoSettimaneBeneficioAGO_CI_GetEffettuata)
                {
                    this.ElencoTipoSettimaneBeneficioAGO_CI_GetEffettuata = true;
                    List<GestioneDecodifica.SettimaneBeneficio> elencoTipoSettimaneBeneficioAGO_CI = null;
                    GestioneDecodifica.GetTipoSettimaneBeneficioAGO_CI(out elencoTipoSettimaneBeneficioAGO_CI);
                    this._ElencoTipoSettimaneBeneficioAGO_CI = elencoTipoSettimaneBeneficioAGO_CI;
                }

                return this._ElencoTipoSettimaneBeneficioAGO_CI;
            }
            set
            {
                _ElencoTipoSettimaneBeneficioAGO_CI = value;
            }
        }

        public List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> ElencoCodiceMaggiorazioneExCombattenti
        {
            get
            {
                if (!this.ElencoCodiceMaggiorazioneExCombattenti_GetEffettuata)
                {
                    this.ElencoCodiceMaggiorazioneExCombattenti_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> elencoCodiceMaggiorazioneExCombattenti = null;
                    GestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out elencoCodiceMaggiorazioneExCombattenti);
                    this._ElencoCodiceMaggiorazioneExCombattenti = elencoCodiceMaggiorazioneExCombattenti;
                }

                return this._ElencoCodiceMaggiorazioneExCombattenti;
            }
            set
            {
                this._ElencoCodiceMaggiorazioneExCombattenti = value;
            }
        }

        public List<GestioneDecodifica.TipologiaPrestazione> ElencoTipologiaPrestazione
        {
            get
            {
                if (!this.ElencoTipologiaPrestazione_GetEffettuata)
                {
                    this.ElencoTipologiaPrestazione_GetEffettuata = true;
                    List<GestioneDecodifica.TipologiaPrestazione> elencoTipologiaPrestazione = null;
                    GestioneDecodifica.GetDecodificaTipologiaPrestazione(out elencoTipologiaPrestazione);
                    this._ElencoTipologiaPrestazione = elencoTipologiaPrestazione;
                }

                return this._ElencoTipologiaPrestazione;
            }
            set
            {
                this._ElencoTipologiaPrestazione = value;
            }
        }

        public List<GestioneDecodifica.TipologiaBeneficioTerrorismo> ElencoTipologiaBeneficioTerrorismo
        {
            get
            {
                if (!this.ElencoTipologiaBeneficioTerrorismo_GetEffettuata)
                {
                    this.ElencoTipologiaBeneficioTerrorismo_GetEffettuata = true;
                    List<GestioneDecodifica.TipologiaBeneficioTerrorismo> elencoTipologiaBeneficioTerrorismo = null;
                    GestioneDecodifica.GetDecTipologiaBeneficioTerrorismo(out elencoTipologiaBeneficioTerrorismo);
                    this._ElencoTipologiaBeneficioTerrorismo = elencoTipologiaBeneficioTerrorismo;
                }

                return this._ElencoTipologiaBeneficioTerrorismo;
            }
            set
            {
                this._ElencoTipologiaBeneficioTerrorismo = value;
            }
        }

        public List<GestioneDecodifica.CDCMMR> ElencoCDCMMR
        {
            get
            {
                if (!this.ElencoCDCMMR_GetEffettuata)
                {
                    this.ElencoCDCMMR_GetEffettuata = true;
                    List<GestioneDecodifica.CDCMMR> elencoCDCMMR = null;
                    GestioneDecodifica.GetCodiciCDCMMR(out elencoCDCMMR);
                    this._ElencoCDCMMR = elencoCDCMMR;
                }

                return this._ElencoCDCMMR;
            }
            set
            {
                this._ElencoCDCMMR = value;
            }
        }

        public List<GestioneDecodifica.DecodificaLegge44997> ElencoLegge44997
        {
            get
            {
                if (!this.ElencoLegge44997_GetEffettuata)
                {
                    this.ElencoLegge44997_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaLegge44997> elencoCodiceLegge44997 = null;
                    GestioneDecodifica.GetElencoDecodificaLegge44997(out elencoCodiceLegge44997);
                    this._ElencoLegge44997 = elencoCodiceLegge44997;
                }
                return this._ElencoLegge44997;
            }
            set
            {
                this._ElencoLegge44997 = value;
            }
        }

        public List<GestioneDecodifica.DomandaRicorso> ElencoDomandeRicorso
        {
            get
            {
                if (!this.ElencoDomandeRicorso_GetEffettuata)
                {
                    this.ElencoDomandeRicorso_GetEffettuata = true;
                    List<GestioneDecodifica.DomandaRicorso> elencoDomandaRicorso = null;
                    GestioneDecodifica.GetElencoDomandaRicorso(out elencoDomandaRicorso);
                    this._ElencoDomandeRicorso = elencoDomandaRicorso;
                }

                return this._ElencoDomandeRicorso;
            }
            set
            {
                this._ElencoDomandeRicorso = value;
            }
        }

        public List<GestioneDecodifica.Mobilita> ElencoCodiceMobilita
        {
            get
            {
                if (!this.ElencoCodiceMobilita_GetEffettuata)
                {
                    this.ElencoCodiceMobilita_GetEffettuata = true;
                    List<GestioneDecodifica.Mobilita> elencoCodiceMobilita = null;
                    GestioneDecodifica.GetCodiceMobilita(out elencoCodiceMobilita);
                    this._ElencoCodiceMobilita = elencoCodiceMobilita;
                }

                return this._ElencoCodiceMobilita;
            }
            set
            {
                this._ElencoCodiceMobilita = value;
            }
        }

        public List<GestioneDecodifica.CodiciNatura> ElencoCodiceNaturaAGO_CI
        {
            get
            {
                if (!this.ElencoCodiceNaturaAGO_CI_GetEffettuata)
                {
                    this.ElencoCodiceNaturaAGO_CI_GetEffettuata = true;
                    List<GestioneDecodifica.CodiciNatura> elencoCodiciNaturaAGO_CI = null;
                    GestioneDecodifica.GetCodiciNatura_AGO_CI(out elencoCodiciNaturaAGO_CI);
                    this._ElencoCodiceNaturaAGO_CI = elencoCodiciNaturaAGO_CI;
                }
                return _ElencoCodiceNaturaAGO_CI;
            }
            set
            {
                this._ElencoCodiceNaturaAGO_CI = value;
            }
        }

        public List<GestioneDecodifica.DecRiconoscimentiInvalidita> ElencoRiconoscimentoInvalidita
        {
            get
            {
                if (!this.ElencoRiconoscimentoInvalidita_GetEffettuata)
                {
                    this.ElencoRiconoscimentoInvalidita_GetEffettuata = true;
                    List<GestioneDecodifica.DecRiconoscimentiInvalidita> elencoRiconoscimentoInvalidita = null;
                    GestioneDecodifica.GetElencoRiconoscimentiInvalidita(out elencoRiconoscimentoInvalidita);
                    this._ElencoRiconoscimentoInvalidita = elencoRiconoscimentoInvalidita;
                }

                return this._ElencoRiconoscimentoInvalidita;
            }
            set
            {
                this._ElencoRiconoscimentoInvalidita = value;
            }
        }

        public List<GestioneDecodifica.DerogaENPALS> ElencoDerogaENPALS
        {
            get
            {
                if (!this.ElencoDerogaENPALS_GetEffettuata)
                {
                    this.ElencoDerogaENPALS_GetEffettuata = true;
                    List<GestioneDecodifica.DerogaENPALS> elencoDerogaENPALS = null;
                    GestioneDecodifica.GetDerogaENPALS(out elencoDerogaENPALS);
                    this._ElencoDerogaENPALS = elencoDerogaENPALS;
                }

                return this._ElencoDerogaENPALS;
            }
            set
            {
                this._ElencoDerogaENPALS = value;
            }
        }

        public List<GestioneAnagraficaAziende.DecodAnagraficaAziende> ElencoAnagraficaAziende
        {
            get
            {
                if (!this.ElencoAnagraficaAziende_GetEffettuata)
                {
                    this.ElencoAnagraficaAziende_GetEffettuata = true;
                    List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziende = null;
                    GestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoAnagraficaAziende);
                    this._ElencoAnagraficaAziende = elencoAnagraficaAziende;
                }

                return this._ElencoAnagraficaAziende;
            }
            set
            {
                this._ElencoAnagraficaAziende = value;
            }
        }

        public List<GestioneDecodifica.DecTipoCalcoloVincenteDAI> ElencoTipoCalcoloVincenteDAI
        {
            get
            {
                if (!this.ElencoTipoCalcoloVincenteDAI_GetEffettuata)
                {
                    this.ElencoTipoCalcoloVincenteDAI_GetEffettuata = true;
                    List<GestioneDecodifica.DecTipoCalcoloVincenteDAI> elencoTipoCalcoloVincente = null;
                    GestioneDecodifica.GetDecodificaTipoCalcoloVincenteDAI(out elencoTipoCalcoloVincente);
                    this._ElencoTipoCalcoloVincenteDAI = elencoTipoCalcoloVincente;
                }

                return this._ElencoTipoCalcoloVincenteDAI;
            }
            set
            {
                this._ElencoTipoCalcoloVincenteDAI = value;
            }
        }

        public List<GestioneDecodifica.DecComparto> ElencoDecComparto
        {
            get
            {
                if (!this.ElencoDecComparto_GetEffettuata)
                {
                    this.ElencoDecComparto_GetEffettuata = true;
                    List<GestioneDecodifica.DecComparto> elencoDecComparto = null;
                    GestioneDecodifica.GetElencoDecComparto(out elencoDecComparto);
                    this._ElencoDecComparto = elencoDecComparto;
                }
                return this._ElencoDecComparto;
            }
            set
            {
                this._ElencoDecComparto = value;
                this.ElencoDecComparto_GetEffettuata = true;
            }
        }

        public List<GestioneDecodifica.DecSettore> ElencoDecSettore
        {
            get
            {
                if (!this.ElencoDecSettore_GetEffettuata)
                {
                    this.ElencoDecSettore_GetEffettuata = true;
                    List<GestioneDecodifica.DecSettore> elencoDecSettore = null;
                    GestioneDecodifica.GetElencoDecSettore(out elencoDecSettore);
                    this._ElencoDecSettore = elencoDecSettore;
                }
                return this._ElencoDecSettore;
            }
            set
            {
                this._ElencoDecSettore = value;
                this.ElencoDecSettore_GetEffettuata = true;
            }
        }

        public List<GestioneDecodifica.DecRuolo> ElencoDecRuolo
        {
            get
            {
                if (!this.ElencoDecRuolo_GetEffettuata)
                {
                    this.ElencoDecRuolo_GetEffettuata = true;
                    List<GestioneDecodifica.DecRuolo> elencoDecRuolo = null;
                    GestioneDecodifica.GetElencoDecRuolo(out elencoDecRuolo);
                    this._ElencoDecRuolo = elencoDecRuolo;
                }
                return this._ElencoDecRuolo;
            }
            set
            {
                this._ElencoDecRuolo = value;
                this.ElencoDecRuolo_GetEffettuata = true;
            }
        }

        public List<GestioneDecodifica.AttivitaSvolta> ElencoAttivitaSvolte
        {
            get
            {
                if (_Contenitore == null || _Contenitore.DatiPensione == null || String.IsNullOrEmpty(_Contenitore.DatiPensione.SiglaCategoria))
                    return null;

                if (!this.ElencoAttivitaSvolte_GetEffettuata)
                {
                    this.ElencoAttivitaSvolte_GetEffettuata = true;

                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, _Contenitore.DatiPensione.SiglaCategoria);
                    char? enteFondo = Utility.GetCharCategoriaFondoPI(Utility.TipoAppartenenza.FS, _Contenitore.DatiPensione.SiglaCategoria);

                    if (Utility.IsDomandaINPDAP(_Contenitore.DatiPensione.Gestione))
                        tipoFondo = Utility.TipoFondo.FS;

                    List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolte = null;
                    GestioneDecodifica.GetAttivitaSvoltaByFondo(tipoFondo.ToString(), enteFondo, out elencoAttivitaSvolte);
                    this._ElencoAttivitaSvolte = elencoAttivitaSvolte;
                }

                return this._ElencoAttivitaSvolte;
            }

            set
            {
                this._ElencoAttivitaSvolte = value;
            }
        }

        public List<GestioneDecodifica.CodiceRequisito1> ElencoCodiceRequisito1
        {
            get
            {
                if (!this.ElencoCodiceRequisito1_GetEffettuata)
                {
                    this.ElencoCodiceRequisito1_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceRequisito1> elencoCodiceRequisito1 = null;
                    GestioneDecodifica.GetCodiceRequisito1(out elencoCodiceRequisito1);
                    this._ElencoCodiceRequisito1 = elencoCodiceRequisito1;
                }

                return this._ElencoCodiceRequisito1;
            }
            set
            {
                this._ElencoCodiceRequisito1 = value;
            }
        }

        public List<GestioneDecodifica.CodiceRequisito2> ElencoCodiceRequisito2
        {
            get
            {
                if (!this.ElencoCodiceRequisito2_GetEffettuata)
                {
                    this.ElencoCodiceRequisito2_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceRequisito2> elencoCodiceRequisito2 = null;
                    GestioneDecodifica.GetCodiceRequisito2(out elencoCodiceRequisito2);
                    this._ElencoCodiceRequisito2 = elencoCodiceRequisito2;
                }

                return this._ElencoCodiceRequisito2;
            }
            set
            {
                this._ElencoCodiceRequisito2 = value;
            }
        }

        public List<GestioneDecodifica.CodiceConvenzioneInternazionale> ElencoCodiceConvenzioneInternazionale
        {
            get
            {
                if (!this.ElencoCodiceConvenzioneInternazionale_GetEffettuata)
                {
                    this.ElencoCodiceConvenzioneInternazionale_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceConvenzioneInternazionale> elencoCodiceConvenzioneInternazionale = null;
                    GestioneDecodifica.GetCodiceConvenzioneInternazionale(out elencoCodiceConvenzioneInternazionale);
                    this._ElencoCodiceConvenzioneInternazionale = elencoCodiceConvenzioneInternazionale;
                }
                return this._ElencoCodiceConvenzioneInternazionale;
            }
            set
            {
                this._ElencoCodiceConvenzioneInternazionale = value;
            }
        }

        public List<GestioneDecodifica.DecodificaCodiceArt22> ElencoCodiceDecodificaArt22
        {
            get
            {
                if (!this.ElencoCodiceDecodificaArt22_GetEffettuata)
                {
                    this.ElencoCodiceDecodificaArt22_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaCodiceArt22> elencoCodiceDecodificaArt22 = null;
                    GestioneDecodifica.GetElencoCodiciArt22(out elencoCodiceDecodificaArt22);
                    this._ElencoCodiceDecodificaArt22 = elencoCodiceDecodificaArt22;
                }

                return this._ElencoCodiceDecodificaArt22;
            }
            set
            {
                this._ElencoCodiceDecodificaArt22 = value;
            }
        }

        public List<GestioneDecodifica.DecodificaCodiceCapitalizzazione> ElencoCodiceDecodificaCapitalizzazione
        {
            get
            {
                if (!this.ElencoCodiceDecodificaCapitalizzazione_GetEffettuata)
                {
                    this.ElencoCodiceDecodificaCapitalizzazione_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaCodiceCapitalizzazione> elencoCodiceDecodificaCodeCapitalizzazione = null;
                    GestioneDecodifica.GetElencoCodiciCapitalizzazione(out elencoCodiceDecodificaCodeCapitalizzazione);
                    this._ElencoCodiceDecodificaCapitalizzazione = elencoCodiceDecodificaCodeCapitalizzazione;
                }

                return this._ElencoCodiceDecodificaCapitalizzazione;
            }
            set
            {
                this._ElencoCodiceDecodificaCapitalizzazione = value;
            }
        }

        public List<GestioneDecodifica.DecodificaCodeEsodo> ElencoCodiceEsodo
        {
            get
            {
                if (!this.ElencoCodiceEsodo_GetEffettuata)
                {
                    this.ElencoCodiceEsodo_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaCodeEsodo> listaCodiceEsodo = null;
                    GestioneDecodifica.GetElencoCodiciEsodo(out listaCodiceEsodo);
                    this._ElencoCodiceEsodo = listaCodiceEsodo;
                }
                return this._ElencoCodiceEsodo;
            }
            set
            {
                this._ElencoCodiceEsodo = value;
            }
        }

        public List<GestioneDecodifica.DecodificaPartTime> ElencoCodiceDecodificaPartTime
        {
            get
            {
                if (!this.ElencoCodiceDecodificaPartTime_GetEffettuata)
                {
                    this.ElencoCodiceDecodificaPartTime_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaPartTime> elencoCodiceDecodificaPartTime = null;
                    GestioneDecodifica.GetElencoCodiciPartTime(out elencoCodiceDecodificaPartTime);
                    this._ElencoCodiceDecodificaPartTime = elencoCodiceDecodificaPartTime;
                }
                return this._ElencoCodiceDecodificaPartTime;
            }
            set
            {
                this._ElencoCodiceDecodificaPartTime = value;
            }
        }

        public List<GestioneDecodifica.DecodificaCausaCessazione> ElencoCodiceCausaCessazione
        {
            get
            {
                if (!this.ElencoCodiceCausaCessazione_GetEffettuata)
                {
                    this.ElencoCodiceCausaCessazione_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaCausaCessazione> listaCodiceCausaCessazione = null;
                    GestioneDecodifica.GetElencoCodiciCausaCessazione(out listaCodiceCausaCessazione);
                    this._ElencoCodiceCausaCessazione = listaCodiceCausaCessazione;
                }
                return this._ElencoCodiceCausaCessazione;
            }
            set
            {
                this._ElencoCodiceCausaCessazione = value;
            }
        }

        public List<GestioneDecodifica.DecodificaTipoLiquidazionePM> ElencoTipoLiquidazionePM
        {
            get
            {
                if (!this.ElencoTipoLiquidazionePM_GetEffettuata)
                {
                    this.ElencoTipoLiquidazionePM_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaTipoLiquidazionePM> listaTipoLiquidazionePM = null;
                    GestioneDecodifica.GetDecodificaTipoLiquidazionePM(out listaTipoLiquidazionePM);
                    this._ElencoTipoLiquidazionePM = listaTipoLiquidazionePM;
                }
                return this._ElencoTipoLiquidazionePM;
            }
            set
            {
                this._ElencoTipoLiquidazionePM = value;
            }
        }

        public List<GestioneDecodifica.CodiceTipoLiquidazionePM> ElencoCodiceTipoLiquidazionePM
        {
            get
            {
                if (!this.ElencoCodiceTipoLiquidazionePM_GetEffettuata)
                {
                    this.ElencoCodiceTipoLiquidazionePM_GetEffettuata = true;
                    List<GestioneDecodifica.CodiceTipoLiquidazionePM> listaCodiceTipoLiquidazione = null;
                    GestioneDecodifica.GetCodiceTipoLiquidazionePM(out listaCodiceTipoLiquidazione);
                    this._ElencoCodiceTipoLiquidazionePM = listaCodiceTipoLiquidazione;
                }
                return this._ElencoCodiceTipoLiquidazionePM;
            }
            set
            {
                this._ElencoCodiceTipoLiquidazionePM = value;
            }
        }

        public List<GestioneDecodifica.DecodificaLegge413> ElencoCodiceLegge413
        {
            get
            {
                if (!this.ElencoCodiceLegge413_GetEffettuata)
                {
                    this.ElencoCodiceLegge413_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaLegge413> elencoCodiceLegge413 = null;
                    GestioneDecodifica.GetDecodificaLegge413(out elencoCodiceLegge413);
                    this._ElencoCodiceLegge413 = elencoCodiceLegge413;
                }
                return this._ElencoCodiceLegge413;
            }
            set
            {
                this._ElencoCodiceLegge413 = value;
            }
        }

        public List<GestioneDecodifica.DecodificaAttivitaSvolta2> ElencoAttivitaSvolta2
        {
            get
            {
                if (!this.ElencoAttivitaSvolta2_GetEffettuata)
                {
                    this.ElencoAttivitaSvolta2_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaAttivitaSvolta2> elencoAttivitaSvolta2 = null;
                    GestioneDecodifica.GetDecodificaAttivitaSvolta2(out elencoAttivitaSvolta2);
                    this._ElencoAttivitaSvolta2 = elencoAttivitaSvolta2;
                }
                return this._ElencoAttivitaSvolta2;
            }
            set
            {
                this._ElencoAttivitaSvolta2 = value;
            }
        }

        public List<GestioneDecodifica.DecodificaTipoLiquidazione> ElencoTipoLiquidazione
        {
            get
            {
                if (!this.ElencoTipoLiquidazione_GetEffettuata)
                {
                    this.ElencoTipoLiquidazione_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaTipoLiquidazione> listaTipoLiquidazione = null;
                    GestioneDecodifica.GetDecodificaTipoLiquidazione(out listaTipoLiquidazione);
                    this._ElencoTipoLiquidazione = listaTipoLiquidazione;
                }
                return this._ElencoTipoLiquidazione;
            }
            set
            {
                this._ElencoTipoLiquidazione = value;
            }
        }

        public List<GestioneDecodifica.CodiciNatura> ElencoCodiciNaturaFS
        {
            get
            {
                if (!this.ElencoCodiciNaturaFS_GetEffettuata)
                {
                    this.ElencoCodiciNaturaFS_GetEffettuata = true;
                    List<GestioneDecodifica.CodiciNatura> elencoCodiciNatura = null;
                    GestioneDecodifica.GetCodiciNatura(out elencoCodiciNatura);
                    this._ElencoCodiciNaturaFS = elencoCodiciNatura;
                }
                return this._ElencoCodiciNaturaFS;
            }
            set
            {
                this._ElencoCodiciNaturaFS = value;
            }
        }


        public List<GestioneDecodifica.DecPersonaleViaggiante> ElencoPersonaleViaggiante
        {
            get
            {
                if (!this.ElencoPersonaleViaggiante_GetEffettuata)
                {
                    this.ElencoPersonaleViaggiante_GetEffettuata = true;
                    List<GestioneDecodifica.DecPersonaleViaggiante> elencoPersonaleViaggiante = null;
                    GestioneDecodifica.GetDecPersonaleViaggiante(out elencoPersonaleViaggiante);
                    this._ElencoPersonaleViaggiante = elencoPersonaleViaggiante;
                }
                return this._ElencoPersonaleViaggiante;
            }
            set
            {
                this._ElencoPersonaleViaggiante = value;
            }
        }

        public List<GestioneDecodifica.DecodificaEnteRipartizioneINPDAP> ElencoDecodificaEnteRipartizioneINPDAP
        {
            get
            {
                if (!this.ElencoDecodificaEnteRipartizioneINPDAP_GetEffettuata)
                {
                    this.ElencoDecodificaEnteRipartizioneINPDAP_GetEffettuata = true;
                    List<GestioneDecodifica.DecodificaEnteRipartizioneINPDAP> elencoDecodificaEnteRipartizioneINPDAP = null;
                    GestioneDecodifica.GetEnteRipartizioneINPDAP(out elencoDecodificaEnteRipartizioneINPDAP);
                    this._ElencoDecodificaEnteRipartizioneINPDAP = elencoDecodificaEnteRipartizioneINPDAP;
                }
                return this._ElencoDecodificaEnteRipartizioneINPDAP;
            }
            set
            {
                this._ElencoDecodificaEnteRipartizioneINPDAP = value;
            }
        }

        public List<GestioneDecodifica.DecMicroqualificaINPDAP> ElencoDecMicroqualificaNPDAP
        {
            get
            {
                if (_Contenitore == null || _Contenitore.DatiPensione == null || String.IsNullOrEmpty(_Contenitore.DatiPensione.SiglaCategoria))
                    return null;

                if (!this.ElencoDecMicroqualificaNPDAP_GetEffettuata)
                {
                    this.ElencoDecMicroqualificaNPDAP_GetEffettuata = true;
                    List<GestioneDecodifica.DecMicroqualificaINPDAP> elencoDecMicroqualificaNPDAP = null;
                    GestioneDecodifica.GetDecMicroqualificaINPDAP(_Contenitore.DatiPensione.SiglaCategoria, out elencoDecMicroqualificaNPDAP);
                    this._ElencoDecMicroqualificaNPDAP = elencoDecMicroqualificaNPDAP;
                }
                return this._ElencoDecMicroqualificaNPDAP;
            }
            set
            {
                this._ElencoDecMicroqualificaNPDAP = value;
            }
        }

        public List<GestioneDecodifica.SettimaneBeneficio> ElencoTipoBenefici
        {
            get
            {
                if (!this.ElencoTipoBenefici_GetEffettuata)
                {
                    this.ElencoTipoBenefici_GetEffettuata = true;
                    List<Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio> elencoTipoBenefici = null;
                    GestioneDecodifica.GetTipoSettimaneBeneficio(out elencoTipoBenefici);
                    this._ElencoTipoBenefici = elencoTipoBenefici;
                }
                return this._ElencoTipoBenefici;
            }
            set
            {
                this._ElencoTipoBenefici = value;
            }
        }

        public List<GestioneDecodifica.DecPensioniPrivilegiate> ElencoPensioniPrivilegiate
        {
            get
            {
                if (!this.ElencoPensioniPrivilegiate_GetEffettuata)
                {
                    this.ElencoPensioniPrivilegiate_GetEffettuata = true;
                    List<GestioneDecodifica.DecPensioniPrivilegiate> elencoPensioniPrivilegiate = null;
                    GestioneDecodifica.GetElencoPensioniPrivilegiate(out elencoPensioniPrivilegiate);
                    this._ElencoPensioniPrivilegiate = elencoPensioniPrivilegiate;
                }

                return this._ElencoPensioniPrivilegiate;
            }
            set
            {
                this._ElencoPensioniPrivilegiate = value;
            }
        }

        public List<GestioneDecodifica.ComunicazioneCampo3> ElencoDecodificaComunicazioneCampo3
        {
            get
            {
                if (!this.ElencoDecodificaComunicazioneCampo3_GetEffettuata)
                {
                    this.ElencoDecodificaComunicazioneCampo3_GetEffettuata = true;
                    List<GestioneDecodifica.ComunicazioneCampo3> elencoDecodificaComunicazioneCampo3 = null;
                    GestioneDecodifica.GetComunicazioneCampo3(out elencoDecodificaComunicazioneCampo3);
                    this._ElencoDecodificaComunicazioneCampo3 = elencoDecodificaComunicazioneCampo3;
                }

                return this._ElencoDecodificaComunicazioneCampo3;
            }
            set
            {
                this._ElencoDecodificaComunicazioneCampo3 = value;
            }
        }

        public List<GestioneDecodifica.CtrlScadenzaIndennizzoINDCOM> ElencoCtrlScadenzaIndennizzoINDCOM
        {
            get
            {
                if (!this.ElencoCtrlScadenzaIndennizzoINDCOM_GetEffettuata)
                {
                    this.ElencoCtrlScadenzaIndennizzoINDCOM_GetEffettuata = true;
                    List<GestioneDecodifica.CtrlScadenzaIndennizzoINDCOM> elencoCtrlScadenzaIndennizzoINDCOM = null;
                    GestioneDecodifica.GetCtrlScadenzaIndennizzoINDCOM(out elencoCtrlScadenzaIndennizzoINDCOM);
                    this._ElencoCtrlScadenzaIndennizzoINDCOM = elencoCtrlScadenzaIndennizzoINDCOM;
                }

                return _ElencoCtrlScadenzaIndennizzoINDCOM;
            }
            set
            {
                _ElencoCtrlScadenzaIndennizzoINDCOM = value;
                ElencoCtrlScadenzaIndennizzoINDCOM_GetEffettuata = true;
            }
        }

        #endregion oggetti

        // Le variabili servono a capire se è stata già effettuata la get del relativo oggetto
        #region variabili booleane
        public bool ElencoDecodificaEnteCassaProfessionale_GetEffettuata { get; set; }
        public bool ElencoDecCodeGruppoOnere_GetEffettuata { get; set; }
        public bool ElencoDecCodeSottoGruppoOnere_GetEffettuata { get; set; }
        public bool ElencoDecBancaFideiussione_GetEffettuata { get; set; }
        public bool ElencoDecodificaTipoQuota_GetEffettuata { get; set; }
        public bool ElencoCtrlDecorrenzaRetrExINPDAI_GetEffettuata { get; set; }
        public bool ElencoCodeGestioneCalcoloRetributivo_GetEffettuata { get; set; }
        public bool ElencoCodeGestioneCalcoloContributivo_GetEffettuata { get; set; }
        public bool ElencoDecodificaEnte_GetEffettuata { get; set; }
        public bool ElencoCatEnteAltraPensione_GetEffettuata { get; set; }
        public bool ElencoCodiceParticolare_GetEffettuata { get; set; }
        public bool ElencoSoggettoBeneficiario_GetEffettuata { get; set; }
        public bool ElencoDecAzienda_GetEffettuata { get; set; }
        public bool ElencoDecAziendaAll_GetEffettuata { get; set; }
        public bool ElencoDecEnteGestioneFondo_GetEffettuata { get; set; }
        public bool ElencoDecCodiceTrattenute_GetEffettuata { get; set; }
        public bool ElencoTipoCalcolo_GetEffettuata { get; set; }
        public bool ElencoCodiceSpecifico_GetEffettuata { get; set; }
        public bool ElencoDecAziendeVOCRED_DAP_GetEffettuata { get; set; }
        public bool ElencoDecAziendeScadenzaAssegnoGGmmAAAA_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAccordiPerTipo0179_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAccordiPerTipo0171_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAccordi_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAccordiLetteraB_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAziendePerTipo0179_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAziendePerTipo0171_GetEffettuata { get; set; }
        public bool ElencoDecodAnagraficaAziendeLetteraB_GetEffettuata { get; set; }
        public bool ElencoPensioneExInpdai_GetEffettuata { get; set; }
        public bool ElencoDecModalitaLiquidazione_GetEffettuata { get; set; }
        public bool ElencoCodiceEliminazione_GetEffettuata { get; set; }
        public bool ElencoStatoEstero_GetEffettuata { get; set; }
        public bool ElencoControlloDinamico_GetEffettuata { get; set; }
        public bool ElencoSiglaFamiliare_GetEffettuata { get; set; }
        public bool ElencoCodMaggiorazioneFamiliari_GetEffettuata { get; set; }
        public bool ElencoCtrlEnteCassaCodiceGestione_GetEffettuata { get; set; }
        public bool ElencoCtrlCatAdeguata_GetEffettuata { get; set; }
        public bool ElencoCtrlTipoUfficio_GetEffettuata { get; set; }
        public bool ElencoDecBancaFideiussioneESPA_GetEffettuata { get; set; }
        public bool ElencoCodeGestioneQuotaFondoIntegrativo_GetEffettuata { get; set; }
        public bool ElencoCodeGestioneQuotaFondoINPGI_GetEffettuata { get; set; }
        public bool ElencoCodiceCieco_GetEffettuata { get; set; }
        public bool ElencoTipoSettimaneBeneficioAGO_CI_GetEffettuata { get; set; }
        public bool ElencoCodiceMaggiorazioneExCombattenti_GetEffettuata { get; set; }
        public bool ElencoTipologiaPrestazione_GetEffettuata { get; set; }
        public bool ElencoTipologiaBeneficioTerrorismo_GetEffettuata { get; set; }
        public bool ElencoCDCMMR_GetEffettuata { get; set; }
        public bool ElencoLegge44997_GetEffettuata { get; set; }
        public bool ElencoDomandeRicorso_GetEffettuata { get; set; }
        public bool ElencoCodiceMobilita_GetEffettuata { get; set; }
        public bool ElencoCodiceNaturaAGO_CI_GetEffettuata { get; set; }
        public bool ElencoRiconoscimentoInvalidita_GetEffettuata { get; set; }
        public bool ElencoDerogaENPALS_GetEffettuata { get; set; }
        public bool ElencoAnagraficaAziende_GetEffettuata { get; set; }
        public bool ElencoTipoCalcoloVincenteDAI_GetEffettuata { get; set; }
        public bool ElencoDecComparto_GetEffettuata { get; set; }
        public bool ElencoDecSettore_GetEffettuata { get; set; }
        public bool ElencoDecRuolo_GetEffettuata { get; set; }
        public bool ElencoAttivitaSvolte_GetEffettuata { get; set; }
        public bool ElencoCodiceRequisito1_GetEffettuata { get; set; }
        public bool ElencoCodiceRequisito2_GetEffettuata { get; set; }
        public bool ElencoCodiceConvenzioneInternazionale_GetEffettuata { get; set; }
        public bool ElencoCodiceDecodificaArt22_GetEffettuata { get; set; }
        public bool ElencoCodiceDecodificaCapitalizzazione_GetEffettuata { get; set; }
        public bool ElencoCodiceEsodo_GetEffettuata { get; set; }
        public bool ElencoCodiceDecodificaPartTime_GetEffettuata { get; set; }
        public bool ElencoCodiceCausaCessazione_GetEffettuata { get; set; }
        public bool ElencoTipoLiquidazionePM_GetEffettuata { get; set; }
        public bool ElencoCodiceTipoLiquidazionePM_GetEffettuata { get; set; }
        public bool ElencoCodiceLegge413_GetEffettuata { get; set; }
        public bool ElencoAttivitaSvolta2_GetEffettuata { get; set; }
        public bool ElencoTipoLiquidazione_GetEffettuata { get; set; }
        public bool ElencoCodiciNaturaFS_GetEffettuata { get; set; }
        public bool ElencoPersonaleViaggiante_GetEffettuata { get; set; }
        public bool ElencoDecodificaEnteRipartizioneINPDAP_GetEffettuata { get; set; }
        public bool ElencoDecMicroqualificaNPDAP_GetEffettuata { get; set; }
        public bool ElencoTipoBenefici_GetEffettuata { get; set; }
        public bool ElencoPensioniPrivilegiate_GetEffettuata { get; set; }
        public bool ElencoDecodificaComunicazioneCampo3_GetEffettuata { get; set; }
        public bool ElencoDecodificaBanchePerSede_GetEffettuata { get; set; }
        public bool ElencoCtrlScadenzaIndennizzoINDCOM_GetEffettuata { get; set; }
        #endregion variabili booleane
        #endregion public properties
    }
}
