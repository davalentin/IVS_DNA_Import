using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneQuadri
    {
        #region QuadroTitolare
        public static void GetQuadroTitolareByIdPensione(Int64 idPensione, out QuadroTitolare quadroTitolare)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroTitolare = (from qT in db.QuadroTitolares
                                      where qT.IdPensione == idPensione
                                      select qT).SingleOrDefault<QuadroTitolare>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroTitolare(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroTitolare(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroTitolare");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroTitolare(QuadroTitolare quadroTitolare)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroTitolare(quadroTitolare.IdPensione, quadroTitolare.Tipo, quadroTitolare.TabAnagrafica, quadroTitolare.TabStatiCivili, quadroTitolare.TabResidenzeEstero);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroTitolare");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroTitolare

        #region QuadroDetrazioni
        public static void GetQuadroDetrazioniByIdPensione(Int64 idPensione, out QuadroDetrazioni quadroDetrazioni)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDetrazioni = (from qD in db.QuadroDetrazionis
                                        where qD.IdPensione == idPensione
                                        select qD).SingleOrDefault<QuadroDetrazioni>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroDetrazioni(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDetrazioni(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDetrazioni");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroDetrazioni(QuadroDetrazioni quadroDetrazioni)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDetrazioni(quadroDetrazioni.IdPensione, quadroDetrazioni.Tipo, quadroDetrazioni.TabDetrazioni);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDetrazioni");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroDetrazioni

        #region QuadroPagamento
        public static void GetQuadroPagamentoByIdPensione(Int64 idPensione, out QuadroPagamento quadroPagamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroPagamento = (from qP in db.QuadroPagamentos
                                       where qP.IdPensione == idPensione
                                       select qP).SingleOrDefault<QuadroPagamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroPagamento(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroPagamento(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroPagamento");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroPagamento(QuadroPagamento quadroPagamento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroPagamento(quadroPagamento.IdPensione, quadroPagamento.Tipo, quadroPagamento.TabPagamento);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroPagamento");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroPagamento

        #region QuadroLiquidazionePensione
        public static void GetQuadroLiquidazionePensioneByIdPensione(Int64 idPensione, out QuadroLiquidazionePensione quadroLiquidazionePensione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroLiquidazionePensione = (from qLP in db.QuadroLiquidazionePensiones
                                                  where qLP.IdPensione == idPensione
                                                  select qLP).SingleOrDefault<QuadroLiquidazionePensione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroLiquidazionePensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroLiquidazionePensione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroLiquidazionePensione");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroLiquidazionePensione(QuadroLiquidazionePensione quadroLiquidazionePensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroLiquidazionePensione(quadroLiquidazionePensione.IdPensione, quadroLiquidazionePensione.Tipo, quadroLiquidazionePensione.TabDatiGenerici,
                    quadroLiquidazionePensione.TabOpzione, quadroLiquidazionePensione.TabPrecedentePensione,
                    quadroLiquidazionePensione.TabIstruttoria, quadroLiquidazionePensione.TabDatiAssicurativi, quadroLiquidazionePensione.TabInail, quadroLiquidazionePensione.TabDatiLegge460,
                    quadroLiquidazionePensione.TabContribuzioneEnpals, quadroLiquidazionePensione.TabStorico, quadroLiquidazionePensione.TabInteressiLegali, quadroLiquidazionePensione.TabSentenzaArt4,
                    quadroLiquidazionePensione.TabSentenze);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroLiquidazionePensione");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroLiquidazionePensione

        #region QuadroDelegatoTutore
        public static void GetQuadroDelegatoTutoreByIdPensione(long idPensione, out QuadroDelegatoTutore quadroDelegatoTutore)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDelegatoTutore = (from qDT in db.QuadroDelegatoTutores
                                            where qDT.IdPensione == idPensione
                                            select qDT).SingleOrDefault<QuadroDelegatoTutore>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaQuadroDelegatoTutore(QuadroDelegatoTutore quadroDelegatoTutore)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDelegatoTutore(quadroDelegatoTutore.IdPensione, quadroDelegatoTutore.Tipo, quadroDelegatoTutore.TabDelegato, quadroDelegatoTutore.TabTutore);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDelegatoTutore");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuadroDelegatoTutore(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDelegatoTutore(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDelegatoTutore");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroDelegatoTutore

        #region QuadroDatiContributivi
        public static void GetQuadroDatiContributiviByIdPensione(long idPensione, out QuadroDatiContributivi quadroDatiContributivi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiContributivi = (from qDC in db.QuadroDatiContributivis
                                              where qDC.IdPensione == idPensione
                                              select qDC).SingleOrDefault<QuadroDatiContributivi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaQuadroDatiContributivi(QuadroDatiContributivi quadroDatiContributivi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDatiContributivi(quadroDatiContributivi.IdPensione, quadroDatiContributivi.Tipo, quadroDatiContributivi.TabDatiCalcolo,
                    quadroDatiContributivi.TabProRata, quadroDatiContributivi.TabContrEsteri, quadroDatiContributivi.TabMaternAcna, quadroDatiContributivi.TabLavAutonomi,
                    quadroDatiContributivi.TabDatiPostDecOriginaria, quadroDatiContributivi.TabDatiFondo, quadroDatiContributivi.TabDatiAgo, quadroDatiContributivi.TabArt11e14,
                    quadroDatiContributivi.TabDatiCalcoloENPALS, quadroDatiContributivi.TabAnte67, quadroDatiContributivi.TabSL33670, quadroDatiContributivi.TabDatiCalcoloINPDAI,
                    quadroDatiContributivi.TabQuotePensione, quadroDatiContributivi.TabVittime, quadroDatiContributivi.TabDatiCalcolo707, quadroDatiContributivi.TabStorico, quadroDatiContributivi.TabIntegrazioneVirtuale,
                    quadroDatiContributivi.TabQuotaFondoIntegrativo, quadroDatiContributivi.TabQuotaFondoINPGI, quadroDatiContributivi.TabDatiEsteri, quadroDatiContributivi.TabMiglioramentiContrattuali, quadroDatiContributivi.TabQuotaFondoINPGIStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDatiContributivi");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuadroDatiContributivi(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDatiContributivi(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDatiContributivi");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroDatiContributivi

        #region QuadroRedditi
        public static void GetQuadroRedditiByIdPensione(long idPensione, out QuadroRedditi quadroRedditi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroRedditi = (from qR in db.QuadroRedditis
                                     where qR.IdPensione == idPensione
                                     select qR).SingleOrDefault<QuadroRedditi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroRedditi(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroRedditi(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroRedditi");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroRedditi(QuadroRedditi quadroRedditi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroRedditi(quadroRedditi.IdPensione, quadroRedditi.Tipo, quadroRedditi.TabRedditi);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroRedditi");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroRedditi

        #region QuadroFamiliari
        public static void GetQuadroFamiliariByIdPensione(Int64 idPensione, out QuadroFamiliari quadroFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroFamiliari = (from qF in db.QuadroFamiliaris
                                       where qF.IdPensione == idPensione
                                       select qF).SingleOrDefault<QuadroFamiliari>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroFamiliari(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroFamiliari(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroFamiliari");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroFamiliari(QuadroFamiliari quadroFamiliari)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroFamiliari(quadroFamiliari.IdPensione, quadroFamiliari.Tipo, quadroFamiliari.TabFamiliari);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroFamiliari");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroFamiliari

        #region QuadroDanteCausa
        public static void GetQuadroDanteCausaByIdPensione(Int64 idPensione, out QuadroDanteCausa quadroDanteCausa)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDanteCausa = (from qDC in db.QuadroDanteCausas
                                        where qDC.IdPensione == idPensione
                                        select qDC).SingleOrDefault<QuadroDanteCausa>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroDanteCausa(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDanteCausa(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDanteCausa");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroDanteCausa(QuadroDanteCausa quadroDanteCausa)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDanteCausa(quadroDanteCausa.IdPensione, quadroDanteCausa.Tipo, quadroDanteCausa.TabAnagrafica, quadroDanteCausa.TabPensioneDiretta, quadroDanteCausa.TabAltraPensione, quadroDanteCausa.TabDatiPensioneCI, quadroDanteCausa.TabSentenza49593);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDanteCausa");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroDanteCausa

        #region QuadroMaggiorazioniBenefici

        public static void GetQuadroMaggiorazioniBeneficiByIdPensione(Int64 idPensione, out QuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroMaggiorazioniBenefici = (from qMB in db.QuadroMaggiorazioniBeneficis
                                                   where qMB.IdPensione == idPensione
                                                   select qMB).SingleOrDefault<QuadroMaggiorazioniBenefici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroMaggiorazioniBenefici(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroMaggiorazioniBenefici(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroMaggiorazioniBenefici");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroMaggiorazioniBenefici(QuadroMaggiorazioniBenefici quadroMaggiorazioniBenefici)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroMaggiorazioniBenefici(quadroMaggiorazioniBenefici.IdPensione, quadroMaggiorazioniBenefici.Tipo,
                    quadroMaggiorazioniBenefici.TabBenefici, quadroMaggiorazioniBenefici.TabLegge407, quadroMaggiorazioniBenefici.TabExCombattente, quadroMaggiorazioniBenefici.TabOneri,
                    quadroMaggiorazioniBenefici.TabPrivilegiate, quadroMaggiorazioniBenefici.TabArticolo2, quadroMaggiorazioniBenefici.TabMaggiorazioni, quadroMaggiorazioniBenefici.TabPrepensionamento,
                    quadroMaggiorazioniBenefici.TabBeneficioVittimeTerrorismo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroMaggiorazioniBenefici");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroMaggiorazioniBenefici

        #region QuadroSupplementi

        public static void GetQuadroSupplementiByIdPensione(Int64 idPensione, out QuadroSupplementi quadroSupplementi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroSupplementi = (from qS in db.QuadroSupplementis
                                         where qS.IdPensione == idPensione
                                         select qS).SingleOrDefault<QuadroSupplementi>();

                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroSupplementi(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroSupplementi(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroSupplementi");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroSupplementi(QuadroSupplementi quadroSupplementi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroSupplementi(quadroSupplementi.IdPensione, quadroSupplementi.Tipo,
                    quadroSupplementi.TabSupplementi, quadroSupplementi.TabIntegrazioneArt11, quadroSupplementi.TabContribuzioneEnpals, quadroSupplementi.TabStoricoSupplementi);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroSupplementi");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroSupplementi

        #region QuadroBititolarita

        public static void GetQuadroBititolaritaByIdPensione(Int64 idPensione, out QuadroBititolarita quadroBititolarita)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroBititolarita = (from qS in db.QuadroBititolaritas
                                          where qS.IdPensione == idPensione
                                          select qS).SingleOrDefault<QuadroBititolarita>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroBititolarita(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroBititolarita(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroBititolarita");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroBititolarita(QuadroBititolarita quadroBititolarita)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroBititolarita(quadroBititolarita.IdPensione, quadroBititolarita.Tipo, quadroBititolarita.TabAltrePensioni);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroBititolarita");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroBititolarita

        #region QuadroEliminazione

        public static void GetQuadroEliminazioneByIdPensione(Int64 idPensione, out QuadroEliminazione quadroEliminazione)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroEliminazione = (from q in db.QuadroEliminaziones
                                          where q.IdPensione == idPensione
                                          select q).SingleOrDefault<QuadroEliminazione>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroEliminazione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroEliminazione(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroEliminazione");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroEliminazione(QuadroEliminazione quadroEliminazione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroEliminazione(quadroEliminazione.IdPensione, quadroEliminazione.Tipo, quadroEliminazione.TabEliminazione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroEliminazione");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroEliminazione

        #region QuadroOneri

        public static void GetQuadroOneriByIdPensione(Int64 idPensione, out QuadroOneri quadroOneri)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroOneri = (from q in db.QuadroOneris
                                   where q.IdPensione == idPensione
                                   select q).SingleOrDefault<QuadroOneri>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroOneri(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroOneri(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroOneri");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroOneri(QuadroOneri quadroOneri)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroOneri(quadroOneri.IdPensione, quadroOneri.Tipo, quadroOneri.TabOneri, quadroOneri.TabPrepensionamento, quadroOneri.TabStorico);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroOneri");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroEliminazione

        #region QuadroDatiFondo

        public static void GetQuadroDatiFondoByIdPensione(Int64 idPensione, out QuadroDatiFondo quadroDatiFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiFondo = (from q in db.QuadroDatiFondos
                                       where q.IdPensione == idPensione
                                       select q).SingleOrDefault<QuadroDatiFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroDatiFondo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDatiFondo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDatiFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroDatiFondo(QuadroDatiFondo quadroDatiFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDatiFondo(quadroDatiFondo.IdPensione, quadroDatiFondo.Tipo, quadroDatiFondo.TabRegistrazioniFondo); 
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDatiFondo");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroDatiFondo

        #region QuadroDatiRecordFondo

        public static void GetQuadroDatiRecordFondoByIdPensione(Int64 idPensione, out List<QuadroDatiRecordFondo> quadroDatiRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiRecordFondo = (from q in db.QuadroDatiRecordFondos
                                             where q.IdPensione == idPensione
                                             select q).ToList<QuadroDatiRecordFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetQuadroDatiRecordFondoByIdRecordFondo(Int64 idRecordFondo, out QuadroDatiRecordFondo quadroDatiRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiRecordFondo = (from q in db.QuadroDatiRecordFondos
                                             where q.IdRecordFondo == idRecordFondo
                                             select q).SingleOrDefault<QuadroDatiRecordFondo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaAllQuadroDatiRecordFondo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllQuadroDatiRecordFondo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllQuadroDatiRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuadroDatiRecordFondoByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDatiRecordFondo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDatiRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroDatiRecordFondo(QuadroDatiRecordFondo quadroDatiRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDatiRecordFondo(quadroDatiRecordFondo.IdPensione, quadroDatiRecordFondo.IdRecordFondo,quadroDatiRecordFondo.TabDatiCalcoloDZ, quadroDatiRecordFondo.TabDatiFondo, quadroDatiRecordFondo.TabDatiCalcolo,
                    quadroDatiRecordFondo.TabLegge460, quadroDatiRecordFondo.TabPrivilegiate, quadroDatiRecordFondo.TabArticolo2, quadroDatiRecordFondo.TabDatiCalcolo707, quadroDatiRecordFondo.TabMiglioramentiContrattualiFS);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDatiRecordFondo");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroDatiFondo


        #region QuadroDatiNoCalcolo

        public static void GetQuadroNoCalcoloByIdPensione(Int64 idPensione, out QuadroDatiNoCalcolo quadroDatiNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiNoCalcolo = (from q in db.QuadroDatiNoCalcolos
                                           where q.IdPensione == idPensione
                                           select q).SingleOrDefault<QuadroDatiNoCalcolo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroDatiNoCalcolo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDatiNoCalcolo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDatiNoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroDatiNoCalcolo(QuadroDatiNoCalcolo quadroDatiNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDatiNoCalcolo(quadroDatiNoCalcolo.IdPensione, quadroDatiNoCalcolo.Tipo, quadroDatiNoCalcolo.TabRegistrazioniNoCalcolo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDatiNoCalcolo");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroDatiNoCalcolo

        #region QuadroRecordNoCalcolo
        public static void GetQuadroDatiRecordNoCalcoloByIdPensione(Int64 idPensione, out List<QuadroDatiRecordNoCalcolo> quadroDatiRecordNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiRecordNoCalcolo = (from q in db.QuadroDatiRecordNoCalcolos
                                                 where q.IdPensione == idPensione
                                                 select q).ToList<QuadroDatiRecordNoCalcolo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void GetQuadroDatiRecordNoCalcoloByIdRecord(Int64 idRecord, out QuadroDatiRecordNoCalcolo quadroDatiRecordNoCalcolo)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroDatiRecordNoCalcolo = (from q in db.QuadroDatiRecordNoCalcolos
                                                 where q.IdRecordDatiNoCalcolo == idRecord
                                                 select q).SingleOrDefault<QuadroDatiRecordNoCalcolo>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaAllQuadroRecordNoCalcolo(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllQuadroDatiRecordNoCalcolo(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllQuadroDatiRecordFondo");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaQuadroRecordNoCalcoloByIdRecord(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroDatiRecordNoCalcolo(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroDatiRecordNoCalcolo");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroRecordNoCalcolo(QuadroDatiRecordNoCalcolo quadroDatiRecordNocalcolo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroDatiRecordNoCalcolo(quadroDatiRecordNocalcolo.IdPensione, quadroDatiRecordNocalcolo.IdRecordDatiNoCalcolo, quadroDatiRecordNocalcolo.TabNoCalcolo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroDatiRecordNoCalcolo");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroRecordNoCalcolo

        #region QuadroDatiPeriodi
        public static void GetQuadroPeriodiByIdPensione(Int64 idPensione, out QuadroPeriodi quadroPeriodi)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroPeriodi = (from q in db.QuadroPeriodis
                                     where q.IdPensione == idPensione
                                     select q).SingleOrDefault<QuadroPeriodi>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroPeriodi(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroPeriodi(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroPeriodi");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroPeriodi(QuadroPeriodi quadroPeriodi)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroPeriodi(quadroPeriodi.IdPensione, quadroPeriodi.Tipo, quadroPeriodi.TabPeriodi);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroPeriodi");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroDatiPeriodi

        #region QuadroDatiAventiDiritto
        public static void GetQuadroAventiDirittoByIdPensione(Int64 idPensione, out QuadroAventiDiritto quadroAventiDiritto)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroAventiDiritto = (from q in db.QuadroAventiDirittos
                                           where q.IdPensione == idPensione
                                           select q).SingleOrDefault<QuadroAventiDiritto>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroAventiDiritto(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroAventiDiritto(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroAventiDiritto");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroAventiDiritto(QuadroAventiDiritto quadroAventiDiritto)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroAventiDiritto(quadroAventiDiritto.IdPensione, quadroAventiDiritto.Tipo, quadroAventiDiritto.TabAventiDiritto);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroAventiDiritto");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroDatiAventiDiritto

        #region QuadroDatiAltreDomandeCollegate
        public static void GetQuadroAltreDomandeCollegateByIdPensione(Int64 idPensione, out QuadroAltreDomandeCollegate quadroAltreDomandeCollegate)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroAltreDomandeCollegate = (from q in db.QuadroAltreDomandeCollegates
                                                   where q.IdPensione == idPensione
                                                   select q).SingleOrDefault<QuadroAltreDomandeCollegate>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroAltreDomandeCollegate(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroAltreDomandeCollegate(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroAltreDomandeCollegate");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroAltreDomandeCollegate(QuadroAltreDomandeCollegate quadroAltreDomandeCollegate)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroAltreDomandeCollegate(quadroAltreDomandeCollegate.IdPensione, quadroAltreDomandeCollegate.Tipo, quadroAltreDomandeCollegate.TabAltreDomandeCollegate);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroAltreDomandeCollegate");
                }
                db.Connection.Close();
            }
        }
        #endregion QuadroDatiAltreDomandeCollegate

        #region QuadroRichiestaBonus

        public static void GetQuadroRichiestaBonusByIdPensione(Int64 idPensione, out QuadroRichiestaBonus quadroRichiestaBonus)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    quadroRichiestaBonus = (from q in db.QuadroRichiestaBonus
                                            where q.IdPensione == idPensione
                                            select q).SingleOrDefault<QuadroRichiestaBonus>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaQuadroRichiestaBonus(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteQuadroRichiestaBonus(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteQuadroRichiestaBonus");
                }
                db.Connection.Close();
            }
        }

        public static void SalvaQuadroRichiestaBonus(QuadroRichiestaBonus quadroRichiestaBonus)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertQuadroRichiestaBonus(quadroRichiestaBonus.IdPensione, quadroRichiestaBonus.Tipo, quadroRichiestaBonus.TabRichiestaBonus, quadroRichiestaBonus.TabEsitoPrenotazione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertQuadroRichiestaBonus");
                }
                db.Connection.Close();
            }
        }

        #endregion QuadroRichiestaBonus
    }
}
