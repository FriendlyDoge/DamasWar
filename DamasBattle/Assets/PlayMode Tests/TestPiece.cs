using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestPiece
    {
        [Test]
        public void TestPiecePositions()
        {
            GameObject b = new GameObject();
            Piece p = b.AddComponent<Piece>();
            BoardController c = b.AddComponent<BoardController>();
            BoardPreparator prep = b.AddComponent<BoardPreparator>();
            prep.setTestData(p);
            c.setTestData(prep);
            float x = 10;
            float y = 5;
            b.transform.position = new Vector2(x,y);
            Assert.AreEqual(p.GetXPosition(), x);
            Assert.AreEqual(p.GetYPosition(), y);
            int x2 = 15;
            int y2 = 15;
            p.MoveTo(x2,y2);
            Assert.AreEqual(p.GetXPosition(), x2);
            Assert.AreEqual(p.GetYPosition(), y2);
        }

        [Test]
        public void TestPieceDama()
        {
            GameObject b = new GameObject();
            BoardController c = b.AddComponent<BoardController>();
            
            Piece p = b.AddComponent<Piece>();
            BoardPreparator prep = b.AddComponent<BoardPreparator>();
            prep.setTestData(p);
            c.setTestData(prep);
            p.transform.position = new Vector2(1, 3);
            Assert.IsFalse(p.isThisDama());
            p.makeDama();
            Assert.IsFalse(p.isThisDama());
            p.transform.position = new Vector2(2,9);
            p.makeDama();
            Assert.IsTrue(p.isThisDama());
        }

        [Test]
        public void TestPieceCanReach()
        {
            GameObject b = new GameObject();
            BoardController c = b.AddComponent<BoardController>();
            Piece p = b.AddComponent<Piece>();
            BoardPreparator prep = b.AddComponent<BoardPreparator>();
            prep.setTestData(p);
            c.setTestData(prep);
            p.transform.position = new Vector2(5, 5);
            Assert.IsTrue(p.CanReach(6));
            Assert.IsFalse(p.CanReach(8));
            p.transform.position = new Vector2(5, 9);
            p.makeDama();
            p.transform.position = new Vector2(5, 5);
            Assert.IsTrue(p.CanReach(6));
            Assert.IsTrue(p.CanReach(4));
            Assert.IsTrue(p.CanReach(8));

        }

        [Test]
        public void TestPieceDie()
        {
            GameObject b = new GameObject();
            BoardController c = b.AddComponent<BoardController>();
            Piece p = b.AddComponent<Piece>();
            BoardPreparator prep = b.AddComponent<BoardPreparator>();
            prep.setTestData(p);
            c.setTestData(prep);
            b.AddComponent<Animator>();

            p.Die();
            Assert.AreNotEqual(p, null);
        }
    }
}
