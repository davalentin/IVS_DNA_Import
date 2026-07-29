using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDecodifica
    {
        public static void GetStatiPensione(out List<DecodificaStatoPensione> statiPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    statiPensione = (from sp in db.DecodificaStatoPensiones select sp).OrderBy(x => x.Id).ToList<DecodificaStatoPensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFondiPensione(out List<DecGestioneFondo> fondiPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    fondiPensione = (from fp in db.DecGestioneFondos where fp.CodGestione == "007" select fp).ToList<DecGestioneFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCasseGDP(out List<DecGestioneFondo> fondiPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    fondiPensione = (from fp in db.DecGestioneFondos where fp.CodGestione == "019" && fp.CodFondo != "008" && fp.CodFondo != "009" select fp).ToList<DecGestioneFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCategoriePensione(out List<DecCatPensione> categoriePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    categoriePensione = (from c in db.DecCatPensiones select c).ToList<DecCatPensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        /// <summary>
        /// Tale metodo va utilizzato solo per il flusso AGO e per il flusso CI. Restituisce la sigla categoria a fronte del codice numerico
        /// E' utilizzato solo nel caso di ricostituzioni di reversibilità dirette
        /// </summary>
        /// <param name="categoriaNum"></param>
        /// <param name="siglaCategoria"></param>
        public static void AGO_CI_GetCategoriaByCategoriaNumerica(string categoriaNum, out string siglaCategoria)
        {
            siglaCategoria = string.Empty;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    List<string> listaSiglaCategoria = (from c in db.DecCatPensiones where c.CodCatPensione == (!string.IsNullOrEmpty(categoriaNum) ? categoriaNum.PadLeft(4, '0') : "")
                                      select c.SiglaCatPensione).ToList<string>();
                    if (listaSiglaCategoria != null && listaSiglaCategoria.Count > 0)
                        siglaCategoria = listaSiglaCategoria[0];
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void FS_GetFondoByCategoriaNumerica(string categoriaNum, out string tipoFondo)
        {
            tipoFondo = string.Empty;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    List<DecCatPensione> listaCategorie = (from c in db.DecCatPensiones
                                                            where c.CodCatPensione == (!string.IsNullOrEmpty(categoriaNum) ? categoriaNum.PadLeft(4, '0') : "")
                                                            select c).ToList<DecCatPensione>();
                    if (listaCategorie != null && listaCategorie.Count > 0)
                    {
                        if (listaCategorie[0].TipoCatPensione == 'C')
                            tipoFondo = listaCategorie[0].SiglaCatPensione;
                        else
                            tipoFondo = listaCategorie[0].SiglaCatPensione.Substring(1);
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodCategoriaBySiglaCategoria(string siglaCategoria, string tipoFondo, out string categoriaNum)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    siglaCategoria = !string.IsNullOrEmpty(siglaCategoria) ? siglaCategoria.Trim() : string.Empty;
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    categoriaNum = (from c in db.DecCatPensiones where c.SiglaCatPensione == siglaCategoria select c.CodCatPensione).SingleOrDefault<string>();
                    if ((String.IsNullOrEmpty(categoriaNum) || categoriaNum.Trim() == "") && !string.IsNullOrEmpty(siglaCategoria) && siglaCategoria.Length > 1)
                    {
                        categoriaNum = (from c in db.DecCatPensiones where c.SiglaCatPensione == tipoFondo select c.CodCatPensione).SingleOrDefault<string>();
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPatronatoByEnteUfficio(string codiceEnte, string codiceUfficio, out DecPatronato decPatronato)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    decPatronato = (from p in db.DecPatronatos 
                                    where p.CodInpsPatronato == codiceEnte && p.CodUfficioPatronato == codiceUfficio
                                    select p).FirstOrDefault<DecPatronato>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetParentela(out List<DecodificaParentela> elencoparenteladc)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoparenteladc = (from p in db.DecodificaParentelas select p).ToList<DecodificaParentela>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaMaggiorazione781ContributiDC(out List<DecodificaMaggiorazione781ContributiDC> elencomagg781contr)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencomagg781contr = (from p in db.DecodificaMaggiorazione781ContributiDCs select p).ToList<DecodificaMaggiorazione781ContributiDC>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceProvenienza(out List<DecodificaCodiceProvenienza> elencodecCodProvenienza)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencodecCodProvenienza = (from p in db.DecodificaCodiceProvenienzas select p).ToList<DecodificaCodiceProvenienza>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciVariDC(out List<DecodificaCodiciVariDC> elencodecCodiciVariDC)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencodecCodiciVariDC = (from p in db.DecodificaCodiciVariDCs select p).ToList<DecodificaCodiciVariDC>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatiCivili(out List<DecodificaStatoCivile> elencoStatiCivili)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoStatiCivili = (from s in db.DecodificaStatoCiviles select s).ToList<DecodificaStatoCivile>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatoPensioneById(byte statoPensione, out string decodificaStatoPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    decodificaStatoPensione = (from c in db.DecodificaStatoPensiones where c.Id == statoPensione select c.DecodificaStato).SingleOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatiEsteri(out List<DecComuneNazione> elencoStatiEsteri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    elencoStatiEsteri = null;
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    var elStatiEsteri = (from s in db.DecComuneNaziones where s.IndAttuale == '1' && s.CodCatastale.StartsWith("Z") select new { s.CodCatastale, s.DescComuneNazione, s.SiglaProvinciaNazione }).Distinct();
                    foreach (var el in elStatiEsteri)
                    {
                        if (elencoStatiEsteri == null)
                            elencoStatiEsteri = new List<DecComuneNazione>();
                        DecComuneNazione statoEstero = new DecComuneNazione();
                        statoEstero.CodCatastale = el.CodCatastale;
                        statoEstero.DescComuneNazione = el.DescComuneNazione;
                        statoEstero.SiglaProvinciaNazione = el.SiglaProvinciaNazione;
                        elencoStatiEsteri.Add(statoEstero);
                    }
                    if (elencoStatiEsteri != null)
                    {
                        
                        elencoStatiEsteri = elencoStatiEsteri.OrderBy(x => x.DescComuneNazione).ToList<DecComuneNazione>();
                        DecComuneNazione statoEstero = new DecComuneNazione();
                        statoEstero.CodCatastale = "Z000";
                        statoEstero.DescComuneNazione = "ITALIA";
                        statoEstero.SiglaProvinciaNazione = "ITA";
                        elencoStatiEsteri.Insert(0, statoEstero);
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatoEsteroByCodCatastale(string codCatastale, out DecComuneNazione statoEstero)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    statoEstero = null;
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    var stEstero = (from s in db.DecComuneNaziones where s.IndAttuale == '1' && s.CodCatastale.StartsWith("Z") && s.CodCatastale == codCatastale select new { s.CodCatastale, s.DescComuneNazione, s.SiglaProvinciaNazione }).Distinct().SingleOrDefault();
                    if (stEstero != null)
                    {
                        statoEstero = new DecComuneNazione();
                        statoEstero.CodCatastale = stEstero.CodCatastale;
                        statoEstero.DescComuneNazione = stEstero.DescComuneNazione;
                        statoEstero.SiglaProvinciaNazione = stEstero.SiglaProvinciaNazione;
                    }
                    else if (codCatastale == "Z000")
                    {
                        statoEstero = new DecComuneNazione();
                        statoEstero.CodCatastale = "Z000";
                        statoEstero.DescComuneNazione = "ITALIA";
                        statoEstero.SiglaProvinciaNazione = "ITA";
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetStatoEsteroBySigla(string sigla, out DecComuneNazione statoEstero)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    statoEstero = null;
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    var stEstero = (from s in db.DecComuneNaziones where s.IndAttuale == '1' && s.CodCatastale.StartsWith("Z") && s.SiglaProvinciaNazione == sigla.Trim() select new { s.CodCatastale, s.DescComuneNazione, s.SiglaProvinciaNazione }).Distinct().SingleOrDefault();
                    if (stEstero != null)
                    {
                        statoEstero = new DecComuneNazione();
                        statoEstero.CodCatastale = stEstero.CodCatastale;
                        statoEstero.DescComuneNazione = stEstero.DescComuneNazione;
                        statoEstero.SiglaProvinciaNazione = stEstero.SiglaProvinciaNazione;
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodCatastaleByComune_Provincia(string comune, string provincia, out string codCatastale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    codCatastale = string.Empty;
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    codCatastale = (from s in db.DecComuneNaziones
                                    where s.IndAttuale == '1' &&
                                        s.DescComuneNazione == comune.Trim() &&
                                        s.SiglaProvinciaNazione == provincia.Trim()
                                    select s.CodCatastale).SingleOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodCatastaleByCap(string cap, out string codCatastale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    codCatastale = string.Empty;
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    codCatastale = (from s in db.DecComuneNaziones
                                    where s.IndAttuale == '1' &&
                                        s.Cap == cap.Trim()
                                    select s.CodCatastale).SingleOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetProvince(out List<DecProvincia> elencoProvince)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoProvince = (from p in db.DecProvincias select p).ToList<DecProvincia>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetComuniPerProvincia(string siglaProvincia, out List<DecComuneNazione> elencoComuni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoComuni = (from c in db.DecComuneNaziones where c.IndAttuale == '1' && !c.CodCatastale.StartsWith("Z") && c.SiglaProvinciaNazione == siglaProvincia select c).ToList<DecComuneNazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetConiugeOFiglio(out List<DecodificaConiugeOFiglio> elencoConiugeOFiglio)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoConiugeOFiglio = (from d in db.DecodificaConiugeOFiglios select d).ToList<DecodificaConiugeOFiglio>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDetrazioniReddito(out List<DecodificaDetrazioniReddito> elencoDetrazioniReddito)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDetrazioniReddito = (from d in db.DecodificaDetrazioniRedditos select d).ToList<DecodificaDetrazioniReddito>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTutore(out List<DecodificaTutore> elencoTutore)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTutore = (from d in db.DecodificaTutores select d).ToList<DecodificaTutore>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDelegato(out List<DecodificaCodiceDelegato> elencoDelegato)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDelegato = (from d in db.DecodificaCodiceDelegatos select d).ToList<DecodificaCodiceDelegato>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSiglaFamiliare(string tipologia, out List<DecodificaSiglaFamiliare> elencoSiglaFamiliare)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoSiglaFamiliare = (from d in db.DecodificaSiglaFamiliares 
                                            where d.Tipologia == tipologia select d).ToList<DecodificaSiglaFamiliare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFamiliare(out List<DecodificaFamiliare> elencoFamiliare)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoFamiliare = (from d in db.DecodificaFamiliares select d).ToList<DecodificaFamiliare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetValidazioneCF(out List<DecodificaValidazioneCF> elencoValidazioneCF)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoValidazioneCF = (from d in db.DecodificaValidazioneCFs select d).ToList<DecodificaValidazioneCF>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetModalitaPagamento(out List<DecodificaModalitaPagamento> elencoModalitaPagamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoModalitaPagamento = (from d in db.DecodificaModalitaPagamentos select d).ToList<DecodificaModalitaPagamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoPagamento(out List<DecodificaTipoPagamento> elencoTipoPagamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoPagamento = (from d in db.DecodificaTipoPagamentos select d).ToList<DecodificaTipoPagamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoCalcolo(out List<DecodificaTipoCalcolo> elencoTipoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoCalcolo = (from d in db.DecodificaTipoCalcolos select d).ToList<DecodificaTipoCalcolo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCausaCarico(out List<DecodificaCausaCarico> elencoCausaCarico)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCausaCarico = (from d in db.DecodificaCausaCaricos select d).ToList<DecodificaCausaCarico>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceEliminazioneByTipologia(out List<DecodificaCodiceEliminazione> elencoCodiceEliminazione,string tipologia)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceEliminazione = (from d in db.DecodificaCodiceEliminaziones where d.Tipologia == tipologia select d).ToList<DecodificaCodiceEliminazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAttivitaSvolta(out List<DecodificaAttivitaSvolta> elencoAttivitaSvolta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoAttivitaSvolta = (from d in db.DecodificaAttivitaSvoltas select d).ToList<DecodificaAttivitaSvolta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAttivitaSvoltaByFondo(string fondo, char? enteFondo, out List<DecodificaAttivitaSvolta> elencoAttivitaSvolta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoAttivitaSvolta = (from d in db.DecodificaAttivitaSvoltas
                                            where d.Fondo == fondo && (!d.EnteFondo.HasValue || d.EnteFondo == enteFondo)
                                            select d).ToList<DecodificaAttivitaSvolta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAttivitaSvoltaById(string id, out DecodificaAttivitaSvolta attivitaSvolta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    attivitaSvolta = (from d in db.DecodificaAttivitaSvoltas
                                            where d.Id == id
                                            select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceCristallizzazione(out List<DecodificaCodiceCristallizzazione> elencoCodiceCristallizzazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceCristallizzazione = (from d in db.DecodificaCodiceCristallizzaziones select d).ToList<DecodificaCodiceCristallizzazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoPensione(out List<DecodificaTipoPensione> elencoTipoPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoPensione = (from d in db.DecodificaTipoPensiones select d).ToList<DecodificaTipoPensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceAzienda(out List<DecodificaCodiceAzienda> elencoCodiceAziendaEL)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceAziendaEL = (from d in db.DecodificaCodiceAziendas select d).ToList<DecodificaCodiceAzienda>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGradoInvalidita(out List<DecodificaGradoInvalidita> elencoGradoInvalidita)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoGradoInvalidita = (from d in db.DecodificaGradoInvaliditas select d).ToList<DecodificaGradoInvalidita>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetProrataEnel(out List<DecodificaProrataEnel> elencoProrataEnel)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoProrataEnel = (from d in db.DecodificaProrataEnels select d).ToList<DecodificaProrataEnel>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGestioneFondoInChiaroByKey(string gestione, string fondo, out string descGestione, out string descFondo)
        {
            using (new MethodExecutionTracer())
            {
                descGestione = "";
                descFondo = "";
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    var result = (from gf in db.DecGestioneFondos
                                  where gf.CodGestione == gestione && gf.CodFondo == fondo
                                  select new { gf.DescGestione, gf.DescFondo }).Distinct().SingleOrDefault();
                    if (result != null)
                    {
                        descGestione = result.DescGestione;
                        descFondo = result.DescFondo;
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetProdottoInChiaroByKey(string prodotto, out string descProdotto)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    descProdotto = (from p in db.DecProdottos
                                    where p.CodProdotto == prodotto
                                    select p.DescProdotto).Distinct().SingleOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipologiaInChiaroByKey(string tipo, out string descTipologia)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    descTipologia = (from t in db.DecTipos
                                     where t.CodTipo == tipo
                                     select t.DescTipo).Distinct().SingleOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetEnteInChiaroByKey(string ente, out string descEnte)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    descEnte = (from e in db.DecEntes
                                where e.CodEnte == ente
                                select e.DescEnte).Distinct().SingleOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetComunicazioneCampi1_2(out List<DecodificaComunicazioneCampo12> elencoComunicazioneCampo12)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoComunicazioneCampo12 = (from d in db.DecodificaComunicazioneCampo12s select d).ToList<DecodificaComunicazioneCampo12>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetComunicazioneCampo3(out List<DecodificaComunicazioneCampo3> elencoComunicazioneCampo3)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoComunicazioneCampo3 = (from d in db.DecodificaComunicazioneCampo3s select d).ToList<DecodificaComunicazioneCampo3>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetComunicazioneCampo4(out List<DecodificaComunicazioneCampo4> elencoComunicazioneCampo4)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoComunicazioneCampo4 = (from d in db.DecodificaComunicazioneCampo4s select d).ToList<DecodificaComunicazioneCampo4>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciNatura(out List<DecodificaCodiciNatura> elencoCodiciNatura)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciNatura = (from d in db.DecodificaCodiciNaturas select d).OrderBy(x => x.TraduzioneSuGP).ToList<DecodificaCodiciNatura>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceCieco(out List<DecodificaCodiceCieco> elencoCodiciCieco)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciCieco = (from d in db.DecodificaCodiceCiecos select d).ToList<DecodificaCodiceCieco>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceTipoBeneficio(out List<DecodificaTipoBeneficio> elencoCodiciBeneficio)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciBeneficio = (from d in db.DecodificaTipoBeneficios select d).ToList<DecodificaTipoBeneficio>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoSupplementi(out List<DecodificaGestioneSupplementi> elencoTipoSupplemento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoSupplemento = (from d in db.DecodificaGestioneSupplementis select d).ToList<DecodificaGestioneSupplementi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciRequisitiParticolari(out List<DecodificaCodiciRequisitiParticolari> elencoCodiciRequisitiParticolari)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciRequisitiParticolari = (from d in db.DecodificaCodiciRequisitiParticolaris select d).ToList<DecodificaCodiciRequisitiParticolari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        } 
        public static void GetCodiceMobilita(out List<DecodificaCodiceMobilita> elencoCodiciMobilita)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciMobilita = (from d in db.DecodificaCodiceMobilitas select d).ToList<DecodificaCodiceMobilita>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceRequisito1(out List<DecodificaCodiceRequisito1> elencoCodiceRequisito1)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceRequisito1 = (from d in db.DecodificaCodiceRequisito1s select d).ToList<DecodificaCodiceRequisito1>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceRequisito2(out List<DecodificaCodiceRequisito2> elencoCodiceRequisito2)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceRequisito2 = (from d in db.DecodificaCodiceRequisito2s select d).ToList<DecodificaCodiceRequisito2>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceSpecifico(out List<DecodificaCodiceSpecifico> elencoCodiceSpecifico)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceSpecifico = (from d in db.DecodificaCodiceSpecificos select d).ToList<DecodificaCodiceSpecifico>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceConvenzioneInternazionale(out List<DecodificaCodiceConvenzioneInternazionale> elencoCodiceConvenzioneInternazionale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceConvenzioneInternazionale = (from d in db.DecodificaCodiceConvenzioneInternazionales select d).ToList<DecodificaCodiceConvenzioneInternazionale>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceRequisitiLegge50392(out List<DecodificaCodiceRequisitiLegge50392> elencoCodiceRequisitiLegge50392)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceRequisitiLegge50392 = (from d in db.DecodificaCodiceRequisitiLegge50392s select d).ToList<DecodificaCodiceRequisitiLegge50392>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceConvenzione(out List<DecodificaCodiceConvenzione> elencoCodiceConvenzione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceConvenzione = (from d in db.DecodificaCodiceConvenziones select d).ToList<DecodificaCodiceConvenzione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceVirtuale(out List<DecodificaCodiceVirtuale> elencoCodiceVirtuale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceVirtuale = (from d in db.DecodificaCodiceVirtuales select d).ToList<DecodificaCodiceVirtuale>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetRegimeLiquidazione(out List<DecodificaRegimeLiquidazione> elencoRegimeLiquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoRegimeLiquidazione = (from d in db.DecodificaRegimeLiquidaziones select d).ToList<DecodificaRegimeLiquidazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceImportoAltraPensione(out List<DecodificaCodiceImportoAltraPensione> elencoCodiceImportoAltraPensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceImportoAltraPensione = (from d in db.DecodificaCodiceImportoAltraPensiones select d).ToList<DecodificaCodiceImportoAltraPensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceMaggiorazioneFamiliari(string tipologia, out List<DecodificaCodMaggFamiliari> elencoCodMaggFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodMaggFamiliari = (from d in db.DecodificaCodMaggFamiliaris where tipologia.Equals(d.TipoAppartenenza) select d).ToList<DecodificaCodMaggFamiliari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceGestione(out List<DecodificaCodiceGestione> elencoCodiceGestione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceGestione = (from d in db.DecodificaCodiceGestiones select d).OrderBy(x => x.Id).ToList<DecodificaCodiceGestione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void GetCodiceGestioneCalcoloRetributivo(out List<DecodificaGestioneCalcoloRetributivo> elencoCodGestioneCalcoloRetributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodGestioneCalcoloRetributivo = (from d in db.DecodificaGestioneCalcoloRetributivos where d.IsFondo == false select d).OrderBy(x => x.Id).ToList<DecodificaGestioneCalcoloRetributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceGestioneCalcoloContributivo(out List<DecodificaGestioneCalcoloContributivo> elencoCodGestioneCalcoloContributivo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodGestioneCalcoloContributivo = (from d in db.DecodificaGestioneCalcoloContributivos where d.IsFondo == false select d).OrderBy(x => x.Id).ToList<DecodificaGestioneCalcoloContributivo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceGestioneQuotaFondoIntegrativo(out List<DecodificaGestioneQuotaFondoIntegrativo> elencoCodGestioneQuotaFondoIntegrativo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodGestioneQuotaFondoIntegrativo = (from d in db.DecodificaGestioneQuotaFondoIntegrativos select d).OrderBy(x => x.Id).ToList<DecodificaGestioneQuotaFondoIntegrativo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceGestioneQuotaFondoINPGI(out List<DecodificaGestioneQuotaFondoINPGI> elencoCodGestioneQuotaFondoINPGI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodGestioneQuotaFondoINPGI = (from d in db.DecodificaGestioneQuotaFondoINPGIs select d).OrderBy(x => x.Id).ToList<DecodificaGestioneQuotaFondoINPGI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetErroriCalcoloCi(out List<ErroriCalcoloCi> elencoErroriCalcoloCi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoErroriCalcoloCi = (from e in db.ErroriCalcoloCis select e).ToList<ErroriCalcoloCi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetErroreCalcoloCiByCode(string codice, out ErroriCalcoloCi erroreCalcoloCi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    erroreCalcoloCi = (from e in db.ErroriCalcoloCis where codice == e.CodErrore select e).SingleOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciParticolari(out List<DecodificaCodiciParticolari> elencoCodiciParticolari)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciParticolari = (from d in db.DecodificaCodiciParticolaris select d).ToList<DecodificaCodiciParticolari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetPensioniExInpdai(out List<DecodificaPensioneExInpdai> elencoPensioniExInpdai)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoPensioniExInpdai = (from d in db.DecodificaPensioneExInpdais select d).ToList<DecodificaPensioneExInpdai>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciCDCMMR(out List<DecodificaCDCMMR> elencoCodiciCDCMMR)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciCDCMMR = (from d in db.DecodificaCDCMMRs select d).ToList<DecodificaCDCMMR>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiciMaggiorazioneExCombattenti(out List<DecodificaMaggiorazioneExCombattenti> elencoCodiciMaggiorazioneExCombattenti)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciMaggiorazioneExCombattenti = (from d in db.DecodificaMaggiorazioneExCombattentis select d).ToList<DecodificaMaggiorazioneExCombattenti>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGestioneTipoRichiesta(out List<DecTipoRichiesta> elencoGestioneTipoRichiesta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoGestioneTipoRichiesta = (from d in db.DecTipoRichiestas select d).ToList<DecTipoRichiesta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGestioneTipoRichiestaByCodTipoRichiesta(string codTipoRichiesta, out DecTipoRichiesta gestioneTipoRichiesta)
        {
            using (new MethodExecutionTracer())
            {
                gestioneTipoRichiesta = null;
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    gestioneTipoRichiesta = (from d in db.DecTipoRichiestas 
                                             where d.CodTipoRichiesta == codTipoRichiesta && d.IndInseribile == '1'
                                             select d).FirstOrDefault<DecTipoRichiesta>();
                    if(gestioneTipoRichiesta == null)
                        gestioneTipoRichiesta = (from d in db.DecTipoRichiestas
                                                 where d.CodTipoRichiesta == codTipoRichiesta
                                                 select d).FirstOrDefault<DecTipoRichiesta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoCalcoloSecondario(out List<DecodificaTipoCalcoloSecondario> elencoTipoCalcoloSecondario)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoCalcoloSecondario = (from d in db.DecodificaTipoCalcoloSecondarios select d).ToList<DecodificaTipoCalcoloSecondario>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSottoGruppoOneri(out List<DecCodeSottoGruppoOneri> elencoSottoGruppoOneri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoSottoGruppoOneri = (from d in db.DecCodeSottoGruppoOneris select d).ToList<DecCodeSottoGruppoOneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGruppoOneri(out List<DecCodeGruppoOneri> elencoGruppoOneri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoGruppoOneri = (from d in db.DecCodeGruppoOneris select d).ToList<DecCodeGruppoOneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetElencoDomandaRicorso(out List<DecodificaDomandaRicorso> elencoDomandaRicorso)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDomandaRicorso = (from d in db.DecodificaDomandaRicorsos select d).ToList<DecodificaDomandaRicorso>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaLegge44997(out List<DecodificaLegge44997> elencoDecodificaLegge44997)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaLegge44997 = (from d in db.DecodificaLegge44997s select d).ToList<DecodificaLegge44997>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaModalitaLiquidazione(out List<DecodificaModalitaLiquidazione> elencoDecodificaModalitaLiquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaModalitaLiquidazione = (from d in db.DecodificaModalitaLiquidaziones select d).ToList<DecodificaModalitaLiquidazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaOpzioneRiliquidazione(out List<DecodificaOpzioneRiliquidazione> elencoDecodificaOpzioneRiliquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaOpzioneRiliquidazione = (from d in db.DecodificaOpzioneRiliquidaziones select d).ToList<DecodificaOpzioneRiliquidazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaCodiceCi21(out List<DecodificaCodiceCi21> elencoDecodificaCodiceCi21)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaCodiceCi21 = (from d in db.DecodificaCodiceCi21s select d).ToList<DecodificaCodiceCi21>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaCodiceCi28(out List<DecodificaCodiceCi28> elencoDecodificaCodiceCi28)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaCodiceCi28 = (from d in db.DecodificaCodiceCi28s select d).ToList<DecodificaCodiceCi28>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaPartTime(out List<DecodificaPartTime> elencoDecodificaPartTime)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaPartTime = (from d in db.DecodificaPartTimes select d).ToList<DecodificaPartTime>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaCodiceEsodo(out List<DecodificaCodiceEsodo> elencoDecodificaCodiceEsodo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaCodiceEsodo = (from d in db.DecodificaCodiceEsodos select d).ToList<DecodificaCodiceEsodo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaArt22(out List<DecodificaArt22> elencoDecodificaArt22)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaArt22 = (from d in db.DecodificaArt22s select d).ToList<DecodificaArt22>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaCodiceCapitalizzazione(out List<DecodificaCodiceCapitalizzazione> elencoDecodificaCodiceCapitalizzazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaCodiceCapitalizzazione = (from d in db.DecodificaCodiceCapitalizzaziones select d).ToList<DecodificaCodiceCapitalizzazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaCausaCessazione(out List<DecodificaCausaCessazione> elencoDecodificaCausaCessazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaCausaCessazione = (from d in db.DecodificaCausaCessaziones select d).ToList<DecodificaCausaCessazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSiglaFamiliareByParentela(string parentela, out char? siglaFamiliare, out string tipoUnione)
        {
            siglaFamiliare = null;
            tipoUnione = null;

            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var appRelazione = (from p in db.DecRelazioneParentelas
                                        where p.CodRelazioneParentela == (!string.IsNullOrEmpty(parentela) ? parentela : string.Empty)
                                        select new { 
                                            SiglaFamiliare = p.SiglaFamiliare,
                                            TipoUnione = p.TipoUnione
                                        }).SingleOrDefault();

                    if (appRelazione != null)
                    {
                        siglaFamiliare = appRelazione.SiglaFamiliare;
                        tipoUnione = appRelazione.TipoUnione;
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetParentelaBySiglaFamiliare(char siglaFamiliare, out string parentela)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    parentela = (from p in db.DecRelazioneParentelas
                                      where p.SiglaFamiliare == siglaFamiliare
                                      select p.CodRelazioneParentela).FirstOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipiDomanda(string gruppo, string prodotto, string tipo,
            string gestione, string fondo, string ente, out List<DecTipoDomanda> listaTipiDomanda)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    listaTipiDomanda = (from td in db.DecTipoDomandas
                                        where td.Gruppo == gruppo && td.Prodotto == prodotto && td.Tipo == tipo &&
                                        (td.Gestione == gestione || td.Gestione == "xxx") &&
                                        (td.Fondo == fondo || td.Fondo == "xxx") &&
                                        (td.Ente == ente || td.Ente == "xxx")
                                        select td).ToList<DecTipoDomanda>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaPensioniPrivilegiate(out List<DecodificaPensioniPrivilegiate> elencoDecodificaPensioniPrivilegiate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaPensioniPrivilegiate = (from d in db.DecodificaPensioniPrivilegiates select d).ToList<DecodificaPensioniPrivilegiate>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaRiconoscimentiInvalidita(out List<DecodificaRiconoscimentiInvalidita> elencoRiconoscimentiInvalidita)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoRiconoscimentiInvalidita = (from d in db.DecodificaRiconoscimentiInvaliditas select d).ToList<DecodificaRiconoscimentiInvalidita>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceTipoBeneficioAGO_CI(out List<DecodificaTipoBeneficioAGO_CI> elencoCodiciBeneficioAGO_CI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiciBeneficioAGO_CI = (from d in db.DecodificaTipoBeneficioAGO_CIs select d).ToList<DecodificaTipoBeneficioAGO_CI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCassaSede(string gruppo, string prodotto, string tipo, string tipoApp, out List<DecodificaCassa> elencoCassaSede)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                    elencoCassaSede = (from d in db.DecodificaCassas
                                       where d.Gruppo == "ALL" && d.Prodotto == "ALL" && d.Tipo == "ALL" && d.TipoApp == tipoApp
                                       select d).Concat((from d in db.DecodificaCassas
                                       where d.Gruppo == gruppo && d.Prodotto == "ALL" && d.Tipo == "ALL" && d.TipoApp == tipoApp
                                       select d)).ToList<DecodificaCassa>().Concat((from d in db.DecodificaCassas
                                       where d.Gruppo == gruppo && d.Prodotto == prodotto && d.Tipo == "ALL" && d.TipoApp == tipoApp
                                       select d)).ToList<DecodificaCassa>().Concat((from d in db.DecodificaCassas
                                       where d.Gruppo == gruppo && d.Prodotto == prodotto && d.Tipo == tipo && d.TipoApp == tipoApp
                                       select d)).ToList<DecodificaCassa>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceEnte(out List<DecodificaEnte> elencoEnte)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoEnte = (from d in db.DecodificaEntes select d).ToList<DecodificaEnte>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static bool CheckProdottoTipo(string prodotto, string tipo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    int c = (from p in db.DecProdottos where p.CodProdotto == prodotto select p.CodProdotto).Count();
                    if (c == 0)
                        return false;
                    c = (from t in db.DecTipos where t.CodTipo == tipo select t.CodTipo).Count();
                    if (c == 0)
                        return false;
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
            return true;
        }

        public static void GetCatEnteAltrePensioni(out List<DecCatEnteAltrePensioni> elencoCatEnteAltrePensioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCatEnteAltrePensioni = (from d in db.DecCatEnteAltrePensionis select d).OrderBy(x => x.CodCategoria).ToList<DecCatEnteAltrePensioni>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoComponenteByCode(char tipoComponente, out string descTipoComponente)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    descTipoComponente = (from c in db.DecodificaTipoComponentes where c.TipoComponente == tipoComponente select c.Descrizione).SingleOrDefault<string>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetGruppo(out List<DecGruppo> elencoGruppo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoGruppo = (from d in db.DecGruppos select d).ToList<DecGruppo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetProdotto(out List<DecProdotto> elencoProdotto)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoProdotto = (from d in db.DecProdottos select d).ToList<DecProdotto>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipo(out List<DecTipo> elencoTipo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoTipo = (from d in db.DecTipos select d).ToList<DecTipo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetUfficiPagatoriEsteri(out List<DecodificaUfficiPagatoriEsteri> elencoUfficiPagatoriEsteri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoUfficiPagatoriEsteri = (from d in db.DecodificaUfficiPagatoriEsteris select d).ToList<DecodificaUfficiPagatoriEsteri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoPensioneFondi(out List<DecodificaTipoPensioneFondi> elencoTipoPensioneFondi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoPensioneFondi = (from d in db.DecodificaTipoPensioneFondis select d).ToList<DecodificaTipoPensioneFondi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDerogaENPALS(out List<DecodificaDerogaENPAL> elencoDerogaENPALS)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDerogaENPALS = (from d in db.DecodificaDerogaENPALs select d).ToList<DecodificaDerogaENPAL>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void GetDecodificaArt58(out List<DecodificaArt58> elencoDecoficicaArt58,string tipoFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecoficicaArt58 = (from d in db.DecodificaArt58s where d.Fondo == tipoFondo select d).ToList<DecodificaArt58>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public static void GetDecodificaPromiscui(out List<DecodificaPromiscui> elencoDecoficicaPromiscui, string tipoFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecoficicaPromiscui = (from d in db.DecodificaPromiscuis where d.Fondo == tipoFondo select d).ToList<DecodificaPromiscui>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetFase(string codFase, out DecFase fase)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    fase = (from f in db.DecFases where f.CodFase == codFase select f).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaTipoLiquidazionePM(out List<DecodificaTipoLiquidazionePM> elencoTipoLiquidazionePM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoLiquidazionePM = (from d in db.DecodificaTipoLiquidazionePMs select d).ToList<DecodificaTipoLiquidazionePM>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaLegge413(out List<DecodificaLegge413> elencoDecodificaLegge413)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaLegge413 = (from d in db.DecodificaLegge413s select d).ToList<DecodificaLegge413>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAttivitaSvolta2(out List<DecodificaAttivitaSvolta2> elencoAttivitaSvolta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoAttivitaSvolta = (from d in db.DecodificaAttivitaSvolta2s select d).ToList<DecodificaAttivitaSvolta2>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoLiquidazione(out List<DecodificaTipoLiquidazione> elencoTipoLiquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoLiquidazione = (from d in db.DecodificaTipoLiquidaziones select d).ToList<DecodificaTipoLiquidazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipologiaFAQ(out List<DecTipologiaFAQ> elencoTipologiaFAQ)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipologiaFAQ = (from d in db.DecTipologiaFAQs select d).ToList<DecTipologiaFAQ>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoLiquidazioneGAS(out List<DecodificaTipoLiquidazioneGA> elencoTipoLiquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoLiquidazione = (from d in db.DecodificaTipoLiquidazioneGAs select d).ToList<DecodificaTipoLiquidazioneGA>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetTipoLiquidazionePI(out List<DecodificaTipoLiquidazionePI> elencoTipoLiquidazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoLiquidazione = (from d in db.DecodificaTipoLiquidazionePIs select d).ToList<DecodificaTipoLiquidazionePI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCodiceTipoLiquidazionePM(out List<DecodificaCodiceTipoLiquidazionePM> elencoCodiceTipoLiquidazionePM)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCodiceTipoLiquidazionePM = (from d in db.DecodificaCodiceTipoLiquidazionePMs select d).ToList<DecodificaCodiceTipoLiquidazionePM>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaTipoQuota(out List<DecodificaTipoQuota> elencoDecodificaTipoQuota)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaTipoQuota = (from d in db.DecodificaTipoQuotas select d).ToList<DecodificaTipoQuota>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaEnteCassaProfessionale(out List<DecodificaEnteCassaProfessionale> elencoDecodificaEnteCassaProfessionale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaEnteCassaProfessionale = (from d in db.DecodificaEnteCassaProfessionales select d).ToList<DecodificaEnteCassaProfessionale>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecEnteGestioneFondo(out List<DecEnteGestioneFondo> elencoDecEnteGestioneFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecEnteGestioneFondo = (from d in db.DecEnteGestioneFondos select d).ToList<DecEnteGestioneFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecCodiceTrattenute(out List<DecCodiceTrattenute> elencoDecCodiceTrattenute)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecCodiceTrattenute = (from d in db.DecCodiceTrattenutes select d).ToList<DecCodiceTrattenute>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecPersonaleViaggiante(out List<DecPersonaleViaggiante> elencoDecPersonaleViaggiante)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecPersonaleViaggiante = (from d in db.DecPersonaleViaggiantes select d).ToList<DecPersonaleViaggiante>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaAttCon(out List<DecodificaAttCon> elencoDecodificaAttCon)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaAttCon = (from d in db.DecodificaAttCons select d).ToList<DecodificaAttCon>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static bool IsPensioneRiferimentoObbligatoria(string codGruppo, string codProdotto, string codTipo)
        {
            bool ret = false;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    var decIstanza = (from dI in db.DecIstanzas where dI.CodGruppo == codGruppo && dI.CodProdotto == codProdotto && dI.CodTipo == codTipo && dI.IndPensRifObbl != '0' 
                                     select dI).ToList<DecIstanza>();
                    if (decIstanza != null && decIstanza.Count > 0)
                        ret = true;

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

            return ret;
        }

        public static void GetDescrizioneIstanza(string codGruppo, string codProdotto, string codTipo, out string descIstanza)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    descIstanza = (from dI in db.DecIstanzas where dI.CodGruppo == codGruppo && dI.CodProdotto == codProdotto && dI.CodTipo == codTipo select dI.DescIstanza).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaSoggettoBeneficiario(out List<DecodificaSoggettoBeneficiario> elencoDecodificaSoggettoBeneficiario)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaSoggettoBeneficiario = (from d in db.DecodificaSoggettoBeneficiarios select d).ToList<DecodificaSoggettoBeneficiario>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaTipologiaPrestazione(out List<DecodificaTipologiaPrestazione> elencoDecodificaTipologiaPrestazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaTipologiaPrestazione = (from d in db.DecodificaTipologiaPrestaziones select d).ToList<DecodificaTipologiaPrestazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecTipologiaBeneficioTerrorismo(out List<DecTipologiaBeneficioTerrorismo> elencoDecTipologiaBeneficioTerrorismo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecTipologiaBeneficioTerrorismo = (from d in db.DecTipologiaBeneficioTerrorismos select d).ToList<DecTipologiaBeneficioTerrorismo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaBancaFideiussione(out List<DecodificaBancaFideiussoria> elencoDecodificaBancaFideiussione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaBancaFideiussione = (from d in db.DecodificaBancaFideiussorias select d).ToList<DecodificaBancaFideiussoria>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaBanchePerSede(out List<DecodificaBanchePerSede> elencoDecodificaBancaFideiussione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaBancaFideiussione = (from d in db.DecodificaBanchePerSedes select d).ToList<DecodificaBanchePerSede>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaSituazione(out List<DecSituazione> elencoDecodificaSituazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                    elencoDecodificaSituazione = (from d in db.DecSituaziones select d).ToList<DecSituazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaEnteRipartizioneINPDAP(out List<DecodificaEnteRipartizioneINPDAP> elencoDecodificaEnteRipartizioneINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaEnteRipartizioneINPDAP = (from d in db.DecodificaEnteRipartizioneINPDAPs select d).ToList<DecodificaEnteRipartizioneINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaInteresseLegale(out List<DecodificaInteresseLegale> elencoDecodificaInteresseLegale)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaInteresseLegale = (from d in db.DecodificaInteresseLegales select d).ToList<DecodificaInteresseLegale>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlNoInvioMailDirettore(out List<CtrlNoInvioMailDirettore> elencoCtrlNoInvioMailDirettore)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCtrlNoInvioMailDirettore = (from d in db.CtrlNoInvioMailDirettores select d).ToList<CtrlNoInvioMailDirettore>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaTipoCalcoloVincenteDAI(out List<DecodificaTipoCalcoloVincenteDAI> elencoDecodificaTipoCalcoloVincenteDAI)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecodificaTipoCalcoloVincenteDAI = (from d in db.DecodificaTipoCalcoloVincenteDAIs select d).ToList<DecodificaTipoCalcoloVincenteDAI>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static bool IsBypassInvioMailDirettore(string siglaCategoria)
        {
            bool ret = false;

            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var result = (from d in db.CtrlNoInvioMailDirettores where d.SiglaCatPensione.Trim() == siglaCategoria.Trim() select d).FirstOrDefault<CtrlNoInvioMailDirettore>();
                    if (result != null)
                        ret = true;

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

            return ret;
        }

        public static void GetCtrlRequisitoEta(out List<CtrlRequisitoEta> elencoCtrlRequisitoEta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCtrlRequisitoEta = (from d in db.CtrlRequisitoEtas select d).ToList<CtrlRequisitoEta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlRequisitoEta_Base(DateTime dataRiferimento, string codCategoria, char sesso, string tipoAppartenenza, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var res = (from d in db.CtrlRequisitoEtas
                               where d.InizioPeriodoPerfRequisiti <= dataRiferimento && d.FinePeriodoPerfRequisiti >= dataRiferimento &&
                               d.Categoria == codCategoria.PadLeft(4, '0') &&
                               d.Sesso == sesso && d.TipoAppartenenza == tipoAppartenenza
                               select d).FirstOrDefault();
                    if (res != null)
                    {
                        reqAA = res.RequisitoAA.GetValueOrDefault();
                        reqMM = res.RequisitoMM.GetValueOrDefault();
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlRequisitoEta_Avanzato(Expression<Func<CtrlRequisitoEta, bool>> whereCondition, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var res = (from d in db.CtrlRequisitoEtas
                               select d).Where(whereCondition).FirstOrDefault();
                    if (res != null)
                    {
                        reqAA = res.RequisitoAA.GetValueOrDefault();
                        reqMM = res.RequisitoMM.GetValueOrDefault();
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlRequisitoEta_Anticipo(DateTime dataRiferimento, out int reqAA, out int reqMM)
        {
            reqAA = 0;
            reqMM = 0;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var res = (from d in db.CtrlRequisitoEta_Anticipos
                                              where d.InizioPeriodoPerfRequisiti <= dataRiferimento && d.FinePeriodoPerfRequisiti >= dataRiferimento
                                              select d).FirstOrDefault();
                    if(res != null)
                    {
                        reqAA = res.RequisitoAA.GetValueOrDefault();
                        reqMM = res.RequisitoMM.GetValueOrDefault();
                    }
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlRicercaGPT(out List<CtrlRicercaGPT> elencoCtrlRicercaGPT)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoCtrlRicercaGPT = (from c in db.CtrlRicercaGPTs select c).ToList<CtrlRicercaGPT>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecMicroqualificaINPDAP(string siglaCategoria, out List<DecMicroqualificaINPDAP> elencoDecMicroqualificaINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecMicroqualificaINPDAP = (from d in db.DecMicroqualificaINPDAPs
                                                     join c in db.CtrlMicroqualificaINPDAPs on d.Id equals c.IdMicroqualifica
                                                     where c.SiglaCategoria == siglaCategoria
                                                     select d).ToList<DecMicroqualificaINPDAP>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetMicroqualificaINPDAPById(long id, out DecMicroqualificaINPDAP microqualificaINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    microqualificaINPDAP = (from d in db.DecMicroqualificaINPDAPs
                                      where d.Id == id
                                      select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetMicroqualificaINPDAPByTraduzioneSuGP(string traduzioneSuGP, out DecMicroqualificaINPDAP microqualificaINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    microqualificaINPDAP = (from d in db.DecMicroqualificaINPDAPs
                                            where d.TraduzioneSuGP == traduzioneSuGP
                                            select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecodificaTipoFelpe(long id, out DecodificaTipoFelpe decodificaTipoFelpe)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    decodificaTipoFelpe = (from d in db.DecodificaTipoFelpes
                                            where d.Id == id
                                            select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlEnteCassaCodiceGestioneByCat(long enteCassa, string categoria, out CtrlEnteCassaCodiceGestione enteCassaCodiceGestione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    enteCassaCodiceGestione = (from d in db.CtrlEnteCassaCodiceGestiones
                                               where d.TraduzioneSuGP == enteCassa.ToString().PadLeft(4,'0') && d.CodiceCategoria == categoria
                                               select d).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlEnteCassaCodiceGestione(out List<CtrlEnteCassaCodiceGestione> elencoEnteCassaCodiceGestione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoEnteCassaCodiceGestione = (from d in db.CtrlEnteCassaCodiceGestiones
                                               select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlCatAdeguata(out List<CtrlCatAdeguata> elencoEnteCassaCodiceGestione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoEnteCassaCodiceGestione = (from d in db.CtrlCatAdeguatas
                                                     select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlTipoUfficio(out List<CtrlTipoUfficio> elencoTipoUfficio)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoTipoUfficio = (from d in db.CtrlTipoUfficios
                                         select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

        }

        public static void GetDecCapitolo(bool PL, out List<DecCapitolo> elencoDecCapitolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecCapitolo = (from d in db.DecCapitolos
                                         where d.PL == PL || !d.PL.HasValue
                                                     select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecComparto(out List<DecComparto> elencoDecComparto)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecComparto = (from d in db.DecCompartos
                                         select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecSettore(out List<DecSettore> elencoDecSettore)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecSettore = (from d in db.DecSettores
                                         select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecRuolo(out List<DecRuolo> elencoDecRuolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    elencoDecRuolo = (from d in db.DecRuolos
                                        select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetCtrlCompartoSettoreRuoloByCat(string categoria, out List<CtrlCompartoSettoreRuolo> ctrlCompartoSettoreRuolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    ctrlCompartoSettoreRuolo = (from d in db.CtrlCompartoSettoreRuolos
                                               where categoria.IndexOf(d.Cassa) >= 0 || d.Cassa == null
                                               select d).ToList();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetDecSede(out List<DecSede> elencoDecSede)
        {            
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    WebDomDataContext db = new WebDomDataContext(ConnectionFactory.GetConnection("WebDomConnectionString"));
                   elencoDecSede = (from dS in db.DecSedes                                 
                                   select dS).ToList<DecSede>();
                    
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }           
        }
    }
}
