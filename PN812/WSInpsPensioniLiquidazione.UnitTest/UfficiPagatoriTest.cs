using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.VarUfficioPag;
using INPS.Pensioni.Liquidazione.ServiceReferences.UfficiPagatoriNew;

namespace INPS.Pensioni.Liquidazione.UnitTest
{


    /// <summary>
    ///This is a test class for UfficiPagatoriTest and is intended
    ///to contain all UfficiPagatoriTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UfficiPagatoriTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            RoleApplications roleApplications;
            RoleApplication roleApplication;
            IdmIdentity ident;

            ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            roleApplications = new RoleApplications();
            roleApplication = roleApplications.Add("Dominio1.Gruppo1.Processo1");
            roleApplication.Add("Ruolo1");

            System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, roleApplications);

            INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];
        }

        /// <summary>
        ///A test for GetUfficioPagatoreByAbiCab
        ///</summary>
        [TestMethod()]
        public void TestGetUfficioPagatoreByAbiCab()
        {
            List<GestioneUfficiPagatori.AreaUfficioPagatore> ufficioPagatore = null;
            string errori = "";

            //posta - postepay evolution - nuovo iban
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(36081, 09999, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore != null, "Ufficio pagatore non nullo");
            //banca esistente
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(05424, 15431, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore == null && ufficioPagatore.First().Abi != 1010 && ufficioPagatore.First().Cab != 40141, "Ufficio pagatore nullo");
            //posta esistente
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(7601, 40097, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore == null && ufficioPagatore.First().Abi != 7601 && ufficioPagatore.First().Frazionario != 40097, "Ufficio pagatore nullo");
            //banca non esistente
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(0, 0, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore != null, "Ufficio pagatore non nullo");
            //posta non esistente
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(0, 0, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore != null, "Ufficio pagatore non nullo");
        }

        /// <summary>
        ///A test for GetUfficioPagatoreByAbiCab
        ///</summary>
        [TestMethod()]
        public void TestGetSetUfficioPagatoreByAbiCassa()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            List<GestioneUfficiPagatori.AreaUfficioPagatore> ufficioPagatore = null;
            string errori = "";

            //cassa esistente
            if (!GestioneUfficiPagatori.GetUfficiPagatoriNew(99999, 3300015, out ufficioPagatore, out errori))
                Assert.Fail(errori);
            Assert.IsFalse(ufficioPagatore == null && ufficioPagatore.First().Abi != 1010 && ufficioPagatore.First().Cab != 40141, "Ufficio pagatore nullo");

            GestioneAreaPagamento.DatiPagamento pagamento = new GestioneAreaPagamento.DatiPagamento();
            pagamento.ABI = 99999;
            pagamento.CAB = 3300015;
            pagamento.TipoPagamento = 'P';

            if (!GestioneAreaPagamento.StorePagamentoByDatiPensione(ref datiPensione, pagamento, "", "", out errori))
                throw new INPS.DNA.DnaApplicationException(errori);
        }

        [TestMethod()]
        public void TestValidaUfficioPagatore()
        {
            // CC Banca corretto
            string nazione = "Italia";
            string codCatastale = string.Empty;
            string iban = "IT30O36772223000EM00070425O";
            string bic = "";
            string abi = "";
            string cab_frazionario = "";
            string libretto = "";
            string modalitaPagamento = "C";
            string errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // CC Banca errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT92R0103020600000055555575";
            bic = "GKCCBEBBXXX";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "C";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Libretto Banca corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT92R0103020600000055555575";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "L";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Libretto Banca errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT92R9999920600000055555575";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "L";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Prepagata Banca corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT92R0103020600000055555575";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "K";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Prepagata Banca errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT92R0103020600000055555575";
            bic = "GKCCBEBBXXX";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "K";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // CC Estero corretto
            nazione = "BELGIO";
            codCatastale = string.Empty;
            iban = "BE24271036123438";
            bic = "GABABEBBXXX";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "C";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // CC Estero errato
            nazione = "BELGIO";
            codCatastale = string.Empty;
            iban = "IT24271036123438";
            bic = "MEDBITMMXXX";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "C";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // CC Posta corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT87E0760101800001003855879";
            bic = "";
            abi = "07601";
            cab_frazionario = "74001";
            libretto = "";
            modalitaPagamento = "C";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // CC Posta errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "IT87E9999901800001003855879";
            bic = "";
            abi = "99999";
            cab_frazionario = "74001";
            libretto = "";
            modalitaPagamento = "C";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // ---------------------------------------------------------------------------------------------------

            // Sportello Banca corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "03062";
            cab_frazionario = "34210";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Sportello Banca errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "02088";
            cab_frazionario = "18300";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Sportello Posta corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "07601";
            cab_frazionario = "74001";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Sportello Posta errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "02088";
            cab_frazionario = "74001";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Libretto Posta corretto
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "07601";
            cab_frazionario = "74001";
            libretto = "123456789012";
            modalitaPagamento = "L";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Libretto Posta errato
            nazione = "Italia";
            codCatastale = string.Empty;
            iban = "";
            bic = "";
            abi = "07601";
            cab_frazionario = "00001";
            libretto = "123456789012";
            modalitaPagamento = "L";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Sportello Estero corretto
            nazione = "BELGIO";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Sportello Estero errato
            nazione = "Italia";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Assegno Estero corretto
            nazione = "BELGIO";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "A";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Assegno Estero errato
            nazione = "Italia";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "";
            cab_frazionario = "";
            libretto = "";
            modalitaPagamento = "A";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerEstero(modalitaPagamento, iban, nazione, bic, null, out errori);
            if (string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Circolarità postale corretto
            nazione = "Italia";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "07601";
            cab_frazionario = "0099999";
            libretto = "";
            modalitaPagamento = "S";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            // Cassa sede corretto
            nazione = "Italia";
            codCatastale = "";
            iban = "";
            bic = "";
            abi = "99999";
            cab_frazionario = "3300004";
            libretto = "";
            modalitaPagamento = "X";
            errori = string.Empty;

            GestioneUfficiPagatori.ValidaUfficioPagatorePerItalia(modalitaPagamento, iban, bic, abi, cab_frazionario, libretto, out errori);
            if (!string.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }

        [TestMethod()]
        public void TestGetListStatiEsteri()
        {
            List<GestioneAreaPagamento.DatiStatoEstero> ListStatiEsteri = null;
            string errori = string.Empty;

            GestioneAreaPagamento.GetListStatiEsteri(out ListStatiEsteri, out errori);

            using (System.IO.StreamWriter sw = System.IO.File.CreateText("C:\\listStatiEsteri.csv"))
            {
                sw.WriteLine("Stati Esteri dal servizio Uffici Pagatori Liste");

                foreach (var i in ListStatiEsteri)
                {
                    sw.WriteLine(i.NomeStato);
                }
            }
        }

        [TestMethod()]
        public void TestGetListStatiEsteriNew()
        {
            List<GestioneAreaPagamento.DatiStatoEstero> lista = null;
            string errori = string.Empty;
            GestioneUfficiPagatori.GetStatiEsteri(out lista, out errori);

            if (lista != null)
            {
                var lista44_77 = lista.FindAll(x => x.CAB.StartsWith("44") || x.CAB.StartsWith("77"));

                GestioneAreaPagamento.DatiStatoEstero statoRandom = lista44_77.FirstOrDefault(x => x.NomeStato.Contains("FRANCIA"));

                List<GestioneUfficiPagatori.AreaUfficioPagatore> ufficio = null;
                GestioneUfficiPagatori.GetUfficiPagatoriNew(Utility.StringToNullableInt(statoRandom.ABI).GetValueOrDefault(), Utility.StringToNullableInt(statoRandom.CAB).GetValueOrDefault(), out ufficio, out errori);
            }
        }

        [TestMethod()]
        public void TestElencoNuoveCoordinate()
        {
            string errori = string.Empty;
            int abi = 99999;
            int cab = 3300004;

            List<GestioneUfficiPagatori.AreaUfficioPagatore> ufficio = null;
            GestioneUfficiPagatori.GetUfficiPagatoriNew(abi, cab, out ufficio, out errori);

        }
    }
}
