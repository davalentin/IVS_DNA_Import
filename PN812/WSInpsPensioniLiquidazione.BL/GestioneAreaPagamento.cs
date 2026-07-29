using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaPagamento
    {
        #region public members
        public static bool GetPagamentoByDatiPensione(GestionePensione.DatiPensione datiPensione, out DatiPagamento areaPagamentoBL, out string errori)
        {
            errori = "";
            areaPagamentoBL = null;
            try
            {
                //recupero pagamento da DB
                GestionePagamento.DatiPagamento PagamentoDB = null;
                GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out PagamentoDB);
                if (PagamentoDB != null)
                {
                    areaPagamentoBL = new DatiPagamento();
                    Utility.ValorizzaOggetti(PagamentoDB, areaPagamentoBL);

                    #region Gestione di circolarità legate al tipo Appartenenza

                    CustomizePagamentoByDatiPensione(datiPensione, ref areaPagamentoBL, true);

                    #endregion Gestione di circolarità legate al tipo Appartenenza

                    return true;
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore nel metodo GetPagamentoByDomanda: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool StorePagamentoByDatiPensione(ref GestionePensione.DatiPensione datiPensione, GestioneAreaPagamento.DatiPagamento areaPagamento, string matricola, string sede, out string errori)
        {
            errori = string.Empty;
            try
            {
                GestioneQuadri.DatiQuadroPagamento datiQuadroPagamento = null;
                GestioneQuadri.GetQuadroPagamentoByDatiPensione(datiPensione, out datiQuadroPagamento);

                GestionePagamento.DatiPagamento datiPagamentoDB = null;
                GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamentoDB);

                if (!GestioneCrossControls.ALL_ControlsBancaItalia(datiPensione, areaPagamento.ABI, areaPagamento.CAB, out errori))
                    return false;

                if (!GestioneCrossControls.ALL_ControlsDatiPagamento(areaPagamento.TipoPagamento, areaPagamento.ModalitaPagamento, areaPagamento.IBAN, areaPagamento.Libretto, out errori))
                    return false;

                if (!GestioneCrossControls.AGO_ControlsDatiPagamentoByDatiCalcolo(datiPensione, areaPagamento.TipoPagamento, areaPagamento.ModalitaPagamento, out errori))
                    return false;

                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneServizioVerificaTitolarita", out ctrl);
                if (ctrl != null && ctrl.ValoreControllo == "SI" && (areaPagamento.TipoPagamento == 'B' || areaPagamento.TipoPagamento == 'P') &&
                    (areaPagamento.ModalitaPagamento == 'C' || areaPagamento.ModalitaPagamento == 'K' || areaPagamento.ModalitaPagamento == 'L') && !string.IsNullOrEmpty(areaPagamento.IBAN))
                {
                    GestioneVerTitolIBAN.AreaTitolarita areaTitolarita = new GestioneVerTitolIBAN.AreaTitolarita();
                    GestioneAnagrafica.DatiAnagrafici datiAnagraficiTitolare = null;
                    GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagraficiTitolare);
                    areaTitolarita.CodiceIban = areaPagamento.IBAN;
                    areaTitolarita.CodiceFiscale = datiAnagraficiTitolare.CodiceFiscale;
                    areaTitolarita.NumDomanda = datiPensione.NDomus.ToString();
                    if (!GestioneVerTitolIBAN.GetStatoTitolarita(ref areaTitolarita, matricola, sede, out errori))
                        return false;
                    if (areaTitolarita.Status == "2" || areaTitolarita.Status == "5" || areaTitolarita.Status == "51" || areaTitolarita.Status == "54")
                    {
                        errori = "La banca ha dichiarato che il beneficiario NON è intestatario o cointestatario del codice IBAN fornito.";
                        return false;
                    }
                    if (areaTitolarita.Status == "3")
                    {
                        errori = "Attendere l'esito della verifica di titolarità dell'Iban.";
                        return false;
                    }
                }

                if (datiPagamentoDB == null)
                    datiPagamentoDB = new GestionePagamento.DatiPagamento();

                GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidateDB = null;
                bool isCambioSede = false;
                if (!IsCambioSedeDestinazione(areaPagamento, datiPagamentoDB, ref datiPensione, out datiNuoveLiquidateDB, out isCambioSede, out errori))
                    return false;

                #region Gestione di circolarità legate ai tipo Appartenenza

                CustomizePagamentoByDatiPensione(datiPensione, ref areaPagamento, false);

                #endregion Gestione di circolarità legate ai tipo Appartenenza

                Utility.ValorizzaOggetti(areaPagamento, datiPagamentoDB);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    if (isCambioSede)
                    {
                        GestionePensione.SalvaPensione(datiPensione);
                        if (datiNuoveLiquidateDB != null)
                            GestioneNuoveLiquidate.SalvaNuoveLiquidate(datiNuoveLiquidateDB);
                    }
                    GestioneQuadri.GestioneSemaforoQuadroPagamento(datiPensione, datiPagamentoDB, ref datiQuadroPagamento);

                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                errori = "Errore nel metodo StorePagamentoByDomanda: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        private static bool IsCambioSedeDestinazione(GestioneAreaPagamento.DatiPagamento areaPagamento, GestionePagamento.DatiPagamento datiPagamentoDB, ref GestionePensione.DatiPensione datiPensione, out GestioneNuoveLiquidate.NuoveLiquidate datiNuoveLiquidateDB, out bool isCambioSede, out string errori)
        {
            isCambioSede = false;
            errori = string.Empty;
            datiNuoveLiquidateDB = null;
            GestioneAnagrafica.DatiAnagrafici titolare = null;
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out titolare);  
            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsPolarizzazionePerGestioneENPALSAttiva(datiPensione) &&
                !Utility.IsRicostituzioneOrRiapertura(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) &&
                titolare != null && !Utility.IsResidenteEstero(titolare.CodiceComuneResidenza))
            {
                GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out datiNuoveLiquidateDB);
                if (datiNuoveLiquidateDB == null)
                {
                    datiNuoveLiquidateDB = new GestioneNuoveLiquidate.NuoveLiquidate();
                    datiNuoveLiquidateDB.IdPensione = datiPensione.Id;
                }

                if (areaPagamento.TipoPagamento == 'E')
                {
                    GestioneControlliDinamici.ControlloDinamico ctrl = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SedePoloENPALS", out ctrl);
                    short codiceSedePoloEnpals = 0;
                    if (ctrl != null && !String.IsNullOrEmpty(ctrl.ValoreControllo))
                        short.TryParse(ctrl.ValoreControllo, out codiceSedePoloEnpals);

                    datiPensione.CodiceSedeDestinazione = codiceSedePoloEnpals;
                    datiPensione.CentroOperativoDestinazione = 0;
                    datiNuoveLiquidateDB.CodiceProcessoDestinazione = 26;
                    isCambioSede = true;
                }
                else if (datiPagamentoDB.TipoPagamento == 'E')
                {
                    isCambioSede = true;
                    if (Utility.IsPoloPALS(datiPensione))
                    {
                        string sedeDestinazione = string.Empty;
                        if (!GestioneWebDom.GetSedeDestinazione(datiPensione.NDomus, titolare.CodiceComuneResidenza, titolare.CAP, out sedeDestinazione, out errori))
                            return false;
                        if (!string.IsNullOrEmpty(sedeDestinazione) && sedeDestinazione.Length == 6)
                        {
                            short codiceSedeDestinazione = 0;
                            byte centroOperativoDestinazione = 0;
                            short.TryParse(sedeDestinazione.Substring(0, 4), out codiceSedeDestinazione);
                            byte.TryParse(sedeDestinazione.Substring(4, 2), out centroOperativoDestinazione);
                            datiPensione.CodiceSedeDestinazione = codiceSedeDestinazione;
                            datiPensione.CentroOperativoDestinazione = centroOperativoDestinazione;
                            datiNuoveLiquidateDB.CodiceProcessoDestinazione = null;
                        }
                    }
                    else
                    {
                        datiPensione.CodiceSedeDestinazione = null;
                        datiPensione.CentroOperativoDestinazione = null;
                        datiNuoveLiquidateDB.CodiceProcessoDestinazione = null;
                    }
                }
            }
            return true;
        }

        public static bool CancelPagamentoByDatiPensione(GestionePensione.DatiPensione datiPensione, out string errori)
        {
            errori = string.Empty;
            try
            {
                GestionePagamento.DatiPagamento datiPagamentoDB = null;
                GestionePagamento.GetPagamentoByIdPensione(datiPensione.Id, out datiPagamentoDB);

                bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {

                    if (datiPagamentoDB != null && datiPensione != null)
                    {
                        if (datiPagamentoDB.TrattenutaInpdap.HasValue || datiPagamentoDB.DataRinunciaTrattenutaInpdap.HasValue)
                        {
                            Utility.ValorizzaOggetti(new GestioneAreaPagamento.DatiPagamento(), datiPagamentoDB);
                            GestionePagamento.SalvaPagamento(datiPensione.Id, datiPagamentoDB);

                        }
                        else
                            GestionePagamento.EliminaPagamentoByIdPensione(datiPensione.Id);

                        GestioneQuadri.InizializzaQuadroPagamento(datiPensione, Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione),
                            Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto), isRiaperturaDomanda);
                    }
                    transactionScope.Complete();
                }

            }
            catch (Exception Ex)
            {
                errori = "Errore nel metodo CancelPagamentoByDomanda: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// gestione della modalità pagamento circolarità per fondo PT
        /// </summary>
        /// <param name="numeroDomanda"></param>
        /// <param name="areaPagamentoBL"></param>
        /// <param name="IsGetOperation"></param>
        private static void CustomizePagamentoByDatiPensione(GestionePensione.DatiPensione datiPensione, ref DatiPagamento areaPagamentoBL, bool IsGetOperation)
        {
            Utility.TipoAppartenenza? tipoAppartenenza = BLCommon.Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            if (tipoAppartenenza.Value == Utility.TipoAppartenenza.FS)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                if (tipoFondo.HasValue)
                {
                    switch (tipoFondo.Value)
                    {
                        case Utility.TipoFondo.PT:
                            if (areaPagamentoBL.TipoPagamento == 'P')
                                if (IsGetOperation && areaPagamentoBL.ModalitaPagamento == 'S')
                                    areaPagamentoBL.ModalitaPagamento = 'X';
                                else
                                    if (areaPagamentoBL.ModalitaPagamento == 'X')
                                        areaPagamentoBL.ModalitaPagamento = 'S';
                            break;
                    }
                }
            }
        }

        #endregion public members

        #region GetListDecodifica

        public static void GetListCassaSede(GestionePensione.DatiPensione datiPensione, int abi, out List<DatiCassaSede> ListCassaSedeBL, out string errori)
        {
            Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            List<GestioneUfficiPagatori.AreaUfficioPagatore> elencoUfficiPagatori = null;
            ListCassaSedeBL = new List<DatiCassaSede>();

            GestioneUfficiPagatori.GetListaCassaSedeNew(out elencoUfficiPagatori, out errori);

            List<GestioneDecodifica.DecCassaSede> elencoCassaSedeAbilitate = null;
            GestioneDecodifica.GetElencoCassaSede(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo, tipoApp, out elencoCassaSedeAbilitate);

            if (elencoCassaSedeAbilitate != null && elencoCassaSedeAbilitate.Count > 0)
            {
                foreach (GestioneDecodifica.DecCassaSede cassaSedeAbilitata in elencoCassaSedeAbilitate)
                {
                    if (elencoUfficiPagatori != null && elencoUfficiPagatori.Count > 0)
                    {
                        GestioneUfficiPagatori.AreaUfficioPagatore ufficioCassa = elencoUfficiPagatori.Find(x => x.Cab == cassaSedeAbilitata.Cab);
                        if (ufficioCassa != null)
                        {
                            DatiCassaSede datiCassa = new DatiCassaSede();
                            datiCassa.Abi = ufficioCassa.Abi;
                            datiCassa.Cab = ufficioCassa.Cab;
                            datiCassa.Agenzia = ufficioCassa.Agenzia;

                            //L'eventuale presenza della cassa sede 3300012 è per il solo fondo VL
                            if (datiCassa.Cab == 3300012 && tipoApp.GetValueOrDefault() == Utility.TipoAppartenenza.FS)
                            {
                                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoApp, datiPensione.SiglaCategoria);
                                if (tipoFondo.GetValueOrDefault() != Utility.TipoFondo.VL)
                                    continue;
                            }
                            ListCassaSedeBL.Add(datiCassa);
                        }
                    }
                }
            }
        }

        public static void GetListStatiEsteri(out List<DatiStatoEstero> ListStatiEsteri, out string errori)
        {
            ListStatiEsteri = null;
            GestioneUfficiPagatori.GetStatiEsteri(out ListStatiEsteri, out errori);
        }

        #endregion GetListDecodifica

        public static Dictionary<string, bool?> GetCrossProperties(GestionePensione.DatiPensione datiPensione, DatiPagamento datiPagamento)
        {
            bool isBancaItaliaFromWebDom = false;
            bool isPolarizzazionePerGestioneENPALSAttiva = false;

            Dictionary<string, bool?> lReturn = new Dictionary<string, bool?>();
            isBancaItaliaFromWebDom = IsBancaItaliaFromWebDom(datiPensione, datiPagamento);
            isPolarizzazionePerGestioneENPALSAttiva = Utility.IsPolarizzazionePerGestioneENPALSAttiva(datiPensione);

            lReturn.Add("IsBancaItaliaFromWebDom", isBancaItaliaFromWebDom);
            lReturn.Add("IsPolarizzazionePerGestioneENPALSAttiva", isPolarizzazionePerGestioneENPALSAttiva);

            return lReturn;
        }

        private static bool IsBancaItaliaFromWebDom(GestionePensione.DatiPensione datiPensione, DatiPagamento datiPagamento)
        {
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            return !(new List<string> { "INDCOM", "APE", "VOCRED", "CRED27", "VOCOOP", "COOP28", "VOESO", "VESO33", "VESO92", "INVCIV", "AS" }.Contains(datiPensione.SiglaCategoria.Trim())) &&
                   Utility.IsDomandaAPEPrecoci(datiPensione) && tipoAppartenenza != Utility.TipoAppartenenza.FS && datiPagamento != null && datiPagamento.ABI == 1000 && datiPagamento.CAB == 6603203 &&
                   datiPagamento.IsFromWebDom;
        }

        #region nested class
        public class DatiPagamento
        {
            public DatiPagamento()
            { }

            public DatiPagamento(string iban, System.Nullable<System.DateTime> decorrenzaPagamento, System.Nullable<char> modalitaPagamento,
                string ufficioPagatore, System.Nullable<int> abi, System.Nullable<int> cab, System.Nullable<int> frazionario, string bic, string libretto, System.Nullable<byte> ultimoMesePagamento,
                System.Nullable<decimal> importoPensioneAltroEnte, System.Nullable<decimal> quotaFissa, System.Nullable<decimal> percentuale, System.Nullable<decimal> quotaConcorsoAltroEnte,
                System.Nullable<char> tipoPagamento, string statoEstero, string nomeUfficioPagatore, string agenziaUfficioPagatore, string capUfficioPagatore, string cittaUfficioPagatore,
                string indirizzoUfficioPagatore, string codCatastaleEstero, bool isFromWebDom)
            {
                this._IBAN = !string.IsNullOrEmpty(iban) ? iban.ToUpperInvariant() : iban;

                this._DecorrenzaPagamento = decorrenzaPagamento;

                this._ModalitaPagamento = modalitaPagamento;

                this._UfficioPagatore = !string.IsNullOrEmpty(ufficioPagatore) ? ufficioPagatore.ToUpperInvariant() : ufficioPagatore;

                this._ABI = abi;

                this._CAB = cab;

                this._Frazionario = frazionario;

                this._BIC = !string.IsNullOrEmpty(bic) ? bic.ToUpperInvariant() : bic;

                this._Libretto = !string.IsNullOrEmpty(libretto) ? libretto.ToUpperInvariant() : libretto;

                this._UltimoMesePagamento = ultimoMesePagamento;

                this._ImportoPensioneAltroEnte = importoPensioneAltroEnte;

                this._QuotaFissa = quotaFissa;

                this._Percentuale = percentuale;

                this._QuotaConcorsoAltroEnte = quotaConcorsoAltroEnte;

                this._TipoPagamento = tipoPagamento;

                this._StatoEstero = !string.IsNullOrEmpty(statoEstero) ? statoEstero.ToUpperInvariant() : statoEstero;

                this._NomeUfficioPagatore = !string.IsNullOrEmpty(nomeUfficioPagatore) ? nomeUfficioPagatore.ToUpperInvariant() : nomeUfficioPagatore;

                this._AgenziaUfficioPagatore = !string.IsNullOrEmpty(agenziaUfficioPagatore) ? agenziaUfficioPagatore.ToUpperInvariant() : agenziaUfficioPagatore;

                this._CapUfficioPagatore = !string.IsNullOrEmpty(capUfficioPagatore) ? capUfficioPagatore.ToUpperInvariant() : capUfficioPagatore;

                this._CittaUfficioPagatore = !string.IsNullOrEmpty(cittaUfficioPagatore) ? cittaUfficioPagatore.ToUpperInvariant() : cittaUfficioPagatore;

                this._IndirizzoUfficioPagatore = !string.IsNullOrEmpty(indirizzoUfficioPagatore) ? indirizzoUfficioPagatore.ToUpperInvariant() : indirizzoUfficioPagatore;

                this._CodCatastaleEstero = !string.IsNullOrEmpty(codCatastaleEstero) ? codCatastaleEstero.ToUpperInvariant() : codCatastaleEstero;

                this._IsFromWebDom = isFromWebDom;
            }

            #region private properties
            private string _IBAN;

            private System.Nullable<System.DateTime> _DecorrenzaPagamento;

            private System.Nullable<char> _ModalitaPagamento;

            private string _UfficioPagatore;

            private System.Nullable<int> _ABI;

            private System.Nullable<int> _CAB;

            private System.Nullable<int> _Frazionario;

            private string _BIC;

            private string _Libretto;

            private System.Nullable<byte> _UltimoMesePagamento;

            private System.Nullable<decimal> _ImportoPensioneAltroEnte;

            private System.Nullable<decimal> _QuotaFissa;

            private System.Nullable<decimal> _Percentuale;

            private System.Nullable<decimal> _QuotaConcorsoAltroEnte;

            private System.Nullable<char> _TipoPagamento;

            private string _StatoEstero;

            private string _NomeUfficioPagatore;

            private string _AgenziaUfficioPagatore;

            private string _CapUfficioPagatore;

            private string _CittaUfficioPagatore;

            private string _IndirizzoUfficioPagatore;

            private string _CodCatastaleEstero;

            private bool _IsFromWebDom;
            #endregion private properties

            #region public properties
            public string IBAN { get { return _IBAN; } set { _IBAN = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPagamento { get { return _DecorrenzaPagamento; } set { _DecorrenzaPagamento = value; } }

            public System.Nullable<char> ModalitaPagamento { get { return _ModalitaPagamento; } set { _ModalitaPagamento = value; } }

            public string UfficioPagatore { get { return _UfficioPagatore; } set { _UfficioPagatore = value; } }

            public System.Nullable<int> ABI { get { return _ABI; } set { _ABI = value; } }

            public System.Nullable<int> CAB { get { return _CAB; } set { _CAB = value; } }

            public System.Nullable<int> Frazionario { get { return _Frazionario; } set { _Frazionario = value; } }

            public string BIC { get { return _BIC; } set { _BIC = value; } }

            public string Libretto { get { return _Libretto; } set { _Libretto = value; } }

            public System.Nullable<byte> UltimoMesePagamento { get { return _UltimoMesePagamento; } set { _UltimoMesePagamento = value; } }

            public System.Nullable<decimal> ImportoPensioneAltroEnte { get { return _ImportoPensioneAltroEnte; } set { _ImportoPensioneAltroEnte = value; } }

            public System.Nullable<decimal> QuotaFissa { get { return _QuotaFissa; } set { _QuotaFissa = value; } }

            public System.Nullable<decimal> Percentuale { get { return _Percentuale; } set { _Percentuale = value; } }

            public System.Nullable<decimal> QuotaConcorsoAltroEnte { get { return _QuotaConcorsoAltroEnte; } set { _QuotaConcorsoAltroEnte = value; } }

            public System.Nullable<char> TipoPagamento { get { return _TipoPagamento; } set { _TipoPagamento = value; } }

            public string StatoEstero { get { return _StatoEstero; } set { _StatoEstero = value; } }

            public string NomeUfficioPagatore { get { return _NomeUfficioPagatore; } set { _NomeUfficioPagatore = value; } }

            public string AgenziaUfficioPagatore { get { return _AgenziaUfficioPagatore; } set { _AgenziaUfficioPagatore = value; } }

            public string CapUfficioPagatore { get { return _CapUfficioPagatore; } set { _CapUfficioPagatore = value; } }

            public string CittaUfficioPagatore { get { return _CittaUfficioPagatore; } set { _CittaUfficioPagatore = value; } }

            public string IndirizzoUfficioPagatore { get { return _IndirizzoUfficioPagatore; } set { _IndirizzoUfficioPagatore = value; } }

            public string CodCatastaleEstero { get { return _CodCatastaleEstero; } set { _CodCatastaleEstero = value; } }

            public bool IsFromWebDom { get { return _IsFromWebDom; } set { _IsFromWebDom = value; } }
            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiPagamento pagamento = (DatiPagamento)obj;
                try
                {
                    if (this._DecorrenzaPagamento != pagamento._DecorrenzaPagamento ||
                        this._ModalitaPagamento != pagamento._ModalitaPagamento ||
                        this._UfficioPagatore != pagamento._UfficioPagatore ||
                        this._ABI != pagamento._ABI ||
                        this._CAB != pagamento._CAB ||
                        this._Frazionario != pagamento._Frazionario ||
                        this._BIC != pagamento._BIC ||
                        this._Libretto != pagamento._Libretto ||
                        this._UltimoMesePagamento != pagamento._UltimoMesePagamento ||
                        this._ImportoPensioneAltroEnte != pagamento._ImportoPensioneAltroEnte ||
                        this._QuotaFissa != pagamento._QuotaFissa ||
                        this._Percentuale != pagamento._Percentuale ||
                        this._QuotaConcorsoAltroEnte != pagamento._QuotaConcorsoAltroEnte ||
                        this._TipoPagamento != pagamento._TipoPagamento ||
                        this._StatoEstero != pagamento._StatoEstero ||
                        this._NomeUfficioPagatore != pagamento._NomeUfficioPagatore ||
                        this._AgenziaUfficioPagatore != pagamento._AgenziaUfficioPagatore ||
                        this._CapUfficioPagatore != pagamento._CapUfficioPagatore ||
                        this._CittaUfficioPagatore != pagamento._CittaUfficioPagatore ||
                        this._IndirizzoUfficioPagatore != pagamento._IndirizzoUfficioPagatore ||
                        this._CodCatastaleEstero != pagamento._CodCatastaleEstero ||
                        this._IsFromWebDom != pagamento._IsFromWebDom)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._DecorrenzaPagamento != null ? this._DecorrenzaPagamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ModalitaPagamento != null ? this._ModalitaPagamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._UfficioPagatore != null ? this._UfficioPagatore.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ABI != null ? this._ABI.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CAB != null ? this._CAB.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Frazionario != null ? this._Frazionario.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._BIC != null ? this._BIC.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Libretto != null ? this._Libretto.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._UltimoMesePagamento != null ? this._UltimoMesePagamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoPensioneAltroEnte != null ? this._ImportoPensioneAltroEnte.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaFissa != null ? this._QuotaFissa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Percentuale != null ? this._Percentuale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaConcorsoAltroEnte != null ? this._QuotaConcorsoAltroEnte.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._TipoPagamento != null ? this._TipoPagamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._StatoEstero != null ? this._StatoEstero.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NomeUfficioPagatore != null ? this._NomeUfficioPagatore.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AgenziaUfficioPagatore != null ? this._AgenziaUfficioPagatore.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CapUfficioPagatore != null ? this._CapUfficioPagatore.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CittaUfficioPagatore != null ? this._CittaUfficioPagatore.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._IndirizzoUfficioPagatore != null ? this._IndirizzoUfficioPagatore.GetHashCode() : 0);
            //    return hash;
            //}
        }

        public class DatiCassaSede
        {
            #region public properties

            //public byte Id { get { return _Id; } set { _Id = value; } }
            //public int Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public int Abi { get { return _Abi; } set { _Abi = value; } }
            public int Cab { get { return _Cab; } set { _Cab = value; } }
            public string Agenzia { get { return _Agenzia; } set { _Agenzia = value; } }

            #endregion public properties

            #region private properties
            //private byte _Id;
            //private int _Descrizione;
            private int _Abi;
            private int _Cab;
            private string _Agenzia;

            #endregion private properties
        }

        public class DatiStatoEstero
        {
            #region public properties
            public string NomeStato { get { return _NomeStato; } set { _NomeStato = value; } }

            public string ABI { get { return _ABI; } set { _ABI = value; } }

            public string CAB { get { return _CAB; } set { _CAB = value; } }

            public string CodCatastale { get { return _CodCatastale; } set { _CodCatastale = value; } }
            #endregion public properties

            #region private properties
            private string _NomeStato;

            private string _ABI;

            private string _CAB;

            private string _CodCatastale;
            #endregion private properties
        }

        #endregion nested class

    }
}
