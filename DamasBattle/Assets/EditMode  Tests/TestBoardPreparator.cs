using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestBoardPreparator
    {
        
        [Test]
        public void TestIsDarkSquare()
        {
            BoardPreparator preparator = new BoardPreparator();
            int i = 0;
            int j = 0;
            bool result = preparator.IsDarkSquare(0,0);
            Assert.AreEqual(true, result);

            result = preparator.IsDarkSquare(0, 1);
            Assert.AreEqual(false, result);
            result = preparator.IsDarkSquare(1, 0);
            Assert.AreEqual(false, result);
            result = preparator.IsDarkSquare(1, 1);
            Assert.AreEqual(true, result);


        }

        [Test]
        public void TestPreparePieces()
        {

        }

    }
}
