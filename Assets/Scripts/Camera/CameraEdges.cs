using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEdges : MonoBehaviour {

    private Camera _camera;
    private int currentWidth;
    private int currentHeight;

    private string _globalTextureName = "_GlobalEdgeTex";

    void SetupRT() {

        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.DepthNormals;

        if(_camera.targetTexture != null) {
            RenderTexture temp = _camera.targetTexture;
            _camera.targetTexture = null;
            DestroyImmediate(temp);
        }

        _camera.targetTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 16);
        _camera.targetTexture.filterMode = FilterMode.Bilinear;

        //Shader.SetGlobalTexture(_globalTextureName, _camera.targetTexture);
    }

    private void Update() {
        if(currentHeight != Screen.currentResolution.height || currentWidth != Screen.currentResolution.width) {
            currentHeight = Screen.height;
            currentWidth = Screen.width;
            SetupRT();
        }    

    
    }
}
