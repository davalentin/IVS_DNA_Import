using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for PuliziaDomandaTest
    /// </summary>
    [TestClass]
    public class PuliziaDomandaTest
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
        public void TestGetPuliziaDomandaByDomanda()
        {
            long numeroDomanda = 2038575700009;
            Entity.PuliziaDomanda entityPuliziaDomanda = null;
            bool IsPuliziaDisponibile = false;
            string messaggioVideo = string.Empty;
            short sedeOperatore = 2100;
            short centroOperativoOperatore = 0;
            BLCommon.Utility.TipoAppartenenza tipoAppRuolo = BLCommon.Utility.TipoAppartenenza.AGO;
            string sedeDiversa = string.Empty;
            INPS.Pensioni.Liquidazione.BLCommon.Utility.Ruolo ruolo = INPS.Pensioni.Liquidazione.BLCommon.Utility.Ruolo.AMMINISTRATORE;

            GestionePuliziaDomanda.GetPuliziaDomandaByDomanda(numeroDomanda, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, ruolo, out entityPuliziaDomanda, out sedeDiversa, 
                out IsPuliziaDisponibile, out messaggioVideo);
        }

        [TestMethod]
        public void TestEseguiPuliziaDomandaByDomanda()
        {
            long numeroDomanda = 2038575700000;
            Entity.PuliziaDomanda entityPuliziaDomanda = null;
            bool IsPuliziaDisponibile = false;
            string messaggioVideo = string.Empty;
            string matricolaOperatore = "12345678";
            short sedeOperatore = 2100;
            short centroOperativoOperatore = 0;
            BLCommon.Utility.TipoAppartenenza tipoAppRuolo = BLCommon.Utility.TipoAppartenenza.FS;
            string sedeDiversa = string.Empty;
            INPS.Pensioni.Liquidazione.BLCommon.Utility.Ruolo ruolo = INPS.Pensioni.Liquidazione.BLCommon.Utility.Ruolo.AMMINISTRATORE;

            GestionePuliziaDomanda.EseguiPuliziaDomandaByDomanda(numeroDomanda, matricolaOperatore, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, out sedeDiversa, out entityPuliziaDomanda, out messaggioVideo);

            GestionePuliziaDomanda.GetPuliziaDomandaByDomanda(numeroDomanda, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, ruolo, out entityPuliziaDomanda, out sedeDiversa, out IsPuliziaDisponibile, 
                out messaggioVideo);
        }
    }
}
