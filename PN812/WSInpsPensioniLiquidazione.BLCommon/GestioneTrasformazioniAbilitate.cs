using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneTrasformazioniAbilitate
    {
        #region public members
        public static void CheckTrasformazioneAbilitata(TrasformazioneAbilitata trasformazioneAbilitata, bool isRiaperturaDomanda, Utility.TipoUnicarpe tipoUnicarpe, out bool IsTrasformazioneAbilitata)
        {
            IsTrasformazioneAbilitata = false;

            if (!isRiaperturaDomanda)
                IsTrasformazioneAbilitata = true;
            else
            {
                TrasformazioniAbilitate trasformazioneAbilitataDA = new TrasformazioniAbilitate();
                Utility.ValorizzaOggetti(trasformazioneAbilitata, trasformazioneAbilitataDA);
                TrasformazioniAbilitate trasformazioneAbilitataResult = null;
                DAGestioneTrasformazioniAbilitate.GetTrasformazioneAbilitata(trasformazioneAbilitataDA, out trasformazioneAbilitataResult);
                if (trasformazioneAbilitataResult != null)
                    IsTrasformazioneAbilitata = true;
            }
        }

        public static void GetAllTrasformazioniAbilitate(out List<TrasformazioneAbilitata> elencoTrasformazioniAbilitate)
        {
            elencoTrasformazioniAbilitate = null;
            List<TrasformazioniAbilitate> elencoTrasformazioniAbilitateDA = null;
            DAGestioneTrasformazioniAbilitate.GetAllTrasformazioniAbilitate(out elencoTrasformazioniAbilitateDA);

            if (elencoTrasformazioniAbilitateDA == null || elencoTrasformazioniAbilitateDA.Count == 0)
                return;

            elencoTrasformazioniAbilitate = new List<TrasformazioneAbilitata>();
            foreach (TrasformazioniAbilitate taDA in elencoTrasformazioniAbilitateDA)
            {
                TrasformazioneAbilitata ta = new TrasformazioneAbilitata();
                Utility.ValorizzaOggetti(taDA, ta);
                elencoTrasformazioniAbilitate.Add(ta);
            }
        }

        public static void SalvaTrasformazioneAbilitata(TrasformazioneAbilitata trasformazioneAbilitata)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (trasformazioneAbilitata != null)
                {
                    TrasformazioniAbilitate trasformazioneAbilitataDA = new TrasformazioniAbilitate();
                    Utility.ValorizzaOggetti(trasformazioneAbilitata, trasformazioneAbilitataDA);
                    DAGestioneTrasformazioniAbilitate.SalvaTrasformazioneAbilitata(trasformazioneAbilitataDA);
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaTrasformazioniAbilitateSuTutteLeSedi(List<TrasformazioneAbilitata> elencoTrasformazioniAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (elencoTrasformazioniAbilitate != null && elencoTrasformazioniAbilitate.Count > 0)
                {
                    foreach (TrasformazioneAbilitata trasformazioneAbilitata in elencoTrasformazioniAbilitate)
                    {
                        TrasformazioniAbilitate trasformazioneAbilitataDA = new TrasformazioniAbilitate();
                        Utility.ValorizzaOggetti(trasformazioneAbilitata, trasformazioneAbilitataDA);
                        DAGestioneTrasformazioniAbilitate.SalvaTrasformazioneAbilitata(trasformazioneAbilitataDA);
                    }
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaTrasformazioneAbilitata(TrasformazioneAbilitata trasformazioneAbilitata)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (trasformazioneAbilitata != null)
                {
                    TrasformazioniAbilitate trasformazioneAbilitataDA = new TrasformazioniAbilitate();
                    Utility.ValorizzaOggetti(trasformazioneAbilitata, trasformazioneAbilitataDA);
                    DAGestioneTrasformazioniAbilitate.EliminaTrasformazioneAbilitata(trasformazioneAbilitataDA);
                    transactionScope.Complete();
                }
            }
        }

        public static void GetSigleCategorieAmmesse(out List<string> elencoSigleCategoria, string tipologia)
        {
            elencoSigleCategoria = null;

            List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione = null;
            GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensione);

            /////////////////////////////////////////////////////////////////////
            //Filtro la lista in base alla TipologiaAppartenenzaOperatore
            elencoCategoriePensione = elencoCategoriePensione.FindAll(x => x.AppartenenzaCatPensione == tipologia);
            /////////////////////////////////////////////////////////////////////

            if (elencoCategoriePensione != null && elencoCategoriePensione.Count > 0)
            {
                List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensioneFS = elencoCategoriePensione.FindAll(x => x.TipoCatPensione == 'C');
                if (elencoCategoriePensioneFS != null && elencoCategoriePensioneFS.Count > 0)
                {
                    foreach (GestioneDecodifica.CategoriaPensione cpFS in elencoCategoriePensioneFS)
                    {
                        if (cpFS.SiglaCatPensione.Trim().ToUpperInvariant() == "PI")
                        {
                            List<char> categoriePI = new List<char> { 'A', '1', 'Y', 'U', 'V' };
                            foreach (char categoria in categoriePI)
                            {
                                string siglaCatPensione = cpFS.SiglaCatPensione.Trim().ToUpperInvariant() + categoria.ToString();

                                DecCatPensione decCatPens = new DecCatPensione();
                                decCatPens.TipoCatPensione = 'V';
                                decCatPens.SiglaCatPensione = "V" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));

                                decCatPens = new DecCatPensione();
                                decCatPens.TipoCatPensione = 'I';
                                decCatPens.SiglaCatPensione = "I" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));

                                decCatPens = new DecCatPensione();
                                decCatPens.TipoCatPensione = 'S';
                                decCatPens.SiglaCatPensione = "S" + siglaCatPensione;
                                decCatPens.CodCatPensione = cpFS.CodCatPensione;
                                elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));
                            }
                        }
                        else
                        {
                            DecCatPensione decCatPens = new DecCatPensione();
                            decCatPens.TipoCatPensione = 'V';
                            decCatPens.SiglaCatPensione = "V" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));

                            decCatPens = new DecCatPensione();
                            decCatPens.TipoCatPensione = 'I';
                            decCatPens.SiglaCatPensione = "I" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));

                            decCatPens = new DecCatPensione();
                            decCatPens.TipoCatPensione = 'S';
                            decCatPens.SiglaCatPensione = "S" + cpFS.SiglaCatPensione;
                            decCatPens.CodCatPensione = cpFS.CodCatPensione;
                            elencoCategoriePensione.Add(new GestioneDecodifica.CategoriaPensione(decCatPens));
                        }
                    }

                    elencoCategoriePensione.RemoveAll(x => x.TipoCatPensione == 'C');
                }

                if (elencoCategoriePensione.Count > 0)
                {
                    elencoSigleCategoria = new List<string>();
                    foreach (GestioneDecodifica.CategoriaPensione cp in elencoCategoriePensione)
                    {
                        elencoSigleCategoria.Add(cp.SiglaCatPensione.Trim());
                    }

                    elencoSigleCategoria = elencoSigleCategoria.OrderBy(x => x).ToList<string>();
                }
            }
        }

        public static void GetSediAmmesse(out List<INPS.DNA.Office> elencoSediProvinciali)
        {
            elencoSediProvinciali = Utility.GetListaSediProvinciali();
        }

        public static bool ControlSiglaCategoria(string siglaCategoria, string tipologia)
        {
            List<string> elencoSigleCategorie = null;
            Utility.GetListaSigleCategoriePerTipoApp(out elencoSigleCategorie, tipologia);
            return elencoSigleCategorie.Exists(x => x == siglaCategoria.Trim());
        }

        public static bool ControlSedeAmmessa(short sede)
        {
            List<INPS.DNA.Office> elencoSedi = null;
            GetSediAmmesse(out elencoSedi);
            return elencoSedi.Exists(x => x.AspnCode.PadLeft(4, '0').Substring(0, 4) == sede.ToString().PadLeft(4, '0'));
        }

        #endregion public members

        #region nested class
        public class TrasformazioneAbilitata
        {
            public TrasformazioneAbilitata()
            { }

            public TrasformazioneAbilitata(string siglaCategoria, System.Nullable<short> sede, string tipologia)
            {
                this._SiglaCategoria = siglaCategoria;
                this._Sede = sede;
                this._Tipologia = tipologia;
            }

            #region private properties
            private string _SiglaCategoria;

            private System.Nullable<short> _Sede;

            private string _Tipologia;
            #endregion private properties

            #region public properties
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            public System.Nullable<short> Sede { get { return _Sede; } set { _Sede = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            #endregion public properties
        }
        #endregion nested class
    }
}
