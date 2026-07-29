using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneRecordDatiFondoINPDAP
    {
        public static void GetRecordDatiFondoINPDAPByIdRecordFondo(long idRecordFondo, out RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            DataCommon.RecordDatiFondoINPDAP recordDatiFondoINPDAP_DB = null;
            recordDatiFondoINPDAP = null;
            DAGestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo, out recordDatiFondoINPDAP_DB);
            if (recordDatiFondoINPDAP_DB == null)
                return;
            recordDatiFondoINPDAP = new RecordDatiFondoINPDAP();
            Utility.ValorizzaOggetti(recordDatiFondoINPDAP_DB, recordDatiFondoINPDAP);
        }

        public static void GetRecordDatiFondoINPDAPByIdPensione(long idPensione, out List<RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP)
        {
            List<DataCommon.RecordDatiFondoINPDAP> listaRecordDatiFondoINPDAP_DB = null;
            listaRecordDatiFondoINPDAP = null;
            DAGestioneRecordDatiFondoINPDAP.GetRecordDatiFondoINPDAPByIdPensione(idPensione, out listaRecordDatiFondoINPDAP_DB);
            if (listaRecordDatiFondoINPDAP_DB == null || listaRecordDatiFondoINPDAP_DB.Count == 0)
                return;
            listaRecordDatiFondoINPDAP = new List<RecordDatiFondoINPDAP>();
            foreach (DataCommon.RecordDatiFondoINPDAP recordDB in listaRecordDatiFondoINPDAP_DB)
            {
                RecordDatiFondoINPDAP recordDatiFondoINPDAP = new RecordDatiFondoINPDAP();
                Utility.ValorizzaOggetti(recordDB, recordDatiFondoINPDAP);
                listaRecordDatiFondoINPDAP.Add(recordDatiFondoINPDAP);
            }
        }

        public static void SalvaRecordDatiFondoINPDAP(long idPensione, long idRecordFondo, RecordDatiFondoINPDAP recordDatiFondoINPDAP)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.RecordDatiFondoINPDAP recordDatiFondoINPDAP_DB = new DataCommon.RecordDatiFondoINPDAP();
                Utility.ValorizzaOggetti(recordDatiFondoINPDAP, recordDatiFondoINPDAP_DB);
                recordDatiFondoINPDAP_DB.IdPensione = idPensione;
                recordDatiFondoINPDAP_DB.IdRecordFondo = idRecordFondo;
                DAGestioneRecordDatiFondoINPDAP.SalvaRecordDatiFondoINPDAP(recordDatiFondoINPDAP_DB);
                transactionScope.Complete();
            }
        }

        public static void EliminaAllRecordDatiFondoINPDAP(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRecordDatiFondoINPDAP.DeleteAllRecordDatiFondoINPDAPByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaRecordDatiFondoINPDAPByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneRecordDatiFondoINPDAP.DeleteRecordDatiFondoINPDAPByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class RecordDatiFondoINPDAP
        {
            public long IdPensione { get; set; }
            public long IdRecordFondo { get; set; }
            public DateTime? DecorrenzaCalcolo { get; set; }
            public bool? TrediciMensilita { get; set; }
            public bool? IntegrazioneMinimo { get; set; }
            public bool? IndennitaIntegrativaSpecialeConglobata { get; set; }
            public decimal? PensioneAnnuaLorda { get; set; }
            public short? ServizioUtileDirittoAA { get; set; }
            public short? ServizioUtileDirittoMM { get; set; }
            public short? ServizioUtileDirittoGG { get; set; }
            public decimal? RMSSenzaLegge33670QA { get; set; }
            public bool? IndennitaAusiliaria { get; set; }
            public bool? IndennitaParaplegici { get; set; }
            public bool? IndennitaSpeciale { get; set; }
            public DateTime? ScadenzaBenefici { get; set; }
            public decimal? PALConBenefici { get; set; }
            public decimal? PensioneAnnuaLorda707 { get; set; }
            public byte? Divisore { get; set; }
            public string Capitolo { get; set; }
            public int? SiglaCategoria { get; set; }
            public short? CodiceSede { get; set; }
            public int? Ncertificato { get; set; }
            public int? NMesiRiscattati { get; set; }
            public int? NMesiTotali { get; set; }
            public DateTime? DecorrenzaSecondaria { get; set; }
            public decimal? CoefficienteTrasformazione { get; set; }
            public bool? TitolareAltraPensione { get; set; }
            public bool? ScadenzaIllimitata { get; set; }
            public int? NumeroRate { get; set; }
            public decimal? ImportoSingolaRata { get; set; }
            public int? PrivilegiataSuperinvaliditaIndennita { get; set; }
            public int? AssegnoIntegrativo { get; set; }
            public int? IntegrazioneIndennitaAssistenza { get; set; }
            public int? IndennitaAccompagnamentoAggiuntiva { get; set; }
            public int? CumuloInfermita { get; set; }
            public int? Categoria2aInfermita { get; set; }
            public int? AssegnoCura { get; set; }
            public int? IndennitaSpecialeAnnua { get; set; }
            public string EnteEquoInd { get; set; }
            public decimal? ImpEquoInd { get; set; }
            public string CodInd { get; set; }
            public DateTime?  DataInizioInd { get; set; }
            public decimal? ImpInd { get; set; }
            public DateTime? DataCessInd { get; set; }
            public decimal? ImpRataIniz{ get; set; }
            public decimal? ImpRataOrd { get; set; }
            public decimal? ImpRataFin { get; set; }
            public int? NumRate { get; set; }
            public short? ServizioUtileDirittoOIAA { get; set; }
            public short? ServizioUtileDirittoOIMM { get; set; }
            public short? ServizioUtileDirittoOIGG { get; set; }

            public bool IsLegge460Null()
            {
                if (!SiglaCategoria.HasValue && !CodiceSede.HasValue &&
                    !Ncertificato.HasValue && !NMesiRiscattati.HasValue &&
                    !NMesiTotali.HasValue && !DecorrenzaSecondaria.HasValue)
                    return true;

                return false;
            }
        }
        #endregion nested class
    }
}
