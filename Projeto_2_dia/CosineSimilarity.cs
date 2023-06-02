using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_2_dia
{
    public class CosineSimilarity
    {
        public static double CalculateAverageCosineSimilarity(Embeddings embeddings1, Embeddings embeddings2)
        {
            if (embeddings1.Data.Count != embeddings2.Data.Count)
            {
                throw new ArgumentException("Embeddings must have the same number of items");
            }

            double totalSimilarity = 0;

            for (int i = 0; i < embeddings1.Data.Count; i++)
            {
                var similarity = CalculateCosineSimilarity(embeddings1.Data[i].Embedding, embeddings2.Data[i].Embedding);
                totalSimilarity += similarity;
            }

            return totalSimilarity / embeddings1.Data.Count;
        }


        public static double CalculateCosineSimilarity(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
        {
            if (vector1.Count != vector2.Count)
            {
                throw new ArgumentException("Vectors must have the same dimension");
            }

            var dotProduct = DotProduct(vector1, vector2);
            var magnitude1 = Magnitude(vector1);
            var magnitude2 = Magnitude(vector2);

            return dotProduct / (magnitude1 * magnitude2);
        }

        private static double DotProduct(IReadOnlyList<float> vector1, IReadOnlyList<float> vector2)
        {
            return vector1.Zip(vector2, (v1, v2) => v1 * v2).Sum();
        }

        private static double Magnitude(IReadOnlyList<float> vector)
        {
            return Math.Sqrt(vector.Sum(v => v * v));
        }
    }

}

