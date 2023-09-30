using Microsoft.VisualStudio.TestTools.UnitTesting;
using PharmacyClient.ViewModels;
using System;

namespace PharmacyClientTests
{
    [TestClass]
    public class FindUser_AllowTest
    {
        [TestMethod]
        public void FindUserCheck_AllowTest()
        {
            string login = "3";
            string password = "3";
            bool isExpected = true;

            AuthViewModel authViewModel = new AuthViewModel();
            bool isAllowed = authViewModel.FindUser(login, password) != null;

            Assert.AreEqual(isExpected, isAllowed);
        }
    }
}
