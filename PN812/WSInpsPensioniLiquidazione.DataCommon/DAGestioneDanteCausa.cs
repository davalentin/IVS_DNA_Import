using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDanteCausa
    {
        public static void SalvaDanteCausa(DanteCausa dantecausaToDB)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));

                db.InsertDanteCausa(
                dantecausaToDB.IdAnagrafica,
                dantecausaToDB.IdPensione,
                dantecausaToDB.SiglaCategoria,
                dantecausaToDB.Sede,
                dantecausaToDB.Certificato,
                dantecausaToDB.DecorrenzaPensione,
                dantecausaToDB.DataMorte,
                dantecausaToDB.ProvenienzaPensione,
                dantecausaToDB.CodiceTipoPensione,
                dantecausaToDB.CodiceBeneficiLegge,
                dantecausaToDB.Maggiorazione781Contributi,
                dantecausaToDB.StatoEEResidenza,
                dantecausaToDB.DecorrenzaResidenza,
                dantecausaToDB.CategoriaAltraPensione,
                dantecausaToDB.EnteAltraPensione,
                dantecausaToDB.CodiceUCAltraPensione,
                dantecausaToDB.CodiceImportoAltraPensione,
                dantecausaToDB.DecorrenzaAltraPensione,
                dantecausaToDB.CessazioneAltraPensione,
                dantecausaToDB.NaturaPensione,
                dantecausaToDB.ImportoPensione311284,
                dantecausaToDB.ImportoPensione1185,
                dantecausaToDB.ImportoPensione1190,
                dantecausaToDB.NContributiDiretta,
                dantecausaToDB.EccedenzaArt5,
                dantecausaToDB.ParentelaDC,
                dantecausaToDB.CodiceTipoPerequazione,
                dantecausaToDB.VirtualePura,
                dantecausaToDB.VirtualeIntegrata,
                dantecausaToDB.Adeguata,
                dantecausaToDB.CodiceEliminazione,
                dantecausaToDB.DecorrenzaEliminazione,
                dantecausaToDB.DecorrenzaEliminazioneContabile,
                dantecausaToDB.TotaleQuoteFisse,
                dantecausaToDB.DataMorteOrigine,
                dantecausaToDB.StatoEEResidenzaByArca,
                dantecausaToDB.CittadinanzaByArca,
                dantecausaToDB.NaturaPensioneAltraPensione,
                dantecausaToDB.CategoriaFascicolo,
                dantecausaToDB.SedeFascicolo,
                dantecausaToDB.NumeroFascicolo,
                dantecausaToDB.IsFascicoloGenerato,
                dantecausaToDB.DataMatrimonioByPrelievo,
                dantecausaToDB.ImportoPagamentoDataMorte49593);
                db.Connection.Close();
            }
        }

        public static void CancellaDanteCausa(long IDdantecausa, long IDAnagrafica)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteDanteCausa(IDdantecausa, IDAnagrafica);
                db.Connection.Close();
            }
        }

        public static void CancellaAllDanteCausaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int a = db.DeleteAllDanteCausa(idPensione);
                db.Connection.Close();
            }
        }

        public static void GetDanteCausabyIdPensione(long idPensione, out DanteCausa danteCausa)
        {
            danteCausa = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    danteCausa = (from f in db.DanteCausas
                                  where f.IdPensione == idPensione
                                  select f).FirstOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetAnagraficaDanteCausabyIdPensione(long idPensione, out Anagrafica anagrafica)
        {
            DanteCausa danteCausa = null;
            anagrafica = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
            {
                GetDanteCausabyIdPensione(idPensione, out danteCausa);
                if (danteCausa != null)
                    DAGestioneAnagrafica.GetAnagraficaByIdAnagrafica(danteCausa.IdAnagrafica, out anagrafica);
                transactionScope.Complete();
            }
        }

        public static void GetPensioniEstereDC(long iDdantecausa, out List<PensioniEstereDC> lPensioniEstereDC)
        {
            lPensioniEstereDC = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    lPensioniEstereDC = (from p in db.PensioniEstereDCs
                                         where p.IdDanteCausa == iDdantecausa
                                         select p).ToList<PensioniEstereDC>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void CancellaPensioniEstereDCByIdPensione(long idPensione)
        {
            DanteCausa danteCausa = null;
            GetDanteCausabyIdPensione(idPensione, out danteCausa);
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.DeletePensioniEstereDC(danteCausa.IdPensione);
                db.Connection.Close();
            }
        }

        public static void SalvaPensioniEstereDC(long? iDdantecausa, byte? CodiciVari, decimal? Importo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                db.InsertPensioniEstereDC(iDdantecausa, CodiciVari, Importo);
                db.Connection.Close();
            }
        }

        public static bool IsPresenteDomandaInLiquidazionePerControlloSpacENPALS(string codiceFiscaleDanteCausa, Expression<Func<DanteCausa, bool>> whereCondition, out List<long> NDomus)
        {
            bool ret = false;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    var filteredDanteCausa = db.DanteCausas.Where(whereCondition);
                    var danteCausa = (from f in filteredDanteCausa
                                      join a in db.Anagraficas on f.IdAnagrafica equals a.Id
                                      join p in db.Pensiones on f.IdPensione equals p.Id
                                      where a.CodiceFiscale == codiceFiscaleDanteCausa
                                      && p.StatoPensione != 4 && p.StatoPensione < 8
                                      select new { DanteCausa =f, NDomus = p.NDomus });
                    
                 NDomus = new List<long>();
                    if (danteCausa.Count() > 0)
                    {
                        foreach (var risultato in danteCausa)
                        {
                            NDomus.Add(risultato.NDomus);
                        }
                        ret = true;
                    }
                    else
                    {
                        NDomus = null;
                    }

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }

            return ret;
        }

    }
}
