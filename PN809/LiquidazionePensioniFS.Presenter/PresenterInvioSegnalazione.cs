using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using System.ServiceModel;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterInvioSegnalazione
    {
        public void InvioSegnalazione(ISegnalazione segnalazione)
        {
            ServizioLiquidazioneClient proxy = new ServizioLiquidazioneClient();
            try
            {
                AreaInvioSegnalazione areaSegnalazione = new AreaInvioSegnalazione();
                areaSegnalazione = segnalazione.InvioSegnalazione;

                try
                {
                    areaSegnalazione.Segnalazione.SedeOperatore = Utility.GetSedeOperatore().ToString().PadLeft(4, '0') + Utility.GetCentroOperativoOperatore().ToString().PadLeft(2, '0') + " - " + 
                        (INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.ExtendedProperties != null ? INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.ExtendedProperties["SEDE"].Trim() : INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice.Name);
                }
                catch(Exception)
                {
                    INPS.DNA.Office o = (from s in INPS.DNA.Context.OfficeList.Offices
                                         where s.Key == ((INPS.DNA.Security.Idm.IdmIdentity)INPS.DNA.Security.DnaPrincipal.CurrentIdentity).OfficeSapCode
                                         select s).FirstOrDefault().Value;
                    if (o != null)
                        areaSegnalazione.Segnalazione.SedeOperatore = string.Format("{0} - {1}", o.AspnCode, (o.ExtendedProperties != null ? o.ExtendedProperties["SEDE"].Trim() : o.Name.Trim()));
                }

                AreaEsito esito = proxy.InvioSegnalazione(areaSegnalazione);
                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    segnalazione.HasError = true;
                    segnalazione.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioSegnalazione, Errore nel metodo InvioSegnalazione");
            }
            finally
            {
                Utility.CloseClient(proxy);
            }
        }
    }
}
