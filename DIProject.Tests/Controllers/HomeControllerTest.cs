using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIProject;
using DIProject.Controllers;

namespace DIProject.Tests.Controllers
{
    [TestClass]
    public class DIPControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            DIPController controller = new DIPController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
