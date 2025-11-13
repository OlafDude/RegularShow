using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class regularShowScript : MonoBehaviour
{

    //globals
    public KMBombInfo bomb;
    public KMAudio Audio;

    //logging 
    static int moduleIdCounter = 1;
    int moduleId;

    //Selectables
    public KMSelectable[] characters;

    //boss options
    public Material[] bossOptions;
    public Renderer bossPic;
    private int bossIndex = 0;
    private List<int> chosenBossIndex = new List<int>();

    //character options
    public Material[] charOptions;
    public Renderer[] characterPics;
    public Renderer[] characterMats;
    private List<int> chosenCharIndex = new List<int>();
    private List<int> basePower = new List<int>();
    private List<int> modifiedPower = new List<int>();
    private List<string> correctChars = new List<string>();
    private int maxPower = 0; 

    //location options
    public Material[] locOptions;
    public Renderer locPic;
    private int locIndex = 0;

    //solve things
    private int stage = 0;
    private bool moduleSolved;
    private bool waiting;
    private bool playedVoid = false; 

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable character in characters)
        {
            KMSelectable pressedCharacter = character;
            character.OnInteract += delegate () { CharacterPress(pressedCharacter); return false; };
        }
    }

    void Start()
    {
        playVoid();
        PickBoss();
        PickCharacters();
        PickLocation();
        findBasePower();
        modifyBasePower();
        applySpecials();
        pickCorrect();
    }



    void CharacterPress(KMSelectable character)
    {
        if (moduleSolved == true || waiting == true)
        {
            return;
        }
        else
        {
            character.AddInteractionPunch(1f);
            if (correctChars.Contains(character.GetComponent<MeshRenderer>().material.name))
            {
                Debug.LogFormat("Regular Show #{0}: Correct Press", moduleId);
                stage++;
                if (stage == 3)
                {
                    moduleSolved = true;
                    waiting = true;
                    StartCoroutine(Solved());
                    Debug.LogFormat("Regular Show #{0}: Module is solved. Yeeeuhhhh!", moduleId);
                }
                else
                {
                    Audio.PlaySoundAtTransform("yeyuh", transform);
                    chosenCharIndex.Clear();
                    basePower.Clear();
                    modifiedPower.Clear();
                    correctChars.Clear();
                    Start();
                }
            }
            else
            {
                Debug.LogFormat("Regular Show #{0}: Incorrect Press", moduleId);
                waiting = true;
                StartCoroutine(Strike());
            }      
        }
    }

    IEnumerator Strike()
    {
        Audio.PlaySoundAtTransform("ohnobro", transform);
        yield return new WaitForSeconds(1.2f);
        waiting = false; 
        GetComponent<KMBombModule>().HandleStrike();

    }
    IEnumerator Solved()
    {
        Audio.PlaySoundAtTransform("jollygoodshow", transform);
        yield return new WaitForSeconds(1.46f);
        waiting = false;
        locPic.material = locOptions[8];
        characterPics[1].material = locOptions[8];
        characterPics[2].material = locOptions[8];
        characterPics[0].material = locOptions[8];
        bossPic.material = locOptions[8];
        GetComponent<KMBombModule>().HandlePass();
    }

    void PickBoss()
    {
        bossIndex = UnityEngine.Random.Range(0, bossOptions.Length);
        while (chosenBossIndex.Contains(bossIndex))
        {
            bossIndex = UnityEngine.Random.Range(0, bossOptions.Length);
        }
        chosenBossIndex.Add(bossIndex);
        bossPic.material = bossOptions[bossIndex];
        Debug.LogFormat("[Regular Show #{0}] Boss is {1}", moduleId, bossOptions[bossIndex].name);
    }

    void PickCharacters()
    {
        for (int i = 0; i <= 2; i++)
        {
            int index = UnityEngine.Random.Range(0, charOptions.Length);
            while (chosenCharIndex.Contains(index))
            {
                index = UnityEngine.Random.Range(0, charOptions.Length);
            }
            chosenCharIndex.Add(index);
            characterMats[i].material = charOptions[index];
            characterPics[i].material = charOptions[index];
        }    
        
        /*/chosenCharIndex.Add(1);
        chosenCharIndex.Add(8);
        chosenCharIndex.Add(7);
        characterMats[0].material = charOptions[1];
        characterMats[1].material = charOptions[8];
        characterMats[2].material = charOptions[7];
        characterPics[0].material = charOptions[1];
        characterPics[1].material = charOptions[8];
        characterPics[2].material = charOptions[7]; /*/

        Debug.LogFormat("[Regular Show #{0}] Characters are: {1}, {2}, and {3}", moduleId, charOptions[chosenCharIndex[0]].name, charOptions[chosenCharIndex[1]].name, charOptions[chosenCharIndex[2]].name);
    }

    void PickLocation()
    {
        locIndex = UnityEngine.Random.Range(0, locOptions.Length-1);
        locPic.material = locOptions[locIndex];
        Debug.LogFormat("[Regular Show #{0}] Location is {1}", moduleId, locOptions[locIndex].name);
    }

    void findBasePower()
    {
        int[] benbase = { 2, 6, 2, 4, 3, 6, 4, 2 };
        int[] cjbase = { 5, 3, 2, 3, 2, 4, 4, 2 };
        int[] donbase = { 3, 4, 2, 4, 4, 6, 4, 6 };
        int[] eilbase = { 5, 2, 1, 2, 2, 3, 3, 2 };
        int[] garybase = { 7, 7, 7, 7, 11, 7, 11, 11 };
        int[] hfgbase = { 3, 3, 2, 5, 3, 5, 2, 2 };
        int[] maelbase = { 1, 6, 6, 2, 1, 6, 2, 1 };
        int[] margbase = { 6, 3, 3, 3, 3, 4, 4, 2 };
        int[] mordbase = { 5, 4, 2, 3, 5, 6, 4, 3 };
        int[] mmbase = { 4, 4, 3, 7, 3, 6, 5, 4 };
        int[] pambase = { 4, 7, 3, 3, 4, 4, 3, 1 };
        int[] popsbase = { 2, 2, 5, 3, 2, 5, 3, 5 };
        int[] regbase = { 6, 6, 6, 6, 7, 6, 8, 8 };
        int[] rigbase = { 4, 3, 2, 2, 6, 6, 4, 3 };
        int[] skipbase = { 7, 9, 7, 8, 9, 10, 11, 8 };
        int[] starbase = { 4, 3, 1, 6, 1, 4, 3, 1 };
        int[] thombase = { 4, 6, 2, 4, 2, 5, 4, 2 };

        List<int[]> basePowerArray = new List<int[]>() { benbase, cjbase, donbase, eilbase, garybase, hfgbase, maelbase, margbase, mordbase, mmbase, pambase, popsbase, regbase, rigbase, skipbase, starbase, thombase };

        for (int i = 0; i <= charOptions.Length - 1; i++)
        {
            basePower.Add(basePowerArray[i][locIndex]);
        }
        Debug.LogFormat("[Regular Show #{0}] Base Powers are: {1}, {2}, and {3}", moduleId, basePower[chosenCharIndex[0]], basePower[chosenCharIndex[1]], basePower[chosenCharIndex[2]]);
    }

    void modifyBasePower()
    {
        modifiedPower = new List<int>(basePower);
        for (int i = 0; i <= charOptions.Length - 1; i++)
        {
            if (i == 0)  //Benson
            {
                if (bomb.GetSerialNumberLetters().Any(x => x == 'B' || x == 'A' || x == 'L') || bomb.GetSerialNumberNumbers().Any(x => x == 1 || x == 5))
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition 1 is triggered", charOptions[i].name);
                }
                if (bossIndex == 7 || bossIndex == 10 || chosenCharIndex.Contains(6))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition 2 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(10))
                {
                    //Debug.LogFormat("{0} condition 3 is triggered", charOptions[i].name);
                    modifiedPower[i] = modifiedPower[i] + 3;
                }
            }

            else if (i == 1)  //CJ
            {
                if (chosenCharIndex.Contains(8))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(7))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (bomb.GetPortCount(Port.RJ45) >= 1 || bomb.IsIndicatorPresent("CLR"))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition3 is triggered", charOptions[i].name);
                }
                if (bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'E' || x == 'I' || x == 'O' || x =='U'))
                {
                    modifiedPower[i] = modifiedPower[i] - 1;
                    //Debug.LogFormat("{0} condition4 is triggered", charOptions[i].name);
                }
            }

            else if (i == 2)  //Don
            {
                if (chosenCharIndex.Contains(13))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (bomb.GetSerialNumberLetters().All(x => x != 'U' && x != 'G' && x != 'A' && x != 'R') && bomb.GetSerialNumberNumbers().All(x => x != 5))
                {
                    modifiedPower[i] = modifiedPower[i] - 1;
                    //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                }
                if (bossIndex == 9 || bossIndex == 4)
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }

            else if (i == 3) //Eileen
            {
                if (chosenCharIndex.Contains(7))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(13))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (bomb.GetSerialNumber()[5] % 2 == 0)
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
                if (bomb.GetPortPlates().Any(PlatformNotSupportedException => PlatformNotSupportedException.Length ==0 ))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 4is triggered", charOptions[i].name);
                }
                if (bossIndex == 11 || bossIndex == 2)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition5 is triggered", charOptions[i].name);
                }
            }

            else if (i == 4)  //Gary
            {
                if (!chosenCharIndex.Contains(12) && !chosenCharIndex.Contains(14))
                {
                    modifiedPower[i] = modifiedPower[i] - 4;
                    //Debug.LogFormat("{0} condition 1is triggered", charOptions[i].name);
                }
                if (bossIndex == 3)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                }
                if (!bomb.IsIndicatorPresent("SND") && !bomb.IsIndicatorPresent("CLR") && !bomb.IsIndicatorPresent("CAR") && !bomb.IsIndicatorPresent("IND") && !bomb.IsIndicatorPresent("FRQ") && !bomb.IsIndicatorPresent("SIG") && !bomb.IsIndicatorPresent("NSA") && !bomb.IsIndicatorPresent("MSA") && !bomb.IsIndicatorPresent("TRN") && !bomb.IsIndicatorPresent("BOB") && !bomb.IsIndicatorPresent("FRK"))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }

            else if (i == 5)  //HFG
            {
                if(chosenCharIndex.Contains(9))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (bomb.GetSerialNumberNumbers().Any (x=> x == 5))
                {
                    modifiedPower[i] = modifiedPower[i] + 5 ;
                    //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                }
                if (bomb.GetSolvableModuleIDs().Any(x => x == "salts" || x == "clappingMusic" || x == "handTurkey"))
                {
                    modifiedPower[i] = modifiedPower[i] + 4;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }

            else if (i == 6)  // Maellard
            {
                if(chosenCharIndex.Contains(11))
                {
                    modifiedPower[i] = modifiedPower[i] + 4;
                    //Debug.LogFormat("{0} condition 1is triggered", charOptions[i].name);
                }
                if (bossIndex == 4)
                {
                    modifiedPower[i] = modifiedPower[i] - 1;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (bomb.GetBatteryCount() > 1)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }

            else if (i == 7)  //Margaret
            {
                if (chosenCharIndex.Contains(1))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(8) || chosenCharIndex.Contains(3))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                }
                if (bossIndex == 2)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
                if (bomb.GetPortCount("DVI") > 0)
                {
                    modifiedPower[i] = modifiedPower[i] + bomb.GetPortCount("DVI");
                    //Debug.LogFormat("{0} condition4 is triggered", charOptions[i].name);
                }
            }
            else if (i == 8)  //Mordecai
            {
                if (chosenCharIndex.Contains(13))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(1))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(7))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition3 is triggered", charOptions[i].name);
                }
                if (bossIndex == 5)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition4 is triggered", charOptions[i].name);
                }
                if (bossIndex == 1)
                {
                    if (bomb.IsPortPresent("Serial"))
                    {
                        modifiedPower[i] = modifiedPower[i] + 4;
                        //Debug.LogFormat("{0} condition 5is triggered", charOptions[i].name);
                    }
                    else
                    {
                        modifiedPower[i] = modifiedPower[i] - 4;
                        //Debug.LogFormat("{0} condition 6is triggered", charOptions[i].name);
                    }
                }
                if (bomb.IsIndicatorPresent("MSA"))
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition is7 triggered", charOptions[i].name);
                }
            }
            else if (i == 9)  //MM
            {
                if (chosenCharIndex.Contains(15) || chosenCharIndex.Contains(5))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition is 1triggered", charOptions[i].name);
                }
                if (bossIndex == 6 || bossIndex == 8)
                {
                    modifiedPower[i] = modifiedPower[i] + 3;
                    //Debug.LogFormat("{0} condition is 2triggered", charOptions[i].name);
                }
                if (bomb.GetBatteryCount() < 1)
                {
                    modifiedPower[i] = modifiedPower[i] - 4;
                    //Debug.LogFormat("{0} condition is 3triggered", charOptions[i].name);
                }
                if (bomb.GetSolvableModuleIDs().Any(x => x == "myMom"))
                {
                    modifiedPower[i] = modifiedPower[i] + 10;
                    //Debug.LogFormat("{0} condition is 4triggered", charOptions[i].name);
                }
            }
            else if (i == 10) //Pam
            {
                if (chosenCharIndex.Contains(0))
                {
                    modifiedPower[i] = modifiedPower[i] + 3;
                    //Debug.LogFormat("{0} condition is 1triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(6))
                {
                    modifiedPower[i] = modifiedPower[i] - 1;
                    //Debug.LogFormat("{0} condition is 2triggered", charOptions[i].name);
                }
                if (bomb.IsPortPresent("PS2"))
                {
                    modifiedPower[i] = modifiedPower[i] + bomb.GetPortCount("PS2");
                    //Debug.LogFormat("{0} condition is 3triggered", charOptions[i].name);
                }
            }
            else if (i == 11)  //Pops
            {
                if (bomb.IsPortPresent("Parallel"))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition is 1triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(11))
                {
                    int[] targChars = { 8, 13, 0, 9, 5, 14, 16 };

                    int[] chosenCharIndexNoPops = chosenCharIndex.Where(x => x != 11).ToArray(); 
                    if (targChars.Contains(chosenCharIndexNoPops[0]) && targChars.Contains(chosenCharIndexNoPops[1]))
                    {
                        modifiedPower[i] = modifiedPower[i] + 7;
                        //Debug.LogFormat("{0} condition is 2triggered", charOptions[i].name);
                    }
                }
            }
            else if (i == 12)  //Reginald
            {
                if (bossIndex == 3)
                {
                    modifiedPower[i] = modifiedPower[i] - 2;
                    //Debug.LogFormat("{0} condition is1 triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(4))
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition is 2triggered", charOptions[i].name);
                }
                if (bomb.GetSerialNumberNumbers().Sum() > 8)
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition is 3triggered", charOptions[i].name);
                }
            }
            else if (i == 13)  //Rigby
            {
                if (chosenCharIndex.Contains(8) || chosenCharIndex.Contains(2) || chosenCharIndex.Contains(3))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                List<int> rBossesP = new List<int> { 1, 8, 10 };
                List<int> rBossesM = new List<int> { 4, 9, 11 };
                if (rBossesP.Contains(bossIndex))
                {
                    modifiedPower[i] = modifiedPower[i] + 3;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (rBossesM.Contains(bossIndex))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition3 is triggered", charOptions[i].name);
                }
                if (bomb.IsIndicatorPresent("FRK"))
                {
                    modifiedPower[i] = modifiedPower[i] + 1;
                    //Debug.LogFormat("{0} condition4 is triggered", charOptions[i].name);
                }
            }
            else if (i == 14)  //Skips
            { 
                if (bossIndex == 3)
                {
                    if (bomb.GetSerialNumber()[5] % 2 == 1)
                    {
                        modifiedPower[i] = modifiedPower[i] + 4;
                        //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                    }
                    else
                    {
                        modifiedPower[i] = modifiedPower[i] - 4;
                        //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                    }
                }
                modifiedPower[i] = modifiedPower[i] - bomb.GetPortCount("SteroRCA");
                if (!chosenCharIndex.Contains(11) && !chosenCharIndex.Contains(16))
                {
                    modifiedPower[i] = modifiedPower[i] - 3;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }
            else if (i == 15)  //Starla
            {
                if (chosenCharIndex.Contains(9))
                {
                    modifiedPower[i] = modifiedPower[i] + 3;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(8))
                {
                    modifiedPower[i] = modifiedPower[i] - 2;
                    //Debug.LogFormat("{0} condition2 is triggered", charOptions[i].name);
                }
                if (bomb.GetSolvableModuleIDs().Any(x => x == "simonsStar" || x == "spangledStars"))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 3is triggered", charOptions[i].name);
                }
            }
            else if (i == 16)  //Thomas
            {
                if (bossIndex == 7)
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition1 is triggered", charOptions[i].name);
                }
                if (chosenCharIndex.Contains(9) || chosenCharIndex.Contains(14))
                {
                    modifiedPower[i] = modifiedPower[i] + 2;
                    //Debug.LogFormat("{0} condition 2is triggered", charOptions[i].name);
                }
                if (bomb.IsIndicatorPresent("TRN"))
                {
                    modifiedPower[i] = modifiedPower[i] - 1;
                    //Debug.LogFormat("{0} condition3 is triggered", charOptions[i].name);
                }
            }
        }
        
        Debug.LogFormat("[Regular Show #{0}] Modified Powers are: {1}, {2}, and {3}", moduleId, modifiedPower[chosenCharIndex[0]], modifiedPower[chosenCharIndex[1]], modifiedPower[chosenCharIndex[2]]);     
    }

    void applySpecials()
    {
        if (locIndex == 4 && (bomb.GetSerialNumberNumbers().Sum() < 7 || bomb.GetSolvableModuleIDs().Any( x => x == "sun" || x == "moon")))
        {
            Debug.LogFormat("Regular Show #{0}: Moon special triggered", moduleId);
        }
        if (bossIndex == 0 && chosenCharIndex.Contains(11))
        {
            Debug.LogFormat("Regular Show #{0}: Pops special triggered", moduleId);
        }
        if (chosenCharIndex.Contains(0) && chosenCharIndex.Contains(8) && chosenCharIndex.Contains(13) && (bomb.GetSerialNumber().Any(x => x == 'F' || x == 'R' || x == 'D' || x == 1 || x == 3)))
        {
            modifiedPower[8] = 0;
            modifiedPower[13] = 0; 
            Debug.LogFormat("Regular Show #{0}: Fired special triggered", moduleId);
        }
        if (bossIndex == 5 && bomb.IsIndicatorOn("BOB"))
        {
            Debug.LogFormat("Regular Show #{0}: BOB special triggered", moduleId);
            for (int i = 0; i<= 2; i++)
            {
                modifiedPower[chosenCharIndex[i]] = basePower[chosenCharIndex[i]];
            }
        }
        if (bossIndex == 2 && locIndex == 0 && bomb.GetSolvableModuleIDs().Any(x => x == "coffeebucks"))
        {
            Debug.LogFormat("Regular Show #{0}: Coffee special triggered", moduleId);
            modifiedPower[1] = modifiedPower[1] - 5;
            modifiedPower[3] = modifiedPower[3] - 5;
            modifiedPower[7] = modifiedPower[7] - 5;
            modifiedPower[10] = modifiedPower[10] - 5;
        }
        if (bossIndex == 4 && chosenCharIndex.Contains(13) && bomb.GetSolvableModuleIDs().Any(x => x == "londonUnderground" || x == "britishSlang"))
        {
            Debug.LogFormat("Regular Show #{0}: British special triggered", moduleId);
        }
        if ((bossIndex == 8 && bomb.GetSolvableModuleIDs().Any(x => x == "soda")) || (bossIndex == 11 && bomb.GetSolvableModuleIDs().Any(x => x == "mirror")))
        {
            Debug.LogFormat("Regular Show #{0}: Missing Boss special triggered", moduleId);
            modifiedPower = new List<int> { 1, 1, 1 };
        }
        if (chosenCharIndex.Contains(1) && chosenCharIndex.Contains(7) && chosenCharIndex.Contains(8))
        {
            modifiedPower[8] = 0;
            Debug.LogFormat("Regular Show #{0}: Simp special triggered", moduleId);
        }
        Debug.LogFormat("[Regular Show #{0}] Special Modified Powers are: {1}, {2}, and {3}", moduleId, modifiedPower[chosenCharIndex[0]], modifiedPower[chosenCharIndex[1]], modifiedPower[chosenCharIndex[2]]);
    }

    void pickCorrect()
    {
        List<int> modifiedPowerChosen = new List<int> { modifiedPower[chosenCharIndex[0]], modifiedPower[chosenCharIndex[1]], modifiedPower[chosenCharIndex[2]] };
        if (locIndex == 4 && (bomb.GetSerialNumberNumbers().Sum() < 7 || bomb.GetSolvableModuleIDs().Any(x => x == "sun" || x == "moon")))
        {
            maxPower = modifiedPowerChosen.Min();
            for (int i = 0; i <= 2; i++)
            {
                if (modifiedPowerChosen[i] == maxPower)
                {
                    correctChars.Add(characterMats[i].material.name);
                }
            }
        }
        else
        {
            maxPower = modifiedPowerChosen.Max();
            for (int i = 0; i <= 2; i++)
            {
                if (modifiedPowerChosen[i] == maxPower)
                {
                    correctChars.Add(characterMats[i].material.name);
                }
            }
        }
        if (bossIndex == 4 && chosenCharIndex.Contains(13))
        {
            correctChars = new List<string>{"Rigby (Instance)"};
        }
        if (bossIndex == 0 && chosenCharIndex.Contains(11))
        {
            correctChars = new List<string> {"Pops (Instance)"};
        }
        Debug.LogFormat("Regular Show #{0}: Correct characters are: {1}", moduleId, string.Join(",",correctChars.ToArray()));
    }

    void playVoid()
    {
        if (playedVoid == false)
        { Audio.PlaySoundAtTransform("void", transform); playedVoid = true; }
    }
}

