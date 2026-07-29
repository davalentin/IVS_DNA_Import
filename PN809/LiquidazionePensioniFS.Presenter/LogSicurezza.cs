using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.DNA.Context;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Data.Common;
using System.Data.SqlClient;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class LogSicurezza
    {
        public static void ScritturaLog(string numeroDomanda, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? gruppo, 
            int idEvento, string ipClient, int returnCode, string descErr, string cfTit, string chiavePensione)
        {
            Sicurezza datiSicurezza = new Sicurezza();
            datiSicurezza.CodApp = "PN809_LIQUIDAZIONEPENSIONIFS";
            datiSicurezza.CfTitolare = cfTit;
            //Su report possono non selezionare la sede
            try
            {
                datiSicurezza.Sede = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0');
            }
            catch (Exception)
            {
                // Eccezione ignorata
            }
            datiSicurezza.IpClient = ipClient;
            datiSicurezza.IdClasseUtente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).UserClassId;
            datiSicurezza.Utente = ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).Matricula;
            datiSicurezza.IdEvento = idEvento;
            datiSicurezza.Ndomus = numeroDomanda;
            datiSicurezza.Gruppo = gruppo;
            datiSicurezza.ReturnCode = returnCode;
            datiSicurezza.DescrizioneErrore = descErr;
            datiSicurezza.ChiavePensione = chiavePensione;
            ScriviLog(datiSicurezza);
        }

        private static void ScriviLog(Sicurezza datiSicurezza)
        {
            using (new MethodExecutionTracer())
            {
                DbCommand dbCommand;
                int value;
                try
                {
                    using (DbConnection dbConnection = ConnectionFactory.GetConnection("SicurezzaConnectionString"))
                    {
                        dbCommand = dbConnection.CreateCommand();
                        dbCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        dbCommand.CommandText = "spAppendToLog";
                        CostruisciParametri(datiSicurezza, ref dbCommand);
                        //La Open è sempre necessaria (tranne nel caso si usi un DataAdapter) 
                        dbCommand.Connection.Open();

                        value = (int)dbCommand.ExecuteNonQuery();

                        //La chiusura della connessione viene fatta al momento della Close o nella Dispose 
                    }
                }
                catch (Exception Ex)
                {
                    Logger.WriteError("Errore scrittura CLog: " + Ex.Message);
                }
            }
        }

        private static void CostruisciParametri(Sicurezza datiSicurezza, ref DbCommand dbCommand)
        {
            if (datiSicurezza == null)
                return;

            DbParameter par = new SqlParameter();
            par.ParameterName = "@Utente";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.String;
            par.Value = datiSicurezza.Utente;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@idClasseUtente";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.Int32;
            par.Value = datiSicurezza.IdClasseUtente;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@idEvento";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.Int32;
            par.Value = datiSicurezza.IdEvento;
            dbCommand.Parameters.Add(par);

            if (datiSicurezza.Ndomus == "REP" || datiSicurezza.Ndomus == "XLS")
            {
                par = new SqlParameter();
                par.ParameterName = "@Parametri";
                par.Direction = System.Data.ParameterDirection.Input;
                par.DbType = System.Data.DbType.String;
                par.Value = "COD_APP=" + datiSicurezza.CodApp + ";" +
                    "TIPO_ESTRAZIONE=" + datiSicurezza.Ndomus + ";";
                dbCommand.Parameters.Add(par);
            }
            else
            {
                par = new SqlParameter();
                par.ParameterName = "@Parametri";
                par.Direction = System.Data.ParameterDirection.Input;
                par.DbType = System.Data.DbType.String;
                par.Value = "NUM_DOM=" + datiSicurezza.Ndomus + ";" +
                    "CF=" + datiSicurezza.CfTitolare + ";" +
                    "SEDE=" + datiSicurezza.Sede + ";" +
                    "GRUPPO=" +
                    (datiSicurezza.Gruppo.HasValue ? datiSicurezza.Gruppo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.AGO ? "AGO" :
                    datiSicurezza.Gruppo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.CI ? "CI" :
                    datiSicurezza.Gruppo.Value == AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp.FS ? "FS" : string.Empty : string.Empty) + ";" +
                    "COD_APP=" + datiSicurezza.CodApp + ";" +
                    (!string.IsNullOrEmpty(datiSicurezza.ChiavePensione) ? "COD_PEN=" + datiSicurezza.ChiavePensione + ";" : string.Empty);
                dbCommand.Parameters.Add(par);
            }
            par = new SqlParameter();
            par.ParameterName = "@ipClient";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.String;
            par.Value = datiSicurezza.IpClient;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@TempoEsecuzione";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.Int32;
            par.Value = 0;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@ReturnCode";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.Int32;
            par.Value = datiSicurezza.ReturnCode;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@DescrizioneErrore";
            par.Direction = System.Data.ParameterDirection.Input;
            par.DbType = System.Data.DbType.String;
            par.Value = datiSicurezza.DescrizioneErrore;
            dbCommand.Parameters.Add(par);

            par = new SqlParameter();
            par.ParameterName = "@ErrorCode";
            par.Direction = System.Data.ParameterDirection.Output;
            par.DbType = System.Data.DbType.Int32;
            par.Value = 0;
            dbCommand.Parameters.Add(par);
        }

        #region nested class
        private class Sicurezza
        {
            public Sicurezza()
            { }

            public Sicurezza(string utente, int idClasseUtente, int idEvento, string ipClient, string nDomus,
                string cfTit, string sede, AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? gruppo,
                int returnCode, string descErrore, string codApp, string chiavePensione)
            {
                _Utente = utente;
                _IdClasseUtente = idClasseUtente;
                _IdEvento = idEvento;
                _IpClient = ipClient;
                _Ndomus = nDomus;
                _CfTitolare = cfTit;
                _Sede = sede;
                _Gruppo = gruppo;
                _ReturnCode = returnCode;
                _DescrizioneErrore = descErrore;
                _CodApp = codApp;
                _ChiavePensione = chiavePensione;
            }

            #region private properties
            private string _Utente;
            private int _IdClasseUtente;
            private int _IdEvento;
            private string _IpClient;
            private string _Ndomus;
            private string _CfTitolare;
            private string _Sede;
            private AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? _Gruppo;
            private int _ReturnCode;
            private string _DescrizioneErrore;
            private string _CodApp;
            private string _ChiavePensione;
            #endregion private properties

            #region public properties
            public string Utente { get { return _Utente; } set { _Utente = value; } }
            public int IdClasseUtente { get { return _IdClasseUtente; } set { _IdClasseUtente = value; } }
            public int IdEvento { get { return _IdEvento; } set { _IdEvento = value; } }
            public string IpClient { get { return _IpClient; } set { _IpClient = value; } }
            public string Ndomus { get { return _Ndomus; } set { _Ndomus = value; } }
            public string CfTitolare { get { return _CfTitolare; } set { _CfTitolare = value; } }
            public string Sede { get { return _Sede; } set { _Sede = value; } }
            public AreaRispostaRiepilogo.DatiRiepilogoDomanda.TipoApp? Gruppo { get { return _Gruppo; } set { _Gruppo = value; } }
            public int ReturnCode { get { return _ReturnCode; } set { _ReturnCode = value; } }
            public string DescrizioneErrore { get { return _DescrizioneErrore; } set { _DescrizioneErrore = value; } }
            public string CodApp { get { return _CodApp; } set { _CodApp = value; } }
            public string ChiavePensione { get { return _ChiavePensione; } set { _ChiavePensione = value; } }
            #endregion public properties

        }
        #endregion nested class
    }
}
