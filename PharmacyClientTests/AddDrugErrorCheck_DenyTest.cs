using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmacyClient.Views;
using System;

namespace PharmacyClientTests
{
    [TestClass]
    public class AddDrugErrorCheck_DenyTest
    {
        [TestMethod]
        public void ErrorCheck_DenyTest()
        {
            string name = "Афобазол";
            string form = "таблетки";
            string dose = "25мг";
            string price = "-250";
            bool isExpected = false;

            NewDrugWindow newDrugWindow = new NewDrugWindow();
            bool isAllowed = newDrugWindow.ErrorCheck(name, form, dose, price).Length == 0;

            Assert.AreEqual(isExpected, isAllowed);
        }
    }
}
