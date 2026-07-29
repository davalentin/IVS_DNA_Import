using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Data;
using System.Data.Common;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestionePrepensionamento
    {
        #region Private Variables

        private static readonly string _DB2ConnectionString = "DB2Conn_Oneri";

        #endregion Private Variables

        public static void GetPrepensionamentoByIdPensione(Int64 idPensione, out Prepensionamento prepensionamento)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    prepensionamento = (from cc in db.Prepensionamentos where cc.IdPensione == idPensione select cc).SingleOrDefault<Prepensionamento>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPrepensionamento(Prepensionamento prepensionamento)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertPrepensionamento(prepensionamento.IdPensione, prepensionamento.CodiceLegge, prepensionamento.SettimaneUtiliDiritto, prepensionamento.SettimaneUtiliMisura,
                    prepensionamento.SettimaneMaggioreAnzianita, prepensionamento.OnereMancataContribuzione, prepensionamento.CodiceAzienda, prepensionamento.CessazioneBeneficioPrepensionamento,
                    prepensionamento.SettimaneAmianto, prepensionamento.CessazioneAmianto);

                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertPrepensionamento");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaPrepensionamentoByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeletePrepensionamento(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure DeletePrepensionamento");
                }
                db.Connection.Close();
            }
        }

        #region TOPPL03

        public static void SelectTOPPL03(Pensione datiPensione, string codiceCategoria, out List<Prepensionamento> listaDatiPrepensionamento)
        {
            listaDatiPrepensionamento = null;

            using (DbConnection connectionDB2 = ConnectionFactory.GetConnection("DB2Conn_Oneri"))
            {
                try
                {
                    DbCommand dbCommand = connectionDB2.CreateCommand();
                    dbCommand.CommandText = "SELECT * FROM #ONERI_PREPENSIONAMENTO#.TOPPL03 WHERE SEDE = ? AND CATEGOR = ? AND CERTIFIC = ? AND DECORR = ?";
                    dbCommand.CommandType = System.Data.CommandType.Text;

                    DbParameter sede = dbCommand.CreateParameter();
                    sede.Direction = System.Data.ParameterDirection.Input;
                    sede.DbType = System.Data.DbType.String;
                    sede.Value = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                    dbCommand.Parameters.Add(sede);

                    DbParameter categoria = dbCommand.CreateParameter();
                    categoria.Direction = System.Data.ParameterDirection.Input;
                    categoria.DbType = System.Data.DbType.String;
                    categoria.Value = codiceCategoria;
                    dbCommand.Parameters.Add(categoria);

                    DbParameter certificato = dbCommand.CreateParameter();
                    certificato.Direction = System.Data.ParameterDirection.Input;
                    certificato.DbType = System.Data.DbType.String;
                    certificato.Value = datiPensione.NCertificato.ToString().PadLeft(8, '0');
                    dbCommand.Parameters.Add(certificato);

                    DbParameter decorrenza = dbCommand.CreateParameter();
                    decorrenza.Direction = System.Data.ParameterDirection.Input;
                    decorrenza.DbType = System.Data.DbType.Date;
                    decorrenza.Value = datiPensione.DecorrenzaOriginaria;
                    dbCommand.Parameters.Add(decorrenza);

                    DbParameter RetPar = dbCommand.CreateParameter();
                    RetPar.Direction = System.Data.ParameterDirection.ReturnValue;
                    RetPar.DbType = System.Data.DbType.Int32;
                    dbCommand.Parameters.Add(RetPar);

                    connectionDB2.Open();
                    DbDataReader dbDataReader = dbCommand.ExecuteReader();
                    while (dbDataReader.Read())
                    {
                        if (listaDatiPrepensionamento == null)
                            listaDatiPrepensionamento = new List<Prepensionamento>();

                        Prepensionamento prepensionamento = new Prepensionamento();
                        prepensionamento.CodiceLegge = dbDataReader["LEXGRUP"] != DBNull.Value && dbDataReader["LEXCOD"] != DBNull.Value ? int.Parse(dbDataReader["LEXGRUP"].ToString()) * 100 + int.Parse(dbDataReader["LEXCOD"].ToString()) : (int?)null;
                        prepensionamento.SettimaneUtiliDiritto = dbDataReader["SETTDIR"] != DBNull.Value ? int.Parse(dbDataReader["SETTDIR"].ToString()) : (int?)null;
                        prepensionamento.SettimaneUtiliMisura = dbDataReader["SETTMIS"] != DBNull.Value ? int.Parse(dbDataReader["SETTMIS"].ToString()) : (int?)null;
                        prepensionamento.SettimaneMaggioreAnzianita = dbDataReader["SETTINCR"] != DBNull.Value ? int.Parse(dbDataReader["SETTINCR"].ToString()) : (int?)null;
                        prepensionamento.OnereMancataContribuzione = dbDataReader["ONERE1"] != DBNull.Value ? decimal.Parse(dbDataReader["ONERE1"].ToString()) : (decimal?)null;
                        prepensionamento.CodiceAzienda = dbDataReader["MATRIC"] != DBNull.Value && !string.IsNullOrEmpty(dbDataReader["MATRIC"].ToString().Trim()) ? long.Parse(dbDataReader["MATRIC"].ToString().Trim()) : (long?)null;
                        prepensionamento.CessazioneBeneficioPrepensionamento = dbDataReader["DATACES2"] != DBNull.Value ? (DateTime)dbDataReader["DATACES2"] : (DateTime?)null;
                        prepensionamento.SettimaneAmianto = dbDataReader["SETTAM"] != DBNull.Value ? int.Parse(dbDataReader["SETTAM"].ToString()) : (int?)null;
                        prepensionamento.CessazioneAmianto = dbDataReader["DATACES3"] != DBNull.Value ? (DateTime)dbDataReader["DATACES3"] : (DateTime?)null;

                        listaDatiPrepensionamento.Add(prepensionamento);
                    }

                    dbDataReader.Close();
                }
                catch (Exception ex)
                {
                    connectionDB2.Close();
                    INPS.DNA.Logging.Logger.LogException(ex);
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione di lettura sulla tabella DB2 TOPPL03: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                }
                finally
                {
                    if (connectionDB2 != null && connectionDB2.State != ConnectionState.Closed) connectionDB2.Close();
                }
            }
        }

        public static void InsertTOPPL03(Pensione datiPensione, Anagrafica datiAnagrafici, Istruttoria datiIstruttoria, Prepensionamento datiPrepensionamento, string categoria)
        {
            using (new MethodExecutionTracer())
            {
                using (DbConnection connection = ConnectionFactory.GetConnection(_DB2ConnectionString))
                {
                    DbCommand command = connection.CreateCommand();

                    try
                    {
                        #region Query

                        StringBuilder sbQuery = new StringBuilder();
                        sbQuery.Append("INSERT INTO #ONERI_PREPENSIONAMENTO#.TOPPL03 VALUES ");
                        sbQuery.Append("( ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,? ) ");

                        #endregion Query

                        command.CommandText = sbQuery.ToString();
                        command.CommandType = System.Data.CommandType.Text;

                        #region Parameters
                        DbParameter parameter = null;

                        //Sede
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Categoria
                        parameter = command.CreateParameter();
                        parameter.Value = categoria;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Certificato
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.NCertificato.ToString().PadLeft(8, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Decorrenza
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.DecorrenzaOriginaria;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Cognome
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Cognome;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Nome
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Nome;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Sesso
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Sesso;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //DataNascita
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.DataNascita;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Legge - Gruppo
                        parameter = command.CreateParameter();
                        parameter.Value = (datiPrepensionamento.CodiceLegge.GetValueOrDefault() / 100);
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Legge - Codice
                        parameter = command.CreateParameter();
                        parameter.Value = (datiPrepensionamento.CodiceLegge.GetValueOrDefault() % 100);
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane Misura
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneUtiliMisura.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane di incremento
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneMaggioreAnzianita.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Tipo Pensione
                        parameter = command.CreateParameter();
                        parameter.Value = "0";
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data Cessazione Sede
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CessazioneBeneficioPrepensionamento;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Matricola Azienda
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CodiceAzienda.ToString();
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Codice Fiscale
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.CodiceFiscale;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Onere mancata contribuzione
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.OnereMancataContribuzione.GetValueOrDefault();
                        parameter.DbType = DbType.Decimal;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Pensioni provvisioria
                        parameter = command.CreateParameter();
                        parameter.Value = datiIstruttoria.CodiceComunicazioneCampo3;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane diritto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneUtiliDiritto.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data cessazione amianto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CessazioneAmianto;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane amianto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneAmianto.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Fill
                        parameter = command.CreateParameter();
                        parameter.Value = string.Empty;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data trasmissione
                        parameter = command.CreateParameter();
                        parameter.Value = DateTime.Now;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        #endregion Parameters

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        INPS.DNA.Logging.Logger.LogException(ex);
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della scrittura sulla tabella DB2 TOPPL03: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                    finally
                    {
                        if (connection != null && connection.State != ConnectionState.Closed) connection.Close();
                    }
                }
            }
        }

        public static void UpdateTOPPL03(Pensione datiPensione, Anagrafica datiAnagrafici, Istruttoria datiIstruttoria, Prepensionamento datiPrepensionamento, string categoria)
        {
            using (new MethodExecutionTracer())
            {
                using (DbConnection connection = ConnectionFactory.GetConnection(_DB2ConnectionString))
                {
                    DbCommand command = connection.CreateCommand();

                    try
                    {
                        #region Query

                        StringBuilder sbQuery = new StringBuilder();
                        sbQuery.Append("UPDATE #ONERI_PREPENSIONAMENTO#.TOPPL03 SET ");
                        sbQuery.Append("COGNOME = ? ");
                        sbQuery.Append(",NOME = ? ");
                        sbQuery.Append(",SESSO = ? ");
                        sbQuery.Append(",DATNASC = ? ");
                        sbQuery.Append(",LEXGRUP = ? ");
                        sbQuery.Append(",LEXCOD = ? ");
                        sbQuery.Append(",SETTMIS = ? ");
                        sbQuery.Append(",SETTINCR = ? ");
                        sbQuery.Append(",TIPOPEN = ? ");
                        sbQuery.Append(",DATACES2 = ? ");
                        sbQuery.Append(",MATRIC = ? ");
                        sbQuery.Append(",CODFIS = ? ");
                        sbQuery.Append(",ONERE1 = ? ");
                        sbQuery.Append(",PROVVI = ? ");
                        sbQuery.Append(",SETTDIR = ? ");
                        sbQuery.Append(",DATACES3 = ? ");
                        sbQuery.Append(",SETTAM = ? ");
                        sbQuery.Append(",DATATRAS = ? ");
                        sbQuery.Append("WHERE SEDE = ? ");
                        sbQuery.Append("AND CATEGOR = ? ");
                        sbQuery.Append("AND CERTIFIC = ? ");
                        sbQuery.Append("AND DECORR = ? ");

                        #endregion Query

                        command.CommandText = sbQuery.ToString();
                        command.CommandType = System.Data.CommandType.Text;

                        #region Parameters
                        DbParameter parameter = null;

                        //Cognome
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Cognome;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Nome
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Nome;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Sesso
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.Sesso;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data di Nascita
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.DataNascita;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Legge - Gruppo
                        parameter = command.CreateParameter();
                        parameter.Value = (datiPrepensionamento.CodiceLegge.GetValueOrDefault() / 100);
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Legge - Codice
                        parameter = command.CreateParameter();
                        parameter.Value = (datiPrepensionamento.CodiceLegge.GetValueOrDefault() % 100);
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane Misura
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneUtiliMisura.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane di incremento
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneMaggioreAnzianita.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Tipo Pensione
                        parameter = command.CreateParameter();
                        parameter.Value = "0";
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data Cessazione Sede
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CessazioneBeneficioPrepensionamento;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Matricola Azienda
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CodiceAzienda.ToString();
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Codice Fiscale
                        parameter = command.CreateParameter();
                        parameter.Value = datiAnagrafici.CodiceFiscale;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Onere mancata contribuzione
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.OnereMancataContribuzione.GetValueOrDefault();
                        parameter.DbType = DbType.Decimal;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Provvisoria
                        parameter = command.CreateParameter();
                        parameter.Value = datiIstruttoria.CodiceComunicazioneCampo3;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane diritto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneUtiliDiritto.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data cessazione amianto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.CessazioneAmianto;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Settimane amianto
                        parameter = command.CreateParameter();
                        parameter.Value = datiPrepensionamento.SettimaneAmianto.GetValueOrDefault();
                        parameter.DbType = DbType.Int16;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Data trasmissione
                        parameter = command.CreateParameter();
                        parameter.Value = DateTime.Now;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Sede
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Categoria
                        parameter = command.CreateParameter();
                        parameter.Value = categoria;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Certificato
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.NCertificato.ToString().PadLeft(8, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Decorrenza
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.DecorrenzaOriginaria;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        #endregion Parameters

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        INPS.DNA.Logging.Logger.LogException(ex);
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione di aggiornamento sulla tabella DB2 TOPPL03: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                    finally
                    {
                        if (connection != null && connection.State != ConnectionState.Closed) connection.Close();
                    }
                }
            }
        }

        public static void DeleteTOPPL03(Pensione datiPensione, string categoria)
        {
            using (new MethodExecutionTracer())
            {
                using (DbConnection connection = ConnectionFactory.GetConnection(_DB2ConnectionString))
                {
                    DbCommand command = connection.CreateCommand();

                    try
                    {
                        #region Query

                        StringBuilder sbQuery = new StringBuilder();
                        sbQuery.Append("DELETE FROM #ONERI_PREPENSIONAMENTO#.TOPPL03 ");
                        sbQuery.Append("WHERE SEDE = ? ");
                        sbQuery.Append("AND CATEGOR = ? ");
                        sbQuery.Append("AND CERTIFIC = ? ");
                        sbQuery.Append("AND DECORR = ? ");

                        #endregion Query

                        command.CommandText = sbQuery.ToString();
                        command.CommandType = System.Data.CommandType.Text;

                        #region Parameters
                        DbParameter parameter = null;

                        //Sede
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.CodiceSede.ToString().PadLeft(4, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Categoria
                        parameter = command.CreateParameter();
                        parameter.Value = categoria;
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Certificato
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.NCertificato.ToString().PadLeft(8, '0');
                        parameter.DbType = DbType.String;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        //Decorrenza
                        parameter = command.CreateParameter();
                        parameter.Value = datiPensione.DecorrenzaOriginaria;
                        parameter.DbType = DbType.Date;
                        parameter.Direction = ParameterDirection.Input;
                        command.Parameters.Add(parameter);

                        #endregion Parameters

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        INPS.DNA.Logging.Logger.LogException(ex);
                        throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della cancellazione sulla tabella DB2 TOPPL03: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                    }
                    finally
                    {
                        if (connection != null && connection.State != ConnectionState.Closed) connection.Close();
                    }
                }
            }
        }

        #endregion TOPPL03
    }
}
