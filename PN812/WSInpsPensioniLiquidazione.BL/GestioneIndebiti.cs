using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.Pensioni.Liquidazione.Service_Reference;
using System;
using System.Collections.Generic;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneIndebiti
    {
        static public bool SalvaIndebito(IndebitoDto indebito)
        {
            Indebito tempIndebito = DAGestioneIndebito.GetIndebito(long.Parse(indebito.Ndompro));
            bool resultInsert;

            if(tempIndebito == null)
            {
                string numeroDomanda = indebito.Ndompro;
                if (numeroDomanda.Length == 14)
                    numeroDomanda = numeroDomanda.Substring(1);

                Indebito entityIndebito = new Indebito()
                {
                    NDomus = long.Parse(numeroDomanda),
                    PeriodoAl = indebito.DataFineDebito,
                    PeriodoDal = indebito.DataInizioDebito
                };
                resultInsert = DAGestioneIndebito.InsertIndebito(entityIndebito);
                if (!resultInsert)
                    return false;

                List<CasualiDebito> casualiDebito = EstraiCasualiDebito(indebito.ContiRic, indebito.Ndompro);
                resultInsert = DAGestioneCasualiDebito.InsertCasuali(entityIndebito.NDomus, casualiDebito);
                return resultInsert;
            } else
            {
                List<CasualiDebito> casualiDebito = EstraiCasualiDebito(indebito.ContiRic, indebito.Ndompro);
                resultInsert = DAGestioneCasualiDebito.UpdateCasuali(tempIndebito.NDomus, casualiDebito);
                return resultInsert;
            }
        }

        static private List<CasualiDebito> EstraiCasualiDebito(List<ContoRicDto> contiRic, string numeroDomanda)
        {
            List<CasualiDebito> casualiDebito = new List<CasualiDebito>();
            foreach (ContoRicDto contoRic in contiRic)
            {
                casualiDebito.Add(new CasualiDebito()
                {
                    CasualeAnalitica = contoRic.CausaleSelezionata.Analitica,
                    CasualeSintetica = contoRic.CausaleSelezionata.Sintetica,
                    ContoRecupero = string.Format("{0} - {1}", contoRic.ContoRecupero.Codice, contoRic.ContoRecupero.Nome),
                    Importo = contoRic.ContoRecupero.Importo,
                    Id = Guid.NewGuid(),
                    Indebito = long.Parse(numeroDomanda)
                });
            }
            return casualiDebito;
        }
    }
}
