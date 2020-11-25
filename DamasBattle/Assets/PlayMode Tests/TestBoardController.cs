using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestBoardController
    {
        private GameObject prefabW = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Pieces/White.prefab");
        private GameObject prefabB = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Pieces/Black.prefab");

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


        [Test]
        public void TestNumberPieces()
        {
            GameObject prepObj = new GameObject();
            BoardPreparator prep = prepObj.AddComponent<BoardPreparator>();
            BoardController controller = prepObj.AddComponent<BoardController>();

            Piece prefabwhite = this.prefabW.GetComponent<Piece>();
            Piece prefabblack = this.prefabB.GetComponent<Piece>();

            prepObj.AddComponent<Piece>();

            prep.setTestData(prepObj.GetComponent<Piece>());

            prep.setPrefabsTest(prefabwhite, prefabblack);
            controller.setTestData(prep);
            
            Piece[][] pieces = prep.PrepareBoard();
            controller.setBoardTestData(pieces);

            int resultWhite = controller.countWhitePieces();
            int resultBlack = controller.countBlackPieces();
            Assert.AreEqual(15,resultBlack);
            Assert.AreEqual(15, resultWhite);
        }

        [Test]
        public void TestIsWhitePiece()
        {
            GameObject prepObj = new GameObject();
            BoardPreparator prep = prepObj.AddComponent<BoardPreparator>();
            BoardController controller = prepObj.AddComponent<BoardController>();
            Piece prefabwhite = this.prefabW.GetComponent<Piece>();
            Piece prefabblack = this.prefabB.GetComponent<Piece>();
            Assert.AreEqual(true, controller.IsWhitePiece(prefabwhite));
            Assert.AreEqual(false, controller.IsWhitePiece(prefabblack));
        }
    }
}
