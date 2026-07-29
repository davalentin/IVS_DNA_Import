using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for TotaliIvsTest
    /// </summary>
    [TestClass]
    public class TotalIvsTest
    {
        public TotalIvsTest()
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
        public void TestEstrazioneDatiCumulIVS()
        {
            long nDomus = 2070800100019;
            string errori = string.Empty;
            INPS.Pensioni.Liquidazione.ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;

            bool test = GestioneTotalIvs.GetDatiCumulIVS(nDomus, out risposta, out errori);
        }

        [TestMethod]
        public void TestEstrazioneDatiCumulRicostituzioneIVS()
        {
            long nDomus = 2024824000023;
            string errori = string.Empty;
            INPS.Pensioni.Liquidazione.ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;

            bool test = GestioneTotalIvs.GetDatiCumulRicostituzioneIVS(nDomus, out risposta, out errori);
        }

        [TestMethod]
        public void TestAggiornaKeyPensioneCUMUL()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2008734900007, null, out datiPensione);

            string errori = string.Empty;

            bool booltest = GestioneTotalIvs.AggiornaCumulo(datiPensione, out errori);
        }

        [TestMethod]
        public void RicercaDomandaAutomatica()
        {
            string errori = string.Empty;
            List<long> numeriDomanda = new List<long>();

            INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext db = new INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext(INPS.DNA.Data.ConnectionFactory.GetConnection("PensioniConnectionString"));
            numeriDomanda = (from p in db.Pensiones
                             where p.Gestione == "202"
                             select p.NDomus).ToList<long>();
            db.Connection.Close();

            //numeriDomanda.Add(2146683200001);

            System.IO.File.Delete("C:\\DomandeCumulo.txt");
            foreach (long numDomanda in numeriDomanda)
            {
                BLCommon.GestionePensione.DatiPensione datiPensione = null;
                BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);

                INPS.Pensioni.Liquidazione.ServiceReferences.TotalIvs.clsDatiCumulo risposta = null;
                bool test = GestioneTotalIvs.GetDatiCumulIVS(numDomanda, out risposta, out errori);
                if (risposta != null && risposta.objDomanda != null && risposta.objDomanda.Interna)
                    System.IO.File.AppendAllText("C:\\DomandeCumulo.txt", numDomanda.ToString() + "\t");
            }
        }
    }
}
