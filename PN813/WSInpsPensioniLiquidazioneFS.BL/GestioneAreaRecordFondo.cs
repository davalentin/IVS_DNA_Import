using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA;
using EntityBLCommon = INPS.Pensioni.Liquidazione.BLCommon.Entity;

namespace INPS.Pensioni.LiquidazioneFs
{
    internal class GestioneAreaRecordFondo
    {
        public static void SalvaRecordFondo(long idPensione, List<Entity.RecordFondo> listaRecordFondo, out List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo)
        {
            listaDatiRecordFondo = new List<GestioneRecordFondo.DatiRecordFondo>();
            List<GestioneRecordFondo.DatiRecordFondo> tempList = new List<GestioneRecordFondo.DatiRecordFondo>();

            if (listaRecordFondo != null && listaRecordFondo.Count > 0)
            {
                foreach (Entity.RecordFondo recordFondo in listaRecordFondo)
                {
                    GestioneRecordFondo.DatiRecordFondo datiRecordFondo = new GestioneRecordFondo.DatiRecordFondo();
                    if (recordFondo != null)
                    {
                        Utility.ValorizzaOggetti(recordFondo, datiRecordFondo);
                    }
                    tempList.Add(datiRecordFondo);
                }


                //GestioneRecordFondo.SalvaRecordFondo(idPensione, listaDatiRecordFondo);
                GestioneRecordFondo.SalvaRecordFondoReturnListaAggiornata(idPensione, tempList, out listaDatiRecordFondo);
            }
            else
                GestioneRecordFondo.SalvaRecordFondo(idPensione, new List<GestioneRecordFondo.DatiRecordFondo>());
        }

        public static void GetListaRecordFondoByIdPensione(ref EntityBLCommon.ContenitoreObject contenitore, out List<INPS.Pensioni.LiquidazioneFs.Entity.RecordFondo> listaRecordFondo)
        {
            listaRecordFondo = new List<INPS.Pensioni.LiquidazioneFs.Entity.RecordFondo>();
            try
            {
                List<GestioneRecordFondo.DatiRecordFondo> listaDatiRecordFondo = contenitore.ListaDatiRecordFondo;
                if (listaDatiRecordFondo != null && listaDatiRecordFondo.Count > 0)
                {
                    foreach (GestioneRecordFondo.DatiRecordFondo datiRecordFondo in listaDatiRecordFondo)
                    {
                        INPS.Pensioni.LiquidazioneFs.Entity.RecordFondo recordFondo = new INPS.Pensioni.LiquidazioneFs.Entity.RecordFondo(datiRecordFondo.Id,
                                                                                                                            datiRecordFondo.CodiceNatura1,
                                                                                                                            datiRecordFondo.CodiceNatura2,
                                                                                                                            datiRecordFondo.CodiceNatura3,
                                                                                                                            datiRecordFondo.CodiceNonCalcolo,
                                                                                                                            datiRecordFondo.DecorrenzaValiditaDati,
                                                                                                                            datiRecordFondo.DataSospensione, true);
                        listaRecordFondo.Add(recordFondo);
                    }
                }

                if (listaRecordFondo != null && listaRecordFondo.Count > 0)
                    listaRecordFondo = listaRecordFondo.OrderBy(rF => rF.DecorrenzaValiditaDati).ToList<Entity.RecordFondo>();
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception Ex)
            {
                throw new DnaApplicationException("Errore nel metodo GetListaRecordFondoByNumeroDomanda.", Ex);
            }
        }
    }
}

