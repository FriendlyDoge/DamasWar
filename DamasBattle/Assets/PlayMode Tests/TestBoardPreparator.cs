using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestBoardPreparator
    {

        [SerializeField] BoardPreparator preparatorPrefab = null;

        // A Test behaves as an ordinary method
        [Test]
        public void TestPrepareBoard()
        {
            GameObject obj = new GameObject();
            BoardPreparator prep = obj.AddComponent<BoardPreparator>();
            GameObject pieceObj = new GameObject();
            pieceObj.AddComponent<Piece>();

            prep.setTestData(pieceObj.GetComponent<Piece>());

            Piece[][] pieces = prep.PrepareBoard();
            Assert.AreEqual(10, pieces.Length);
            for(int i = 0; i < pieces.Length; i++)
            {
                Assert.AreEqual(10, pieces[i].Length);
            }

        }
    }
}
