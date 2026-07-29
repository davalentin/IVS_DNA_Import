using System;
using System.ServiceModel;
using INPS.DNA;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterRichiestaBonus
    {
        public bool GetRichiestaBonus(IRichiestaBonus richiestaBonus)
        {
            bool isDataFromDb = false;
            AreaRichiestaBonus areaRichiestaBonus = null;
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(richiestaBonus.domanda.NumeroDomanda);
            AreaEsito risultatoGetRichiestaBonusByDomanda = null;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoGetRichiestaBonusByDomanda = objWS.GetRichiestaBonusByDomanda(out areaRichiestaBonus, out isDataFromDb, areaRichiestaDomanda, matricola, sede);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRichiestaBonus, Errore nel metodo GetRichiestaBonus");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetRichiestaBonusByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                richiestaBonus.HasError = true;
                richiestaBonus.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoGetRichiestaBonusByDomanda.Messaggio;
                richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
            }
            else
            {
                if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.NessunErrore)
                {
                    richiestaBonus.HasError = false;
                    richiestaBonus.ErrorMessage = "";
                    richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;

                }
                else if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
                {
                    richiestaBonus.HasError = true;
                    richiestaBonus.ErrorMessage = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;
                    richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                }
            }
            return isDataFromDb;
        }

        public void SalvaRichiestaBonus(IRichiestaBonus richiestaBonus)
        {
            AreaRichiestaBonus areaRichiestaBonus = richiestaBonus.areaRichiestaBonus;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Int64 ndomanda = Int64.Parse(richiestaBonus.domanda.NumeroDomanda);
            AreaEsito risultatoStoreRichiestaBonusByDomanda = null;
            try
            {
                risultatoStoreRichiestaBonusByDomanda = objWS.StoreDatiRichiestaBonus(ndomanda, ref areaRichiestaBonus);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRichiestaBonus, Errore nel metodo SalvaRichiestaBonus");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risultatoStoreRichiestaBonusByDomanda != null && risultatoStoreRichiestaBonusByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    richiestaBonus.HasError = true;
                    richiestaBonus.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoStoreRichiestaBonusByDomanda.Messaggio;
                    richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                }
                else
                {
                    if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.NessunErrore)
                    {
                        richiestaBonus.HasError = false;
                        richiestaBonus.ErrorMessage = "";
                        richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                    }
                    else if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
                    {
                        richiestaBonus.HasError = true;
                        richiestaBonus.ErrorMessage = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;
                        richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterRichiestaBonus, Errore nel metodo SalvaRichiestaBonus" + ex);
            }
        }

        public void EliminaRichiestaBonus(IRichiestaBonus richiestaBonus)
        {
            AreaRichiestaBonus areaRichiestaBonus = richiestaBonus.areaRichiestaBonus;
            short sede = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Int64 numeroDomanda = Int64.Parse(richiestaBonus.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaRichiestaBonusByDomanda = null;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                risultatoEliminaRichiestaBonusByDomanda = objWS.EliminaRichiestaBonusByDomanda(out areaRichiestaBonus, numeroDomanda, matricola, sede);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterRichiestaBonus, Errore nel metodo EliminaRichiestaBonus");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                if (risultatoEliminaRichiestaBonusByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    richiestaBonus.HasError = true;
                    richiestaBonus.ErrorMessage = "#_SERVICE_ERROR_#" + risultatoEliminaRichiestaBonusByDomanda.Messaggio;
                    richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                }
                else
                {
                    if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.NessunErrore)
                    {
                        richiestaBonus.HasError = false;
                        richiestaBonus.ErrorMessage = "";
                        richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                    }
                    else if (areaRichiestaBonus.RichiestaBonus.Esito == GestioneRichiestaBonusTipoRitornoRichiestaBonus.Errore)
                    {
                        richiestaBonus.HasError = true;
                        richiestaBonus.ErrorMessage = areaRichiestaBonus.RichiestaBonus.MessaggioVideo;
                        richiestaBonus.areaRichiestaBonus = areaRichiestaBonus;
                    }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterRichiestaBonus, Errore nel metodo EliminaRichiestaBonus" + ex);
            }
        }
    }
}
