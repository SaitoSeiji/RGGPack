using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RPGEnums;

namespace Tests
{
    public class Test_battle_targetResource
    {
        [TestCase(10,10, ResourceType.HP,true,false)]
        [TestCase(10, 10, ResourceType.SP, true, false)]
        [TestCase(9,10, ResourceType.HP,true,true)]
        [TestCase(10,9, ResourceType.SP,true,true)]
        [TestCase(10,10, ResourceType.SP,false,true)]
        public void Test_IsUseAble(int hpnow, int spnow,ResourceType resourcetype,bool iscure,bool expect)
        {
            var pl = CreatePl(hpnow,spnow);
            Assert.AreEqual(expect, Battle_targetResource.IsUseAble(resourcetype, iscure,pl));
        }

        PlayerChar CreatePl(int hpnow,int spnow)
        {

            var pl = new SavedDBData_player();
            pl._hpMax = 10;
            pl._hpNow = hpnow;
            pl._spMax = 10;
            pl._spNow = spnow;
            pl._attack = 10;
            pl._guard = 10;
            return new PlayerChar(pl);
        }
    }
}
