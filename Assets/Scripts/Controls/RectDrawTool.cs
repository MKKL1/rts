using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class RectDrawTool
    {
        public void DrawScreenRectBorder(Rect rect, float thickness, Color color, Texture borderTexture)
        {
            DrawBorderRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color, borderTexture);
            DrawBorderRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color, borderTexture);
            DrawBorderRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color, borderTexture);
            DrawBorderRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color, borderTexture);
        }

        public void DrawBorderRect(Rect rect, Color color, Texture borderTexture)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, borderTexture);
            GUI.color = Color.white;
        }

        public Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            // Calculate corners
            var topLeft = Vector3.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
            // Create Rect
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
    }
}