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
    public class DAGestioneIstruttoria
    {
        public static void GetIstruttoriaByIdPensione(Int64 idPensione, out Istruttoria istruttoria)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    istruttoria = (from i in db.Istruttorias
                                   where i.IdPensione == idPensione
                                   select i).SingleOrDefault<Istruttoria>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaIstruttoria(Istruttoria istruttoria)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertIstruttoria(istruttoria.IdPensione, istruttoria.ScadenzaRevisioneSanitaria, istruttoria.Legge44997,
                    istruttoria.CodiceMobilita, istruttoria.NRiconoscimentiInvalidita, istruttoria.NSettGodimentoAssegno, istruttoria.ClasseInvalidita1Codice,
                    istruttoria.ClasseInvalidita2Codice, istruttoria.NSettimaneOBG, istruttoria.NContributiVolontari, istruttoria.NContributiVVAnzianita,
                    istruttoria.NContributiUtiliLavoratoriAutonomi, istruttoria.NSettimaneVVDirittoLavoratoriAutonomi, istruttoria.NSettimaneVVMisuraLavoratoriAutonomi,
                    istruttoria.Requisiti781Settimane, istruttoria.AccertamentoAutomatico, istruttoria.CodiceOpzioneRiliquidazione,
                    istruttoria.DataDomandaOpzione, istruttoria.DecorrenzaOpzione, istruttoria.CodiceRequisitiParticolari, istruttoria.CodiceParticolareSoggettoDerogato,
                    istruttoria.CodiceP18PrecedentePensione, istruttoria.SedePrecedentePensione, istruttoria.CertificatoPrecedentePensione,
                    istruttoria.DecorrenzaCaricoPrecedentePensione, istruttoria.CodiceComunicazioneCampo1, istruttoria.CodiceComunicazioneCampo2,
                    istruttoria.CodiceComunicazioneCampo3, istruttoria.CodiceComunicazioneCampo4, istruttoria.CodiceDomandaRicorso,
                    istruttoria.CodiceCdCmMr, istruttoria.CodiceContrattoEquiparato, istruttoria.CodiceLivelloEquip, istruttoria.CodiceArt1Legge5990,
                    istruttoria.DecorrenzaOriginariaAltraPensione, istruttoria.ImportoAdeguataAoi, istruttoria.ImportoPagamentoAoi,
                    istruttoria.CodiceCentroOperativo, istruttoria.CodPosizioneLavoro, istruttoria.ScadenzaRevisioneAssegno,
                    istruttoria.PensioneSurroga, istruttoria.CodiceASL, istruttoria.TipoPensioneExInpdai,
                    istruttoria.RiliquidazionePostCristallizzazione, istruttoria.CodiceImporto, istruttoria.CodiceLiquidazione, istruttoria.CodiceIsola, istruttoria.ModalitaLiquidazione,
                    istruttoria.Provvisoria, istruttoria.TipoCalcoloVincenteUnicarpe, istruttoria.RiduzioneAssegno, istruttoria.CodiceAziendaEditoria, istruttoria.CodiceAziendaEditoriaPerTipo0171,
                    istruttoria.CodiceAziendaEditoriaPerTipo0179, istruttoria.TrattamentoDisagi, istruttoria.CodiceNaturaPrecedentePensione, istruttoria.FacoltaComputoPrecedentePensione, istruttoria.CodiceEnte,
                    istruttoria.CodiceAziendaEditoriaLetteraB, istruttoria.I_AGGANCIO, istruttoria.I_SETTEST, istruttoria.TipoCalcoloPrecedente, istruttoria.GP1AF08, istruttoria.NSettimaneOI);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertIstruttoria");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaIstruttoriaByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteIstruttoria(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteIstruttoria");
                }
                db.Connection.Close();
            }
        }
    }
}

