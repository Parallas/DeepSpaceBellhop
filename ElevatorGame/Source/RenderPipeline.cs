using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ElevatorGame.Source;

public static class RenderPipeline
{
    public static void DrawGame(RenderTarget2D renderTarget2D, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, Action[] drawActions)
    {
        GraphicsDevice graphicsDevice = graphics.GraphicsDevice;

        Rectangle bounds = graphicsDevice.PresentationParameters.Bounds;
        int screenWidth = bounds.Width;
        int screenHeight = bounds.Height;
        int rtWidth = renderTarget2D.Width;
        int rtHeight = renderTarget2D.Height;
        
        graphicsDevice.SetRenderTarget(renderTarget2D);
        foreach (Action drawAction in drawActions)
        {
            drawAction?.Invoke();
        }

        int nearestScale = (int)Math.Floor((decimal)screenHeight / rtHeight);
        RenderTarget2D scaledRt = new RenderTarget2D(graphicsDevice, rtWidth * nearestScale, rtHeight * nearestScale);
        graphicsDevice.SetRenderTarget(scaledRt);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        {
            spriteBatch.Draw(renderTarget2D, new Rectangle(0, 0, scaledRt.Width, scaledRt.Height), Color.White);
        }
        spriteBatch.End();
        graphicsDevice.Reset();

        float aspectRatio = (float)rtWidth / (float)rtHeight;
        int newWidth = (int)(screenHeight * aspectRatio);
        int newHeight = screenHeight;
        
        int xOffset = screenWidth / 2 - newWidth / 2;
        int yOffset = screenHeight / 2 - newHeight / 2;
        spriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
        {
            spriteBatch.Draw(scaledRt, new Rectangle(xOffset, yOffset, newWidth, newHeight), Color.White);
        }
        spriteBatch.End();
        
        scaledRt.Dispose();
    }
}