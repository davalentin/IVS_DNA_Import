using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Presenter.Interface;

namespace INPS.Pensioni.LiquidazionePensione.Presenter.IView
{
    public interface ISedi : IViewUI
    {
        string CommaSeparatedSedi { get; set; }
        Dictionary<string, string> DictionaryOfficeList { get; set; }
        string Sede { get; set; }
        List<string> SediAbilitate { get; set; }
        INPS.DNA.Office SelectedOffice { get; set; }

    }
}
