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

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for PagamentoTest
    /// </summary>
    [TestClass]
    public class PagamentoTest
    {
        public PagamentoTest()
        {
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
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

        [TestMethod]
        public void TestGetSaveDeletePagamentoFromDB()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2005507600002, null, out idPensione);

            BLCommon.GestionePagamento.DatiPagamento pagamento = new BLCommon.GestionePagamento.DatiPagamento();
            pagamento.ABI = 01010;
            pagamento.CAB = 40141;
            pagamento.CapUfficioPagatore = "80056";
            BLCommon.GestionePagamento.SalvaPagamento(idPensione, pagamento);

            BLCommon.GestionePagamento.GetPagamentoByIdPensione(idPensione, out pagamento);

            Assert.IsTrue(pagamento != null && pagamento.ABI == 01010 &&
                pagamento.CAB == 40141, "Pagamento non valorizzato correttamente");

            BLCommon.GestionePagamento.EliminaPagamentoByIdPensione(idPensione);

            BLCommon.GestionePagamento.GetPagamentoByIdPensione(idPensione, out pagamento);

            Assert.IsTrue(pagamento == null, "Pagamento non nullo");
        }

        [TestMethod]
        public void TestGetPagamentoByDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            //pagamento non presente su db ma presente su ws modPag banca sportello (abi + cab)
            string errori = "";
            GestioneAreaPagamento.DatiPagamento areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag banca cc (iban + bic)
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag banca libretto (solo iban)
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag posta sportello
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag posta libretto
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag posta cc
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");
              
            //pagamento non presente su db ma presente su ws modPag estero sportello (solo stato)
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag estero assegno (solo stato)
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente su db ma presente su ws modPag estero cc (iban + bic + stato)
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            //pagamento non presente per domanda non presente
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(null, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento == null, "Area pagamento non nulla");

            //pagamento non presente per domanda presente
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento == null, "Area pagamento non nulla");
        }

        [TestMethod]
        public void TestGetSaveCancelPagamentoByDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            //pagamento non presente su db ma presente su ws modPag banca sportello (abi + cab)
            string errori = "";
            GestioneAreaPagamento.DatiPagamento areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            if (!GestioneAreaPagamento.StorePagamentoByDatiPensione(ref datiPensione, areaPagamento, "", "", out errori))
                Assert.Fail(errori);
            Assert.IsTrue(errori == "", "Salvataggio Pagamento non riuscito correttamente");

            //pagamento presente su DB per domanda presente
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento != null, "Area pagamento non valorizzata correttamente");

            if (!GestioneAreaPagamento.CancelPagamentoByDatiPensione(datiPensione, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(errori == "", "Eliminazione Pagamento non riuscito correttamente");

            //pagamento non presente per domanda presente
            areaPagamento = null;
            if (!GestioneAreaPagamento.GetPagamentoByDatiPensione(datiPensione, out areaPagamento, out errori))
                Assert.Fail(errori);
            Assert.IsTrue(areaPagamento == null, "Area pagamento non nulla");
        }
        [TestMethod]
        public void GetStatoTitolarita()
        {
            string errori = "";
            GestioneVerTitolIBAN.AreaTitolarita areaTitolarita = new GestioneVerTitolIBAN.AreaTitolarita();
            areaTitolarita.CodiceIban = "DE50512308004613259968";
            areaTitolarita.CodiceFiscale = "BLLFNC80C60H501P";
            areaTitolarita.NumDomanda = "0";
            if(!GestioneVerTitolIBAN.GetStatoTitolarita(ref areaTitolarita, "", "", out errori))
                Assert.Fail(errori);
            Assert.IsTrue((areaTitolarita.Status == "1" || areaTitolarita.Status == "4" || areaTitolarita.Status == "3"), areaTitolarita.Note);
        }
    }
}

