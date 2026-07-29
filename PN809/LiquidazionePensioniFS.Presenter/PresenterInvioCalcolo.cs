using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using System.Runtime.Serialization;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterInvioCalcolo 
    {
        #region public members
        public void CalcolaDomanda(IInfoCalcolo infoCalcolo, IQuadriSemafori quadriSemafori, out string transactionId)
        {
           
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            long ndomanda = infoCalcolo.datiPensione.NDomus;
            bool isVerify = infoCalcolo.IsVerify;
            bool isConsultazioniANFVerificate = infoCalcolo.IsConsultazioniANFVerificate;
            bool isReingegnerizzato = infoCalcolo.IsReingegnerizzato;
            string statoPensione = string.Empty;
            int certificato = 0;
            string chiavePensione = string.Empty;
            GestioneFamiliariConsultazioneUnificataANF[] listaConsultazioniANF = null;
            GestioneAnniRichiestaBonusDatiPrenotazioneElaborazioni[] listaPrenotazioneElaborazioni = null;
            transactionId = null;
            string flagIndennizzo = null;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoCalcolo.areaEsito = objWS.CalcolaDomanda(out statoPensione, out certificato, out chiavePensione, out listaConsultazioniANF, out listaPrenotazioneElaborazioni, out transactionId, out flagIndennizzo, ndomanda, matricola, sedeOperatore, centroOperativoOperatore, isVerify, isReingegnerizzato, quadriSemafori.areaQuadri, isConsultazioniANFVerificate);
                infoCalcolo.statoPensione = statoPensione;
                infoCalcolo.certificato = certificato;
                infoCalcolo.chiavePensione = chiavePensione;
                infoCalcolo.FlagIndennizzo = flagIndennizzo;
                if (listaConsultazioniANF != null && listaConsultazioniANF.Count() > 0)
                    infoCalcolo.ListaConsultazioniANF = listaConsultazioniANF.ToList();
                if (listaPrenotazioneElaborazioni != null && listaPrenotazioneElaborazioni.Count() > 0)
                    infoCalcolo.ListaPrenotazioneElaborazioni = listaPrenotazioneElaborazioni.ToList();
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo CalcolaDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaCI05(IInfoPostCalcolo infoCI05)
        {
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            long ndomanda = infoCI05.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoCI05.areaEsito = objWS.AggiornaCI05(out statoPensione, ndomanda, matricola, sedeOperatore, centroOperativoOperatore);
                infoCI05.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaCI05");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaWebDom(IInfoPostCalcolo infoWebDom)
        {
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            long ndomanda = infoWebDom.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoWebDom.areaEsito = objWS.AggiornaWebDom(out statoPensione, ndomanda, matricola, sedeOperatore, centroOperativoOperatore);
                infoWebDom.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaWebDom");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaFelpe(IInfoPostCalcolo infoFelpe)
        {
            short sedeOperatore = Utility.GetSedeOperatore();
            short centroOperativoOperatore = Utility.GetCentroOperativoOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            long ndomanda = infoFelpe.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoFelpe.areaEsito = objWS.AggiornaFelpe(out statoPensione, ndomanda, matricola, sedeOperatore, centroOperativoOperatore);
                infoFelpe.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaFelpe");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaOneri(IInfoPostCalcolo infoOneri)
        {
            long ndomanda = infoOneri.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoOneri.areaEsito = objWS.AggiornaOneri(out statoPensione, ndomanda);
                infoOneri.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaOneri");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaSai(IInfoPostCalcolo infoSai)
        {
            long ndomanda = infoSai.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoSai.areaEsito = objWS.AggiornaSai(out statoPensione, ndomanda);
                infoSai.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaSai");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaINPDAP(IInfoPostCalcolo infoINPDAP)
        {
            long ndomanda = infoINPDAP.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoINPDAP.areaEsito = objWS.AggiornaINPDAP(out statoPensione, ndomanda);
                infoINPDAP.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaINPDAP");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaTotal(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaTotal(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaTotal");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaBooking(IInfoPostCalcolo infoBooking, out List<GestioneAnniRichiestaBonusDatiPrenotazioneElaborazioni> listaPrenotazioneElaborazioni)
        {
            long ndomanda = infoBooking.datiPensione.NDomus;
            string statoPensione = string.Empty;
            short sedeOperatore = Utility.GetSedeOperatore();
            string matricola = Utility.GetMatricolaOperatore();
            GestioneAnniRichiestaBonusDatiPrenotazioneElaborazioni[] listaPrenotazioneElaborazioniApp = null;
            listaPrenotazioneElaborazioni = null;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoBooking.areaEsito = objWS.AggiornaBooking(out statoPensione, out listaPrenotazioneElaborazioniApp, ndomanda, matricola, sedeOperatore);
                infoBooking.statoPensione = statoPensione;
                if (listaPrenotazioneElaborazioniApp != null && listaPrenotazioneElaborazioniApp.Count() > 0)
                    listaPrenotazioneElaborazioni = listaPrenotazioneElaborazioniApp.ToList();
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaBooking");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaTot(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaTotalPerTot(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaTot");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaNoteDebito(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaNoteDiDebito(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaNoteDebito");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        public void AggiornaPianiDiPagamento(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaPianiDiPagamento(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaPianiDiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaEquoInd(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaEquoInd(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaPianiDiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaIndennitaSpeciale(IInfoPostCalcolo infoTotal)
        {
            long ndomanda = infoTotal.datiPensione.NDomus;
            string statoPensione = string.Empty;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                infoTotal.areaEsito = objWS.AggiornaIndennSpec(out statoPensione, ndomanda);
                infoTotal.statoPensione = statoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo AggiornaPianiDiPagamento");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetIsDomandaVerify(IInfoCalcolo infoCalcolo)
        {
            long ndomanda = infoCalcolo.datiPensione.NDomus;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                bool isVerify = false;
                infoCalcolo.areaEsito = objWS.GetIsDomandaVerify(out isVerify, ndomanda);
                infoCalcolo.IsVerify = isVerify;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo GetIsDomandaVerify");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetIsNuovoCalcolo(IInfoCalcolo infoCalcolo)
        {
            long ndomanda = infoCalcolo.datiPensione.NDomus;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                bool isNuovoCalcolo = false;
                bool esitoInattesa = false;
                infoCalcolo.areaEsito = objWS.IsNuovoCalcolo(out isNuovoCalcolo, out esitoInattesa, ndomanda, infoCalcolo.IsVerify);
                infoCalcolo.IsNuovoCalcolo = isNuovoCalcolo;
                infoCalcolo.BloccaInvio = esitoInattesa; //TEST
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo GetIsNuovoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetEsitoNuovoCalcolo(IInfoCalcolo infoCalcolo, string transactionId)
        {
            long ndomanda = infoCalcolo.datiPensione.NDomus;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                var res = objWS.GetEsitoNuovoCalcolo(ndomanda, transactionId);
                infoCalcolo.areaEsito = res != null ? res.Esito : null;
                infoCalcolo.statoPensione = res.EsitoNuovoCalcolo.StatoPensione;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterInvioCalcolo, Errore nel metodo GetIsNuovoCalcolo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }
        #endregion public members
    }

    [Serializable]
    public class CustomException : Exception
    {
        public CustomException()
        {
        }

        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
