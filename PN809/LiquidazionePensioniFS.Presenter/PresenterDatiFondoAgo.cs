using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneAgo;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDatiFondoAgo
    {
        public void GetDatiFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiFondo.domanda.ProgStorico;
            AreaEsito risultatoGetDatiFondoByDomanda = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoGetDatiFondoByDomanda = objWS.GetQuadroDatiFondoByDomanda(out areaDatiFondo, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo GetDatiFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetDatiFondoByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoGetDatiFondoByDomanda.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void AddRegistrazioneFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoAddRegistrazioneFondoByDomanda = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoAddRegistrazioneFondoByDomanda = objWS.AddRegistrazioneFondoByDomanda(out areaDatiFondo, numeroDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo AddRegistrazioneFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoAddRegistrazioneFondoByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoAddRegistrazioneFondoByDomanda.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void CancelRegistrazioneFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelRegistrazioneFondoByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoCancelRegistrazioneFondoByIdRecordFondo = objWS.CancelRegistrazioneFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo CancelRegistrazioneFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelRegistrazioneFondoByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoCancelRegistrazioneFondoByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void CancelAllRegistrazioneFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelRegistrazioniFondoByDomanda = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoCancelRegistrazioniFondoByDomanda = objWS.CancelRegistrazioniFondoByDomanda(out areaDatiFondo, numeroDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo CancelAllRegistrazioneFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelRegistrazioniFondoByDomanda.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoCancelRegistrazioniFondoByDomanda.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void GetRegistrazioneFondoByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiFondo.domanda.ProgStorico;
            AreaEsito risultatoGetRegistrazioneFondoByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoGetRegistrazioneFondoByIdRecordFondo = objWS.GetRegistrazioneFondoByIdRecordFondo(areaRichiestaDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo GetRegistrazioneFondoByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoGetRegistrazioneFondoByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoGetRegistrazioneFondoByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #region Tab Dati Fondo
        public void StoreDatiFondoByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoStoreDatiFondoByIdRecordFondo = objWS.StoreDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo StoreDatiFondoByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiFondoByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiFondoByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiFondoByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoEliminaDatiFondoByIdRecordFondo = objWS.CancelDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo EliminaDatiFondoByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoEliminaDatiFondoByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoEliminaDatiFondoByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }
        #endregion Tab Dati Fondo

        #region Tab Dati Calcolo

        public void StoreDatiCalcoloByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiCalcoloByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoStoreDatiCalcoloByIdRecordFondo = objWS.StoreDatiCalcoloByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo StoreDatiCalcoloByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiCalcoloByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiCalcoloByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiCalcoloByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaDatiCalcoloByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoEliminaDatiCalcoloByIdRecordFondo = objWS.CancelDatiCalcoloByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo EliminaDatiCalcoloByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoEliminaDatiCalcoloByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoEliminaDatiCalcoloByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #endregion Tab Dati Calcolo

        #region Tab Dati Privilegiate

        public void StoreDatiPrivilegiateByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiPrivilegiateByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoStoreDatiPrivilegiateByIdRecordFondo = objWS.StoreDatiPrivilegiateByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo StoreDatiPrivilegiateByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiPrivilegiateByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiPrivilegiateByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiPrivilegiateByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiPrivilegiateByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoCancelDatiPrivilegiateByIdRecordFondo = objWS.CancelDatiPrivilegiateByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo EliminaDatiPrivilegiateByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiPrivilegiateByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoCancelDatiPrivilegiateByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #endregion Tab Dati Privilegiate

        #region Tab Dati Articolo 2

        public void StoreDatiArticolo2ByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiArticolo2ByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoStoreDatiArticolo2ByIdRecordFondo = objWS.StoreDatiArticolo2ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo StoreDatiArticolo2ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiArticolo2ByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiArticolo2ByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiArticolo2ByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiArticolo2ByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoCancelDatiArticolo2ByIdRecordFondo = objWS.CancelDatiArticolo2ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo EliminaDatiArticolo2ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiArticolo2ByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoCancelDatiArticolo2ByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #endregion Tab Dati Articolo 2

        public void StoreQuadroDatiFondoByIdRecordFondo(IDatiFondoAgo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreQuadroDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneAgoClient objWS = new ServizioLiquidazioneAgoClient();

            try
            {
                risultatoStoreQuadroDatiFondoByIdRecordFondo = objWS.StoreQuadroDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondoAgo, Errore nel metodo StoreQuadroDatiFondoByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreQuadroDatiFondoByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreQuadroDatiFondoByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
            }

            datiFondo.areaDatiFondo = areaDatiFondo;
        }
    }
}
