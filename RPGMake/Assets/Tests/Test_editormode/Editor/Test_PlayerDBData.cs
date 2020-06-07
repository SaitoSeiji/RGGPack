using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Test_PlayerDBData
    {
        SavedDBData_player _myPlData;

        [SetUp]
        public void SetUp()
        {
            _myPlData = new SavedDBData_player();
        }

        [TestCase(1, 10)]
        [TestCase(2, 11)]
        [TestCase(3, 12)]
        public void Test_GetNextLevelExp_raw(int level, int exp)
        {
            _myPlData.ExpRate = 1100;
            _myPlData._firstExp = 10;
            _myPlData.InitNeedExpList();
            var next = _myPlData.GetTargetLevelExp_raw(level);
            Assert.AreEqual(exp, next);
        }
        [TestCase(1, 10)]
        [TestCase(2, 21)]
        [TestCase(3, 33)]
        public void Test_GetNextLevelExp_sum(int level, int exp)
        {
            _myPlData.ExpRate = 1100;
            _myPlData._firstExp = 10;
            _myPlData.InitNeedExpList();
            var next = _myPlData.GetTargetLevelExp_sum(level);
            Assert.AreEqual(exp, next);
        }
        [TestCase(1,1,1)]
        [TestCase(15,1,2)]
        [TestCase(23,1,3)]
        public void Test_UpdateLevel(int haveexp,int firstlevel,int resultlevel)
        {
            _myPlData.ExpRate = 1100;
            _myPlData._firstExp = 10;
            _myPlData._exp = haveexp;
            _myPlData._level = firstlevel;
            _myPlData.InitNeedExpList();
            var up = _myPlData.UpdateLevel();
            Assert.AreEqual(resultlevel, _myPlData._level);
            Assert.AreEqual(resultlevel-firstlevel, up);
        }

        //そのうち核
        public void Test_UpdateSkill(int firstlevel, int resultlevel)
        {
            //_myPlData.ExpRate = 1100;
            //_myPlData._firstExp = 10;
            //_myPlData._level = firstlevel;
            //_myPlData.InitNeedExpList();
            //var up = _myPlData.UpdateLevel();
            //Assert.AreEqual(resultlevel, _myPlData._level);
            //Assert.AreEqual(resultlevel - firstlevel, up);
        }
    }
}
