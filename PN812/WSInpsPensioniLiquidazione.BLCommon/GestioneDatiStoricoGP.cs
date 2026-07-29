using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDatiStoricoGP
    {
        public static void GetDatiStoricoGPByIdPensione(long idPensione, out DatiStoricoGP datiStoricoGP)
        {
            DataCommon.DatiStoricoGP storicoGP_DB = null;
            datiStoricoGP = null;
            DAGestioneDatiStoricoGP.GetDatiStoricoGPByIdPensione(idPensione, out storicoGP_DB);
            if (storicoGP_DB == null)
                return;
            datiStoricoGP = new DatiStoricoGP();
            Utility.ValorizzaOggetti(storicoGP_DB, datiStoricoGP);
        }

        public static void SalvaDatiStoricoGP(long idPensione, DatiStoricoGP datiStoricoGP)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.DatiStoricoGP storicoGP_DB = new DataCommon.DatiStoricoGP();
                Utility.ValorizzaOggetti(datiStoricoGP, storicoGP_DB);
                storicoGP_DB.IdPensione = idPensione;
                DAGestioneDatiStoricoGP.SalvaDatiStoricoGP(storicoGP_DB);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiStoricoGPByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiStoricoGP.EliminaDatiStoricoGPByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiStoricoGP
        {
            public long IdPensione { get; set; }
            public decimal? AnzAl95 { get; set; }
            public int? AttivitaEconomica { get; set; }
            public char? CodiceComunicazioneCampo3 { get; set; }
            public byte? CodiceMobilita { get; set; }
            public long? CodiceParticolareSoggettoDerogato { get; set; }
            public DateTime? DecorrenzaOriginaria { get; set; }
            public DateTime? FineAssicurazione { get; set; }
            public DateTime? FineUltimoLavoro { get; set; }
            public DateTime? InizioAssicurazione { get; set; }
            public DateTime? InizioUltimoLavoro { get; set; }
            public byte? Legge44997 { get; set; }
            public string ModalitaLiquidazione { get; set; }
            public int? NContributiVolontari { get; set; }
            public int? NContributiVVAnzianita { get; set; }
            public int? NSettimaneOBG { get; set; }
            public int? ProfessioneIndividuale { get; set; }
            public short? QuotaA2707 { get; set; }
            public short? QuotaA707 { get; set; }
            public decimal? QuotaAl95 { get; set; }
            public short? QuotaB707 { get; set; }
            public short? QuotaC2707 { get; set; }
            public short? QuotaC707 { get; set; }
            public short? QuotaD707 { get; set; }
            public decimal? RetribuzioneBiennio { get; set; }
            public decimal? RetribuzionePonderataAGO707 { get; set; }
            public decimal? RetribuzioneSettimanaleAgoQuotaA { get; set; }
            public decimal? RetribuzioneSettimanaleAgoQuotaB { get; set; }
            public decimal? RetribuzioneUltimoAnnoQuotaA { get; set; }
            public bool RiduzioneRetributiva { get; set; }
            public decimal? RiduzioneRetributivaPercentuale { get; set; }
            public DateTime? ScadenzaRevisioneSanitaria { get; set; }
            public byte? TipoCalcolo { get; set; }
            public char? Contributivo { get; set; }
            public DateTime? DataPerfezionamentoRequisiti { get; set; }
            public DateTime? DataFineCalcoloArretrati { get; set; }
            public decimal? ImportoLordo { get; set; }
            public string NaturaPensione { get; set; }
            public int? NSettimaneBeneficio { get; set; }
            public int? GP1ALB1 { get; set; }
            public short? GP1AXE3 { get; set; }
            public string GP2BB05 { get; set; }
            public bool? IsScadenzaAssegnoConGiorno { get; set; }
            public byte? CodiceSpecifico { get; set; }
            public DateTime? DecorrenzaMaggiorazioneSociale { get; set; }
            public DateTime? ScadenzaAssegno { get; set; }
            public short? GP1AV91H { get; set; }
            public string TipoSettimaneBeneficio { get; set; }
            public byte? NumeroFigli { get; set; }
            public DateTime? DataEliminazioneContabile { get; set; }
            public DateTime? DataRinunciaTrattenutaInpdap { get; set; }
            public byte? CodiceTipoPerequazione { get; set; }
            public decimal? VirtualePura { get; set; }
            public decimal? VirtualeIntegrata { get; set; }
            public decimal? Adeguata { get; set; }
            public DateTime? DecorrenzaOriginariaPrima { get; set; }
            public string IABTIPEN { get; set; }

            public bool IsDatiComma707Null()
            {
                if (!QuotaA707.HasValue && !QuotaA2707.HasValue && !QuotaB707.HasValue && !QuotaC707.HasValue && !QuotaC2707.HasValue && !QuotaD707.HasValue &&
                    !RetribuzionePonderataAGO707.HasValue)
                    return true;
                return false;
            }

            public short? GP1AZ11F { get; set; }
            public decimal? GP2BB06 { get; set; }
            public bool? TrattenutaFondoCredito { get; set; }
        }
        #endregion nested class
    }
}
