using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AgileSqlClub.MergeUi.UnitTests
{
    [TestFixture]
    class DataTableRepositoryTests
    {
        [Test]
        public void Returns_New_Table_When_Not_In_Scripts()
        {
            
        }

        [Test]
        public void Returns_Existing_Table_When_In_Pre_Deploy_Script()
        {
            
        }

        [Test]
        public void Returns_Existing_Table_When_In_Post_Deploy_Script()
        {

        }

        [Test]
        public void For_Existing_Script_Correctly_Sets_Start_And_End_Positions_So_Script_Can_Be_Replaced()
        {

        }


        /*
            Process is - read from dacpac, update script in project ?? a bit odd
         */
    }
}
