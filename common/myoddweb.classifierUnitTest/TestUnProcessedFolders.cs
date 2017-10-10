using System;
using myoddweb.classifier.core;
using Moq;
using NUnit.Framework;
using myoddweb.classifier.interfaces;

namespace myoddweb.classifierUnitTest64
{
  [TestFixture]
  class TestUnProcessedFolders
  {
    [Test]
    public void TestConstructorEngineCannotBeNull()
    {
      var ns = new Mock<Microsoft.Office.Interop.Outlook._NameSpace>();
      var en = new Mock<IEngine>();
      var mp = new MailProcessor( en.Object, ns.Object );

      Assert.Throws<ArgumentNullException>(() => new UnProcessedFolders( null, mp ));
    }

    [Test]
    public void TestConstructorMailProcessorCannotBeNull()
    {
      var en = new Mock<IEngine>();

      Assert.Throws<ArgumentNullException>(() => new UnProcessedFolders(en.Object, null ));
    }
  }
}
