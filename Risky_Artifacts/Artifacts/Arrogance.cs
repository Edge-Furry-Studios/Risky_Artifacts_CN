﻿using R2API;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Risky_Artifacts.Artifacts
{
    public class Arrogance
    {
        public static bool enabled = true;
        public static ArtifactDef artifact;

        public static int runMountainCount = 0;
        public static int stageMountainCount = 0;

        public Arrogance()
        {
            if (!enabled) return;
            artifact = ScriptableObject.CreateInstance<ArtifactDef>();
            artifact.cachedName = "RiskyArtifactOfArrogance";
            artifact.nameToken = "RISKYARTIFACTS_ARROGANCE_NAME";
            artifact.descriptionToken = "RISKYARTIFACTS_ARROGANCE_DESC";
            artifact.smallIconDeselectedSprite = RiskyArtifactsPlugin.assetBundle.LoadAsset<Sprite>("texArrogDisabled.png");
            artifact.smallIconSelectedSprite = RiskyArtifactsPlugin.assetBundle.LoadAsset<Sprite>("texArrogEnabled.png");
            RiskyArtifactsPlugin.FixScriptableObjectName(artifact);
            ContentAddition.AddArtifactDef(artifact);
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ProperSave"))
            {
                HandleSave();
            }


            On.RoR2.ShrineBossBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                if (NetworkServer.active)
                {
                    stageMountainCount++;
                }
                orig(self, interactor);
            };

            On.RoR2.Run.Start += (orig, self) =>
            {
                runMountainCount = 0;
                stageMountainCount = 0;
                orig(self);
            };

            On.RoR2.Stage.Start += (orig, self) =>
            {
                runMountainCount += stageMountainCount;
                stageMountainCount = 0;
                orig(self);

                if (RunArtifactManager.instance.IsArtifactEnabled(artifact.artifactIndex))
                {
                    if (TeleporterInteraction.instance)
                    {
                        for (int i = 0; i < runMountainCount; i++)
                        {
                            TeleporterInteraction.instance.AddShrineStack();
                        }
                    }
                }
            };

            
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleSave()
        {
            ProperSave.SaveFile.OnGatgherSaveData += Save;
            ProperSave.Loading.OnLoadingEnded += Load;
        }

        public static void Save(Dictionary<string, object> dict)
        {
            dict.Add("riskyArtifact.Arrogance.runCount", runMountainCount);
        }

        public void Load(ProperSave.SaveFile save)
        {
            runMountainCount = save.GetModdedData<int>("riskyArtifact.Arrogance.runCount");
        }
    }
}
