using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using System.Reflection;
using StardewValley.Objects;

namespace lyl.br
{
    internal sealed class ModEntry : Mod
    {
        private Harmony? harmony;
        private MethodInfo? drawDuringUse;
        private MethodInfo? drawDuringUsePath;
        public override void Entry(IModHelper helper)
        {
            harmony = new Harmony("lyl.battlering");
            helper.Events.GameLoop.GameLaunched += OnLaunched;
            helper.Events.Input.ButtonPressed += OnPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }



        private void OnLaunched(object? sender, GameLaunchedEventArgs e)
        {
            drawDuringUse = AccessTools.Method(typeof(StardewValley.Tools.MeleeWeapon), "drawDuringUse", new Type[] { typeof(int), typeof(int), typeof(SpriteBatch), typeof(Vector2), typeof(Farmer), typeof(string), typeof(int), typeof(bool) });
            drawDuringUsePath = typeof(MyPatch).GetMethod(nameof(MyPatch.WuQiDongHuaBianDa_Prefix));
            harmony?.Patch(drawDuringUse, prefix: new HarmonyMethod(drawDuringUsePath));
        }



        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !e.IsOneSecond) return;
            if (Game1.player.isWearingRing("lyl.br_bigring"))
            {


            }

        }

        private void OnPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.paused || Game1.activeClickableMenu is StardewValley.Menus.InventoryPage) return;

            // 按左键改变当前武器的攻击范围
            if (e.Button.Equals(SButton.MouseLeft))
            {
                if (Game1.player.CurrentTool is MeleeWeapon meleeWeapon)
                {

                    if (Game1.player.isWearingRing("lyl.br.cp_bigring"))
                    {
                        Ring l = Game1.player.leftRing.Value;
                        Ring r = Game1.player.rightRing.Value;
                        if (l!= null && r!= null && l.ItemId.Equals(r.ItemId) && l.ItemId.Equals("lyl.br.cp_bigring"))
                        {
                            meleeWeapon.addedAreaOfEffect.Value = 200;
                            MyPatch.ff = 4f;//武器动画大小倍率
                        }
                        else
                        {
                            meleeWeapon.addedAreaOfEffect.Value = 100;
                            MyPatch.ff = 2f;//武器动画大小倍率
                        }

                    }
                    else
                    {

                        if (Game1.weaponData.TryGetValue(meleeWeapon.ItemId, out var data))
                        {
                            meleeWeapon.addedAreaOfEffect.Value = data.AreaOfEffect;
                        };
                        MyPatch.ff = 1f;//武器动画大小倍率

                    }

                }





            }
        }




    }


}


public static class MyPatch
{

    public static float ff = 1f;

    public static bool WuQiDongHuaBianDa_Prefix(int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition, Farmer f, string weaponItemId, int type, bool isOnSpecial)
    {

        SourceCodeChange.StardewValley_Tools_MeleeWeapon__drawDuringUse(frameOfFarmerAnimation, facingDirection, spriteBatch, playerPosition, f, weaponItemId, type, isOnSpecial, ff);
        return false;
    }

}

public static class SourceCodeChange
{

    public static void StardewValley_Tools_MeleeWeapon__drawDuringUse(int frameOfFarmerAnimation, int facingDirection, SpriteBatch spriteBatch, Vector2 playerPosition, Farmer f, string weaponItemId, int type, bool isOnSpecial, float multiplier)
    {
        float multiplierSize = 4f * multiplier;
        Vector2 center = new Vector2(1f, 15f);
        ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(weaponItemId);
        Texture2D texture = dataOrErrorItem.GetTexture() ?? Tool.weaponsTexture;
        Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
        float drawLayer = f.getDrawLayer();
        FarmerRenderer.FarmerSpriteLayers layer = f.FacingDirection switch
        {
            0 => FarmerRenderer.FarmerSpriteLayers.ToolUp,
            2 => FarmerRenderer.FarmerSpriteLayers.ToolDown,
            _ => FarmerRenderer.FarmerSpriteLayers.TOOL_IN_USE_SIDE,
        };
        float layerDepth = FarmerRenderer.GetLayerDepth(drawLayer, FarmerRenderer.FarmerSpriteLayers.ToolUp);
        float layerDepth2 = FarmerRenderer.GetLayerDepth(drawLayer, layer);
        if (type != 1)
        {
            if (isOnSpecial)
            {
                switch (type)
                {
                    case 3:
                        switch (f.FacingDirection)
                        {
                            case 0:
                                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 44f), sourceRect, Color.White, MathF.PI * -9f / 16f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                                break;
                            case 1:
                                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 4f), sourceRect, Color.White, MathF.PI * -3f / 16f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                                break;
                            case 2:
                                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 52f, playerPosition.Y + 4f), sourceRect, Color.White, -5.105088f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                                break;
                            case 3:
                                spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 56f, playerPosition.Y - 4f), sourceRect, Color.White, MathF.PI * 3f / 16f, new Vector2(15f, 15f), multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                                break;
                        }

                        break;
                    case 2:
                        switch (facingDirection)
                        {
                            case 1:
                                switch (frameOfFarmerAnimation)
                                {
                                    case 0:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 32f - 12f, playerPosition.Y - 80f), sourceRect, Color.White, MathF.PI * -3f / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 1:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 64f - 48f), sourceRect, Color.White, MathF.PI / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 2:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 128f - 16f, playerPosition.Y - 64f - 12f), sourceRect, Color.White, MathF.PI * 3f / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 3:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 72f, playerPosition.Y - 64f + 16f - 32f), sourceRect, Color.White, MathF.PI / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 4:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 96f, playerPosition.Y - 64f + 16f - 16f), sourceRect, Color.White, MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 5:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 96f - 12f, playerPosition.Y - 64f + 16f), sourceRect, Color.White, MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 6:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 96f - 16f, playerPosition.Y - 64f + 40f - 8f), sourceRect, Color.White, MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 7:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 96f - 8f, playerPosition.Y + 40f), sourceRect, Color.White, MathF.PI * 5f / 16f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                }

                                break;
                            case 3:
                                switch (frameOfFarmerAnimation)
                                {
                                    case 0:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 4f + 8f, playerPosition.Y - 56f - 64f), sourceRect, Color.White, MathF.PI / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 1:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 32f, playerPosition.Y - 32f), sourceRect, Color.White, MathF.PI * -5f / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 2:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 12f, playerPosition.Y + 8f), sourceRect, Color.White, MathF.PI * -7f / 8f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 3:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 32f - 4f, playerPosition.Y + 8f), sourceRect, Color.White, MathF.PI * -3f / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 4:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f - 24f, playerPosition.Y + 64f + 12f - 64f), sourceRect, Color.White, 4.31969f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 5:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 20f, playerPosition.Y + 64f + 40f - 64f), sourceRect, Color.White, 3.926991f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 6:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, playerPosition.Y + 64f + 56f), sourceRect, Color.White, 3.926991f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 7:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 8f, playerPosition.Y + 64f + 64f), sourceRect, Color.White, 3.7306414f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                }

                                break;
                            default:
                                switch (frameOfFarmerAnimation)
                                {
                                    case 0:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 24f, playerPosition.Y - 21f - 8f - 64f), sourceRect, Color.White, -MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 1:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f - 64f + 4f), sourceRect, Color.White, -MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 2:
                                        spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 20f - 64f), sourceRect, Color.White, -MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        break;
                                    case 3:
                                        if (facingDirection == 2)
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), sourceRect, Color.White, -3.926991f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }
                                        else
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 21f + 32f - 64f), sourceRect, Color.White, -MathF.PI / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }

                                        break;
                                    case 4:
                                        if (facingDirection == 2)
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f + 8f, playerPosition.Y + 32f), sourceRect, Color.White, -3.926991f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }

                                        break;
                                    case 5:
                                        if (facingDirection == 2)
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f - 20f), sourceRect, Color.White, MathF.PI * 3f / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }

                                        break;
                                    case 6:
                                        if (facingDirection == 2)
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 54f), sourceRect, Color.White, MathF.PI * 3f / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }

                                        break;
                                    case 7:
                                        if (facingDirection == 2)
                                        {
                                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f + 12f, playerPosition.Y + 64f + 58f), sourceRect, Color.White, MathF.PI * 3f / 4f, Vector2.Zero, multiplierSize, SpriteEffects.None, layerDepth2);
                                        }

                                        break;
                                }

                                if (f.FacingDirection == 0)
                                {
                                    f.FarmerRenderer.draw(spriteBatch, f.FarmerSprite, f.FarmerSprite.SourceRect, f.getLocalPosition(Game1.viewport), new Vector2(0f, (f.yOffset + 128f - (float)(f.GetBoundingBox().Height / 2)) / 4f + 4f), layerDepth2, Color.White, 0f, f);
                                }

                                break;
                        }

                        break;
                }

                return;
            }

            switch (facingDirection)
            {
                case 1:
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 40f, playerPosition.Y - 64f + 8f), sourceRect, Color.White, -MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth);
                            break;
                        case 1:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 64f + 28f), sourceRect, Color.White, 0f, center, multiplierSize, SpriteEffects.None, layerDepth);
                            break;
                        case 2:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 16f), sourceRect, Color.White, MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 3:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 4f), sourceRect, Color.White, MathF.PI / 2f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 4:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 28f, playerPosition.Y + 4f), sourceRect, Color.White, MathF.PI * 5f / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 5:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), sourceRect, Color.White, MathF.PI * 3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 6:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 48f, playerPosition.Y + 4f), sourceRect, Color.White, MathF.PI * 3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 7:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y + 64f + 12f), sourceRect, Color.White, 1.9634954f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                    }

                    break;
                case 3:
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X - 16f, playerPosition.Y - 64f - 16f), sourceRect, Color.White, MathF.PI / 4f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth);
                            break;
                        case 1:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X - 48f, playerPosition.Y - 64f + 20f), sourceRect, Color.White, 0f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth);
                            break;
                        case 2:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X - 64f + 32f, playerPosition.Y + 16f), sourceRect, Color.White, -MathF.PI / 4f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                            break;
                        case 3:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 4f, playerPosition.Y + 44f), sourceRect, Color.White, -MathF.PI / 2f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                            break;
                        case 4:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 52f), sourceRect, Color.White, MathF.PI * -5f / 8f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                            break;
                        case 5:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), sourceRect, Color.White, MathF.PI * -3f / 4f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                            break;
                        case 6:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 80f, playerPosition.Y + 40f), sourceRect, Color.White, MathF.PI * -3f / 4f, center, multiplierSize, SpriteEffects.FlipHorizontally, layerDepth2);
                            break;
                        case 7:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X - 44f, playerPosition.Y + 96f), sourceRect, Color.White, -5.105088f, center, multiplierSize, SpriteEffects.FlipVertically, layerDepth2);
                            break;
                    }

                    break;
                case 0:
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 32f), sourceRect, Color.White, MathF.PI * -3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 1:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 48f), sourceRect, Color.White, -MathF.PI / 2f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 2:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), sourceRect, Color.White, MathF.PI * -3f / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 3:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 48f, playerPosition.Y - 52f), sourceRect, Color.White, -MathF.PI / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 4:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 40f), sourceRect, Color.White, 0f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 5:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), sourceRect, Color.White, MathF.PI / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 6:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f, playerPosition.Y - 40f), sourceRect, Color.White, MathF.PI / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 7:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 44f, playerPosition.Y + 64f), sourceRect, Color.White, -1.9634954f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                    }

                    break;
                case 2:
                    switch (frameOfFarmerAnimation)
                    {
                        case 0:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 56f, playerPosition.Y - 16f), sourceRect, Color.White, MathF.PI / 8f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 1:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 52f, playerPosition.Y - 8f), sourceRect, Color.White, MathF.PI / 2f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 2:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 40f, playerPosition.Y), sourceRect, Color.White, MathF.PI / 2f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 3:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 16f, playerPosition.Y + 4f), sourceRect, Color.White, MathF.PI * 3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 4:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 8f, playerPosition.Y + 8f), sourceRect, Color.White, MathF.PI, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 5:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 12f, playerPosition.Y), sourceRect, Color.White, 3.5342917f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 6:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 12f, playerPosition.Y), sourceRect, Color.White, 3.5342917f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                        case 7:
                            spriteBatch.Draw(texture, new Vector2(playerPosition.X + 44f, playerPosition.Y + 64f), sourceRect, Color.White, -5.105088f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                            break;
                    }

                    break;
            }

            return;
        }

        frameOfFarmerAnimation %= 2;
        switch (facingDirection)
        {
            case 1:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 16f), sourceRect, Color.White, MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                    case 1:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 8f, playerPosition.Y - 24f), sourceRect, Color.White, MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                }

                break;
            case 3:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 16f, playerPosition.Y - 16f), sourceRect, Color.White, MathF.PI * -3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                    case 1:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 8f, playerPosition.Y - 24f), sourceRect, Color.White, MathF.PI * -3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                }

                break;
            case 0:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 4f, playerPosition.Y - 40f), sourceRect, Color.White, -MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                    case 1:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 64f - 16f, playerPosition.Y - 48f), sourceRect, Color.White, -MathF.PI / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                }

                break;
            case 2:
                switch (frameOfFarmerAnimation)
                {
                    case 0:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 32f, playerPosition.Y - 8f), sourceRect, Color.White, MathF.PI * 3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                    case 1:
                        spriteBatch.Draw(texture, new Vector2(playerPosition.X + 21f, playerPosition.Y + 20f), sourceRect, Color.White, MathF.PI * 3f / 4f, center, multiplierSize, SpriteEffects.None, layerDepth2);
                        break;
                }

                break;
        }
    }


}







