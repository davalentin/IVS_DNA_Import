using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestionePensioneINPDAP
    {
        public static void GetPensioneINPDAPRecordFondoByIdPensione(long idPensione, out List<DatiPensioneINPDAP> listaDatiPensioneINPDAP)
        {
            List<PensioneINPDAP> listaPensioneINPDAP = null;
            listaDatiPensioneINPDAP = null;
            DAGestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(idPensione, out listaPensioneINPDAP);
            if (listaPensioneINPDAP == null || listaPensioneINPDAP.Count == 0)
                return;
            listaDatiPensioneINPDAP = new List<DatiPensioneINPDAP>();
            foreach (PensioneINPDAP pensioneINPDAP in listaPensioneINPDAP)
            {
                DatiPensioneINPDAP datiPensioneINPDAP = new DatiPensioneINPDAP();
                Utility.ValorizzaOggetti(pensioneINPDAP, datiPensioneINPDAP);
                listaDatiPensioneINPDAP.Add(datiPensioneINPDAP);
            }
        }

        public static void GetPensioneINPDAPByIdRecordFondo(long idRecordFondo, out DatiPensioneINPDAP datiPensioneINPDAP)
        {
            PensioneINPDAP pensioneINPDAP = null;
            datiPensioneINPDAP = null;
            DAGestionePensioneINPDAP.GetPensioneINPDAPByIdRecordFondo(idRecordFondo, out pensioneINPDAP);
            if (pensioneINPDAP == null)
                return;
            datiPensioneINPDAP = new DatiPensioneINPDAP();
            Utility.ValorizzaOggetti(pensioneINPDAP, datiPensioneINPDAP);
        }

        public static void SalvaPensioneINPDAPRecordFondo(long idPensione, long idRecordFondo, DatiPensioneINPDAP datiPensioneINPDAP)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneINPDAP pensioneINPDAP = new PensioneINPDAP();
                Utility.ValorizzaOggetti(datiPensioneINPDAP, pensioneINPDAP);
                pensioneINPDAP.IdPensione = idPensione;
                pensioneINPDAP.IdRecordFondo = idRecordFondo;
                DAGestionePensioneINPDAP.SalvaPensioneINPDAPRecordFondo(pensioneINPDAP);
                transactionScope.Complete();
            }
        }

        public static void EliminaPensioneINPDAP(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneINPDAP.EliminaPensioneINPDAPByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaPensioneINPDAPByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePensioneINPDAP.EliminaPensioneINPDAPByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #region nested classes
        public class DatiPensioneINPDAP
        {
            #region public properties
            public long? IdPensione { get; set; }
            public long? IdRecordFondo { get; set; }
            public DateTime? DecorrenzaEconomica { get; set; }
            public bool? RequisitiAnte247 { get; set; }
            public byte? TrimesteRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
            public decimal? AliquotaMediaINPDAP { get; set; }
            public DateTime? DataRivalsaINPDAP { get; set; }
            public long? CausaCessazione { get; set; }
            public bool? TitolareAltraPensione { get; set; }
            public bool? DirittoIndennitaIntegrativaSpeciale { get; set; }
            public bool? RiduzioneL537 { get; set; }
            public bool? IISAbbattimentoAnni { get; set; }
            public short? VVUtiliDirittoAA { get; set; }
            public byte? VVUtiliDirittoMM { get; set; }
            public byte? VVUtiliDirittoGG { get; set; }
            public short? VVUtiliMisuraAA { get; set; }
            public byte? VVUtiliMisuraMM { get; set; }
            public byte? VVUtiliMisuraGG { get; set; }
            public long? Microqualifica { get; set; }
            public byte? AnniMax { get; set; }
            public byte? AnniUtili { get; set; }
            public int? Comparto { get; set; }
            public int? Settore { get; set; }
            public int? Ruolo { get; set; }
            public string CfAmministrazione { get; set; }
            public  string ProgAmministrazione { get; set; }
            #endregion public properties

            #region public methods
            public bool IsNull()
            {
                if (this.DecorrenzaEconomica.HasValue ||
                    this.RequisitiAnte247.HasValue ||
                    this.TrimesteRequisiti.HasValue ||
                    this.AnnoRequisiti.HasValue ||
                    this.AnzianitaAnni.HasValue ||
                    this.AliquotaMediaINPDAP.HasValue ||
                    this.DataRivalsaINPDAP.HasValue ||
                    this.CausaCessazione.HasValue ||
                    this.TitolareAltraPensione.HasValue ||
                    this.DirittoIndennitaIntegrativaSpeciale.HasValue ||
                    this.RiduzioneL537.HasValue ||
                    this.IISAbbattimentoAnni.HasValue ||
                    this.VVUtiliDirittoAA.HasValue ||
                    this.VVUtiliDirittoMM.HasValue ||
                    this.VVUtiliDirittoGG.HasValue ||
                    this.VVUtiliMisuraAA.HasValue ||
                    this.VVUtiliMisuraMM.HasValue ||
                    this.VVUtiliMisuraGG.HasValue ||
                    this.Microqualifica.HasValue ||
                    this.AnniMax.HasValue ||
                    this.AnniUtili.HasValue)
                    return false;

                return true;
            }
            #endregion public methods
        }

        #endregion nested classes
    }
}
