using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneRecordDatiFondoINPDAP
    {
        public static void GetRecordDatiFondoINPDAPByIdPensione(long idPensione, out List<RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP)
        {
            listaRecordDatiFondoINPDAP = null;

            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                listaRecordDatiFondoINPDAP = (from c in db.RecordDatiFondoINPDAPs where c.IdPensione == idPensione select c).ToList<RecordDatiFondoINPDAP>();
                db.Connection.Close();
            }
        }

        public static void GetRecordDatiFondoINPDAPByIdRecordFondo(long idRecordFondo, out RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            recordDatiFondoINPDAP = null;

            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                recordDatiFondoINPDAP = (from c in db.RecordDatiFondoINPDAPs where c.IdRecordFondo == idRecordFondo select c).FirstOrDefault();
                db.Connection.Close();
            }
        }

        public static void SalvaRecordDatiFondoINPDAP(RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertRecordDatiFondoINPDAP(recordDatiFondoINPDAP.IdPensione, recordDatiFondoINPDAP.IdRecordFondo, recordDatiFondoINPDAP.DecorrenzaCalcolo, recordDatiFondoINPDAP.TrediciMensilita,
                    recordDatiFondoINPDAP.IntegrazioneMinimo, recordDatiFondoINPDAP.IndennitaIntegrativaSpecialeConglobata, recordDatiFondoINPDAP.PensioneAnnuaLorda, recordDatiFondoINPDAP.ServizioUtileDirittoAA,
                    recordDatiFondoINPDAP.ServizioUtileDirittoMM, recordDatiFondoINPDAP.ServizioUtileDirittoGG, recordDatiFondoINPDAP.RMSSenzaLegge33670QA, recordDatiFondoINPDAP.IndennitaAusiliaria, 
                    recordDatiFondoINPDAP.IndennitaParaplegici, recordDatiFondoINPDAP.IndennitaSpeciale, recordDatiFondoINPDAP.ScadenzaBenefici, recordDatiFondoINPDAP.PALConBenefici, 
                    recordDatiFondoINPDAP.PensioneAnnuaLorda707, recordDatiFondoINPDAP.Divisore, recordDatiFondoINPDAP.Capitolo, recordDatiFondoINPDAP.SiglaCategoria, recordDatiFondoINPDAP.CodiceSede,
                    recordDatiFondoINPDAP.Ncertificato, recordDatiFondoINPDAP.NMesiRiscattati, recordDatiFondoINPDAP.NMesiTotali, recordDatiFondoINPDAP.DecorrenzaSecondaria, 
                    recordDatiFondoINPDAP.CoefficienteTrasformazione, recordDatiFondoINPDAP.TitolareAltraPensione, recordDatiFondoINPDAP.ScadenzaIllimitata, recordDatiFondoINPDAP.NumeroRate, recordDatiFondoINPDAP.ImportoSingolaRata,
                    recordDatiFondoINPDAP.PrivilegiataSuperinvaliditaIndennita, recordDatiFondoINPDAP.AssegnoIntegrativo, recordDatiFondoINPDAP.IntegrazioneIndennitaAssistenza, recordDatiFondoINPDAP.IndennitaAccompagnamentoAggiuntiva, recordDatiFondoINPDAP.CumuloInfermita,
                    recordDatiFondoINPDAP.Categoria2aInfermita, recordDatiFondoINPDAP.AssegnoCura, recordDatiFondoINPDAP.IndennitaSpecialeAnnua, recordDatiFondoINPDAP.EnteEquoInd, recordDatiFondoINPDAP.ImpEquoInd, recordDatiFondoINPDAP.CodInd, recordDatiFondoINPDAP.DataInizioInd,
                    recordDatiFondoINPDAP.ImpInd, recordDatiFondoINPDAP.DataCessInd, recordDatiFondoINPDAP.ImpRataIniz, recordDatiFondoINPDAP.ImpRataOrd, recordDatiFondoINPDAP.ImpRataFin, recordDatiFondoINPDAP.NumRate, recordDatiFondoINPDAP.ServizioUtileDirittoOIAA, 
                    recordDatiFondoINPDAP.ServizioUtileDirittoOIMM, recordDatiFondoINPDAP.ServizioUtileDirittoOIGG);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertRecordDatiFondoINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteAllRecordDatiFondoINPDAPByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteAllRecordDatiFondoINPDAP(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteAllRecordDatiFondoINPDAP");
                }
                db.Connection.Close();
            }
        }

        public static void DeleteRecordDatiFondoINPDAPByIdRecordFondo(long idRecordFondo)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteRecordDatiFondoINPDAP(idRecordFondo);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeleteRecordDatiFondoINPDAP");
                }
                db.Connection.Close();
            }
        }
    }
}
