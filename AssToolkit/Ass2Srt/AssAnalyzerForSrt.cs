﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Ass2Srt
{
    class AssAnalyzerForSrt : AssAnalyzer
    {
        string strDialogues;
        int type;
        bool DoubleOrSingle;

        public AssAnalyzerForSrt(string strDialogues, int type, bool DoubleOrSingle)
        {
            this.strDialogues = strDialogues;
            this.type = type;       //0-简英，1-繁英，2-简中，3-繁中，4-英文
            this.DoubleOrSingle = DoubleOrSingle;   //T-Double, F-Single
        }

        public string[] Analyze()
        {
            try
            {
                int subCount = 1;
                List<string> lSrtLines = new List<string>();
                string[] strSpittedDialogues = DialogueSplitter(strDialogues);
                foreach (string strDialogue in strSpittedDialogues)
                {
                    lSrtLines.AddRange(DialogueProcessor(strDialogue, subCount));
                    subCount++;
                }

                return lSrtLines.ToArray();
            }

            catch
            {
                return null;
            }
        }

        private List<string> DialogueProcessor(string strDialogue, int subCount)
        {
            List<string> lDialogueProcessed = new List<string>();
            List<string> lDialogueGot = new List<string>();
            //lDialogueProcessed.Add(GetTime(strDialogue));
            lDialogueGot = GetDialogue(strDialogue);
            if (lDialogueGot.Count == 0)
            {

            }

            else
            {
                lDialogueProcessed.Add(subCount.ToString());
                lDialogueProcessed.AddRange(GetDialogue(strDialogue));
                lDialogueProcessed.Add("");
            }

            return lDialogueProcessed;
        }

        private string GetTime(string strDialogue)
        {
            string strSrtTime;
            List<string> lTime = new List<string>();
            Regex rx = new Regex(@"\d{1}:\d{2}:\d{2}.\d{2}");
            MatchCollection MC = rx.Matches(strDialogue);
            foreach (Match m in MC)
            {
                lTime.Add(m.ToString());
            }

            strSrtTime = "0" + lTime[0].Replace('.', ',') + "0 --> " + "0" + lTime[1].Replace('.', ',') + "0";
            
            return strSrtTime;
        }

        private List<string> GetDialogue(string strDialogue)
        {
            List<string> lDialogues = new List<string>();
            Regex rx = new Regex(@"[^\{^\}]+");
            Regex exEngOrChi = new Regex(@"[\u4e00-\u9fa5]");
            strDialogue = strDialogue.Replace("\\h", " ");
            MatchCollection MC = rx.Matches(strDialogue);
            string strDialogueLong = "";
            string strControlCmd = "";
            if (MC.Count > 0)
            {
                if (MC[0].ToString().Contains(",,"))
                {
                    string t = MC[0].ToString();
                    string[] tt = SplitString(t, ",,");
                    strDialogueLong += tt[tt.Length - 1];
                }
            }

            for (int i = 1; i < MC.Count; i++)
            {
                string t = MC[i].ToString();

                if (t[0] == '\\')
                {
                    string[] controlCmds = t.Split('\\');
                    foreach (string s in controlCmds)
                    {
                        if (s.Contains("an") || s.Contains("pos"))
                        {
                            strControlCmd += "{" + "\\" + s + "}";
                            continue;
                        }
                    }

                    continue;
                }

                else if (t == " ")
                {
                    continue;
                }

                //if (t.Contains("\\N"))
                //{

                strDialogueLong += t;
            }

            //strDialogue = strControlCmd + strDialogue;
            //lDialogues.Add(GetTime(strDialogue));
            string[] ss = SplitString(strDialogueLong, "\\N");
            List<string> lSS = new List<string>();
            foreach (string s in ss)
            {
                if (s == "")
                {
                    continue;
                }

                switch (type)
                {
                    case 0:
                        {
                            lSS.Add(s);
                            break;
                        }
                    case 1:
                        {
                            if (exEngOrChi.IsMatch(s))
                            {
                                lSS.Add(ConvertToTrad(s));
                            }

                            else
                            {
                                lSS.Add(s);
                            }

                            break;
                        }
                    case 2:
                        {
                            if (DoubleOrSingle)
                            {
                                if (exEngOrChi.IsMatch(s))
                                {
                                    lSS.Add(s);
                                }

                                else
                                {

                                }
                            }

                            else
                            {
                                lSS.Add(s);
                            }

                            break;
                        }
                    case 3:
                        {
                            if (DoubleOrSingle)
                            {
                                if (exEngOrChi.IsMatch(s))
                                {
                                    lSS.Add(ConvertToTrad(s));
                                }

                                else
                                {

                                }
                            }

                            else
                            {
                                lSS.Add(ConvertToTrad(s));
                            }

                            break;
                        }
                    case 4:
                        {
                            if (exEngOrChi.IsMatch(s))
                            {

                            }
                            else
                            {
                                lSS.Add(s);
                            }

                            break;
                        }
                }

            }
                //}

            if (lSS.Count > 0)
            {
                if (lSS[0] != "■")
                {
                    lDialogues.Add(GetTime(strDialogue));
                    lSS[0] = strControlCmd + lSS[0];
                    lDialogues.AddRange(lSS);
                }
            }

            return lDialogues;
        }
    }
}
