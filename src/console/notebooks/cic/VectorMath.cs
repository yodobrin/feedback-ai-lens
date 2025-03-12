public static class VectorMath
{
    // If your embeddings are guaranteed to be length 1536, you can fix that in the code.
    // Or you can remove references to VectorDimension and just use vector.Length.

    public const int VectorDimension = 1536;

    public static float Length(float[] vector)
    {
        float sum = 0;
        for (int i = 0; i < VectorDimension; i++)
        {
            sum += vector[i] * vector[i];
        }
        return (float)Math.Sqrt(sum);
    }

    public static float DotProduct(float[] a, float[] b)
    {
        float sum = 0;
        for (int i = 0; i < VectorDimension; i++)
        {
            sum += a[i] * b[i];
        }
        return sum;
    }

    // Standard Cosine Similarity: dot(a, b) / (|a| * |b|)
    public static float CosineSimilarity(float[] a, float[] b)
    {
        float dot = DotProduct(a, b);
        float magA = Length(a);
        float magB = Length(b);

        // Handle potential divide-by-zero if either vector is all zeros
        if (magA < 1e-8f || magB < 1e-8f) return 0f;

        return dot / (magA * magB);
    }
}
