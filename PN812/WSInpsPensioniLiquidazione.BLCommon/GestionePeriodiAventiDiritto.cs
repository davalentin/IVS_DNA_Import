using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestionePeriodiAventiDiritto
    {
        // GET
        /// <summary>
        /// Recupera le informazioni dei periodi di parentela con il dante causa per le domande di spacchettamento (tabella PeriodiAventiDiritto)
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaPeriodiAventiDiritto"></param>
        public static void GetPeriodiAventiDiritto(long idPensione, long? idAventeDiritto, out List<PeriodoAventiDiritto> listaPeriodiAventiDiritto)
        {
            listaPeriodiAventiDiritto = new List<PeriodoAventiDiritto>();
            List<PeriodiAventiDiritto> listaPeriodiAventiDirittoDB = null;

            Expression<Func<PeriodiAventiDiritto, bool>> whereCondition = (p) => p.IdPensione == idPensione;
            if (idAventeDiritto.HasValue)
            {
                Expression<Func<PeriodiAventiDiritto, bool>> predicateFiltroAventeDiritto = p => p.IdAventeDiritto == idAventeDiritto.Value;
                whereCondition = whereCondition.And(predicateFiltroAventeDiritto);
            }

            DAGestionePeriodiAventiDiritto.GetPeriodiAventiDirittoByIdPensione(whereCondition, out listaPeriodiAventiDirittoDB);
            if (listaPeriodiAventiDirittoDB != null && listaPeriodiAventiDirittoDB.Count > 0)
            {
                foreach (PeriodiAventiDiritto periodoAventiDirittoDB in listaPeriodiAventiDirittoDB)
                    listaPeriodiAventiDiritto.Add(new PeriodoAventiDiritto(periodoAventiDirittoDB));
            }
        }

        // SAVE
        /// <summary>
        /// Salva le informazioni inerenti ai periodi di parentela con il dante causa per le domande di spacchettamento (tabella PeriodiAventiDiritto)
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="listaPeriodiAventiDiritto"></param>
        public static void SavePeriodiAventiDiritto(long idPensione, List<PeriodoAventiDiritto> listaPeriodiAventiDiritto)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                List<long> listaIdAventeDiritto = listaPeriodiAventiDiritto.GroupBy(x => x.IdAventeDiritto).Select(y => y.First().IdAventeDiritto).ToList();

                foreach (long idAventeDiritto in listaIdAventeDiritto)
                    DAGestionePeriodiAventiDiritto.DeleteAllPeriodiAventiDirittoByIdAventeDiritto(idAventeDiritto);

                if (listaPeriodiAventiDiritto != null && listaPeriodiAventiDiritto.Count > 0)
                {
                    foreach (PeriodoAventiDiritto periodoAventiDiritto in listaPeriodiAventiDiritto)
                    {
                        PeriodiAventiDiritto periodiAventiDirittoDB = new PeriodiAventiDiritto();
                        Utility.ValorizzaOggetti(periodoAventiDiritto, periodiAventiDirittoDB);
                        periodiAventiDirittoDB.IdPensione = idPensione;
                        DAGestionePeriodiAventiDiritto.SalvaPeriodoAventiDiritto(periodiAventiDirittoDB);
                    }
                }

                transactionScope.Complete();
            }
        }

        // DELETE
        /// <summary>
        /// Elimina le informazioni inerenti ai periodi di parentela con il dante causa per le domande di spacchettamento (tabella PeriodiAventiDiritto)
        /// </summary>
        /// <param name="idPensione"></param>
        public static void DeleteAllPeriodiAventiDiritto(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePeriodiAventiDiritto.DeleteAllPeriodiAventiDirittoByIdPensione(idPensione);

                transactionScope.Complete();
            }
        }

        /// <summary>
        /// Elimina le informazioni inerenti ai periodi di parentela con il dante causa per le domande di spacchettamento riferite ad un determinato AventeDiritto (tabella PeriodiAventiDiritto)
        /// </summary>
        /// <param name="idPensione"></param>
        /// <param name="idAventeDiritto"></param>
        public static void DeletePeriodiAventiDirittoByIdAventeDiritto(long idAventeDiritto)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestionePeriodiAventiDiritto.DeleteAllPeriodiAventiDirittoByIdAventeDiritto(idAventeDiritto);

                transactionScope.Complete();
            }
        }

        #region nested classes
        public class PeriodoAventiDiritto
        {
            #region private properties
            private string _TipoUnione;
            #endregion private properties

            #region public properties
            public long Id { get; set; }
            public long IdPensione { get; set; }
            public long IdAventeDiritto { get; set; }
            public char? GradoParentela { get; set; }
            public string TipoUnione { get { return this._TipoUnione ?? string.Empty; } set { _TipoUnione = value ?? string.Empty; } }
            public DateTime? DecorrenzaPeriodo { get; set; }
            public DateTime? CessazionePeriodo { get; set; }
            public decimal? PercSpettante { get; set; }
            public decimal? CoeffRiduzione { get; set; }
            public decimal? PercGiudice { get; set; }
            public bool IsFromWebDom { get; set; }
            public bool? IsFromGP { get; set; }
            #endregion public properties

            #region public methods
            public PeriodoAventiDiritto() { }

            public PeriodoAventiDiritto(long idPensione, long idAventeDiritto, char? gradoParentela, string tipoUnione, DateTime? decorrenzaPeriodo, DateTime? cessazionePeriodo, decimal? percSpettante, decimal? coeffRiduzione,
                decimal? percGiudice, bool isFromWebDom, bool? isFromGP)
            {
                this.IdPensione = idPensione;
                this.IdAventeDiritto = idAventeDiritto;
                this.GradoParentela = gradoParentela;
                this.TipoUnione = tipoUnione;
                this.DecorrenzaPeriodo = decorrenzaPeriodo;
                this.CessazionePeriodo = cessazionePeriodo;
                this.PercSpettante = percSpettante;
                this.CoeffRiduzione = coeffRiduzione;
                this.PercGiudice = percGiudice;
                this.IsFromWebDom = isFromWebDom;
                this.IsFromGP = isFromGP;
            }

            public PeriodoAventiDiritto(PeriodiAventiDiritto periodoAventiDiritto)
            {
                this.Id = periodoAventiDiritto.Id;
                this.IdPensione = periodoAventiDiritto.IdPensione;
                this.IdAventeDiritto = periodoAventiDiritto.IdAventeDiritto;
                this.GradoParentela = periodoAventiDiritto.GradoParentela;
                this.TipoUnione = periodoAventiDiritto.TipoUnione;
                this.DecorrenzaPeriodo = periodoAventiDiritto.DecorrenzaPeriodo;
                this.CessazionePeriodo = periodoAventiDiritto.CessazionePeriodo;
                this.PercSpettante = periodoAventiDiritto.PercSpettante;
                this.CoeffRiduzione = periodoAventiDiritto.CoeffRiduzione;
                this.PercGiudice = periodoAventiDiritto.PercGiudice;
                this.IsFromWebDom = periodoAventiDiritto.IsFromWebDom;
                this.IsFromGP = periodoAventiDiritto.IsFromGP;
            }

            public override bool Equals(object obj)
            {
                PeriodoAventiDiritto periodoAventiDiritto = (PeriodoAventiDiritto)obj;
                try
                {
                    if (this.CessazionePeriodo != periodoAventiDiritto.CessazionePeriodo ||
                        this.CoeffRiduzione != periodoAventiDiritto.CoeffRiduzione ||
                        this.DecorrenzaPeriodo != periodoAventiDiritto.DecorrenzaPeriodo ||
                        this.GradoParentela != periodoAventiDiritto.GradoParentela ||
                        this.TipoUnione != periodoAventiDiritto.TipoUnione ||
                        this.PercSpettante != periodoAventiDiritto.PercSpettante ||
                        this.PercGiudice != periodoAventiDiritto.PercGiudice ||
                        this.IsFromWebDom != periodoAventiDiritto.IsFromWebDom ||
                        this.IsFromGP != periodoAventiDiritto.IsFromGP
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            #endregion public methods
        }
        #endregion nested classes
    }
}
