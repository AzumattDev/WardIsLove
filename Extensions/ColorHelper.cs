using System;
using UnityEngine;

namespace WardIsLove.Extensions
{
    public static class ColorHelper
    {
        private const float TOLERANCE = 0.0001f;

        private static readonly string[] colorProperties =
        {
            "_TintColor", "_Color", "_EmissionColor", "_BorderColor", "_ReflectColor", "_RimColor", "_MainColor",
            "_CoreColor", "_FresnelColor", "_CutoutColor"
        };

        public static HSBColor ColorToHSV(Color color)
        {
            HSBColor ret = new(0f, 0f, 0f, color.a);

            float r = color.r;
            float g = color.g;
            float b = color.b;

            float max = Mathf.Max(r, Mathf.Max(g, b));

            if (max <= 0)
                return ret;

            float min = Mathf.Min(r, Mathf.Min(g, b));
            float dif = max - min;

            if (max > min)
            {
                if (Math.Abs(g - max) < TOLERANCE)
                    ret.H = (b - r) / dif * 60f + 120f;
                else if (Math.Abs(b - max) < TOLERANCE)
                    ret.H = (r - g) / dif * 60f + 240f;
                else if (b > g)
                    ret.H = (g - b) / dif * 60f + 360f;
                else
                    ret.H = (g - b) / dif * 60f;
                if (ret.H < 0)
                    ret.H = ret.H + 360f;
            }
            else
            {
                ret.H = 0;
            }

            ret.H *= 1f / 360f;
            ret.S = dif / max * 1f;
            ret.B = max;

            return ret;
        }

        public static Color HSVToColor(HSBColor hsbColor)
        {
            float r = hsbColor.B;
            float g = hsbColor.B;
            float b = hsbColor.B;
            if (!(Math.Abs(hsbColor.S) > TOLERANCE))
                return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.A);
            float max = hsbColor.B;
            float dif = hsbColor.B * hsbColor.S;
            float min = hsbColor.B - dif;

            float h = hsbColor.H * 360f;

            switch (h)
            {
                case < 60f:
                    r = max;
                    g = h * dif / 60f + min;
                    b = min;
                    break;
                case < 120f:
                    r = -(h - 120f) * dif / 60f + min;
                    g = max;
                    b = min;
                    break;
                case < 180f:
                    r = min;
                    g = max;
                    b = (h - 120f) * dif / 60f + min;
                    break;
                case < 240f:
                    r = min;
                    g = -(h - 240f) * dif / 60f + min;
                    b = max;
                    break;
                case < 300f:
                    r = (h - 240f) * dif / 60f + min;
                    g = min;
                    b = max;
                    break;
                case <= 360f:
                    r = max;
                    g = min;
                    b = -(h - 360f) * dif / 60 + min;
                    break;
                default:
                    r = 0;
                    g = 0;
                    b = 0;
                    break;
            }

            return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.A);
        }

        public static Color ConvertRGBColorByHUE(Color rgbColor, float hue)
        {
            float brightness = ColorToHSV(rgbColor).B;
            if (brightness < TOLERANCE)
                brightness = TOLERANCE;
            HSBColor hsv = ColorToHSV(rgbColor / brightness);
            hsv.H = hue;
            Color color = HSVToColor(hsv) * brightness;
            color.a = rgbColor.a;
            return color;
        }

        public static void ChangeObjectColorByHUE(GameObject go, float hue)
        {
            Renderer[]? renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer? rend in renderers)
            {
                Material[] mats;
                if (!Application.isPlaying)
                    mats = rend.sharedMaterials;
                else
                    mats = rend.materials;
                if (mats.Length == 0)
                    continue;
                foreach (string? colorProperty in colorProperties)
                foreach (Material? mat in mats)
                    if (mat != null && mat.HasProperty(colorProperty))
                        _ = setMatHUEColor(mat, colorProperty, hue);
            }

            ParticleSystemRenderer[]? psRenderers = go.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (ParticleSystemRenderer? rend in psRenderers)
            {
                Material? mat = rend.trailMaterial;
                if (mat == null)
                    continue;

                mat = new Material(mat) { name = mat.name + " (Instance)" };
                rend.trailMaterial = mat;
                foreach (string? colorProperty in colorProperties)
                    if (mat != null && mat.HasProperty(colorProperty))
                        _ = setMatHUEColor(mat, colorProperty, hue);
            }

            SkinnedMeshRenderer[]? skinRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer? skinRend in skinRenderers)
            {
                Material[] mats;
                if (!Application.isPlaying)
                    mats = skinRend.sharedMaterials;
                else
                    mats = skinRend.materials;
                if (mats.Length == 0)
                    continue;
                foreach (string? colorProperty in colorProperties)
                foreach (Material? mat in mats)
                    if (mat != null && mat.HasProperty(colorProperty))
                        _ = setMatHUEColor(mat, colorProperty, hue);
            }

            Projector[]? projectors = go.GetComponentsInChildren<Projector>(true);
            foreach (Projector? proj in projectors)
            {
                if (!proj.material.name.EndsWith("(Instance)"))
                    proj.material = new Material(proj.material) { name = proj.material.name + " (Instance)" };
                Material? mat = proj.material;
                if (mat == null)
                    continue;
                foreach (string? colorProperty in colorProperties)
                    if (mat != null && mat.HasProperty(colorProperty))
                        proj.material = setMatHUEColor(mat, colorProperty, hue);
            }

            Light[]? lights = go.GetComponentsInChildren<Light>(true);
            foreach (Light? light in lights)
            {
                HSBColor hsv = ColorToHSV(light.color);
                hsv.H = hue;
                light.color = HSVToColor(hsv);
            }

            ParticleSystem[]? particles = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem? ps in particles)
            {
                ParticleSystem.MainModule main = ps.main;
                HSBColor hsv = ColorToHSV(ps.main.startColor.color);
                hsv.H = hue;
                main.startColor = HSVToColor(hsv);

                ParticleSystem.ColorOverLifetimeModule colorProperty = ps.colorOverLifetime;
                ParticleSystem.MinMaxGradient colorPS = colorProperty.color;
                Gradient? gradient = colorProperty.color.gradient;
                GradientColorKey[]? keys = colorProperty.color.gradient.colorKeys;

                float offsetGradient = 0;
                hsv = ColorToHSV(keys[0].color);
                HSBColor hsv2 = ColorToHSV(keys[1].color);
                offsetGradient = Math.Abs(hsv2.H - hsv.H);
                hsv.H = hue;
                keys[0].color = HSVToColor(hsv);
                for (int i = 1; i < keys.Length; ++i)
                {
                    hsv = ColorToHSV(keys[i].color);
                    hsv.H = Mathf.Repeat(hsv.H + offsetGradient, 1.0f);
                    keys[i].color = HSVToColor(hsv);
                }

                gradient.colorKeys = keys;
                colorPS.gradient = gradient;
                colorProperty.color = colorPS;
            }

            //var shaderColorGradients = go.GetComponentsInChildren<RFX1_ShaderColorGradient>(true);

            //foreach (var shaderColorGradient in shaderColorGradients)
            //{
            //    shaderColorGradient.HUE = hue;
            //}
        }

        private static Material setMatHUEColor(Material mat, string name, float hueColor)
        {
            Color oldColor = mat.GetColor(name);
            Color color = ConvertRGBColorByHUE(oldColor, hueColor);
            mat.SetColor(name, color);
            return mat;
        }

        private static Material setMatAlphaColor(Material mat, string name, float alpha)
        {
            Color oldColor = mat.GetColor(name);
            oldColor.a = alpha;
            mat.SetColor(name, oldColor);
            return mat;
        }

        public struct HSBColor
        {
            public float H;
            public float S;
            public float B;
            public float A;

            public HSBColor(float h, float s, float b, float a)
            {
                H = h;
                S = s;
                B = b;
                A = a;
            }
        }
    }
}