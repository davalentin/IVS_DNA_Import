using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.Common;
using INPS.DNA.Data;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneDBSComuni
    {
        public static void GetCodInpsComuneByCodCatastale(string codiceCatastale, string tipoAppartenenza, int codiceComuneInpsDaConfrontare, bool isPrelievoFS, out int codInpsComune)
        {
            codInpsComune = 0;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    using (DbConnection dbConnection = ConnectionFactory.GetConnection("DBS_ComuniConnectionString"))
                    {
                        DbCommand dbCommand = dbConnection.CreateCommand();
                        dbCommand.CommandText = "spGetCodInpsComune";
                        dbCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        DbParameter InPar = dbCommand.CreateParameter();
                        InPar.ParameterName = "Fisco";
                        InPar.Direction = System.Data.ParameterDirection.Input;
                        InPar.DbType = System.Data.DbType.String;
                        InPar.Value = codiceCatastale;
                        dbCommand.Parameters.Add(InPar);

                        DbParameter InOutPar = dbCommand.CreateParameter();
                        InOutPar.ParameterName = "mess";
                        InOutPar.Direction = System.Data.ParameterDirection.InputOutput;
                        InOutPar.DbType = System.Data.DbType.String;
                        InOutPar.Value = "";
                        dbCommand.Parameters.Add(InOutPar);

                        DbParameter RetPar = dbCommand.CreateParameter();
                        RetPar.Direction = System.Data.ParameterDirection.ReturnValue;
                        RetPar.DbType = System.Data.DbType.Int32;
                        dbCommand.Parameters.Add(RetPar);

                        dbConnection.Open();
                        DbDataReader dbDataReader = dbCommand.ExecuteReader();
                        while (dbDataReader.Read())
                        {
                            if (dbDataReader["situazione"] != DBNull.Value && dbDataReader["Codice_Inps"] != DBNull.Value && !String.IsNullOrEmpty(dbDataReader["Codice_Inps"].ToString().Trim()))
                            {
                                if (!String.IsNullOrEmpty(tipoAppartenenza) && tipoAppartenenza.Trim().ToUpperInvariant() == "FS" && isPrelievoFS)
                                {
                                    if (int.Parse(dbDataReader["Codice_Inps"].ToString()) == codiceComuneInpsDaConfrontare) 
                                    {
                                        codInpsComune = int.Parse(dbDataReader["Codice_Inps"].ToString());
                                        break;
                                    }

                                }
                                else
                                {
                                    codInpsComune = int.Parse(dbDataReader["Codice_Inps"].ToString());
                                    break;
                                }

                            }
                        }

                        dbDataReader.Close();
                    }

                    transactionScope.Complete();
                }
            }
        }
    }
}
