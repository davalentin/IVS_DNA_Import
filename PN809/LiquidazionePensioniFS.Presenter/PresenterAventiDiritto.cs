using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAventiDiritto
    {
        public void GetAventiDiritto(IAventiDiritto aventiDiritto)
        {
            aventiDiritto.HasError = false;
            aventiDiritto.ErrorMessage = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(aventiDiritto.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = aventiDiritto.domanda.ProgStorico;

            try
            {
                AreaAventiDiritto areaAventiDiritto = null;
                AreaEsito esito = objWS.GetAreaAventiDirittoByDomanda(out areaAventiDiritto, areaRichiestaDomanda);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aventiDiritto.HasError = true;
                    aventiDiritto.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aventiDiritto.AreaAventiDiritto = areaAventiDiritto;
                }
            }

            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                aventiDiritto.HasError = true;
                aventiDiritto.ErrorMessage = "Errore nel recupero degli aventi diritto";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiAventiDiritto(IAventiDiritto aventiDiritto)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAventiDiritto areaAventiDiritto = aventiDiritto.AreaAventiDiritto;
            long ndomanda = Int64.Parse(aventiDiritto.domanda.NumeroDomanda);
            try
            {
                AreaEsito esito = objWS.SalvaDatiAventiDirittoByDomanda(ndomanda, ref areaAventiDiritto);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aventiDiritto.HasError = true;
                    aventiDiritto.ErrorMessage = esito.Messaggio;
                    return;
                }
                else
                    aventiDiritto.AreaAventiDiritto = areaAventiDiritto;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAventiDiritto: errore nel metodo SalvaDatiAventiDiritto: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void StoreAventiDiritto(IAventiDiritto aventiDiritto)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaAventiDiritto areaAventiDiritto = aventiDiritto.AreaAventiDiritto;
            long ndomanda = Int64.Parse(aventiDiritto.domanda.NumeroDomanda);
            try
            {
                AreaEsito esito = objWS.StoreAventiDiritto(ndomanda, ref areaAventiDiritto);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    aventiDiritto.HasError = true;
                    aventiDiritto.ErrorMessage = esito.Messaggio;
                    return;
                }
                else
                    aventiDiritto.AreaAventiDiritto = areaAventiDiritto;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterAventiDiritto: errore nel metodo StoreAventiDiritto: ");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaAventiDirittoFromWebDom(IAventiDiritto aventiDiritto)
        {
            aventiDiritto.HasError = false;
            aventiDiritto.ErrorMessage = string.Empty;
            short sedeOperatore = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            long ndomanda = Int64.Parse(aventiDiritto.domanda.NumeroDomanda);
            try
            {
                AreaAventiDiritto areaAventiDiritto = null;
                AreaEsito esito = objWS.AggiornaAventiDirittoFromWebDom(out areaAventiDiritto, ndomanda, sedeOperatore, matricola);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aventiDiritto.HasError = true;
                    aventiDiritto.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aventiDiritto.AreaAventiDiritto = areaAventiDiritto;
                }
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                aventiDiritto.HasError = true;
                aventiDiritto.ErrorMessage = "Errore nel recupero degli aventi diritto da WebDom";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaAventiDirittoFromArchivioPensione(IAventiDiritto aventiDiritto)
        {
            aventiDiritto.HasError = false;
            aventiDiritto.ErrorMessage = string.Empty;
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            long ndomanda = Int64.Parse(aventiDiritto.domanda.NumeroDomanda);
            try
            {
                AreaAventiDiritto areaAventiDiritto = null;
                AreaEsito esito = objWS.AggiornaAventiDirittoFromArchivioPensione(out areaAventiDiritto, ndomanda, sedeOperatore, centroOperativoOperatore, matricola);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    aventiDiritto.HasError = true;
                    aventiDiritto.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    aventiDiritto.AreaAventiDiritto = areaAventiDiritto;
                }
            }

            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                aventiDiritto.HasError = true;
                aventiDiritto.ErrorMessage = "Errore nel recupero degli aventi diritto dall'Archivio Pensione";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
