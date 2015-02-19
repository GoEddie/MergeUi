using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileSqlClub.MergeUi.Merge;
using Moq;
using NUnit.Framework;

namespace AgileSqlClub.MergeUi.UnitTests.Merge
{
    [TestFixture]
    class _MergeStatementRepository
    {
        [Test]
        public void Creates_A_New_Merge_Statement_When_It_Doesnt_Have_One()
        {
            var builder = new Mock<MergeStatementFromMetadataBuilder>();
            var expected = new MergeStatementDescriptor();
            
            builder.Setup(p => p.BuildFromMetaData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(expected);
            
            var repository = new MergeStatementRepository(builder.Object);
            var actual = repository.GetMergeStatement("a","b","c");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Merge_Statments_Are_Stored_And_Returned_Rather_Than_New_Ones_Each_Request()
        {
            var builder = new Mock<MergeStatementFromMetadataBuilder>();
            var expected = new MergeStatementDescriptor();

            builder.Setup(p => p.BuildFromMetaData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(expected);

            var repository = new MergeStatementRepository(builder.Object);
            var actual = repository.GetMergeStatement("a", "b", "c");
            actual = repository.GetMergeStatement("a", "b", "c");
            actual = repository.GetMergeStatement("a", "b", "c");
            actual = repository.GetMergeStatement("a", "b", "c");
            actual = repository.GetMergeStatement("a", "b", "c");
          
            Assert.AreEqual(expected, actual);

            builder.Verify(p => p.BuildFromMetaData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Exactly(1));

        }

        [Test]
        public void Returns_Existing_Statement()
        {

        }



    }
}
