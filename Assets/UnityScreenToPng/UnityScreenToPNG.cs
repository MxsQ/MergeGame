/*
    Author: Matthew M. (Masterio)
    Compatibility: Unity 2021.3.0+, URP
*/

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.Collections;
using System.IO;

namespace USTP
{
    public class UnityScreenToPNG : MonoBehaviour
    {
        #region PROPERTIES
        // input
        public int frameRate = 30;
        public int duration = 1;
        // transition/loop
        public LoopMethod loopTransitionMethod = LoopMethod.Off;
        public float loopTransitionFrames = 0f;
        public float transitionMultiplier = 1.15f;
        // post process
        public PostProcessMethod postProcessMethod = PostProcessMethod.Off;
        public bool rgbTest = false;
        public GameObject actors;                                           // only for PostProcessMethod.OnOutputTexture 
        public Renderer postProcessRenderer;                                // only for PostProcessMethod.OnOutputTexture
        // mask
        public MaskMethod maskMethod = MaskMethod.Off;
        public Renderer maskRenderer;                                       // mask is applied on frame capture end (uses camera black backgound)
        // output
        public string folderPath = "C:\\UnityScreenToPNG";
        public string fileName = "";
        public int digits = 3;
        public bool overrideFolderContent = true;

        // default
        private string _fullPath = "";                      // folder path
        private Camera _camera;                             // camera component
        private int _currentFrame = 0;                      // current frame
        private int _screenWidth;                           // screen width
        private int _screenHeight;                          // screen height
        private Texture2D _alphaTest_Black;                 // tmp texture
        private Texture2D _alphaTest_White;                 // tmp texture
        private Texture2D _alphaTest_Red;                   // tmp texture
        private Texture2D _alphaTest_Green;                 // tmp texture
        private Texture2D _alphaTest_Blue;                  // tmp texture
        private Texture2D _maskTexture;                     // tmp texture
        private int _defaultCaptureFramerate = 0;           // default

        // loop
        private int _totalFrames = 0;
        private int _transitionFrames = 0;
        private int _outputFrames = 0;
        private Texture2D[] _array_a;                       // first half of input frames (transition_a inlcuded)
        private Texture2D[] _array_b;                       // second half of input frames (transition_b inlcuded)

        // output frames
        private Texture2D[] _output;                        // second half of input frames (transition_b inlcuded)

        private CaptureStage _stage = CaptureStage.NotStarted;
        private static bool _started = false;               // used to stop other instances

        public enum LoopMethod
        {
            Off = 0,
            [Tooltip("Transition (Standard): Linear transition. Frames A and B are multiplied (+alpha correction).")]
            Transition_Standard = 1,
            [Tooltip("Transition (Normalized): Normalized transition. Frames A and B are multiplied (+alpha correction).")]
            Transition_Normalized = 2,
            [Tooltip("Transition (Custom): Normalized transition (multiplied by custom value). Frames A and B are multiplied (+alpha correction).")]
            Transition_Custom = 3,
            [Tooltip("Overlap: Draws new frame over old frame.")]
            Overlap = 4
        }

        public enum PostProcessMethod
        {
            Off = 0,                // no post effects
            [Tooltip("Bad for loops (causing glow effect in transition frames).")]
            OnInputTexture = 1,   // bad for transitions ( except advanced transition xD )
            [Tooltip("Good for loops.\n\nRequires an AdditionalRenderer and Actors objects.")]
            OnOutputTexture = 2     // good for transitions // apply post effect after transition is done
        }

        public enum MaskMethod
        {
            Off = 0,                // no mask
            OnInputTexture = 1,
            OnOutputTexture = 2
        }

        private enum CaptureStage
        {
            NotStarted = 0,
            Prepare = 1,
            Recording = 2,
            Finalizing = 3,
            Done = 4
        }
        #endregion

        #region MAIN
        public bool isLoop { get => loopTransitionMethod != LoopMethod.Off && _transitionFrames > 1; }
        public bool cameraPostProcessing { get => _camera.GetUniversalAdditionalCameraData().renderPostProcessing; set => _camera.GetUniversalAdditionalCameraData().renderPostProcessing = value; }

        /// <summary>
        /// Starts recorder.
        /// </summary>
        public void StartRecording()
        {
            if (_stage != CaptureStage.NotStarted)
                return;

            _started = true;

            _stage = CaptureStage.Prepare;

            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            CreateOutputFolder();

            _defaultCaptureFramerate = Time.captureFramerate;
            _camera = gameObject.GetComponentInChildren<Camera>(true);
            Time.captureFramerate = frameRate;

            // correct start settings if needed 
            if (frameRate % 2 > 0)
                frameRate = Mathf.FloorToInt(frameRate + 1);

            _transitionFrames = Mathf.FloorToInt((float)frameRate * loopTransitionFrames);

            if (_transitionFrames < 2)
                _transitionFrames = 0;

            if (_transitionFrames >= frameRate)
                _transitionFrames = frameRate / 2;

            if (_transitionFrames % 2 > 0)
                _transitionFrames = Mathf.FloorToInt(_transitionFrames + 1);

            _totalFrames = frameRate * duration + (isLoop ? _transitionFrames : 0);
            _outputFrames = _totalFrames - _transitionFrames;

            // init output array
            _output = new Texture2D[_totalFrames];

            // init tmp textures (TextureFormat.RGB24 alpha not needed)
            _alphaTest_Black = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _alphaTest_White = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _alphaTest_Red = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _alphaTest_Green = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
            _alphaTest_Blue = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);

            cameraPostProcessing = false;

            if (postProcessRenderer != null)
                postProcessRenderer.gameObject.SetActive(false);

            if (maskRenderer != null)
                maskRenderer.gameObject.SetActive(false);

            if (actors != null)
                actors.SetActive(true);

            StartCoroutine(CaptureFrame());
        }

        private void RenderMaskTexture()
        {
            if (maskMethod != MaskMethod.Off && maskRenderer != null && _maskTexture == null)
            {
                if (actors != null)
                    actors.SetActive(false);

                maskRenderer.gameObject.SetActive(true);

                _maskTexture = new Texture2D(_screenWidth, _screenHeight, TextureFormat.RGB24, false);
                CopyScreenToTexture(ref _maskTexture, Color.black, false);

                maskRenderer.gameObject.SetActive(false);

                if (actors != null)
                    actors.SetActive(true);
            }
        }

        private void LateUpdate()
        {
            if (!_started)
            {
                if (_stage == CaptureStage.NotStarted)
                {
                    if (Input.anyKeyDown)
                    {
                        StartRecording();
                    }
                }
            }
            else if (_stage == CaptureStage.NotStarted)
                gameObject.SetActive(false);
        }

        IEnumerator CaptureFrame()
        {
            yield return new WaitForEndOfFrame();

            if (_stage == CaptureStage.Prepare)
            {
                RenderMaskTexture();
                _stage = CaptureStage.Recording;

                StartCoroutine(CaptureFrame());
            }
            else if (_stage == CaptureStage.Recording)
            {
                if (_currentFrame < _totalFrames)
                {
                    AddOutputFrame(_currentFrame);
                    _currentFrame++;
                    StartCoroutine(CaptureFrame());
                }
                else
                {
                    if (actors != null)
                        actors.SetActive(false);

                    Debug.Log("COMPUTING PLEASE WAIT ...");
                    _stage = CaptureStage.Finalizing;

                    yield return new WaitForSecondsRealtime(0.5f);

                    StartCoroutine(CaptureFrame());
                }
            }
            else if (_stage == CaptureStage.Finalizing)
            {
                StopCoroutine("CaptureFrame");
                OnFinalize();
            }
        }

        void OnGUI()
        {
            if (_stage == CaptureStage.Finalizing)
                GUI.Label(new Rect(_screenWidth / 2 - 40, _screenHeight / 2 - 10, 256, 20), "COMPUTING...");
        }

        private void OnFinalize()
        {
            _stage = CaptureStage.Done;
            enabled = false;

            // try loop
            Output_ConvertToLoop();

            // try apply post effects
            Output_ApplyPostEffects();

            // try apply mask
            Output_ApplyMask();

            // save files
            ExportOutputToFiles();

            // final utilities
            Debug.Log("Complete! " + (isLoop ? _currentFrame - _transitionFrames : _currentFrame) + " video frames rendered." + (isLoop ? " " + _transitionFrames + " transition frames used." : ""));

            Time.captureFramerate = _defaultCaptureFramerate;

            // stop player
            EditorApplication.isPlaying = false;
        }
        #endregion

        #region LOOP TRANSITION
        private void Texture_Transition_Standard(int transitionFrame, ref Texture2D tex_a, ref Texture2D tex_b, ref Texture2D tex_final)
        {
            float transition = Mathf.Clamp01((float)(transitionFrame + 1) / (float)(_transitionFrames + 1));
            Vector2 alpha = (new Vector2(transition, 1f - transition))/*.normalized*/;

            // add to the first x transition frames
            Color color;
            for (int y = 0; y < tex_final.height; ++y)
            {
                for (int x = 0; x < tex_final.width; ++x)
                {
                    // get colors
                    Color A = tex_a.GetPixel(x, y);                               // buffered first x textures
                    Color B = tex_b.GetPixel(x, y);                               // add last x textures

                    // keep oryginal alphas
                    float Aa = A.a;
                    float Ba = B.a;

                    // apply alpha
                    A.a = A.a * alpha.x;
                    B.a = B.a * alpha.y;

                    // transition
                    float a = (1f - A.a) * B.a + A.a;
                    float r = ((1f - A.a) * B.a * B.r + A.a * A.r) / a;
                    float g = ((1f - A.a) * B.a * B.g + A.a * A.g) / a;
                    float b = ((1f - A.a) * B.a * B.b + A.a * A.b) / a;

                    // correct final alpha
                    if (Aa >= Mathf.Epsilon && Ba >= Mathf.Epsilon) // TODO: check if artifacts appears (on flame)
                    {
                        float mean = (Aa * Ba) * (1f + (0.5f - Mathf.Abs(alpha.x - 0.5f)) * 0.2f); // add 0%-20% (max in middle of transition)

                        if (a < mean)
                            a = mean;
                    }

                    color = new Color(r, g, b, a);

                    if (color.a <= Mathf.Epsilon)
                        color = Color.clear;

                    tex_final.SetPixel(x, y, color);
                }
            }
        }

        private void Texture_Transition_Normalized(int transitionFrame, ref Texture2D tex_a, ref Texture2D tex_b, ref Texture2D tex_final)
        {
            float transition = Mathf.Clamp01((float)(transitionFrame + 1) / (float)(_transitionFrames + 1));
            Vector2 alpha = (new Vector2(transition, 1f - transition)).normalized;

            // add to the first x transition frames
            Color color;
            for (int y = 0; y < tex_final.height; ++y)
            {
                for (int x = 0; x < tex_final.width; ++x)
                {
                    // get colors
                    Color A = tex_a.GetPixel(x, y);                               // buffered first x textures
                    Color B = tex_b.GetPixel(x, y);                               // add last x textures

                    // keep oryginal alphas
                    float Aa = A.a;
                    float Ba = B.a;

                    // apply alpha
                    A.a = A.a * alpha.x;
                    B.a = B.a * alpha.y;

                    // transition
                    float a = (1f - A.a) * B.a + A.a;
                    float r = ((1f - A.a) * B.a * B.r + A.a * A.r) / a;
                    float g = ((1f - A.a) * B.a * B.g + A.a * A.g) / a;
                    float b = ((1f - A.a) * B.a * B.b + A.a * A.b) / a;

                    // correct final alpha
                    if (Aa >= Mathf.Epsilon && Ba >= Mathf.Epsilon) // TODO: check if artifacts appears (on flame)
                    {
                        float mean = (Aa * Ba) * (1f + (0.5f - Mathf.Abs(alpha.x - 0.5f)) * 0.2f); // add 0%-20% (max in middle of transition)

                        if (a < mean)
                            a = mean;
                    }

                    color = new Color(r, g, b, a);

                    if (color.a <= Mathf.Epsilon)
                        color = Color.clear;

                    tex_final.SetPixel(x, y, color);
                }
            }
        }

        private void Texture_Transition_Custom(int transitionFrame, ref Texture2D tex_a, ref Texture2D tex_b, ref Texture2D tex_final)
        {
            float transition = Mathf.Clamp01((float)(transitionFrame + 1) / (float)(_transitionFrames));
            Vector2 alpha = (new Vector2(transition, 1f - transition)).normalized * transitionMultiplier;

            // add to the first x transition frames
            Color color;
            for (int y = 0; y < tex_final.height; ++y)
            {
                for (int x = 0; x < tex_final.width; ++x)
                {
                    // get colors
                    Color A = tex_a.GetPixel(x, y);                               // buffered first x textures
                    Color B = tex_b.GetPixel(x, y);                               // add last x textures

                    // apply alpha
                    A.a *= alpha.x;
                    B.a *= alpha.y;

                    if (A.a <= Mathf.Epsilon)
                        A = Color.clear;

                    if (B.a <= Mathf.Epsilon)
                        B = Color.clear;

                    // transition
                    float a = (1f - A.a) * B.a + A.a;
                    float r = ((1f - A.a) * B.a * B.r + A.a * A.r) / a;
                    float g = ((1f - A.a) * B.a * B.g + A.a * A.g) / a;
                    float b = ((1f - A.a) * B.a * B.b + A.a * A.b) / a;

                    color = new Color(r, g, b, a);

                    if (color.a <= Mathf.Epsilon)
                        color = Color.clear;

                    tex_final.SetPixel(x, y, color);
                }
            }
        }

        private void Texture_Overlap(int transitionFrame, ref Texture2D tex_a, ref Texture2D tex_b, ref Texture2D tex_final)
        {
            int half = _transitionFrames / 2;

            Vector2 alpha = new Vector2
            (
                Mathf.Clamp01(((float)transitionFrame) / ((float)half)),
                Mathf.Clamp01(1f - ((float)(transitionFrame - half + 1) / ((float)half)))
            );

            // add to the first x transition frames
            Color color;
            for (int y = 0; y < tex_final.height; y++)
            {
                for (int x = 0; x < tex_final.width; x++)
                {
                    // get colors
                    Color A = tex_a.GetPixel(x, y);                               // buffered first x textures
                    Color B = tex_b.GetPixel(x, y);                               // add last x textures

                    // apply alpha
                    A.a *= alpha.x;
                    B.a *= alpha.y;

                    if (A.a <= Mathf.Epsilon)
                        A = Color.clear;

                    if (B.a <= Mathf.Epsilon)
                        B = Color.clear;

                    // overlay
                    float a = (1f - A.a) * B.a + A.a;
                    float r = ((1f - A.a) * B.a * B.r + A.a * A.r) / a;
                    float g = ((1f - A.a) * B.a * B.g + A.a * A.g) / a;
                    float b = ((1f - A.a) * B.a * B.b + A.a * A.b) / a;

                    color = new Color(r, g, b, a);

                    if (color.a <= Mathf.Epsilon)
                        color = Color.clear;

                    tex_final.SetPixel(x, y, color);
                }
            }
        }
        #endregion

        #region CAPTURE
        private void CopyScreenToTexture(ref Texture2D texture, Color background_color, bool render_post_effects, bool end = false)
        {
            // set black or white background
            _camera.backgroundColor = background_color;

            // turn on/off post effects
            cameraPostProcessing = render_post_effects;

            // inlcude post-effects
            if (render_post_effects || end)
            {
                RenderTexture rt = new RenderTexture(texture.width, texture.height, 24);
                _camera.targetTexture = rt;
                _camera.Render();
                RenderTexture.active = rt; // set it to use the ReadPixels:

                texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                texture.Apply();

                _camera.targetTexture = null;
                RenderTexture.active = null;
                Destroy(rt);
            }
            // exclude post-effects
            else
            {
                _camera.Render();

                texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                texture.Apply();
            }
        }

        private void StartTransparencyTest(bool render_post_effects, bool end = false)
        {
            // rgb background test
            //  post process affects black and white backgrounds (example: bloom glows)
            //  so if object has dark color [ (r || g || b) < 1f ] it is recognized as transparent
            //  additional tests fix that problem
            if (rgbTest && postProcessMethod != PostProcessMethod.Off)
            {
                CopyScreenToTexture(ref _alphaTest_Red, Color.red, false, end);
                CopyScreenToTexture(ref _alphaTest_Green, Color.green, false, end);
                CopyScreenToTexture(ref _alphaTest_Blue, Color.blue, false, end);
            }

            // black/white background test
            CopyScreenToTexture(ref _alphaTest_White, Color.white, render_post_effects, end);
            CopyScreenToTexture(ref _alphaTest_Black, Color.black, render_post_effects, end);
        }

        private Texture2D GetTransparentScreen(bool apply_mask, bool post_effects, bool end)
        {
            // generates colored backgound
            StartTransparencyTest(post_effects, end);

            Color color;
            float alpha;
            bool rgb_test = rgbTest && postProcessMethod != PostProcessMethod.Off;
            Texture2D texture = new Texture2D(_screenWidth, _screenHeight, TextureFormat.ARGB32, false);    // TextureFormat.ARGB32 (keeps alpha channel)

            // each column
            for (int y = 0; y < texture.height; y++)
            {
                // each row
                for (int x = 0; x < texture.width; x++)
                {
                    // alpha
                    Color white = _alphaTest_White.GetPixel(x, y);
                    Color black = _alphaTest_Black.GetPixel(x, y);

                    // retrieve opaque texture color (takes inverted min color as alpha)
                    alpha = 1f - Mathf.Min(white.r - black.r, white.g - black.g, white.b - black.b);

                    // +RGB TEST (**experimental**)
                    if (rgb_test)
                    {
                        // remove transparency from opaque pixels
                        Color red = _alphaTest_Red.GetPixel(x, y);
                        Color green = _alphaTest_Green.GetPixel(x, y);
                        Color blue = _alphaTest_Blue.GetPixel(x, y);

                        // transparent (rgb backgrounds goes through)
                        if (red.r >= Mathf.Epsilon && green.g >= Mathf.Epsilon && blue.b >= Mathf.Epsilon)
                        {
                            /*
                            // exception for grey objects 
                            // TODO:
                            if (
                               red.g > Mathf.Epsilon && red.b > Mathf.Epsilon &&
                               green.r > Mathf.Epsilon && green.b > Mathf.Epsilon &&
                               blue.r > Mathf.Epsilon && blue.g > Mathf.Epsilon
                               )
                            {
                                //float max_r = Mathf.Max(red.g, red.b);
                                //float max_g = Mathf.Max(green.r, green.b);
                                //float max_b = Mathf.Max(blue.r, blue.g);

                                //if (max_r > Mathf.Epsilon && max_g > Mathf.Epsilon && max_b > Mathf.Epsilon)
                                    alpha = 1f;
                            }
                            */
                        }
                        // opaque
                        else
                        {
                            float max = Mathf.Clamp01(Mathf.Max(red.r, green.g, blue.b));

                            // exception for black objects
                            if (max <= Mathf.Epsilon)
                                alpha = 1f;
                            else
                                alpha = Mathf.Clamp01(alpha * (alpha + (1f - max)));
                            //alpha = Mathf.Clamp01(alpha + max * 0.4f);
                        }
                    }

                    // set color
                    if (alpha <= Mathf.Epsilon)
                        color = Color.clear;
                    else
                    {
                        color = black / alpha;
                        color.a = alpha * (apply_mask ? _maskTexture.GetPixel(x, y).r : 1f);
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            // texture with transparent background
            return texture;
        }

        private void ApplyMaskToTexture(Texture2D texture)
        {
            Color color;

            // each column
            for (int y = 0; y < texture.height; y++)
            {
                // each row
                for (int x = 0; x < texture.width; x++)
                {
                    color = texture.GetPixel(x, y);
                    color.a *= _maskTexture.GetPixel(x, y).r;
                    texture.SetPixel(x, y, color);
                }
            }
        }

        private void AddOutputFrame(int frame)
        {
            // mask can be applied on input frame or on end by Output_ApplyMask()
            bool apply_mask = maskMethod == MaskMethod.OnInputTexture && _maskTexture != null;
            bool post_effects = postProcessMethod == PostProcessMethod.OnInputTexture;
            bool end = false;

            // get texture from captured screen
            _output[frame] = GetTransparentScreen(apply_mask, post_effects, end);

            Debug.Log("Frame " + frame.ToString() + " captured.");
        }

        private void CreateTranstitionArrays()
        {
            // 0. SETTINGS:                     frameRate = 30      loopTransitionFrames = 10
            //
            // 1. CAPTURED INPUT FRAMES:        |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|35|36|37|38|39|
            // 
            // 2. COPY TO BUFFER ARRAYS A & B:
            //
            //                     BUFFER_A:    |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|                             |------- transition b --------|
            //                     BUFFER_B:    |------- transition a --------|                             |20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|35|36|37|38|39|
            // 
            // 3. EXPORT TO FILES / OUTPUT ARRAY (LOOPED OUTPUT):
            //
            //                   FINAL ARRAY:   |20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|35|36|37|38|39|    
            //                                                                |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|
            //                                                                |------- overlay A on B ------|

            _array_a = new Texture2D[_totalFrames / 2]; // frames + transition A
            _array_b = new Texture2D[_totalFrames / 2]; // frames + transition B

            int array_a_size = _array_a.Length;
            int array_b_size = _array_b.Length + _array_a.Length;

            for (int frame = 0; frame < _output.Length; frame++)
            {
                // buffer frames
                if (frame < array_a_size)
                {
                    //Debug.Log("Frame " + frame + " captured.");
                    _array_a[frame] = _output[frame];
                }
                else if (frame < array_b_size)
                {
                    //Debug.Log("Frame " + frame + " captured.");
                    _array_b[frame - _array_a.Length] = _output[frame];
                }
                else
                    Debug.LogError("Frame " + frame + " skipped during transition process!");
            }
        }

        private void Output_ConvertToLoop()
        {
            if (!isLoop)
                return;

            CreateTranstitionArrays();

            Texture2D[] final_array = new Texture2D[_outputFrames];

            // current frame
            int frame = 0;

            // first array part
            for (int i = 0; i < _array_b.Length - _transitionFrames; i++)
            {
                final_array[frame] = _array_b[i];
                frame++;
            }

            // transition
            for (int i = 0; i < _transitionFrames; i++)
            {
                // get textures
                Texture2D tex_final = new Texture2D(_screenWidth, _screenHeight, TextureFormat.ARGB32, false);
                Texture2D tex_a = _array_a[i];
                Texture2D tex_b = _array_b[_array_b.Length - _transitionFrames + i];

                // choose transition method
                if (loopTransitionMethod == LoopMethod.Transition_Standard)
                {
                    Texture_Transition_Standard(i, ref tex_a, ref tex_b, ref tex_final);
                }
                else if (loopTransitionMethod == LoopMethod.Transition_Normalized)
                {
                    Texture_Transition_Normalized(i, ref tex_a, ref tex_b, ref tex_final);
                }
                else if (loopTransitionMethod == LoopMethod.Transition_Custom)
                {
                    Texture_Transition_Custom(i, ref tex_a, ref tex_b, ref tex_final);
                }
                else if (loopTransitionMethod == LoopMethod.Overlap)
                {
                    Texture_Overlap(i, ref tex_a, ref tex_b, ref tex_final);
                }
                
                // save result frame
                final_array[frame] = tex_final;
                frame++;
            }

            // last array part
            for (int i = _transitionFrames; i < _array_a.Length; i++)  // save non-transition screens to files
            {
                final_array[frame] = _array_a[i];
                frame++;
            }

            // override old output
            _output = final_array;
        }

        private void Output_ApplyPostEffects()
        {
            // 1. activate post process on camera, activate texture renderer
            // 2. render output textures on camera re-capture with post effects
            // 3. make black/white and fast save to the file

            if (postProcessMethod == PostProcessMethod.OnOutputTexture)
            {
                if (actors != null)
                    actors.SetActive(false);

                if (postProcessRenderer != null)
                    postProcessRenderer.gameObject.SetActive(true);

                cameraPostProcessing = true;

                for (int i = 0; i < _output.Length; i++)
                {
                    if (_output[i] != null)
                    {
                        // activate plane
                        _output[i].Apply();
                        postProcessRenderer.material.mainTexture = _output[i];

                        _output[i] = GetTransparentScreen(maskMethod == MaskMethod.OnOutputTexture && _maskTexture != null, true, true);

                        //Debug.LogError("output[" + i + "] + post-process rendered.");
                    }
                    else
                        Debug.LogError("output[" + i + "] is null!");
                }
            }
        }

        private void Output_ApplyMask()
        {
            if (maskMethod == MaskMethod.OnOutputTexture && _maskTexture != null)
            {
                for (int i = 0; i < _output.Length; i++)
                {
                    if (_output[i] != null)
                    {
                        ApplyMaskToTexture(_output[i]);
                    }
                    else
                        Debug.LogError("output[" + i + "] is null!");
                }
            }
        }

        private void ExportOutputToFiles()
        {
            if (_output != null)
            {
                for (int i = 0; i < _output.Length; i++)
                {
                    SavePng(_output[i], i);
                }
            }
        }

        private void CreateOutputFolder()
        {
            string path = folderPath;
            _fullPath = folderPath;

            if (!overrideFolderContent)
            {
                int count = 1;

                while (System.IO.Directory.Exists(_fullPath))
                {
                    _fullPath = path + " " + count;
                    count++;
                }
            }

            System.IO.Directory.CreateDirectory(_fullPath);
        }

        private void SavePng(Texture2D texture, int frame)
        {
            string name = string.Format("{0}\\{1}{2:D0" + digits + "}.png", _fullPath, fileName, frame);

            if (texture != null)
            {
                var output = texture.EncodeToPNG();
                File.WriteAllBytes(name, output);
            }
            else
                Debug.LogError("output[" + frame + "] is null!");

            Debug.Log("Frame " + frame + " exported to " + name);
        }
        #endregion

        #region EDITOR UI
        // EDITOR UI :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        [CustomEditor(typeof(UnityScreenToPNG))]
        [CanEditMultipleObjects]
        public class UnityScreenToPNGEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                // DrawDefaultInspector();

                //GUI.enabled = false;
                //EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((UnityScreenToPNG)target), typeof(UnityScreenToPNG), false);
                //GUI.enabled = true;

                if (Application.isPlaying)
                    GUI.enabled = false;

                GUILayout.Space(10);

                UnityScreenToPNG component = (UnityScreenToPNG)target;

                // start change check
                EditorGUI.BeginChangeCheck();

                Undo.RecordObject(component, "Change Values");

                GUILayout.Label("CAPTURE", EditorStyles.boldLabel);

                // output details
                GUILayout.BeginVertical("helpbox");
                {
                    int transitionFrames = Mathf.FloorToInt((float)component.frameRate * component.loopTransitionFrames);

                    if (transitionFrames < 2)
                        transitionFrames = 0;

                    if (transitionFrames >= component.frameRate)
                        transitionFrames = component.frameRate / 2;

                    if (transitionFrames % 2 > 0)
                        transitionFrames = Mathf.FloorToInt(transitionFrames + 1);

                    int totalFrames = component.frameRate * component.duration + (component.isLoop ? transitionFrames : 0);
                    //int outputFrames = totalFrames - transitionFrames;

                    GUILayout.Label("Output frames: " + totalFrames + ", loop transition frames: " + transitionFrames);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    GUILayout.BeginVertical("helpbox");
                    {
                        component.frameRate = EditorGUILayout.IntSlider("Frame Rate", component.frameRate, 6, 60);
                        component.duration = EditorGUILayout.IntSlider("Duration", component.duration, 1, 30);
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("helpbox");
                    {
                        component.loopTransitionMethod = (LoopMethod)EditorGUILayout.EnumPopup("Loop", component.loopTransitionMethod);

                        if (component.loopTransitionMethod != LoopMethod.Off)
                        {
                            component.loopTransitionFrames = EditorGUILayout.Slider("Transition Frames", component.loopTransitionFrames, 0f, 0.5f);

                            if (component.loopTransitionFrames > 0f)
                            {
                                if (component.loopTransitionMethod == LoopMethod.Transition_Custom)
                                {
                                    component.transitionMultiplier = (EditorGUILayout.Slider(
                                    new GUIContent("Transition Alpha Boost", "Increase this value if transition alpha is too low in the middle frame."),
                                    component.transitionMultiplier, 0.5f, 1.5f));
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("helpbox");
                    {
                        component.postProcessMethod = (PostProcessMethod)EditorGUILayout.EnumPopup("Post Process", component.postProcessMethod);

                        if (component.postProcessMethod != PostProcessMethod.Off)
                        {
                            component.rgbTest = EditorGUILayout.Toggle("+RGB Alpha Test", component.rgbTest);
                        }

                        if (component.postProcessMethod == PostProcessMethod.OnOutputTexture)
                        {
                            component.postProcessRenderer = (Renderer)EditorGUILayout.ObjectField("Post Process Renderer", component.postProcessRenderer, typeof(Renderer), true);
                            component.actors = (GameObject)EditorGUILayout.ObjectField("Actors", component.actors, typeof(GameObject), true);
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("helpbox");
                    {
                        component.maskMethod = (MaskMethod)EditorGUILayout.EnumPopup("Mask", component.maskMethod);

                        if (component.maskMethod != MaskMethod.Off)
                            component.maskRenderer = (Renderer)EditorGUILayout.ObjectField("Mask Renderer", component.maskRenderer, typeof(Renderer), true);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.Space(20);

                GUILayout.Label("FILES", EditorStyles.boldLabel);
                GUILayout.BeginVertical("helpbox");
                {
                    component.folderPath = EditorGUILayout.TextField("Folder Path", component.folderPath);
                    component.fileName = EditorGUILayout.TextField("File Name", component.fileName);
                    component.digits = EditorGUILayout.IntSlider("Digits", component.digits, 1, 3);
                    component.overrideFolderContent = EditorGUILayout.Toggle("Override Files", component.overrideFolderContent);
                }
                GUILayout.EndVertical();

                // output details
                GUILayout.BeginVertical("helpbox");
                {
                    string path_preview = component.folderPath + "\\" + component.fileName;

                    for (int i = 0; i < component.digits; i++)
                    {
                        path_preview += "0";
                    }

                    GUILayout.Label("First file: " + path_preview + ".png");
                }
                GUILayout.EndVertical();
                
                // end change check
                if (EditorGUI.EndChangeCheck())
                {
                    if (component.frameRate % 2 > 0)
                        component.frameRate++;

                    float mod100 = component.transitionMultiplier % 0.01f;

                    if (mod100 > 0f)
                        component.transitionMultiplier = Mathf.Clamp(component.transitionMultiplier - mod100, 0f, 2f);
                    
                    Undo.RecordObject(component, "Change Values");

                    EditorUtility.SetDirty(component);
                    EditorUtility.SetDirty(component.gameObject);
                }

                GUILayout.Space(20);

                GUILayout.BeginVertical("helpbox");
                {
                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("UNITY SCREEN TO PNG V1.02 (FREEWARE)");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("© 2022 Matthew M. (Masterio)");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                
                        if (GUILayout.Button("Video Tutorial", GUILayout.Width(120)))
                        {
                            Application.OpenURL("https://youtu.be/1akewVYTIgI");
                        }

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Reload", GUILayout.Width(120)))
                        {
                            // Create new undo group
                            Undo.IncrementCurrentGroup();
                            {
                                Undo.RegisterFullObjectHierarchyUndo(component.gameObject, "Reload");

                                Debug.Log("Creating missing assets...");

                                // camera
                                Camera camera = TryCreateCamera(component);

                                // material
                                string material_path = TryCreateMaterial(component);

                                // mask material
                                string material_mask_path = TryCreateMaskMaterial(component);

                                // additional renderer
                                TryCreateAdditionalRenderer(component, camera, material_path);

                                // mask renderer
                                TryCreateMaskRenderer(component, camera, material_mask_path);

                                // actors
                                TryCreateActors(component);

                                Debug.Log("Complete.");
                            }
                            // Name undo group
                            Undo.SetCurrentGroupName("Reload");
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                }
                GUILayout.EndVertical();

                GUI.enabled = true;
            }

            private Camera TryCreateCamera(UnityScreenToPNG component)
            {
                // create game object
                Transform camera_tr = component.transform.Find("Camera");

                // create if not exists
                if (camera_tr == null)
                {
                    GameObject go = new GameObject("Camera");
                    Undo.RegisterCreatedObjectUndo(go, "Create Object");
                    Undo.SetTransformParent(go.transform, component.transform, "Set Parent");
                    go.transform.localPosition = new Vector3(0f, 0f, 0.2f);
                    go.transform.localScale = Vector3.one;
                    go.transform.localRotation = Quaternion.identity;
                    camera_tr = go.transform;

                    Debug.Log("Camera transform created.");
                }
                else
                    Debug.Log("Camera transform already exists.");

                // camera component
                Camera camera = camera_tr.GetComponent<Camera>();

                // create if not exists
                if (camera == null)
                {
                    //camera = camera_tr.gameObject.AddComponent<Camera>();
                    camera = Undo.AddComponent<Camera>(camera_tr.gameObject);
                    Undo.AddComponent<UniversalAdditionalCameraData>(camera_tr.gameObject);

                    // settings
                    camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                    camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                    camera.useOcclusionCulling = false;
                    camera.clearFlags = CameraClearFlags.SolidColor;

                    Debug.Log("Camera component created.");
                }
                else
                    Debug.Log("Camera component already exists.");

                return camera;
            }

            private void TryCreateAdditionalRenderer(UnityScreenToPNG component, Camera camera, string material_path)
            {
                // create game object
                Transform ar_tr = component.postProcessRenderer == null ? camera.transform.Find("PostProcessRenderer") : component.postProcessRenderer.transform;
                GameObject ar_go;

                // create if not exists
                if (ar_tr == null)
                {
                    ar_go = new GameObject("PostProcessRenderer");
                    Undo.RegisterCreatedObjectUndo(ar_go, "Create Object");
                    ar_go.SetActive(false);
                    ar_tr = ar_go.transform;
                    //ar_tr.SetParent(camera.transform);
                    Undo.SetTransformParent(ar_go.transform, camera.transform, "Set Parent");

                    ar_tr.localPosition = new Vector3(0f, 0f, 0.86f);
                    ar_tr.localScale = Vector3.one;
                    ar_tr.localRotation = Quaternion.identity;

                    Debug.Log("PostProcessRenderer transform created.");
                }
                else
                {
                    ar_go = ar_tr.gameObject;
                    Debug.Log("PostProcessRenderer transform already exists.");
                }

                // add Mesh Filter if not exists
                MeshFilter mf = ar_go.GetComponent<MeshFilter>();

                if (mf == null)
                {
                    // template
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    //mf = ar_go.AddComponent<MeshFilter>();
                    mf = Undo.AddComponent<MeshFilter>(ar_go);
                    mf.sharedMesh = quad.GetComponent<MeshFilter>().sharedMesh;

                    // destroy template
                    DestroyImmediate(quad);

                    Debug.Log("PostProcessRenderer: MeshFilter created.");
                }
                else
                    Debug.Log("PostProcessRenderer: MeshFilter already exists.");

                // add Mesh Renderer if not exists
                MeshRenderer mr = ar_go.GetComponent<MeshRenderer>();

                if (mr == null)
                {
                    //mr = ar_go.AddComponent<MeshRenderer>();
                    mr = Undo.AddComponent<MeshRenderer>(ar_go);

                    Debug.Log("PostProcessRenderer: MeshRenderer created.");
                }
                else
                    Debug.Log("PostProcessRenderer: MeshRenderer already exists.");

                // assign missing renderer
                component.postProcessRenderer = mr;

                // assign material
                if (component.postProcessRenderer != null)
                {
                    // turn of some features
                    component.postProcessRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    component.postProcessRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    component.postProcessRenderer.allowOcclusionWhenDynamic = false;
                }

                // apply default material
                if (component.postProcessRenderer != null && component.postProcessRenderer.sharedMaterial == null)
                {
                    //Debug.LogWarning("AdditionalRenderer: MeshRenderer sharedMaterial has been not found.");

                    Material material = (Material)AssetDatabase.LoadAssetAtPath(material_path, typeof(Material));

                    if (material != null)
                    {
                        component.postProcessRenderer.sharedMaterial = material;
                        Debug.Log("PostProcessRenderer: MeshRenderer sharedMaterial assigned from: " + material_path);
                    }
                    else
                        Debug.LogError("PostProcessRenderer: MeshRenderer sharedMaterial not found at: " + material_path);
                }
            }

            private void TryCreateMaskRenderer(UnityScreenToPNG component, Camera camera, string material_path)
            {
                // create game object
                Transform ar_tr = component.maskRenderer == null ? camera.transform.Find("MaskRenderer") : component.maskRenderer.transform;
                GameObject ar_go;

                // create if not exists
                if (ar_tr == null)
                {
                    ar_go = new GameObject("MaskRenderer");
                    Undo.RegisterCreatedObjectUndo(ar_go, "Create Object");
                    ar_go.SetActive(false);
                    ar_tr = ar_go.transform;
                    //ar_tr.SetParent(camera.transform);
                    Undo.SetTransformParent(ar_go.transform, camera.transform, "Set Parent");

                    ar_tr.localPosition = new Vector3(0f, 0f, 10f);
                    ar_tr.localScale = new Vector3(12f, 12f, 12f);
                    ar_tr.localRotation = Quaternion.identity;

                    Debug.Log("MaskRenderer transform created.");
                }
                else
                {
                    ar_go = ar_tr.gameObject;
                    Debug.Log("MaskRenderer transform already exists.");
                }

                // add Mesh Filter if not exists
                MeshFilter mf = ar_go.GetComponent<MeshFilter>();

                if (mf == null)
                {
                    // template
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);

                    //mf = ar_go.AddComponent<MeshFilter>();
                    mf = Undo.AddComponent<MeshFilter>(ar_go);

                    mf.sharedMesh = quad.GetComponent<MeshFilter>().sharedMesh;

                    // destroy template
                    DestroyImmediate(quad);

                    Debug.Log("MaskRenderer: MeshFilter created.");
                }
                else
                    Debug.Log("MaskRenderer: MeshFilter already exists.");

                // add Mesh Renderer if not exists
                MeshRenderer mr = ar_go.GetComponent<MeshRenderer>();

                if (mr == null)
                {
                    //mr = ar_go.AddComponent<MeshRenderer>();
                    mr = Undo.AddComponent<MeshRenderer>(ar_go);
                    Debug.Log("MaskRenderer: MeshRenderer created.");
                }
                else
                    Debug.Log("MaskRenderer: MeshRenderer already exists.");

                // assign missing renderer
                component.maskRenderer = mr;

                // assign material
                if (component.maskRenderer != null)
                {
                    // turn of some features
                    component.maskRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    component.maskRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    component.maskRenderer.allowOcclusionWhenDynamic = false;
                }

                // apply default material
                if (component.maskRenderer != null && component.maskRenderer.sharedMaterial == null)
                {
                    //Debug.LogWarning("MaskRenderer: MeshRenderer sharedMaterial has been not found.");

                    Material material = (Material)AssetDatabase.LoadAssetAtPath(material_path, typeof(Material));

                    if (material != null)
                    {
                        component.maskRenderer.sharedMaterial = material;
                        Debug.Log("MaskRenderer: MeshRenderer sharedMaterial assigned from: " + material_path);
                    }
                    else
                        Debug.LogError("MaskRenderer: MeshRenderer sharedMaterial not found at: " + material_path);
                }
            }

            private void TryCreateActors(UnityScreenToPNG component)
            {
                if (component.actors == null)
                {
                    // create game object
                    Transform actors_tr = component.transform.Find("Actors");

                    // create if not exists
                    if (actors_tr == null)
                    {
                        GameObject actors_go = new GameObject("Actors");
                        Undo.RegisterCreatedObjectUndo(actors_go, "Create Object");
                        actors_tr = actors_go.transform;
                        //actors_tr.SetParent(component.transform);
                        Undo.SetTransformParent(actors_tr.transform, component.transform, "Set Parent");

                        actors_tr.localPosition = Vector3.zero;
                        actors_tr.localScale = Vector3.one;
                        actors_tr.localRotation = Quaternion.identity;

                        Debug.Log("Actors transform created.");
                    }
                    else
                        Debug.Log("Actors transform already exists.");

                    // assign
                    component.actors = actors_tr.gameObject;
                }
            }

            private const string MATERIAL_NAME = "UnityScreenToPNG_PostProcess";
            private const string MATERIAL_MASK_NAME = "UnityScreenToPNG_Mask";
            private const string MATERIAL_SHADER = "Universal Render Pipeline/2D/Sprite-Unlit-Default";     // can be changed to any sprite type

            private string TryCreateMaterial(UnityScreenToPNG component)
            {
                // find script file
                var script = MonoScript.FromMonoBehaviour((UnityScreenToPNG)target);
                string script_path = AssetDatabase.GetAssetPath(script);
                Debug.Log("UnityScreenToPNG.cs found at: " + script_path);

                // get script folder location
                string folder_path = "Assets";
                if (!string.IsNullOrEmpty(script_path))
                    folder_path = script_path.Replace("/" + (typeof(UnityScreenToPNG)).Name + ".cs", "");

                //Debug.Log("Searching " + MATERIAL_NAME + " in: " + folder_path);

                // find material at script location
                string[] guids = AssetDatabase.FindAssets(MATERIAL_NAME + " t:Material", new string[] { folder_path });

                if (guids.Length > 0)
                {
                    Debug.Log("Material " + MATERIAL_NAME + " found.");

                    foreach (string guid in guids)
                    {
                        //Debug.LogError((folder_path + "/" + MATERIAL_NAME + ".mat") + " == " + AssetDatabase.GUIDToAssetPath(guid));
                        // compare asset pathes to find exact material
                        if ((folder_path + "/" + MATERIAL_NAME + ".mat").Equals(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            return AssetDatabase.GUIDToAssetPath(guid);
                        }
                    }
                }

                // create new material
                {
                    Debug.Log("Material " + MATERIAL_NAME + " not found, creating new material...");


                    // Create Material for 'Output Post Effects'
                    Shader shader = Shader.Find(MATERIAL_SHADER);
                    if (shader == null)
                    {
                        shader = Shader.Find("Specular");
                        Debug.LogError("Material Shader" + MATERIAL_SHADER + " not found! Manual shader set required.");
                    }

                    Material material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folder_path + "/" + MATERIAL_NAME + ".mat");

                    // Print the path of the created asset
                    Debug.Log("Material " + MATERIAL_NAME + " created at: " + AssetDatabase.GetAssetPath(material));

                    return AssetDatabase.GetAssetPath(material);
                }
            }

            private string TryCreateMaskMaterial(UnityScreenToPNG component)
            {
                // find script file
                var script = MonoScript.FromMonoBehaviour((UnityScreenToPNG)target);
                string script_path = AssetDatabase.GetAssetPath(script);
                Debug.Log("UnityScreenToPNG.cs found at: " + script_path);

                // get script folder location
                string folder_path = "Assets";
                if (!string.IsNullOrEmpty(script_path))
                    folder_path = script_path.Replace("/" + (typeof(UnityScreenToPNG)).Name + ".cs", "");

                //Debug.Log("Searching " + MATERIAL_NAME + " in: " + folder_path);

                // find material at script location
                string[] guids = AssetDatabase.FindAssets(MATERIAL_MASK_NAME + " t:Material", new string[] { folder_path });

                if (guids.Length > 0)
                {
                    Debug.Log("Material " + MATERIAL_MASK_NAME + " found.");

                    foreach (string guid in guids)
                    {
                        //Debug.LogError((folder_path + "/" + MATERIAL_NAME + ".mat") + " == " + AssetDatabase.GUIDToAssetPath(guid));
                        // compare asset pathes to find exact material
                        if ((folder_path + "/" + MATERIAL_MASK_NAME + ".mat").Equals(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            return AssetDatabase.GUIDToAssetPath(guid);
                        }
                    }
                }

                // create new material
                {
                    Debug.Log("Material " + MATERIAL_MASK_NAME + " not found, creating new material...");


                    // Create Material for 'Output Post Effects'
                    Shader shader = Shader.Find(MATERIAL_SHADER);
                    if (shader == null)
                    {
                        shader = Shader.Find("Specular");
                        Debug.LogError("Material Shader" + MATERIAL_SHADER + " not found! Manual shader set required.");
                    }

                    Material material = new Material(shader);
                    AssetDatabase.CreateAsset(material, folder_path + "/" + MATERIAL_MASK_NAME + ".mat");

                    // Print the path of the created asset
                    Debug.Log("Material " + MATERIAL_NAME + " created at: " + AssetDatabase.GetAssetPath(material));

                    return AssetDatabase.GetAssetPath(material);
                }
            }
        }
        // EDITOR UI :::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        #endregion
    }
}
#endif