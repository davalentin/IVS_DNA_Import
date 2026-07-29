using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDatiContributiviCi
    {
        #region PrestazioniEE

        public static void SalvaPrestazioneEstera(PensioniCiPrestazioniEE prestazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                long? idPrestazioneEE = 0;
                db.InsertPensioniCiPrestazioniEE(prestazione.Id, prestazione.IdPensione, prestazione.CodiceStatoEE, prestazione.CodiceIstituzione,
                    prestazione.MatricolaIstituzioneEE, prestazione.ContributiEEDecorrenzaOriginaria, prestazione.ContributiEERicalcolo,
                    prestazione.DecorrenzaLiquidazioneStatoEE, prestazione.ContributiEEDiritto, prestazione.SospensioneCautelativaIntegrazione,
                    prestazione.EtaSospensione, prestazione.CodiceArt48, prestazione.DecorrenzaArt48, prestazione.QuotaIntegrazioneEEeArgentinaResidentiItalia,
                    prestazione.DecorrenzaIntegrazione, prestazione.DecorrenzaRicalcolo, prestazione.CodiceConvenzione, prestazione.CodicePi,
                    prestazione.Confermato, prestazione.IsStorico, ref idPrestazioneEE);
                prestazione.Id = idPrestazioneEE.HasValue ? idPrestazioneEE.Value : 0;
                db.Connection.Close();
            }
        }

        public static void GetPrestazioniEEByIdPensione(long idPensione, bool isStorico, out List<PensioniCiPrestazioniEE> listaPrestazioniEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaPrestazioniEE = (from p in db.PensioniCiPrestazioniEEs
                                      where p.IdPensione == idPensione && p.IsStorico == isStorico
                                      select p).ToList<PensioniCiPrestazioniEE>();
                db.Connection.Close();
            }
        }

        public static void DeletePrestazioniEE(long idPrestazioneEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioniCiPrestazioniEE(idPrestazioneEE);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioniCiPrestazioniEE");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllPrestazioniEENoStoricoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllPensioniCiPrestazioniEENoStorico(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllPensioniCiPrestazioniEENoStorico");
                }
                db.Connection.Close();
            }
        }
        #endregion PrestazioniEE

        #region ImportiEsteri
        public static void SalvaImportoEstero(PensioniCiImportiEsteri importo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.InsertPensioniCiImportiEsteri(importo.Id, importo.IDPrestazioneEE, importo.DecorrenzaPrestazioneEE, importo.CessazionePrestazioneEE, importo.ImportoPrestazioneEE);
                db.Connection.Close();
            }
        }

        public static void GetImportiEsteriByIdPensione(long idPensione, out List<PensioniCiImportiEsteri> listaImportiEsteri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaImportiEsteri = (from p in db.PensioniCiImportiEsteris
                                      where p.PensioniCiPrestazioniEE.IdPensione == idPensione
                                      select p).ToList<PensioniCiImportiEsteri>();
                db.Connection.Close();
            }
        }

        public static void DeleteImportiEsteri(long idImportiEsteri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioniCiImportiEsteri(idImportiEsteri);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioniCiImportiEsteri");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllImportiEsteriByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllPensioniCiImportiEsteri(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllPensioniCiImportiEsteri");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteImportiEsteriPerPrestazione(long idPrestazioneEE)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllPensioniCiImportiEsteriPerPrestazione(idPrestazioneEE);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllPensioniCiImportiEsteriPerPrestazione");
                }
                db.Connection.Close();
            }
        }
        #endregion ImportiEsteri

        #region DatiGenerici
        public static void GetDatiGenericiByIdPensione(Int64 idPensione, out PensioniDatiGenerici datiGenerici)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    datiGenerici = (from pDG in db.PensioniDatiGenericis
                                    where pDG.IdPensione == idPensione
                                    select pDG).SingleOrDefault<PensioniDatiGenerici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiGenerici(PensioniDatiGenerici datiGenerici)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioniDatiGenerici(datiGenerici.IdPensione,
                    datiGenerici.RegimeLiquidazione, datiGenerici.ContributiItalianiEdEsteriAl1295,
                    datiGenerici.NSettFittiziePrepensionamento, datiGenerici.CodiceVirtuale, datiGenerici.ConiugeSuperstite,
                    datiGenerici.DecorrenzaCodiceVirtuale, datiGenerici.ImportoPensioneEEInvalido,
                    datiGenerici.DataRicalcoloPrestazioneEE, datiGenerici.NContributiItalia, datiGenerici.DecorrenzaBonus,
                    datiGenerici.DeliberaCee126, datiGenerici.ImportoCristallizzazione3481, datiGenerici.UfficioPagatoreArretratiEE,
                    datiGenerici.CodiceScadenzaAssegno, datiGenerici.AnniDifferimento, datiGenerici.CodiciMotivazioniCi281,
                    datiGenerici.CodiciMotivazioniCi282, datiGenerici.CodiciCi21, datiGenerici.CodiceBloccoArretratiEE,
                    datiGenerici.CodicePensioneRiliquidata, datiGenerici.ApplicazioneSentenza49593, datiGenerici.DecorrenzaArt2Dpcm,
                    datiGenerici.DataPrecedenteLiquidazione, datiGenerici.DataArrivoDomanda, datiGenerici.Data1Domanda, datiGenerici.VVMisuraAl1292,
                    datiGenerici.VVMisuraDL50392, datiGenerici.SettimanePerCalcoloContributivo,
                    datiGenerici.SettimaneItalianeDiritto, datiGenerici.ImportoIVS, datiGenerici.MaternitaAcna, datiGenerici.RMS8888,
                    datiGenerici.RMS9090, datiGenerici.CMSM, datiGenerici.RiduzioneRetributiva, datiGenerici.RiduzioneRetributivaPercentuale, datiGenerici.AnzAl95,
                    datiGenerici.QuotaAl95, datiGenerici.EnteCassa, datiGenerici.EnteIstruttoreExInpdap, datiGenerici.FacoltaComputo, datiGenerici.ScadenzaAssegno,
                    datiGenerici.ImportoUltimaRetribuzione, datiGenerici.InizioUltimoLavoro, datiGenerici.FineUltimoLavoro, datiGenerici.ImportoLordoAllaDecorrenza,
                    datiGenerici.AnnoBancaFideiussoria, datiGenerici.ProgressivoBancaFideiussoria, datiGenerici.DataAssunzioneCarico, datiGenerici.ImportoLordo,
                    datiGenerici.TipoCumulo, datiGenerici.CumuloEsterno, datiGenerici.SettimaneUtiliMisura, datiGenerici.TipoCertificazioneFelpe,
                    datiGenerici.CodRicalcoloSentenza, datiGenerici.ReqArt2DL503, datiGenerici.PL_Coeftrasf, datiGenerici.TipologiaCumulo, 
                    datiGenerici.ImportoMensileAllaDecorrenzaOriginaria, datiGenerici.ImportoMensileAlGennaio2001, datiGenerici.ImportoMensilePensioneEstera, datiGenerici.CodiceCI28, datiGenerici.CodiceConvenzioneAgo, datiGenerici.ImportoAl200312,
                    datiGenerici.TotaleSettimaneEstereUtiliPerDiritto, datiGenerici.ContribuzioneEsteraTotale);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPensioniCiDatiGenerici");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteDatiGenericiByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioniDatiGenerici(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePensioniDatiGenerici");
                }
                db.Connection.Close();
            }
        }

        #endregion DatiGenerici

        #region Importi Esteri PensioniCIImportiValuta

        public static void GetDatiImportiEsteriValutaByIdPensione(long idPensione, out List<PensioniCiImportiValuta> LpensioniCiImportiValuta)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LpensioniCiImportiValuta = (from pCiIV in db.PensioniCiImportiValutas
                                                where pCiIV.IdPensione == idPensione
                                                select pCiIV).ToList<PensioniCiImportiValuta>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiImportiEsteriValuta(PensioniCiImportiValuta pensioniCIImportiValuta)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPensioniCiImportiValuta(pensioniCIImportiValuta.IdPensione, pensioniCIImportiValuta.DecorrenzaPrestazioneEE,
                                                              pensioniCIImportiValuta.ImportoPrestazioneEE);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure SalvaDatiImportiEsteriValuta");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteDatiImportiEsteriValutaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePensioniCiImportiValuta(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiImportiEsteri");
                }
                db.Connection.Close();
            }
        }

        #endregion Importi Esteri PensioniCIImportiValuta

        #region MaternitaAcna

        public static void GetDatiMaternitaAcnaByIdPensione(long idPensione, out List<MaternitaAcna> LmaternitaAcna)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    LmaternitaAcna = (from pMA in db.MaternitaAcnas
                                      where pMA.IdPensione == idPensione
                                      select pMA).ToList<MaternitaAcna>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaDatiMaternitaAcna(MaternitaAcna maternitaAcna)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertMaternitaAcna(maternitaAcna.IdPensione, maternitaAcna.ImportoIVS, maternitaAcna.SettimaneAl1292,
                                                    maternitaAcna.SettimaneDL50392, maternitaAcna.Tipo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure SalvaDatiMaternitaAcna");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteDatiMaternitaAcnaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteMaternitaAcna(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiMaternitaAcna");
                }
                db.Connection.Close();
            }
        }

        #endregion MaternitaAcna

        #region DatiPostDecOriginaria
        public static void SalvaDatiPostDecOriginaria(long idPensione, List<DatiPostDecOriginaria> listaDatiPostDecOriginaria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                if (listaDatiPostDecOriginaria != null && listaDatiPostDecOriginaria.Count > 0)
                {
                    foreach (DatiPostDecOriginaria datiPostDecOriginaria in listaDatiPostDecOriginaria)
                    {
                        int result = db.InsertDatiPostDecOriginaria(idPensione, datiPostDecOriginaria.Decorrenza, datiPostDecOriginaria.CTR, datiPostDecOriginaria.IVS, datiPostDecOriginaria.SettimaneRetributive, datiPostDecOriginaria.SettimaneVV, datiPostDecOriginaria.RMS);
                        if (result != 0)
                        {
                            throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiPostDecOriginaria");
                        }
                    }
                }
                db.Connection.Close();
            }
        }

        public static void GetDatiPostDecOriginariaByIdPensione(long idPensione, out List<DatiPostDecOriginaria> listaDatiPostDecOriginaria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaDatiPostDecOriginaria = (from d in db.DatiPostDecOriginarias
                                              where d.IdPensione == idPensione
                                              select d).ToList<DatiPostDecOriginaria>();
                db.Connection.Close();
            }
        }

        public static void DeleteAllDatiPostDecOriginariaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllDatiPostDecOriginaria(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllDatiPostDecOriginaria");
                }
                db.Connection.Close();
            }
        }

        #endregion DatiPostDecOriginaria

        #region RedditiPerIntegrazioneVirtuale
        public static void SalvaRedditiPerIntegrazioneVirtuale(long idPensione, List<RedditiPerIntegrazioneVirtuale> listaRedditiPerIntegrazioneVirtuale)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                if (listaRedditiPerIntegrazioneVirtuale != null && listaRedditiPerIntegrazioneVirtuale.Count > 0)
                {
                    foreach (RedditiPerIntegrazioneVirtuale redditiIntegrazioneVirtuale in listaRedditiPerIntegrazioneVirtuale)
                    {
                        int result = db.InsertRedditiPerIntegrazioneVirtuale(redditiIntegrazioneVirtuale.Id, idPensione, redditiIntegrazioneVirtuale.Anno, redditiIntegrazioneVirtuale.Reddito, redditiIntegrazioneVirtuale.IsTitolare);
                        if (result != 0)
                        {
                            throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRedditiPerIntegrazioneVirtuale");
                        }
                    }
                }
                db.Connection.Close();
            }
        }

        public static void GetRedditiPerIntegrazioneVirtuale(long idPensione, out List<RedditiPerIntegrazioneVirtuale> listaRedditiPerIntegrazioneVirtuale)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaRedditiPerIntegrazioneVirtuale = (from d in db.RedditiPerIntegrazioneVirtuales
                                              where d.IdPensione == idPensione
                                                       select d).ToList<RedditiPerIntegrazioneVirtuale>();
                db.Connection.Close();
            }
        }

        public static void EliminaAllRedditiPerIntegrazioneVirtuale(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRedditiPerIntegrazioneVirtuale(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRedditiPerIntegrazioneVirtuale");
                }
                db.Connection.Close();
            }
        }

        #endregion RedditiPerIntegrazioneVirtuale
    }
}
