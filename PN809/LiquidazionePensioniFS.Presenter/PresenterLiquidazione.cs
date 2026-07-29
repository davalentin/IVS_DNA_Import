using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;

using System.Web;

using INPS.DNA;
using INPS.DNA.Logging;
using INPS.DNA.Services;
using INPS.DNA.Services.FaultContract;

using INPS.Pensioni.LiquidazionePensione.Presenter;
using INPS.Pensioni.LiquidazionePensione.Presenter.Contract;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterLiquidazione
    {
        public void GetInfoLiquidazione(IView.IInfoLiquidazione infoLiquidazione)
        {
            InfoLiquidazione info = new InfoLiquidazione();
            info.Domanda = infoLiquidazione.InfoLiquidazione.Domanda;
            info.Categoria = infoLiquidazione.InfoLiquidazione.Categoria;
            info.Tipo = infoLiquidazione.InfoLiquidazione.Tipo;
            info.Sede = infoLiquidazione.InfoLiquidazione.Sede;
            info.Certificato = infoLiquidazione.InfoLiquidazione.Certificato;
            info.CodiceFiscale = infoLiquidazione.InfoLiquidazione.CodiceFiscale;
            info.Nome =infoLiquidazione.InfoLiquidazione.Nome;
            info.Cognome = infoLiquidazione.InfoLiquidazione.Cognome;
            infoLiquidazione.InfoLiquidazione = info;
        }
    }
}

