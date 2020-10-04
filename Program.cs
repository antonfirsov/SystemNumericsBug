using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SystemNumericsBug
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector<float> v = CreateRandomTestVector(42, -5, 5);
            Vector<float> r = FastRound(v);

            Console.WriteLine(v.ToString());
            Console.WriteLine(r.ToString());

            try
            {
                AssertEvenRoundIsCorrect(r, v);
                Console.WriteLine("Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private static Vector<float> CreateRandomTestVector(int seed, float min, float max)
        {
            var data = new float[Vector<float>.Count];
            var rnd = new Random(seed);

            for (int i = 0; i < Vector<float>.Count; i++)
            {
                float v = ((float)rnd.NextDouble() * (max - min)) + min;
                data[i] = v;
            }

            return new Vector<float>(data);
        }
        
        private static void AssertEvenRoundIsCorrect(Vector<float> r, Vector<float> v)
        {
            for (int i = 0; i < Vector<float>.Count; i++)
            {
                int actual = (int)r[i];
                int expected = Re(v[i]);

                if (expected != actual)
                {
                    throw new Exception("Round result incorrect!");
                }
            }
            
            static int Re(float f) => (int)Math.Round(f, MidpointRounding.ToEven);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Vector<float> FastRound(Vector<float> v)
        {
            var magic0 = new Vector<int>(int.MinValue); // 0x80000000
            var sgn0 = Vector.AsVectorSingle(magic0);
            var and0 = Vector.BitwiseAnd(sgn0, v);
            var or0 = Vector.BitwiseOr(and0, new Vector<float>(8388608.0f));
            var add0 = Vector.Add(v, or0);
            return Vector.Subtract(add0, or0);
        }
    }
}