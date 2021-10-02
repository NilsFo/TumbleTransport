using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://github.com/libgdx/libgdx/blob/master/gdx/src/com/badlogic/gdx/math/Interpolation.java
// Thanks, LibGDX!
// <3

public abstract class Interpolation //: MonoBehaviour
{
    static public float PI = (float)Math.PI;
    static public float PI2 = PI * 2;
    static public float HALF_PI = PI / 2;

    // ###################################################
    public enum InterpolationMethod : UInt32
    {
        Linear,
        Smooth,
        Smooth2,
        Smoother,

        Pow2,
        Pow2In,
        Pow2Out,
        Pow2InInverse,
        Pow2OutInverse,
        Pow3,
        Pow3In,
        Pow3Out,
        Pow4,
        Pow4In,
        Pow4Out,
        Pow5,
        Pow5In,
        Pow5Out,
        Sine,
        SineIn,
        SineOut,

        exp10,
        exp10In,
        exp10Out,
        exp5,
        exp5In,
        exp5Out,

        Circle,
        CircleIn,
        CircleOut,

        Elastic,
        ElasticIn,
        ElasticOut,

        Bounce,
        BounceIn,
        BounceOut,

        Swing,
        SwingIn,
        SwingOut
    }

    public static Interpolation getInterpolation(InterpolationMethod method)
    {
        return new Linear();
        //return null;
    }

    public abstract float apply(float a);

    public float interpolate(float start, float end, float a)
    {
        return start + (end - start) * apply(a);
    }

    // ###################################################

    public class Linear : Interpolation
    {
        public override float apply(float a)
        {
            return a;
        }
    }


    public class Smooth : Interpolation
    {
        public override float apply(float a)
        {
            return a * a * (3f - 2f * a);
        }
    }

    public class Smooth2 : Interpolation
    {
        public override float apply(float a)
        {
            a = a * a * (3f - 2f * a);
            return a * a * (3f - 2f * a);
        }
    }

    public class Smoother : Interpolation
    {
        public override float apply(float a)
        {
            return a * a * a * (a * (a * 6f - 15f) + 10f);
        }
    }

    // ###################################################

    public class Sine : Interpolation
    {
        public override float apply(float a)
        {
            return (float)((1 - Math.Cos(a * PI)) / 2);
        }
    }


    public class SineIn : Interpolation
    {
        public override float apply(float a)
        {
            return (float)(1.0f - Math.Cos(a * HALF_PI));
        }
    }


    public class SineOut : Interpolation
    {
        public override float apply(float a)
        {
            return (float)Math.Sin(a * HALF_PI);
        }
    }

    // ###################################################

    public class Circle : Interpolation
    {
        public override float apply(float a)
        {
            if (a <= 0.5f)
            {
                a *= 2;
                return (1 - (float)Math.Sqrt(1 - a * a)) / 2;
            }
            a--;
            a *= 2;
            return ((float)Math.Sqrt(1 - a * a) + 1) / 2;
        }

    }

    public class CircleIn : Interpolation
    {
        public override float apply(float a)
        {
            return 1 - (float)Math.Sqrt(1 - a * a);
        }
    };

    public class CircleOut : Interpolation
    {
        public override float apply(float a)
        {
            a--;
            return (float)Math.Sqrt(1 - a * a);
        }
    };


    // ###################################################

    public class Pow : Interpolation
    {

        protected int power;
        public Pow(int power)
        {
            this.power = power;
        }

        public override float apply(float a)
        {
            if (a <= 0.5f) return (float)Math.Pow(a * 2, power) / 2;
            return (float)Math.Pow((a - 1) * 2, power) / (power % 2 == 0 ? -2 : 2) + 1;
        }
    }

    public class PowIn : Pow
    {
        public PowIn(int power) : base(power)
        {
        }

        public override float apply(float a)
        {
            return (float)Math.Pow(a, power);
        }
    }

    public class PowOut : Pow
    {
        public PowOut(int power) : base(power)
        {
        }

        public override float apply(float a)
        {
            return (float)Math.Pow(a - 1, power) * (power % 2 == 0 ? -1 : 1) + 1;
        }
    }

    public class Pow2InInverse : Interpolation
    {
        public override float apply(float a)
        {
            return (float)Math.Sqrt(a);
        }
    }

    public class Pow2OutInverse : Interpolation
    {
        public override float apply(float a)
        {
            return 1 - (float)Math.Sqrt(-(a - 1));
        }
    }

    // ###################################################

    public class Exp : Interpolation
    {
        protected float value, power, min, scale;

        public Exp(float value, float power)
        {
            this.value = value;
            this.power = power;
            min = (float)Math.Pow(value, -power);
            scale = 1 / (1 - min);
        }

        public override float apply(float a)
        {
            if (a <= 0.5f) return ((float)Math.Pow(value, power * (a * 2 - 1)) - min) * scale / 2;
            return (2 - ((float)Math.Pow(value, -power * (a * 2 - 1)) - min) * scale) / 2;
        }
    }

    public class ExpIn : Exp
    {
        public ExpIn(float value, float power) : base(value, power)
        {
        }

        public override float apply(float a)
        {
            return ((float)Math.Pow(value, power * (a - 1)) - min) * scale;
        }
    }

    public class ExpOut : Exp
    {
        public ExpOut(float value, float power) : base(value, power)
        {
        }

        public override float apply(float a)
        {
            return 1 - ((float)Math.Pow(value, -power * a) - min) * scale;
        }
    }

    // ###################################################

    public class Elastic : Interpolation
    {
        protected float value, power, scale, bounces;

        public Elastic(float value, float power, int bounces, float scale)
        {
            this.value = value;
            this.power = power;
            this.scale = scale;
            this.bounces = bounces * PI * (bounces % 2 == 0 ? 1 : -1);
        }

        public override float apply(float a)
        {
            if (a <= 0.5f)
            {
                a *= 2;
                return (float)(Math.Pow(value, power * (a - 1)) * Math.Sin(a * bounces) * scale / 2);
            }
            a = 1 - a;
            a *= 2;
            return (float)(1 - Math.Pow(value, power * (a - 1)) * Math.Sin(a * bounces) * scale / 2);
        }
    }

    public class ElasticIn : Elastic
    {
        public ElasticIn(float value, float power, int bounces, float scale) : base(value, power, bounces, scale)
        {

        }

        public override float apply(float a)
        {
            if (a >= 0.99) return 1;
            return (float)(Math.Pow(value, power * (a - 1)) * Math.Sin(a * bounces) * scale);
        }
    }

    public class ElasticOut : Elastic
    {
        public ElasticOut(float value, float power, int bounces, float scale) : base(value, power, bounces, scale)
        {

        }

        public override float apply(float a)
        {
            if (a == 0) return 0;
            a = 1 - a;
            return ((float)(1 - Math.Pow(value, power * (a - 1)) * Math.Sin(a * bounces) * scale));
        }
    }

    // ###################################################

    public class BounceOut : Interpolation
    {
        protected float[] widths, heights;

        public BounceOut(float[] widths, float[] heights)
        {
            if (widths.Length != heights.Length)
                throw new Exception("Must be the same number of widths and heights.");
            this.widths = widths;
            this.heights = heights;
        }

        public BounceOut(int bounces)
        {
            if (bounces < 2 || bounces > 5) throw new Exception("Bounces Cannot be < 2 or > 5. Bounces param: " + bounces);
            widths = new float[bounces];
            heights = new float[bounces];
            heights[0] = 1;

            switch (bounces)
            {
                case 2:
                    widths[0] = 0.6f;
                    widths[1] = 0.4f;
                    heights[1] = 0.33f;
                    break;
                case 3:
                    widths[0] = 0.4f;
                    widths[1] = 0.4f;
                    widths[2] = 0.2f;
                    heights[1] = 0.33f;
                    heights[2] = 0.1f;
                    break;
                case 4:
                    widths[0] = 0.34f;
                    widths[1] = 0.34f;
                    widths[2] = 0.2f;
                    widths[3] = 0.15f;
                    heights[1] = 0.26f;
                    heights[2] = 0.11f;
                    heights[3] = 0.03f;
                    break;
                case 5:
                    widths[0] = 0.3f;
                    widths[1] = 0.3f;
                    widths[2] = 0.2f;
                    widths[3] = 0.1f;
                    widths[4] = 0.1f;
                    heights[1] = 0.45f;
                    heights[2] = 0.3f;
                    heights[3] = 0.15f;
                    heights[4] = 0.06f;
                    break;
            }
            widths[0] *= 2;
        }

        public override float apply(float a)
        {
            if (a == 1) return 1;
            a += widths[0] / 2;
            float width = 0, height = 0;
            for (int i = 0, n = widths.Length; i < n; i++)
            {
                width = widths[i];
                if (a <= width)
                {
                    height = heights[i];
                    break;
                }
                a -= width;
            }
            a /= width;
            float z = 4 / width * height * a;
            return 1 - (z - z * a) * width;
        }
    }

    public class Bounce : BounceOut
    {
        public Bounce(float[] widths, float[] heights) : base(widths, heights)
        {
        }

        public Bounce(int bounces) : base(bounces)
        {
        }

        private float bout(float a)
        {
            float test = a + widths[0] / 2;
            if (test < widths[0]) return test / (widths[0] / 2) - 1;
            return base.apply(a);
        }

        public override float apply(float a)
        {
            if (a <= 0.5f) return (1 - bout(1 - a * 2)) / 2;
            return bout(a * 2 - 1) / 2 + 0.5f;
        }
    }

    public class BounceIn : BounceOut
    {
        public BounceIn(float[] widths, float[] heights) : base(widths, heights)
        {
        }

        public BounceIn(int bounces) : base(bounces)
        {
        }

        public override float apply(float a)
        {
            return 1f - base.apply(a);
        }
    }

    // ###################################################

    public class Swing : Interpolation
    {
        protected float scale;

        public Swing(float scale)
        {
            this.scale = scale * 2;
        }

        public override float apply(float a)
        {
            if (a <= 0.5f)
            {
                a *= 2;
                return a * a * ((scale + 1) * a - scale) / 2;
            }
            a--;
            a *= 2;
            return a * a * ((scale + 1) * a + scale) / 2 + 1;
        }
    }

    public class SwingOut : Interpolation
    {
        protected float scale;

        public SwingOut(float scale)
        {
            this.scale = scale * 2;
        }

        public override float apply(float a)
        {
            a--;
            return a * a * ((scale + 1) * a + scale) + 1;
        }
    }

    public class SwingIn : Interpolation
    {
        protected float scale;

        public SwingIn(float scale)
        {
            this.scale = scale;
        }

        public override float apply(float a)
        {
            return a * a * ((scale + 1) * a - scale);
        }
    }

}
