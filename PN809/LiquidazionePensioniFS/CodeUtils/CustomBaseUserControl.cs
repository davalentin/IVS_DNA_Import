using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INPS.DNA.UI.Web;
using INPS.DNA.UI.Web.Intranet;
using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.View;
using INPS.Pensioni.LiquidazionePensione.View.Web;
using INPS.Pensioni.LiquidazionePensione.View.Web.UserControls;
using INPS.DNA.Caching;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using INPS.Pensioni.LiquidazionePensione.Presenter.Enum;

namespace INPS.Pensioni.LiquidazionePensione.View.Web
{
    public class CustomBaseUserControl : BaseViewUserControl
    {
        internal AreaTitolare.DatiPensione GetDatiPensione(ITitolarePensione ITitolare)
        {
            if (Session["DatiPensione"] != null)
            {
                AreaTitolare.DatiPensione datiPensione = (AreaTitolare.DatiPensione)Session["DatiPensione"];
                return datiPensione;
            }
            else
            {
                AreaTitolare titolare = GetDatiTitolare(ITitolare);
                Session["DatiPensione"] = (AreaTitolare.DatiPensione)titolare.Pensione;
                return titolare.Pensione;
            }
        }

        private AreaTitolare GetDatiTitolare(ITitolarePensione ITitolare)
        {
            PresenterTitolare presenterTitolare = new PresenterTitolare();
            AreaTitolare titolare = new AreaTitolare();
            titolare = presenterTitolare.CaricaTitolare(ITitolare);
            return titolare;
        }

        internal void GetDataSistema(UtilityTipoAppartenenza? tipoAppartenenza, IControlliDinamici IControlliDinamici)
        {
            DateTime? dataSistema = IControlliDinamici.DataSistema;

            if (INPS.DNA.Caching.Cache.Get<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString()) == null)
            {
                // Get del valore
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                presenter.GetDataSistema(tipoAppartenenza, out dataSistema);
                INPS.DNA.Caching.Cache.Add<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString(), dataSistema);
            }
            else
                dataSistema = INPS.DNA.Caching.Cache.Get<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString());

            IControlliDinamici.DataSistema = dataSistema;
        }

        internal void GetDataSistema(UtilityTipoAppartenenza? tipoAppartenenza, out DateTime? dataSistema)
        {
            dataSistema = null;

            if (INPS.DNA.Caching.Cache.Get<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString()) == null)
            {
                // Get del valore
                Presenter.PresenterControlliDinamici presenter = new PresenterControlliDinamici();
                presenter.GetDataSistema(tipoAppartenenza, out dataSistema);
                INPS.DNA.Caching.Cache.Add<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString(), dataSistema);
            }
            else
                dataSistema = INPS.DNA.Caching.Cache.Get<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString());
        }

        internal void SetDataSistema(UtilityTipoAppartenenza? tipoAppartenenza, IControlliDinamici IControlliDinamici)
        {
            if (IControlliDinamici.DataSistema.HasValue)
                INPS.DNA.Caching.Cache.Add<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString(), IControlliDinamici.DataSistema);
            else
                INPS.DNA.Caching.Cache.Add<DateTime?>("DataSistema" + tipoAppartenenza.GetValueOrDefault().ToString(), DateTime.Now);
        }
    }
}
