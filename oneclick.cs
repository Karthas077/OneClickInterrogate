using System.Collections.Generic;
using System;
using XiaWorld;
using FairyGUI;
using HarmonyLib;

namespace OneClickInterrogate
{
    public class OneClickInterrogate
    {
        [HarmonyPatch(typeof(Wnd_JianghuTalk), "OnInit")]
        class OneClickInterrogatePatch
        {
            static void Postfix(ref Wnd_JianghuTalk __instance)
            {
                try
                {
                    GButton interrogateButton;

                    var talkWindow = __instance;
                    if (talkWindow.UIInfo.GetChild("OneClick.Interrogate") == null)
                    {
                        interrogateButton = (GButton)UIPackage.CreateObjectFromURL("ui://ncbwb41mv9j6ah");

                        interrogateButton.name = "OneClick.Interrogate";
                        interrogateButton.title = "Interrogate";
                        interrogateButton.text = "Interrogate";
                        talkWindow.UIInfo.AddChild(interrogateButton);

                        //interrogateButton.onClick.Set(Interrogate);
                        interrogateButton.onClick.Add(delegate ()
                        {
                            Npc player = (Npc)Traverse.Create(talkWindow).Field("player").GetValue();
                            Npc target = (Npc)Traverse.Create(talkWindow).Field("target").GetValue();
                            int curnpc = (int)Traverse.Create(talkWindow).Field("targetseed").GetValue();
                            //float favour = (float)Traverse.Create(talkWindow).Field("targetdata").Field("favour").GetValue();
                            //Maybe make based on current favour
                            int learned = 0;
                            foreach (KeyValuePair<int, JianghuMgr.JHNpcData> keyValuePair in JianghuMgr.Instance.KnowNpcData)
                            {
                                int key = keyValuePair.Key;
                                if (key == curnpc)
                                    continue;
                                bool sameSchool = JianghuMgr.JHNpcLocaltionSeed2Data(curnpc).s == JianghuMgr.JHNpcLocaltionSeed2Data(key).s;
                                if (!sameSchool)
                                    continue;

                                //JianghuNpcDef jhnpcDataBySeed = JianghuMgr.Instance.GetJHNpcDataBySeed(key);
                                //g_emJHNpcDataType.Feature
                                for(g_emJHNpcDataType t = g_emJHNpcDataType.Feature; t <= g_emJHNpcDataType.Hobby3; t++)
                                {
                                    bool iKnow = JianghuMgr.Instance.IsKnowNpc(key, t);
                                    bool targetKnows = JianghuMgr.Instance.CheckNpcKnowOther(curnpc, key, t);
                                    if(!iKnow && targetKnows)
                                    {
                                        JianghuMgr.Instance.UnLockNpcDataKnow(key, t, false);
                                        learned++;
                                    }
                                }
                            }
                            string feedback = string.Format("{0}{1}{2}{3}{4}{5}", new object[]
                                {
                                    player.GetName(),
                                    " interrogates ",
                                    target.GetName(),
                                    " about all of their fellow sect members and learned ",
                                    learned,
                                    " things."
                                });
                            talkWindow.SetTxt(feedback);
                        }
                        );
                    }
                    else
                    {
                        interrogateButton = (GButton)talkWindow.UIInfo.GetChild("OneClick.Interrogate");
                    }

                }
                catch (Exception e)
                {
                    KLog.Dbg("[OCI] error" + e.ToString(), new object[0]);
                }
            }
        }
    }
}
