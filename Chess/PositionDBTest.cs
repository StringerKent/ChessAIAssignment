using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess
{
    [TestClass]
    public class PositionDBTest
    {
        [TestMethod]
        public void TestHash() {
            var game = new Game();
            game.New();

            var hash1 = game.Hash;
            Assert.IsTrue(game.TryStringMove("e2-e4"));
            var hash2 = game.Hash;
            Assert.AreNotEqual(hash1, hash2);
            Assert.IsTrue(game.TryStringMove("d7-d5"));
            var hash3 = game.Hash;
            Assert.AreNotEqual(hash2, hash3);

            Assert.IsTrue(game.TryStringMove("e4-d5"));
            var hash4 = game.Hash;
            Assert.AreNotEqual(hash3, hash4);

            game.UndoLastMove();
            Assert.AreEqual(game.Hash, hash3);
            game.UndoLastMove();
            Assert.AreEqual(game.Hash, hash2);
            game.UndoLastMove();
            Assert.AreEqual(game.Hash, hash1);

        }
        
        [TestMethod]
        public void ScoreBitShift1() {
            var commandNo = (byte)124;
            var legal = true;
            var check = true;
            var score = (short)8100;
            var scorInfo = ScoreInfo.Mate | ScoreInfo.InsufficientMaterial;
            var build = PositionsDatabase.Pack(commandNo, legal, check, score, scorInfo);

            byte oCommandNo;
            bool oLegal;
            bool oCheck;
            ScoreInfo oScoreInfo;
            int oScore;

            PositionsDatabase.Unpack(build, out oCommandNo, out oLegal, out oCheck, out oScoreInfo, out oScore);

            Assert.AreEqual(oCommandNo, commandNo);
            Assert.AreEqual(oLegal, legal);
            Assert.AreEqual(check, oCheck);
            Assert.AreEqual(oScore, score);
            Assert.AreEqual(oScoreInfo, scorInfo);
        }

        [TestMethod]
        public void ScoreBitShiftZero() {
            var commandNo = (byte)0;
            var legal = false;
            var check = false;
            var score = 0;
            var recursions = (byte)0;
            var scoreInfo = (ScoreInfo)0;//ScoreInfo.DrawByRepetion | ScoreInfo.StaleMate;
            var build = PositionsDatabase.Pack(commandNo, legal, check ,score, scoreInfo);

            byte oCommandNo;
            bool oLegal;
            bool oCheck;
            ScoreInfo oScoreInfo;
            int oScore;

            PositionsDatabase.Unpack(build, out oCommandNo, out oLegal, out oCheck, out oScoreInfo, out oScore);

            Assert.AreEqual(commandNo, oCommandNo);
            Assert.AreEqual(legal, oLegal);
            Assert.AreEqual(check, oCheck);
            Assert.AreEqual(score, oScore);
            Assert.AreEqual(scoreInfo, oScoreInfo);
        }

        [TestMethod]
        public void ScoreBitShiftNegScore() {
            var commandNo = (byte)8;
            var legal = false;
            var check = false;
            var score = -1000;
            var recursions = (byte)4;
            var scoreInfo = ScoreInfo.DrawByRepetion | ScoreInfo.StaleMate;
            var build = PositionsDatabase.Pack(commandNo, legal, check, score, scoreInfo);

            byte oCommandNo;
            bool oLegal;
            bool oCheck;
            ScoreInfo oScoreInfo;
            int oScore;

            PositionsDatabase.Unpack(build, out oCommandNo, out oLegal, out oCheck, out oScoreInfo, out oScore);

            Assert.AreEqual(commandNo, oCommandNo);
            Assert.AreEqual(legal, oLegal);
            Assert.AreEqual(score, oScore);
            Assert.AreEqual(scoreInfo, oScoreInfo);
        }

        
        
        [TestMethod]
        public void ScoreBitShiftMax() {
            var commandNo = (byte)127;
            var legal = true;
            bool check = true;
            var score = 8190;
            var recursions = (byte)31;
            var scoreInfo = ScoreInfo.DrawByRepetion | ScoreInfo.StaleMate | ScoreInfo.Mate | ScoreInfo.InsufficientMaterial;
            var build = PositionsDatabase.Pack(commandNo, legal, check, score, scoreInfo);

            byte oCommandNo;
            bool oLegal;
            bool oCheck;
            ScoreInfo oScoreInfo;
            int oScore;
            byte oRecursions;

            PositionsDatabase.Unpack(build, out oCommandNo, out oLegal, out oCheck ,out oScoreInfo, out oScore);

            Assert.AreEqual(commandNo, oCommandNo);
            Assert.AreEqual(legal, oLegal);
            Assert.AreEqual(check, oCheck);
            Assert.AreEqual(score, oScore);
            Assert.AreEqual(scoreInfo, oScoreInfo);
        }

        [TestMethod]
        public void Test()
        {
            Console.WriteLine(Convert.ToString((short)100, 2));
            Console.WriteLine(Convert.ToString((short)-100, 2));
        }
     }
}