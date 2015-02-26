using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AgileSqlClub.MergeUi.UnitTests
{
    [TestFixture]
    class ImporterTests
    {
        [Test]
        public void test()
        {
            var i = new Importer();
            i.GetData();
        }
    }
}
