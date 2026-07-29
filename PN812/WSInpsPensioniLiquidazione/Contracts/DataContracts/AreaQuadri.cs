using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using System.Web;
using System.Data;

namespace INPS.Pensioni.Liquidazione.Service.Contracts.DataContracts
{
    [DataContract]
    public class AreaQuadri
    {
        #region private properties

        private DatiQuadroTitolare _QuadroTitolare;
        private DatiQuadroDetrazioni _QuadroDetrazioni;
        private DatiQuadroPagamento _QuadroPagamento;
        private DatiQuadroLiquidazionePensione _QuadroLiquidazionePensione;
        private DatiQuadroDelegatoTutore _QuadroDelegatoTutore;
        private DatiQuadroDatiContributivi _QuadroDatiContributivi;
        private DatiQuadroRedditi _QuadroRedditi;
        private DatiQuadroFamiliari _QuadroFamiliari;
        private DatiQuadroDanteCausa _QuadroDanteCausa;
        private DatiQuadroMaggiorazioniBenefici _QuadroMaggiorazioniBenefici;
        private DatiQuadroSupplementi _QuadroSupplementi;
        private DatiQuadroBititolarita _QuadroBititolarita;
        private DatiQuadroEliminazione _QuadroEliminazione;
        private DatiQuadroOneri _QuadroOneri;
        private DatiQuadroDatiFondo _QuadroDatiFondo;
        private DatiQuadroDatiNoCalcolo _QuadroDatiNoCalcolo;
        private DatiQuadroPeriodi _QuadroPeriodi;
        private DatiQuadroAventiDiritto _QuadroAventiDiritto;
        private DatiQuadroAltreDomandeCollegate _QuadroAltreDomandeCollegate;
        private DatiQuadroRichiestaBonus _QuadroRichiestaBonus;

        #endregion private properties

        #region public data member
        [DataMember]
        public DatiQuadroTitolare QuadroTitolare { get { return _QuadroTitolare; } set { _QuadroTitolare = value; } }
        [DataMember]
        public DatiQuadroDetrazioni QuadroDetrazioni { get { return _QuadroDetrazioni; } set { _QuadroDetrazioni = value; } }
        [DataMember]
        public DatiQuadroPagamento QuadroPagamento { get { return _QuadroPagamento; } set { _QuadroPagamento = value; } }
        [DataMember]
        public DatiQuadroLiquidazionePensione QuadroLiquidazionePensione { get { return _QuadroLiquidazionePensione; } set { _QuadroLiquidazionePensione = value; } }
        [DataMember]
        public DatiQuadroDelegatoTutore QuadroDelegatoTutore { get { return _QuadroDelegatoTutore; } set { _QuadroDelegatoTutore = value; } }
        [DataMember]
        public DatiQuadroDatiContributivi QuadroDatiContributivi { get { return _QuadroDatiContributivi; } set { _QuadroDatiContributivi = value; } }
        [DataMember]
        public DatiQuadroRedditi QuadroRedditi { get { return _QuadroRedditi; } set { _QuadroRedditi = value; } }
        [DataMember]
        public DatiQuadroFamiliari QuadroFamiliari { get { return _QuadroFamiliari; } set { _QuadroFamiliari = value; } }
        [DataMember]
        public DatiQuadroDanteCausa QuadroDanteCausa { get { return _QuadroDanteCausa; } set { _QuadroDanteCausa = value; } }
        [DataMember]
        public DatiQuadroMaggiorazioniBenefici QuadroMaggiorazioniBenefici { get { return _QuadroMaggiorazioniBenefici; } set { _QuadroMaggiorazioniBenefici = value; } }
        [DataMember]
        public DatiQuadroSupplementi QuadroSupplementi { get { return _QuadroSupplementi; } set { _QuadroSupplementi = value; } }
        [DataMember]
        public DatiQuadroBititolarita QuadroBititolarita { get { return _QuadroBititolarita; } set { _QuadroBititolarita = value; } }
        [DataMember]
        public DatiQuadroEliminazione QuadroEliminazione { get { return _QuadroEliminazione; } set { _QuadroEliminazione = value; } }
        [DataMember]
        public DatiQuadroOneri QuadroOneri { get { return _QuadroOneri; } set { _QuadroOneri = value; } }
        [DataMember]
        public DatiQuadroDatiFondo QuadroDatiFondo { get { return _QuadroDatiFondo; } set { _QuadroDatiFondo = value; } }
        [DataMember]
        public DatiQuadroDatiNoCalcolo QuadroDatiNoCalcolo { get { return _QuadroDatiNoCalcolo; } set { _QuadroDatiNoCalcolo = value; } }
        [DataMember]
        public DatiQuadroPeriodi QuadroPeriodi { get { return _QuadroPeriodi; } set { _QuadroPeriodi = value; } }
        [DataMember]
        public DatiQuadroAventiDiritto QuadroAventiDiritto { get { return _QuadroAventiDiritto; } set { _QuadroAventiDiritto = value; } }
        [DataMember]
        public DatiQuadroAltreDomandeCollegate QuadroAltreDomandeCollegate { get { return _QuadroAltreDomandeCollegate; } set { _QuadroAltreDomandeCollegate = value; } }
        [DataMember]
        public DatiQuadroRichiestaBonus QuadroRichiestaBonus { get { return _QuadroRichiestaBonus; } set { _QuadroRichiestaBonus = value; } }

        #endregion public data member

        #region nested class

        [DataContract]
        public class DatiQuadroTitolare
        {
            public DatiQuadroTitolare()
            {
            }

            internal DatiQuadroTitolare(BLCommon.GestioneQuadri.DatiQuadroTitolare quadroTitolare)
            {
                switch (quadroTitolare.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroTitolare.TabAnagrafica.HasValue && quadroTitolare.TabAnagrafica.Value == 2) ||
                            (quadroTitolare.TabStatiCivili.HasValue && quadroTitolare.TabStatiCivili.Value == 2) ||
                            (quadroTitolare.TabResidenzeEstero.HasValue && quadroTitolare.TabResidenzeEstero.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroTitolare.TabAnagrafica.HasValue && quadroTitolare.TabAnagrafica.Value == 0) ||
                            (quadroTitolare.TabStatiCivili.HasValue && quadroTitolare.TabStatiCivili.Value == 0) ||
                            (quadroTitolare.TabResidenzeEstero.HasValue && quadroTitolare.TabResidenzeEstero.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabAnagrafica = ImpostaSemaforoTab(quadroTitolare.TabAnagrafica);
                this._TabStatiCivili = ImpostaSemaforoTab(quadroTitolare.TabStatiCivili);
                this._TabResidenzeEstero = ImpostaSemaforoTab(quadroTitolare.TabResidenzeEstero);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabAnagrafica;

            private Semaforo _TabStatiCivili;

            private Semaforo _TabResidenzeEstero;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabAnagrafica { get { return _TabAnagrafica; } set { _TabAnagrafica = value; } }
            [DataMember]
            public Semaforo TabStatiCivili { get { return _TabStatiCivili; } set { _TabStatiCivili = value; } }
            [DataMember]
            public Semaforo TabResidenzeEstero { get { return _TabResidenzeEstero; } set { _TabResidenzeEstero = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroDetrazioni
        {
            public DatiQuadroDetrazioni()
            {
            }

            internal DatiQuadroDetrazioni(BLCommon.GestioneQuadri.DatiQuadroDetrazioni quadroDetrazioni)
            {
                switch (quadroDetrazioni.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroDetrazioni.TabDetrazioni.HasValue && quadroDetrazioni.TabDetrazioni.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroDetrazioni.TabDetrazioni.HasValue && quadroDetrazioni.TabDetrazioni.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabDetrazioni = ImpostaSemaforoTab(quadroDetrazioni.TabDetrazioni);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabDetrazioni;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabDetrazioni { get { return _TabDetrazioni; } set { _TabDetrazioni = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroPagamento
        {
            public DatiQuadroPagamento()
            {
            }

            internal DatiQuadroPagamento(BLCommon.GestioneQuadri.DatiQuadroPagamento quadroPagamento)
            {
                switch (quadroPagamento.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroPagamento.TabPagamento.HasValue && quadroPagamento.TabPagamento.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroPagamento.TabPagamento.HasValue && quadroPagamento.TabPagamento.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabPagamento = ImpostaSemaforoTab(quadroPagamento.TabPagamento);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabPagamento;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabPagamento { get { return _TabPagamento; } set { _TabPagamento = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroLiquidazionePensione
        {
            public DatiQuadroLiquidazionePensione()
            {
            }

            internal DatiQuadroLiquidazionePensione(BLCommon.GestioneQuadri.DatiQuadroLiquidazionePensione quadroLiquidazionePensione)
            {
                switch (quadroLiquidazionePensione.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroLiquidazionePensione.TabDatiGenerici.HasValue && quadroLiquidazionePensione.TabDatiGenerici.Value == 2) ||
                            (quadroLiquidazionePensione.TabOpzione.HasValue && quadroLiquidazionePensione.TabOpzione.Value == 2) ||
                            (quadroLiquidazionePensione.TabPrecedentePensione.HasValue && quadroLiquidazionePensione.TabPrecedentePensione.Value == 2) ||
                            (quadroLiquidazionePensione.TabIstruttoria.HasValue && quadroLiquidazionePensione.TabIstruttoria.Value == 2) ||
                            (quadroLiquidazionePensione.TabDatiAssicurativi.HasValue && quadroLiquidazionePensione.TabDatiAssicurativi.Value == 2) ||
                            (quadroLiquidazionePensione.TabInail.HasValue && quadroLiquidazionePensione.TabInail.Value == 2) ||
                            (quadroLiquidazionePensione.TabDatiLegge460.HasValue && quadroLiquidazionePensione.TabDatiLegge460.Value == 2) ||
                            (quadroLiquidazionePensione.TabContribuzioneEnpals.HasValue && quadroLiquidazionePensione.TabContribuzioneEnpals.Value == 2) ||
                            (quadroLiquidazionePensione.TabInteressiLegali.HasValue && quadroLiquidazionePensione.TabInteressiLegali.Value == 2) ||
                            (quadroLiquidazionePensione.TabSentenzaArt4.HasValue && quadroLiquidazionePensione.TabSentenzaArt4.Value == 2) ||
                            (quadroLiquidazionePensione.TabSentenze.HasValue && quadroLiquidazionePensione.TabSentenze.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroLiquidazionePensione.TabDatiGenerici.HasValue && quadroLiquidazionePensione.TabDatiGenerici.Value == 0) ||
                            (quadroLiquidazionePensione.TabOpzione.HasValue && quadroLiquidazionePensione.TabOpzione.Value == 0) ||
                            (quadroLiquidazionePensione.TabPrecedentePensione.HasValue && quadroLiquidazionePensione.TabPrecedentePensione.Value == 0) ||
                            (quadroLiquidazionePensione.TabIstruttoria.HasValue && quadroLiquidazionePensione.TabIstruttoria.Value == 0) ||
                            (quadroLiquidazionePensione.TabDatiAssicurativi.HasValue && quadroLiquidazionePensione.TabDatiAssicurativi.Value == 0) ||
                            (quadroLiquidazionePensione.TabInail.HasValue && quadroLiquidazionePensione.TabInail.Value == 0) ||
                            (quadroLiquidazionePensione.TabDatiLegge460.HasValue && quadroLiquidazionePensione.TabDatiLegge460.Value == 0) ||
                            (quadroLiquidazionePensione.TabContribuzioneEnpals.HasValue && quadroLiquidazionePensione.TabContribuzioneEnpals.Value == 0) ||
                            (quadroLiquidazionePensione.TabInteressiLegali.HasValue && quadroLiquidazionePensione.TabInteressiLegali.Value == 0) ||
                            (quadroLiquidazionePensione.TabSentenzaArt4.HasValue && quadroLiquidazionePensione.TabSentenzaArt4.Value == 0) ||
                            (quadroLiquidazionePensione.TabSentenze.HasValue && quadroLiquidazionePensione.TabSentenze.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabDatiGenerici = ImpostaSemaforoTab(quadroLiquidazionePensione.TabDatiGenerici);
                this._TabOpzione = ImpostaSemaforoTab(quadroLiquidazionePensione.TabOpzione);
                this._TabPrecedentePensione = ImpostaSemaforoTab(quadroLiquidazionePensione.TabPrecedentePensione);
                this._TabIstruttoria = ImpostaSemaforoTab(quadroLiquidazionePensione.TabIstruttoria);
                this._TabDatiAssicurativi = ImpostaSemaforoTab(quadroLiquidazionePensione.TabDatiAssicurativi);
                this._TabInail = ImpostaSemaforoTab(quadroLiquidazionePensione.TabInail);
                this._TabDatiLegge460 = ImpostaSemaforoTab(quadroLiquidazionePensione.TabDatiLegge460);
                this._TabDatiContributiviEnpals = ImpostaSemaforoTab(quadroLiquidazionePensione.TabContribuzioneEnpals);
                this._TabStorico = ImpostaSemaforoTab(quadroLiquidazionePensione.TabStorico);
                this._TabInteressiLegali = ImpostaSemaforoTab(quadroLiquidazionePensione.TabInteressiLegali);
                this._TabSentenzaArt4 = ImpostaSemaforoTab(quadroLiquidazionePensione.TabSentenzaArt4);
                this._TabSentenze = ImpostaSemaforoTab(quadroLiquidazionePensione.TabSentenze);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabDatiGenerici;

            private Semaforo _TabOpzione;

            private Semaforo _TabPrecedentePensione;

            private Semaforo _TabIstruttoria;

            private Semaforo _TabDatiAssicurativi;

            private Semaforo _TabInail;

            private Semaforo _TabDatiLegge460;

            private Semaforo _TabDatiContributiviEnpals;

            private Semaforo _TabStorico;

            private Semaforo _TabInteressiLegali;

            private Semaforo _TabSentenzaArt4;

            private Semaforo _TabSentenze;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabDatiGenerici { get { return _TabDatiGenerici; } set { _TabDatiGenerici = value; } }
            [DataMember]
            public Semaforo TabOpzione { get { return _TabOpzione; } set { _TabOpzione = value; } }
            [DataMember]
            public Semaforo TabPrecedentePensione { get { return _TabPrecedentePensione; } set { _TabPrecedentePensione = value; } }
            [DataMember]
            public Semaforo TabIstruttoria { get { return _TabIstruttoria; } set { _TabIstruttoria = value; } }
            [DataMember]
            public Semaforo TabDatiAssicurativi { get { return _TabDatiAssicurativi; } set { _TabDatiAssicurativi = value; } }
            [DataMember]
            public Semaforo TabInail { get { return _TabInail; } set { _TabInail = value; } }
            [DataMember]
            public Semaforo TabDatiLegge460 { get { return _TabDatiLegge460; } set { _TabDatiLegge460 = value; } }
            [DataMember]
            public Semaforo TabDatiContributiviEnpals { get { return _TabDatiContributiviEnpals; } set { _TabDatiContributiviEnpals = value; } }
            [DataMember]
            public Semaforo TabStorico { get { return _TabStorico; } set { _TabStorico = value; } }
            [DataMember]
            public Semaforo TabInteressiLegali { get { return _TabInteressiLegali; } set { _TabInteressiLegali = value; } }
            [DataMember]
            public Semaforo TabSentenzaArt4 { get { return _TabSentenzaArt4; } set { _TabSentenzaArt4 = value; } }
            [DataMember]
            public Semaforo TabSentenze { get { return _TabSentenze; } set { _TabSentenze = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroDelegatoTutore
        {
            public DatiQuadroDelegatoTutore()
            {
            }

            internal DatiQuadroDelegatoTutore(BLCommon.GestioneQuadri.DatiQuadroDelegatoTutore quadroDelegatoTutore)
            {
                switch (quadroDelegatoTutore.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroDelegatoTutore.TabDelegato.HasValue && quadroDelegatoTutore.TabDelegato.Value == 2) ||
                            (quadroDelegatoTutore.TabTutore.HasValue && quadroDelegatoTutore.TabTutore.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroDelegatoTutore.TabDelegato.HasValue && quadroDelegatoTutore.TabDelegato.Value == 0) ||
                            (quadroDelegatoTutore.TabTutore.HasValue && quadroDelegatoTutore.TabTutore.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabDelegato = ImpostaSemaforoTab(quadroDelegatoTutore.TabDelegato);
                this._TabTutore = ImpostaSemaforoTab(quadroDelegatoTutore.TabTutore);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabDelegato;
            private Semaforo _TabTutore;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabDelegato { get { return _TabDelegato; } set { _TabDelegato = value; } }
            [DataMember]
            public Semaforo TabTutore { get { return _TabTutore; } set { _TabTutore = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroDatiContributivi
        {
            public DatiQuadroDatiContributivi()
            {
            }

            internal DatiQuadroDatiContributivi(BLCommon.GestioneQuadri.DatiQuadroDatiContributivi quadroDatiContributivi)
            {
                switch (quadroDatiContributivi.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroDatiContributivi.TabDatiCalcolo.HasValue && quadroDatiContributivi.TabDatiCalcolo.Value == 2) ||
                            (quadroDatiContributivi.TabProRata.HasValue && quadroDatiContributivi.TabProRata.Value == 2) ||
                            (quadroDatiContributivi.TabContrEsteri.HasValue && quadroDatiContributivi.TabContrEsteri.Value == 2) ||
                            (quadroDatiContributivi.TabMaternAcna.HasValue && quadroDatiContributivi.TabMaternAcna.Value == 2) ||
                            (quadroDatiContributivi.TabDatiPostDecOriginaria.HasValue && quadroDatiContributivi.TabDatiPostDecOriginaria.Value == 2) ||
                            (quadroDatiContributivi.TabLavAutonomi.HasValue && quadroDatiContributivi.TabLavAutonomi.Value == 2) ||
                            (quadroDatiContributivi.TabDatiFondo.HasValue && quadroDatiContributivi.TabDatiFondo.Value == 2) ||
                            (quadroDatiContributivi.TabDatiAgo.HasValue && quadroDatiContributivi.TabDatiAgo.Value == 2) ||
                            (quadroDatiContributivi.TabArt11e14.HasValue && quadroDatiContributivi.TabArt11e14.Value == 2) ||
                            (quadroDatiContributivi.TabDatiCalcoloENPALS.HasValue && quadroDatiContributivi.TabDatiCalcoloENPALS.Value == 2) ||
                            (quadroDatiContributivi.TabAnte67.HasValue && quadroDatiContributivi.TabAnte67.Value == 2) ||
                            (quadroDatiContributivi.TabDatiCalcoloINPDAI.HasValue && quadroDatiContributivi.TabDatiCalcoloINPDAI.Value == 2) ||
                            (quadroDatiContributivi.TabQuotePensione.HasValue && quadroDatiContributivi.TabQuotePensione.Value == 2) ||
                            (quadroDatiContributivi.TabVittime.HasValue && quadroDatiContributivi.TabVittime.Value == 2) ||
                            (quadroDatiContributivi.TabDatiCalcolo707.HasValue && quadroDatiContributivi.TabDatiCalcolo707.Value == 2) ||
                            (quadroDatiContributivi.TabQuotaFondoIntegrativo.HasValue && quadroDatiContributivi.TabQuotaFondoIntegrativo == 2) ||
                            (quadroDatiContributivi.TabQuotaFondoINPGI.HasValue && quadroDatiContributivi.TabQuotaFondoINPGI == 2) ||
                            (quadroDatiContributivi.TabDatiEsteri.HasValue && quadroDatiContributivi.TabDatiEsteri == 2) ||
                            (quadroDatiContributivi.TabMiglioramentiContrattuali.HasValue && quadroDatiContributivi.TabMiglioramentiContrattuali.Value == 2)
                            )
                            //|| (quadroDatiContributivi.TabLegge407.HasValue && quadroDatiContributivi.TabLegge407.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroDatiContributivi.TabDatiCalcolo.HasValue && quadroDatiContributivi.TabDatiCalcolo.Value == 0) ||
                            (quadroDatiContributivi.TabProRata.HasValue && quadroDatiContributivi.TabProRata.Value == 0) ||
                            (quadroDatiContributivi.TabContrEsteri.HasValue && quadroDatiContributivi.TabContrEsteri.Value == 0) ||
                            (quadroDatiContributivi.TabMaternAcna.HasValue && quadroDatiContributivi.TabMaternAcna.Value == 0) ||
                            (quadroDatiContributivi.TabDatiPostDecOriginaria.HasValue && quadroDatiContributivi.TabDatiPostDecOriginaria.Value == 0) ||
                            (quadroDatiContributivi.TabLavAutonomi.HasValue && quadroDatiContributivi.TabLavAutonomi.Value == 0) ||
                            (quadroDatiContributivi.TabDatiFondo.HasValue && quadroDatiContributivi.TabDatiFondo.Value == 0) ||
                            (quadroDatiContributivi.TabDatiAgo.HasValue && quadroDatiContributivi.TabDatiAgo.Value == 0) ||
                            (quadroDatiContributivi.TabArt11e14.HasValue && quadroDatiContributivi.TabArt11e14.Value == 0) ||
                            (quadroDatiContributivi.TabDatiCalcoloENPALS.HasValue && quadroDatiContributivi.TabDatiCalcoloENPALS.Value == 0) ||
                            (quadroDatiContributivi.TabAnte67.HasValue && quadroDatiContributivi.TabAnte67.Value == 0) ||
                            (quadroDatiContributivi.TabDatiCalcoloINPDAI.HasValue && quadroDatiContributivi.TabDatiCalcoloINPDAI.Value == 0) ||
                            (quadroDatiContributivi.TabQuotePensione.HasValue && quadroDatiContributivi.TabQuotePensione.Value == 0) ||
                            (quadroDatiContributivi.TabVittime.HasValue && quadroDatiContributivi.TabVittime.Value == 0) ||
                            (quadroDatiContributivi.TabDatiCalcolo707.HasValue && quadroDatiContributivi.TabDatiCalcolo707.Value == 0) ||
                            (quadroDatiContributivi.TabQuotaFondoIntegrativo.HasValue && quadroDatiContributivi.TabQuotaFondoIntegrativo.Value == 0) ||
                            (quadroDatiContributivi.TabQuotaFondoINPGI.HasValue && quadroDatiContributivi.TabQuotaFondoINPGI.Value == 0) ||
                            (quadroDatiContributivi.TabDatiEsteri.HasValue && quadroDatiContributivi.TabDatiEsteri.Value == 0) ||
                            (quadroDatiContributivi.TabMiglioramentiContrattuali.HasValue && quadroDatiContributivi.TabMiglioramentiContrattuali.Value == 0)
                            )
                            //|| (quadroDatiContributivi.TabLegge407.HasValue && quadroDatiContributivi.TabLegge407.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabDatiCalcolo = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiCalcolo);
                this._TabProRata = ImpostaSemaforoTab(quadroDatiContributivi.TabProRata);
                this._TabContrEsteri = ImpostaSemaforoTab(quadroDatiContributivi.TabContrEsteri);
                this._TabMaternAcna = ImpostaSemaforoTab(quadroDatiContributivi.TabMaternAcna);
                this._TabDatiPostDecOriginaria = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiPostDecOriginaria);
                this._TabLavAutonomi = ImpostaSemaforoTab(quadroDatiContributivi.TabLavAutonomi);
                this._TabDatiFondo = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiFondo);
                this._TabDatiAgo = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiAgo);
                this._TabArt11e14 = ImpostaSemaforoTab(quadroDatiContributivi.TabArt11e14);
                this._TabDatiCalcoloENPALS = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiCalcoloENPALS);
                this._TabAnte67 = ImpostaSemaforoTab(quadroDatiContributivi.TabAnte67);
                this._TabSL33670 = ImpostaSemaforoTab(quadroDatiContributivi.TabSL33670);
                //this._TabLegge407    = ImpostaSemaforoTab(quadroDatiContributivi.TabLegge407);
                this._TabDatiCalcoloINPDAI = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiCalcoloINPDAI);
                this._TabQuotePensione = ImpostaSemaforoTab(quadroDatiContributivi.TabQuotePensione);
                this._TabVittime = ImpostaSemaforoTab(quadroDatiContributivi.TabVittime);
                this._TabDatiCalcolo707 = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiCalcolo707);
                this._TabStorico = ImpostaSemaforoTab(quadroDatiContributivi.TabStorico);
                this._TabIntegrazioneVirtuale = ImpostaSemaforoTab(quadroDatiContributivi.TabIntegrazioneVirtuale);
                this._TabQuotaFondoIntegrativo = ImpostaSemaforoTab(quadroDatiContributivi.TabQuotaFondoIntegrativo);
                this._TabQuotaFondoINPGI = ImpostaSemaforoTab(quadroDatiContributivi.TabQuotaFondoINPGI);
                this._TabDatiEsteri = ImpostaSemaforoTab(quadroDatiContributivi.TabDatiEsteri);
                this._TabMiglioramentiContrattuali = ImpostaSemaforoTab(quadroDatiContributivi.TabMiglioramentiContrattuali);
                this._TabQuotaFondoINPGIStorico = ImpostaSemaforoTab(quadroDatiContributivi.TabQuotaFondoINPGIStorico);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabDatiCalcolo;

            private Semaforo _TabProRata;

            private Semaforo _TabContrEsteri;

            private Semaforo _TabMaternAcna;

            private Semaforo _TabDatiPostDecOriginaria;

            private Semaforo _TabLavAutonomi;

            private Semaforo _TabDatiFondo;

            private Semaforo _TabDatiAgo;

            private Semaforo _TabArt11e14;

            private Semaforo _TabDatiCalcoloENPALS;

            private Semaforo _TabAnte67;

            private Semaforo _TabSL33670;

            private Semaforo _TabDatiCalcoloINPDAI;

            private Semaforo _TabQuotePensione;

            private Semaforo _TabVittime;

            private Semaforo _TabDatiCalcolo707;

            private Semaforo _TabStorico; // Il tab non influisce sul semaforo del quadro

            private Semaforo _TabIntegrazioneVirtuale;

            private Semaforo _TabQuotaFondoIntegrativo;

            private Semaforo _TabQuotaFondoINPGI;

            private Semaforo _TabDatiEsteri;

            public Semaforo _TabMiglioramentiContrattuali;

            public Semaforo _TabQuotaFondoINPGIStorico;

            //private Semaforo _TabLegge407;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabDatiCalcolo { get { return _TabDatiCalcolo; } set { _TabDatiCalcolo = value; } }
            [DataMember]
            public Semaforo TabProRata { get { return _TabProRata; } set { _TabProRata = value; } }
            [DataMember]
            public Semaforo TabContrEsteri { get { return _TabContrEsteri; } set { _TabContrEsteri = value; } }
            [DataMember]
            public Semaforo TabMaternAcna { get { return _TabMaternAcna; } set { _TabMaternAcna = value; } }
            [DataMember]
            public Semaforo TabDatiPostDecOriginaria { get { return _TabDatiPostDecOriginaria; } set { _TabDatiPostDecOriginaria = value; } }
            [DataMember]
            public Semaforo TabLavAutonomi { get { return _TabLavAutonomi; } set { _TabLavAutonomi = value; } }
            [DataMember]
            public Semaforo TabDatiFondo { get { return _TabDatiFondo; } set { _TabDatiFondo = value; } }
            [DataMember]
            public Semaforo TabDatiAgo { get { return _TabDatiAgo; } set { _TabDatiAgo = value; } }
            [DataMember]
            public Semaforo TabArt11e14 { get { return _TabArt11e14; } set { _TabArt11e14 = value; } }
            [DataMember]
            public Semaforo TabDatiCalcoloENPALS { get { return _TabDatiCalcoloENPALS; } set { _TabDatiCalcoloENPALS = value; } }
            //[DataMember]
            //public Semaforo TabLegge407 { get { return _TabLegge407; } set { _TabLegge407 = value; } }
            [DataMember]
            public Semaforo TabAnte67 { get { return _TabAnte67; } set { _TabAnte67 = value; } }
            [DataMember]
            public Semaforo TabSL33670 { get { return _TabSL33670; } set { _TabSL33670 = value; } }
            [DataMember]
            public Semaforo TabDatiCalcoloINPDAI { get { return _TabDatiCalcoloINPDAI; } set { _TabDatiCalcoloINPDAI = value; } }
            [DataMember]
            public Semaforo TabQuotePensione { get { return _TabQuotePensione; } set { _TabQuotePensione = value; } }
            [DataMember]
            public Semaforo TabVittime { get { return _TabVittime; } set { _TabVittime = value; } }
            [DataMember]
            public Semaforo TabDatiCalcolo707 { get { return _TabDatiCalcolo707; } set { _TabDatiCalcolo707 = value; } }
            [DataMember]
            public Semaforo TabStorico { get { return _TabStorico; } set { _TabStorico = value; } }
            [DataMember]
            public Semaforo TabIntegrazioneVirtuale { get { return _TabIntegrazioneVirtuale; } set { _TabIntegrazioneVirtuale = value; } }
            [DataMember]
            public Semaforo TabQuotaFondoIntegrativo { get { return _TabQuotaFondoIntegrativo; } set { _TabQuotaFondoIntegrativo = value; } }
            [DataMember]
            public Semaforo TabQuotaFondoINPGI { get { return _TabQuotaFondoINPGI; } set { _TabQuotaFondoINPGI = value; } }
            [DataMember]
            public Semaforo TabDatiEsteri { get { return _TabDatiEsteri; } set { _TabDatiEsteri = value; } }
            [DataMember]
            public Semaforo TabMiglioramentiContrattuali { get { return _TabMiglioramentiContrattuali; } set { _TabMiglioramentiContrattuali = value; } }
            [DataMember]
            public Semaforo TabQuotaFondoINPGIStorico { get { return _TabQuotaFondoINPGIStorico; } set { _TabQuotaFondoINPGIStorico = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroRedditi
        {
            public DatiQuadroRedditi()
            {
            }

            internal DatiQuadroRedditi(BLCommon.GestioneQuadri.DatiQuadroRedditi quadroRedditi)
            {
                switch (quadroRedditi.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroRedditi.TabRedditi.HasValue && quadroRedditi.TabRedditi.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroRedditi.TabRedditi.HasValue && quadroRedditi.TabRedditi.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabRedditi = ImpostaSemaforoTab(quadroRedditi.TabRedditi);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabRedditi;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabRedditi { get { return _TabRedditi; } set { _TabRedditi = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroFamiliari
        {
            public DatiQuadroFamiliari()
            {
            }

            internal DatiQuadroFamiliari(BLCommon.GestioneQuadri.DatiQuadroFamiliari quadroFamiliari)
            {
                switch (quadroFamiliari.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroFamiliari.TabFamiliari.HasValue && quadroFamiliari.TabFamiliari.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroFamiliari.TabFamiliari.HasValue && quadroFamiliari.TabFamiliari.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabFamiliari = ImpostaSemaforoTab(quadroFamiliari.TabFamiliari);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabFamiliari;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabFamiliari { get { return _TabFamiliari; } set { _TabFamiliari = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroDanteCausa
        {
            public DatiQuadroDanteCausa()
            {
            }

            internal DatiQuadroDanteCausa(BLCommon.GestioneQuadri.DatiQuadroDanteCausa quadroDanteCausa)
            {
                switch (quadroDanteCausa.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroDanteCausa.TabAnagrafica.HasValue && quadroDanteCausa.TabAnagrafica.Value == 2) ||
                            (quadroDanteCausa.TabPensioneDiretta.HasValue && quadroDanteCausa.TabPensioneDiretta.Value == 2) ||
                            (quadroDanteCausa.TabAltraPensione.HasValue && quadroDanteCausa.TabAltraPensione.Value == 2) ||
                            (quadroDanteCausa.TabDatiPensioneCI.HasValue && quadroDanteCausa.TabDatiPensioneCI.Value == 2) ||
                            (quadroDanteCausa.TabSentenza49593.HasValue && quadroDanteCausa.TabSentenza49593.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroDanteCausa.TabAnagrafica.HasValue && quadroDanteCausa.TabAnagrafica.Value == 0) ||
                            (quadroDanteCausa.TabPensioneDiretta.HasValue && quadroDanteCausa.TabPensioneDiretta.Value == 0) ||
                            (quadroDanteCausa.TabAltraPensione.HasValue && quadroDanteCausa.TabAltraPensione.Value == 0) ||
                            (quadroDanteCausa.TabDatiPensioneCI.HasValue && quadroDanteCausa.TabDatiPensioneCI.Value == 0) ||
                            (quadroDanteCausa.TabSentenza49593.HasValue && quadroDanteCausa.TabSentenza49593.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabAnagrafica = ImpostaSemaforoTab(quadroDanteCausa.TabAnagrafica);
                this._TabPensioneDiretta = ImpostaSemaforoTab(quadroDanteCausa.TabPensioneDiretta);
                this._TabAltraPensione = ImpostaSemaforoTab(quadroDanteCausa.TabAltraPensione);
                this._TabDatiPensioneCI = ImpostaSemaforoTab(quadroDanteCausa.TabDatiPensioneCI);
                this._TabSentenza49593 = ImpostaSemaforoTab(quadroDanteCausa.TabSentenza49593);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabAnagrafica;

            private Semaforo _TabPensioneDiretta;

            private Semaforo _TabAltraPensione;

            private Semaforo _TabDatiPensioneCI;

            private Semaforo _TabSentenza49593;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabAnagrafica { get { return _TabAnagrafica; } set { _TabAnagrafica = value; } }
            [DataMember]
            public Semaforo TabPensioneDiretta { get { return _TabPensioneDiretta; } set { _TabPensioneDiretta = value; } }
            [DataMember]
            public Semaforo TabAltraPensione { get { return _TabAltraPensione; } set { _TabAltraPensione = value; } }
            [DataMember]
            public Semaforo TabDatiPensioneCI { get { return _TabDatiPensioneCI; } set { _TabDatiPensioneCI = value; } }
            [DataMember]
            public Semaforo TabSentenza49593 { get { return _TabSentenza49593; } set { _TabSentenza49593 = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroMaggiorazioniBenefici
        {
            public DatiQuadroMaggiorazioniBenefici()
            {
            }

            internal DatiQuadroMaggiorazioniBenefici(BLCommon.GestioneQuadri.DatiQuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici)
            {
                switch (quadroMaggiorazioniBenefici.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroMaggiorazioniBenefici.TabExCombattente.HasValue && quadroMaggiorazioniBenefici.TabExCombattente.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabBenefici.HasValue && quadroMaggiorazioniBenefici.TabBenefici.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && quadroMaggiorazioniBenefici.TabMaggiorazioni.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabLegge407.HasValue && quadroMaggiorazioniBenefici.TabLegge407.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue && quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && quadroMaggiorazioniBenefici.TabPrivilegiate.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabArticolo2.HasValue && quadroMaggiorazioniBenefici.TabArticolo2.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else if ((quadroMaggiorazioniBenefici.TabExCombattente.HasValue && quadroMaggiorazioniBenefici.TabExCombattente.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabBenefici.HasValue && quadroMaggiorazioniBenefici.TabBenefici.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && quadroMaggiorazioniBenefici.TabMaggiorazioni.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabLegge407.HasValue && quadroMaggiorazioniBenefici.TabLegge407.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue && quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && quadroMaggiorazioniBenefici.TabPrivilegiate.Value == 2) ||
                            (quadroMaggiorazioniBenefici.TabArticolo2.HasValue && quadroMaggiorazioniBenefici.TabArticolo2.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroMaggiorazioniBenefici.TabExCombattente.HasValue && quadroMaggiorazioniBenefici.TabExCombattente.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabBenefici.HasValue && quadroMaggiorazioniBenefici.TabBenefici.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabMaggiorazioni.HasValue && quadroMaggiorazioniBenefici.TabMaggiorazioni.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabLegge407.HasValue && quadroMaggiorazioniBenefici.TabLegge407.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.HasValue && quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabPrivilegiate.HasValue && quadroMaggiorazioniBenefici.TabPrivilegiate.Value == 0) ||
                            (quadroMaggiorazioniBenefici.TabArticolo2.HasValue && quadroMaggiorazioniBenefici.TabArticolo2.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabExCombattente = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabExCombattente);
                this._TabBenefici = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabBenefici);
                this._TabMaggiorazioni = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabMaggiorazioni);
                this._TabDL407 = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabLegge407);
                this._TabBeneficioVittimeTerrorismo = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo);
                this._TabPrivilegiate = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabPrivilegiate);
                this._TabArticolo2 = ImpostaSemaforoTab(quadroMaggiorazioniBenefici.TabArticolo2);

            }

            #region private properties
            private Semaforo _Quadro;
            private Semaforo _TabExCombattente;
            private Semaforo _TabBenefici;
            private Semaforo _TabDL407;
            private Semaforo _TabBeneficioVittimeTerrorismo;
            private Semaforo _TabPrivilegiate;
            private Semaforo _TabArticolo2;
            private Semaforo _TabMaggiorazioni;

            #endregion private properties

            #region public data member

            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabExCombattente { get { return _TabExCombattente; } set { _TabExCombattente = value; } }
            [DataMember]
            public Semaforo TabBenefici { get { return _TabBenefici; } set { _TabBenefici = value; } }
            [DataMember]
            public Semaforo TabDL407 { get { return _TabDL407; } set { _TabDL407 = value; } }
            [DataMember]
            public Semaforo TabBeneficioVittimeTerrorismo { get { return _TabBeneficioVittimeTerrorismo; } set { _TabBeneficioVittimeTerrorismo = value; } }
            [DataMember]
            public Semaforo TabPrivilegiate { get { return _TabPrivilegiate; } set { _TabPrivilegiate = value; } }
            [DataMember]
            public Semaforo TabArticolo2 { get { return _TabArticolo2; } set { _TabArticolo2 = value; } }
            [DataMember]
            public Semaforo TabMaggiorazioni { get { return _TabMaggiorazioni; } set { _TabMaggiorazioni = value; } }

            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroSupplementi
        {
            public DatiQuadroSupplementi()
            {
            }

            internal DatiQuadroSupplementi(BLCommon.GestioneQuadri.DatiQuadroSupplementi quadroSupplementi)
            {
                switch (quadroSupplementi.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if ((quadroSupplementi.TabSupplementi.HasValue && quadroSupplementi.TabSupplementi.Value == 2) ||
                            (quadroSupplementi.TabIntegrazioneArt11.HasValue && quadroSupplementi.TabIntegrazioneArt11.Value == 2) ||
                             (quadroSupplementi.TabContribuzioneEnpals.HasValue && quadroSupplementi.TabContribuzioneEnpals.Value == 2))
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if ((quadroSupplementi.TabSupplementi.HasValue && quadroSupplementi.TabSupplementi.Value == 0) ||
                            (quadroSupplementi.TabIntegrazioneArt11.HasValue && quadroSupplementi.TabIntegrazioneArt11.Value == 0) ||
                            (quadroSupplementi.TabContribuzioneEnpals.HasValue && quadroSupplementi.TabContribuzioneEnpals.Value == 0))
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabSupplementi = ImpostaSemaforoTab(quadroSupplementi.TabSupplementi);
                this._TabIntegrazioneArt11 = ImpostaSemaforoTab(quadroSupplementi.TabIntegrazioneArt11);
                this._TabDatiContribuzioneEnpals = ImpostaSemaforoTab(quadroSupplementi.TabContribuzioneEnpals);
                this._TabStoricoSupplementi = ImpostaSemaforoTab(quadroSupplementi.TabStoricoSupplementi);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabSupplementi;

            private Semaforo _TabIntegrazioneArt11;

            private Semaforo _TabDatiContribuzioneEnpals;

            private Semaforo _TabStoricoSupplementi;

            #endregion private properties

            #region public data member

            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabSupplementi { get { return _TabSupplementi; } set { _TabSupplementi = value; } }
            [DataMember]
            public Semaforo TabIntegrazioneArt11 { get { return _TabIntegrazioneArt11; } set { _TabIntegrazioneArt11 = value; } }
            [DataMember]
            public Semaforo TabDatiContribuzioneEnpals { get { return _TabDatiContribuzioneEnpals; } set { _TabDatiContribuzioneEnpals = value; } }
            [DataMember]
            public Semaforo TabStoricoSupplementi { get { return _TabStoricoSupplementi; } set { _TabStoricoSupplementi = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroBititolarita
        {
            public DatiQuadroBititolarita()
            {
            }

            internal DatiQuadroBititolarita(BLCommon.GestioneQuadri.DatiQuadroBititolarita quadroBititolarita)
            {
                switch (quadroBititolarita.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroBititolarita.TabAltrePensioni.HasValue && quadroBititolarita.TabAltrePensioni.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroBititolarita.TabAltrePensioni.HasValue && quadroBititolarita.TabAltrePensioni.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabAltrePensioni = ImpostaSemaforoTab(quadroBititolarita.TabAltrePensioni);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabAltrePensioni;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabAltrePensioni { get { return _TabAltrePensioni; } set { _TabAltrePensioni = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroEliminazione
        {
            public DatiQuadroEliminazione()
            {
            }

            internal DatiQuadroEliminazione(BLCommon.GestioneQuadri.DatiQuadroEliminazione quadroEliminazione)
            {
                switch (quadroEliminazione.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroEliminazione.TabEliminazione.HasValue && quadroEliminazione.TabEliminazione.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroEliminazione.TabEliminazione.HasValue && quadroEliminazione.TabEliminazione.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabEliminazione = ImpostaSemaforoTab(quadroEliminazione.TabEliminazione);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabEliminazione;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabEliminazione { get { return _TabEliminazione; } set { _TabEliminazione = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroOneri
        {
            #region public data member
            [DataMember]
            public Semaforo Quadro { get; set; }
            [DataMember]
            public Semaforo TabOneri { get; set; }
            [DataMember]
            public Semaforo TabPrepensionamento { get; set; }
            [DataMember]
            public Semaforo TabStorico { get; set; }
            #endregion public data member

            public DatiQuadroOneri()
            {
            }

            internal DatiQuadroOneri(BLCommon.GestioneQuadri.DatiQuadroOneri quadroOneri)
            {
                switch (quadroOneri.Tipo)
                {
                    //non necessario
                    case 0:
                        this.Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    //facoltativo
                    case 1:
                        if ((quadroOneri.TabOneri.HasValue && quadroOneri.TabOneri == 2) || (quadroOneri.TabPrepensionamento.HasValue && quadroOneri.TabPrepensionamento == 2))
                            this.Quadro = Semaforo.Verde;
                        else
                            this.Quadro = Semaforo.Giallo;
                        break;
                    //obbligatorio
                    case 2:
                        if ((quadroOneri.TabOneri.HasValue && quadroOneri.TabOneri == 0) || (quadroOneri.TabPrepensionamento.HasValue && quadroOneri.TabPrepensionamento == 0))

                            this.Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this.Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }
                this.TabOneri = ImpostaSemaforoTab(quadroOneri.TabOneri);
                this.TabPrepensionamento = ImpostaSemaforoTab(quadroOneri.TabPrepensionamento);
                this.TabStorico = ImpostaSemaforoTab(quadroOneri.TabStorico);
            }
        }

        [DataContract]
        public class DatiQuadroDatiFondo
        {
            #region public data member
            [DataMember]
            public Semaforo Quadro { get; set; }
            [DataMember]
            public Semaforo TabRegistrazioniFondo { get; set; }

            #endregion public data member

            public DatiQuadroDatiFondo()
            { }

            internal DatiQuadroDatiFondo(BLCommon.GestioneQuadri.DatiQuadroDatiFondo quadroDatiFondo)
            {
                switch (quadroDatiFondo.Tipo)
                {
                    //non necessario
                    case 0:
                        this.Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    //facoltativo
                    case 1:
                        if ((quadroDatiFondo.TabRegistrazioniFondo.HasValue && quadroDatiFondo.TabRegistrazioniFondo == 2))
                            this.Quadro = Semaforo.Verde;
                        else
                            this.Quadro = Semaforo.Giallo;
                        break;
                    //obbligatorio
                    case 2:
                        if ((quadroDatiFondo.TabRegistrazioniFondo.HasValue && quadroDatiFondo.TabRegistrazioniFondo == 2))

                            this.Quadro = Semaforo.Verde;
                        else
                            this.Quadro = Semaforo.Rosso_Abilitato;
                        break;
                    default:
                        break;
                }
                this.TabRegistrazioniFondo = ImpostaSemaforoTab(quadroDatiFondo.TabRegistrazioniFondo);
            }
        }

        [DataContract]
        public class DatiQuadroDatiNoCalcolo
        {
            #region public data member
            [DataMember]
            public Semaforo Quadro { get; set; }
            [DataMember]
            public Semaforo TabRecordNoCalcolo { get; set; }
            #endregion public data member

            public DatiQuadroDatiNoCalcolo()
            { }

            internal DatiQuadroDatiNoCalcolo(BLCommon.GestioneQuadri.DatiQuadroDatiNoCalcolo quadroDatiNoCalcolo)
            {
                switch (quadroDatiNoCalcolo.Tipo)
                {
                    //non necessario
                    case 0:
                        this.Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    //facoltativo
                    case 1:
                        if ((quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo.HasValue && quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo == 2))
                            this.Quadro = Semaforo.Verde;
                        else
                            this.Quadro = Semaforo.Giallo;
                        break;
                    //obbligatorio
                    case 2:
                        if ((quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo.HasValue && quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo == 2))

                            this.Quadro = Semaforo.Verde;
                        else
                            this.Quadro = Semaforo.Rosso_Abilitato;
                        break;
                    default:
                        break;
                }
                this.TabRecordNoCalcolo = ImpostaSemaforoTab(quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo);
            }
        }

        [DataContract]
        public class DatiQuadroPeriodi
        {
            public DatiQuadroPeriodi()
            {
            }

            internal DatiQuadroPeriodi(BLCommon.GestioneQuadri.DatiQuadroPeriodi quadroPeriodi)
            {
                switch (quadroPeriodi.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroPeriodi.TabPeriodi.HasValue && quadroPeriodi.TabPeriodi.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroPeriodi.TabPeriodi.HasValue && quadroPeriodi.TabPeriodi.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabPeriodi = ImpostaSemaforoTab(quadroPeriodi.TabPeriodi);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabPeriodi;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabPeriodi { get { return _TabPeriodi; } set { _TabPeriodi = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroAventiDiritto
        {
            public DatiQuadroAventiDiritto()
            {
            }

            internal DatiQuadroAventiDiritto(BLCommon.GestioneQuadri.DatiQuadroAventiDiritto quadroAventiDiritto)
            {
                switch (quadroAventiDiritto.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroAventiDiritto.TabAventiDiritto.HasValue && quadroAventiDiritto.TabAventiDiritto.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroAventiDiritto.TabAventiDiritto.HasValue && quadroAventiDiritto.TabAventiDiritto.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabAventiDiritto = ImpostaSemaforoTab(quadroAventiDiritto.TabAventiDiritto);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabAventiDiritto;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabAventiDiritto { get { return _TabAventiDiritto; } set { _TabAventiDiritto = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroAltreDomandeCollegate
        {
            public DatiQuadroAltreDomandeCollegate()
            {
            }

            internal DatiQuadroAltreDomandeCollegate(BLCommon.GestioneQuadri.DatiQuadroAltreDomandeCollegate quadroAltreDomandeCollegate)
            {
                switch (quadroAltreDomandeCollegate.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroAltreDomandeCollegate.TabAltreDomandeCollegate.HasValue && quadroAltreDomandeCollegate.TabAltreDomandeCollegate.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroAltreDomandeCollegate.TabAltreDomandeCollegate.HasValue && quadroAltreDomandeCollegate.TabAltreDomandeCollegate.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabAltreDomandeCollegate = ImpostaSemaforoTab(quadroAltreDomandeCollegate.TabAltreDomandeCollegate);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabAltreDomandeCollegate;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabAltreDomandeCollegate { get { return _TabAltreDomandeCollegate; } set { _TabAltreDomandeCollegate = value; } }
            #endregion public data member
        }

        [DataContract]
        public class DatiQuadroRichiestaBonus
        {
            public DatiQuadroRichiestaBonus()
            {
            }

            internal DatiQuadroRichiestaBonus(BLCommon.GestioneQuadri.DatiQuadroRichiestaBonus quadroRichiestaBonus)
            {
                switch (quadroRichiestaBonus.Tipo)
                {
                    case 0:
                        this._Quadro = Semaforo.Rosso_NonAbilitato;
                        break;
                    case 1:
                        if (quadroRichiestaBonus.TabRichiestaBonus.HasValue && quadroRichiestaBonus.TabRichiestaBonus.Value == 2)
                            this._Quadro = Semaforo.Verde;
                        else
                            this._Quadro = Semaforo.Giallo;
                        break;
                    case 2:
                        if (quadroRichiestaBonus.TabRichiestaBonus.HasValue && quadroRichiestaBonus.TabRichiestaBonus.Value == 0)
                            this._Quadro = Semaforo.Rosso_Abilitato;
                        else
                            this._Quadro = Semaforo.Verde;
                        break;
                    default:
                        break;
                }

                this._TabRichiestaBonus = ImpostaSemaforoTab(quadroRichiestaBonus.TabRichiestaBonus);
                this._TabEsitoPrenotazione = ImpostaSemaforoTab(quadroRichiestaBonus.TabEsitoPrenotazione);
            }

            #region private properties
            private Semaforo _Quadro;

            private Semaforo _TabRichiestaBonus;

            private Semaforo _TabEsitoPrenotazione;
            #endregion private properties

            #region public data member
            [DataMember]
            public Semaforo Quadro { get { return _Quadro; } set { _Quadro = value; } }
            [DataMember]
            public Semaforo TabRichiestaBonus { get { return _TabRichiestaBonus; } set { _TabRichiestaBonus = value; } }
            [DataMember]
            public Semaforo TabEsitoPrenotazione { get { return _TabEsitoPrenotazione; } set { _TabEsitoPrenotazione = value; } }
            #endregion public data member
        }

        #endregion nested class

        public enum Semaforo
        {
            Rosso_NonAbilitato,
            Rosso_Abilitato,
            Giallo,
            Verde
        };

        internal static Semaforo ImpostaSemaforoTab(byte? tab)
        {
            switch (tab)
            {
                case 0:
                    return Semaforo.Rosso_Abilitato;
                case 1:
                    return Semaforo.Giallo;
                case 2:
                    return Semaforo.Verde;
                case null:
                default:
                    return Semaforo.Rosso_NonAbilitato;
            }
        }

        public enum Tab
        {
            Titolare, Detrazioni, LiquidazionePensione, Pagamento,
            DelegatoTutore, Familiare, MaggiorazioniEBenefici, Redditi,
            Supplementi, Bititolarita, DanteCausa, DatiCalcolo, Eliminazione, Oneri,
            DatiFondo, DatiNoCalcolo, Periodi, AventiDiritto, AltreDomandeCollegate,
            RichiestaBonus
        };
    }
}
