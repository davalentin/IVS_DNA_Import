using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for IstruttoriaTest
    /// </summary>
    [TestClass]
    public class IstruttoriaTest
    {
        public IstruttoriaTest()
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


        //Recupero istruttoria per numero domanda
        [TestMethod]
        public void TestGetIstruttoriaPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria(null, 2, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null,null,null,null, null, null, null, null);
            GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);

            GestioneIstruttoria.DatiIstruttoria istruttoria = null;
            //test un' eliminazione presente per numero domanda presente
            GestioneIstruttoria.GetIstruttoriaByIdPensione(idPensione, out istruttoria);
            Assert.IsTrue(istruttoria != null && istruttoria.Equals(datiIstruttoria), "Errore nel recupero dell'istruttoria");

            GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);

            GestionePensione.GetIdPensioneByNumeroDomanda(2125517900002, null, out idPensione);

            GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);

            istruttoria = null;
            //test un'eliminazione assente per numero domanda presente
            GestioneIstruttoria.GetIstruttoriaByIdPensione(idPensione, out istruttoria);
            Assert.IsTrue(istruttoria == null, "Istruttoria non nulla o vuota");

            datiIstruttoria = new GestioneIstruttoria.DatiIstruttoria(null, 1, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null,null, null, null, null, null, null);
            GestioneIstruttoria.SalvaIstruttoria(idPensione, datiIstruttoria);

            GestioneIstruttoria.EliminaIstruttoriaByIdPensione(idPensione);
        }
    }
}
