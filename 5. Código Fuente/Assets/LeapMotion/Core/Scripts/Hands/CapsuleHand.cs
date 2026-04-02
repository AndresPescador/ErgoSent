/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Attributes;

namespace Leap.Unity {



    /** A basic Leap hand model constructed dynamically vs. using pre-existing geometry*/
    public class CapsuleHand : HandModelBase {

    Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    Vector3 L_W_FrontRightValue;

        private const int TOTAL_JOINT_COUNT = 4 * 5;
    private const float CYLINDER_MESH_RESOLUTION = 0.1f; //in centimeters, meshes within this resolution will be re-used
    private const int THUMB_BASE_INDEX = (int)Finger.FingerType.TYPE_THUMB * 4;
    private const int PINKY_BASE_INDEX = (int)Finger.FingerType.TYPE_PINKY * 4;

    private static int _leftColorIndex = 0;
    private static int _rightColorIndex = 0;
    private static Color[] _leftColorList = { new Color(0.0f, 0.0f, 1.0f), new Color(0.2f, 0.0f, 0.4f), new Color(0.0f, 0.2f, 0.2f) };
    private static Color[] _rightColorList = { new Color(1.0f, 0.0f, 0.0f), new Color(1.0f, 1.0f, 0.0f), new Color(1.0f, 0.5f, 0.0f) };

    [SerializeField]
    private Chirality handedness;

    [SerializeField]
    private bool _showArm = true;

    [SerializeField]
    private bool _castShadows = true;

    [SerializeField]
    private Material _material;

    [SerializeField]
    private Mesh _sphereMesh;

    [MinValue(3)]
    [SerializeField]
    private int _cylinderResolution = 12;

    [MinValue(0)]
    [SerializeField]
    private float _jointRadius = 0.008f;

    [MinValue(0)]
    [SerializeField]
    private float _cylinderRadius = 0.006f;

    [MinValue(0)]
    [SerializeField]
    private float _palmRadius = 0.015f;

    private Material _sphereMat;
    private Hand _hand;
    private Vector3[] _spherePositions;

    public override ModelType HandModelType {
      get {
        return ModelType.Graphics;
      }
    }

    public override Chirality Handedness {
      get {
        return handedness;
      }
      set { }
    }

    public override bool SupportsEditorPersistence() {
      return true;
    }

    public override Hand GetLeapHand() {
      return _hand;
    }

    public override void SetLeapHand(Hand hand) {
      _hand = hand;
    }

    public override void InitHand() {
      if (_material != null) {
        _sphereMat = new Material(_material);
        _sphereMat.hideFlags = HideFlags.DontSaveInEditor;
      }
    }

    private void OnValidate() {
      _meshMap.Clear();
    }

    public override void BeginHand() {
      base.BeginHand();

      if (_hand.IsLeft) {
        _sphereMat.color = _leftColorList[_leftColorIndex];
        _leftColorIndex = (_leftColorIndex + 1) % _leftColorList.Length;
      } else {
        _sphereMat.color = _rightColorList[_rightColorIndex];
        _rightColorIndex = (_rightColorIndex + 1) % _rightColorList.Length;
      }
    }

        public override void UpdateHand()
        {
            if (_spherePositions == null || _spherePositions.Length != TOTAL_JOINT_COUNT)
            {
                _spherePositions = new Vector3[TOTAL_JOINT_COUNT];
            }

            if (_sphereMat == null)
            {
                _sphereMat = new Material(_material);
                _sphereMat.hideFlags = HideFlags.DontSaveInEditor;
            }

            //Update all joint spheres in the fingers
            foreach (var finger in _hand.Fingers)
            {
                for (int j = 0; j < 4; j++)
                {
                    int key = getFingerJointIndex((int)finger.Type, j);

                    if (finger.bones[j].Type.ToString() == "TYPE_METACARPAL")
                    {
                        if (finger.Type.ToString() == "TYPE_THUMB")
                        {
                            L_W_FrontRightValue = finger.bones[j].PrevJoint.ToVector3();
                        }

                    }

                    Vector3 position = finger.Bone((Bone.BoneType)j).NextJoint.ToVector3();
                    _spherePositions[key] = position;

                    drawSphere(position);
                }
            }

            //Now we just have a few more spheres for the hands
            //PalmPos, WristPos, and mockThumbJointPos, which is derived and not taken from the frame obj

            Vector3 palmPosition = _hand.PalmPosition.ToVector3();
            //drawSphere(palmPosition, _palmRadius);
            drawSphere(palmPosition);

            Vector3 thumbBaseToPalm = _spherePositions[THUMB_BASE_INDEX] - _hand.PalmPosition.ToVector3();
            Vector3 mockThumbJointPos = _hand.PalmPosition.ToVector3() + Vector3.Reflect(thumbBaseToPalm, _hand.Basis.xBasis.ToVector3());
            drawSphere(mockThumbJointPos);

            //If we want to show the arm, do the calculations and display the meshes
            if (_showArm)
            {
                var arm = _hand.Arm;

                Vector3 right = arm.Basis.xBasis.ToVector3() * arm.Width * 0.7f * 0.5f;
                Vector3 wrist = arm.WristPosition.ToVector3();
                Vector3 elbow = arm.ElbowPosition.ToVector3();

                float armLength = Vector3.Distance(wrist, elbow);
                wrist -= arm.Direction.ToVector3() * armLength * 0.05f;

                Vector3 armFrontRight = wrist + right;
                Vector3 armFrontLeft = wrist - right;
                Vector3 armBackRight = elbow + right;
                Vector3 armBackLeft = elbow - right;

                drawSphere(armFrontRight);
                drawSphere(armFrontLeft);
                drawSphere(armBackLeft);
                drawSphere(armBackRight);
                drawSphere(arm.NextJoint.ToVector3());


                drawCylinder(armFrontLeft, armFrontRight);
                drawCylinder(armBackLeft, armBackRight);
                drawCylinder(armFrontLeft, armBackLeft);
                drawCylinder(armFrontRight, armBackRight);


                Vector3 palm = _hand.PalmPosition.ToVector3();

                Vector3 HandToWrist = palm - L_W_FrontRightValue;
                Vector3 ArmToWrist = armFrontRight - L_W_FrontRightValue;
                
                ////Calculo de las variables para degradado////

                //Calculo del angulo horizontal
                //Obtener los 3 puntos de análisis
                Vector3 palma2 = _hand.PalmPosition.ToVector3();
                palma2.Set(palma2.x, 0, palma2.z);
                Vector3 muñeca2 = arm.NextJoint.ToVector3();
                muñeca2.Set(muñeca2.x, 0, muñeca2.z);
                Vector3 codo2 = arm.PrevJoint.ToVector3();
                codo2.Set(codo2.x, 0, codo2.z);

                //Restar los vectores
                Vector3 palma_muñeca2 = palma2 - muñeca2;
                Vector3 muñeca_codo2 = muñeca2 - codo2;

                //Hallar el producto punto
                float DotProduct2 = Vector3.Dot(palma_muñeca2, muñeca_codo2);

                //Hallar magnitud de los vectores
                float mag_palma_muñeca2 = Vector3.Magnitude(palma_muñeca2);
                float mag_muñeca_codo2 = Vector3.Magnitude(muñeca_codo2);

                //Obtener el ángulo
                float cosAngle2 = DotProduct2 / (mag_palma_muñeca2 * mag_muñeca_codo2);
                float Angle2 = Mathf.Acos(cosAngle2) * Mathf.Rad2Deg;


                //Calculo del angulo vertical
                //Obtener los 3 puntos de análisis
                Vector3 palma3 = _hand.PalmPosition.ToVector3();
                palma3.Set(0, palma3.y, palma3.z);
                Vector3 muñeca3 = arm.NextJoint.ToVector3();
                muñeca3.Set(0, muñeca3.y, muñeca3.z);
                Vector3 codo3 = arm.PrevJoint.ToVector3();
                codo3.Set(0, codo3.y, codo3.z);

                //Restar los vectores
                Vector3 palma_muñeca3 = palma3 - muñeca3;
                Vector3 muñeca_codo3 = muñeca3 - codo3;

                //Hallar el producto punto
                float DotProduct3 = Vector3.Dot(palma_muñeca3, muñeca_codo3);

                //Hallar magnitud de los vectores
                float mag_palma_muñeca3 = Vector3.Magnitude(palma_muñeca3);
                float mag_muñeca_codo3 = Vector3.Magnitude(muñeca_codo3);

                //Obtener el ángulo
                float cosAngle3 = DotProduct3 / (mag_palma_muñeca3 * mag_muñeca_codo3);
                float Angle3 = Mathf.Acos(cosAngle3) * Mathf.Rad2Deg ; //Aca va un menos diez por algo
                //Debug.Log(cosAngle3);


                float angle = Mathf.Max(Angle2/20, Angle3/30);

                //float angle = Mathf.Abs(140 - Vector3.Angle(HandToWrist, ArmToWrist))/20;

                
                //drawSphere(palmPosition, _palmRadius);

                //drawCylinder(arm.NextJoint.ToVector3(), palmPosition);
                gradient = new Gradient();

                // Populate the color keys at the relative time 0 and 1 (0 and 100%)
                colorKey = new GradientColorKey[2];
                colorKey[0].color = Color.green;
                colorKey[0].time = 0.0f;
                colorKey[1].color = Color.red;
                colorKey[1].time = 1.0f;

                // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
                alphaKey = new GradientAlphaKey[2];
                alphaKey[0].alpha = 1.0f;
                alphaKey[0].time = 0.0f;
                alphaKey[1].alpha = 0.0f;
                alphaKey[1].time = 1.0f;

                gradient.SetKeys(colorKey, alphaKey);

                // What's the color at the relative time 0.25 (25 %) ?
                //Debug.Log(angle);
                _sphereMat.color = gradient.Evaluate(angle);
                //Debug.Log(gradient.Evaluate(angle));


                //Draw cylinders between finger joints
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int keyA = getFingerJointIndex(i, j);
                        int keyB = getFingerJointIndex(i, j + 1);

                        Vector3 posA = _spherePositions[keyA];
                        Vector3 posB = _spherePositions[keyB];

                        drawCylinder(posA, posB);
                    }
                }

                //Draw cylinders between finger knuckles
                for (int i = 0; i < 4; i++)
                {
                    int keyA = getFingerJointIndex(i, 0);
                    int keyB = getFingerJointIndex(i + 1, 0);

                    Vector3 posA = _spherePositions[keyA];
                    Vector3 posB = _spherePositions[keyB];

                    drawCylinder(posA, posB);
                }

                //Draw the rest of the hand
                drawCylinder(mockThumbJointPos, THUMB_BASE_INDEX);
                drawCylinder(mockThumbJointPos, PINKY_BASE_INDEX);
            }
        }

    private void drawSphere(Vector3 position) {
      drawSphere(position, _jointRadius);
    }

    private void drawSphere(Vector3 position, float radius) {
      //multiply radius by 2 because the default unity sphere has a radius of 0.5 meters at scale 1.
      Graphics.DrawMesh(_sphereMesh, 
                        Matrix4x4.TRS(position, 
                                      Quaternion.identity, 
                                      Vector3.one * radius * 2.0f * transform.lossyScale.x), 
                        _sphereMat, 0, 
                        null, 0, null, _castShadows);
    }

    private void drawCylinder(Vector3 a, Vector3 b) {
      float length = (a - b).magnitude;

      Graphics.DrawMesh(getCylinderMesh(length),
                        Matrix4x4.TRS(a, 
                                      Quaternion.LookRotation(b - a), 
                                      new Vector3(transform.lossyScale.x, transform.lossyScale.x, 1)),
                        _material,
                        gameObject.layer, 
                        null, 0, null, _castShadows);
    }

    private void drawCylinder(int a, int b) {
      drawCylinder(_spherePositions[a], _spherePositions[b]);
    }

    private void drawCylinder(Vector3 a, int b) {
      drawCylinder(a, _spherePositions[b]);
    }

    private int getFingerJointIndex(int fingerIndex, int jointIndex) {
      return fingerIndex * 4 + jointIndex;
    }

    private Dictionary<int, Mesh> _meshMap = new Dictionary<int, Mesh>();
    private Mesh getCylinderMesh(float length) {
      int lengthKey = Mathf.RoundToInt(length * 100 / CYLINDER_MESH_RESOLUTION);

      Mesh mesh;
      if (_meshMap.TryGetValue(lengthKey, out mesh)) {
        return mesh;
      }

      mesh = new Mesh();
      mesh.name = "GeneratedCylinder";
      mesh.hideFlags = HideFlags.DontSave;

      List<Vector3> verts = new List<Vector3>();
      List<Color> colors = new List<Color>();
      List<int> tris = new List<int>();

      Vector3 p0 = Vector3.zero;
      Vector3 p1 = Vector3.forward * length;
      for (int i = 0; i < _cylinderResolution; i++) {
        float angle = (Mathf.PI * 2.0f * i) / _cylinderResolution;
        float dx = _cylinderRadius * Mathf.Cos(angle);
        float dy = _cylinderRadius * Mathf.Sin(angle);

        Vector3 spoke = new Vector3(dx, dy, 0);

        verts.Add(p0 + spoke);
        verts.Add(p1 + spoke);

        colors.Add(Color.white);
        colors.Add(Color.white);

        int triStart = verts.Count;
        int triCap = _cylinderResolution * 2;

        tris.Add((triStart + 0) % triCap);
        tris.Add((triStart + 2) % triCap);
        tris.Add((triStart + 1) % triCap);

        tris.Add((triStart + 2) % triCap);
        tris.Add((triStart + 3) % triCap);
        tris.Add((triStart + 1) % triCap);
      }

      mesh.SetVertices(verts);
      mesh.SetIndices(tris.ToArray(), MeshTopology.Triangles, 0);
      mesh.RecalculateBounds();
      mesh.RecalculateNormals();
      mesh.UploadMeshData(true);

      _meshMap[lengthKey] = mesh;

      return mesh;
    }
  }
}
