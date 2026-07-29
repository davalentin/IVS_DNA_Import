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
    public class GestioneDatiContributiviCi
    {
        #region PrestazioniEE
        public static void SalvaListaPrestazioniEstere(long idPensione, List<PensioniCiPrestazioniEE> listaPrestazioni)
        {
            if (idPensione != 0 && listaPrestazioni != null && listaPrestazioni.Count > 0)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (PensioniCiPrestazioniEE prestazione in listaPrestazioni)
                    {
                        prestazione.IdPensione = idPensione;
                        SalvaPrestazioneEstera(prestazione);
                    }
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaPrestazioneEstera(PensioniCiPrestazioniEE prestazione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.PensioniCiPrestazioniEE prestazioneDB = new DataCommon.PensioniCiPrestazioniEE();
                Utility.ValorizzaOggetti(prestazione, prestazioneDB);
                DAGestioneDatiContributiviCi.SalvaPrestazioneEstera(prestazioneDB);
                prestazione.Id = prestazioneDB.Id;
                transactionScope.Complete();
            }
        }

        public static void GetPrestazioniEEByIdPensione(long idPensione, out List<PensioniCiPrestazioniEE> listaPrestazioniEE)
        {
            listaPrestazioniEE = new List<PensioniCiPrestazioniEE>();

            List<DataCommon.PensioniCiPrestazioniEE> listaPrestazioniEEDB;
            DAGestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(idPensione, false, out listaPrestazioniEEDB);
            if (listaPrestazioniEEDB != null && listaPrestazioniEEDB.Count > 0)
            {
                foreach (DataCommon.PensioniCiPrestazioniEE PrestazioneEEDB in listaPrestazioniEEDB)
                {
                    PensioniCiPrestazioniEE PrestazioneEE = new PensioniCiPrestazioniEE();
                    Utility.ValorizzaOggetti(PrestazioneEEDB, PrestazioneEE);
                    listaPrestazioniEE.Add(PrestazioneEE);
                }
            }
        }

        public static void GetPrestazioniEEStoricoByIdPensione(long idPensione, out List<PensioniCiPrestazioniEE> listaPrestazioniEEStorico)
        {
            listaPrestazioniEEStorico = new List<PensioniCiPrestazioniEE>();
            List<DataCommon.PensioniCiPrestazioniEE> listaPrestazioniEEStoricoDB;
            DAGestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(idPensione, true, out listaPrestazioniEEStoricoDB);
            if (listaPrestazioniEEStoricoDB != null && listaPrestazioniEEStoricoDB.Count > 0)
            {
                foreach (DataCommon.PensioniCiPrestazioniEE prestazioneEEDBStorico in listaPrestazioniEEStoricoDB)
                {
                    PensioniCiPrestazioniEE prestazioneEE = new PensioniCiPrestazioniEE();
                    Utility.ValorizzaOggetti(prestazioneEEDBStorico, prestazioneEE);
                    listaPrestazioniEEStorico.Add(prestazioneEE);
                }
            }
        }

        public static void EliminaPrestazioniEE(long idPrestazioniEE)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteImportiEsteriPerPrestazione(idPrestazioniEE);
                DAGestioneDatiContributiviCi.DeletePrestazioniEE(idPrestazioniEE);
                transactionScope.Complete();
            }
        }

        public static void EliminaAllPrestazioniEE(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteAllImportiEsteriByIdPensione(idPensione);
                DAGestioneDatiContributiviCi.DeleteAllPrestazioniEENoStoricoByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }
        #endregion PrestazioniEE

        #region ImportiEsteri
        public static void SalvaImportoEstero(PensioniCiImportiEsteri importo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.PensioniCiImportiEsteri importoDB = new DataCommon.PensioniCiImportiEsteri();
                Utility.ValorizzaOggetti(importo, importoDB);
                DAGestioneDatiContributiviCi.SalvaImportoEstero(importoDB);
                transactionScope.Complete();
            }
        }

        public static void GetImportiEsteriByIdPensione(long idPensione, out List<PensioniCiImportiEsteri> listaImportiEsteri)
        {
            listaImportiEsteri = new List<PensioniCiImportiEsteri>();

            List<DataCommon.PensioniCiImportiEsteri> listaImportiEsteriDB;
            DAGestioneDatiContributiviCi.GetImportiEsteriByIdPensione(idPensione, out listaImportiEsteriDB);
            if (listaImportiEsteriDB != null && listaImportiEsteriDB.Count > 0)
            {
                foreach (DataCommon.PensioniCiImportiEsteri ImportoEsteroDB in listaImportiEsteriDB)
                {
                    PensioniCiImportiEsteri ImportoEstero = new PensioniCiImportiEsteri();
                    Utility.ValorizzaOggetti(ImportoEsteroDB, ImportoEstero);
                    listaImportiEsteri.Add(ImportoEstero);
                }
            }
        }

        public static void EliminaImportiEsteri(long idImportiEsteri)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteImportiEsteri(idImportiEsteri);
                transactionScope.Complete();
            }
        }

        public static void EliminaAllImportiEsteri(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteAllImportiEsteriByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaImportiEsteriPerPrestazione(long idPrestazioneEE)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteImportiEsteriPerPrestazione(idPrestazioneEE);
                transactionScope.Complete();
            }
        }
        #endregion ImportiEsteri



        #region Importi Esteri PensioniCIImportiValuta

        public static void SalvaImportiEsteriValuta(PensioniCiImportiValuta PensioniCiImportiValuta)
        {
            if (PensioniCiImportiValuta != null)
            {
                DataCommon.PensioniCiImportiValuta ImportoValutaDB = new DataCommon.PensioniCiImportiValuta();
                Utility.ValorizzaOggetti(PensioniCiImportiValuta, ImportoValutaDB);
                DAGestioneDatiContributiviCi.SalvaDatiImportiEsteriValuta(ImportoValutaDB);
            }
        }

        public static void GetImportiEsteriValutaByIdPensione(long idPensione, out List<PensioniCiImportiValuta> LpensioniCiImportiValuta)
        {
            LpensioniCiImportiValuta = null;
            List<DataCommon.PensioniCiImportiValuta> listaImportiValutaDB = null;
            DAGestioneDatiContributiviCi.GetDatiImportiEsteriValutaByIdPensione(idPensione, out listaImportiValutaDB);
            if (listaImportiValutaDB != null && listaImportiValutaDB.Count > 0)
            {
                LpensioniCiImportiValuta = new List<PensioniCiImportiValuta>();
                foreach (DataCommon.PensioniCiImportiValuta ImportoValutaDB in listaImportiValutaDB)
                {
                    PensioniCiImportiValuta ImportoValuta = new PensioniCiImportiValuta();
                    Utility.ValorizzaOggetti(ImportoValutaDB, ImportoValuta);
                    LpensioniCiImportiValuta.Add(ImportoValuta);
                }
            }

        }

        public static void EliminaAllImportiEsteriValutaByIdPensione(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteDatiImportiEsteriValutaByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion Importi Esteri PensioniCIImportiValuta

        #region MaternitaAcna

        public static void SalvaMaternitaAcna(PensioniCiMaternitaAcna MaternitaAcna)
        {
            if (MaternitaAcna != null)
            {
                DataCommon.MaternitaAcna MaternitaAcnaDB = new DataCommon.MaternitaAcna();
                Utility.ValorizzaOggetti(MaternitaAcna, MaternitaAcnaDB);
                DAGestioneDatiContributiviCi.SalvaDatiMaternitaAcna(MaternitaAcnaDB);
            }
        }

        public static void GetMaternitaAcnaByIdPensione(long idPensione, out List<PensioniCiMaternitaAcna> LmaternitaAcna)
        {
            LmaternitaAcna = null;
            List<DataCommon.MaternitaAcna> listaMaternitaAcnaDB = null;
            DAGestioneDatiContributiviCi.GetDatiMaternitaAcnaByIdPensione(idPensione, out listaMaternitaAcnaDB);
            if (listaMaternitaAcnaDB != null && listaMaternitaAcnaDB.Count > 0)
            {
                LmaternitaAcna = new List<PensioniCiMaternitaAcna>();
                foreach (DataCommon.MaternitaAcna MaternitaAcnaDB in listaMaternitaAcnaDB)
                {
                    PensioniCiMaternitaAcna MaternitaAcna = new PensioniCiMaternitaAcna();
                    Utility.ValorizzaOggetti(MaternitaAcnaDB, MaternitaAcna);
                    LmaternitaAcna.Add(MaternitaAcna);
                }
            }
        }

        public static void EliminaAllMaternitaAcna(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteDatiMaternitaAcnaByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion MaternitaAcna

        #region Lavoratori Autonomi

        //public static void SalvaLavoratoriAutonomi(PensioniCiMaternitaAcna MaternitaAcna)
        //{
        //    if (MaternitaAcna != null)
        //    {
        //        DataCommon.MaternitaAcna MaternitaAcnaDB = new DataCommon.MaternitaAcna();
        //        Utility.ValorizzaOggetti(MaternitaAcna, MaternitaAcnaDB);
        //        DAGestioneDatiContributiviCi.SalvaDatiMaternitaAcna(MaternitaAcnaDB);
        //    }
        //}

        //public static void GetLavoratoriAutonomiByNumeroDomanda(long numeroDomanda, out List<PensioniCiMaternitaAcna> LmaternitaAcna)
        //{
        //    LmaternitaAcna = null;
        //    List<DataCommon.MaternitaAcna> listaMaternitaAcnaDB = null;
        //    DAGestioneDatiContributiviCi.GetDatiMaternitaAcnaByNumeroDomanda(numeroDomanda, out listaMaternitaAcnaDB);
        //    if (listaMaternitaAcnaDB != null && listaMaternitaAcnaDB.Count > 0)
        //    {
        //        LmaternitaAcna = new List<PensioniCiMaternitaAcna>();
        //        foreach (DataCommon.MaternitaAcna MaternitaAcnaDB in listaMaternitaAcnaDB)
        //        {
        //            PensioniCiMaternitaAcna MaternitaAcna = new PensioniCiMaternitaAcna();
        //            Utility.ValorizzaOggetti(MaternitaAcnaDB, MaternitaAcna);
        //            LmaternitaAcna.Add(MaternitaAcna);
        //        }
        //    }
        //}

        //public static void EliminaLavoratoriAutonomi(long idPensione)
        //{
        //    using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
        //                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
        //    {
        //        DAGestioneDatiContributiviCi.DeleteDatiMaternitaAcna(idPensione);
        //        transactionScope.Complete();
        //    }
        //}

        #endregion Lavoratori Autonomi

        #region DatiPostDecOriginaria
        public static void SalvaDatiPostDecOriginaria(long idPensione, List<DatiPostDecOriginaria> listaDatiPostDecOriginaria)
        {
            if (listaDatiPostDecOriginaria != null && listaDatiPostDecOriginaria.Count > 0)
            {
                List<DataCommon.DatiPostDecOriginaria> listaDatiPostDecOriginariaDB = new List<INPS.Pensioni.Liquidazione.DataCommon.DatiPostDecOriginaria>();

                foreach (DatiPostDecOriginaria datiPostDecOriginaria in listaDatiPostDecOriginaria)
                {
                    DataCommon.DatiPostDecOriginaria datiPostDecOriginariaDB = new INPS.Pensioni.Liquidazione.DataCommon.DatiPostDecOriginaria();

                    Utility.ValorizzaOggetti(datiPostDecOriginaria, datiPostDecOriginariaDB);
                    datiPostDecOriginariaDB.IdPensione = idPensione;
                    listaDatiPostDecOriginariaDB.Add(datiPostDecOriginariaDB);
                }

                if (listaDatiPostDecOriginariaDB.Count > 0)
                    DAGestioneDatiContributiviCi.SalvaDatiPostDecOriginaria(idPensione, listaDatiPostDecOriginariaDB);
                else
                    DAGestioneDatiContributiviCi.DeleteAllDatiPostDecOriginariaByIdPensione(idPensione);
            }
            else
            {
                DAGestioneDatiContributiviCi.DeleteAllDatiPostDecOriginariaByIdPensione(idPensione);
            }
        }

        public static void GetDatiPostDecOriginariaByIdPensione(long idPensione, out List<DatiPostDecOriginaria> listaDatiPostDecOriginaria)
        {
            listaDatiPostDecOriginaria = new List<DatiPostDecOriginaria>();

            List<DataCommon.DatiPostDecOriginaria> listaDatiPostDecOriginariaDB;
            DAGestioneDatiContributiviCi.GetDatiPostDecOriginariaByIdPensione(idPensione, out listaDatiPostDecOriginariaDB);
            if (listaDatiPostDecOriginariaDB != null && listaDatiPostDecOriginariaDB.Count > 0)
            {
                foreach (DataCommon.DatiPostDecOriginaria datiPostDecOriginariaDB in listaDatiPostDecOriginariaDB)
                {
                    DatiPostDecOriginaria datiPostDecOriginaria = new DatiPostDecOriginaria();
                    Utility.ValorizzaOggetti(datiPostDecOriginariaDB, datiPostDecOriginaria);
                    listaDatiPostDecOriginaria.Add(datiPostDecOriginaria);
                }
            }
        }

        public static void EliminaAllDatiPostDecOriginaria(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.DeleteAllDatiPostDecOriginariaByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }
        #endregion DatiPostDecOriginaria

        #region RedditiPerIntegrazioneVirtuale

        public static void SalvaRedditiPerIntegrazioneVirtuale(long idPensione, List<PensioniCiRedditiPerIntegrazioneVirtuale> LredditiPerIntegrazioneVirtuale)
        {
            if (LredditiPerIntegrazioneVirtuale != null && LredditiPerIntegrazioneVirtuale.Count() > 0)
            {
                List<DataCommon.RedditiPerIntegrazioneVirtuale> listaRedditiIntegrazVirtualeDB = new List<INPS.Pensioni.Liquidazione.DataCommon.RedditiPerIntegrazioneVirtuale>();

                foreach (PensioniCiRedditiPerIntegrazioneVirtuale datiRedditiIV in LredditiPerIntegrazioneVirtuale)
                {
                    DataCommon.RedditiPerIntegrazioneVirtuale datiRedditiIVDB = new INPS.Pensioni.Liquidazione.DataCommon.RedditiPerIntegrazioneVirtuale();

                    Utility.ValorizzaOggetti(datiRedditiIV, datiRedditiIVDB);
                    datiRedditiIVDB.IdPensione = idPensione;
                    listaRedditiIntegrazVirtualeDB.Add(datiRedditiIVDB);
                }
                DAGestioneDatiContributiviCi.SalvaRedditiPerIntegrazioneVirtuale(idPensione, listaRedditiIntegrazVirtualeDB);
            }
        }

        public static void GetRedditiPerIntegrazioneVirtuale(long idPensione, out List<PensioniCiRedditiPerIntegrazioneVirtuale> LredditiPerIntegrazioneVirtuale)
        {
            LredditiPerIntegrazioneVirtuale = null;
            List<DataCommon.RedditiPerIntegrazioneVirtuale> listaRedditiIntegrazVirtualeDB = null;
            DAGestioneDatiContributiviCi.GetRedditiPerIntegrazioneVirtuale(idPensione, out listaRedditiIntegrazVirtualeDB);
            if (listaRedditiIntegrazVirtualeDB != null && listaRedditiIntegrazVirtualeDB.Count > 0)
            {
                LredditiPerIntegrazioneVirtuale = new List<PensioniCiRedditiPerIntegrazioneVirtuale>();
                foreach (DataCommon.RedditiPerIntegrazioneVirtuale RedditiIntegrazVirtualeDB in listaRedditiIntegrazVirtualeDB)
                {
                    PensioniCiRedditiPerIntegrazioneVirtuale RedditiIntegrazVirtuale = new PensioniCiRedditiPerIntegrazioneVirtuale();
                    Utility.ValorizzaOggetti(RedditiIntegrazVirtualeDB, RedditiIntegrazVirtuale);
                    LredditiPerIntegrazioneVirtuale.Add(RedditiIntegrazVirtuale);
                }
            }
        }

        public static void EliminaAllRedditiPerIntegrazioneVirtuale(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneDatiContributiviCi.EliminaAllRedditiPerIntegrazioneVirtuale(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion RedditiPerIntegrazioneVirtuale

        #region nested classes
        [Serializable]
        public class PensioniCiPrestazioniEE
        {

            #region private properties
            private long _Id;

            private long _IdPensione;

            private string _CodiceStatoEE;

            private string _CodiceIstituzione;

            private string _MatricolaIstituzioneEE;

            private System.Nullable<int> _ContributiEEDecorrenzaOriginaria;

            private System.Nullable<int> _ContributiEERicalcolo;

            private System.Nullable<System.DateTime> _DecorrenzaLiquidazioneStatoEE;

            private System.Nullable<int> _ContributiEEDiritto;

            private System.Nullable<char> _SospensioneCautelativaIntegrazione;

            private System.Nullable<byte> _EtaSospensione;

            private System.Nullable<char> _CodiceArt48;

            private System.Nullable<System.DateTime> _DecorrenzaArt48;

            private System.Nullable<decimal> _QuotaIntegrazioneEEeArgentinaResidentiItalia;

            private System.Nullable<System.DateTime> _DecorrenzaIntegrazione;

            private System.Nullable<System.DateTime> _DecorrenzaRicalcolo;

            private System.Nullable<byte> _CodiceConvenzione;

            private System.Nullable<char> _CodicePi;

            private bool? _Confermato;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }

            public System.String CodiceStatoEE { get { return _CodiceStatoEE; } set { _CodiceStatoEE = value; } }

            public System.String CodiceIstituzione { get { return _CodiceIstituzione; } set { _CodiceIstituzione = value; } }

            public System.String MatricolaIstituzioneEE { get { return _MatricolaIstituzioneEE; } set { _MatricolaIstituzioneEE = value; } }

            public System.Nullable<int> ContributiEEDecorrenzaOriginaria { get { return _ContributiEEDecorrenzaOriginaria; } set { _ContributiEEDecorrenzaOriginaria = value; } }

            public System.Nullable<int> ContributiEERicalcolo { get { return _ContributiEERicalcolo; } set { _ContributiEERicalcolo = value; } }

            public System.Nullable<System.DateTime> DecorrenzaLiquidazioneStatoEE { get { return _DecorrenzaLiquidazioneStatoEE; } set { _DecorrenzaLiquidazioneStatoEE = value; } }

            public System.Nullable<int> ContributiEEDiritto { get { return _ContributiEEDiritto; } set { _ContributiEEDiritto = value; } }

            public System.Nullable<char> SospensioneCautelativaIntegrazione { get { return _SospensioneCautelativaIntegrazione; } set { _SospensioneCautelativaIntegrazione = value; } }

            public System.Nullable<byte> EtaSospensione { get { return _EtaSospensione; } set { _EtaSospensione = value; } }

            public System.Nullable<char> CodiceArt48 { get { return _CodiceArt48; } set { _CodiceArt48 = value; } }

            public System.Nullable<System.DateTime> DecorrenzaArt48 { get { return _DecorrenzaArt48; } set { _DecorrenzaArt48 = value; } }

            public System.Nullable<decimal> QuotaIntegrazioneEEeArgentinaResidentiItalia { get { return _QuotaIntegrazioneEEeArgentinaResidentiItalia; } set { _QuotaIntegrazioneEEeArgentinaResidentiItalia = value; } }

            public System.Nullable<System.DateTime> DecorrenzaIntegrazione { get { return _DecorrenzaIntegrazione; } set { _DecorrenzaIntegrazione = value; } }

            public System.Nullable<System.DateTime> DecorrenzaRicalcolo { get { return _DecorrenzaRicalcolo; } set { _DecorrenzaRicalcolo = value; } }

            public System.Nullable<byte> CodiceConvenzione { get { return _CodiceConvenzione; } set { _CodiceConvenzione = value; } }

            public System.Nullable<char> CodicePi { get { return _CodicePi; } set { _CodicePi = value; } }

            public bool? Confermato { get { return _Confermato; } set { _Confermato = value; } }
            #endregion public properties
        }

        public class PensioniCiImportiEsteri
        {

            #region private properties

            private long _Id;

            private System.Nullable<System.DateTime> _DecorrenzaPrestazioneEE;

            private System.Nullable<System.DateTime> _CessazionePrestazioneEE;

            private System.Nullable<decimal> _ImportoPrestazioneEE;

            private long _IDPrestazioneEE;

            #endregion private properties

            #region public properties

            public long Id { get { return _Id; } set { _Id = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPrestazioneEE { get { return _DecorrenzaPrestazioneEE; } set { _DecorrenzaPrestazioneEE = value; } }

            public System.Nullable<System.DateTime> CessazionePrestazioneEE { get { return _CessazionePrestazioneEE; } set { _CessazionePrestazioneEE = value; } }

            public System.Nullable<decimal> ImportoPrestazioneEE { get { return _ImportoPrestazioneEE; } set { _ImportoPrestazioneEE = value; } }

            public long IDPrestazioneEE { get { return _IDPrestazioneEE; } set { _IDPrestazioneEE = value; } }

            #endregion public properties
        }

        #region PensioniCiImportiValuta

        public class PensioniCiImportiValuta
        {
            #region public properties
            public long? IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? DecorrenzaPrestazioneEE { get { return _DecorrenzaPrestazioneEE; } set { _DecorrenzaPrestazioneEE = value; } }
            public decimal? ImportoPrestazioneEE { get { return _ImportoPrestazioneEE; } set { _ImportoPrestazioneEE = value; } }
            #endregion public properties

            #region private properties
            private long? _IdPensione;
            private DateTime? _DecorrenzaPrestazioneEE;
            private decimal? _ImportoPrestazioneEE;
            #endregion private properties
        }

        //public class ImportiEsteri
        //{
        //    public ImportiEsteri()
        //    {
        //    }

        //    #region public properties

        //    public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        //    public List<PensioniCiImportiValuta> LpensioniCiImportiValuta { get { return _LpensioniCiImportiValuta; } set { _LpensioniCiImportiValuta = value; } }
        //    #endregion public properties

        //    #region private properties

        //    private long _IdPensione;
        //    public List<PensioniCiImportiValuta> _LpensioniCiImportiValuta;

        //    #endregion private properties

        //}

        #endregion PensioniCiImportiValuta

        public class PensioniCiMaternitaAcna
        {
            #region private properties
            private long? _Id;
            private long _IdPensione;
            private decimal? _ImportoIVS;
            private int? _SettimaneAl1292;
            private int? _SettimaneDL50392;
            private char? _Tipo;
            #endregion private properties

            #region public properties
            public long? Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
            public int? SettimaneAl1292 { get { return _SettimaneAl1292; } set { _SettimaneAl1292 = value; } }
            public int? SettimaneDL50392 { get { return _SettimaneDL50392; } set { _SettimaneDL50392 = value; } }
            public char? Tipo { get { return _Tipo; } set { _Tipo = value; } }
            #endregion public properties
        }

        //public class IntegrazioneArt11
        //{
        //    private long _IdPensione;
        //    private decimal? _ImportoIVS;
        //    private DateTime? _Decorrenza;

        //    public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
        //    public decimal? ImportoIVS { get { return _ImportoIVS; } set { _ImportoIVS = value; } }
        //    public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
        //}

        public class PensioniCiLavoratoriAutonomi
        {
            #region private properties
            private long? _Id;
            private long _IdPensione;
            private int? _NContributiUtiliLavoratoriAutonomi;
            private int? _NSettimaneVVDirittoLavoratoriAutonomi;
            private int? _NSettimaneVVMisuraLavoratoriAutonomi;
            #endregion private properties

            #region public properties
            public long? Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int? NContributiUtiliLavoratoriAutonomi { get { return _NContributiUtiliLavoratoriAutonomi; } set { _NContributiUtiliLavoratoriAutonomi = value; } }
            public int? NSettimaneVVDirittoLavoratoriAutonomi { get { return _NSettimaneVVDirittoLavoratoriAutonomi; } set { _NSettimaneVVDirittoLavoratoriAutonomi = value; } }
            public int? NSettimaneVVMisuraLavoratoriAutonomi { get { return _NSettimaneVVMisuraLavoratoriAutonomi; } set { _NSettimaneVVMisuraLavoratoriAutonomi = value; } }
            #endregion public properties
        }

        public class DatiPostDecOriginaria
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private DateTime? _Decorrenza;
            private int? _CTR;
            private decimal? _IVS;
            private int? _SettimaneRetributive;
            private int? _SettimaneVV;
            private decimal? _RMS;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public int? CTR { get { return _CTR; } set { _CTR = value; } }
            public decimal? IVS { get { return _IVS; } set { _IVS = value; } }
            public int? SettimaneRetributive { get { return _SettimaneRetributive; } set { _SettimaneRetributive = value; } }
            public int? SettimaneVV { get { return _SettimaneVV; } set { _SettimaneVV = value; } }
            public decimal? RMS { get { return _RMS; } set { _RMS = value; } }
            #endregion public properties
        }

        public class PensioniCiRedditiPerIntegrazioneVirtuale
        {
            #region private properties
            private long _Id;
            private long _IdPensione;
            private int _Anno;
            private decimal? _Reddito;
            private bool _IsTitolare;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdPensione { get { return _IdPensione; } set { _IdPensione = value; } }
            public int Anno { get { return _Anno; } set { _Anno = value; } }
            public decimal? Reddito { get { return _Reddito; } set { _Reddito = value; } }
            public bool IsTitolare { get { return _IsTitolare; } set { _IsTitolare = value; } }
            #endregion public properties
        }

        #endregion nested classes
    }
}

