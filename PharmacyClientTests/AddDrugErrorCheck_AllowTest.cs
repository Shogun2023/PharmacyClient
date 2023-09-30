using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmacyClient.Views;
using System;

namespace PharmacyClientTests
{
    [TestClass]
    public class AddDrugErrorCheck_AllowTest
    {
        [TestMethod]
        public void ErrorCheck_AllowTest()
        {
            string name = "Афобазол";
            string form = "таблетки";
            string dose = "25мг";
            string price = "250";
            bool isExpected = true;

            NewDrugWindow newDrugWindow = new NewDrugWindow();
            bool isAllowed = newDrugWindow.ErrorCheck(name, form, dose, price).Length == 0;

            Assert.AreEqual(isExpected, isAllowed);
        }
    }
}
