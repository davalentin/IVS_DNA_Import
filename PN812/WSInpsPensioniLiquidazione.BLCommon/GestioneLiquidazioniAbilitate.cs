using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneLiquidazioniAbilitate
    {
        #region public members
        public static void CheckLiquidazioneAbilitata(LiquidazioneAbilitata liquidazioneAbilitata, Utility.TipoDomanda tipoDomanda, bool isAutomatica, bool? isPLUnicarpe, out bool IsLiquidazioneAbilitata)
        {
            IsLiquidazioneAbilitata = false;
            bool isDomandaGestioneAutomatica = false;
            if (liquidazioneAbilitata.Tipologia == "FS")
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, liquidazioneAbilitata.SiglaCategoria);
                isDomandaGestioneAutomatica = tipoFondo == Utility.TipoFondo.FS || tipoFondo == Utility.TipoFondo.PT ||
                    Utility.GetListaSigleCategorieINPDAP().Exists(x => x == liquidazioneAbilitata.SiglaCategoria.Trim().ToUpper());
            }
            LiquidazioniAbilitate liquidazioneAbilitataDA = new LiquidazioniAbilitate();
            Utility.ValorizzaOggetti(liquidazioneAbilitata, liquidazioneAbilitataDA);
            LiquidazioniAbilitate liquidazioneAbilitataResult = null;
            DAGestioneLiquidazioniAbilitate.GetLiquidazioneAbilitata(liquidazioneAbilitataDA, out liquidazioneAbilitataResult);
            if (liquidazioneAbilitataResult != null)
            {
                switch (tipoDomanda)
                {
                    case Utility.TipoDomanda.Normale:
                    case Utility.TipoDomanda.Superstiti:
                    case Utility.TipoDomanda.Ripristino:
                    case Utility.TipoDomanda.RipristinoSuperstiti:
                    case Utility.TipoDomanda.Riliquidazione:
                    case Utility.TipoDomanda.RiliquidazioneSuperstiti:
                        if (isDomandaGestioneAutomatica)
                            IsLiquidazioneAbilitata = isAutomatica ? liquidazioneAbilitataResult.AbilitazioneAutomatica.GetValueOrDefault() : liquidazioneAbilitataResult.AbilitazioneManuale;
                        else
                            IsLiquidazioneAbilitata = true;
                        break;
                    case Utility.TipoDomanda.Ricostituzione:
                        if (isDomandaGestioneAutomatica)
                            IsLiquidazioneAbilitata = isPLUnicarpe.GetValueOrDefault() ? liquidazioneAbilitataResult.RicostituzioneDaAutomatica.GetValueOrDefault() : liquidazioneAbilitataResult.Ricostituzione.GetValueOrDefault();
                        else
                            IsLiquidazioneAbilitata = liquidazioneAbilitataResult.Ricostituzione.GetValueOrDefault();
                        break;
                }

                if (!IsLiquidazioneAbilitata || (!isDomandaGestioneAutomatica && !isAutomatica && !liquidazioneAbilitataResult.AbilitazioneManuale))
                    IsLiquidazioneAbilitata = false;
            }
        }

        public static void GetAllLiquidazioniAbilitate(out List<LiquidazioneAbilitata> elencoLiquidazioniAbilitate)
        {
            elencoLiquidazioniAbilitate = null;
            List<LiquidazioniAbilitate> elencoLiquidazioniAbilitateDA = null;
            DAGestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitateDA);

            if (elencoLiquidazioniAbilitateDA == null || elencoLiquidazioniAbilitateDA.Count == 0)
                return;

            elencoLiquidazioniAbilitate = new List<LiquidazioneAbilitata>();
            foreach (LiquidazioniAbilitate laDA in elencoLiquidazioniAbilitateDA)
            {
                LiquidazioneAbilitata la = new LiquidazioneAbilitata();
                Utility.ValorizzaOggetti(laDA, la);
                elencoLiquidazioniAbilitate.Add(la);
            }
        }

        public static void SalvaLiquidazioneAbilitata(LiquidazioneAbilitata liquidazioneAbilitata)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (liquidazioneAbilitata != null)
                {
                    LiquidazioniAbilitate liquidazioneAbilitataDA = new LiquidazioniAbilitate();
                    Utility.ValorizzaOggetti(liquidazioneAbilitata, liquidazioneAbilitataDA);
                    DAGestioneLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(liquidazioneAbilitataDA);
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaLiquidazioniAbilitateSuTutteLeSedi(List<LiquidazioneAbilitata> elencoLiquidazioniAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (elencoLiquidazioniAbilitate != null && elencoLiquidazioniAbilitate.Count > 0)
                {
                    foreach (LiquidazioneAbilitata liquidazioneAbilitata in elencoLiquidazioniAbilitate)
                    {
                        LiquidazioniAbilitate liquidazioneAbilitataDA = new LiquidazioniAbilitate();
                        Utility.ValorizzaOggetti(liquidazioneAbilitata, liquidazioneAbilitataDA);
                        DAGestioneLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(liquidazioneAbilitataDA);
                    }
                    transactionScope.Complete();
                }
            }
        }

        public static void EliminaLiquidazioneAbilitata(LiquidazioneAbilitata liquidazioneAbilitata)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (liquidazioneAbilitata != null)
                {
                    LiquidazioniAbilitate liquidazioneAbilitataDA = new LiquidazioniAbilitate();
                    Utility.ValorizzaOggetti(liquidazioneAbilitata, liquidazioneAbilitataDA);
                    DAGestioneLiquidazioniAbilitate.EliminaLiquidazioneAbilitata(liquidazioneAbilitataDA);
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
        public class LiquidazioneAbilitata
        {
            public LiquidazioneAbilitata()
            { }

            public LiquidazioneAbilitata(string siglaCategoria, System.Nullable<short> sede, string tipologia,
                 System.Nullable<bool> ricostituzione, bool abilitazioneManuale, System.Nullable<bool> ricostituzioneDaAutomatica, System.Nullable<bool> abilitazioneAutomatica)
            {
                this._SiglaCategoria = siglaCategoria;
                this._Sede = sede;
                this._Tipologia = tipologia;
                this._Ricostituzione = ricostituzione;
                this._AbilitazioneManuale = abilitazioneManuale;
                this._RicostituzioneDaAutomatica = ricostituzioneDaAutomatica;
                this._AbilitazioneAutomatica = abilitazioneAutomatica;
            }

            #region private properties
            private string _SiglaCategoria;

            private System.Nullable<short> _Sede;

            private string _Tipologia;

            private System.Nullable<bool> _Ricostituzione;

            private bool _AbilitazioneManuale;

            private System.Nullable<bool> _RicostituzioneDaAutomatica;

            private System.Nullable<bool> _AbilitazioneAutomatica; 
            #endregion private properties

            #region public properties
            public string SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }

            public System.Nullable<short> Sede { get { return _Sede; } set { _Sede = value; } }

            public string Tipologia { get { return _Tipologia; } set { _Tipologia = value; } }

            public System.Nullable<bool> Ricostituzione { get { return _Ricostituzione; } set { _Ricostituzione = value; } }

            public bool AbilitazioneManuale { get { return _AbilitazioneManuale; } set { _AbilitazioneManuale = value; } }

            public System.Nullable<bool> RicostituzioneDaAutomatica { get { return _RicostituzioneDaAutomatica; } set { _RicostituzioneDaAutomatica = value; } }

            public System.Nullable<bool> AbilitazioneAutomatica { get { return _AbilitazioneAutomatica; } set { _AbilitazioneAutomatica = value; } }
            #endregion public properties
        }
        #endregion nested class
    }
}
