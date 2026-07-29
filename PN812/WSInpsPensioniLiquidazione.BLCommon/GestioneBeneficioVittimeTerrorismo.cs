using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneBeneficioVittimeTerrorismo
    {
        public static void GetBeneficioVittimeTerrorismoByIdPensione(Int64 idPensione, out DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            BeneficioVittimeTerrorismo beneficioVittimeTerrorismo = null;
            datiBeneficioVittimeTerrorismo = null;
            DAGestioneBeneficioVittimeTerrorismo.GetBeneficioVittimeTerrorismoByIdPensione(idPensione, out beneficioVittimeTerrorismo);
            if (beneficioVittimeTerrorismo == null)
                return;
            datiBeneficioVittimeTerrorismo = new DatiBeneficioVittimeTerrorismo();
            Utility.ValorizzaOggetti(beneficioVittimeTerrorismo, datiBeneficioVittimeTerrorismo);
        }

        public static void SalvaBeneficioVittimeTerrorismo(long idPensione, DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                BeneficioVittimeTerrorismo beneficioVittimeTerrorismo = new BeneficioVittimeTerrorismo();
                Utility.ValorizzaOggetti(datiBeneficioVittimeTerrorismo, beneficioVittimeTerrorismo);
                beneficioVittimeTerrorismo.IdPensione = idPensione;
                DAGestioneBeneficioVittimeTerrorismo.SalvaBeneficioVittimeTerrorismo(beneficioVittimeTerrorismo);
                transactionScope.Complete();
            }
        }

        public static void EliminaBeneficioVittimeTerrorismoByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneBeneficioVittimeTerrorismo.EliminaBeneficioVittimeTerrorismoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #region nested class
        public class DatiBeneficioVittimeTerrorismo
        {
            #region public properties

            public long? IdPensione { get; set; }
            public long? SoggettoBeneficiario { get; set; }
            public char? CodiceEvento { get; set; }
            public DateTime? DataEventoTerroristico { get; set; }
            public long? TipologiaPrestazione { get; set; }
            public long? TipologiaBeneficio { get; set; }
            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiBeneficioVittimeTerrorismo beneficioVittimeTerrorismo = (DatiBeneficioVittimeTerrorismo)obj;
                try
                {
                    if (this.SoggettoBeneficiario != beneficioVittimeTerrorismo.SoggettoBeneficiario ||
                        this.CodiceEvento != beneficioVittimeTerrorismo.CodiceEvento ||
                        this.DataEventoTerroristico != beneficioVittimeTerrorismo.DataEventoTerroristico ||
                        this.TipologiaPrestazione != beneficioVittimeTerrorismo.TipologiaPrestazione ||
                        this.TipologiaBeneficio != beneficioVittimeTerrorismo.TipologiaBeneficio
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
                hash = (hash * 7) + (this.SoggettoBeneficiario != null ? this.SoggettoBeneficiario.GetHashCode() : 0);
                hash = (hash * 7) + (this.CodiceEvento != null ? this.CodiceEvento.GetHashCode() : 0);
                hash = (hash * 7) + (this.DataEventoTerroristico != null ? this.DataEventoTerroristico.GetHashCode() : 0);
                hash = (hash * 7) + (this.TipologiaPrestazione != null ? this.TipologiaPrestazione.GetHashCode() : 0);
                hash = (hash * 7) + (this.TipologiaBeneficio != null ? this.TipologiaBeneficio.GetHashCode() : 0);
                return hash;
            }
            #endregion public members
        }
        #endregion nested class
    }
}
