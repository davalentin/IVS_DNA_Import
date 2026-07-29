using System;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterProvvisorieCoefficienti
    {
        public void GetDataDecorrenzaProvvisoriaObbligatoria(IDecorrenzaProvvisoriaCoefficienti IDecPrvCoeff)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaProvvisoriePerCoefficienti areaProvvisoriePerCoefficienti = null;
            try
            {
                esito = objWS.GetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(out areaProvvisoriePerCoefficienti, IDecPrvCoeff.TipoAppartenenza);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    IDecPrvCoeff.HasError = true;
                    IDecPrvCoeff.ErrorMessage = esito.Messaggio;
                }
                else
                    if (areaProvvisoriePerCoefficienti != null && areaProvvisoriePerCoefficienti.DataDecorrenzaProvvisoriaObbligatoria.HasValue)
                        IDecPrvCoeff.DataDecorrenzaProvvisoriaObbligatoria = areaProvvisoriePerCoefficienti.DataDecorrenzaProvvisoriaObbligatoria;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterProvvisorieCoefficienti, Errore nel metodo GetDataDecorrenzaProvvisoriaObbligatoria");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SetDataDecorrenzaProvvisoriaObbligatoria(IDecorrenzaProvvisoriaCoefficienti iDecProvvObbl)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito;
            AreaProvvisoriePerCoefficienti areaProvvisCoeff = new AreaProvvisoriePerCoefficienti();
            areaProvvisCoeff.DataDecorrenzaProvvisoriaObbligatoria = iDecProvvObbl.DataDecorrenzaProvvisoriaObbligatoria;

            try
            {
                esito = objWS.SetDataDecorrenzaProvvisorieObbligatoriePerCoefficienti(iDecProvvObbl.TipoAppartenenza, areaProvvisCoeff);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    iDecProvvObbl.HasError = true;
                    iDecProvvObbl.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterProvvisorieCoefficienti, Errore nel metodo SetDataDecorrenzaProvvisoriaObbligatoria");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
    }
}
