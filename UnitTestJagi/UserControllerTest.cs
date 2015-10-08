using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestJagi
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void Test_Index_ViewModel()
        {
            // Arrange
            var userManager = new UserManager<MemoryUser>(new MemoryUserStore());
        }
    }
}
