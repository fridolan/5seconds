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

        public AbilitySlot[] AbilitySlots = AbilitySlot.InitArray();

        public override void OnLoad()
        {
            TileShader = new Shader("tileAtlasV.glsl", "tileAtlasF.glsl");
            Zoom = 1 / 8f;
        }

        public override void OnRenderFrame(FrameEventArgs args)
        {
            RenderStage();
            RenderHUD((float)args.Time);

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
                    Size = (Textures.INFO_hp_bar.SizeRatio * Window.Height * 0.1f, Window.Height * 0.1f),
                    TextureId = Textures.hp_bar_empty,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = true,
                RenderHeader = false,
                RenderHover = false,
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
                    Size = (Textures.INFO_hp_bar.SizeRatio * Window.Height * 0.1f, Window.Height * 0.1f),
                    TextureId = Textures.mana_bar_empty,
                },
                RenderBaseElement = true,
                RenderTexture = false,
                RenderBorder = false,
                RenderInnerText = true,
                RenderHeader = false,
                RenderHover = false,
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
            Vector2 innerSize = abilitySlotsBackground.Size - new Vector2(margin,margin) * 2;
            Vector2 elementSize = (innerSize.Y, innerSize.Y);
            float distanceBetweenElements = (innerSize.X - elementSize.X * AbilitySlots.Length) / (AbilitySlots.Length - 1);

            Console.WriteLine($"{abilitySlotsBackground.Position - innerPos}");

            for(int i = 0; i < AbilitySlots.Length; i++)
            {
                AbilitySlots[i].Position = innerPos + new Vector2(elementSize.X + distanceBetweenElements, 0) * i;
                AbilitySlots[i].Size = elementSize;
            }
        }

        private void RenderHUD(float dT)
        {
            Stage stage = Client.Game.CurrentStage;

            if (Client.Game == null) return;

            remainingTimeBar.Size.X = (Client.Game.InputTimeLeft / Client.Game.InputPhaseLength) * Window.Width;
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
                { // Ability Slots
                    AbilitySlots[0].Ability = c.Abilities[0];
                    abilitySlotsBackground.Render();

                    for(int i = 0; i < AbilitySlots.Length; i++)
                    {
                        HudElement element = new HudElement()
                        {
                            Position = AbilitySlots[i].Position,
                            Size = AbilitySlots[i].Size,
                            TextureId = Textures.slot,
                            RenderInnerElement = true,
                            InnerMarginRatio = Textures.INFO_slot.MarginToSizeRatio,
                            InnerElement = new()
                            {
                                TextureId = AbilitySlots[i].Ability.Icon,
                            }
                        };

                        element.Render();
                    }
                }
            }
        }

        private void RenderStage()
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
                SpecialTileMesh.RectAt(HoveredTile, 0, (1, 1));
            }

            // Path lines
            if (stage.PlayerEntity is Entity playerEntity)
            {
                List<Vector2i> goals = [.. playerEntity.ActionList.Actions.OfType<IStartGoalInput>().Select(a => a.Goal)];
                goals.ForEach((g) =>
                {
                    SpecialTileMesh.RectAt(g, 1, (1, 1));
                });
            }

            SpecialTileMesh.UploadToGPU();

            GL.BindVertexArray(SpecialTileMesh.Vao);
            GL.DrawElements(PrimitiveType.Triangles, SpecialTileMesh.IndexCount, DrawElementsType.UnsignedInt, 0);
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
                if (Keybind.LEFTCLICK.IsPressed())
                {
                    bool relative = Keybind.SHIFT.IsDown();
                    MoveEntityAction action = new()
                    {
                        CancelOnDisplace = true,
                        EntityID = playerEntity.ID,
                        Relative = relative,
                        Goal = HoveredTile,
                        Start = playerEntity.Position,
                    };

                    //Console.WriteLine($"Move Player Entity {playerEntity.ID} {HoveredTile}");

                    playerEntity.ActionList.AddActionClient(action);
                }

                Entity? entityAtHover = stage.Entities[HoveredTile.Y][HoveredTile.X];

                if (Keybind.RIGHTCLICK.IsPressed() && entityAtHover != null)
                {
                    CatchEntityAction action = new()
                    {
                        CancelOnDisplace = false,
                        EntityID = playerEntity.ID,
                        ToEntityID = entityAtHover.ID,
                    };

                    playerEntity.ActionList.AddActionClient(action);
                }

                AbilityContext context = new()
                {
                  SourceEntity = playerEntity,
                  TargetEntity = entityAtHover,
                  Stage = stage,
                  TargetTile = HoveredTile,  
                };

                if (Keybind.ONE.IsPressed()) Ability.Use(AbilitySlots[0].Ability, context);
                if (Keybind.TWO.IsPressed()) Ability.Use(AbilitySlots[1].Ability, context);
                if (Keybind.THREE.IsPressed()) Ability.Use(AbilitySlots[2].Ability, context);
                if (Keybind.FOUR.IsPressed()) Ability.Use(AbilitySlots[3].Ability, context);
                if (Keybind.FIVE.IsPressed()) Ability.Use(AbilitySlots[4].Ability, context);
                if (Keybind.SIX.IsPressed()) Ability.Use(AbilitySlots[5].Ability, context);
                if (Keybind.SEVEN.IsPressed()) Ability.Use(AbilitySlots[6].Ability, context);
                if (Keybind.EIGHT.IsPressed()) Ability.Use(AbilitySlots[7].Ability, context);
                if (Keybind.NINE.IsPressed()) Ability.Use(AbilitySlots[8].Ability, context);
                if (Keybind.ZERO.IsPressed()) Ability.Use(AbilitySlots[9].Ability, context);
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