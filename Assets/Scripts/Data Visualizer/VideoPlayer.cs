using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour {
    private MovieTexture movie;
    private int vSyncPrevious;

	// Use this for initialization
	void Start () {
        movie = ((MovieTexture)GetComponent<Renderer>().material.mainTexture);
        vSyncPrevious = QualitySettings.vSyncCount;
        QualitySettings.vSyncCount = 0;
        movie.loop = true;
        movie.Play();
    }

    void Update()
    {
        if (!movie.isPlaying)
            QualitySettings.vSyncCount = vSyncPrevious;
    }
}
