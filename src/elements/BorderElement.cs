using OpenTK.Mathematics;

namespace fiveSeconds
{
    public class BorderElement() : WithBaseElement
    {
        public int borderSize = 4;
        public int borderTexture = 666;
        
        public bool RenderBaseElement = false;

        public void Render()
        {
            renderBorder(BaseElement, borderSize, borderTexture);

            if (RenderBaseElement)
            {
                HudRenderer.renderer.elements.Add(new()
                {
                    Position = BaseElement.Position + (borderSize, borderSize),
                    Size = BaseElement.Size - (borderSize * 2, borderSize * 2),
                    TextureId = BaseElement.TextureId,
                    TexCoords = BaseElement.TexCoords,
                    Rotation = BaseElement.Rotation,
                });
            }
        }

        public static void renderBorder(HudElement baseElement, int borderSize, int borderTexture)
        {
            // Left
            HudRenderer.renderer.elements.Add(
                new HudElement
                {
                    Position = baseElement.Position,
                    Size = (borderSize, baseElement.Size.Y),
                    TextureId = borderTexture,

                }
            );
            // Top
            HudRenderer.renderer.elements.Add(
                new HudElement
                {
                    Position = baseElement.Position,
                    Size = (baseElement.Size.X, borderSize),
                    TextureId = borderTexture,
                }
            );
            // Right
            HudRenderer.renderer.elements.Add(
                new HudElement
                {
                    Position = baseElement.Position + (baseElement.Size.X - borderSize, 0),
                    Size = (borderSize, baseElement.Size.Y),
                    TextureId = borderTexture,
                }
            );
            // Bottom
            HudRenderer.renderer.elements.Add(
                new HudElement
                {
                    Position = baseElement.Position + (0, baseElement.Size.Y - borderSize),
                    Size = (baseElement.Size.X, borderSize),
                    TextureId = borderTexture,
                }
            );
        }

        public bool Hovered(out Vector2 position)
        {
            return BaseElement.Hovered(out position);
        }
    }
}