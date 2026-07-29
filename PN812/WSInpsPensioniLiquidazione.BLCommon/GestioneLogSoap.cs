using System;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Collections.Generic;
using System.Linq;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;


namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneLogSoap
    {
        public static void SalvaLogSoap(object obj, Utility.Servizio serviceName, Utility.MetodoServizio methodName, Utility.SOAPLogDirection direction, string numDomanda, Guid? guid = null, string suffixMethodName = null)
        {
            try
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    string xmlTotale = string.Empty;
                    if (obj.GetType() == typeof(string) || obj.GetType() == typeof(String))
                        xmlTotale = obj.ToString();
                    else
                        xmlTotale = Utility.GetXmlFromObject(obj);

                    var listXml = new List<string>();

                    if (!string.IsNullOrEmpty(xmlTotale))
                        listXml = xmlTotale.SplitByLength(43600).ToList(); // 43600 è il valore limite della clipboard per copiare il testo

                    foreach (var item in listXml.Select((xml, index) => new { xml, index }))
                    {
                        LogSoap log = new LogSoap
                        {
                            Direction = Utility.GetDescription(direction)[0],
                            MethodName = !string.IsNullOrEmpty(suffixMethodName) ? string.Format("{0}_{1}", methodName.ToString(), suffixMethodName) : methodName.ToString(),
                            ServiceName = serviceName.ToString(),
                            Xml = item.xml,
                            NumDomanda = Utility.StringToLong(numDomanda),
                            Guid = guid,
                            Progressivo = (byte)item.index
                        };

                        DAGestioneLogSoap.SalvaLogSoap(log);
                    }
                    transactionScope.Complete();
                }
            }

            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.WriteError(ex.Message);
            }
        }

        public static void EliminaLogSoap(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                DAGestioneLogSoap.DeleteLogSoap(idPensione);
                transactionScope.Complete();
            }
        }

        //ENG - Aggiornamento Memo86
        public static void GetTimestampMinimo(long ndomus, out DateTime? dataTimestampMinimo)
        {
            dataTimestampMinimo = null;
            DAGestioneLogSoap.GetTimestampMinimo(ndomus, out dataTimestampMinimo);

        }
    }
}
