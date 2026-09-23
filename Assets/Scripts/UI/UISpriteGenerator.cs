using UnityEngine;

public static class UISpriteGenerator
{
    public static Sprite CreateRoundedRect(int width, int height, int radius, Color color)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(0, 0, 0, 0);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = true;

                if (x < radius && y < radius)
                    inside = CornerDistance(x, y, radius, radius, radius);
                else if (x >= width - radius && y < radius)
                    inside = CornerDistance(x, y, width - radius - 1, radius, radius);
                else if (x < radius && y >= height - radius)
                    inside = CornerDistance(x, y, radius, height - radius - 1, radius);
                else if (x >= width - radius && y >= height - radius)
                    inside = CornerDistance(x, y, width - radius - 1, height - radius - 1, radius);

                if (inside)
                {
                    float alpha = 1f;
                    float edgeDist = 1f;

                    if (x < radius && y < radius)
                        edgeDist = CornerDistanceNorm(x, y, radius, radius, radius);
                    else if (x >= width - radius && y < radius)
                        edgeDist = CornerDistanceNorm(x, y, width - radius - 1, radius, radius);
                    else if (x < radius && y >= height - radius)
                        edgeDist = CornerDistanceNorm(x, y, radius, height - radius - 1, radius);
                    else if (x >= width - radius && y >= height - radius)
                        edgeDist = CornerDistanceNorm(x, y, width - radius - 1, height - radius - 1, radius);

                    if (edgeDist < 1f)
                        alpha = Mathf.Clamp01(edgeDist * 2f);

                    pixels[y * width + x] = new Color(color.r, color.g, color.b, color.a * alpha);
                }
                else
                {
                    pixels[y * width + x] = transparent;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private static bool CornerDistance(float x, float y, float cx, float cy, float r)
    {
        float dx = x - cx;
        float dy = y - cy;
        return (dx * dx + dy * dy) <= (r * r);
    }

    private static float CornerDistanceNorm(float x, float y, float cx, float cy, float r)
    {
        float dx = x - cx;
        float dy = y - cy;
        float dist = Mathf.Sqrt(dx * dx + dy * dy);
        return dist / r;
    }

    public static Sprite CreateCircle(int diameter, Color color)
    {
        Texture2D tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(0, 0, 0, 0);
        Color[] pixels = new Color[diameter * diameter];
        float radius = diameter / 2f;
        float center = diameter / 2f;

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                float dx = x - center + 0.5f;
                float dy = y - center + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= radius)
                {
                    float alpha = 1f;
                    if (dist > radius - 1.5f)
                        alpha = Mathf.Clamp01((radius - dist) / 1.5f);

                    pixels[y * diameter + x] = new Color(color.r, color.g, color.b, color.a * alpha);
                }
                else
                {
                    pixels[y * diameter + x] = transparent;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, diameter, diameter), new Vector2(0.5f, 0.5f), 100f);
    }

    public static Sprite CreateGradient(int width, int height, Color top, Color bottom)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            Color c = Color.Lerp(bottom, top, t);
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = c;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    public static Sprite CreateStar(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(0, 0, 0, 0);
        Color white = new Color(1, 1, 1, 1);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size * 0.48f;
        int spikes = 5;
        float outerRadius = radius;
        float innerRadius = radius * 0.42f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x + 0.5f, y + 0.5f);
                if (IsInsideStar(point, center, outerRadius, innerRadius, spikes))
                {
                    pixels[y * size + x] = white;
                }
                else
                {
                    pixels[y * size + x] = transparent;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private static bool IsInsideStar(Vector2 point, Vector2 center, float outerRadius, float innerRadius, int spikes)
    {
        float deltaX = point.x - center.x;
        float deltaY = point.y - center.y;
        float angle = Mathf.Atan2(deltaY, deltaX);
        if (angle < 0) angle += Mathf.PI * 2f;

        float angleStep = Mathf.PI * 2f / (spikes * 2f);
        float spikeIndex = Mathf.Floor(angle / angleStep);
        float angleInSpike = angle - spikeIndex * angleStep;
        float ratioToCenter = 1f - Mathf.Abs(1f - (angleInSpike / angleStep) * 2f);
        float radiusAtAngle = Mathf.Lerp(outerRadius, innerRadius, ratioToCenter);

        float dist = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
        return dist <= radiusAtAngle;
    }

    public static Sprite CreateShadow(int width, int height, int radius, Color color, int spread = 4)
    {
        int size = Mathf.Max(width, height) + spread * 4;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        Color transparent = new Color(0, 0, 0, 0);
        Color[] pixels = new Color[size * size];

        float cx = size / 2f;
        float cy = size / 2f;
        float rx = width / 2f + spread;
        float ry = height / 2f + spread;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - cx + 0.5f) / rx;
                float dy = (y - cy + 0.5f) / ry;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= 1f)
                {
                    float alpha = Mathf.Clamp01((1f - dist) * 3f) * color.a;
                    pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
                else
                {
                    pixels[y * size + x] = transparent;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
