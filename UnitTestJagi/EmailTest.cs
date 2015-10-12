using Jagi.Utility;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Mail;
using System.Threading;

namespace UnitTestJagi
{
    [TestClass]
    public class EmailTest
    {
        [TestMethod]
        public void Test_Gmail_Had_Sent()
        {
            Mailer email = new Mailer(
                new EmailSetting { Email = "redmine.excelsior@gmail.com", Password = "490910490910" });
            
            bool hadSent = false;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            WaitCallback waitCB = new WaitCallback((o) => { hadSent = true; ((AutoResetEvent)o).Set(); });
            email.SendGMail("oblin228@gmail.com", "test", "Test", waitCB, autoEvent);

            autoEvent.WaitOne();

            Assert.IsTrue(hadSent);
        }

        [TestMethod ]
        public void Test_Gmail_Injection_Sent()
        {
            EmailSetting setting = new EmailSetting { Email = "redmine.excelsior@gmail.com", Password = "490910490910" };

            var container = new UnityContainer();
            container.RegisterInstance(typeof(EmailSetting), setting);
            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            Mailer email = new Mailer();
            bool hadSent = false;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            WaitCallback waitCB = new WaitCallback((o) => { hadSent = true; ((AutoResetEvent)o).Set(); });
            email.SendGMail("oblin228@gmail.com", "test", "Test", waitCB, autoEvent);

            autoEvent.WaitOne();

            Assert.IsTrue(hadSent);
        }
    }
}
