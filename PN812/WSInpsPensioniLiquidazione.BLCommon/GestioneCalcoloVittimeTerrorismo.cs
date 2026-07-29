using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCalcoloVittimeTerrorismo
    {
        public static void GetCalcoloVittimeTerrorismoByIdPensione(Int64 idPensione, out List<DatiCalcoloVittimeTerrorismo> datiCalcoloVittimeTerrorismo)
        {
            List<CalcoloVittimeTerrorismo> calcoloVittimeTerrorismo = null;
            datiCalcoloVittimeTerrorismo = null;
            DAGestioneCalcoloVittimeTerrorismo.GetCalcoloVittimeTerrorismoByIdPensione(idPensione, out calcoloVittimeTerrorismo);
            if (calcoloVittimeTerrorismo == null || calcoloVittimeTerrorismo.Count == 0)
                return;
            datiCalcoloVittimeTerrorismo = new List<DatiCalcoloVittimeTerrorismo>();
            foreach (CalcoloVittimeTerrorismo calcolo in calcoloVittimeTerrorismo)
            {
                DatiCalcoloVittimeTerrorismo datiCalcolo = new DatiCalcoloVittimeTerrorismo();
                Utility.ValorizzaOggetti(calcolo, datiCalcolo);
                datiCalcoloVittimeTerrorismo.Add(datiCalcolo);
            }
        }

        public static void SalvaCalcoloVittimeTerrorismo(long idPensione, DatiCalcoloVittimeTerrorismo datiCalcoloVittimeTerrorismo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CalcoloVittimeTerrorismo calcoloVittimeTerrorismo = new CalcoloVittimeTerrorismo();
                Utility.ValorizzaOggetti(datiCalcoloVittimeTerrorismo, calcoloVittimeTerrorismo);
                calcoloVittimeTerrorismo.IdPensione = idPensione;
                DAGestioneCalcoloVittimeTerrorismo.SalvaCalcoloVittimeTerrorismo(calcoloVittimeTerrorismo);
                transactionScope.Complete();
            }
        }

        public static void EliminaCalcoloVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCalcoloVittimeTerrorismo.EliminaCalcoloVittimeTerrorismoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiCalcoloVittimeTerrorismo
        {
            #region public properties

            public long? IdPensione { get; set; }
            public char? Tipo { get; set; }
            public DateTime? DecorrenzaBeneficio { get; set; }
            public long? CodiceGestioneRetr { get; set; }
            public long? CodiceGestioneContr { get; set; }
            public char? Quota { get; set; }
            public string CodiceTipoQuota { get; set; }
            public int? Settimane { get; set; }
            public decimal? RMS { get; set; }
            public char? Beneficio { get; set; }
            public decimal? Ammontare { get; set; }
            public decimal? Montante { get; set; }
            public decimal? ImportoPensione { get; set; }
            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiCalcoloVittimeTerrorismo calcoloVittimeTerrorismo = (DatiCalcoloVittimeTerrorismo)obj;
                try
                {
                    if (this.Tipo != calcoloVittimeTerrorismo.Tipo ||
                        this.DecorrenzaBeneficio != calcoloVittimeTerrorismo.DecorrenzaBeneficio ||
                        this.CodiceGestioneRetr != calcoloVittimeTerrorismo.CodiceGestioneRetr ||
                        this.CodiceGestioneContr != calcoloVittimeTerrorismo.CodiceGestioneContr ||
                        this.Quota != calcoloVittimeTerrorismo.Quota ||
                        this.CodiceTipoQuota != calcoloVittimeTerrorismo.CodiceTipoQuota ||
                        this.Settimane != calcoloVittimeTerrorismo.Settimane ||
                        this.RMS != calcoloVittimeTerrorismo.RMS ||
                        this.Beneficio != calcoloVittimeTerrorismo.Beneficio ||
                        this.Ammontare != calcoloVittimeTerrorismo.Ammontare ||
                        this.Montante != calcoloVittimeTerrorismo.Montante ||
                        this.ImportoPensione != calcoloVittimeTerrorismo.ImportoPensione
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int hash = 13;
                hash = (hash * 7) + (this.Tipo != null ? this.Tipo.GetHashCode() : 0);
                hash = (hash * 7) + (this.DecorrenzaBeneficio != null ? this.DecorrenzaBeneficio.GetHashCode() : 0);
                hash = (hash * 7) + (this.CodiceGestioneRetr != null ? this.CodiceGestioneRetr.GetHashCode() : 0);
                hash = (hash * 7) + (this.CodiceGestioneContr != null ? this.CodiceGestioneContr.GetHashCode() : 0);
                hash = (hash * 7) + (this.Quota != null ? this.Quota.GetHashCode() : 0);
                hash = (hash * 7) + (this.CodiceTipoQuota != null ? this.CodiceTipoQuota.GetHashCode() : 0);
                hash = (hash * 7) + (this.Settimane != null ? this.Settimane.GetHashCode() : 0);
                hash = (hash * 7) + (this.RMS != null ? this.RMS.GetHashCode() : 0);
                hash = (hash * 7) + (this.Beneficio != null ? this.Beneficio.GetHashCode() : 0);
                hash = (hash * 7) + (this.Ammontare != null ? this.Ammontare.GetHashCode() : 0);
                hash = (hash * 7) + (this.Montante != null ? this.Montante.GetHashCode() : 0);
                hash = (hash * 7) + (this.ImportoPensione != null ? this.ImportoPensione.GetHashCode() : 0);
                return hash;
            }
            #endregion public members
        }
        #endregion nested class
    }
}
