using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    [TestClass]
    public class ErroriCalcoloTest
    {
        [TestMethod]
        public void TestMethod1()
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
        public void TestGetDetrazioni()
        {
            GestioneErroriCalcolo.ErroriCalcolo erroriCalcolo;
            GestioneErroriCalcolo.GetErroriCalcolo(981, GestioneErroriCalcolo.Procedura.ALL, GestioneErroriCalcolo.Gestione.ALL, out erroriCalcolo);

            Assert.IsFalse(erroriCalcolo == null, "Oggetto Null");
            
        }


    }
}
