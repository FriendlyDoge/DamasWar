using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestBoardController
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestUpdatePlayer()
        {
            GameObject prepObj = new GameObject();
            BoardPreparator prep = prepObj.AddComponent<BoardPreparator>();
            BoardController controller = prepObj.AddComponent<BoardController>();

            prepObj.AddComponent<Piece>();

            prep.setTestData(prepObj.GetComponent<Piece>());

            controller.setTestData(prep);

            controller.UpdateCurrentPlayer();
            Assert.AreEqual(2, controller.GetCurrentPlayerNumber());
            controller.UpdateCurrentPlayer();
            Assert.AreEqual(1, controller.GetCurrentPlayerNumber());
        }
    }
}
