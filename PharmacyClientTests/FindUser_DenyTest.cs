using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmacyClient.ViewModels;
using System;

namespace PharmacyClientTests
{
    [TestClass]
    public class FindUser_DenyTest
    {
        [TestMethod]
        public void FindUserCheck_DenyTest()
        {
            string login = "3";
            string password = "4";
            bool isExpected = false;

            AuthViewModel authViewModel = new AuthViewModel();
            bool isAllowed = authViewModel.FindUser(login, password) != null;

            Assert.AreEqual(isExpected, isAllowed);
        }
    }
}
