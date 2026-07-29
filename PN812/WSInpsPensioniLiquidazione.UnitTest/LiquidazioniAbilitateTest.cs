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

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for LiquidazioniAbilitateTest
    /// </summary>
    [TestClass]
    public class LiquidazioniAbilitateTest
    {
        public LiquidazioniAbilitateTest()
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
        public void TestGetAllLiquidazioniAbilitate()
        {
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.LiquidazioneAbilitata> elencoLiquidazioniAbilitate = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);
            Assert.IsTrue(elencoLiquidazioniAbilitate != null && elencoLiquidazioniAbilitate.Count > 0, "Nessuna liquidazione abilitata recuperata");
        }

        [TestMethod]
        public void TestGetSaveCancelLiquidazioneAbilitata()
        {
            List<INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.LiquidazioneAbilitata> elencoLiquidazioniAbilitate = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);
            Assert.IsTrue(elencoLiquidazioniAbilitate != null && elencoLiquidazioniAbilitate.Count > 0, "Nessuna liquidazione abilitata recuperata");

            int count = elencoLiquidazioniAbilitate.Count;

            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liquidazioniAbilitate = new INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
            liquidazioniAbilitate.SiglaCategoria = "VVL";
            liquidazioniAbilitate.Sede = 2100;
            liquidazioniAbilitate.Tipologia = "FS";
            liquidazioniAbilitate.Ricostituzione = false;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(liquidazioniAbilitate);

            elencoLiquidazioniAbilitate = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);
            Assert.IsTrue(elencoLiquidazioniAbilitate.Count > count, "Salvataggio non eseguito");

            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.EliminaLiquidazioneAbilitata(liquidazioniAbilitate);

            elencoLiquidazioniAbilitate = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);
            Assert.IsTrue(elencoLiquidazioniAbilitate.Count == count, "Eliminazione non eseguita");
        }
    }
}

