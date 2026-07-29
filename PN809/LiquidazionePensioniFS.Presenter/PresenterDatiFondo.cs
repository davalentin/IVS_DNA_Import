using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazioneFs;
using System.ServiceModel;
using INPS.DNA;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDatiFondo
    {
        public void GetDatiFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiFondo.domanda.ProgStorico;
            AreaEsito risultatoGetDatiFondoByDomanda = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoGetDatiFondoByDomanda = objWS.GetQuadroDatiFondoByDomanda(out areaDatiFondo, areaRichiestaDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo GetDatiFondo");
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

        public void AddRegistrazioneFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoAddRegistrazioneFondoByDomanda = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoAddRegistrazioneFondoByDomanda = objWS.AddRegistrazioneFondoByDomanda(out areaDatiFondo, numeroDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo AddRegistrazioneFondo");
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

        public void CancelRegistrazioneFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelRegistrazioneFondoByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoCancelRegistrazioneFondoByIdRecordFondo = objWS.CancelRegistrazioneFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo CancelRegistrazioneFondo");
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

        public void CancelAllRegistrazioneFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = null;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelRegistrazioniFondoByDomanda = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoCancelRegistrazioniFondoByDomanda = objWS.CancelRegistrazioniFondoByDomanda(out areaDatiFondo, numeroDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo CancelAllRegistrazioneFondo");
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

        public void GetRegistrazioneFondoByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = datiFondo.domanda.ProgStorico;
            AreaEsito risultatoGetRegistrazioneFondoByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoGetRegistrazioneFondoByIdRecordFondo = objWS.GetRegistrazioneFondoByIdRecordFondo(areaRichiestaDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo GetRegistrazioneFondoByIdRecordFondo");
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
        public void StoreDatiFondoByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiFondoByIdRecordFondo = objWS.StoreDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiFondoByIdRecordFondo");
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

        public void EliminaDatiFondoByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoEliminaDatiFondoByIdRecordFondo = objWS.CancelDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiFondoByIdRecordFondo");
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
        
        public void StoreDatiCalcoloByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiCalcoloByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiCalcoloByIdRecordFondo = objWS.StoreDatiCalcoloByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiCalcoloByIdRecordFondo");
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

        public void EliminaDatiCalcoloByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaDatiCalcoloByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoEliminaDatiCalcoloByIdRecordFondo = objWS.CancelDatiCalcoloByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiCalcoloByIdRecordFondo");
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

        #region Tab Dati Calcolo 707

        public void StoreDatiCalcolo707ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiCalcolo707ByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiCalcolo707ByIdRecordFondo = objWS.StoreDatiCalcolo707ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiCalcolo707ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiCalcolo707ByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiCalcolo707ByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiCalcolo707ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoEliminaDatiCalcolo707ByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoEliminaDatiCalcolo707ByIdRecordFondo = objWS.CancelDatiCalcolo707ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiCalcolo707ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoEliminaDatiCalcolo707ByIdRecordFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoEliminaDatiCalcolo707ByIdRecordFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #endregion Tab Dati Calcolo 707


        #region Tab Dati Legge 4/60

        public void StoreDatiLegge460ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiLegge460ForDatiFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiLegge460ForDatiFondo = objWS.StoreDatiLegge460ForDatiFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiLegge460ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoStoreDatiLegge460ForDatiFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoStoreDatiLegge460ForDatiFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        public void EliminaDatiLegge460ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiLegge460ForDatiFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoCancelDatiLegge460ForDatiFondo = objWS.CancelDatiLegge460ForDatiFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiLegge460ByIdRecordFondo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (risultatoCancelDatiLegge460ForDatiFondo.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                datiFondo.HasError = true;
                datiFondo.ErrorMessage = risultatoCancelDatiLegge460ForDatiFondo.Messaggio;
            }
            else
            {
                datiFondo.HasError = false;
                datiFondo.ErrorMessage = "";
                datiFondo.areaDatiFondo = areaDatiFondo;
            }
        }

        #endregion Tab Dati Legge 4/60

        #region Tab Dati Privilegiate

        public void StoreDatiPrivilegiateByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiPrivilegiateByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiPrivilegiateByIdRecordFondo = objWS.StoreDatiPrivilegiateByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiPrivilegiateByIdRecordFondo");
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

        public void EliminaDatiPrivilegiateByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiPrivilegiateByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoCancelDatiPrivilegiateByIdRecordFondo = objWS.CancelDatiPrivilegiateByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiPrivilegiateByIdRecordFondo");
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

        public void StoreDatiArticolo2ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreDatiArticolo2ByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreDatiArticolo2ByIdRecordFondo = objWS.StoreDatiArticolo2ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreDatiArticolo2ByIdRecordFondo");
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

        public void EliminaDatiArticolo2ByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoCancelDatiArticolo2ByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoCancelDatiArticolo2ByIdRecordFondo = objWS.CancelDatiArticolo2ByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo EliminaDatiArticolo2ByIdRecordFondo");
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

        public void StoreQuadroDatiFondoByIdRecordFondo(IDatiFondo datiFondo)
        {
            AreaDatiFondo areaDatiFondo = datiFondo.areaDatiFondo;
            Int64 numeroDomanda = Int64.Parse(datiFondo.domanda.NumeroDomanda);
            AreaEsito risultatoStoreQuadroDatiFondoByIdRecordFondo = null;

            ServizioLiquidazioneFsClient objWS = new ServizioLiquidazioneFsClient();

            try
            {
                risultatoStoreQuadroDatiFondoByIdRecordFondo = objWS.StoreQuadroDatiFondoByIdRecordFondo(numeroDomanda, ref areaDatiFondo);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDatiFondo, Errore nel metodo StoreQuadroDatiFondoByIdRecordFondo");
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
