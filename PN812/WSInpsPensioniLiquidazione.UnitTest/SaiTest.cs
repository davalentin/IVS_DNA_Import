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
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for SaiTest
    /// </summary>
    [TestClass]
    public class SaiTest
    {
        public SaiTest()
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
        public void GetWsSai()
        {
            string errori = string.Empty;
            Assert.IsTrue(GestioneSAI.GetDatiWsSai(out errori), errori);
        }

        [TestMethod]
        public void TestGetWsSai()
        {
            string errori = string.Empty;
            List<long> numeriDomanda = new List<long>();

            INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext db = new INPS.Pensioni.Liquidazione.DataCommon.PensioniDataContext(INPS.DNA.Data.ConnectionFactory.GetConnection("PensioniConnectionString"));
            numeriDomanda = (from p in db.Pensiones
                                        where p.Gestione == "018" && p.Gruppo != "0031"
                                        select p.NDomus).ToList<long>();
            db.Connection.Close();

            //numeriDomanda.Add(2146683200001);

            System.IO.File.Delete("C:\\New_DomandeSAI.txt");
            foreach (long numDomanda in numeriDomanda)
            {
                BLCommon.GestionePensione.DatiPensione datiPensione = null;
                BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(numDomanda, null, out datiPensione);

                TipoRichiesta.GET? tipoRicGET = TipoRichiesta.GET.GETSAI;
                if (Utility.IsRiaperturaDomanda(datiPensione.Id))
                    tipoRicGET = TipoRichiesta.GET.GETSAY;
                else if (Utility.IsRicostituzione_MotiviContributivi(datiPensione))
                    tipoRicGET = TipoRichiesta.GET.GETSAR;
                else if (Utility.IsRicostituzione_Supplemento(datiPensione))
                    tipoRicGET = TipoRichiesta.GET.GETSAS;
                else if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione)
                    tipoRicGET = TipoRichiesta.GET.GETSAR;

                Entity.SAI datiSAI = new Entity.SAI();
                GestioneSAI.GetDatiSAI(numDomanda, null, tipoRicGET.GetValueOrDefault(), ref datiSAI, out errori);
                if (!errori.StartsWith("Domanda non trovata sul SAI."))
                    System.IO.File.AppendAllText("C:\\New_DomandeSAI.txt", numDomanda.ToString() + "\t");
            }
        }
    }
}
