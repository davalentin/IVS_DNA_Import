using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDatiStoricoGP
    {
        public static void SalvaDatiStoricoGP(DatiStoricoGP datiStoricoGP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertDatiStoricoGP(datiStoricoGP.IdPensione, datiStoricoGP.AnzAl95, datiStoricoGP.AttivitaEconomica, datiStoricoGP.CodiceComunicazioneCampo3, datiStoricoGP.CodiceMobilita,
                    datiStoricoGP.CodiceParticolareSoggettoDerogato, datiStoricoGP.DecorrenzaOriginaria, datiStoricoGP.FineAssicurazione, datiStoricoGP.FineUltimoLavoro, datiStoricoGP.InizioAssicurazione,
                    datiStoricoGP.InizioUltimoLavoro, datiStoricoGP.Legge44997, datiStoricoGP.ModalitaLiquidazione, datiStoricoGP.NContributiVolontari, datiStoricoGP.NContributiVVAnzianita,
                    datiStoricoGP.NSettimaneOBG, datiStoricoGP.ProfessioneIndividuale, datiStoricoGP.QuotaA2707, datiStoricoGP.QuotaA707, datiStoricoGP.QuotaAl95, datiStoricoGP.QuotaB707,
                    datiStoricoGP.QuotaC2707, datiStoricoGP.QuotaC707, datiStoricoGP.QuotaD707, datiStoricoGP.RetribuzioneBiennio, datiStoricoGP.RetribuzionePonderataAGO707,
                    datiStoricoGP.RetribuzioneSettimanaleAgoQuotaA, datiStoricoGP.RetribuzioneSettimanaleAgoQuotaB, datiStoricoGP.RetribuzioneUltimoAnnoQuotaA, datiStoricoGP.RiduzioneRetributiva,
                    datiStoricoGP.RiduzioneRetributivaPercentuale, datiStoricoGP.ScadenzaRevisioneSanitaria, datiStoricoGP.TipoCalcolo, datiStoricoGP.Contributivo, datiStoricoGP.DataPerfezionamentoRequisiti,
                    datiStoricoGP.DataFineCalcoloArretrati, datiStoricoGP.ImportoLordo, datiStoricoGP.NaturaPensione, datiStoricoGP.NSettimaneBeneficio, datiStoricoGP.GP1ALB1, datiStoricoGP.CodiceSpecifico,
                    datiStoricoGP.GP1AXE3, datiStoricoGP.DecorrenzaMaggiorazioneSociale, datiStoricoGP.GP2BB05, datiStoricoGP.IsScadenzaAssegnoConGiorno, datiStoricoGP.ScadenzaAssegno, datiStoricoGP.GP1AV91H,
                    datiStoricoGP.GP1AZ11F, datiStoricoGP.TipoSettimaneBeneficio, datiStoricoGP.NumeroFigli, datiStoricoGP.DataEliminazioneContabile, datiStoricoGP.DataRinunciaTrattenutaInpdap, datiStoricoGP.GP2BB06,
                    datiStoricoGP.CodiceTipoPerequazione, datiStoricoGP.VirtualePura, datiStoricoGP.VirtualeIntegrata,  datiStoricoGP.Adeguata, datiStoricoGP.DecorrenzaOriginariaPrima, datiStoricoGP.TrattenutaFondoCredito, datiStoricoGP.IABTIPEN);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertDatiStoricoGP");

                db.Connection.Close();
            }
        }

        public static void EliminaDatiStoricoGPByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteDatiStoricoGP(idPensione);
                if (result != 0)
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteDatiStoricoGP");

                db.Connection.Close();
            }
        }


        public static void GetDatiStoricoGPByIdPensione(long idPensione, out DatiStoricoGP datiStoricoGP)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    datiStoricoGP = (from elem in db.DatiStoricoGPs
                                     where elem.IdPensione == idPensione
                                     select elem).SingleOrDefault();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
    }
}
