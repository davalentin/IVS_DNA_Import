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
    public class GestioneFondo
    {
        public static void GetIdFondoByIdPensione(Int64 idPensione, out long idFondo)
        {
            idFondo = 0;
            DAGestioneFondo.GetIdFondoByIdPensione(idPensione, out idFondo);
        }

        public static void GetFondoDatiGenericiByIdPensione(Int64 idPensione, out DatiFondo datiFondo)
        {
            PensioneFondoDatiGenerici fondo = null;
            datiFondo = null;
            DAGestioneFondo.GetFondoDatiGenericiByIdPensione(idPensione, out fondo);
            if (fondo == null)
                return;
            datiFondo = new DatiFondo();
            Utility.ValorizzaOggetti(fondo, datiFondo);
        }

        public static void SalvaFondoDatiGenerici(long idPensione, DatiFondo datiFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoDatiGenerici fondo = new PensioneFondoDatiGenerici();
                Utility.ValorizzaOggetti(datiFondo, fondo);
                fondo.IdPensione = idPensione;
                DAGestioneFondo.SalvaFondoDatiGenerici(fondo);
                datiFondo.Id = fondo.Id;
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoDatiGenerici(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoDatiGenericiByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void GetFondoXXByDatiPensione(GestionePensione.DatiPensione datiPensione, out Object objectFondoXX)
        {
            objectFondoXX = null;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);

            GestioneFondo.DatiFondoEL datiFondoEL = null;
            GestioneFondo.DatiFondoTT datiFondoTT = null;
            GestioneFondo.DatiFondoET datiFondoET = null;
            GestioneFondo.DatiFondoVL datiFondoVL = null;
            List<GestioneFondo.DatiFondoFST> listaDatiFondoFS = null;
            List<GestioneFondo.DatiFondoPT> listaDatiFondoPT = null;
            List<GestioneFondo.DatiFondoPI> datiFondoPI = null;
            GestioneFondo.DatiFondoCL datiFondoCL = null;
            GestioneFondo.DatiFondoES datiFondoES = null;
            List<GestioneFondo.DatiFondoDZ> listadatiFondoDZ = null;
            GestioneFondo.DatiFondoGAS datiFondoGAS = null;
            GestioneFondo.DatiFondoPM datiFondoPM = null;
            List<GestionePensioneINPDAP.DatiPensioneINPDAP> listaDatiFondoINPDAP = null;

            if (tipoFondo.HasValue)
            {
                switch (tipoFondo)
                {
                    case Utility.TipoFondo.EL:
                        GestioneFondo.GetFondoELByIdPensione(datiPensione.Id, out datiFondoEL);
                        objectFondoXX = datiFondoEL;
                        break;
                    case Utility.TipoFondo.TT:
                        GestioneFondo.GetFondoTTByIdPensione(datiPensione.Id, out datiFondoTT);
                        objectFondoXX = datiFondoTT;
                        break;
                    case Utility.TipoFondo.ET:
                        GestioneFondo.GetFondoETByIdPensione(datiPensione.Id, out datiFondoET);
                        objectFondoXX = datiFondoET;
                        break;
                    case Utility.TipoFondo.VL:
                        GestioneFondo.GetFondoVLByIdPensione(datiPensione.Id, out datiFondoVL);
                        objectFondoXX = datiFondoVL;
                        break;
                    case Utility.TipoFondo.FS:
                        GestioneFondo.GetFondoFSRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoFS);
                        objectFondoXX = listaDatiFondoFS;
                        break;
                    case Utility.TipoFondo.PT:
                        GestioneFondo.GetFondoPTRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoPT);
                        objectFondoXX = listaDatiFondoPT;
                        break;
                    case Utility.TipoFondo.PI:
                    case Utility.TipoFondo.PL:
                        GestioneFondo.GetFondoPIRecordFondoByIdPensione(datiPensione.Id, out datiFondoPI);
                        objectFondoXX = datiFondoPI;
                        break;
                    case Utility.TipoFondo.CL:
                        GestioneFondo.GetFondoCLByIdPensione(datiPensione.Id, out datiFondoCL);
                        objectFondoXX = datiFondoCL;
                        break;
                    case Utility.TipoFondo.ES:
                        GestioneFondo.GetFondoESByIdPensione(datiPensione.Id, out datiFondoES);
                        objectFondoXX = datiFondoES;
                        break;
                    case Utility.TipoFondo.DZ:
                        GestioneFondo.GetFondoDZRecordFondoByIdPensione(datiPensione.Id, out listadatiFondoDZ);
                        objectFondoXX = listadatiFondoDZ;
                        break;
                    case Utility.TipoFondo.GAS:
                        GestioneFondo.GetFondoGASByIdPensione(datiPensione.Id, out datiFondoGAS);
                        objectFondoXX = datiFondoGAS;
                        break;
                    case Utility.TipoFondo.PM:
                        GestioneFondo.GetFondoPMByIdPensione(datiPensione.Id, out datiFondoPM);
                        objectFondoXX = datiFondoPM;
                        break;
                }
            }
            else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                GestionePensioneINPDAP.GetPensioneINPDAPRecordFondoByIdPensione(datiPensione.Id, out listaDatiFondoINPDAP);
                objectFondoXX = listaDatiFondoINPDAP;
            }
        }

        //public static Utility.TipoFondo? GetTipoFondoByNumeroDomanda(long numeroDomanda)
        //{
        //    if (datiPensione != null)
        //    {
        //        return Utility.GeTipoFondoByCategoria(datiPensione.SiglaCategoria);
        //    }
        //    else
        //        return null;
        //}

        #region FondoEL
        public static void GetFondoELByIdPensione(Int64 idPensione, out DatiFondoEL datiFondoEL)
        {
            PensioneFondoEL fondoEL = null;
            datiFondoEL = null;
            DAGestioneFondo.GetFondoELByIdPensione(idPensione, out fondoEL);
            if (fondoEL == null)
                return;
            datiFondoEL = new DatiFondoEL();
            Utility.ValorizzaOggetti(fondoEL, datiFondoEL);
        }

        public static void SalvaFondoEL(long idFondo, DatiFondoEL datiFondoEL)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoEL fondoEL = new PensioneFondoEL();
                Utility.ValorizzaOggetti(datiFondoEL, fondoEL);
                fondoEL.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoEL(fondoEL);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoEL(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoELByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoEL

        #region Fondo TT

        public static void GetFondoTTByIdPensione(long idPensione, out DatiFondoTT datiFondoTT)
        {
            PensioneFondoTT fondoTT = null;
            datiFondoTT = null;
            DAGestioneFondo.GetFondoTTByIdPensione(idPensione, out fondoTT);
            if (fondoTT == null)
                return;
            datiFondoTT = new DatiFondoTT();
            Utility.ValorizzaOggetti(fondoTT, datiFondoTT);
        }

        public static void SalvaFondoTT(long idFondo, DatiFondoTT datiFondoTT)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoTT fondoTT = new PensioneFondoTT();
                Utility.ValorizzaOggetti(datiFondoTT, fondoTT);
                fondoTT.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoTT(fondoTT);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoTT(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoTTByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion Fondo TT

        #region FondoET

        public static void GetFondoETByIdPensione(Int64 idPensione, out DatiFondoET datiFondoET)
        {
            PensioneFondoET fondoET = null;
            datiFondoET = null;
            DAGestioneFondo.GetFondoETByIdPensione(idPensione, out fondoET);
            if (fondoET == null)
                return;
            datiFondoET = new DatiFondoET();
            Utility.ValorizzaOggetti(fondoET, datiFondoET);
        }

        public static void SalvaFondoET(long idFondo, DatiFondoET datiFondoET)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoET fondoET = new PensioneFondoET();
                Utility.ValorizzaOggetti(datiFondoET, fondoET);
                fondoET.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoET(fondoET);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoET(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoETByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoET

        #region FondoVL

        public static void GetFondoVLByIdPensione(Int64 idPensione, out DatiFondoVL datiFondoVL)
        {
            PensioneFondoVL fondoVL = null;
            datiFondoVL = null;
            DAGestioneFondo.GetFondoVLByIdPensione(idPensione, out fondoVL);
            if (fondoVL == null)
                return;
            datiFondoVL = new DatiFondoVL();
            Utility.ValorizzaOggetti(fondoVL, datiFondoVL);
        }

        public static void SalvaFondoVL(long idFondo, DatiFondoVL datiFondoVL)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoVL fondoVL = new PensioneFondoVL();
                Utility.ValorizzaOggetti(datiFondoVL, fondoVL);
                fondoVL.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoVL(fondoVL);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoVL(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoVLByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoVL

        #region FondoFST

        public static void GetFondoFSTByIdPensione(Int64 idPensione, out DatiFondoFST datiFondoFST)
        {
            PensioneFondoFST fondoFST = null;
            datiFondoFST = null;
            DAGestioneFondo.GetFondoFSTByIdPensione(idPensione, out fondoFST);
            if (fondoFST == null)
                return;
            datiFondoFST = new DatiFondoFST();
            Utility.ValorizzaOggetti(fondoFST, datiFondoFST);
        }

        public static void GetFondoFSRecordFondoByIdPensione(Int64 idPensione, out List<DatiFondoFST> listaDatiFondoFST)
        {
            List<PensioneFondoFST> listaFondoFST = null;
            listaDatiFondoFST = null;
            DAGestioneFondo.GetFondoFSTRecordFondoByIdPensione(idPensione, out listaFondoFST);
            if (listaFondoFST == null || listaFondoFST.Count == 0)
                return;
            listaDatiFondoFST = new List<DatiFondoFST>();
            foreach (PensioneFondoFST fondoFST in listaFondoFST)
            {
                DatiFondoFST datiFondoFST = new DatiFondoFST();
                Utility.ValorizzaOggetti(fondoFST, datiFondoFST);
                listaDatiFondoFST.Add(datiFondoFST);
            }
        }

        public static void GetFondoFSTByIdRecordFondo(Int64 idRecordFondo, out DatiFondoFST datiFondoFST)
        {
            PensioneFondoFST fondoFST = null;
            datiFondoFST = null;
            DAGestioneFondo.GetFondoFSTByIdRecordFondo(idRecordFondo, out fondoFST);
            if (fondoFST == null)
                return;
            datiFondoFST = new DatiFondoFST();
            Utility.ValorizzaOggetti(fondoFST, datiFondoFST);
        }

        public static void SalvaFondoFST(long idFondo, DatiFondoFST datiFondoFST)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoFST fondoFST = new PensioneFondoFST();
                Utility.ValorizzaOggetti(datiFondoFST, fondoFST);
                fondoFST.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoFST(fondoFST);
                transactionScope.Complete();
            }
        }

        public static void SalvaFondoFSTRecordFondo(long idFondo, long idRecordFondo, DatiFondoFST datiFondoFST)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoFST fondoFST = new PensioneFondoFST();
                Utility.ValorizzaOggetti(datiFondoFST, fondoFST);
                fondoFST.IdFondo = idFondo;
                fondoFST.IdRecordFondo = idRecordFondo;
                DAGestioneFondo.SalvaFondoFSTRecordFondo(fondoFST);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoFST(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoFSTByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoFSTByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoFSTByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #endregion FondoFST

        #region FondoPT

        public static void GetFondoPTByIdPensione(Int64 idPensione, out DatiFondoPT datiFondoPT)
        {
            PensioneFondoPT fondoPT = null;
            datiFondoPT = null;
            DAGestioneFondo.GetFondoPTByIdPensione(idPensione, out fondoPT);
            if (fondoPT == null)
                return;
            datiFondoPT = new DatiFondoPT();
            Utility.ValorizzaOggetti(fondoPT, datiFondoPT);
        }

        public static void GetFondoPTRecordFondoByIdPensione(Int64 idPensione, out List<DatiFondoPT> listaDatiFondoPT)
        {
            List<PensioneFondoPT> listaFondoPT = null;
            listaDatiFondoPT = null;
            DAGestioneFondo.GetFondoPTRecordFondoByIdPensione(idPensione, out listaFondoPT);
            if (listaFondoPT == null || listaFondoPT.Count == 0)
                return;
            listaDatiFondoPT = new List<DatiFondoPT>();
            foreach (PensioneFondoPT fondoPT in listaFondoPT)
            {
                DatiFondoPT datiFondoPT = new DatiFondoPT();
                Utility.ValorizzaOggetti(fondoPT, datiFondoPT);
                listaDatiFondoPT.Add(datiFondoPT);
            }
        }

        public static void GetFondoPTByIdRecordFondo(Int64 idRecordFondo, out DatiFondoPT datiFondoPT)
        {
            PensioneFondoPT fondoPT = null;
            datiFondoPT = null;
            DAGestioneFondo.GetFondoPTByIdRecordFondo(idRecordFondo, out fondoPT);
            if (fondoPT == null)
                return;
            datiFondoPT = new DatiFondoPT();
            Utility.ValorizzaOggetti(fondoPT, datiFondoPT);
        }

        public static void SalvaFondoPT(long idFondo, DatiFondoPT datiFondoPT)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoPT fondoPT = new PensioneFondoPT();
                Utility.ValorizzaOggetti(datiFondoPT, fondoPT);
                fondoPT.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoPT(fondoPT);
                transactionScope.Complete();
            }
        }

        public static void SalvaFondoPTRecordFondo(long idFondo, long idRecordFondo, DatiFondoPT datiFondoPT)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoPT fondoPT = new PensioneFondoPT();
                Utility.ValorizzaOggetti(datiFondoPT, fondoPT);
                fondoPT.IdFondo = idFondo;
                fondoPT.IdRecordFondo = idRecordFondo;
                DAGestioneFondo.SalvaFondoPTRecordFondo(fondoPT);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoPT(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoPTByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoPTByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoPTByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #endregion FondoPT

        #region FondoPI

        public static void GetElencoPensioneFondoPIByIdPensione_pretabella(Int64 idPensione, out List<PretabellaPensioneFondoPI> datiAgoFondoPI)
        {
            datiAgoFondoPI = new List<PretabellaPensioneFondoPI>();

            object oggetto;

            DAGestioneFondo.GetFondoPIRecordFondoByIdPensione_pretabella(idPensione, out oggetto);

            if (oggetto == null)
                return;

            datiAgoFondoPI = ConvertToFondoPIDTOList(oggetto);
        }
        private static List<PretabellaPensioneFondoPI> ConvertToFondoPIDTOList(object queryObject)
        {
            List<PretabellaPensioneFondoPI> result = new List<PretabellaPensioneFondoPI>();

            if (queryObject == null)
                return result;

            IQueryable query = queryObject as IQueryable;
            if (query == null)
                return result;

            System.Collections.IEnumerable list = query.Cast<object>().ToList();

            foreach (object item in list)
            {
                Type t = item.GetType();

                PretabellaPensioneFondoPI dto = new PretabellaPensioneFondoPI();

                dto.IdFondo = (long)t.GetProperty("IdFondo").GetValue(item, null);
                dto.IdRecordFondo = (long)t.GetProperty("IdRecordFondo").GetValue(item, null);
                dto.SemaforoRecord = (byte?)t.GetProperty("SemaforoRecord").GetValue(item, null);
                dto.DecorrenzaFondo = (DateTime?)t.GetProperty("DecorrenzaValiditaDati").GetValue(item, null);

                result.Add(dto);
            }

            return result;
        }

        public static void GetDettaglioPensioneFondoPIByIdRecord(long idRecordFondo, out DatiFondoPI datiPensioneFondoPI)
        {
            PensioneFondoPI datiPensioneFondoPIEntity = null;
            datiPensioneFondoPI = null;

            DAGestioneFondo.GetPensioneFondoPIByIdRecord(idRecordFondo, out datiPensioneFondoPIEntity);

            if (datiPensioneFondoPIEntity == null)
                return;

            datiPensioneFondoPI = new DatiFondoPI();
            Utility.ValorizzaOggetti(datiPensioneFondoPIEntity, datiPensioneFondoPI);
        }

        public static void GetFondoPIByIdPensione(Int64 idPensione, out DatiFondoPI datiFondoPI)
        {
            PensioneFondoPI fondoPI = null;
            datiFondoPI = null;
            DAGestioneFondo.GetFondoPIByIdPensione(idPensione, out fondoPI);
            if (fondoPI == null)
                return;
            datiFondoPI = new DatiFondoPI();
            Utility.ValorizzaOggetti(fondoPI, datiFondoPI);
        }

        public static void GetFondoPIRecordFondoByIdPensione(Int64 idPensione, out List<DatiFondoPI> listaDatiFondoPI)
        {
            List<PensioneFondoPI> listaFondoPI = null;
            listaDatiFondoPI = null;
            DAGestioneFondo.GetFondoPIRecordFondoByIdPensione(idPensione, out listaFondoPI);
            if (listaFondoPI == null || listaFondoPI.Count == 0)
                return;
            listaDatiFondoPI = new List<DatiFondoPI>();
            foreach (PensioneFondoPI fondoPI in listaFondoPI)
            {
                DatiFondoPI datiFondoPI = new DatiFondoPI();
                Utility.ValorizzaOggetti(fondoPI, datiFondoPI);
                listaDatiFondoPI.Add(datiFondoPI);
            }
        }

        public static void SalvaFondoPIRecordFondo(long idFondo, long? idRecordFondo, DatiFondoPI datiFondoPI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoPI fondoPI = new PensioneFondoPI();
                Utility.ValorizzaOggetti(datiFondoPI, fondoPI);
                fondoPI.IdFondo = idFondo;
                fondoPI.IdRecordFondo = idRecordFondo;
                DAGestioneFondo.SalvaFondoPIRecordFondo(fondoPI);
                datiFondoPI.Id = fondoPI.Id;
                transactionScope.Complete();
            }
        }

        public static void SalvaFondoPIEmpty(long idFondo, List<long> listaIdRecordFondo)
        {
            string csv = string.Join(",", listaIdRecordFondo.Select(x => x.ToString()).ToArray());
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.SalvaFondoPIEmpty(idFondo, csv);
                transactionScope.Complete();
            }

        }

        public static void EliminaFondoPIRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoPIRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoPI(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoPIByIdPensione(idPensione);
                DAGestioneFondo.EliminaFondoPIByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoPI

        #region Dati AGO PI

        //restituisce i dati della pretabella
        public static void GetElencoDatiAgoPIByIdPensione_pretabella(Int64 idPensione, out List<PretabellaDatiAgoFondoPI> datiAgoFondoPI)
        {
            datiAgoFondoPI = new List<PretabellaDatiAgoFondoPI>();
            List<DatiAgoPensioneFondoPI> listaEntity = null;

            //List<DatiAgoPensioneFondoPI> listaDatiAgo = new List<DatiAgoPensioneFondoPI>();
            DAGestioneFondo.GetListaDatiAgoPIByIdPensione(idPensione, out listaEntity);

            if (listaEntity == null)
                return;

            foreach (DatiAgoPensioneFondoPI entity in listaEntity)
            {
                PretabellaDatiAgoFondoPI dto = new PretabellaDatiAgoFondoPI();
                Utility.ValorizzaOggetti(entity, dto);
                datiAgoFondoPI.Add(dto);
            }
        }

        public static void GetDatiAgoPIById(long idDatiAgo, out DatiAgoPI datiAgoPI)
        {
            DatiAgoPensioneFondoPI datiAgoEntity = null;
            datiAgoPI = null;

            DAGestioneFondo.GetDatiAgoPIById(idDatiAgo, out datiAgoEntity);

            if (datiAgoEntity == null)
                return;

            datiAgoPI = new DatiAgoPI();
            Utility.ValorizzaOggetti(datiAgoEntity, datiAgoPI);
        }

        public static void GetListaDatiAgoPIByIdPensione(long idPensione, out List<DatiAgoPI> listaDatiAgoPI)
        {
            listaDatiAgoPI = new List<DatiAgoPI>();

            List<DatiAgoPensioneFondoPI> listaEntity = null;

            DAGestioneFondo.GetListaDatiAgoPIByIdPensione(idPensione, out listaEntity);

            if (listaEntity == null || listaEntity.Count == 0)
                return;

            foreach (DatiAgoPensioneFondoPI entity in listaEntity)
            {
                DatiAgoPI dto = new DatiAgoPI();
                Utility.ValorizzaOggetti(entity, dto);
                listaDatiAgoPI.Add(dto);
            }
        }

        public static void InsertOrUpdateDatiAgoPI(long? idDatiAgoFondoPI, DatiAgoPI datiAgoPI)
        {
            using (TransactionScope transactionScope =
                TransactionScopeFactory.Create(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoPensioneFondoPI datiAgoEntity = new DatiAgoPensioneFondoPI();

                Utility.ValorizzaOggetti(datiAgoPI, datiAgoEntity);
                datiAgoEntity.Id = idDatiAgoFondoPI != null ? (long)idDatiAgoFondoPI : 0;

                DAGestioneFondo.SalvaDatiAgoPIRecordFondo(datiAgoEntity);

                transactionScope.Complete();
            }
        }

        public static void SalvaDatiAgoPIRecordFondo(long? idFondo, DatiAgoPI datiAgoPI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoPensioneFondoPI datiAgoEntity = new DatiAgoPensioneFondoPI();
                Utility.ValorizzaOggetti(datiAgoPI, datiAgoEntity);
                datiAgoEntity.IdFondo = idFondo;
                DAGestioneFondo.SalvaDatiAgoPIRecordFondo(datiAgoEntity);
                transactionScope.Complete();
            }
        }
        public static void EliminaDatiAgoPI(long idPensione)
        {
            using (TransactionScope transactionScope =
                TransactionScopeFactory.Create(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoPIByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiAgoPISingolo(long idDatiAgo)
        {
            using (TransactionScope transactionScope =
              TransactionScopeFactory.Create(
                  TransactionScopeOption.Required,
                  new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoPIById(idDatiAgo);
                transactionScope.Complete();
            }
        }

        #endregion Dati AGO PI

        #region Dati AGO TEORICO PI

        public static void GetListaDatiAgoTeoricoPIByIdPensione(
            long idPensione,
            out List<DatiAgoTeoricoPI> listaDatiAgoTeoricoPI)
        {
            listaDatiAgoTeoricoPI = new List<DatiAgoTeoricoPI>();

            List<DatiAgoTeoricoPensioneFondoPI> listaEntity = null;

            DAGestioneFondo.GetListaDatiAgoTeoricoPIByIdPensione(idPensione, out listaEntity);

            if (listaEntity == null || listaEntity.Count == 0)
                return;

            foreach (DatiAgoTeoricoPensioneFondoPI entity in listaEntity)
            {
                DatiAgoTeoricoPI dto = new DatiAgoTeoricoPI();
                Utility.ValorizzaOggetti(entity, dto);
                listaDatiAgoTeoricoPI.Add(dto);
            }
        }

        public static void SalvaDatiAgoTeoricoPI(long idPensioneFondoPI, DatiAgoTeoricoPI datiAgoTeoricoPI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoTeoricoPensioneFondoPI datiAgoEntity = new DatiAgoTeoricoPensioneFondoPI();

                Utility.ValorizzaOggetti(datiAgoTeoricoPI, datiAgoEntity);

                datiAgoEntity.IdPensioneFondoPI = idPensioneFondoPI;

                DAGestioneFondo.SalvaDatiAgoTeoricoPI(datiAgoEntity);

                transactionScope.Complete();
            }
        }

        public static void SalvaDatiAgoTeoricoPIRecordFondo(long? idFondo, DatiAgoTeoricoPI datiAgoTeoricoPI)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoTeoricoPensioneFondoPI datiAgoEntity = new DatiAgoTeoricoPensioneFondoPI();
                Utility.ValorizzaOggetti(datiAgoTeoricoPI, datiAgoEntity);
                datiAgoEntity.IdFondo = idFondo;
                DAGestioneFondo.SalvaDatiAgoTeoricoPIRecordFondo(datiAgoEntity);
                transactionScope.Complete();
            }
        }

        public static void EliminaDatiAgoTeoricoPI(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoTeoricoPIByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion Dati AGO TEORICO PI


        #region FondoGAS

        public static void GetFondoGASByIdPensione(Int64 idPensione, out DatiFondoGAS datiFondoGAS)
        {
            PensioneFondoGA fondoGAS = null;
            datiFondoGAS = null;
            DAGestioneFondo.GetFondoGASByIdPensione(idPensione, out fondoGAS);
            if (fondoGAS == null)
                return;
            datiFondoGAS = new DatiFondoGAS();
            Utility.ValorizzaOggetti(fondoGAS, datiFondoGAS);
        }

        public static void SalvaFondoGAS(long idFondo, DatiFondoGAS datiFondoGAS)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoGA fondoGAS = new PensioneFondoGA();
                Utility.ValorizzaOggetti(datiFondoGAS, fondoGAS);
                fondoGAS.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoGAS(fondoGAS);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoGAS(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoGASByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoGAS

        #region FondoCL

        public static void GetFondoCLByIdPensione(Int64 idPensione, out DatiFondoCL datiFondoCL)
        {
            PensioneFondoCL fondoCL = null;
            datiFondoCL = null;
            DAGestioneFondo.GetFondoCLByIdPensione(idPensione, out fondoCL);
            if (fondoCL == null)
                return;
            datiFondoCL = new DatiFondoCL();
            Utility.ValorizzaOggetti(fondoCL, datiFondoCL);
        }

        public static void SalvaFondoCL(long idFondo, DatiFondoCL datiFondoCL)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoCL fondoCL = new PensioneFondoCL();
                Utility.ValorizzaOggetti(datiFondoCL, fondoCL);
                fondoCL.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoCL(fondoCL);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoCL(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoCLByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoCL

        #region FondoDZ

        public static void GetFondoDZByIdPensione(Int64 idPensione, out DatiFondoDZ datiFondoDZ)
        {
            PensioneFondoDZ fondoDZ = null;
            datiFondoDZ = null;
            DAGestioneFondo.GetFondoDZByIdPensione(idPensione, out fondoDZ);
            if (fondoDZ == null)
                return;
            datiFondoDZ = new DatiFondoDZ();
            Utility.ValorizzaOggetti(fondoDZ, datiFondoDZ);
        }

        public static void GetFondoDZRecordFondoByIdPensione(Int64 idPensione, out List<DatiFondoDZ> listaDatiFondoDZ)
        {
            List<PensioneFondoDZ> listaFondoDZ = null;
            listaDatiFondoDZ = null;
            DAGestioneFondo.GetFondoDZRecordFondoByIdPensione(idPensione, out listaFondoDZ);
            if (listaFondoDZ == null || listaFondoDZ.Count == 0)
                return;
            listaDatiFondoDZ = new List<DatiFondoDZ>();
            foreach (PensioneFondoDZ fondoDZ in listaFondoDZ)
            {
                DatiFondoDZ datiFondoDZ = new DatiFondoDZ();
                Utility.ValorizzaOggetti(fondoDZ, datiFondoDZ);
                listaDatiFondoDZ.Add(datiFondoDZ);
            }
        }

        public static void GetFondoDZByIdRecordFondo(Int64 idRecordFondo, out DatiFondoDZ datiFondoDZ)
        {
            PensioneFondoDZ fondoDZ = null;
            datiFondoDZ = null;
            DAGestioneFondo.GetFondoDZByIdRecordFondo(idRecordFondo, out fondoDZ);
            if (fondoDZ == null)
                return;
            datiFondoDZ = new DatiFondoDZ();
            Utility.ValorizzaOggetti(fondoDZ, datiFondoDZ);
        }

        public static void SalvaFondoDZ(long idFondo, DatiFondoDZ datiFondoDZ)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoDZ fondoDZ = new PensioneFondoDZ();
                Utility.ValorizzaOggetti(datiFondoDZ, fondoDZ);
                fondoDZ.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoDZ(fondoDZ);
                transactionScope.Complete();
            }
        }

        public static void SalvaFondoDZRecordFondo(long idFondo, long idRecordFondo, DatiFondoDZ datiFondoDZ)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoDZ fondoDZ = new PensioneFondoDZ();
                Utility.ValorizzaOggetti(datiFondoDZ, fondoDZ);
                fondoDZ.IdFondo = idFondo;
                fondoDZ.IdRecordFondo = idRecordFondo;
                fondoDZ.Sospensione = datiFondoDZ.Sospensione;
                fondoDZ.PensioneBaseAnnua = datiFondoDZ.PensioneBaseAnnua;
                DAGestioneFondo.SalvaFondoDZRecordFondo(fondoDZ);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoDZ(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoDZByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoDZByIdRecordFondo(long idRecordFondo)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoDZByIdRecordFondo(idRecordFondo);
                transactionScope.Complete();
            }
        }

        #endregion FondoDZ

        #region FondoES
        public static void GetFondoESByIdPensione(Int64 idPensione, out DatiFondoES datiFondoES)
        {
            PensioneFondoES fondoES = null;
            datiFondoES = null;
            DAGestioneFondo.GetFondoESByIdPensione(idPensione, out fondoES);
            if (fondoES == null)
                return;
            datiFondoES = new DatiFondoES();
            Utility.ValorizzaOggetti(fondoES, datiFondoES);
        }

        public static void SalvaFondoES(long idFondo, DatiFondoES datiFondoES)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoES fondoES = new PensioneFondoES();
                Utility.ValorizzaOggetti(datiFondoES, fondoES);
                fondoES.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoES(fondoES);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoES(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaFondoESByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        #endregion FondoES

        #region FondoPM
        public static void GetFondoPMByIdPensione(Int64 idPensione, out DatiFondoPM datiFondoPM)
        {
            PensioneFondoPM fondoPM = null;
            datiFondoPM = null;
            DAGestioneFondo.GetFondoPMByIdPensione(idPensione, out fondoPM);
            if (fondoPM == null)
                return;
            datiFondoPM = new DatiFondoPM();
            Utility.ValorizzaOggetti(fondoPM, datiFondoPM);
        }

        public static void GetFondoPMRecordFondoByIdPensione(Int64 idPensione, out List<DatiFondoPM> listaDatiFondoPM)
        {
            List<PensioneFondoPM> listaFondoPM = null;
            listaDatiFondoPM = null;
            DAGestioneFondo.GetFondoPMRecordFondoByIdPensione(idPensione, out listaFondoPM);
            if (listaFondoPM == null || listaFondoPM.Count == 0)
                return;
            listaDatiFondoPM = new List<DatiFondoPM>();
            foreach (PensioneFondoPM fondoPM in listaFondoPM)
            {
                DatiFondoPM datiFondoPM = new DatiFondoPM();
                Utility.ValorizzaOggetti(fondoPM, datiFondoPM);
                listaDatiFondoPM.Add(datiFondoPM);
            }
        }

        public static void SalvaFondoPM(long idFondo, DatiFondoPM datiFondoPM)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoPM fondoPM = new PensioneFondoPM();
                Utility.ValorizzaOggetti(datiFondoPM, fondoPM);
                fondoPM.IdFondo = idFondo;
                DAGestioneFondo.SalvaFondoPM(fondoPM);
                transactionScope.Complete();
            }
        }

        public static void SalvaFondoPMRecordFondo(long idFondo, long idRecordFondo, DatiFondoPM datiFondoPM)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                PensioneFondoPM fondoPM = new PensioneFondoPM();
                Utility.ValorizzaOggetti(datiFondoPM, fondoPM);
                fondoPM.IdFondo = idFondo;
                fondoPM.IdRecordFondo = idRecordFondo;
                DAGestioneFondo.SalvaFondoPMRecordFondo(fondoPM);
                transactionScope.Complete();
            }
        }

        public static void EliminaFondoPM(long idPensione)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoPMByIdPensione(idPensione);
                DAGestioneFondo.EliminaFondoPMByIdPensione(idPensione);
                transactionScope.Complete();
            }

        }
        #endregion FondoPM

        #region Dati AGO PM

        public static void EliminaDatiAgoPM(long idPensione)
        {
            using (TransactionScope transactionScope =
                TransactionScopeFactory.Create(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneFondo.EliminaDatiAgoPMByIdPensione(idPensione);
                transactionScope.Complete();
            }
        }

        public static void GetListaDatiAgoPMByIdPensione(long idPensione, out List<DatiAgoPM> listaDatiAgoPM)
        {
            listaDatiAgoPM = new List<DatiAgoPM>();

            List<DatiAgoPensioneFondoPM> listaEntity = null;

            DAGestioneFondo.GetListaDatiAgoPMByIdPensione(idPensione, out listaEntity);

            if (listaEntity == null || listaEntity.Count == 0)
                return;

            foreach (DatiAgoPensioneFondoPM entity in listaEntity)
            {
                DatiAgoPM dto = new DatiAgoPM();
                Utility.ValorizzaOggetti(entity, dto);
                listaDatiAgoPM.Add(dto);
            }
        }

        public static void InsertOrUpdateDatiAgoPM(long? idDatiAgoFondoPM, DatiAgoPM datiAgoPM)
        {
            using (TransactionScope transactionScope =
                TransactionScopeFactory.Create(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoPensioneFondoPM datiAgoEntity = new DatiAgoPensioneFondoPM();

                Utility.ValorizzaOggetti(datiAgoPM, datiAgoEntity);
                datiAgoEntity.Id = idDatiAgoFondoPM != null ? (long)idDatiAgoFondoPM : 0;

                DAGestioneFondo.SalvaDatiAgoPMRecordFondo(datiAgoEntity);

                transactionScope.Complete();
            }
        }

        public static void SalvaDatiAgoPMRecordFondo(long idFondo, DatiAgoPM datiAgoPM)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DatiAgoPensioneFondoPM datiAgoEntity = new DatiAgoPensioneFondoPM();
                Utility.ValorizzaOggetti(datiAgoPM, datiAgoEntity);
                datiAgoEntity.IdFondo = idFondo;
                DAGestioneFondo.SalvaDatiAgoPMRecordFondo(datiAgoEntity);
                transactionScope.Complete();
            }
        }

        #endregion Dati Ago PM 
        #region nested class
        public class DatiFondo
        {
            public DatiFondo()
            { }
            public DatiFondo(long idFondo, string aliquotaIrpef, string capitalizzazioneNetta, string tipoRecord,
                System.Nullable<System.DateTime> dataEliminazione, System.Nullable<System.DateTime> dataUltimaRicostituzione,
                System.Nullable<System.DateTime> dataRipristinoPagamento, System.Nullable<short> codiceCategoriaPensioneSospesa,
                System.Nullable<short> codiceSedePensioneSospesa, string nCertificatoPensioneSospesa,
                string codicePensionePrecedente, System.Nullable<byte> codiceCristallizzazione,
                char? tipoPensione, string attivitaSvolta,
                System.Nullable<System.DateTime> decorrenza, System.Nullable<System.DateTime> decorrenzaValiditaDati,
                System.Nullable<System.DateTime> dataSospensione, System.Nullable<int> servizioUtileAAMM,
                System.Nullable<int> servizioUtileGG, System.Nullable<decimal> retribuzionePensionabile,
                string codiceNatura, System.Nullable<byte> codiceDirittoQuoteFisse,
                System.Nullable<decimal> retribuzionePensioneExCombattente,
                System.Nullable<bool> attribuzioneBonus, System.Nullable<System.DateTime> inizioBonus,
                System.Nullable<System.DateTime> fineBonus, byte? codiceSpecifico,
                System.Nullable<char> codiceRequisiti1, System.Nullable<char> codiceRequisiti2, bool? chkDL407, bool? articolo2, bool? privilegiate,
                bool riduzioneRetributiva, decimal? riduzioneRetributivaPercentuale, short? QuotaA707, short? quotaA2707, short? QuotaB707, short? QuotaC707,
                short? quotaC2707, short? QuotaD707, byte? quotaA707AA, byte? quotaA707MM, byte? quotaA707GG, byte? quotaB707AA, byte? quotaB707MM, byte? quotaB707GG,
                byte? quotaC707AA, byte? quotaC707MM, byte? quotaC707GG, decimal? RetribuzionePonderataAGO707, decimal? retrPondAnnuaAGOLimite,
                short? QuotaAES707, short? QuotaBES707, int? settimaneUtiliDiritto, bool? bypassDinamicoCodiceSpecifico, int? settimaneUtiliDirittoOI)
            {
                this._Id = idFondo;

                this._AliquotaIrpef = aliquotaIrpef;

                this._CapitalizzazioneNetta = capitalizzazioneNetta;

                this._TipoRecord = tipoRecord;

                this._DataEliminazione = dataEliminazione;

                this._DataUltimaRicostituzione = dataUltimaRicostituzione;

                this._DataRipristinoPagamento = dataRipristinoPagamento;

                this._CodiceCategoriaPensioneSospesa = codiceCategoriaPensioneSospesa;

                this._CodiceSedePensioneSospesa = codiceSedePensioneSospesa;

                this._NCertificatoPensioneSospesa = nCertificatoPensioneSospesa;

                this._CodicePensionePrecedente = codicePensionePrecedente;

                this._CodiceCristallizzazione = codiceCristallizzazione;

                this._TipoPensione = tipoPensione;

                this._AttivitaSvolta = attivitaSvolta;

                this._Decorrenza = decorrenza;

                this._DecorrenzaValiditaDati = decorrenzaValiditaDati;

                this._DataSospensione = dataSospensione;

                this._ServizioUtileAAMM = servizioUtileAAMM;

                this._ServizioUtileGG = servizioUtileGG;

                this._RetribuzionePensionabile = retribuzionePensionabile;

                this._CodiceNatura = codiceNatura;

                this._CodiceDirittoQuoteFisse = codiceDirittoQuoteFisse;

                this._RetribuzionePensioneExCombattente = retribuzionePensioneExCombattente;

                this._AttribuzioneBonus = attribuzioneBonus;

                this._InizioBonus = inizioBonus;

                this._FineBonus = fineBonus;

                this._CodiceSpecifico = codiceSpecifico;

                this._CodiceRequisiti1 = codiceRequisiti1;

                this._CodiceRequisiti2 = codiceRequisiti2;

                this._ChkDL407 = chkDL407;

                this._Articolo2 = articolo2;

                this._Privilegiate = privilegiate;

                this._RiduzioneRetributiva = riduzioneRetributiva;

                this._RiduzioneRetributivaPercentuale = riduzioneRetributivaPercentuale;

                this._QuotaA707 = QuotaA707;

                this._QuotaA2707 = quotaA2707;

                this._QuotaB707 = QuotaB707;

                this._QuotaC707 = QuotaC707;

                this._QuotaC2707 = quotaC2707;

                this._QuotaD707 = QuotaD707;

                this._QuotaA707AA = quotaA707AA;

                this._QuotaA707MM = quotaA707MM;

                this._QuotaA707GG = quotaA707GG;

                this._QuotaB707AA = quotaB707AA;

                this._QuotaB707MM = quotaB707MM;

                this._QuotaB707GG = quotaB707GG;

                this._QuotaC707AA = quotaC707AA;

                this._QuotaC707MM = quotaC707MM;

                this._QuotaC707GG = quotaC707GG;

                this._RetribuzionePonderataAGO707 = RetribuzionePonderataAGO707;

                this._RetrPondAnnuaAGOLimite = retrPondAnnuaAGOLimite;

                this._QuotaAES707 = QuotaAES707;

                this._QuotaBES707 = QuotaBES707;

                this._SettimaneUtiliDiritto = settimaneUtiliDiritto;

                this._ByPassDinamicoCodiceSpecifico = bypassDinamicoCodiceSpecifico;

                this._SettimaneUtiliDirittoOI = settimaneUtiliDirittoOI;
            }

            #region private properties
            private long _Id;

            private string _AliquotaIrpef;

            private string _CapitalizzazioneNetta;

            private string _TipoRecord;

            private System.Nullable<System.DateTime> _DataEliminazione;

            private System.Nullable<System.DateTime> _DataUltimaRicostituzione;

            private System.Nullable<System.DateTime> _DataRipristinoPagamento;

            private System.Nullable<short> _CodiceCategoriaPensioneSospesa;

            private System.Nullable<short> _CodiceSedePensioneSospesa;

            private string _NCertificatoPensioneSospesa;

            private string _CodicePensionePrecedente;

            private System.Nullable<byte> _CodiceCristallizzazione;

            private char? _TipoPensione;

            private string _AttivitaSvolta;

            private System.Nullable<System.DateTime> _Decorrenza;

            private System.Nullable<System.DateTime> _DecorrenzaValiditaDati;

            private System.Nullable<System.DateTime> _DataSospensione;

            private System.Nullable<int> _ServizioUtileAAMM;

            private System.Nullable<int> _ServizioUtileGG;

            private System.Nullable<decimal> _RetribuzionePensionabile;

            private string _CodiceNatura;

            private System.Nullable<byte> _CodiceDirittoQuoteFisse;

            private System.Nullable<decimal> _RetribuzionePensioneExCombattente;

            private System.Nullable<bool> _AttribuzioneBonus;

            private System.Nullable<System.DateTime> _InizioBonus;

            private System.Nullable<System.DateTime> _FineBonus;

            private byte? _CodiceSpecifico;

            private System.Nullable<char> _CodiceRequisiti1;

            private System.Nullable<char> _CodiceRequisiti2;

            private System.Nullable<bool> _ChkDL407;

            private System.Nullable<bool> _Articolo2;

            private System.Nullable<bool> _Privilegiate;

            private bool _RiduzioneRetributiva;

            private System.Nullable<decimal> _RiduzioneRetributivaPercentuale;

            private System.Nullable<short> _QuotaA707;

            private short? _QuotaA2707;

            private System.Nullable<short> _QuotaB707;

            private System.Nullable<short> _QuotaC707;

            private short? _QuotaC2707;

            private System.Nullable<short> _QuotaD707;

            private byte? _QuotaA707AA;

            private byte? _QuotaA707MM;

            private byte? _QuotaA707GG;

            private byte? _QuotaB707AA;

            private byte? _QuotaB707MM;

            private byte? _QuotaB707GG;

            private byte? _QuotaC707AA;

            private byte? _QuotaC707MM;

            private byte? _QuotaC707GG;

            private System.Nullable<decimal> _RetribuzionePonderataAGO707;

            private System.Nullable<decimal> _RetrPondAnnuaAGOLimite;

            private System.Nullable<short> _QuotaAES707;

            private System.Nullable<short> _QuotaBES707;

            private int? _SettimaneUtiliDiritto;
            private long? _PersonaleViaggiante;
            private string _CodiceSpecificoGP;
            private System.Nullable<char> _CodiceSpecificoTraduzione;

            private System.Nullable<bool> _ByPassDinamicoCodiceSpecifico;
            private int? _SettimaneUtiliDirittoOI;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }

            public string AliquotaIrpef { get { return _AliquotaIrpef; } set { _AliquotaIrpef = value; } }

            public string CapitalizzazioneNetta { get { return _CapitalizzazioneNetta; } set { _CapitalizzazioneNetta = value; } }

            public string TipoRecord { get { return _TipoRecord; } set { _TipoRecord = value; } }

            public System.Nullable<System.DateTime> DataEliminazione { get { return _DataEliminazione; } set { _DataEliminazione = value; } }

            public System.Nullable<System.DateTime> DataUltimaRicostituzione { get { return _DataUltimaRicostituzione; } set { _DataUltimaRicostituzione = value; } }

            public System.Nullable<System.DateTime> DataRipristinoPagamento { get { return _DataRipristinoPagamento; } set { _DataRipristinoPagamento = value; } }

            public System.Nullable<short> CodiceCategoriaPensioneSospesa { get { return _CodiceCategoriaPensioneSospesa; } set { _CodiceCategoriaPensioneSospesa = value; } }

            public System.Nullable<short> CodiceSedePensioneSospesa { get { return _CodiceSedePensioneSospesa; } set { _CodiceSedePensioneSospesa = value; } }

            public string NCertificatoPensioneSospesa { get { return _NCertificatoPensioneSospesa; } set { _NCertificatoPensioneSospesa = value; } }

            public string CodicePensionePrecedente { get { return _CodicePensionePrecedente; } set { _CodicePensionePrecedente = value; } }

            public System.Nullable<byte> CodiceCristallizzazione { get { return _CodiceCristallizzazione; } set { _CodiceCristallizzazione = value; } }

            public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }

            public string AttivitaSvolta { get { return _AttivitaSvolta; } set { _AttivitaSvolta = value; } }

            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }

            public System.Nullable<System.DateTime> DecorrenzaValiditaDati { get { return _DecorrenzaValiditaDati; } set { _DecorrenzaValiditaDati = value; } }

            public System.Nullable<System.DateTime> DataSospensione { get { return _DataSospensione; } set { _DataSospensione = value; } }

            public System.Nullable<int> ServizioUtileAAMM { get { return _ServizioUtileAAMM; } set { _ServizioUtileAAMM = value; } }

            public System.Nullable<int> ServizioUtileGG { get { return _ServizioUtileGG; } set { _ServizioUtileGG = value; } }

            public System.Nullable<decimal> RetribuzionePensionabile { get { return _RetribuzionePensionabile; } set { _RetribuzionePensionabile = value; } }

            public string CodiceNatura { get { return _CodiceNatura; } set { _CodiceNatura = value; } }

            public System.Nullable<byte> CodiceDirittoQuoteFisse { get { return _CodiceDirittoQuoteFisse; } set { _CodiceDirittoQuoteFisse = value; } }

            public System.Nullable<decimal> RetribuzionePensioneExCombattente { get { return _RetribuzionePensioneExCombattente; } set { _RetribuzionePensioneExCombattente = value; } }

            public System.Nullable<bool> AttribuzioneBonus { get { return _AttribuzioneBonus; } set { _AttribuzioneBonus = value; } }

            public System.Nullable<System.DateTime> InizioBonus { get { return _InizioBonus; } set { _InizioBonus = value; } }

            public System.Nullable<System.DateTime> FineBonus { get { return _FineBonus; } set { _FineBonus = value; } }

            public byte? CodiceSpecifico { get { return _CodiceSpecifico; } set { _CodiceSpecifico = value; } }

            public System.Nullable<char> CodiceRequisiti1 { get { return _CodiceRequisiti1; } set { _CodiceRequisiti1 = value; } }

            public System.Nullable<char> CodiceRequisiti2 { get { return _CodiceRequisiti2; } set { _CodiceRequisiti2 = value; } }

            public System.Nullable<bool> ChkDL407 { get { return _ChkDL407; } set { _ChkDL407 = value; } }

            public System.Nullable<bool> Articolo2 { get { return _Articolo2; } set { _Articolo2 = value; } }

            public System.Nullable<bool> Privilegiate { get { return _Privilegiate; } set { _Privilegiate = value; } }

            public bool RiduzioneRetributiva { get { return _RiduzioneRetributiva; } set { _RiduzioneRetributiva = value; } }

            public System.Nullable<decimal> RiduzioneRetributivaPercentuale { get { return _RiduzioneRetributivaPercentuale; } set { _RiduzioneRetributivaPercentuale = value; } }

            public System.Nullable<short> QuotaA707 { get { return _QuotaA707; } set { _QuotaA707 = value; } }

            public short? QuotaA2707 { get { return _QuotaA2707; } set { _QuotaA2707 = value; } }

            public System.Nullable<short> QuotaB707 { get { return _QuotaB707; } set { _QuotaB707 = value; } }

            public System.Nullable<short> QuotaC707 { get { return _QuotaC707; } set { _QuotaC707 = value; } }

            public short? QuotaC2707 { get { return _QuotaC2707; } set { _QuotaC2707 = value; } }

            public System.Nullable<short> QuotaD707 { get { return _QuotaD707; } set { _QuotaD707 = value; } }

            public byte? QuotaA707AA { get { return _QuotaA707AA; } set { _QuotaA707AA = value; } }

            public byte? QuotaA707MM { get { return _QuotaA707MM; } set { _QuotaA707MM = value; } }

            public byte? QuotaA707GG { get { return _QuotaA707GG; } set { _QuotaA707GG = value; } }

            public byte? QuotaB707AA { get { return _QuotaB707AA; } set { _QuotaB707AA = value; } }

            public byte? QuotaB707MM { get { return _QuotaB707MM; } set { _QuotaB707MM = value; } }

            public byte? QuotaB707GG { get { return _QuotaB707GG; } set { _QuotaB707GG = value; } }

            public byte? QuotaC707AA { get { return _QuotaC707AA; } set { _QuotaC707AA = value; } }

            public byte? QuotaC707MM { get { return _QuotaC707MM; } set { _QuotaC707MM = value; } }

            public byte? QuotaC707GG { get { return _QuotaC707GG; } set { _QuotaC707GG = value; } }

            public System.Nullable<decimal> RetribuzionePonderataAGO707 { get { return _RetribuzionePonderataAGO707; } set { _RetribuzionePonderataAGO707 = value; } }

            public System.Nullable<decimal> RetrPondAnnuaAGOLimite { get { return _RetrPondAnnuaAGOLimite; } set { _RetrPondAnnuaAGOLimite = value; } }

            public System.Nullable<short> QuotaAES707 { get { return _QuotaAES707; } set { _QuotaAES707 = value; } }

            public System.Nullable<short> QuotaBES707 { get { return _QuotaBES707; } set { _QuotaBES707 = value; } }

            public int? SettimaneUtiliDiritto { get { return _SettimaneUtiliDiritto; } set { _SettimaneUtiliDiritto = value; } }
            public long? PersonaleViaggiante { get { return _PersonaleViaggiante; } set { _PersonaleViaggiante = value; } }

            public string CodiceSpecificoGP { get { return _CodiceSpecificoGP; } set { _CodiceSpecificoGP = value; } }

            public System.Nullable<char> CodiceSpecificoTraduzione { get { return _CodiceSpecificoTraduzione; } set { _CodiceSpecificoTraduzione = value; } }

            public System.Nullable<bool> BypassDinamicoCodiceSpecifico { get { return _ByPassDinamicoCodiceSpecifico; } set { _ByPassDinamicoCodiceSpecifico = value; } }

            public int? SettimaneUtiliDirittoOI { get { return _SettimaneUtiliDirittoOI; } set { _SettimaneUtiliDirittoOI = value; } }
            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiFondo fondo = (DatiFondo)obj;
                try
                {
                    if ((this._AliquotaIrpef != null ? this._AliquotaIrpef.Trim() : null) != (fondo._AliquotaIrpef != null ? fondo._AliquotaIrpef.Trim() : null) ||
                        (this._CapitalizzazioneNetta != null ? this._CapitalizzazioneNetta.Trim() : null) != (fondo._CapitalizzazioneNetta != null ? fondo._CapitalizzazioneNetta.Trim() : null) ||
                        (this._TipoRecord != null ? this._TipoRecord.Trim() : null) != (fondo._TipoRecord != null ? fondo._TipoRecord.Trim() : null) ||
                        this._DataEliminazione != fondo._DataEliminazione ||
                        this._DataUltimaRicostituzione != fondo._DataUltimaRicostituzione ||
                        this._DataRipristinoPagamento != fondo._DataRipristinoPagamento ||
                        this._CodiceCategoriaPensioneSospesa != fondo._CodiceCategoriaPensioneSospesa ||
                        this._CodiceSedePensioneSospesa != fondo._CodiceSedePensioneSospesa ||
                        (this._NCertificatoPensioneSospesa != null ? this._NCertificatoPensioneSospesa.Trim() : null) != (fondo._NCertificatoPensioneSospesa != null ? fondo._NCertificatoPensioneSospesa.Trim() : null) ||
                        (this._CodicePensionePrecedente != null ? this._CodicePensionePrecedente.Trim() : null) != (fondo._CodicePensionePrecedente != null ? fondo._CodicePensionePrecedente.Trim() : null) ||
                        this._CodiceCristallizzazione != fondo._CodiceCristallizzazione ||
                        this._TipoPensione != fondo._TipoPensione ||
                        (this._AttivitaSvolta != null ? this._AttivitaSvolta.Trim() : null) != (fondo._AttivitaSvolta != null ? fondo._AttivitaSvolta.Trim() : null) ||
                        this._Decorrenza != fondo._Decorrenza ||
                        this._DecorrenzaValiditaDati != fondo._DecorrenzaValiditaDati ||
                        this._DataSospensione != fondo._DataSospensione ||
                        this._ServizioUtileAAMM != fondo._ServizioUtileAAMM ||
                        this._ServizioUtileGG != fondo._ServizioUtileGG ||
                        this._RetribuzionePensionabile != fondo._RetribuzionePensionabile ||
                        (this._CodiceNatura != null ? this._CodiceNatura.Trim() : null) != (fondo._CodiceNatura != null ? fondo._CodiceNatura.Trim() : null) ||
                        this._CodiceDirittoQuoteFisse != fondo._CodiceDirittoQuoteFisse ||
                        this._RetribuzionePensioneExCombattente != fondo._RetribuzionePensioneExCombattente ||
                        this._AttribuzioneBonus != fondo._AttribuzioneBonus ||
                        this._InizioBonus != fondo._InizioBonus ||
                        this._FineBonus != fondo._FineBonus ||
                        this._CodiceSpecifico != fondo._CodiceSpecifico ||
                        this._CodiceRequisiti1 != fondo._CodiceRequisiti1 ||
                        this._CodiceRequisiti2 != fondo._CodiceRequisiti2 ||
                        this._ChkDL407 != fondo._ChkDL407 ||
                        this._Articolo2 != fondo._Articolo2 ||
                        this._Privilegiate != fondo._Privilegiate ||
                        this._RiduzioneRetributiva != fondo._RiduzioneRetributiva ||
                        this._RiduzioneRetributivaPercentuale != fondo._RiduzioneRetributivaPercentuale ||
                        this._QuotaA707 != fondo._QuotaA707 ||
                        this._QuotaA2707 != fondo._QuotaA2707 ||
                        this._QuotaB707 != fondo._QuotaB707 ||
                        this._QuotaC707 != fondo._QuotaC707 ||
                        this._QuotaC2707 != fondo._QuotaC2707 ||
                        this._QuotaD707 != fondo._QuotaD707 ||
                        this._QuotaA707AA != fondo._QuotaA707AA ||
                        this._QuotaA707MM != fondo._QuotaA707MM ||
                        this._QuotaA707GG != fondo._QuotaA707GG ||
                        this._QuotaB707AA != fondo._QuotaB707AA ||
                        this._QuotaB707MM != fondo._QuotaB707MM ||
                        this._QuotaB707GG != fondo._QuotaB707GG ||
                        this._QuotaC707AA != fondo._QuotaC707AA ||
                        this._QuotaC707MM != fondo._QuotaC707MM ||
                        this._QuotaC707GG != fondo._QuotaC707GG ||
                        this._RetribuzionePonderataAGO707 != fondo.RetribuzionePonderataAGO707 ||
                        this._RetrPondAnnuaAGOLimite != fondo.RetrPondAnnuaAGOLimite ||
                        this._QuotaAES707 != fondo._QuotaAES707 ||
                        this._QuotaBES707 != fondo._QuotaBES707 ||
                        this._SettimaneUtiliDiritto != fondo._SettimaneUtiliDiritto ||
                        this._SettimaneUtiliDirittoOI != fondo._SettimaneUtiliDirittoOI
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._AliquotaIrpef != null ? this._AliquotaIrpef.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CapitalizzazioneNetta != null ? this._CapitalizzazioneNetta.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._TipoRecord != null ? this._TipoRecord.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataEliminazione != null ? this._DataEliminazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataUltimaRicostituzione != null ? this._DataUltimaRicostituzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataRipristinoPagamento != null ? this._DataRipristinoPagamento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceCategoriaPensioneSospesa != null ? this._CodiceCategoriaPensioneSospesa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceSedePensioneSospesa != null ? this._CodiceSedePensioneSospesa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NCertificatoPensioneSospesa != null ? this._NCertificatoPensioneSospesa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodicePensionePrecedente != null ? this._CodicePensionePrecedente.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceCristallizzazione != null ? this._CodiceCristallizzazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._TipoPensione != null ? this._TipoPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AttivitaSvolta != null ? this._AttivitaSvolta.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Decorrenza != null ? this._Decorrenza.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaValiditaDati != null ? this._DecorrenzaValiditaDati.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataSospensione != null ? this._DataSospensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ServizioUtileAAMM != null ? this._ServizioUtileAAMM.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ServizioUtileGG != null ? this._ServizioUtileGG.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetribuzionePensionabile != null ? this._RetribuzionePensionabile.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceNatura != null ? this._CodiceNatura.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceDirittoQuoteFisse != null ? this._CodiceDirittoQuoteFisse.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetribuzionePensioneExCombattente != null ? this._RetribuzionePensioneExCombattente.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AttribuzioneBonus != null ? this._AttribuzioneBonus.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._InizioBonus != null ? this._InizioBonus.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._FineBonus != null ? this._FineBonus.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceSpecifico != null ? this._CodiceSpecifico.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceRequisiti1 != null ? this._CodiceRequisiti1.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceRequisiti2 != null ? this._CodiceRequisiti2.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ChkDL407 != null ? this._ChkDL407.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Articolo2 != null ? this._Articolo2.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Privilegiate != null ? this._Privilegiate.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RiduzioneRetributiva.GetHashCode());
            //    hash = (hash * 7) + (this._RiduzioneRetributivaPercentuale != null ? this._RiduzioneRetributivaPercentuale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaA707 != null ? this._QuotaA707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaA2707 != null ? this._QuotaA2707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaB707 != null ? this._QuotaB707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaC707 != null ? this._QuotaC707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaC2707 != null ? this._QuotaC2707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaD707 != null ? this._QuotaD707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaA707AA != null ? this._QuotaA707AA.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaA707MM != null ? this._QuotaA707MM.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaA707GG != null ? this._QuotaA707GG.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaB707AA != null ? this._QuotaB707AA.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaB707MM != null ? this._QuotaB707MM.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaB707GG != null ? this._QuotaB707GG.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaC707AA != null ? this._QuotaC707AA.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaC707MM != null ? this._QuotaC707MM.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._QuotaC707GG != null ? this._QuotaC707GG.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetribuzionePonderataAGO707 != null ? this._RetribuzionePonderataAGO707.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetrPondAnnuaAGOLimite != null ? this._RetrPondAnnuaAGOLimite.GetHashCode() : 0);
            //    return hash;
            //}

            public bool IsFondoNull()
            {
                if (String.IsNullOrEmpty(this._AliquotaIrpef) && String.IsNullOrEmpty(this._AttivitaSvolta) && !this._AttribuzioneBonus.HasValue &&
                    String.IsNullOrEmpty(this._CapitalizzazioneNetta) && this._CodiceCategoriaPensioneSospesa.HasValue &&
                    !this._CodiceCristallizzazione.HasValue && !this._CodiceDirittoQuoteFisse.HasValue && String.IsNullOrEmpty(this._CodiceNatura) &&
                    String.IsNullOrEmpty(this._CodicePensionePrecedente) && !this._CodiceRequisiti1.HasValue && !this._CodiceRequisiti2.HasValue &&
                    !this._CodiceSedePensioneSospesa.HasValue && !this._CodiceSpecifico.HasValue && !this._DataEliminazione.HasValue && !this._DataRipristinoPagamento.HasValue &&
                    !this._DataSospensione.HasValue && !this._DataUltimaRicostituzione.HasValue && !this._Decorrenza.HasValue &&
                    !this._DecorrenzaValiditaDati.HasValue && !this._FineBonus.HasValue && !this._InizioBonus.HasValue && String.IsNullOrEmpty(this._NCertificatoPensioneSospesa) &&
                    !this._RetribuzionePensionabile.HasValue && !this._RetribuzionePensioneExCombattente.HasValue && !this._ServizioUtileAAMM.HasValue &&
                    !this._ServizioUtileGG.HasValue && !this._TipoPensione.HasValue && String.IsNullOrEmpty(this._TipoRecord) &&
                    (!this._ChkDL407.HasValue || !this.ChkDL407.Value) && (!this._Articolo2.HasValue || !this.Articolo2.Value) && (!this._Privilegiate.HasValue || !this.Privilegiate.Value) &&
                    !this._RiduzioneRetributiva && !this._RiduzioneRetributivaPercentuale.HasValue &&
                    !this._QuotaA707.HasValue && !this._QuotaA2707.HasValue && !this._QuotaB707.HasValue && !this._QuotaC707.HasValue && !this._QuotaC2707.HasValue && !this._QuotaD707.HasValue &&
                    !this._QuotaA707AA.HasValue && !this._QuotaA707MM.HasValue && !this._QuotaA707GG.HasValue && !this._QuotaB707AA.HasValue && !this._QuotaB707MM.HasValue && !this._QuotaB707GG.HasValue &&
                    !this._QuotaC707AA.HasValue && !this._QuotaC707MM.HasValue && !this._QuotaC707GG.HasValue &&
                    !this._RetribuzionePonderataAGO707.HasValue && !this._RetrPondAnnuaAGOLimite.HasValue &&
                    !this._QuotaAES707.HasValue && !this._QuotaBES707.HasValue)
                    return true;
                else
                    return false;
            }

            public bool IsDatiComma707Null()
            {
                if (!_QuotaA707.HasValue && !_QuotaA2707.HasValue && !_QuotaB707.HasValue && !_QuotaC707.HasValue && !_QuotaC2707.HasValue && !_QuotaD707.HasValue &&
                    !_QuotaA707AA.HasValue && !_QuotaA707MM.HasValue && !_QuotaA707GG.HasValue && !_QuotaB707AA.HasValue && !_QuotaB707MM.HasValue && !_QuotaB707GG.HasValue &&
                    !_QuotaC707AA.HasValue && !_QuotaC707MM.HasValue && !_QuotaC707GG.HasValue &&
                    !_RetribuzionePonderataAGO707.HasValue && !_QuotaAES707.HasValue && !_QuotaBES707.HasValue)
                    return true;
                return false;
            }

            #endregion public members
        }

        public class DatiFondoET
        {
            public DatiFondoET()
            { }

            public DatiFondoET(long? _CodAzienda, DateTime? _DataEsonero, DateTime? _DecorrenzaTeorica, decimal? _ContributiAgoLegge140830, decimal? _ContributiAgoLegge40245,
                    byte? _CodiceRateo66, decimal? _RetribuzioneEsodo, int? _GGInterruzione, short? _NSettimaneLeva, short? _NSettimaneRichiamato, decimal? _Stipendio, decimal? _Importo13ma,
                    decimal? _Importo14ma, decimal? _ElementiAccessori, decimal? _Competenze40Percento, byte? _GradoInvalidita, decimal? _ImportoRenditaInail,
                    decimal? _RetribuzioneEffettiva, bool? _PartTime, int? _AAInterruzione, int? _MMInterruzione, bool? _CodiceServizioMilitare, bool? _CodiceEsodo, bool? _Requisiti247_243,
                    byte? _NumeroTriSemRequisiti, short? _AnnoRequisiti, int? _AnzianitaAnni, long? _PersonaleViaggiante, short? setAnzTotAltraPensione, decimal? baseAltraPensione,
                    string categoriaAltraPensione, int? certificatoAltraPensione, decimal? rmsImpAltraPensione, DateTime? decorrenzaAltraPensione, short? revAltraPensione, byte? tipoLiquidazione,
                    DateTime? decorrenzaPrimoSupplemento, decimal? impContribPrimoSupplemento, DateTime? decorrenzaSecondoSupplemento, decimal? impContribSecondoSupplemento
                )
            {
                this._CodAzienda = _CodAzienda;
                this._DataEsonero = _DataEsonero;
                this._DecorrenzaTeorica = _DecorrenzaTeorica;
                this._ContributiAgoLegge140830 = _ContributiAgoLegge140830;
                this._ContributiAgoLegge40245 = _ContributiAgoLegge40245;
                this._CodiceRateo66 = _CodiceRateo66;
                this._RetribuzioneEsodo = _RetribuzioneEsodo;
                this._GGInterruzione = _GGInterruzione;
                this._NSettimaneLeva = _NSettimaneLeva;
                this._NSettimaneRichiamato = _NSettimaneRichiamato;
                this._Stipendio = _Stipendio;
                this._Importo13ma = _Importo13ma;
                this._Importo14ma = _Importo14ma;
                this._ElementiAccessori = _ElementiAccessori;
                this._Competenze40Percento = _Competenze40Percento;
                this._GradoInvalidita = _GradoInvalidita;
                this._ImportoRenditaInail = _ImportoRenditaInail;
                this._RetribuzioneEffettiva = _RetribuzioneEffettiva;
                this._PartTime = _PartTime;
                this._AAInterruzione = _AAInterruzione;
                this._MMInterruzione = _MMInterruzione;
                this._CodiceServizioMilitare = _CodiceServizioMilitare;
                this._CodiceEsodo = _CodiceEsodo;
                this._Requisiti247_243 = _Requisiti247_243;
                this._NumeroTriSemRequisiti = _NumeroTriSemRequisiti;
                this._AnnoRequisiti = _AnnoRequisiti;
                this._AnzianitaAnni = _AnzianitaAnni;
                this._PersonaleViaggiante = _PersonaleViaggiante;
                this._SetAnzTotAltraPensione = setAnzTotAltraPensione;
                this._BaseAltraPensione = baseAltraPensione;
                this._CategoriaAltraPensione = categoriaAltraPensione;
                this._CertificatoAltraPensione = certificatoAltraPensione;
                this._RmsImpAltraPensione = rmsImpAltraPensione;
                this._DecorrenzaAltraPensione = decorrenzaAltraPensione;
                this._RevAltraPensione = revAltraPensione;
                this._TipoLiquidazione = tipoLiquidazione;
                this._DecorrenzaPrimoSupplemento = decorrenzaPrimoSupplemento;
                this._ImpContribPrimoSupplemento = impContribPrimoSupplemento;
                this._DecorrenzaSecondoSupplemento = decorrenzaSecondoSupplemento;
                this._ImpContribSecondoSupplemento = impContribSecondoSupplemento;
            }

            #region private properties

            private long _IdFondo;
            private long? _CodAzienda;
            private System.Nullable<System.DateTime> _DataEsonero;
            private System.Nullable<System.DateTime> _DecorrenzaTeorica;
            private System.Nullable<decimal> _ContributiAgoLegge140830;
            private System.Nullable<decimal> _ContributiAgoLegge40245;
            private System.Nullable<byte> _CodiceRateo66;
            private System.Nullable<decimal> _RetribuzioneEsodo;
            private System.Nullable<int> _GGInterruzione;
            private System.Nullable<short> _NSettimaneLeva;
            private System.Nullable<short> _NSettimaneRichiamato;
            private System.Nullable<decimal> _Stipendio;
            private System.Nullable<decimal> _Importo13ma;
            private System.Nullable<decimal> _Importo14ma;
            private System.Nullable<decimal> _ElementiAccessori;
            private System.Nullable<decimal> _Competenze40Percento;
            private System.Nullable<byte> _GradoInvalidita;
            private System.Nullable<decimal> _ImportoRenditaInail;
            private System.Nullable<decimal> _RetribuzioneEffettiva;
            private System.Nullable<bool> _PartTime;
            private System.Nullable<int> _AAInterruzione;
            private System.Nullable<int> _MMInterruzione;
            private System.Nullable<bool> _CodiceServizioMilitare;
            private System.Nullable<bool> _CodiceEsodo;
            private System.Nullable<bool> _Requisiti247_243;
            private System.Nullable<byte> _NumeroTriSemRequisiti;
            private System.Nullable<short> _AnnoRequisiti;
            private System.Nullable<int> _AnzianitaAnni;
            private long? _PersonaleViaggiante;
            private System.Nullable<short> _SetAnzTotAltraPensione;
            private System.Nullable<decimal> _BaseAltraPensione;
            private string _CategoriaAltraPensione;
            private System.Nullable<int> _CertificatoAltraPensione;
            private System.Nullable<decimal> _RmsImpAltraPensione;
            private System.Nullable<System.DateTime> _DecorrenzaAltraPensione;
            private System.Nullable<short> _RevAltraPensione;
            private System.Nullable<byte> _TipoLiquidazione;
            private System.Nullable<System.DateTime> _DecorrenzaPrimoSupplemento;
            private System.Nullable<decimal> _ImpContribPrimoSupplemento;
            private System.Nullable<System.DateTime> _DecorrenzaSecondoSupplemento;
            private System.Nullable<decimal> _ImpContribSecondoSupplemento;

            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public long? CodAzienda { get { return _CodAzienda; } set { _CodAzienda = value; } }
            public System.Nullable<System.DateTime> DataEsonero { get { return _DataEsonero; } set { _DataEsonero = value; } }
            public System.Nullable<System.DateTime> DecorrenzaTeorica { get { return _DecorrenzaTeorica; } set { _DecorrenzaTeorica = value; } }
            public System.Nullable<decimal> ContributiAgoLegge140830 { get { return _ContributiAgoLegge140830; } set { _ContributiAgoLegge140830 = value; } }
            public System.Nullable<decimal> ContributiAgoLegge40245 { get { return _ContributiAgoLegge40245; } set { _ContributiAgoLegge40245 = value; } }
            public System.Nullable<byte> CodiceRateo66 { get { return _CodiceRateo66; } set { _CodiceRateo66 = value; } }
            public System.Nullable<decimal> RetribuzioneEsodo { get { return _RetribuzioneEsodo; } set { _RetribuzioneEsodo = value; } }
            public System.Nullable<int> GGInterruzione { get { return _GGInterruzione; } set { _GGInterruzione = value; } }
            public System.Nullable<short> NSettimaneLeva { get { return _NSettimaneLeva; } set { _NSettimaneLeva = value; } }
            public System.Nullable<short> NSettimaneRichiamato { get { return _NSettimaneRichiamato; } set { _NSettimaneRichiamato = value; } }
            public System.Nullable<decimal> Stipendio { get { return _Stipendio; } set { _Stipendio = value; } }
            public System.Nullable<decimal> Importo13ma { get { return _Importo13ma; } set { _Importo13ma = value; } }
            public System.Nullable<decimal> Importo14ma { get { return _Importo14ma; } set { _Importo14ma = value; } }
            public System.Nullable<decimal> ElementiAccessori { get { return _ElementiAccessori; } set { _ElementiAccessori = value; } }
            public System.Nullable<decimal> Competenze40Percento { get { return _Competenze40Percento; } set { _Competenze40Percento = value; } }
            public System.Nullable<byte> GradoInvalidita { get { return _GradoInvalidita; } set { _GradoInvalidita = value; } }
            public System.Nullable<decimal> ImportoRenditaInail { get { return _ImportoRenditaInail; } set { _ImportoRenditaInail = value; } }
            public System.Nullable<decimal> RetribuzioneEffettiva { get { return _RetribuzioneEffettiva; } set { _RetribuzioneEffettiva = value; } }
            public System.Nullable<bool> PartTime { get { return _PartTime; } set { _PartTime = value; } }
            public System.Nullable<int> AAInterruzione { get { return _AAInterruzione; } set { _AAInterruzione = value; } }
            public System.Nullable<int> MMInterruzione { get { return _MMInterruzione; } set { _MMInterruzione = value; } }
            public System.Nullable<bool> CodiceServizioMilitare { get { return _CodiceServizioMilitare; } set { _CodiceServizioMilitare = value; } }
            public System.Nullable<bool> CodiceEsodo { get { return _CodiceEsodo; } set { _CodiceEsodo = value; } }
            public bool? Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }
            public byte? NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public System.Nullable<int> AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }
            public long? PersonaleViaggiante { get { return _PersonaleViaggiante; } set { _PersonaleViaggiante = value; } }
            public System.Nullable<short> SetAnzTotAltraPensione { get { return _SetAnzTotAltraPensione; } set { _SetAnzTotAltraPensione = value; } }
            public System.Nullable<decimal> BaseAltraPensione { get { return _BaseAltraPensione; } set { _BaseAltraPensione = value; } }
            public string CategoriaAltraPensione { get { return _CategoriaAltraPensione; } set { _CategoriaAltraPensione = value; } }
            public System.Nullable<int> CertificatoAltraPensione { get { return _CertificatoAltraPensione; } set { _CertificatoAltraPensione = value; } }
            public System.Nullable<decimal> RmsImpAltraPensione { get { return _RmsImpAltraPensione; } set { _RmsImpAltraPensione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaAltraPensione { get { return _DecorrenzaAltraPensione; } set { _DecorrenzaAltraPensione = value; } }
            public System.Nullable<short> RevAltraPensione { get { return _RevAltraPensione; } set { _RevAltraPensione = value; } }
            public System.Nullable<byte> TipoLiquidazione { get { return _TipoLiquidazione; } set { _TipoLiquidazione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaPrimoSupplemento { get { return _DecorrenzaPrimoSupplemento; } set { _DecorrenzaPrimoSupplemento = value; } }
            public System.Nullable<decimal> ImpContribPrimoSupplemento { get { return _ImpContribPrimoSupplemento; } set { _ImpContribPrimoSupplemento = value; } }
            public System.Nullable<System.DateTime> DecorrenzaSecondoSupplemento { get { return _DecorrenzaSecondoSupplemento; } set { _DecorrenzaSecondoSupplemento = value; } }
            public System.Nullable<decimal> ImpContribSecondoSupplemento { get { return _ImpContribSecondoSupplemento; } set { _ImpContribSecondoSupplemento = value; } }

            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiFondoET fondoET = (DatiFondoET)obj;
                try
                {
                    if (this._AAInterruzione != fondoET._AAInterruzione ||
                         this._AnnoRequisiti != fondoET._AnnoRequisiti ||
                         this._AnzianitaAnni != fondoET._AnzianitaAnni ||
                         this._CodAzienda != fondoET._CodAzienda ||
                         this._CodiceEsodo != fondoET._CodiceEsodo ||
                         this._CodiceRateo66 != fondoET._CodiceRateo66 ||
                         this._CodiceServizioMilitare != fondoET._CodiceServizioMilitare ||
                         this._Competenze40Percento != fondoET._Competenze40Percento ||
                         this._ContributiAgoLegge140830 != fondoET._ContributiAgoLegge140830 ||
                         this._ContributiAgoLegge40245 != fondoET._ContributiAgoLegge40245 ||
                         this._DataEsonero != fondoET._DataEsonero ||
                         this._DecorrenzaTeorica != fondoET._DecorrenzaTeorica ||
                         this._ElementiAccessori != fondoET._ElementiAccessori ||
                         this._GGInterruzione != fondoET._GGInterruzione ||
                         this._GradoInvalidita != fondoET._GradoInvalidita ||
                         this._Importo13ma != fondoET._Importo13ma ||
                         this._Importo14ma != fondoET._Importo14ma ||
                         this._ImportoRenditaInail != fondoET._ImportoRenditaInail ||
                         this._MMInterruzione != fondoET._MMInterruzione ||
                         this._NSettimaneLeva != fondoET._NSettimaneLeva ||
                         this._NSettimaneRichiamato != fondoET._NSettimaneRichiamato ||
                         this._NumeroTriSemRequisiti != fondoET._NumeroTriSemRequisiti ||
                         this._PartTime != fondoET._PartTime ||
                         this._Requisiti247_243 != fondoET._Requisiti247_243 ||
                         this._RetribuzioneEffettiva != fondoET._RetribuzioneEffettiva ||
                         this._RetribuzioneEsodo != fondoET._RetribuzioneEsodo ||
                         this._Stipendio != fondoET._Stipendio ||
                         this._PersonaleViaggiante != fondoET._PersonaleViaggiante ||
                         this._SetAnzTotAltraPensione != fondoET._SetAnzTotAltraPensione ||
                         this._BaseAltraPensione != fondoET._BaseAltraPensione ||
                         this._CategoriaAltraPensione != fondoET._CategoriaAltraPensione ||
                         this._CertificatoAltraPensione != fondoET._CertificatoAltraPensione ||
                         this._RmsImpAltraPensione != fondoET._RmsImpAltraPensione ||
                         this._DecorrenzaAltraPensione != fondoET._DecorrenzaAltraPensione ||
                         this._RevAltraPensione != fondoET._RevAltraPensione ||
                         this._TipoLiquidazione != fondoET._TipoLiquidazione ||
                         this._DecorrenzaPrimoSupplemento != fondoET._DecorrenzaPrimoSupplemento ||
                         this._ImpContribPrimoSupplemento != fondoET._ImpContribPrimoSupplemento ||
                         this._DecorrenzaSecondoSupplemento != fondoET._DecorrenzaSecondoSupplemento ||
                         this._ImpContribSecondoSupplemento != fondoET._ImpContribSecondoSupplemento
                    )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._AAInterruzione != null ? this._AAInterruzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoRequisiti != null ? this._AnnoRequisiti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnzianitaAnni != null ? this._AnzianitaAnni.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodAzienda != null ? this._CodAzienda.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceEsodo != null ? this._CodiceEsodo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceRateo66 != null ? this._CodiceRateo66.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceServizioMilitare != null ? this._CodiceServizioMilitare.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Competenze40Percento != null ? this._Competenze40Percento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ContributiAgoLegge140830 != null ? this._ContributiAgoLegge140830.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ContributiAgoLegge40245 != null ? this._ContributiAgoLegge40245.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DataEsonero != null ? this._DataEsonero.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaTeorica != null ? this._DecorrenzaTeorica.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ElementiAccessori != null ? this._ElementiAccessori.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._GGInterruzione != null ? this._GGInterruzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._GradoInvalidita != null ? this._GradoInvalidita.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Importo13ma != null ? this._Importo13ma.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Importo14ma != null ? this._Importo14ma.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImportoRenditaInail != null ? this._ImportoRenditaInail.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MMInterruzione != null ? this._MMInterruzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NSettimaneLeva != null ? this._NSettimaneLeva.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NSettimaneRichiamato != null ? this._NSettimaneRichiamato.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NumeroTriSemRequisiti != null ? this._NumeroTriSemRequisiti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._PartTime != null ? this._PartTime.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Requisiti247_243 != null ? this._Requisiti247_243.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetribuzioneEffettiva != null ? this._RetribuzioneEffettiva.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RetribuzioneEsodo != null ? this._RetribuzioneEsodo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Stipendio != null ? this._Stipendio.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._PersonaleViaggiante != null ? this._PersonaleViaggiante.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._SetAnzTotAltraPensione != null ? this._SetAnzTotAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._BaseAltraPensione != null ? this._BaseAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CategoriaAltraPensione != null ? this._CategoriaAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CertificatoAltraPensione != null ? this._CertificatoAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RmsImpAltraPensione != null ? this._RmsImpAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaAltraPensione != null ? this._DecorrenzaAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._RevAltraPensione != null ? this._RevAltraPensione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._TipoLiquidazione != null ? this._TipoLiquidazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaPrimoSupplemento != null ? this._DecorrenzaPrimoSupplemento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImpContribPrimoSupplemento != null ? this._ImpContribPrimoSupplemento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaSecondoSupplemento != null ? this._DecorrenzaSecondoSupplemento.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ImpContribSecondoSupplemento != null ? this._ImpContribSecondoSupplemento.GetHashCode() : 0);
            //    return hash;
            //}

            public bool IsNullAltraPensioneDatiAgo()
            {
                if (this._SetAnzTotAltraPensione.HasValue ||
                this._BaseAltraPensione.HasValue ||
                this._CategoriaAltraPensione != null ||
                this._CertificatoAltraPensione.HasValue ||
                this._RmsImpAltraPensione.HasValue ||
                this._DecorrenzaAltraPensione.HasValue ||
                this._RevAltraPensione.HasValue ||
                this._TipoLiquidazione.HasValue ||
                this._DecorrenzaPrimoSupplemento.HasValue ||
                this._ImpContribPrimoSupplemento.HasValue ||
                this._DecorrenzaSecondoSupplemento.HasValue ||
                this._ImpContribSecondoSupplemento.HasValue)
                    return false;
                return true;
            }
            #endregion public members
        }

        public class DatiFondoEL
        {
            public DatiFondoEL()
            { }
            public DatiFondoEL(string tettoAgo, long? codiceAzienda, System.Nullable<byte> annoAnzianitaPregressa, System.Nullable<byte> meseAnzianitaPregressa,
                System.Nullable<byte> annoRiscatti, System.Nullable<byte> meseRiscatti, System.Nullable<System.DateTime> decorrenzaTeorica,
                System.Nullable<byte> annoArt3Legge107971, System.Nullable<byte> meseArt3Legge107971, System.Nullable<byte> annoServizioMilitare, System.Nullable<byte> meseServizioMilitare,
                System.Nullable<byte> maggiorazioneSenzaLegge33670, System.Nullable<System.DateTime> decorrenza,
                System.Nullable<byte> proRataEnel, System.Nullable<byte> gradoInvalidita,
                System.Nullable<byte> percentualeMaggiorazione, System.Nullable<byte> percentualeRiduzione,
                System.Nullable<char> convenzioneInternazionale, System.Nullable<bool> requisiti247_243,
                System.Nullable<byte> numeroTriSemRequisiti, System.Nullable<short> annoRequisiti, System.Nullable<int> anzianitaAnni)
            {
                this._TettoAgo = tettoAgo;

                this._CodiceAzienda = codiceAzienda;

                this._AnnoAnzianitaPregressa = annoAnzianitaPregressa;

                this._MeseAnzianitaPregressa = meseAnzianitaPregressa;

                this._AnnoRiscatti = annoRiscatti;

                this._MeseRiscatti = meseRiscatti;

                this._DecorrenzaTeorica = decorrenzaTeorica;

                this._AnnoArt3Legge107971 = annoArt3Legge107971;

                this._MeseArt3Legge107971 = meseArt3Legge107971;

                this._AnnoServizioMilitare = annoServizioMilitare;

                this._MeseServizioMilitare = meseServizioMilitare;

                this._MaggiorazioneSenzaLegge33670 = maggiorazioneSenzaLegge33670;

                this._Decorrenza = decorrenza;

                this._ProRataEnel = proRataEnel;

                this._GradoInvalidita = gradoInvalidita;

                this._PercentualeMaggiorazione = percentualeMaggiorazione;

                this._PercentualeRiduzione = percentualeRiduzione;

                this._ConvenzioneInternazionale = convenzioneInternazionale;

                this._Requisiti247_243 = requisiti247_243;

                this._NumeroTriSemRequisiti = numeroTriSemRequisiti;

                this._AnnoRequisiti = annoRequisiti;

                this._AnzianitaAnni = anzianitaAnni;
            }

            #region private properties
            private string _TettoAgo;

            private long? _CodiceAzienda;

            private System.Nullable<byte> _AnnoAnzianitaPregressa;

            private System.Nullable<byte> _MeseAnzianitaPregressa;

            private System.Nullable<byte> _AnnoRiscatti;

            private System.Nullable<byte> _MeseRiscatti;

            private System.Nullable<System.DateTime> _DecorrenzaTeorica;

            private System.Nullable<byte> _AnnoArt3Legge107971;

            private System.Nullable<byte> _MeseArt3Legge107971;

            private System.Nullable<byte> _AnnoServizioMilitare;

            private System.Nullable<byte> _MeseServizioMilitare;

            private System.Nullable<byte> _MaggiorazioneSenzaLegge33670;

            private System.Nullable<System.DateTime> _Decorrenza;

            private System.Nullable<byte> _ProRataEnel;

            private System.Nullable<byte> _GradoInvalidita;

            private System.Nullable<byte> _PercentualeMaggiorazione;

            private System.Nullable<byte> _PercentualeRiduzione;

            private System.Nullable<char> _ConvenzioneInternazionale;

            private System.Nullable<bool> _Requisiti247_243;

            private System.Nullable<byte> _NumeroTriSemRequisiti;

            private System.Nullable<short> _AnnoRequisiti;

            private System.Nullable<int> _AnzianitaAnni;

            #endregion private properties

            #region public properties
            public string TettoAgo { get { return _TettoAgo; } set { _TettoAgo = value; } }
            public long? CodiceAzienda { get { return _CodiceAzienda; } set { _CodiceAzienda = value; } }
            public System.Nullable<byte> AnnoAnzianitaPregressa { get { return _AnnoAnzianitaPregressa; } set { _AnnoAnzianitaPregressa = value; } }
            public System.Nullable<byte> MeseAnzianitaPregressa { get { return _MeseAnzianitaPregressa; } set { _MeseAnzianitaPregressa = value; } }
            public System.Nullable<byte> AnnoRiscatti { get { return _AnnoRiscatti; } set { _AnnoRiscatti = value; } }
            public System.Nullable<byte> MeseRiscatti { get { return _MeseRiscatti; } set { _MeseRiscatti = value; } }
            public System.Nullable<System.DateTime> DecorrenzaTeorica { get { return _DecorrenzaTeorica; } set { _DecorrenzaTeorica = value; } }
            public System.Nullable<byte> AnnoArt3Legge107971 { get { return _AnnoArt3Legge107971; } set { _AnnoArt3Legge107971 = value; } }
            public System.Nullable<byte> MeseArt3Legge107971 { get { return _MeseArt3Legge107971; } set { _MeseArt3Legge107971 = value; } }
            public System.Nullable<byte> AnnoServizioMilitare { get { return _AnnoServizioMilitare; } set { _AnnoServizioMilitare = value; } }
            public System.Nullable<byte> MeseServizioMilitare { get { return _MeseServizioMilitare; } set { _MeseServizioMilitare = value; } }
            public System.Nullable<byte> MaggiorazioneSenzaLegge33670 { get { return _MaggiorazioneSenzaLegge33670; } set { _MaggiorazioneSenzaLegge33670 = value; } }
            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public System.Nullable<byte> ProRataEnel { get { return _ProRataEnel; } set { _ProRataEnel = value; } }
            public System.Nullable<byte> GradoInvalidita { get { return _GradoInvalidita; } set { _GradoInvalidita = value; } }
            public System.Nullable<byte> PercentualeMaggiorazione { get { return _PercentualeMaggiorazione; } set { _PercentualeMaggiorazione = value; } }
            public System.Nullable<byte> PercentualeRiduzione { get { return _PercentualeRiduzione; } set { _PercentualeRiduzione = value; } }
            public System.Nullable<char> ConvenzioneInternazionale { get { return _ConvenzioneInternazionale; } set { _ConvenzioneInternazionale = value; } }
            public bool? Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }
            public byte? NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public System.Nullable<int> AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }

            #endregion public properties

            #region public members
            public override bool Equals(object obj)
            {
                DatiFondoEL fondoEL = (DatiFondoEL)obj;
                try
                {
                    if ((this._TettoAgo != null ? this._TettoAgo.Trim() : null) != (fondoEL._TettoAgo != null ? fondoEL._TettoAgo.Trim() : null) ||
                         this._CodiceAzienda != fondoEL._CodiceAzienda ||
                         this._AnnoAnzianitaPregressa != fondoEL._AnnoAnzianitaPregressa ||
                         this._MeseAnzianitaPregressa != fondoEL._MeseAnzianitaPregressa ||
                         this._AnnoRiscatti != fondoEL._AnnoRiscatti ||
                         this._MeseRiscatti != fondoEL._MeseRiscatti ||
                         this._DecorrenzaTeorica != fondoEL._DecorrenzaTeorica ||
                         this._AnnoArt3Legge107971 != fondoEL._AnnoArt3Legge107971 ||
                         this._MeseArt3Legge107971 != fondoEL._MeseArt3Legge107971 ||
                         this._AnnoServizioMilitare != fondoEL._AnnoServizioMilitare ||
                         this._MeseServizioMilitare != fondoEL._MeseServizioMilitare ||
                         this._MaggiorazioneSenzaLegge33670 != fondoEL._MaggiorazioneSenzaLegge33670 ||
                         this._Decorrenza != fondoEL._Decorrenza ||
                         this._ProRataEnel != fondoEL._ProRataEnel ||
                         this._GradoInvalidita != fondoEL._GradoInvalidita ||
                         this._PercentualeMaggiorazione != fondoEL._PercentualeMaggiorazione ||
                         this._PercentualeRiduzione != fondoEL._PercentualeRiduzione ||
                         this._ConvenzioneInternazionale != fondoEL._ConvenzioneInternazionale ||
                         this._Requisiti247_243 != fondoEL._Requisiti247_243 ||
                         this._NumeroTriSemRequisiti != fondoEL._NumeroTriSemRequisiti ||
                         this._AnnoRequisiti != fondoEL._AnnoRequisiti ||
                         this._AnzianitaAnni != fondoEL._AnzianitaAnni)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            //TODO GETHASHCODE
            //public override int GetHashCode()
            //{
            //    int hash = 13;
            //    hash = (hash * 7) + (this._TettoAgo != null ? this._TettoAgo.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._CodiceAzienda != null ? this._CodiceAzienda.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoAnzianitaPregressa != null ? this._AnnoAnzianitaPregressa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MeseAnzianitaPregressa != null ? this._MeseAnzianitaPregressa.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoRiscatti != null ? this._AnnoRiscatti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MeseRiscatti != null ? this._MeseRiscatti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._DecorrenzaTeorica != null ? this._DecorrenzaTeorica.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoArt3Legge107971 != null ? this._AnnoArt3Legge107971.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MeseArt3Legge107971 != null ? this._MeseArt3Legge107971.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoServizioMilitare != null ? this._AnnoServizioMilitare.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MeseServizioMilitare != null ? this._MeseServizioMilitare.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._MaggiorazioneSenzaLegge33670 != null ? this._MaggiorazioneSenzaLegge33670.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Decorrenza != null ? this._Decorrenza.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ProRataEnel != null ? this._ProRataEnel.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._GradoInvalidita != null ? this._GradoInvalidita.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._PercentualeMaggiorazione != null ? this._PercentualeMaggiorazione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._PercentualeRiduzione != null ? this._PercentualeRiduzione.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._ConvenzioneInternazionale != null ? this._ConvenzioneInternazionale.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._Requisiti247_243 != null ? this._Requisiti247_243.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._NumeroTriSemRequisiti != null ? this._NumeroTriSemRequisiti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnnoRequisiti != null ? this._AnnoRequisiti.GetHashCode() : 0);
            //    hash = (hash * 7) + (this._AnzianitaAnni != null ? this._AnzianitaAnni.GetHashCode() : 0);
            //    return hash;
            //}
            #endregion public members
        }

        public class DatiFondoTT
        {
            public DatiFondoTT()
            { }

            public DatiFondoTT(int? riscattiContributiFissiAnni, int? riscattiContributiFissiMesi, int? riscattiContributiFissiGiorni, int? riscattiRiservaMatematicaAnni,
                                int? riscattiRiservaMatematicaMesi, int? riscattiRiservaMatematicaGiorni, int? periodiFigurativiAnni, int? periodiFigurativiMesi, int? periodiFigurativiGiorni,
                                DateTime? decorrenza, DateTime? decorrenzaTeorica, decimal? supplementoLegge58367, decimal? pensioneMensileAl53, decimal? retribuzioneUltimoAnnoQuotaA,
                                decimal? retribuzioneBiennio, decimal? elementiAccessori, decimal? renditaInailAnnua, decimal? retribuzioneMensileInail, decimal? pensioneDirettaGenitori,
                                decimal? retribuzioneSupplementi, bool requisiti247_243, byte? numeroTriSemRequisiti, short? annoRequisiti, int? anzianitaAnni, char convenzioneInternazionale,
                                long? ditta, bool codiceArt5L58, bool dimissioniAnte97)
            {
                _RiscattiContributiFissiAnni = riscattiContributiFissiAnni;
                _RiscattiContributiFissiMesi = riscattiContributiFissiMesi;
                _RiscattiContributiFissiGiorni = riscattiContributiFissiGiorni;
                _RiscattiRiservaMatematicaAnni = riscattiRiservaMatematicaAnni;
                _RiscattiRiservaMatematicaMesi = riscattiRiservaMatematicaMesi;
                _RiscattiRiservaMatematicaGiorni = riscattiRiservaMatematicaGiorni;
                _PeriodiFigurativiAnni = periodiFigurativiAnni;
                _PeriodiFigurativiMesi = periodiFigurativiMesi;
                _PeriodiFigurativiGiorni = periodiFigurativiGiorni;
                _Decorrenza = decorrenza;
                _DecorrenzaTeorica = decorrenzaTeorica;
                _SupplementoLegge58367 = supplementoLegge58367;
                _PensioneMensileAl53 = pensioneMensileAl53;
                _RetribuzioneUltimoAnnoQuotaA = retribuzioneUltimoAnnoQuotaA;
                _RetribuzioneBiennio = retribuzioneBiennio;
                _ElementiAccessori = elementiAccessori;
                _RenditaInailAnnua = renditaInailAnnua;
                _RetribuzioneMensileInail = retribuzioneMensileInail;
                _PensioneDirettaGenitori = pensioneDirettaGenitori;
                _RetribuzioneSupplementi = retribuzioneSupplementi;
                _Requisiti247_243 = requisiti247_243;
                _NumeroTriSemRequisiti = numeroTriSemRequisiti;
                _AnnoRequisiti = annoRequisiti;
                _AnzianitaAnni = anzianitaAnni;
                _ConvenzioneInternazionale = convenzioneInternazionale;
                _Ditta = ditta;
                _CodiceArt5L58 = codiceArt5L58;
                _DimissioniAnte97 = dimissioniAnte97;
            }

            #region private properties

            private long _IdFondo;
            private int? _RiscattiContributiFissiAnni;
            private int? _RiscattiContributiFissiMesi;
            private int? _RiscattiContributiFissiGiorni;
            private int? _RiscattiRiservaMatematicaAnni;
            private int? _RiscattiRiservaMatematicaMesi;
            private int? _RiscattiRiservaMatematicaGiorni;
            private int? _PeriodiFigurativiAnni;
            private int? _PeriodiFigurativiMesi;
            private int? _PeriodiFigurativiGiorni;
            private DateTime? _Decorrenza;
            private DateTime? _DecorrenzaTeorica;
            private decimal? _SupplementoLegge58367;
            private decimal? _PensioneMensileAl53;
            private decimal? _RetribuzioneUltimoAnnoQuotaA;
            private decimal? _RetribuzioneBiennio;
            private decimal? _ElementiAccessori;
            private decimal? _RenditaInailAnnua;
            private decimal? _RetribuzioneMensileInail;
            private decimal? _PensioneDirettaGenitori;
            private decimal? _RetribuzioneSupplementi;
            private bool? _Requisiti247_243;
            private byte? _NumeroTriSemRequisiti;
            private short? _AnnoRequisiti;
            private int? _AnzianitaAnni;
            private char? _ConvenzioneInternazionale;
            private long? _Ditta;
            private bool? _CodiceArt5L58;
            private bool? _DimissioniAnte97;

            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public int? RiscattiContributiFissiAnni { get { return _RiscattiContributiFissiAnni; } set { _RiscattiContributiFissiAnni = value; } }
            public int? RiscattiContributiFissiMesi { get { return _RiscattiContributiFissiMesi; } set { _RiscattiContributiFissiMesi = value; } }
            public int? RiscattiContributiFissiGiorni { get { return _RiscattiContributiFissiGiorni; } set { _RiscattiContributiFissiGiorni = value; } }
            public int? RiscattiRiservaMatematicaAnni { get { return _RiscattiRiservaMatematicaAnni; } set { _RiscattiRiservaMatematicaAnni = value; } }
            public int? RiscattiRiservaMatematicaMesi { get { return _RiscattiRiservaMatematicaMesi; } set { _RiscattiRiservaMatematicaMesi = value; } }
            public int? RiscattiRiservaMatematicaGiorni { get { return _RiscattiRiservaMatematicaGiorni; } set { _RiscattiRiservaMatematicaGiorni = value; } }
            public int? PeriodiFigurativiAnni { get { return _PeriodiFigurativiAnni; } set { _PeriodiFigurativiAnni = value; } }
            public int? PeriodiFigurativiMesi { get { return _PeriodiFigurativiMesi; } set { _PeriodiFigurativiMesi = value; } }
            public int? PeriodiFigurativiGiorni { get { return _PeriodiFigurativiGiorni; } set { _PeriodiFigurativiGiorni = value; } }
            public DateTime? Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public DateTime? DecorrenzaTeorica { get { return _DecorrenzaTeorica; } set { _DecorrenzaTeorica = value; } }
            public decimal? SupplementoLegge58367 { get { return _SupplementoLegge58367; } set { _SupplementoLegge58367 = value; } }
            public decimal? PensioneMensileAl53 { get { return _PensioneMensileAl53; } set { _PensioneMensileAl53 = value; } }
            public decimal? RetribuzioneUltimoAnnoQuotaA { get { return _RetribuzioneUltimoAnnoQuotaA; } set { _RetribuzioneUltimoAnnoQuotaA = value; } }
            public decimal? RetribuzioneBiennio { get { return _RetribuzioneBiennio; } set { _RetribuzioneBiennio = value; } }
            public decimal? ElementiAccessori { get { return _ElementiAccessori; } set { _ElementiAccessori = value; } }
            public decimal? RenditaInailAnnua { get { return _RenditaInailAnnua; } set { _RenditaInailAnnua = value; } }
            public decimal? RetribuzioneMensileInail { get { return _RetribuzioneMensileInail; } set { _RetribuzioneMensileInail = value; } }
            public decimal? PensioneDirettaGenitori { get { return _PensioneDirettaGenitori; } set { _PensioneDirettaGenitori = value; } }
            public decimal? RetribuzioneSupplementi { get { return _RetribuzioneSupplementi; } set { _RetribuzioneSupplementi = value; } }
            public bool? Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }
            public byte? NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public int? AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }
            public char? ConvenzioneInternazionale { get { return _ConvenzioneInternazionale; } set { _ConvenzioneInternazionale = value; } }
            public long? Ditta { get { return _Ditta; } set { _Ditta = value; } }
            public bool? CodiceArt5L58 { get { return _CodiceArt5L58; } set { _CodiceArt5L58 = value; } }
            public bool? DimissioniAnte97 { get { return _DimissioniAnte97; } set { _DimissioniAnte97 = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoTT fondoTT = (DatiFondoTT)obj;
                try
                {
                    if (this._AnnoRequisiti != fondoTT._AnnoRequisiti ||
                         this._AnzianitaAnni != fondoTT._AnzianitaAnni ||
                         this._CodiceArt5L58 != fondoTT._CodiceArt5L58 ||
                         this._ConvenzioneInternazionale != fondoTT._ConvenzioneInternazionale ||
                         this._Decorrenza != fondoTT._Decorrenza ||
                         this._DecorrenzaTeorica != fondoTT._DecorrenzaTeorica ||
                         this._DimissioniAnte97 != fondoTT._DimissioniAnte97 ||
                         this._Ditta != fondoTT._Ditta ||
                         this._ElementiAccessori != fondoTT._ElementiAccessori ||
                         this._NumeroTriSemRequisiti != fondoTT._NumeroTriSemRequisiti ||
                         this._PensioneDirettaGenitori != fondoTT._PensioneDirettaGenitori ||
                         this._PensioneMensileAl53 != fondoTT._PensioneMensileAl53 ||
                         this._PeriodiFigurativiAnni != fondoTT._PeriodiFigurativiAnni ||
                         this._PeriodiFigurativiGiorni != fondoTT._PeriodiFigurativiGiorni ||
                         this._PeriodiFigurativiMesi != fondoTT._PeriodiFigurativiMesi ||
                         this._RenditaInailAnnua != fondoTT._RenditaInailAnnua ||
                         this._Requisiti247_243 != fondoTT._Requisiti247_243 ||
                         this._RetribuzioneBiennio != fondoTT._RetribuzioneBiennio ||
                         this._RetribuzioneMensileInail != fondoTT._RetribuzioneMensileInail ||
                         this._RetribuzioneSupplementi != fondoTT._RetribuzioneSupplementi ||
                         this._RetribuzioneUltimoAnnoQuotaA != fondoTT._RetribuzioneUltimoAnnoQuotaA ||
                         this._RiscattiContributiFissiAnni != fondoTT._RiscattiContributiFissiAnni ||
                         this._RiscattiContributiFissiGiorni != fondoTT._RiscattiContributiFissiGiorni ||
                         this._RiscattiContributiFissiMesi != fondoTT._RiscattiContributiFissiMesi ||
                         this._RiscattiRiservaMatematicaAnni != fondoTT._RiscattiRiservaMatematicaAnni ||
                         this._RiscattiRiservaMatematicaGiorni != fondoTT._RiscattiRiservaMatematicaGiorni ||
                         this._RiscattiRiservaMatematicaMesi != fondoTT._RiscattiRiservaMatematicaMesi ||
                         this._SupplementoLegge58367 != fondoTT._SupplementoLegge58367
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoVL
        {
            public DatiFondoVL()
            { }

            public DatiFondoVL(decimal? aliquotaIrpef, DateTime? dataInvalidita, decimal? importoPensione1977, DateTime? decorrenzaPensione, DateTime? decorrenzaPensioneAgo,
                    decimal? importoPensioneAgo, decimal? importoPensioneAgoSupplementare, DateTime? decorrenza, byte? codiceArt22, int? servizioUtileQuotaA1, int? servizioUtileQuotaB,
                    int? servizioUtileQuotaA2, int? servizioUtileQuotaC, int? prosecuzioneVolontariaAA, int? riscattiRicongiunzioniAA, byte? codiceCapitalizzazione, decimal? importoPercentualeCapitalizzazione,
                    int? retribuzioneSettimanaleAgoQuotaA, decimal? retribuzioneSettimanaleAgoQuotaB, decimal? quotaMensileCapitalizzazione, decimal? capitaleErogato, int? prosecuzioneVolontariaMM,
                    int? prosecuzioneVolontariaGG, int? riscattiRicongiunzioniMM, int? riscattiRicongiunzioniGG, char? convenzioneInternazionale, bool? lavoratorePrecoce,
                    bool? requisiti247_243, byte? numeroTriSemRequisiti, short? annoRequisiti, int? anzianitaAnni)
            {
                this._AliquotaIrpef = aliquotaIrpef;
                this._DataInvalidita = dataInvalidita;
                this._ImportoPensione1977 = importoPensione1977;
                this._DecorrenzaPensione = decorrenzaPensione;
                this._DecorrenzaPensioneAgo = decorrenzaPensioneAgo;
                this._ImportoPensioneAgo = importoPensioneAgo;
                this._ImportoPensioneAgoSupplementare = importoPensioneAgoSupplementare;
                this._Decorrenza = decorrenza;
                this._CodiceArt22 = codiceArt22;
                this._ServizioUtileQuotaA1 = servizioUtileQuotaA1;
                this._ServizioUtileQuotaB = servizioUtileQuotaB;
                this._ServizioUtileQuotaA2 = servizioUtileQuotaA2;
                this._ServizioUtileQuotaC = servizioUtileQuotaC;
                this._ProsecuzioneVolontariaAA = prosecuzioneVolontariaAA;
                this._RiscattiRicongiunzioniAA = riscattiRicongiunzioniAA;
                this._CodiceCapitalizzazione = codiceCapitalizzazione;
                this._ImportoPercentualeCapitalizzazione = importoPercentualeCapitalizzazione;
                this._RetribuzioneSettimanaleAgoQuotaA = retribuzioneSettimanaleAgoQuotaA;
                this._RetribuzioneSettimanaleAgoQuotaB = retribuzioneSettimanaleAgoQuotaB;
                this._QuotaMensileCapitalizzazione = quotaMensileCapitalizzazione;
                this._CapitaleErogato = capitaleErogato;
                this._ProsecuzioneVolontariaMM = prosecuzioneVolontariaMM;
                this._ProsecuzioneVolontariaGG = prosecuzioneVolontariaGG;
                this._RiscattiRicongiunzioniMM = riscattiRicongiunzioniMM;
                this._RiscattiRicongiunzioniGG = riscattiRicongiunzioniGG;
                this._ConvenzioneInternazionale = convenzioneInternazionale;
                this._LavoratorePrecoce = lavoratorePrecoce;
                this._Requisiti247_243 = requisiti247_243;
                this._NumeroTriSemRequisiti = numeroTriSemRequisiti;
                this._AnnoRequisiti = annoRequisiti;
                this._AnzianitaAnni = anzianitaAnni;
            }

            #region private properties

            private long _IdFondo;
            private System.Nullable<decimal> _AliquotaIrpef;
            private System.Nullable<System.DateTime> _DataInvalidita;
            private System.Nullable<decimal> _ImportoPensione1977;
            private System.Nullable<System.DateTime> _DecorrenzaPensione;
            private System.Nullable<System.DateTime> _DecorrenzaPensioneAgo;
            private System.Nullable<decimal> _ImportoPensioneAgo;
            private System.Nullable<decimal> _ImportoPensioneAgoSupplementare;
            private System.Nullable<System.DateTime> _Decorrenza;
            private System.Nullable<byte> _CodiceArt22;
            private System.Nullable<int> _ServizioUtileQuotaA1;
            private System.Nullable<int> _ServizioUtileQuotaB;
            private System.Nullable<int> _ServizioUtileQuotaA2;
            private System.Nullable<int> _ServizioUtileQuotaC;
            private System.Nullable<int> _ProsecuzioneVolontariaAA;
            private System.Nullable<int> _RiscattiRicongiunzioniAA;
            private System.Nullable<byte> _CodiceCapitalizzazione;
            private System.Nullable<decimal> _ImportoPercentualeCapitalizzazione;
            private System.Nullable<decimal> _RetribuzioneSettimanaleAgoQuotaA;
            private System.Nullable<decimal> _RetribuzioneSettimanaleAgoQuotaB;
            private System.Nullable<decimal> _QuotaMensileCapitalizzazione;
            private System.Nullable<decimal> _CapitaleErogato;
            private System.Nullable<int> _ProsecuzioneVolontariaMM;
            private System.Nullable<int> _ProsecuzioneVolontariaGG;
            private System.Nullable<int> _RiscattiRicongiunzioniMM;
            private System.Nullable<int> _RiscattiRicongiunzioniGG;
            private System.Nullable<char> _ConvenzioneInternazionale;
            private System.Nullable<bool> _LavoratorePrecoce;
            private System.Nullable<bool> _Requisiti247_243;
            private System.Nullable<byte> _NumeroTriSemRequisiti;
            private System.Nullable<short> _AnnoRequisiti;
            private System.Nullable<int> _AnzianitaAnni;

            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public System.Nullable<decimal> AliquotaIrpef { get { return _AliquotaIrpef; } set { _AliquotaIrpef = value; } }
            public System.Nullable<System.DateTime> DataInvalidita { get { return _DataInvalidita; } set { _DataInvalidita = value; } }
            public System.Nullable<decimal> ImportoPensione1977 { get { return _ImportoPensione1977; } set { _ImportoPensione1977 = value; } }
            public System.Nullable<System.DateTime> DecorrenzaPensione { get { return _DecorrenzaPensione; } set { _DecorrenzaPensione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaPensioneAgo { get { return _DecorrenzaPensioneAgo; } set { _DecorrenzaPensioneAgo = value; } }
            public System.Nullable<decimal> ImportoPensioneAgo { get { return _ImportoPensioneAgo; } set { _ImportoPensioneAgo = value; } }
            public System.Nullable<decimal> ImportoPensioneAgoSupplementare { get { return _ImportoPensioneAgoSupplementare; } set { _ImportoPensioneAgoSupplementare = value; } }
            public System.Nullable<System.DateTime> Decorrenza { get { return _Decorrenza; } set { _Decorrenza = value; } }
            public System.Nullable<byte> CodiceArt22 { get { return _CodiceArt22; } set { _CodiceArt22 = value; } }
            public System.Nullable<int> ServizioUtileQuotaA1 { get { return _ServizioUtileQuotaA1; } set { _ServizioUtileQuotaA1 = value; } }
            public System.Nullable<int> ServizioUtileQuotaB { get { return _ServizioUtileQuotaB; } set { _ServizioUtileQuotaB = value; } }
            public System.Nullable<int> ServizioUtileQuotaA2 { get { return _ServizioUtileQuotaA2; } set { _ServizioUtileQuotaA2 = value; } }
            public System.Nullable<int> ServizioUtileQuotaC { get { return _ServizioUtileQuotaC; } set { _ServizioUtileQuotaC = value; } }
            public System.Nullable<int> ProsecuzioneVolontariaAA { get { return _ProsecuzioneVolontariaAA; } set { _ProsecuzioneVolontariaAA = value; } }
            public System.Nullable<int> RiscattiRicongiunzioniAA { get { return _RiscattiRicongiunzioniAA; } set { _RiscattiRicongiunzioniAA = value; } }
            public System.Nullable<byte> CodiceCapitalizzazione { get { return _CodiceCapitalizzazione; } set { _CodiceCapitalizzazione = value; } }
            public System.Nullable<decimal> ImportoPercentualeCapitalizzazione { get { return _ImportoPercentualeCapitalizzazione; } set { _ImportoPercentualeCapitalizzazione = value; } }
            public System.Nullable<decimal> RetribuzioneSettimanaleAgoQuotaA { get { return _RetribuzioneSettimanaleAgoQuotaA; } set { _RetribuzioneSettimanaleAgoQuotaA = value; } }
            public System.Nullable<decimal> RetribuzioneSettimanaleAgoQuotaB { get { return _RetribuzioneSettimanaleAgoQuotaB; } set { _RetribuzioneSettimanaleAgoQuotaB = value; } }
            public System.Nullable<decimal> QuotaMensileCapitalizzazione { get { return _QuotaMensileCapitalizzazione; } set { _QuotaMensileCapitalizzazione = value; } }
            public System.Nullable<decimal> CapitaleErogato { get { return _CapitaleErogato; } set { _CapitaleErogato = value; } }
            public System.Nullable<int> ProsecuzioneVolontariaMM { get { return _ProsecuzioneVolontariaMM; } set { _ProsecuzioneVolontariaMM = value; } }
            public System.Nullable<int> ProsecuzioneVolontariaGG { get { return _ProsecuzioneVolontariaGG; } set { _ProsecuzioneVolontariaGG = value; } }
            public System.Nullable<int> RiscattiRicongiunzioniMM { get { return _RiscattiRicongiunzioniMM; } set { _RiscattiRicongiunzioniMM = value; } }
            public System.Nullable<int> RiscattiRicongiunzioniGG { get { return _RiscattiRicongiunzioniGG; } set { _RiscattiRicongiunzioniGG = value; } }
            public System.Nullable<char> ConvenzioneInternazionale { get { return _ConvenzioneInternazionale; } set { _ConvenzioneInternazionale = value; } }
            public System.Nullable<bool> LavoratorePrecoce { get { return _LavoratorePrecoce; } set { _LavoratorePrecoce = value; } }
            public bool? Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }
            public byte? NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public System.Nullable<int> AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoVL fondoVL = (DatiFondoVL)obj;
                try
                {
                    if (this._DataInvalidita != fondoVL._DataInvalidita ||
                        this._ImportoPensione1977 != fondoVL._ImportoPensione1977 ||
                        this._DecorrenzaPensione != fondoVL._DecorrenzaPensione ||
                        this._DecorrenzaPensioneAgo != fondoVL._DecorrenzaPensioneAgo ||
                        this._ImportoPensioneAgo != fondoVL._ImportoPensioneAgo ||
                        this._ImportoPensioneAgoSupplementare != fondoVL._ImportoPensioneAgoSupplementare ||
                        this._Decorrenza != fondoVL._Decorrenza ||
                        this._ServizioUtileQuotaA1 != fondoVL._ServizioUtileQuotaA1 ||
                        this._ServizioUtileQuotaB != fondoVL._ServizioUtileQuotaB ||
                        this._ServizioUtileQuotaA2 != fondoVL._ServizioUtileQuotaA2 ||
                        this._ServizioUtileQuotaC != fondoVL._ServizioUtileQuotaC ||
                        this._RetribuzioneSettimanaleAgoQuotaA != fondoVL._RetribuzioneSettimanaleAgoQuotaA ||
                        this._RetribuzioneSettimanaleAgoQuotaB != fondoVL._RetribuzioneSettimanaleAgoQuotaB ||
                        this._QuotaMensileCapitalizzazione != fondoVL._QuotaMensileCapitalizzazione ||
                        this._CapitaleErogato != fondoVL._CapitaleErogato ||
                        this._ConvenzioneInternazionale != fondoVL._ConvenzioneInternazionale ||
                        this._CodiceArt22 != fondoVL._CodiceArt22 ||
                        this._ProsecuzioneVolontariaAA != fondoVL._ProsecuzioneVolontariaAA ||
                        this._ProsecuzioneVolontariaMM != fondoVL._ProsecuzioneVolontariaMM ||
                        this._ProsecuzioneVolontariaGG != fondoVL._ProsecuzioneVolontariaGG ||
                        this._RiscattiRicongiunzioniAA != fondoVL._RiscattiRicongiunzioniAA ||
                        this._RiscattiRicongiunzioniMM != fondoVL._RiscattiRicongiunzioniMM ||
                        this._RiscattiRicongiunzioniGG != fondoVL._RiscattiRicongiunzioniGG ||
                        this._CodiceCapitalizzazione != fondoVL._CodiceCapitalizzazione ||
                        this._ImportoPercentualeCapitalizzazione != fondoVL._ImportoPercentualeCapitalizzazione ||
                        this._AliquotaIrpef != fondoVL._AliquotaIrpef ||
                        this._LavoratorePrecoce != fondoVL._LavoratorePrecoce ||
                        this._Requisiti247_243 != fondoVL._Requisiti247_243 ||
                        this._NumeroTriSemRequisiti != fondoVL._NumeroTriSemRequisiti ||
                        this._AnnoRequisiti != fondoVL._AnnoRequisiti ||
                        this._AnzianitaAnni != fondoVL._AnzianitaAnni)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoFST
        {
            public DatiFondoFST()
            { }

            public DatiFondoFST(bool? _RequisitiAnte247, byte? _TrimesteRequisiti, int? _AnzianitaAnni, long? _CausaCessazione, bool? _PagamentoIndennitaIntegrativaSpeciale,
                    bool? _IndennitaIntegrativaSpecialeConglobata, bool? _TrediciMensilita, DateTime? _DecorrenzaCalcolo, bool? _TitolareAltraPensione, decimal? _PensioneAnnuaLorda,
                    short? _ServizioUtileDirittoAA, short? _ServizioUtileDirittoMM, short? _ServizioUtileDirittoGG, int? _PrivilegiataSuperinvaliditaIndennita, int? _AssegnoIntegrativo,
                    int? _IntegrazioneIndennitaAssistenza, int? _IndennitaAccompagnamentoAggiuntiva, int? _CumuloInfermita, int? _Categoria2aInfermita, int? _AssegnoCura, int? _IndennitaSpecialeAnnua,
                    DateTime? _DecorrenzaEconomica, short? annoRequisiti, bool? _DirittoIndennitaIntegrativaSpeciale, bool? _IntegrazioneMinimo, bool? _RiduzioneL537, bool? _IISAbbattimentoAnni,
                    decimal? rmsSenzaLegge33670QA, DateTime? _ScadenzaBenefici, decimal? _PALConBenefici, bool? _ScadenzaIllimitata, short? _VVUtiliDiritto, short? _VVUtiliMisura,
                    decimal? _PensioneAnnuaLorda707, decimal? _CoefficienteTrasformazione, decimal? _PensioneAnnuaLorda214, bool? isPensioneAnnuaLordaDaPrelievo, short tipologiaPensione, bool? isPensioneAnnuaLorda707DaPrelievo,
                    decimal? _IndennitaIntegrativaSpecialeLorda, short? _ServizioUtileDirittoOIAA, short? _ServizioUtileDirittoOIMM, short? _ServizioUtileDirittoOIGG, short? _XFSFAAGO)
            {
                this._RequisitiAnte247 = _RequisitiAnte247;
                this._TrimesteRequisiti = _TrimesteRequisiti;
                this._AnzianitaAnni = _AnzianitaAnni;
                this._CausaCessazione = _CausaCessazione;
                this._PagamentoIndennitaIntegrativaSpeciale = _PagamentoIndennitaIntegrativaSpeciale;
                this._IndennitaIntegrativaSpecialeConglobata = _IndennitaIntegrativaSpecialeConglobata;
                this._TrediciMensilita = _TrediciMensilita;
                this._DecorrenzaCalcolo = _DecorrenzaCalcolo;
                this._TitolareAltraPensione = _TitolareAltraPensione;
                this._PensioneAnnuaLorda = _PensioneAnnuaLorda;
                this._ServizioUtileDirittoAA = _ServizioUtileDirittoAA;
                this._ServizioUtileDirittoMM = _ServizioUtileDirittoMM;
                this._ServizioUtileDirittoGG = _ServizioUtileDirittoGG;
                this._PrivilegiataSuperinvaliditaIndennita = _PrivilegiataSuperinvaliditaIndennita;
                this._AssegnoIntegrativo = _AssegnoIntegrativo;
                this._IntegrazioneIndennitaAssistenza = _IntegrazioneIndennitaAssistenza;
                this._IndennitaAccompagnamentoAggiuntiva = _IndennitaAccompagnamentoAggiuntiva;
                this._CumuloInfermita = _CumuloInfermita;
                this._Categoria2aInfermita = _Categoria2aInfermita;
                this._AssegnoCura = _AssegnoCura;
                this._IndennitaSpecialeAnnua = _IndennitaSpecialeAnnua;
                this._DecorrenzaEconomica = _DecorrenzaEconomica;
                this._DirittoIndennitaIntegrativaSpeciale = _DirittoIndennitaIntegrativaSpeciale;
                this._IntegrazioneMinimo = _IntegrazioneMinimo;
                this._RiduzioneL537 = _RiduzioneL537;
                this._IISAbbattimentoAnni = _IISAbbattimentoAnni;
                this._RMSSenzaLegge33670QA = rmsSenzaLegge33670QA;
                this._ScadenzaBenefici = _ScadenzaBenefici;
                this._PALConBenefici = _PALConBenefici;
                this._ScadenzaIllimitata = _ScadenzaIllimitata;
                this._VVUtiliDiritto = _VVUtiliDiritto;
                this._VVUtiliMisura = _VVUtiliMisura;
                this._PensioneAnnuaLorda707 = _PensioneAnnuaLorda707;
                this._CoefficienteTrasformazione = _CoefficienteTrasformazione;
                this._PensioneAnnuaLorda214 = _PensioneAnnuaLorda214;
                this._IsPensioneAnnuaLordaDaPrelievo = isPensioneAnnuaLordaDaPrelievo;
                this._TipologiaPensione = tipologiaPensione;
                this._IsPensioneAnnuaLorda707DaPrelievo = isPensioneAnnuaLorda707DaPrelievo;
                this._IndennitaIntegrativaSpecialeLorda = _IndennitaIntegrativaSpecialeLorda;
                this._ServizioUtileDirittoOIAA = _ServizioUtileDirittoOIAA;
                this._ServizioUtileDirittoOIMM = _ServizioUtileDirittoOIMM;
                this._ServizioUtileDirittoOIGG = _ServizioUtileDirittoOIGG;
                //ENG - RIC REVERSIBILITA 024
                this._XFSFAAGO = _XFSFAAGO;
            }

            #region private properties

            private long _IdFondo;
            private long? _IdRecordFondo;
            private bool? _RequisitiAnte247;
            private byte? _TrimesteRequisiti;
            private int? _AnzianitaAnni;
            private long? _CausaCessazione;
            private bool? _PagamentoIndennitaIntegrativaSpeciale;
            private bool? _IndennitaIntegrativaSpecialeConglobata;
            private bool? _TrediciMensilita;
            private DateTime? _DecorrenzaCalcolo;
            private bool? _TitolareAltraPensione;
            private decimal? _PensioneAnnuaLorda;
            private short? _ServizioUtileDirittoAA;
            private short? _ServizioUtileDirittoMM;
            private short? _ServizioUtileDirittoGG;
            private int? _PrivilegiataSuperinvaliditaIndennita;
            private int? _AssegnoIntegrativo;
            private int? _IntegrazioneIndennitaAssistenza;
            private int? _IndennitaAccompagnamentoAggiuntiva;
            private int? _CumuloInfermita;
            private int? _Categoria2aInfermita;
            private int? _AssegnoCura;
            private int? _IndennitaSpecialeAnnua;
            private DateTime? _DecorrenzaEconomica;
            private short? _AnnoRequisiti;
            private bool? _DirittoIndennitaIntegrativaSpeciale;
            private bool? _IntegrazioneMinimo;
            private bool? _RiduzioneL537;
            private bool? _IISAbbattimentoAnni;
            private decimal? _RMSSenzaLegge33670QA;
            private DateTime? _ScadenzaBenefici;
            private decimal? _PALConBenefici;
            private bool? _ScadenzaIllimitata;
            private short? _VVUtiliDiritto;
            private short? _VVUtiliMisura;
            private decimal? _PensioneAnnuaLorda707;
            private decimal? _CoefficienteTrasformazione;
            private decimal? _PensioneAnnuaLorda214;
            //ENG - PL Reversibilita 024
            private bool? _IsPensioneAnnuaLordaDaPrelievo;
            public short? _TipologiaPensione;
            //ENG - PL Reversibilita 024
            private bool? _IsPensioneAnnuaLorda707DaPrelievo;
            private decimal? _IndennitaIntegrativaSpecialeLorda;
            private short? _ServizioUtileDirittoOIAA;
            private short? _ServizioUtileDirittoOIMM;
            private short? _ServizioUtileDirittoOIGG;
            //ENG - RIC/TRF REVERSIBILITA 024
            private short? _XFSFAAGO;

            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }
            public bool? RequisitiAnte247 { get { return _RequisitiAnte247; } set { _RequisitiAnte247 = value; } }
            public byte? TrimesteRequisiti { get { return _TrimesteRequisiti; } set { _TrimesteRequisiti = value; } }
            public int? AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }
            public long? CausaCessazione { get { return _CausaCessazione; } set { _CausaCessazione = value; } }
            public bool? PagamentoIndennitaIntegrativaSpeciale { get { return _PagamentoIndennitaIntegrativaSpeciale; } set { _PagamentoIndennitaIntegrativaSpeciale = value; } }
            public bool? IndennitaIntegrativaSpecialeConglobata { get { return _IndennitaIntegrativaSpecialeConglobata; } set { _IndennitaIntegrativaSpecialeConglobata = value; } }
            public bool? TrediciMensilita { get { return _TrediciMensilita; } set { _TrediciMensilita = value; } }
            public DateTime? DecorrenzaCalcolo { get { return _DecorrenzaCalcolo; } set { _DecorrenzaCalcolo = value; } }
            public bool? TitolareAltraPensione { get { return _TitolareAltraPensione; } set { _TitolareAltraPensione = value; } }
            public decimal? PensioneAnnuaLorda { get { return _PensioneAnnuaLorda; } set { _PensioneAnnuaLorda = value; } }
            public short? ServizioUtileDirittoAA { get { return _ServizioUtileDirittoAA; } set { _ServizioUtileDirittoAA = value; } }
            public short? ServizioUtileDirittoMM { get { return _ServizioUtileDirittoMM; } set { _ServizioUtileDirittoMM = value; } }
            public short? ServizioUtileDirittoGG { get { return _ServizioUtileDirittoGG; } set { _ServizioUtileDirittoGG = value; } }
            public int? PrivilegiataSuperinvaliditaIndennita { get { return _PrivilegiataSuperinvaliditaIndennita; } set { _PrivilegiataSuperinvaliditaIndennita = value; } }
            public int? AssegnoIntegrativo { get { return _AssegnoIntegrativo; } set { _AssegnoIntegrativo = value; } }
            public int? IntegrazioneIndennitaAssistenza { get { return _IntegrazioneIndennitaAssistenza; } set { _IntegrazioneIndennitaAssistenza = value; } }
            public int? IndennitaAccompagnamentoAggiuntiva { get { return _IndennitaAccompagnamentoAggiuntiva; } set { _IndennitaAccompagnamentoAggiuntiva = value; } }
            public int? CumuloInfermita { get { return _CumuloInfermita; } set { _CumuloInfermita = value; } }
            public int? Categoria2aInfermita { get { return _Categoria2aInfermita; } set { _Categoria2aInfermita = value; } }
            public int? AssegnoCura { get { return _AssegnoCura; } set { _AssegnoCura = value; } }
            public int? IndennitaSpecialeAnnua { get { return _IndennitaSpecialeAnnua; } set { _IndennitaSpecialeAnnua = value; } }
            public DateTime? DecorrenzaEconomica { get { return _DecorrenzaEconomica; } set { _DecorrenzaEconomica = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public bool? DirittoIndennitaIntegrativaSpeciale { get { return _DirittoIndennitaIntegrativaSpeciale; } set { _DirittoIndennitaIntegrativaSpeciale = value; } }
            public bool? IntegrazioneMinimo { get { return _IntegrazioneMinimo; } set { _IntegrazioneMinimo = value; } }
            public bool? RiduzioneL537 { get { return _RiduzioneL537; } set { _RiduzioneL537 = value; } }
            public bool? IISAbbattimentoAnni { get { return _IISAbbattimentoAnni; } set { _IISAbbattimentoAnni = value; } }
            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
            public DateTime? ScadenzaBenefici { get { return _ScadenzaBenefici; } set { _ScadenzaBenefici = value; } }
            public decimal? PALConBenefici { get { return _PALConBenefici; } set { _PALConBenefici = value; } }
            public bool? ScadenzaIllimitata { get { return _ScadenzaIllimitata; } set { _ScadenzaIllimitata = value; } }
            public short? VVUtiliDiritto { get { return _VVUtiliDiritto; } set { _VVUtiliDiritto = value; } }
            public short? VVUtiliMisura { get { return _VVUtiliMisura; } set { _VVUtiliMisura = value; } }
            public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
            public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
            public decimal? PensioneAnnuaLorda214 { get { return _PensioneAnnuaLorda214; } set { _PensioneAnnuaLorda214 = value; } }
            //ENG - PL Reversibilita 024
            public bool? IsPensioneAnnuaLordaDaPrelievo { get { return _IsPensioneAnnuaLordaDaPrelievo; } set { _IsPensioneAnnuaLordaDaPrelievo = value; } }
            public short? TipologiaPensione { get { return _TipologiaPensione; } set { _TipologiaPensione = value; } }
            //ENG - PL Reversibilita 024
            public bool? IsPensioneAnnuaLorda707DaPrelievo { get { return _IsPensioneAnnuaLorda707DaPrelievo; } set { _IsPensioneAnnuaLorda707DaPrelievo = value; } }
            public decimal? IndennitaIntegrativaSpecialeLorda { get { return _IndennitaIntegrativaSpecialeLorda; } set { _IndennitaIntegrativaSpecialeLorda = value; } }
            public short? ServizioUtileDirittoOIAA { get { return _ServizioUtileDirittoOIAA; } set { _ServizioUtileDirittoOIAA = value; } }
            public short? ServizioUtileDirittoOIMM { get { return _ServizioUtileDirittoOIMM; } set { _ServizioUtileDirittoOIMM = value; } }
            public short? ServizioUtileDirittoOIGG { get { return _ServizioUtileDirittoOIGG; } set { _ServizioUtileDirittoOIGG = value; } }
            //ENG - RIC/TRF REVERSIBILITA 024
            public short? XFSFAAGO { get { return _XFSFAAGO; } set { _XFSFAAGO = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoFST fondoFST = (DatiFondoFST)obj;
                try
                {
                    if (this._RequisitiAnte247 != fondoFST._RequisitiAnte247 ||
                        this._TrimesteRequisiti != fondoFST._TrimesteRequisiti ||
                        this._AnzianitaAnni != fondoFST._AnzianitaAnni ||
                        this._CausaCessazione != fondoFST._CausaCessazione ||
                        this._PagamentoIndennitaIntegrativaSpeciale != fondoFST._PagamentoIndennitaIntegrativaSpeciale ||
                        this._IndennitaIntegrativaSpecialeConglobata != fondoFST._IndennitaIntegrativaSpecialeConglobata ||
                        this._TrediciMensilita != fondoFST._TrediciMensilita ||
                        this._DecorrenzaCalcolo != fondoFST._DecorrenzaCalcolo ||
                        this._TitolareAltraPensione != fondoFST._TitolareAltraPensione ||
                        this._PensioneAnnuaLorda != fondoFST._PensioneAnnuaLorda ||
                        this._ServizioUtileDirittoAA != fondoFST._ServizioUtileDirittoAA ||
                        this._ServizioUtileDirittoMM != fondoFST._ServizioUtileDirittoMM ||
                        this._ServizioUtileDirittoGG != fondoFST._ServizioUtileDirittoGG ||
                        this._PrivilegiataSuperinvaliditaIndennita != fondoFST._PrivilegiataSuperinvaliditaIndennita ||
                        this._AssegnoIntegrativo != fondoFST._AssegnoIntegrativo ||
                        this._IntegrazioneIndennitaAssistenza != fondoFST._IntegrazioneIndennitaAssistenza ||
                        this._IndennitaAccompagnamentoAggiuntiva != fondoFST._IndennitaAccompagnamentoAggiuntiva ||
                        this._CumuloInfermita != fondoFST._CumuloInfermita ||
                        this._Categoria2aInfermita != fondoFST._Categoria2aInfermita ||
                        this._AssegnoCura != fondoFST._AssegnoCura ||
                        this._IndennitaSpecialeAnnua != fondoFST._IndennitaSpecialeAnnua ||
                        this._DecorrenzaEconomica != fondoFST._DecorrenzaEconomica ||
                        this._AnnoRequisiti != fondoFST._AnnoRequisiti ||
                        this._DirittoIndennitaIntegrativaSpeciale != fondoFST._DirittoIndennitaIntegrativaSpeciale ||
                        this._IntegrazioneMinimo != fondoFST._IntegrazioneMinimo ||
                        this._RiduzioneL537 != fondoFST._RiduzioneL537 ||
                        this._IISAbbattimentoAnni != fondoFST._IISAbbattimentoAnni ||
                        this._RMSSenzaLegge33670QA != fondoFST._RMSSenzaLegge33670QA ||
                        this._ScadenzaBenefici != fondoFST._ScadenzaBenefici ||
                        this._PALConBenefici != fondoFST._PALConBenefici ||
                        this._ScadenzaIllimitata != fondoFST._ScadenzaIllimitata ||
                        this._VVUtiliDiritto != fondoFST._VVUtiliDiritto ||
                        this._VVUtiliMisura != fondoFST._VVUtiliMisura ||
                        this._PensioneAnnuaLorda707 != fondoFST._PensioneAnnuaLorda707 ||
                        this._CoefficienteTrasformazione != fondoFST._CoefficienteTrasformazione ||
                        this._PensioneAnnuaLorda214 != fondoFST._PensioneAnnuaLorda214 ||
                        this._IndennitaIntegrativaSpecialeLorda != fondoFST._IndennitaIntegrativaSpecialeLorda ||
                        this._ServizioUtileDirittoOIAA != fondoFST._ServizioUtileDirittoOIAA ||
                        this._ServizioUtileDirittoOIMM != fondoFST._ServizioUtileDirittoOIMM ||
                        this._ServizioUtileDirittoOIGG != fondoFST._ServizioUtileDirittoOIGG ||
                        this._XFSFAAGO != fondoFST._XFSFAAGO)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool IsPrivilegiataNull()
            {
                if (!this._IndennitaSpecialeAnnua.HasValue &&
                    !this._AssegnoCura.HasValue &&
                    !this._Categoria2aInfermita.HasValue &&
                    !this._CumuloInfermita.HasValue &&
                    !this._IndennitaAccompagnamentoAggiuntiva.HasValue &&
                    !this._IntegrazioneIndennitaAssistenza.HasValue &&
                    !this._AssegnoIntegrativo.HasValue &&
                    !this._PrivilegiataSuperinvaliditaIndennita.HasValue)
                    return true;

                return false;
            }

            public bool IsArticolo2Null()
            {
                if (!this._ScadenzaBenefici.HasValue &&
                    !this._PALConBenefici.HasValue &&
                    !this._ScadenzaIllimitata.HasValue)
                    return true;

                return false;
            }
        }

        public class DatiFondoPT
        {
            public DatiFondoPT()
            { }

            public DatiFondoPT(DateTime? _FinestraMobile, bool? _RequisitiAnte247, byte? _TrimesteRequisiti, int? _AnzianitaAnni, long? _CausaCessazione,
                bool? _PagamentoIndennitaIntegrativaSpeciale, bool? _IndennitaIntegrativaSpecialeConglobata, bool? _TrediciMensilita,
                DateTime? _DecorrenzaCalcolo, int? _SiglaCategoria, short? _CodiceSede, int? _Ncertificato, int? _NMesiRiscattati, int? _NMesiTotali,
                decimal? _PensioneAnnuaLorda, short? _ServizioUtileDirittoAA, short? _ServizioUtileDirittoMM, short? _ServizioUtileDirittoGG, int? _PrivilegiataSuperinvaliditaIndennita,
                int? _AssegnoIntegrativo, int? _IntegrazioneIndennitaAssistenza, int? _IndennitaAccompagnamentoAggiuntiva, int? _CumuloInfermita, int? _Categoria2aInfermita,
                int? _AssegnoCura, int? _IndennitaSpecialeAnnua, DateTime? _DataInzioBeneficioArt2, DateTime? _DataFineBeneficioArt2, DateTime? _DecorrenzaEconomica, short? annoRequisiti,
                bool? _DirittoIndennitaIntegrativaSpeciale, bool? _IntegrazioneMinimo, bool? _RiduzioneL537, bool? _IISAbbattimentoAnni, DateTime? _DecorrenzaSecondaria, bool? _OnereMEF,
                decimal? _RipartizioneInpdap, DateTime? scadenzaBenefici, decimal? palConBenefici, decimal? incrementoContrattuale, bool? scadenzaIllimitata,
                bool isOnereMefFromGpUgualeSI, short? _VVUtiliDiritto, short? _VVUtiliMisura, decimal? _PensioneAnnuaLorda707, bool? _TitolareAltraPensione,
                decimal? _CoefficienteTrasformazione, decimal? _PensioneAnnuaLorda214, decimal? rmsSenzaLegge33670QA, bool? isPensioneAnnuaLordaDaPrelievo, short tipologiaPensione, bool? isPensioneAnnuaLorda707DaPrelievo,
                decimal? _IndennitaIntegrativaSpecialeLorda, short? _ServizioUtileDirittoOIAA, short? _ServizioUtileDirittoOIMM, short? _ServizioUtileDirittoOIGG, short? _XFSFAAGO)
            {
                this._FinestraMobile = _FinestraMobile;
                this._RequisitiAnte247 = _RequisitiAnte247;
                this._TrimesteRequisiti = _TrimesteRequisiti;
                this._AnzianitaAnni = _AnzianitaAnni;
                this._CausaCessazione = _CausaCessazione;
                this._PagamentoIndennitaIntegrativaSpeciale = _PagamentoIndennitaIntegrativaSpeciale;
                this._IndennitaIntegrativaSpecialeConglobata = _IndennitaIntegrativaSpecialeConglobata;
                this._TrediciMensilita = _TrediciMensilita;
                this._DecorrenzaCalcolo = _DecorrenzaCalcolo;
                this._SiglaCategoria = _SiglaCategoria;
                this._CodiceSede = _CodiceSede;
                this._Ncertificato = _Ncertificato;
                this._NMesiRiscattati = _NMesiRiscattati;
                this._NMesiTotali = _NMesiTotali;
                this._PensioneAnnuaLorda = _PensioneAnnuaLorda;
                this._ServizioUtileDirittoAA = _ServizioUtileDirittoAA;
                this._ServizioUtileDirittoMM = _ServizioUtileDirittoMM;
                this._ServizioUtileDirittoGG = _ServizioUtileDirittoGG;
                this._PrivilegiataSuperinvaliditaIndennita = _PrivilegiataSuperinvaliditaIndennita;
                this._AssegnoIntegrativo = _AssegnoIntegrativo;
                this._IntegrazioneIndennitaAssistenza = _IntegrazioneIndennitaAssistenza;
                this._IndennitaAccompagnamentoAggiuntiva = _IndennitaAccompagnamentoAggiuntiva;
                this._CumuloInfermita = _CumuloInfermita;
                this._Categoria2aInfermita = _Categoria2aInfermita;
                this._AssegnoCura = _AssegnoCura;
                this._IndennitaSpecialeAnnua = _IndennitaSpecialeAnnua;
                this._DataInzioBeneficioArt2 = _DataInzioBeneficioArt2;
                this._DataFineBeneficioArt2 = _DataFineBeneficioArt2;
                this._DecorrenzaEconomica = _DecorrenzaEconomica;
                this._DirittoIndennitaIntegrativaSpeciale = _DirittoIndennitaIntegrativaSpeciale;
                this._IntegrazioneMinimo = _IntegrazioneMinimo;
                this._RiduzioneL537 = _RiduzioneL537;
                this._IISAbbattimentoAnni = _IISAbbattimentoAnni;
                this._DecorrenzaSecondaria = _DecorrenzaSecondaria;
                this._OnereMEF = _OnereMEF;
                this._RipartizioneInpdap = _RipartizioneInpdap;
                this._ScadenzaBenefici = scadenzaBenefici;
                this._PALConBenefici = palConBenefici;
                this._IncrementoContrattuale = incrementoContrattuale;
                this._ScadenzaIllimitata = scadenzaIllimitata;
                this._IsOnereMefFromGpUgualeSI = isOnereMefFromGpUgualeSI;
                this._VVUtiliDiritto = _VVUtiliDiritto;
                this._VVUtiliMisura = _VVUtiliMisura;
                this._PensioneAnnuaLorda707 = _PensioneAnnuaLorda707;
                this._TitolareAltraPensione = _TitolareAltraPensione;
                this._CoefficienteTrasformazione = _CoefficienteTrasformazione;
                this._PensioneAnnuaLorda214 = _PensioneAnnuaLorda214;
                this._RMSSenzaLegge33670QA = rmsSenzaLegge33670QA;
                this._IsPensioneAnnuaLordaDaPrelievo = isPensioneAnnuaLordaDaPrelievo;
                this._TipologiaPensione = tipologiaPensione;
                this._IsPensioneAnnuaLorda707DaPrelievo = isPensioneAnnuaLorda707DaPrelievo;
                this._IndennitaIntegrativaSpecialeLorda = _IndennitaIntegrativaSpecialeLorda;
                this._ServizioUtileDirittoOIAA = _ServizioUtileDirittoOIAA;
                this._ServizioUtileDirittoOIMM = _ServizioUtileDirittoOIMM;
                this._ServizioUtileDirittoOIGG = _ServizioUtileDirittoOIGG;
                this._XFSFAAGO = _XFSFAAGO;
            }

            #region private properties

            private long _IdFondo;
            private long? _IdRecordFondo;
            private DateTime? _FinestraMobile;
            private bool? _RequisitiAnte247;
            private byte? _TrimesteRequisiti;
            private int? _AnzianitaAnni;
            private long? _CausaCessazione;
            private bool? _PagamentoIndennitaIntegrativaSpeciale;
            private bool? _IndennitaIntegrativaSpecialeConglobata;
            private bool? _TrediciMensilita;
            private DateTime? _DecorrenzaCalcolo;
            private int? _SiglaCategoria;
            private short? _CodiceSede;
            private int? _Ncertificato;
            private int? _NMesiRiscattati;
            private int? _NMesiTotali;
            private decimal? _PensioneAnnuaLorda;
            private short? _ServizioUtileDirittoAA;
            private short? _ServizioUtileDirittoMM;
            private short? _ServizioUtileDirittoGG;
            private int? _PrivilegiataSuperinvaliditaIndennita;
            private int? _AssegnoIntegrativo;
            private int? _IntegrazioneIndennitaAssistenza;
            private int? _IndennitaAccompagnamentoAggiuntiva;
            private int? _CumuloInfermita;
            private int? _Categoria2aInfermita;
            private int? _AssegnoCura;
            private int? _IndennitaSpecialeAnnua;
            private DateTime? _DataInzioBeneficioArt2;
            private DateTime? _DataFineBeneficioArt2;
            private DateTime? _DecorrenzaEconomica;
            private short? _AnnoRequisiti;
            private bool? _DirittoIndennitaIntegrativaSpeciale;
            private bool? _IntegrazioneMinimo;
            private bool? _RiduzioneL537;
            private bool? _IISAbbattimentoAnni;
            private DateTime? _DecorrenzaSecondaria;
            private bool? _OnereMEF;
            private decimal? _RipartizioneInpdap;
            private DateTime? _ScadenzaBenefici;
            private decimal? _PALConBenefici;
            private decimal? _IncrementoContrattuale;
            private System.Nullable<bool> _ScadenzaIllimitata;
            private bool _IsOnereMefFromGpUgualeSI;
            private short? _VVUtiliDiritto;
            private short? _VVUtiliMisura;
            private decimal? _PensioneAnnuaLorda707;
            private bool? _TitolareAltraPensione;
            private decimal? _CoefficienteTrasformazione;
            private decimal? _PensioneAnnuaLorda214;
            private decimal? _RMSSenzaLegge33670QA;
            //ENG - PL Reversibilita 024
            private bool? _IsPensioneAnnuaLordaDaPrelievo;
            private short? _TipologiaPensione;
            //ENG - PL Reversibilita 024
            private bool? _IsPensioneAnnuaLorda707DaPrelievo;
            private decimal? _IndennitaIntegrativaSpecialeLorda;
            private short? _ServizioUtileDirittoOIAA;
            private short? _ServizioUtileDirittoOIMM;
            private short? _ServizioUtileDirittoOIGG;
            //ENG - RIC/TRF REVERSIBILITA 024
            private short? _XFSFAAGO;

            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }
            public DateTime? FinestraMobile { get { return _FinestraMobile; } set { _FinestraMobile = value; } }
            public bool? RequisitiAnte247 { get { return _RequisitiAnte247; } set { _RequisitiAnte247 = value; } }
            public byte? TrimesteRequisiti { get { return _TrimesteRequisiti; } set { _TrimesteRequisiti = value; } }
            public int? AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }
            public long? CausaCessazione { get { return _CausaCessazione; } set { _CausaCessazione = value; } }
            public bool? PagamentoIndennitaIntegrativaSpeciale { get { return _PagamentoIndennitaIntegrativaSpeciale; } set { _PagamentoIndennitaIntegrativaSpeciale = value; } }
            public bool? IndennitaIntegrativaSpecialeConglobata { get { return _IndennitaIntegrativaSpecialeConglobata; } set { _IndennitaIntegrativaSpecialeConglobata = value; } }
            public bool? TrediciMensilita { get { return _TrediciMensilita; } set { _TrediciMensilita = value; } }
            public DateTime? DecorrenzaCalcolo { get { return _DecorrenzaCalcolo; } set { _DecorrenzaCalcolo = value; } }
            public int? SiglaCategoria { get { return _SiglaCategoria; } set { _SiglaCategoria = value; } }
            public short? CodiceSede { get { return _CodiceSede; } set { _CodiceSede = value; } }
            public int? Ncertificato { get { return _Ncertificato; } set { _Ncertificato = value; } }
            public int? NMesiRiscattati { get { return _NMesiRiscattati; } set { _NMesiRiscattati = value; } }
            public int? NMesiTotali { get { return _NMesiTotali; } set { _NMesiTotali = value; } }
            public decimal? PensioneAnnuaLorda { get { return _PensioneAnnuaLorda; } set { _PensioneAnnuaLorda = value; } }
            public short? ServizioUtileDirittoAA { get { return _ServizioUtileDirittoAA; } set { _ServizioUtileDirittoAA = value; } }
            public short? ServizioUtileDirittoMM { get { return _ServizioUtileDirittoMM; } set { _ServizioUtileDirittoMM = value; } }
            public short? ServizioUtileDirittoGG { get { return _ServizioUtileDirittoGG; } set { _ServizioUtileDirittoGG = value; } }
            public int? PrivilegiataSuperinvaliditaIndennita { get { return _PrivilegiataSuperinvaliditaIndennita; } set { _PrivilegiataSuperinvaliditaIndennita = value; } }
            public int? AssegnoIntegrativo { get { return _AssegnoIntegrativo; } set { _AssegnoIntegrativo = value; } }
            public int? IntegrazioneIndennitaAssistenza { get { return _IntegrazioneIndennitaAssistenza; } set { _IntegrazioneIndennitaAssistenza = value; } }
            public int? IndennitaAccompagnamentoAggiuntiva { get { return _IndennitaAccompagnamentoAggiuntiva; } set { _IndennitaAccompagnamentoAggiuntiva = value; } }
            public int? CumuloInfermita { get { return _CumuloInfermita; } set { _CumuloInfermita = value; } }
            public int? Categoria2aInfermita { get { return _Categoria2aInfermita; } set { _Categoria2aInfermita = value; } }
            public int? AssegnoCura { get { return _AssegnoCura; } set { _AssegnoCura = value; } }
            public int? IndennitaSpecialeAnnua { get { return _IndennitaSpecialeAnnua; } set { _IndennitaSpecialeAnnua = value; } }
            public DateTime? DataInzioBeneficioArt2 { get { return _DataInzioBeneficioArt2; } set { _DataInzioBeneficioArt2 = value; } }
            public DateTime? DataFineBeneficioArt2 { get { return _DataFineBeneficioArt2; } set { _DataFineBeneficioArt2 = value; } }
            public DateTime? DecorrenzaEconomica { get { return _DecorrenzaEconomica; } set { _DecorrenzaEconomica = value; } }
            public short? AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public bool? DirittoIndennitaIntegrativaSpeciale { get { return _DirittoIndennitaIntegrativaSpeciale; } set { _DirittoIndennitaIntegrativaSpeciale = value; } }
            public bool? IntegrazioneMinimo { get { return _IntegrazioneMinimo; } set { _IntegrazioneMinimo = value; } }
            public bool? RiduzioneL537 { get { return _RiduzioneL537; } set { _RiduzioneL537 = value; } }
            public bool? IISAbbattimentoAnni { get { return _IISAbbattimentoAnni; } set { _IISAbbattimentoAnni = value; } }
            public DateTime? DecorrenzaSecondaria { get { return _DecorrenzaSecondaria; } set { _DecorrenzaSecondaria = value; } }
            public bool? OnereMEF { get { return _OnereMEF; } set { _OnereMEF = value; } }
            public decimal? RipartizioneInpdap { get { return _RipartizioneInpdap; } set { _RipartizioneInpdap = value; } }
            public DateTime? ScadenzaBenefici { get { return _ScadenzaBenefici; } set { _ScadenzaBenefici = value; } }
            public decimal? PALConBenefici { get { return _PALConBenefici; } set { _PALConBenefici = value; } }
            public decimal? IncrementoContrattuale { get { return _IncrementoContrattuale; } set { _IncrementoContrattuale = value; } }
            public System.Nullable<bool> ScadenzaIllimitata { get { return _ScadenzaIllimitata; } set { _ScadenzaIllimitata = value; } }
            public bool IsOnereMefFromGpUgualeSI { get { return _IsOnereMefFromGpUgualeSI; } set { _IsOnereMefFromGpUgualeSI = value; } }
            public short? VVUtiliDiritto { get { return _VVUtiliDiritto; } set { _VVUtiliDiritto = value; } }
            public short? VVUtiliMisura { get { return _VVUtiliMisura; } set { _VVUtiliMisura = value; } }
            public decimal? PensioneAnnuaLorda707 { get { return _PensioneAnnuaLorda707; } set { _PensioneAnnuaLorda707 = value; } }
            public bool? TitolareAltraPensione { get { return _TitolareAltraPensione; } set { _TitolareAltraPensione = value; } }
            public decimal? CoefficienteTrasformazione { get { return _CoefficienteTrasformazione; } set { _CoefficienteTrasformazione = value; } }
            public decimal? PensioneAnnuaLorda214 { get { return _PensioneAnnuaLorda214; } set { _PensioneAnnuaLorda214 = value; } }
            public decimal? RMSSenzaLegge33670QA { get { return _RMSSenzaLegge33670QA; } set { _RMSSenzaLegge33670QA = value; } }
            //ENG - PL Reversibilita 024
            public bool? IsPensioneAnnuaLordaDaPrelievo { get { return _IsPensioneAnnuaLordaDaPrelievo; } set { _IsPensioneAnnuaLordaDaPrelievo = value; } }
            public short? TipologiaPensione { get { return _TipologiaPensione; } set { _TipologiaPensione = value; } }
            //ENG - PL Reversibilita 024
            public bool? IsPensioneAnnuaLorda707DaPrelievo { get { return _IsPensioneAnnuaLorda707DaPrelievo; } set { _IsPensioneAnnuaLorda707DaPrelievo = value; } }
            public decimal? IndennitaIntegrativaSpecialeLorda { get { return _IndennitaIntegrativaSpecialeLorda; } set { _IndennitaIntegrativaSpecialeLorda = value; } }
            public short? ServizioUtileDirittoOIAA { get { return _ServizioUtileDirittoOIAA; } set { _ServizioUtileDirittoOIAA = value; } }
            public short? ServizioUtileDirittoOIMM { get { return _ServizioUtileDirittoOIMM; } set { _ServizioUtileDirittoOIMM = value; } }
            public short? ServizioUtileDirittoOIGG { get { return _ServizioUtileDirittoOIGG; } set { _ServizioUtileDirittoOIGG = value; } }
            //ENG - RIC/TRF REVERSIBILITA 024
            public short? XFSFAAGO { get { return _XFSFAAGO; } set { _XFSFAAGO = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoPT fondoPT = (DatiFondoPT)obj;
                try
                {
                    if (this._FinestraMobile != fondoPT._FinestraMobile ||
                        this._RequisitiAnte247 != fondoPT._RequisitiAnte247 ||
                        this._TrimesteRequisiti != fondoPT._TrimesteRequisiti ||
                        this._AnzianitaAnni != fondoPT._AnzianitaAnni ||
                        this._CausaCessazione != fondoPT._CausaCessazione ||
                        this._PagamentoIndennitaIntegrativaSpeciale != fondoPT._PagamentoIndennitaIntegrativaSpeciale ||
                        this._IndennitaIntegrativaSpecialeConglobata != fondoPT._IndennitaIntegrativaSpecialeConglobata ||
                        this._TrediciMensilita != fondoPT._TrediciMensilita ||
                        this._DecorrenzaCalcolo != fondoPT._DecorrenzaCalcolo ||
                        this._SiglaCategoria != fondoPT._SiglaCategoria ||
                        this._CodiceSede != fondoPT._CodiceSede ||
                        this._Ncertificato != fondoPT._Ncertificato ||
                        this._NMesiRiscattati != fondoPT._NMesiRiscattati ||
                        this._NMesiTotali != fondoPT._NMesiTotali ||
                        this._PensioneAnnuaLorda != fondoPT._PensioneAnnuaLorda ||
                        this._ServizioUtileDirittoAA != fondoPT._ServizioUtileDirittoAA ||
                        this._ServizioUtileDirittoMM != fondoPT._ServizioUtileDirittoMM ||
                        this._ServizioUtileDirittoGG != fondoPT._ServizioUtileDirittoGG ||
                        this._PrivilegiataSuperinvaliditaIndennita != fondoPT._PrivilegiataSuperinvaliditaIndennita ||
                        this._AssegnoIntegrativo != fondoPT._AssegnoIntegrativo ||
                        this._IntegrazioneIndennitaAssistenza != fondoPT._IntegrazioneIndennitaAssistenza ||
                        this._IndennitaAccompagnamentoAggiuntiva != fondoPT._IndennitaAccompagnamentoAggiuntiva ||
                        this._CumuloInfermita != fondoPT.CumuloInfermita ||
                        this._Categoria2aInfermita != fondoPT._Categoria2aInfermita ||
                        this._AssegnoCura != fondoPT._AssegnoCura ||
                        this._IndennitaSpecialeAnnua != fondoPT._IndennitaSpecialeAnnua ||
                        this._DataInzioBeneficioArt2 != fondoPT._DataInzioBeneficioArt2 ||
                        this._DataFineBeneficioArt2 != fondoPT._DataFineBeneficioArt2 ||
                        this._DecorrenzaEconomica != fondoPT._DecorrenzaEconomica ||
                        this._AnnoRequisiti != fondoPT._AnnoRequisiti ||
                        this._DirittoIndennitaIntegrativaSpeciale != fondoPT._DirittoIndennitaIntegrativaSpeciale ||
                        this._IntegrazioneMinimo != fondoPT._IntegrazioneMinimo ||
                        this._RiduzioneL537 != fondoPT._RiduzioneL537 ||
                        this._IISAbbattimentoAnni != fondoPT._IISAbbattimentoAnni ||
                        this._DecorrenzaSecondaria != fondoPT._DecorrenzaSecondaria ||
                        this._OnereMEF != fondoPT._OnereMEF ||
                        this._RipartizioneInpdap != fondoPT._RipartizioneInpdap ||
                        this._ScadenzaBenefici != fondoPT._ScadenzaBenefici ||
                        this._PALConBenefici != fondoPT._PALConBenefici ||
                        this._IncrementoContrattuale != fondoPT._IncrementoContrattuale ||
                        this._ScadenzaIllimitata != fondoPT._ScadenzaIllimitata ||
                        this._VVUtiliDiritto != fondoPT._VVUtiliDiritto ||
                        this._VVUtiliMisura != fondoPT._VVUtiliMisura ||
                        this._PensioneAnnuaLorda707 != fondoPT._PensioneAnnuaLorda707 ||
                        this._TitolareAltraPensione != fondoPT._TitolareAltraPensione ||
                        this._CoefficienteTrasformazione != fondoPT._CoefficienteTrasformazione ||
                        this._PensioneAnnuaLorda214 != fondoPT._PensioneAnnuaLorda214 ||
                        this._IndennitaIntegrativaSpecialeLorda != fondoPT._IndennitaIntegrativaSpecialeLorda ||
                        this._ServizioUtileDirittoOIAA != fondoPT._ServizioUtileDirittoOIAA ||
                        this._ServizioUtileDirittoOIMM != fondoPT._ServizioUtileDirittoOIMM ||
                        this._ServizioUtileDirittoOIGG != fondoPT._ServizioUtileDirittoOIGG ||
                        this._XFSFAAGO != fondoPT._XFSFAAGO)

                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool IsLegge460Null()
            {
                if (!_SiglaCategoria.HasValue && !_CodiceSede.HasValue &&
                    !_Ncertificato.HasValue && !_NMesiRiscattati.HasValue &&
                    !_NMesiTotali.HasValue && !_DecorrenzaSecondaria.HasValue)
                    return true;

                return false;
            }

            public bool IsArticolo2Null()
            {
                if (!_ScadenzaBenefici.HasValue && !_PALConBenefici.HasValue && !_ScadenzaIllimitata.HasValue)
                    return true;

                return false;
            }

            public bool IsPrivilegiataNull()
            {
                if (!this._IndennitaSpecialeAnnua.HasValue &&
                    !this._AssegnoCura.HasValue &&
                    !this._Categoria2aInfermita.HasValue &&
                    !this._CumuloInfermita.HasValue &&
                    !this._IndennitaAccompagnamentoAggiuntiva.HasValue &&
                    !this._IntegrazioneIndennitaAssistenza.HasValue &&
                    !this._AssegnoIntegrativo.HasValue &&
                    !this._PrivilegiataSuperinvaliditaIndennita.HasValue)
                    return true;

                return false;
            }
        }

        public class DatiFondoPI
        {
            #region private properties
            private long _Id;

            private long _IdFondo;

            private long? _IdRecordFondo;


            private char? _TipoPensione;

            private System.Nullable<System.DateTime> _DecorrenzaPrescrizione;

            private System.Nullable<byte> _TipoLiquidazione;

            private System.Nullable<byte> _DirittoQuoteFisse;

            private System.Nullable<decimal> _ImportoIIS;

            private System.Nullable<byte> _DirittoIIS;

            private System.Nullable<decimal> _PensioneFacoltativaMensile;

            private System.Nullable<decimal> _StipendioAnnuo;

            private System.Nullable<char> _IndennitaMedica;

            private System.Nullable<bool> _NonVedente;

            private string _NumeroMatricola;

            private System.Nullable<System.DateTime> _DecorrenzaPensioneEliminata;

            private string _Qualifica;

            private System.Nullable<short> _RiscattiAA;

            private System.Nullable<short> _RiscattiMM;

            private System.Nullable<short> _RiscattiGG;

            private System.Nullable<bool> _Requisiti247_243;

            private System.Nullable<byte> _NumeroTriSemRequisiti;

            private System.Nullable<short> _AnnoRequisiti;

            private System.Nullable<int> _AnzianitaAnni;

            private System.Nullable<short> _ServizioNonUtileAA;

            private System.Nullable<short> _ServizioNonUtileMM;

            private System.Nullable<short> _ServizioNonUtileGG;

            private System.Nullable<byte> _Livello;

            private System.Nullable<short> _SettimaneMaggiorazione;

            private System.Nullable<short> _SettimaneEsclusive;

            private System.Nullable<short> _SettimaneINPDAI;

            private string _CodiceCategoria;

            private System.Nullable<short> _Sede;

            private System.Nullable<int> _Certificato;

            private System.Nullable<decimal> _StipendioBase;

            private System.Nullable<char> _AttCon;

            private System.Nullable<decimal> _PercentualeCapitalizzazione;

            private System.Nullable<char> _CodiceMaggiorazione;

            private System.Nullable<decimal> _PensComplRiv1_95;

            private decimal? _RMSQuotaA;

            private decimal? _RMSQuotaB;

            private short? _NSettimaneQuotaA;

            private short? _NSettimaneQuotaB;

            private decimal? _IncrementoDPR346;

            private string _TipoRegolamento;

            private decimal? _Ass7_62;

            private decimal? _AssPers;

            private string _Scatti;

            public short? _SedeServ;

            public short? _Fisse;

            private System.Nullable<byte> _SemaforoRecord;
            #endregion private properties

            #region public properties
            public long Id { get { return _Id; } set { _Id = value; } }
            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }

            public long? IdRecordFondo { get { return _IdRecordFondo; } set { _IdRecordFondo = value; } }

            public char? TipoPensione { get { return _TipoPensione; } set { _TipoPensione = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPrescrizione { get { return _DecorrenzaPrescrizione; } set { _DecorrenzaPrescrizione = value; } }

            public System.Nullable<byte> TipoLiquidazione { get { return _TipoLiquidazione; } set { _TipoLiquidazione = value; } }

            public System.Nullable<byte> DirittoQuoteFisse { get { return _DirittoQuoteFisse; } set { _DirittoQuoteFisse = value; } }

            public System.Nullable<decimal> ImportoIIS { get { return _ImportoIIS; } set { _ImportoIIS = value; } }

            public System.Nullable<byte> DirittoIIS { get { return _DirittoIIS; } set { _DirittoIIS = value; } }

            public System.Nullable<decimal> PensioneFacoltativaMensile { get { return _PensioneFacoltativaMensile; } set { _PensioneFacoltativaMensile = value; } }

            public System.Nullable<decimal> StipendioAnnuo { get { return _StipendioAnnuo; } set { _StipendioAnnuo = value; } }

            public System.Nullable<char> IndennitaMedica { get { return _IndennitaMedica; } set { _IndennitaMedica = value; } }

            public System.Nullable<bool> NonVedente { get { return _NonVedente; } set { _NonVedente = value; } }

            public string NumeroMatricola { get { return _NumeroMatricola; } set { _NumeroMatricola = value; } }

            public System.Nullable<System.DateTime> DecorrenzaPensioneEliminata { get { return _DecorrenzaPensioneEliminata; } set { _DecorrenzaPensioneEliminata = value; } }

            public string Qualifica { get { return _Qualifica; } set { _Qualifica = value; } }

            public System.Nullable<short> RiscattiAA { get { return _RiscattiAA; } set { _RiscattiAA = value; } }

            public System.Nullable<short> RiscattiMM { get { return _RiscattiMM; } set { _RiscattiMM = value; } }

            public System.Nullable<short> RiscattiGG { get { return _RiscattiGG; } set { _RiscattiGG = value; } }

            public System.Nullable<bool> Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }

            public System.Nullable<byte> NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }

            public System.Nullable<short> AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }

            public System.Nullable<int> AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }

            public System.Nullable<short> ServizioNonUtileAA { get { return _ServizioNonUtileAA; } set { _ServizioNonUtileAA = value; } }

            public System.Nullable<short> ServizioNonUtileMM { get { return _ServizioNonUtileMM; } set { _ServizioNonUtileMM = value; } }

            public System.Nullable<short> ServizioNonUtileGG { get { return _ServizioNonUtileGG; } set { _ServizioNonUtileGG = value; } }

            public System.Nullable<byte> Livello { get { return _Livello; } set { _Livello = value; } }

            public System.Nullable<short> SettimaneMaggiorazione { get { return _SettimaneMaggiorazione; } set { _SettimaneMaggiorazione = value; } }

            public System.Nullable<short> SettimaneEsclusive { get { return _SettimaneEsclusive; } set { _SettimaneEsclusive = value; } }

            public System.Nullable<short> SettimaneINPDAI { get { return _SettimaneINPDAI; } set { _SettimaneINPDAI = value; } }

            public string CodiceCategoria { get { return _CodiceCategoria; } set { _CodiceCategoria = value; } }

            public System.Nullable<short> Sede { get { return _Sede; } set { _Sede = value; } }

            public System.Nullable<int> Certificato { get { return _Certificato; } set { _Certificato = value; } }

            public System.Nullable<decimal> StipendioBase { get { return _StipendioBase; } set { _StipendioBase = value; } }

            public System.Nullable<char> AttCon { get { return _AttCon; } set { _AttCon = value; } }

            public System.Nullable<decimal> PercentualeCapitalizzazione { get { return _PercentualeCapitalizzazione; } set { _PercentualeCapitalizzazione = value; } }

            public System.Nullable<char> CodiceMaggiorazione { get { return _CodiceMaggiorazione; } set { _CodiceMaggiorazione = value; } }

            public System.Nullable<decimal> PensComplRiv1_95 { get { return _PensComplRiv1_95; } set { _PensComplRiv1_95 = value; } }

            public decimal? RMSQuotaA { get { return _RMSQuotaA; } set { _RMSQuotaA = value; } }

            public decimal? RMSQuotaB { get { return _RMSQuotaB; } set { _RMSQuotaB = value; } }

            public short? NSettimaneQuotaA { get { return _NSettimaneQuotaA; } set { _NSettimaneQuotaA = value; } }

            public short? NSettimaneQuotaB { get { return _NSettimaneQuotaB; } set { _NSettimaneQuotaB = value; } }

            public decimal? IncrementoDPR346 { get { return _IncrementoDPR346; } set { _IncrementoDPR346 = value; } }

            public string TipoRegolamento { get { return _TipoRegolamento; } set { _TipoRegolamento = value; } }

            public decimal? Ass7_62 { get { return _Ass7_62; } set { _Ass7_62 = value; } }

            public decimal? AssPers { get { return _AssPers; } set { _AssPers = value; } }

            public string Scatti { get { return _Scatti; } set { _Scatti = value; } }

            public short? SedeServ { get { return _SedeServ; } set { _SedeServ = value; } }

            public short? Fisse { get { return _Fisse; } set { _Fisse = value; } }

            public System.Nullable<byte> SemaforoRecord { get { return _SemaforoRecord; } set { _SemaforoRecord = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoPI fondoPI = (DatiFondoPI)obj;
                try
                {
                    if (this._AnnoRequisiti != fondoPI._AnnoRequisiti ||
                        this._AnzianitaAnni != fondoPI._AnzianitaAnni ||
                        this._DecorrenzaPensioneEliminata != fondoPI._DecorrenzaPensioneEliminata ||
                        this._DecorrenzaPrescrizione != fondoPI._DecorrenzaPrescrizione ||
                        this._DirittoIIS != fondoPI._DirittoIIS ||
                        this._DirittoQuoteFisse != fondoPI._DirittoQuoteFisse ||
                        this._ImportoIIS != fondoPI._ImportoIIS ||
                        this._IndennitaMedica != fondoPI._IndennitaMedica ||
                        this._NonVedente != fondoPI._NonVedente ||
                        this._NumeroMatricola != fondoPI._NumeroMatricola ||
                        this._NumeroTriSemRequisiti != fondoPI._NumeroTriSemRequisiti ||
                        this._PensioneFacoltativaMensile != fondoPI._PensioneFacoltativaMensile ||
                        this._PercentualeCapitalizzazione != fondoPI._PercentualeCapitalizzazione ||
                        this._Qualifica != fondoPI._Qualifica ||
                        this._Requisiti247_243 != fondoPI._Requisiti247_243 ||
                        this._RiscattiAA != fondoPI._RiscattiAA ||
                        this._RiscattiMM != fondoPI._RiscattiMM ||
                        this._RiscattiGG != fondoPI._RiscattiGG ||
                        this._StipendioAnnuo != fondoPI._StipendioAnnuo ||
                        this._TipoLiquidazione != fondoPI._TipoLiquidazione ||
                        this._TipoPensione != fondoPI._TipoPensione ||
                        this._ServizioNonUtileAA != fondoPI._ServizioNonUtileAA ||
                        this._ServizioNonUtileMM != fondoPI._ServizioNonUtileMM ||
                        this._ServizioNonUtileGG != fondoPI._ServizioNonUtileGG ||
                        this._Livello != fondoPI._Livello ||
                        this._SettimaneMaggiorazione != fondoPI._SettimaneMaggiorazione ||
                        this._SettimaneEsclusive != fondoPI._SettimaneEsclusive ||
                        this._SettimaneINPDAI != fondoPI._SettimaneINPDAI ||
                        this._CodiceCategoria != fondoPI._CodiceCategoria ||
                        this._Sede != fondoPI._Sede ||
                        this._Certificato != fondoPI._Certificato ||
                        this._StipendioBase != fondoPI._StipendioBase ||
                        this._AttCon != fondoPI._AttCon ||
                        this._CodiceMaggiorazione != fondoPI._CodiceMaggiorazione ||
                        this._PensComplRiv1_95 != fondoPI._PensComplRiv1_95 ||
                        this._RMSQuotaA != fondoPI._RMSQuotaA ||
                        this._RMSQuotaB != fondoPI._RMSQuotaB ||
                        this._NSettimaneQuotaA != fondoPI._NSettimaneQuotaA ||
                        this._NSettimaneQuotaB != fondoPI._NSettimaneQuotaB
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoGAS
        {
            public DatiFondoGAS()
            { }

            public DatiFondoGAS(System.Nullable<System.DateTime> decorrenzaOriginariaPensione, System.Nullable<byte> etaMaturazioneRequisiti, System.Nullable<System.DateTime> decorrenzaDatiAgo,
                System.Nullable<byte> codiceTipoLiquidazione, System.Nullable<short> settimaneAnzianitaEsclusiva, System.Nullable<decimal> importoContributiEsclusivi, System.Nullable<decimal> contribuzioneEsclusiva, System.Nullable<decimal> contributiTotaliSupplementoDPR143271,
                System.Nullable<decimal> contribuzioneEsclusivaDPR143271, System.Nullable<short> mesiUtiliIndennitaAggiuntiva, System.Nullable<short> mesiNonUtiliIndennitaAggiuntiva, System.Nullable<short> servizioUtileIndennitaAggiuntiva,
                System.Nullable<decimal> retribuzione, System.Nullable<decimal> importo, System.Nullable<bool> codicePensioneRidotta, System.Nullable<decimal> conguaglio,
                System.Nullable<System.DateTime> decorrenzaValiditaDati, System.Nullable<short> mesiAnte46, System.Nullable<short> anzianitaUtileDal46, System.Nullable<bool> codiceDimissioni,
                System.Nullable<short> percentualeRiduzione, System.Nullable<System.DateTime> decorrenzaTeorica, string ditta, string convenzione,
                System.Nullable<bool> requisiti247_243, System.Nullable<byte> numeroTriSemRequisiti, System.Nullable<short> annoRequisiti, System.Nullable<int> anzianitaAnni,
                System.Nullable<decimal> cCTotaliArt14, System.Nullable<System.DateTime> decDPCM, System.Nullable<decimal> rMSArt14, System.Nullable<decimal> rMSSent72,
                System.Nullable<decimal> cCTotaliArt11, System.Nullable<decimal> cCEsclusivaArt11, System.Nullable<System.DateTime> sospensioneAGO, System.Nullable<int> anniDifferimento, System.Nullable<char> codiceSpecificoAgo)
            {
                this._DecorrenzaOriginariaPensione = decorrenzaOriginariaPensione;
                this._EtaMaturazioneRequisiti = etaMaturazioneRequisiti;
                this._DecorrenzaDatiAgo = decorrenzaDatiAgo;
                this._CodiceTipoLiquidazione = codiceTipoLiquidazione;
                this._SettimaneAnzianitaEsclusiva = settimaneAnzianitaEsclusiva;
                this._ImportoContributiEsclusivi = importoContributiEsclusivi;
                this._ContribuzioneEsclusiva = contribuzioneEsclusiva;
                this._ContributiTotaliSupplementoDPR143271 = contributiTotaliSupplementoDPR143271;
                this._ContribuzioneEsclusivaDPR143271 = contribuzioneEsclusivaDPR143271;
                this._MesiUtiliIndennitaAggiuntiva = mesiUtiliIndennitaAggiuntiva;
                this._MesiNonUtiliIndennitaAggiuntiva = mesiNonUtiliIndennitaAggiuntiva;
                this._ServizioUtileIndennitaAggiuntiva = servizioUtileIndennitaAggiuntiva;
                this._Retribuzione = retribuzione;
                this._Importo = importo;
                this._CodicePensioneRidotta = codicePensioneRidotta;
                this._Conguaglio = conguaglio;
                this._DecorrenzaValiditaDati = decorrenzaValiditaDati;
                this._MesiAnte46 = mesiAnte46;
                this._AnzianitaUtileDal46 = anzianitaUtileDal46;
                this._CodiceDimissioni = codiceDimissioni;
                this._PercentualeRiduzione = percentualeRiduzione;
                this._DecorrenzaTeorica = decorrenzaTeorica;
                this._Ditta = ditta;
                this._Convenzione = convenzione;
                this._Requisiti247_243 = requisiti247_243;
                this._NumeroTriSemRequisiti = numeroTriSemRequisiti;
                this._AnnoRequisiti = annoRequisiti;
                this._AnzianitaAnni = anzianitaAnni;
                this._CCTotaliArt14 = cCTotaliArt14;
                this._DecDPCM = decDPCM;
                this._RMSArt14 = rMSArt14;
                this._RMSSent72 = rMSSent72;
                this._CCTotaliArt11 = cCTotaliArt11;
                this._CCEsclusivaArt11 = cCEsclusivaArt11;
                this._SospensioneAGO = sospensioneAGO;
                this._AnniDifferimento = anniDifferimento;
                this._CodiceSpecificoAgo = codiceSpecificoAgo;
            }

            #region private properties

            private long _IdFondo;
            private System.Nullable<System.DateTime> _DecorrenzaOriginariaPensione;
            private System.Nullable<byte> _EtaMaturazioneRequisiti;
            private System.Nullable<System.DateTime> _DecorrenzaDatiAgo;
            private System.Nullable<byte> _CodiceTipoLiquidazione;
            private System.Nullable<short> _SettimaneAnzianitaEsclusiva;
            private System.Nullable<decimal> _ImportoContributiEsclusivi;
            private System.Nullable<decimal> _ContribuzioneEsclusiva;
            private System.Nullable<decimal> _ContributiTotaliSupplementoDPR143271;
            private System.Nullable<decimal> _ContribuzioneEsclusivaDPR143271;
            private System.Nullable<short> _MesiUtiliIndennitaAggiuntiva;
            private System.Nullable<short> _MesiNonUtiliIndennitaAggiuntiva;
            private System.Nullable<short> _ServizioUtileIndennitaAggiuntiva;
            private System.Nullable<decimal> _Retribuzione;
            private System.Nullable<decimal> _Importo;
            private System.Nullable<bool> _CodicePensioneRidotta;
            private System.Nullable<decimal> _Conguaglio;
            private System.Nullable<System.DateTime> _DecorrenzaValiditaDati;
            private System.Nullable<short> _MesiAnte46;
            private System.Nullable<short> _AnzianitaUtileDal46;
            private System.Nullable<bool> _CodiceDimissioni;
            private System.Nullable<short> _PercentualeRiduzione;
            private System.Nullable<System.DateTime> _DecorrenzaTeorica;
            private string _Ditta;
            private string _Convenzione;
            private System.Nullable<bool> _Requisiti247_243;
            private System.Nullable<byte> _NumeroTriSemRequisiti;
            private System.Nullable<short> _AnnoRequisiti;
            private System.Nullable<int> _AnzianitaAnni;
            private System.Nullable<decimal> _CCTotaliArt14;
            private System.Nullable<System.DateTime> _DecDPCM;
            private System.Nullable<decimal> _RMSArt14;
            private System.Nullable<decimal> _RMSSent72;
            private System.Nullable<decimal> _CCTotaliArt11;
            private System.Nullable<decimal> _CCEsclusivaArt11;

            private System.Nullable<System.DateTime> _SospensioneAGO;
            private System.Nullable<int> _AnniDifferimento;
            private System.Nullable<char> _CodiceSpecificoAgo;
            #endregion private properties

            #region public properties

            public long IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }
            public System.Nullable<System.DateTime> DecorrenzaOriginariaPensione { get { return _DecorrenzaOriginariaPensione; } set { _DecorrenzaOriginariaPensione = value; } }
            public System.Nullable<byte> EtaMaturazioneRequisiti { get { return _EtaMaturazioneRequisiti; } set { _EtaMaturazioneRequisiti = value; } }
            public System.Nullable<System.DateTime> DecorrenzaDatiAgo { get { return _DecorrenzaDatiAgo; } set { _DecorrenzaDatiAgo = value; } }
            public System.Nullable<byte> CodiceTipoLiquidazione { get { return _CodiceTipoLiquidazione; } set { _CodiceTipoLiquidazione = value; } }
            public System.Nullable<short> SettimaneAnzianitaEsclusiva { get { return _SettimaneAnzianitaEsclusiva; } set { _SettimaneAnzianitaEsclusiva = value; } }
            public System.Nullable<decimal> ImportoContributiEsclusivi { get { return _ImportoContributiEsclusivi; } set { _ImportoContributiEsclusivi = value; } }
            public System.Nullable<decimal> ContribuzioneEsclusiva { get { return _ContribuzioneEsclusiva; } set { _ContribuzioneEsclusiva = value; } }
            public System.Nullable<decimal> ContributiTotaliSupplementoDPR143271 { get { return _ContributiTotaliSupplementoDPR143271; } set { _ContributiTotaliSupplementoDPR143271 = value; } }
            public System.Nullable<decimal> ContribuzioneEsclusivaDPR143271 { get { return _ContribuzioneEsclusivaDPR143271; } set { _ContribuzioneEsclusivaDPR143271 = value; } }
            public System.Nullable<short> MesiUtiliIndennitaAggiuntiva { get { return _MesiUtiliIndennitaAggiuntiva; } set { _MesiUtiliIndennitaAggiuntiva = value; } }
            public System.Nullable<short> MesiNonUtiliIndennitaAggiuntiva { get { return _MesiNonUtiliIndennitaAggiuntiva; } set { _MesiNonUtiliIndennitaAggiuntiva = value; } }
            public System.Nullable<short> ServizioUtileIndennitaAggiuntiva { get { return _ServizioUtileIndennitaAggiuntiva; } set { _ServizioUtileIndennitaAggiuntiva = value; } }
            public System.Nullable<decimal> Retribuzione { get { return _Retribuzione; } set { _Retribuzione = value; } }
            public System.Nullable<decimal> Importo { get { return _Importo; } set { _Importo = value; } }
            public System.Nullable<bool> CodicePensioneRidotta { get { return _CodicePensioneRidotta; } set { _CodicePensioneRidotta = value; } }
            public System.Nullable<decimal> Conguaglio { get { return _Conguaglio; } set { _Conguaglio = value; } }
            public System.Nullable<System.DateTime> DecorrenzaValiditaDati { get { return _DecorrenzaValiditaDati; } set { _DecorrenzaValiditaDati = value; } }
            public System.Nullable<short> MesiAnte46 { get { return _MesiAnte46; } set { _MesiAnte46 = value; } }
            public System.Nullable<short> AnzianitaUtileDal46 { get { return _AnzianitaUtileDal46; } set { _AnzianitaUtileDal46 = value; } }
            public System.Nullable<bool> CodiceDimissioni { get { return _CodiceDimissioni; } set { _CodiceDimissioni = value; } }
            public System.Nullable<short> PercentualeRiduzione { get { return _PercentualeRiduzione; } set { _PercentualeRiduzione = value; } }
            public System.Nullable<System.DateTime> DecorrenzaTeorica { get { return _DecorrenzaTeorica; } set { _DecorrenzaTeorica = value; } }
            public string Ditta { get { return _Ditta; } set { _Ditta = value; } }
            public string Convenzione { get { return _Convenzione; } set { _Convenzione = value; } }
            public System.Nullable<bool> Requisiti247_243 { get { return _Requisiti247_243; } set { _Requisiti247_243 = value; } }
            public System.Nullable<byte> NumeroTriSemRequisiti { get { return _NumeroTriSemRequisiti; } set { _NumeroTriSemRequisiti = value; } }
            public System.Nullable<short> AnnoRequisiti { get { return _AnnoRequisiti; } set { _AnnoRequisiti = value; } }
            public System.Nullable<int> AnzianitaAnni { get { return _AnzianitaAnni; } set { _AnzianitaAnni = value; } }
            public System.Nullable<decimal> CCTotaliArt14 { get { return _CCTotaliArt14; } set { _CCTotaliArt14 = value; } }
            public System.Nullable<System.DateTime> DecDPCM { get { return _DecDPCM; } set { _DecDPCM = value; } }
            public System.Nullable<decimal> RMSArt14 { get { return _RMSArt14; } set { _RMSArt14 = value; } }
            public System.Nullable<decimal> RMSSent72 { get { return _RMSSent72; } set { _RMSSent72 = value; } }
            public System.Nullable<decimal> CCTotaliArt11 { get { return _CCTotaliArt11; } set { _CCTotaliArt11 = value; } }
            public System.Nullable<decimal> CCEsclusivaArt11 { get { return _CCEsclusivaArt11; } set { _CCEsclusivaArt11 = value; } }
            public System.Nullable<System.DateTime> SospensioneAGO { get { return _SospensioneAGO; } set { _SospensioneAGO = value; } }
            public System.Nullable<int> AnniDifferimento { get { return _AnniDifferimento; } set { _AnniDifferimento = value; } }
            public System.Nullable<char> CodiceSpecificoAgo { get { return _CodiceSpecificoAgo; } set { _CodiceSpecificoAgo = value; } }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoGAS fondoGAS = (DatiFondoGAS)obj;
                try
                {
                    if (this._DecorrenzaOriginariaPensione != fondoGAS._DecorrenzaOriginariaPensione ||
                       this._EtaMaturazioneRequisiti != fondoGAS._EtaMaturazioneRequisiti ||
                       this._DecorrenzaDatiAgo != fondoGAS._DecorrenzaDatiAgo ||
                       this._CodiceTipoLiquidazione != fondoGAS._CodiceTipoLiquidazione ||
                       this._SettimaneAnzianitaEsclusiva != fondoGAS._SettimaneAnzianitaEsclusiva ||
                       this._ImportoContributiEsclusivi != fondoGAS._ImportoContributiEsclusivi ||
                       this._ContribuzioneEsclusiva != fondoGAS._ContribuzioneEsclusiva ||
                       this._ContributiTotaliSupplementoDPR143271 != fondoGAS._ContributiTotaliSupplementoDPR143271 ||
                       this._ContribuzioneEsclusivaDPR143271 != fondoGAS._ContribuzioneEsclusivaDPR143271 ||
                       this._MesiUtiliIndennitaAggiuntiva != fondoGAS._MesiUtiliIndennitaAggiuntiva ||
                       this._MesiNonUtiliIndennitaAggiuntiva != fondoGAS._MesiNonUtiliIndennitaAggiuntiva ||
                       this._ServizioUtileIndennitaAggiuntiva != fondoGAS._ServizioUtileIndennitaAggiuntiva ||
                       this._Retribuzione != fondoGAS._Retribuzione ||
                       this._Importo != fondoGAS._Importo ||
                       this._CodicePensioneRidotta != fondoGAS._CodicePensioneRidotta ||
                       this._Conguaglio != fondoGAS._Conguaglio ||
                       this._DecorrenzaValiditaDati != fondoGAS._DecorrenzaValiditaDati ||
                       this._MesiAnte46 != fondoGAS._MesiAnte46 ||
                       this._AnzianitaUtileDal46 != fondoGAS._AnzianitaUtileDal46 ||
                       this._CodiceDimissioni != fondoGAS._CodiceDimissioni ||
                       this._PercentualeRiduzione != fondoGAS._PercentualeRiduzione ||
                       this._DecorrenzaTeorica != fondoGAS._DecorrenzaTeorica ||
                       this._Ditta != fondoGAS._Ditta ||
                       this._Convenzione != fondoGAS._Convenzione ||
                       this._Requisiti247_243 != fondoGAS._Requisiti247_243 ||
                       this._NumeroTriSemRequisiti != fondoGAS._NumeroTriSemRequisiti ||
                       this._AnnoRequisiti != fondoGAS._AnnoRequisiti ||
                       this._AnzianitaAnni != fondoGAS._AnzianitaAnni ||
                       this._CCTotaliArt14 != fondoGAS.CCTotaliArt14 ||
                       this._DecDPCM != fondoGAS.DecDPCM ||
                       this._RMSArt14 != fondoGAS.RMSArt14 ||
                       this._RMSSent72 != fondoGAS.RMSSent72 ||
                       this._CCTotaliArt11 != fondoGAS.CCTotaliArt11 ||
                       this._CCEsclusivaArt11 != fondoGAS.CCEsclusivaArt11)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoCL
        {
            public decimal? ImportoAltraPensione { get; set; }
            public bool? CodicePensioneSenzaRequisiti { get; set; }
            public short? AnniDifferimento { get; set; }
            public byte? EtaPerfezionamentoRequisiti { get; set; }
            public DateTime? DataPerfezionamentoRequisiti { get; set; }
            public char? ContrProvv { get; set; }

            public override bool Equals(object obj)
            {
                DatiFondoCL fondoCL = (DatiFondoCL)obj;
                try
                {
                    if (this.ImportoAltraPensione != fondoCL.ImportoAltraPensione ||
                       this.CodicePensioneSenzaRequisiti != fondoCL.CodicePensioneSenzaRequisiti ||
                       this.AnniDifferimento != fondoCL.AnniDifferimento ||
                       this.EtaPerfezionamentoRequisiti != fondoCL.EtaPerfezionamentoRequisiti ||
                       this.DataPerfezionamentoRequisiti != fondoCL.DataPerfezionamentoRequisiti ||
                       this.ContrProvv != fondoCL.ContrProvv)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoDZ
        {
            #region public properties
            public long IdFondo { get; set; }
            public long? IdRecordFondo { get; set; }
            public short? RiscattiAA { get; set; }
            public short? RiscattiMM { get; set; }
            public bool? CodiceCaroPane { get; set; }
            public short? CodiceBenefici { get; set; }
            public bool? CodiceDZ { get; set; }
            public short? MaggiorazionePensionePrivilegiataAA { get; set; }
            public short? MaggiorazionePensionePrivilegiataMM { get; set; }
            public bool? CodiceEsodo { get; set; }
            public short? MaggiorazioneAnzianitaEsodoAA { get; set; }
            public short? MaggiorazioneAnzianitaEsodoMM { get; set; }
            public decimal? RetribuzioneAlNettoBeneficiEsodo { get; set; }
            public DateTime? DataCessazioneServizio { get; set; }
            public short? ClasseAnte50 { get; set; }
            public int? PercentualeLiquidazionePensione { get; set; }
            public decimal? PensioneBaseAnnua { get; set; }
            public string Ditta { get; set; }
            public DateTime? Sospensione { get; set; }
            public bool? Requisiti247_243 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }
            public bool? RaggiuntoRequisiti311297 { get; set; }
            public DateTime? DecorrenzaSecondaria { get; set; }
            public DateTime? DecorrenzaValidita { get; set; }
            #endregion public properties
        }

        public class DatiFondoES
        {
            #region public properties
            public long _IdFondo { get; set; }

            public char? TipoPensione { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaRequisitiAnzianita { get; set; }

            public System.Nullable<short> EtaRequisiti { get; set; }

            public System.Nullable<System.DateTime> Decorrenza { get; set; }

            public System.Nullable<byte> TipoLiquidazione { get; set; }

            public System.Nullable<decimal> ContributiDifferimentoQuota { get; set; }

            public System.Nullable<int> NSettimaneLegge37758Art24 { get; set; }

            public System.Nullable<int> NSettimaneLegge37758Art57 { get; set; }

            public System.Nullable<decimal> ImportoContributiLegge37758Art24 { get; set; }

            public System.Nullable<decimal> ImportoContributiLegge37758Art57 { get; set; }

            public System.Nullable<decimal> ImportoContributiLegge143271Art14 { get; set; }

            public System.Nullable<decimal> BaseAltraPensione { get; set; }

            public System.Nullable<short> CategoriaAltraPensione { get; set; }

            public System.Nullable<int> NSettimaneSenzaLegge33670Art24QuotaA { get; set; }

            public System.Nullable<int> NSettimaneSenzaLegge33670Art57QuotaA { get; set; }

            public System.Nullable<decimal> ContributiTotaliSenzaLegge33670 { get; set; }

            public System.Nullable<decimal> ContributiSupplementoLegge143271 { get; set; }

            public System.Nullable<decimal> ContributiSupplementoAgo { get; set; }

            public System.Nullable<decimal> ContributiSupplementoFondo { get; set; }

            public System.Nullable<byte> CodiceAzienda { get; set; }

            public System.Nullable<int> MesiRiscatti { get; set; }

            public System.Nullable<decimal> UnaTantum6901 { get; set; }

            public System.Nullable<decimal> PensioneFondoAl67 { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaArticolo24 { get; set; }

            public System.Nullable<decimal> ContributiLegge37758Art24 { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaLegge37758Art57Pre67Periodo1 { get; set; }

            public System.Nullable<decimal> ContributiLegge37758Art57Periodo1 { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaLegge37758Art57Pre67Periodo2 { get; set; }

            public System.Nullable<decimal> ContributiLegge37758Art57Periodo2 { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaLegge37758Art57Pre67Periodo3 { get; set; }

            public System.Nullable<decimal> ContributiLegge37758Art57Periodo3 { get; set; }

            public System.Nullable<decimal> ImportoInPagamentoPre67 { get; set; }

            public System.Nullable<char> CodicePensioneInPagamentoPre67 { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaDati { get; set; }

            public System.Nullable<byte> CodiceOnOff { get; set; }

            public System.Nullable<byte> ClassePensioneAnte50 { get; set; }

            public System.Nullable<int> MMServizioUtile { get; set; }

            public System.Nullable<decimal> Retribuzione { get; set; }

            public System.Nullable<int> MMServizioUtile2 { get; set; }

            public System.Nullable<decimal> Retribuzione2 { get; set; }

            public System.Nullable<int> MMServizioUtile3 { get; set; }

            public System.Nullable<decimal> Retribuzione3 { get; set; }

            public System.Nullable<int> MMServizioUtile4 { get; set; }

            public System.Nullable<decimal> Retribuzione4 { get; set; }

            public string MaggiorazioneInvalidita { get; set; }

            public System.Nullable<bool> AnnoUtile { get; set; }

            public System.Nullable<byte> Articolo58 { get; set; }

            public System.Nullable<bool> Articolo59 { get; set; }

            public System.Nullable<byte> CodiciRetributivi { get; set; }

            public string CodiceEsattoria { get; set; }

            public System.Nullable<bool> CodiceDz { get; set; }

            public System.Nullable<bool> Optanti { get; set; }

            public System.Nullable<bool> MaggiorazionePrivilegiata { get; set; }

            public System.Nullable<byte> Promiscui { get; set; }

            public System.Nullable<bool> Saltuari { get; set; }

            public System.Nullable<decimal> IntegrazioneArticolo11 { get; set; }

            public System.Nullable<int> AnniDifferimento { get; set; }

            public System.Nullable<char> ConvenzioneInternazionale { get; set; }

            public System.Nullable<int> AnniRiscatti { get; set; }

            public System.Nullable<byte> EtaMaturazioneRequisiti { get; set; }

            public System.Nullable<int> SettimaneArt24QA { get; set; }

            public System.Nullable<int> SettimaneArt24QB { get; set; }

            public System.Nullable<System.DateTime> Sospensione { get; set; }

            public System.Nullable<char> CodiceSpecificoAgo { get; set; }

            public System.Nullable<System.DateTime> DecorrenzaTeorica { get; set; }

            public System.Nullable<byte> CodiceTipoLiquidazione { get; set; }

            public System.Nullable<bool> Requisiti247_243 { get; set; }

            public System.Nullable<byte> NumeroTriSemRequisiti { get; set; }

            public System.Nullable<short> AnnoRequisiti { get; set; }

            public System.Nullable<int> AnzianitaAnni { get; set; }

            public System.Nullable<System.DateTime> DecDPCM { get; set; }

            public System.Nullable<decimal> RmsDPCM { get; set; }

            public System.Nullable<decimal> RMSSent72 { get; set; }

            public System.Nullable<decimal> CCArt14SenzaLegge33670 { get; set; }

            public System.Nullable<int> NSettimaneAnzianitaTotaliSenzaLegge33670 { get; set; }

            public System.Nullable<decimal> RMSSenzaLegge33670QA { get; set; }

            public System.Nullable<decimal> RMSSenzaLegge33670QB { get; set; }
            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoES fondoES = (DatiFondoES)obj;
                try
                {
                    if (
                        this.AnnoUtile != fondoES.AnnoUtile ||
                        this.Articolo58 != fondoES.Articolo58 ||
                        this.Articolo59 != fondoES.Articolo59 ||
                        this.BaseAltraPensione != fondoES.BaseAltraPensione ||
                        this.CategoriaAltraPensione != fondoES.CategoriaAltraPensione ||
                        this.ClassePensioneAnte50 != fondoES.ClassePensioneAnte50 ||
                        this.CodiceAzienda != fondoES.CodiceAzienda ||
                        this.CodiceDz != fondoES.CodiceDz ||
                        this.CodiceEsattoria != fondoES.CodiceEsattoria ||
                        this.CodiceOnOff != fondoES.CodiceOnOff ||
                        this.CodicePensioneInPagamentoPre67 != fondoES.CodicePensioneInPagamentoPre67 ||
                        this.CodiciRetributivi != fondoES.CodiciRetributivi ||
                        this.ContributiDifferimentoQuota != fondoES.ContributiDifferimentoQuota ||
                        this.ContributiLegge37758Art24 != fondoES.ContributiLegge37758Art24 ||
                        this.ContributiLegge37758Art57Periodo1 != fondoES.ContributiLegge37758Art57Periodo1 ||
                        this.ContributiLegge37758Art57Periodo2 != fondoES.ContributiLegge37758Art57Periodo2 ||
                        this.ContributiLegge37758Art57Periodo3 != fondoES.ContributiLegge37758Art57Periodo3 ||
                        this.ContributiSupplementoAgo != fondoES.ContributiSupplementoAgo ||
                        this.ContributiSupplementoFondo != fondoES.ContributiSupplementoFondo ||
                        this.ContributiSupplementoLegge143271 != fondoES.ContributiSupplementoLegge143271 ||
                        this.ContributiTotaliSenzaLegge33670 != fondoES.ContributiTotaliSenzaLegge33670 ||
                        this.Decorrenza != fondoES.Decorrenza ||
                        this.DecorrenzaArticolo24 != fondoES.DecorrenzaArticolo24 ||
                        this.DecorrenzaDati != fondoES.DecorrenzaDati ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo1 != fondoES.DecorrenzaLegge37758Art57Pre67Periodo1 ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo2 != fondoES.DecorrenzaLegge37758Art57Pre67Periodo2 ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo3 != fondoES.DecorrenzaLegge37758Art57Pre67Periodo3 ||
                        this.DecorrenzaRequisitiAnzianita != fondoES.DecorrenzaRequisitiAnzianita ||
                        this.EtaRequisiti != fondoES.EtaRequisiti ||
                        this.ImportoContributiLegge143271Art14 != fondoES.ImportoContributiLegge143271Art14 ||
                        this.ImportoContributiLegge37758Art24 != fondoES.ImportoContributiLegge37758Art24 ||
                        this.ImportoContributiLegge37758Art57 != fondoES.ImportoContributiLegge37758Art57 ||
                        this.ImportoInPagamentoPre67 != fondoES.ImportoInPagamentoPre67 ||
                        this.MaggiorazioneInvalidita != fondoES.MaggiorazioneInvalidita ||
                        this.MaggiorazionePrivilegiata != fondoES.MaggiorazionePrivilegiata ||
                        this.MMServizioUtile != fondoES.MMServizioUtile ||
                        this.MMServizioUtile2 != fondoES.MMServizioUtile2 ||
                        this.MMServizioUtile3 != fondoES.MMServizioUtile3 ||
                        this.MMServizioUtile4 != fondoES.MMServizioUtile4 ||
                        this.NSettimaneLegge37758Art24 != fondoES.NSettimaneLegge37758Art24 ||
                        this.NSettimaneLegge37758Art57 != fondoES.NSettimaneLegge37758Art57 ||
                        this.NSettimaneSenzaLegge33670Art24QuotaA != fondoES.NSettimaneSenzaLegge33670Art24QuotaA ||
                        this.Optanti != fondoES.Optanti ||
                        this.PensioneFondoAl67 != fondoES.PensioneFondoAl67 ||
                        this.Promiscui != fondoES.Promiscui ||
                        this.Retribuzione != fondoES.Retribuzione ||
                        this.Retribuzione2 != fondoES.Retribuzione2 ||
                        this.Retribuzione3 != fondoES.Retribuzione3 ||
                        this.Retribuzione4 != fondoES.Retribuzione4 ||
                        this.Saltuari != fondoES.Saltuari ||
                        this.TipoLiquidazione != fondoES.TipoLiquidazione ||
                        this.TipoPensione != fondoES.TipoPensione ||
                        this.UnaTantum6901 != fondoES.UnaTantum6901 ||
                        this.IntegrazioneArticolo11 != fondoES.IntegrazioneArticolo11 ||
                        this.AnniDifferimento != fondoES.AnniDifferimento ||
                        this.ConvenzioneInternazionale != fondoES.ConvenzioneInternazionale ||
                        this.AnniRiscatti != fondoES.AnniRiscatti ||
                        this.EtaMaturazioneRequisiti != fondoES.EtaMaturazioneRequisiti ||
                        this.SettimaneArt24QA != fondoES.SettimaneArt24QA ||
                        this.SettimaneArt24QB != fondoES.SettimaneArt24QB ||
                        this.Sospensione != fondoES.Sospensione ||
                        this.CodiceSpecificoAgo != fondoES.CodiceSpecificoAgo ||
                        this.DecorrenzaTeorica != fondoES.DecorrenzaTeorica ||
                        this.CodiceTipoLiquidazione != fondoES.CodiceTipoLiquidazione ||
                        this.Requisiti247_243 != fondoES.Requisiti247_243 ||
                        this.NumeroTriSemRequisiti != fondoES.NumeroTriSemRequisiti ||
                        this.AnnoRequisiti != fondoES.AnnoRequisiti ||
                        this.AnzianitaAnni != fondoES.AnzianitaAnni ||
                        this.DecDPCM != fondoES.DecDPCM ||
                        this.RmsDPCM != fondoES.RmsDPCM ||
                        this.RMSSent72 != fondoES.RMSSent72 ||
                        //S.L. 336/70
                        this.CCArt14SenzaLegge33670 != fondoES.CCArt14SenzaLegge33670 ||
                        this.NSettimaneAnzianitaTotaliSenzaLegge33670 != fondoES.NSettimaneAnzianitaTotaliSenzaLegge33670 ||
                        this.RMSSenzaLegge33670QA != fondoES.RMSSenzaLegge33670QA ||
                        this.RMSSenzaLegge33670QB != fondoES.RMSSenzaLegge33670QB
                        )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public bool isNull()
            {

                try
                {
                    if (
                        this.AnnoUtile != null ||
                        this.Articolo58 != null ||
                        this.Articolo59 != null ||
                        this.BaseAltraPensione != null ||
                        this.CategoriaAltraPensione != null ||
                        this.ClassePensioneAnte50 != null ||
                        this.CodiceAzienda != null ||
                        this.CodiceDz != null ||
                        this.CodiceEsattoria != null ||
                        this.CodiceOnOff != null ||
                        this.CodicePensioneInPagamentoPre67 != null ||
                        this.CodiciRetributivi != null ||
                        this.ContributiDifferimentoQuota != null ||
                        this.ContributiLegge37758Art24 != null ||
                        this.ContributiLegge37758Art57Periodo1 != null ||
                        this.ContributiLegge37758Art57Periodo2 != null ||
                        this.ContributiLegge37758Art57Periodo3 != null ||
                        this.ContributiSupplementoAgo != null ||
                        this.ContributiSupplementoFondo != null ||
                        this.ContributiSupplementoLegge143271 != null ||
                        this.ContributiTotaliSenzaLegge33670 != null ||
                        this.Decorrenza != null ||
                        this.DecorrenzaArticolo24 != null ||
                        this.DecorrenzaDati != null ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo1 != null ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo2 != null ||
                        this.DecorrenzaLegge37758Art57Pre67Periodo3 != null ||
                        this.DecorrenzaRequisitiAnzianita != null ||
                        this.EtaRequisiti != null ||
                        this.ImportoContributiLegge143271Art14 != null ||
                        this.ImportoContributiLegge37758Art24 != null ||
                        this.ImportoContributiLegge37758Art57 != null ||
                        this.ImportoInPagamentoPre67 != null ||
                        this.MaggiorazioneInvalidita != null ||
                        this.MaggiorazionePrivilegiata != null ||
                        this.MMServizioUtile != null ||
                        this.MMServizioUtile2 != null ||
                        this.MMServizioUtile3 != null ||
                        this.MMServizioUtile4 != null ||
                        this.NSettimaneLegge37758Art24 != null ||
                        this.NSettimaneLegge37758Art57 != null ||
                        this.NSettimaneSenzaLegge33670Art24QuotaA != null ||
                        this.Optanti != null ||
                        this.PensioneFondoAl67 != null ||
                        this.Promiscui != null ||
                        this.Retribuzione != null ||
                        this.Retribuzione2 != null ||
                        this.Retribuzione3 != null ||
                        this.Retribuzione4 != null ||
                        this.Saltuari != null ||
                        this.TipoLiquidazione != null ||
                        this.TipoPensione != null ||
                        this.UnaTantum6901 != null ||
                        this.IntegrazioneArticolo11 != null ||
                        this.AnniDifferimento != null ||
                        this.ConvenzioneInternazionale != null ||
                        this.AnniRiscatti != null ||
                        this.EtaMaturazioneRequisiti.HasValue ||
                        this.SettimaneArt24QA.HasValue ||
                        this.SettimaneArt24QB.HasValue ||
                        this.Sospensione.HasValue ||
                        this.CodiceSpecificoAgo.HasValue ||
                        this.DecorrenzaTeorica.HasValue ||
                        this.CodiceTipoLiquidazione.HasValue ||
                        this.Requisiti247_243.HasValue ||
                        this.NumeroTriSemRequisiti.HasValue ||
                        this.AnnoRequisiti.HasValue ||
                        this.AnzianitaAnni.HasValue ||
                        this.DecDPCM.HasValue ||
                        this.RmsDPCM.HasValue ||
                        this.RMSSent72.HasValue ||
                        this.CCArt14SenzaLegge33670.HasValue ||
                        this.NSettimaneAnzianitaTotaliSenzaLegge33670.HasValue ||
                        this.RMSSenzaLegge33670QA.HasValue ||
                        this.RMSSenzaLegge33670QB.HasValue)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiFondoPM
        {
            #region public properties
            public long Id { get; set; }
            public long IdFondo { get; set; }
            public long? IdRecordFondo { get; set; }
            public char? CodiceTipo { get; set; }
            public System.Nullable<System.DateTime> DecorrenzaOriginariaAgo { get; set; }
            public System.Nullable<byte> EtaMaturazioneRequisiti { get; set; }
            public System.Nullable<System.DateTime> DecorrenzaAgo { get; set; }
            public System.Nullable<byte> CodiceTipoLiquidazione { get; set; }
            public System.Nullable<int> AnzianitaEsclusiva { get; set; }
            public System.Nullable<decimal> ContributiEsclusiviArt11 { get; set; }
            public string ContribuzioneEsclusiva { get; set; }
            public System.Nullable<decimal> ContributiTotaliLegge143271 { get; set; }
            public System.Nullable<decimal> ContributiEsclusiviLegge143271 { get; set; }
            public System.Nullable<bool> DirittoAgo { get; set; }
            public System.Nullable<bool> AnnoUtileUltimoDecennio { get; set; }
            public System.Nullable<char> ConvenzioneInternazionale { get; set; }
            public System.Nullable<System.DateTime> Decorrenza { get; set; }
            public System.Nullable<short> MesiNavigazioneEffettiva { get; set; }
            public System.Nullable<short> GiorniNavigazioneEffettiva { get; set; }
            public System.Nullable<short> MesiRadePilotaggio { get; set; }
            public System.Nullable<short> GiorniRadePilotaggio { get; set; }
            public System.Nullable<short> MesiTBCDS { get; set; }
            public System.Nullable<short> GiorniTBCDS { get; set; }
            public System.Nullable<short> MesiMalattia { get; set; }
            public System.Nullable<short> GiorniMalattia { get; set; }
            public System.Nullable<short> MesiNavigazioneEE { get; set; }
            public System.Nullable<short> GiorniNavigazioneEE { get; set; }
            public System.Nullable<short> MesiAltriServizi { get; set; }
            public System.Nullable<short> GiorniAltriServizi { get; set; }
            public System.Nullable<short> MesiNavigazioneMilitare { get; set; }
            public System.Nullable<short> GiorniNavigazioneMilitare { get; set; }
            public System.Nullable<short> MesiDoppioMilitare { get; set; }
            public System.Nullable<short> GiorniDoppioMilitare { get; set; }
            public System.Nullable<short> MesiDoppioMercantile { get; set; }
            public System.Nullable<short> GiorniDoppioMercantile { get; set; }
            public System.Nullable<short> MesiServizioMilitareATerra { get; set; }
            public System.Nullable<short> GiorniServizioMilitareATerra { get; set; }
            public System.Nullable<byte> CodiceDifferimentoPrivilegiato { get; set; }
            public System.Nullable<short> PeriodoDifferimento1 { get; set; }
            public System.Nullable<short> PeriodoDifferimento2 { get; set; }
            public System.Nullable<short> TelegrafistaServizioMacchinaAA { get; set; }
            public System.Nullable<short> TelegrafistaServizioMacchinaMM { get; set; }
            public System.Nullable<byte> TipoLiquidazione { get; set; }
            public string GestioneSpeciale1Supplemento { get; set; }
            public string GestioneSpeciale2Supplemento { get; set; }
            public System.Nullable<decimal> RMSDPCM161289 { get; set; }
            public System.Nullable<decimal> RMS7290 { get; set; }
            public System.Nullable<char> CL413 { get; set; }
            public System.Nullable<char> AttivitaSvolta2 { get; set; }
            public byte? NumeroTriSemRequisiti { get; set; }
            public short? AnnoRequisiti { get; set; }
            public int? AnzianitaAnni { get; set; }

            //Nuovi Campi
            /// <summary>
            /// XPMCORIP
            /// </summary>
            public short? CorresponsioneIP { get; set; }

            /// <summary>
            /// XPMRIPMM
            /// </summary>
            public short? MesiRiparametrazione { get; set; }

            /// <summary>
            /// XPMRIPAA
            /// </summary>
            public short? AnniRiparametrazione { get; set; }

            /// <summary>
            /// XPMESCLU
            /// </summary>
            public short? CodiceEsclusione { get; set; }

            /// <summary>
            /// XPMSTATO
            /// </summary>
            public short? Stato { get; set; }

            /// <summary>
            /// XPMRENDI
            /// </summary>
            public decimal? Rendimento { get; set; }

            /// <summary>
            /// XPMDPCDC
            /// </summary>
            public int? CodiceDPCDC { get; set; }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiFondoPM datiFondoPM = (DatiFondoPM)obj;
                try
                {
                    if (
                        this.CodiceTipo != datiFondoPM.CodiceTipo ||
                        this.DecorrenzaOriginariaAgo != datiFondoPM.DecorrenzaOriginariaAgo ||
                        this.EtaMaturazioneRequisiti != datiFondoPM.EtaMaturazioneRequisiti ||
                        this.DecorrenzaAgo != datiFondoPM.DecorrenzaAgo ||
                        this.CodiceTipoLiquidazione != datiFondoPM.CodiceTipoLiquidazione ||
                        this.AnzianitaEsclusiva != datiFondoPM.AnzianitaEsclusiva ||
                        this.ContributiEsclusiviArt11 != datiFondoPM.ContributiEsclusiviArt11 ||
                        this.ContribuzioneEsclusiva != datiFondoPM.ContribuzioneEsclusiva ||
                        this.ContributiTotaliLegge143271 != datiFondoPM.ContributiTotaliLegge143271 ||
                        this.ContributiEsclusiviLegge143271 != datiFondoPM.ContributiEsclusiviLegge143271 ||
                        this.DirittoAgo != datiFondoPM.DirittoAgo ||
                        this.AnnoUtileUltimoDecennio != datiFondoPM.AnnoUtileUltimoDecennio ||
                        this.ConvenzioneInternazionale != datiFondoPM.ConvenzioneInternazionale ||
                        this.Decorrenza != datiFondoPM.Decorrenza ||
                        this.MesiNavigazioneEffettiva != datiFondoPM.MesiNavigazioneEffettiva ||
                        this.GiorniNavigazioneEffettiva != datiFondoPM.GiorniNavigazioneEffettiva ||
                        this.MesiRadePilotaggio != datiFondoPM.MesiRadePilotaggio ||
                        this.GiorniRadePilotaggio != datiFondoPM.GiorniRadePilotaggio ||
                        this.MesiTBCDS != datiFondoPM.MesiTBCDS ||
                        this.GiorniTBCDS != datiFondoPM.GiorniTBCDS ||
                        this.MesiMalattia != datiFondoPM.MesiMalattia ||
                        this.GiorniMalattia != datiFondoPM.GiorniMalattia ||
                        this.MesiNavigazioneEE != datiFondoPM.MesiNavigazioneEE ||
                        this.GiorniNavigazioneEE != datiFondoPM.GiorniNavigazioneEE ||
                        this.MesiAltriServizi != datiFondoPM.MesiAltriServizi ||
                        this.GiorniAltriServizi != datiFondoPM.GiorniAltriServizi ||
                        this.MesiNavigazioneMilitare != datiFondoPM.MesiNavigazioneMilitare ||
                        this.GiorniNavigazioneMilitare != datiFondoPM.GiorniNavigazioneMilitare ||
                        this.MesiDoppioMilitare != datiFondoPM.MesiDoppioMilitare ||
                        this.GiorniDoppioMilitare != datiFondoPM.GiorniDoppioMilitare ||
                        this.MesiDoppioMercantile != datiFondoPM.MesiDoppioMercantile ||
                        this.GiorniDoppioMercantile != datiFondoPM.GiorniDoppioMercantile ||
                        this.MesiServizioMilitareATerra != datiFondoPM.MesiServizioMilitareATerra ||
                        this.GiorniServizioMilitareATerra != datiFondoPM.GiorniServizioMilitareATerra ||
                        this.CodiceDifferimentoPrivilegiato != datiFondoPM.CodiceDifferimentoPrivilegiato ||
                        this.PeriodoDifferimento1 != datiFondoPM.PeriodoDifferimento1 ||
                        this.PeriodoDifferimento2 != datiFondoPM.PeriodoDifferimento2 ||
                        this.TelegrafistaServizioMacchinaAA != datiFondoPM.TelegrafistaServizioMacchinaAA ||
                        this.TelegrafistaServizioMacchinaMM != datiFondoPM.TelegrafistaServizioMacchinaMM ||
                        this.TipoLiquidazione != datiFondoPM.TipoLiquidazione ||
                        this.GestioneSpeciale1Supplemento != datiFondoPM.GestioneSpeciale1Supplemento ||
                        this.GestioneSpeciale2Supplemento != datiFondoPM.GestioneSpeciale2Supplemento ||
                        this.RMSDPCM161289 != datiFondoPM.RMSDPCM161289 ||
                        this.RMS7290 != datiFondoPM.RMS7290 ||
                        this.CL413 != datiFondoPM.CL413 ||
                        this.AttivitaSvolta2 != datiFondoPM.AttivitaSvolta2 ||
                        this.NumeroTriSemRequisiti != datiFondoPM.NumeroTriSemRequisiti ||
                        this.AnnoRequisiti != datiFondoPM.AnnoRequisiti ||
                        this.AnzianitaAnni != datiFondoPM.AnzianitaAnni
                        )
                        return false;

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public class DatiAgoPI
        {
            #region private properties

            private System.Nullable<System.DateTime> _DecorrenzaDatiAgo;

            private string _CodiceSpecificoAgo;

            private System.Nullable<short> _TipoLiquidazione;

            private System.Nullable<System.DateTime> _SospensioneAgo;

            private string _CodiceNatura;

            private System.Nullable<short> _SettimaneVV;

            private System.Nullable<decimal> _RMSQuotaA;

            private System.Nullable<int> _NSettimaneQuotaA;

            private System.Nullable<int> _NSettimaneEsclusiveQuotaA;

            private System.Nullable<decimal> _RMSQuotaB;

            private System.Nullable<int> _NSettimaneQuotaB;

            private System.Nullable<int> _NSettimaneEsclusiveQuotaB;

            private System.Nullable<decimal> _Montante;

            private System.Nullable<decimal> _MontanteEsclusivo;

            private System.Nullable<int> _NSettimane;

            private string _CausaCarico;

            private System.Nullable<decimal> _RMSQuotaAOmogenea;

            private System.Nullable<decimal> _RMSQuotaBOmogenea;
            private long? _IdFondo;

            private System.Nullable<byte> _SemaforoRecord;

            private System.Nullable<short> _DirittoQuoteFisse;

            private System.Nullable<decimal> _Ctres;

            private System.Nullable<int> _NSettimaneExCombattente;

            private System.Nullable<decimal> _RMSRetributiva;


            #endregion private properties

            #region public properties

            // Dati generali AGO

            public System.Nullable<System.DateTime> DecorrenzaDatiAgo
            {
                get { return _DecorrenzaDatiAgo; }
                set { _DecorrenzaDatiAgo = value; }
            }

            public string CodiceSpecificoAgo
            {
                get { return _CodiceSpecificoAgo; }
                set { _CodiceSpecificoAgo = value; }
            }

            public System.Nullable<short> TipoLiquidazione
            {
                get { return _TipoLiquidazione; }
                set { _TipoLiquidazione = value; }
            }

            public System.Nullable<System.DateTime> SospensioneAgo
            {
                get { return _SospensioneAgo; }
                set { _SospensioneAgo = value; }
            }

            public string CodiceNatura
            {
                get { return _CodiceNatura; }
                set { _CodiceNatura = value; }
            }

            public System.Nullable<short> SettimaneVV
            {
                get { return _SettimaneVV; }
                set { _SettimaneVV = value; }
            }

            // Quota A
            public System.Nullable<decimal> RMSQuotaA
            {
                get { return _RMSQuotaA; }
                set { _RMSQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneQuotaA
            {
                get { return _NSettimaneQuotaA; }
                set { _NSettimaneQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaA
            {
                get { return _NSettimaneEsclusiveQuotaA; }
                set { _NSettimaneEsclusiveQuotaA = value; }
            }

            // Quota B
            public System.Nullable<decimal> RMSQuotaB
            {
                get { return _RMSQuotaB; }
                set { _RMSQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneQuotaB
            {
                get { return _NSettimaneQuotaB; }
                set { _NSettimaneQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaB
            {
                get { return _NSettimaneEsclusiveQuotaB; }
                set { _NSettimaneEsclusiveQuotaB = value; }
            }

            // Totali
            public System.Nullable<decimal> Montante
            {
                get { return _Montante; }
                set { _Montante = value; }
            }

            public System.Nullable<decimal> MontanteEsclusivo
            {
                get { return _MontanteEsclusivo; }
                set { _MontanteEsclusivo = value; }
            }

            public System.Nullable<int> NSettimane
            {
                get { return _NSettimane; }
                set { _NSettimane = value; }
            }

            public string CausaCarico
            {
                get { return _CausaCarico; }
                set { _CausaCarico = value; }
            }

            public System.Nullable<decimal> RMSQuotaAOmogenea
            {
                get { return _RMSQuotaAOmogenea; }
                set { _RMSQuotaAOmogenea = value; }
            }

            public System.Nullable<decimal> RMSQuotaBOmogenea
            {
                get { return _RMSQuotaBOmogenea; }
                set { _RMSQuotaBOmogenea = value; }
            }

            public System.Nullable<byte> SemaforoRecord
            {
                get { return _SemaforoRecord; }
                set { _SemaforoRecord = value; }
            }

            public long? IdFondo { get { return _IdFondo; } set { _IdFondo = value; } }

            public short? DirittoQuoteFisse { get { return _DirittoQuoteFisse; } set { _DirittoQuoteFisse = value; } }

            public decimal? Ctres { get { return _Ctres; } set { _Ctres = value; } }

            public System.Nullable<int> NSettimaneExCombattente
            {
                get { return _NSettimaneExCombattente; }
                set { _NSettimaneExCombattente = value; }
            }

            public System.Nullable<decimal> RMSRetributiva
            {
                get { return _RMSRetributiva; }
                set { _RMSRetributiva = value; }
            }
            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiAgoPI datiAgo = (DatiAgoPI)obj;
                try
                {
                    if (this._DecorrenzaDatiAgo != datiAgo._DecorrenzaDatiAgo ||
                        this._CodiceSpecificoAgo != datiAgo._CodiceSpecificoAgo ||
                        this._TipoLiquidazione != datiAgo._TipoLiquidazione ||
                        this._SospensioneAgo != datiAgo._SospensioneAgo ||
                        this._CodiceNatura != datiAgo._CodiceNatura ||
                        this._SettimaneVV != datiAgo._SettimaneVV ||
                        this._RMSQuotaA != datiAgo._RMSQuotaA ||
                        this._NSettimaneQuotaA != datiAgo._NSettimaneQuotaA ||
                        this._NSettimaneEsclusiveQuotaA != datiAgo._NSettimaneEsclusiveQuotaA ||
                        this._RMSQuotaB != datiAgo._RMSQuotaB ||
                        this._NSettimaneQuotaB != datiAgo._NSettimaneQuotaB ||
                        this._NSettimaneEsclusiveQuotaB != datiAgo._NSettimaneEsclusiveQuotaB ||
                        this._Montante != datiAgo._Montante ||
                        this._MontanteEsclusivo != datiAgo._MontanteEsclusivo ||
                        this._NSettimane != datiAgo._NSettimane
                       )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class DatiAgoTeoricoPI
        {
            #region private properties

            private System.Nullable<System.DateTime> _DecorrenzaDatiAgoTeorico;

            private System.Nullable<short> _TipoLiquidazione;

            private System.Nullable<System.DateTime> _SospensioneAGOTeorica;

            private System.Nullable<decimal> _RMSQuotaA;

            private System.Nullable<decimal> _RMSQuotaB;

            private System.Nullable<int> _NSettimaneTotaliQuotaA;

            private System.Nullable<int> _NSettimaneTotaliQuotaB;

            private System.Nullable<int> _NSettimaneEsclusiveQuotaA;

            private System.Nullable<int> _NSettimaneEsclusiveQuotaB;

            private System.Nullable<decimal> _RMSOmogeneaQuotaA;

            private System.Nullable<decimal> _RMSOmogeneaQuotaB;

            #endregion private properties

            #region public properties

            // Decorrenza dati AGO teorico
            public System.Nullable<System.DateTime> DecorrenzaDatiAgoTeorico
            {
                get { return _DecorrenzaDatiAgoTeorico; }
                set { _DecorrenzaDatiAgoTeorico = value; }
            }

            // Tipo liquidazione
            public System.Nullable<short> TipoLiquidazione
            {
                get { return _TipoLiquidazione; }
                set { _TipoLiquidazione = value; }
            }

            // Sospensione AGO teorica
            public System.Nullable<System.DateTime> SospensioneAGOTeorica
            {
                get { return _SospensioneAGOTeorica; }
                set { _SospensioneAGOTeorica = value; }
            }

            // Quota A
            public System.Nullable<decimal> RMSQuotaA
            {
                get { return _RMSQuotaA; }
                set { _RMSQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneTotaliQuotaA
            {
                get { return _NSettimaneTotaliQuotaA; }
                set { _NSettimaneTotaliQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaA
            {
                get { return _NSettimaneEsclusiveQuotaA; }
                set { _NSettimaneEsclusiveQuotaA = value; }
            }

            // Quota B
            public System.Nullable<decimal> RMSQuotaB
            {
                get { return _RMSQuotaB; }
                set { _RMSQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneTotaliQuotaB
            {
                get { return _NSettimaneTotaliQuotaB; }
                set { _NSettimaneTotaliQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaB
            {
                get { return _NSettimaneEsclusiveQuotaB; }
                set { _NSettimaneEsclusiveQuotaB = value; }
            }

            // RMS omogenea
            public System.Nullable<decimal> RMSOmogeneaQuotaA
            {
                get { return _RMSOmogeneaQuotaA; }
                set { _RMSOmogeneaQuotaA = value; }
            }

            public System.Nullable<decimal> RMSOmogeneaQuotaB
            {
                get { return _RMSOmogeneaQuotaB; }
                set { _RMSOmogeneaQuotaB = value; }
            }

            #endregion public properties

            public override bool Equals(object obj)
            {
                DatiAgoTeoricoPI datiAgoTeorico = (DatiAgoTeoricoPI)obj;
                try
                {
                    if (this._DecorrenzaDatiAgoTeorico != datiAgoTeorico._DecorrenzaDatiAgoTeorico ||
                        this._TipoLiquidazione != datiAgoTeorico._TipoLiquidazione ||
                        this._SospensioneAGOTeorica != datiAgoTeorico._SospensioneAGOTeorica ||
                        this._RMSQuotaA != datiAgoTeorico._RMSQuotaA ||
                        this._NSettimaneTotaliQuotaA != datiAgoTeorico._NSettimaneTotaliQuotaA ||
                        this._NSettimaneEsclusiveQuotaA != datiAgoTeorico._NSettimaneEsclusiveQuotaA ||
                        this._RMSQuotaB != datiAgoTeorico._RMSQuotaB ||
                        this._NSettimaneTotaliQuotaB != datiAgoTeorico._NSettimaneTotaliQuotaB ||
                        this._NSettimaneEsclusiveQuotaB != datiAgoTeorico._NSettimaneEsclusiveQuotaB ||
                        this._RMSOmogeneaQuotaA != datiAgoTeorico._RMSOmogeneaQuotaA ||
                        this._RMSOmogeneaQuotaB != datiAgoTeorico._RMSOmogeneaQuotaB
                       )
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }

        public class PretabellaDatiAgoFondoPI
        {
            #region private properties
            private long _Id;
            private System.Nullable<System.DateTime> _DecorrenzaDatiAgo;
            private System.Nullable<byte> _SemaforoRecord;
            #endregion

            public long Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public System.Nullable<System.DateTime> DecorrenzaDatiAgo
            {
                get { return _DecorrenzaDatiAgo; }
                set { _DecorrenzaDatiAgo = value; }
            }

            public System.Nullable<byte> SemaforoRecord
            {
                get { return _SemaforoRecord; }
                set { _SemaforoRecord = value; }
            }


        }


        public class PretabellaPensioneFondoPI
        {
            #region private properties
            private long _IdFondo;
            private long _IdRecordFondo;
            private DateTime? _DecorrenzaFondo;
            private System.Nullable<byte> _SemaforoRecord;
            #endregion


            public long IdFondo
            {
                get { return _IdFondo; }
                set { _IdFondo = value; }
            }

            public long IdRecordFondo
            {
                get { return _IdRecordFondo; }
                set { _IdRecordFondo = value; }
            }

            public DateTime? DecorrenzaFondo
            {
                get { return _DecorrenzaFondo; }
                set { _DecorrenzaFondo = value; }
            }

            public System.Nullable<byte> SemaforoRecord
            {
                get { return _SemaforoRecord; }
                set { _SemaforoRecord = value; }
            }

        }


        public class DatiAgoPM
        {
            #region private properties

            private System.Nullable<long> _Id;

            private long _IdFondo;

            private System.Nullable<short> _TipoLiquidazione;

            private System.Nullable<System.DateTime> _DecorrenzaContributiva;

            private System.Nullable<System.DateTime> _SospensionePensione;

            private System.Nullable<System.DateTime> _DecorrenzaReversibileAgo;

            private System.Nullable<decimal> _RMSQuotaA;

            private System.Nullable<int> _NSettimaneQuotaA;

            private System.Nullable<int> _NsettimaneEsclusiveQuotaA;

            private System.Nullable<int> _NSettimaneAnzianitaVV;

            private System.Nullable<decimal> _ImportoContrLegge3351995;

            private System.Nullable<decimal> _ImportoContrLegge3771958art24;

            private System.Nullable<decimal> _ImportoContrSupLegge14321971art14;

            private System.Nullable<decimal> _BaseAltraPensione;

            private System.Nullable<decimal> _ImportoDPR4881968art11;

            private System.Nullable<decimal> _ImportoContEsclusiviSupDPR4881968art11;

            private string _YPMANNIR;

            private System.Nullable<int> _EtaMaturazioneRequisiti;

            private System.Nullable<System.DateTime> _DecDPCM16121989art2;

            private System.Nullable<decimal> _RMSDPCM;

            private string _CodiceSpecificoLiquidazioneAgo;

            private System.Nullable<decimal> _RMSQuotaB;

            private System.Nullable<int> _NSettimaneQuotaB;

            private System.Nullable<int> _NSettimaneEsclusiveQuotaB;

            private string _YPM503ET;

            private string _YPM503AS;

            private string _YPMTPCOD;

            private string _YPMDECSS;

            private string _YPMSOSSS;

            private string _YPMAUTON;

            private System.Nullable<decimal> _MontanteLegge3351995;

            private System.Nullable<int> _NSettimaneContributive;

            private System.Nullable<decimal> _ImportoContrQuotaD;

            private System.Nullable<decimal> _MontanteQuotaD;

            private System.Nullable<int> _NSettimaneContributiveQuotaD;

            private System.Nullable<int> _NSettimane707A;

            private System.Nullable<int> _NSettimane707B;

            private string _YPMCALC707;

            private string _YPMPROGR;

            #endregion

            #region public properties
            public System.Nullable<long> Id
            {
                get { return _Id; }
                set { _Id = value; }
            }

            public long IdFondo
            {
                get { return _IdFondo; }
                set { _IdFondo = value; }
            }

            public System.Nullable<short> TipoLiquidazione
            {
                get { return _TipoLiquidazione; }
                set { _TipoLiquidazione = value; }
            }

            public System.Nullable<System.DateTime> DecorrenzaContributiva
            {
                get { return _DecorrenzaContributiva; }
                set { _DecorrenzaContributiva = value; }
            }

            public System.Nullable<System.DateTime> SospensionePensione
            {
                get { return _SospensionePensione; }
                set { _SospensionePensione = value; }
            }

            public System.Nullable<System.DateTime> DecorrenzaReversibileAgo
            {
                get { return _DecorrenzaReversibileAgo; }
                set { _DecorrenzaReversibileAgo = value; }
            }

            public System.Nullable<decimal> RMSQuotaA
            {
                get { return _RMSQuotaA; }
                set { _RMSQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneQuotaA
            {
                get { return _NSettimaneQuotaA; }
                set { _NSettimaneQuotaA = value; }
            }

            public System.Nullable<int> NsettimaneEsclusiveQuotaA
            {
                get { return _NsettimaneEsclusiveQuotaA; }
                set { _NsettimaneEsclusiveQuotaA = value; }
            }

            public System.Nullable<int> NSettimaneAnzianitaVV
            {
                get { return _NSettimaneAnzianitaVV; }
                set { _NSettimaneAnzianitaVV = value; }
            }

            public System.Nullable<decimal> ImportoContrLegge3351995
            {
                get { return _ImportoContrLegge3351995; }
                set { _ImportoContrLegge3351995 = value; }
            }

            public System.Nullable<decimal> ImportoContrLegge3771958art24
            {
                get { return _ImportoContrLegge3771958art24; }
                set { _ImportoContrLegge3771958art24 = value; }
            }

            public System.Nullable<decimal> ImportoContrSupLegge14321971art14
            {
                get { return _ImportoContrSupLegge14321971art14; }
                set { _ImportoContrSupLegge14321971art14 = value; }
            }

            public System.Nullable<decimal> BaseAltraPensione
            {
                get { return _BaseAltraPensione; }
                set { _BaseAltraPensione = value; }
            }

            public System.Nullable<decimal> ImportoDPR4881968art11
            {
                get { return _ImportoDPR4881968art11; }
                set { _ImportoDPR4881968art11 = value; }
            }

            public System.Nullable<decimal> ImportoContEsclusiviSupDPR4881968art11
            {
                get { return _ImportoContEsclusiviSupDPR4881968art11; }
                set { _ImportoContEsclusiviSupDPR4881968art11 = value; }
            }

            public string YPMANNIR
            {
                get { return _YPMANNIR; }
                set { _YPMANNIR = value; }
            }

            public System.Nullable<int> EtaMaturazioneRequisiti
            {
                get { return _EtaMaturazioneRequisiti; }
                set { _EtaMaturazioneRequisiti = value; }
            }

            public System.Nullable<System.DateTime> DecDPCM16121989art2
            {
                get { return _DecDPCM16121989art2; }
                set { _DecDPCM16121989art2 = value; }
            }

            public System.Nullable<decimal> RMSDPCM
            {
                get { return _RMSDPCM; }
                set { _RMSDPCM = value; }
            }

            public string CodiceSpecificoLiquidazioneAgo
            {
                get { return _CodiceSpecificoLiquidazioneAgo; }
                set { _CodiceSpecificoLiquidazioneAgo = value; }
            }

            public System.Nullable<decimal> RMSQuotaB
            {
                get { return _RMSQuotaB; }
                set { _RMSQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneQuotaB
            {
                get { return _NSettimaneQuotaB; }
                set { _NSettimaneQuotaB = value; }
            }

            public System.Nullable<int> NSettimaneEsclusiveQuotaB
            {
                get { return _NSettimaneEsclusiveQuotaB; }
                set { _NSettimaneEsclusiveQuotaB = value; }
            }

            public string YPM503ET
            {
                get { return _YPM503ET; }
                set { _YPM503ET = value; }
            }

            public string YPM503AS
            {
                get { return _YPM503AS; }
                set { _YPM503AS = value; }
            }

            public string YPMTPCOD
            {
                get { return _YPMTPCOD; }
                set { _YPMTPCOD = value; }
            }

            public string YPMDECSS
            {
                get { return _YPMDECSS; }
                set { _YPMDECSS = value; }
            }

            public string YPMSOSSS
            {
                get { return _YPMSOSSS; }
                set { _YPMSOSSS = value; }
            }

            public string YPMAUTON
            {
                get { return _YPMAUTON; }
                set { _YPMAUTON = value; }
            }

            public System.Nullable<decimal> MontanteLegge3351995
            {
                get { return _MontanteLegge3351995; }
                set { _MontanteLegge3351995 = value; }
            }

            public System.Nullable<int> NSettimaneContributive
            {
                get { return _NSettimaneContributive; }
                set { _NSettimaneContributive = value; }
            }

            public System.Nullable<decimal> ImportoContrQuotaD
            {
                get { return _ImportoContrQuotaD; }
                set { _ImportoContrQuotaD = value; }
            }

            public System.Nullable<decimal> MontanteQuotaD
            {
                get { return _MontanteQuotaD; }
                set { _MontanteQuotaD = value; }
            }

            public System.Nullable<int> NSettimaneContributiveQuotaD
            {
                get { return _NSettimaneContributiveQuotaD; }
                set { _NSettimaneContributiveQuotaD = value; }
            }

            public System.Nullable<int> NSettimane707A
            {
                get { return _NSettimane707A; }
                set { _NSettimane707A = value; }
            }

            public System.Nullable<int> NSettimane707B
            {
                get { return _NSettimane707B; }
                set { _NSettimane707B = value; }
            }

            public string YPMCALC707
            {
                get { return _YPMCALC707; }
                set { _YPMCALC707 = value; }
            }

            public string YPMPROGR
            {
                get { return _YPMPROGR; }
                set { _YPMPROGR = value; }
            }
            #endregion
            public override bool Equals(object obj)
            {
                DatiAgoPM other = (DatiAgoPM)obj;

                return
                    this._Id == other._Id &&
                    this._IdFondo == other._IdFondo &&
                    this._TipoLiquidazione == other._TipoLiquidazione &&
                    this._DecorrenzaContributiva == other._DecorrenzaContributiva &&
                    this._SospensionePensione == other._SospensionePensione &&
                    this._DecorrenzaReversibileAgo == other._DecorrenzaReversibileAgo &&
                    this._RMSQuotaA == other._RMSQuotaA &&
                    this._NSettimaneQuotaA == other._NSettimaneQuotaA &&
                    this._NsettimaneEsclusiveQuotaA == other._NsettimaneEsclusiveQuotaA &&
                    this._NSettimaneAnzianitaVV == other._NSettimaneAnzianitaVV &&
                    this._ImportoContrLegge3351995 == other._ImportoContrLegge3351995 &&
                    this._ImportoContrLegge3771958art24 == other._ImportoContrLegge3771958art24 &&
                    this._ImportoContrSupLegge14321971art14 == other._ImportoContrSupLegge14321971art14 &&
                    this._BaseAltraPensione == other._BaseAltraPensione &&
                    this._ImportoDPR4881968art11 == other._ImportoDPR4881968art11 &&
                    this._ImportoContEsclusiviSupDPR4881968art11 == other._ImportoContEsclusiviSupDPR4881968art11 &&
                    this._YPMANNIR == other._YPMANNIR &&
                    this._EtaMaturazioneRequisiti == other._EtaMaturazioneRequisiti &&
                    this._DecDPCM16121989art2 == other._DecDPCM16121989art2 &&
                    this._RMSDPCM == other._RMSDPCM &&
                    this._CodiceSpecificoLiquidazioneAgo == other._CodiceSpecificoLiquidazioneAgo &&
                    this._RMSQuotaB == other._RMSQuotaB &&
                    this._NSettimaneQuotaB == other._NSettimaneQuotaB &&
                    this._NSettimaneEsclusiveQuotaB == other._NSettimaneEsclusiveQuotaB &&
                    this._YPM503ET == other._YPM503ET &&
                    this._YPM503AS == other._YPM503AS &&
                    this._YPMTPCOD == other._YPMTPCOD &&
                    this._YPMDECSS == other._YPMDECSS &&
                    this._YPMSOSSS == other._YPMSOSSS &&
                    this._YPMAUTON == other._YPMAUTON &&
                    this._MontanteLegge3351995 == other._MontanteLegge3351995 &&
                    this._NSettimaneContributive == other._NSettimaneContributive &&
                    this._ImportoContrQuotaD == other._ImportoContrQuotaD &&
                    this._MontanteQuotaD == other._MontanteQuotaD &&
                    this._NSettimaneContributiveQuotaD == other._NSettimaneContributiveQuotaD &&
                    this._NSettimane707A == other._NSettimane707A &&
                    this._NSettimane707B == other._NSettimane707B &&
                    this._YPMCALC707 == other._YPMCALC707 &&
                    this._YPMPROGR == other._YPMPROGR;
            }
        }
        #endregion nested class
    }
}
