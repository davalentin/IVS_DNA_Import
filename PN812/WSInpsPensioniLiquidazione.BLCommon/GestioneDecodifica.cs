using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDecodifica
    {
        public static void GetStatiPensione(out List<GestioneDecodifica.StatoPensione> elencoStatiPensione)
        {
            elencoStatiPensione = null;
            List<DecodificaStatoPensione> elencoStatiPensioneDB = null;
            DAGestioneDecodifica.GetStatiPensione(out elencoStatiPensioneDB);
            if (elencoStatiPensioneDB != null && elencoStatiPensioneDB.Count > 0)
            {
                elencoStatiPensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoPensione>();
                foreach (DecodificaStatoPensione statoDB in elencoStatiPensioneDB)
                {
                    elencoStatiPensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoPensione(statoDB));
                }
            }
        }

        public static void GetFondiPensione(out List<GestioneDecodifica.FondoPensione> elencoFondiPensione)
        {
            elencoFondiPensione = null;
            List<DecGestioneFondo> elencoFondiPensioneDB = null;
            DAGestioneDecodifica.GetFondiPensione(out elencoFondiPensioneDB);
            if (elencoFondiPensioneDB != null && elencoFondiPensioneDB.Count > 0)
            {
                elencoFondiPensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.FondoPensione>();
                foreach (DecGestioneFondo fondoDB in elencoFondiPensioneDB)
                {
                    elencoFondiPensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.FondoPensione(fondoDB));
                }
            }
        }

        public static void GetCasseGDP(out List<GestioneDecodifica.FondoPensione> elencoFondiPensione)
        {
            elencoFondiPensione = null;
            List<DecGestioneFondo> elencoFondiPensioneDB = null;
            DAGestioneDecodifica.GetCasseGDP(out elencoFondiPensioneDB);
            if (elencoFondiPensioneDB != null && elencoFondiPensioneDB.Count > 0)
            {
                elencoFondiPensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.FondoPensione>();
                foreach (DecGestioneFondo fondoDB in elencoFondiPensioneDB)
                {
                    elencoFondiPensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.FondoPensione(fondoDB));
                }
            }
        }

        public static void GetParentelaDC(out List<GestioneDecodifica.ParentelaDC> elencoparentela)
        {
            elencoparentela = null;
            List<DecodificaParentela> elencoParentelaDB = null;
            DAGestioneDecodifica.GetParentela(out elencoParentelaDB);
            if (elencoParentelaDB != null && elencoParentelaDB.Count > 0)
            {
                elencoparentela = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ParentelaDC>();
                foreach (DecodificaParentela parentelaDB in elencoParentelaDB)
                {
                    elencoparentela.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ParentelaDC(parentelaDB));
                }
            }
        }

        public static void GetMaggiorazione781ContributiDC(out List<GestioneDecodifica.Maggiorazione781> elencomaggcontr781)
        {
            elencomaggcontr781 = null;
            List<DecodificaMaggiorazione781ContributiDC> elencoelencomaggcontr781DB = null;
            DAGestioneDecodifica.GetDecodificaMaggiorazione781ContributiDC(out elencoelencomaggcontr781DB);
            if (elencoelencomaggcontr781DB != null && elencoelencomaggcontr781DB.Count > 0)
            {
                elencomaggcontr781 = new List<GestioneDecodifica.Maggiorazione781>();
                foreach (DecodificaMaggiorazione781ContributiDC parentelaDB in elencoelencomaggcontr781DB)
                    elencomaggcontr781.Add(new GestioneDecodifica.Maggiorazione781(parentelaDB));
            }
        }

        public static void GetCodiceProvenienza(out List<GestioneDecodifica.CodiceProvenienza> elencodiceprovenienza)
        {
            elencodiceprovenienza = null;
            List<DecodificaCodiceProvenienza> elencoCodiceProvenienzaDB = null;
            DAGestioneDecodifica.GetCodiceProvenienza(out elencoCodiceProvenienzaDB);
            if (elencoCodiceProvenienzaDB != null && elencoCodiceProvenienzaDB.Count > 0)
            {
                elencodiceprovenienza = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceProvenienza>();
                foreach (DecodificaCodiceProvenienza provenienzaDB in elencoCodiceProvenienzaDB)
                {
                    elencodiceprovenienza.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceProvenienza(provenienzaDB));
                }
            }
        }

        public static void GetCodiciVariDC(out List<GestioneDecodifica.CodiciVari> elencoCodiciVariDC)
        {
            elencoCodiciVariDC = null;
            List<DecodificaCodiciVariDC> elencoCodiciVariDCDB = null;
            DAGestioneDecodifica.GetCodiciVariDC(out elencoCodiciVariDCDB);
            if (elencoCodiciVariDCDB != null && elencoCodiciVariDCDB.Count > 0)
            {
                elencoCodiciVariDC = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciVari>();
                foreach (DecodificaCodiciVariDC codicivariDB in elencoCodiciVariDCDB)
                {
                    elencoCodiciVariDC.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciVari(codicivariDB));
                }
            }
        }

        public static void GetCategoriePensione(out List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione)
        {
            elencoCategoriePensione = null;
            List<DecCatPensione> elencoCategoriePensioneDB = null;
            DAGestioneDecodifica.GetCategoriePensione(out elencoCategoriePensioneDB);
            if (elencoCategoriePensioneDB != null && elencoCategoriePensioneDB.Count > 0)
            {
                elencoCategoriePensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CategoriaPensione>();
                foreach (DecCatPensione categoriaDB in elencoCategoriePensioneDB)
                {
                    elencoCategoriePensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CategoriaPensione(categoriaDB));
                }
            }
        }

        public static void GetCodCategoriaBySiglaCategoria(string siglaCategoria, out string categoriaNum)
        {
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, siglaCategoria);
            DAGestioneDecodifica.GetCodCategoriaBySiglaCategoria(siglaCategoria, tipoFondo.HasValue ? tipoFondo.ToString() : string.Empty, out categoriaNum);
        }

        /// <summary>
        /// Tale metodo va utilizzato solo per il flusso AGO e per il flusso CI. Restituisce la sigla categoria a fronte del codice numerico
        /// E' utilizzato solo nel caso di ricostituzioni di reversibilità dirette
        /// </summary>
        /// <param name="categoriaNum"></param>
        /// <param name="siglaCategoria"></param>
        public static void AGO_CI_GetCategoriaByCategoriaNumerica(string categoriaNum, out string siglaCategoria)
        {
            DAGestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(categoriaNum, out siglaCategoria);
        }

        public static void FS_GetFondoByCategoriaNumerica(string categoriaNum, int certificato, out Utility.TipoFondo? tipoFondo)
        {
            tipoFondo = null;
            string strTipoFondo = string.Empty;

            // Gestione particolare per i fondi FS e PT
            // I certificati sono stati recuperati dalla tabella DecGestioneFondoCatPens di Richieste2003
            if (!string.IsNullOrEmpty(categoriaNum))
            {
                int catNum = 0;
                int.TryParse(categoriaNum, out catNum);

                if (catNum == 24)
                {
                    if (certificato < 2099999 || certificato > 2700000)
                        tipoFondo = Utility.TipoFondo.FS;
                    else
                        tipoFondo = Utility.TipoFondo.PT;
                }
            }

            if (!tipoFondo.HasValue)
            {
                DAGestioneDecodifica.FS_GetFondoByCategoriaNumerica(categoriaNum, out strTipoFondo);
                if (!string.IsNullOrEmpty(strTipoFondo))
                    tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, strTipoFondo);
            }
        }

        public static void GetPatronatoByEnteUfficio(string codiceEnte, string codiceUfficio, out GestioneDecodifica.Patronato patronato)
        {
            patronato = null;
            DecPatronato patronatoDB = null;
            DAGestioneDecodifica.GetPatronatoByEnteUfficio(codiceEnte, codiceUfficio, out patronatoDB);
            if (patronatoDB != null)
                patronato = new GestioneDecodifica.Patronato(patronatoDB);
        }

        public static void GetStatiCivili(out List<GestioneDecodifica.StatoCivile> elencoStatiCivili)
        {
            elencoStatiCivili = null;
            List<DecodificaStatoCivile> elencoStatiCiviliDB = null;
            DAGestioneDecodifica.GetStatiCivili(out elencoStatiCiviliDB);
            if (elencoStatiCiviliDB != null && elencoStatiCiviliDB.Count > 0)
            {
                elencoStatiCivili = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoCivile>();
                foreach (DecodificaStatoCivile statoCivileDB in elencoStatiCiviliDB)
                {
                    elencoStatiCivili.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoCivile(statoCivileDB));
                }
            }
        }

        public static void GetStatoPensioneById(byte statoPensione, out string decodificaStatoPensione)
        {
            DAGestioneDecodifica.GetStatoPensioneById(statoPensione, out decodificaStatoPensione);
        }

        public static void GetStatiEsteri(out List<GestioneDecodifica.StatoEstero> elencoStatiEsteri)
        {
            elencoStatiEsteri = null;
            List<DecComuneNazione> elencoStatiEsteriDB = null;
            DAGestioneDecodifica.GetStatiEsteri(out elencoStatiEsteriDB);
            if (elencoStatiEsteriDB != null && elencoStatiEsteriDB.Count > 0)
            {
                elencoStatiEsteri = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoEstero>();
                foreach (DecComuneNazione statoEsteroDB in elencoStatiEsteriDB)
                {
                    elencoStatiEsteri.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoEstero(statoEsteroDB));
                }
            }
        }

        public static int CheckPeriodiAppartenenzaUE(string codice, DateTime? decorrenzaPensione)
        {
            int returnValue = 0;
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                var result = db.CheckPeriodiAppartenenzaUE(codice, decorrenzaPensione);
                if (result != null && result.ReturnValue != null)
                    returnValue = (int)result.ReturnValue;
                db.Connection.Close();
            }
            return returnValue;
        }

        public static void GetStatoEsteroPerCodiceCatastale(string codCatastale, out GestioneDecodifica.StatoEstero statoEstero)
        {
            statoEstero = null;
            DecComuneNazione statoEsteroDB = null;
            DAGestioneDecodifica.GetStatoEsteroByCodCatastale(codCatastale, out statoEsteroDB);
            if (statoEsteroDB != null)
                statoEstero = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.StatoEstero(statoEsteroDB);
        }

        public static void GetCodiceCatastalePerComune_Provincia(string comune, string provincia, out string codCatastale)
        {
            codCatastale = string.Empty;
            try
            {
                DAGestioneDecodifica.GetCodCatastaleByComune_Provincia(comune, provincia, out codCatastale);
            }
            catch (InvalidOperationException ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                throw new INPS.DNA.DnaValidationException(string.Format("Comune/Stato non univoco per i seguenti parametri di input:  comune – {0}, provincia - {1}.", comune, provincia));
            }
        }

        public static void GetCodiceCatastalePerCap(string cap, out string codCatastale)
        {
            codCatastale = string.Empty;
            try
            {
                DAGestioneDecodifica.GetCodCatastaleByCap(cap, out codCatastale);
            }
            catch (InvalidOperationException ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
                throw new INPS.DNA.DnaValidationException(string.Format("Comune/Stato non univoco per il seguente parametro di input:  cap – {0}.", cap));
            }
        }

        public static void GetProvince(out List<GestioneDecodifica.Provincia> elencoProvince)
        {
            elencoProvince = null;
            List<DecProvincia> elencoProvinceDB = null;
            DAGestioneDecodifica.GetProvince(out elencoProvinceDB);
            if (elencoProvinceDB != null && elencoProvinceDB.Count > 0)
            {
                elencoProvince = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Provincia>();
                foreach (DecProvincia provinciaDB in elencoProvinceDB)
                {
                    elencoProvince.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Provincia(provinciaDB));
                }
            }
        }

        public static void GetComuniPerProvincia(string siglaProvincia, out List<GestioneDecodifica.Comune> elencoComuni)
        {
            elencoComuni = null;
            List<DecComuneNazione> elencoComuniDB = null;
            DAGestioneDecodifica.GetComuniPerProvincia(siglaProvincia, out elencoComuniDB);
            if (elencoComuniDB != null && elencoComuniDB.Count > 0)
            {
                elencoComuni = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Comune>();
                foreach (DecComuneNazione comuneDB in elencoComuniDB)
                {
                    elencoComuni.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Comune(comuneDB));
                }
            }
        }

        public static void GetConiugeOFiglio(out List<GestioneDecodifica.ConiugeOFiglio> elencoConiugeOFiglio)
        {
            elencoConiugeOFiglio = null;
            List<DecodificaConiugeOFiglio> elencoConiugeOFiglioDB = null;
            DAGestioneDecodifica.GetConiugeOFiglio(out elencoConiugeOFiglioDB);
            if (elencoConiugeOFiglioDB != null && elencoConiugeOFiglioDB.Count > 0)
            {
                elencoConiugeOFiglio = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ConiugeOFiglio>();
                foreach (DecodificaConiugeOFiglio coniugeOFiglioDB in elencoConiugeOFiglioDB)
                {
                    elencoConiugeOFiglio.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ConiugeOFiglio(coniugeOFiglioDB));
                }
            }
        }

        public static void GetDetrazioniReddito(out List<GestioneDecodifica.DetrazioniReddito> elencoDetrazioniReddito)
        {
            elencoDetrazioniReddito = null;
            List<DecodificaDetrazioniReddito> elencoDetrazioniRedditoDB = null;
            DAGestioneDecodifica.GetDetrazioniReddito(out elencoDetrazioniRedditoDB);
            if (elencoDetrazioniRedditoDB != null && elencoDetrazioniRedditoDB.Count > 0)
            {
                elencoDetrazioniReddito = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DetrazioniReddito>();
                foreach (DecodificaDetrazioniReddito detrazioniRedditoDB in elencoDetrazioniRedditoDB)
                {
                    elencoDetrazioniReddito.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DetrazioniReddito(detrazioniRedditoDB));
                }
            }
        }

        public static void GetTutore(out List<GestioneDecodifica.Tutore> elencoTutore)
        {
            elencoTutore = null;
            List<DecodificaTutore> elencoTutoreDB = null;
            DAGestioneDecodifica.GetTutore(out elencoTutoreDB);
            if (elencoTutoreDB != null && elencoTutoreDB.Count > 0)
            {
                elencoTutore = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Tutore>();
                foreach (DecodificaTutore tutoreDB in elencoTutoreDB)
                {
                    elencoTutore.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Tutore(tutoreDB));
                }
            }
        }

        public static void GetDelegato(out List<GestioneDecodifica.Delegato> elencoDelegato)
        {
            elencoDelegato = null;
            List<DecodificaCodiceDelegato> elencoDelegatoDB = null;
            DAGestioneDecodifica.GetDelegato(out elencoDelegatoDB);
            if (elencoDelegatoDB != null && elencoDelegatoDB.Count > 0)
            {
                elencoDelegato = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Delegato>();
                foreach (DecodificaCodiceDelegato delegatoDB in elencoDelegatoDB)
                {
                    elencoDelegato.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Delegato(delegatoDB));
                }
            }
        }

        public static void GetSiglaFamiliareByTipologia(string tipologia, out List<GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliare)
        {
            elencoSiglaFamiliare = null;
            List<DecodificaSiglaFamiliare> elencoSiglaFamiliareDB = null;
            DAGestioneDecodifica.GetSiglaFamiliare(tipologia, out elencoSiglaFamiliareDB);
            if (elencoSiglaFamiliareDB != null && elencoSiglaFamiliareDB.Count > 0)
            {
                elencoSiglaFamiliare = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SiglaFamiliare>();
                foreach (DecodificaSiglaFamiliare siglaFamiliareDB in elencoSiglaFamiliareDB)
                {
                    elencoSiglaFamiliare.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SiglaFamiliare(siglaFamiliareDB));
                }
            }
        }

        public static void GetFamiliare(out List<GestioneDecodifica.Familiare> elencoFamiliare)
        {
            elencoFamiliare = null;
            List<DecodificaFamiliare> elencoFamiliareDB = null;
            DAGestioneDecodifica.GetFamiliare(out elencoFamiliareDB);
            if (elencoFamiliareDB != null && elencoFamiliareDB.Count > 0)
            {
                elencoFamiliare = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Familiare>();
                foreach (DecodificaFamiliare familiareDB in elencoFamiliareDB)
                {
                    elencoFamiliare.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Familiare(familiareDB));
                }
            }
        }

        public static void GetValidazioneCF(out List<GestioneDecodifica.ValidazioneCF> elencoValidazioneCF)
        {
            elencoValidazioneCF = null;
            List<DecodificaValidazioneCF> elencoValidazioneCFDB = null;
            DAGestioneDecodifica.GetValidazioneCF(out elencoValidazioneCFDB);
            if (elencoValidazioneCFDB != null && elencoValidazioneCFDB.Count > 0)
            {
                elencoValidazioneCF = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ValidazioneCF>();
                foreach (DecodificaValidazioneCF validazioneCFDB in elencoValidazioneCFDB)
                {
                    elencoValidazioneCF.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ValidazioneCF(validazioneCFDB));
                }
            }
        }

        public static void GetModalitaPagamento(out List<GestioneDecodifica.ModalitaPagamento> elencoModalitaPagamento)
        {
            elencoModalitaPagamento = null;
            List<DecodificaModalitaPagamento> elencoModalitaPagamentoDB = null;
            DAGestioneDecodifica.GetModalitaPagamento(out elencoModalitaPagamentoDB);
            if (elencoModalitaPagamentoDB != null && elencoModalitaPagamentoDB.Count > 0)
            {
                elencoModalitaPagamento = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ModalitaPagamento>();
                foreach (DecodificaModalitaPagamento modalitaPagamentoDB in elencoModalitaPagamentoDB)
                {
                    elencoModalitaPagamento.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ModalitaPagamento(modalitaPagamentoDB));
                }
            }
        }

        public static void GetTipoPagamento(out List<GestioneDecodifica.TipoPagamento> elencoTipoPagamento)
        {
            elencoTipoPagamento = null;
            List<DecodificaTipoPagamento> elencoTipoPagamentoDB = null;
            DAGestioneDecodifica.GetTipoPagamento(out elencoTipoPagamentoDB);
            if (elencoTipoPagamentoDB != null && elencoTipoPagamentoDB.Count > 0)
            {
                elencoTipoPagamento = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoPagamento>();
                foreach (DecodificaTipoPagamento tipoPagamentoDB in elencoTipoPagamentoDB)
                {
                    elencoTipoPagamento.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoPagamento(tipoPagamentoDB));
                }
            }
        }

        public static void GetTipoCalcolo(out List<GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo)
        {
            elencoTipoCalcolo = null;
            List<DecodificaTipoCalcolo> elencoTipoCalcoloDB = null;
            DAGestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcoloDB);
            if (elencoTipoCalcoloDB != null && elencoTipoCalcoloDB.Count > 0)
            {
                elencoTipoCalcolo = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoCalcolo>();
                foreach (DecodificaTipoCalcolo tipoCalcoloDB in elencoTipoCalcoloDB)
                {
                    elencoTipoCalcolo.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoCalcolo(tipoCalcoloDB));
                }
            }
        }

        public static void GetTipoCalcoloSecondario(out List<GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondario)
        {
            elencoTipoCalcoloSecondario = null;
            List<DecodificaTipoCalcoloSecondario> elencoTipoCalcoloSecondarioDB = null;
            DAGestioneDecodifica.GetTipoCalcoloSecondario(out elencoTipoCalcoloSecondarioDB);
            if (elencoTipoCalcoloSecondarioDB != null && elencoTipoCalcoloSecondarioDB.Count > 0)
            {
                elencoTipoCalcoloSecondario = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoCalcoloSecondario>();
                foreach (DecodificaTipoCalcoloSecondario TipoCalcoloSecondarioDB in elencoTipoCalcoloSecondarioDB)
                {
                    elencoTipoCalcoloSecondario.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoCalcoloSecondario(TipoCalcoloSecondarioDB));
                }
            }
        }

        public static void GetCausaCarico(out List<GestioneDecodifica.CausaCarico> elencoCausaCarico)
        {
            elencoCausaCarico = null;
            List<DecodificaCausaCarico> elencoCausaCaricoDB = null;
            DAGestioneDecodifica.GetCausaCarico(out elencoCausaCaricoDB);
            if (elencoCausaCaricoDB != null && elencoCausaCaricoDB.Count > 0)
            {
                elencoCausaCarico = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CausaCarico>();
                foreach (DecodificaCausaCarico causaCaricoDB in elencoCausaCaricoDB)
                {
                    elencoCausaCarico.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CausaCarico(causaCaricoDB));
                }
            }
        }

        public static void GetCodiceEliminazioneByTipologia(out List<GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazione, Utility.TipoAppartenenza? tipoApp)
        {
            elencoCodiceEliminazione = null;
            List<DecodificaCodiceEliminazione> elencoCodiceEliminazioneDB = null;
            DAGestioneDecodifica.GetCodiceEliminazioneByTipologia(out elencoCodiceEliminazioneDB, tipoApp.ToString());
            if (elencoCodiceEliminazioneDB != null && elencoCodiceEliminazioneDB.Count > 0)
            {
                elencoCodiceEliminazione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceEliminazione>();
                foreach (DecodificaCodiceEliminazione codiceEliminazioneDB in elencoCodiceEliminazioneDB)
                {
                    elencoCodiceEliminazione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceEliminazione(codiceEliminazioneDB));
                }
            }
        }

        public static void GetAttivitaSvoltaByFondo(string fondo, char? enteFondo, out List<GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolta)
        {
            elencoAttivitaSvolta = null;
            List<DecodificaAttivitaSvolta> elencoAttivitaSvoltaDB = null;
            DAGestioneDecodifica.GetAttivitaSvoltaByFondo(fondo, enteFondo, out elencoAttivitaSvoltaDB);
            if (elencoAttivitaSvoltaDB != null && elencoAttivitaSvoltaDB.Count > 0)
            {
                elencoAttivitaSvolta = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.AttivitaSvolta>();
                foreach (DecodificaAttivitaSvolta attivitaSvoltaDB in elencoAttivitaSvoltaDB)
                {
                    elencoAttivitaSvolta.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.AttivitaSvolta(attivitaSvoltaDB));
                }
            }
        }

        public static void GetAttivitaSvoltaById(string id, out GestioneDecodifica.AttivitaSvolta attivitaSvolta)
        {
            attivitaSvolta = null;
            DecodificaAttivitaSvolta attivitaSvoltaDB = null;
            DAGestioneDecodifica.GetAttivitaSvoltaById(id, out attivitaSvoltaDB);
            if (attivitaSvoltaDB != null)
            {
                attivitaSvolta = new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.AttivitaSvolta(attivitaSvoltaDB);
            }
        }

        public static void GetCodiceCristallizzazione(out List<GestioneDecodifica.CodiceCristallizzazione> elencoCodiceCristallizzazione)
        {
            elencoCodiceCristallizzazione = null;
            List<DecodificaCodiceCristallizzazione> elencoCodiceCristallizzazioneDB = null;
            DAGestioneDecodifica.GetCodiceCristallizzazione(out elencoCodiceCristallizzazioneDB);
            if (elencoCodiceCristallizzazioneDB != null && elencoCodiceCristallizzazioneDB.Count > 0)
            {
                elencoCodiceCristallizzazione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceCristallizzazione>();
                foreach (DecodificaCodiceCristallizzazione codiceCristallizzazioneDB in elencoCodiceCristallizzazioneDB)
                {
                    elencoCodiceCristallizzazione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceCristallizzazione(codiceCristallizzazioneDB));
                }
            }
        }

        public static void GetTipoPensione(out List<GestioneDecodifica.TipoPensione> elencoTipoPensione)
        {
            elencoTipoPensione = null;
            List<DecodificaTipoPensione> elencoTipoPensioneDB = null;
            DAGestioneDecodifica.GetTipoPensione(out elencoTipoPensioneDB);
            if (elencoTipoPensioneDB != null && elencoTipoPensioneDB.Count > 0)
            {
                elencoTipoPensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoPensione>();
                foreach (DecodificaTipoPensione tipoPensioneDB in elencoTipoPensioneDB)
                {
                    elencoTipoPensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoPensione(tipoPensioneDB));
                }
            }
        }

        public static void GetCodiceAzienda(out List<GestioneDecodifica.CodiceAzienda> elencoCodiceAzienda)
        {
            elencoCodiceAzienda = null;
            List<DecodificaCodiceAzienda> elencoCodiceAziendaDB = null;
            DAGestioneDecodifica.GetCodiceAzienda(out elencoCodiceAziendaDB);
            if (elencoCodiceAziendaDB != null && elencoCodiceAziendaDB.Count > 0)
            {
                elencoCodiceAzienda = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceAzienda>();
                foreach (DecodificaCodiceAzienda codiceAziendaDB in elencoCodiceAziendaDB)
                {
                    elencoCodiceAzienda.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceAzienda(codiceAziendaDB));
                }
            }
        }

        public static void GetGradoInvalidita(out List<GestioneDecodifica.GradoInvalidita> elencoGradoInvalidita)
        {
            elencoGradoInvalidita = null;
            List<DecodificaGradoInvalidita> elencoGradoInvaliditaDB = null;
            DAGestioneDecodifica.GetGradoInvalidita(out elencoGradoInvaliditaDB);
            if (elencoGradoInvaliditaDB != null && elencoGradoInvaliditaDB.Count > 0)
            {
                elencoGradoInvalidita = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.GradoInvalidita>();
                foreach (DecodificaGradoInvalidita gradoInvaliditaDB in elencoGradoInvaliditaDB)
                {
                    elencoGradoInvalidita.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.GradoInvalidita(gradoInvaliditaDB));
                }
            }
        }

        public static void GetProrataEnel(out List<GestioneDecodifica.ProrataEnel> elencoProrataEnel)
        {
            elencoProrataEnel = null;
            List<DecodificaProrataEnel> elencoProrataEnelDB = null;
            DAGestioneDecodifica.GetProrataEnel(out elencoProrataEnelDB);
            if (elencoProrataEnelDB != null && elencoProrataEnelDB.Count > 0)
            {
                elencoProrataEnel = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ProrataEnel>();
                foreach (DecodificaProrataEnel prorataEnelDB in elencoProrataEnelDB)
                {
                    elencoProrataEnel.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ProrataEnel(prorataEnelDB));
                }
            }
        }

        public static void GetGestioneFondoInChiaro(string gestione, string fondo, out string descGestione, out string descFondo)
        {
            descGestione = "";
            descFondo = "";
            DAGestioneDecodifica.GetGestioneFondoInChiaroByKey(gestione, fondo, out descGestione, out descFondo);
        }

        public static void GetProdottoInChiaro(string prodotto, out string descProdotto)
        {
            descProdotto = "";
            DAGestioneDecodifica.GetProdottoInChiaroByKey(prodotto, out descProdotto);
        }

        public static void GetTipologiaInChiaro(string tipo, out string descTipologia)
        {
            descTipologia = "";
            DAGestioneDecodifica.GetTipologiaInChiaroByKey(tipo, out descTipologia);
        }

        public static void GetEnteInChiaro(string ente, out string descEnte)
        {
            descEnte = "";
            DAGestioneDecodifica.GetEnteInChiaroByKey(ente, out descEnte);
        }

        public static void GetFiltroInChiaro(string codeTipoRichiesta, out string descFiltro)
        {
            descFiltro = string.Empty;

            if (!string.IsNullOrEmpty(codeTipoRichiesta))
            {
                GestioneDecodifica.GestioneCodiceTipoRichiesta gestioneCodeTipoRichiesta = null;
                GetGestioneTipoRichiestaByCodTipoRichiesta(codeTipoRichiesta, out gestioneCodeTipoRichiesta);
                if (gestioneCodeTipoRichiesta != null)
                    descFiltro = gestioneCodeTipoRichiesta.Filtro;
            }
        }

        public static void GetComunicazioneCampi1_2(out List<GestioneDecodifica.ComunicazioneCampi1_2> elencoComunicazioneCampo12)
        {
            elencoComunicazioneCampo12 = null;
            List<DecodificaComunicazioneCampo12> elencoComunicazioneCampo12DB = null;
            DAGestioneDecodifica.GetComunicazioneCampi1_2(out elencoComunicazioneCampo12DB);
            if (elencoComunicazioneCampo12DB != null && elencoComunicazioneCampo12DB.Count > 0)
            {
                elencoComunicazioneCampo12 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampi1_2>();
                foreach (DecodificaComunicazioneCampo12 comunicazioneCampo12DB in elencoComunicazioneCampo12DB)
                {
                    elencoComunicazioneCampo12.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampi1_2(comunicazioneCampo12DB));
                }
            }
        }

        public static void GetComunicazioneCampo3(out List<GestioneDecodifica.ComunicazioneCampo3> elencoComunicazioneCampo3)
        {
            elencoComunicazioneCampo3 = null;
            List<DecodificaComunicazioneCampo3> elencoComunicazioneCampo3DB = null;
            DAGestioneDecodifica.GetComunicazioneCampo3(out elencoComunicazioneCampo3DB);
            if (elencoComunicazioneCampo3DB != null && elencoComunicazioneCampo3DB.Count > 0)
            {
                elencoComunicazioneCampo3 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampo3>();
                foreach (DecodificaComunicazioneCampo3 comunicazioneCampo3DB in elencoComunicazioneCampo3DB)
                {
                    elencoComunicazioneCampo3.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampo3(comunicazioneCampo3DB));
                }
            }
        }

        public static void GetComunicazioneCampo4(out List<GestioneDecodifica.ComunicazioneCampo4> elencoComunicazioneCampo4)
        {
            elencoComunicazioneCampo4 = null;
            List<DecodificaComunicazioneCampo4> elencoComunicazioneCampo4DB = null;
            DAGestioneDecodifica.GetComunicazioneCampo4(out elencoComunicazioneCampo4DB);
            if (elencoComunicazioneCampo4DB != null && elencoComunicazioneCampo4DB.Count > 0)
            {
                elencoComunicazioneCampo4 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampo4>();
                foreach (DecodificaComunicazioneCampo4 comunicazioneCampo4DB in elencoComunicazioneCampo4DB)
                {
                    elencoComunicazioneCampo4.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ComunicazioneCampo4(comunicazioneCampo4DB));
                }
            }
        }

        public static void GetCodiciNatura(out List<GestioneDecodifica.CodiciNatura> elencoCodiciNatura)
        {
            elencoCodiciNatura = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaFS = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaDB = null;
            DAGestioneDecodifica.GetCodiciNatura(out elencoCodiciNaturaDB);
            elencoCodiciNaturaFS = elencoCodiciNaturaDB.FindAll(x => x.Tipologia == Utility.TipoAppartenenza.FS.ToString());
            if (elencoCodiciNaturaFS != null && elencoCodiciNaturaFS.Count > 0)
            {
                elencoCodiciNatura = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura>();
                foreach (DecodificaCodiciNatura codiciNaturaDB in elencoCodiciNaturaFS)
                {
                    elencoCodiciNatura.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura(codiciNaturaDB));
                }
            }
        }

        public static void GetCodiciNatura_AGO_CI(out List<GestioneDecodifica.CodiciNatura> elencoCodiciNatura)
        {
            elencoCodiciNatura = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaAGO = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaDB = null;
            DAGestioneDecodifica.GetCodiciNatura(out elencoCodiciNaturaDB);
            elencoCodiciNaturaAGO = new List<DecodificaCodiciNatura>();
            elencoCodiciNaturaAGO = elencoCodiciNaturaDB.FindAll(x => x.Tipologia == "AGO-CI");
            if (elencoCodiciNaturaAGO != null && elencoCodiciNaturaAGO.Count > 0)
            {
                elencoCodiciNatura = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura>();
                foreach (DecodificaCodiciNatura codiciNaturaDB in elencoCodiciNaturaAGO)
                {
                    elencoCodiciNatura.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura(codiciNaturaDB));
                }
            }
        }

        public static void GetCodiciNaturaCI(out List<GestioneDecodifica.CodiciNatura> elencoCodiciNatura)
        {
            elencoCodiciNatura = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaCI = null;
            List<DecodificaCodiciNatura> elencoCodiciNaturaDB = null;
            DAGestioneDecodifica.GetCodiciNatura(out elencoCodiciNaturaDB);
            elencoCodiciNaturaCI = new List<DecodificaCodiciNatura>();
            elencoCodiciNaturaCI = elencoCodiciNaturaDB.FindAll(x => x.Tipologia == Utility.TipoAppartenenza.CI.ToString());
            if (elencoCodiciNaturaCI != null && elencoCodiciNaturaCI.Count > 0)
            {
                elencoCodiciNatura = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura>();
                foreach (DecodificaCodiciNatura codiciNaturaDB in elencoCodiciNaturaCI)
                {
                    elencoCodiciNatura.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiciNatura(codiciNaturaDB));
                }
            }
        }

        public static void GetCodiceCieco(out List<GestioneDecodifica.Cieco> elencoCodiceCieco)
        {
            elencoCodiceCieco = null;
            List<DecodificaCodiceCieco> elencoCodiceCiecoDB = null;
            DAGestioneDecodifica.GetCodiceCieco(out elencoCodiceCiecoDB);
            if (elencoCodiceCiecoDB != null && elencoCodiceCiecoDB.Count > 0)
            {
                elencoCodiceCieco = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Cieco>();
                foreach (DecodificaCodiceCieco codiceCiecoDB in elencoCodiceCiecoDB)
                {
                    elencoCodiceCieco.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Cieco(codiceCiecoDB));
                }
            }
        }

        public static void GetTipoSettimaneBeneficio(out List<GestioneDecodifica.SettimaneBeneficio> elencoSettimaneBeneficio)
        {
            elencoSettimaneBeneficio = null;
            List<DecodificaTipoBeneficio> elencoTipoBeneficioDB = null;
            DAGestioneDecodifica.GetCodiceTipoBeneficio(out elencoTipoBeneficioDB);
            if (elencoTipoBeneficioDB != null && elencoTipoBeneficioDB.Count > 0)
            {
                elencoSettimaneBeneficio = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio>();
                foreach (DecodificaTipoBeneficio tipoBeneficioDB in elencoTipoBeneficioDB)
                {
                    elencoSettimaneBeneficio.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio(tipoBeneficioDB));
                }
            }
        }

        public static void GetTipoSupplementi(out List<GestioneDecodifica.TipoSupplementi> elencoTipoSupplementi)
        {
            elencoTipoSupplementi = null;
            List<DecodificaGestioneSupplementi> elencoGestioneSupplementiDB = null;
            DAGestioneDecodifica.GetTipoSupplementi(out elencoGestioneSupplementiDB);
            if (elencoGestioneSupplementiDB != null && elencoGestioneSupplementiDB.Count > 0)
            {
                elencoTipoSupplementi = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoSupplementi>();
                foreach (DecodificaGestioneSupplementi tipoSupplementoDB in elencoGestioneSupplementiDB)
                {
                    elencoTipoSupplementi.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipoSupplementi(tipoSupplementoDB));
                }
            }
        }

        public static void GetCodiceMobilita(out List<GestioneDecodifica.Mobilita> elencoCodiceMobilita)
        {
            elencoCodiceMobilita = null;
            List<DecodificaCodiceMobilita> elencoCodiceMobilitaDB = null;
            DAGestioneDecodifica.GetCodiceMobilita(out elencoCodiceMobilitaDB);
            if (elencoCodiceMobilitaDB != null && elencoCodiceMobilitaDB.Count > 0)
            {
                elencoCodiceMobilita = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Mobilita>();
                foreach (DecodificaCodiceMobilita codiceMobilitaDB in elencoCodiceMobilitaDB)
                {
                    elencoCodiceMobilita.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.Mobilita(codiceMobilitaDB));
                }
            }
        }

        public static void GetCodiciRequisitiParticolari(out List<GestioneDecodifica.CodiceRequisitoParticolare> elencoCodiciRequisitiParticolari)
        {
            elencoCodiciRequisitiParticolari = null;
            List<DecodificaCodiciRequisitiParticolari> elencoCodiciRequisitiParticolariDB = null;
            DAGestioneDecodifica.GetCodiciRequisitiParticolari(out elencoCodiciRequisitiParticolariDB);
            if (elencoCodiciRequisitiParticolariDB != null && elencoCodiciRequisitiParticolariDB.Count > 0)
            {
                elencoCodiciRequisitiParticolari = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitoParticolare>();
                foreach (DecodificaCodiciRequisitiParticolari codiceRequisitoParticolareDB in elencoCodiciRequisitiParticolariDB)
                {
                    elencoCodiciRequisitiParticolari.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitoParticolare(codiceRequisitoParticolareDB));
                }
            }
        }

        public static void GetCodiceRequisito1(out List<GestioneDecodifica.CodiceRequisito1> elencoCodiceRequisito1)
        {
            elencoCodiceRequisito1 = null;
            List<DecodificaCodiceRequisito1> elencoCodiceRequisito1DB = null;
            DAGestioneDecodifica.GetCodiceRequisito1(out elencoCodiceRequisito1DB);
            if (elencoCodiceRequisito1DB != null && elencoCodiceRequisito1DB.Count > 0)
            {
                elencoCodiceRequisito1 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito1>();
                foreach (DecodificaCodiceRequisito1 codiceRequisito1DB in elencoCodiceRequisito1DB)
                {
                    elencoCodiceRequisito1.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito1(codiceRequisito1DB));
                }
            }
        }

        public static void GetCodiceRequisito2(out List<GestioneDecodifica.CodiceRequisito2> elencoCodiceRequisito2)
        {
            elencoCodiceRequisito2 = null;
            List<DecodificaCodiceRequisito2> elencoCodiceRequisito2DB = null;
            DAGestioneDecodifica.GetCodiceRequisito2(out elencoCodiceRequisito2DB);
            if (elencoCodiceRequisito2DB != null && elencoCodiceRequisito2DB.Count > 0)
            {
                elencoCodiceRequisito2 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito2>();
                foreach (DecodificaCodiceRequisito2 codiceRequisito2DB in elencoCodiceRequisito2DB)
                {
                    elencoCodiceRequisito2.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisito2(codiceRequisito2DB));
                }
            }
        }

        public static void GetCodiceSpecifico(out List<GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico)
        {
            elencoCodiceSpecifico = null;
            List<DecodificaCodiceSpecifico> elencoCodiceSpecificoDB = null;
            DAGestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecificoDB);
            if (elencoCodiceSpecificoDB != null && elencoCodiceSpecificoDB.Count > 0)
            {
                elencoCodiceSpecifico = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceSpecifico>();
                foreach (DecodificaCodiceSpecifico codiceSpecificoDB in elencoCodiceSpecificoDB)
                {
                    elencoCodiceSpecifico.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceSpecifico(codiceSpecificoDB));
                }
            }
        }

        public static void GetCodiceConvenzioneInternazionale(out List<GestioneDecodifica.CodiceConvenzioneInternazionale> elencoCodiceConvenzioneInternazionale)
        {
            elencoCodiceConvenzioneInternazionale = null;
            List<DecodificaCodiceConvenzioneInternazionale> elencoCodiceConvenzioneInternazionaleDB = null;
            DAGestioneDecodifica.GetCodiceConvenzioneInternazionale(out elencoCodiceConvenzioneInternazionaleDB);
            if (elencoCodiceConvenzioneInternazionaleDB != null && elencoCodiceConvenzioneInternazionaleDB.Count > 0)
            {
                elencoCodiceConvenzioneInternazionale = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzioneInternazionale>();
                foreach (DecodificaCodiceConvenzioneInternazionale codiceConvenzioneInternazionaleDB in elencoCodiceConvenzioneInternazionaleDB)
                {
                    elencoCodiceConvenzioneInternazionale.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzioneInternazionale(codiceConvenzioneInternazionaleDB));
                }
            }
        }

        public static void GetCodiceRequisitiLegge50392(out List<GestioneDecodifica.CodiceRequisitiLegge50392> elencoCodiceRequisitiLegge50392)
        {
            elencoCodiceRequisitiLegge50392 = null;
            List<DecodificaCodiceRequisitiLegge50392> elencoCodiceRequisitiLegge50392DB = null;
            DAGestioneDecodifica.GetCodiceRequisitiLegge50392(out elencoCodiceRequisitiLegge50392DB);
            if (elencoCodiceRequisitiLegge50392DB != null && elencoCodiceRequisitiLegge50392DB.Count > 0)
            {
                elencoCodiceRequisitiLegge50392 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitiLegge50392>();
                foreach (DecodificaCodiceRequisitiLegge50392 codiceRequisitiLegge50392DB in elencoCodiceRequisitiLegge50392DB)
                {
                    elencoCodiceRequisitiLegge50392.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceRequisitiLegge50392(codiceRequisitiLegge50392DB));
                }
            }
        }

        public static void GetCodiceConvenzione(out List<GestioneDecodifica.CodiceConvenzione> elencoCodiceConvenzione)
        {
            elencoCodiceConvenzione = null;
            List<DecodificaCodiceConvenzione> elencoCodiceConvenzioneDB = null;
            DAGestioneDecodifica.GetCodiceConvenzione(out elencoCodiceConvenzioneDB);
            if (elencoCodiceConvenzioneDB != null && elencoCodiceConvenzioneDB.Count > 0)
            {
                elencoCodiceConvenzione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzione>();
                foreach (DecodificaCodiceConvenzione codiceConvenzioneDB in elencoCodiceConvenzioneDB)
                {
                    elencoCodiceConvenzione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceConvenzione(codiceConvenzioneDB));
                }
            }
        }

        public static void GetCodiceVirtuale(out List<GestioneDecodifica.CodiceVirtuale> elencoCodiceVirtuale)
        {
            elencoCodiceVirtuale = null;
            List<DecodificaCodiceVirtuale> elencoCodiceVirtualeDB = null;
            DAGestioneDecodifica.GetCodiceVirtuale(out elencoCodiceVirtualeDB);
            if (elencoCodiceVirtualeDB != null && elencoCodiceVirtualeDB.Count > 0)
            {
                elencoCodiceVirtuale = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceVirtuale>();
                foreach (DecodificaCodiceVirtuale codiceVirtualeDB in elencoCodiceVirtualeDB)
                {
                    elencoCodiceVirtuale.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceVirtuale(codiceVirtualeDB));
                }
            }
        }

        public static void GetRegimeLiquidazione(out List<GestioneDecodifica.RegimeLiquidazione> elencoRegimeLiquidazione)
        {
            elencoRegimeLiquidazione = null;
            List<DecodificaRegimeLiquidazione> elencoRegimeLiquidazioneDB = null;
            DAGestioneDecodifica.GetRegimeLiquidazione(out elencoRegimeLiquidazioneDB);
            if (elencoRegimeLiquidazioneDB != null && elencoRegimeLiquidazioneDB.Count > 0)
            {
                elencoRegimeLiquidazione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.RegimeLiquidazione>();
                foreach (DecodificaRegimeLiquidazione regimeLiquidazioneDB in elencoRegimeLiquidazioneDB)
                {
                    elencoRegimeLiquidazione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.RegimeLiquidazione(regimeLiquidazioneDB));
                }
            }
        }

        public static void GetCodiceImportoAltraPensione(out List<GestioneDecodifica.ImportoAltraPensione> elencoImportoAltraPensione)
        {
            elencoImportoAltraPensione = null;
            List<DecodificaCodiceImportoAltraPensione> elencoImportoAltraPensioneDB = null;
            DAGestioneDecodifica.GetCodiceImportoAltraPensione(out elencoImportoAltraPensioneDB);
            if (elencoImportoAltraPensioneDB != null && elencoImportoAltraPensioneDB.Count > 0)
            {
                elencoImportoAltraPensione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ImportoAltraPensione>();
                foreach (DecodificaCodiceImportoAltraPensione importoAltraPensioneDB in elencoImportoAltraPensioneDB)
                {
                    elencoImportoAltraPensione.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ImportoAltraPensione(importoAltraPensioneDB));
                }
            }
        }

        public static void GetCodMaggiorazioneFamiliari(string tipologia, out List<GestioneDecodifica.CodMaggiorazioneFamiliari> elencoCodMaggiorazioneFamiliari)
        {
            elencoCodMaggiorazioneFamiliari = null;
            List<DecodificaCodMaggFamiliari> elencoCodMaggiorazioneFamiliariDB = null;
            DAGestioneDecodifica.GetCodiceMaggiorazioneFamiliari(tipologia, out elencoCodMaggiorazioneFamiliariDB);
            if (elencoCodMaggiorazioneFamiliariDB != null && elencoCodMaggiorazioneFamiliariDB.Count > 0)
            {
                elencoCodMaggiorazioneFamiliari = new List<BLCommon.GestioneDecodifica.CodMaggiorazioneFamiliari>();
                foreach (DecodificaCodMaggFamiliari CodMaggiorazioneFamiliariDB in elencoCodMaggiorazioneFamiliariDB)
                {
                    elencoCodMaggiorazioneFamiliari.Add(new BLCommon.GestioneDecodifica.CodMaggiorazioneFamiliari(CodMaggiorazioneFamiliariDB));
                }
            }
        }

        public static void GetCodeGestioneCalcoloRetributivo(out List<GestioneDecodifica.CodeGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetrib)
        {
            elencoCodeGestioneCalcoloRetrib = null;
            List<DecodificaGestioneCalcoloRetributivo> elencoCodeGestioneCalcoloRetribDB = null;
            DAGestioneDecodifica.GetCodiceGestioneCalcoloRetributivo(out elencoCodeGestioneCalcoloRetribDB);
            if (elencoCodeGestioneCalcoloRetribDB != null && elencoCodeGestioneCalcoloRetribDB.Count > 0)
            {
                elencoCodeGestioneCalcoloRetrib = new List<CodeGestioneCalcoloRetributivo>();
                foreach (DecodificaGestioneCalcoloRetributivo CodeGestioneCalcoloRetribDB in elencoCodeGestioneCalcoloRetribDB)
                    elencoCodeGestioneCalcoloRetrib.Add(new BLCommon.GestioneDecodifica.CodeGestioneCalcoloRetributivo(CodeGestioneCalcoloRetribDB));
            }
        }

        public static void GetCodeGestioneCalcoloContributivo(out List<GestioneDecodifica.CodeGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContrib)
        {
            elencoCodeGestioneCalcoloContrib = null;
            List<DecodificaGestioneCalcoloContributivo> elencoCodeGestioneCalcoloContribDB = null;
            DAGestioneDecodifica.GetCodiceGestioneCalcoloContributivo(out elencoCodeGestioneCalcoloContribDB);
            if (elencoCodeGestioneCalcoloContribDB != null && elencoCodeGestioneCalcoloContribDB.Count > 0)
            {
                elencoCodeGestioneCalcoloContrib = new List<CodeGestioneCalcoloContributivo>();
                foreach (DecodificaGestioneCalcoloContributivo CodeGestioneCalcoloContribDB in elencoCodeGestioneCalcoloContribDB)
                    elencoCodeGestioneCalcoloContrib.Add(new BLCommon.GestioneDecodifica.CodeGestioneCalcoloContributivo(CodeGestioneCalcoloContribDB));
            }
        }

        public static void GetCodeGestioneQuotaFondoIntegrativo(out List<GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo> elencoCodeGestioneQuotaFondoIntegrativo)
        {
            elencoCodeGestioneQuotaFondoIntegrativo = null;
            List<DecodificaGestioneQuotaFondoIntegrativo> elencoCodeGestioneQuotaFondoIntegrativoDB = null;
            DAGestioneDecodifica.GetCodiceGestioneQuotaFondoIntegrativo(out elencoCodeGestioneQuotaFondoIntegrativoDB);
            if (elencoCodeGestioneQuotaFondoIntegrativoDB != null && elencoCodeGestioneQuotaFondoIntegrativoDB.Count > 0)
            {
                elencoCodeGestioneQuotaFondoIntegrativo = new List<CodeGestioneQuotaFondoIntegrativo>();
                foreach (DecodificaGestioneQuotaFondoIntegrativo CodeGestioneQuotaFondoIntegrativoDB in elencoCodeGestioneQuotaFondoIntegrativoDB)
                    elencoCodeGestioneQuotaFondoIntegrativo.Add(new BLCommon.GestioneDecodifica.CodeGestioneQuotaFondoIntegrativo(CodeGestioneQuotaFondoIntegrativoDB));
            }
        }

        public static void GetCodeGestioneQuotaFondoINPGI(out List<GestioneDecodifica.CodeGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGI)
        {
            elencoCodeGestioneQuotaFondoINPGI = null;
            List<DecodificaGestioneQuotaFondoINPGI> elencoCodeGestioneQuotaFondoINPGIDB = null;
            DAGestioneDecodifica.GetCodiceGestioneQuotaFondoINPGI(out elencoCodeGestioneQuotaFondoINPGIDB);
            if (elencoCodeGestioneQuotaFondoINPGIDB != null && elencoCodeGestioneQuotaFondoINPGIDB.Count > 0)
            {
                elencoCodeGestioneQuotaFondoINPGI = new List<CodeGestioneQuotaFondoINPGI>();
                foreach (DecodificaGestioneQuotaFondoINPGI CodeGestioneQuotaFondoINPGIDB in elencoCodeGestioneQuotaFondoINPGIDB)
                    elencoCodeGestioneQuotaFondoINPGI.Add(new BLCommon.GestioneDecodifica.CodeGestioneQuotaFondoINPGI(CodeGestioneQuotaFondoINPGIDB));
            }
        }

        public static void GetErroriCalcoloCi(out List<GestioneDecodifica.ErroreCalcoloCi> elencoErroriCalcoloCi)
        {
            elencoErroriCalcoloCi = null;
            List<ErroriCalcoloCi> elencoErroriCalcoloCiDB = null;
            DAGestioneDecodifica.GetErroriCalcoloCi(out elencoErroriCalcoloCiDB);
            if (elencoErroriCalcoloCiDB != null && elencoErroriCalcoloCiDB.Count > 0)
            {
                elencoErroriCalcoloCi = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ErroreCalcoloCi>();
                foreach (ErroriCalcoloCi erroreCalcoloCiDB in elencoErroriCalcoloCiDB)
                {
                    elencoErroriCalcoloCi.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.ErroreCalcoloCi(erroreCalcoloCiDB));
                }
            }
        }

        public static void GetErroreCalcoloCiByCode(string codice, out GestioneDecodifica.ErroreCalcoloCi erroreCalcoloCi)
        {
            erroreCalcoloCi = null;
            ErroriCalcoloCi erroreCalcoloCiDB = null;
            DAGestioneDecodifica.GetErroreCalcoloCiByCode(codice, out erroreCalcoloCiDB);
            if (erroreCalcoloCiDB != null)
                erroreCalcoloCi = new ErroreCalcoloCi(erroreCalcoloCiDB);
        }

        public static void GetCodiceGestione(out List<CodeGestione> elencoCodeGestione)
        {
            elencoCodeGestione = null;
            List<DecodificaCodiceGestione> elencoCodeGestioneDB = null;
            DAGestioneDecodifica.GetCodiceGestione(out elencoCodeGestioneDB);
            if (elencoCodeGestioneDB != null && elencoCodeGestioneDB.Count > 0)
            {
                elencoCodeGestione = new List<CodeGestione>();
                foreach (DecodificaCodiceGestione CodeGestioneDB in elencoCodeGestioneDB)
                    elencoCodeGestione.Add(new CodeGestione(CodeGestioneDB));
            }
        }

        public static void GetCodiciParticolari(out List<GestioneDecodifica.CodiceParticolare> elencoCodiciParticolari)
        {
            elencoCodiciParticolari = null;
            List<DecodificaCodiciParticolari> elencoCodiciParticolariDB = null;
            DAGestioneDecodifica.GetCodiciParticolari(out elencoCodiciParticolariDB);
            if (elencoCodiciParticolariDB != null && elencoCodiciParticolariDB.Count > 0)
            {
                elencoCodiciParticolari = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceParticolare>();
                foreach (DecodificaCodiciParticolari codiceParticolareDB in elencoCodiciParticolariDB)
                {
                    elencoCodiciParticolari.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceParticolare(codiceParticolareDB));
                }
            }
        }

        public static void GetPensioniExInpdai(out List<GestioneDecodifica.PensioneExInpdai> elencoPensioniExInpdai)
        {
            elencoPensioniExInpdai = null;
            List<DecodificaPensioneExInpdai> elencoPensioniExInpdaiDB = null;
            DAGestioneDecodifica.GetPensioniExInpdai(out elencoPensioniExInpdaiDB);
            if (elencoPensioniExInpdaiDB != null && elencoPensioniExInpdaiDB.Count > 0)
            {
                elencoPensioniExInpdai = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.PensioneExInpdai>();
                foreach (DecodificaPensioneExInpdai pensioneExInpdaiDB in elencoPensioniExInpdaiDB)
                {
                    elencoPensioniExInpdai.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.PensioneExInpdai(pensioneExInpdaiDB));
                }
            }
        }

        public static void GetCodiciCDCMMR(out List<GestioneDecodifica.CDCMMR> elencoCodiciCDCMMR)
        {
            elencoCodiciCDCMMR = null;
            List<DecodificaCDCMMR> elencoCodiciCDCMMRDB = null;
            DAGestioneDecodifica.GetCodiciCDCMMR(out elencoCodiciCDCMMRDB);
            if (elencoCodiciCDCMMRDB != null && elencoCodiciCDCMMRDB.Count > 0)
            {
                elencoCodiciCDCMMR = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CDCMMR>();
                foreach (DecodificaCDCMMR codiceCDCMMRDB in elencoCodiciCDCMMRDB)
                {
                    elencoCodiciCDCMMR.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CDCMMR(codiceCDCMMRDB));
                }
            }
        }


        public static void GetCodiciMaggiorazioneExCombattenti(out List<GestioneDecodifica.CodiceMaggiorazioneExCombattenti> elencoCodiciCodiceMaggiorazioneExCombattenti)
        {
            elencoCodiciCodiceMaggiorazioneExCombattenti = null;
            List<DecodificaMaggiorazioneExCombattenti> elencoCodiciMaggiorazioneExCombattentiDB = null;
            DAGestioneDecodifica.GetCodiciMaggiorazioneExCombattenti(out elencoCodiciMaggiorazioneExCombattentiDB);
            if (elencoCodiciMaggiorazioneExCombattentiDB != null && elencoCodiciMaggiorazioneExCombattentiDB.Count > 0)
            {
                elencoCodiciCodiceMaggiorazioneExCombattenti = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CodiceMaggiorazioneExCombattenti>();
                foreach (DecodificaMaggiorazioneExCombattenti codiciMaggiorazioneExCombattentiDB in elencoCodiciMaggiorazioneExCombattentiDB)
                {
                    elencoCodiciCodiceMaggiorazioneExCombattenti.Add(new GestioneDecodifica.CodiceMaggiorazioneExCombattenti(codiciMaggiorazioneExCombattentiDB));
                }
            }
        }

        public static void GetGestioneCodeTipoRichiesta(out List<GestioneDecodifica.GestioneCodiceTipoRichiesta> elencoGestioneCodeTipoRichiesta)
        {
            elencoGestioneCodeTipoRichiesta = null;
            List<DecTipoRichiesta> elencoGestioneTipoRichiestaDB = null;
            DAGestioneDecodifica.GetGestioneTipoRichiesta(out elencoGestioneTipoRichiestaDB);
            if (elencoGestioneTipoRichiestaDB != null && elencoGestioneTipoRichiestaDB.Count > 0)
            {
                elencoGestioneCodeTipoRichiesta = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.GestioneCodiceTipoRichiesta>();
                foreach (DecTipoRichiesta CodeGestioneTipoRichiestaDB in elencoGestioneTipoRichiestaDB)
                {
                    elencoGestioneCodeTipoRichiesta.Add(new GestioneDecodifica.GestioneCodiceTipoRichiesta(CodeGestioneTipoRichiestaDB));
                }
            }
        }

        public static void GetGestioneTipoRichiestaByCodTipoRichiesta(string codTipoRichiesta, out GestioneDecodifica.GestioneCodiceTipoRichiesta gestioneCodeTipoRichiesta)
        {
            gestioneCodeTipoRichiesta = null;
            DecTipoRichiesta gestioneTipoRichiestaDB = null;
            DAGestioneDecodifica.GetGestioneTipoRichiestaByCodTipoRichiesta(codTipoRichiesta, out gestioneTipoRichiestaDB);
            if (gestioneTipoRichiestaDB != null)
                gestioneCodeTipoRichiesta = new GestioneDecodifica.GestioneCodiceTipoRichiesta(gestioneTipoRichiestaDB);
        }

        public static void GetGruppoOneri(out List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri)
        {
            elencoGruppoOneri = null;
            List<DecCodeGruppoOneri> elencoGruppoOneriDB = null;
            DAGestioneDecodifica.GetGruppoOneri(out elencoGruppoOneriDB);
            if (elencoGruppoOneriDB != null && elencoGruppoOneriDB.Count > 0)
            {
                elencoGruppoOneri = new List<GestioneDecodifica.GruppoOneri>();
                foreach (DecCodeGruppoOneri GruppoOneriDB in elencoGruppoOneriDB)
                    elencoGruppoOneri.Add(new GestioneDecodifica.GruppoOneri(GruppoOneriDB));
            }
        }

        public static void GetSottoGruppoOneri(out List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri)
        {
            elencoSottoGruppoOneri = null;
            List<DecCodeSottoGruppoOneri> elencoSottoGruppoOneriDB = null;
            DAGestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneriDB);
            if (elencoSottoGruppoOneriDB != null && elencoSottoGruppoOneriDB.Count > 0)
            {
                elencoSottoGruppoOneri = new List<GestioneDecodifica.SottoGruppoOneri>();
                foreach (DecCodeSottoGruppoOneri SottoGruppoOneriDB in elencoSottoGruppoOneriDB)
                    elencoSottoGruppoOneri.Add(new GestioneDecodifica.SottoGruppoOneri(SottoGruppoOneriDB));
            }
        }


        public static void GetElencoDomandaRicorso(out List<GestioneDecodifica.DomandaRicorso> elencoDomandaRicorso)
        {
            elencoDomandaRicorso = null;
            List<DecodificaDomandaRicorso> elencoDecodificaDomandaRicorsoDB = null;
            DAGestioneDecodifica.GetElencoDomandaRicorso(out elencoDecodificaDomandaRicorsoDB);
            if (elencoDecodificaDomandaRicorsoDB != null && elencoDecodificaDomandaRicorsoDB.Count > 0)
            {
                elencoDomandaRicorso = new List<GestioneDecodifica.DomandaRicorso>();
                foreach (DecodificaDomandaRicorso DomandaRicorsoDB in elencoDecodificaDomandaRicorsoDB)
                    elencoDomandaRicorso.Add(new GestioneDecodifica.DomandaRicorso(DomandaRicorsoDB));
            }
        }

        public static void GetElencoDecodificaLegge44997(out List<GestioneDecodifica.DecodificaLegge44997> elencoDecodificaLegge44997)
        {
            elencoDecodificaLegge44997 = null;
            List<Liquidazione.DataCommon.DecodificaLegge44997> elencoDecodificaLegge44997DB = null;
            DAGestioneDecodifica.GetDecodificaLegge44997(out elencoDecodificaLegge44997DB);
            if (elencoDecodificaLegge44997DB != null && elencoDecodificaLegge44997DB.Count > 0)
            {
                elencoDecodificaLegge44997 = new List<GestioneDecodifica.DecodificaLegge44997>();
                foreach (Liquidazione.DataCommon.DecodificaLegge44997 Legge44997DB in elencoDecodificaLegge44997DB)
                    elencoDecodificaLegge44997.Add(new GestioneDecodifica.DecodificaLegge44997(Legge44997DB));
            }
        }

        public static void GetElencoDecModalitaLiquidazione(out List<GestioneDecodifica.DecModalitaLiquidazione> elencoDecodificaModalitaLiquidazione)
        {
            elencoDecodificaModalitaLiquidazione = null;
            List<Liquidazione.DataCommon.DecodificaModalitaLiquidazione> elencoDecodificaModalitaLiquidazioneDB = null;
            DAGestioneDecodifica.GetDecodificaModalitaLiquidazione(out elencoDecodificaModalitaLiquidazioneDB);
            if (elencoDecodificaModalitaLiquidazioneDB != null && elencoDecodificaModalitaLiquidazioneDB.Count > 0)
            {
                elencoDecodificaModalitaLiquidazione = new List<GestioneDecodifica.DecModalitaLiquidazione>();
                foreach (Liquidazione.DataCommon.DecodificaModalitaLiquidazione ModalitaLiquidazioneDB in elencoDecodificaModalitaLiquidazioneDB)
                    elencoDecodificaModalitaLiquidazione.Add(new GestioneDecodifica.DecModalitaLiquidazione(ModalitaLiquidazioneDB));
            }
        }

        public static void GetElencoDecOpzioneRiliquidazione(out List<GestioneDecodifica.DecOpzioneRiliquidazione> elencoDecOpzioneRiliquidazione)
        {
            elencoDecOpzioneRiliquidazione = null;
            List<Liquidazione.DataCommon.DecodificaOpzioneRiliquidazione> elencoDecodificaOpzioneRiliquidazioneDB = null;
            DAGestioneDecodifica.GetDecodificaOpzioneRiliquidazione(out elencoDecodificaOpzioneRiliquidazioneDB);
            if (elencoDecodificaOpzioneRiliquidazioneDB != null && elencoDecodificaOpzioneRiliquidazioneDB.Count > 0)
            {
                elencoDecOpzioneRiliquidazione = new List<GestioneDecodifica.DecOpzioneRiliquidazione>();
                foreach (Liquidazione.DataCommon.DecodificaOpzioneRiliquidazione OpzioneRiliquidazioneDB in elencoDecodificaOpzioneRiliquidazioneDB)
                    elencoDecOpzioneRiliquidazione.Add(new GestioneDecodifica.DecOpzioneRiliquidazione(OpzioneRiliquidazioneDB));
            }
        }

        public static void GetElencoDecCodiceCi21(out List<GestioneDecodifica.DecCodiceCi21> elencoDecodificaCodiceCi21)
        {
            elencoDecodificaCodiceCi21 = null;
            List<Liquidazione.DataCommon.DecodificaCodiceCi21> elencoDecodificaCodiceCi21DB = null;
            DAGestioneDecodifica.GetDecodificaCodiceCi21(out elencoDecodificaCodiceCi21DB);
            if (elencoDecodificaCodiceCi21DB != null && elencoDecodificaCodiceCi21DB.Count > 0)
            {
                elencoDecodificaCodiceCi21 = new List<GestioneDecodifica.DecCodiceCi21>();
                foreach (Liquidazione.DataCommon.DecodificaCodiceCi21 DecodificaCodiceCi21DB in elencoDecodificaCodiceCi21DB)
                    elencoDecodificaCodiceCi21.Add(new GestioneDecodifica.DecCodiceCi21(DecodificaCodiceCi21DB));
            }
        }

        public static void GetElencoDecCodiceCi28(out List<GestioneDecodifica.DecCodiceCi28> elencoDecodificaCodiceCi28)
        {
            elencoDecodificaCodiceCi28 = null;
            List<Liquidazione.DataCommon.DecodificaCodiceCi28> elencoDecodificaCodiceCi28DB = null;
            DAGestioneDecodifica.GetDecodificaCodiceCi28(out elencoDecodificaCodiceCi28DB);
            if (elencoDecodificaCodiceCi28DB != null && elencoDecodificaCodiceCi28DB.Count > 0)
            {
                elencoDecodificaCodiceCi28 = new List<GestioneDecodifica.DecCodiceCi28>();
                foreach (Liquidazione.DataCommon.DecodificaCodiceCi28 DecodificaCodiceCi28DB in elencoDecodificaCodiceCi28DB)
                    elencoDecodificaCodiceCi28.Add(new GestioneDecodifica.DecCodiceCi28(DecodificaCodiceCi28DB));
            }
        }

        public static void GetElencoCodiciPartTime(out List<GestioneDecodifica.DecodificaPartTime> elencoDecodificaPartTime)
        {
            elencoDecodificaPartTime = null;
            List<Liquidazione.DataCommon.DecodificaPartTime> elencoDecodificaPartTimeDB = null;
            DAGestioneDecodifica.GetDecodificaPartTime(out elencoDecodificaPartTimeDB);
            if (elencoDecodificaPartTimeDB != null && elencoDecodificaPartTimeDB.Count > 0)
            {
                elencoDecodificaPartTime = new List<GestioneDecodifica.DecodificaPartTime>();
                foreach (Liquidazione.DataCommon.DecodificaPartTime DecodificaPartTimeDB in elencoDecodificaPartTimeDB)
                    elencoDecodificaPartTime.Add(new GestioneDecodifica.DecodificaPartTime(DecodificaPartTimeDB));
            }
        }

        public static void GetElencoCodiciEsodo(out List<GestioneDecodifica.DecodificaCodeEsodo> elencoDecodificaCodeEsodo)
        {
            elencoDecodificaCodeEsodo = null;
            List<Liquidazione.DataCommon.DecodificaCodiceEsodo> elencoDecodificaCodiceEsodoDB = null;
            DAGestioneDecodifica.GetDecodificaCodiceEsodo(out elencoDecodificaCodiceEsodoDB);
            if (elencoDecodificaCodiceEsodoDB != null && elencoDecodificaCodiceEsodoDB.Count > 0)
            {
                elencoDecodificaCodeEsodo = new List<GestioneDecodifica.DecodificaCodeEsodo>();
                foreach (Liquidazione.DataCommon.DecodificaCodiceEsodo DecodificaCodiceEsodoDB in elencoDecodificaCodiceEsodoDB)
                    elencoDecodificaCodeEsodo.Add(new GestioneDecodifica.DecodificaCodeEsodo(DecodificaCodiceEsodoDB));
            }
        }

        public static void GetElencoCodiciArt22(out List<GestioneDecodifica.DecodificaCodiceArt22> elencoDecodificaCodeArt22)
        {
            elencoDecodificaCodeArt22 = null;
            List<Liquidazione.DataCommon.DecodificaArt22> elencoDecodificaArt22DB = null;
            DAGestioneDecodifica.GetDecodificaArt22(out elencoDecodificaArt22DB);
            if (elencoDecodificaArt22DB != null && elencoDecodificaArt22DB.Count > 0)
            {
                elencoDecodificaCodeArt22 = new List<GestioneDecodifica.DecodificaCodiceArt22>();
                foreach (Liquidazione.DataCommon.DecodificaArt22 decodificaCodiceArt22DB in elencoDecodificaArt22DB)
                    elencoDecodificaCodeArt22.Add(new GestioneDecodifica.DecodificaCodiceArt22(decodificaCodiceArt22DB));
            }
        }

        public static void GetElencoCodiciCapitalizzazione(out List<GestioneDecodifica.DecodificaCodiceCapitalizzazione> elencoDecodificaCodeCapitalizzazione)
        {
            elencoDecodificaCodeCapitalizzazione = null;
            List<Liquidazione.DataCommon.DecodificaCodiceCapitalizzazione> elencoDecodificaCodiceCapitalizzazioneDB = null;
            DAGestioneDecodifica.GetDecodificaCodiceCapitalizzazione(out elencoDecodificaCodiceCapitalizzazioneDB);
            if (elencoDecodificaCodiceCapitalizzazioneDB != null && elencoDecodificaCodiceCapitalizzazioneDB.Count > 0)
            {
                elencoDecodificaCodeCapitalizzazione = new List<GestioneDecodifica.DecodificaCodiceCapitalizzazione>();
                foreach (Liquidazione.DataCommon.DecodificaCodiceCapitalizzazione decodificaCodiceCapitalizzazioneDB in elencoDecodificaCodiceCapitalizzazioneDB)
                    elencoDecodificaCodeCapitalizzazione.Add(new GestioneDecodifica.DecodificaCodiceCapitalizzazione(decodificaCodiceCapitalizzazioneDB));
            }
        }

        public static void GetElencoCodiciCausaCessazione(out List<GestioneDecodifica.DecodificaCausaCessazione> elencoDecodificaCausaCessazione)
        {
            elencoDecodificaCausaCessazione = null;
            List<Liquidazione.DataCommon.DecodificaCausaCessazione> elencoCausaCessazioneDB = null;
            DAGestioneDecodifica.GetDecodificaCausaCessazione(out elencoCausaCessazioneDB);
            if (elencoCausaCessazioneDB != null && elencoCausaCessazioneDB.Count > 0)
            {
                elencoDecodificaCausaCessazione = new List<GestioneDecodifica.DecodificaCausaCessazione>();
                foreach (Liquidazione.DataCommon.DecodificaCausaCessazione decodificaCausaCessazioneDB in elencoCausaCessazioneDB)
                    elencoDecodificaCausaCessazione.Add(new GestioneDecodifica.DecodificaCausaCessazione(decodificaCausaCessazioneDB));
            }
        }

        public static void GetTipoSettimaneBeneficioAGO_CI(out List<GestioneDecodifica.SettimaneBeneficio> elencoSettimaneBeneficioAGO_CI)
        {
            elencoSettimaneBeneficioAGO_CI = null;
            List<DecodificaTipoBeneficioAGO_CI> elencoTipoBeneficioDB = null;
            DAGestioneDecodifica.GetCodiceTipoBeneficioAGO_CI(out elencoTipoBeneficioDB);
            if (elencoTipoBeneficioDB != null && elencoTipoBeneficioDB.Count > 0)
            {
                elencoSettimaneBeneficioAGO_CI = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio>();
                foreach (DecodificaTipoBeneficioAGO_CI tipoBeneficioDB in elencoTipoBeneficioDB)
                {
                    elencoSettimaneBeneficioAGO_CI.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.SettimaneBeneficio(tipoBeneficioDB));
                }
            }
        }

        public static void GetSiglaFamiliareByParentela(string parentela, out char? siglaFamiliare, out string tipoUnione)
        {
            DAGestioneDecodifica.GetSiglaFamiliareByParentela(parentela, out siglaFamiliare, out tipoUnione);
        }

        public static void GetParentelaBySiglaFamiliare(char siglaFamiliare, out string parentela)
        {
            DAGestioneDecodifica.GetParentelaBySiglaFamiliare(siglaFamiliare, out parentela);
        }

        public static void GetTipoDomanda(string gruppo, string prodotto, string tipo, string gestione, string fondo, string ente,
            bool? indConvInt, bool? att107, string canale, string fonte, string fase, out char? tipoDomanda)
        {
            tipoDomanda = null;

            switch (fase)
            {
                case "0001":
                    tipoDomanda = 'M';
                    return;
                case "0060":
                case "0061":
                    tipoDomanda = 'I';
                    return;
                case "0062":
                    tipoDomanda = 'X';
                    return;
                case "0063":
                    tipoDomanda = 'Y';
                    return;
                default:
                    break;
            }

            List<DecTipoDomanda> listaTipiDomanda = null;
            DAGestioneDecodifica.GetTipiDomanda(gruppo, prodotto, tipo, gestione, fondo, ente, out listaTipiDomanda);
            if (listaTipiDomanda == null || listaTipiDomanda.Count == 0)
                return;

            //filtro per canale e fonte
            #region Canale_Fonte
            List<DecTipoDomanda> listaTipiDomandaFiltrata = new List<DecTipoDomanda>();
            foreach (DecTipoDomanda td in listaTipiDomanda)
            {
                if (td.Canale == "xxx" && td.Fonte == "xx")
                    listaTipiDomandaFiltrata.Add(td);
                else if (td.Canale == "xxx" && td.Fonte != "xx")
                {
                    if (td.Fonte.StartsWith("D"))
                    {
                        if (fonte != td.Fonte.Replace('D', '0'))
                            listaTipiDomandaFiltrata.Add(td);
                    }
                    else
                    {
                        if (fonte == td.Fonte)
                            listaTipiDomandaFiltrata.Add(td);
                    }
                }
                else if (td.Canale != "xxx" && td.Fonte == "xx")
                {
                    if (td.Canale.StartsWith("D"))
                    {
                        if (canale != td.Canale.Replace('D', '0'))
                            listaTipiDomandaFiltrata.Add(td);
                    }
                    else
                    {
                        if (canale == td.Canale)
                            listaTipiDomandaFiltrata.Add(td);
                    }
                }
                else
                {
                    if (((td.Canale.StartsWith("D") && canale != td.Canale.Replace('D', '0')) ||
                        (!td.Canale.StartsWith("D") && canale == td.Canale)) &&
                        ((td.Fonte.StartsWith("D") && fonte != td.Fonte.Replace('D', '0')) ||
                        (!td.Fonte.StartsWith("D") && fonte == td.Fonte)))
                    {
                        listaTipiDomandaFiltrata.Add(td);
                    }
                }
            }
            #endregion Canale_Fonte
            if (listaTipiDomandaFiltrata == null || listaTipiDomandaFiltrata.Count == 0)
                return;
            //filtro per indConvInt e Att107
            #region IndConvint_Att107
            List<DecTipoDomanda> listaTipiDomandaFinale = new List<DecTipoDomanda>();
            foreach (DecTipoDomanda td in listaTipiDomandaFiltrata)
            {
                if (!td.IndConvInt.HasValue && !td.Att107.HasValue)
                    listaTipiDomandaFinale.Add(td);
                else if (!td.IndConvInt.HasValue && td.Att107.HasValue)
                {
                    if (att107.HasValue && att107.Value == td.Att107.Value)
                        listaTipiDomandaFinale.Add(td);
                }
                else if (td.IndConvInt.HasValue && !td.Att107.HasValue)
                {
                    if (indConvInt.HasValue && indConvInt.Value == td.IndConvInt.Value)
                        listaTipiDomandaFinale.Add(td);
                }
                else
                {
                    if (att107.HasValue && att107.Value == td.Att107.Value &&
                        indConvInt.HasValue && indConvInt.Value == td.IndConvInt.Value)
                        listaTipiDomandaFinale.Add(td);
                }
            }
            #endregion IndConvint_Att107

            if (listaTipiDomandaFinale == null || listaTipiDomandaFinale.Count == 0)
                return;

            tipoDomanda = listaTipiDomandaFinale[0].TipoDomanda;
        }


        public static void GetElencoPensioniPrivilegiate(out List<DecPensioniPrivilegiate> elencoPensioniPrivilegiate)
        {
            elencoPensioniPrivilegiate = new List<DecPensioniPrivilegiate>();
            List<DecodificaPensioniPrivilegiate> elencoPrivilegiateDB = null;
            DAGestioneDecodifica.GetDecodificaPensioniPrivilegiate(out elencoPrivilegiateDB);
            foreach (DecodificaPensioniPrivilegiate PrivilegiateDB in elencoPrivilegiateDB)
            {
                DecPensioniPrivilegiate decPensioniPrivilegiateBL = new DecPensioniPrivilegiate();
                Utility.ValorizzaOggetti(PrivilegiateDB, decPensioniPrivilegiateBL);
                elencoPensioniPrivilegiate.Add(decPensioniPrivilegiateBL);
            }
        }

        public static void GetElencoRiconoscimentiInvalidita(out List<DecRiconoscimentiInvalidita> elencoRiconoscimentiInvalidita)
        {
            elencoRiconoscimentiInvalidita = new List<DecRiconoscimentiInvalidita>();
            List<DecodificaRiconoscimentiInvalidita> elencoRiconoscimentiInvaliditaDB = null;
            DAGestioneDecodifica.GetDecodificaRiconoscimentiInvalidita(out elencoRiconoscimentiInvaliditaDB);
            foreach (DecodificaRiconoscimentiInvalidita RiconoscimentiInvaliditaDB in elencoRiconoscimentiInvaliditaDB)
            {
                DecRiconoscimentiInvalidita decRiconoscimentiInvaliditaBL = new DecRiconoscimentiInvalidita();
                Utility.ValorizzaOggetti(RiconoscimentiInvaliditaDB, decRiconoscimentiInvaliditaBL);
                elencoRiconoscimentiInvalidita.Add(decRiconoscimentiInvaliditaBL);
            }
        }

        public static void GetElencoCassaSede(string gruppo, string prodotto, string tipo, Utility.TipoAppartenenza? tipoApp, out List<DecCassaSede> elencoCassaSede)
        {
            elencoCassaSede = new List<DecCassaSede>();
            List<DecodificaCassa> elencoCassaSedeDB = null;
            DAGestioneDecodifica.GetCassaSede(gruppo, prodotto, tipo, tipoApp.HasValue ? tipoApp.Value.ToString() : string.Empty, out elencoCassaSedeDB);
            if (elencoCassaSedeDB != null && elencoCassaSedeDB.Count > 0)
            {
                foreach (DecodificaCassa DecodificaCassaDB in elencoCassaSedeDB)
                {
                    DecCassaSede decCassaSedeBL = new DecCassaSede();
                    Utility.ValorizzaOggetti(DecodificaCassaDB, decCassaSedeBL);
                    elencoCassaSede.Add(decCassaSedeBL);
                }
            }
        }

        public static void GetElencoEnte(out List<DecodeEnte> elencoEnte)
        {
            elencoEnte = new List<DecodeEnte>();
            List<DecodificaEnte> elencoEnteDB = null;
            DAGestioneDecodifica.GetCodiceEnte(out elencoEnteDB);
            foreach (DecodificaEnte EnteDB in elencoEnteDB)
            {
                DecodeEnte decEnteBL = new DecodeEnte();
                Utility.ValorizzaOggetti(EnteDB, decEnteBL);
                elencoEnte.Add(decEnteBL);
            }
        }

        public static bool CheckProdottoTipo(string prodotto, string tipo)
        {
            return DAGestioneDecodifica.CheckProdottoTipo(prodotto, tipo);
        }

        public static void GetCatEnteAltrePensioni(out List<GestioneDecodifica.CatEnteAltraPensione> elencoCatEnteAltrePensioni)
        {
            elencoCatEnteAltrePensioni = null;
            List<DecCatEnteAltrePensioni> elencoCatEnteAltrePensioniDB = null;
            DAGestioneDecodifica.GetCatEnteAltrePensioni(out elencoCatEnteAltrePensioniDB);
            if (elencoCatEnteAltrePensioniDB != null && elencoCatEnteAltrePensioniDB.Count > 0)
            {
                elencoCatEnteAltrePensioni = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CatEnteAltraPensione>();
                foreach (DecCatEnteAltrePensioni catEnteDB in elencoCatEnteAltrePensioniDB)
                {
                    elencoCatEnteAltrePensioni.Add(new INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.CatEnteAltraPensione(catEnteDB));
                }
            }
        }

        public static void GetTipoComponenteByCode(char tipoComponente, out string descTipoComponente)
        {
            DAGestioneDecodifica.GetTipoComponenteByCode(tipoComponente, out descTipoComponente);
        }

        public static void GetGruppo(out List<GestioneDecodifica.Gruppo> elencoGruppo)
        {
            elencoGruppo = null;
            List<DecGruppo> elencoGruppoDB = null;
            DAGestioneDecodifica.GetGruppo(out elencoGruppoDB);
            if (elencoGruppoDB != null && elencoGruppoDB.Count > 0)
            {
                elencoGruppo = new List<GestioneDecodifica.Gruppo>();
                foreach (DecGruppo decodificaGruppoDB in elencoGruppoDB)
                {
                    GestioneDecodifica.Gruppo gruppo = new GestioneDecodifica.Gruppo();
                    Utility.ValorizzaOggetti(decodificaGruppoDB, gruppo);
                    elencoGruppo.Add(gruppo);
                }
            }
        }

        public static void GetProdotto(out List<GestioneDecodifica.Prodotto> elencoProdotto)
        {
            elencoProdotto = null;
            List<DecProdotto> elencoProdottoDB = null;
            DAGestioneDecodifica.GetProdotto(out elencoProdottoDB);
            if (elencoProdottoDB != null && elencoProdottoDB.Count > 0)
            {
                elencoProdotto = new List<GestioneDecodifica.Prodotto>();
                foreach (DecProdotto decodificaProdottoDB in elencoProdottoDB)
                {
                    GestioneDecodifica.Prodotto prodotto = new GestioneDecodifica.Prodotto();
                    Utility.ValorizzaOggetti(decodificaProdottoDB, prodotto);
                    elencoProdotto.Add(prodotto);
                }
            }
        }

        public static void GetTipo(out List<GestioneDecodifica.Tipo> elencoTipo)
        {
            elencoTipo = null;
            List<DecTipo> elencoTipoDB = null;
            DAGestioneDecodifica.GetTipo(out elencoTipoDB);
            if (elencoTipoDB != null && elencoTipoDB.Count > 0)
            {
                elencoTipo = new List<GestioneDecodifica.Tipo>();
                foreach (DecTipo decodificaTipoDB in elencoTipoDB)
                {
                    GestioneDecodifica.Tipo tipo = new GestioneDecodifica.Tipo();
                    Utility.ValorizzaOggetti(decodificaTipoDB, tipo);
                    elencoTipo.Add(tipo);
                }
            }
        }

        public static void GetFiltro(out List<GestioneDecodifica.Filtro> listaFiltro)
        {
            listaFiltro = new List<GestioneDecodifica.Filtro>();
            List<GestioneDecodifica.GestioneCodiceTipoRichiesta> ListaCodTipoRichiesta = null;
            GestioneDecodifica.GetGestioneCodeTipoRichiesta(out ListaCodTipoRichiesta);
            if (ListaCodTipoRichiesta != null)
            {
                //elimino dalla lista tutti i record che hanno il Filtro duplicato
                ListaCodTipoRichiesta = ListaCodTipoRichiesta.GroupBy(x => x.Filtro).Select(x => x.First()).ToList();

                foreach (GestioneDecodifica.GestioneCodiceTipoRichiesta codfiltro in ListaCodTipoRichiesta)
                {
                    GestioneDecodifica.Filtro filtro = new GestioneDecodifica.Filtro();
                    filtro.Codice = codfiltro.Filtro;
                    filtro.Descrizione = codfiltro.DescTipoRichiesta;
                    listaFiltro.Add(filtro);
                }
            }
        }

        public static void GetUfficiPagatoriEsteri(out List<UfficiPagatoriEsteri> elencoUfficiPagatoriEsteri)
        {
            elencoUfficiPagatoriEsteri = null;
            List<DecodificaUfficiPagatoriEsteri> elencoUfficiPagatoriEsteriDB = null;

            DAGestioneDecodifica.GetUfficiPagatoriEsteri(out elencoUfficiPagatoriEsteriDB);
            if (elencoUfficiPagatoriEsteriDB != null && elencoUfficiPagatoriEsteriDB.Count > 0)
            {
                elencoUfficiPagatoriEsteri = new List<UfficiPagatoriEsteri>();
                foreach (DecodificaUfficiPagatoriEsteri ufficioPagatoreDB in elencoUfficiPagatoriEsteriDB)
                {
                    UfficiPagatoriEsteri ufficioPagatore = new UfficiPagatoriEsteri();
                    Utility.ValorizzaOggetti(ufficioPagatoreDB, ufficioPagatore);
                    elencoUfficiPagatoriEsteri.Add(ufficioPagatore);
                }
            }
        }

        public static void GetTipoPensioneFondi(out List<TipoPensioneFondi> elencoTipoPensioneFondi)
        {
            elencoTipoPensioneFondi = null;
            List<DecodificaTipoPensioneFondi> elencoTipoPensioneFondiDB = null;

            DAGestioneDecodifica.GetTipoPensioneFondi(out elencoTipoPensioneFondiDB);
            if (elencoTipoPensioneFondiDB != null && elencoTipoPensioneFondiDB.Count > 0)
            {
                elencoTipoPensioneFondi = new List<TipoPensioneFondi>();
                foreach (DecodificaTipoPensioneFondi tipoPensioneFondiDB in elencoTipoPensioneFondiDB)
                {
                    TipoPensioneFondi tipoPensioneFondi = new TipoPensioneFondi();
                    Utility.ValorizzaOggetti(tipoPensioneFondiDB, tipoPensioneFondi);
                    elencoTipoPensioneFondi.Add(tipoPensioneFondi);
                }
            }
        }

        public static void GetDerogaENPALS(out List<DerogaENPALS> elencoDerogaENPALS)
        {
            elencoDerogaENPALS = null;
            List<DecodificaDerogaENPAL> elencoDerogaENPALSDB = null;

            DAGestioneDecodifica.GetDerogaENPALS(out elencoDerogaENPALSDB);
            if (elencoDerogaENPALSDB != null && elencoDerogaENPALSDB.Count > 0)
            {
                elencoDerogaENPALS = new List<DerogaENPALS>();
                foreach (DecodificaDerogaENPAL derogaENPALSDB in elencoDerogaENPALSDB)
                {
                    DerogaENPALS derogaENPALS = new DerogaENPALS();
                    Utility.ValorizzaOggetti(derogaENPALSDB, derogaENPALS);
                    elencoDerogaENPALS.Add(derogaENPALS);
                }
            }
        }

        public static void GetDecodificaArt58(out List<DecArt58> elencoArt58, string tipoFondo)
        {
            elencoArt58 = null;
            List<DecodificaArt58> elencoArt58Db = null;

            DAGestioneDecodifica.GetDecodificaArt58(out elencoArt58Db, tipoFondo);
            if (elencoArt58Db != null && elencoArt58Db.Count > 0)
            {
                elencoArt58 = new List<DecArt58>();
                foreach (DecodificaArt58 objDb in elencoArt58Db)
                {
                    DecArt58 objBl = new DecArt58();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    elencoArt58.Add(objBl);
                }
            }

        }

        public static void GetDecodificaPromiscui(out List<DecPromiscui> elencoPromiscui, string tipoFondo)
        {
            elencoPromiscui = null;
            List<DecodificaPromiscui> elencoPromiscuiDb = null;

            DAGestioneDecodifica.GetDecodificaPromiscui(out elencoPromiscuiDb, tipoFondo);
            if (elencoPromiscuiDb != null && elencoPromiscuiDb.Count > 0)
            {
                elencoPromiscui = new List<DecPromiscui>();
                foreach (DecodificaPromiscui objDb in elencoPromiscuiDb)
                {
                    DecPromiscui objBl = new DecPromiscui();
                    Utility.ValorizzaOggetti(objDb, objBl);
                    elencoPromiscui.Add(objBl);
                }
            }

        }

        public static string GetDescFase(string codFase)
        {
            DecFase fase = null;
            DAGestioneDecodifica.GetFase(codFase, out fase);
            if (fase != null)
                return fase.DescFase;
            return codFase;
        }

        public static void GetDecodificaTipoLiquidazionePM(out List<DecodificaTipoLiquidazionePM> elencoTipoLiquidazionePM)
        {
            elencoTipoLiquidazionePM = null;
            List<DataCommon.DecodificaTipoLiquidazionePM> elencoTipoLiquidazionePMDB = null;
            DAGestioneDecodifica.GetDecodificaTipoLiquidazionePM(out elencoTipoLiquidazionePMDB);
            if (elencoTipoLiquidazionePMDB != null && elencoTipoLiquidazionePMDB.Count > 0)
            {
                elencoTipoLiquidazionePM = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePM>();
                foreach (DataCommon.DecodificaTipoLiquidazionePM tipoLiquidazionePMDB in elencoTipoLiquidazionePMDB)
                {
                    DecodificaTipoLiquidazionePM tipoLiquidazionePM = new DecodificaTipoLiquidazionePM();
                    Utility.ValorizzaOggetti(tipoLiquidazionePMDB, tipoLiquidazionePM);
                    elencoTipoLiquidazionePM.Add(tipoLiquidazionePM);
                }
            }
        }

        public static void GetDecodificaLegge413(out List<DecodificaLegge413> elencoDecodificaLegge413)
        {
            elencoDecodificaLegge413 = null;
            List<DataCommon.DecodificaLegge413> elencoDecodificaLegge413DB = null;
            DAGestioneDecodifica.GetDecodificaLegge413(out elencoDecodificaLegge413DB);
            if (elencoDecodificaLegge413DB != null && elencoDecodificaLegge413DB.Count > 0)
            {
                elencoDecodificaLegge413 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaLegge413>();
                foreach (DataCommon.DecodificaLegge413 decodificaLegge413DB in elencoDecodificaLegge413DB)
                {
                    DecodificaLegge413 decodificaLegge413 = new DecodificaLegge413();
                    Utility.ValorizzaOggetti(decodificaLegge413DB, decodificaLegge413);
                    elencoDecodificaLegge413.Add(decodificaLegge413);
                }
            }
        }

        public static void GetDecodificaAttivitaSvolta2(out List<DecodificaAttivitaSvolta2> elencoDecodificaAttivitaSvolta2)
        {
            elencoDecodificaAttivitaSvolta2 = null;
            List<DataCommon.DecodificaAttivitaSvolta2> elencoDecodificaAttivitaSvolta2DB = null;
            DAGestioneDecodifica.GetAttivitaSvolta2(out elencoDecodificaAttivitaSvolta2DB);
            if (elencoDecodificaAttivitaSvolta2DB != null && elencoDecodificaAttivitaSvolta2DB.Count > 0)
            {
                elencoDecodificaAttivitaSvolta2 = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaAttivitaSvolta2>();
                foreach (DataCommon.DecodificaAttivitaSvolta2 decodificaAttivitaSvolta2DB in elencoDecodificaAttivitaSvolta2DB)
                {
                    DecodificaAttivitaSvolta2 decodificaAttivitaSvolta2 = new DecodificaAttivitaSvolta2();
                    Utility.ValorizzaOggetti(decodificaAttivitaSvolta2DB, decodificaAttivitaSvolta2);
                    elencoDecodificaAttivitaSvolta2.Add(decodificaAttivitaSvolta2);
                }
            }
        }

        public static void GetDecodificaTipoLiquidazione(out List<DecodificaTipoLiquidazione> elencoDecodificaTipoLiquidazione)
        {
            elencoDecodificaTipoLiquidazione = null;
            List<DataCommon.DecodificaTipoLiquidazione> elencoDecodificaTipoLiquidazioneDB = null;
            DAGestioneDecodifica.GetTipoLiquidazione(out elencoDecodificaTipoLiquidazioneDB);
            if (elencoDecodificaTipoLiquidazioneDB != null && elencoDecodificaTipoLiquidazioneDB.Count > 0)
            {
                elencoDecodificaTipoLiquidazione = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazione>();
                foreach (DataCommon.DecodificaTipoLiquidazione decodificaTipoLiquidazioneDB in elencoDecodificaTipoLiquidazioneDB)
                {
                    DecodificaTipoLiquidazione decodificaTipoLiquidazione = new DecodificaTipoLiquidazione();
                    Utility.ValorizzaOggetti(decodificaTipoLiquidazioneDB, decodificaTipoLiquidazione);
                    elencoDecodificaTipoLiquidazione.Add(decodificaTipoLiquidazione);
                }
            }
        }

        public static void GetTipologiaFAQ(out List<TipologiaFAQ> elencoTipologiaFAQ)
        {
            elencoTipologiaFAQ = null;
            List<DataCommon.DecTipologiaFAQ> elencoTipologiaFAQDB = null;
            DAGestioneDecodifica.GetTipologiaFAQ(out elencoTipologiaFAQDB);
            if (elencoTipologiaFAQDB != null && elencoTipologiaFAQDB.Count > 0)
            {
                elencoTipologiaFAQ = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.TipologiaFAQ>();
                foreach (DataCommon.DecTipologiaFAQ tipologiaFAQDB in elencoTipologiaFAQDB)
                {
                    TipologiaFAQ tipologiaFAQ = new TipologiaFAQ();
                    Utility.ValorizzaOggetti(tipologiaFAQDB, tipologiaFAQ);
                    elencoTipologiaFAQ.Add(tipologiaFAQ);
                }
            }
        }

        public static void GetDecodificaTipoLiquidazioneGAS(out List<DecodificaTipoLiquidazioneGAS> elencoDecodificaTipoLiquidazioneGAS)
        {
            elencoDecodificaTipoLiquidazioneGAS = null;
            List<DataCommon.DecodificaTipoLiquidazioneGA> elencoDecodificaTipoLiquidazioneGAS_DB = null;
            DAGestioneDecodifica.GetTipoLiquidazioneGAS(out elencoDecodificaTipoLiquidazioneGAS_DB);
            if (elencoDecodificaTipoLiquidazioneGAS_DB != null && elencoDecodificaTipoLiquidazioneGAS_DB.Count > 0)
            {
                elencoDecodificaTipoLiquidazioneGAS = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazioneGAS>();
                foreach (DataCommon.DecodificaTipoLiquidazioneGA decodificaTipoLiquidazioneGAS_DB in elencoDecodificaTipoLiquidazioneGAS_DB)
                {
                    DecodificaTipoLiquidazioneGAS decodificaTipoLiquidazioneGAS = new DecodificaTipoLiquidazioneGAS();
                    Utility.ValorizzaOggetti(decodificaTipoLiquidazioneGAS_DB, decodificaTipoLiquidazioneGAS);
                    elencoDecodificaTipoLiquidazioneGAS.Add(decodificaTipoLiquidazioneGAS);
                }
            }
        }

        public static void GetDecodificaTipoLiquidazionePI(out List<DecodificaTipoLiquidazionePI> elencoDecodificaTipoLiquidazionePI)
        {
            elencoDecodificaTipoLiquidazionePI = null;
            List<DataCommon.DecodificaTipoLiquidazionePI> elencoDecodificaTipoLiquidazionePI_DB = null;
            DAGestioneDecodifica.GetTipoLiquidazionePI(out elencoDecodificaTipoLiquidazionePI_DB);
            if (elencoDecodificaTipoLiquidazionePI_DB != null && elencoDecodificaTipoLiquidazionePI_DB.Count > 0)
            {
                elencoDecodificaTipoLiquidazionePI = new List<INPS.Pensioni.Liquidazione.BLCommon.GestioneDecodifica.DecodificaTipoLiquidazionePI>();
                foreach (DataCommon.DecodificaTipoLiquidazionePI decodificaTipoLiquidazionePI_DB in elencoDecodificaTipoLiquidazionePI_DB)
                {
                    DecodificaTipoLiquidazionePI decodificaTipoLiquidazionePI = new DecodificaTipoLiquidazionePI();
                    Utility.ValorizzaOggetti(decodificaTipoLiquidazionePI_DB, decodificaTipoLiquidazionePI);
                    elencoDecodificaTipoLiquidazionePI.Add(decodificaTipoLiquidazionePI);
                }
            }
        }

        public static void GetCodiceTipoLiquidazionePM(out List<CodiceTipoLiquidazionePM> elencoCodiceTipoLiquidazionePM)
        {
            elencoCodiceTipoLiquidazionePM = null;
            List<DataCommon.DecodificaCodiceTipoLiquidazionePM> elencoCodiceTipoLiquidazionePMDB = null;
            DAGestioneDecodifica.GetCodiceTipoLiquidazionePM(out elencoCodiceTipoLiquidazionePMDB);
            if (elencoCodiceTipoLiquidazionePMDB != null && elencoCodiceTipoLiquidazionePMDB.Count > 0)
            {
                elencoCodiceTipoLiquidazionePM = new List<CodiceTipoLiquidazionePM>();
                foreach (DataCommon.DecodificaCodiceTipoLiquidazionePM codiceTipoLiquidazionePMDB in elencoCodiceTipoLiquidazionePMDB)
                {
                    CodiceTipoLiquidazionePM codiceTipoLiquidazionePM = new CodiceTipoLiquidazionePM();
                    Utility.ValorizzaOggetti(codiceTipoLiquidazionePMDB, codiceTipoLiquidazionePM);
                    elencoCodiceTipoLiquidazionePM.Add(codiceTipoLiquidazionePM);
                }
            }
        }

        public static void GetDecodificaCodiceTipoQuota(out List<DecodificaTipoQuota> elencoTipoQuota)
        {
            elencoTipoQuota = null;
            List<DataCommon.DecodificaTipoQuota> elencoDb = null;
            DAGestioneDecodifica.GetDecodificaTipoQuota(out elencoDb);
            if (elencoDb != null && elencoDb.Count > 0)
            {
                elencoTipoQuota = elencoDb.Select(x => { var r = new DecodificaTipoQuota(); Utility.ValorizzaOggetti(x, r); return r; }).ToList();
            }
        }

        public static void GetDecodificaEnteCassaProfessionale(out List<DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionale)
        {
            elencoDecodificaEnteCassaProfessionale = null;
            List<DataCommon.DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionaleDB = null;
            DAGestioneDecodifica.GetDecodificaEnteCassaProfessionale(out elencoDecodificaEnteCassaProfessionaleDB);
            if (elencoDecodificaEnteCassaProfessionaleDB != null && elencoDecodificaEnteCassaProfessionaleDB.Count > 0)
            {
                elencoDecodificaEnteCassaProfessionale = new List<DecodificaEnteCassaProfessionale>();
                foreach (DataCommon.DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionaleDB in elencoDecodificaEnteCassaProfessionaleDB)
                {
                    DecodificaEnteCassaProfessionale decodificaEnteCassaProfessionale = new DecodificaEnteCassaProfessionale();
                    Utility.ValorizzaOggetti(decodificaEnteCassaProfessionaleDB, decodificaEnteCassaProfessionale);
                    elencoDecodificaEnteCassaProfessionale.Add(decodificaEnteCassaProfessionale);
                }
            }
        }

        public static void GetDecEnteGestioneFondo(out List<DecEnteGestioneFondo> elencoDecEnteGestioneFondo)
        {
            elencoDecEnteGestioneFondo = null;
            List<DataCommon.DecEnteGestioneFondo> elencoDecEnteGestioneFondoDB = null;
            DAGestioneDecodifica.GetDecEnteGestioneFondo(out elencoDecEnteGestioneFondoDB);
            if (elencoDecEnteGestioneFondoDB != null && elencoDecEnteGestioneFondoDB.Count > 0)
            {
                elencoDecEnteGestioneFondo = new List<DecEnteGestioneFondo>();
                foreach (DataCommon.DecEnteGestioneFondo decEnteGestioneFondoDB in elencoDecEnteGestioneFondoDB)
                {
                    DecEnteGestioneFondo decEnteGestioneFondo = new DecEnteGestioneFondo();
                    Utility.ValorizzaOggetti(decEnteGestioneFondoDB, decEnteGestioneFondo);
                    elencoDecEnteGestioneFondo.Add(decEnteGestioneFondo);
                }
            }
        }

        public static void GetDecCodiceTrattenute(out List<DecCodiceTrattenute> elencoDecCodiceTrattenute)
        {
            elencoDecCodiceTrattenute = null;
            List<DataCommon.DecCodiceTrattenute> elencoDecCodiceTrattenuteDB = null;
            DAGestioneDecodifica.GetDecCodiceTrattenute(out elencoDecCodiceTrattenuteDB);
            if (elencoDecCodiceTrattenuteDB != null && elencoDecCodiceTrattenuteDB.Count > 0)
            {
                elencoDecCodiceTrattenute = new List<DecCodiceTrattenute>();
                foreach (DataCommon.DecCodiceTrattenute decCodiceTrattenuteDB in elencoDecCodiceTrattenuteDB)
                {
                    DecCodiceTrattenute decCodiceTrattenute = new DecCodiceTrattenute();
                    Utility.ValorizzaOggetti(decCodiceTrattenuteDB, decCodiceTrattenute);
                    elencoDecCodiceTrattenute.Add(decCodiceTrattenute);
                }
            }
        }

        public static void GetDecPersonaleViaggiante(out List<DecPersonaleViaggiante> elencoDecPersonaleViaggiante)
        {
            elencoDecPersonaleViaggiante = null;
            List<DataCommon.DecPersonaleViaggiante> elencoDecPersonaleViaggianteDB = null;
            DAGestioneDecodifica.GetDecPersonaleViaggiante(out elencoDecPersonaleViaggianteDB);
            if (elencoDecPersonaleViaggianteDB != null && elencoDecPersonaleViaggianteDB.Count > 0)
            {
                elencoDecPersonaleViaggiante = new List<DecPersonaleViaggiante>();
                foreach (DataCommon.DecPersonaleViaggiante decPersonaleViaggianteDB in elencoDecPersonaleViaggianteDB)
                {
                    DecPersonaleViaggiante decPersonaleViaggiante = new DecPersonaleViaggiante();
                    Utility.ValorizzaOggetti(decPersonaleViaggianteDB, decPersonaleViaggiante);
                    elencoDecPersonaleViaggiante.Add(decPersonaleViaggiante);
                }
            }
        }

        public static void GetDecodificaAttCon(out List<AttCon> elencoDecodificaAttCon)
        {
            elencoDecodificaAttCon = null;
            List<DataCommon.DecodificaAttCon> elencoDecodificaAttConDB = null;
            DAGestioneDecodifica.GetDecodificaAttCon(out elencoDecodificaAttConDB);
            if (elencoDecodificaAttConDB != null && elencoDecodificaAttConDB.Count > 0)
            {
                elencoDecodificaAttCon = new List<AttCon>();
                foreach (DataCommon.DecodificaAttCon decodificaAttConDB in elencoDecodificaAttConDB)
                {
                    AttCon attCon = new AttCon();
                    Utility.ValorizzaOggetti(decodificaAttConDB, attCon);
                    elencoDecodificaAttCon.Add(attCon);
                }
            }
        }

        public static bool IsPensioneRiferimentoObbligatoria(string codGruppo, string codProdotto, string codTipo)
        {
            return DAGestioneDecodifica.IsPensioneRiferimentoObbligatoria(codGruppo, codProdotto, codTipo);
        }

        public static void GetDecodificaSoggettoBeneficiario(out List<SoggettoBeneficiario> elencoSoggettoBeneficiario)
        {
            elencoSoggettoBeneficiario = null;
            List<DataCommon.DecodificaSoggettoBeneficiario> elencoDecodificaSoggettoBeneficiarioDB = null;
            DAGestioneDecodifica.GetDecodificaSoggettoBeneficiario(out elencoDecodificaSoggettoBeneficiarioDB);
            if (elencoDecodificaSoggettoBeneficiarioDB != null && elencoDecodificaSoggettoBeneficiarioDB.Count > 0)
            {
                elencoSoggettoBeneficiario = new List<SoggettoBeneficiario>();
                foreach (DataCommon.DecodificaSoggettoBeneficiario decodificaSoggettoBeneficiarioDB in elencoDecodificaSoggettoBeneficiarioDB)
                {
                    SoggettoBeneficiario soggettoBeneficiario = new SoggettoBeneficiario();
                    Utility.ValorizzaOggetti(decodificaSoggettoBeneficiarioDB, soggettoBeneficiario);
                    elencoSoggettoBeneficiario.Add(soggettoBeneficiario);
                }
            }
        }

        public static void GetDecodificaTipologiaPrestazione(out List<TipologiaPrestazione> elencoTipologiaPrestazione)
        {
            elencoTipologiaPrestazione = null;
            List<DataCommon.DecodificaTipologiaPrestazione> elencoDecodificaTipologiaPrestazioneDB = null;
            DAGestioneDecodifica.GetDecodificaTipologiaPrestazione(out elencoDecodificaTipologiaPrestazioneDB);
            if (elencoDecodificaTipologiaPrestazioneDB != null && elencoDecodificaTipologiaPrestazioneDB.Count > 0)
            {
                elencoTipologiaPrestazione = new List<TipologiaPrestazione>();
                foreach (DataCommon.DecodificaTipologiaPrestazione decodificaTipologiaPrestazioneDB in elencoDecodificaTipologiaPrestazioneDB)
                {
                    TipologiaPrestazione tipologiaPrestazione = new TipologiaPrestazione();
                    Utility.ValorizzaOggetti(decodificaTipologiaPrestazioneDB, tipologiaPrestazione);
                    elencoTipologiaPrestazione.Add(tipologiaPrestazione);
                }
            }
        }

        public static void GetDecTipologiaBeneficioTerrorismo(out List<TipologiaBeneficioTerrorismo> elencoTipologiaBeneficioTerrorismo)
        {
            elencoTipologiaBeneficioTerrorismo = null;
            List<DataCommon.DecTipologiaBeneficioTerrorismo> elencoDecTipologiaBeneficioTerrorismoDB = null;
            DAGestioneDecodifica.GetDecTipologiaBeneficioTerrorismo(out elencoDecTipologiaBeneficioTerrorismoDB);
            if (elencoDecTipologiaBeneficioTerrorismoDB != null && elencoDecTipologiaBeneficioTerrorismoDB.Count > 0)
            {
                elencoTipologiaBeneficioTerrorismo = new List<TipologiaBeneficioTerrorismo>();
                foreach (DataCommon.DecTipologiaBeneficioTerrorismo decTipologiaBeneficioTerrorismoDB in elencoDecTipologiaBeneficioTerrorismoDB)
                {
                    TipologiaBeneficioTerrorismo tipologiaBeneficioTerrorismo = new TipologiaBeneficioTerrorismo();
                    Utility.ValorizzaOggetti(decTipologiaBeneficioTerrorismoDB, tipologiaBeneficioTerrorismo);
                    elencoTipologiaBeneficioTerrorismo.Add(tipologiaBeneficioTerrorismo);
                }
            }
        }

        public static void GetDecodificaSituazione(out List<DecSituazione> elencoSituazioneWebDom)
        {
            elencoSituazioneWebDom = null;
            List<DataCommon.DecSituazione> elencoDecSituazioneDB = null;
            DAGestioneDecodifica.GetDecodificaSituazione(out elencoDecSituazioneDB);
            if (elencoDecSituazioneDB != null && elencoDecSituazioneDB.Count > 0)
            {
                elencoSituazioneWebDom = new List<DecSituazione>();
                foreach (DataCommon.DecSituazione decodificaSituazioneDB in elencoDecSituazioneDB)
                {
                    DecSituazione situazione = new DecSituazione();
                    Utility.ValorizzaOggetti(decodificaSituazioneDB, situazione);
                    elencoSituazioneWebDom.Add(situazione);
                }
            }
        }

        public static void GetDescrizioneIstanza(string gruppo, string prodotto, string tipo, out string descIstanza)
        {
            descIstanza = string.Empty;
            DAGestioneDecodifica.GetDescrizioneIstanza(gruppo, prodotto, tipo, out descIstanza);
        }

        public static void GetEnteRipartizioneINPDAP(out List<DecodificaEnteRipartizioneINPDAP> elencoEnteRipartizioneINPDAP)
        {
            elencoEnteRipartizioneINPDAP = null;
            List<DataCommon.DecodificaEnteRipartizioneINPDAP> elencoEnteRipartizioneINPDAPDB = null;
            DAGestioneDecodifica.GetDecodificaEnteRipartizioneINPDAP(out elencoEnteRipartizioneINPDAPDB);
            if (elencoEnteRipartizioneINPDAPDB != null && elencoEnteRipartizioneINPDAPDB.Count > 0)
            {
                elencoEnteRipartizioneINPDAP = new List<DecodificaEnteRipartizioneINPDAP>();
                foreach (DataCommon.DecodificaEnteRipartizioneINPDAP decEnteRipartizioneINPDAPDB in elencoEnteRipartizioneINPDAPDB)
                {
                    DecodificaEnteRipartizioneINPDAP EnteRipartizioneINPDAP = new DecodificaEnteRipartizioneINPDAP();
                    Utility.ValorizzaOggetti(decEnteRipartizioneINPDAPDB, EnteRipartizioneINPDAP);
                    elencoEnteRipartizioneINPDAP.Add(EnteRipartizioneINPDAP);
                }
            }
        }

        public static void GetDecodificaInteresseLegale(out List<DecodificaInteresseLegale> elencoDecodificaInteresseLegale)
        {
            elencoDecodificaInteresseLegale = null;
            List<DataCommon.DecodificaInteresseLegale> elencoDecodificaInteresseLegaleDB = null;
            DAGestioneDecodifica.GetDecodificaInteresseLegale(out elencoDecodificaInteresseLegaleDB);
            if (elencoDecodificaInteresseLegaleDB != null && elencoDecodificaInteresseLegaleDB.Count > 0)
            {
                elencoDecodificaInteresseLegale = new List<DecodificaInteresseLegale>();
                foreach (DataCommon.DecodificaInteresseLegale decInteresseLegaleDB in elencoDecodificaInteresseLegaleDB)
                {
                    DecodificaInteresseLegale interesseLegale = new DecodificaInteresseLegale();
                    Utility.ValorizzaOggetti(decInteresseLegaleDB, interesseLegale);
                    elencoDecodificaInteresseLegale.Add(interesseLegale);
                }
            }
        }

        public static void GetCtrlNoInvioMailDirettore(out List<CtrlNoInvioMailDirettore> elencoCtrlNoInvioMailDirettore)
        {
            elencoCtrlNoInvioMailDirettore = null;
            List<DataCommon.CtrlNoInvioMailDirettore> elencoCtrlNoInvioMailDirettoreDB = null;
            DAGestioneDecodifica.GetCtrlNoInvioMailDirettore(out elencoCtrlNoInvioMailDirettoreDB);
            if (elencoCtrlNoInvioMailDirettoreDB != null && elencoCtrlNoInvioMailDirettoreDB.Count > 0)
            {
                elencoCtrlNoInvioMailDirettore = new List<CtrlNoInvioMailDirettore>();
                foreach (DataCommon.CtrlNoInvioMailDirettore ctrlNoInvioMailDirettoreDB in elencoCtrlNoInvioMailDirettoreDB)
                {
                    CtrlNoInvioMailDirettore ctrlNoInvioMailDirettore = new CtrlNoInvioMailDirettore();
                    Utility.ValorizzaOggetti(ctrlNoInvioMailDirettoreDB, ctrlNoInvioMailDirettore);
                    elencoCtrlNoInvioMailDirettore.Add(ctrlNoInvioMailDirettore);
                }
            }
        }

        public static void GetDecodificaTipoCalcoloVincenteDAI(out List<DecTipoCalcoloVincenteDAI> elencoDecodificaTipoCalcoloVincenteDAI)
        {
            elencoDecodificaTipoCalcoloVincenteDAI = null;
            List<DataCommon.DecodificaTipoCalcoloVincenteDAI> elencoDecodificaTipoCalcoloVincenteDB = null;
            DAGestioneDecodifica.GetDecodificaTipoCalcoloVincenteDAI(out elencoDecodificaTipoCalcoloVincenteDB);
            if (elencoDecodificaTipoCalcoloVincenteDB != null && elencoDecodificaTipoCalcoloVincenteDB.Count > 0)
            {
                elencoDecodificaTipoCalcoloVincenteDAI = new List<DecTipoCalcoloVincenteDAI>();
                foreach (DataCommon.DecodificaTipoCalcoloVincenteDAI decodificaTipoCalcoloVincenteDB in elencoDecodificaTipoCalcoloVincenteDB)
                {
                    DecTipoCalcoloVincenteDAI decTipoCalcoloVincente = new DecTipoCalcoloVincenteDAI();
                    Utility.ValorizzaOggetti(decodificaTipoCalcoloVincenteDB, decTipoCalcoloVincente);
                    elencoDecodificaTipoCalcoloVincenteDAI.Add(decTipoCalcoloVincente);
                }
            }
        }

        public static bool IsBypassInvioMailDirettore(string siglaCategoria)
        {
            return DAGestioneDecodifica.IsBypassInvioMailDirettore(siglaCategoria);
        }

        public static void GetCtrlRequisitoEta(out List<CtrlRequisitoEta> elencoCtrlRequisitoEta)
        {
            elencoCtrlRequisitoEta = null;
            List<DataCommon.CtrlRequisitoEta> elencoCtrlRequisitoEtaDB = null;
            DAGestioneDecodifica.GetCtrlRequisitoEta(out elencoCtrlRequisitoEtaDB);
            if (elencoCtrlRequisitoEtaDB != null && elencoCtrlRequisitoEtaDB.Count > 0)
            {
                elencoCtrlRequisitoEta = new List<CtrlRequisitoEta>();
                foreach (DataCommon.CtrlRequisitoEta decodificaCtrlRequisitoEtaDB in elencoCtrlRequisitoEtaDB)
                {
                    CtrlRequisitoEta decodificaCtrlRequisitoEta = new CtrlRequisitoEta();
                    Utility.ValorizzaOggetti(decodificaCtrlRequisitoEtaDB, decodificaCtrlRequisitoEta);
                    elencoCtrlRequisitoEta.Add(decodificaCtrlRequisitoEta);
                }
            }
        }

        public static void GetCtrlRequisitoEta_Base(DateTime dataRiferimento, string codCategoria, char sesso, string tipoAppartenenza, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            DAGestioneDecodifica.GetCtrlRequisitoEta_Base(dataRiferimento, codCategoria, sesso, tipoAppartenenza, out reqAA, out reqMM);
        }

        public static void GetCtrlRequisitoEta_Avanzato(DateTime dataRiferimento, string codCategoria, char? sesso, string tipoAppartenenza, string codTipo, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            Expression<Func<DataCommon.CtrlRequisitoEta, bool>> whereCondition = p => true;
            whereCondition = whereCondition.And(x => x.InizioPeriodoPerfRequisiti <= dataRiferimento && x.FinePeriodoPerfRequisiti >= dataRiferimento);
            if (!string.IsNullOrEmpty(codCategoria))
                whereCondition = whereCondition.And(x => x.Categoria == codCategoria.PadLeft(4, '0'));
            if (sesso.HasValue)
                whereCondition = whereCondition.And(x => x.Sesso == sesso.Value);
            if (!string.IsNullOrEmpty(tipoAppartenenza))
                whereCondition = whereCondition.And(x => x.TipoAppartenenza == tipoAppartenenza);
            if (!string.IsNullOrEmpty(codTipo))
                whereCondition = whereCondition.And(x => x.CodTipo == codTipo);

            DAGestioneDecodifica.GetCtrlRequisitoEta_Avanzato(whereCondition, out reqAA, out reqMM);
        }

        public static void GetCtrlRequisitoEta_Anticipo(DateTime dataRiferimento, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            DAGestioneDecodifica.GetCtrlRequisitoEta_Anticipo(dataRiferimento, out reqAA, out reqMM);
        }

        public static void GetCtrlRicercaGPT(out List<CtrlRicercaGPT> elencoCtrlRicercaGPT)
        {
            elencoCtrlRicercaGPT = null;
            List<DataCommon.CtrlRicercaGPT> elencoCtrlRicercaGPTDB = null;
            DAGestioneDecodifica.GetCtrlRicercaGPT(out elencoCtrlRicercaGPTDB);
            if (elencoCtrlRicercaGPTDB != null && elencoCtrlRicercaGPTDB.Count > 0)
            {
                elencoCtrlRicercaGPT = new List<CtrlRicercaGPT>();
                foreach (DataCommon.CtrlRicercaGPT gptDB in elencoCtrlRicercaGPTDB)
                {
                    CtrlRicercaGPT gpt = new CtrlRicercaGPT();
                    Utility.ValorizzaOggetti(gptDB, gpt);
                    elencoCtrlRicercaGPT.Add(gpt);
                }
            }
        }

        public static void GetDecMicroqualificaINPDAP(string siglaCategoria, out List<DecMicroqualificaINPDAP> elencoDecMicroqualificaINPDAP)
        {
            elencoDecMicroqualificaINPDAP = null;
            List<DataCommon.DecMicroqualificaINPDAP> elencoDecMicroqualificaINPDAPDB = null;
            DAGestioneDecodifica.GetDecMicroqualificaINPDAP(siglaCategoria, out elencoDecMicroqualificaINPDAPDB);
            if (elencoDecMicroqualificaINPDAPDB != null && elencoDecMicroqualificaINPDAPDB.Count > 0)
            {
                elencoDecMicroqualificaINPDAP = new List<DecMicroqualificaINPDAP>();
                foreach (DataCommon.DecMicroqualificaINPDAP microqualificaDB in elencoDecMicroqualificaINPDAPDB)
                {
                    DecMicroqualificaINPDAP microqualifica = new DecMicroqualificaINPDAP();
                    Utility.ValorizzaOggetti(microqualificaDB, microqualifica);
                    elencoDecMicroqualificaINPDAP.Add(microqualifica);
                }
            }
        }

        public static void GetMicroqualificaById(long id, out DecMicroqualificaINPDAP microqualificaINPDAP)
        {
            microqualificaINPDAP = null;
            DataCommon.DecMicroqualificaINPDAP microqualificaDB = null;
            DAGestioneDecodifica.GetMicroqualificaINPDAPById(id, out microqualificaDB);
            if (microqualificaDB != null)
            {
                microqualificaINPDAP = new DecMicroqualificaINPDAP();
                Utility.ValorizzaOggetti(microqualificaDB, microqualificaINPDAP);
            }
        }

        public static void GetMicroqualificaByTraduzioneSuGP(string traduzioneSuGP, out DecMicroqualificaINPDAP microqualificaINPDAP)
        {
            microqualificaINPDAP = null;
            DataCommon.DecMicroqualificaINPDAP microqualificaDB = null;
            DAGestioneDecodifica.GetMicroqualificaINPDAPByTraduzioneSuGP(traduzioneSuGP, out microqualificaDB);
            if (microqualificaDB != null)
            {
                microqualificaINPDAP = new DecMicroqualificaINPDAP();
                Utility.ValorizzaOggetti(microqualificaDB, microqualificaINPDAP);
            }
        }

        public static void GetCtrlEnteCassaCodiceGestione(out List<CtrlEnteCassaCodiceGestione> elencoEnteCassaCodiceGestione)
        {
            elencoEnteCassaCodiceGestione = null;
            List<DataCommon.CtrlEnteCassaCodiceGestione> elencoEnteCassaCodiceGestioneDB = null;
            DAGestioneDecodifica.GetCtrlEnteCassaCodiceGestione(out elencoEnteCassaCodiceGestioneDB);
            if (elencoEnteCassaCodiceGestioneDB != null && elencoEnteCassaCodiceGestioneDB.Count > 0)
            {
                elencoEnteCassaCodiceGestione = new List<CtrlEnteCassaCodiceGestione>();
                foreach (DataCommon.CtrlEnteCassaCodiceGestione enteCassaCodiceGestioneDB in elencoEnteCassaCodiceGestioneDB)
                {
                    CtrlEnteCassaCodiceGestione enteCassaCodiceGestione = new CtrlEnteCassaCodiceGestione();
                    Utility.ValorizzaOggetti(enteCassaCodiceGestioneDB, enteCassaCodiceGestione);
                    elencoEnteCassaCodiceGestione.Add(enteCassaCodiceGestione);
                }
            }
        }

        public static void GetCtrlCatAdeguata(out List<CtrlCatAdeguata> elencoCatAdeguata)
        {
            elencoCatAdeguata = null;
            List<DataCommon.CtrlCatAdeguata> elencoCatAdeguataDB = null;
            DAGestioneDecodifica.GetCtrlCatAdeguata(out elencoCatAdeguataDB);
            if (elencoCatAdeguataDB != null && elencoCatAdeguataDB.Count > 0)
            {
                elencoCatAdeguata = new List<CtrlCatAdeguata>();
                foreach (DataCommon.CtrlCatAdeguata catAdeguataDB in elencoCatAdeguataDB)
                {
                    CtrlCatAdeguata catAdeguata = new CtrlCatAdeguata();
                    Utility.ValorizzaOggetti(catAdeguataDB, catAdeguata);
                    elencoCatAdeguata.Add(catAdeguata);
                }
            }
        }

        public static void GetCtrlTipoUfficio(out List<CtrlTipoUfficio> elencoTipoUfficio)
        {

            elencoTipoUfficio = null;
            List<DataCommon.CtrlTipoUfficio> elencoTipoUfficioDB = null;
            DAGestioneDecodifica.GetCtrlTipoUfficio(out elencoTipoUfficioDB);
            if (elencoTipoUfficioDB != null && elencoTipoUfficioDB.Count > 0)
            {
                elencoTipoUfficio = new List<CtrlTipoUfficio>();
                foreach (DataCommon.CtrlTipoUfficio tipoUfficioDB in elencoTipoUfficioDB)
                {
                    CtrlTipoUfficio tipoUfficio = new CtrlTipoUfficio();
                    Utility.ValorizzaOggetti(tipoUfficioDB, tipoUfficio);
                    elencoTipoUfficio.Add(tipoUfficio);
                }
            }
        }

        public static void GetDecCapitolo(bool PL, out List<DecCapitolo> elencoDecCapitolo)
        {
            elencoDecCapitolo = null;
            List<DataCommon.DecCapitolo> elencoDecCapitoloDB = null;
            DAGestioneDecodifica.GetDecCapitolo(PL, out elencoDecCapitoloDB);
            if (elencoDecCapitoloDB != null && elencoDecCapitoloDB.Count > 0)
            {
                elencoDecCapitolo = new List<DecCapitolo>();
                foreach (DataCommon.DecCapitolo decCapitoloDB in elencoDecCapitoloDB)
                {
                    DecCapitolo decCapitolo = new DecCapitolo();
                    Utility.ValorizzaOggetti(decCapitoloDB, decCapitolo);
                    elencoDecCapitolo.Add(decCapitolo);
                }
            }
        }

        public static void GetCtrlCompartoSettoreRuoloByCat(string categoria, out List<CtrlCompartoSettoreRuolo> elencoCtrlCompartoSettoreRuolo)
        {
            elencoCtrlCompartoSettoreRuolo = null;
            List<DataCommon.CtrlCompartoSettoreRuolo> elencoCtrlCompartoSettoreRuoloByCatDB = null;
            DAGestioneDecodifica.GetCtrlCompartoSettoreRuoloByCat(categoria, out elencoCtrlCompartoSettoreRuoloByCatDB);
            if (elencoCtrlCompartoSettoreRuoloByCatDB != null && elencoCtrlCompartoSettoreRuoloByCatDB.Count > 0)
            {
                elencoCtrlCompartoSettoreRuolo = new List<CtrlCompartoSettoreRuolo>();
                foreach (DataCommon.CtrlCompartoSettoreRuolo decDB in elencoCtrlCompartoSettoreRuoloByCatDB)
                {
                    CtrlCompartoSettoreRuolo dec = new CtrlCompartoSettoreRuolo();
                    Utility.ValorizzaOggetti(decDB, dec);
                    elencoCtrlCompartoSettoreRuolo.Add(dec);
                }
            }
        }

        public static void GetElencoDecComparto(out List<DecComparto> elencoDecComparto)
        {
            elencoDecComparto = null;
            List<DataCommon.DecComparto> elencoDecCompartoDB = null;
            DAGestioneDecodifica.GetDecComparto(out elencoDecCompartoDB);
            if (elencoDecCompartoDB != null && elencoDecCompartoDB.Count > 0)
            {
                elencoDecComparto = new List<DecComparto>();
                foreach (DataCommon.DecComparto decCompartoDB in elencoDecCompartoDB)
                {
                    DecComparto decComparto = new DecComparto();
                    Utility.ValorizzaOggetti(decCompartoDB, decComparto);
                    elencoDecComparto.Add(decComparto);
                }
            }
        }

        public static void GetElencoDecSettore(out List<DecSettore> elencoDecSettore)
        {
            elencoDecSettore = null;
            List<DataCommon.DecSettore> elencoDecSettoreDB = null;
            DAGestioneDecodifica.GetDecSettore(out elencoDecSettoreDB);
            if (elencoDecSettoreDB != null && elencoDecSettoreDB.Count > 0)
            {
                elencoDecSettore = new List<DecSettore>();
                foreach (DataCommon.DecSettore decSettoreDB in elencoDecSettoreDB)
                {
                    DecSettore decSettore = new DecSettore();
                    Utility.ValorizzaOggetti(decSettoreDB, decSettore);
                    elencoDecSettore.Add(decSettore);
                }
            }
        }

        public static void GetElencoDecRuolo(out List<DecRuolo> elencoDecRuolo)
        {
            elencoDecRuolo = null;
            List<DataCommon.DecRuolo> elencoDecRuoloDB = null;
            DAGestioneDecodifica.GetDecRuolo(out elencoDecRuoloDB);
            if (elencoDecRuoloDB != null && elencoDecRuoloDB.Count > 0)
            {
                elencoDecRuolo = new List<DecRuolo>();
                foreach (DataCommon.DecRuolo decRuoloDB in elencoDecRuoloDB)
                {
                    DecRuolo decRuolo = new DecRuolo();
                    Utility.ValorizzaOggetti(decRuoloDB, decRuolo);
                    elencoDecRuolo.Add(decRuolo);
                }
            }
        }

        public static void GetElencoDecSede(out List<DecSede> elencoDecSede)
        {
            elencoDecSede = null;
            List<DataCommon.DecSede> elencoDecSedeDB = null;
            DAGestioneDecodifica.GetDecSede(out elencoDecSedeDB);
            if (elencoDecSedeDB != null && elencoDecSedeDB.Count > 0)
            {
                elencoDecSede = new List<DecSede>();
                foreach (DataCommon.DecSede decSedeDB in elencoDecSedeDB)
                {
                    DecSede decSede = new DecSede();
                    Utility.ValorizzaOggetti(decSedeDB, decSede);
                    elencoDecSede.Add(decSede);
                }
            }
        }

        public static void GetDecodificaBanchePerSede(out List<DecodificaBanchePerSede> elencoBanchePerSede)
        {
            elencoBanchePerSede = null;
            List<DataCommon.DecodificaBanchePerSede> elencoBanchePerSedeDB = null;
            DAGestioneDecodifica.GetDecodificaBanchePerSede(out elencoBanchePerSedeDB);
            if (elencoBanchePerSedeDB != null && elencoBanchePerSedeDB.Count > 0)
            {
                elencoBanchePerSede = new List<DecodificaBanchePerSede>();
                foreach (DataCommon.DecodificaBanchePerSede decBanchePerSedeDB in elencoBanchePerSedeDB)
                {
                    DecodificaBanchePerSede BanchePerSede = new DecodificaBanchePerSede();
                    Utility.ValorizzaOggetti(decBanchePerSedeDB, BanchePerSede);
                    elencoBanchePerSede.Add(BanchePerSede);
                }
            }
        }

        public static void GetCtrlScadenzaIndennizzoINDCOM(out List<CtrlScadenzaIndennizzoINDCOM> elencoCtrlScadenzaIndennizzoINDCOM)
        {
            elencoCtrlScadenzaIndennizzoINDCOM = null;
            List<DataCommon.CtrlScadenzaIndennizzoINDCOM> elencoCtrlScadenzaIndennizzoINDCOMDB = null;
            DAGestioneCtrlScadenzaIndennizzoINDCOM.GetCtrlScadenzaIndennizzoINDCOM(out elencoCtrlScadenzaIndennizzoINDCOMDB);
            if (elencoCtrlScadenzaIndennizzoINDCOMDB != null && elencoCtrlScadenzaIndennizzoINDCOMDB.Count > 0)
            {
                elencoCtrlScadenzaIndennizzoINDCOM = new List<CtrlScadenzaIndennizzoINDCOM>();
                foreach (DataCommon.CtrlScadenzaIndennizzoINDCOM ctrlScadenzaIndennizzoINDCOMDB in elencoCtrlScadenzaIndennizzoINDCOMDB)
                {
                    CtrlScadenzaIndennizzoINDCOM ctrlScadenzaIndennizzoINDCOM = new CtrlScadenzaIndennizzoINDCOM();
                    Utility.ValorizzaOggetti(ctrlScadenzaIndennizzoINDCOMDB, ctrlScadenzaIndennizzoINDCOM);
                    elencoCtrlScadenzaIndennizzoINDCOM.Add(ctrlScadenzaIndennizzoINDCOM);
                }
            }
        }
        #region nested class
        public class FondoPensione
        {
            public FondoPensione(DecGestioneFondo fondoPensione)
            {
                _CodFondo = fondoPensione.CodFondo;
                _CodGestione = fondoPensione.CodGestione;
                _DescFondo = fondoPensione.DescFondo;
                _DescGestione = fondoPensione.DescGestione;
            }

            #region private properties
            private string _CodFondo;
            private string _CodGestione;
            private string _DescFondo;
            private string _DescGestione;
            #endregion private properties

            #region public properties
            public string CodFondo
            {
                get { return _CodFondo; }
                set { _CodFondo = value; }
            }

            public string CodGestione
            {
                get { return _CodGestione; }
                set { _CodGestione = value; }
            }

            public string DescFondo
            {
                get { return _DescFondo; }
                set { _DescFondo = value; }
            }

            public string DescGestione
            {
                get { return _DescGestione; }
                set { _DescGestione = value; }
            }
            #endregion public properties

        }

        public class CategoriaPensione
        {
            public CategoriaPensione() { }

            public CategoriaPensione(DecCatPensione categoriaPensione)
            {
                _CodCatPensione = categoriaPensione.CodCatPensione;
                _SiglaCatPensione = categoriaPensione.SiglaCatPensione;
                _TipoCatPensione = categoriaPensione.TipoCatPensione;
                _AppartenenzaCatPensione = categoriaPensione.AppartenenzaCatPensione;

            }

            #region private fields
            private string _CodCatPensione;
            private string _SiglaCatPensione;
            private char _TipoCatPensione;
            private string _AppartenenzaCatPensione;
            #endregion private fields

            #region public properties
            public string CodCatPensione
            {
                get { return _CodCatPensione; }
                set { _CodCatPensione = value; }
            }

            public string SiglaCatPensione
            {
                get { return _SiglaCatPensione; }
                set { _SiglaCatPensione = value; }
            }

            public char TipoCatPensione
            {
                get { return _TipoCatPensione; }
                set { _TipoCatPensione = value; }
            }
            public string AppartenenzaCatPensione
            {
                get { return _AppartenenzaCatPensione; }
                set { _AppartenenzaCatPensione = value; }
            }
            #endregion public properties
        }

        public class StatoPensione
        {
            public StatoPensione(DecodificaStatoPensione statoPensione)
            {
                _DecodificaStato = statoPensione.DecodificaStato;
                _CodiceStato = statoPensione.Id.ToString();
            }

            #region private fields
            private string _DecodificaStato;
            private string _CodiceStato;
            #endregion private fields

            #region public properties
            public string DecodificaStato
            {
                get { return _DecodificaStato; }
                set { _DecodificaStato = value; }
            }
            public string CodiceStato
            {
                get { return _CodiceStato; }
                set { _CodiceStato = value; }
            }
            #endregion public properties
        }

        public class Patronato
        {
            public Patronato(DecPatronato patronato)
            {
                this._CodInpsPatronato = patronato.CodInpsPatronato;
                this._CodUfficioPatronato = patronato.CodUfficioPatronato;
                this._DescPatronato = patronato.DescPatronato;
                this._DescUfficioPatronato = patronato.DescUfficioPatronato;
                this._TipoEnte = patronato.TipoEnte;
            }

            #region public properties
            public string CodInpsPatronato { get { return _CodInpsPatronato; } set { _CodInpsPatronato = value; } }

            public string CodUfficioPatronato { get { return _CodUfficioPatronato; } set { _CodUfficioPatronato = value; } }

            public string DescPatronato { get { return _DescPatronato; } set { _DescPatronato = value; } }

            public string DescUfficioPatronato { get { return _DescUfficioPatronato; } set { _DescUfficioPatronato = value; } }

            public string TipoEnte { get { return _TipoEnte; } set { _TipoEnte = value; } }
            #endregion public properties

            #region private properties
            private string _CodInpsPatronato;

            private string _CodUfficioPatronato;

            private string _DescPatronato;

            private string _DescUfficioPatronato;

            private string _TipoEnte;
            #endregion private properties
        }

        public class StatoCivile
        {
            public StatoCivile(DecodificaStatoCivile statoCivile)
            {
                this._Id = statoCivile.Id;
                this._Descrizione = statoCivile.Descrizione != null ? statoCivile.Descrizione : "";
            }

            #region public properties
            public char Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private char _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ParentelaDC
        {
            public ParentelaDC(DecodificaParentela parenteladc)
            {
                this._Id = parenteladc.Id.ToString();
                this._Descrizione = parenteladc.Descrizione != null ? parenteladc.Descrizione : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class Maggiorazione781
        {
            public Maggiorazione781(DecodificaMaggiorazione781ContributiDC maggiorazione781)
            {
                this._Id = maggiorazione781.Id.ToString();
                this._Descrizione = maggiorazione781.Descrizione != null ? maggiorazione781.Descrizione : "";
            }

            #region public properties

            public string Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private string _Id;
            private string _Descrizione;

            #endregion private properties
        }

        public class CodiceProvenienza
        {
            public CodiceProvenienza(DecodificaCodiceProvenienza codiceProvenienza)
            {
                this._Id = codiceProvenienza.Id.ToString();
                this._Descrizione = codiceProvenienza.Descrizione != null ? codiceProvenienza.Descrizione : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiciVari
        {
            public CodiciVari(DecodificaCodiciVariDC CodiciVari)
            {
                this._Id = CodiciVari.Id.ToString();
                this._Descrizione = CodiciVari.Descrizione != null ? CodiciVari.Descrizione : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class StatoEstero
        {
            public StatoEstero(DecComuneNazione statoEstero)
            {
                this._CodCatastale = statoEstero.CodCatastale != null ? statoEstero.CodCatastale.Trim() : "";
                this._Descrizione = statoEstero.DescComuneNazione != null ? statoEstero.DescComuneNazione.Trim() : "";
                this._Sigla = statoEstero.SiglaProvinciaNazione != null ? statoEstero.SiglaProvinciaNazione.Trim() : "";
            }

            #region public properties
            public string CodCatastale { get { return _CodCatastale; } set { _CodCatastale = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string Sigla { get { return _Sigla; } set { _Sigla = value; } }
            #endregion public properties

            #region private properties
            private string _CodCatastale;

            private string _Descrizione;

            private string _Sigla;
            #endregion private properties
        }

        public class Provincia
        {
            public Provincia(DecProvincia provincia)
            {
                this._SiglaProvincia = provincia.SiglaProvincia != null ? provincia.SiglaProvincia.Trim() : "";
                this._DescrizioneProvincia = provincia.DescProvincia != null ? provincia.DescProvincia.Trim() : "";
                this._DescrizioneRegione = provincia.DescRegione != null ? provincia.DescRegione.Trim() : "";
            }

            #region public properties
            public string SiglaProvincia { get { return _SiglaProvincia; } set { _SiglaProvincia = value; } }

            public string DescrizioneProvincia { get { return _DescrizioneProvincia; } set { _DescrizioneProvincia = value; } }

            public string DescrizioneRegione { get { return _DescrizioneRegione; } set { _DescrizioneRegione = value; } }
            #endregion public properties

            #region private properties
            private string _SiglaProvincia;

            private string _DescrizioneProvincia;

            private string _DescrizioneRegione;
            #endregion private properties
        }

        public class Comune
        {
            public Comune(DecComuneNazione comune)
            {
                this._CodCatastale = comune.CodCatastale != null ? comune.CodCatastale.Trim() : "";
                this._Descrizione = comune.DescComuneNazione != null ? comune.DescComuneNazione.Trim() : "";
                this._SiglaProvincia = comune.SiglaProvinciaNazione != null ? comune.SiglaProvinciaNazione.Trim() : "";
                this._Cap = comune.Cap != null ? comune.Cap.Trim() : "";
            }

            #region public properties
            public string CodCatastale { get { return _CodCatastale; } set { _CodCatastale = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string SiglaProvincia { get { return _SiglaProvincia; } set { _SiglaProvincia = value; } }

            public string Cap { get { return _Cap; } set { _Cap = value; } }
            #endregion public properties

            #region private properties
            private string _CodCatastale;

            private string _Descrizione;

            private string _SiglaProvincia;

            private string _Cap;
            #endregion private properties
        }

        public class ConiugeOFiglio
        {
            public ConiugeOFiglio(DecodificaConiugeOFiglio coniugeOFiglio)
            {
                this._Id = coniugeOFiglio.Id.ToString();
                this._Descrizione = coniugeOFiglio.Descrizione != null ? coniugeOFiglio.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class DetrazioniReddito
        {
            public DetrazioniReddito(DecodificaDetrazioniReddito detrazioniReddito)
            {
                this._Id = detrazioniReddito.Id.ToString();
                this._Descrizione = detrazioniReddito.Descrizione != null ? detrazioniReddito.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class Tutore
        {
            public Tutore(DecodificaTutore tutore)
            {
                this._Id = tutore.Id.ToString();
                this._Descrizione = tutore.Descrizione != null ? tutore.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class Delegato
        {
            public Delegato(DecodificaCodiceDelegato delegato)
            {
                this._Id = delegato.Id.ToString();
                this._Descrizione = delegato.Descrizione != null ? delegato.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class SiglaFamiliare
        {
            public SiglaFamiliare(DecodificaSiglaFamiliare siglaFamiliare)
            {
                this._Id = siglaFamiliare.Id.ToString();
                this._Descrizione = siglaFamiliare.Descrizione != null ? siglaFamiliare.Descrizione.Trim() : "";
                this._TipoUnione = siglaFamiliare.TipoUnione;
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            private string _TipoUnione;
            #endregion private properties
        }

        public class Familiare
        {
            public Familiare(DecodificaFamiliare familiare)
            {
                this._Id = familiare.Id.ToString();
                this._Descrizione = familiare.Descrizione != null ? familiare.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ValidazioneCF
        {
            public ValidazioneCF(DecodificaValidazioneCF validazioneCF)
            {
                this._Id = validazioneCF.Id.ToString();
                this._Descrizione = validazioneCF.Descrizione != null ? validazioneCF.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ModalitaPagamento
        {
            public ModalitaPagamento(DecodificaModalitaPagamento modalitaPagamento)
            {
                this._Id = modalitaPagamento.Id.ToString();
                this._Descrizione = modalitaPagamento.Descrizione != null ? modalitaPagamento.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class TipoPagamento
        {
            public TipoPagamento(DecodificaTipoPagamento tipoPagamento)
            {
                this._Id = tipoPagamento.Id.ToString();
                this._Descrizione = tipoPagamento.Descrizione != null ? tipoPagamento.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class TipoCalcolo
        {
            public TipoCalcolo(DecodificaTipoCalcolo tipoCalcolo)
            {
                this._Id = tipoCalcolo.Id.ToString();
                this._Descrizione = tipoCalcolo.Descrizione != null ? tipoCalcolo.Descrizione.Trim() : "";
                this._TraduzioneSuGP = tipoCalcolo.TraduzioneSuGP;
                this._Tipo = tipoCalcolo.Tipo != null ? tipoCalcolo.Tipo.Trim() : "";
                this._Tipologia = tipoCalcolo.Tipologia != null ? tipoCalcolo.Tipologia.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public System.Nullable<byte> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            private System.Nullable<byte> _TraduzioneSuGP;

            private string _Tipo;

            private string _Tipologia;
            #endregion private properties
        }

        public class CausaCarico
        {
            public CausaCarico(DecodificaCausaCarico causaCarico)
            {
                this._Id = causaCarico.Id.ToString();
                this._Descrizione = causaCarico.Descrizione != null ? causaCarico.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceEliminazione
        {
            public CodiceEliminazione(DecodificaCodiceEliminazione codiceEliminazione)
            {
                this._Id = codiceEliminazione.Id.ToString();
                this._Descrizione = codiceEliminazione.Descrizione != null ? codiceEliminazione.Descrizione.Trim() : "";
                this._TestoVideo = codiceEliminazione.TestoVideo;
                this.TraduzioneSuGP = codiceEliminazione.TraduzioneSuGP;
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string TestoVideo { get { return _TestoVideo; } set { _TestoVideo = value; } }

            public char? TraduzioneSuGP { get; set; }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            private string _TestoVideo;

            #endregion private properties
        }

        public class AttivitaSvolta
        {
            public AttivitaSvolta(DecodificaAttivitaSvolta attivitaSvolta)
            {
                this._Id = attivitaSvolta.Id;
                this._Descrizione = attivitaSvolta.Descrizione != null ? attivitaSvolta.Descrizione.Trim() : "";
                this._Fondo = attivitaSvolta.Fondo != null ? attivitaSvolta.Fondo.Trim() : "";
                this._TraduzioneSuGp = attivitaSvolta.TraduzioneSuGp != null ? attivitaSvolta.TraduzioneSuGp.Trim() : "";
                this._InizioValidita = attivitaSvolta.InizioValidita.HasValue ? attivitaSvolta.InizioValidita : null;
                this._FineValidita = attivitaSvolta.FineValidita.HasValue ? attivitaSvolta.FineValidita : null;
                this._LimiteEta = attivitaSvolta.LimiteEta.HasValue ? attivitaSvolta.LimiteEta : null;
                this._LimiteServizio = attivitaSvolta.LimiteServizio.HasValue ? attivitaSvolta.LimiteServizio : null;
                this._PersonaleViaggiante = attivitaSvolta.PersonaleViaggiante.HasValue ? attivitaSvolta.PersonaleViaggiante : null;
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            public string TraduzioneSuGp { get { return _TraduzioneSuGp; } set { _TraduzioneSuGp = value; } }

            public System.Nullable<System.DateTime> InizioValidita { get { return _InizioValidita; } set { _InizioValidita = value; } }

            public System.Nullable<System.DateTime> FineValidita { get { return _FineValidita; } set { _FineValidita = value; } }

            public System.Nullable<byte> LimiteEta { get { return _LimiteEta; } set { _LimiteEta = value; } }

            public System.Nullable<byte> LimiteServizio { get { return _LimiteServizio; } set { _LimiteServizio = value; } }

            public System.Nullable<bool> PersonaleViaggiante { get { return _PersonaleViaggiante; } set { _PersonaleViaggiante = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            private string _Fondo;

            private string _TraduzioneSuGp;

            private System.Nullable<System.DateTime> _InizioValidita;

            private System.Nullable<System.DateTime> _FineValidita;

            private System.Nullable<byte> _LimiteEta;

            private System.Nullable<byte> _LimiteServizio;

            private System.Nullable<bool> _PersonaleViaggiante;
            #endregion private properties
        }

        public class CodiceCristallizzazione
        {
            public CodiceCristallizzazione(DecodificaCodiceCristallizzazione codiceCristallizzazione)
            {
                this._Id = codiceCristallizzazione.Id.ToString();
                this._Descrizione = codiceCristallizzazione.Descrizione != null ? codiceCristallizzazione.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class TipoPensione
        {
            public TipoPensione(DecodificaTipoPensione tipoPensione)
            {
                this._Id = tipoPensione.Id;
                this._Descrizione = tipoPensione.Descrizione != null ? tipoPensione.Descrizione.Trim() : "";
            }

            #region public properties
            public char Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private char _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceAzienda
        {
            public CodiceAzienda(DecodificaCodiceAzienda codiceAzienda)
            {
                this._Id = codiceAzienda.Id;
                this._TraduzioneGp = codiceAzienda.TraduzioneGp;
                this._Descrizione = codiceAzienda.Descrizione != null ? codiceAzienda.Descrizione.Trim() : "";
                this._Fondo = codiceAzienda.Fondo;

            }

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public string TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public properties

            #region private properties
            private long _Id;
            private string _TraduzioneGp;
            private string _Descrizione;
            private string _Fondo;
            #endregion private properties
        }

        public class GradoInvalidita
        {
            public GradoInvalidita(DecodificaGradoInvalidita gradoInvalidita)
            {
                this._Id = gradoInvalidita.Id.ToString();
                this._Descrizione = gradoInvalidita.Descrizione != null ? gradoInvalidita.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ProrataEnel
        {
            public ProrataEnel(DecodificaProrataEnel prorataEnel)
            {
                this._Id = prorataEnel.Id.ToString();
                this._Descrizione = prorataEnel.Descrizione != null ? prorataEnel.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ComunicazioneCampi1_2
        {
            public ComunicazioneCampi1_2(DecodificaComunicazioneCampo12 comunicazioneCampo12)
            {
                this._Campo1 = comunicazioneCampo12.Campo1;
                this._Campo2 = comunicazioneCampo12.Campo2;
                this._Descrizione = comunicazioneCampo12.Descrizione != null ? comunicazioneCampo12.Descrizione.Trim() : "";
                this.Tipologia = comunicazioneCampo12.Tipologia;
            }

            #region public properties
            public System.Nullable<byte> Campo1 { get { return _Campo1; } set { _Campo1 = value; } }

            public System.Nullable<char> Campo2 { get { return _Campo2; } set { _Campo2 = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }
            #endregion public properties

            #region private properties
            private System.Nullable<byte> _Campo1;

            private System.Nullable<char> _Campo2;

            private string _Descrizione;

            private string _Tipologia;
            #endregion private properties
        }

        public class ComunicazioneCampo3
        {
            public ComunicazioneCampo3(DecodificaComunicazioneCampo3 comunicazioneCampo3)
            {
                this._Id = comunicazioneCampo3.Id.ToString();
                this._Descrizione = comunicazioneCampo3.Descrizione != null ? comunicazioneCampo3.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ComunicazioneCampo4
        {
            public ComunicazioneCampo4(DecodificaComunicazioneCampo4 comunicazioneCampo4)
            {
                this._Id = comunicazioneCampo4.Id.ToString();
                this._Descrizione = comunicazioneCampo4.Descrizione != null ? comunicazioneCampo4.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiciNatura
        {
            public CodiciNatura(DecodificaCodiciNatura codiciNatura)
            {
                this._TraduzioneSuGP = codiciNatura.TraduzioneSuGP;
                this._Posizione = codiciNatura.Posizione;
                this._Descrizione = codiciNatura.Descrizione != null ? codiciNatura.Descrizione.Trim() : "";
                this._Tipologia = codiciNatura.Tipologia != null ? codiciNatura.Tipologia.Trim() : "";
                this._Fondo = codiciNatura.Fondo != null ? codiciNatura.Fondo.Trim() : "";
            }

            #region public properties

            public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

            public System.Nullable<byte> Posizione { get { return _Posizione; } set { _Posizione = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public properties

            #region private properties
            private System.Nullable<char> _TraduzioneSuGP;

            private System.Nullable<byte> _Posizione;

            private string _Descrizione;

            private string _Tipologia;

            private string _Fondo;
            #endregion private properties
        }

        public class Cieco
        {
            public Cieco(DecodificaCodiceCieco cieco)
            {
                this._Id = cieco.Id.ToString();
                this._Descrizione = cieco.Descrizione != null ? cieco.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class SettimaneBeneficio
        {
            public SettimaneBeneficio(DecodificaTipoBeneficio settimaneBeneficio)
            {
                this._Id = settimaneBeneficio.Id;
                this._Descrizione = settimaneBeneficio.Descrizione != null ? settimaneBeneficio.Descrizione.Trim() : "";
            }

            public SettimaneBeneficio(DecodificaTipoBeneficioAGO_CI settimaneBeneficioAGO_CI)
            {
                this._Id = settimaneBeneficioAGO_CI.Id;
                this._Descrizione = settimaneBeneficioAGO_CI.Descrizione != null ? settimaneBeneficioAGO_CI.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class TipoSupplementi
        {
            public TipoSupplementi(DecodificaGestioneSupplementi tipoSupplementi)
            {
                this._TraduzioneSuGP = tipoSupplementi.TraduzioneSuGP;
                this._Descrizione = tipoSupplementi.Descrizione != null ? tipoSupplementi.Descrizione.Trim() : "";
                this._Tipologia = tipoSupplementi.Tipologia != null ? tipoSupplementi.Tipologia.Trim() : "";
                this._Fondo = tipoSupplementi.Fondo != null ? tipoSupplementi.Fondo.Trim() : "";
            }

            #region public properties
            public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public properties

            #region private properties
            private System.Nullable<char> _TraduzioneSuGP;

            private string _Descrizione;

            private string _Tipologia;

            private string _Fondo;
            #endregion private properties
        }

        public class Mobilita
        {
            public Mobilita(DecodificaCodiceMobilita mobilita)
            {
                this._Id = mobilita.Id.ToString();
                this._Descrizione = mobilita.Descrizione != null ? mobilita.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceRequisitoParticolare
        {
            public CodiceRequisitoParticolare(DecodificaCodiciRequisitiParticolari codiceRequisitoParticolare)
            {
                this._Id = codiceRequisitoParticolare.Id.ToString();
                this._Descrizione = codiceRequisitoParticolare.Descrizione != null ? codiceRequisitoParticolare.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceRequisito1
        {
            public CodiceRequisito1(DecodificaCodiceRequisito1 codiceRequisito1)
            {
                this._Id = codiceRequisito1.Id.ToString();
                this._Descrizione = codiceRequisito1.Descrizione != null ? codiceRequisito1.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceRequisito2
        {
            public CodiceRequisito2(DecodificaCodiceRequisito2 codiceRequisito2)
            {
                this.Id = codiceRequisito2.Id;
                this.Descrizione = !string.IsNullOrEmpty(codiceRequisito2.Descrizione) ? codiceRequisito2.Descrizione.Trim() : string.Empty;
            }

            #region public properties
            public char Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class CodiceSpecifico
        {
            public CodiceSpecifico(DecodificaCodiceSpecifico codiceSpecifico)
            {
                this._Id = codiceSpecifico.Id;
                this.TraduzioneGp = codiceSpecifico.TraduzioneGp;
                this._Descrizione = codiceSpecifico.Descrizione != null ? codiceSpecifico.Descrizione.Trim() : "";
                this._TipoPensione = codiceSpecifico.TipoPensione;
                this._TipoSelezionabile = codiceSpecifico.TipoSelezionabile;
                this._Fondo = codiceSpecifico.Fondo;
                this._EnteFondo = codiceSpecifico.EnteFondo;
            }

            #region public properties

            public byte? Id { get { return _Id; } set { _Id = value; } }
            public char? TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
            public byte? TipoSelezionabile { get { return _TipoSelezionabile; } set { _TipoSelezionabile = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            public char? EnteFondo { get { return _EnteFondo; } set { _EnteFondo = value; } }

            #endregion public properties

            #region private properties

            private byte? _Id;
            private char? _TraduzioneGp;
            private string _Descrizione;
            private char? _TipoPensione;
            private byte? _TipoSelezionabile;
            private string _Fondo;
            private char? _EnteFondo;
            #endregion private properties
        }

        public class CodiceConvenzioneInternazionale
        {
            public CodiceConvenzioneInternazionale(DecodificaCodiceConvenzioneInternazionale codiceConvenzioneInternazionale)
            {
                this._Id = codiceConvenzioneInternazionale.Id.ToString();
                this._Descrizione = codiceConvenzioneInternazionale.Descrizione != null ? codiceConvenzioneInternazionale.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceRequisitiLegge50392
        {
            public CodiceRequisitiLegge50392(DecodificaCodiceRequisitiLegge50392 codiceRequisitiLegge50392)
            {
                this._Id = codiceRequisitiLegge50392.Id.ToString();
                this._Descrizione = codiceRequisitiLegge50392.Descrizione != null ? codiceRequisitiLegge50392.Descrizione.Trim() : "";
                this._TraduzioneSuGP = codiceRequisitiLegge50392.TraduzioneSuGP != null ? codiceRequisitiLegge50392.TraduzioneSuGP : (char?)null;
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public char? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;

            private char? _TraduzioneSuGP;
            #endregion private properties
        }

        public class CodiceConvenzione
        {
            public CodiceConvenzione(DecodificaCodiceConvenzione codiceConvenzione)
            {
                this._Id = codiceConvenzione.Id.ToString();
                this._Descrizione = codiceConvenzione.Descrizione != null ? codiceConvenzione.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodiceVirtuale
        {
            public CodiceVirtuale(DecodificaCodiceVirtuale codiceVirtuale)
            {
                this._Id = codiceVirtuale.Id.ToString();
                this._Descrizione = codiceVirtuale.Descrizione != null ? codiceVirtuale.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class RegimeLiquidazione
        {
            public RegimeLiquidazione(DecodificaRegimeLiquidazione regimeLiquidazione)
            {
                this._Id = regimeLiquidazione.Id.ToString();
                this._Descrizione = regimeLiquidazione.Descrizione != null ? regimeLiquidazione.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class ImportoAltraPensione
        {
            public ImportoAltraPensione(DecodificaCodiceImportoAltraPensione importoAltraPensione)
            {
                this._Id = importoAltraPensione.Id.ToString();
                this._Descrizione = importoAltraPensione.Descrizione != null ? importoAltraPensione.Descrizione.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Id;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodMaggiorazioneFamiliari
        {
            public CodMaggiorazioneFamiliari(DecodificaCodMaggFamiliari codMaggiorazioneFamiliari)
            {
                this._Id = codMaggiorazioneFamiliari.Id.ToString();
                this._CampoVideo = codMaggiorazioneFamiliari.CampoVideo != null ? codMaggiorazioneFamiliari.CampoVideo.Trim() : "";
                this._Descrizione = codMaggiorazioneFamiliari.Descrizione != null ? codMaggiorazioneFamiliari.Descrizione.Trim() : "";
                this._TipoAppartenenza = codMaggiorazioneFamiliari.TipoAppartenenza != null ? codMaggiorazioneFamiliari.TipoAppartenenza.Trim() : "";
            }

            #region public properties
            public string Id { get { return _Id; } set { _Id = value; } }

            public string CampoVideo { get { return _CampoVideo; } set { _CampoVideo = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string TipoAppartenenza { get { return _TipoAppartenenza; } set { _TipoAppartenenza = value; } }

            #endregion public properties

            #region private properties
            private string _Id;

            private string _CampoVideo;

            private string _Descrizione;

            private string _TipoAppartenenza;
            #endregion private properties
        }

        public class CodeGestioneCalcoloRetributivo
        {
            public CodeGestioneCalcoloRetributivo(DecodificaGestioneCalcoloRetributivo codeGestioneCalcoloRetributivo)
            {
                this._Id = codeGestioneCalcoloRetributivo.Id;
                this._Descrizione = codeGestioneCalcoloRetributivo.Descrizione;
                this._TraduzioneSuGP = codeGestioneCalcoloRetributivo.TraduzioneSuGP;
                this._IsFondo = codeGestioneCalcoloRetributivo.IsFondo;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private string _TraduzioneSuGP;
            private bool _IsFondo;


            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public bool IsFondo { get { return _IsFondo; } set { _IsFondo = value; } }

            #endregion public properties
        }

        public class CodeGestioneCalcoloContributivo
        {
            public CodeGestioneCalcoloContributivo(DecodificaGestioneCalcoloContributivo codeGestioneCalcoloContributivo)
            {
                this._Id = codeGestioneCalcoloContributivo.Id;
                this._Descrizione = codeGestioneCalcoloContributivo.Descrizione;
                this._TraduzioneSuGP = codeGestioneCalcoloContributivo.TraduzioneSuGP;
                this._IsFondo = codeGestioneCalcoloContributivo.IsFondo;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private string _TraduzioneSuGP;
            private bool _IsFondo;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public bool IsFondo { get { return _IsFondo; } set { _IsFondo = value; } }

            #endregion public properties
        }

        public class CodeGestioneQuotaFondoIntegrativo
        {
            public CodeGestioneQuotaFondoIntegrativo(DecodificaGestioneQuotaFondoIntegrativo codeGestioneQuotaFondoIntegrativo)
            {
                this._Id = codeGestioneQuotaFondoIntegrativo.Id;
                this._Descrizione = codeGestioneQuotaFondoIntegrativo.Descrizione;
                this._TraduzioneSuGP = codeGestioneQuotaFondoIntegrativo.TraduzioneSuGP;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private string _TraduzioneSuGP;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

            #endregion public properties
        }

        public class CodeGestioneQuotaFondoINPGI
        {
            public CodeGestioneQuotaFondoINPGI(DecodificaGestioneQuotaFondoINPGI codeGestioneQuotaFondoINPGI)
            {
                this._Id = codeGestioneQuotaFondoINPGI.Id;
                this._PeriodoDal = codeGestioneQuotaFondoINPGI.PeriodoDal;
                this._PeriodoAl = codeGestioneQuotaFondoINPGI.PeriodoAl;
                this._TipoQuota = codeGestioneQuotaFondoINPGI.TipoQuota.ToString();
                this._TraduzioneSuGP = codeGestioneQuotaFondoINPGI.TraduzioneSuGP;
                this._Descrizione = codeGestioneQuotaFondoINPGI.Descrizione;
            }

            #region private properties

            private long _Id;
            private DateTime? _PeriodoDal;
            private DateTime? _PeriodoAl;
            private string _TipoQuota;
            private string _TraduzioneSuGP;
            private string _Descrizione;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public DateTime? PeriodoDal { get { return _PeriodoDal; } set { _PeriodoDal = value; } }
            public DateTime? PeriodoAl { get { return _PeriodoAl; } set { _PeriodoAl = value; } }
            public string TipoQuota { get { return _TipoQuota; } set { _TipoQuota = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties
        }

        public class ErroreCalcoloCi
        {
            public ErroreCalcoloCi(ErroriCalcoloCi erroriCalcoloCi)
            {
                this._Codice = erroriCalcoloCi.CodErrore != null ? erroriCalcoloCi.CodErrore.Trim() : "";
                this._Descrizione = erroriCalcoloCi.DescErrore != null ? erroriCalcoloCi.DescErrore.Trim() : "";
            }

            #region public properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private string _Codice;

            private string _Descrizione;
            #endregion private properties
        }

        public class CodeGestione
        {
            public CodeGestione(DecodificaCodiceGestione codeGestione)
            {
                this._Id = codeGestione.Id;
                this._Descrizione = codeGestione.Descrizione;
                this._TraduzioneSuGP = codeGestione.TraduzioneSuGp;
                this._Legge = codeGestione.Legge;
            }

            #region private properties

            private long _Id;
            private string _Descrizione;
            private short? _TraduzioneSuGP;
            private string _Legge;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public short? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public string Legge { get { return _Legge; } set { _Legge = value; } }

            #endregion public properties
        }

        public class CodiceParticolare
        {
            public CodiceParticolare(DecodificaCodiciParticolari codiceParticolare)
            {
                this._Id = codiceParticolare.Id;
                this._Descrizione = codiceParticolare.Descrizione != null ? codiceParticolare.Descrizione.Trim() : "";
                this._TraduzioneSuGp = codiceParticolare.TraduzioneSuGP;
                this._CodCategoria = codiceParticolare.CodCategoria != null ? codiceParticolare.CodCategoria.Trim() : "";
            }

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public char? TraduzioneSuGp { get { return _TraduzioneSuGp; } set { _TraduzioneSuGp = value; } }

            public string CodCategoria { get { return _CodCategoria; } set { _CodCategoria = value; } }
            #endregion public properties

            #region private properties
            private long _Id;

            private string _Descrizione;

            private char? _TraduzioneSuGp;

            private string _CodCategoria;
            #endregion private properties
        }

        public class PensioneExInpdai
        {
            public PensioneExInpdai(DecodificaPensioneExInpdai pensioneExInpdai)
            {
                this._Id = pensioneExInpdai.Id;
                this._Descrizione = pensioneExInpdai.Descrizione != null ? pensioneExInpdai.Descrizione.Trim() : "";
                this._TraduzioneSuGp = pensioneExInpdai.TraduzioneSuGP;
                this._Categoria094 = pensioneExInpdai.Categoria094;
            }

            #region public properties
            public byte Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public byte? TraduzioneSuGp { get { return _TraduzioneSuGp; } set { _TraduzioneSuGp = value; } }

            private System.Nullable<bool> Categoria094 { get { return _Categoria094; } set { _Categoria094 = value; } }
            #endregion public properties

            #region private properties
            private byte _Id;

            private string _Descrizione;

            private byte? _TraduzioneSuGp;

            private System.Nullable<bool> _Categoria094;
            #endregion private properties
        }

        public class CDCMMR
        {
            public CDCMMR(DecodificaCDCMMR codiceCDCMMR)
            {
                this._Id = codiceCDCMMR.Id;
                this._Descrizione = codiceCDCMMR.Descrizione != null ? codiceCDCMMR.Descrizione.Trim() : "";
            }

            #region public properties
            public byte Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties

            #region private properties
            private byte _Id;

            private string _Descrizione;
            #endregion private properties
        }



        public class CodiceMaggiorazioneExCombattenti
        {
            public CodiceMaggiorazioneExCombattenti(DecodificaMaggiorazioneExCombattenti codiceMaggiorazioneExCombattenti)
            {
                this._Id = codiceMaggiorazioneExCombattenti.Id;
                this._Descrizione = codiceMaggiorazioneExCombattenti.Descrizione;
                this._TraduzioneSuGP = codiceMaggiorazioneExCombattenti.TraduzioneSuGP;
            }

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }

            #endregion public properties

            #region private properties
            private long _Id;

            private string _Descrizione;

            private string _TraduzioneSuGP;

            #endregion private properties
        }

        public class GestioneCodiceTipoRichiesta
        {
            public GestioneCodiceTipoRichiesta(DecTipoRichiesta codiceGestioneTipoRichiesta)
            {
                this._CodTipoRichiesta = codiceGestioneTipoRichiesta.CodTipoRichiesta;
                this._DescTipoRichiesta = codiceGestioneTipoRichiesta.DescTipoRichiesta;
                this._Filtro = codiceGestioneTipoRichiesta.Filtro;
            }

            #region public properties

            public string CodTipoRichiesta { get { return _CodTipoRichiesta; } set { _CodTipoRichiesta = value; } }
            public string DescTipoRichiesta { get { return _DescTipoRichiesta; } set { _DescTipoRichiesta = value; } }
            public string Filtro { get { return _Filtro; } set { _Filtro = value; } }
            #endregion public properties

            #region private properties


            private string _CodTipoRichiesta;
            private string _DescTipoRichiesta;
            private string _Filtro;
            #endregion private properties
        }

        public class TipoCalcoloSecondario
        {
            public TipoCalcoloSecondario(DecodificaTipoCalcoloSecondario tipoCalcoloSecondario)
            {
                this.Id = tipoCalcoloSecondario.Id;
                this.Descrizione = tipoCalcoloSecondario.Descrizione;
                this.TraduzioneSuGP = tipoCalcoloSecondario.TraduzioneSuGP;
                this.IdTipoCalcolo = tipoCalcoloSecondario.IdTipoCalcolo;
                this.Gruppo = tipoCalcoloSecondario.Gruppo;
                this.Prodotto = tipoCalcoloSecondario.Prodotto;
                this.Tipo = tipoCalcoloSecondario.Tipo;
            }

            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public byte? TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public byte? IdTipoCalcolo { get { return _IdTipoCalcolo; } set { _IdTipoCalcolo = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }

            #endregion public properties

            #region private properties

            private byte _Id;
            private string _Descrizione;
            private byte? _TraduzioneSuGP;
            private byte? _IdTipoCalcolo;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;

            #endregion private properties
        }

        public class GruppoOneri
        {
            public GruppoOneri(DecCodeGruppoOneri decCodeGruppoOneri)
            {
                this.Id = decCodeGruppoOneri.Id;
                this.Code = decCodeGruppoOneri.Code;
                this.Descrizione = decCodeGruppoOneri.Descrizione;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Code { get { return _Code; } set { _Code = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private string _Code;
            private string _Descrizione;

            #endregion private properties
        }

        public class SottoGruppoOneri
        {
            public SottoGruppoOneri(DecCodeSottoGruppoOneri decCodeSottoGruppoOneri)
            {
                this.Id = decCodeSottoGruppoOneri.Id;
                this._IdOnere = decCodeSottoGruppoOneri.IdGruppoOneri;
                this.Code = decCodeSottoGruppoOneri.Code;
                this.Descrizione = decCodeSottoGruppoOneri.Descrizione;
                this.IsPubblica = decCodeSottoGruppoOneri.IsPubblica;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdOnere { get { return _IdOnere; } set { _IdOnere = value; } }
            public string Code { get { return _Code; } set { _Code = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public bool? IsPubblica { get { return _IsPubblica; } set { _IsPubblica = value; } }
            #endregion public properties

            #region private properties

            private long _Id;
            private long _IdOnere;
            private string _Code;
            private string _Descrizione;
            private bool? _IsPubblica;

            #endregion private properties
        }


        public class DomandaRicorso
        {
            public DomandaRicorso(DecodificaDomandaRicorso decodificaDomandaRicorso)
            {
                this.Id = decodificaDomandaRicorso.Id;
                this.Descrizione = decodificaDomandaRicorso.Descrizione;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecodificaLegge44997
        {
            public DecodificaLegge44997(Liquidazione.DataCommon.DecodificaLegge44997 decodificaLegge44997)
            {
                this.Id = decodificaLegge44997.Id;
                this.Descrizione = decodificaLegge44997.Descrizione;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecModalitaLiquidazione
        {
            public DecModalitaLiquidazione(Liquidazione.DataCommon.DecodificaModalitaLiquidazione decModalitaLiquidazione)
            {
                this.ValoreAggPeco = decModalitaLiquidazione.ValoreAggPeco;
                this.TraduzioneGp = decModalitaLiquidazione.TraduzioneGp;
                this.Descrizione = decModalitaLiquidazione.Descrizione;
            }

            #region public properties

            public string ValoreAggPeco { get { return _ValoreAggPeco; } set { _ValoreAggPeco = value; } }
            public char TraduzioneGp { get { return _TraduzioneGp; } set { _TraduzioneGp = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private string _ValoreAggPeco;
            private char _TraduzioneGp;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecOpzioneRiliquidazione
        {
            public DecOpzioneRiliquidazione(Liquidazione.DataCommon.DecodificaOpzioneRiliquidazione decOpzioneRiliquidazione)
            {
                this.Id = decOpzioneRiliquidazione.Id;
                this.Descrizione = decOpzioneRiliquidazione.Descrizione;
                
            }

            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private byte _Id;
            private string _Descrizione;
            

            #endregion private properties
        }

        public class DecCodiceCi28
        {
            public DecCodiceCi28(Liquidazione.DataCommon.DecodificaCodiceCi28 decCodiceCi28)
            {
                this.Codice = decCodiceCi28.Codice;
                this.Descrizione = decCodiceCi28.Descrizione;
            }

            #region public properties

            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private string _Codice;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecCodiceCi21
        {
            public DecCodiceCi21(Liquidazione.DataCommon.DecodificaCodiceCi21 decCodiceCi21)
            {
                this.Codice = decCodiceCi21.Codice;
                this.Descrizione = decCodiceCi21.Descrizione;
            }

            #region public properties

            public char Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private char _Codice;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecodificaCodeEsodo
        {
            public DecodificaCodeEsodo(Liquidazione.DataCommon.DecodificaCodiceEsodo decodificaCodiceEsodo)
            {
                this.Codice = decodificaCodiceEsodo.Valore;
                this.Descrizione = decodificaCodiceEsodo.Descrizione;
            }

            #region public properties

            public bool Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private bool _Codice;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecodificaPartTime
        {
            public DecodificaPartTime(Liquidazione.DataCommon.DecodificaPartTime decodificaPartTime)
            {
                this.Codice = decodificaPartTime.Valore;
                this.Descrizione = decodificaPartTime.Descrizione;
            }

            #region public properties

            public bool Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private bool _Codice;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecodificaCodiceArt22
        {
            public DecodificaCodiceArt22(Liquidazione.DataCommon.DecodificaArt22 decodificaArt22)
            {
                this.Id = decodificaArt22.Id;
                this.Descrizione = decodificaArt22.Descrizione;
                this.TipoSelezionabile = decodificaArt22.TipoSelezionabile;
                this.Fondo = decodificaArt22.Fondo;
            }

            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public byte? TipoSelezionabile { get { return _TipoSelezionabile; } set { _TipoSelezionabile = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            #endregion public properties

            #region private properties

            private byte _Id;
            private string _Descrizione;
            private byte? _TipoSelezionabile;
            private string _Fondo;

            #endregion private properties
        }

        public class DecodificaCodiceCapitalizzazione
        {
            public DecodificaCodiceCapitalizzazione(Liquidazione.DataCommon.DecodificaCodiceCapitalizzazione decodificaCodiceCapitalizzazione)
            {
                this.Codice = decodificaCodiceCapitalizzazione.Id;
                this.Descrizione = decodificaCodiceCapitalizzazione.Descrizione;
            }

            #region public properties

            public byte Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties

            private byte _Codice;
            private string _Descrizione;

            #endregion private properties
        }

        public class DecodificaCausaCessazione
        {
            public DecodificaCausaCessazione(Liquidazione.DataCommon.DecodificaCausaCessazione decodificaCausaCessazione)
            {
                this.Id = decodificaCausaCessazione.Id;
                this.TipoPensione = decodificaCausaCessazione.TipoPensione;
                this.TraduzioneSuGP = decodificaCausaCessazione.TraduzioneSuGP;
                this.InizioValidita = decodificaCausaCessazione.InizioValidita;
                this.FineValidita = decodificaCausaCessazione.FineValidita;
                this.Descrizione = decodificaCausaCessazione.Descrizione;
                this.Fondo = decodificaCausaCessazione.Fondo;
            }

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }
            public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public DateTime? InizioValidita { get { return _InizioValidita; } set { _InizioValidita = value; } }
            public DateTime? FineValidita { get { return _FineValidita; } set { _FineValidita = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            #endregion public properties

            #region private properties

            private long _Id;
            private char? _TipoPensione;
            private string _TraduzioneSuGP;
            private DateTime? _InizioValidita;
            private DateTime? _FineValidita;
            private string _Descrizione;
            private string _Fondo;

            #endregion private properties
        }

        public class DecPensioniPrivilegiate
        {
            #region public properties

            public int Id { get { return _Id; } set { _Id = value; } }
            public System.Nullable<char> TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public System.Nullable<byte> Posizione { get { return _Posizione; } set { _Posizione = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }

            #endregion public properties

            #region private properties
            private int _Id;
            private System.Nullable<char> _TraduzioneSuGP;
            private System.Nullable<byte> _Posizione;
            private string _Descrizione;
            private string _Fondo;
            #endregion private properties
        }

        public class DecRiconoscimentiInvalidita
        {
            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties
            private byte _Id;
            private string _Descrizione;
            #endregion private properties
        }

        public class DecCassaSede
        {
            #region public properties

            public long? Cab { get { return _Cab; } set { _Cab = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }

            #endregion public properties

            #region private properties
            private long? _Cab;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private string _TipoApp;
            #endregion private properties
        }

        public class DecodeEnte
        {
            #region public properties

            public byte Id { get { return _Id; } set { _Id = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }

            #endregion public properties

            #region private properties
            private byte _Id;
            private string _Descrizione;
            #endregion private properties
        }

        public class CatEnteAltraPensione
        {
            public CatEnteAltraPensione(DecCatEnteAltrePensioni catEnte)
            {
                _CodCategoria = catEnte.CodCategoria;
                _CodEnte = catEnte.CodEnte;
                _TipoApp = catEnte.TipoApp;
            }

            #region private fields
            private string _CodCategoria;
            private char _CodEnte;
            private string _TipoApp;
            #endregion private fields

            #region public properties
            public string CodCategoria { get { return _CodCategoria; } set { _CodCategoria = value; } }
            public char CodEnte { get { return _CodEnte; } set { _CodEnte = value; } }
            public string TipoApp { get { return _TipoApp; } set { _TipoApp = value; } }
            #endregion public properties
        }

        public class Gruppo
        {
            #region private properties
            private string _CodGruppo;
            private string _DescGruppo;
            #endregion private properties

            #region public properties

            public string CodGruppo
            {
                get { return _CodGruppo; }
                set { _CodGruppo = value; }
            }

            public string DescGruppo
            {
                get { return _DescGruppo; }
                set { _DescGruppo = value; }
            }

            #endregion public properties
        }

        public class Prodotto
        {
            #region private properties
            private string _CodProdotto;
            private string _DescProdotto;
            private string _DescEstesaProdotto;
            private char _TipoCatPensione;
            private string _TipoStampaReiezione;
            #endregion private properties

            #region public properties

            public string CodProdotto
            {
                get { return _CodProdotto; }
                set { _CodProdotto = value; }
            }

            public string DescProdotto
            {
                get { return _DescProdotto; }
                set { _DescProdotto = value; }
            }

            public string DescEstesaProdotto
            {
                get { return _DescEstesaProdotto; }
                set { _DescEstesaProdotto = value; }
            }

            public char TipoCatPensione
            {
                get { return _TipoCatPensione; }
                set { _TipoCatPensione = value; }
            }

            public string TipoStampaReiezione
            {
                get { return _TipoStampaReiezione; }
                set { _TipoStampaReiezione = value; }
            }

            #endregion public properties
        }

        public class Tipo
        {
            #region private properties
            private string _CodTipo;
            private string _DescTipo;
            private string _DescEstesaTipo;
            #endregion private properties

            #region public properties

            public string CodTipo
            {
                get { return _CodTipo; }
                set { _CodTipo = value; }
            }

            public string DescTipo
            {
                get { return _DescTipo; }
                set { _DescTipo = value; }
            }

            public string DescEstesaTipo
            {
                get { return _DescEstesaTipo; }
                set { _DescEstesaTipo = value; }
            }

            #endregion public properties

        }

        public class Filtro
        {
            #region private properties
            private string _CodFiltro;
            private string _DescFiltro;
            #endregion private properties

            #region public properties
            public string Codice
            {
                get { return _CodFiltro; }
                set { _CodFiltro = value; }
            }

            public string Descrizione
            {
                get { return _DescFiltro; }
                set { _DescFiltro = value; }
            }

            #endregion public properties

        }

        public class UfficiPagatoriEsteri
        {
            #region private properties
            private int _Id;
            private string _Descrizione;
            private int? _CodiceStato;
            private int? _CodiceIstituzione;
            #endregion private properties

            #region public properties
            public int Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }

            public int? CodiceStato
            {
                get { return _CodiceStato; }
                set { _CodiceStato = value; }
            }

            public int? CodiceIstituzione
            {
                get { return _CodiceIstituzione; }
                set { _CodiceIstituzione = value; }
            }

            #endregion public properties

        }

        public class TipoPensioneFondi
        {
            #region private properties
            private long _Id;
            private string _Gruppo;
            private string _Prodotto;
            private string _Tipo;
            private System.Nullable<char> _CodiceSpecifico;
            private System.Nullable<short> _TipoPensione;
            private string _Fondo;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public string Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public System.Nullable<char> CodiceSpecifico { get { return _CodiceSpecifico; } set { _CodiceSpecifico = value; } }
            public System.Nullable<short> TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }
            public string Fondo { get { return _Fondo; } set { _Fondo = value; } }
            #endregion public properties
        }

        public class DerogaENPALS
        {
            #region private properties
            private string _Codice;
            private string _Descrizione;
            #endregion private properties

            #region public properties
            public string Codice { get { return _Codice; } set { _Codice = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            #endregion public properties
        }

        public class DecArt58
        {
            #region private properties

            private byte _Id;
            private string _Descrizione;
            private string _Fondo;

            #endregion private properties

            #region public properties
            public byte Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }

            public string Fondo
            {
                get { return _Fondo; }
                set { _Fondo = value; }
            }

            #endregion public properties
        }

        public class DecPromiscui
        {
            #region private properties

            private byte _Id;
            private string _Descrizione;
            private string _Fondo;

            #endregion private properties

            #region public properties
            public byte Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public string Descrizione
            {
                get { return _Descrizione; }
                set { _Descrizione = value; }
            }

            public string Fondo
            {
                get { return _Fondo; }
                set { _Fondo = value; }
            }

            #endregion public properties
        }

        public class DecodificaTipoLiquidazionePM
        {
            #region public properties
            public byte Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class DecodificaLegge413
        {
            #region public properties
            public char Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class DecodificaAttivitaSvolta2
        {
            #region public properties
            public char Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class DecodificaTipoLiquidazione
        {
            #region public properties
            public byte Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class CodiceTipoLiquidazionePM
        {
            #region public properties
            public byte Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class TipologiaFAQ
        {
            #region public properties
            public string Codice { get; set; }
            public string Descrizione { get; set; }
            public int? Contatore { get; set; }
            #endregion public properties
        }

        public class DecodificaTipoLiquidazioneGAS
        {
            #region public properties
            public byte Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }

        public class DecodificaTipoLiquidazionePI
        {
            #region public properties
            public byte Id { get; set; }
            public string Descrizione { get; set; }
            #endregion public properties
        }
        public class DecodificaTipoQuota
        {
            public string Codice { get; set; }
            public string Decodifica { get; set; }
        }

        public class DecodificaEnteCassaProfessionale
        {
            public long Id { get; set; }
            public string TraduzioneSuGP { get; set; }
            public string Descrizione { get; set; }
        }

        public class DecEnteGestioneFondo
        {
            public long Id { get; set; }
            public string Codice { get; set; }
            public short? CodiceFondo { get; set; }
            public string Ente { get; set; }
            public string Tipologia { get; set; }
            public bool? IsTrattenuteAmmesse { get; set; }
        }

        public class DecCodiceTrattenute
        {
            public string CodiceTrattenute { get; set; }
            public string CodiceEnteGestioneFondo { get; set; }
        }

        public class DecPersonaleViaggiante
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
            public byte? TraduzioneSuGP { get; set; }
        }

        public class AttCon
        {
            public char Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class SoggettoBeneficiario
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
            public string TraduzioneSuGP { get; set; }
        }

        public class TipologiaPrestazione
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class TipologiaBeneficioTerrorismo
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class DecSituazione
        {
            public string CodSituazione { get; set; }
            public string DescSituazione { get; set; }
        }


        public class DecodificaEnteRipartizioneINPDAP
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class DecodificaInteresseLegale
        {
            public long Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class CtrlNoInvioMailDirettore
        {
            public long Id { get; set; }
            public short CodCatPensione { get; set; }
            public string SiglaCatPensione { get; set; }
        }

        public class DecTipoCalcoloVincenteDAI
        {
            public char Id { get; set; }
            public string Descrizione { get; set; }
        }

        public class CtrlRequisitoEta
        {
            public long Id { get; set; }
            public DateTime InizioPeriodoPerfRequisiti { get; set; }
            public DateTime FinePeriodoPerfRequisiti { get; set; }
            public char? Sesso { get; set; }
            public string Categoria { get; set; }
            public byte? RequisitoAA { get; set; }
            public byte? RequisitoMM { get; set; }
            public string TipoAppartenenza { get; set; }
            public string CodTipo { get; set; }
        }

        public class CtrlRicercaGPT
        {
            public string Codice { get; set; }
            public char GPT { get; set; }
        }

        public class DecMicroqualificaINPDAP
        {
            public long Id { get; set; }
            public string TraduzioneSuGP { get; set; }
            public string Descrizione { get; set; }
        }

        public class CtrlEnteCassaCodiceGestione
        {

            public string CodiceCategoria { get; set; }
            public string TraduzioneSuGP { get; set; }
            public string CodiciGestione { get; set; }
            public string Professione { get; set; }
        }

        public partial class CtrlCatAdeguata
        {
            public string CodCategoria { get; set; }
            public string CodGruppo { get; set; }
            public string CodProdotto { get; set; }
            public string CodTipo { get; set; }
            public bool? IsTrasfRic { get; set; }
            public DateTime? DataInizio { get; set; }
            public DateTime? DataFine { get; set; }
        }


        public partial class CtrlTipoUfficio
        {
            public string CodTipoUfficio { get; set; }
            public string DescTipoUfficio { get; set; }
            public short TraduzioneSuGP { get; set; }
        }

        public partial class DecCapitolo
        {
            public string Capitolo { get; set; }

            public string DescrizioneCapitolo { get; set; }

            public bool? PL { get; set; }
        }

        public partial class CtrlCompartoSettoreRuolo
        {
            public string Cassa { get; set; }

            public int CodiceComparto { get; set; }

            public int CodiceSettore { get; set; }

            public int CodiceRuolo { get; set; }
        }

        public partial class DecComparto
        {
            public int Codice { get; set; }
            public string Descrizione { get; set; }
        }

        public partial class DecSettore
        {
            public int Codice { get; set; }
            public string Descrizione { get; set; }
        }

        public partial class DecRuolo
        {
            public int Codice { get; set; }
            public string Descrizione { get; set; }
        }

        public partial class DecSede
        {
            public string CodiceSedeMeta { get; set; }
            public string DescSede { get; set; }
            public string SiglaProvincia { get; set; }
            public string NomeDirettore { get; set; }
            public string CodProvincia { get; set; }
            public string CodZona { get; set; }
            public string CodCentroOperativo { get; set; }
            public string DataUltimaModifica { get; set; }
            public string CodTipoSede { get; set; }
            public string CAPSede { get; set; }
            public string IndirizzoSede { get; set; }
            public string ComuneSede { get; set; }
            public string ProvinciaSede { get; set; }
            public string IndirizzoEMail { get; set; }
            public string Codice6 { get; set; }
            public char? CodAttivitaSede { get; set; }

        }

        public partial class DecodificaBanchePerSede
        {
            public long Id { get; set; }
            public string TraduzioneSuGP { get; set; }
            public string Descrizione { get; set; }
            public string CodiceSede { get; set; }
        }

        public class CtrlScadenzaIndennizzoINDCOM
        {
            public string Tipologia { get; set; }
            public string Sesso { get; set; }
            public DateTime? DataNascitaDal { get; set; }
            public DateTime? DataNascitaAl { get; set; }
            public byte? PrepopolaAnni { get; set; }
            public byte? PrepopolaMesi { get; set; }
            public byte? PrepopolaGiorni { get; set; }
            public byte? ControlloAnni { get; set; }
            public byte? ControlloMesi { get; set; }
            public byte? ControlloGiorni { get; set; }
        }
        #endregion nested class
    }
}
