using Vintagestory.API.Common;
using Foundation.Extensions;
using System;
using Vintagestory.API.Server;

namespace vscraftingspeedmod.src
{
    public class VSCraftingSpeedMod : ModSystem
    {
        private ICoreAPI api;

        public override void StartPre(ICoreAPI api)
        {
            VSCraftingSpeedModConfigFile.Current = api.LoadOrCreateConfig<VSCraftingSpeedModConfigFile>(typeof(VSCraftingSpeedMod).Name + ".json");
            api.World.Config.SetFloat("totalGrowthDaysSpeedMultiplier", VSCraftingSpeedModConfigFile.Current.TotalGrowthDaysSpeedMultiplier);
            api.World.Config.SetFloat("meltingDurationSpeedMultiplier", VSCraftingSpeedModConfigFile.Current.MeltingDurationSpeedMultiplier);
            base.StartPre(api);
        }


        public override void Start(ICoreAPI api)
        {
            this.api = api;
            base.Start(api);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);
            AdjustBlockSpeeds();
            AdjustItemSpeeds();
        }

        public override double ExecuteOrder()
        {
            /// Worldgen:
            /// - GenTerra: 0 
            /// - RockStrata: 0.1
            /// - Deposits: 0.2
            /// - Caves: 0.3
            /// - Blocklayers: 0.4
            /// Asset Loading
            /// - Json Overrides loader: 0.05
            /// - Load hardcoded mantle block: 0.1
            /// - Block and Item Loader: 0.2
            /// - Recipes (Smithing, Knapping, Clayforming, Grid recipes, Alloys) Loader: 1
            /// 
            return 1.1;
        }

        private void AdjustBlockSpeeds()
        {
            foreach (var block in api.World.Blocks)
            {
                SetBlockTotalGrowthDaysSpeed(block, api.World.Config.GetFloat("totalGrowthDaysSpeedMultiplier"));
            }
        }

        private void AdjustItemSpeeds()
        {
            foreach (var item in api.World.Items)
            {
                SetItemMeltingSpeed(item, api.World.Config.GetFloat("totalRecipeSpeedMultiplier"));
            }
        }

        private void SetItemMeltingSpeed(Item item, float speedModifier)
        {
            if (item.CombustibleProps == null)
                return;

            item.CombustibleProps.MeltingDuration = item.CombustibleProps.MeltingDuration * speedModifier;
        }

        private void SetBlockTotalGrowthDaysSpeed(Block block, float speedModifier)
        {
            if (block.CropProps == null)
                return;
            block.CropProps.TotalGrowthDays = block.CropProps.TotalGrowthDays * speedModifier;
        }
    }

    public class VSCraftingSpeedModConfigFile
    {
        public static VSCraftingSpeedModConfigFile Current { get; set; }
        public float MeltingDurationSpeedMultiplier = 0.1F;

        public float TotalGrowthDaysSpeedMultiplier = 0.1F;
    }
}
