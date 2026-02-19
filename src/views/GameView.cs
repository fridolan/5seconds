using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace fiveSeconds
{
    public class GameView : View
    {
        private Shader TileShader;

        public Vector2 CameraPos = new(0f, 0f);
        public float CameraAngle = 0f;

        public Vector2i HoveredTile;

        public Entity? ControlledEntity;
        private Mesh SpecialTileMesh = new();
        private Mesh EffectMesh = new();

        private Vector2 margin = new Vector2(Window.Width, Window.Width) / 128;

        public AbilitySlot[] AbilitySlots = AbilitySlot.InitArray();
        public Ability MoveAbility;
        public Ability CatchAbility;

        public override void OnLoad()
        {
            TileShader = new Shader("tileAtlasV.glsl", "tileAtlasF.glsl");
            Zoom = 1 / 8f;
        }

        public override void OnRenderFrame(FrameEventArgs args)
        {
            if (Client.Game?.CurrentStage == null) return;

            float dT = (float)args.Time;

            RenderStage(dT);
            RenderHUD(dT);

            HudRenderer.Draw(true);
            TextHandler.renderer.Draw();
        }


        private HudElement remainingTimeBar;
        private ButtonElement hpBarEmpty;
        private ButtonElement hpBarFilling;
        private ButtonElement manaBarEmpty;
        private ButtonElement manaBarFilling;
        private HudElement abilitySlotsBackground;

        public GameView()
        {
            remainingTimeBar = new()
            {
                Position = (0, 0),
                Size = (Window.Width, Window.Height * 0.02f),
                TextureId = Textures.slightly_transparent_white,
            };

            hpBarEmpty = new()
            {
                BaseElement = new()
                {
                    Position = (0 + Window.Height * 0.01f, 0 + Window.Height * 0.03f),
                    Size = (Textures.INFO_hp_bar.SizeRatio * Window.Height * 0.07f, Window.Height * 0.07f),
                    TextureId = Textures.hp_bar_empty,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = true,
                RenderHeader = false,
                RenderHover = false,
                TextSize = 0.7f,
            };

            hpBarFilling = new()
            {
                BaseElement = new()
                {
                    Position = hpBarEmpty.BaseElement.Position,
                    Size = hpBarEmpty.BaseElement.Size,
                    TextureId = Textures.hp_bar,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = false,
                RenderHeader = false,
                RenderHover = false,
            };

            manaBarEmpty = new()
            {
                BaseElement = new()
                {
                    Position = (0 + Window.Height * 0.01f, hpBarEmpty.LowerLeft.Y + Window.Height * 0.00f),
                    Size = (Textures.INFO_hp_bar.SizeRatio * Window.Height * 0.07f, Window.Height * 0.07f),
                    TextureId = Textures.mana_bar_empty,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = true,
                RenderHeader = false,
                RenderHover = false,
                TextSize = 0.7f,
            };

            manaBarFilling = new()
            {
                BaseElement = new()
                {
                    Position = manaBarEmpty.BaseElement.Position,
                    Size = manaBarEmpty.BaseElement.Size,
                    TextureId = Textures.mana_bar,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = false,
                RenderHeader = false,
                RenderHover = false,
            };

            TextureInfo slotsInfo = Textures.INFO_actions_slots;

            abilitySlotsBackground = new()
            {
                Position = (Window.Width / 2 - Window.Width / 3, Window.Height * 0.82f),
                Size = (2 * Window.Width / 3, 2 * Window.Width / 3 / slotsInfo.SizeRatio),
                TextureId = Textures.actions_slots
            };

            float margin = slotsInfo.Margin.Y / (float)slotsInfo.Size.Y * abilitySlotsBackground.Size.Y;
            Vector2 innerPos = abilitySlotsBackground.Position + new Vector2(margin, margin);
            Vector2 innerSize = abilitySlotsBackground.Size - new Vector2(margin, margin) * 2;
            Vector2 elementSize = (innerSize.Y, innerSize.Y);
            float distanceBetweenElements = (innerSize.X - elementSize.X * AbilitySlots.Length) / (AbilitySlots.Length - 1);

            Console.WriteLine($"{abilitySlotsBackground.Position - innerPos}");

            for (int i = 0; i < AbilitySlots.Length; i++)
            {
                AbilitySlots[i].Position = innerPos + new Vector2(elementSize.X + distanceBetweenElements, 0) * i;
                AbilitySlots[i].Size = elementSize;
            }
        }

        private void RenderHUD(float dT)
        {
            Stage stage = Client.Game.CurrentStage;

            if (Client.Game == null) return;

            remainingTimeBar.Size.X = (float)(Client.Game.InputTimeLeft / (double)Client.Game.InputPhaseLength) * Window.Width;
            remainingTimeBar.Render();

            if (stage.PlayerEntity is ICombat c)
            {
                { // HP & Mana HUD Bars
                    hpBarEmpty.Text = $"{c.Stats.CurrentHealth} / {c.Stats.MaxHealth}";
                    manaBarEmpty.Text = $"{c.Stats.CurrentMana} / {c.Stats.MaxMana}";
                    TextureInfo hpBarInfo = Textures.INFO_hp_bar;

                    hpBarFilling.BaseElement.Size.X = hpBarEmpty.BaseElement.Size.X *
                    ((hpBarInfo.Inner.X / (float)hpBarInfo.Size.X) * c.Stats.CurrentHealth / (float)c.Stats.MaxHealth + hpBarInfo.Margin.X / (float)hpBarInfo.Size.X);
                    hpBarFilling.BaseElement.TexCoords =
                    (0, 0, ((hpBarInfo.Inner.X / (float)hpBarInfo.Size.X) * c.Stats.CurrentHealth / (float)c.Stats.MaxHealth + hpBarInfo.Margin.X / (float)hpBarInfo.Size.X), 1);

                    manaBarFilling.BaseElement.Size.X = manaBarEmpty.BaseElement.Size.X *
                    ((hpBarInfo.Inner.X / (float)hpBarInfo.Size.X) * c.Stats.CurrentMana / (float)c.Stats.MaxMana + hpBarInfo.Margin.X / (float)hpBarInfo.Size.X);
                    manaBarFilling.BaseElement.TexCoords =
                    (0, 0, ((hpBarInfo.Inner.X / (float)hpBarInfo.Size.X) * c.Stats.CurrentMana / (float)c.Stats.MaxMana + hpBarInfo.Margin.X / (float)hpBarInfo.Size.X), 1);

                    hpBarEmpty.Render(dT);
                    manaBarEmpty.Render(dT);
                    hpBarFilling.Render(dT);
                    manaBarFilling.Render(dT);
                }
                { // Other Abilities
                    MoveAbility = c.Abilities[0];
                    CatchAbility = c.Abilities[1];
                }
                { // Ability Slots
                    AbilitySlots[0].Ability = c.Abilities[2];
                    AbilitySlots[1].Ability = c.Abilities[3];
                    abilitySlotsBackground.Render();

                    for (int i = 0; i < AbilitySlots.Length; i++)
                    {
                        AbilitySlot slot = AbilitySlots[i];
                        HudElement element = new()
                        {

                            Position = slot.Position,
                            Size = slot.Size,
                            TextureId = Textures.slot,
                            RenderInnerElement = true,
                            InnerMarginRatio = Textures.INFO_slot.MarginToSizeRatio,
                            InnerElement = new()
                            {
                                TextureId = slot.Ability == null ? 0 : slot.Ability.Icon,
                            }
                        };

                        element.Render();
                    }
                }
                { // ActionList
                    ActionList actionList = stage.PlayerEntity.ActionList;
                    List<AbilityAction> actions = actionList.Actions.GetRange(actionList.NextActionIndex, actionList.Actions.Count - actionList.NextActionIndex);

                    for (int i = 0; i < actions.Count; i++)
                    {
                        AbilityAction action = actions[i];

                        Vector2 size = (Window.Width / 16, Window.Width / 16);
                        HudElement element = new()
                        {
                            Position = hpBarEmpty.UpperRight + (margin.X, 0) + (i * (size.X + margin.X), 0),
                            Size = size,
                            TextureId = Textures.slot,
                            InnerMarginRatio = Textures.INFO_slot.MarginToSizeRatio,
                            InnerElement = new()
                            {
                                TextureId = action.Ability.Icon,
                            },
                            RenderInnerElement = true,
                        };

                        element.Render();
                    }
                    ;
                }
            }
        }

        private void RenderStage(float dT)
        {
            if (Client.Game.CurrentStage == null) return;

            Stage stage = Client.Game.CurrentStage;

            if (stage.TileMeshDirty) stage.CreateTileMesh();
            if (stage.EntityMeshDirty) stage.CreateEntityMesh();

            Matrix4 projection = Matrix4.CreateOrthographic(2f * Window.aspectRatio / View.CurrentView.Zoom, 2f / View.CurrentView.Zoom, -10f, 10f);

            Matrix4 view =
                Matrix4.CreateTranslation(-CameraPos.X, -CameraPos.Y, 0f) *
                Matrix4.CreateRotationZ(-CameraAngle);

            TileShader.Use();
            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uView"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uProjection"), false, ref projection);
            GL.Uniform1(GL.GetUniformLocation(TileShader.Handle, "uTilesPerRow"), 8f);


            // Render Tiles
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Tile.TextureId);
            TileShader.SetInt("uAtlas", 0);

            Matrix4 model =
                Matrix4.CreateRotationZ(0f) *
                Matrix4.CreateTranslation(0, 0, 0f);

            GL.UniformMatrix4(GL.GetUniformLocation(TileShader.Handle, "uModel"), false, ref model);

            GL.BindVertexArray(stage.TileMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, stage.TileMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

            // Render Entities
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Entity.TextureId);
            TileShader.SetInt("uAtlas", 0);

            GL.BindVertexArray(stage.EntityMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, stage.EntityMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

            // Render Special Tile Layer
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Textures.special_tile_atlas);
            TileShader.SetInt("uAtlas", 0);

            SpecialTileMesh.Clear();
            if (stage.ValidTilePos(HoveredTile))
            {
                SpecialTileMesh.RectAt(HoveredTile, SpecialTileIndeces.SELECT, (1, 1));
            }

            // Path lines
            if (stage.PlayerEntity is Entity playerEntity)
            {
                List<Vector2i> goals = [.. playerEntity.ActionList.Actions.Select(action => action.Input).Where(action => action is IInputStartGoal).Select(a => ((IInputStartGoal)a).Goal)];
                goals.ForEach((g) =>
                {
                    SpecialTileMesh.RectAt(g, SpecialTileIndeces.HIGHLIGHT, (1, 1));
                });
            }

            // Entity hp & mana bars
            stage.EntityList.ForEach((e) =>
            {
                if (e is ICombat c)
                {
                    SpecialTileMesh.AtlasRect(e.Position - (0, 1), (c.Stats.HealthPercentage, 1), (0, 0), (c.Stats.HealthPercentage, 1), SpecialTileIndeces.HP_BAR);
                    SpecialTileMesh.AtlasRect(e.Position - (0, 1), (c.Stats.ManaPercentage, 1), (0, 0), (c.Stats.ManaPercentage, 1), SpecialTileIndeces.MANA_BAR);
                }
            });

            SpecialTileMesh.UploadToGPU();

            GL.BindVertexArray(SpecialTileMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, SpecialTileMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

            // Render Effects
            EffectMesh.Clear();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Textures.effect_atlas);

            stage.Projectiles.ForEach((p) =>
            {
                p.DrawCallback(p,EffectMesh, dT);
            });

            EffectMesh.UploadToGPU();

            GL.BindVertexArray(EffectMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, EffectMesh.IndexCount, DrawElementsType.UnsignedInt, 0);

        }

        public override void HandleInputs(FrameEventArgs args)
        {
            float dT = (float)args.Time;

            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            HandleCameraInputs(args);
            HandleGameplayInputs(args);
        }

        public void HandleCameraInputs(FrameEventArgs args)
        {
            float dT = (float)args.Time;

            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            //Vector2i tilePosition = ScreenToTilePosition(Input.mousePos, CameraAngle - Game.ships[0].Angle);

            // Zoom
            if (mouse.ScrollDelta.Y < 0)
            {
                Zoom = Zoom / 2;
            }
            if (mouse.ScrollDelta.Y > 0)
            {
                Zoom = Zoom * 2;
            }

            float normalizingFactor =
                (Keybind.UP.IsDown() ^ Keybind.DOWN.IsDown())
                && (Keybind.LEFT.IsDown() ^ Keybind.RIGHT.IsDown()) ? MathF.Sqrt(2) / 2f : 1;

            float magnitude = dT * normalizingFactor * 2;

            // Camera Movement
            if (Keybind.UP.IsDown())
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle);
            }
            if (Keybind.DOWN.IsDown())
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle + MathF.PI);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle + MathF.PI);
            }
            if (Keybind.LEFT.IsDown())
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle - MathF.PI / 2);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle - MathF.PI / 2);
            }
            if (Keybind.RIGHT.IsDown())
            {
                CameraPos.Y += magnitude / Zoom * MathF.Cos(-CameraAngle + MathF.PI / 2);
                CameraPos.X += magnitude / Zoom * MathF.Sin(-CameraAngle + MathF.PI / 2);
            }
        }

        private AbilityAction? newAction = null;

        public void HandleGameplayInputs(FrameEventArgs args)
        {
            float dT = (float)args.Time;


            KeyboardState keyboard = Input.keyboard;
            MouseState mouse = Input.mouse;

            Stage stage = Client.Game.CurrentStage;

            HoveredTile = ScreenToTilePosition(mouse.Position, 0);
            Vector2 hoveredTileF = ScreenToTilePositionFloat(mouse.Position, 0);
            bool validHover = stage.ValidTilePos(HoveredTile);

            if (Window.Client == null) return;

            Entity? playerEntity = stage.PlayerEntity;

            if (Keybind.ZERO.IsPressed())
            {
                //((ICombat)playerEntity).Stats.CurrentMana -= 5;
            }

            // Console.WriteLine($"mTP {HoveredTile} {validHover} {playerEntity}");

            if (Keybind.LEFTCLICK.IsPressed())
            {
                Console.WriteLine(stage.EntityList[0].ID);
            }

            if (validHover && playerEntity != null)
            {
                /*  if (Keybind.LEFTCLICK.IsPressed())
                 {
                     bool relative = Keybind.SHIFT.IsDown();
                     MoveEntityAbility action = new()
                     {
                         CancelOnDisplace = true,
                         EntityID = playerEntity.ID,
                         Relative = relative,
                         Goal = HoveredTile,
                         Start = playerEntity.Position,
                     };

                     //Console.WriteLine($"Move Player Entity {playerEntity.ID} {HoveredTile}");

                     playerEntity.AddAction(action);
                 } */

                Entity? entityAtHover = stage.Entities[HoveredTile.Y][HoveredTile.X];

                /*                 if (Keybind.RIGHTCLICK.IsPressed() && entityAtHover != null)
                                {
                                    CatchEntityAbility action = new()
                                    {
                                        CancelOnDisplace = false,
                                        EntityID = playerEntity.ID,
                                        ToEntityID = entityAtHover.ID,
                                    };

                                    playerEntity.AddAction(action);
                                } */

                AbilityContext context = new()
                {
                    HoveredEntity = entityAtHover,
                    SourceEntity = playerEntity,
                    HoveredTile = HoveredTile,
                    Stage = stage,
                };

                (Keybind keybind, Ability ability)[] abilityKeybinds = [
                    (Keybind.LEFTCLICK, MoveAbility),
                    (Keybind.ONE, AbilitySlots[0].Ability),
                    (Keybind.TWO, AbilitySlots[1].Ability),
                    (Keybind.THREE, AbilitySlots[2].Ability),
                    (Keybind.FOUR, AbilitySlots[3].Ability),
                    (Keybind.FIVE, AbilitySlots[4].Ability),
                    (Keybind.SIX, AbilitySlots[5].Ability),
                    (Keybind.SEVEN, AbilitySlots[6].Ability),
                    (Keybind.EIGHT, AbilitySlots[7].Ability),
                    (Keybind.NINE, AbilitySlots[8].Ability),
                    (Keybind.ZERO, AbilitySlots[9].Ability),
                ];


                for (int i = 0; i < abilityKeybinds.Length; i++)
                {
                    Keybind keybind = abilityKeybinds[i].keybind;
                    Ability ability = abilityKeybinds[i].ability;
                    if (ability == null) continue;

                    if (keybind.IsPressed())
                    {
                        Console.WriteLine("init abilityinput");
                        newAction = new()
                        {
                            Ability = ability,
                            Input = ability.GetNewAbilityInput()
                        };
                    }
                    if (keybind.IsDown() && newAction != null)
                    {
                        if (Keybind.CANCEL.IsPressed()) newAction = null;
                        else newAction.Input.HandleAbilityInput(context);
                    }
                    if (keybind.IsReleased() && newAction != null)
                    {
                        Console.WriteLine("submit abilityinput");
                        //ability.SubmitAbilityInput(playerEntity);

                        if (newAction.Input.Complete)
                        {
                            playerEntity.AddAction(newAction);
                            newAction.Input.Updating = false;
                        }
                        else newAction = null;
                    }
                }

                if (Keybind.BACK.IsPressed())
                {
                    if (playerEntity.ActionList.Actions.Count > 0)
                    {
                        playerEntity.RemoveActionAtIndex(playerEntity.ActionList.Actions.Count - 1);
                    }
                }
            }

            if (Keybind.INTERACT.IsPressed())
            {
                Pathfinder.Test(Client.Game.CurrentStage.Tiles);
            }
        }

        private Vector2i ScreenToTilePosition(Vector2 mousePos, float angle)
        {
            return (Vector2i)ScreenToTilePositionFloat(mousePos, angle);
        }

        private Vector2 ScreenToTilePositionFloat(Vector2 mousePos, float angle)
        {
            Vector2 m = (Window.Width / 2, Window.Height / 2) + RotateVector(new Vector2(mousePos.X, Window.Height - mousePos.Y) - (Window.Width / 2, Window.Height / 2), angle);
            Vector2 offset = CameraPos;

            Vector2 tile = (
                (int)Math.Floor((m.X / Window.Width * 2f - 1.0f) / Zoom * Window.aspectRatio + offset.X /* + 0.5f */),
                (int)Math.Floor((m.Y - Window.Height / 2) / Window.Height * 2 / Zoom + offset.Y /* + 0.5f */));

            /* tile = (
                Math.Max(Math.Min(tile.X, MapWidth - 1), 0),
                Math.Max(Math.Min(tile.Y, MapHeight - 1), 0)); */

            //Console.WriteLine($"Tile at {tile} {mousePos}");

            return (tile.X, tile.Y);
        }

        private static Vector2 RotateVector(Vector2 vec, float angle)
        {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            return new Vector2(
                vec.X * cos - vec.Y * sin,
                vec.X * sin + vec.Y * cos
            );
        }
    }
}