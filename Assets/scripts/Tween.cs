using System;
using System.Collections.Generic;

public delegate float Easing(float t);

public static class Easings
{
    // CSS-lik cubic-bezier(x1,y1,x2,y2), där x styr tiden (monoton), y är eased värde.
    // x1,x2 bör ligga i [0,1] för garanterad monotonitet.
    public static Easing CubicBezier(float x1, float y1, float x2, float y2)
    {
        // Förberäkna koefficienter för snabbare eval
        float ax = 3 * x1 - 3 * x2 + 1;
        float bx = -6 * x1 + 3 * x2;
        float cx = 3 * x1;

        float ay = 3 * y1 - 3 * y2 + 1;
        float by = -6 * y1 + 3 * y2;
        float cy = 3 * y1;

        float SampleX(float t) => ((ax * t + bx) * t + cx) * t;
        float SampleY(float t) => ((ay * t + by) * t + cy) * t;
        float SampleDX(float t) => (3 * ax * t + 2 * bx) * t + cx;

        // Invertera x(t) ≈ progress -> t via Newton-Raphson, fallback till bisection
        float SolveT(float x, float eps = 1e-6f)
        {
            float t = x; // startgissning
            for (int i = 0; i < 8; i++)
            {
                float xEst = SampleX(t) - x;
                float dx = SampleDX(t);
                if (Math.Abs(xEst) < eps) return t;
                if (Math.Abs(dx) < 1e-6f) break; // undvik div/0
                t -= xEst / dx;
            }
            // Bisection fallback (garanterad konvergens)
            float lo = 0f, hi = 1f; t = x;
            while (hi - lo > eps)
            {
                float xEst = SampleX(t);
                if (xEst < x) lo = t; else hi = t;
                t = 0.5f * (lo + hi);
            }
            return t;
        }

        return (float p) =>
        {
            if (p <= 0f) return 0f;
            if (p >= 1f) return 1f;
            float t = SolveT(p);
            return SampleY(t);
        };
    }

    // Bas
    public static readonly Easing Linear = t => t;

    // Vanliga presets (som CSS)
    public static readonly Easing Ease        = CubicBezier(0.25f, 0.10f, 0.25f, 1.00f);
    public static readonly Easing EaseIn      = CubicBezier(0.42f, 0.00f, 1.00f, 1.00f);
    public static readonly Easing EaseOut     = CubicBezier(0.00f, 0.00f, 0.58f, 1.00f);
    public static readonly Easing EaseInOut   = CubicBezier(0.42f, 0.00f, 0.58f, 1.00f);

    // Några extra kvalitativa kurvor
    public static readonly Easing Smoothstep  = t => t * t * (3 - 2 * t);
    public static readonly Easing Smootherstep= t => t * t * t * (t * (6 * t - 15) + 10);
}

public class Tween
{
    public float From { get; }
    public float To { get; }
    public float Duration { get; }
    public Easing Ease { get; }
    public Action<float> OnUpdate { get; }
    public Action OnComplete { get; }
    public float Elapsed { get; private set; }
    public bool IsFinished { get; private set; }

    public Tween(float from, float to, float duration, Action<float> onUpdate,
                 Action onComplete = null, Easing easing = null)
    {
        From = from;
        To = to;
        Duration = Math.Max(1e-6f, duration);
        OnUpdate = onUpdate;
        OnComplete = onComplete;
        Ease = easing ?? Easings.Linear;
    }

    public void Update(float deltaTime)
    {
        if (IsFinished) return;

        Elapsed += deltaTime;
        float t = Math.Clamp(Elapsed / Duration, 0f, 1f);
        float eased = Ease(t);
        float value = From + (To - From) * eased;
        OnUpdate?.Invoke(value);

        if (t >= 1f)
        {
            IsFinished = true;
            OnComplete?.Invoke();
        }
    }
}

public class TweenManager
{
    private readonly List<Tween> tweens = new();

    public Tween Add(Tween tween)
    {
        tweens.Add(tween);
        return tween;
    }

    public void Update(float deltaTime)
    {
        for (int i = tweens.Count - 1; i >= 0; i--)
        {
            tweens[i].Update(deltaTime);
            if (tweens[i].IsFinished)
                tweens.RemoveAt(i);
        }
    }
}
