using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using System.Data.Common;
using System.Data;
using INPS.DNA.Data;
using System.Data.SqlClient;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneTipologieAutomaticheUnicarpe
    {
        public static void GetAllTipologieAutomaticheUnicarpe(out List<TipologiaAutomaticaUnicarpe> listaDatiTipologieAutomaticheUnicarpe)
        {
            listaDatiTipologieAutomaticheUnicarpe = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    using (DbConnection dbConnection = ConnectionFactory.GetConnection("UnicarpeConnectionString"))
                    {
                        try
                        {
                            DbCommand dbCommand = dbConnection.CreateCommand();
                            dbCommand.CommandText = "dbo.GET_TABLE_MEMO6";
                            dbCommand.CommandType = System.Data.CommandType.StoredProcedure;

                            dbConnection.Open();
                            DbDataReader dbDataReader = dbCommand.ExecuteReader();
                            while (dbDataReader.Read())
                            {
                                if (listaDatiTipologieAutomaticheUnicarpe == null)
                                    listaDatiTipologieAutomaticheUnicarpe = new List<TipologiaAutomaticaUnicarpe>();

                                TipologiaAutomaticaUnicarpe tipologiaAutomaticaUnicarpe = new TipologiaAutomaticaUnicarpe();
                                tipologiaAutomaticaUnicarpe.SiglaCategoria = dbDataReader["Categoria"] != DBNull.Value ? dbDataReader["Categoria"].ToString() : null;
                                tipologiaAutomaticaUnicarpe.Gruppo = dbDataReader["Prodotto"] != DBNull.Value ? dbDataReader["Prodotto"].ToString().Substring(0, 4) : null;
                                tipologiaAutomaticaUnicarpe.Prodotto = dbDataReader["Prodotto"] != DBNull.Value ? dbDataReader["Prodotto"].ToString().Substring(4, 4) : null;
                                tipologiaAutomaticaUnicarpe.Tipo = dbDataReader["Prodotto"] != DBNull.Value ? dbDataReader["Prodotto"].ToString().Substring(8, 4) : null;
                                tipologiaAutomaticaUnicarpe.CodiceTipoRichiesta = dbDataReader["Tipologia"] != DBNull.Value ? dbDataReader["Tipologia"].ToString() : null;
                                tipologiaAutomaticaUnicarpe.DecorrenzaMinima = dbDataReader["DecoMin"] != DBNull.Value ? (DateTime)dbDataReader["DecoMin"] : (DateTime?)null;

                                listaDatiTipologieAutomaticheUnicarpe.Add(tipologiaAutomaticaUnicarpe);
                            }
                            dbDataReader.Close();
                        }
                        catch (Exception ex)
                        {
                            dbConnection.Close();
                            INPS.DNA.Logging.Logger.LogException(ex);
                            throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione di lettura sulla stored GET_TABLE_MEMO6: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                        }
                        finally
                        {
                            if (dbConnection != null && dbConnection.State != ConnectionState.Closed) dbConnection.Close();
                        }
                    }
                    transactionScope.Complete();
                }
            }
        }

        #region Nested Class
        public class TipologiaAutomaticaUnicarpe
        {
            public string SiglaCategoria { get; set; }
            public string Gruppo { get; set; }
            public string Prodotto { get; set; }
            public string Tipo { get; set; }
            public string CodiceTipoRichiesta { get; set; }
            public DateTime? DecorrenzaMinima { get; set; }
        }
        #endregion Nested Class
    }
}
