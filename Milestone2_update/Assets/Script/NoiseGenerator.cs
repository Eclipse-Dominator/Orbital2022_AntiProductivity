using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int octaves;
    public float frequency;
    public float lacunarity;
    public float persistence;
    public int numOfPoints;
    public float groundThreshold;
    public float airThreshold;
    public float minLowerbound;
    public float maxUpperbound;
    public float chunkSize;
    public ComputeShader noiseShader;

    public bool changed = true;
    public void GenerateNoise(Vector3Int chunkPos, ComputeBuffer pointsBuffer)
    {
        // generate 1 extra to account for when merging between chunks
        int actualDim = numOfPoints + 1;
        int numThread = Mathf.CeilToInt(actualDim / 8.0f);
        noiseShader.SetBuffer(0, "points", pointsBuffer);

        noiseShader.SetFloat("groundThreshold", groundThreshold);
        noiseShader.SetFloat("airThreshold", airThreshold);
        noiseShader.SetFloat("octaves", octaves);
        noiseShader.SetFloat("frequency", frequency);
        noiseShader.SetFloat("lacunarity", lacunarity);
        noiseShader.SetFloat("persistence", persistence);
        noiseShader.SetFloat("chunkSize", chunkSize);
        
        noiseShader.SetFloat("minLowerbound", minLowerbound);
        noiseShader.SetFloat("maxUpperbound", maxUpperbound);
        noiseShader.SetInt("numOfPt", actualDim);

        noiseShader.SetInts("offset", new int[] { chunkPos.x, chunkPos.y, chunkPos.z });
        //noiseShader.SetVector("offset", new Vector4(chunkPos.x,chunkPos.y,chunkPos.z));
        
        noiseShader.Dispatch(0, numThread, numThread, numThread);
        //pointsBuffer.Release();
    }

    private void OnValidate()
    {
        changed = true;
    }
}
