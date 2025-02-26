using AsepriteDotNet.Aseprite;
using ElevatorGame.Source.Pause;
using Engine;
using Engine.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite;

namespace ElevatorGame.Source;

public class Cursor()
{
    public enum CursorSprites
    {
        Default,
        Dialog,
        FastForward,
        Wait,
        Interact,
        OpenPhone,
        ClosePhone,
        OpenTickets,
        CloseTickets,
    }

    public CursorSprites CursorSprite { get; set; }

    public CursorSprites CursorSpriteOverride { get; set; }

    public Vector2 ViewPosition => RtScreen.ToScreenSpace(
        InputManager.MousePosition.ToVector2(),
        MainGame.RenderBufferSize,
        MainGame.ScreenBounds
    );

    public Vector2 WorldPosition => ViewPosition + MainGame.ScreenPosition;
    public Vector2 TiltOffset => (
        Vector2.Clamp(
            ViewPosition,
            Vector2.Zero,
            MainGame.GameBounds.Size.ToVector2()
        ) - MainGame.GameBounds.Size.ToVector2() / 2f
    ) * (8 / 120f);

    private AsepriteFile _file;
    private AnimatedSprite _sprite;

    public void LoadContent()
    {
        _file = ContentLoader.Load<AsepriteFile>("graphics/Cursor");
        _sprite = _file.CreateSpriteSheet(MainGame.Graphics.GraphicsDevice, false).CreateAnimatedSprite("Tag");

        _sprite.Origin = new(4);
    }

    public void Update()
    {
        CursorSpriteOverride = CursorSprite;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.SetFrame((int)CursorSpriteOverride);

        switch(CursorSpriteOverride)
        {
            case CursorSprites.Default:
            {
                _sprite.SetFrame(0);
                break;
            }
            case CursorSprites.Dialog:
            {
                _sprite.SetFrame(1);
                break;
            }
            case CursorSprites.FastForward:
            {
                _sprite.SetFrame(5);
                break;
            }
            case CursorSprites.Wait:
            {
                _sprite.SetFrame(2);
                break;
            }
            case CursorSprites.Interact:
            {
                _sprite.SetFrame(InputManager.GetDown(MouseButtons.LeftButton) ? 4 : 3);
                break;
            }
            case CursorSprites.OpenPhone:
            {
                _sprite.SetFrame(6);
                break;
            }
            case CursorSprites.ClosePhone:
            {
                _sprite.SetFrame(7);
                break;
            }
            case CursorSprites.OpenTickets:
            {
                _sprite.SetFrame(8);
                break;
            }
            case CursorSprites.CloseTickets:
            {
                _sprite.SetFrame(9);
                break;
            }
        }

        _sprite.Draw(spriteBatch, Vector2.Floor(ViewPosition));
    }
}
