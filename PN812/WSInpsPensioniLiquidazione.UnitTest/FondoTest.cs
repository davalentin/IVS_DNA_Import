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
    /// Summary description for FondoTest
    /// </summary>
    [TestClass]
    public class FondoTest
    {
        public FondoTest()
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


        //Recupero fondo dati generici per numero domanda
        [TestMethod]
        public void TestGetFondoDatiGenericiPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestioneFondo.EliminaFondoDatiGenerici(idPensione);

            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo(0,
                "q","a","1",new DateTime(2011,01,01),new DateTime(2011,02,01),new DateTime(2011,03,01),1,1,"12",
                "qw",1,'1',
                "1", null, null, null, 2, 3, 23.4M, "we", 1, 33.43M, true, new DateTime(2010, 06, 01), new DateTime(2011, 06, 01), 1, ' ', '0', false, false, false, false, null,null,null,null,null,null,
                null,null,null, null, null, null, null, null, null, null, null, null, null, null, null);
            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);
            
            GestioneFondo.DatiFondo fondo = null;
            //test un' eliminazione presente per numero domanda presente
            GestioneFondo.GetFondoDatiGenericiByIdPensione(idPensione, out fondo);
            Assert.IsTrue(fondo != null && fondo.Equals(datiFondo), "Errore nel recupero dei dati generici del fondo");

            GestioneFondo.EliminaFondoDatiGenerici(idPensione);

            fondo = null;
            //test un'eliminazione assente per numero domanda presente
            GestioneFondo.GetFondoDatiGenericiByIdPensione(idPensione, out fondo);
            Assert.IsTrue(fondo == null, "PensioneFondoDatiGenerici non nulla o vuota");

            datiFondo = new GestioneFondo.DatiFondo(0,
                "q", "a", "1", new DateTime(2011, 01, 01), new DateTime(2011, 02, 01), new DateTime(2011, 03, 01), 1, 1, "12",
                "qs", 1, '1',
                "10", null, null, null, 2, 3, 23.4M, "we", 1, 33.43M, true, new DateTime(2010, 06, 01), new DateTime(2011, 06, 01), 1, ' ', '0', true, true, true, false, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);

            GestioneFondo.EliminaFondoDatiGenerici(idPensione);
        }

        //Recupero fondo EL per numero domanda
        [TestMethod]
        public void TestGetFondoELPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestioneFondo.DatiFondo datiFondo = new GestioneFondo.DatiFondo(0,
                "q", "a", "1", new DateTime(2011, 01, 01), new DateTime(2011, 02, 01), new DateTime(2011, 03, 01), 1, 1, "12",
                "qw", 1, '1',
                "1", null, null, null, 2, 3, 23.4M, "we", 1, 33.43M, true, new DateTime(2010, 06, 01), new DateTime(2011, 06, 01), 1, ' ', '0', true, true, true, true, 55, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            GestioneFondo.SalvaFondoDatiGenerici(idPensione, datiFondo);

            long idFondo = 0;
            GestioneFondo.GetIdFondoByIdPensione(idPensione, out idFondo);

            GestioneFondo.EliminaFondoEL(idPensione);

            GestioneFondo.DatiFondoEL datiFondoEL = new GestioneFondo.DatiFondoEL("q", 1, 2, 3, 10, 12,null, 2, 1, 1, null, 2, null, 3, 2, 1, null, 'e',true, 2, 1, 1);

            GestioneFondo.SalvaFondoEL(idFondo, datiFondoEL);

            GestioneFondo.DatiFondoEL fondoEL = null;
            //test un' eliminazione presente per numero domanda presente
            GestioneFondo.GetFondoELByIdPensione(idPensione, out fondoEL);
            Assert.IsTrue(fondoEL != null && fondoEL.Equals(datiFondoEL), "Errore nel recupero del fondo EL");

            GestioneFondo.EliminaFondoEL(idPensione);

            GestioneFondo.EliminaFondoDatiGenerici(idPensione);

            fondoEL = null;
            //test un'eliminazione assente per numero domanda presente
            GestioneFondo.GetFondoELByIdPensione(idPensione, out fondoEL);
            Assert.IsTrue(fondoEL == null, "PensioneFondoEL non nulla o vuota");
        }

        [TestMethod]
        public void TestEqualsFondoDatiGenerici()
        {
            GestioneFondo.DatiFondo datiFondo1 = new GestioneFondo.DatiFondo();
            if(!datiFondo1.Equals(new GestioneFondo.DatiFondo()))
                Assert.Fail();
        }
    }
}
