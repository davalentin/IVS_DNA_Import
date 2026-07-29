using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterAltreDomandeCollegate
    {
        public void GetAltreDomandeCollegate(IAltreDomandeCollegate altreDomandeCollegate)
        {
            altreDomandeCollegate.HasError = false;
            altreDomandeCollegate.ErrorMessage = string.Empty;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(altreDomandeCollegate.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = altreDomandeCollegate.domanda.ProgStorico;

            try
            {
                AreaAltreDomandeCollegate areaAltreDomandeCollegate = null;
                AreaEsito esito = objWS.GetAreaAltreDomandeCollegateByDomanda(out areaAltreDomandeCollegate, areaRichiestaDomanda);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    altreDomandeCollegate.HasError = true;
                    altreDomandeCollegate.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    altreDomandeCollegate.AreaAltreDomandeCollegate = areaAltreDomandeCollegate;
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
                altreDomandeCollegate.HasError = true;
                altreDomandeCollegate.ErrorMessage = "Errore nel recupero delle altre domande collegate";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetAventiDiritto(IAltreDomandeCollegate altreDomandeCollegate)
        {
            altreDomandeCollegate.HasError = false;
            altreDomandeCollegate.ErrorMessage = string.Empty;
            short sedeOperatore = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(altreDomandeCollegate.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = altreDomandeCollegate.domanda.ProgStorico;
            long nDomandaAventeDiritto = altreDomandeCollegate.NumeroDomandaAventeDiritto;
            try
            {
                AreaAltreDomandeCollegate areaAltreDomandeCollegate = null;
                AreaEsito esito = objWS.GetAventiDirittoDomandaCollegataByDomanda(out areaAltreDomandeCollegate, areaRichiestaDomanda, nDomandaAventeDiritto, sedeOperatore, matricola);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    altreDomandeCollegate.HasError = true;
                    altreDomandeCollegate.ErrorMessage = esito.Messaggio;
                }
                else
                {
                    altreDomandeCollegate.AreaAltreDomandeCollegate = areaAltreDomandeCollegate;
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
                altreDomandeCollegate.HasError = true;
                altreDomandeCollegate.ErrorMessage = "Errore nel recupero degli aventi diritto";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
