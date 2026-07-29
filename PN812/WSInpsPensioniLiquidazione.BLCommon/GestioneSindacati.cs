using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.DNA.Logging;
using System.Configuration;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneSindacati
    {
        #region public methods

        public static string GetIdCategoriaForSindacato(string SiglaCategoria, out string errori)
        {
            errori = string.Empty;
            string IdCategoria = string.Empty;
            if (String.IsNullOrEmpty(SiglaCategoria))
            {
                errori = "Categoria mancante";
                return string.Empty;
            }

            GestioneDecodifica.GetCodCategoriaBySiglaCategoria(SiglaCategoria, out IdCategoria);
            if (String.IsNullOrEmpty(IdCategoria))
            {
                errori = "Codice categoria mancante";
                return string.Empty;
            }

            IdCategoria = IdCategoria.Substring(IdCategoria.Length - 3); // ultimi tre caratteri
            return IdCategoria;
        }

        public static List<Entity.Sindacato> GetElencoSindacatiAttivi(List<Entity.Sindacato> elencoSindacati, out string errori)
        {
            errori = string.Empty;
            elencoSindacati = elencoSindacati.FindAll(x => x.Stato == Liquidazione.BLCommon.Utility.StatoSindacato.Attivo);
            if (elencoSindacati.Count == 0)
            {
                errori = "Non sono presenti Sindacati attivi";
                elencoSindacati = null;
            }

            return elencoSindacati;
        }

        #endregion public methods
    }
}
