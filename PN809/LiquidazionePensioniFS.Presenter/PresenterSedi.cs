using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterSedi
    {
        public void GetCommaSeparatedSedi(IView.ISedi SelezioneSede)
        {
            StringBuilder catBuilder = new StringBuilder();

            foreach (var i in INPS.DNA.Context.OfficeList.Offices)
            {
                string nomeSede = string.Format("{0}-{1}", i.Value.AspnCode, (i.Value.ExtendedProperties != null ? i.Value.ExtendedProperties["SEDE"].Trim() : i.Value.Name.Trim()));
                catBuilder.Append(";");
                catBuilder.Append(nomeSede);
            }

            SelezioneSede.CommaSeparatedSedi = catBuilder.ToString();
        }

        public void GetOfficeSediAbilitate(IView.ISedi SelezioneSede)
        {
            SelezioneSede.DictionaryOfficeList = new Dictionary<string, string>();
            SelezioneSede.DictionaryOfficeList.Clear();
            foreach (string item in SelezioneSede.SediAbilitate)
            {
                INPS.DNA.Office o = (from s in INPS.DNA.Context.OfficeList.Offices
                                     where s.Key == item
                                     select s).FirstOrDefault().Value;
				if (o == null)
					continue;
                string nomeSede = string.Format("{0} - {1}", o.AspnCode, (o.ExtendedProperties != null ? o.ExtendedProperties["SEDE"].Trim() : o.Name.Trim()));
                SelezioneSede.DictionaryOfficeList.Add(nomeSede, (o.ExtendedProperties != null ? o.ExtendedProperties["SEDE"].Trim() : o.Name.Trim()));
            }
        }

        public List<string> GetOfficeAspnCodeAbilitati(List<string> sediAbilitate)
        {
            List<string> elencoSediAbilitate = new List<string>();
            foreach (string item in sediAbilitate)
            {
                INPS.DNA.Office o = (from s in INPS.DNA.Context.OfficeList.Offices
                                     where s.Key == item
                                     select s).FirstOrDefault().Value;
                if (o == null)
                    continue;

                elencoSediAbilitate.Add(o.AspnCode);
            }

            return elencoSediAbilitate;
        }

        public void GetOffice(IView.ISedi SelezioneSede)
        {
            SelezioneSede.SelectedOffice = (from s in INPS.DNA.Context.OfficeList.Offices
                                            where string.Compare((s.Value.ExtendedProperties != null ? s.Value.ExtendedProperties["SEDE"].Trim() : s.Value.Name), SelezioneSede.Sede, true, System.Globalization.CultureInfo.InvariantCulture) == 0
                                            select s).FirstOrDefault().Value;
        }

        public void GetCommaSeparatedSediCode(IView.ISedi SelezioneSede)
        {
            StringBuilder catBuilder = new StringBuilder();

            foreach (string aspnCode in ((from s in INPS.DNA.Context.OfficeList.Offices
                                          select s.Value.AspnCode).Distinct().ToList()).OrderBy(x => Convert.ToInt32(x)).ToList())
            {
                string nomeSede = aspnCode;
                catBuilder.Append(";");
                catBuilder.Append(nomeSede);
            }
            SelezioneSede.CommaSeparatedSedi = catBuilder.ToString();
        }
    }
}
